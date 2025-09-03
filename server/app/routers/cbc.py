from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from typing import Optional
from app.db import get_db
from app import models
from app.schemas import CBCResultsResponse, CBCResultOut, CBCResultCreate
from app.crud import list_cbc, create_cbc

router = APIRouter(prefix="/cbc-results", tags=["cbc"])

@router.get("/", response_model=CBCResultsResponse)
def get_cbc(patientId: Optional[int] = None, limit: int = 10, offset: int = 0, db: Session = Depends(get_db)):
    items, total = list_cbc(db, limit=limit, offset=offset, patient_id=patientId)
    return {"results": [CBCResultOut.model_validate(i) for i in items], "total": total, "limit": limit, "offset": offset}

@router.get("/{id}", response_model=CBCResultOut)
def get_cbc_by_id(id: int, db: Session = Depends(get_db)):
    r = db.query(models.CBCResult).get(id)
    if not r:
        from fastapi import HTTPException
        raise HTTPException(status_code=404, detail="CBC result not found")
    return CBCResultOut.model_validate(r)

@router.post("/", response_model=CBCResultOut)
def create_cbc_result(req: CBCResultCreate, db: Session = Depends(get_db)):
    r = create_cbc(db, **req.model_dump())
    return CBCResultOut.model_validate(r)
