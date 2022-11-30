using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralNetworks.Service.MethodOfLeastSquares
{
    class MethodOfLeastSquaresCalcService
    {
        public Point[] Points { get; private set; }

        public Function Function { get => GetFunction(); }

        public MethodOfLeastSquaresCalcService(Point[] points)
        {
            Points = points;
        }

        private Function GetFunction()
        {
            Vector<double> bAnda = GetNormalGaussEquationsLeftMatrix().Solve(GetNormalGaussEquationsRightVector());
            return new Function("func", $"({bAnda[1]}) * x + ({bAnda[0]})".Replace(',', '.'), "x");
        }

        private Matrix<double> GetNormalGaussEquationsLeftMatrix()
        {
            double[,] matrixArray = new double[2, 2];
            matrixArray[0, 0] = Points.Length;
            matrixArray[0, 1] = CalculateXSum();
            matrixArray[1, 0] = matrixArray[0, 1];
            matrixArray[1, 1] = CalculateXSquareSum();
            
            return DenseMatrix.OfArray(matrixArray);
        }

        private Vector<double> GetNormalGaussEquationsRightVector()
        {
            double[] vector = {CalculateYSum(), CalculateXYProductSum()};
            return DenseVector.OfArray(vector);
        }

        private double CalculateXSum()
        {
            return Points.Select(x => x.X).Sum();
        }

        private double CalculateYSum()
        {
            return Points.Select(x => x.Y).Sum();
        }

        private double CalculateXSquareSum()
        {
            return Points.Select(x => Math.Pow(x.X, 2)).Sum();
        }

        private double CalculateXYProductSum()
        {
            return Points.Select(x => x.X * x.Y).Sum();
        }
    }
}
