﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors.Messages
{
    public record VendorGenerationCompleteMessage
    {
        public int CreatedVendors { get; set; }
    }
}
