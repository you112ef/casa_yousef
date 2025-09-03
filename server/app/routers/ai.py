from fastapi import APIRouter, UploadFile, File, Form
from fastapi.responses import JSONResponse
import tempfile, os
from app.core.config import settings
from app.utils.analysis import SpermAnalyzerService

router = APIRouter(prefix="/ai/sperm", tags=["ai"])

analyzer = SpermAnalyzerService(settings.YOLO_MODEL_PATH)

@router.post("/analyze-image")
async def analyze_image(file: UploadFile = File(...)):
    with tempfile.NamedTemporaryFile(delete=False) as tmp:
        content = await file.read()
        tmp.write(content)
        tmp_path = tmp.name
    try:
        try:
            res = analyzer.analyze_image(tmp_path)
        except Exception as e:
            res = {"status": "error", "note": "AI engine failed", "error": str(e)}
        return JSONResponse(res)
    finally:
        try:
            os.remove(tmp_path)
        except Exception:
            pass

@router.post("/analyze-video")
async def analyze_video(file: UploadFile = File(...), durationSec: int | None = Form(None)):
    with tempfile.NamedTemporaryFile(delete=False) as tmp:
        content = await file.read()
        tmp.write(content)
        tmp_path = tmp.name
    try:
        try:
            res = analyzer.analyze_video(tmp_path, durationSec)
        except Exception as e:
            res = {"status": "error", "note": "AI engine failed", "error": str(e)}
        return JSONResponse(res)
    finally:
        try:
            os.remove(tmp_path)
        except Exception:
            pass
