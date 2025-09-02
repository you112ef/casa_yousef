# Sky CASA API Documentation

## Overview
This document provides detailed information about the Sky CASA API, which allows developers to integrate with the medical laboratory analysis system.

## Authentication
Sky CASA uses role-based authentication. API access is granted based on user roles:
- Administrator: Full access to all API endpoints
- Technician: Access to data entry and retrieval endpoints
- Viewer: Read-only access to data

## Base URL
```
http://localhost:8080/api/v1
```

## Data Formats
All API requests and responses use JSON format.

### Date Format
Dates are represented in ISO 8601 format:
```
YYYY-MM-DDTHH:MM:SSZ
```

### Error Responses
All error responses follow this format:
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": "Additional details about the error"
  }
}
```

## API Endpoints

### Patients

#### Get All Patients
```
GET /patients
```

**Parameters:**
- `limit` (optional): Number of patients to return (default: 10)
- `offset` (optional): Number of patients to skip (default: 0)
- `search` (optional): Search term to filter patients

**Response:**
```json
{
  "patients": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "dateOfBirth": "1980-01-01T00:00:00Z",
      "gender": "Male",
      "phoneNumber": "123-456-7890",
      "email": "john.doe@example.com",
      "address": "123 Main St, City, State 12345"
    }
  ],
  "total": 1,
  "limit": 10,
  "offset": 0
}
```

#### Get Patient by ID
```
GET /patients/{id}
```

**Response:**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1980-01-01T00:00:00Z",
  "gender": "Male",
  "phoneNumber": "123-456-7890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State 12345",
  "createdAt": "2023-01-01T10:00:00Z",
  "updatedAt": "2023-01-01T10:00:00Z"
}
```

#### Create Patient
```
POST /patients
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1980-01-01",
  "gender": "Male",
  "phoneNumber": "123-456-7890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State 12345"
}
```

**Response:**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1980-01-01T00:00:00Z",
  "gender": "Male",
  "phoneNumber": "123-456-7890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State 12345",
  "createdAt": "2023-01-01T10:00:00Z",
  "updatedAt": "2023-01-01T10:00:00Z"
}
```

#### Update Patient
```
PUT /patients/{id}
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1980-01-01",
  "gender": "Male",
  "phoneNumber": "123-456-7890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State 12345"
}
```

**Response:**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1980-01-01T00:00:00Z",
  "gender": "Male",
  "phoneNumber": "123-456-7890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State 12345",
  "createdAt": "2023-01-01T10:00:00Z",
  "updatedAt": "2023-01-01T11:00:00Z"
}
```

#### Delete Patient
```
DELETE /patients/{id}
```

**Response:**
```json
{
  "message": "Patient deleted successfully"
}
```

### CBC Test Results

#### Get All CBC Test Results
```
GET /cbc-results
```

**Parameters:**
- `patientId` (optional): Filter by patient ID
- `limit` (optional): Number of results to return (default: 10)
- `offset` (optional): Number of results to skip (default: 0)
- `startDate` (optional): Filter results from this date
- `endDate` (optional): Filter results to this date

**Response:**
```json
{
  "results": [
    {
      "id": 1,
      "patientId": 1,
      "technicianId": 1,
      "testDate": "2023-01-01T10:00:00Z",
      "wbc": 5.0,
      "rbc": 4.5,
      "hemoglobin": 14.0,
      "hematocrit": 42.0,
      "plateletCount": 250.0,
      "mcv": 90.0,
      "mch": 32.0,
      "mchc": 33.0,
      "rdw": 12.0,
      "mpv": 8.0,
      "pdw": 15.0,
      "neutrophilsPercent": 60.0,
      "lymphocytesPercent": 30.0,
      "monocytesPercent": 8.0,
      "eosinophilsPercent": 2.0,
      "basophilsPercent": 0.5,
      "bandsPercent": 1.0,
      "atypicalLymphocytesPercent": 0.5,
      "nrbc": 0.0,
      "neutrophilsAbsolute": 3.0,
      "lymphocytesAbsolute": 1.5,
      "monocytesAbsolute": 0.4,
      "eosinophilsAbsolute": 0.1,
      "basophilsAbsolute": 0.0,
      "bandsAbsolute": 0.05,
      "wbcInterpretation": "Normal",
      "rbcInterpretation": "Normal",
      "hgbInterpretation": "Normal",
      "pltInterpretation": "Normal",
      "differentialInterpretation": "Normal",
      "criticalValueFlag": false,
      "comments": "Normal CBC results",
      "qcStatus": "ACCEPTED",
      "reviewedBy": 1,
      "reviewDate": "2023-01-01T11:00:00Z"
    }
  ],
  "total": 1,
  "limit": 10,
  "offset": 0
}
```

#### Get CBC Test Result by ID
```
GET /cbc-results/{id}
```

