Imports System.Windows.Data
Imports System.Windows.Controls
Imports System.Globalization

Namespace Utilities

    Public Class NotVisibilityConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal type As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            If TypeOf value Is Boolean Then
                If value Then
                    Return Windows.Visibility.Visible
                Else
                    Return Windows.Visibility.Collapsed
                End If
            End If

            Return Nothing
        End Function

        Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            If TypeOf value Is Boolean Then
                If value Then
                    Return Windows.Visibility.Visible
                Else
                    Return Windows.Visibility.Collapsed
                End If
            End If

            Return Nothing
        End Function
    End Class

End Namespace
