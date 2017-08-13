using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class Helper
    {
        private readonly static long KB = 1024;
        private readonly static long MB = KB * 1024;
        private readonly static long GB = MB * 1024;
        private readonly static long TB = GB * 1024;
        private readonly static long PB = TB * 1024;

        /// <summary>
        /// 返回文件占用空间大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Int64 CalcSpace(Int64 size)
        {
            return ((Int64)Math.Ceiling(size / 4096.0)) * 4096;
        }

        /// <summary>
        /// 返回用KB、MB、GB、TB表示的大小的字符串，保留两为小数
        /// </summary>
        /// <param name="lSize">字节数</param>
        /// <returns></returns>
        public static String GetSizeString(long lSize)
        {
            if (lSize == 0) return "0KB";
            else if (lSize < KB) return "1KB";

            double dSize = (double)lSize;

            if (lSize < MB) return String.Format("{0:F}KB", dSize / KB);
            else if (lSize < GB) return String.Format("{0:F}MB", dSize / MB);
            else if (lSize < TB) return String.Format("{0:F}GB", dSize / GB);
            else if (lSize < PB) return String.Format("{0:F}TB", dSize / TB);
            else return "N/A";
        }

        /// <summary>
        /// 返回父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String GetUpFolder(String path)
        {
            String upFolder = path.Substring(0, path.LastIndexOf('\\'));
            if (upFolder.EndsWith(":")) upFolder += "\\";
            return upFolder;
        }

        /// <summary>
        /// 用记事本打开内容
        /// </summary>
        /// <param name="strContext"></param>
        public static void OpenEdit(String strContext)
        {
            #region [ 启动记事本 ] 

            System.Diagnostics.Process Proc;

            try
            {
                // 启动记事本 
                Proc = new System.Diagnostics.Process();
                Proc.StartInfo.FileName = "notepad.exe";
                Proc.StartInfo.UseShellExecute = false;
                Proc.StartInfo.RedirectStandardInput = true;
                Proc.StartInfo.RedirectStandardOutput = true;

                Proc.Start();
            }
            catch
            {
                Proc = null;
            }

            #endregion

            #region [ 传递数据给记事本 ] 

            if (Proc != null)
            {
                // 调用 API, 传递数据 
                while (Proc.MainWindowHandle == IntPtr.Zero)
                {
                    Proc.Refresh();
                }

                IntPtr vHandle = Win32API.FindWindowEx(Proc.MainWindowHandle, IntPtr.Zero, "Edit", null);

                // 传递数据给记事本 
                Win32API.SendMessage(vHandle, Win32API.WM_SETTEXT, 0, strContext);
            }
            else
            {
                LogForm form = new LogForm(strContext);
                form.ShowDialog();
            }

            #endregion
        }
    }
}
