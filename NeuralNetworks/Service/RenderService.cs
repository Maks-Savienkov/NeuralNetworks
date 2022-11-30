using NeuralNetworks.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeuralNetworks.Service
{
    public delegate Size ImageSize();
    public delegate Range RangeDelegate();

    class RenderService
    {
        public static readonly double DPI = 96;
        private ImageSize imageSize;
        private RangeDelegate X_Range;
        private RangeDelegate Y_Range;

        private Image Image;
        private (double singleXSegmentLength, double singleYSegmentLength, Point zeroPoint) drawInfo;

        private FunctionWithinBoundaries function;
        private Point[] _points;
        private Point[] _firstClassPoints;
        private Point[] _secondClassPoints;
        private double _nextX;

        public RenderService(Image image, ImageSize imageSize, RangeDelegate x_range, RangeDelegate y_range)
        {
            Image = image;
            this.imageSize = imageSize;
            X_Range = x_range;
            Y_Range = y_range;
        }

        public void UpdateRenderData(FunctionWithinBoundaries function, Point[] points, double nextX)
        {
            this.function = function;
            _points = points;
            _nextX = nextX;
        }
        public void UpdateRenderData(FunctionWithinBoundaries function, Point[] firstClassPoints, Point[] seconfClassPoints)
        {
            this.function = function;
            _firstClassPoints = firstClassPoints;
            _secondClassPoints = seconfClassPoints;
        }

        public void RenderFrame() => Image.Source = GetFrame();

        private BitmapSource GetFrame()
        {
            RenderTargetBitmap bitmap = new(
                Convert.ToInt32(Math.Max(imageSize().Width, 1)),
                Convert.ToInt32(Math.Max(imageSize().Height, 1)),
                96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new();

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                drawInfo = GetDrawInfo();
                DrawStartLines(dc);
                DrawGraph(dc);
                DrawFirstClassPoints(dc);
                DrawSecondClassPoints(dc);
                //DrawPrognozedPoint(dc);
            }

            bitmap.Render(drawingVisual);
            return bitmap;
        }

        private void DrawFirstClassPoints(DrawingContext dc)
        {
            Pen pen = new(Brushes.Blue, 1);

            for (int i = 0; i < _firstClassPoints.Length; i++)
            {
                Point actualCoord = new(
                    drawInfo.zeroPoint.X + _firstClassPoints[i].X * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - _firstClassPoints[i].Y * drawInfo.singleYSegmentLength);
                dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
            }
        }

        private void DrawSecondClassPoints(DrawingContext dc)
        {
            Pen pen = new(Brushes.Red, 1);

            for (int i = 0; i < _secondClassPoints.Length; i++)
            {
                Point actualCoord = new(
                    drawInfo.zeroPoint.X + _secondClassPoints[i].X * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - _secondClassPoints[i].Y * drawInfo.singleYSegmentLength);
                dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
            }
        }

        private void DrawPrognozedPoint(DrawingContext dc)
        {
            Pen pen = new(Brushes.Red, 1);
            double y = function.Function.calculate(_nextX);
            Point actualCoord = new(
                    drawInfo.zeroPoint.X + _nextX * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - function.Function.calculate(_nextX) * drawInfo.singleYSegmentLength);
            dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
        }

        private void DrawStartLines(DrawingContext dc)
        {
            DrawXStartLine(dc);
            DrawYStartLine(dc);
        }

        private void DrawGraph(DrawingContext dc)
        {
            //DrawStartPoints(dc);

            Pen pen = new(Brushes.Blue, 1);

            Point prevpoint = new(
                    drawInfo.zeroPoint.X + function.LowerLimit * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - function.Function.calculate(function.LowerLimit) * drawInfo.singleYSegmentLength);
            for (double j = function.LowerLimit; Math.Round(j, 2) < function.UpperLimit + 0.1; j += 0.1)
            {
                Point nextPoint = new(
                    drawInfo.zeroPoint.X + j * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - function.Function.calculate(j) * drawInfo.singleYSegmentLength);
                dc.DrawLine(pen, prevpoint, nextPoint);
                prevpoint = nextPoint;
            }
        }

        private (double singleXSegmentLength, double singleYSegmentLength, Point zeroPoint) GetDrawInfo()
        {
            double singleYSegmentLength = (imageSize().Height - 10) / Math.Abs(Y_Range().LowerLimit - Y_Range().UpperLimit);
            double singleXSegmentLength = (imageSize().Width - 10) / Math.Abs(X_Range().LowerLimit - X_Range().UpperLimit);

            double x_coord = Math.Abs(X_Range().LowerLimit);
            if (X_Range().LowerLimit > 0)
            {
                x_coord = -1 * X_Range().LowerLimit;
            }
            Point zeroPoint = new(x_coord * drawInfo.singleXSegmentLength + 5, Y_Range().UpperLimit * drawInfo.singleYSegmentLength);

            return (singleXSegmentLength, singleYSegmentLength, zeroPoint);
        }

        private void DrawStartPoints(DrawingContext dc)
        {
            Pen pen = new(Brushes.Black, 1);

            for (int i = 0; i < _points.Length; i++)
            {
                Point actualCoord = new(
                    drawInfo.zeroPoint.X + _points[i].X * drawInfo.singleXSegmentLength,
                    drawInfo.zeroPoint.Y - _points[i].Y * drawInfo.singleYSegmentLength);
                dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
            }
        }

        private void DrawXStartLine(DrawingContext dc)
        {
            Pen pen = new(Brushes.Black, 1);

            Point actualCoord = new(0, 0);
            for (int i = (int)X_Range().LowerLimit; i < X_Range().UpperLimit; i++)
            {
                double x_coord = -X_Range().LowerLimit;
                if (X_Range().LowerLimit < 0)
                {
                    x_coord = Math.Abs(X_Range().LowerLimit);
                }

                double y_coord = 0;
                if (Y_Range().LowerLimit > 0)
                {
                    y_coord = Math.Abs(Y_Range().LowerLimit);
                }

                actualCoord = new(
                    (i + x_coord) * drawInfo.singleXSegmentLength + 5,
                    (Y_Range().UpperLimit - y_coord) * drawInfo.singleYSegmentLength);
                dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
                dc.DrawLine(pen, actualCoord, new Point(actualCoord.X + drawInfo.singleXSegmentLength, actualCoord.Y));
                dc.DrawText(new FormattedText(
                        $"{i}", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 8, Brushes.Black, 96),
                        actualCoord);
            }
            dc.DrawEllipse(Brushes.Black, pen, new Point(actualCoord.X + drawInfo.singleXSegmentLength, actualCoord.Y), 2, 2);
        }

        private void DrawYStartLine(DrawingContext dc)
        {
            Pen pen = new(Brushes.Black, 1);

            Point actualCoord = new(0, 0);
            for (int i = (int)Y_Range().LowerLimit; i < Y_Range().UpperLimit; i++)
            {
                double x_coord = 0;
                if (X_Range().LowerLimit < 0)
                {
                    x_coord = Math.Abs(X_Range().LowerLimit);
                }

                double y_coord = -Y_Range().LowerLimit;
                if (Y_Range().LowerLimit < 0)
                {
                    y_coord = Math.Abs(Y_Range().LowerLimit);
                }

                actualCoord = new(
                    x_coord * drawInfo.singleXSegmentLength + 5,
                    (i + y_coord) * drawInfo.singleYSegmentLength);
                dc.DrawEllipse(Brushes.Black, pen, actualCoord, 2, 2);
                dc.DrawLine(pen, actualCoord, new Point(actualCoord.X, actualCoord.Y + drawInfo.singleYSegmentLength));
            }
            dc.DrawEllipse(Brushes.Black, pen, new Point(actualCoord.X, actualCoord.Y + drawInfo.singleYSegmentLength), 2, 2);
        }
    }
}
