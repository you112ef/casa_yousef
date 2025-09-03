from sqlalchemy import Column, Integer, String, DateTime, Float, Boolean, ForeignKey
from sqlalchemy.orm import relationship
from datetime import datetime
from app.db import Base

class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True)
    password_hash = Column(String)
    name = Column(String, nullable=True)
    email = Column(String, nullable=True)
    role = Column(String, default="admin")
    created_at = Column(DateTime, default=datetime.utcnow)

class Patient(Base):
    __tablename__ = "patients"
    id = Column(Integer, primary_key=True, index=True)
    first_name = Column(String)
    last_name = Column(String)
    date_of_birth = Column(String)
    gender = Column(String)
    phone_number = Column(String)
    email = Column(String)
    address = Column(String)
    created_at = Column(DateTime, default=datetime.utcnow)

class CBCResult(Base):
    __tablename__ = "cbc_results"
    id = Column(Integer, primary_key=True, index=True)
    patient_id = Column(Integer, ForeignKey("patients.id"))
    technician_id = Column(Integer, nullable=True)
    test_date = Column(DateTime, default=datetime.utcnow)
    wbc = Column(Float, nullable=True)
    rbc = Column(Float, nullable=True)
    hemoglobin = Column(Float, nullable=True)
    hematocrit = Column(Float, nullable=True)
    platelet_count = Column(Float, nullable=True)
    mcv = Column(Float, nullable=True)
    mch = Column(Float, nullable=True)
    mchc = Column(Float, nullable=True)
    rdw = Column(Float, nullable=True)
    mpv = Column(Float, nullable=True)
    pdw = Column(Float, nullable=True)
    neutrophils_percent = Column(Float, nullable=True)
    lymphocytes_percent = Column(Float, nullable=True)
    monocytes_percent = Column(Float, nullable=True)
    eosinophils_percent = Column(Float, nullable=True)
    basophils_percent = Column(Float, nullable=True)
    bands_percent = Column(Float, nullable=True)
    atypical_lymphocytes_percent = Column(Float, nullable=True)
    nrbc = Column(Float, nullable=True)
    neutrophils_absolute = Column(Float, nullable=True)
    lymphocytes_absolute = Column(Float, nullable=True)
    monocytes_absolute = Column(Float, nullable=True)
    eosinophils_absolute = Column(Float, nullable=True)
    basophils_absolute = Column(Float, nullable=True)
    bands_absolute = Column(Float, nullable=True)
    wbc_interpretation = Column(String, nullable=True)
    rbc_interpretation = Column(String, nullable=True)
    hgb_interpretation = Column(String, nullable=True)
    plt_interpretation = Column(String, nullable=True)
    differential_interpretation = Column(String, nullable=True)
    critical_value_flag = Column(Boolean, default=False)
    comments = Column(String, nullable=True)
    qc_status = Column(String, default="PENDING")
    reviewed_by = Column(Integer, nullable=True)
    review_date = Column(DateTime, nullable=True)

    patient = relationship("Patient")
