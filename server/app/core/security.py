from datetime import datetime, timedelta, timezone
from jose import jwt
from passlib.context import CryptContext
from app.core.config import settings

# Use pbkdf2_sha256 to avoid binary bcrypt dependency on some platforms
pwd_context = CryptContext(schemes=["pbkdf2_sha256"], deprecated="auto")

ALGORITHM = "HS256"


def create_access_token(subject: str, expires_minutes: int = settings.JWT_EXPIRE_MINUTES) -> str:
    expire = datetime.now(timezone.utc) + timedelta(minutes=expires_minutes)
    to_encode = {"sub": subject, "exp": expire}
    return jwt.encode(to_encode, settings.JWT_SECRET, algorithm=ALGORITHM)


def verify_password(plain: str, hashed: str) -> bool:
    return pwd_context.verify(plain, hashed)


def hash_password(plain: str) -> str:
    return pwd_context.hash(plain)
