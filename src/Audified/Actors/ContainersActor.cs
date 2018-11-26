using System;
using System.Collections.ObjectModel;
using System.Linq;
using Akka.Actor;
using Audified.Messages;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Audified
{
    public class ContainersActor : ReceiveActor
    {
        private readonly DockerClient client;

        public ContainersActor(DockerClient client)
        {
            this.client = client;

            Receive<ListContainers.Request>(message => Handle(message));
        }

        public static Props GetProps(DockerClient client) =>
            Props.Create<ContainersActor>(() => new ContainersActor(client));

        private void Handle(ListContainers.Request message)
        {
            var options = new ContainersListParameters { All = true };
            client.Containers
                .ListContainersAsync(options)
                .PipeTo(
                    recipient: Sender,
                    sender: Self,
                    success: containers =>
                    {
                        var containersId = containers
                            .Select(x => x.ID)
                            .ToList();
                        return new ListContainers.Response(new ReadOnlyCollection<string>(containersId));
                    });
        }
    }
}