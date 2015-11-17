using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WcfClientTest
{
    [TestClass]
    public class ClientCreateTest
    {
        [TestMethod]
        public void ServiceClient_Should_Not_Be_Null()
        {
            
            MyServiceClientWrapper myServiceClientWrapper = new MyServiceClientWrapper();

            string actual = myServiceClientWrapper.GetData(1);

            Assert.IsNotNull(actual);

        }
    }
}
