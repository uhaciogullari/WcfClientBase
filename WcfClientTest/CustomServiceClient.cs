using WcfClientBase;
using WcfClientTest.ServiceReference1;

namespace WcfClientTest
{

    public class CustomServiceClient : Service1Client
    {

        private string ConfigurationFilePath { get; set; }

        /// <summary>
        /// Initializes a new instance of the CustomServiceClient class.
        /// </summary>
        /// <param name="configurationFilePath">Path to the custom configuration file.</param>
        public CustomServiceClient(string configurationFilePath)
        {
            ConfigurationFilePath = configurationFilePath;
        }

        public CustomServiceClient()
        {
            throw new System.NotImplementedException();
        }


        protected override IService1 CreateChannel()
        {
            ChannelFactory<IService1> factory =
                new ChannelFactory<IService1>(
                    "*", /* or use "*" for the first end point */
                    ConfigurationFilePath);
            return factory.CreateChannel();
        }


    }
}