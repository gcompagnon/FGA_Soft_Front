Public Class SuiviTitreManuel
    Dim Connection As New Connection()

    Private Sub SuiviTitreManuel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.setDatasource()
    End Sub

    Public Sub SetDatasource()
        'Dim Connection As New Connection()
        Dim Marequete As String

        'Connection a la BDD
        Connection.ToConnectBase()

        Marequete = "select distinct s.isin_titre,f.libelle_titre,f.dateinventaire,f.[Type_Produit],f.[Secteur],f.[Sous_Secteur] "
        Marequete = Marequete & " from [STRAT_TITRE_GRILLE] s inner join ptf_fga f on f.isin_titre = s.isin_titre inner join (SELECT  [isin_titre],Max(Dateinventaire) as Madate FROM [PTF_FGA] Group by isin_titre) rqt on rqt.[isin_titre] = s.isin_titre where s.source = 'Manuel' and rqt.Madate = f.dateinventaire "
        Marequete = Marequete & " union "
        Marequete = Marequete & " select distinct s.isin_titre, p.libelle_titre, p.date,p.type_produit,p.secteur,p.sous_secteur "
        Marequete = Marequete & " from [STRAT_TITRE_GRILLE] s inner join ptf_proxy p on p.code_titre = s.isin_titre inner join (SELECT  [code_titre],Max(Date) as Madate FROM [PTF_proxy] Group by code_titre) rqt on rqt.code_titre = p.code_titre and rqt.Madate = p.date where s.source = 'Manuel' and s.isin_titre not in (select distinct isin_titre from ptf_fga) "

        'Marequete = "Select [ISIN_titre],[Id_Grille],[Date],[Poids]*100 as Poids,[Source] from strat_titre_Grille where source = 'Manuel' order by isin_titre"
        DG_TitreManuel.DataSource = Connection.LoadDataGridByString(Marequete)
        'DG_TitreManuel.Columns("Poids").HeaderText = "Poids (en %)"
        DG_TitreManuel.Refresh()

    End Sub

    Private Sub DG_TitreManuel_CellMouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DG_TitreManuel.CellMouseDoubleClick
        Dim maj As New MajTitreManuel
        maj.Show()
    End Sub

    Private Sub Btn_Supprimer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Supprimer.Click
        Dim marequete As String
        Dim isin_titre As String

        Try
            isin_titre = DG_TitreManuel.CurrentRow.Cells(0).Value
            Dim ConfirmationAnnulation = MessageBox.Show("Voulez vous vraiment supprimer ce titre ?", "Avertissement", MessageBoxButtons.YesNo)
            If ConfirmationAnnulation = Windows.Forms.DialogResult.Yes Then
                marequete = "Delete from Strat_titre_grille where isin_titre = '" & isin_titre & "'"
                Connection.RequeteSql(marequete)
                Me.SetDatasource()
            End If
        Catch ex As Exception
            MessageBox.Show("Pas de titre de sélectionné", "Erreur", MessageBoxButtons.OK)
        End Try


    End Sub
End Class