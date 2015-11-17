using System;
using System.IO;
using System.Threading.Tasks;
using WcfClientBase;
using WcfClientTest.ServiceReference1;

namespace WcfClientTest
{
    public class MyServiceClientWrapper : ServiceClientBase<CustomServiceClient>,IService1
    {
        protected override CustomServiceClient InitializeServiceClient()
        {
            //Read DB or file
            var endPoint = File.ReadAllText("cfg.xml");

            string proxyConfigurationPath = CustomChannelFactoryHelper.GetConfigurationFilePath(endPoint, String.Format("{0}.config", Guid.NewGuid().ToString("N")));

            CustomServiceClient service1Client = new CustomServiceClient(proxyConfigurationPath);

            return service1Client;
        }

        public string GetData(int value)
        {
            return PerformServiceOperation(client => client.GetData(value));
        }

        public Task<string> GetDataAsync(int value)
        {
            throw new NotImplementedException();
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            throw new NotImplementedException();
        }

        public Task<CompositeType> GetDataUsingDataContractAsync(CompositeType composite)
        {
            throw new NotImplementedException();
        }
    }
}
