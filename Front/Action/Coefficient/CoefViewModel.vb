Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Action.Coefficient
    Public MustInherit Class CoefViewModel
        Inherits Watcher.Supervisor

        Protected co As New Connection()
        Protected _is_sect As Boolean = False
        Dim isSimul As Boolean

        Public Sub New()
            initWatchers(New KeyValuePair(Of String, Object)("CritereList", New ObservableCollection(Of CritereView)),
                         New KeyValuePair(Of String, Object)("SecteurList", New List(Of Secteur)),
                         New KeyValuePair(Of String, Object)("AutoCompute", True),
                         New KeyValuePair(Of String, Object)("ShowDescription", False),
                         New KeyValuePair(Of String, Object)("Total", 0))

            populateCriteres()
            populateSecteurs(False)
            fillCriteresCoefs()
            computeTotal()
        End Sub

        Public Sub New(isSimul As Boolean)
            initWatchers(New KeyValuePair(Of String, Object)("CritereList", New ObservableCollection(Of CritereView)),
                         New KeyValuePair(Of String, Object)("SecteurList", New List(Of Secteur)),
                         New KeyValuePair(Of String, Object)("AutoCompute", True),
                         New KeyValuePair(Of String, Object)("ShowDescription", False),
                         New KeyValuePair(Of String, Object)("Total", 0))

            Me.isSimul = isSimul

            populateCriteres()
            populateSecteurs(isSimul)
            fillCriteresCoefs()
            computeTotal()
        End Sub

#Region "Property"
        Public Property CritereList As ObservableCollection(Of CritereView)
            Get
                Return getWatcher(Of ObservableCollection(Of CritereView))("CritereList")
            End Get
            Set(ByVal value As ObservableCollection(Of CritereView))
                setWatcher("CritereList", value)
                sortCritereList()
            End Set
        End Property

        Public Property SecteurList As List(Of SecteurView)
            Get
                Return getWatcher(Of List(Of SecteurView))("SecteurList")
            End Get
            Set(ByVal value As List(Of SecteurView))
                setWatcher("SecteurList", value)
            End Set
        End Property

        Public Property AutoCompute As Boolean
            Get
                Return getWatcher(Of Boolean)("AutoCompute")
            End Get
            Set(ByVal value As Boolean)
                setWatcher("AutoCompute", value)
            End Set
        End Property

        Public Property ShowDescription As Boolean
            Get
                Return getWatcher(Of Boolean)("ShowDescription")
            End Get
            Set(ByVal value As Boolean)
                setWatcher("ShowDescription", value)
            End Set
        End Property

#End Region ' !Property

#Region "MustOverride"
        Protected MustOverride Function getSQLCritere(isSimul As Boolean) As String

        ''' <summary>
        ''' Récupère l'ensemble des secteurs.
        ''' </summary>
        ''' <remarks></remarks>
        Protected MustOverride Sub populateSecteurs(isSimul As Boolean)

        ''' <summary>
        ''' Récupère les coefficients des différents secteurs sur un critère donnée.
        ''' </summary>
        Protected MustOverride Function getCoefsFromCritere(ByVal elt As CritereView, isSimul As Boolean) As ObservableCollection(Of CoefView)

        ''' <summary>
        ''' Sauvegarde un coefficient.
        ''' </summary>
        Public MustOverride Sub saveToBDD(ByVal coefView As CoefView, isSimul As Boolean)

        Protected MustOverride Function getTabName(isSimul As Boolean) As String

        ''' <summary>
        ''' Support de l export de l excel
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub ExportToExcel()

#End Region

