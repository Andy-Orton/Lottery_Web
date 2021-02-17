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
    public class UserActor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public int NumTickets { get; }

        public UserActor(int numTickets)
        {
            NumTickets = numTickets;
            Log.Info("New User");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LotterySalesOpen msg:
                    Log.Info("Sales Open");
                    break;
            }
        }
    }
}
