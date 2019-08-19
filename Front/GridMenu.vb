Public Class GridMenu
    Inherits ContextMenuStrip

    Private _cell As DataGridViewCell

    Sub New(ByVal components As System.ComponentModel.IContainer)
        MyBase.New(components)
    End Sub

    Sub New()
        MyBase.New()
    End Sub

    Public Sub linkCell(ByVal linkedCell As DataGridViewCell)
        _cell = linkedCell
    End Sub

    Public Function getcell() As DataGridViewCell
        Return _cell
    End Function

    ''' <summary>
    ''' Affiche le menu contextuel au niveau du curseur de la souris.
    ''' </summary>
    ''' <param name="cell">La cellule cliquée</param>
    Public Sub popContextMenu(ByVal cell As DataGridViewCell)
        Dim grid As DataGridView = cell.DataGridView
        Dim position As Point = grid.PointToClient(Cursor.Position)

        linkCell(cell)
        Show(grid, position)
    End Sub
End Class
