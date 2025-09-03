from fastapi import FastAPI, Depends, Request
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy.orm import Session
from app.core.config import settings
from app.db import Base, engine, get_db
from app import models
from app.core.security import hash_password
from app.routers import auth as auth_router, patients as patients_router, cbc as cbc_router, ai as ai_router
from app.deps import get_current_user

Base.metadata.create_all(bind=engine)

app = FastAPI(title="Sky CASA API", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.cors_origins_list,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Seed admin user
@app.on_event("startup")
def seed_admin():
    db = next(get_db())
    if not db.query(models.User).filter(models.User.username == "admin").first():
        u = models.User(username="admin", password_hash=hash_password("admin123"), role="admin", name="Administrator")
        db.add(u)
        db.commit()
        print("Seeded default admin user: admin/admin123")

# Inject current user dependency for /auth/me
app.dependency_overrides[get_current_user] = lambda authorization=Depends(): __get_current_user(authorization)

from app.deps import get_current_user as __get_current_user  # noqa: E402

@app.get("/api/v1/health")
def health():
    return {"status": "healthy"}

api = FastAPI()
api.include_router(auth_router.router)
api.include_router(patients_router.router)
api.include_router(cbc_router.router)
api.include_router(ai_router.router)

app.mount("/api/v1", api)
