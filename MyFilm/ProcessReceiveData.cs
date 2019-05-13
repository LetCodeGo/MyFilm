using Serilog;
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
        /// 窗体隐藏在通知栏时不能收到消息，需要先使窗口正常显示
        /// </summary>
        public static Action ShowSearchResultAction = null;

        public static void ReceiveData(object hWnd)
        {
            Log.Information("ReceiveData by [{Type}]", processCommunicateType.ToString());

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
                while (true)
                {
                    pipeServer.WaitForConnection();

                    byte[] bytesWrite = Encoding.Default.GetBytes(CommonString.MainFormTitle);
                    pipeServer.Write(bytesWrite, 0, bytesWrite.Length);
                    pipeServer.Flush();

                    byte[] bytes = new byte[1024];
                    int length = pipeServer.Read(bytes, 0, 1024);
                    string strTemp = Encoding.Default.GetString(bytes, 0, length);

                    if (strTemp[0] == '0' && strTemp[1] == '1')
                    {
                        CommonString.WebSearchKeyWord = strTemp.Substring(2);
                        ShowSearchResultAction?.BeginInvoke(null, null);
                    }

                    Log.Information("ReceiveData exit[{A}], search{B}",
                        strTemp[0] == '1',
                        (strTemp[0] == '0' && strTemp[1] == '1') ?
                        string.Format("[true], data[{0}]", CommonString.WebSearchKeyWord) :
                        "[false]");

                    pipeServer.Disconnect();

                    if (strTemp[0] == '1') break;
                }
            }
        }

        private static void ReceiveDataBySharedMemory(object hWnd)
        {
            using (var mmf = MemoryMappedFile.CreateOrOpen(
                CommonString.MemoryMappedName, 1024, MemoryMappedFileAccess.ReadWrite))
            {
                using (var viewAccessor = mmf.CreateViewAccessor(0, 1024))
                {
                    Semaphore mmfReceiveWrite = new Semaphore(0, 1,
                        CommonString.SharedMemorySemaphoreReceiveWriteName);
                    Semaphore mmfReceiveRead = new Semaphore(0, 1,
                        CommonString.SharedMemorySemaphoreReceiveReadName);
                    Semaphore mmfSendWrite = new Semaphore(0, 1,
                        CommonString.SharedMemorySemaphoreSendWriteName);
                    Semaphore mmfSendRead = new Semaphore(0, 1,
                        CommonString.SharedMemorySemaphoreSendReadName);

                    while (true)
                    {
                        mmfReceiveWrite.WaitOne();

                        byte[] bytesWrite = Encoding.Default.GetBytes(CommonString.MainFormTitle);

                        viewAccessor.Write(0, bytesWrite.Length);
                        viewAccessor.WriteArray<byte>(4, bytesWrite, 0, bytesWrite.Length);
                        viewAccessor.Flush();

                        mmfSendRead.Release();
                        mmfReceiveRead.WaitOne();

                        byte[] bytes = new byte[1024];
                        int length = viewAccessor.ReadInt32(0);
                        viewAccessor.ReadArray<byte>(4, bytes, 0, length);
                        string strTemp = Encoding.Default.GetString(bytes, 0, length);

                        mmfSendWrite.Release();

                        if (strTemp[0] == '0' && strTemp[1] == '1')
                        {
                            CommonString.WebSearchKeyWord = strTemp.Substring(2);
                            ShowSearchResultAction?.BeginInvoke(null, null);
                        }

                        if (strTemp[0] == '1') break;
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

            while (true)
            {
                Socket acceptSocket = serverSocket.Accept();

                byte[] bytesWrite = Encoding.Default.GetBytes(CommonString.MainFormTitle);
                acceptSocket.Send(bytesWrite);

                byte[] bytes = new byte[1024];
                int length = acceptSocket.Receive(bytes);
                string strTemp = Encoding.Default.GetString(bytes, 0, length);

                if (strTemp[0] == '0' && strTemp[1] == '1')
                {
                    CommonString.WebSearchKeyWord = strTemp.Substring(2);
                    ShowSearchResultAction?.BeginInvoke(null, null);
                }

                acceptSocket.Close();

                if (strTemp[0] == '1') break;
            }
        }
    }
}
