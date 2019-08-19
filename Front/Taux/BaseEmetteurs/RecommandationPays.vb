Namespace Taux.BaseEmetteurs

    Public Class RecommandationPays

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Label1.Font = New Font(Label1.Font, FontStyle.Bold)

            'Tentative de connnection
            co.ToConnectBase()

            'Remplisssage de la 1er datagrid a chaque load
            Dim paramName As List(Of String) = New List(Of String)()
            Dim paramDonnee As List(Of Object) = New List(Of Object)()
            DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetPaysRecommandationGrid", paramName, paramDonnee))

            CbRecommandation.DataSource = co.SelectSimple("TX_RECOMMANDATION", "libelle")
            CbRecommandation.SelectedIndex = -1

        End Sub

        ''' <summary>
        ''' Clique sur CbPays
        ''' </summary>
        Private Sub CbPays_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPays.Click
            CbPays.Items.Clear()
            Dim i As Integer = 0
            For i = 0 To co.SelectDistinctSimple("PAYS", "libelle").Count - 1 Step 1
                CbPays.Items.Add(co.SelectDistinctSimple("PAYS", "libelle")(i))
            Next
        End Sub

        ''' <summary>
        ''' Changement de valeur sur CbPays
        ''' </summary>
        Private Sub CbPays_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPays.SelectedIndexChanged
            CbRecommandation.SelectedIndex = -1
            TCommentaire.Clear()

            'Chargement de l'historique d'un pays
            DataGridHistorique.ClearSelection()
            Dim colName As List(Of String) = New List(Of String)()
            colName.Add("@libelle")
            Dim donnee As List(Of Object) = New List(Of Object)()
            donnee.Add(CbPays.Text)
            DataGridHistorique.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetPaysRecommandationHistoGrid", colName, donnee))

            'Peupler les composant CbRecommandation et TCommentaire
            TLogin.Text = Utilisateur.login
            If (co.SelectDistinctWhere("TX_RECOMMANDATION_PAYS", "id_pays", "id_pays", co.SelectDistinctWhere("PAYS", "id", "libelle", CbPays.Text).First).Count > 0) Then
                CbRecommandation.Text = co.ProcedureStockéeList("GetLastPaysRecommandation", colName, donnee)(2)
                TCommentaire.SelectedText = co.ProcedureStockéeList("GetLastPaysRecommandation", colName, donnee)(4)
                Select Case co.ProcedureStockéeList("GetLastPaysRecommandation", colName, donnee)(5)
                    Case "Négatif"
                        RbNégatif.Checked = True
                    Case Else
                        RbNeutre.Checked = True
                End Select
            Else
                CbRecommandation.Text = ""
            End If
        End Sub

        ''' <summary>
        ''' Bouton modification recommandation pays
        ''' </summary>
        Private Sub BChanger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRecommandation.Click
            If (String.IsNullOrEmpty(CbPays.Text) = False And String.IsNullOrEmpty(TLogin.Text) = False And String.IsNullOrEmpty(CbRecommandation.Text) = False) Then

                Dim colName As List(Of String) = New List(Of String)()
                colName.Add("id_pays")
                colName.Add("date")
                colName.Add("id_recommandation")
                colName.Add("id_utilisateur")
                colName.Add("commentaire")
                colName.Add("id_recommandation_isr")
                Dim donnee As List(Of Object) = New List(Of Object)()
                donnee.Add(co.SelectDistinctWhere("PAYS", "id", "libelle", CbPays.Text).First)
                donnee.Add(DateTime.Now.ToString)
                donnee.Add(co.SelectDistinctWhere("TX_RECOMMANDATION", "id", "libelle", CbRecommandation.Text).First)
                donnee.Add(TLogin.Text)
                donnee.Add(TCommentaire.Text)
                If RbNégatif.Checked Then
                    donnee.Add("Neg")
                Else
                    donnee.Add("Ne")
                End If
                'Ajout dans la BBD de la nouvelle recommandation
                co.Insert("TX_RECOMMANDATION_PAYS", colName, donnee)

                'Mettre a jour les 2 data grid
                DataGridPays.ClearSelection()
                Dim paramName As List(Of String) = New List(Of String)()
                Dim paramDonnee As List(Of Object) = New List(Of Object)()
                DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetPaysRecommandationGrid", paramName, paramDonnee))
                DataGridHistorique.ClearSelection()
                paramName.Add("@libelle")
                paramDonnee.Add(CbPays.Text)
                DataGridHistorique.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetPaysRecommandationHistoGrid", paramName, paramDonnee))

                log.Log(ELog.Information, "BChanger_Click", "Modication du rating du pays " & CbPays.Text)

            Else
                MessageBox.Show("Il manque des informations pour changer la recommandation du pays " & CbPays.Text, "Echec du changement de recommandation", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        ''' <summary>
        ''' Click sur TCommentaire
        ''' </summary>
        Private Sub TCommentaire_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TCommentaire.MouseClick
            TCommentaire.Clear()
        End Sub

        ''' <summary>
        ''' Rezise l'ihm automatiquement
        ''' </summary>
        Private Sub RecommandationPays_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
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

        ''' <summary>
        ''' Bouton créer un nouveau pays
        ''' </summary>
        Private Sub BCréer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If String.IsNullOrEmpty(CbPays.Text) = False Then
                If (co.SelectDistinctWhere("PAYS", "libelle", "libelle", CbPays.Text).Count = 0 And co.SelectDistinctWhere("ZONE_GEOGRAPHIQUE", "pays", "pays", CbPays.Text).Count = 0) Then
                    Dim colNames As List(Of String) = New List(Of String)()
                    Dim donnee As List(Of Object) = New List(Of Object)()
                    'Ajout dans la table ZONE_GEOGRAPHIQUE
                    colNames.Add("pays")
                    colNames.Add("zone")
                    donnee.Add(CbPays.Text)
                    donnee.Add(Nothing)
                    co.Insert("ZONE_GEOGRAPHIQUE", colNames, donnee)
                    colNames.Clear()
                    donnee.Clear()
                    'Ajout dans la table PAYS
                    colNames.Add("id")
                    colNames.Add("libelle")
                    colNames.Add("utilisateur")
                    donnee.Add(CbPays.Text)
                    donnee.Add(CbPays.Text)
                    donnee.Add(Utilisateur.login)
                    co.Insert("PAYS", colNames, donnee)
                    log.Log(ELog.Information, "BChanger_Click", "Création d'un nouveau pays : " & CbPays.Text)
                    MessageBox.Show("Ajout du pays " & CbPays.Text & " dans la table PAYS", "Succès de l'ajout d'un nouveau pays", MessageBoxButtons.OK, MessageBoxIcon.None)
                Else
                    MessageBox.Show("Le pays " & CbPays.Text & " existe déjà dans la table PAYS", "Echec de l'ajout d'un nouveau pays", MessageBoxButtons.OK, MessageBoxIcon.None)
                End If
            Else
                MessageBox.Show("La case pays est vide !", "Echec de l'ajout d'un nouveau pays", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        ''' <summary>
        ''' Recharger BaseEmetteurs
        ''' </summary>
        Private Sub RecommandationPays_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Dim be As New BaseEmetteurs
            be.Show()
        End Sub


    End Class
End Namespace


