using System;
using Akka.Actor;
using Audified.Messages;
using Docker.DotNet;

namespace Audified
{
    public class AudifiedSupervisor : ReceiveActor
    {
        private IActorRef containers;

        public AudifiedSupervisor()
        {
            Receive<ListContainers.Response>(message => Handle(message));
        }

        private void Handle(ListContainers.Response message)
        {
            foreach (var id in message.ContainersId)
                Console.WriteLine(id);
        }

        protected override void PreStart()
        {
            var localhost = new Uri("unix:///var/run/docker.sock");
            var client = new DockerClientConfiguration(localhost)
                .CreateClient();

            containers = Context.ActorOf(ContainersActor.GetProps(client), "containers");

            Context.System.Scheduler.ScheduleTellOnce(
                delay: TimeSpan.FromSeconds(1),
                receiver: containers,
                message: new ListContainers.Request(),
                sender: Self);
        }
    }
}