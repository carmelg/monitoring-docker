using System;
using System.Linq;
using Akka.Actor;
using Audified.Messages;
using Docker.DotNet;

namespace Audified.Actors
{
    public class AudifiedSupervisor : ReceiveActor
    {
        private IDockerClient client;
        private IActorRef store;

        public AudifiedSupervisor()
        {
            Receive<ListContainers.Response>(message => Handle(message));
        }

        private void Handle(ListContainers.Response message)
        {
            var newContainersId = message.ContainersId
                .Where(x => Context.Child(x) == Nobody.Instance);

            foreach (var newContainerId in newContainersId)
                Context.ActorOf(ContainerActor.GetProps(client, store), newContainerId);
        }

        protected override void PreStart()
        {
            var localhost = new Uri("unix:///var/run/docker.sock");
            client = new DockerClientConfiguration(localhost)
                .CreateClient();

            var containers = Context.ActorOf(ContainersActor.GetProps(client), "containers");
            store = Context.ActorOf<ElasticSearchActor>("store");

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(1),
                    interval: TimeSpan.FromSeconds(5),
                    receiver: containers,
                    message: new ListContainers.Request(),
                    sender: Self
                );

        }
    }
}