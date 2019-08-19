Imports System.Windows
Imports WindowsApplication1.Action.Coefficient

Namespace Action.Coefficient

    Public Class BaseActionCoefSecteur

        Private _coefViewModel As CoefViewModelSecteur

        Public Sub New()
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            _coefViewModel = New CoefViewModelSecteur

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            'TODO : get last date.
            Me.DataContext = _coefViewModel
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Public Sub New(isSimul As Boolean)

            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            _coefViewModel = New CoefViewModelSecteur(isSimul)

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            'TODO : get last date.
            Me.DataContext = _coefViewModel
            Windows.Forms.Cursor.Current = Cursors.Default

        End Sub

#Region "Méthodes privées"

        ''' <summary>
        ''' change le critere avec son adjacent.
        ''' </summary>
        Private Sub swapCritereLeaf(ByVal critere As CritereView, ByVal up As Boolean)
            Dim swap_crit = findNextCritere(critere, up)

            If Not critere.Equals(swap_crit) Then
                ' swap with previous child position
                CritereView.swap_crit(critere, swap_crit)
            Else
                ' Change parent
                Dim swap_parent As CritereView = findNextCritere(critere.Parent, up)

                If critere.Parent.Equals(swap_parent) Then
                    ' change root
                    Dim swap_root As CritereView = findNextCritere(swap_parent.Parent, up)

                    If Not swap_root.Equals(swap_parent.Parent) Then
                        ' find last parent in root.
                        If up Then
                            swap_parent = swap_root.Children.Last()
                        Else
                            swap_parent = swap_root.Children.First()
                        End If
                    End If
                End If

                If Not critere.Parent.Equals(swap_parent) Then
                    ' Change critere's parent
                    critere.Parent = swap_parent
                End If
            End If

            _coefViewModel.sortCritereList()
        End Sub

        ''' <summary>
        ''' change le critere parent avec son adjacent.
        ''' </summary>
        Private Sub swapCritereMiddle(ByVal critere As CritereView, ByVal up As Boolean)
            Dim swap_crit = findNextCritere(critere, up)

            If critere.Equals(swap_crit) Then
                ' change root
                Dim swap_root As CritereView = findNextCritere(swap_crit.Parent, up)

                If Not swap_root.Equals(swap_crit.Parent) Then
                    ' find last parent in root.
                    If Not swap_root.Children.Count = 0 Then
                        If up Then
                            swap_crit = swap_root.Children.Last()
                        Else
                            swap_crit = swap_root.Children.First()
                        End If
                    End If

                    ' Change critere's parent.
                    critere.Parent = swap_root
                End If
            End If

            If Not critere.Equals(swap_crit) Then
                ' swap with previous child position
                CritereView.swap_crit(critere, swap_crit)
            End If


            _coefViewModel.sortCritereList()
        End Sub

        ''' <summary>
        ''' Recherche le critere frère adjacent dans le sens de "up".
        ''' </summary>
        Private Function findNextCritere(ByVal critere As CritereView, ByVal up As Boolean) As CritereView
            Dim list As List(Of CritereView)

            If critere Is Nothing Then
                Return critere
            End If

            If critere.Parent Is Nothing Then
                ' Root critere
                list = _coefViewModel.CritereList.ToList.FindAll(AddressOf CoefViewModel.isRoot).OrderBy(Function(x) x.Position).ToList
            Else
                ' child critere
                list = critere.Parent.Children
            End If

            Dim swap_pos = findNextPos(critere.Position, list, up)

            If swap_pos >= 0 AndAlso swap_pos < list.Count Then
                Return list(swap_pos)
            Else
                Return critere
            End If
        End Function

        ''' <summary>
        ''' Recherche la position adjacente dans le sens up
        ''' </summary>
        Private Function findNextPos(ByVal pos As Integer, ByVal list As List(Of CritereView), ByVal up As Boolean) As Integer
            Dim swap_pos As Integer

            If up Then
                swap_pos = 0

                While swap_pos < list.Count _
                    AndAlso list(swap_pos).Position < pos

                    swap_pos += 1
                End While

                swap_pos -= 1
            Else
                swap_pos = list.Count - 1

                While swap_pos >= 0 _
                    AndAlso list(swap_pos).Position > pos
                    swap_pos -= 1
                End While

                swap_pos += 1
            End If

            Return swap_pos
        End Function

