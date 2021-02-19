using Akka.Actor;
using Akka.Event;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class UserActor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public int NumTickets { get; }

        public UserActor(int numTickets)
        {
            NumTickets = numTickets;
            Receive<LotterySalesOpen>(msg =>
            {
                int[] selectedBalls = { 1, 2, 3, 4, 5, 6 };
                Context.ActorSelection("akka://LotteryActorSystem/user/LotterySupervisor/PeriodActor").Tell(new TicketBoughtMessage {name = Self.Path.Name, balls = selectedBalls });
            });

            Receive<BadTicketRequest>(msg =>
            {
                Log.Error("Bad Actor");
            });
        }
    }
}