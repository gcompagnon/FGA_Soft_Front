Public Class Tree(Of T)
    Private _root As TreeNode(Of T)

    Public Sub New()
    End Sub

    Public ReadOnly Property Root() As TreeNode(Of T)
        Get
            Return _root
        End Get
    End Property

    Public Sub New(ByVal val As T)
        _root = New TreeNode(Of T)(val, 0)
    End Sub

    Public Function addChild(ByVal child As T) As TreeNode(Of T)
        Return _root.addChild(child)
    End Function

    Public Sub clear()
        _root.clear()
    End Sub
End Class

Public Class TreeNode(Of T)
    Private _value As T
    Private _childTable As New Hashtable()
    ' Profondeur du noeud.
    Private _height As Integer

    Public ReadOnly Property Value As T
        Get
            Return _value
        End Get
    End Property

    Protected ReadOnly Property ChildTable As Hashtable
        Get
            Return _childTable
        End Get
    End Property


    Public ReadOnly Property Children As ICollection
        Get
            Return _childTable.Values
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return _height
        End Get
    End Property

    Friend Sub New(ByVal val As T, ByVal height As Integer)
        _value = val
        _height = height
    End Sub

    Sub clear()
        _value = Nothing

        For Each child As TreeNode(Of T) In Children
            child.clear()
        Next
        _childTable.Clear()
    End Sub

    Public Function addChild(ByVal child_value As T) As TreeNode(Of T)
        Dim child As New TreeNode(Of T)(child_value, _height + 1)

        _childTable.Add(child_value, child)

        Return child
    End Function

    Protected Sub addChild(ByRef child As TreeNode(Of T))
        _childTable.Add(child.Value, child)
    End Sub

    Private Function getChild(ByVal key As T) As TreeNode(Of T)
        Return _childTable(key)
    End Function

    Function getChildrenToString() As List(Of String)
        Dim res As New List(Of String)

        For Each child As TreeNode(Of T) In Children
            res.Add(child.ToString)
        Next

        Return res
    End Function

    Function getChildrenMaxHeight() As Integer
        Dim h As Integer = Height

        For Each child As TreeNode(Of T) In Children
            h = Math.Max(h, child.getChildrenMaxHeight())
        Next

        Return h
    End Function

End Class