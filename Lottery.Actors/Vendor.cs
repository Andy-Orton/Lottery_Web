using Akka.Actor;
using Akka.Event;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class Vendor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public Vendor()
        {
            Log.Info("Created Vendor");
            Receive<TicketBoughtMessage>(msg =>
            {
                Log.Info($"{msg.name} user bought a ticket");

            });
        }
    }
}
