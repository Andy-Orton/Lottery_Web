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
            var msg = new SupervisorPeriodMessage { NumberOfVendors = 300 };
            var supervisorActor = ActorOfAsTestActorRef(() => new LotterySupervisor(), TestActor);
            supervisorActor.Tell(msg);
            Assert.IsNotNull(ExpectMsg(msg, TimeSpan.FromSeconds(5)));
        }

    }
}
