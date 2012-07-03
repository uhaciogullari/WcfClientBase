WcfClientBase
=============
WcfClientBase contains a simple base class that encapsulates WCF service calls.

WcfClientBase introduces an abstract class that handles creating a service client, making the call, closing the channel or aborting it if any exceptions occur. You can also extend exception handling mechanism. At the moment, it doesn't support asynchronous calls. It's thread safe, it can be used with singleton life-cycle to make successive service calls.

NuGet Package : https://nuget.org/packages/WcfClientBase