**Response:**
```json
{
  "id": 1,
  "patientId": 1,
  "technicianId": 1,
  "testDate": "2023-01-01T10:00:00Z",
  "wbc": 5.0,
  "rbc": 4.5,
  "hemoglobin": 14.0,
  "hematocrit": 42.0,
  "plateletCount": 250.0,
  "mcv": 90.0,
  "mch": 32.0,
  "mchc": 33.0,
  "rdw": 12.0,
  "mpv": 8.0,
  "pdw": 15.0,
  "neutrophilsPercent": 60.0,
  "lymphocytesPercent": 30.0,
  "monocytesPercent": 8.0,
  "eosinophilsPercent": 2.0,
  "basophilsPercent": 0.5,
  "bandsPercent": 1.0,
  "atypicalLymphocytesPercent": 0.5,
  "nrbc": 0.0,
  "neutrophilsAbsolute": 3.0,
  "lymphocytesAbsolute": 1.5,
  "monocytesAbsolute": 0.4,
  "eosinophilsAbsolute": 0.1,
  "basophilsAbsolute": 0.0,
  "bandsAbsolute": 0.05,
  "wbcInterpretation": "Normal",
  "rbcInterpretation": "Normal",
  "hgbInterpretation": "Normal",
  "pltInterpretation": "Normal",
  "differentialInterpretation": "Normal",
  "criticalValueFlag": false,
  "comments": "Normal CBC results",
  "qcStatus": "ACCEPTED",
  "reviewedBy": 1,
  "reviewDate": "2023-01-01T11:00:00Z",
  "createdAt": "2023-01-01T10:00:00Z",
  "updatedAt": "2023-01-01T11:00:00Z"
}
```

#### Create CBC Test Result
```
POST /cbc-results
```

**Request Body:**
```json
{
  "patientId": 1,
  "technicianId": 1,
  "testDate": "2023-01-01T10:00:00Z",
  "wbc": 5.0,
  "rbc": 4.5,
  "hemoglobin": 14.0,
  "hematocrit": 42.0,
  "plateletCount": 250.0,
  "mcv": 90.0,
  "mch": 32.0,
  "mchc": 33.0,
  "rdw": 12.0,
  "mpv": 8.0,
  "pdw": 15.0,
  "neutrophilsPercent": 60.0,
  "lymphocytesPercent": 30.0,
  "monocytesPercent": 8.0,
  "eosinophilsPercent": 2.0,
  "basophilsPercent": 0.5,
  "bandsPercent": 1.0,
  "atypicalLymphocytesPercent": 0.5,
  "nrbc": 0.0,
  "neutrophilsAbsolute": 3.0,
  "lymphocytesAbsolute": 1.5,
  "monocytesAbsolute": 0.4,
  "eosinophilsAbsolute": 0.1,
  "basophilsAbsolute": 0.0,
  "bandsAbsolute": 0.05,
  "comments": "Normal CBC results",
  "qcStatus": "PENDING"
}
```

**Response:**
```json
{
  "id": 1,
  "patientId": 1,
  "technicianId": 1,
  "testDate": "2023-01-01T10:00:00Z",
  "wbc": 5.0,
  "rbc": 4.5,
  "hemoglobin": 14.0,
  "hematocrit": 42.0,
  "plateletCount": 250.0,
  "mcv": 90.0,
  "mch": 32.0,
  "mchc": 33.0,
  "rdw": 12.0,
  "mpv": 8.0,
  "pdw": 15.0,
  "neutrophilsPercent": 60.0,
  "lymphocytesPercent": 30.0,
  "monocytesPercent": 8.0,
  "eosinophilsPercent": 2.0,
  "basophilsPercent": 0.5,
  "bandsPercent": 1.0,
  "atypicalLymphocytesPercent": 0.5,
  "nrbc": 0.0,
  "neutrophilsAbsolute": 3.0,
  "lymphocytesAbsolute": 1.5,
  "monocytesAbsolute": 0.4,
  "eosinophilsAbsolute": 0.1,
  "basophilsAbsolute": 0.0,
  "bandsAbsolute": 0.05,
  "wbcInterpretation": "Normal",
  "rbcInterpretation": "Normal",
  "hgbInterpretation": "Normal",
  "pltInterpretation": "Normal",
  "differentialInterpretation": "Normal",
  "criticalValueFlag": false,
  "comments": "Normal CBC results",
  "qcStatus": "PENDING",
  "createdAt": "2023-01-01T10:00:00Z",
  "updatedAt": "2023-01-01T10:00:00Z"
}
```

### Analysis Types

#### Get All Analysis Types
```
GET /analysis-types
```

**Response:**
```json
{
  "analysisTypes": [
    {
      "id": 1,
      "name": "Complete Blood Count",
      "description": "Full blood panel analysis",
      "category": "Hematology",
      "units": "Cells/μL",
      "testMethod": "Automated Analyzer"
    },
    {
      "id": 2,
      "name": "Urinalysis",
      "description": "Urinary system analysis",
      "category": "Clinical Chemistry",
      "units": "Various",
      "testMethod": "Automated Analyzer"
    }
  ]
}
```

## Error Codes
- `PATIENT_NOT_FOUND`: The specified patient could not be found
- `INVALID_DATA`: The provided data is invalid
- `DATABASE_ERROR`: An error occurred while accessing the database
- `UNAUTHORIZED`: Access to the requested resource is not authorized
- `INTERNAL_ERROR`: An unexpected error occurred on the server

## Rate Limiting
The Sky CASA API implements rate limiting to prevent abuse:
- 1000 requests per hour per IP address
- 100 requests per minute per IP address

Exceeding these limits will result in a 429 (Too Many Requests) response.

## Versioning
The API uses semantic versioning. Breaking changes will result in a new major version number.

## Support
For API support, please contact the development team or refer to the documentation.