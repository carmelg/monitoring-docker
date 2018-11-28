using System;
using Akka.Actor;
using Floatc.Producer.Messages;

namespace Floatc.Producer.Actors
{
    public class ProducerSupervisor : ReceiveActor
    {
        protected override void PreStart()
        {
            var producer = Context.ActorOf<RabbitMqProducerActor>();

            Context.System.Scheduler
                .ScheduleTellOnce(
                    delay: TimeSpan.FromSeconds(10),
                    receiver: producer,
                    message: new StartPublishingMessage(),
                    sender: Self
                );
        }
    }
}