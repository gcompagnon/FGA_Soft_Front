Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace FileLink

    Public Class StockViewModel
        Implements INotifyPropertyChanged

        Private _stock As Stock
        Private _parent As StockViewModel
        Dim _children As ReadOnlyCollection(Of StockViewModel)

        Private _isSelected As Boolean
        Private _isExpanded As Boolean
        Private _isHighlighted As Boolean

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Sub New(ByRef stock As Stock)
            Me.New(stock, Nothing)
        End Sub

        Sub New(ByRef stock As Stock, ByRef parent As StockViewModel)
            _stock = stock
            _parent = parent

            _children = New ReadOnlyCollection(Of StockViewModel)((From child In _stock.Children
                                                                  Select New StockViewModel(child, Me)).ToList())
        End Sub

        Public Property IsSelected() As Boolean
            Get
                Return _isSelected
            End Get
            Set(ByVal value As Boolean)
                If Not value = _isSelected Then
                    _isSelected = value
                    Me.OnPropertyChanged("IsSelected")
                    IsHighlighted = True
                End If
            End Set
        End Property

        Public Property IsExpanded() As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                If Not value = _isExpanded Then
                    _isExpanded = value
                    Me.OnPropertyChanged("IsExpanded")
                End If

                ' Expand up to root
                If _isExpanded AndAlso _parent IsNot Nothing Then
                    _parent.IsExpanded = True
                End If
            End Set
        End Property

        Public Property IsHighlighted() As Boolean
            Get
                Return _isHighlighted
            End Get
            Set(ByVal value As Boolean)
                If Not value = _isHighlighted Then
                    _isHighlighted = value
                    Me.OnPropertyChanged("IsHighlighted")
                End If
            End Set
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return _stock.Name
            End Get
        End Property

        Public ReadOnly Property Id() As Integer
            Get
                Return _stock.Id
            End Get
        End Property

        Public ReadOnly Property Type() As StockType
            Get
                Return _stock.Type
            End Get
        End Property
        Public ReadOnly Property Parent() As StockViewModel
            Get
                Return _parent
            End Get
        End Property

        Public ReadOnly Property Children() As ReadOnlyCollection(Of StockViewModel)
            Get
                Return _children
            End Get
        End Property

        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Function getPath(ByVal withRoot As Boolean) As String
            If Parent IsNot Nothing AndAlso (withRoot OrElse Parent.Parent IsNot Nothing) Then
                Return Parent.getPath(withRoot) & "\" & Name
            Else
                Return Name
            End If
        End Function

        Public Sub linkFilesToStock(ByVal fullpaths As String())
            For Each fullpath As String In fullpaths
                linkFileToStock(System.IO.Path.GetDirectoryName(fullpath),
                                System.IO.Path.GetFileName(fullpath),
                                "")
            Next
        End Sub

        Public Sub linkFileToStock(ByVal url As String, ByVal filename As String, ByVal description As String)
            Dim co As New Connection()

            Dim id_icb As Integer = -1
            Dim id_fga As Integer = -1
            Dim id_val As Integer = -1

            Select Case Me.Type
                Case StockType.VALEUR
                    id_val = Me.Id
                Case StockType.FGA
                    id_fga = Me.Id
                Case StockType.ICB
                    id_icb = Me.Id
                Case Else
                    Return
            End Select

            co.ProcedureStockée("ACT_Add_File_Link", _
                                New List(Of String)({"@id_sector_icb",
                                                     "@id_sector_fga",
                                                     "@id_value",
                                                     "@url",
                                                     "@fname",
                                                     "@desc"}), _
                                New List(Of Object)({id_icb,
                                                     id_fga,
                                                     id_val,
                                                     url,
                                                     filename,
                                                     description}))
        End Sub
    End Class
End Namespace