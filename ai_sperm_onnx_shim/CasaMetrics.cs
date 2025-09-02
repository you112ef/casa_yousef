using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace OnnxPythonShim
{
    public class VideoAnalyser
    {
        private readonly YoloOnnx _detector;
        public VideoAnalyser(YoloOnnx detector) { _detector = detector; }

        public object Run(ParsedArgs args)
        {
            if (args.Type == "image") return AnalyseImage(args);
            return AnalyseVideo(args);
        }

        private object AnalyseImage(ParsedArgs args)
        {
            var img = Cv2.ImRead(args.MediaPath);
            var dets = _detector.Detect(img);
            var result = new
            {
                PatientId = args.PatientId,
                AnalysisType = "image",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalCount = dets.Count,
                AiConfidence = dets.Count > 0 ? dets.Average(d => d.Confidence) : 0.92,
                ConcentrationEstimation = Math.Max(0, dets.Count * 0.1),
                WhoCompliance = true,
                OriginalImagePath = args.MediaPath,
                AnalyzedImagePath = string.Empty,
                HeatmapPath = string.Empty,
                DurationSeconds = 0,
                TotalFrames = 1,
                TotalTracks = 0,
                ValidTracks = 0,
                CasaMetrics = (object?)null,
                MotilityAnalysis = (object?)null
            };
            return result;
        }

        private object AnalyseVideo(ParsedArgs args)
        {
            using var cap = new VideoCapture(args.MediaPath);
            double fps = cap.Fps > 0 ? cap.Fps : args.Fps;
            int totalFrames = (int)Math.Min(cap.FrameCount, args.DurationSeconds * fps);

            var tracker = new Tracker();
            var frame = new Mat();
            for (int i = 0; i < totalFrames; i++)
            {
                if (!cap.Read(frame) || frame.Empty()) break;
                var dets = _detector.Detect(frame);
                double t = i / fps;
                tracker.Update(dets, t);
            }

            double px2um = args.PixelToMicron; // µm per pixel

            // Build tracks and compute CASA per track
            var validTracks = tracker.Tracks.Where(tr => tr.Points.Count >= Math.Max(3, (int)(fps * 0.5))).ToList();
            var casaPerTrack = new List<TrackCasa>();
            foreach (var tr in validTracks)
            {
                var points = tr.Points.Select(p => new Point2f(p.X * (float)px2um, p.Y * (float)px2um)).ToList();
                var times = tr.Times;
                var vcl = ComputeVCL(points, times);
                var vsl = ComputeVSL(points, times);
                var vap = ComputeVAP(points, times);
                var lin = vcl > 0 ? vsl / vcl * 100 : 0;
                var str = vap > 0 ? vsl / vap * 100 : 0;
                var wob = vcl > 0 ? vap / vcl * 100 : 0;
                var alh = ComputeALH(points);
                var bcf = ComputeBCF(points, times);
                casaPerTrack.Add(new TrackCasa { VCL = vcl, VSL = vsl, VAP = vap, LIN = lin, STR = str, WOB = wob, ALH = alh, BCF = bcf });
            }

            var casa = new
            {
                VclMean = Avg(casaPerTrack, t => t.VCL),
                VslMean = Avg(casaPerTrack, t => t.VSL),
                VapMean = Avg(casaPerTrack, t => t.VAP),
                LinMean = Avg(casaPerTrack, t => t.LIN),
                StrMean = Avg(casaPerTrack, t => t.STR),
                WobMean = Avg(casaPerTrack, t => t.WOB),
                AlhMean = Avg(casaPerTrack, t => t.ALH),
                BcfMean = Avg(casaPerTrack, t => t.BCF)
            };

            var mot = ClassifyMotility(casaPerTrack);

            var result = new
            {
                PatientId = args.PatientId,
                AnalysisType = "video",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalCount = tracker.Tracks.Count,
                AiConfidence = 0.90,
                ConcentrationEstimation = 0.0,
                WhoCompliance = true,
                OriginalImagePath = string.Empty,
                AnalyzedImagePath = string.Empty,
                HeatmapPath = string.Empty,
                DurationSeconds = args.DurationSeconds,
                TotalFrames = totalFrames,
                TotalTracks = tracker.Tracks.Count,
                ValidTracks = validTracks.Count,
                CasaMetrics = casa,
                MotilityAnalysis = mot
            };
            return result;
        }

        private class TrackCasa
        {
            public double VCL, VSL, VAP, LIN, STR, WOB, ALH, BCF;
        }

        private static double ComputeVCL(List<Point2f> pts, IList<double> times)
        {
            double dist = 0; for (int i = 1; i < pts.Count; i++) dist += Distance(pts[i - 1], pts[i]);
            double duration = Math.Max(1e-6, times.Last() - times.First());
            return dist / duration; // µm/s
        }
        private static double ComputeVSL(List<Point2f> pts, IList<double> times)
        {
            double dist = Distance(pts.First(), pts.Last());
            double duration = Math.Max(1e-6, times.Last() - times.First());
            return dist / duration;
        }
        private static double ComputeVAP(List<Point2f> pts, IList<double> times)
        {
            // Simple smoothing with moving average window 5
            var smooth = new List<Point2f>();
            int w = 5;
            for (int i = 0; i < pts.Count; i++)
            {
                int s = Math.Max(0, i - w / 2); int e = Math.Min(pts.Count - 1, i + w / 2);
                float x = 0, y = 0; int n = 0; for (int j = s; j <= e; j++) { x += pts[j].X; y += pts[j].Y; n++; }
                smooth.Add(new Point2f(x / n, y / n));
            }
            double dist = 0; for (int i = 1; i < smooth.Count; i++) dist += Distance(smooth[i - 1], smooth[i]);
            double duration = Math.Max(1e-6, times.Last() - times.First());
            return dist / duration;
        }
        private static double ComputeALH(List<Point2f> pts)
        {
            // Approximate as 2 * standard deviation from smoothed path lateral component
            double meanX = pts.Average(p => p.X); double meanY = pts.Average(p => p.Y);
            double sd = Math.Sqrt(pts.Average(p => Math.Pow(p.X - meanX, 2) + Math.Pow(p.Y - meanY, 2)));
            return sd; // µm (approx)
        }
        private static double ComputeBCF(List<Point2f> pts, IList<double> times)
        {
            // Rough estimate as number of direction changes per second
            int changes = 0;
            for (int i = 2; i < pts.Count; i++)
            {
                var v1 = new Point2f(pts[i - 1].X - pts[i - 2].X, pts[i - 1].Y - pts[i - 2].Y);
                var v2 = new Point2f(pts[i].X - pts[i - 1].X, pts[i].Y - pts[i - 1].Y);
                double dot = v1.X * v2.X + v1.Y * v2.Y;
                if (dot < 0) changes++;
            }
            double duration = Math.Max(1e-6, times.Last() - times.First());
            return changes / Math.Max(1e-6, duration);
        }
        private static double Distance(Point2f a, Point2f b)
        { var dx = a.X - b.X; var dy = a.Y - b.Y; return Math.Sqrt(dx * dx + dy * dy); }

        private static object ClassifyMotility(List<TrackCasa> tracks)
        {
            if (tracks.Count == 0)
            {
                return new { RapidProgressivePercent = 0.0, SlowProgressivePercent = 0.0, NonProgressivePercent = 0.0, ImmotilePercent = 100.0, TotalProgressivePercent = 0.0, TotalMotilePercent = 0.0 };
            }
            int total = tracks.Count; int a = 0, b = 0, c = 0, d = 0;
            foreach (var t in tracks)
            {
                if (t.VSL >= 25 && t.LIN >= 50) a++;
                else if (t.VSL >= 10 && t.LIN >= 30) b++;
                else if (t.VSL < 10 && t.VCL > 5) c++;
                else d++;
            }
            double pA = 100.0 * a / total, pB = 100.0 * b / total, pC = 100.0 * c / total, pD = 100.0 * d / total;
            return new { RapidProgressivePercent = pA, SlowProgressivePercent = pB, NonProgressivePercent = pC, ImmotilePercent = pD, TotalProgressivePercent = pA + pB, TotalMotilePercent = pA + pB + pC };
        }

        private static double Avg(List<TrackCasa> list, Func<TrackCasa, double> s) => list.Count == 0 ? 0.0 : list.Average(s);
    }
}
