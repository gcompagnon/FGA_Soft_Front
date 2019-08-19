Imports System.Windows.Controls
Imports System.Collections.ObjectModel

Namespace Action.Consultation
    Public Class GridConfig
        Public _viewModel As GridConfigViewModel

        Property Prop As String

        Sub New()
            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            _viewModel = Me.DataContext
        End Sub


        ''' <summary>
        ''' Monte la catégorie ou le critère d'une position
        ''' </summary>
        ''' <remarks>Des changements de catégorie ou de root peuvent avoir lieu</remarks>
        Private Sub DG_BPosUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            If DGColumn.SelectedCells.Count = 0 OrElse Not TypeOf DGColumn.SelectedCells(0).Item Is ColumnConfig Then
                Return
            End If

            swap(_viewModel.Columns, _viewModel.Columns.IndexOf(DGColumn.SelectedCells(0).Item), False)
        End Sub

        ''' <summary>
        ''' Descend la catégorie ou le critère d'une position
        ''' </summary>
        ''' <remarks>Des changements de catégorie ou de root peuvent avoir lieu</remarks>
        Private Sub DG_BPosDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            If DGColumn.SelectedCells.Count = 0 OrElse Not TypeOf DGColumn.SelectedCells(0).Item Is ColumnConfig Then
                Return
            End If

            swap(_viewModel.Columns, _viewModel.Columns.IndexOf(DGColumn.SelectedCells(0).Item), True)
        End Sub

        Private Sub swap(ByVal collection As ObservableCollection(Of ColumnConfig), ByVal index As Integer, ByVal isDown As Boolean)
            Dim delta As Integer = 0

            If isDown Then
                delta = 1
            End If

            If index < 1 - delta Or index >= collection.Count - delta Then
                Return
            End If

            Dim dest As Integer = index - 1 + 2 * delta
            Dim swap As ColumnConfig = _viewModel.Columns(dest)
            _viewModel.Columns(dest) = _viewModel.Columns(index)
            _viewModel.Columns(index) = swap
            Me.UpdateLayout()
        End Sub

        Private Sub DeleteCriteria_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            For Each cell As DataGridCellInfo In DGColumn.SelectedCells
                If TypeOf cell.Item Is ColumnConfig Then
                    _viewModel.Columns.Remove(cell.Item)
                End If
            Next

        End Sub

        Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles userControl.Loaded
            _viewModel.Fill()
        End Sub
    End Class
End Namespace