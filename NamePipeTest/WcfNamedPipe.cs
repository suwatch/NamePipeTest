using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace NamePipeTest
{
    class WcfNamedPipe
    {
        const string BaseAddress = "net.pipe://localhost/HelloService";

        public static void Run(string[] args)
        {
            if (args.Length > 0 && args[0] == "server")
            {
                ServiceHost serviceHost = new ServiceHost(typeof(HelloService), new Uri(BaseAddress));
                NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647 };
                serviceHost.AddServiceEndpoint(typeof(IHelloService), binding, BaseAddress);
                serviceHost.Open();

                Console.WriteLine("Service started. Available in following endpoints");
                foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
                {
                    Console.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
                }

                Console.WriteLine("Press enter to exit ..");
                Console.ReadLine();
                serviceHost.Close();
            }
            else
            {
                var proxy = new HelloServiceProxy();
                Console.WriteLine(proxy.Echo(args.Length > 0 ? string.Join(" ", args) : "Hello World!"));
                proxy.Close();
            }
        }

        [ServiceContract]
        public interface IHelloService
        {
            [OperationContract]
            string Echo(string msg);
        }

        public class HelloService : IHelloService
        {
            public string Echo(string msg)
            {
                // simply echo with upper case
                return msg.ToUpper();
            }
        }

        public class HelloServiceProxy : ClientBase<IHelloService>
        {
            public HelloServiceProxy()
                : base(new ServiceEndpoint(ContractDescription.GetContract(typeof(IHelloService)),
                    new NetNamedPipeBinding(), new EndpointAddress(BaseAddress)))
            {
            }

            public string Echo(string msg)
            {
                return Channel.Echo(msg);
            }
        }
    }
}
