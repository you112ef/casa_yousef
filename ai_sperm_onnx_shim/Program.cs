using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace OnnxPythonShim
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var parsed = ArgParser.Parse(args);
                if (!parsed.IsValid)
                {
                    Console.Error.WriteLine(parsed.ErrorMessage);
                    return 1;
                }

                string appDir = AppContext.BaseDirectory;
                string modelPath = Path.Combine(appDir, "..", "ai_sperm_onnx", "models", "yolov5s.onnx");
                if (!File.Exists(modelPath))
                {
                    // Fallback to relative if running from published folder next to model
                    string tryAlt = Path.Combine(appDir, "ai_sperm_onnx", "models", "yolov5s.onnx");
                    if (File.Exists(tryAlt)) modelPath = tryAlt;
                }

                using var detector = new YoloOnnx(modelPath);
                var analyser = new VideoAnalyser(detector);
                var result = analyser.Run(parsed);

                var json = JsonConvert.SerializeObject(result, Formatting.None);
                Console.WriteLine(json);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Shim error: {ex.Message}");
                return 2;
            }
        }
    }

    public class ParsedArgs
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Type { get; set; } = "image"; // image|video
        public string MediaPath { get; set; } = string.Empty;
        public int PatientId { get; set; } = 1;
        public int DurationSeconds { get; set; } = 15;
        public double Fps { get; set; } = 30;
        public double PixelToMicron { get; set; } = 0.5; // µm per pixel
    }

    public static class ArgParser
    {
        public static ParsedArgs Parse(string[] args)
        {
            // This shim is invoked as: python <script> --type video --media path --patient 1 --duration 15
            var p = new ParsedArgs();
            try
            {
                var list = new List<string>(args);
                // Skip the first token if it is a python script path
                int idx = 0;
                if (list.Count > 0 && list[0].EndsWith(".py", StringComparison.OrdinalIgnoreCase)) idx = 1;
                for (int i = idx; i < list.Count; i++)
                {
                    var a = list[i];
                    if (a == "--type" && i + 1 < list.Count) { p.Type = list[++i]; }
                    else if (a == "--media" && i + 1 < list.Count) { p.MediaPath = list[++i]; }
                    else if (a == "--patient" && i + 1 < list.Count) { int.TryParse(list[++i], out var id); p.PatientId = id; }
                    else if (a == "--duration" && i + 1 < list.Count) { int.TryParse(list[++i], out var d); p.DurationSeconds = d; }
                    else if (a == "--fps" && i + 1 < list.Count) { double.TryParse(list[++i], out var f); p.Fps = f; }
                    else if (a == "--px2um" && i + 1 < list.Count) { double.TryParse(list[++i], out var v); p.PixelToMicron = v; }
                }
                if (string.IsNullOrWhiteSpace(p.MediaPath) || !File.Exists(p.MediaPath))
                {
                    p.IsValid = false; p.ErrorMessage = "Invalid media path"; return p;
                }
                p.IsValid = true; return p;
            }
            catch (Exception ex)
            {
                return new ParsedArgs { IsValid = false, ErrorMessage = ex.Message };
            }
        }
    }
}
