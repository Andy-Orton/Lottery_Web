using Akka.Actor;
using Akka.Event;
using ClassLib;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class UserActor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public int NumTickets { get; }
        public int BoughtTickets { get; set; }
        private Random random = new Random();

        public UserActor(int numTickets)
        {
            BoughtTickets = 0;
            NumTickets = numTickets;

            Receive<LotterySalesOpen>(msg =>
            {
                Log.Info($"{Self.Path.Name} Woke up");
                //Parallel.ForEach(Enumerable.Range(0, numTickets), (_) =>
                //{
                Self.Tell(new BuyNTicketMessage (NumTickets));
                //});
            });

            Receive<BadTicketRequest>(msg =>
            {
                Log.Error("Bad Actor");
            });

            Receive<TicketReceiptMessage>(msg =>
            {
                Log.Info($"{Self.Path.Name} confirming ticket receipt received");
            });


            Receive<BuyNTicketMessage>(msg => {
                if(msg.numTicketsLeftToBuy <= 0)
                {
                    return;
                }
                Context.ActorSelection(ActorTypes.PeriodActorReference).Tell(new BuyTicketMessage { lotteryTicket = new LotteryTicket(Self.Path.Name) });
                Self.Tell(new BuyNTicketMessage(msg.numTicketsLeftToBuy-1));
            });


        }


    }

    public record BuyNTicketMessage (int numTicketsLeftToBuy);
}