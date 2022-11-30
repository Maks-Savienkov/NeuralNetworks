using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralNetworks.Models
{
    class Perceptron
    {
        const double LEARNING_RATE = 0.6;
        private double _x;
        private double _y;
        private double _wx;
        private double _wy;

        public double WeightX
        {
            get => _wx;
        }

        public double WeightY
        {
            get => _wy;
        }


        public Perceptron(double x, double y, double wx, double wy)
        {
            _x = x;
            _y = y;
            _wx = wx;
            _wy = wy;
         }

        public double Predict()
        {
            return CalcSigmoidFunction(CalcOutput());
        }

        public double Predict(Point point)
        {
            return CalcSigmoidFunction(CalcOutput(point));
        }

        private double CalcOutput(Point point)
        {
            return point.X * _wx + point.Y * _wy;
        }

        public double CalcOutput()
        {
            return _x * _wx + _y * _wy;
        }

        public void UpdateWeights(double weightDelta)
        {
            _wx = CalcNewWeight(_wx, _x, weightDelta);
            _wy = CalcNewWeight(_wy, _y, weightDelta);
        }

        public static double CalcSigmoidFunction(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -x));
        }

        public static double CalcSigmoidDifferential(double x)
        {
            return CalcSigmoidFunction(x) * (1 - CalcSigmoidFunction(x));
        }        

        public static double CalcError(double actual, double expected)
        {
            return actual - expected;
        }

        public static double CalcWeihtsDelta(double error, double output)
        {
            return error * CalcSigmoidDifferential(output);
        }

        private static double CalcNewWeight(double actualWeight, double output, double weightDelta)
        {
            return actualWeight - output * weightDelta * LEARNING_RATE;
        }
    }
}
