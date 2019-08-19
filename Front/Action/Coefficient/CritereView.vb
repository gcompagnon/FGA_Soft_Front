Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports WindowsApplication1.TableDynamique

Namespace Action.Coefficient

    Public Class CritereView

        Private _coefViewModel As CoefViewModel
        Private _critere As CritereSupervisor

        Private _parent As CritereView
        Private _children As New List(Of CritereView)
        Private _coefs As New ObservableCollection(Of CoefView)

        Private Sub New(ByVal coefViewModel As CoefViewModel, ByVal parent As CritereView, ByVal critere As CritereSupervisor)
            _critere = critere
            _coefViewModel = coefViewModel
            Me.Parent = parent
            update()
        End Sub

        Public Sub New(ByVal coefViewModel As CoefViewModel, ByVal id As Integer, ByVal parent As CritereView, ByVal name As String, ByVal position As Integer, ByVal description As String, ByVal CAP_min As Double, ByVal CAP_max As Double)
            Me.New(coefViewModel, parent, New CritereSupervisor(id, name, position, description, CAP_min, CAP_max))
        End Sub

        Public Sub New(ByVal coefViewModel As CoefViewModel, ByVal id As Integer, ByVal parent As CritereView, ByVal name As String, ByVal position As Integer)
            Me.New(coefViewModel, id, parent, name, position, Nothing, 0, 100)
        End Sub

#Region "Properties"

        Public Property Id() As Integer
            Get
                Return _critere.Id
            End Get
            Set(ByVal value As Integer)
                _critere.Id = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _critere.Name
            End Get
            Set(ByVal value As String)
                _critere.Name = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _critere.Description
            End Get
            Set(ByVal value As String)
                _critere.Description = value
            End Set
        End Property

        Public Property Position() As Integer
            Get
                Return _critere.Position
            End Get
            Set(ByVal value As Integer)
                _critere.Position = value
            End Set
        End Property

        Public Property CAPMin() As Double?
            Get
                If IsLeaf Then
                    Return _critere.CAPMin
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Double?)
                If IsLeaf Then
                    _critere.CAPMin = value
                End If
            End Set
        End Property

        Public Property CAPMax() As Double?
            Get
                If IsLeaf Then
                    Return _critere.CAPMax
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Double?)
                If IsLeaf Then
                    _critere.CAPMax = value
                End If
            End Set
        End Property

        Public Property Group As Integer?
            Get
                If IsLeaf Then
                    Return _critere.Group
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Integer?)
                If IsLeaf Then
                    _critere.Group = value
                End If
            End Set
        End Property

        Public Property Format As ColumnFormat?
            Get
                If IsLeaf Then
                    Return _critere.Format
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As ColumnFormat?)
                If IsLeaf Then
                    _critere.Format = value
                End If
            End Set

        End Property

        Public Property Precision As Integer?
            Get
                If IsLeaf Then
                    Return _critere.Precision
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Integer?)
                If IsLeaf Then
                    _critere.Precision = value
                End If
            End Set

        End Property

        Public Property IsInverse As Boolean?
            Get
                If IsLeaf Then
                    Return _critere.IsInverse
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Boolean?)
                If IsLeaf Then
                    _critere.IsInverse = value
                End If
            End Set
        End Property

        Public Property Parent() As CritereView
            Get
                Return _parent
            End Get
            Set(ByVal value As CritereView)
                If _parent IsNot Nothing Then
                    ' changement de parent
                    Me.Position = 0
                    _parent.Children.Remove(Me)
                End If

                _parent = value
                If _parent Is Nothing Then
                    Level = 0
                Else
                    Level = Parent.Level + 1
                    _parent.addChild(Me)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Liste des critères enfants triées par position .
        ''' </summary>
        Public ReadOnly Property Children() As List(Of CritereView)
            Get
                Return _children
            End Get
        End Property

        Public Property Level() As Integer
            Get
                Return _critere.Level
            End Get
            Set(ByVal value As Integer)
                _critere.Level = value
            End Set
        End Property

        Public Property Coefs As ObservableCollection(Of CoefView)
            Get
                Return _coefs
            End Get
            Set(ByVal value As ObservableCollection(Of CoefView))
                _coefs = value
            End Set
        End Property

        Public ReadOnly Property CoefViewModel
            Get
                Return _coefViewModel
            End Get
        End Property

        Public ReadOnly Property HasChanged As Boolean
            Get
                Return _critere.HasChanged
            End Get
        End Property

        ''' <summary>
        ''' Return Weither the critere is a root or not.
        ''' </summary>
        Public ReadOnly Property IsRoot As Boolean
            Get
                Return CoefViewModel.isRoot(Me)
            End Get
        End Property

        ''' <summary>
        ''' Return Weither the critere parent is a root.
        ''' </summary>
        Public ReadOnly Property IsMiddle As Boolean
            Get
                Return Not IsRoot AndAlso Not IsLeaf
            End Get
        End Property

        ''' <summary>
        ''' Return Weither the critere is a leaf or not.
        ''' </summary>
        Public ReadOnly Property IsLeaf As Boolean
            Get
                Return CoefViewModel.isLeaf(Me)
            End Get
        End Property

#End Region ' !Properties

#Region "Methodes publiques"
        ''' <summary>
        ''' Récupère sous forme de liste l'objet actuel et l'ensemble de ses descendants triés par position.
        ''' </summary>
        Public Function toList() As List(Of CritereView)
            Dim list As New List(Of CritereView)

            list.Add(Me)
            For Each child As CritereView In Children.OrderBy(Function(x) x.Position)
                For Each elt As CritereView In child.toList
                    list.Add(elt)
                Next
            Next

            Return list
        End Function

        ''' <summary>
        ''' Ajoute child à Children, met à jour son parent et sa position puis trie la liste selon la position des enfants.
        ''' </summary>
        Public Sub addChild(ByVal child As CritereView)
            If Not Children.Contains(child) Then
                If child.Position = 0 Then
                    ' Set child position at the end of Children.
                    child.Position = 1

                    If Children.Count > 0 Then
                        child.Position += Children.Max(Function(c As CritereView) c.Position)
                    End If
                End If

                ' Insert child keeping order in children position
                Dim i = 0
                While i < Children.Count AndAlso child.Position > Children(i).Position
                    i += 1
                End While

                Children.Insert(i, child)
            End If
        End Sub

        ''' <summary>
        ''' Met à jour les données.
        ''' </summary>
        Public Sub update()
            _critere.update()
        End Sub
#End Region ' !Methodes publiques

        Public Shared Sub swap_crit(ByVal critere As CritereView, ByVal swap_crit As CritereView)
            If critere.Parent.Equals(swap_crit.Parent) Then
                Dim tmp = critere.Position
                critere.Position = swap_crit.Position
                swap_crit.Position = tmp

                critere.Parent.Children.Remove(critere)
                critere.Parent.Children.Remove(swap_crit)
                critere.Parent.addChild(critere)
                critere.Parent.addChild(swap_crit)
            Else
                ' Change critere's parent
                critere.Parent = swap_crit.Parent
            End If
        End Sub

    End Class

End Namespace
