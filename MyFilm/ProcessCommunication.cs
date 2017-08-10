using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ProcessCommunication
    {
        protected enum ProcessCommunicationType
        {
            PIPE,
            SHAREDMEMORY,
            TCP
        }

        protected readonly static ProcessCommunicationType processCommunicateType =
            ProcessCommunicationType.PIPE;
    }
}
