Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Data.SqlClient

Namespace Action.Consultation
    Public Class GridConfigViewModel

        Private co As New Connection
        Property Columns As New ObservableCollection(Of ColumnConfig)
        Property ColumnNames As ObservableCollection(Of String)

        Sub Fill()
            FillColumnNames()
        End Sub

#Region "DatabaseAccess"

        Sub FillColumnNames()
            'ColumnNames = New ObservableCollection(Of String)(co.SelectColonneName("DATA_FACTSET", , True))
            ColumnNames = New ObservableCollection(Of String)(co.SelectColonneName2())

            'ColumnNames.Clear()

            'For Each n As String In co.SelectColonneName("DATA_FACTSET", , True)
            '    ColumnNames.Add(n)
            'Next
        End Sub

#End Region ' !DatabaseAccess
    End Class
End Namespace
