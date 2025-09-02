using System;
using System.Collections.Generic;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace OnnxPythonShim
{
    public record Detection(Rect Rect, float Confidence, int ClassId);

    public class YoloOnnx : IDisposable
    {
        private readonly InferenceSession _session;
        private readonly int _inputW = 640;
        private readonly int _inputH = 640;

        public YoloOnnx(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public List<Detection> Detect(Mat image, float confThreshold = 0.3f, float iouThreshold = 0.45f)
        {
            int w = image.Width, h = image.Height;
            var blob = CvDnn.BlobFromImage(image, 1 / 255.0, new OpenCvSharp.Size(_inputW, _inputH), new Scalar(), true, false);
            var input = new DenseTensor<float>(blob.ToMat().ToRectangularArray(), new[] { 1, 3, _inputH, _inputW });

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", input) };
            using var results = _session.Run(inputs);
            var output = results[0].AsEnumerable<float>();
            var arr = new List<float>(output);

            // Simple placeholder decoding for YOLOv5s exported shape (1,25200,85). We use a very naive approach for demo.
            int stride = 85; int num = arr.Count / stride;
            var detections = new List<Detection>();
            for (int i = 0; i < num; i++)
            {
                int offset = i * stride;
                float cx = arr[offset + 0];
                float cy = arr[offset + 1];
                float ow = arr[offset + 2];
                float oh = arr[offset + 3];
                float obj = arr[offset + 4];
                if (obj < confThreshold) continue;
                int bestId = 0; float best = 0f;
                for (int c = 5; c < 85; c++)
                {
                    float score = arr[offset + c] * obj;
                    if (score > best) { best = score; bestId = c - 5; }
                }
                if (best < confThreshold) continue;
                int x = (int)((cx - ow / 2) / _inputW * w);
                int y = (int)((cy - oh / 2) / _inputH * h);
                int rw = (int)(ow / _inputW * w);
                int rh = (int)(oh / _inputH * h);
                var rect = new Rect(Math.Max(0, x), Math.Max(0, y), Math.Min(w - x, rw), Math.Min(h - y, rh));
                detections.Add(new Detection(rect, best, bestId));
            }

            // NMS (very simple)
            detections.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
            var final = new List<Detection>();
            foreach (var det in detections)
            {
                bool keep = true;
                foreach (var f in final)
                {
                    if (IoU(det.Rect, f.Rect) > iouThreshold) { keep = false; break; }
                }
                if (keep) final.Add(det);
            }
            return final;
        }

        private static double IoU(Rect a, Rect b)
        {
            var inter = a & b; double interArea = inter.Width * inter.Height;
            double union = a.Width * a.Height + b.Width * b.Height - interArea;
            return union <= 0 ? 0 : interArea / union;
        }

        public void Dispose() => _session.Dispose();
    }
}