#End Region ' !Méthodes privées

#Region "Events"
        Private Sub BCompute_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCompute.Click
            _coefViewModel.ComputeCoefs()
        End Sub

        Private Sub BApply_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BApply.Click
            _coefViewModel.saveToBDD()
            Me.Close()
        End Sub

        Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCancel.Click
            Me.Close()
        End Sub

        ''' <summary>
        ''' Show Or hide description Column.
        ''' </summary>
        Private Sub BConfig_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            changeVisibility(ColumnDescription)
            ColumnPosition.Visibility = ColumnDescription.Visibility
            ColumnCAPMin.Visibility = ColumnDescription.Visibility
            ColumnCAPMax.Visibility = ColumnDescription.Visibility
            ColumnFormat.Visibility = ColumnDescription.Visibility
            ColumnGroup.Visibility = ColumnDescription.Visibility
            ColumnPrecision.Visibility = ColumnDescription.Visibility
            ColumnIsInverse.Visibility = ColumnDescription.Visibility
        End Sub

        ''' <summary>
        ''' Switch column visibility between visible and hidden.
        ''' </summary>
        Private Sub changeVisibility(ByRef column As System.Windows.Controls.DataGridColumn)
            If column.Visibility = System.Windows.Visibility.Visible Then
                column.Visibility = System.Windows.Visibility.Hidden
            Else
                column.Visibility = System.Windows.Visibility.Visible
            End If
        End Sub

        ''' <summary>
        ''' Ajout d'un nouveau secteur
        ''' </summary>
        Private Sub BAddSector_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAddSector.Click
            If CBSelectedSector.Text = "" Then
                MessageBox.Show("Veuillez sélectionner un secteur avant d'en ajouter.")
                Return
            End If

            Dim secteur As SecteurView = CBSelectedSector.SelectedValue

            ' Add Secteur
            _coefViewModel.SecteurList.Add(secteur)
            _coefViewModel.OtherSecteurs.Remove(secteur)

            ' Add coefs to secteur
            For Each crit As CritereView In _coefViewModel.CritereList
                crit.Coefs.Add(New CoefView(0, _coefViewModel, secteur, crit))
            Next

        End Sub

        ''' <summary>
        ''' Ajout d'un nouveau critère.
        ''' </summary>
        Private Sub BAddCriteria_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAddCriteria.Click
            If CBSelectedCritere.Text = "" Then
                MessageBox.Show("Veuillez sélectionner un critère avant d'en ajouter.")
                Return
            End If

            ' Update new criteria's parent.
            Dim critere As CritereView = CBSelectedCritere.SelectedValue
            Dim crit_parent As CritereView = DGCoef.SelectedValue

            If crit_parent Is Nothing Then
                crit_parent = _coefViewModel.CritereList.ToList.FindAll(AddressOf CoefViewModel.isRoot).OrderBy(Function(x) x.Position).First
            End If

            If CoefViewModel.isRoot(crit_parent) Then
                If crit_parent.Children.Count <> 0 Then
                    crit_parent = crit_parent.Children.First
                Else
                    crit_parent = _coefViewModel.CritereList.FirstOrDefault(Function(x) x.IsMiddle)

                    If crit_parent Is Nothing Then
                        MessageBox.Show("Veuillez ajouter une catégorie avant d'ajouter un critère.")
                        Return
                    End If
                End If
            ElseIf Not CoefViewModel.isRoot(crit_parent.Parent) Then
                ' Add critere behind selected leaf.
                critere.Position = crit_parent.Position + 1
                For Each crit As CritereView In crit_parent.Parent.Children
                    If crit.Position >= critere.Position Then
                        crit.Position += 1
                    End If
                Next

                crit_parent = crit_parent.Parent
            End If

            critere.Parent = crit_parent

            ' Add coefs to new criteria
            For i = 0 To _coefViewModel.SecteurList.Count - 1
                critere.Coefs.Add(New CoefView(0,
                                               _coefViewModel,
                                               _coefViewModel.SecteurList(i),
                                               critere))
            Next

            ' Update criteria lists
            Dim pos As Integer = _coefViewModel.CritereList.IndexOf(crit_parent)
            For Each crit As CritereView In crit_parent.Children.Where(Function(x) x.Position <= critere.Position)
                pos += 1
            Next

            _coefViewModel.CritereList.Insert(pos, critere)
            _coefViewModel.OtherCriteres.Remove(critere)
        End Sub

        ''' <summary>
        ''' Monte la catégorie ou le critère d'une position
        ''' </summary>
        ''' <remarks>Des changements de catégorie ou de root peuvent avoir lieu</remarks>
        Private Sub DG_BPosUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim selected As CritereView = DGCoef.SelectedItem

            If selected.IsLeaf Then
                swapCritereLeaf(selected, True)
            ElseIf selected.IsMiddle Then
                swapCritereMiddle(selected, True)
            End If
        End Sub

        ''' <summary>
        ''' Descend la catégorie ou le critère d'une position
        ''' </summary>
        ''' <remarks>Des changements de catégorie ou de root peuvent avoir lieu</remarks>
        Private Sub DG_BPosDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim selected As CritereView = DGCoef.SelectedItem

            If selected.IsLeaf Then
                swapCritereLeaf(selected, False)
            ElseIf selected.IsMiddle Then
                swapCritereMiddle(selected, False)
            End If

            _coefViewModel.CritereList = _coefViewModel.CritereList
        End Sub

        ''' <summary>
        ''' Ajout d'une nouvelle catégorie.
        ''' </summary>
        Private Sub AddCategory_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAddCategory.Click
            ' Update new criteria's parent.
            Dim critere As New CritereView(_coefViewModel, 0, Nothing, "nouvelle catégorie", 0)
            Dim crit_root As CritereView = DGCoef.SelectedValue

            If crit_root Is Nothing Then
                crit_root = _coefViewModel.CritereList.ToList.FindAll(AddressOf CoefViewModel.isRoot).OrderBy(Function(x) x.Position).First
            End If

            If CoefViewModel.isLeaf(crit_root) AndAlso crit_root.Parent IsNot Nothing Then
                crit_root = crit_root.Parent
            End If

            If crit_root.IsMiddle Then
                ' Add critere behind selected middle.
                critere.Position = crit_root.Position + 1
                For Each crit As CritereView In crit_root.Parent.Children.Where(Function(x) x.Position >= critere.Position)
                    crit.Position += 1
                Next

                crit_root = crit_root.Parent
            End If

            critere.Parent = crit_root

            ' Add coefs to new criteria
            For i = 0 To _coefViewModel.SecteurList.Count - 1
                critere.Coefs.Add(New CoefView(0,
                                               _coefViewModel,
                                               _coefViewModel.SecteurList(i),
                                               critere))
            Next

            ' Update criteria lists
            Dim pos As Integer = _coefViewModel.CritereList.IndexOf(crit_root)
            For Each crit As CritereView In crit_root.Children.Where(Function(x) x.Position <= critere.Position)
                pos += 1
                For Each child In crit.Children
                    pos += 1
                Next
            Next

            _coefViewModel.CritereList.Insert(pos, critere)
        End Sub

        ''' <summary>
        ''' Supprime une catégorie ou un critère.
        ''' </summary>
        Private Sub DeleteCriteria_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim critere As CritereView = DGCoef.SelectedValue

            ' Cannot remove root.
            If critere Is Nothing OrElse critere.IsRoot Then
                Return
            End If

            If critere.IsMiddle Then
                If critere.Children.Count > 0 Then
                    ' check children to be removed.
                    If MessageBox.Show("Voulez-vous supprimer les critères fils de " & critere.Name & " ?",
                                       "Confirmation de suppression",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question,
                                       MessageBoxResult.No) = MessageBoxResult.Yes Then
                        While critere.Children.Count > 0
                            _coefViewModel.RemoveCritere(critere.Children(0))
                        End While
                    Else
                        Return
                    End If
                End If

            End If

            _coefViewModel.RemoveCritere(critere)
        End Sub
#End Region

        ''' <summary>
        '''  extraction des donnees dans un nouveau fichier excel
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BExportExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
            _coefViewModel.ExportToExcel()
        End Sub
    End Class

End Namespace
