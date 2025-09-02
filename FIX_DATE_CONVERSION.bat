@echo off
echo =====================================================
echo Sky CASA - Fix Date Conversion Issue
echo =====================================================
echo.

echo This script will fix the date conversion issue in the Sky CASA application.
echo The issue occurs when trying to convert date strings like "2025-08-31 00:00:00" 
echo to Date type using VB.NET's Conversions.ToDate method.
echo.

echo Creating backup of existing files...
if exist "SafeConversionHelper.vb" (
    copy "SafeConversionHelper.vb" "SafeConversionHelper.vb.backup" >nul
    echo Backup of SafeConversionHelper.vb created.
)

echo.

echo Applying date conversion fix...
echo Adding enhanced date conversion functions to SafeConversionHelper.vb...

(
echo.
echo ''' ^<summary^>
echo ''' Enhanced safe date conversion with multiple format support
echo ''' ^</summary^>
echo ''' ^<param name="input"^>The string to convert^</param^>
echo ''' ^<param name="defaultValue"^>The default value to return if conversion fails^</param^>
echo ''' ^<returns^>The converted Date value or the default value^</returns^>
echo Public Function EnhancedSafeConvertToDate(input As String, Optional defaultValue As Date = #1/1/1900#) As Date
echo     ' Handle null or empty strings
echo     If String.IsNullOrEmpty(input) Then
echo         Return defaultValue
echo     End If
echo.
echo     ' Trim the input
echo     Dim trimmedInput As String = input.Trim()
echo.
echo     ' Try multiple date formats that might be encountered
echo     Dim dateFormats As String() = {"yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm", "MM/dd/yyyy", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy"}
echo.
echo     ' Try to parse with specific formats
echo     Dim result As Date
echo     For Each format As String In dateFormats
echo         If DateTime.TryParseExact(trimmedInput, format, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, result) Then
echo             Return result
echo         End If
echo     Next
echo.
echo     ' Try standard parsing as fallback
echo     If DateTime.TryParse(trimmedInput, result) Then
echo         Return result
echo     End If
echo.
echo     ' Return default value if all parsing attempts fail
echo     Return defaultValue
echo End Function
echo.
echo ''' ^<summary^>
echo ''' Replacement for VB.NET's Conversions.ToDate with better error handling
echo ''' ^</summary^>
echo ''' ^<param name="value"^>The value to convert^</param^>
echo ''' ^<returns^>The converted Date value^</returns^>
echo Public Function SafeToDateConversion(value As Object) As Date
echo     Try
echo         ' Handle null values
echo         If value Is Nothing OrElse IsDBNull(value) Then
echo             Return DateTime.Now
echo         End If
echo.
echo         ' If it's already a Date, return it directly
echo         If TypeOf value Is Date Then
echo             Return CType(value, Date)
echo         End If
echo.
echo         ' Convert to string and use enhanced conversion
echo         Dim stringValue As String = value.ToString()
echo         Return EnhancedSafeConvertToDate(stringValue, DateTime.Now)
echo     Catch ex As Exception
echo         ' Log the error and return current date as fallback
echo         System.Diagnostics.Debug.WriteLine("SafeToDateConversion error: " & ex.Message)
echo         Return DateTime.Now
echo     End Try
echo End Function
) >> "SafeConversionHelper.vb"

echo.
echo Date conversion fix applied successfully!
echo.
echo The fix includes:
echo 1. EnhancedSafeConvertToDate function with multiple format support
echo 2. SafeToDateConversion function as a replacement for Conversions.ToDate
echo 3. Support for various date formats including "yyyy-MM-dd HH:mm:ss"
echo.
echo To use the fix in your VB.NET code, replace calls to:
echo   Conversions.ToDate(value)
echo with:
echo   SafeToDateConversion(value)
echo.
echo Or for more control, use:
echo   EnhancedSafeConvertToDate(dateString, defaultValue)
echo.
echo A backup of the original SafeConversionHelper.vb file has been created.
echo.
pause