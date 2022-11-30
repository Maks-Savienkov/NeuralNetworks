using org.mariuszgromada.math.mxparser;

namespace NeuralNetworks.Models
{
    class FunctionWithinBoundaries
    {
        public Function Function { get; private set; }
        public Range Range { get; private set; }

        public double LowerLimit { get => Range.LowerLimit; }
        public double UpperLimit { get => Range.UpperLimit; }

        public FunctionWithinBoundaries(Function function, double lowerLimit, double upperLimit)
        {
            Function = function;
            Range = new Range(lowerLimit, upperLimit);
        }
    }
}
