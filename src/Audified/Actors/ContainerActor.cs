using System;
using Akka.Actor;
using Audified.Messages;
using Docker.DotNet;

namespace Audified.Actors
{
    public class ContainerActor : ReceiveActor
    {
        private readonly IDockerClient client;
        private readonly IActorRef store;

        public static Props GetProps(IDockerClient client, IActorRef store) =>
            Props.Create<ContainerActor>(() => new ContainerActor(client, store));

        public ContainerActor(IDockerClient client, IActorRef store)
        {
            this.client = client;
            this.store = store;

            Receive<InspectContainer>(message => Handle(message));
        }

        private void Handle(InspectContainer message)
        {
            client.Containers
                .InspectContainerAsync(message.ContainerId)
                .PipeTo(
                    recipient: store,
                    sender: Self,
                    success: response => new StoreInspectContainerData(
                        id: response.ID,
                        name: response.Name,
                        image: response.Image,
                        isRunning: response.State.Running
                    )
                );
        }

        protected override void PreStart()
        {
            var containerId = Self.Path.Name;

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(1),
                    interval: TimeSpan.FromSeconds(5),
                    receiver: Self,
                    message: new InspectContainer(containerId),
                    sender: Self
                );
        }
    }
}