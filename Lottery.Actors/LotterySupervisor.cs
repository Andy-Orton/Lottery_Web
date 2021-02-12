using Akka.Actor;
using Akka.Event;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Actors
{
    public class LotterySupervisor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Lottery Application started");
        protected override void PostStop() => Log.Info("Lottery Application stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SupervisorUserGeneratorMessage sup:
                    Log.Info(sup.tickets.ToString());
                    break;
                default:
                    Log.Info("Got Message that I didn't know how to do anything with");
                    break;
            }
            
        }

        public static Props Props() => Akka.Actor.Props.Create<LotterySupervisor>();
    }
}
