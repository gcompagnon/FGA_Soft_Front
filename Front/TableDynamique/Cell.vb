Imports System.ComponentModel

Namespace Action

    Public MustInherit Class Cell
        Implements INotifyPropertyChanged

        Private _oldData As String

        Public MustOverride Property Data As String

        Public ReadOnly Property DataHasChanged As Boolean
            Get
                Return Data <> OldData
            End Get
        End Property

        Public Property OldData As String
            Get
                Return _oldData
            End Get
            Set(ByVal value As String)
                _oldData = value
                OnPropertyChanged("OldData")
            End Set
        End Property

        Protected Sub New(ByVal oldData As String)
            Me.OldData = oldData
        End Sub

        Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            If Not String.IsNullOrEmpty(propertyName) Then
                RaiseEvent PropertyChanged(Me,
                          New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class

End Namespace
