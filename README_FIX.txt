AForge.NET Library Missing Fix Instructions:

1. Download the AForge.NET framework:
   - Go to https://github.com/andrewkirillov/AForge.NET
   - Download the latest release or clone the repository

2. Extract the required libraries:
   - You need AForge.Video.DirectShow.dll (version 2.2.5.0)
   - Also copy these dependencies to the same folder:
     * AForge.dll
     * AForge.Video.dll

3. Place these DLL files in the same directory as Sky_CASA.exe:
   - D:\New folder (4)\Sky CASA\

4. Alternatively, you can install via NuGet Package Manager:
   - Install-Package AForge.Video.DirectShow -Version 2.2.5

If you continue to have issues, you may need to:
- Check if your application requires any specific architecture (x86/x64)
- Ensure all dependencies are compatible with .NET Framework 4.0