using Akka.Actor;
using ClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class TicketScorerActor : ReceiveActor
    {
        public TicketScorerActor()
        {
            Receive<TicketListMessage>(msg =>
            {
                var scoredTickets = new List<LotteryTicket>();
                //score ticket


                Sender.Tell(new TicketListMessage(msg.lotteryTickets));
                
            });
        }


    }
}
