# Changelog
All notable changes to the Sky CASA project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Login functionality with authentication against database
- Comprehensive documentation structure
- Developer guide
- User manual
- API documentation
- Test framework with unit tests
- Build scripts
- Project files for Visual Studio
- Main application form

### Changed
- Enhanced README with comprehensive information
- Improved project structure

### Fixed
- Case limit issue by implementing GetAllData method
- UI display limitations
- Assembly loading issue in login-enabled application

### Security
- Updated dependencies
- Added authentication layer to application

## [1.2.0] - 2025-09-01

### Added
- GetAllData method to DataAccessLayer to fix case limit issue
- UI update guide and automation tools
- Comprehensive verification tools
- Complete case limit fix guide
- PowerShell script to automate UI updates
- Batch file to run verification

### Changed
- Updated UI code to use GetAllData instead of GetSampleData
- Enhanced documentation with fix details

### Fixed
- "Cannot add more than 10 cases" issue
- Database display limitations in UI

## [1.1.0] - 2025-08-15

### Added
- CBC analysis functionality
- Patient data management
- Test result recording
- Quality control features
- Report generation capabilities
- Database schema for medical laboratory use
- Business logic for CBC interpretation
- Data access layer for SQLite operations

### Changed
- Improved data validation
- Enhanced error handling
- Better user interface

## [1.0.0] - 2025-07-01

### Added
- Initial release of Sky CASA
- Basic patient management
- Simple test result recording
- SQLite database integration
- Windows Forms user interface
- Core application framework

[Unreleased]: https://github.com/you112ef/casa_yousef/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/you112ef/casa_yousef/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/you112ef/casa_yousef/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/you112ef/casa_yousef/releases/tag/v1.0.0