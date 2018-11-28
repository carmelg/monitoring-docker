using System;
using System.Text;
using Akka.Actor;
using Floatc.Producer.Messages;
using RabbitMQ.Client;

namespace Floatc.Producer.Actors
{
    public class RabbitMqProducerActor : ReceiveActor
    {
        private const string QueueName = "Wpc2018";

        private IModel channel;

        public RabbitMqProducerActor()
        {
            Become(Idle);
        }

        private void Idle()
        {
            Receive<StartPublishingMessage>(message =>
            {
                var factory = new ConnectionFactory { HostName = "broker" };
                var connection = factory.CreateConnection();
                channel = connection.CreateModel();

                Context.System.Scheduler
                    .ScheduleTellRepeatedly(
                        initialDelay: TimeSpan.FromSeconds(1),
                        interval: TimeSpan.FromSeconds(1),
                        receiver: Self,
                        message: new PublishMessage("Hello WPC2018"),
                        sender: Self
                    );

                Become(Publishing);
            });
        }

        private void Publishing()
        {
            Receive<PublishMessage>(message =>
            {
                channel.QueueDeclare(queue: QueueName,
                                                 durable: false,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(message.Value);

                channel.BasicPublish(exchange: "",
                                     routingKey: QueueName,
                                     basicProperties: null,
                                     body: body);
            });
        }
    }
}