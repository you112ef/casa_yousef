# PowerShell script to help debug data conversion issues
Write-Host "Sky CASA - Data Conversion Error Debugging" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "This error occurs when trying to convert an empty string to a Double." -ForegroundColor Yellow
Write-Host "The error is in the Rec form's Button8_Click event." -ForegroundColor Yellow
Write-Host ""

Write-Host "Common debugging steps:" -ForegroundColor Cyan
Write-Host "1. Check all TextBox, ComboBox, or input controls in the Rec form" -ForegroundColor White
Write-Host "2. Look for code that converts text values to Double, Decimal, or other numeric types" -ForegroundColor White
Write-Host "3. Check database queries that might return empty strings" -ForegroundColor White
Write-Host "4. Verify that all numeric inputs have proper validation" -ForegroundColor White
Write-Host ""

Write-Host "Recommended fix pattern:" -ForegroundColor Cyan
Write-Host @"
' Instead of:
Dim value As Double = CDbl(TextBox1.Text)

' Use:
Dim value As Double
If String.IsNullOrWhiteSpace(TextBox1.Text) Then
    TextBox1.Text = "0" ' or handle appropriately
End If

If Not Double.TryParse(TextBox1.Text, value) Then
    MessageBox.Show("Please enter a valid number in " & TextBox1.Name)
    Exit Sub
End If
"@ -ForegroundColor White

Write-Host ""
Write-Host "Database considerations:" -ForegroundColor Cyan
Write-Host "Check if any database fields that should contain numbers might be empty strings" -ForegroundColor White
Write-Host "Use ISNULL() or COALESCE() in SQL queries to provide default values" -ForegroundColor White
Write-Host ""

Write-Host "Example SQL fix:" -ForegroundColor Cyan
Write-Host "SELECT ISNULL(NumericField, 0) AS NumericField FROM TableName" -ForegroundColor White
Write-Host ""

Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")