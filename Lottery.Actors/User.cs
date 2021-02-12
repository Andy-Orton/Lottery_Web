using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class User : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }
    }
}
