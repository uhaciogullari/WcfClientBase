namespace WcfClientBase
{
    public interface IServiceConfiguration
    {
        string EndpointConfigurationName { get; set; }
        string EndpointConfigurationXml { get; set; }
    }
}