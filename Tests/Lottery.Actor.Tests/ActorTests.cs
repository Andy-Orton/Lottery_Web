using Akka.Actor;
using Akka.TestKit.NUnit3;
using Lottery.Actors;
using Lottery.Actors.Messages;
using NUnit.Framework;
using System;

namespace Tests.Lottery.Actor.Tests
{
    public class ActorTests : TestKit
    {

        public ActorTests() : base()
        {

        }
        private static ActorSystem LotteryActorSystem;

        //[SetUp]
        //private void SetUp()
        //{
        //    LotteryActorSystem = ActorSystem.Create("Lottery");
            
        //}

        [Test]
        public void SupervisorPeriodTest()
        {
            LotteryActorSystem = ActorSystem.Create("Lottery");
            var msg = new SupervisorPeriodMessage { NumberOfVendors = 300 };
            IActorRef supervisorActor = LotteryActorSystem.ActorOf(Props.Create(() => new LotterySupervisor()), "Supervisor");
            supervisorActor.Tell(msg);
            Assert.IsNotNull(ExpectMsg(msg, TimeSpan.FromSeconds(5)));

            var usMsg = new SupervisorUserGeneratorMessage() { NumberOfUsers = 20 };

            supervisorActor.Tell(new BeginPeriodMessage() { NumberOfTickets = 200, NumberOfUsers = 10, NumberOfVendors = 20 });

            var UserGenerator = LotteryActorSystem.ActorSelection("/user/Supervisor/UserGenerator");

        }


    }
}
