Imports System.Collections.ObjectModel
Imports System.Windows.Input

Namespace FileLink

    Class StockTreeViewModel

        Private _root As StockViewModel
        Private _topSector As ReadOnlyCollection(Of StockViewModel)
        Public ReadOnly Property TopSector As ReadOnlyCollection(Of StockViewModel)
            Get
                Return _topSector
            End Get
        End Property

        Sub New(ByVal root As Stock)
            _root = New StockViewModel(root)

            _root.IsExpanded = True
            _topSector = New ReadOnlyCollection(Of StockViewModel)({_root})

            _searchCommand = New SearchStockTreeCommand(Me)
        End Sub

#Region "Search Stock"
        Private _searchText As String = ""
        Public Property SearchText() As String
            Get
                Return _searchText
            End Get
            Set(ByVal value As String)
                If value.ToUpper = _searchText Then
                    Return
                End If

                _searchText = value.ToUpper
                _matchingStockEnumerator = Nothing
            End Set
        End Property

        Private _searchCommand As SearchStockTreeCommand
        Public Property SearchCommand() As ICommand
            Get
                Return _searchCommand
            End Get
            Set(ByVal value As ICommand)
                _searchCommand = value
            End Set
        End Property

        Private _matchingStockEnumerator As IEnumerator(Of StockViewModel)

        Sub PerformSearch()
            If _matchingStockEnumerator Is Nothing OrElse Not _matchingStockEnumerator.MoveNext Then
                Me.VerifyMatchingStockEnumerator()
            End If

            Dim stock = _matchingStockEnumerator.Current

            If stock Is Nothing Then
                Return
            End If

            If stock.Parent IsNot Nothing Then
                stock.Parent.IsExpanded = True
            End If

            stock.IsSelected = True
        End Sub

        Private Sub VerifyMatchingStockEnumerator()
            Dim matches As IEnumerable(Of StockViewModel) = FindMatches(_searchText, _root)
            _matchingStockEnumerator = matches.GetEnumerator

            If Not _matchingStockEnumerator.MoveNext Then
                MessageBox.Show("Aucun nom trouvé. Veuillez vérifier votre recherche.",
                                "Les critères de recherche n'ont retourné aucune valeur",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information)
            End If
        End Sub

        Private Function FindMatches(ByVal searchText As String, ByVal stock As StockViewModel) As IEnumerable(Of StockViewModel)
            FindMatches = New List(Of StockViewModel)

            If stock.Name.ToUpper.Contains(searchText) Then
                DirectCast(FindMatches, List(Of StockViewModel)).Add(stock)
            End If

            For Each child In stock.Children
                For Each match In FindMatches(searchText, child)
                    DirectCast(FindMatches, List(Of StockViewModel)).Add(match)
                Next
            Next
        End Function

        Private Class SearchStockTreeCommand
            Implements ICommand

            ReadOnly _sectorTree As StockTreeViewModel

            Sub New(ByVal sectorTree As StockTreeViewModel)
                _sectorTree = sectorTree
            End Sub

            Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
                Return True
            End Function

            Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
                AddHandler(ByVal value As EventHandler)
                End AddHandler

                RemoveHandler(ByVal value As EventHandler)
                End RemoveHandler

                RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)

                End RaiseEvent
            End Event

            Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
                _sectorTree.PerformSearch()
            End Sub

        End Class
#End Region

    End Class
End Namespace