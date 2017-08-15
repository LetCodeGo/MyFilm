using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyFilm
{
    public class ProcessReceiveData : ProcessCommunication
    {
        /// <summary>
        /// 接收数据结束标志，在程序要退出时设置
        /// </summary>
        public static bool receiveExit = false;

        /// <summary>
        /// 窗体隐藏在通知栏时不能收到消息，需要先使窗口正常显示
        /// </summary>
        public static Action ShowSearchResultAction = null;

        public static void ReceiveData(object hWnd)
        {
            switch (processCommunicateType)
            {
                case ProcessCommunicationType.PIPE:
                    ReceiveDataByPipe(hWnd);
                    break;
                case ProcessCommunicationType.SHAREDMEMORY:
                    ReceiveDataBySharedMemory(hWnd);
                    break;
                case ProcessCommunicationType.TCP:
                    ReceiveDataByTcp(hWnd);
                    break;
                default: break;
            }
        }

        private static void ReceiveDataByPipe(object hWnd)
        {
            using (NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(
                    CommonString.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte))
            {
                while (!receiveExit)
                {
                    pipeServer.WaitForConnection();

                    byte[] bytes = new byte[1024];
                    int length = pipeServer.Read(bytes, 0, 1024);
                    CommonString.WebSearchKeyWord = Encoding.Default.GetString(bytes, 0, length);

                    if (!(receiveExit || String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord)))
                    {
                        // 不关心调用结果
                        ShowSearchResultAction?.BeginInvoke(null, null);
                        //Win32API.PostMessage((IntPtr)hWnd, Win32API.WM_SEARCH, 0, 0);
                    }

                    pipeServer.Disconnect();
                }
            }
        }

        private static void ReceiveDataBySharedMemory(object hWnd)
        {
            using (var mmf = MemoryMappedFile.CreateOrOpen(
                CommonString.MemoryMappedName, 1024, MemoryMappedFileAccess.ReadWrite))
            {
                using (var mmViewStream = mmf.CreateViewStream(0, 1024))
                {
                    Semaphore mmfWrite = new Semaphore(1, 1, CommonString.SharedMemorySemaphoreWriteName);
                    Semaphore mmfRead = new Semaphore(0, 1, CommonString.SharedMemorySemaphoreReadName);

                    while (!receiveExit)
                    {
                        mmfRead.WaitOne();

                        mmViewStream.Seek(0, SeekOrigin.Begin);

                        byte[] bytesInt = new byte[4];
                        mmViewStream.Read(bytesInt, 0, 4);
                        int byteLen = BitConverter.ToInt32(bytesInt, 0);

                        byte[] bytes = new byte[byteLen];
                        mmViewStream.Read(bytes, 0, byteLen);
                        CommonString.WebSearchKeyWord = Encoding.Default.GetString(bytes, 0, byteLen);

                        if (!(receiveExit || String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord)))
                        {
                            // 不关心调用结果
                            ShowSearchResultAction?.BeginInvoke(null, null);
                            //Win32API.PostMessage((IntPtr)hWnd, Win32API.WM_SEARCH, 0, 0);
                        }

                        mmfWrite.Release();
                    }
                }
            }
        }

        private static void ReceiveDataByTcp(object hWnd)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, 9321));
            serverSocket.Listen(1);

            while (!receiveExit)
            {
                Socket acceptSocket = serverSocket.Accept();

                byte[] bytes = new byte[1024];
                int length = acceptSocket.Receive(bytes);
                CommonString.WebSearchKeyWord = Encoding.Default.GetString(bytes, 0, length);

                if (!(receiveExit || String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord)))
                {
                    // 不关心调用结果
                    ShowSearchResultAction?.BeginInvoke(null, null);
                    //Win32API.PostMessage((IntPtr)hWnd, Win32API.WM_SEARCH, 0, 0);
                }

                acceptSocket.Close();
            }
        }
    }
}
