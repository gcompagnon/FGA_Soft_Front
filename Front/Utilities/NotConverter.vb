Imports System.Windows.Data
Imports System.Globalization

Namespace Utilities

    Public Class NotConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal type As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            If TypeOf value Is Boolean Then
                Return Not value
            End If

            Return Nothing
        End Function

        Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            If TypeOf value Is Boolean Then
                Return Not value
            End If

            Return Nothing
        End Function

    End Class

End Namespace
