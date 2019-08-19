Imports WindowsApplication1.Referentiel
Imports WindowsApplication1

Namespace Taux.BaseEmetteurs
    Public Class RecommandationSecteur

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub RecommendationSousSecteur_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Tentative de connnection
            co.ToConnectBase()

            'Remplisssage de la 1er datagrid a chaque load
            Dim paramName As List(Of String) = New List(Of String)()
            Dim paramDonnee As List(Of Object) = New List(Of Object)()
            DataGridSousSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetSousSecteurRecommandationGrid", paramName, paramDonnee))


            CbRecommandation.DataSource = co.SelectSimple("TX_RECOMMANDATION", "libelle")
            CbRecommandation.SelectedIndex = -1

        End Sub

        ''' <summary>
        ''' Click CbSousSecteur
        ''' </summary>
        Private Sub CbSousSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSousSecteur.Click
            'CbSousSecteur.Items.Clear()
            'On remplie les sous secteurs en fonction de la date saisie par l'utilisateur
            Dim i As Integer = 0
            If (String.IsNullOrEmpty(CbSecteur.Text) = False And co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).Count > 0) Then
                Dim id_secteur As Object = co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First()

                Dim name As New List(Of String)(New String() {"id_secteur", "id"})
                Dim donnee As New List(Of Object)(New Object() {id_secteur, " LIKE '%O %'"})
                CbSousSecteur.DataSource = co.SelectDistinctWheres("SOUS_SECTEUR", "libelle", name, donnee)

                'For i = 0 To co.SelectDistinctWhere("SOUS_SECTEUR", "libelle", "id_secteur", id_secteur).Count - 1 Step 1
                'CbSousSecteur.Items.Add(co.SelectDistinctWhere("SOUS_SECTEUR", "libelle", "id_secteur", id_secteur)(i))
                'Next
            Else
                'For i = 0 To co.SelectSimple("SOUS_SECTEUR", "libelle").Count - 1 Step 1
                'CbSousSecteur.Items.Add(co.SelectSimple("SOUS_SECTEUR", "libelle")(i))
                'Next
                Dim name As New List(Of String)(New String() {"id"})
                Dim donnee As New List(Of Object)(New Object() {" LIKE '%O %'"})
                CbSousSecteur.DataSource = co.SelectDistinctWheres("SOUS_SECTEUR", "libelle", name, donnee)
            End If
        End Sub

        ''' <summary>
        ''' Click CbSecteur
        ''' </summary>
        Private Sub CbSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSecteur.Click
            'CbSecteur.Items.Clear()
            'CbSousSecteur.Items.Clear()
            'CbSousSecteur.Text = ""
            CbSousSecteur.SelectedIndex = -1
            RbNeutre.Checked = True

            'On remplie les combobox en fonction de la date saisie par l'utilisateur
            Dim name As New List(Of String)(New String() {"id"})
            Dim donnee As New List(Of Object)(New Object() {" LIKE '%O %'"})
            CbSecteur.DataSource = co.SelectDistinctWheres("SECTEUR", "libelle", name, donnee)
            'Dim i As Integer = 0
            'For i = 0 To co.SelectDistinctSimple("SECTEUR", "libelle").Count - 1 Step 1
            ' CbSecteur.Items.Add(co.SelectDistinctSimple("SECTEUR", "libelle")(i))
            'Next
        End Sub

        ''' <summary>
        ''' Click BCréerSousSecteur
        ''' </summary>
        Private Sub BCréerSousSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If String.IsNullOrEmpty(CbSecteur.Text) = False Then
                'Juste création d'un nouveau secteur
                If String.IsNullOrEmpty(CbSecteur.Text) = False And String.IsNullOrEmpty(CbSousSecteur.Text) = True Then
                    If (co.SelectDistinctWhere("SECTEUR", "libelle", "libelle", CbSecteur.Text).Count = 0) Then
                        Dim colNames As List(Of String) = New List(Of String)()
                        Dim donnee As List(Of Object) = New List(Of Object)()

                        'Ajout dans la table SECTEUR
                        colNames.Add("id")
                        colNames.Add("libelle")
                        colNames.Add("utilisateur")
                        donnee.Add(CbSecteur.Text)
                        donnee.Add(CbSecteur.Text)
                        donnee.Add(Utilisateur.login)
                        co.Insert("SECTEUR", colNames, donnee)

                        log.Log(ELog.Information, "BCréerSecteur_Click", "Création d'un secteur pays : " & CbSecteur.Text)
                        MessageBox.Show("Ajout du secteur " & CbSecteur.Text & " dans la table SECTEUR", "Succès de l'ajout d'un nouveau secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
                    Else
                        MessageBox.Show("Le secteur " & CbSecteur.Text & " existe déjà dans la table SECTEUR", "Echec de l'ajout d'un nouveau secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
                    End If

                Else
                    If (co.SelectDistinctWhere("SOUS_SECTEUR", "libelle", "libelle", CbSousSecteur.Text).Count = 0) Then
                        Dim colNames As List(Of String) = New List(Of String)()
                        Dim donnee As List(Of Object) = New List(Of Object)()

                        'Check si le SECTEUR existe dans la base de donnée
                        If (co.SelectDistinctWhere("SECTEUR", "libelle", "libelle", CbSecteur.Text).Count = 0) Then
                            'Ajout dans la table SECTEUR
                            colNames.Add("id")
                            colNames.Add("libelle")
                            colNames.Add("utilisateur")
                            donnee.Add(CbSecteur.Text)
                            donnee.Add(CbSecteur.Text)
                            donnee.Add(Utilisateur.login)
                            co.Insert("SECTEUR", colNames, donnee)
                            colNames.Clear()
                            donnee.Clear()

                            log.Log(ELog.Information, "BCréerSousSecteur_Click", "Création d'un nouveau secteur : " & CbSecteur.Text)
                            MessageBox.Show("Ajout du secteur " & CbSecteur.Text & " dans la table SECTEUR", "Succès de l'ajout d'un nouveau secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
                        End If

                        'Ajout dans la table SOUS_SECTEUR
                        colNames.Add("id")
                        colNames.Add("libelle")
                        colNames.Add("id_secteur")
                        colNames.Add("utilisateur")
                        donnee.Add(CbSousSecteur.Text)
                        donnee.Add(CbSousSecteur.Text)
                        donnee.Add(co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First())
                        donnee.Add(Utilisateur.login)
                        co.Insert("SOUS_SECTEUR", colNames, donnee)

                        log.Log(ELog.Information, "BCréerSousSecteur_Click", "Création d'un nouveau sous secteur : " & CbSousSecteur.Text)
                        MessageBox.Show("Ajout du sous secteur " & CbSousSecteur.Text & " dans la table SOUS_SECTEUR", "Succès de l'ajout d'un nouveau sous secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
                    Else
                        MessageBox.Show("Le sous secteurs " & CbSousSecteur.Text & " existe déjà dans la table SOUS_SECTEUR", "Echec de l'ajout d'un nouveau sous secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
                    End If
                End If
            Else
                MessageBox.Show("La case Secteur et Sous Secteur sont vides !", "Echec de l'ajout d'un nouveau sous secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        ''' <summary>
        ''' Click BRecommandation
        ''' </summary>
        Private Sub BRecommandation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRecommandation.Click
            If String.IsNullOrEmpty(CbSousSecteur.Text) = False And String.IsNullOrEmpty(TLogin.Text) = False And String.IsNullOrEmpty(CbRecommandation.Text) = False Then
                'Ajout de la nouvelle recommandation
                Dim colName As List(Of String) = New List(Of String)()
                colName.Add("id_sous_secteur")
                colName.Add("date")
                colName.Add("id_recommandation")
                colName.Add("id_utilisateur")
                colName.Add("commentaire")
                colName.Add("id_recommandation_isr")
                Dim donnee As List(Of Object) = New List(Of Object)()
                donnee.Add(co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).First)
                donnee.Add(DateTime.Now.ToString)
                donnee.Add(co.SelectDistinctWhere("TX_RECOMMANDATION", "id", "libelle", CbRecommandation.Text).First)
                donnee.Add(TLogin.Text)
                donnee.Add(TCommentaire.Text)
                If RbNégatif.Checked Then
                    donnee.Add("Neg")
                Else
                    donnee.Add("Ne")
                End If
                co.Insert("TX_RECOMMANDATION_SOUS_SECTEUR", colName, donnee)

                'Refresh affichage des datagrids
                DataGridSousSecteur.ClearSelection()
                DataGridHistorique.ClearSelection()
                Dim colNames As List(Of String) = New List(Of String)()
                Dim donnees As List(Of Object) = New List(Of Object)()
                DataGridSousSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetSousSecteurRecommandationGrid", colNames, donnees))
                colNames.Add("@libelle")
                donnees.Add(CbSousSecteur.Text)
                DataGridHistorique.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetSousSecteurRecommandationHistoGrid", colNames, donnees))

                log.Log(ELog.Information, "BRecommandation_Click", "Modication du rating du sous secteur " & CbSousSecteur.Text)
            Else
                MessageBox.Show("Il manque des informations pour valider la recommandation du sous secteur !", "Echec de l'ajout d'un nouveau sous secteur", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        ''' <summary>
        ''' CbSousSecteur change
        ''' </summary>
        Private Sub CbSousSecteur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSousSecteur.SelectedIndexChanged
            'CbRecommandation.Items.Clear()
            CbRecommandation.SelectedIndex = -1
            TCommentaire.Clear()

            'Chargement de l'historique d'un sous secteur
            DataGridHistorique.ClearSelection()
            Dim colName As List(Of String) = New List(Of String)()
            colName.Add("@libelle")
            Dim donnee As List(Of Object) = New List(Of Object)()
            donnee.Add(CbSousSecteur.Text)
            DataGridHistorique.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetSousSecteurRecommandationHistoGrid", colName, donnee))

            'On peuple les autres composant graphique
            TLogin.Text = Utilisateur.login
            If CbSousSecteur.Text <> "" Then
                Dim id_sous_secteur As Object = co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).First
                If (co.SelectDistinctWhere("TX_RECOMMANDATION_SOUS_SECTEUR", "id_sous_secteur", "id_sous_secteur", id_sous_secteur).Count > 0) Then
                    CbRecommandation.SelectedItem = co.ProcedureStockéeList("GetLastSousSecteurRecommandation", colName, donnee)(3)
                    TCommentaire.SelectedText = co.ProcedureStockéeList("GetLastSousSecteurRecommandation", colName, donnee)(5)
                    Select Case co.ProcedureStockéeList("GetLastSousSecteurRecommandation", colName, donnee)(6)
                        Case "Négatif"
                            RbNégatif.Checked = True
                        Case Else
                            RbNeutre.Checked = True
                    End Select
                Else
                    CbRecommandation.Text = ""
                End If
            End If
        End Sub

        ''' <summary>
        ''' Refresh l'écran d'accueil des signatures
        ''' </summary>
        Private Sub RecommandationSecteur_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Dim be As New BaseEmetteurs
            be.Show()
        End Sub

        Private Sub RecommandationSecteur_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Dim Feuille As Form
            Feuille = Me
            Static Longueur As Long
            Static Hauteur As Long
            Dim PropLongueur As Single
            Dim PropHauteur As Single
            If ((Longueur > 0) And (Hauteur > 0)) Then
                PropLongueur = Feuille.Width / Longueur
                PropHauteur = Feuille.Height / Hauteur
                Dim Ctrl As Control
                On Error Resume Next
                For Each Ctrl In Feuille.Controls
                    Ctrl.Left = CInt(Ctrl.Left * PropLongueur)
                    Ctrl.Top = CInt(Ctrl.Top * PropHauteur)
                    Ctrl.Width = CInt(Ctrl.Width * PropLongueur)
                    Ctrl.Height = CInt(Ctrl.Height * PropHauteur)
                Next
                On Error GoTo 0
            End If
            Longueur = Feuille.Width
            Hauteur = Feuille.Height
        End Sub
    End Class
End Namespace
