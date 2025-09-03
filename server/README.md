Sky CASA Backend (FastAPI)

Endpoints (prefixed with /api/v1):
- POST /auth/login
- GET /auth/me
- CRUD /patients
- GET/POST /cbc-results
- POST /ai/sperm/analyze-image
- POST /ai/sperm/analyze-video

Quick start:
1) python -m venv .venv && source .venv/bin/activate
2) pip install -r requirements.txt
3) cp .env.example .env  # adjust values if needed
4) uvicorn app.main:app --reload --host 0.0.0.0 --port 8000

Environment (.env):
- DATABASE_URL=sqlite:///./sky_casa_web.db
- JWT_SECRET=change-me
- JWT_EXPIRE_MINUTES=4320
- CORS_ORIGINS=http://localhost:5173,http://127.0.0.1:5173
- YOLO_MODEL_PATH=./ai_sperm_onnx/models/yolov5s.onnx

Notes:
- On first run, admin user is seeded: username=admin, password=admin123 (change after login).
- AI endpoints require packages in requirements-ai.txt; if unavailable, they still accept files and return a minimal result structure.
