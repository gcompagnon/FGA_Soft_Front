Namespace FileLink

    Public Enum StockType
        ICB
        FGA
        VALEUR
        NONE
    End Enum

    Public Class Stock

        Private _children As List(Of Stock)
        Private _name As String
        Private _id As Integer
        Private _type As StockType

        Sub New(ByVal valeur As Object, ByVal id As Integer, ByVal type As StockType)
            Me.New(valeur, id, type, New List(Of Stock))
        End Sub

        Public Sub New(ByVal name As String, ByVal id As Integer, ByVal type As StockType, ByVal children As List(Of Stock))
            _name = name
            _id = id
            _type = type
            _children = children
        End Sub

        Public ReadOnly Property Children() As List(Of Stock)
            Get
                Return _children
            End Get
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property Type() As StockType
            Get
                Return _type
            End Get
            Set(ByVal value As StockType)
                _type = value
            End Set
        End Property

    End Class
End Namespace
