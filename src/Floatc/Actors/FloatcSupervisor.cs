using System;
using System.Linq;
using Akka.Actor;
using Floatc.Messages;

namespace Floatc.Actors
{
    public class FloatcSupervisor : ReceiveActor
    {
        private const string QueueName = "Wpc2018";
        private const int MinMessages = 50;
        private const int MaxMessages = 100;

        private int counter;

        public FloatcSupervisor()
        {
            Receive<GetQueueMessage.Response>(message => Handle(message));
        }

        private void Handle(GetQueueMessage.Response message)
        {
            if (message.Count > 20)
            {
                var actor = Context.ActorOf<ContainerManagementActor>();
                actor.Tell(new AddContainerInstance());
            }
            else if (message.Count < 20 && Context.GetChildren().Any(IsNotRabbitManagementActor))
            {
                var actor = Context.GetChildren().First(IsNotRabbitManagementActor);
                actor.Tell(new RemoveContainerInstance());
            }
        }

        protected override void PreStart()
        {
            var actor = Context.ActorOf<RabbitMqManagementActor>("rabbitmq-management");

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    initialDelay: TimeSpan.FromSeconds(10),
                    interval: TimeSpan.FromSeconds(5),
                    receiver: actor,
                    message: new GetQueueMessage.Request(QueueName),
                    sender: Self
                );
        }

        private static Func<IActorRef, bool> IsNotRabbitManagementActor = x => x.Path.Name != "rabbitmq-management";
    }
}