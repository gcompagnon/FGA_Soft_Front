Imports System.Collections.ObjectModel

Namespace Action

    Public Class Table

        Public Property Headers As ObservableCollection(Of Cell)

        Public Property Columns As ObservableCollection(Of Column)


        Public Sub New()
            Columns = New ObservableCollection(Of Column)
            Headers = New ObservableCollection(Of Cell)
        End Sub

    End Class

End Namespace