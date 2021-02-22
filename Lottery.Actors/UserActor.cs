using Akka.Actor;
using Akka.Event;
using ClassLib;
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
        private Random random = new Random();

        public UserActor(int numTickets)
        {
            NumTickets = numTickets;

            Receive<LotterySalesOpen>(msg =>
            {
                
                Context.ActorSelection("akka://LotteryActorSystem/user/LotterySupervisor/PeriodActor").Tell(new BuyTicketMessage {lotteryTicket = new LotteryTicket(Self.Path.Name)});
                
            });

            Receive<BadTicketRequest>(msg =>
            {
                Log.Error("Bad Actor");
            });

            Receive<TicketReceiptMessage>(msg =>
            {
                Log.Info($"{Self.Path.Name} confirming ticket receipt received");
            });
        }
    }
}