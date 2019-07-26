using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Dto
{
    public class PowerUsageSampleDto
    {
        public DateTime SampleDate { get; set; }
        public double Current { get; set; } // [A]
        public double Voltage { get; set; } // [V]
    }
}
