
namespace WcfClientBase
{
    public class ChannelFactory<T> : System.ServiceModel.ChannelFactory<T>
    {
        /// <summary>
        /// Path to the configuration file.
        /// </summary>
        private  string ConfigurationFileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the ChannelFactory class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint in the configuration file.</param>
        /// <param name="configurationFileName">Path to the configuration file.</param>
        public ChannelFactory(string endpointConfigurationName, string configurationFileName)
            : base(typeof(T))
        {
            ConfigurationFileName = configurationFileName;
            InitializeEndpoint(endpointConfigurationName, null);
        }

        /// <summary>
        /// Applies the configuration found in the custom configuration file.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint in the configuration file.</param>
        protected override void ApplyConfiguration(string endpointConfigurationName)
        {
            ChannelFactoryHelper.ApplyConfiguration(ConfigurationFileName, Endpoint, endpointConfigurationName);
        }
    }
}