using System.ServiceModel;

namespace WcfClientBase.Test
{
    public class LoggingSampleServiceManager : DefaultSampleServiceManager
    {
        private readonly ILogger _logger;

        public LoggingSampleServiceManager(ILogger logger)
        {
            _logger = logger;
        }

        protected override void HandleCommunicationException(CommunicationException exception)
        {
            _logger.LogException(exception);
        }

        protected override void HandleFaultException(FaultException exception)
        {
            _logger.LogException(exception);
        }

        protected override void HandleTimeoutException(System.TimeoutException exception)
        {
            _logger.LogException(exception);
        }

    }
}