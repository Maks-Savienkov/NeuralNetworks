using NeuralNetworks.Models;
using NeuralNetworks.Service;
using NeuralNetworks.Service.MethodOfLeastSquares;
using NeuralNetworks.Service.PerceptronMethod;
using org.mariuszgromada.math.mxparser;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeuralNetworks
{
    public class Controler
    {
        private MethodOfLeastSquaresCalcService _methodOfLeastSquaresService;
        private RenderService _renderService;
        private СlassificationProblemTask _classificationProblemTask;

        public Controler(Image image, ImageSize imageSize, RangeDelegate x_range, RangeDelegate y_range)
        {
            _renderService = new(image, imageSize, x_range, y_range);
        }

        public void Predict(Point[] points, double nextX)
        {
            _methodOfLeastSquaresService = new MethodOfLeastSquaresCalcService(points);

            FunctionWithinBoundaries functions = new FunctionWithinBoundaries(_methodOfLeastSquaresService.Function, points[0].X - 5, points[points.Length - 1].X + 5);

            _renderService.UpdateRenderData(functions, points, nextX);
            _renderService.RenderFrame();
        }

        public void Clasificate(Point[] points)
        {
            FunctionWithinBoundaries functions = new(
                _classificationProblemTask.Function,
                points.Select(x => x.X).Min() - 5,
                points.Select(x => x.X).Max() + 5
            );

            (Point[], Point[]) classifiedPoints = _classificationProblemTask.ClassificatePoints(points);

            _renderService.UpdateRenderData(functions, classifiedPoints.Item1, classifiedPoints.Item2);
            _renderService.RenderFrame();
        }

        public void TeachNetwork()
        {
            _classificationProblemTask = new СlassificationProblemTask();
            _classificationProblemTask.StudyNetwork(5);
        }
    }
}
