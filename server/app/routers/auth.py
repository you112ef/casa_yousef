from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from app.db import get_db
from app import models
from app.schemas import LoginRequest, TokenResponse, UserOut
from app.core.security import verify_password, create_access_token
from app.deps import get_current_user

router = APIRouter(prefix="/auth", tags=["auth"])

@router.post("/login", response_model=TokenResponse)
def login(req: LoginRequest, db: Session = Depends(get_db)):
    user = db.query(models.User).filter(models.User.username == req.username).first()
    if not user or not verify_password(req.password, user.password_hash):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    token = create_access_token(user.username)
    return TokenResponse(token=token, user=UserOut.model_validate(user))

@router.get("/me", response_model=UserOut)
def me(current_user: models.User = Depends(get_current_user)):
    return UserOut.model_validate(current_user)
