WcfClientBase
=============
WcfClientBase contains a simple base class that encapsulates WCF service calls.

WcfClientBase introduces an abstract class that handles creating a service client, making the call, closing the channel or aborting it if any exceptions occur. You can also extend exception handling mechanism. At the moment, it doesn't support asynchronous calls. It's thread safe, it can be used with singleton life-cycle to make successive service calls.

NuGet Package : https://nuget.org/packages/WcfClientBase




    //MemberServiceClient is the class generated by SvcUtil
    public class MemberServiceManager : ServiceClientBase<MemberServiceClient>
    {
        
        //makes a call to GetUsername operation, closes the channel and handles the exceptions
        //you may want to implement another base class for overriding exception handling methods
        //return value will be default of return type if any exceptions occur
        public string GetUsername(int userId)
        {
            return PerformServiceOperation(client => client.GetUserTypeFromProfileID(userId));
        }

        //or you can manually check if any exceptions occured with this overload
        public bool TryGetUsername(int userId, out string username)
        {
            return TryPerformServiceOperation(client => client.GetUserTypeFromProfileID(userId), out username);
        }

    }
	
## License

WcfClientBase is licensed under [MIT](http://opensource.org/licenses/MIT "Read more about the MIT license form"). Refer to license.txt for more information.