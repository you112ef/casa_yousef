using System;
using System.Data;
using System.Data.SQLite;

/// <summary>
/// برنامج اختبار قاعدة البيانات
/// يختبر اتصال قاعدة البيانات وعمليات البحث الأساسية
/// </summary>
class TestDatabaseConnection
{
    static void Main()
    {
        Console.WriteLine("=== Sky CASA Database Connection Test ===");
        Console.WriteLine("تاريخ الاختبار: " + DateTime.Now);
        Console.WriteLine();

        try
        {
            // Test 1: Database connection
            Console.WriteLine("1. اختبار الاتصال بقاعدة البيانات...");
            TestConnection();
            Console.WriteLine("✓ نجح الاتصال بقاعدة البيانات");
            Console.WriteLine();

            // Test 2: DataAccessLayer
            Console.WriteLine("2. اختبار طبقة الوصول للبيانات...");
            TestDataAccessLayer();
            Console.WriteLine("✓ نجح اختبار طبقة الوصول للبيانات");
            Console.WriteLine();

            // Test 3: Table structures
            Console.WriteLine("3. اختبار هيكل الجداول...");
            TestTableStructures();
            Console.WriteLine("✓ نجح اختبار هيكل الجداول");
            Console.WriteLine();

            // Test 4: Sample data
            Console.WriteLine("4. اختبار البيانات التجريبية...");
            TestSampleData();
            Console.WriteLine("✓ نجح اختبار البيانات التجريبية");
            Console.WriteLine();

            // Test 5: CBC data validation
            Console.WriteLine("5. اختبار بيانات فحص الدم الشامل...");
            TestCBCData();
            Console.WriteLine("✓ نجح اختبار بيانات CBC");
            Console.WriteLine();

            Console.WriteLine("=== جميع الاختبارات نجحت! ===");
            Console.WriteLine("قاعدة البيانات جاهزة للاستخدام");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ خطأ في الاختبار: {ex.Message}");
            Console.WriteLine($"تفاصيل الخطأ: {ex.StackTrace}");
        }

        Console.WriteLine();
        Console.WriteLine("اضغط أي مفتاح للخروج...");
        Console.ReadKey();
    }

    static void TestConnection()
    {
        string connectionString = "Data Source=database.db;Version=3;";
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new SQLiteCommand("SELECT 1", connection))
            {
                var result = cmd.ExecuteScalar();
                if (result.ToString() != "1")
                    throw new Exception("فشل اختبار الاتصال الأساسي");
            }
        }
    }

    static void TestDataAccessLayer()
    {
        var dal = new DataAccessLayer("database.db");
        var result = dal.ExecuteQuery("SELECT COUNT(*) as total FROM sqlite_master WHERE type='table'");
        
        if (result.Rows.Count == 0)
            throw new Exception("فشل في استرداد معلومات الجداول");

        Console.WriteLine($"   عدد الجداول: {result.Rows[0]["total"]}");
    }

    static void TestTableStructures()
    {
        string connectionString = "Data Source=database.db;Version=3;";
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            
            string[] expectedTables = { "admin", "patients", "doctors", "cbc", "urine", "stool" };
            
            foreach (string table in expectedTables)
            {
                using (var cmd = new SQLiteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{table}'", connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        throw new Exception($"الجدول {table} غير موجود");
                    
                    Console.WriteLine($"   ✓ الجدول {table} موجود");
                }
            }
        }
    }

    static void TestSampleData()
    {
        var dal = new DataAccessLayer("database.db");
        
        // Test admin data
        var adminData = dal.ExecuteQuery("SELECT COUNT(*) as count FROM admin");
        int adminCount = Convert.ToInt32(adminData.Rows[0]["count"]);
        Console.WriteLine($"   عدد المستخدمين الإداريين: {adminCount}");
        
        if (adminCount == 0)
            throw new Exception("لا توجد بيانات إدارية");

        // Test patients data
        var patientsData = dal.ExecuteQuery("SELECT COUNT(*) as count FROM patients");
        int patientsCount = Convert.ToInt32(patientsData.Rows[0]["count"]);
        Console.WriteLine($"   عدد المرضى: {patientsCount}");

        // Test doctors data
        var doctorsData = dal.ExecuteQuery("SELECT COUNT(*) as count FROM doctors");
        int doctorsCount = Convert.ToInt32(doctorsData.Rows[0]["count"]);
        Console.WriteLine($"   عدد الأطباء: {doctorsCount}");
    }

    static void TestCBCData()
    {
        var dal = new DataAccessLayer("database.db");
        
        // Test CBC structure
        var cbcColumns = dal.ExecuteQuery("PRAGMA table_info(cbc)");
        Console.WriteLine($"   عدد أعمدة جدول CBC: {cbcColumns.Rows.Count}");
        
        if (cbcColumns.Rows.Count < 20)
            throw new Exception("جدول CBC لا يحتوي على العدد المطلوب من الأعمدة");

        // Test CBC data
        var cbcData = dal.ExecuteQuery("SELECT COUNT(*) as count FROM cbc WHERE wbc IS NOT NULL");
        int cbcCount = Convert.ToInt32(cbcData.Rows[0]["count"]);
        Console.WriteLine($"   عدد نتائج CBC مع بيانات: {cbcCount}");

        // Test specific CBC record
        var sampleCBC = dal.ExecuteQuery("SELECT patient_id, wbc, rbc, hemoglobin FROM cbc WHERE wbc IS NOT NULL LIMIT 1");
        if (sampleCBC.Rows.Count > 0)
        {
            var row = sampleCBC.Rows[0];
            Console.WriteLine($"   مثال: مريض {row["patient_id"]}, WBC: {row["wbc"]}, RBC: {row["rbc"]}, Hgb: {row["hemoglobin"]}");
        }
    }
}