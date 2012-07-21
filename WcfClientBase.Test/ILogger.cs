using System;

namespace WcfClientBase.Test
{
    public interface ILogger
    {
        void LogException(Exception ex);
    }
}