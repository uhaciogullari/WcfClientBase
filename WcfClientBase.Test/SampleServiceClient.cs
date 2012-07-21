using System;
using System.ServiceModel;

namespace WcfClientBase.Test
{
    public class SampleServiceClient : ICommunicationObject, ISampleService
    {
        public void Abort()
        {
        }

        public void Close()
        {
        }

        public void Close(TimeSpan timeout)
        {
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
        }

        public void Open(TimeSpan timeout)
        {
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public CommunicationState State
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;
        public void ThrowFaultException()
        {
            throw new FaultException();
        }

        public void ThrowTimeoutException()
        {
            throw new TimeoutException();
        }

        public void ThrowCommunicationException()
        {
            throw new CommunicationException();
        }
    }
}