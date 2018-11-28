using System;
using Akka.Actor;
using Floatc.Messages;

namespace Floatc.Actors
{
    public class FloatcSupervisor : ReceiveActor
    {
        private const string QueueName = "Wpc2018";

        public FloatcSupervisor()
        {
            Receive<GetQueueMessage.Response>(message =>
            {
                Console.WriteLine(message.Count);
            });
        }

        protected override void PreStart()
        {
            var actor = Context.ActorOf<RabbitMqManagementActor>();

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(10),
                    interval: TimeSpan.FromSeconds(10),
                    receiver: actor,
                    message: new GetQueueMessage.Request(QueueName),
                    sender: Self
                );
        }
    }
}