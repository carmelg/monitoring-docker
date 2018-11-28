using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Floatc.Messages;
using Newtonsoft.Json;

namespace Floatc.Actors
{
    public class RabbitMqManagementActor : ReceiveActor
    {
        private readonly HttpClient client;

        public RabbitMqManagementActor()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("http://broker:15672"),
            };
            var secrets = Encoding.UTF8.GetBytes("guest:guest");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(secrets));

            ReceiveAsync<GetQueueMessage.Request>(async message =>
            {
                var response = await client.GetAsync($"/api/queues/{message.VirtualHost}/{message.Queue}");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var document = JsonConvert.DeserializeObject<Document>(json);

                Sender.Tell(new GetQueueMessage.Response(document.Messages));
            });
        }

        private class Document
        {
            public int Messages { get; set; }
        }
    }
}