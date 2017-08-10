using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ProcessSendData
    {
        public static void SendDataByPipe(String data)
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

        public static void SendDataBySharedMemory(String data)
        {
            long capacity = 1 << 10;

            // 创建或者打开共享内存  
            using (var mmf = MemoryMappedFile.CreateOrOpen(
                "myfilmMMF", capacity, MemoryMappedFileAccess.ReadWrite))
            {
                // 通过MemoryMappedFile的CreateViewAccssor方法获得共享内存的访问器
                using (var viewAccessor = mmf.CreateViewAccessor(0, capacity))
                {
                    // 向共享内存开始位置写入字符串的长度
                    viewAccessor.Write(0, data.Length);
                    // 向共享内存4位置写入字符  
                    viewAccessor.WriteArray<char>(4, data.ToArray(), 0, data.Length);
                }
            }
        }
    }
}
