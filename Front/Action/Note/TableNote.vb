Imports System.Collections.ObjectModel

Namespace Action.Note

    Public Class TableNote

        Public Property Id As Integer
        Public Property Name As String

        Sub New()
        End Sub

        Public Sub New(ByVal id As Integer, ByVal name As String)
            Me.Id = id
            Me.Name = name
        End Sub
    End Class
End Namespace
