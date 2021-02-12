using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors.Messages
{
    public record BeginPeriodMessage
    {
        public int NumberOfUsers { get; init; }
        public int NumberOfTickets { get; init; }
        public int NumberOfVendors { get; init; }
    }
}
