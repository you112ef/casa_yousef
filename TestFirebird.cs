using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Attempting to load FirebirdSql.Data.FirebirdClient.dll...");
            
            // Try to load the assembly
            Assembly assembly = Assembly.LoadFrom("FirebirdSql.Data.FirebirdClient.dll");
            
            Console.WriteLine("Assembly loaded successfully!");
            Console.WriteLine("Assembly Name: " + assembly.FullName);
            
            // Try to get a type from the assembly
            Type type = assembly.GetType("FirebirdSql.Data.FirebirdClient.FbConnection");
            if (type != null)
            {
                Console.WriteLine("FbConnection type found!");
            }
            else
            {
                Console.WriteLine("FbConnection type not found!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading assembly: " + ex.Message);
            Console.WriteLine("Exception type: " + ex.GetType().FullName);
            
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner exception: " + ex.InnerException.Message);
            }
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}