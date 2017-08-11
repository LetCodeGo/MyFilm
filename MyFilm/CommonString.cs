using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class CommonString
    {
        public static String DataBaseName = "myfilm";
        public static String WebSearchKeyWord = String.Empty;


        public readonly static String PipeName = "myfilm_pipe";
        public readonly static String MemoryMappedName = "myfilm_memory_mapped";

        public readonly static String SharedMemorySemaphoreReadName = "myfilm_SM_SR";
        public readonly static String SharedMemorySemaphoreWriteName = "myfilm_SM_SW";
        public readonly static String AppMutexName = "myfilm_app_mutex";
    }
}
