using Akka.Actor;
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
        private List<LotteryTicket> tickets = new ();
        public TicketListActor()
        {
        }
    }
}
