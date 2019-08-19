Imports WindowsApplication1.TableDynamique

Namespace Action.Coefficient

    Public Class CritereSupervisor
        Inherits Watcher.Supervisor

        Public Sub New(ByVal id As Integer, ByVal name As String, ByVal position As Integer, ByVal description As String, ByVal CAP_min As Double?, ByVal CAP_max As Double?)
            Me.Id = id

            initWatchers(New KeyValuePair(Of String, Object)("Name", name),
                         New KeyValuePair(Of String, Object)("Level", 0),
                         New KeyValuePair(Of String, Object)("Position", position),
                         New KeyValuePair(Of String, Object)("Description", description),
                         New KeyValuePair(Of String, Object)("CAPMin", CAP_min),
                         New KeyValuePair(Of String, Object)("CAPMax", CAP_max),
                         New KeyValuePair(Of String, Object)("Format", ColumnFormat.Normal),
                         New KeyValuePair(Of String, Object)("Precision", 0),
                         New KeyValuePair(Of String, Object)("Group", 0),
                         New KeyValuePair(Of String, Object)("IsInverse", False))
        End Sub

#Region "Properties"
        Public Property Id() As Integer

        Public Property Name() As String
            Get
                Return getWatcher(Of String)("Name")
            End Get
            Set(ByVal value As String)
                setWatcher("Name", value)
            End Set
        End Property

        Public Property Level() As Integer
            Get
                Return getWatcher(Of Integer)("Level")
            End Get
            Set(ByVal value As Integer)
                setWatcher(Of Integer)("Level", value)
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

        Public Property Description() As String
            Get
                Return getWatcher(Of String)("Description")
            End Get
            Set(ByVal value As String)
                setWatcher("Description", value)
            End Set
        End Property

        Public Property CAPMin() As Double?
            Get
                Return getWatcher(Of Double?)("CAPMin")
            End Get
            Set(ByVal value As Double?)
                setWatcher("CAPMin", value)
            End Set
        End Property

        Public Property CAPMax() As Double?
            Get
                Return getWatcher(Of Double?)("CAPMax")
            End Get
            Set(ByVal value As Double?)
                setWatcher("CAPMax", value)
            End Set
        End Property

        Public Property Format() As ColumnFormat
            Get
                Return getWatcher(Of ColumnFormat)("Format")
            End Get
            Set(ByVal value As ColumnFormat)
                setWatcher("Format", value)
            End Set
        End Property

        Public Property Precision() As Integer?
            Get
                Return getWatcher(Of Integer?)("Precision")
            End Get
            Set(ByVal value As Integer?)
                setWatcher("Precision", value)
            End Set
        End Property

        Public Property Group() As Integer?
            Get
                Return getWatcher(Of Integer?)("Group")
            End Get
            Set(ByVal value As Integer?)
                setWatcher("Group", value)
            End Set
        End Property

        Public Property IsInverse() As Boolean?
            Get
                Return getWatcher(Of Boolean?)("IsInverse")
            End Get
            Set(ByVal value As Boolean?)
                setWatcher("IsInverse", value)
            End Set
        End Property

#End Region ' !Properties
    End Class

End Namespace
