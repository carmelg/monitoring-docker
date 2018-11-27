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

            Receive<StoreData<Payload>>(message => Handle(message));
        }

        private void Handle(StoreData<Payload> message)
        {
            Console.WriteLine("Indexing data...");
            var response = client.Index(message.Payload, x => x.Index("persons").Type("persons"));
            Console.WriteLine($"Index result: {response.Result}");
        }

        protected override void PreStart()
        {
            var payload = new Payload
            {
                Id = "1",
                Firstname = "Martijn",
                Lastname = "Laarman"
            };

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(1),
                    interval: TimeSpan.FromSeconds(5),
                    receiver: Self,
                    message: new StoreData<Payload>(payload),
                    sender: Self
                );
        }

        public class Payload
        {
            public string Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
        }
    }
}