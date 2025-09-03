from sqlalchemy.orm import Session
from sqlalchemy import func
from app import models
from app.core.security import hash_password

# Users

def get_user_by_username(db: Session, username: str):
    return db.query(models.User).filter(models.User.username == username).first()


def create_user(db: Session, username: str, password: str, **kwargs):
    user = models.User(username=username, password_hash=hash_password(password), **kwargs)
    db.add(user)
    db.commit()
    db.refresh(user)
    return user

# Patients

def list_patients(db: Session, limit: int = 10, offset: int = 0, search: str | None = None):
    q = db.query(models.Patient)
    if search:
        like = f"%{search}%"
        q = q.filter((models.Patient.first_name.ilike(like)) | (models.Patient.last_name.ilike(like)))
    total = q.count()
    items = q.order_by(models.Patient.id.desc()).offset(offset).limit(limit).all()
    return items, total


def create_patient(db: Session, **data):
    p = models.Patient(
        first_name=data.get("firstName"),
        last_name=data.get("lastName"),
        date_of_birth=data.get("dateOfBirth"),
        gender=data.get("gender"),
        phone_number=data.get("phoneNumber"),
        email=data.get("email"),
        address=data.get("address"),
    )
    db.add(p)
    db.commit()
    db.refresh(p)
    return p

# CBC

def list_cbc(db: Session, limit: int = 10, offset: int = 0, patient_id: int | None = None):
    q = db.query(models.CBCResult)
    if patient_id:
        q = q.filter(models.CBCResult.patient_id == patient_id)
    total = q.count()
    items = q.order_by(models.CBCResult.id.desc()).offset(offset).limit(limit).all()
    return items, total


def create_cbc(db: Session, **data):
    r = models.CBCResult(
        patient_id=data.get("patientId"),
        technician_id=data.get("technicianId"),
        wbc=data.get("wbc"),
        rbc=data.get("rbc"),
        hemoglobin=data.get("hemoglobin"),
        hematocrit=data.get("hematocrit"),
        platelet_count=data.get("plateletCount"),
    )
    db.add(r)
    db.commit()
    db.refresh(r)
    return r
