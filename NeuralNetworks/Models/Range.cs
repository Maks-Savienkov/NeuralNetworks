namespace NeuralNetworks.Models
{
    public class Range
    {
        public double UpperLimit { get; private set; }
        public double LowerLimit { get; private set; }

        public Range(double lowerLimit, double upperLimit)
        {
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
        }
    }
}
