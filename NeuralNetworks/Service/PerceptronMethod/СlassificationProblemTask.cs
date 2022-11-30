using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NeuralNetworks.Models;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace NeuralNetworks.Service.PerceptronMethod
{
    class СlassificationProblemTask
    {
        public Point[] Points { get; private set; }
        private Perceptron perceptron;

        public Function Function { get => GetFunction(); }

        public СlassificationProblemTask() { }
         
        private Function GetFunction()
        {
            return new Function("func", $"({Math.Abs(perceptron.WeightX) / Math.Abs(perceptron.WeightY)}) * x".Replace(',', '.'), "x");
        }

        public (Point[] firstClass, Point[] secondClass) ClassificatePoints(Point[] points)
        {
            List<Point> firstClass = new List<Point>();
            List<Point> secondClass = new List<Point>();

            foreach (Point point in points)
            {
                if (PredictClass(perceptron.Predict(point)) == 0)
                {
                    firstClass.Add(point);
                }
                else
                {
                    secondClass.Add(point);
                }
            }

            return (firstClass.ToArray(), secondClass.ToArray());
        }

        public void StudyNetwork(int generationCount)
        {
            (Point[], int[]) parseData = ParseEducationalSampleFromFile(@"./educational-sample.txt");
            Point[] educationalSample = parseData.Item1;
            int[] expected = parseData.Item2;

            perceptron = new Perceptron(
                educationalSample[0].X,
                educationalSample[0].Y,
                0.3,
                0.4);

            for (int i = 0; i < generationCount; i++)
            {
                for (int j = 0; j < educationalSample.Length; j++)
                {
                    perceptron = new Perceptron(
                        educationalSample[j].X,
                        educationalSample[j].Y,
                        perceptron.WeightX,
                        perceptron.WeightY);

                    double predict = perceptron.Predict();
                    int predictedClass = PredictClass(predict);

                    while (predictedClass != expected[j])
                    {
                        double error = Perceptron.CalcError(predict, expected[j]);
                        double output = perceptron.CalcOutput();
                        double weightDelta = Perceptron.CalcWeihtsDelta(error, output);
                        perceptron.UpdateWeights(weightDelta);
                        predict = perceptron.Predict();
                        predictedClass = PredictClass(predict);
                    }
                }
            }
        }

        private int PredictClass(double predict)
        {
            if (predict < 0.5)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private (Point[] samples, int[] expected) ParseEducationalSampleFromFile(string filelUrl)
        {
            string text = File.ReadAllText(filelUrl);

            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Point[] samples = new Point[lines.Length];
            int[] expected = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Trim().Split('-');

                // process samples
                string[] sampleStr = parts[0].Split(';');
                samples[i] = new Point(double.Parse(sampleStr[0]), double.Parse(sampleStr[1]));

                // process expected
                expected[i] = int.Parse(parts[1]);
            }
            return (samples, expected);
        }
    }
}
