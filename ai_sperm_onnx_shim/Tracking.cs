using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace OnnxPythonShim
{
    public class Track
    {
        public int Id { get; set; }
        public List<Point2f> Points { get; } = new();
        public List<double> Times { get; } = new();
        public Rect LastRect { get; set; }
        public double LastTime { get; set; }
        public bool Active { get; set; } = true;
    }

    public class Tracker
    {
        private int _nextId = 1;
        private readonly List<Track> _tracks = new();

        public IReadOnlyList<Track> Tracks => _tracks;

        public void Update(IEnumerable<Detection> detections, double time, double iouThreshold = 0.3)
        {
            var dets = detections.ToList();
            var assigned = new HashSet<int>();

            // Try to match existing tracks by IoU
            foreach (var tr in _tracks.Where(t => t.Active))
            {
                double bestIoU = 0; int bestIdx = -1;
                for (int i = 0; i < dets.Count; i++)
                {
                    if (assigned.Contains(i)) continue;
                    double iou = IoU(tr.LastRect, dets[i].Rect);
                    if (iou > bestIoU)
                    {
                        bestIoU = iou; bestIdx = i;
                    }
                }
                if (bestIdx >= 0 && bestIoU >= iouThreshold)
                {
                    var d = dets[bestIdx];
                    tr.LastRect = d.Rect;
                    var center = new Point2f(d.Rect.X + d.Rect.Width / 2f, d.Rect.Y + d.Rect.Height / 2f);
                    tr.Points.Add(center);
                    tr.Times.Add(time);
                    tr.LastTime = time;
                    assigned.Add(bestIdx);
                }
                else
                {
                    tr.Active = false;
                }
            }

            // Create new tracks for unassigned detections
            for (int i = 0; i < dets.Count; i++)
            {
                if (assigned.Contains(i)) continue;
                var d = dets[i];
                var tr = new Track
                {
                    Id = _nextId++,
                    LastRect = d.Rect,
                    LastTime = time,
                    Active = true
                };
                var center = new Point2f(d.Rect.X + d.Rect.Width / 2f, d.Rect.Y + d.Rect.Height / 2f);
                tr.Points.Add(center);
                tr.Times.Add(time);
                _tracks.Add(tr);
            }
        }

        private static double IoU(Rect a, Rect b)
        {
            var inter = a & b; double interArea = inter.Width * inter.Height;
            double union = a.Width * a.Height + b.Width * b.Height - interArea;
            return union <= 0 ? 0 : interArea / union;
        }
    }
}
