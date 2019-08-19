Public Class MajTitreManuel
    Dim Connection As New Connection()
    Dim DGVC_poids As New DataGridViewColumn()
    Dim Cell_poids As New DataGridViewTextBoxCell()

    Private Sub MajTitreManuel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim stm As New SuiviTitreManuel
        Lbl_CodeTitre.Text = stm.DG_TitreManuel.CurrentRow.Cells(0).Value
        Connection.ToConnectBase()

        DG_MajTitreManuel.DataSource = Connection.LoadDataGridByString("Select id,type,decomposition from Strat_Grille")
        DG_MajTitreManuel.EnableHeadersVisualStyles = True

        'Propriété de la colonne insérée
        DGVC_poids.Name = "Poids"
        DGVC_poids.HeaderText = "Poids (en %)"
        DGVC_poids.CellTemplate = Cell_poids

        DG_MajTitreManuel.Columns.Add(DGVC_poids)
        'On met toutes les colonnes non modifiables excepté celle du poids on on peut mettre le poids
        DG_MajTitreManuel.Columns("Id").ReadOnly = True
        DG_MajTitreManuel.Columns("Type").ReadOnly = True
        DG_MajTitreManuel.Columns("Decomposition").ReadOnly = True
        DG_MajTitreManuel.Columns("Poids").ReadOnly = False

        initialisationPoids(Lbl_CodeTitre.Text)
    End Sub

    Private Sub initialisationPoids(ByVal code_titre)
        Dim Marequete As String
        Dim result As List(Of Object)
        Dim nbtitre As Integer
        Dim find As Boolean
        Dim i, j As Integer

        Marequete = "select * from strat_titre_grille where date in "
        Marequete = Marequete & "(SELECT max([Date])"
        Marequete = Marequete & " FROM [STRAT_TITRE_GRILLE]"
        Marequete = Marequete & " where isin_titre = '" & code_titre & "'"
        Marequete = Marequete & " group by [ISIN_titre])"
        Marequete = Marequete & " and isin_titre = '" & code_titre & "'"

        Try
            result = Connection.RequeteSqlToList(Marequete)
            nbtitre = result.Count / 5 '5 car on a 5 champs : isin_titre,date, grille,poids,source
            For i = 0 To DG_MajTitreManuel.Rows.Count - 1
                find = False
                For j = 0 To result.Count - 1 Step 5
                    If DG_MajTitreManuel.Rows(i).Cells.Item("Id").Value = result(j + 1).ToString Then
                        DG_MajTitreManuel.Rows(i).Cells.Item("poids").Value = CDbl(result(j + 3).ToString) * 100
                        find = True
                    End If
                Next
                If (Not find) Then DG_MajTitreManuel.Rows(i).Cells.Item("Poids").Value = 0
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Btn_Valider_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Valider.Click
        Dim i As Integer
        Dim Total As Double
        Dim time As DateTime
        Try
            Total = 0
            For i = 0 To DG_MajTitreManuel.Rows.Count - 1
                Total = Total + DG_MajTitreManuel.Rows(i).Cells.Item("Poids").Value
            Next
            If (Total <> 100) Then
                MessageBox.Show("Le total n'est pas égal à 100%", "Attention", MessageBoxButtons.OK)
            Else
                Dim suivi As New SuiviTitreManuel
                For i = 0 To DG_MajTitreManuel.Rows.Count - 1
                    If (DG_MajTitreManuel.Rows(i).Cells.Item("Poids").Value > 0) Then
                        time = DateTime.Now
                        CreateRqtTitreGrille(Lbl_CodeTitre.Text, DG_MajTitreManuel.Rows(i).Cells.Item("Id").Value, time, DG_MajTitreManuel.Rows(i).Cells.Item("Poids").Value / 100, "Manuel")
                        suivi.DG_TitreManuel.Update()
                    End If
                Next
                suivi.SetDatasource()
                Me.Dispose()
            End If
            ' rafraichir la datid: DG_Titre.Datasource
            'Me.BeginInvoke(New AccueilTitreGrille.SetDataSourceAsync(AddressOf AccueilTitreGrille.SetDatasource))
        Catch ex As Exception
            MessageBox.Show("Impossible de créer la requete")
        End Try
    End Sub

    Function CreateRqtTitreGrille(ByVal Isin_Titre As String, ByVal IdGrille As String, ByVal DateTitre As String, ByVal Poids As Double, ByVal Source As String) As Boolean
        Dim Marequete As String
        Marequete = "Insert into Strat_titre_Grille values ('" & Isin_Titre & "','" & IdGrille & "',Cast('" & DateTitre & "' as datetime)," & Poids & ",'" & Source & "')"
        Try
            Connection.RequeteSql(Marequete)
        Catch ex As Exception
            MessageBox.Show("Enregistrement impossible", "Attention", MessageBoxButtons.OK)
        End Try

    End Function

    Private Sub Btn_Annuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Annuler.Click
        Me.Dispose()
    End Sub

    Private Sub MajTitreManuel_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Me.Dispose()
    End Sub


    Private Sub DG_MajTitreManuel_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DG_MajTitreManuel.CellEndEdit
        If Not (IsNumeric(DG_MajTitreManuel.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)) Then
            MessageBox.Show("Valeur entrée non valide")
            DG_MajTitreManuel.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = 0
        End If
    End Sub

End Class