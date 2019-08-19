Imports WindowsApplication1.Taux.BaseEmetteurs


Public Class ModelPortfolio

    Dim co As Connection = New Connection()
    Dim fichier As Fichier = New Fichier()
    Dim excel As Excel = New Excel()
    Dim da As DGrid = New DGrid()



    Dim secteur_fga As String

    'flag pour empecher de faire d'autre event lors d'un traitement sur la datagrid
    Private Shared DsecteursNoEvent As Boolean = False
    Private Shared DHistoriqueNoEvent As Boolean = False



    Private Sub ModelPortfolio_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Connection a la base
        co.ToConnectBase()


        'Base Sectorielle
        TMaxDate.Text = co.SelectDistinctSimple("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "DESC").FirstOrDefault
        CbIndiceSecteur.DataSource = New List(Of String)(New String() {"Stoxx 600"})
        TCommentaireSecteur.ShowToolBar = True
        TCommentaireSecteur.AutoSize = True

    End Sub



#Region "Sectorielle"

    Private Sub BRevue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRevue.Click
        Me.Close()
        Dim bs As New Revue
        bs.Show()
        bs.maj_secteur(secteur_fga)
    End Sub



    'Private Sub DNews_RowsAdded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles DNews.RowsAdded
    '   If DSecteurs.RowCount <> 0 Then
    'Dim secteur As String = DSecteurs.CurrentRow.Cells.Item(1).Value
    'DNews.CurrentRow.Cells.Item(1).Value = DateTime.Now
    '   End If
    'End Sub

    Private Sub BExcelSectorielle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelSectorielle.Click
        da.DataGridToNewExcel(DSecteursIcb, "Grille sectorielle " & TMaxDate.Text)
    End Sub


    Public Function IsRTF(ByVal strText As String) As Boolean
        Return strText.StartsWith("{\rtf1")
    End Function


    Private Sub DSecteursFga_CellDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DSecteursFga.CellDoubleClick
        If Not DsecteursNoEvent Then
            If DSecteursIcb.CurrentRow IsNot Nothing Then
                secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur FGA").Index).Value
                Me.Close()
                Dim bs As New Revue
                bs.Show()
                bs.cbSecteur.Text = secteur_fga
            End If
        End If
    End Sub


    ''' <summary>
    ''' Changement de secteur pour afficher les détails
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub newClickFga()
        If (DSecteursFga.RowCount()) Then
            If DSecteursFga.CurrentRow IsNot Nothing Then


                secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur FGA").Index).Value
                Dim id_secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id FGA").Index).Value
                Dim secteur_icb As String = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur ICB").Index).Value
                Dim id_secteur_icb As String = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id ICB").Index).Value

                'Grille News Flow
                'DNews.DataSource = co.LoadDataGridByString("SELECT CONVERT(VARCHAR, date, 103) As Date, Titre,Libelle FROM ACT_FGA_SECTOR_NEWS WHERE id_secteur='" & id_secteur_fga & "' ORDER BY date DESC")

                'Grille Histo
                Dim RecoFga As String = "SELECT 'ICB' As type, CONVERT(VARCHAR, date, 103) As date, date As datee,  '" & Replace(secteur_icb, "'", "''") & "' As 'Secteur',  id_recommandation As 'recommandation' FROM ACT_ICB_SECTOR_RECOMMANDATION WHERE id_secteur='" & id_secteur_icb & "' UNION "
                RecoFga = RecoFga & "SELECT 'FGA' As type, CONVERT(VARCHAR, date, 103) As date, date As datee,  '" & Replace(secteur_fga, "'", "''") & "' As 'Secteur', id_recommandation As 'recommandation' FROM ACT_FGA_SECTOR_RECOMMANDATION WHERE id_secteur='" & id_secteur_fga & "' ORDER BY date, type ASC"
                DRecoFga.DataSource = co.LoadDataGridByString(RecoFga)
                DRecoFga.Columns("datee").Visible = False


                'Last commentaire
                'If co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur_fga).Count <> 0 Then
                'TCommentaire.Rtf = co.SelectWhere2("ACT_FGA_SECTOR_RECOMMANDATION", "commentaire", "id_secteur", id_secteur_fga, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur_fga).FirstOrDefault, 1).FirstOrDefault
                'Else
                TCommentaireSecteur.Rtf = ""
                If DRecoFga.RowCount > 0 Then
                    DRecoFga.CurrentCell = DRecoFga.Rows(0).Cells(1)
                End If
                'End If

                ColoriageRecommandation(DRecoFga)

            End If
        End If
    End Sub


    'Public Sub newClickIcb()
    '    If (DSecteursIcb.RowCount()) Then
    '        If DSecteursIcb.CurrentRow IsNot Nothing Then


    '            'secteur_fga = DSecteursIcb.CurrentRow.Cells.Item(DSecteursIcb.Columns("Secteur FGA").Index).Value
    '            'Dim id_secteur_fga = DSecteursIcb.CurrentRow.Cells.Item(DSecteursIcb.Columns("Id FGA").Index).Value
    '            Dim secteur_icb As String = DSecteursIcb.CurrentRow.Cells.Item(DSecteursIcb.Columns("Secteur ICB").Index).Value
    '            Dim id_secteur_icb As String = DSecteursIcb.CurrentRow.Cells.Item(DSecteursIcb.Columns("Id ICB").Index).Value

    '            'Grille recommandation secteur icb
    '            Dim RecoIcb As String = "SELECT	CONVERT(VARCHAR, date, 103) As date,  '" & secteur_icb & "' As 'ICB',  id_recommandation As 'recommandation' FROM ACT_ICB_SECTOR_RECOMMANDATION WHERE id_secteur='" & id_secteur_icb & "' ORDER BY date ASC"
    '            DRecoIcb.DataSource = co.LoadDataGridByString(RecoIcb)

    '            ColoriageRecommandation(DRecoIcb)

    '        End If
    '    End If
    'End Sub



    'Private Sub DSecteurs_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursIcb.CurrentCellChanged
    '    If (Not DsecteursNoEvent) Then
    '        newClickIcb()
    '    End If
    'End Sub


    'Private Sub DSecteurs_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DSecteursIcb.DataBindingComplete
    '    If (Not DsecteursNoEvent) Then
    '        If DSecteursIcb.RowCount > 0 Then
    '            DsecteursNoEvent = True
    '            newClickIcb()
    '            'ColoriageRecommandation(DRecoFga)
    '            DSecteursIcb.CurrentCell = DSecteursIcb.Rows(0).Cells(1)
    '            DsecteursNoEvent = False
    '        End If
    '    End If
    'End Sub

    Private Sub DSecteursFga_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursFga.CurrentCellChanged
        newClickFga()
    End Sub




    Private Sub DSecteursFga_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DSecteursFga.DataBindingComplete
        'newClickFga()
    End Sub


    Public Sub ColoriageRecommandation(ByVal DataGrid As DataGridView)
        For i = 0 To DataGrid.Rows.Count - 1
            If DataGrid.Rows(i).Cells.Item(DataGrid.Columns("recommandation").Index).Value.ToString.Contains("-") Then ' DataGrid.Rows(i).Cells(2).Value.ToString.Contains("-") Then
                DataGrid.Rows(i).DefaultCellStyle.BackColor = Color.LightPink
                DataGrid.Rows(i).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Bold)
            ElseIf DataGrid.Rows(i).Cells.Item(DataGrid.Columns("recommandation").Index).Value.ToString.Contains("+") Then
                DataGrid.Rows(i).DefaultCellStyle.BackColor = Color.LightGreen
                DataGrid.Rows(i).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Bold)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Traitements lorsque la datagrid des DHistorique est chargée: présentation de la grille
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DRecoFga_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DRecoFga.DataBindingComplete
        'If (Not DHistoriqueNoEvent) Then
        '    If DRecoFga.RowCount > 0 Then
        '        DHistoriqueNoEvent = True
        '        For i = 0 To DRecoFga.Rows.Count - 1
        '            If DRecoFga.Rows(i).Cells(1).Value.ToString.Contains("Recommandation") Then
        '                DRecoFga.Rows(i).DefaultCellStyle.BackColor = Color.LightGoldenrodYellow
        '            ElseIf IsDBNull(DRecoFga.Rows(i).Cells(2).Value) = False Then
        '                If DRecoFga.Rows(i).Cells(1).Value.ToString.Contains("Commentaire") Then
        '                    DRecoFga.Rows(i).DefaultCellStyle.BackColor = Color.LightBlue
        '                End If
        '                If IsRTF(DRecoFga.Rows(i).Cells(2).Value) Then
        '                    'tranformer le RTF en text visible
        '                    Dim rtBox As System.Windows.Forms.RichTextBox = New System.Windows.Forms.RichTextBox()
        '                    rtBox.Rtf = DRecoFga.Rows(i).Cells(2).Value
        '                    DRecoFga.Rows(i).Cells(2).Value = rtBox.Text
        '                End If
        '            End If
        '        Next
        '        da.AutoFiltre(DRecoFga)
        '        DHistoriqueNoEvent = False
        '    End If
        'End If

        ColoriageRecommandation(DRecoFga)
    End Sub

    Private Sub CbIndiceSecteur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbIndiceSecteur.SelectedIndexChanged
        RemplirGrilleSecteur()
    End Sub



    Public Sub RemplirGrilleSecteur()

        Dim mon_indice As String = ""
        Select Case CbIndiceSecteur.Text
            Case "Stoxx 600"
                mon_indice = "SXXP"
            Case "Euro zone"
                mon_indice = "SXXE"
            Case "Ex euro"
                mon_indice = "SXXA"
        End Select


        Dim fga As String = " SELECT id_secteur, MAX(date) As date into #recommandation_fga FROM ACT_FGA_SECTOR_RECOMMANDATION GROUP BY id_secteur"

        fga = fga & " Select distinct"
        fga = fga & " i.id As id_industry, i.libelle As libelle_industry, "
        fga = fga & " sus.id As id_supersector, sus.libelle As libelle_supersector, "
        fga = fga & " f.id AS id_fga ,f.libelle As libelle_fga"
        fga = fga & " into #sectors"
        fga = fga & " from ACT_SUBSECTOR ss "
        fga = fga & " LEFT OUTER JOIN ACT_SECTOR s on s.id = ss.id_sector"
        fga = fga & " LEFT OUTER JOIN ACT_SUPERSECTOR sus ON sus.id=s.id_supersector"
        fga = fga & " LEFT OUTER JOIN ACT_FGA_SECTOR f ON f.id = ss.id_fga_sector"
        fga = fga & " LEFT OUTER JOIN ACT_INDUSTRY i on i.id = sus.id_industry"
        fga = fga & " order by id_fga"

        fga = fga & " DECLARE @indice As VARCHAR(4)"
        fga = fga & " Set @indice='" & mon_indice & "'"

        fga = fga & " DECLARE @date As DATETIME"
        fga = fga & " Set @date = (SELECT MAX(date) FROM ACT_DATA_FACTSET_AGR)"

        fga = fga & " SELECT"
        fga = fga & " s.id_supersector  As 'Id ICB', s.libelle_supersector  As 'Secteur ICB',"
        fga = fga & " s.id_fga As 'Id FGA', s.libelle_fga As 'Secteur FGA',"
        fga = fga & " CONVERT(VARCHAR, rfgad.date, 103) As 'Date',"
        fga = fga & " rfga.id_recommandation As 'FGA Recommandation',	"
        fga = fga & " a." & mon_indice & " As 'Poids Indice', "
        fga = fga & " 1/9 As 'Poids FGA',"
        fga = fga & " 1/9-a." & mon_indice & " As 'Diff', "
        fga = fga & " '' As 'New Reco'"
        fga = fga & " FROM #sectors s"
        fga = fga & " LEFT OUTER JOIN ACT_DATA_FACTSET_AGR a ON a.date=@date and a.indice= @indice and a.ICB_SUPERSECTOR IS NOT NULL and a.FGA_SECTOR IS NOT NULL and a.fga_sector=s.id_fga"
        fga = fga & " LEFT OUTER JOIN #recommandation_fga rfgad ON rfgad.id_secteur=s.id_fga"
        fga = fga & " LEFT OUTER JOIN ACT_FGA_SECTOR_RECOMMANDATION rfga ON rfga.id_secteur=s.id_fga and rfga.date=rfgad.date"
        fga = fga & " ORDER BY CONVERT(int,s.id_fga)"

        fga = fga & " DROP TABLE #recommandation_fga DROP TABLE #sectors"

        DSecteursFga.DataSource = co.LoadDataGridByString(fga)
        DSecteursFga.DefaultCellStyle.Format = "n2"



        Dim icb As String = "DECLARE @indice As VARCHAR(4)"
        icb = icb & " Set @indice='" & mon_indice & "'"

        icb = icb & " DECLARE @date As DATETIME"
        icb = icb & " Set @date = (SELECT MAX(date) FROM ACT_DATA_FACTSET_AGR)"

        icb = icb & " SELECT id_secteur, MAX(date) As date into #recommandation_icb FROM ACT_ICB_SECTOR_RECOMMANDATION GROUP BY id_secteur"

        icb = icb & " SELECT"
        icb = icb & " s.id  As 'Id ICB', s.libelle  As 'secteur ICB',"
        icb = icb & " CONVERT(VARCHAR, rfgad.date, 103)As 'Date',"
        icb = icb & " rfga.id_recommandation As 'FGA Recommandation',	"
        icb = icb & " a." & mon_indice & " As 'Poids Indice', "
        icb = icb & " 1/9 As 'Poids FGA',"
        icb = icb & " 1/9-a." & mon_indice & " As 'Diff', "
        icb = icb & " '' As 'New Reco'"
        icb = icb & " FROM ACT_SUPERSECTOR s"
        icb = icb & " LEFT OUTER JOIN ACT_DATA_FACTSET_AGR a ON a.date=@date and a.indice= @indice and a.ICB_SUPERSECTOR IS NOT NULL and a.FGA_SECTOR IS NULL and a.icb_supersector=s.id"
        icb = icb & " LEFT OUTER JOIN #recommandation_icb rfgad ON rfgad.id_secteur=s.id"
        icb = icb & " LEFT OUTER JOIN ACT_ICB_SECTOR_RECOMMANDATION rfga ON rfga.id_secteur=s.id and rfga.date=rfgad.date"
        icb = icb & " ORDER BY s.id"

        icb = icb & " DROP TABLE #recommandation_icb"


        DSecteursIcb.DataSource = co.LoadDataGridByString(icb)
        DSecteursIcb.DefaultCellStyle.Format = "n2"

    End Sub

    Private Sub DRecoFga_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DRecoFga.CurrentCellChanged
        If DRecoFga.CurrentRow IsNot Nothing Then

            secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur FGA").Index).Value
            Dim id_secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id FGA").Index).Value
            Dim secteur_icb As String = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur ICB").Index).Value
            Dim id_secteur_icb As String = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id ICB").Index).Value
            Dim type As String = DRecoFga.CurrentRow.Cells.Item(DRecoFga.Columns("Type").Index).Value
            Dim datee As Date = DRecoFga.CurrentRow.Cells.Item(DRecoFga.Columns("Datee").Index).Value

            'Last commentaire
            If co.SelectDistinctWhere("ACT_" & type & "_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur_fga).Count <> 0 Then
                TCommentaireSecteur.Rtf = co.SelectWhere2("ACT_" & type & "_SECTOR_RECOMMANDATION", "commentaire", "id_secteur", id_secteur_fga, "date", datee, 1).FirstOrDefault
                'co.SelectDistinctWhere("ACT_" & type & "_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur_fga).FirstOrDefault
            Else
                TCommentaireSecteur.Rtf = ""
            End If

        End If
    End Sub

    Private Sub BCommentaire_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCommentaireSecteur.Click
        'secteur_fga = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur FGA").Index).Value
        'Dim secteur_icb As String = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Secteur ICB").Index).Value

        Dim type As String = DRecoFga.CurrentRow.Cells.Item(DRecoFga.Columns("Type").Index).Value
        Dim id_secteur As String
        If type = "ICB" Then
            id_secteur = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id ICB").Index).Value
        Else
            id_secteur = DSecteursFga.CurrentRow.Cells.Item(DSecteursFga.Columns("Id FGA").Index).Value
        End If
        Dim datee As Date = DRecoFga.CurrentRow.Cells.Item(DRecoFga.Columns("Datee").Index).Value

        co.Updates("ACT_" & type & "_SECTOR_RECOMMANDATION", New List(Of String)(New String() {"commentaire"}), New List(Of Object)(New Object() {TCommentaireSecteur.Rtf}), New List(Of String)(New String() {"date", "id_secteur"}), New List(Of Object)(New Object() {datee, id_secteur}))
    End Sub


#End Region



#Region "Isin"

    Sub SelectionIsin()


    End Sub

    Private Sub CbIsinSelection_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbIsinSelection.SelectedIndexChanged
        'Afficher le portefeuille modele par isin et par secteur

        Dim id_secteur_fga As String = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", CbIsinSelection.Text).FirstOrDefault



    End Sub

    Private Sub BCommentaireIsin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCommentaireIsin.Click
        'Changement de commentaire pour la valeur
    End Sub

#End Region






End Class