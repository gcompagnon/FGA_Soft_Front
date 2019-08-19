
Public Class AffecterTitreGrille
    Dim DGVC_poids As New DataGridViewColumn()
    Dim Cell_poids As New DataGridViewTextBoxCell()
    Dim Connection As New Connection()

    Private Sub AffecterTitreGrille_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim codeTitre As String

        'Dim DGVC_poids As New DataGridViewColumn()
        'Dim Cell_poids As New DataGridViewTextBoxCell()
        'Connection a la BDD
        Connection.ToConnectBase()

        DG_Grille.DataSource = Connection.LoadDataGridByString("Select id,type,decomposition from Strat_Grille")
        DG_Grille.EnableHeadersVisualStyles = True

        'Propriété de la colonne insérée
        DGVC_poids.Name = "Poids"
        DGVC_poids.HeaderText = "Poids (en %)"
        DGVC_poids.CellTemplate = Cell_poids

        DG_Grille.Columns.Add(DGVC_poids)
        'On met toutes les colonnes non modifiables excepté celle du poids on on peut mettre le poids
        DG_Grille.Columns("Id").ReadOnly = True
        DG_Grille.Columns("Type").ReadOnly = True
        DG_Grille.Columns("Decomposition").ReadOnly = True
        DG_Grille.Columns("Poids").ReadOnly = False

        'Initialisation des colonnes à 0 
        initialisationPoids()

        'DGVC_poids.HeaderText = "Poids"
        'DGVC_poids.DataGridView.Columns.Add("Poids", "toto")
        'DGVC_poids.DataGridView.Columns(0).Name = "Poids"
        'DGVC_poids.DataGridView.Rows.Add(10)
        'DGVC_poids.DataGridView.Rows(0).SetValues("test")

        'DG_Grille.Columns.Add(DGVC_poids)
        Dim ac As New AccueilTitreGrille
        codeTitre = ac.DG_Titre.CurrentRow.Cells(0).Value
        Lbl_CodeTitre.Text = codeTitre

    End Sub

    Private Sub AffecterTitreGrille_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'En quittant, on ecrase toutes les ressources du formulaire
        Me.Dispose()
    End Sub

    Private Sub initialisationPoids()
        For i = 0 To DG_Grille.Rows.Count - 1
            DG_Grille.Rows(i).Cells.Item("Poids").Value = 0
        Next
    End Sub

    Private Sub DG_Grille_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DG_Grille.CellEndEdit

        If Not (IsNumeric(DG_Grille.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)) Then
            MessageBox.Show("Valeur entrée non valide")
            DG_Grille.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = 0
        End If


    End Sub

    Private Sub Btn_Annuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Annuler.Click
        Me.Dispose()
    End Sub

    Private Sub Btn_Valider_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Valider.Click
        Dim i As Integer
        Dim Total As Double
        Dim time As DateTime
        Try
            Total = 0
            For i = 0 To DG_Grille.Rows.Count - 1
                Total = Total + DG_Grille.Rows(i).Cells.Item("Poids").Value
            Next
            If (Total <> 100) Then
                MessageBox.Show("Le total n'est pas égal à 100%", "Attention", MessageBoxButtons.OK)
            Else
                Dim atg As New AccueilTitreGrille
                For i = 0 To DG_Grille.Rows.Count - 1
                    If (DG_Grille.Rows(i).Cells.Item("Poids").Value > 0) Then
                        time = DateTime.Now
                        CreateRqtTitreGrille(Lbl_CodeTitre.Text, DG_Grille.Rows(i).Cells.Item("Id").Value, time, DG_Grille.Rows(i).Cells.Item("Poids").Value / 100, "Manuel")
                        atg.DG_Titre.Update()
                    End If
                Next
                atg.SetDatasource()
                Me.Dispose()
            End If
            ' rafraichir la datid: DG_Titre.Datasource
            'Me.BeginInvoke(New AccueilTitreGrille.SetDataSourceAsync(AddressOf AccueilTitreGrille.SetDatasource))
        Catch ex As Exception
            MessageBox.Show("Impossible de créer la requete")
        End Try

    End Sub


    Public Function CreateRqtTitreGrille(ByVal Isin_Titre As String, ByVal IdGrille As String, ByVal DateTitre As String, ByVal Poids As Double, ByVal Source As String) As Boolean
        Dim Marequete As String
        Marequete = "Insert into Strat_titre_Grille values ('" & Isin_Titre & "','" & IdGrille & "',Cast('" & DateTitre & "' as datetime)," & Poids & ",'" & Source & "')"
        Try
            Connection.RequeteSql(Marequete)
        Catch ex As Exception
            MessageBox.Show("Enregistrement impossible", "Attention", MessageBoxButtons.OK)
        End Try

    End Function

End Class
