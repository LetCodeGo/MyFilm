using System;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyFilm
{
    public class ProcessSendData : ProcessCommunication
    {
        public static bool exitCall = false;
        private readonly static int sendDataMaxLength = 50;

        public static void SendData(String data)
        {
            switch (processCommunicateType)
            {
                case ProcessCommunicationType.PIPE:
                    SendDataByPipe(data);
                    break;
                case ProcessCommunicationType.SHAREDMEMORY:
                    SendDataBySharedMemory(data);
                    break;
                case ProcessCommunicationType.TCP:
                    SendDataByTcp(data);
                    break;
                default: break;
            }
        }

        private static void SendDataByPipe(String data)
        {
            bool connectFlag = true;
            string connectErrMsg = "";
            string title = "";

            using (NamedPipeClientStream pipeSend =
                new NamedPipeClientStream(
                    "localhost", CommonString.PipeName, PipeDirection.InOut,
                    PipeOptions.None, TokenImpersonationLevel.None))
            {
                try
                {
                    pipeSend.Connect(300);

                    byte[] bytesRead = new byte[1024];
                    int length = pipeSend.Read(bytesRead, 0, 1024);
                    title = Encoding.Default.GetString(bytesRead, 0, length);

                    bool searchFlag = ((!exitCall) && (data.Length <= sendDataMaxLength));
                    string strSend = string.Format("{0}{1}{2}",
                        exitCall ? "1" : "0", searchFlag ? "1" : "0", searchFlag ? data : "");

                    byte[] bytes = Encoding.Default.GetBytes(strSend);
                    pipeSend.Write(bytes, 0, bytes.Length);
                    pipeSend.Flush();
                }
                catch (Exception ex)
                {
                    connectFlag = false;
                    connectErrMsg = ex.Message;
                }
            }

            if (!exitCall)
            {
                if (!connectFlag)
                    MessageBox.Show(string.Format(
                        "搜索 {0} 失败\n{1}！", data, connectErrMsg));
                else if (data.Length > sendDataMaxLength)
                    MessageBox.Show(string.Format(
                        "搜索的字符串\n{0}\n长度为 {1}\n超过 50 ！", data, data.Length), title);
            }
        }

        private static void SendDataBySharedMemory(String data)
        {
            bool connectFlag = true;
            string connectErrMsg = "";
            string title = "";

            using (var mmf = MemoryMappedFile.CreateOrOpen(
                CommonString.MemoryMappedName, 1024, MemoryMappedFileAccess.ReadWrite))
            {
                using (var viewAccessor = mmf.CreateViewAccessor(0, 1024))
                {
                    try
                    {
                        Semaphore mmfReceiveWrite = Semaphore.OpenExisting(
                            CommonString.SharedMemorySemaphoreReceiveWriteName);
                        Semaphore mmfReceiveRead = Semaphore.OpenExisting(
                            CommonString.SharedMemorySemaphoreReceiveReadName);
                        Semaphore mmfSendWrite = Semaphore.OpenExisting(
                            CommonString.SharedMemorySemaphoreSendWriteName);
                        Semaphore mmfSendRead = Semaphore.OpenExisting(
                            CommonString.SharedMemorySemaphoreSendReadName);

                        mmfReceiveWrite.Release();
                        mmfSendRead.WaitOne();

                        byte[] bytesRead = new byte[1024];
                        int length = viewAccessor.ReadInt32(0);
                        viewAccessor.ReadArray<byte>(4, bytesRead, 0, length);
                        title = Encoding.Default.GetString(bytesRead, 0, length);

                        bool searchFlag = ((!exitCall) && (data.Length <= sendDataMaxLength));
                        string strSend = string.Format("{0}{1}{2}",
                            exitCall ? "1" : "0", searchFlag ? "1" : "0", searchFlag ? data : "");

                        byte[] bytes = Encoding.Default.GetBytes(strSend);
                        viewAccessor.Write(0, bytes.Length);
                        viewAccessor.WriteArray<byte>(4, bytes, 0, bytes.Length);

                        mmfReceiveRead.Release();
                        mmfSendWrite.WaitOne();
                    }
                    catch (Exception ex)
                    {
                        connectFlag = false;
                        connectErrMsg = ex.Message;
                    }
                }
            }

            if (!exitCall)
            {
                if (!connectFlag)
                    MessageBox.Show(string.Format(
                        "搜索 {0} 失败\n{1}！", data, connectErrMsg));
                else if (data.Length > sendDataMaxLength)
                    MessageBox.Show(string.Format(
                        "搜索的字符串\n{0}\n长度为 {1}\n超过 50 ！", data, data.Length), title);
            }
        }

        private static void SendDataByTcp(String data)
        {
            string title = "";

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(ip, 9321));

            byte[] bytesRead = new byte[1024];
            int length = clientSocket.Receive(bytesRead);
            title = Encoding.Default.GetString(bytesRead, 0, length);

            bool searchFlag = ((!exitCall) && (data.Length <= sendDataMaxLength));
            string strSend = string.Format("{0}{1}{2}",
                exitCall ? "1" : "0", searchFlag ? "1" : "0", searchFlag ? data : "");

            byte[] bytes = Encoding.Default.GetBytes(strSend);
            clientSocket.Send(bytes);
            clientSocket.Close();

            if (!exitCall)
            {
                if (data.Length > sendDataMaxLength)
                    MessageBox.Show(string.Format(
                        "搜索的字符串\n{0}\n长度为 {1}\n超过 50 ！", data, data.Length), title);
            }
        }
    }
}
