using System;
using Akka.Actor;
using Audified.Messages;
using Nest;

namespace Audified.Actors
{
    public class ElasticSearchActor : ReceiveActor
    {
        private readonly IElasticClient client;

        public ElasticSearchActor()
        {
            var node = new Uri("http://store:9200");

            var settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);

            Receive<StoreInspectContainerData>(message => Handle(message));
        }

        private void Handle(StoreInspectContainerData message)
        {
            var document = new Document
            {
                Id = message.Id,
                Name = message.Name,
                Image = message.Image,
                IsRunning = message.IsRunning
            };

            client.Index(document, x => x.Index("containers").Type("containers"));
        }

        private class Document
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public bool IsRunning { get; set; }
        }
    }
}