#Region "Méthodes protected"
        ''' <summary>
        ''' Récupère l'ensemble des critères
        ''' </summary>
        Protected Sub populateCriteres()
            ' Remonte les NULL (1ere génération) en début de liste et tri les enfants avec la position.
            Dim criteres As New List(Of CritereView)

            Dim sql As String = getSQLCritere(isSimul)

            Dim list As List(Of Dictionary(Of String, Object)) = co.sqlToListDico(sql)
            Dim index As Integer = 0
            Dim found As Boolean = True

            ' Add root elements
            While index < list.Count
                If Not list(index).ContainsKey("id_parent") Then
                    ' root element
                    Dim root As New CritereView(Me,
                                                list(index)("id_critere"),
                                                Nothing,
                                                list(index)("nom"),
                                                list(index)("position"))

                    If list(index).ContainsKey("description") Then
                        root.Description = list(index)("description")
                    End If
                    If list(index).ContainsKey("CAP_min") Then
                        root.CAPMin = list(index)("CAP_min")
                    End If
                    If list(index).ContainsKey("CAP_max") Then
                        root.CAPMax = list(index)("CAP_max")
                    End If
                    If list(index).ContainsKey("format") Then
                        root.Format = DirectCast([Enum].ToObject(GetType(ColumnFormat), list(index)("format")), ColumnFormat)
                    End If
                    If list(index).ContainsKey("precision") Then
                        root.Precision = list(index)("precision")
                    End If
                    If list(index).ContainsKey("groupe") Then
                        root.Group = list(index)("groupe")
                    End If
                    If list(index).ContainsKey("inverse") Then
                        root.IsInverse = list(index)("inverse")
                    End If
                    root.update()

                    criteres.Add(root)
                    list.RemoveAt(index)
                Else
                    index = index + 1
                End If
            End While

            ' Add children when parent is found
            While list.Count > 0
                If index = list.Count Then
                    If Not found Then
                        ' Parcourt sans ajout
                        Exit While
                    End If
                    index = 0
                    found = False
                End If

                Dim row As Dictionary(Of String, Object) = list(index)
                Dim id_parent As Integer = row("id_parent")

                ' Find parent
                For Each par As CritereView In criteres
                    If par.Id = id_parent Then
                        Dim child As New CritereView(Me,
                                                     row("id_critere"),
                                                     par,
                                                     row("nom"),
                                                     row("position"))

                        If list(index).ContainsKey("description") Then
                            child.Description = list(index)("description")
                        End If
                        If list(index).ContainsKey("CAP_min") Then
                            child.CAPMin = list(index)("CAP_min")
                        End If
                        If list(index).ContainsKey("CAP_max") Then
                            child.CAPMax = list(index)("CAP_max")
                        End If
                        If list(index).ContainsKey("format") Then
                            child.Format = Convert.ToInt32(list(index)("format"))
                        End If
                        If list(index).ContainsKey("precision") Then
                            child.Precision = Convert.ToInt32(list(index)("precision"))
                        End If
                        If list(index).ContainsKey("groupe") Then
                            child.Group = Convert.ToInt32(list(index)("groupe"))
                        End If
                        If list(index).ContainsKey("inverse") Then
                            child.IsInverse = list(index)("inverse")
                        End If
                        child.update()

                        criteres.Add(child)
                        found = True
                        list.Remove(row)
                        Exit For
                    End If
                Next
            End While

            ' Transform tree to list.
            sortCritereList(criteres)
        End Sub

        Public Sub sortCritereList()
            sortCritereList(Me.CritereList.ToList)
        End Sub

        Public Sub sortCritereList(ByVal list As List(Of CritereView))
            CritereList.Clear()

            For Each root As CritereView In list.ToList.FindAll(AddressOf isRoot).OrderBy(Function(x) x.Position)
                For Each elt As CritereView In root.toList
                    CritereList.Add(elt)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Détermine si le critère n'a pas de parent.
        ''' </summary>
        Public Shared Function isRoot(ByVal critere As CritereView) As Boolean
            Return critere.Parent Is Nothing
        End Function

        ''' <summary>
        ''' Détermine si le critère n'a pas d'enfant.
        ''' </summary>
        Public Shared Function isLeaf(ByVal critere As CritereView) As Boolean
            Return critere.Parent IsNot Nothing AndAlso Not isRoot(critere.Parent)
        End Function

        ''' <summary>
        ''' Récupère les coefficients des critères
        ''' </summary>
        Protected Sub fillCriteresCoefs()
            For Each elt As CritereView In CritereList
                elt.Coefs = getCoefsFromCritere(elt, isSimul)
            Next
        End Sub

