using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ProcessCommunication
    {
        public enum ProcessCommunicationType
        {
            PIPE,
            SHAREDMEMORY,
            TCP
        }

        // 此值仅登录时修改
        public static ProcessCommunicationType processCommunicateType =
            ProcessCommunicationType.PIPE;
    }
}
