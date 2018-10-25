using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;

namespace NamePipeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //WcfNamedPipe.Run(args);
            Program.Run(args);
        }

        static void Run(string[] args)
        {

            if (args.Length > 0)
            {
                var nServers = 5;
                while (true)
                {
                    var server = new NamedPipeServerStream("PipesOfPiece", PipeDirection.InOut, nServers, 
                        PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    Console.Write("Waiing for client ... ");
                    server.WaitForConnection();
                    Console.WriteLine("connected");
                    StartServer(server);
                }
            }
            else
            {
                //Client
                var client = new NamedPipeClientStream("PipesOfPiece");
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);

                while (true)
                {
                    string input = Console.ReadLine();
                    if (String.IsNullOrEmpty(input)) break;
                    writer.WriteLine(input);
                    writer.Flush();
                    Console.WriteLine(reader.ReadLine());
                }
            }
        }

        static void StartServer(NamedPipeServerStream server)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (StreamReader reader = new StreamReader(server))
                    using (StreamWriter writer = new StreamWriter(server))
                    {
                        while (true)
                        {
                            var line = reader.ReadLine();
                            if (line == null)
                                break;
                            Console.WriteLine("Received: " + line);
                            var sent = string.Join(string.Empty, line.Reverse());
                            Console.WriteLine("Sent: " + sent);
                            writer.WriteLine(sent);
                            writer.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine("Exited!");
            });
        }
    }
}