#End Region ' !Méthodes protected

#Region "Méthodes publiques"
        ''' <summary>
        ''' Calcul les coefficients en remontant sur les parents.
        ''' </summary>
        Sub updateCoefsBottomUp(ByVal coefView As CoefView)
            For Each elt As CritereView In CritereList
                If elt.Coefs.Contains(coefView) Then
                    If elt.Parent IsNot Nothing Then
                        Dim total As Decimal = 0
                        Dim pos As Integer = elt.Coefs.IndexOf(coefView)

                        For Each child As CritereView In elt.Parent.Children
                            total = total + child.Coefs(pos).Data
                        Next
                        elt.Parent.Coefs(pos).Data = total
                    Else
                        computeTotal()
                    End If

                    Return
                End If
            Next
        End Sub

        ''' <summary>
        ''' Calcul le score total de chaque secteur, la somme des coefficients des critères racines.
        ''' </summary>
        Protected Sub computeTotal()

            For i = 0 To SecteurList.Count - 1
                Dim total As Decimal = 0

                For Each root As CritereView In Me.CritereList.ToList.FindAll(AddressOf isRoot).OrderBy(Function(x) x.Position)
                    Dim coef = root.Coefs(i).Data

                    If coef IsNot Nothing Then
                        total = total + coef
                    End If
                Next
                SecteurList(i).Total = total
            Next
        End Sub

        ''' <summary>
        ''' Recalcul tous les coefficients.
        ''' </summary>
        Sub ComputeCoefs()
            Dim roots = Me.CritereList.ToList.FindAll(AddressOf isRoot).OrderBy(Function(x) x.Position)
            Dim oldAutoCompute = AutoCompute
            AutoCompute = False

            For i = 0 To SecteurList.Count - 1
                Dim total As Decimal = 0

                ' compute From root to children.
                For Each root As CritereView In roots
                    Dim coef

                    updateCoefsTopDown(root, i)
                    coef = root.Coefs(i).Data

                    If coef IsNot Nothing Then
                        total += coef
                    End If
                Next

                SecteurList(i).Total = total
            Next
            AutoCompute = oldAutoCompute
        End Sub

        ''' <summary>
        ''' Calcul les coefficients en descendant sur les enfants
        ''' </summary>
        Public Sub updateCoefsTopDown(ByVal critere As CritereView, ByVal pos As Integer)
            If critere.Children.Count > 0 Then
                Dim total As Decimal = 0

                For Each child As CritereView In critere.Children
                    updateCoefsTopDown(child, pos)
                    total = total + child.Coefs(pos).Data
                Next

                critere.Coefs(pos).Data = total
            End If
        End Sub

        ''' <summary>
        ''' Sauvegarde les différents critères et coefficients dans la base.
        ''' </summary>
        Public Sub saveToBDD()
            ' simple précaution
            sortCritereList()

            For Each crit As CritereView In Me.CritereList
                ' save critere.
                If crit.HasChanged Then
                    saveToBDD(crit)
                    crit.update()
                End If

                ' save coefs
                For Each coef As CoefView In crit.Coefs
                    If coef.DataHasChanged Then
                        saveToBDD(coef, isSimul)
                        coef.OldData = coef.Data
                    End If
                Next
            Next

            deleteCriteres()
        End Sub

        ''' <summary>
        ''' Sauvegarde un critère.
        ''' </summary>
        Sub saveToBDD(ByVal crit As CritereView)
            ' Try to find criteria's id.
            If crit.Id = 0 Then
                getIdFromBDD(crit)
            End If

            'ignore CAP for not leaf criteria.
            If Not crit.IsLeaf Then
                crit.CAPMin = Nothing
                crit.CAPMax = Nothing
            End If

            ' find parent.
            If crit.Parent IsNot Nothing AndAlso crit.Parent.Id = 0 Then
                saveToBDD(crit.Parent)
                crit.Parent.update()
            End If

            ' Save or update criteria.
            If crit.Id = 0 Then
                ' create criteria

                ' Set position after existing criterias in database.
                Dim pos_list = co.SelectDistinctWheres("ACT_COEF_CRITERE",
                                                       "position",
                                                       New List(Of String)({"id_parent", "is_sector"}),
                                                       New List(Of Object)({crit.Parent.Id, IIf(_is_sect, 1, 0)}))
                Dim pos = 0

                While pos < pos_list.Count AndAlso crit.Position > pos_list(pos)
                    pos += 1
                End While

                While pos < pos_list.Count AndAlso crit.Position = pos_list(pos)
                    crit.Position += 1
                    pos += 1
                End While

                co.Insert("ACT_COEF_CRITERE",
                          New List(Of String)({"id_parent",
                                               "nom",
                                               "position",
                                               "description",
                                               "CAP_min",
                                               "CAP_max",
                                               "format",
                                               "precision",
                                               "groupe",
                                               "inverse",
                                               "is_sector"}),
                          New List(Of Object)({crit.Parent.Id,
                                               crit.Name,
                                               crit.Position,
                                               crit.Description,
                                               crit.CAPMin,
                                               crit.CAPMax,
                                               crit.Format,
                                               crit.Precision,
                                               crit.Group,
                                               IIf(crit.IsInverse, 1, 0),
                                               IIf(_is_sect, 1, 0)}))

                getIdFromBDD(crit)
            Else
                ' update criteria
                co.Updates("ACT_COEF_CRITERE",
                       New List(Of String)({"id_parent",
                                            "nom",
                                            "position",
                                            "description",
                                            "CAP_min",
                                            "CAP_max",
                                            "format",
                                            "precision",
                                            "groupe",
                                            "inverse",
                                            "is_sector"}),
                       New List(Of Object)({IIf(crit.Parent Is Nothing, Nothing, crit.Parent.Id),
                                            crit.Name,
                                            crit.Position,
                                            crit.Description,
                                            crit.CAPMin,
                                            crit.CAPMax,
                                            crit.Format,
                                            crit.Precision,
                                            crit.Group,
                                            IIf(crit.IsInverse, 1, 0),
                                            IIf(_is_sect, 1, 0)}),
                       New List(Of String)({"id_critere"}),
                       New List(Of Object)({crit.Id}))
            End If
        End Sub

        Protected Sub getIdFromBDD(ByVal crit As CritereView)
            Dim list = co.SelectDistinctWheres("ACT_COEF_CRITERE",
                                                "id_critere",
                                                New List(Of String)({"nom", "is_sector"}),
                                                New List(Of Object)({crit.Name, IIf(_is_sect, 1, 0)}))
            If list.Count > 0 Then
                ' update criteria's id.
                crit.Id = list.First
            End If
        End Sub

        Protected Sub deleteCriteres()
            Dim tab As String = getTabName(Me.isSimul)

            ' Remove unused critere FROM tab name
            For Each id_crit As Integer In co.SelectDistinctSimple(tab,
                                                                   "id_critere")
                Dim found = False
                For Each crit As CritereView In CritereList
                    If crit.Id = id_crit Then
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then
                    co.DeleteWhere(tab,
                                   "id_critere",
                                   id_crit)
                End If
            Next

            ' Remove unused critere FROM ACT_COEF_CRITERE
            For Each id_crit As Integer In co.RequeteSqlToList("  SELECT id_critere " _
                                                               + " FROM ACT_COEF_CRITERE" _
                                                               + " WHERE id_critere NOT IN (SELECT id_critere" _
                                                               + "                          FROM ACT_COEF_INDICE" _
                                                               + "                          UNION" _
                                                               + "                          SELECT id_critere" _
                                                               + "                          FROM ACT_COEF_SECTEUR)")
                ' TODO: sort id_critereList root to leaf ????
                Dim found = False
                For Each crit As CritereView In CritereList
                    If crit.Id = id_crit Then
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then
                    co.DeleteWhere("ACT_COEF_CRITERE",
                                   "id_critere",
                                   id_crit)
                End If
            Next
        End Sub





#End Region ' !Méthodes publiques
    End Class
End Namespace