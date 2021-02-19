using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors.Messages
{
    public record TicketBoughtMessage
    {
        public string name { get; init; }
        public int[] balls { get; init; }
    }
}
