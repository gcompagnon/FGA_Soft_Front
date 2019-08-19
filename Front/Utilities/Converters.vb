Imports System.Windows.Data
Imports System.Globalization

Namespace Utilities

    Public Class LevelToIndentConverter
        Implements IValueConverter

        Private Const INDENT_SIZE As Double = 19


        Public Function Convert(ByVal o As Object, ByVal type As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            Return New System.Windows.Thickness(CType(o, Integer) * INDENT_SIZE, 0, 0, 0)
        End Function

        Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function

    End Class

End Namespace

