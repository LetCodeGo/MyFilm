using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class CommonString
    {
        public static String DbIP = "127.0.0.1";
        public static String DbUserName = string.Empty;
        public static String DbPassword = string.Empty;
        public static String DbName = "myfilm";

        public static String WebSearchKeyWord = String.Empty;

        public readonly static String PipeName = "myfilm_pipe";
        public readonly static String MemoryMappedName = "myfilm_memory_mapped";

        public readonly static String SharedMemorySemaphoreReadName = "myfilm_SM_SR";
        public readonly static String SharedMemorySemaphoreWriteName = "myfilm_SM_SW";
        public readonly static String AppMutexName = "myfilm_app_mutex";
    }
}
