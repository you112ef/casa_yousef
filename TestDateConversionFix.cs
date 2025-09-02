using System;
using System.Windows.Forms;

class TestDateConversionFix
{
    [STAThread]
    static void Main()
    {
        Console.WriteLine("Testing Safe Date Conversion Fix...");
        
        // Test the safe conversion method with various inputs
        var testCases = new[]
        {
            "2025-08-31 00:00:00",
            "2025-09-01 12:30:45",
            "",
            null,
            "invalid date",
            "01/01/2025",
            "2025-13-45" // Invalid date
        };
        
        Console.WriteLine("Testing SafeConvertToDate method:");
        foreach (var testCase in testCases)
        {
            try
            {
                DateTime result = SafeConvertToDate(testCase, DateTime.Now);
                Console.WriteLine($"Input: '{testCase}' -> Output: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Input: '{testCase}' -> Exception: {ex.Message}");
            }
        }
        
        Console.WriteLine("\nTest completed. The fix should prevent InvalidCastException errors.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    
    // Safe date conversion method (same as in Rec.cs)
    private static DateTime SafeConvertToDate(string input, DateTime defaultValue)
    {
        // Handle null or empty strings
        if (string.IsNullOrEmpty(input))
        {
            return defaultValue;
        }
        
        // Try to parse the string as a date
        if (DateTime.TryParse(input.Trim(), out DateTime result))
        {
            return result;
        }
        else
        {
            // Return default value if parsing fails
            return defaultValue;
        }
    }
}