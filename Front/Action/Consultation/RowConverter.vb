Imports System.Windows.Data
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace Action.Consultation
    Public Class RowConverter
        Implements IValueConverter
#Region "IValueConverter Members"

        Private Function IValueConverter_Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If CInt(value) > 2000 Then
                Return 1
            ElseIf CInt(value) > 4000 Then
                Return 0
            Else
                Return -1
            End If
        End Function

        Private Function IValueConverter_ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function

#End Region
    End Class
End Namespace