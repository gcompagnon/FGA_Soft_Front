Namespace Utilities

    Public Class TreeAction
        Inherits Utilities.Tree(Of String)

        Private _root As TreeNodeAction

        Public Sub New()
        End Sub

        Public Overloads ReadOnly Property Root() As TreeNodeAction
            Get
                Return _root
            End Get
        End Property

        Public Sub New(ByVal val As String, ByVal id As Integer)
            _root = New TreeNodeAction(val, id, 0)
        End Sub

        Public Overloads Function addChild(ByVal val As String, ByVal id As Integer) As TreeNodeAction
            Return _root.addChild(val, id)
        End Function
    End Class

    Public Class TreeNodeAction
        Inherits Utilities.TreeNode(Of String)

        Private _id As Integer
        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Sub New(ByVal val As String, ByVal id As Integer, ByVal height As Integer)
            MyBase.New(val, height)

            _id = id
        End Sub

        Public Overloads Function addChild(ByVal val As String, ByVal id As Integer) As TreeNodeAction
            Dim child As New TreeNodeAction(val, id, Height + 1)

            addChild(child)

            Return child
        End Function

        Public Function getChildrenSorted() As List(Of TreeNodeAction)
            Dim childrenSorted As New List(Of TreeNodeAction)

            For Each child As TreeNodeAction In Children
                Dim i = 0
                While i < childrenSorted.Count AndAlso child.Value > childrenSorted(i).Value
                    i += 1
                End While

                childrenSorted.Insert(i, child)
            Next

            Return childrenSorted
        End Function
    End Class
End Namespace