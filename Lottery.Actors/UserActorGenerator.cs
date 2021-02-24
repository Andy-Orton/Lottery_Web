using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class UserActorGenerator : ReceiveActor
    {
        private Random random = new Random();
        private ILoggingAdapter log { get; } = Context.GetLogger();
        private List<IActorRef> users = new ();

        public UserActorGenerator()
        {
            Receive<SupervisorUserGeneratorMessage>(msg =>
            {
                for (int i = 0; i < msg.NumberOfUsers; i++)
                {
                    var user = Context.ActorOf(Props.Create(() => new UserActor(random.Next(msg.MinTickets, msg.MaxTickets))), "User" + i);
                    users.Add(user);
                }
                Sender.Tell(new UserGenerationCompleteMessage() { CreatedChildren = Context.GetChildren().Count() });
            });

            Receive<DoneBuyingTicketsMessage>(msg =>
            {
                users.Remove(Sender);
                if(users.Count == 0)
                {
                    Context.Parent.Tell(new AllUserTicketPurchasesCompleteMessage());
                }
            });
        }
    }
    public record AllUserTicketPurchasesCompleteMessage;
}
