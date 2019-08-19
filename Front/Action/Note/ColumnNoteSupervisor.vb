
Namespace Action.Note
    Public Class ColumnNoteSupervisor
        Inherits Watcher.Supervisor

        Public Sub New(ByVal id As Integer, ByVal name As String, ByVal is_activated As Boolean, ByVal is_note As Boolean, ByVal coef As Double?, ByVal position As Integer)
            Me.Id = id

            initWatchers(New KeyValuePair(Of String, Object)("Name", name),
                         New KeyValuePair(Of String, Object)("Is_Activated", is_activated),
                         New KeyValuePair(Of String, Object)("Is_Note", is_note),
                         New KeyValuePair(Of String, Object)("Coef", coef),
                         New KeyValuePair(Of String, Object)("Position", position))
        End Sub

        Public Sub New()
            Me.New(0, Nothing, False, False, Nothing, 0)
        End Sub

#Region "Properties"
        Public Property Id As Integer

        Public Property Name() As String
            Get
                Return getWatcher(Of String)("Name")
            End Get
            Set(ByVal value As String)
                setWatcher("Name", value)
            End Set
        End Property

        Public Property Is_Activated As Boolean
            Get
                Return getWatcher(Of Boolean)("Is_Activated")
            End Get
            Set(ByVal value As Boolean)
                setWatcher("Is_Activated", value)
            End Set
        End Property

        Public Property Is_Note As Boolean
            Get
                Return getWatcher(Of Boolean)("Is_Note")
            End Get
            Set(ByVal value As Boolean)
                setWatcher("Is_Note", value)
            End Set
        End Property

        Public Property Coef As Double?
            Get
                Return getWatcher(Of Double?)("Coef")
            End Get
            Set(ByVal value As Double?)
                setWatcher("Coef", value)
            End Set
        End Property

        Public Property Position As Integer
            Get
                Return getWatcher(Of Integer)("Position")
            End Get
            Set(ByVal value As Integer)
                setWatcher("Position", value)
            End Set
        End Property
#End Region ' !Properties

    End Class
End Namespace