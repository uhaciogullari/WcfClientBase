namespace WcfClientBase.Test
{
    public interface ISampleService
    {
        void ThrowFaultException();
        void ThrowTimeoutException();
        void ThrowCommunicationException();
    }
}