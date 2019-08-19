Namespace Watcher

    Public Class Watcher(Of T)
        Implements IWatcher

        Public Sub New()
        End Sub

        Public Sub New(ByVal data As T)
            Me.Data = data
            Me.OldData = Me.Data
        End Sub

#Region "Properties"
        Public Property Data As T
        Private Property OldData As T

        Public ReadOnly Property HasChanged As Boolean Implements IWatcher.HasChanged
            Get
                If Me.Data Is Nothing Then
                    If Me.OldData Is Nothing Then
                        Return False
                    Else
                        Return True
                    End If
                Else
                    Return Not Me.Data.Equals(Me.OldData)
                End If
            End Get
        End Property
#End Region ' !Properties

#Region "Méthodes publiques"
        Public Sub update() Implements IWatcher.update
            Me.OldData = Me.Data
        End Sub

        Public Sub update(ByVal value As T)
            Me.Data = value
            update()
        End Sub
#End Region ' !Méthodes publiques

    End Class

End Namespace
