using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ThreadScanDisk
    {
        public delegate void ThreadSacnDiskCallback(bool rst);
        public delegate void ThreadSacnDiskProgressSetView(double pos, string msg);
        public delegate void ThreadSacnDiskProgressFinish();

        /// <summary>
        /// 磁盘路径
        /// </summary>
        private string diskPath = "";
        /// <summary>
        /// 磁盘描述
        /// </summary>
        private string diskDescribe = "";
        /// <summary>
        /// 对mkv等媒体文件是否用mediainfo查看信息
        /// </summary>
        private bool scanMediaInfo = true;
        /// <summary>
        /// 设定的最大扫描深度
        /// </summary>
        private int setMaxScanLayer = int.MaxValue;

        private ThreadSacnDiskCallback threadCallback = null;
        private ThreadSacnDiskProgressSetView progressSetView = null;
        private ThreadSacnDiskProgressFinish progressFinish = null;

        /// <summary>
        /// film_info id
        /// </summary>
        private int startIdGlobal = 0;

        /// <summary>
        /// 实际扫描的磁盘最大深度
        /// </summary>
        private int actualMaxScanLayer = 0;

        /// <summary>
        /// 是否为完全扫描
        /// </summary>
        private bool bCompleteScan = true;

        private int diskScanIndex = 1;
        private int diskScanNum = 1;

        /// <summary>
        /// 查看媒体文件信息
        /// </summary>
        private static MediaInfoLib.MediaInfo mediaInfo = null;
        private static bool mediaInfoInitFlag = false;
        private static string mediaInfoInitErrMsg = "";

        public ThreadScanDisk(string diskPath, string diskDescribe,
            bool scanMediaInfo, int setScanLayer,
            ThreadSacnDiskCallback threadCallback, ThreadSacnDiskProgressSetView progressSetView,
            ThreadSacnDiskProgressFinish progressFinish)
        {
            this.diskPath = diskPath;
            this.diskDescribe = diskDescribe;
            this.scanMediaInfo = scanMediaInfo;
            this.setMaxScanLayer = setScanLayer;
            this.threadCallback = threadCallback;
            this.progressSetView = progressSetView;
            this.progressFinish = progressFinish;

            if (!mediaInfoInitFlag)
            {
                mediaInfo = new MediaInfoLib.MediaInfo(
                ref mediaInfoInitFlag, ref mediaInfoInitErrMsg);
            }
        }

        public static bool MediaInfoState(ref string errMsg)
        {
            if (!mediaInfoInitFlag)
            {
                mediaInfo = new MediaInfoLib.MediaInfo(
                ref mediaInfoInitFlag, ref mediaInfoInitErrMsg);
            }

            errMsg = mediaInfoInitErrMsg;
            return mediaInfoInitFlag;
        }

        public void ScanDisk()
        {
            progressSetView?.Invoke(0, diskPath);

            diskScanNum = CountDisk();

            actualMaxScanLayer = 0;
            bCompleteScan = true;

            int maxId = SqlData.GetInstance().GetMaxIdOfFilmInfo();
            int startId = maxId + 1;
            startIdGlobal = startId;
            diskScanIndex = 1;

            DriveInfo driveInfo = new DriveInfo(diskPath);
            DataTable dt = CommonDataTable.GetFilmInfoDataTable();
            DataRow dr = dt.NewRow();
            dr["id"] = startIdGlobal++;
            dr["name"] = driveInfo.RootDirectory.Name;
            dr["path"] = driveInfo.RootDirectory.FullName;
            dr["size"] = -1;
            dr["create_t"] = driveInfo.RootDirectory.CreationTime;
            dr["modify_t"] = driveInfo.RootDirectory.LastWriteTime;
            dr["is_folder"] = true;
            dr["to_watch"] = false;
            dr["to_watch_ex"] = false;
            dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["to_delete"] = false;
            dr["to_delete_ex"] = false;
            dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["content"] = String.Empty;
            dr["pid"] = -1;
            dr["disk_desc"] = diskDescribe;
            dt.Rows.Add(dr);

            Dictionary<String, int> maxCidDic = new Dictionary<String, int>();
            maxCidDic.Add(driveInfo.RootDirectory.FullName, startIdGlobal - 1);

            ScanAllInFolder(driveInfo.RootDirectory,
                startIdGlobal - 1, setMaxScanLayer, ref dt, ref maxCidDic);

            progressSetView?.Invoke(96, "写入数据库");

            Dictionary<int, long> sizeDic = new Dictionary<int, long>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dt.Rows[i]["is_folder"]))
                {
                    int maxCid = maxCidDic[dt.Rows[i]["path"].ToString()];
                    dt.Rows[i]["max_cid"] = maxCid;

                    if (bCompleteScan)
                    {
                        sizeDic.Add(i, 0);
                        for (int j = i + 1; j + startId <= maxCid; j++)
                        {
                            if (!Convert.ToBoolean(dt.Rows[j]["is_folder"]))
                                sizeDic[i] += Helper.CalcSpace(Convert.ToInt64(dt.Rows[j]["size"]));
                        }
                    }
                }
                else dt.Rows[i]["max_cid"] = dt.Rows[i]["id"];
            }

            // 简略扫描不计算文件夹大小
            if (bCompleteScan)
            {
                foreach (KeyValuePair<int, long> kv in sizeDic)
                    dt.Rows[kv.Key]["size"] = kv.Value;
            }

            //InsertDataToFilmInfo(dt, 0, dt.Rows.Count);
            int maxInsertRows = 1000;
            int insertTimes = dt.Rows.Count / maxInsertRows;
            if (dt.Rows.Count % maxInsertRows != 0) insertTimes += 1;
            for (int i = 0; i < insertTimes; i++)
            {
                SqlData.GetInstance().InsertDataToFilmInfo(dt, i * maxInsertRows, maxInsertRows);
            }

            // 更新磁盘信息
            SqlData.GetInstance().InsertOrUpdateDataToDiskInfo(
                diskDescribe, driveInfo.TotalFreeSpace, driveInfo.TotalSize,
                bCompleteScan, bCompleteScan ? actualMaxScanLayer : setMaxScanLayer);

            progressSetView?.Invoke(100, "完成");
            threadCallback?.Invoke(bCompleteScan);
            progressFinish?.Invoke();
        }

        /// <summary>
        /// 扫描文件夹内容（不包含此文件夹）
        /// </summary>
        /// <param name="directoryInfo">此文件夹信息</param>
        /// <param name="pid">此文件夹数据库id</param>
        /// <param name="setScanLayer">设定的最多扫描层数</param>
        /// <param name="dt">记录要向film_info表中插入的数据</param>
        /// <param name="maxCidDic">记录文件夹下递归的子文件夹或文件的最大id号</param>
        private void ScanAllInFolder(
            DirectoryInfo directoryInfo,
            int pid, int setScanLayer, ref DataTable dt,
            ref Dictionary<String, int> maxCidDic)
        {
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            FileInfo[] fileInfoArray = directoryInfo.GetFiles();

            Array.Sort<DirectoryInfo>(directoryInfoArray, (d1, d2) => d1.Name.CompareTo(d2.Name));
            Array.Sort<FileInfo>(fileInfoArray, (f1, f2) => f1.Name.CompareTo(f2.Name));

            if (setScanLayer <= 0)
            {
                if (directoryInfoArray.Length > 0 || fileInfoArray.Length > 0) bCompleteScan = false;
                return;
            }

            if (directoryInfoArray.Length > 0 || fileInfoArray.Length > 0)
                actualMaxScanLayer = Math.Max(actualMaxScanLayer, setMaxScanLayer - setScanLayer + 1);

            int i = 0;
            foreach (DirectoryInfo childDirectoryInfo in directoryInfoArray)
            {
                i++;
                if ((childDirectoryInfo.Attributes & FileAttributes.System) == FileAttributes.System) continue;

                diskScanIndex++;
                progressSetView?.Invoke(diskScanIndex * 96.0 / diskScanNum, childDirectoryInfo.FullName);

                DataRow dr = dt.NewRow();
                dr["id"] = startIdGlobal++;
                dr["name"] = childDirectoryInfo.Name;
                dr["path"] = childDirectoryInfo.FullName;
                dr["size"] = -1;
                dr["create_t"] = childDirectoryInfo.CreationTime;
                dr["modify_t"] = childDirectoryInfo.LastWriteTime;
                dr["is_folder"] = true;
                dr["to_watch"] = false;
                dr["to_watch_ex"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["to_delete_ex"] = false;
                dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["content"] = String.Empty;
                dr["pid"] = pid;
                dr["disk_desc"] = diskDescribe;
                dt.Rows.Add(dr);

                maxCidDic.Add(childDirectoryInfo.FullName, startIdGlobal - 1);
                if (i == directoryInfoArray.Length)
                {
                    List<String> keyPathArray = new List<String>(maxCidDic.Keys);
                    foreach (String keyPath in keyPathArray)
                    {
                        if (childDirectoryInfo.FullName.Contains(keyPath.TrimEnd('\\') + "\\"))
                            maxCidDic[keyPath] = startIdGlobal - 1;
                    }
                }

                ScanAllInFolder(childDirectoryInfo,
                    startIdGlobal - 1, setScanLayer - 1, ref dt, ref maxCidDic);
            }

            int j = 0;
            bool mediaFlag = false;
            foreach (FileInfo fileInfo in fileInfoArray)
            {
                j++;
                diskScanIndex++;
                progressSetView?.Invoke(diskScanIndex * 96.0 / diskScanNum, fileInfo.FullName);

                DataRow dr = dt.NewRow();
                dr["id"] = startIdGlobal++;
                dr["name"] = fileInfo.Name;
                dr["path"] = fileInfo.FullName;
                dr["size"] = fileInfo.Length;
                dr["create_t"] = fileInfo.CreationTime;
                dr["modify_t"] = fileInfo.LastWriteTime;
                dr["is_folder"] = false;
                dr["to_watch"] = false;
                dr["to_watch_ex"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["to_delete_ex"] = false;
                dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                // 文件夹内只查看第一个媒体信息
                dr["content"] = GetFileContent(fileInfo, ref mediaFlag);
                dr["pid"] = pid;
                dr["disk_desc"] = diskDescribe;
                dt.Rows.Add(dr);

                if (j == fileInfoArray.Length)
                {
                    List<String> keyPathArray = new List<String>(maxCidDic.Keys);
                    foreach (String keyPath in keyPathArray)
                    {
                        if (fileInfo.FullName.Contains(keyPath.TrimEnd('\\') + "\\"))
                            maxCidDic[keyPath] = startIdGlobal - 1;
                    }
                }
            }
        }

        private string GetFileContent(FileInfo fi, ref bool mediaFlag)
        {
            // 只读取 10KB 以下 __game_version_info__.gvi 文件内容
            if (fi.Name.ToLower() == "__game_version_info__.gvi" && fi.Length <= 10240)
                return File.ReadAllText(fi.FullName);
            else if (this.scanMediaInfo && (!mediaFlag) &&
                CommonString.MediaExts.Contains(fi.Extension.ToLower()) &&
                (!fi.Name.ToLower().Contains(".sample.")))
            {
                if (mediaInfoInitFlag)
                {
                    mediaInfo.Open(fi.FullName);
                    string strTemp = mediaInfo.Inform();
                    mediaFlag = true;
                    mediaInfo.Close();

                    return strTemp;
                }
                else return mediaInfoInitErrMsg;
            }
            else return String.Empty;
        }

        private int CountDisk()
        {
            int count = 1;
            CountFolder(diskPath, setMaxScanLayer, ref count);
            return count;
        }

        private void CountFolder(string folderPath, int setScanLayer, ref int count)
        {
            if (setScanLayer <= 0) return;

            count += Directory.GetFiles(folderPath).Length;

            string[] folderArray = Directory.GetDirectories(folderPath);

            foreach (string childFolderPath in folderArray)
            {
                DirectoryInfo di = new DirectoryInfo(childFolderPath);
                if ((di.Attributes & FileAttributes.System) == FileAttributes.System) continue;

                count++;
                CountFolder(childFolderPath, setScanLayer - 1, ref count);
            }
        }
    }
}
