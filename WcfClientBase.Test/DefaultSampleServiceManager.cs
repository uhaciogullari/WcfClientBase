namespace WcfClientBase.Test
{
    public class DefaultSampleServiceManager : ServiceClientBase<SampleServiceClient>, ISampleService
    {
        public void ThrowFaultException()
        {
            PerformServiceOperation(client => client.ThrowFaultException());
        }

        public void ThrowTimeoutException()
        {
            PerformServiceOperation(client => client.ThrowTimeoutException());
        }

        public void ThrowCommunicationException()
        {
            PerformServiceOperation(client => client.ThrowCommunicationException());
        }
    }
}