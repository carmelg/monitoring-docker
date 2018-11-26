using System;
using Akka.Actor;
using Audified.Messages;
using Docker.DotNet;

namespace Audified.Actors
{
    public class ContainerActor : ReceiveActor
    {
        private readonly IDockerClient client;

        public ContainerActor(IDockerClient client)
        {
            this.client = client;

            Receive<InspectContainer.Request>(message => Handle(message));
            Receive<InspectContainer.Response>(message => Handle(message));
        }

        private void Handle(InspectContainer.Request request)
        {
            client.Containers
                .InspectContainerAsync(request.ContainerId)
                .PipeTo(
                    recipient: Self,
                    sender: Self,
                    success: response => new InspectContainer.Response(
                        id: response.ID,
                        name: response.Name,
                        image: response.Image,
                        isRunning: response.State.Running
                    )
                );
        }

        public static Props GetProps(IDockerClient client) =>
            Props.Create<ContainerActor>(() => new ContainerActor(client));

        private void Handle(InspectContainer.Response message)
        {
            Console.WriteLine($"{message.Name} // {message.Image} // {message.IsRunning}");
            Console.WriteLine("**********************************************************");
        }

        protected override void PreStart()
        {
            var containerId = Self.Path.Name;

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(1),
                    interval: TimeSpan.FromSeconds(5),
                    receiver: Self,
                    message: new InspectContainer.Request(containerId),
                    sender: Self
                );
        }
    }
}