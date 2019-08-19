Imports System.Collections.ObjectModel
Imports System.Windows.Controls
Imports System.Windows
Imports System.Windows.Input
Imports System.Collections.Specialized
Imports System.Reflection


''' <summary>
''' This is a TreeViewMultiSelect class I wrote. I have in my head an idea
''' for a better one that is based on a ListView, but I haven't had time to
''' write it. The idea for the ListView involves having a sub list of all
''' the tree structure passed in. Items are added and removed as nodes are
''' opened and closed. The sub list wraps the original objects with an object
''' that has an indentation property and the item styling indents according
''' to this property.
''' </summary>

Public Class TreeViewMultiSelect
    Inherits TreeView
    ' ********************************************************************
    ' Private Fields
    ' ********************************************************************
#Region "Private Fields"

    ''' <summary>
    ''' Used for selecting a node by item
    ''' </summary>
    Private _itemToSelectByPath As ObservableCollection(Of Object) = Nothing
    Private _lastItemToSelectControl As ItemsControl = Nothing
    ''' <summary>
    ''' The previous item selected in multi-select
    ''' </summary>
    Private _previousItem As TreeViewItemMultiSelect = Nothing
    ''' <summary>
    ''' The collection of selected TreeViewItems
    ''' </summary>
    Private _selectedTreeViewItems As ObservableCollection(Of TreeViewItemMultiSelect)
    Private _updatingFromSelectedTreeViewItems As Boolean = False
    ''' <summary>
    ''' The collection of selected data items
    ''' </summary>
    Private _selectedItems As ObservableCollection(Of Object)
    Private _updatingFromSelectedItems As Boolean = False
    ''' <summary>
    ''' Reflection property for setting a private property
    ''' </summary>
    Private _isSelectionChangeActiveProperty As PropertyInfo

    Private _itemToTreeViewItem As Dictionary(Of Object, TreeViewItemMultiSelect)

#End Region

    ' ********************************************************************
    ' Public Methods
    ' ********************************************************************
#Region "Public Methods"

    ''' <summary>
    ''' Constructor. Initializes class fields.
    ''' </summary>
    Public Sub New()
        _itemToSelectByPath = New ObservableCollection(Of Object)()

        _isSelectionChangeActiveProperty = GetType(TreeView).GetProperty("IsSelectionChangeActive", BindingFlags.NonPublic Or BindingFlags.Instance)

        ' Set up collections
        _itemToTreeViewItem = New Dictionary(Of Object, TreeViewItemMultiSelect)()

        _selectedItems = New ObservableCollection(Of Object)()
        _selectedTreeViewItems = New ObservableCollection(Of TreeViewItemMultiSelect)()

        ' Hook into events

        AddHandler _selectedItems.CollectionChanged, AddressOf selectedItems_CollectionChanged
        AddHandler _selectedTreeViewItems.CollectionChanged, AddressOf selectedTreeViewItems_CollectionChanged
    End Sub

    ''' <summary>
    ''' Not implemented yet.
    ''' Better to use select item by path-chain, but this can be used to
    ''' select a data item. Does it really slowly, scan the data for the item,
    ''' gets the tree node chain to it, and selects it.
    ''' </summary>
    ''' <param name="userItemToSelect"></param>
    Public Sub SelectItem(ByVal userItemToSelect As Object)
        If Not ItemsSource Is Nothing Then
            Dim userItemPathToSelect As New ArrayList()
            Dim enumeratorStack As New Stack(Of IEnumerator)()
            Dim currentEnumerator As IEnumerator = ItemsSource.GetEnumerator()

            ' TODO work out how get the next collection down from the binding
            'NotFinished:
            'goto NotFinished;

            If userItemPathToSelect.Count > 0 Then
                SelectItemByPath(userItemPathToSelect)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Removes the item passed in to from the selected items
    ''' </summary>
    ''' <param name="itemToDeselect"></param>
    Public Sub DeselectItem(ByVal itemToDeselect As Object)
        If _itemToTreeViewItem.ContainsKey(itemToDeselect) Then
            Dim treeViewItem As TreeViewItemMultiSelect = _itemToTreeViewItem(itemToDeselect)
            treeViewItem.IsSelected = False
            If _selectedTreeViewItems.Contains(treeViewItem) Then
                _selectedTreeViewItems.Remove(treeViewItem)
            End If
        End If
    End Sub

    Public Sub SelectItemByPath(ByVal userItemToSelectByPath As ArrayList)
        _itemToSelectByPath.Clear()
        For Each item As Object In userItemToSelectByPath
            _itemToSelectByPath.Add(item)
        Next
        _lastItemToSelectControl = Me
        If Not SelectItemByPath(_lastItemToSelectControl, _itemToSelectByPath) Then
            AddHandler MyBase.ItemContainerGenerator.StatusChanged, AddressOf ItemContainerGenerator_StatusChanged
        End If
    End Sub

    ''' <summary>
    ''' Selects the first node in the tree view returns true if the
    ''' selection was successful or false if this needs to be called
    ''' again when the base.ItemContainerGenerator.StatusChanged event
    ''' is fired. Note if false is returned the same items control (which
    ''' will be modified as it'a a ref) needs to be passed in next time
    ''' to complete the selection.
    ''' </summary>
    ''' <returns></returns>
    Friend Shared Function SelectItemByPath(ByRef itemsControl As ItemsControl, ByVal itemToSelectByPath As IList) As Boolean
        Dim source As IList = TryCast(itemsControl.ItemsSource, IList)

        If source Is Nothing OrElse itemToSelectByPath.Count < 1 Then
            itemToSelectByPath.Clear()
            Return True
        End If
        If Not source.Contains(itemToSelectByPath(0)) Then
            itemToSelectByPath.Clear()
            Return True
        End If

        If itemsControl.HasItems Then
            ' Check that the items container generator has been started.
            ' If so then just select the item, if not then return false;

            Dim item As TreeViewItem = TryCast(itemsControl.ItemContainerGenerator.ContainerFromItem(itemToSelectByPath(0)), TreeViewItem)
            If Not item Is Nothing Then
                If itemToSelectByPath.Count = 1 Then
                    itemToSelectByPath.RemoveAt(0)
                    item.IsSelected = True
                    item.BringIntoView()
                    Return True
                Else
                    itemToSelectByPath.RemoveAt(0)
                    item.IsExpanded = True
                    itemsControl = item
                    Return SelectItemByPath(itemsControl, itemToSelectByPath)
                End If
            End If
        End If
        Return False
    End Function

    ''' <summary>
    ''' Adds a tree view item to the multi-selection without going through
    ''' the normal event chain. Used only by TreeViewItemMultiSelect.
    ''' </summary>
    ''' <param name="newItem"></param>
    Friend Sub AddTreeViewItemToSelection(ByVal newItem As TreeViewItemMultiSelect)
        If Not _selectedTreeViewItems.Contains(newItem) Then
            _selectedTreeViewItems.Add(newItem)
        End If
        _previousItem = newItem
    End Sub

