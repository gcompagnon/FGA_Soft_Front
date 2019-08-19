Namespace Action.Coefficient

    Public Class SecteurView
        Inherits Watcher.Supervisor

        Private _secteur As Secteur

        Public Sub New(ByVal secteur As Secteur)
            _secteur = secteur

            initWatchers(New KeyValuePair(Of String, Object)("Total", 0))
        End Sub

        Public Sub New(ByVal id As Integer, ByVal libelle As String)
            Me.New(New Secteur(id, libelle))
        End Sub

#Region "Properties"
        Public Property Id As Integer
            Get
                Return _secteur.Id
            End Get
            Set(ByVal value As Integer)
                _secteur.Id = value
            End Set
        End Property

        Public Property Libelle As String
            Get
                Return _secteur.Libelle
            End Get
            Set(ByVal value As String)
                _secteur.Libelle = value
            End Set
        End Property

        Public Property Total As Decimal
            Get
                Return getWatcher(Of Decimal)("Total")
            End Get
            Set(ByVal value As Decimal)
                setWatcher("Total", value)
            End Set
        End Property
#End Region ' !Properties

    End Class
End Namespace
