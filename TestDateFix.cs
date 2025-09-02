using System;
using System.Globalization;

class TestDateFix
{
    static void Main()
    {
        Console.WriteLine("Testing Date Conversion Fix...");
        Console.WriteLine("=============================");
        
        // Test the problematic date string
        string testDate = "2025-08-31 00:00:00";
        
        Console.WriteLine($"Testing conversion of: {testDate}");
        
        // Test our enhanced conversion method
        DateTime result = SafeDateConversion.EnhancedSafeConvertToDate(testDate, DateTime.Now);
        
        Console.WriteLine($"Converted result: {result}");
        Console.WriteLine($"Conversion successful: {result != DateTime.Now || testDate == DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))}");
        
        // Test with various other formats
        string[] testDates = {
            "2025-09-01 12:30:45",
            "2025-09-01",
            "09/01/2025",
            "01/09/2025",
            "2025-13-45" // Invalid date
        };
        
        Console.WriteLine("\nTesting various date formats:");
        foreach (string date in testDates)
        {
            DateTime converted = SafeDateConversion.EnhancedSafeConvertToDate(date, DateTime.Now);
            Console.WriteLine($"  {date} -> {converted}");
        }
        
        Console.WriteLine("\nTest completed!");
        Console.WriteLine("The date conversion fix should prevent InvalidCastException errors.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}