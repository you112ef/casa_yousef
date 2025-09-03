from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime

class UserOut(BaseModel):
    id: int
    username: str
    name: Optional[str] = None
    email: Optional[str] = None
    role: Optional[str] = None

    class Config:
        from_attributes = True

class TokenResponse(BaseModel):
    token: str
    user: Optional[UserOut] = None

class LoginRequest(BaseModel):
    username: str
    password: str

class PatientBase(BaseModel):
    firstName: Optional[str] = None
    lastName: Optional[str] = None
    dateOfBirth: Optional[str] = None
    gender: Optional[str] = None
    phoneNumber: Optional[str] = None
    email: Optional[str] = None
    address: Optional[str] = None

class PatientCreate(PatientBase):
    pass

class PatientOut(PatientBase):
    id: int
    createdAt: datetime

    class Config:
        from_attributes = True

class PatientsResponse(BaseModel):
    patients: List[PatientOut]
    total: int
    limit: int
    offset: int

class CBCResultBase(BaseModel):
    patientId: Optional[int] = None
    technicianId: Optional[int] = None
    testDate: Optional[datetime] = None
    wbc: Optional[float] = None
    rbc: Optional[float] = None
    hemoglobin: Optional[float] = None
    hematocrit: Optional[float] = None
    plateletCount: Optional[float] = None

class CBCResultCreate(CBCResultBase):
    pass

class CBCResultOut(CBCResultBase):
    id: int

    class Config:
        from_attributes = True

class CBCResultsResponse(BaseModel):
    results: List[CBCResultOut]
    total: int
    limit: int
    offset: int
