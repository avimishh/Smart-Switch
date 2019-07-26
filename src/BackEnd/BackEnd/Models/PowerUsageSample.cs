using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace BackEnd.Models
{
    /// <summary>
    /// A current and voltage sample of a given device at a given time for 1 minute.
    /// </summary>
    public class PowerUsageSample
    {
        [Key]
        public int PowerUsageSampleId { get; set; }
        public DateTime SampleDate { get; set; }
        public double Current { get; set; } // [A]
        public double Voltage { get; set; } // [V]

        public PowerUsageSample() { }

        public PowerUsageSample(DateTime dt, double v, double c)
        {
            Current = c;
            SampleDate = dt;
            Voltage = v;
        }

        public PowerUsageSample(double v, double c) : this(DateTime.Now, v, c) { }

        public double GetWattage() => Current * Voltage; // [W]

        // assuming 1[kW] during 1[hour] is 0.53[ILS].
        // every sample represents 1 minute of use, so we take the wattage and multiply it by (0.53 / 60) * 10^-3
        public double GetCost() => GetWattage() * 0.00000883333; // [Israeli new shekel]
    }
}