#End Region

    ' ********************************************************************
    ' Protected Methods
    ' ********************************************************************
#Region "Protected Methods"

    ''' <summary>
    ''' Gets an empty container for puting in the tree.
    ''' </summary>
    ''' <returns></returns>
    Protected Overloads Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New TreeViewItemMultiSelect(Me)
    End Function

    ''' <summary>
    ''' Handles when the selected item changes. Handles the multi-select functionality.
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overloads Overrides Sub OnSelectedItemChanged(ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        Dim newItem As TreeViewItemMultiSelect = DirectCast(GetType(TreeView).GetField("_selectedContainer", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(DirectCast(Me, TreeView)), TreeViewItemMultiSelect)

        ' Turn off updating from TreeViewItems
        _isSelectionChangeActiveProperty.SetValue(DirectCast(Me, TreeView), True, Nothing)
        If (Keyboard.Modifiers And ModifierKeys.Control) = ModifierKeys.Control Then
            ' We are either :
            ' a ) Adding a new item so newItem!=null & !selectedTreeViewItems.Contains(newItem)
            ' b ) Removing the last selection so newItem==null;
            ' c ) Removing a previous selection selectedTreeViewItems.Contains(newItem)


            If Not newItem Is Nothing AndAlso Not _selectedTreeViewItems.Contains(newItem) Then
                ' Adding a new item
                If Not _previousItem Is Nothing Then
                    _previousItem.IsSelected = True
                End If
                _selectedTreeViewItems.Add(newItem)
                _previousItem = newItem
            ElseIf newItem Is Nothing AndAlso _previousItem IsNot Nothing Then
                ' Removing the last selected item

                _selectedTreeViewItems.Remove(_previousItem)
                _previousItem = Nothing
            ElseIf _selectedTreeViewItems.Contains(newItem) Then
                ' Removing a previously selected item

                newItem.IsSelected = False
                _selectedTreeViewItems.Remove(newItem)

                ' The selection needs to be put back on the last selected item
                If Not _previousItem Is Nothing Then
                    _previousItem.IsSelected = True
                End If
                _previousItem = Nothing
            End If
        Else
            While _selectedTreeViewItems.Count > 0
                Dim firstItem As TreeViewItemMultiSelect = _selectedTreeViewItems(0)
                If firstItem.IsSelected AndAlso firstItem IsNot newItem Then
                    firstItem.IsSelected = False
                End If
                _selectedTreeViewItems.RemoveAt(0)
            End While
            If Not newItem Is Nothing Then
                _selectedTreeViewItems.Add(newItem)
            End If

            _previousItem = newItem
        End If
        ' Turn back on updating from TreeViewItems
        _isSelectionChangeActiveProperty.SetValue(DirectCast(Me, TreeView), False, Nothing)

        ' when the newItem is null that's when the last item was deselected.
        MyBase.OnSelectedItemChanged(e)
    End Sub

#End Region

    ' ********************************************************************
    ' Properties
    ' ********************************************************************
#Region "Properties"

    ''' <summary>
    ''' Gets the collection of selected items
    ''' </summary>
    Public ReadOnly Property SelectedItems() As ObservableCollection(Of Object)
        Get
            Return _selectedItems
        End Get
    End Property

    ''' <summary>
    ''' Gets the collection of selected items
    ''' </summary>
    Public ReadOnly Property SelectedTreeViewItems() As ObservableCollection(Of TreeViewItemMultiSelect)
        Get
            Return _selectedTreeViewItems
        End Get
    End Property

#End Region

    ' ********************************************************************
    ' Events And Event Handlers
    ' ********************************************************************

    ''' <summary>
    ''' Handles when a node gets expanded when we are waiting to select
    ''' a lower node.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ItemContainerGenerator_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)
        RemoveHandler _lastItemToSelectControl.ItemContainerGenerator.StatusChanged, AddressOf ItemContainerGenerator_StatusChanged
        If Not SelectItemByPath(_lastItemToSelectControl, _itemToSelectByPath) Then
            AddHandler _lastItemToSelectControl.ItemContainerGenerator.StatusChanged, AddressOf ItemContainerGenerator_StatusChanged
        End If
    End Sub

    ''' <summary>
    ''' Handles when the selected tree view items collections changes. Updates
    ''' the data item collection accordingly.
    ''' </summary>
    Private Sub selectedTreeViewItems_CollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
        _updatingFromSelectedTreeViewItems = True

        Dim userItems As New List(Of Object)()

        Dim treeViewitems As New List(Of TreeViewItemMultiSelect)(_selectedTreeViewItems)

        For Each treeViewItem As TreeViewItemMultiSelect In treeViewitems
            If Not treeViewItem.Header Is Nothing Then
                If Not _updatingFromSelectedItems Then
                    userItems.Add(treeViewItem.Header)
                    If Not _selectedItems.Contains(treeViewItem.Header) Then
                        _selectedItems.Add(treeViewItem.Header)
                        _itemToTreeViewItem.Add(treeViewItem.Header, treeViewItem)
                    End If
                End If
            Else
                _selectedTreeViewItems.Remove(treeViewItem)
            End If
        Next

        If Not _updatingFromSelectedItems Then
            Dim testItems As New List(Of Object)(_selectedItems)
            For Each item As Object In testItems
                If Not userItems.Contains(item) Then
                    _selectedItems.Remove(item)
                End If
            Next
        End If

        _updatingFromSelectedTreeViewItems = False
    End Sub

    Sub selectedItems_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
        _updatingFromSelectedItems = True
        If Not _updatingFromSelectedTreeViewItems Then
            Select Case e.Action
                Case NotifyCollectionChangedAction.Add
                    For Each item As Object In e.NewItems
                        SelectItem(item)
                    Next
                Case NotifyCollectionChangedAction.Remove
                    For Each item As Object In e.OldItems
                        DeselectItem(item)

                    Next
                Case Else
            End Select
        End If

        ' Not real happy with this solution.
        Select Case e.Action
            Case NotifyCollectionChangedAction.Remove
                For Each item As Object In e.OldItems
                    _itemToTreeViewItem.Remove(item)
                Next
            Case Else
        End Select
        _updatingFromSelectedItems = False
    End Sub
End Class


Public Class TreeViewItemMultiSelect

    Inherits TreeViewItem
    ''' <summary>
    ''' The parent tree view control
    ''' </summary>
    Private parentTreeView As TreeViewMultiSelect

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="parentTreeView"></param>
    Public Sub New(ByVal parentTreeView As TreeViewMultiSelect)
        Me.parentTreeView = parentTreeView
    End Sub

    ''' <summary>
    ''' Generates controls that hold data in the treeview
    ''' </summary>
    Protected Overloads Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New TreeViewItemMultiSelect(parentTreeView)
    End Function

    ''' <summary>
    ''' Handles when the mouse down is clicked
    ''' </summary>
    Protected Overloads Overrides Sub OnMouseLeftButtonDown(ByVal e As MouseButtonEventArgs)
        If Me.IsSelected AndAlso ((Keyboard.Modifiers And ModifierKeys.Control) = ModifierKeys.Control) Then
            Me.IsSelected = False
        ElseIf Not IsSelected AndAlso IsFocused Then
            Me.IsSelected = True

            ' This overcomes a corner case where the parent tree list isn't updated
            parentTreeView.AddTreeViewItemToSelection(Me)
        End If
        MyBase.OnMouseLeftButtonDown(e)
    End Sub
End Class