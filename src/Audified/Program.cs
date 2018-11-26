using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Audified
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = Run().GetAwaiter().GetResult();
            foreach (var name in names)
                Console.WriteLine(name);
        }

        static async Task<IEnumerable<string>> Run()
        {
            var localhost = new Uri("unix:///var/run/docker.sock");
            var client = new DockerClientConfiguration(localhost)
                .CreateClient();

            var options = new ContainersListParameters
            {
                Limit = 10
            };
            var containers = await client.Containers.ListContainersAsync(options);
            return containers
                .Select(x => x.Names.DefaultIfEmpty("N.A.").FirstOrDefault());
        }
    }
}
