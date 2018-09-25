using System;
using System.Collections.Generic;
using System.IO;

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
            // "C:\\" 的父文件夹为 "\\"
            if (path.TrimEnd(new char[] { '\\' }).EndsWith(":")) return "\\";

            if (path.IndexOf(':') == -1) return path;

            int index = path.LastIndexOf('\\');
            if (index == -1) return path;

            String upFolder = path.Substring(0, index);
            if (upFolder.EndsWith(":")) upFolder += "\\";
            return upFolder;
        }

        /// <summary>
        /// 将 input 中所有在 charInString 的字符替换为 replace
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charInString"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static String Replace(String input, String charInString, Char replace)
        {
            char[] charArray = input.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
                if (charInString.IndexOf(charArray[i]) != -1) charArray[i] = replace;
            return new String(charArray);
        }

        /// <summary>
        /// 数组切片
        /// </summary>
        /// <param name="a"></param>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int[] ArraySlice(int[] a, int startIndex, int len)
        {
            int[] r = new int[Math.Min(a.Length - startIndex, len)];
            for (int i = startIndex, n = 0; i < a.Length && n < len; i++, n++) r[n] = a[i];
            return r;
        }

        /// <summary>
        /// 用记事本打开文件路径或内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="strContext"></param>
        public static void OpenEdit(String filePath, String strContext)
        {
            #region 启动 notepad++

            System.Diagnostics.Process ProcNotePad = null;

            List<String> programFolderList = new List<String>();
            programFolderList.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            if (Environment.Is64BitOperatingSystem)
                programFolderList.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            foreach (String programFolder in programFolderList)
            {
                if (String.IsNullOrWhiteSpace(programFolder)) continue;

                String notePadPath = Path.Combine(programFolder, "Notepad++", "notepad++.exe");

                if (File.Exists(notePadPath))
                {
                    try
                    {
                        ProcNotePad = new System.Diagnostics.Process();
                        ProcNotePad.StartInfo.FileName = notePadPath;
                        ProcNotePad.StartInfo.Arguments = filePath;
                        ProcNotePad.StartInfo.UseShellExecute = true;
                        ProcNotePad.StartInfo.RedirectStandardInput = false;
                        ProcNotePad.StartInfo.RedirectStandardOutput = false;

                        ProcNotePad.Start();
                        return;
                    }
                    catch
                    {
                        ProcNotePad = null;
                    }
                }
            }
            #endregion

            if (ProcNotePad == null)
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
}
