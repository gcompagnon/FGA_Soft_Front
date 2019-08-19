Imports WindowsApplication1.Action.Note
Imports System.Collections.ObjectModel

Namespace Action.Note

    Public Class BaseActionNote

        Private _noteViewModel As New NoteViewModel
        Private _hasApply As Boolean = False

        Sub New(ByVal secteurFGA As String)
            Me.New()

            _noteViewModel.selectTable(secteurFGA)
        End Sub

        Public Sub New()

            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Me.DataContext = _noteViewModel
        End Sub

        Public ReadOnly Property HasApply As Boolean
            Get
                Return _hasApply
            End Get
        End Property

#Region "Events"
        ''' <summary>
        ''' Suppression d'une ligne
        ''' </summary>
        Private Sub DG_BDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim secteur As TableNoteModel = CBSelectedTable.SelectedValue
            If secteur Is Nothing Then
                Return
            End If

            ' Remove selected items
            Dim list = DGColumns.SelectedItems

            While list.Count > 0
                Dim column As ColumnNoteModel = list(0)
                list.Remove(column)
                secteur.Columns.Remove(column)
                _noteViewModel.removeColumn(secteur, column)
            End While

        End Sub

        ''' <summary>
        ''' Changement de table
        ''' </summary>
        Private Sub CBSelectedTable_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CBSelectedTable.SelectionChanged
            Dim table As TableNoteModel = CBSelectedTable.SelectedValue

            ' Update OtherColumns.
            If table IsNot Nothing Then
                _noteViewModel.fillOtherColumns(table)
                NoteViewModel.sortColumns(table.Columns, "Position", True)
            End If
        End Sub

        ''' <summary>
        ''' Déplacement d'une ligne vers le haut
        ''' </summary>
        Private Sub DG_BPosUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim secteur As TableNoteModel = CBSelectedTable.SelectedValue
            If secteur Is Nothing Then
                Return
            End If

            Dim columns As IOrderedEnumerable(Of ColumnNoteModel) = secteur.Columns.OrderBy(Function(x) x.Position)
            Dim selected As IOrderedEnumerable(Of ColumnNoteModel) = DGColumns.SelectedItems.Cast(Of ColumnNoteModel)().
                ToList.OrderBy(Function(x) x.Position)

            SwapPosition(selected.ToList, columns.ToList)

            ' Nasty but refresh UI
            secteur.Columns = secteur.Columns
        End Sub

        ''' <summary>
        ''' Déplacement d'une ligne vers le bas
        ''' </summary>
        Private Sub DG_BPosDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim secteur As TableNoteModel = CBSelectedTable.SelectedValue
            If secteur Is Nothing Then
                Return
            End If

            Dim columns As IOrderedEnumerable(Of ColumnNoteModel) = secteur.Columns.OrderByDescending(Function(x) x.Position)
            Dim selected As IOrderedEnumerable(Of ColumnNoteModel) = DGColumns.SelectedItems.Cast(Of ColumnNoteModel)().
                ToList.OrderByDescending(Function(x) x.Position)

            SwapPosition(selected.ToList, columns.ToList)

            ' Nasty but refresh UI
            secteur.Columns = secteur.Columns
        End Sub

        ''' <summary>
        ''' Ajoute une nouvelle colonne.
        ''' </summary>
        Private Sub BAddColumn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAddColumn.Click
            If CBSelectedTable.Text = "" Then
                MessageBox.Show("Veuillez sélectionner une table avant d'ajouter une colonne.")
                Return
            End If

            Dim table As TableNoteModel = CBSelectedTable.SelectedValue
            Dim column As ColumnNoteModel
            Dim position As Integer = 0

            ' Get last position
            If table.Columns.Count > 0 Then
                position = table.Columns.OrderBy(Function(x) x.Position).ToList.ElementAt(table.Columns.Count - 1).Position + 1
            End If

            If CBSelectedColumn.SelectedItem Is Nothing Then
                ' Add empty column
                column = New ColumnNoteModel(table, 0, "", True, True, Nothing, position)
            Else
                ' Add existing column (from another table)
                column = CBSelectedColumn.SelectedItem
                column = New ColumnNoteModel(table, 0, column.Name, True, column.Is_Note, column.Coef, position)
            End If

            ' Add new column at the end of the table
            table.AddColumn(column)
        End Sub

        ''' <summary>
        ''' Déselectionne sur la touche "Echappe"
        ''' </summary>
        Private Sub Window_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyDown
            If e.Key = Windows.Input.Key.Escape Then
                DGColumns.UnselectAll()
            End If
        End Sub

        Private Sub BApply_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BApply.Click
            _noteViewModel.saveAll()
            _hasApply = True
            Me.Close()
        End Sub

        Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCancel.Click
            Me.Close()
        End Sub
#End Region ' !Events

#Region "Methodes privees"
        ''' <summary>
        ''' Déplace selected colonnes de columns d'une position.
        ''' </summary>
        ''' <remarks>columns doit être trié en fonction d'un déplacement vers le bas ou vers le haut.</remarks>
        Private Sub SwapPosition(ByVal selected As List(Of ColumnNoteModel), ByVal columns As List(Of ColumnNoteModel))
            For i = 0 To selected.Count - 1
                Dim index As Integer = columns.IndexOf(selected(i))
                Dim old As ColumnNoteModel

                ' Ignore root element
                If index = 0 Then
                    Continue For
                End If

                old = columns(index - 1)

                ' Swap with unselected element
                If Not selected.Contains(old) Then
                    Dim tmp As Integer = old.Position
                    old.Position = selected(i).Position
                    selected(i).Position = tmp
                End If
            Next
        End Sub

#End Region ' !Methodes privees
    End Class
End Namespace
