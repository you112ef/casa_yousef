' Safe Conversion Helper Functions for VB.NET
' These functions can be added to your project to prevent conversion errors

Module SafeConversionHelper

    ''' <summary>
    ''' Safely converts a string to Double, returning a default value if conversion fails
    ''' </summary>
    ''' <param name="input">The string to convert</param>
    ''' <param name="defaultValue">The default value to return if conversion fails</param>
    ''' <returns>The converted Double value or the default value</returns>
    Public Function SafeConvertToDouble(input As String, Optional defaultValue As Double = 0.0) As Double
        ' Handle null or empty strings
        If String.IsNullOrEmpty(input) Then
            Return defaultValue
        End If
        
        ' Try to parse the string
        Dim result As Double
        If Double.TryParse(input.Trim(), result) Then
            Return result
        Else
            ' Return default value if parsing fails
            Return defaultValue
        End If
    End Function
    
    ''' <summary>
    ''' Safely converts a string to Double with error reporting
    ''' </summary>
    ''' <param name="input">The string to convert</param>
    ''' <param name="result">The converted Double value</param>
    ''' <param name="fieldName">The name of the field for error reporting</param>
    ''' <returns>True if conversion was successful, False otherwise</returns>
    Public Function TryConvertToDouble(input As String, ByRef result As Double, Optional fieldName As String = "Value") As Boolean
        ' Handle null or empty strings
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show($"Please enter a valid number for {fieldName}")
            Return False
        End If
        
        ' Try to parse the string
        If Double.TryParse(input.Trim(), result) Then
            Return True
        Else
            MessageBox.Show($"Please enter a valid number for {fieldName}. Invalid input: {input}")
            Return False
        End If
    End Function
    
    ''' <summary>
    ''' Safely converts an object (possibly from database) to Double
    ''' </summary>
    ''' <param name="value">The object to convert</param>
    ''' <param name="defaultValue">The default value to return if conversion fails</param>
    ''' <returns>The converted Double value or the default value</returns>
    Public Function SafeConvertObjectToDouble(value As Object, Optional defaultValue As Double = 0.0) As Double
        ' Handle null values
        If value Is Nothing OrElse IsDBNull(value) Then
            Return defaultValue
        End If
        
        ' Convert to string and then to double
        Return SafeConvertToDouble(value.ToString(), defaultValue)
    End Function
    
    ''' <summary>
    ''' Safely converts a string to Date, returning a default value if conversion fails
    ''' </summary>
    ''' <param name="input">The string to convert</param>
    ''' <param name="defaultValue">The default value to return if conversion fails</param>
    ''' <returns>The converted Date value or the default value</returns>
    Public Function SafeConvertToDate(input As String, Optional defaultValue As Date = #1/1/1900#) As Date
        ' Handle null or empty strings
        If String.IsNullOrEmpty(input) Then
            Return defaultValue
        End If
        
        ' Try to parse the string as a date
        Dim result As Date
        If DateTime.TryParse(input.Trim(), result) Then
            Return result
        Else
            ' Return default value if parsing fails
            Return defaultValue
        End If
    End Function
    
    ''' <summary>
    ''' Safely converts a string to Date with error reporting
    ''' </summary>
    ''' <param name="input">The string to convert</param>
    ''' <param name="result">The converted Date value</param>
    ''' <param name="fieldName">The name of the field for error reporting</param>
    ''' <returns>True if conversion was successful, False otherwise</returns>
    Public Function TryConvertToDate(input As String, ByRef result As Date, Optional fieldName As String = "Date") As Boolean
        ' Handle null or empty strings
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show($"Please enter a valid date for {fieldName}")
            Return False
        End If
        
        ' Try to parse the string as a date
        If DateTime.TryParse(input.Trim(), result) Then
            Return True
        Else
            MessageBox.Show($"Please enter a valid date for {fieldName}. Invalid input: {input}")
            Return False
        End If
    End Function

End Module

' Usage Examples:
'
' Example 1: Simple conversion with default value
' Dim price As Double = SafeConversionHelper.SafeConvertToDouble(txtPrice.Text, 0.0)
'
' Example 2: Conversion with error reporting
' Dim quantity As Double
' If SafeConversionHelper.TryConvertToDouble(txtQuantity.Text, quantity, "Quantity") Then
'     ' Use the quantity value
'     Dim total As Double = price * quantity
' Else
'     ' Handle the error - message already shown
'     Exit Sub
' End If
'
' Example 3: Converting database values
' Dim dbValue As Double = SafeConversionHelper.SafeConvertObjectToDouble(dataReader("PriceField"), 0.0)
'
' Example 4: Safe date conversion
' Dim selectedDate As Date = SafeConversionHelper.SafeConvertToDate(dateString, DateTime.Now)
'
' Example 5: Date conversion with error reporting
' Dim appointmentDate As Date
' If SafeConversionHelper.TryConvertToDate(txtAppointmentDate.Text, appointmentDate, "Appointment Date") Then
'     ' Use the appointmentDate value
' Else
'     ' Handle the error - message already shown
'     Exit Sub
' End If