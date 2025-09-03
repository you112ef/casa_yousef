Sky CASA Web: Frontend + Backend

Frontend
- Path: web-frontend/
- Stack: React 19 + TypeScript + Tailwind V4 + shadcn/ui
- Commands:
  - bun install
  - bun dev
  - bun build
- Env: copy web-frontend/.env.example to .env and set VITE_API_BASE_URL to backend URL (default http://localhost:8000/api/v1)

Backend
- Path: server/
- Stack: FastAPI + SQLite + JWT
- Commands:
  - python -m venv .venv && source .venv/bin/activate
  - pip install -r requirements.txt
  - cp .env.example .env (edit if needed)
  - uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
- Notes: seeds admin user admin/admin123 on first run; change password after login.

Integration
- Login page: /login -> POST /auth/login -> stores JWT and loads app
- Topbar navigation to Dashboard, Data View, Analysis, AI, WHO Report, Settings
- Data View hits /patients and /cbc-results with pagination params
- AI uploads hit /ai/sperm/analyze-image and /ai/sperm/analyze-video; install AI extras in server/requirements.txt for full functionality

Design
- Teal topbar, RTL/LTR toggle, responsive layouts with no overlap for all breakpoints.
