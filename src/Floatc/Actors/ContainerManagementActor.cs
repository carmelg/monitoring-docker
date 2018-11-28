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

        public ContainerManagementActor()
        {
            ReceiveAsync<AddContainerInstance>(message => Handle(message));
        }

        private async Task Handle(AddContainerInstance message)
        {
            var imageId = await GetImageId();
            var networkId = await GetNetworkId();

            if (!string.IsNullOrWhiteSpace(imageId) && !string.IsNullOrWhiteSpace(networkId))
            {
                var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters { Image = imageId });
                await client.Networks.ConnectNetworkAsync(networkId, new NetworkConnectParameters { Container = container.ID });

                await client.Containers.StartContainerAsync(container.ID, null);
            }

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