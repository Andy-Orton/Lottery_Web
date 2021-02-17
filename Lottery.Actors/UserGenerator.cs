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
    public class UserGenerator : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public List<IActorRef> _users { get; set; }

        public UserGenerator()
        {
            _users = new List<IActorRef>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SupervisorUserGeneratorMessage msg:
                    for(int i = 0; i < msg.NumberOfUsers; i++)
                    {
                        IActorRef _user = Context.ActorOf(Props.Create(() => new User()), "User" + i);
                        _users.Add(_user);
                        Log.Info($"User{i} has been created");
                    }
                    break;
            }
        }
    }
}
