using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Threading;

namespace WcfClientBase
{
    //http://blogs.u2u.be/diederik/post/2010/07/29/Get-your-WCF-client-configuration-from-anywhere.aspx
    public static class ChannelFactoryHelper
    {
        /// <summary>
        /// Lock on the file.
        /// </summary>
        private static readonly ReaderWriterLock ReaderWriterLock = new ReaderWriterLock();

        /// <summary>
        /// Service model sections.
        /// </summary>
        private static readonly Dictionary<string, ServiceModelSectionGroup> Groups = new Dictionary<string, ServiceModelSectionGroup>();

        /// <summary>
        /// 
        /// </summary>
        public static void ApplyConfiguration(string configurationFileName, ServiceEndpoint serviceEndpoint, string configurationName)
        {
            if (string.IsNullOrEmpty(configurationFileName))
            {
                return;
            }

            ServiceModelSectionGroup serviceModeGroup = GetGroup(configurationFileName);
            LoadChannelBehaviors(serviceEndpoint, configurationName, serviceModeGroup);
        }



        //Create a file and delete if client created.
        public static string GetConfigurationFilePath(string configuration, string fileName)
        {
            // Create a file in isolated storage. 
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, store);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(configuration);
            writer.Close();
            stream.Close();

            // Retrieve the actual path of the file (using reflection). 
            FieldInfo fieldInfo = stream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                string proxyConfigurationPath = fieldInfo.GetValue(stream).ToString();

