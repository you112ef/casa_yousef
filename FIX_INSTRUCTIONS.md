# Fix for Missing AForge.Video.DirectShow Assembly

## Error Details
```
System.IO.FileNotFoundException: Could not load file or assembly 'AForge.Video.DirectShow, Version=2.2.5.0, Culture=neutral, PublicKeyToken=61ea4348d43881b7' or one of its dependencies. The system cannot find the file specified.
```

## Root Cause
The application is trying to use the AForge.Video.DirectShow library for video processing, but the required DLL files are missing from the application directory.

## Solutions (in order of recommendation)

### Solution 1: Using NuGet (Recommended)
1. Run the `install_dependencies.bat` file provided in this folder
2. This will download the required packages via NuGet
3. Copy these files to the main application directory:
   - `packages\AForge.2.2.5\lib\AForge.dll`
   - `packages\AForge.Video.2.2.5\lib\AForge.Video.dll`
   - `packages\AForge.Video.DirectShow.2.2.5\lib\AForge.Video.DirectShow.dll`

### Solution 2: Manual Download
1. Download the AForge.NET framework from: https://github.com/andrewkirillov/AForge.NET
2. Build the solution or extract the pre-built binaries
3. Copy these files to the application directory:
   - AForge.dll
   - AForge.Video.dll
   - AForge.Video.DirectShow.dll

### Solution 3: Alternative Sources
1. Download from NuGet Gallery website directly:
   - https://www.nuget.org/packages/AForge/
   - https://www.nuget.org/packages/AForge.Video/
   - https://www.nuget.org/packages/AForge.Video.DirectShow/

## Additional Configuration
The `Sky_CASA.exe.config` file has been created with:
- JIT debugging enabled
- Assembly binding redirects for the AForge libraries

## Verification
After copying the DLL files:
1. Restart the application
2. Try the functionality that was causing the error (Button11_Click)
3. If issues persist, check Windows Event Viewer for additional error details

## Notes
- Ensure all DLLs are the same version (2.2.5.0)
- Make sure the architecture (x86/x64) matches your application
- The application targets .NET Framework 4.0