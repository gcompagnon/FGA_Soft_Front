Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Namespace FileLink

    Public Class Image
        Inherits FileLink.File

        Private _image As ImageSource = Nothing

        Sub New(ByVal fullpath As String)
            MyBase.New(fullpath)

            _image = getImageFromResourceString(fullpath)
        End Sub

        Sub New()
            MyBase.New()
        End Sub

#Region "Propriétés"
        Public Property Image() As ImageSource
            Get
                Return _image
            End Get
            Set(ByVal value As ImageSource)
                _image = value
            End Set
        End Property
#End Region

        Private Function getImageFromResourceString(ByVal filename As String) As BitmapImage
            Dim image As New BitmapImage()

            image.BeginInit()
            image.UriSource = New Uri(filename)
            image.EndInit()

            Return image
        End Function

    End Class
End Namespace