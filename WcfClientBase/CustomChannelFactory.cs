using System.ServiceModel;

namespace WcfClientBase
{

    public class CustomChannelFactory<T> : ChannelFactory<T>
    {
        /// <summary>
        /// Path to the configuration file.
        /// </summary>
        private readonly string _configurationFileName;

        /// <summary>
        /// Initializes a new instance of the CustomChannelFactory class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint in the configuration file.</param>
        /// <param name="configurationFileName">Path to the configuration file.</param>
        public CustomChannelFactory(string endpointConfigurationName, string configurationFileName)
            : base(typeof(T))
        {
            _configurationFileName = configurationFileName;
            InitializeEndpoint(endpointConfigurationName, null);
        }

        /// <summary>
        /// Applies the configuration found in the custom configuration file.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint in the configuration file.</param>
        protected override void ApplyConfiguration(string endpointConfigurationName)
        {
            CustomChannelFactoryHelper.ApplyConfiguration(_configurationFileName, Endpoint, endpointConfigurationName);
        }
    }
}