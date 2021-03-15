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
    public class VendorActor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public VendorActor()
        {
            Receive<BuyTicketMessage>(msg =>
            {
                Log.Info($"{msg.lotteryTicket.Player} buying a ticket");
                Context.ActorSelection($"/user/LotterySupervisor/PeriodActor/TicketListActor").Tell(msg);
            });

            Receive<TicketBoughtMessage>(msg =>
            {
                Log.Info($"{msg.lotteryTicket.Player}'s ticket has been bought");
                Context.ActorSelection($"/user/LotterySupervisor/UserGenerator/{msg.lotteryTicket.Player}").Tell(new TicketReceiptMessage { });
            });
        }
    }
}
