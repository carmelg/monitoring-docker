using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Audified
{
    class Program
    {
        private static readonly AutoResetEvent handle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            ActorSystem system = null;

            Task.Run(() =>
            {
                var random = new Random(10);
                Console.WriteLine("Initializing system...");

                system = ActorSystem.Create("audified");
                var root = system.ActorOf<AudifiedSupervisor>();

                Console.WriteLine("System initialized");
            });

            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Shutting down the system...");
                if (system != null)
                    system.Terminate();
                Console.WriteLine("Bye bye!");

                handle.Set();
            };

            handle.WaitOne();
        }
    }
}
