using System;

namespace CSV
{
    public class CarStatistics
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }

        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }

        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Horsepower;
            Max = Math.Max(Max, car.Horsepower);
            Min = Math.Min(Min, car.Horsepower);
            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }
    }
}
