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
    public class LotteryStatistics : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public List<LotteryTicket> Tickets { get; set; }

        public LotteryStatistics()
        {
            Tickets = new List<LotteryTicket>();
            Receive<TopTenWinnersMessage>(msg =>
            {
                Sender.Tell(new TopTenWinnersMessage());
            });
        }
    }
}
