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
    public class TicketListActor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        private List<LotteryTicket> tickets = new ();
        public TicketListActor()
        {
            Receive<BuyTicketMessage>(msg =>
            {
                Log.Info($"Ticket added to list: {msg.lotteryTicket.ToString()}");
                tickets.Add(msg.lotteryTicket);
                Sender.Tell(new TicketBoughtMessage { lotteryTicket = msg.lotteryTicket });
            });
        }


    }
}
