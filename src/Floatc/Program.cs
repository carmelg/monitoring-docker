using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Floatc.Actors;

namespace Floatc
{
    class Program
    {
        private static readonly AutoResetEvent handle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            ActorSystem system = null;

            Task.Run(() =>
            {
                Console.WriteLine("Initializing system...");

                system = ActorSystem.Create("floatc");
                var root = system.ActorOf<FloatcSupervisor>();

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
