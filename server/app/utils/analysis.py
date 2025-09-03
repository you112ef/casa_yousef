from typing import Any, Dict, List, Tuple
import os

class SpermAnalyzerService:
    def __init__(self, model_path: str | None = None) -> None:
        self.model_path = model_path
        self._ort_session = None
        # Try ONNXRuntime first (lightweight for hosting)
        try:
            import onnxruntime as ort  # type: ignore
            if model_path and os.path.exists(model_path):
                self._ort_session = ort.InferenceSession(model_path, providers=["CPUExecutionProvider"])  # type: ignore
        except Exception:
            self._ort_session = None

        # Fallback to ultralytics if available
        self._yolo = None
        if self._ort_session is None:
            try:
                from ultralytics import YOLO  # type: ignore
                self._yolo = YOLO(model_path) if model_path else YOLO("yolov8n.pt")
            except Exception:
                self._yolo = None

    def _preprocess(self, path: str):
        import cv2
        import numpy as np
        img = cv2.imread(path)
        if img is None:
            raise RuntimeError("Failed to read image")
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        # Resize to 640x640 without letterbox (simple)
        img_resized = cv2.resize(img, (640, 640))
        x = img_resized.astype("float32") / 255.0
        x = x.transpose(2, 0, 1)[None, ...]
        return img, img_resized, x

    def _nms(self, boxes: List[Tuple[float, float, float, float]], scores: List[float], iou_thresh: float = 0.45):
        # Simple NMS
        import numpy as np
        if not boxes:
            return []
        boxes_np = np.array(boxes)
        scores_np = np.array(scores)
        x1 = boxes_np[:, 0]
        y1 = boxes_np[:, 1]
        x2 = boxes_np[:, 2]
        y2 = boxes_np[:, 3]
        areas = (x2 - x1 + 1) * (y2 - y1 + 1)
        order = scores_np.argsort()[::-1]
        keep = []
        while order.size > 0:
            i = order[0]
            keep.append(i)
            xx1 = np.maximum(x1[i], x1[order[1:]])
            yy1 = np.maximum(y1[i], y1[order[1:]])
            xx2 = np.minimum(x2[i], x2[order[1:]])
            yy2 = np.minimum(y2[i], y2[order[1:]])
            w = np.maximum(0.0, xx2 - xx1 + 1)
            h = np.maximum(0.0, yy2 - yy1 + 1)
            inter = w * h
            iou = inter / (areas[i] + areas[order[1:]] - inter + 1e-6)
            inds = np.where(iou <= iou_thresh)[0]
            order = order[inds + 1]
        return keep

    def analyze_image(self, file_path: str) -> Dict[str, Any]:
        # ONNXRuntime path
        if self._ort_session is not None:
            import numpy as np
            import cv2
            orig, resized, x = self._preprocess(file_path)
            outputs = self._ort_session.run(None, {self._ort_session.get_inputs()[0].name: x})
            out = outputs[0]  # [1, N, 85] for yolov5
            preds = out[0]
            boxes = []
            scores = []
            for det in preds:
                cx, cy, w, h, obj = det[:5]
                cls_scores = det[5:]
                cls = int(np.argmax(cls_scores))
                score = float(obj * cls_scores[cls])
                if score < 0.25:
                    continue
                x1 = float(cx - w / 2)
                y1 = float(cy - h / 2)
                x2 = float(cx + w / 2)
                y2 = float(cy + h / 2)
                boxes.append((x1, y1, x2, y2))
                scores.append(score)
            keep = self._nms(boxes, scores)
            kept = [
                {
                    "box": [boxes[i][0], boxes[i][1], boxes[i][2], boxes[i][3]],
                    "score": scores[i],
                }
                for i in keep
            ]
            return {"status": "ok", "detections": kept, "engine": "onnxruntime"}

        # Ultralytics fallback
        if self._yolo is not None:
            results = self._yolo(file_path)
            dets = []
            for r in results:
                b = r.boxes.xyxy.tolist() if getattr(r, "boxes", None) is not None else []
                dets.append({"boxes": b, "num": len(b)})
            return {"status": "ok", "results": dets, "engine": "ultralytics"}

        # Minimal fallback
        return {"status": "ok", "detections": [], "note": "AI engine unavailable"}

    def analyze_video(self, file_path: str, duration_sec: int | None = None) -> Dict[str, Any]:
        # TODO: implement frame sampling + detection
        return {"status": "ok", "message": "video analysis basic", "durationSec": duration_sec}
