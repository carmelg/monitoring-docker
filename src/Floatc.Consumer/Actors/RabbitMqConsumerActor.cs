using System;
using System.Text;
using Akka.Actor;
using Floatc.Consumer.Messages;
using RabbitMQ.Client;

namespace Floatc.Consumer.Actors
{
    public class RabbitMqConsumerActor : ReceiveActor
    {
        private const string QueueName = "Wpc2018";

        private IModel channel;

        public RabbitMqConsumerActor()
        {
            Become(Idle);
        }

        private void Idle()
        {
            Receive<StartConsumingMessage>(message =>
            {
                var factory = new ConnectionFactory { HostName = "broker" };
                var connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(queue: QueueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                Context.System.Scheduler
                    .ScheduleTellRepeatedly(
                        initialDelay: TimeSpan.FromSeconds(1),
                        interval: TimeSpan.FromSeconds(1),
                        receiver: Self,
                        message: new ConsumeMessage(),
                        sender: Self
                    );

                Become(Consuming);
            });
        }

        private void Consuming()
        {
            Receive<ConsumeMessage>(message =>
            {
                var response = channel.BasicGet(QueueName, autoAck: true);
                if (response != null)
                {
                    var value = Encoding.UTF8.GetString(response.Body);
                    Console.WriteLine($" [x] Received {value}");
                }
            });
        }
    }
}