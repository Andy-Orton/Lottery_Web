using ClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors.Messages
{
    public record BuyTicketMessage
    {
        public LotteryTicket lotteryTicket { get; init; }
    }
}
