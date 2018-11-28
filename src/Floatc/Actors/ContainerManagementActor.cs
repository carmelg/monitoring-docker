using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Docker.DotNet;
using Docker.DotNet.Models;
using Floatc.Messages;

namespace Floatc.Actors
{
    public class ContainerManagementActor : ReceiveActor
    {
        private IDockerClient client;
        private string containerId;

        public ContainerManagementActor()
        {
            Become(Idle);
        }

        private void Idle()
        {
            ReceiveAsync<AddContainerInstance>(message => Handle(message));
        }

        private void Running()
        {
            ReceiveAsync<RemoveContainerInstance>(message => Handle(message));
        }

        private async Task Handle(AddContainerInstance message)
        {
            var imageId = await GetImageId();
            var networkId = await GetNetworkId();

            if (string.IsNullOrWhiteSpace(imageId) || string.IsNullOrWhiteSpace(networkId)) return;

            var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters { Image = imageId });
            await client.Networks.ConnectNetworkAsync(networkId, new NetworkConnectParameters { Container = container.ID });
            await client.Containers.StartContainerAsync(container.ID, null);

            containerId = container.ID;

            Become(Running);
        }

        private async Task Handle(RemoveContainerInstance message)
        {
            Console.WriteLine($"Stopping container {containerId}");
            var stopped = await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 10 });

            if (!stopped) return;
            Console.WriteLine($"Stopped container {containerId}");

            Console.WriteLine($"Removing container {containerId}");
            await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true });
            Console.WriteLine($"Removed container {containerId}");

            Context.Stop(Self);
        }

        protected override void PreStart()
        {
            var localhost = new Uri("unix:///var/run/docker.sock");
            client = new DockerClientConfiguration(localhost)
                .CreateClient();
        }

        private async Task<string> GetImageId()
        {
            var parameters = new ImagesListParameters { MatchName = "wpc2018_consumer:latest" };
            var images = await client.Images.ListImagesAsync(parameters);
            var image = images.FirstOrDefault();

            return image?.ID;
        }

        private async Task<string> GetNetworkId()
        {
            var networks = await client.Networks.ListNetworksAsync(null);
            var network = networks.FirstOrDefault(x => x.Name == "wpc2018_default");

            return network?.ID;
        }
    }
}