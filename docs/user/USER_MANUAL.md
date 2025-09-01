# Sky CASA User Manual

## Table of Contents
1. [Introduction](#introduction)
2. [System Requirements](#system-requirements)
3. [Installation](#installation)
4. [Getting Started](#getting-started)
5. [Main Features](#main-features)
6. [Data Management](#data-management)
7. [Analysis Types](#analysis-types)
8. [Reporting](#reporting)
9. [Troubleshooting](#troubleshooting)
10. [FAQ](#faq)

## Introduction
Sky CASA (Computerized Analysis System for Laboratories) is a comprehensive medical laboratory analysis application designed for processing and analyzing various types of medical tests. The system provides healthcare professionals with tools to manage patient data, perform analysis, and generate reports.

## System Requirements
- Windows 7 or later
- .NET Framework 4.7.2 or later
- 4 GB RAM minimum (8 GB recommended)
- 500 MB available disk space
- Screen resolution of 1024x768 or higher

## Installation
1. Download the Sky CASA installation package
2. Run the installer executable
3. Follow the installation wizard prompts
4. Launch the application from the desktop shortcut or Start menu

## Getting Started
1. Launch Sky CASA application
2. Create a new patient record or select an existing patient
3. Select the type of analysis to perform
4. Enter the test results
5. Save the data
6. Generate reports as needed

## Main Features
- Patient data management
- Multiple analysis types (CBC, urinalysis, stool analysis, etc.)
- Quality control and validation
- Report generation
- Data export capabilities
- User management with role-based access

## Data Management
### Patient Management
- Add new patients with demographic information
- Edit existing patient records
- Search and filter patient records
- View patient history

### Test Results
- Record test results for various analysis types
- Validate data entry with built-in checks
- View historical test results
- Compare results over time

## Analysis Types
### Complete Blood Count (CBC)
- White blood cell count (WBC)
- Red blood cell count (RBC)
- Hemoglobin (HGB)
- Hematocrit (HCT)
- Platelet count (PLT)
- Differential counts (Neutrophils, Lymphocytes, Monocytes, etc.)

### Urinalysis
- Physical examination (color, clarity, specific gravity)
- Chemical examination (pH, protein, glucose, etc.)
- Microscopic examination (cells, crystals, bacteria)

### Stool Analysis
- Physical examination (color, consistency)
- Chemical examination (occult blood, pH)
- Microscopic examination (parasites, bacteria, yeast)

## Reporting
- Generate individual patient reports
- Create summary reports for multiple patients
- Export reports to PDF, Excel, or Word formats
- Print reports directly from the application

## Troubleshooting
### Common Issues
#### "Cannot add more than 10 cases"
This was a known issue that has been fixed. If you encounter this issue:
1. Ensure you are using the latest version of Sky CASA
2. Run `RunUIUpdate.bat` to automatically fix UI code
3. For large datasets, consider implementing pagination

#### Database Connection Issues
1. Ensure `database.db` exists in the application directory
2. Check file permissions
3. Verify the connection string in `Sky_CASA.exe.config`

#### Missing Dependencies
1. Ensure all DLL files are in the application directory
2. Check that the correct .NET Framework version is installed

### Error Messages
- **"Database file not found"**: The database file is missing or the path is incorrect
- **"Invalid data entry"**: Data validation has failed; check the entered values
- **"Connection failed"**: Unable to connect to the database; check the connection settings

## FAQ
### How do I add a new patient?
1. Click on the "Patients" tab
2. Click the "Add New" button
3. Fill in the patient information
4. Click "Save"

### How do I perform a CBC analysis?
1. Select a patient from the patient list
2. Click on the "CBC" tab
3. Enter the test results in the appropriate fields
4. Click "Save" to store the results

### How do I generate a report?
1. Navigate to the "Reports" section
2. Select the type of report you want to generate
3. Apply any filters or date ranges
4. Click "Generate Report"
5. Preview, print, or export the report as needed

### How do I update normal ranges?
1. Navigate to the "Settings" section
2. Select "Normal Ranges"
3. Choose the analysis type and parameter
4. Update the normal range values
5. Click "Save" to apply the changes

### How do I back up the database?
1. Close the Sky CASA application
2. Copy the `database.db` file to a secure location
3. Store the backup in a safe place
4. To restore, replace the existing `database.db` file with the backup

For additional support, please contact the technical support team.