﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class ActorTypes
    {
        public const string StatsActor = "StatsActor";
        public const string UserGenerator = "UserGenerator";
        public const string PeriodActor = "PeriodActor";
        public const string AllUsers = UserGenerator + "/*";
        public const string VendorRoundRobin = "VendorRoundRobin";
        public const string TicketListActor = "TicketListActor";
    }
}
