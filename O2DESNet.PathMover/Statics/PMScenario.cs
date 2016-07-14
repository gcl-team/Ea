﻿using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace O2DESNet.PathMover
{
    public class PMScenario
    {
        public List<Path> Paths { get; private set; }
        public List<ControlPoint> ControlPoints { get; private set; }
        public Dictionary<Path, double[]> PathCoordinates { get; private set; } // for display

        public PMScenario()
        {
            Paths = new List<Path>();
            ControlPoints = new List<ControlPoint>();
            PathCoordinates = new Dictionary<Path, double[]>();
        }

        #region Path Mover Builder

        /// <summary>
        /// Create and return a new path
        /// </summary>
        public Path CreatePath(double length, double fullSpeed, Direction direction = Direction.TwoWay)
        {
            var path = new Path(this, length, fullSpeed, direction);
            Paths.Add(path);
            return path;
        }

        /// <summary>
        /// Create and return a new control point
        /// </summary>
        public ControlPoint CreateControlPoint(Path path, double position)
        {
            var controlPoint = new ControlPoint(this);
            path.Add(controlPoint, position);
            ControlPoints.Add(controlPoint);
            return controlPoint;
        }

        /// <summary>
        /// Connect two paths at specified positions
        /// </summary>
        public void Connect(Path path_0, Path path_1, double position_0, double position_1)
        {
            path_1.Add(CreateControlPoint(path_0, position_0), position_1);
        }

        /// <summary>
        /// Connect the Path to the Control Point at specific positions
        /// </summary>
        public void Connect(Path path, double position, ControlPoint controlPoint)
        {
            if (controlPoint.Positions.ContainsKey(path)) throw new Exception("The Control Point exists on the Path.");
            path.Add(controlPoint, position);
        }

        /// <summary>
        /// Connect the end of path_0 to the start of path_1
        /// </summary>
        public void Connect(Path path_0, Path path_1) { Connect(path_0, path_1, path_0.Length, 0); }
        #endregion

        #region For Static Routing (Distance-Based)
        public void Initialize()
        {
            ConstructRoutingTables();
            ConstructPathingTables();
        }
        private void ConstructRoutingTables()
        {
            foreach (var cp in ControlPoints) cp.RoutingTable = new Dictionary<ControlPoint, ControlPoint>();
            var incompleteSet = ControlPoints.ToList();
            var edges = Paths.SelectMany(path => GetEdges(path)).ToList();
            while (incompleteSet.Count > 0)
            {
                ConstructRoutingTables(incompleteSet.First().Id, edges);
                incompleteSet.RemoveAll(cp => cp.RoutingTable.Count == ControlPoints.Count - 1);
            }
        }
        private void ConstructRoutingTables(int sourceIndex, List<Tuple<int, int, double>> edges)
        {
            var edgeList = edges.ToList();
            var dijkstra = new Dijkstra(edges);

            var sinkIndices = new HashSet<int>(ControlPoints.Select(cp => cp.Id));
            sinkIndices.Remove(sourceIndex);
            foreach (var target in ControlPoints[sourceIndex].RoutingTable.Keys) sinkIndices.Remove(target.Id);

            while (sinkIndices.Count > 0)
            {
                var sinkIndex = sinkIndices.First();
                var path = dijkstra.ShortestPath(sourceIndex, sinkIndex);
                path.Add(sourceIndex);
                path.Reverse();
                for (int i = 0; i < path.Count - 1; i++)
                {
                    for (int j = i + 1; j < path.Count; j++)
                        ControlPoints[path[i]].RoutingTable[ControlPoints[path[j]]] = ControlPoints[path[i + 1]];
                    sinkIndices.Remove(path[i + 1]);
                }
            }
        }
        private void ConstructPathingTables()
        {
            foreach (var cp in ControlPoints) cp.PathingTable = new Dictionary<ControlPoint, Path>();
            foreach (var path in Paths)
            {
                // assume same pair of control points are connected only by one path
                if (path.Direction != Direction.Backward)
                    for (int i = 0; i < path.ControlPoints.Count - 1; i++)
                        path.ControlPoints[i].PathingTable.Add(path.ControlPoints[i + 1], path);
                if (path.Direction != Direction.Forward)
                    for (int i = path.ControlPoints.Count - 1; i > 0; i--)
                        path.ControlPoints[i].PathingTable.Add(path.ControlPoints[i - 1], path);
            }
        }
        private List<Tuple<int, int, double>> GetEdges(Path path)
        {
            var edges = new List<Tuple<int, int, double>>();
            for (int i = 0; i < path.ControlPoints.Count - 1; i++)
            {
                var length = path.ControlPoints[i + 1].Positions[path] - path.ControlPoints[i].Positions[path];
                var from = path.ControlPoints[i].Id;
                var to = path.ControlPoints[i + 1].Id;
                if (path.Direction != Direction.Backward) edges.Add(new Tuple<int, int, double>(from, to, length));
                if (path.Direction != Direction.Forward) edges.Add(new Tuple<int, int, double>(to, from, length));
            }
            return edges;
        }
        #endregion

        #region For Display
        
        private void InitDrawingParams(DrawingParams dParams)
        {
            dParams.Init(PathCoordinates.Values.SelectMany(c => new List<double[]> { new double[] { c[0], c[1] }, new double[] { c[2], c[3] } }));
        }

        public void DrawToImage(string file, DrawingParams dParams)
        {
            InitDrawingParams(dParams);
            Bitmap bitmap = new Bitmap(Convert.ToInt32(dParams.Width), Convert.ToInt32(dParams.Height), PixelFormat.Format32bppArgb);
            Draw(Graphics.FromImage(bitmap), dParams, init: false);
            bitmap.Save(file, ImageFormat.Png);
        }

        public void Draw(Graphics g, DrawingParams dParams, bool init = true)
        {
            if (init) InitDrawingParams(dParams);
            foreach (var path in Paths) DrawPath(g, path, dParams);
            foreach (var cp in ControlPoints) DrawControlPoint(g, cp, dParams);
        }

        private void DrawControlPoint(Graphics g, ControlPoint cp, DrawingParams dParams)
        {
            var pos = cp.Positions.First();
            var coords = PathCoordinates[pos.Key];
            DenseVector start = new double[] { coords[0], coords[1] };
            DenseVector end = new double[] { coords[2], coords[3] };
            DenseVector coord = new double[] {
                coords[0] + (coords[2]- coords[0])/ pos.Key.Length * pos.Value,
                coords[1] + (coords[3]- coords[1])/ pos.Key.Length * pos.Value
            };
            var tail = Slip(coord, coord + (end - start), dParams.ControlPointSize / 2);

            var pen = new Pen(dParams.ControlPointColor, dParams.ControlPointThickness);
            g.DrawLine(pen, dParams.GetPoint(Rotate(tail, coord, Math.PI / 4)), dParams.GetPoint(Rotate(tail, coord, -3 * Math.PI / 4)));
            g.DrawLine(pen, dParams.GetPoint(Rotate(tail, coord, -Math.PI / 4)), dParams.GetPoint(Rotate(tail, coord, 3 * Math.PI / 4)));
        }

        private void DrawPath(Graphics g, Path path, DrawingParams dParams)
        {
            if (!PathCoordinates.ContainsKey(path))
                throw new Exception(string.Format("Coordinates for {0} are not specified.", path.Id));
            var coords = PathCoordinates[path];
            DenseVector start = new double[] { coords[0], coords[1] };
            DenseVector end = new double[] { coords[2], coords[3] };
            var mid = (start + end) / 2;

            var pen = new Pen(dParams.PathColor, dParams.PathThickness);
            g.DrawLine(pen, dParams.GetPoint(start), dParams.GetPoint(end));
            // draw arrows on path
            DenseVector vetex, tail;
            if (path.Direction == Direction.TwoWay || path.Direction == Direction.Forward)
            {
                vetex = Slip(mid, start, (end - start).L2Norm() * 0.1);
                tail = Slip(vetex, start, dParams.ArrowSize);
                g.DrawLine(pen, dParams.GetPoint(vetex), dParams.GetPoint(Rotate(tail, vetex, dParams.ArrowAngle / 2)));
                g.DrawLine(pen, dParams.GetPoint(vetex), dParams.GetPoint(Rotate(tail, vetex, -dParams.ArrowAngle / 2)));
            }
            if (path.Direction == Direction.TwoWay || path.Direction == Direction.Backward)
            {
                vetex = Slip(mid, end, (end - start).L2Norm() * 0.1);
                tail = Slip(vetex, end, dParams.ArrowSize);
                g.DrawLine(pen, dParams.GetPoint(vetex), dParams.GetPoint(Rotate(tail, vetex, dParams.ArrowAngle / 2)));
                g.DrawLine(pen, dParams.GetPoint(vetex), dParams.GetPoint(Rotate(tail, vetex, -dParams.ArrowAngle / 2)));
            }
        }
        
        private DenseVector Rotate(DenseVector point, DenseVector centre, double theta)
        {
            return DenseMatrix.OfRowArrays(new double[][] {
                new double[] {Math.Cos(theta), - Math.Sin(theta) },
                new double[] {Math.Sin(theta), Math.Cos(theta) }
            }) * DenseMatrix.OfColumnVectors(new DenseVector[] { point - centre }).ToColumnArrays()[0]
            + centre;
        }

        private DenseVector Slip(DenseVector point, DenseVector towards, double distance)
        {
            return point + (towards - point) * distance / (towards - point).L2Norm();
        }

        #endregion
    }
}
