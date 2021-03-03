using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using ClassLib;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class TicketListActor : ReceivePersistentActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        private List<LotteryTicket> unscoredTickets = new ();
        private List<LotteryTicket> scoredTickets = new();
        private List<IActorRef> scorers = new ();
        public override string PersistenceId { get { return $"{Self.Path.Name}"; } }
        public TicketListActor()
        {
            Command<BuyTicketMessage>(msg => 
            {
                Persist(msg, msg =>
                {
                    Log.Info($"Ticket added to list: {msg.lotteryTicket}");
                    unscoredTickets.Add(msg.lotteryTicket);
                    Sender.Tell(new TicketBoughtMessage { lotteryTicket = msg.lotteryTicket });
                });
            });

            Recover<BuyTicketMessage>(msg =>
            {
                Log.Info("Replaying message from journal");
                Log.Info($"Ticket added to list: {msg.lotteryTicket}");
                unscoredTickets.Add(msg.lotteryTicket);
            });

            //Receive<ScoreTicketsMessage>(msg =>
            //{
            //    //How to scale appropiately if running in the cluster
            //    var splitSize = unscoredTickets.Count / Environment.ProcessorCount;
            //    var splitTicketLists = splitList(unscoredTickets, splitSize);
            //    foreach(var list in splitTicketLists)
            //    {
            //        var scorer = Context.ActorOf(Props.Create<TicketScorerActor>());
            //        scorers.Add(scorer);
            //        scorer.Tell(new TicketListMessage(list, msg.WinningLotteryTicket));
            //    }
            //    Become(Scoring);
            //});

        }

        public void Scoring()
        {
            Command<TicketListMessage>(msg =>
            {
                scorers.Remove(Sender);
                scoredTickets.AddRange(msg.lotteryTickets);
                if(scorers.Count == 0)
                {
                    Context.ActorSelection("../" + ActorTypes.StatsActor).Tell(new AllTicketsScoredMessage(scoredTickets));
                }
            });
        }

        private List<List<LotteryTicket>> splitList(List<LotteryTicket> lotteryTickets, int chunkSize)
        {
            var list = new List<List<LotteryTicket>>();
            for(int i = 0; i < lotteryTickets.Count; i += chunkSize)
            {
                list.Add(lotteryTickets.GetRange(i, Math.Min(chunkSize, lotteryTickets.Count - i)));
            }
            return list;
        }

    }
    public record TicketListMessage(List<LotteryTicket> lotteryTickets, LotteryTicket WinningLotteryTicket);
    public record AllTicketsScoredMessage(List<LotteryTicket> scoredTickets);
}
