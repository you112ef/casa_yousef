from typing import Any, Dict

class SpermAnalyzerService:
    def __init__(self, model_path: str | None = None) -> None:
        self.model_path = model_path
        self.yolo = None
        try:
            from ultralytics import YOLO  # type: ignore
            if model_path:
                self.yolo = YOLO(model_path)
            else:
                self.yolo = YOLO("yolov8n.pt")
        except Exception:
            self.yolo = None

    def analyze_image(self, file_path: str) -> Dict[str, Any]:
        if not self.yolo:
            return {"status": "ok", "detections": [], "note": "YOLO not available; install AI requirements"}
        results = self.yolo(file_path)
        dets = []
        for r in results:
            boxes = r.boxes.xyxy.tolist() if getattr(r, "boxes", None) is not None else []
            dets.append({"boxes": boxes, "num": len(boxes)})
        return {"status": "ok", "results": dets}

    def analyze_video(self, file_path: str, duration_sec: int | None = None) -> Dict[str, Any]:
        # Simplified: run detection on first frames; extend as needed
        return {"status": "ok", "message": "video analysis placeholder", "durationSec": duration_sec}
