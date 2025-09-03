from pydantic_settings import BaseSettings
from typing import List

class Settings(BaseSettings):
    DATABASE_URL: str = "sqlite:///./sky_casa_web.db"
    JWT_SECRET: str = "change-me"
    JWT_EXPIRE_MINUTES: int = 4320
    CORS_ORIGINS: str = "http://localhost:5173,http://127.0.0.1:5173"
    YOLO_MODEL_PATH: str = "./ai_sperm_onnx/models/yolov5s.onnx"

    @property
    def cors_origins_list(self) -> List[str]:
        return [o.strip() for o in self.CORS_ORIGINS.split(",") if o.strip()]

settings = Settings(_env_file=".env", _env_file_encoding="utf-8")
