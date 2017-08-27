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