                return proxyConfigurationPath;
            }
            return null;
        }

        /// <summary>
        /// Load the endpoint with the binding, address, behavior etc. from the alternative config file 
        /// </summary>
        public static ServiceEndpoint LoadChannelBehaviors(ServiceEndpoint serviceEndpoint, string configurationName, ServiceModelSectionGroup serviceModeGroup)
        {
            bool isWildcard = string.Equals(configurationName, "*", StringComparison.Ordinal);
            ChannelEndpointElement provider = LookupChannel(serviceModeGroup, configurationName, serviceEndpoint.Contract.ConfigurationName, isWildcard);

            if (provider == null)
            {
                return null;
            }

            if (serviceEndpoint.Binding == null)
            {
                serviceEndpoint.Binding = LookupBinding(serviceModeGroup, provider.Binding, provider.BindingConfiguration);
            }

            if (serviceEndpoint.Address == null)
            {
                serviceEndpoint.Address = new EndpointAddress(provider.Address, LookupIdentity(provider.Identity), provider.Headers.Headers);
            }

            if (serviceEndpoint.Behaviors.Count == 0 && !String.IsNullOrEmpty(provider.BehaviorConfiguration))
            {
                LoadBehaviors(serviceModeGroup, provider.BehaviorConfiguration, serviceEndpoint);
            }

            serviceEndpoint.Name = provider.Contract;

            return serviceEndpoint;
        }

        /// <summary>
        /// Load the ServiceModel section from the config file
        /// </summary>
        private static ServiceModelSectionGroup GetGroup(string configurationFileName)
        {
            ServiceModelSectionGroup group;

            // Get a read lock while we access the cache collection
            ReaderWriterLock.AcquireReaderLock(-1);
            try
            {
                // Check to see if we already have a group for the given configuration
                if (Groups.TryGetValue(configurationFileName, out group))
                {
                    // We found group so return it and we are done
                    return group;
                }
            }
            finally
            {
                // always release the lock safely
                ReaderWriterLock.ReleaseReaderLock();
            }

            // if we get here, we couldn't get a group so we need to create one
            // this will involve modifying the collection so we need a write lock
            ReaderWriterLock.AcquireWriterLock(-1);
            try
            {
                // check an open group wasn't created on another thread while we were
                // acquiring the writer lock
                if (Groups.TryGetValue(configurationFileName, out group))
                {
                    // we found a group so return it and we are done
                    return group;
                }

                ExeConfigurationFileMap executionFileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configurationFileName
                };

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
                group = ServiceModelSectionGroup.GetSectionGroup(config);

                // store it in the cache
                Groups.Add(configurationFileName, group);

                return group;
            }
            finally
            {
                // always release the writer lock!
                ReaderWriterLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Find the endpoint in the alternative config file that matches the required contract and configuration
        /// </summary>
        private static ChannelEndpointElement LookupChannel(ServiceModelSectionGroup serviceModeGroup, string configurationName, string contractName, bool wildcard)
        {

            try
            {
                ChannelEndpointElement channelEndpointElement = serviceModeGroup.Client.Endpoints.Cast<ChannelEndpointElement>()
               .FirstOrDefault(endpoint => endpoint.Contract == contractName
                   && (endpoint.Name == configurationName || wildcard));

                return channelEndpointElement;
            }
            catch (Exception)
            {

                throw new InvalidOperationException("ChannelEndpoint Not Found or Missing ");
            }
        }

        /// <summary>
        /// Configures the binding for the selected endpoint
        /// </summary>
        private static Binding LookupBinding(ServiceModelSectionGroup group, string bindingName, string configurationName)
        {
            BindingCollectionElement bindingElementCollection = group.Bindings[bindingName];
            if (bindingElementCollection.ConfiguredBindings.Count == 0)
            {
                return null;
            }

            IBindingConfigurationElement bindingConfigurationElement = bindingElementCollection.ConfiguredBindings.First(item => item.Name == configurationName);

            Binding binding = GetBinding(bindingConfigurationElement);
            if (bindingConfigurationElement != null)
            {
                bindingConfigurationElement.ApplyConfiguration(binding);
            }

            return binding;
        }

        /// <summary>
        /// Gets the endpoint identity from the configuration file
        /// </summary>
        private static EndpointIdentity LookupIdentity(IdentityElement element)
        {
            EndpointIdentity identity = null;
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            PropertyInformation userPrincipalName = properties["userPrincipalName"];

            if (userPrincipalName != null && userPrincipalName.ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }

            PropertyInformation servicePrincipalName = properties["servicePrincipalName"];

            if (servicePrincipalName != null && servicePrincipalName.ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }

            PropertyInformation dns = properties["dns"];

            if (dns != null && dns.ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }

            PropertyInformation rsa = properties["rsa"];

            if (rsa != null && rsa.ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }

            PropertyInformation certificate = properties["certificate"];

            if (certificate != null && certificate.ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
                if (supportingCertificates.Count == 0)
                {
                    throw new InvalidOperationException("UnableToLoadCertificateIdentity");
                }

                X509Certificate2 primaryCertificate = supportingCertificates[0];
                supportingCertificates.RemoveAt(0);
                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }

            return identity;
        }

        /// <summary>
        /// Adds the configured behavior to the selected endpoint
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Type.InvokeMember", Justification = "This is a real hack, but there is no other way of doing it :(")]
        private static void LoadBehaviors(ServiceModelSectionGroup group, string behaviorConfiguration, ServiceEndpoint serviceEndpoint)
        {
            EndpointBehaviorElement behaviorElement = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
            foreach (BehaviorExtensionElement behaviorExtension in behaviorElement)
            {
                object extension = behaviorExtension.GetType().InvokeMember(
                    "CreateBehavior",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    behaviorExtension,
                    null,
                    CultureInfo.InvariantCulture);
                if (extension != null)
                {
                    serviceEndpoint.Behaviors.Add((IEndpointBehavior)extension);
                }
            }
        }

        /// <summary>
        /// Helper method to create the right binding depending on the configuration element
        /// </summary>
        private static Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is NetTcpBindingElement)
            {
                return new NetTcpBinding();
            }

            if (configurationElement is NetMsmqBindingElement)
            {
                return new NetMsmqBinding();
            }

            if (configurationElement is BasicHttpBindingElement)
            {
                return new BasicHttpBinding();
            }

            if (configurationElement is NetNamedPipeBindingElement)
            {
                return new NetNamedPipeBinding();
            }

            if (configurationElement is NetPeerTcpBindingElement)
            {
                return new NetPeerTcpBinding();
            }

            if (configurationElement is WSDualHttpBindingElement)
            {
                return new WSDualHttpBinding();
            }

            if (configurationElement is WSHttpBindingElement)
            {
                return new WSHttpBinding();
            }

            if (configurationElement is WSFederationHttpBindingElement)
            {
                return new WSFederationHttpBinding();
            }

            if (configurationElement is CustomBindingElement)
            {
                return new CustomBinding();
            }

            return null;
        }
    }
}