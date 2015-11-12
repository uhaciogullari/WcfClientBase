using System.ServiceModel;

namespace WcfClientBase
{

    public class CustomChannelFactory<T> : ChannelFactory<T>
    {
        private string configurationFileName;


        public CustomChannelFactory(string endpointConfigurationName, string configurationFileName)
            : base(typeof(T))
        {
            this.configurationFileName = configurationFileName;
            InitializeEndpoint(endpointConfigurationName, null);
        }

        protected override void ApplyConfiguration(string endpointConfigurationName)
        {
            //TODO:
        }
    }
}