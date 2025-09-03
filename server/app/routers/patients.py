from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from typing import Optional
from app.db import get_db
from app import models
from app.schemas import PatientsResponse, PatientOut, PatientCreate
from app.crud import list_patients, create_patient

router = APIRouter(prefix="/patients", tags=["patients"])

@router.get("/", response_model=PatientsResponse)
def get_patients(limit: int = 10, offset: int = 0, search: Optional[str] = None, db: Session = Depends(get_db)):
    items, total = list_patients(db, limit=limit, offset=offset, search=search)
    return {"patients": [PatientOut.model_validate(i) for i in items], "total": total, "limit": limit, "offset": offset}

@router.post("/", response_model=PatientOut)
def create_patients(req: PatientCreate, db: Session = Depends(get_db)):
    p = create_patient(db, **req.model_dump())
    return PatientOut.model_validate(p)

@router.get("/{id}", response_model=PatientOut)
def get_patient(id: int, db: Session = Depends(get_db)):
    p = db.query(models.Patient).get(id)
    if not p:
        from fastapi import HTTPException
        raise HTTPException(status_code=404, detail="Patient not found")
    return PatientOut.model_validate(p)
