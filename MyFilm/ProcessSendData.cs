using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ProcessSendData : ProcessCommunication
    {
        public static void SendData(String data)
        {
            if (data.Length > 100) return;

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
            using (NamedPipeClientStream pipeSend =
                new NamedPipeClientStream(
                    "localhost", "myfilmpipe", PipeDirection.InOut,
                    PipeOptions.None, TokenImpersonationLevel.None))
            {
                pipeSend.Connect(300);

                byte[] bytes = Encoding.Default.GetBytes(data);
                pipeSend.Write(bytes, 0, bytes.Length);
                pipeSend.Flush();
            }
        }

        private static void SendDataBySharedMemory(String data)
        {
            using (var mmf = MemoryMappedFile.CreateOrOpen(
                "myfilmMMF", 1024, MemoryMappedFileAccess.ReadWrite))
            {
                using (var viewAccessor = mmf.CreateViewAccessor(0, 1024))
                {
                    Semaphore mmfWrite = Semaphore.OpenExisting("myfilmMMF_WriteMap");
                    Semaphore mmfRead = Semaphore.OpenExisting("myfilmMMF_ReadMap");

                    mmfWrite.WaitOne();

                    byte[] bytes = Encoding.Default.GetBytes(data);
                    // 在起始点写入字符长度，占4各字节
                    viewAccessor.Write(0, bytes.Length);
                    viewAccessor.WriteArray<byte>(4, bytes, 0, bytes.Length);
                    viewAccessor.Flush();

                    mmfRead.Release();
                }
            }
        }

        private static void SendDataByTcp(String data)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(ip, 9321));

            clientSocket.Send(Encoding.Default.GetBytes(data));
            clientSocket.Close();
        }
    }
}
