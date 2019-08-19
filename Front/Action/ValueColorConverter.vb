Imports System.Data
Imports System.Windows.Data

Namespace Action

    Public Class ValueColorConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert

            Console.WriteLine("TOTOTOTOTOTOTOTOTTO")

            Dim str As String = value

            If str Is Nothing Then
                Return Nothing
            End If

            If str = "ADS" Then
                Return Brushes.Red
            Else
                Return Brushes.Transparent

            End If

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace

