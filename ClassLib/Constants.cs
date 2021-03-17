using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class Constants
    {
        public const string StatsActor = "StatsActor";
        public const string UserGenerator = "UserGenerator";
        public const string PeriodActor = "PeriodActor";
        public const string AllUsers = UserGenerator + "/*";
        public const string VendorRoundRobin = "VendorRoundRobin";
        public const string TicketListActor = "TicketListActor";
        public const string PeriodActorReference = "/user/LotterySupervisor/PeriodActor";
        public const string ActorSystemName = "LotteryActorSystem";
        public const string ActorSystemHost = "localhost";
        public const string ActorSystemPort = "4053";
    }
}
