using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors.Messages
{
    public record UserGenerationCompleteMessage
    {
        public int CreatedChildren { get; init; }
    }
}
