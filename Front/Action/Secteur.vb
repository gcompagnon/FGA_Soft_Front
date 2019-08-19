Namespace Action

    Public Class Secteur

        Sub New()
        End Sub

        Public Property Id As Integer
        Public Property Libelle As String

        Public Sub New(ByVal id As Integer, ByVal libelle As String)
            Me.Id = id
            Me.Libelle = libelle
        End Sub
    End Class

End Namespace
