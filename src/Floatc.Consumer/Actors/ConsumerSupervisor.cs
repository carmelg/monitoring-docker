using System;
using Akka.Actor;
using Floatc.Consumer.Messages;

namespace Floatc.Consumer.Actors
{
    public class ConsumerSupervisor : ReceiveActor
    {
        protected override void PreStart()
        {
            var consumer = Context.ActorOf<RabbitMqConsumerActor>();

            Context.System.Scheduler
                .ScheduleTellOnce(
                    delay: TimeSpan.FromSeconds(10),
                    receiver: consumer,
                    message: new StartConsumingMessage(),
                    sender: Self
                );
        }
    }
}