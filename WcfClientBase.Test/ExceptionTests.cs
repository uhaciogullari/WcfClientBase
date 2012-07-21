using System;
using System.ServiceModel;
using Moq;
using NUnit.Framework;

namespace WcfClientBase.Test
{
    [TestFixture]
    public class ExceptionTests
    {
        private DefaultSampleServiceManager _defaultServiceManager;
        private LoggingSampleServiceManager _loggingServiceManager;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _defaultServiceManager = new DefaultSampleServiceManager();

            _loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            _loggingServiceManager = new LoggingSampleServiceManager(_loggerMock.Object);
        }

        [Test]
        public void ThrowFaultException_ReThrows_DefaultImplementation()
        {
            Assert.Throws(typeof(FaultException), _defaultServiceManager.ThrowFaultException);
        }

        [Test]
        public void ThrowCommunicationException_ReThrows_DefaultImplementation()
        {
            Assert.Throws(typeof(CommunicationException), _defaultServiceManager.ThrowCommunicationException);
        }

        [Test]
        public void ThrowTimeoutException_ReThrows_DefaultImplementation()
        {
            Assert.Throws(typeof(TimeoutException), _defaultServiceManager.ThrowTimeoutException);
        }

        [Test]
        public void ThrowFaultException_LogsFaultException_LoggingImplementation()
        {
            _loggerMock.Setup(logger => logger.LogException(It.Is<FaultException>(exception => true))).Verifiable();

            _loggingServiceManager.ThrowFaultException();
        }

        [Test]
        public void ThrowCommunicationException_LogsCommunicationException_LoggingImplementation()
        {
            _loggerMock.Setup(logger => logger.LogException(It.Is<CommunicationException>(exception => true))).Verifiable();

            _loggingServiceManager.ThrowCommunicationException();
        }

        [Test]
        public void ThrowTimeOutException_LogsTimeOutException_LoggingImplementation()
        {
            _loggerMock.Setup(logger => logger.LogException(It.Is<TimeoutException>(exception => true))).Verifiable();

            _loggingServiceManager.ThrowTimeoutException();
        }
    }
}