# Sky CASA Application User Guide

This guide explains how to use the enhanced features of the Sky CASA medical laboratory application.

## Table of Contents
1. [Getting Started](#getting-started)
2. [Main Application Features](#main-application-features)
3. [Patient Management](#patient-management)
4. [Data Validation](#data-validation)
5. [Reporting](#reporting)
6. [Configuration](#configuration)
7. [Backup and Recovery](#backup-and-recovery)
8. [Troubleshooting](#troubleshooting)

## Getting Started

### System Requirements
- Windows operating system
- .NET Framework
- SQLite database engine

### Installation
1. Extract all files to a folder
2. Run `Sky_CASA.exe` to start the application
3. The application will automatically create necessary files on first run

### First Run
On first run, the application will:
- Create a default configuration file
- Initialize the database
- Set up logging
- Perform an initial backup

## Main Application Features

### Main Dashboard
The main dashboard provides:
- Table selection for viewing data
- Data loading capabilities
- Patient records access

### Loading Data
1. Select a table from the dropdown
2. Click "Load All Data" to view records
3. Data will be displayed in the grid view

### Accessing Patient Records
Click the "Patient Records" button to open the patient management interface.

## Patient Management

### Viewing Patients
The patient records screen displays:
- All patients in a searchable grid
- Patient details including name, date of birth, gender, and contact information

### Searching Patients
1. Enter a patient name in the search box
2. Click "Search" to filter results
3. Click "Refresh" to clear search and show all patients

### Adding New Patients
The application validates all patient information:
- Name is required
- Date of birth must be valid
- Gender must be specified
- Phone number is validated for format

### Editing Patients
Select a patient from the grid to view details and make changes.

## Data Validation

### Automatic Validation
The application automatically validates:
- Phone number formats
- Email addresses
- Date ranges
- Required fields
- Gender values
- Patient IDs

### Validation Feedback
When validation fails:
- Clear error messages are displayed
- Invalid data is highlighted
- Users are guided to correct issues

### Medical Test Validation
CBC test values are validated against normal ranges:
- Low values are flagged
- High values are flagged
- Normal values are confirmed

## Reporting

### Patient Summary Reports
Generate patient summary reports:
1. Navigate to patient records
2. Select patients to include
3. Click "Generate Report"
4. Save report to file

### CBC Analysis Reports
Generate detailed CBC analysis reports:
1. Select a patient
2. View their CBC data
3. Generate report for analysis

### Statistical Reports
Generate statistical summaries:
1. Select data to analyze
2. Choose columns for statistics
3. Generate summary report

### Saving Reports
All reports can be saved as text files in the `reports` folder.

## Configuration

### Application Settings
Manage application settings through the configuration file:
- Database path
- Maximum records per page
- Logging options
- Date formats
- Language preferences
- Backup settings

### Configuration File
The `appsettings.xml` file contains all application settings:
```xml
<configuration>
  <settings>
    <setting key="DatabasePath" value="database.db" />
    <setting key="MaxRecordsPerPage" value="50" />
    <setting key="EnableLogging" value="true" />
    <!-- Additional settings -->
  </settings>
</configuration>
```

### Changing Settings
To change settings:
1. Close the application
2. Edit `appsettings.xml`
3. Restart the application

## Backup and Recovery

### Automatic Backups
The application automatically creates backups:
- Based on configured interval (default: 7 days)
- Stored in the `backups` folder
- Named with timestamps

### Manual Backups
Create backups manually:
1. Access backup functionality through settings
2. Choose backup location
3. Confirm backup creation

### Restoring Backups
Restore from backups:
1. Access restore functionality
2. Select backup file
3. Confirm restoration (warning: overwrites current data)

### Backup Management
Manage backups:
- View available backups
- Delete old backups to save space
- Verify backup integrity

## Troubleshooting

### Common Issues

#### Application Won't Start
1. Check that all required files are present
2. Verify .NET Framework installation
3. Check Windows Event Viewer for errors

#### Database Connection Errors
1. Verify database file exists
2. Check file permissions
3. Ensure no other applications are using the database

#### Data Not Loading
1. Check database connection
2. Verify table exists
3. Check application logs for errors

### Log Files
Application logs are stored in `application.log`:
- Contains detailed information about application operation
- Useful for troubleshooting
- Automatically cleared on each application start

### Error Messages
Common error messages and solutions:
- "Database file not found" - Check database path in settings
- "Invalid data format" - Correct data entry according to validation rules
- "Connection failed" - Check database file permissions

### Support
For additional support:
1. Check application logs
2. Review this user guide
3. Contact technical support with log files

## Best Practices

### Data Entry
- Always validate data before saving
- Use consistent date formats
- Enter complete patient information
- Regularly backup data

### Security
- Protect database files
- Use strong passwords for any protected features
- Regularly update the application
- Monitor access logs

### Maintenance
- Regularly clean up old backups
- Monitor disk space
- Update configuration as needed
- Review logs for issues

## Conclusion

The enhanced Sky CASA application provides robust features for medical laboratory data management with improved reliability, security, and usability. Following this guide will help you make the most of the application's capabilities while maintaining data integrity and security.