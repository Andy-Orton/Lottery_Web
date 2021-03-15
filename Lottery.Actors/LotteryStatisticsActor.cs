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
    public class LotteryStatisticsActor : ReceiveActor
    {
        private LotteryTicket WinningTicket;

        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public List<LotteryTicket> Tickets { get; set; }

        public LotteryStatisticsActor()
        {
            Tickets = new List<LotteryTicket>();
            Receive<TopTenWinnersMessage>(msg =>
            {
                Sender.Tell(new TopTenWinnersMessage());
            });

            Receive<GenerateStatisticsMessage>(msg =>
            {
                WinningTicket = new LotteryTicket("WinningTicket");
                Context.ActorSelection("../" + Constants.TicketListActor).Tell(new ScoreTicketsMessage(WinningTicket));
            });

            Receive<AllTicketsScoredMessage>(msg =>
            {
                Context.Parent.Forward(msg);
            });
        }
    }

    public record ScoreTicketsMessage(LotteryTicket WinningLotteryTicket);
}
