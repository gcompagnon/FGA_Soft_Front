Imports System.Windows.Forms.DataVisualization.Charting
Imports WindowsApplication1.Utilities
Imports WindowsApplication1.FileLink
Imports Microsoft.Office
Imports WindowsApplication1.Action.Consultation
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Windows.Controls
Imports System.Data.SqlClient

Namespace Action
    Public Class BaseAction
        Implements PrintPreviewForm

        Dim co As Connection = New Connection()
        Dim fichier As Fichier = New Fichier()
        Dim excel As Excel = New Excel()
        Dim da As DGrid = New DGrid()
        Dim help As New HelpWindowAction()

        Private MyDataGridViewPrinter As New DataGridViewPrinter.DataGridViewPrinter(Me)

        ' Représentation des secteurs ICB, FGA et des entreprises sous forme d'objets
        ' Réutilisé par FileLink
        Private Stoxx As Stock = Nothing

        'Liste des secteurs ICB,FGA et des entreprises
        Private secteursICB As New List(Of Object)
        Private secteursFGA As New List(Of Object)
        Private entreprises As New List(Of Object)

        ''' <summary>
        ''' Liste des différentes grilles pour gérer les correspondances
        ''' </summary>
        Private grids As New List(Of DataGridView)
        Private gridmenus As New Dictionary(Of DataGridView, GridMenu)

        ''' <summary>
        ''' Dictionnaire des cellules sélectionnées avec le nom de la compagnie
        ''' et les numéros des colonnes de la ligne sélectionnées
        ''' </summary>
        Private selectedCells As New Dictionary(Of String, List(Of Integer))
        Private updatingSelection As Boolean = False
        Private updatingCombobox As Boolean = False

        ''' <summary>
        ''' Indique si un changement à de commentaire a eu lieu
        ''' </summary>
        Private commentHasChanged As Boolean = False
        Private oldCBValue As New Dictionary(Of System.Windows.Forms.ComboBox, String)

        Private _isLoading As Boolean = False
        'Private _weeklycheck As Boolean = False

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub BaseAction_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            _isLoading = True

            'Connection a la base
            co.ToConnectBase()

            'Chargement des listes de secteurs et d'entreprise.
            'secteursICB = co.SelectDistinctSimple("ACT_SUPERSECTOR", "libelle")
            secteursICB = co.SelectDistinctWheres("ref_security.SECTOR", "label", {"class_name", "level"}.ToList, {"GICS", 0}.ToList, , )
            'secteursFGA = co.SelectDistinctSimple("ACT_FGA_SECTOR", "libelle")
            secteursFGA = co.SelectDistinctWheres("ref_security.SECTOR", "label", {"class_name", "level"}.ToList, {"FGA_EU", 0}.ToList, , )
            'entreprises = co.SelectDistinctSimple("ACT_DATA_FACTSET", "company_name")
            'entreprises = co.SelectDistinctSimple("ref_security.ASSET", "FinancialInstrumentName")
            entreprises = co.SelectWhereNotNull("DATA_FACTSET", "Company_Name", "ISIN")

            'Récupération des différentes grilles
            TOnglet.TabPages.Remove(TStyle)
            TOngletEurope.TabPages.Remove(TSecteurAnalyse)
            TOngletEurope.TabPages.Remove(TGenerale)
            TOngletEurope.TabPages.Remove(TCroissance)
            TOngletEurope.TabPages.Remove(TQualite)
            TOngletEurope.TabPages.Remove(TValorisation)
            TOngletEurope.TabPages.Remove(TMomentum)
            TOngletEurope.TabPages.Remove(TNewsFlow)
            'grids.Add(DGenerale)
            'grids.Add(DCroissance)
            'grids.Add(DQualite)
            'grids.Add(DValorisation)
            'grids.Add(DMomentum)
            grids.Add(DValeursBlend)
            'grids.Add(DValeursNote)

            'gridmenus.Add(DGenerale, DGVMenu)
            'gridmenus.Add(DCroissance, DGVMenu)
            'gridmenus.Add(DQualite, DGVMenu)
            'gridmenus.Add(DValorisation, DGVMenu)
            'gridmenus.Add(DMomentum, DGVMenu)
            gridmenus.Add(DSecteursBlend, DGVMenu)
            gridmenus.Add(DSecteursNote, DGVMenu)
            gridmenus.Add(DValeursBlend, DGVMenu)
            'gridmenus.Add(DValeursNote, DGVMenu)
            gridmenus.Add(DSecteursReco, RecoMenu)
            gridmenus.Add(DValeursReco, RecoMenu)

            'Remplissage des combobox dates
            CbDateGeneral.DataSource = co.SelectDistinctSimple("DATA_FACTSET", "date", "DESC")
            CbDateGrowth.DataSource = CbDateGeneral.DataSource
            CbDateValue.DataSource = CbDateGeneral.DataSource
            CbDateRadar.DataSource = CbDateGeneral.DataSource


            'Remplissage onglet général
            secteursICB.Add("")
            secteursICB.Sort()
            CbSuperSector.DataSource = secteursICB
            'CbIndice.DataSource = New List(Of String)(New String() {"Stoxx 600", "Euro zone", "Ex euro"})
            CbIndiceGeneral.DataSource = New List(Of String)(New String() {"Europe", "USA"})
            'CbPrintGrille.DataSource = New List(Of String)(New String() {"Général", "Croissance", "Rentabilité", "Valorisation", "Momentum"})


            'Remplissage onglet Secteur/Analyse
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''''''''''''''''''''''''''''''''A DECOMMENTER SI ON VEUX REMETTRE LES SCORES DES SECTEURS''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' '' '' ''DSecteursIcb.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("Act_Sectorielle", New List(Of String)(New String() {}), New List(Of Object)(New Object() {})))
            '' '' ''da.RowHeaderCell(DSecteursNote)
            '' '' ''DSecteursNote.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_Sectorielle_Note", New List(Of String)(New String() {}), New List(Of Object)(New Object() {})))
            '' '' ''DSecteursNote.Columns(2).Frozen = True
            '' '' ''DSecteursNote.Columns(0).ReadOnly = True
            '' '' ''DSecteursNote.Columns(1).ReadOnly = True
            '' '' ''DSecteursNote.Columns(2).ReadOnly = True
            '' '' ''For i = 3 To DSecteursNote.ColumnCount - 1
            '' '' ''    'DSecteursNote.Columns(i - 1).ReadOnly = False
            '' '' ''    If IsDBNull(DSecteursNote.Rows(0).Cells(i).Value) Then
            '' '' ''        DSecteursNote.Columns(i).DefaultCellStyle.BackColor = Color.LightGray
            '' '' ''    End If
            '' '' ''Next
            '' '' ''DSecteursNote.Columns("Secteur FGA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            '' '' ''DSecteursNote.Columns("Sensibilité à amélioration macro").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("CA exposé aux émergents").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("CA exposé aux US").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Hausse du coût de refinancement").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Augmentation fiscalité").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Sécurité du dividende").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Baisse de l'€ favorable").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Hausse de l'€ favorable").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Sensibilité à la hausse du prix de l'énergie").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("Sensibilité à la hausse des coûts matières").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("CA exposé en Europe").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            '' '' ''DSecteursNote.Columns("""Shareholder friendly""").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter


            'Initialisation des Editbox
            'TCommentaireNews.ShowToolBar = True

            TCommentSecteursReco.ShowToolBar = True
            TCommentSecteursRecoChange.ShowToolBar = True
            TCommentValeursReco.ShowToolBar = True
            TCommentValeursRecoChange.ShowToolBar = True

            'Enlève la sélection automatique.
            'TOngletEurope.SelectTab(1)
            DValeursBlend.ClearSelection()
            'DValeursNote.ClearSelection()
            TOngletEurope.SelectTab(0)

            'Affiche la liste des superSecteurs par défault
            CbSuperSector.SelectedIndex = 0
            'fillGridsGeneral()
            refreshOldCBValue()

            ' Select Combobox Indice
            TOnglet.Focus()
            GBFiltre.Focus()
            LHeader.Focus()
            CbIndiceGeneral.Focus()

            setSize()
            Label15_Click()
            Label11_Click()
            RadioButton_Chart2.Select()
            DTPcheck1.Value = CbDateGeneral.Text
            Dim stock As List(Of Dictionary(Of String, Object))
            stock = co.sqlToListDico("SELECT distinct DATE AS 'date' FROM DATA_FACTSET WHERE date<'" + CbDateGeneral.Text + "'")
            If stock.Count > 0 Then
                DTPcheck2.Value = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<'" + CbDateGeneral.Text + "'"))(0))("date")
                stock.Clear()
            End If
            'ne plus faire le controle des chgt de scores
            'WeeklyCheck(0)
            _isLoading = False
        End Sub

        ''' <summary>
        ''' Click sur le menu importation d'un nouveau fichier
        ''' </summary>
        Private Sub ImportationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.Close()
            Dim bs As New BaseActionImportation
            bs.Show()
        End Sub

        Private Sub myPrintDocument_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles myPrintDocument.PrintPage
            Dim more As Boolean = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more = True Then
                e.HasMorePages = True
            End If
        End Sub

        Private Sub BPrintt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            'Dim dataChoice As DataGridView = Nothing
            'If String.IsNullOrEmpty(CbPrint.Text) = False Then
            'Select Case CbPrint.Text
            '   Case "Agrégation"
            'dataChoice = DAgregation
            '   Case "Growth"
            'dataChoice = DGrowth
            '    Case "Value"
            'dataChoice = DValue
            '    Case "Garp"
            'dataChoice = DGarp
            '    Case Else
            'MsgBox("Cette datagrid n'existe pas")
            'End Select

            'If MyDataGridViewPrinter.SetupThePrinting(dataChoice, myPrintDocument, Me.Text) Then
            'myPrintDocument.Print()
            'End If

            'Else
            'MessageBox.Show("Il faut choisir la datagrid à imprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            'End If
        End Sub

        ''' <summary>
        ''' Preview de TOUTES les impressions
        ''' </summary>
        Public Sub Preview(ByVal PrintCenterReportOnPage As Boolean, ByVal PrintFont As System.Drawing.Font, ByVal PrintFontColor As Color) Implements PrintPreviewForm.Preview
            'Dim dataChoice As DataGridView = Nothing
            'Select Case CbPrint.Text
            '    Case "Agrégation"
            'dataChoice = DAgregation
            '    Case "Growth"
            'dataChoice = DGrowth
            '    Case "Value"
            'dataChoice = DValue
            '    Case Else
            'MsgBox("Cette datagrid n'existe pas")
            'End Select

            'MyDataGridViewPrinter.Init_Parameters(dataChoice, myPrintDocument, PrintCenterReportOnPage, True, myPrintDocument.DocumentName, New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, PrintFont, PrintFontColor, True)
            'Dim MyPrintPreviewDialog As New PrintPreviewDialog()
            'MyPrintPreviewDialog.Document = myPrintDocument
            'MyPrintPreviewDialog.ShowDialog()
        End Sub


#Region "Europe - Grille Facset : Générale, croissance, rentabilité, valorarisation"

        '        <STAThread()> _
        '        Private Sub BGeneral_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BChargerGeneral.Click
        '            'fillGridsGeneral()
        '        End Sub

        '        ''' <summary>
        '        ''' Rempli l'ensemble des grilles à partir des valeurs renseignées dans les combobox.
        '        ''' </summary>
        '        Private Sub fillGridsGeneral()
        '            If co.SelectDistinctWhere("ACT_DATA_FACTSET", "date", "date", CbDateGeneral.Text).Count > 0 Then
        '                Windows.Forms.Cursor.Current = Cursors.WaitCursor

        '                'Variable en commun avec toutes les grilles
        '                Dim mon_supersector As String = co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", CbSuperSector.Text).FirstOrDefault
        '                Dim mon_sector_fga As String = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", CbSectorFGA.Text).FirstOrDefault

        '                Dim mon_indice As String = ""
        '                Select Case CbIndiceGeneral.Text
        '                    Case "Stoxx 600"
        '                        mon_indice = "SXXP"
        '                    Case "Euro zone"
        '                        mon_indice = "SXXE"
        '                    Case "Ex euro"
        '                        mon_indice = "SXXA"
        '                End Select

        '                'Dim type As String
        '                'Select Case CbSuperSector.Text
        '                '    Case "**isin**"
        '                'Type = "isin"
        '                '    Case "**super sectors**"
        '                'Type = "super sectors"
        '                '    Case Else
        '                'Type = "secteur"
        '                'End Select
        '                'Dim sql As String = Replace(Replace(Replace(Replace(fichier.LectureFichierSql(My.Settings.PATH & "\SQL_SCRIPTS\TEMPLATE\ACTION", "Agregation.sql"), "mon_indice", indice), "ma_date", CbDateGeneral.Text), "mon_secteur", secteur), "mon_type", Type)

        '                'If CbSector.Text = "" Then
        '                '   sql = Replace(sql,"sqlsupersector", "
        '                'Else
        '                'End If




        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille DGenerale
        '                clearGrid(DGenerale)

        '                Dim sqlAgr As String = ""
        '                sqlAgr = sqlAgr & " NULL AS 'Isin',"
        '                sqlAgr = sqlAgr & " " & mon_indice & " As '" & mon_indice & "',"
        '                sqlAgr = sqlAgr & " NULL As 'Price', "
        '                sqlAgr = sqlAgr & " MARKET_CAP_EUR As 'Mkt Cap m€',"
        '                sqlAgr = sqlAgr & " SALES_CY_EUR As 'Sales CY m€', "
        '                sqlAgr = sqlAgr & " NULL As 'SRI EUROPE',"
        '                sqlAgr = sqlAgr & " NULL As 'SRI EURO',"
        '                sqlAgr = sqlAgr & " NULL As 'SRI ExEURO',"
        '                sqlAgr = sqlAgr & " NULL As 'Liquidity',"
        '                sqlAgr = sqlAgr & " NULL As 'Exclu',"
        '                sqlAgr = sqlAgr & " NULL As 'Load Factor LY %',"
        '                sqlAgr = sqlAgr & " NULL As 'Load Factor CY %',"
        '                sqlAgr = sqlAgr & " NULL As 'Revenue Passenger LY',"
        '                sqlAgr = sqlAgr & " NULL As 'Revenue Passenger CY',"
        '                sqlAgr = sqlAgr & " NULL As 'Prod per day CY/LY %',"
        '                sqlAgr = sqlAgr & " NULL As 'Prod per day NY/CY %',"
        '                sqlAgr = sqlAgr & " NULL As 'Tier1 CY %',"
        '                sqlAgr = sqlAgr & " NULL As 'Tier1 NY %'"



        '                Dim sqlFact = ""
        '                sqlFact = sqlFact & " f.isin AS 'Isin',"
        '                sqlFact = sqlFact & " " & mon_indice & " As '" & mon_indice & "',"
        '                sqlFact = sqlFact & " PRICE As 'Price', "
        '                sqlFact = sqlFact & " MARKET_CAP_EUR As 'Mkt Cap m€',"
        '                sqlFact = sqlFact & " SALES_CY_EUR As 'Sales CY m€', "
        '                sqlFact = sqlFact & " i.europe As 'SRI EUROPE',"
        '                sqlFact = sqlFact & " i.euro As 'SRI EURO',"
        '                sqlFact = sqlFact & " i.exeuro As 'SRI ExEURO',"
        '                sqlFact = sqlFact & " LIQUIDITY_TEST As 'Liquidity',"
        '                sqlFact = sqlFact & " v.EXCLUSION As 'Exclu',"
        '                sqlFact = sqlFact & " LOAD_FACTOR_LY As 'Load Factor LY %',"
        '                sqlFact = sqlFact & " LOAD_FACTOR_CY As 'Load Factor CY %',"
        '                sqlFact = sqlFact & " REVENUE_PASSENGER_LY As 'Revenue Passenger LY',"
        '                sqlFact = sqlFact & " REVENUE_PASSENGER_CY As 'Revenue Passenger CY',"
        '                sqlFact = sqlFact & " PROD_PER_DAY_GROWTH_CY As 'Prod per day CY/LY %',"
        '                sqlFact = sqlFact & " PROD_PER_DAY_GROWTH_NY As 'Prod per day NY/CY %',"
        '                sqlFact = sqlFact & " TIER_1_CY As 'Tier1 CY %',"
        '                sqlFact = sqlFact & " TIER_1_NY As 'Tier1 NY %'"

        '                da.RowHeaderCell(DGenerale)
        '                DGenerale.DataSource = co.LoadDataGridByString(sql(sqlAgr, sqlFact, mon_indice, mon_supersector, mon_sector_fga))

        '                'Format de toutes les colonnes
        '                For Each col As DataGridViewColumn In DGenerale.Columns
        '                    col.ReadOnly = True
        '                Next

        '                DGenerale.Columns("FGA Sector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DGenerale.Columns("Company").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'DGenerale.Columns("ICB Industry").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DGenerale.Columns("ICB SuperSector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'DGenerale.Columns("Country").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'DGenerale.Columns("Country").Frozen = True
        '                DGenerale.Columns(mon_indice).DefaultCellStyle.Format = "#0.##\%"
        '                DGenerale.Columns("Price").DefaultCellStyle.Format = "n2"
        '                DGenerale.Columns("Mkt Cap m€").DefaultCellStyle.Format = "n0"
        '                DGenerale.Columns("Sales CY m€").DefaultCellStyle.Format = "n0"

        '                For Each col As DataGridViewColumn In DGenerale.Columns
        '                    If col.HeaderText.Contains("SRI") Then
        '                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        '                        col.Width = 60

        '                        For Each row As DataGridViewRow In DGenerale.Rows
        '                            Dim cell As DataGridViewCell = row.Cells(col.HeaderText)
        '                            Dim note As Decimal

        '                            cell.Style.Format = "n1"
        '                            If Not IsDBNull(cell.Value) AndAlso Decimal.TryParse(cell.Value, note) Then
        '                                If note < 10 Then
        '                                    cell.Style.BackColor = Color.LightPink
        '                                End If
        '                            End If
        '                        Next
        '                    End If
        '                Next

        '                DGenerale.Columns("Liquidity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        '                DGenerale.Columns("Exclu").ReadOnly = False
        '                DGenerale.Columns("Isin").Visible = False
        '                'DGenerale.Columns.Remove("ICB Industry")




        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille momentum
        '                clearGrid(DMomentum)

        '                sqlAgr = ""
        '                sqlAgr = sqlAgr & " PERF_1M As 'Perf 1M %', PERF_3M  As 'Perf 3M %', PERF_6M As 'Perf 6M %', PERF_1YR  As 'Perf 1Y %', PERF_MTD  As 'Perf MTD %', PERF_YTD  As 'Perf YTD %', "
        '                sqlAgr = sqlAgr & " VOL_1M  As 'Vol 1M', VOL_3M  As 'Vol 3M', VOL_1YR  As 'Vol 1Y', "
        '                sqlAgr = sqlAgr & " BETA_1YR   As 'Beta 1Y', "
        '                sqlAgr = sqlAgr & " NULL As 'Price 1M %', NULL As 'Price 3M %', NULL As 'Price 1Y %', NULL As 'Price 5Y %', NULL As 'Price', NULL As 'Price 52 high', NULL As 'Price 52 low', "
        '                sqlAgr = sqlAgr & " NULL As 'Target', NULL As 'Target FB', NULL As 'Target UpSide %',"
        '                sqlAgr = sqlAgr & " EPS_CHG_1M As 'EPS Rev 1M %', EPS_CHG_3M As 'EPS Rev 3M %', EPS_CHG_6M As 'EPS Rev 6M %', EPS_CHG_1YR As 'EPS Rev 1Y %', EPS_CHG_YTD As 'EPS Rev YTD %',"
        '                sqlAgr = sqlAgr & " NULL As 'Rating pos %', NULL As 'Suivi rating', NULL As 'Rating pos FB %', NULL As 'Suivi FB',"
        '                sqlAgr = sqlAgr & " MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                sqlFact = ""
        '                sqlFact = sqlFact & " PERF_1M As 'Perf 1M %', PERF_3M  As 'Perf 3M %', PERF_6M As 'Perf 6M %', PERF_1YR  As 'Perf 1Y %', PERF_MTD  As 'Perf MTD %', PERF_YTD  As 'Perf YTD %', "
        '                sqlFact = sqlFact & " VOL_1M  As 'Vol 1M', VOL_3M  As 'Vol 3M', VOL_1YR  As 'Vol 1Y', "
        '                sqlFact = sqlFact & " BETA_1YR   As 'Beta 1Y', "
        '                sqlFact = sqlFact & " PRICE_PCTIL_1M As 'Price 1M %', PRICE_PCTIL_3M As 'Price 3M %', PRICE_PCTIL_1YR As 'Price 1Y %', PRICE_PCTIL_5YR As 'Price 5Y %', PRICE As 'Price', PRICE_52_HIGH As 'Price 52 high', PRICE_52_LOW As 'Price 52 low', "
        '                sqlFact = sqlFact & " TARGET As 'Target', TARGET_FB As 'Target FB', TARGET_UPDOWN_FB As 'Target UpSide %',"
        '                sqlFact = sqlFact & " EPS_CHG_1M As 'EPS Rev 1M %', EPS_CHG_3M As 'EPS Rev 3M %', EPS_CHG_6M As 'EPS Rev 6M %', EPS_CHG_1YR As 'EPS Rev 1Y %', EPS_CHG_YTD As 'EPS Rev YTD %',"
        '                sqlFact = sqlFact & " RATING_POS_PCT As 'Rating pos %', RATING_TOT As ' Suivi rating', RATING_POS_PCT_FB As 'Rating pos FB %', RATING_TOT_FB As 'Suivi FB',"
        '                sqlFact = sqlFact & " MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                da.RowHeaderCell(DMomentum)
        '                DMomentum.DataSource = co.LoadDataGridByString(sql(sqlAgr, sqlFact, mon_indice, mon_supersector, mon_sector_fga))
        '                ' lorsque le chargement est terminé , l 'event datagrid complete est appelé.
        '                'da.AutoFiltre(DMomentum, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5}))
        '                da.PresentationDataGrid(DMomentum, 1)
        '                DMomentum.Columns("Perf 1M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Perf 3M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Perf 6M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Perf 1Y %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Perf MTD %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Perf YTD %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Price 1M %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 3M %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 1Y %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 5Y %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 1M %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Price 3M %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Price 1Y %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Price 5Y %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Crncy").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price").DefaultCellStyle.Format = "n2"
        '                DMomentum.Columns("Beta 1Y").DefaultCellStyle.Format = "n2"
        '                DMomentum.Columns("Price 52 high").DefaultCellStyle.Format = "n2"
        '                DMomentum.Columns("Price 52 low").DefaultCellStyle.Format = "n2"
        '                DMomentum.Columns("Price").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 52 high").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Price 52 low").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Target").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Target FB").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Target UpSide %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Rating pos %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Suivi rating").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Rating pos FB %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        '                DMomentum.Columns("Suivi FB").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        '                DMomentum.Columns("Target UpSide %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Rating pos %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Suivi rating").DefaultCellStyle.Format = "n0"
        '                DMomentum.Columns("Rating pos FB %").DefaultCellStyle.Format = "#0\%"
        '                DMomentum.Columns("Suivi FB").DefaultCellStyle.Format = "n0"
        '                DMomentum.Columns("EPS Rev 1M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("EPS Rev 3M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("EPS Rev 6M %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("EPS Rev 1Y %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("EPS Rev YTD %").DefaultCellStyle.Format = "#0.#\%"
        '                DMomentum.Columns("Target").DefaultCellStyle.Format = "n1"
        '                DMomentum.Columns("Target FB").DefaultCellStyle.Format = "n1"





        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille croissance
        '                clearGrid(DCroissance)

        '                'Ligne d'agrégation
        '                sqlAgr = ""
        '                sqlAgr = sqlAgr & " EPS_GROWTH_CY As 'EPS CY/LY %', "
        '                sqlAgr = sqlAgr & " EPS_GROWTH_NY As 'EPS NY/CY %',"
        '                sqlAgr = sqlAgr & " SALES_GROWTH_CY As 'Sales CY/LY %', "
        '                sqlAgr = sqlAgr & " SALES_GROWTH_NY As 'Sales NY/CY %',"
        '                sqlAgr = sqlAgr & " EBIT_GROWTH_CY As 'EBIT CY/LY %', "
        '                sqlAgr = sqlAgr & " EBIT_GROWTH_NY As 'EBIT NY/CY %',"
        '                sqlAgr = sqlAgr & " PBT_GROWTH_CY As 'PBT CY/LY %', "
        '                sqlAgr = sqlAgr & " PBT_GROWTH_NY As 'PBT NY/CY %',"
        '                sqlAgr = sqlAgr & " EPS_LY As 'EPS LY', "
        '                sqlAgr = sqlAgr & " EPS_CY As 'EPS CY', "
        '                sqlAgr = sqlAgr & " EPS_NY As 'EPS NY', "
        '                sqlAgr = sqlAgr & " SALES_LY As 'Sales LY', "
        '                sqlAgr = sqlAgr & " SALES_CY As 'Sales CY', "
        '                sqlAgr = sqlAgr & " SALES_NY As 'Sales NY',"
        '                sqlAgr = sqlAgr & " EBIT_LY As 'EBIT LY', "
        '                sqlAgr = sqlAgr & " EBIT_CY As 'EBIT CY', "
        '                sqlAgr = sqlAgr & " EBIT_NY As 'EBIT NY',"
        '                sqlAgr = sqlAgr & " PBT_LY As 'PBT LY', "
        '                sqlAgr = sqlAgr & " PBT_CY As 'PBT CY', "
        '                sqlAgr = sqlAgr & " PBT_NY As 'PBT NY',"
        '                sqlAgr = sqlAgr & "  MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                sqlFact = ""
        '                sqlFact = sqlFact & " EPS_GROWTH_CY As 'EPS CY/LY %', "
        '                sqlFact = sqlFact & " EPS_GROWTH_NY As 'EPS NY/CY %',"
        '                sqlFact = sqlFact & " SALES_GROWTH_CY As 'Sales CY/LY %', "
        '                sqlFact = sqlFact & " SALES_GROWTH_NY As 'Sales NY/CY %',"
        '                sqlFact = sqlFact & " EBIT_GROWTH_CY As 'EBIT CY/LY %', "
        '                sqlFact = sqlFact & " EBIT_GROWTH_NY As 'EBIT NY/CY %',"
        '                sqlFact = sqlFact & " PBT_GROWTH_CY As 'PBT CY/LY %', "
        '                sqlFact = sqlFact & " PBT_GROWTH_NY As 'PBT NY/CY %',"
        '                sqlFact = sqlFact & " EPS_LY As 'EPS LY', "
        '                sqlFact = sqlFact & " EPS_CY As 'EPS CY', "
        '                sqlFact = sqlFact & " EPS_NY As 'EPS NY', "
        '                sqlFact = sqlFact & " SALES_LY As 'Sales LY', "
        '                sqlFact = sqlFact & " SALES_CY As 'Sales CY', "
        '                sqlFact = sqlFact & " SALES_NY As 'Sales NY',"
        '                sqlFact = sqlFact & " EBIT_LY As 'EBIT LY', "
        '                sqlFact = sqlFact & " EBIT_CY As 'EBIT CY', "
        '                sqlFact = sqlFact & " EBIT_NY As 'EBIT NY',"
        '                sqlFact = sqlFact & " PBT_LY As 'PBT LY', "
        '                sqlFact = sqlFact & " PBT_CY As 'PBT CY', "
        '                sqlFact = sqlFact & " PBT_NY As 'PBT NY',"
        '                sqlFact = sqlFact & " MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                da.RowHeaderCell(DCroissance)
        '                DCroissance.DataSource = co.LoadDataGridByString(sql(sqlAgr, sqlFact, mon_indice, mon_supersector, mon_sector_fga))

        '                'Format de toutes les colonnes
        '                'da.AutoFiltre(DCroissance, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5}))
        '                DCroissance.Columns("FGA Sector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DCroissance.Columns("Company").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DCroissance.Columns("ICB SuperSector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'Croissance
        '                DCroissance.Columns("Sales LY").DefaultCellStyle.Format = "n0"
        '                DCroissance.Columns("Sales CY").DefaultCellStyle.Format = "n0"
        '                DCroissance.Columns("Sales NY").DefaultCellStyle.Format = "n0"
        '                DCroissance.Columns("Sales CY/LY %").DefaultCellStyle.Format = "#0.##\%"
        '                DCroissance.Columns("Sales NY/CY %").DefaultCellStyle.Format = "#0.##\%"
        '                DCroissance.Columns("EPS LY").DefaultCellStyle.Format = "n2"
        '                DCroissance.Columns("EPS CY").DefaultCellStyle.Format = "n2"
        '                DCroissance.Columns("EPS NY").DefaultCellStyle.Format = "n2"
        '                DCroissance.Columns("EPS CY/LY %").DefaultCellStyle.Format = "#0.#\%"
        '                DCroissance.Columns("EPS NY/CY %").DefaultCellStyle.Format = "#0.#\%"










        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille Rentabilité
        '                clearGrid(DQualite)

        '                'Ligne d'agrégation
        '                sqlAgr = ""
        '                sqlAgr = sqlAgr & " EBIT_MARGIN_LY As 'EBIT Margin LY %', "
        '                sqlAgr = sqlAgr & " EBIT_MARGIN_CY As 'EBIT Margin CY %', "
        '                sqlAgr = sqlAgr & " EBIT_MARGIN_NY As 'EBIT Margin NY %',"
        '                sqlAgr = sqlAgr & " EBIT_MARGIN_DIFF_LY_CY As 'EBIT Margin CY/LY (bp)',"
        '                sqlAgr = sqlAgr & " EBIT_MARGIN_DIFF_CY_NY As 'EBIT Margin NY/CY (bp)',"
        '                sqlAgr = sqlAgr & " NULL AS 'EBIT Margin / Avg5Y x',"
        '                sqlAgr = sqlAgr & " NULL AS 'EBIT Margin Momentum', "
        '                sqlAgr = sqlAgr & " PBT_RWA_LY As 'PBT/RWA LY %', "
        '                sqlAgr = sqlAgr & " PBT_RWA_CY As 'PBT/RWA CY %', "
        '                sqlAgr = sqlAgr & " PBT_RWA_NY As 'PBT/RWA NY %',"
        '                sqlAgr = sqlAgr & " PBT_RWA_DIFF_LY_CY As 'PBT/RWA CY/LY',"
        '                sqlAgr = sqlAgr & " PBT_RWA_DIFF_CY_NY As 'PBT/RWA NY/CY',"
        '                sqlAgr = sqlAgr & " PBT_SALES_LY As 'PBT/SALES LY %', "
        '                sqlAgr = sqlAgr & " PBT_SALES_CY As 'PBT/SALES CY %', "
        '                sqlAgr = sqlAgr & " PBT_SALES_NY As 'PBT/SALES NY %',"
        '                sqlAgr = sqlAgr & " PBT_SALES_DIFF_LY_CY As 'PBT/SALES CY/LY (bp)',"
        '                sqlAgr = sqlAgr & " PBT_SALES_DIFF_CY_NY As 'PBT/SALES NY/CY (bp)',"
        '                sqlAgr = sqlAgr & " ROE_LY As 'ROE LY %', "
        '                sqlAgr = sqlAgr & " ROE_CY As 'ROE CY %', "
        '                sqlAgr = sqlAgr & " ROE_NY As 'ROE NY %', "
        '                sqlAgr = sqlAgr & " ROTE_LY As 'ROTE LY %', "
        '                sqlAgr = sqlAgr & " ROTE_CY As 'ROTE CY %', "
        '                sqlAgr = sqlAgr & " ROTE_NY As 'ROTE NY %', "
        '                sqlAgr = sqlAgr & " RORWA_LY As 'RORWA LY %', "
        '                sqlAgr = sqlAgr & " RORWA_CY As 'RORWA CY %', "
        '                sqlAgr = sqlAgr & " RORWA_NY As 'RORWA NY %', "
        '                sqlAgr = sqlAgr & " NET_DEBT_EBITDA_LY As 'Net Debt/EBITDA LY x', "
        '                sqlAgr = sqlAgr & " NET_DEBT_EBITDA_CY As 'Net Debt/EBITDA CY x', "
        '                sqlAgr = sqlAgr & " NET_DEBT_EBITDA_NY As 'Net Debt/EBITDA NY x', "
        '                sqlAgr = sqlAgr & " GEARING_LY As 'Gearing LY %', "
        '                sqlAgr = sqlAgr & " GEARING_CY As 'Gearing CY %', "
        '                sqlAgr = sqlAgr & " GEARING_NY As 'Gearing NY %', "
        '                sqlAgr = sqlAgr & " NET_DEBT_LY As 'Net Debt LY', "
        '                sqlAgr = sqlAgr & " NET_DEBT_CY As 'Net Debt CY', "
        '                sqlAgr = sqlAgr & " NET_DEBT_NY As 'Net Debt NY', "
        '                sqlAgr = sqlAgr & " CAPEX_SALES_NY As 'Capex/Sales LY %', "
        '                sqlAgr = sqlAgr & " CAPEX_SALES_CY As 'Capex/Sales CY %', "
        '                sqlAgr = sqlAgr & " CAPEX_SALES_LY As 'Capex/Sales NY %', "
        '                sqlAgr = sqlAgr & " FCF_YLD_LY As 'FCF Yld LY %', "
        '                sqlAgr = sqlAgr & " FCF_YLD_CY As 'FCF Yld CY %', "
        '                sqlAgr = sqlAgr & " FCF_YLD_NY As 'FCF Yld NY %', "
        '                sqlAgr = sqlAgr & " CAPEX_LY As 'Capex LY', "
        '                sqlAgr = sqlAgr & " CAPEX_CY As 'Capex CY', "
        '                sqlAgr = sqlAgr & " CAPEX_NY As 'Capex NY', "
        '                sqlAgr = sqlAgr & " FCF_LY As 'FCF LY', "
        '                sqlAgr = sqlAgr & " FCF_CY As 'FCF CY', "
        '                sqlAgr = sqlAgr & " FCF_NY As 'FCF NY', "
        '                sqlAgr = sqlAgr & " MARKET_CAP_EUR As 'Mkt Cap m€'"
        '                'sqlAgr = sqlAgr & " DPS_LY As 'DPS LY',"
        '                'sqlAgr = sqlAgr & " DPS_CY As 'DPS CY',"
        '                'sqlAgr = sqlAgr & " DPS_NY As 'DPS NY',"
        '                'sqlAgr = sqlAgr & " PAYOUT_LY As 'Payout LY %',"
        '                'sqlAgr = sqlAgr & " PAYOUT_CY As 'Payout CY %',"
        '                'sqlAgr = sqlAgr & " PAYOUT_NY As 'Payout NY %',"
        '                sqlAgr = sqlAgr & ", NULL As 'Coût/Revenu %'"


        '                sqlFact = ""
        '                sqlFact = sqlFact & " EBIT_MARGIN_LY As 'EBIT Margin LY %', "
        '                sqlFact = sqlFact & " EBIT_MARGIN_CY As 'EBIT Margin CY %', "
        '                sqlFact = sqlFact & " EBIT_MARGIN_NY As 'EBIT Margin NY %',"
        '                sqlFact = sqlFact & " EBIT_MARGIN_DIFF_LY_CY As 'EBIT Margin CY/LY (bp)',"
        '                sqlFact = sqlFact & " EBIT_MARGIN_DIFF_CY_NY As 'EBIT Margin NY/CY (bp)',"
        '                sqlFact = sqlFact & " EBIT_MARGIN_ON_AVG5Y AS 'EBIT Margin / Avg5Y x', "
        '                sqlFact = sqlFact & " CASE  WHEN EBIT_MARGIN_STM <= EBIT_MARGIN_NTM AND EBIT_MARGIN_NTM <= EBIT_MARGIN_LTM THEN '-' "
        '                sqlFact = sqlFact & "       WHEN EBIT_MARGIN_STM >= EBIT_MARGIN_NTM AND EBIT_MARGIN_NTM >= EBIT_MARGIN_LTM THEN '+'"
        '                sqlFact = sqlFact & "       ELSE NULL "
        '                sqlFact = sqlFact & " END AS 'EBIT Margin Momentum', "
        '                sqlFact = sqlFact & " PBT_RWA_LY As 'PBT/RWA LY %', "
        '                sqlFact = sqlFact & " PBT_RWA_CY As 'PBT/RWA CY %', "
        '                sqlFact = sqlFact & " PBT_RWA_NY As 'PBT/RWA NY %',"
        '                sqlFact = sqlFact & " PBT_RWA_DIFF_LY_CY As 'PBT/RWA CY/LY',"
        '                sqlFact = sqlFact & " PBT_RWA_DIFF_CY_NY As 'PBT/RWA NY/CY',"
        '                sqlFact = sqlFact & " PBT_SALES_LY As 'PBT/SALES LY %', "
        '                sqlFact = sqlFact & " PBT_SALES_CY As 'PBT/SALES CY %', "
        '                sqlFact = sqlFact & " PBT_SALES_NY As 'PBT/SALES NY %',"
        '                sqlFact = sqlFact & " PBT_SALES_DIFF_LY_CY As 'PBT/SALES CY/LY (bp)',"
        '                sqlFact = sqlFact & " PBT_SALES_DIFF_CY_NY As 'PBT/SALES NY/CY (bp)',"
        '                sqlFact = sqlFact & " ROE_LY As 'ROE LY %', "
        '                sqlFact = sqlFact & " ROE_CY As 'ROE CY %', "
        '                sqlFact = sqlFact & " ROE_NY As 'ROE NY %', "
        '                sqlFact = sqlFact & " ROTE_LY As 'ROTE LY %', "
        '                sqlFact = sqlFact & " ROTE_CY As 'ROTE CY %', "
        '                sqlFact = sqlFact & " ROTE_NY As 'ROTE NY %', "
        '                sqlFact = sqlFact & " RORWA_LY As 'RORWA LY %', "
        '                sqlFact = sqlFact & " RORWA_CY As 'RORWA CY %', "
        '                sqlFact = sqlFact & " RORWA_NY As 'RORWA NY %', "
        '                sqlFact = sqlFact & " NET_DEBT_EBITDA_LY As 'Net Debt/EBITDA LY x', "
        '                sqlFact = sqlFact & " NET_DEBT_EBITDA_CY As 'Net Debt/EBITDA CY x', "
        '                sqlFact = sqlFact & " NET_DEBT_EBITDA_NY As 'Net Debt/EBITDA NY x', "
        '                sqlFact = sqlFact & " GEARING_LY As 'Gearing LY %', "
        '                sqlFact = sqlFact & " GEARING_CY As 'Gearing CY %', "
        '                sqlFact = sqlFact & " GEARING_NY As 'Gearing NY %', "
        '                sqlFact = sqlFact & " NET_DEBT_LY As 'Net Debt LY', "
        '                sqlFact = sqlFact & " NET_DEBT_CY As 'Net Debt CY', "
        '                sqlFact = sqlFact & " NET_DEBT_NY As 'Net Debt NY', "
        '                sqlFact = sqlFact & " CAPEX_SALES_NY As 'Capex/Sales LY %', "
        '                sqlFact = sqlFact & " CAPEX_SALES_CY As 'Capex/Sales CY %', "
        '                sqlFact = sqlFact & " CAPEX_SALES_LY As 'Capex/Sales NY %', "
        '                sqlFact = sqlFact & " FCF_YLD_LY As 'FCF Yld LY %', "
        '                sqlFact = sqlFact & " FCF_YLD_CY As 'FCF Yld CY %', "
        '                sqlFact = sqlFact & " FCF_YLD_NY As 'FCF Yld NY %', "
        '                sqlFact = sqlFact & " CAPEX_LY As 'Capex LY', "
        '                sqlFact = sqlFact & " CAPEX_CY As 'Capex CY', "
        '                sqlFact = sqlFact & " CAPEX_NY As 'Capex NY', "
        '                sqlFact = sqlFact & " FCF_LY As 'FCF LY', "
        '                sqlFact = sqlFact & " FCF_CY As 'FCF CY', "
        '                sqlFact = sqlFact & " FCF_NY As 'FCF NY', "
        '                sqlFact = sqlFact & " MARKET_CAP_EUR As 'Mkt Cap m€'"
        '                'sqlFact = sqlFact & " DPS_LY As 'DPS LY',"
        '                'sqlFact = sqlFact & " DPS_CY As 'DPS CY',"
        '                'sqlFact = sqlFact & " DPS_NY As 'DPS NY',"
        '                'sqlFact = sqlFact & " PAYOUT_LY As 'Payout LY %',"
        '                'sqlFact = sqlFact & " PAYOUT_CY As 'Payout CY %',"
        '                'sqlFact = sqlFact & " PAYOUT_NY As 'Payout NY %',"
        '                sqlFact = sqlFact & ", COST_INCOME_NTM As 'Coût/Revenu %'"
        '                da.RowHeaderCell(DQualite)
        '                DQualite.DataSource = co.LoadDataGridByString(sql(sqlAgr, sqlFact, mon_indice, mon_supersector, mon_sector_fga))

        '                'Format de toutes les colonnes
        '                'da.AutoFiltre(DQualite, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5}))
        '                DQualite.Columns("FGA Sector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DQualite.Columns("Company").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DQualite.Columns("ICB SuperSector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'Rentabilité
        '                DQualite.Columns("ROE LY %").DefaultCellStyle.Format = "#0.#\%"
        '                DQualite.Columns("ROE CY %").DefaultCellStyle.Format = "#0.#\%"
        '                DQualite.Columns("ROE NY %").DefaultCellStyle.Format = "#0.#\%"
        '                DQualite.Columns("Coût/Revenu %").DefaultCellStyle.Format = "#0.#\%"
        '                DQualite.Columns("EBIT Margin Momentum").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        '                DQualite.Columns("EBIT Margin Momentum").Width = 80
        '                DQualite.Columns("EBIT Margin Momentum").DefaultCellStyle.Font = New Font(Control.DefaultFont.FontFamily.ToString, 12, FontStyle.Bold)
        '                DQualite.Columns("EBIT Margin / Avg5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                'DQualite.Columns("DPS LY").DefaultCellStyle.Format = "n2"
        '                'DQualite.Columns("DPS CY").DefaultCellStyle.Format = "n2"
        '                'DQualite.Columns("DPS NY").DefaultCellStyle.Format = "n2"
        '                'DQualite.Columns("Payout LY %").DefaultCellStyle.Format = "n0"
        '                'DQualite.Columns("Payout CY %").DefaultCellStyle.Format = "n0"
        '                'DQualite.Columns("Payout NY %").DefaultCellStyle.Format = "n0"

        '                If Not CbSectorFGA.Text.ToLower = "banques" AndAlso Not CbSuperSector.Text.StartsWith("**") Then
        '                    DQualite.Columns("Coût/Revenu %").Visible = False
        '                End If









        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille valorisation
        '                clearGrid(DValorisation)

        '                'Ligne d'agrégation
        '                sqlAgr = ""
        '                sqlAgr = sqlAgr & " DIV_YLD_NTM As 'Dvd Yld %', "
        '                sqlAgr = sqlAgr & " PE_CY As 'PE CY x', "
        '                sqlAgr = sqlAgr & " PE_NY As 'PE NY x', "
        '                sqlAgr = sqlAgr & " PE_ON_AVG5Y As 'PE / Avg5Y x', "
        '                sqlAgr = sqlAgr & " PE_ON_AVG10Y As 'PE / Avg10Y x', "
        '                sqlAgr = sqlAgr & " PE_VS_IND_ON_AVG5Y As 'PE rel / Avg5Y x', "
        '                sqlAgr = sqlAgr & " PB_CY As 'PB CY x', "
        '                sqlAgr = sqlAgr & " PB_NY As 'PB NY x', "
        '                sqlAgr = sqlAgr & " PB_ON_AVG5Y As 'PB / Avg5Y x', "
        '                sqlAgr = sqlAgr & " PB_ON_AVG10Y As 'PB / Avg10Y x', "
        '                sqlAgr = sqlAgr & " PB_VS_IND_ON_AVG5Y As 'PB rel / Avg5Y x', "
        '                sqlAgr = sqlAgr & " EV_SALES_CY As 'EV/Sales CY x',"
        '                sqlAgr = sqlAgr & " EV_SALES_NY As 'EV/Sales NY x',  "
        '                sqlAgr = sqlAgr & " EV_SALES_ON_AVG5Y As 'EV_Sales / Avg5Y x', "
        '                sqlAgr = sqlAgr & " EV_SALES_ON_AVG10Y As 'EV_Sales / Avg10Y x',  "
        '                sqlAgr = sqlAgr & " EV_EBITDA_CY As 'EV/EBITDA CY x',"
        '                sqlAgr = sqlAgr & " EV_EBITDA_NY As 'EV/EBITDA NY x',  "
        '                sqlAgr = sqlAgr & " EV_EBITDA_ON_AVG5Y As 'EV_EBITDA / Avg5Y x', "
        '                sqlAgr = sqlAgr & " EV_EBITDA_ON_AVG10Y As 'EV_EBITDA / Avg10Y x', "
        '                sqlAgr = sqlAgr & " EV_EBIT_CY As 'EV/EBIT CY x',"
        '                sqlAgr = sqlAgr & " EV_EBIT_NY As 'EV/EBIT NY x',  "
        '                sqlAgr = sqlAgr & " EV_EBIT_ON_AVG5Y As 'EV_EBIT / Avg5Y x', "
        '                sqlAgr = sqlAgr & " EV_EBIT_ON_AVG10Y As 'EV_EBIT / Avg10Y x',"
        '                sqlAgr = sqlAgr & " P_TBV_CY As 'P_TBV CY x',"
        '                sqlAgr = sqlAgr & " P_TBV_NY As 'P_TBV NY x',"
        '                sqlAgr = sqlAgr & " P_TBV_ON_AVG5Y As 'P_TBV / Avg5Y x',"
        '                sqlAgr = sqlAgr & " P_TBV_NTM_AVG5Y As 'P_TBV_NTM / Avg5Y x',"
        '                sqlAgr = sqlAgr & " MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                sqlFact = ""
        '                sqlFact = sqlFact & " DIV_YLD_NTM As 'Dvd Yld %', "
        '                sqlFact = sqlFact & " PE_CY As 'PE CY x', "
        '                sqlFact = sqlFact & " PE_NY As 'PE NY x', "
        '                sqlFact = sqlFact & " PE_ON_AVG5Y As 'PE / Avg5Y x', "
        '                sqlFact = sqlFact & " PE_ON_AVG10Y As 'PE / Avg10Y x', "
        '                sqlFact = sqlFact & " PE_VS_IND_ON_AVG5Y As 'PE rel / Avg5Y x', "
        '                sqlFact = sqlFact & " PB_CY As 'PB CY x', "
        '                sqlFact = sqlFact & " PB_NY As 'PB NY x', "
        '                sqlFact = sqlFact & " PB_ON_AVG5Y As 'PB / Avg5Y x', "
        '                sqlFact = sqlFact & " PB_ON_AVG10Y As 'PB / Avg10Y x', "
        '                sqlFact = sqlFact & " PB_VS_IND_ON_AVG5Y As 'PB rel / Avg5Y x', "
        '                sqlFact = sqlFact & " EV_SALES_CY As 'EV/Sales CY x',"
        '                sqlFact = sqlFact & " EV_SALES_NY As 'EV/Sales NY x',  "
        '                sqlFact = sqlFact & " EV_SALES_ON_AVG5Y As 'EV_Sales / Avg5Y x', "
        '                sqlFact = sqlFact & " EV_SALES_ON_AVG10Y As 'EV_Sales / Avg10Y x',  "
        '                sqlFact = sqlFact & " EV_EBITDA_CY As 'EV/EBITDA CY x',"
        '                sqlFact = sqlFact & " EV_EBITDA_NY As 'EV/EBITDA NY x',  "
        '                sqlFact = sqlFact & " EV_EBITDA_ON_AVG5Y As 'EV_EBITDA / Avg5Y x', "
        '                sqlFact = sqlFact & " EV_EBITDA_ON_AVG10Y As 'EV_EBITDA / Avg10Y x', "
        '                sqlFact = sqlFact & " EV_EBIT_CY As 'EV/EBIT CY x',"
        '                sqlFact = sqlFact & " EV_EBIT_NY As 'EV/EBIT NY x',  "
        '                sqlFact = sqlFact & " EV_EBIT_ON_AVG5Y As 'EV_EBIT / Avg5Y x', "
        '                sqlFact = sqlFact & " EV_EBIT_ON_AVG10Y As 'EV_EBIT / Avg10Y x',"
        '                sqlFact = sqlFact & " P_TBV_CY As 'P_TBV CY x',"
        '                sqlFact = sqlFact & " P_TBV_NY As 'P_TBV NY x',"
        '                sqlFact = sqlFact & " NULL As 'P_TBV / Avg5Y x',"
        '                sqlFact = sqlFact & " NULL As 'P_TBV_NTM / Avg5Y x',"
        '                sqlFact = sqlFact & " MARKET_CAP_EUR As 'Mkt Cap m€'"

        '                da.RowHeaderCell(DValorisation)
        '                DValorisation.DataSource = co.LoadDataGridByString(sql(sqlAgr, sqlFact, mon_indice, mon_supersector, mon_sector_fga))

        '                'Format de toutes les colonnes
        '                'da.AutoFiltre(DValorisation, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5}))
        '                DValorisation.Columns("FGA Sector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DValorisation.Columns("Company").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                DValorisation.Columns("ICB SuperSector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                'Valorisation
        '                DValorisation.Columns("Dvd Yld %").DefaultCellStyle.Format = "#0.#\%"
        '                DValorisation.Columns("PE CY x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PE NY x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PE / AVG5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PE / AVG10Y x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PE rel / Avg5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PB CY x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PB NY x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PB / AVG5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PB / AVG10Y x").DefaultCellStyle.Format = "#0.#\x"
        '                DValorisation.Columns("PB rel / Avg5Y x").DefaultCellStyle.Format = "#0.#\x"

        '                'DValorisation.Columns.Remove("PE NTM AVG5Y x")
        '                'DValorisation.Columns.Remove("PB NTM AVG5Y x")
        '                'DValorisation.Columns.Remove("EV_SALES NTM AVG5Y x")
        '                'DValorisation.Columns.Remove("EV_EBITDA NTM AVG5Y x")
        '                'DValorisation.Columns.Remove("EV_EBIT NTM AVG5Y x")


        '                ''test suivant les secteurs
        '                'If CbSuperSector.Text = "Banques" Then
        '                '    DGenerale.Columns.Remove("EBIT Margin CY %")
        '                '    DGenerale.Columns.Remove("EBIT Margin NY %")
        '                '    DGenerale.Columns.Remove("EBIT Margin LY %")
        '                '    DGenerale.Columns.Remove("EV/EBITDA CY x")
        '                '    DGenerale.Columns.Remove("EV/EBITDA NY x")
        '                'ElseIf CbSuperSector.Text <> "Banques" Then
        '                '    DGenerale.Columns.Remove("Country")
        '                '    DMomentum.Columns.Remove("Country")
        '                '    DCroissance.Columns.Remove("Country")
        '                '    DQualite.Columns.Remove("Country")
        '                '    DValorisation.Columns.Remove("Country")
        '                'End If

        '                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '                'Construction grille Valeur/analyse
        '                ValeurAnalyseBlend()

        '                Windows.Forms.Cursor.Current = Cursors.Default
        '            Else
        '                MessageBox.Show("La table ACT_DATA_FACTSET est vide au " & CbDateGeneral.Text & " !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
        '            End If

        '        End Sub

        '        Public Sub dataBinding(ByVal dataGrid As DataGridView)
        '            If (Not dataGrid.Columns("ICB Industry") Is Nothing) Then

        '                If dataGrid.Name = "DGenerale" Then
        '                    'da.AutoFiltre(DGenerale, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5, 6, 15}))
        '                Else
        '                    da.AutoFiltre(dataGrid, New List(Of Integer)(New Integer() {0, 1, 2, 3, 4, 5, 6}))
        '                End If

        '                dataGrid.Columns("Mkt Cap m€").DefaultCellStyle.Format = "n0"
        '                dataGrid.Columns.Remove("ICB Industry")

        '                If dataGrid.Name <> "DGenerale" Then
        '                    dataGrid.Columns.Remove("Mkt Cap m€")
        '                End If



        '                'test suivant les secteurs
        '                'secteurFGA banques
        '                If CbSectorFGA.Text <> "Banques" Then
        '                    dataGrid.Columns.Remove("Country")
        '                    dataGrid.Columns("Company").Frozen = True
        '                    'If dataGrid.Name = "DGenerale" Then
        '                    '    DGenerale.Columns.Remove("Tier1 CY %")
        '                    '    DGenerale.Columns.Remove("Tier1 NY %")
        '                    'ElseIf dataGrid.Name = "DQualite" Then
        '                    '    DQualite.Columns("EBIT Margin LY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("EBIT Margin CY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("EBIT Margin NY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("EBIT Margin CY/LY (bp)").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("EBIT Margin NY/CY (bp)").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns.Remove("PBT/RWA LY %")
        '                    '    DQualite.Columns.Remove("PBT/RWA CY %")
        '                    '    DQualite.Columns.Remove("PBT/RWA NY %")
        '                    '    DQualite.Columns.Remove("PBT/RWA CY/LY")
        '                    '    DQualite.Columns.Remove("PBT/RWA NY/CY")
        '                    '    DQualite.Columns.Remove("PBT/SALES LY %")
        '                    '    DQualite.Columns.Remove("PBT/SALES CY %")
        '                    '    DQualite.Columns.Remove("PBT/SALES NY %")
        '                    '    DQualite.Columns.Remove("PBT/SALES CY/LY (bp)")
        '                    '    DQualite.Columns.Remove("PBT/SALES NY/CY (bp)")
        '                    '    DQualite.Columns.Remove("ROTE LY %")
        '                    '    DQualite.Columns.Remove("ROTE CY %")
        '                    '    DQualite.Columns.Remove("ROTE NY %")
        '                    '    DQualite.Columns.Remove("RORWA LY %")
        '                    '    DQualite.Columns.Remove("RORWA CY %")
        '                    '    DQualite.Columns.Remove("RORWA NY %")

        '                    '    DQualite.Columns("CAPEX LY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("CAPEX CY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("CAPEX NY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("Capex/Sales LY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Capex/Sales CY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Capex/Sales NY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("FCF LY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("FCF CY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("FCF NY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("FCF Yld LY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("FCF Yld CY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("FCF Yld NY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Net Debt LY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("Net Debt CY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("Net Debt NY").DefaultCellStyle.Format = "n0"
        '                    '    DQualite.Columns("Gearing LY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Gearing CY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Gearing NY %").DefaultCellStyle.Format = "#0.#\%"
        '                    '    DQualite.Columns("Net Debt/EBITDA LY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DQualite.Columns("Net Debt/EBITDA CY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DQualite.Columns("Net Debt/EBITDA NY x").DefaultCellStyle.Format = "#0.#\x"
        '                    'ElseIf dataGrid.Name = "DCroissance" Then
        '                    '    DCroissance.Columns.Remove("PBT CY/LY %")
        '                    '    DCroissance.Columns.Remove("PBT NY/CY %")
        '                    '    DCroissance.Columns.Remove("PBT LY")
        '                    '    DCroissance.Columns.Remove("PBT CY")
        '                    '    DCroissance.Columns.Remove("PBT NY")
        '                    '    DCroissance.Columns("EBIT LY").DefaultCellStyle.Format = "n0"
        '                    '    DCroissance.Columns("EBIT CY").DefaultCellStyle.Format = "n0"
        '                    '    DCroissance.Columns("EBIT NY").DefaultCellStyle.Format = "n0"
        '                    '    DCroissance.Columns("EBIT CY/LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DCroissance.Columns("EBIT NY/CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    'ElseIf dataGrid.Name = "DValorisation" Then
        '                    '    DValorisation.Columns("EV/SALES CY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV/SALES NY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_SALES / AVG5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_SALES / AVG10Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV/EBITDA CY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV/EBITDA NY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_EBITDA / AVG5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_EBITDA / AVG10Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV/EBIT CY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV/EBIT NY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_EBIT / AVG5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("EV_EBIT / AVG10Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns.Remove("P_TBV CY x")
        '                    '    DValorisation.Columns.Remove("P_TBV NY x")
        '                    '    DValorisation.Columns.Remove("P_TBV / Avg5Y x")
        '                    '    DValorisation.Columns.Remove("P_TBV_NTM / Avg5Y x")
        '                    'End If
        '                Else 'CbSectorFGA.Text = "Banques"
        '                    dataGrid.Columns("Country").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '                    dataGrid.Columns("Country").Frozen = True
        '                    'If dataGrid.Name = "DGenerale" Then
        '                    '    DGenerale.Columns("Tier1 CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DGenerale.Columns("Tier1 NY %").DefaultCellStyle.Format = "#0.##\%"
        '                    'ElseIf dataGrid.Name = "DQualite" Then
        '                    '    DQualite.Columns.Remove("EBIT Margin LY %")
        '                    '    DQualite.Columns.Remove("EBIT Margin CY %")
        '                    '    DQualite.Columns.Remove("EBIT Margin NY %")
        '                    '    DQualite.Columns.Remove("EBIT Margin CY/LY (bp)")
        '                    '    DQualite.Columns.Remove("EBIT Margin NY/CY (bp)")
        '                    '    DQualite.Columns.Remove("Net Debt/EBITDA LY x")
        '                    '    DQualite.Columns.Remove("Net Debt/EBITDA CY x")
        '                    '    DQualite.Columns.Remove("Net Debt/EBITDA NY x")
        '                    '    DQualite.Columns.Remove("Gearing LY %")
        '                    '    DQualite.Columns.Remove("Gearing CY %")
        '                    '    DQualite.Columns.Remove("Gearing NY %")
        '                    '    DQualite.Columns.Remove("Net Debt LY")
        '                    '    DQualite.Columns.Remove("Net Debt CY")
        '                    '    DQualite.Columns.Remove("Net Debt NY")
        '                    '    DQualite.Columns.Remove("Capex/Sales LY %")
        '                    '    DQualite.Columns.Remove("Capex/Sales CY %")
        '                    '    DQualite.Columns.Remove("Capex/Sales NY %")
        '                    '    DQualite.Columns.Remove("FCF Yld LY %")
        '                    '    DQualite.Columns.Remove("FCF Yld CY %")
        '                    '    DQualite.Columns.Remove("FCF Yld NY %")
        '                    '    DQualite.Columns.Remove("Capex LY")
        '                    '    DQualite.Columns.Remove("Capex CY")
        '                    '    DQualite.Columns.Remove("Capex NY")
        '                    '    DQualite.Columns.Remove("FCF LY")
        '                    '    DQualite.Columns.Remove("FCF CY")
        '                    '    DQualite.Columns.Remove("FCF NY")
        '                    '    DQualite.Columns("PBT/RWA LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/RWA CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/RWA NY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/RWA CY/LY").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/RWA NY/CY").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/SALES LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/SALES CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/SALES NY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("PBT/SALES CY/LY (bp)").DefaultCellStyle.Format = "n2"
        '                    '    DQualite.Columns("PBT/SALES NY/CY (bp)").DefaultCellStyle.Format = "n2"
        '                    '    DQualite.Columns("ROTE LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("ROTE CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("ROTE NY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("RORWA LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("RORWA CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DQualite.Columns("RORWA NY %").DefaultCellStyle.Format = "#0.##\%"
        '                    'ElseIf dataGrid.Name = "DValorisation" Then
        '                    '    DValorisation.Columns.Remove("EV/SALES CY x")
        '                    '    DValorisation.Columns.Remove("EV/SALES NY x")
        '                    '    DValorisation.Columns.Remove("EV_SALES / AVG5Y x")
        '                    '    DValorisation.Columns.Remove("EV_SALES / AVG10Y x")
        '                    '    DValorisation.Columns.Remove("EV/EBITDA CY x")
        '                    '    DValorisation.Columns.Remove("EV/EBITDA NY x")
        '                    '    DValorisation.Columns.Remove("EV_EBITDA / AVG5Y x")
        '                    '    DValorisation.Columns.Remove("EV_EBITDA / AVG10Y x")
        '                    '    DValorisation.Columns.Remove("EV/EBIT CY x")
        '                    '    DValorisation.Columns.Remove("EV/EBIT NY x")
        '                    '    DValorisation.Columns.Remove("EV_EBIT / AVG5Y x")
        '                    '    DValorisation.Columns.Remove("EV_EBIT / AVG10Y x")
        '                    '    DValorisation.Columns("P_TBV / Avg5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("P_TBV_NTM / Avg5Y x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("P_TBV CY x").DefaultCellStyle.Format = "#0.#\x"
        '                    '    DValorisation.Columns("P_TBV NY x").DefaultCellStyle.Format = "#0.#\x"
        '                    'ElseIf dataGrid.Name = "DCroissance" Then
        '                    '    DCroissance.Columns.Remove("EBIT CY/LY %")
        '                    '    DCroissance.Columns.Remove("EBIT NY/CY %")
        '                    '    DCroissance.Columns.Remove("EBIT LY")
        '                    '    DCroissance.Columns.Remove("EBIT CY")
        '                    '    DCroissance.Columns.Remove("EBIT NY")
        '                    '    DCroissance.Columns("PBT CY/LY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DCroissance.Columns("PBT NY/CY %").DefaultCellStyle.Format = "#0.##\%"
        '                    '    DCroissance.Columns("PBT LY").DefaultCellStyle.Format = "n0"
        '                    '    DCroissance.Columns("PBT CY").DefaultCellStyle.Format = "n0"
        '                    '    DCroissance.Columns("PBT NY").DefaultCellStyle.Format = "n0"
        '                    'End If
        '                End If

        '                'secteurFGA compagnies aériennes
        '                'If CbSectorFGA.Text <> "Compagnies aériennes" Then
        '                '    If dataGrid.Name = "DGenerale" Then
        '                '        DGenerale.Columns.Remove("Load Factor LY %")
        '                '        DGenerale.Columns.Remove("Load Factor CY %")
        '                '        DGenerale.Columns.Remove("Revenue Passenger LY")
        '                '        DGenerale.Columns.Remove("Revenue Passenger CY")
        '                '    End If
        '                'Else 'CbSectorFGA.Text = "Compagnies aériennes"
        '                '    DGenerale.Columns("Load Factor LY %").DefaultCellStyle.Format = "#0.##\%"
        '                '    DGenerale.Columns("Load Factor CY %").DefaultCellStyle.Format = "#0.##\%"
        '                '    DGenerale.Columns("Revenue Passenger LY").DefaultCellStyle.Format = "n0"
        '                '    DGenerale.Columns("Revenue Passenger CY").DefaultCellStyle.Format = "n0"
        '                'End If

        '                'secteurFGA exploration & production
        '                'If CbSectorFGA.Text <> "Exploration & Production" Then
        '                '    If dataGrid.Name = "DGenerale" Then
        '                '        DGenerale.Columns.Remove("Prod per day CY/LY %")
        '                '        DGenerale.Columns.Remove("Prod per day NY/CY %")
        '                '    End If
        '                'Else 'CbSectorFGA.Text = "Exploration & Production"
        '                '    DGenerale.Columns("Prod per day CY/LY %").DefaultCellStyle.Format = "#0.##\%"
        '                '    DGenerale.Columns("Prod per day NY/CY %").DefaultCellStyle.Format = "#0.##\%"
        '                'End If
        '            End If

        '            Coloriage(dataGrid)
        '            selectRow(dataGrid)
        '        End Sub


        '        Private Sub DataGridView_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DGenerale.DataBindingComplete, DCroissance.DataBindingComplete, DQualite.DataBindingComplete, DValorisation.DataBindingComplete, DMomentum.DataBindingComplete, DValeursBlend.DataBindingComplete
        '            Try
        '                dataBinding(sender)
        '            Catch ex As Exception

        '            End Try
        '        End Sub

        '        Private Sub DataGridView_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DGenerale.Sorted, DCroissance.Sorted, DQualite.Sorted, DValorisation.Sorted, DMomentum.Sorted
        '            Try
        '                Coloriage(sender)
        '            Catch ex As Exception

        '            End Try
        '        End Sub

        '        ''' <summary>
        '        ''' Colorie les agrégations du plus foncé au moins foncé
        '        ''' </summary>
        '        Public Sub Coloriage(ByVal datagrid As DataGridView)
        '            Dim mon_indice As String = ""
        '            Select Case CbIndiceGeneral.Text
        '                Case "Stoxx 600"
        '                    mon_indice = "SXXP"
        '                Case "Euro zone"
        '                    mon_indice = "SXXE"
        '                Case "Ex euro"
        '                    mon_indice = "SXXA"
        '            End Select


        '            If datagrid.RowCount > 0 Then
        '                'DAgregation.Columns(1).Frozen = True
        '                For i = 0 To datagrid.Rows.Count - 1

        '                    If datagrid.Item(datagrid.Columns("Company").Index, i).Value.contains("* ") Then
        '                        'ligne indice
        '                        If IsDBNull(datagrid.Item(datagrid.Columns("FGA Sector").Index, i).Value) And IsDBNull(datagrid.Item(datagrid.Columns("ICB SuperSector").Index, i).Value) Then
        '                            'If DAgregation.Item(DAgregation.Columns("FGA Sector").Index, i).Value = Nothing Then 'And DAgregation.Rows(i).Cells(0).Value.ToString.Contains("**") Then
        '                            datagrid.Rows(i).DefaultCellStyle.BackColor = Color.Peru
        '                            datagrid.Rows(i).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Bold)
        '                            'End If
        '                        End If
        '                        'ligne supersector
        '                        If IsDBNull(datagrid.Item(datagrid.Columns("FGA Sector").Index, i).Value) And IsDBNull(datagrid.Item(datagrid.Columns("ICB SuperSector").Index, i).Value) = False Then
        '                            'If DAgregation.Item(DAgregation.Columns("FGA Sector").Index, i).Value = Nothing Then 'And DAgregation.Rows(i).Cells(0).Value.ToString.Contains("**") Then
        '                            datagrid.Rows(i).DefaultCellStyle.BackColor = Color.Tan
        '                            datagrid.Rows(i).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Bold)
        '                            'End If
        '                        End If
        '                        'ligne fga sector
        '                        If IsDBNull(datagrid.Item(datagrid.Columns("FGA Sector").Index, i).Value) = False And IsDBNull(datagrid.Item(datagrid.Columns("ICB SuperSector").Index, i).Value) = False Then
        '                            'If DAgregation.Item(DAgregation.Columns("FGA Sector").Index, i).Value = Nothing Then 'And DAgregation.Rows(i).Cells(0).Value.ToString.Contains("**") Then
        '                            datagrid.Rows(i).DefaultCellStyle.BackColor = Color.PaleGoldenrod
        '                            datagrid.Rows(i).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Bold)
        '                            'End If
        '                        End If

        '                    ElseIf Not IsDBNull(datagrid.Item(datagrid.Columns("Crncy").Index, i).Value) Then
        '                        'ligne currency

        '                        If datagrid.Item(datagrid.Columns("Crncy").Index, i).Value = "USD" Then
        '                            datagrid.Rows(i).DefaultCellStyle.BackColor = Color.LightGray
        '                        End If
        '                    End If
        '                Next
        '            End If
        '        End Sub



        '        ''' <summary>
        '        ''' Construit la réquête sql pour les aggrégation d'indice
        '        ''' </summary>
        '        Public Function sqlAgregation(ByVal col As String, ByVal mon_indice As String, ByVal mon_supersector As String, ByVal mon_sector_fga As String) As String
        '            Dim sql As String

        '            sql = "SELECT NULL As 'Ticker', " & " '* " & mon_indice & " *'  As 'Company',	NULL As Country, si.libelle As 'ICB Industry', ss.libelle As 'ICB SuperSector', s.libelle As 'FGA Sector', NULL As 'Crncy',"
        '            sql = sql & col
        '            sql = sql & " FROM ACT_DATA_FACTSET_AGR f "
        '            sql = sql & " LEFT OUTER JOIN ACT_FGA_SECTOR s ON f.FGA_SECTOR = s.id"
        '            sql = sql & " LEFT OUTER JOIN ACT_SUPERSECTOR ss ON f.ICB_SUPERSECTOR = ss.id"
        '            sql = sql & " LEFT OUTER JOIN ACT_INDUSTRY si ON f.ICB_INDUSTRY = si.id"
        '            sql = sql & " WHERE "
        '            'Cas agrégation 1 indice
        '            sql = sql & " (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR IS NULL and f.FGA_SECTOR IS NULL  AND f.date='" & CbDateGeneral.Text & "')"
        '            'Cas agrégation super_sector
        '            If CbSectorFGA.Text <> "**isin**" Then
        '                If CbSuperSector.Text = "**SuperSectors**" And CbSectorFGA.Text = "" Then
        '                    'Cas agrégation tous les super_sectors
        '                    sql = sql & "  OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR IS NOT NULL and f.FGA_SECTOR IS NULL AND f.date='" & CbDateGeneral.Text & "') "
        '                ElseIf CbSuperSector.Text = "**SuperSectors**" And CbSectorFGA.Text = "**Sectors**" Then
        '                    sql = sql & "  OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR IS NOT NULL and f.FGA_SECTOR IS NOT NULL AND f.date='" & CbDateGeneral.Text & "')   OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR IS NOT NULL and f.FGA_SECTOR IS NULL AND f.date='" & CbDateGeneral.Text & "') "
        '                ElseIf CbSuperSector.Text <> "**SuperSectors**" And CbSectorFGA.Text = "" Then
        '                    'Cas agrégation 1 super_sector
        '                    sql = sql & "  OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR='" & mon_supersector & "'  AND f.date='" & CbDateGeneral.Text & "') "
        '                ElseIf CbSuperSector.Text <> "**SuperSectors**" And CbSectorFGA.Text <> "" And CbSectorFGA.Text <> "**Sectors**" Then
        '                    'Cas agrégation 1 super_sector + 1 sector
        '                    sql = sql & "  OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR='" & mon_supersector & "' and f.FGA_SECTOR='" & mon_sector_fga & "'  AND f.date='" & CbDateGeneral.Text & "' )  OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR='" & mon_supersector & "' and f.FGA_SECTOR IS NULL  AND f.date='" & CbDateGeneral.Text & "' ) "
        '                ElseIf CbSuperSector.Text = "" And CbSectorFGA.Text = "**Sectors**" Then
        '                    'Cas agrégation tous les sectors
        '                    sql = sql & " OR (f.INDICE= '" & mon_indice & "' and f.ICB_SUPERSECTOR IS NOT NULL and f.FGA_SECTOR IS NOT NULL AND f.date='" & CbDateGeneral.Text & "') "
        '                End If
        '            End If
        '            If CbSuperSector.Text = "**SuperSectors**" Or CbSectorFGA.Text = "**Sectors**" Then
        '                sql = sql & " ORDER BY 'ICB SuperSector', 'FGA Sector','Mkt Cap m€' DESC "
        '            End If

        '            Return sql

        '        End Function

        '        ''' <summary>
        '        ''' Construit la réquête sql pour les isins factset
        '        ''' </summary>
        '        Public Function sqlFactset(ByVal col As String, ByVal mon_indice As String, ByVal mon_supersector As String, ByVal mon_sector_fga As String) As String
        '            Dim sql As String

        '            sql = ""

        '            'supersector + sector
        '            If (CbSuperSector.Text <> "**SuperSectors**" And CbSectorFGA.Text <> "**Sectors**") Then

        '                sql = sql & " UNION SELECT v.TICKER_BLOOMBERG  As 'Ticker', company_name  As 'Company',	c.french As Country, si.libelle As 'ICB Industry', ss.libelle As 'ICB SuperSector', s.libelle As 'FGA Sector', f.currency As 'Crncy',"
        '                sql = sql & col
        '                sql = sql & " FROM ACT_DATA_FACTSET f "
        '                sql = sql & " LEFT OUTER JOIN	ACT_FGA_SECTOR s ON f.FGA_SECTOR = s.id"
        '                sql = sql & " LEFT OUTER JOIN ACT_SUPERSECTOR ss ON f.ICB_SUPERSECTOR = ss.id"
        '                sql = sql & " LEFT OUTER JOIN ACT_INDUSTRY si ON f.ICB_INDUSTRY = si.id"
        '                sql = sql & " LEFT OUTER JOIN ACT_VALEUR v ON f.isin = v.isin"
        '                sql = sql & " LEFT OUTER JOIN ISR_NOTE i on i.isin=f.isin and i.date= (SELECT TOP 1 date FROM ISR_NOTE where date <= '" & CbDateGeneral.Text & "') "
        '                sql = sql & " LEFT OUTER JOIN [ref].COUNTRY c on c.iso2=f.country"
        '                sql = sql & " WHERE "

        '                If CbSuperSector.Text <> "**Isin**" And CbSuperSector.Text <> "**SuperSectors**" Then
        '                    'isin d'1 supersector
        '                    If CbSectorFGA.Text = "" Then
        '                        sql = sql & " (f." & mon_indice & " IS NOT NULL and f.ICB_SUPERSECTOR= '" & mon_supersector & "' and f.date='" & CbDateGeneral.Text & "') OR  ( f.ICB_SUPERSECTOR= '" & mon_supersector & "' and f.SXXP IS NULL AND f.SXXA IS NULL AND f.SXXE IS NULL and f.date='" & CbDateGeneral.Text & "') "
        '                    ElseIf CbSectorFGA.Text <> "" And CbSectorFGA.Text <> "**Sectors**" Then
        '                        sql = sql & " (f." & mon_indice & " IS NOT NULL and f.ICB_SUPERSECTOR= '" & mon_supersector & "' and f.FGA_SECTOR='" & mon_sector_fga & "' and  f.date='" & CbDateGeneral.Text & "') OR (f.SXXP IS NULL AND f.SXXA IS NULL AND f.SXXE IS NULL and f.ICB_SUPERSECTOR= '" & mon_supersector & "' and f.FGA_SECTOR='" & mon_sector_fga & "' and f.date='" & CbDateGeneral.Text & "') "
        '                    End If
        '                ElseIf CbSuperSector.Text = "**Isin**" And CbSectorFGA.Text = "" Then
        '                    'isin de l'indice
        '                    sql = sql & " (f." & mon_indice & " IS NOT NULL and f.date='" & CbDateGeneral.Text & "') OR (f.SXXP IS NULL AND f.SXXA IS NULL AND f.SXXE IS NULL and f.date='" & CbDateGeneral.Text & "')"
        '                End If
        '                sql = sql & " ORDER BY 'ICB SuperSector','FGA Sector', 'Mkt Cap m€' DESC "
        '            End If

        '            Return sql

        '        End Function

        '        ''' <summary>
        '        '''  Construit la réquête sql finale
        '        ''' </summary>
        '        Public Function sql(ByVal colAgr As String, ByVal colFactset As String, ByVal mon_indice As String, ByVal mon_supersector As String, ByVal mon_sector_fga As String) As String
        '            Return sqlAgregation(colAgr, mon_indice, mon_supersector, mon_sector_fga) & sqlFactset(colFactset, mon_indice, mon_supersector, mon_sector_fga)
        '        End Function

        '        ''' <summary>
        '        ''' BViderGeneral : Vide la datagrid  DAgregation de l'onglet général
        '        ''' </summary>
        '        Private Sub BViderGeneral_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '            'clearGrid(DGenerale)
        '        End Sub

        '        ' ''' <summary>
        '        ' ''' BExcelAggr : Exporte les grilles Factset vers Excel
        '        ' ''' </summary>
        '        'Private Sub BExcelAggr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelAggr.Click
        '        '    da.DataGridsToNewExcel(New List(Of DataGridView)({DGenerale, DCroissance, DQualite, DValorisation, DMomentum}),
        '        '                           New List(Of String)({"Général", "Croissance", "Rentabilité", "Valorisation", "Momentum"}),
        '        '                           New List(Of Integer)({1, 1, 1, 1, 1}), CbDateGeneral.Text)
        '        'End Sub

        '        ' ''' BPrint : Exporte les grilles Factset vers Excel
        '        ' ''' </summary>
        '        'Private Sub BPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelAggr.Click
        '        '    da.DataGridsToNewExcel(New List(Of DataGridView)({DGenerale, DCroissance, DQualite, DValorisation, DMomentum}),
        '        '                           New List(Of String)({"Général", "Croissance", "Rentabilité", "Valorisation", "Momentum"}),
        '        '                           New List(Of Integer)({1, 1, 1, 1, 1}), CbDateGeneral.Text)
        '        'End Sub

        '        ''' <summary>
        '        ''' Click sur le menu NewScreen : Ouvre la New Screen
        '        ''' </summary>
        '        Private Sub NewScreenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewScreenToolStripMenuItem.Click
        '            Dim window As New BaseActionConsultation()
        '            window.Show()
        '        End Sub

        '        ''' <summary>
        '        ''' Click sur le menu Isin : cherche les distinct isin dans la base ACT_DATA_FACTSET
        '        ''' </summary>
        '        Private Sub IsinToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IsinToolStripMenuItem.Click
        '            Dim sql As String = "CREATE TABLE #isin (isin varchar(12), company_name varchar(120))"
        '            sql = sql & " INSERT INTO #isin SELECT DISTINCT isin, NULL FROM ACT_DATA_FACTSET"
        '            sql = sql & " UPDATE #isin SET company_name = a.company_name FROM #isin as i"
        '            sql = sql & " LEFT OUTER JOIN ACT_DATA_FACTSET a ON a.isin = i.isin"
        '            sql = sql & " WHERE i.company_name IS null"
        '            sql = sql & " SELECT isin As 'Isin', company_name AS 'Libellé' FROM #isin ORDER BY company_name"
        '            sql = sql & " DROP TABLE #isin"

        '            co.SqlToNewExcel(New List(Of String)(New String() {sql}))
        '        End Sub

        '        ''' <summary>
        '        ''' Construit un radar de facon inteligente en fonction des colonnes CY, NY, Avg5Y, Avg10Y
        '        ''' </summary>
        '        Private Sub DValorisation_radar(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        '            Dim indicator As List(Of String) = New List(Of String)

        '            If DValorisation.Columns(DValorisation.CurrentCell.ColumnIndex).HeaderText.Contains("CY") Then
        '                indicator.Add("PE_CY As 'PE CY x'")
        '                indicator.Add("PB_CY As 'PB CY x'")
        '                indicator.Add("EV_SALES_CY As 'EV_SALES CY x'")
        '                indicator.Add("EV_EBITDA_CY As 'EV_EBITDA CY x'")
        '                indicator.Add("EV_EBIT_CY As 'EV_EBIT CY x'")
        '                Radar.GraphRadar.Titles(0).Text = "Valorisation " & DateTime.Now.Year
        '            ElseIf DValorisation.Columns(DValorisation.CurrentCell.ColumnIndex).HeaderText.Contains("NY") Then
        '                indicator.Add("PE_NY As 'PE NY x'")
        '                indicator.Add("PB_NY As 'PB NY x'")
        '                indicator.Add("EV_SALES_NY As 'EV_SALES NY x'")
        '                indicator.Add("EV_EBITDA_NY As 'EV_EBITDA NY x'")
        '                indicator.Add("EV_EBIT_NY As 'EV_EBIT NY x'")
        '                Radar.GraphRadar.Titles(0).Text = "Valorisation " & DateTime.Now.Year + 1
        '            ElseIf DValorisation.Columns(DValorisation.CurrentCell.ColumnIndex).HeaderText.Contains("Avg5Y") Then
        '                indicator.Add("PE_ON_AVG5Y As 'PE/Avg5Y x'")
        '                indicator.Add("PB_ON_AVG5Y As 'PB/Avg5Y x'")
        '                indicator.Add("EV_SALES_ON_AVG5Y As 'EV_SALES/Avg5Y x'")
        '                indicator.Add("EV_EBITDA_ON_AVG5Y As 'EV_EBITDA/Avg5Y x'")
        '                indicator.Add("EV_EBIT_ON_AVG5Y As 'EV_EBIT/Avg5Y x'")
        '                Radar.GraphRadar.Titles(0).Text = "Valorisation AVG5Y"
        '            ElseIf DValorisation.Columns(DValorisation.CurrentCell.ColumnIndex).HeaderText.Contains("Avg10Y") Then
        '                indicator.Add("PE_ON_AVG10Y As 'PE/Avg10Y x'")
        '                indicator.Add("PB_ON_AVG10Y As 'PB/Avg10Y x'")
        '                indicator.Add("EV_SALES_ON_AVG10Y As 'EV_SALES/Avg10Y x'")
        '                indicator.Add("EV_EBITDA_ON_AVG10Y As 'EV_EBITDA/Avg10Y x'")
        '                indicator.Add("EV_EBIT_ON_AVG10Y As 'EV_EBIT/Avg10Y x'")
        '                Radar.GraphRadar.Titles(0).Text = "Valorisation AVG10Y"
        '            Else
        '                Exit Sub
        '            End If


        '            Dim mon_indice As String = ""
        '            Select Case CbIndiceGeneral.Text
        '                Case "Stoxx 600"
        '                    mon_indice = "SXXP"
        '                Case "Euro zone"
        '                    mon_indice = "SXXE"
        '                Case "Ex euro"
        '                    mon_indice = "SXXA"
        '            End Select
        '            If String.IsNullOrEmpty(mon_indice) = False Then
        '                Dim indice As DataTable = co.RequeteSqlToDataSetAutomatic("ACT_DATA_FACTSET_AGR", indicator, New List(Of String)(New String() {"Date", "indice", "ICB_SUPERSECTOR ", "FGA_sector "}), New List(Of Object)(New Object() {CbDateGeneral.Text, mon_indice, "IS NULL", "IS NULL"}), "DValorisation").Tables(0)
        '                Radar.GraphRadar.Series("Indice").Points.DataBindXY(nameColumnDataTable(indice), indice.Rows(0).ItemArray)
        '            End If

        '            'radar secteur ICB
        '            Dim mon_sector_icb As String = co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", DValorisation.CurrentRow.Cells(DValorisation.Columns("ICB SuperSector").Index).Value.ToString).FirstOrDefault
        '            If String.IsNullOrEmpty(mon_sector_icb) = False Then
        '                Dim secteur_icb As DataTable = co.RequeteSqlToDataSetAutomatic("ACT_DATA_FACTSET_AGR", indicator, New List(Of String)(New String() {"Date", "indice", "FGA_sector ", "ICB_SUPERSECTOR"}), New List(Of Object)(New Object() {CbDateGeneral.Text, mon_indice, "IS NULL", mon_sector_icb}), "DValorisation").Tables(0)
        '                Radar.GraphRadar.Series("Secteur ICB").Points.DataBindXY(nameColumnDataTable(secteur_icb), secteur_icb.Rows(0).ItemArray)
        '            End If

        '            'radar secteur FGA
        '            Dim mon_sector_fga As String = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", DValorisation.CurrentRow.Cells(DValorisation.Columns("FGA Sector").Index).Value.ToString).FirstOrDefault
        '            If String.IsNullOrEmpty(mon_sector_fga) = False Then
        '                Dim secteur As DataTable = co.RequeteSqlToDataSetAutomatic("ACT_DATA_FACTSET_AGR", indicator, New List(Of String)(New String() {"Date", "indice", "FGA_sector"}), New List(Of Object)(New Object() {CbDateGeneral.Text, mon_indice, mon_sector_fga}), "DValorisation").Tables(0)
        '                Radar.GraphRadar.Series("Secteur FGA").Points.DataBindXY(nameColumnDataTable(secteur), secteur.Rows(0).ItemArray)
        '            End If

        '            'radar valeur
        '            If DValorisation.CurrentRow.Cells(DValorisation.Columns("Company").Index).Value.ToString.Contains("*") = False Then
        '                Dim valeur As DataTable = co.RequeteSqlToDataSetAutomatic("ACT_DATA_FACTSET", indicator, New List(Of String)(New String() {"Date", "company_name"}), New List(Of Object)(New Object() {CbDateGeneral.Text, DValorisation.CurrentRow.Cells(DValorisation.Columns("Company").Index).Value}), "DValorisation").Tables(0)
        '                '.Rows(0).ItemArray()
        '                Radar.GraphRadar.Series("Valeur").Points.DataBindXY(nameColumnDataTable(valeur), valeur.Rows(0).ItemArray)
        '            End If

        '            Radar.Show()

        '        End Sub


#End Region

#Region "Europe - Valeur/Analyse"

        ''' <summary>
        ''' Met à jour l'onglet valeur/analyse
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValeurAnalyseBlend()
            'Réinitialise la grille
            clearGrid(DValeursBlend)
            'clearGrid(DValeursNote)
            Dim FGA_classname As String
            If CbIndiceGeneral.Text = "Europe" Then
                FGA_classname = "FGA_EU"
            ElseIf CbIndiceGeneral.Text = "USA" Then
                FGA_classname = "FGA_US"
            Else
                Return
            End If
            Dim id_fga As Integer =
            co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {FGA_classname, 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault()
            'co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", CbSectorFGA.Text).FirstOrDefault()

            'Vérification de la selection du secteur FGA
            If id_fga = 0 Then
                Return
            End If

            da.RowHeaderCell(DValeursBlend)
            DValeursBlend.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_DataGridBlendValeur",
                                                                                                        New List(Of String)(New String() {"@date", "@id_fga", "@FGA"}),
                                                                                                        New List(Of Object)(New Object() {CbDateGeneral.Text, id_fga, CbIndiceGeneral.Text})))
            If DValeursBlend.ColumnCount > 0 Then
                DValeursBlend.DefaultCellStyle.Format = "n1"
                DValeursBlend.Columns(10).Frozen = True

                DValeursBlend.Columns("Company Name").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                DValeursBlend.Columns("Poids").DefaultCellStyle.Format = "#0.##\%"
                DValeursBlend.Columns("liquidity").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                DValeursBlend.Columns("liquidity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DValeursBlend.Columns("liquidity").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursBlend.Columns("liquidity").Width = 42

                DValeursBlend.Columns("Note").DefaultCellStyle.Format = "n2"
                DValeursBlend.Columns("Quint Quant").DefaultCellStyle.Format = "n0"
                DValeursBlend.Columns("Quint Quant").Visible = False

                ' Separator next Total
                addColumnSeparator(DValeursBlend,
                                   DValeursBlend.Columns.IndexOf(DValeursBlend.Columns("ISR")) + 1,
                                   True)

                ' ISR
                DValeursBlend.Columns("ISR").DefaultCellStyle.BackColor = Color.LightGreen
                DValeursBlend.Columns("ISR").DefaultCellStyle.Format = "n2"
                DValeursBlend.Columns("note ISR").DefaultCellStyle.BackColor = Color.LightGreen
                DValeursBlend.Columns("note ISR").DefaultCellStyle.Format = "n2"
                DValeursBlend.Columns("note ISR").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursBlend.Columns("note ISR").Width = 90

                'Growth
                DValeursBlend.Columns("Growth").DefaultCellStyle.BackColor = Color.LightBlue
                DValeursBlend.Columns("Growth").DefaultCellStyle.Format = "n2"
                setFormatFromRoot(DValeursBlend, "CROISSANCE", Color.LightBlue)

                'Value
                DValeursBlend.Columns("Profit").DefaultCellStyle.BackColor = Color.LightCoral
                DValeursBlend.Columns("Profit").DefaultCellStyle.Format = "n2"
                setFormatFromRoot(DValeursBlend, "QUALITE", Color.LightCoral)

                'Yield = profit
                DValeursBlend.Columns("Value").DefaultCellStyle.BackColor = Color.LightSlateGray
                DValeursBlend.Columns("Value").DefaultCellStyle.Format = "n2"
                setFormatFromRoot(DValeursBlend, "VALORISATION", Color.LightSlateGray)

                'loadValeurNotes(id_fga, CbSectorFGA.Text)

                ' Greyed non Euro values
                'If DValeursBlend.Columns.Contains("Crncy") Then
                '    DValeursBlend.Columns("Crncy").Visible = False

                '    For Each row As DataGridViewRow In DValeursBlend.Rows
                '        If Not IsDBNull(row.Cells("Crncy").Value) Then
                '            'ligne currency
                '            If row.Cells("Crncy").Value = "USD" Then
                '                row.Cells("Ticker").Style.BackColor = Color.LightGray
                '                row.Cells("Company Name").Style.BackColor = Color.LightGray
                '                row.Cells("Poids").Style.BackColor = Color.LightGray
                '            End If
                '        End If
                '    Next
                'End If

                ' Use colour to distinct quintils. 
                Color_quintile(DValeursBlend, "Quint Quant", New List(Of String)({"Note"})) ', "liquidity", "Poids", "Company Name", "Ticker"
            End If
        End Sub

        ''' <summary>
        ''' Clear datagrid content and remove added columns.
        ''' </summary>
        Private Sub clearGrid(ByVal grid As DataGridView)
            grid.DataSource = Nothing
            grid.Columns.Clear()
        End Sub

        ''' <summary>
        ''' Ajoute une colonne vide en guise de séparateur dans grid à l'index pos.
        ''' </summary>
        Private Sub addColumnSeparator(ByVal grid As DataGridView, ByVal pos As Integer, ByVal isfrozen As Boolean)
            Dim empty_col As New DataGridViewColumn(New DataGridViewTextBoxCell())

            empty_col.HeaderText = ""
            empty_col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            empty_col.Width = 5
            empty_col.DefaultCellStyle.BackColor = Color.Black
            empty_col.ReadOnly = True

            grid.Columns.Insert(pos, empty_col)

            If isfrozen Then
                For i = 0 To pos
                    grid.Columns(i).Frozen = True
                Next
            End If
        End Sub

        ''' <summary>
        ''' Color each column in grid according to its root.
        ''' </summary>
        Private Sub setFormatFromRoot(ByVal grid As DataGridView, ByVal rootname As String, ByVal color As Color)
            Dim sql As String = "SELECT COALESCE(crit.description, crit.nom) AS nom" +
                "   , COALESCE(crit.precision, 0) AS precision" +
                "   , COALESCE(crit.format, 0) AS format" +
                " FROM ACT_COEF_CRITERE crit" +
                "   INNER JOIN ACT_COEF_CRITERE parent ON parent.id_critere = crit.id_parent" +
                "   INNER JOIN ACT_COEF_CRITERE root ON root.id_critere = parent.id_parent" +
                " WHERE crit.is_sector = 1" +
                "   AND root.nom = '" + rootname + "'"

            For Each dico In co.sqlToListDico(sql)
                Dim format As String = "#0."
                For index = dico("precision") To 1 Step -1
                    format &= "#"
                Next

                Select Case (dico("format"))
                    Case TableDynamique.ColumnFormat.Facteur
                        format &= "\x"
                    Case TableDynamique.ColumnFormat.Pourcentage
                        format &= "\%"
                    Case Else
                        format = "n" & dico("precision")
                End Select

                If grid.Columns.Contains(dico("nom")) Then
                    For Each row As DataGridViewRow In grid.Rows
                        grid.Columns(dico("nom")).DefaultCellStyle.BackColor = color
                        grid.Columns(dico("nom")).DefaultCellStyle.Format = format
                        grid.Columns(dico("nom")).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                        grid.Columns(dico("nom")).Width = 90
                    Next
                End If

            Next
        End Sub


        ''' <summary>
        ''' Color each row's cell according to its quintile.
        ''' </summary>
        ''' <param name="col_quint">The Quintil column's name</param>
        ''' <param name="columns">The column list to color</param>
        Private Sub Color_quintile(ByVal dataGridView As DataGridView, ByVal col_quint As String, ByVal columns As List(Of String))
            For Each row As DataGridViewRow In dataGridView.Rows
                Dim quint As Object = row.Cells(col_quint).Value


                If IsDBNull(quint) OrElse quint Is Nothing OrElse Not Integer.TryParse(quint, Nothing) Then
                    Continue For
                End If


                Select Case quint
                    Case 1
                        For Each col In columns
                            row.Cells(col).Style.BackColor = Color.LimeGreen
                        Next
                    Case 2
                        For Each col In columns
                            row.Cells(col).Style.BackColor = Color.LightGreen
                        Next
                    Case 4
                        For Each col In columns
                            row.Cells(col).Style.BackColor = Color.LightGray
                        Next
                    Case 5
                        For Each col In columns
                            row.Cells(col).Style.BackColor = Color.DimGray
                        Next
                    Case Else

                End Select

            Next
        End Sub

        Private Sub loadValeurNotes(ByVal secteurFGA As String)
            'Dim ids = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", secteurFGA)
            Dim ids = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_EU", 0, secteurFGA}.ToList, , )

            If ids.Count = 0 Then
                ' Invalid FGA Sector.
                Return
            End If

            'loadValeurNotes(ids.First, secteurFGA)

        End Sub

        'Private Sub loadValeurNotes(ByVal id_fga As String, ByVal secteurFGA As String)
        '    Dim colonnes As String = ""
        '    Dim sql As String
        '    Dim colDico = getValeurColonneNotes(secteurFGA)

        '    ' Create tmp table #notes with every notes
        '    sql = "SELECT"
        '    sql = sql & " rec.id_valeur"
        '    For Each dico In colDico
        '        dico("nom") = dico("nom").Replace("'", "''")

        '        sql = sql & ",CASE WHEN col.id_column=" & dico("id") _
        '            & " THEN rec.note ELSE NULL END AS """ & dico("nom") & """"
        '    Next
        '    sql = sql & " INTO #notes"
        '    sql = sql & " FROM ACT_NOTE_RECORD rec"
        '    sql = sql & " INNER JOIN ACT_NOTE_COLUMN col ON col.id_column=rec.id_column"
        '    sql = sql & " INNER JOIN ACT_NOTE_TABLE tab ON tab.id_table=col.id_table"
        '    sql = sql & " INNER JOIN ACT_VALEUR val ON val.id=rec.id_valeur"
        '    sql = sql & " WHERE tab.nom='" & secteurFGA.Replace("'", "''") & "'"
        '    sql = sql & " AND rec.note IS NOT NULL"


        '    ' Group notes on each line
        '    Dim first As Boolean = True
        '    For Each dico In colDico
        '        If first Then
        '            sql = sql & " UPDATE #notes SET"
        '            first = False
        '        Else
        '            sql = sql & " ,"
        '        End If
        '        sql = sql & " """ & dico("nom") & """ = "
        '        sql = sql & " ( SELECT TOP 1 """ & dico("nom") & """"
        '        sql = sql & "   FROM #notes b"
        '        sql = sql & "   WHERE b.id_valeur = #notes.id_valeur"
        '        sql = sql & "   AND b.""" & dico("nom") & """ IS NOT NULL )"
        '    Next


        '    ' Fill DValeursNote
        '    Dim colNote As String = ""
        '    For Each dico In colDico
        '        colNote = colNote & " ,""" & dico("nom") & """"
        '    Next

        '    sql = sql & " SELECT distinct"
        '    ' Prevent doublons in datagrid.
        '    sql = sql & " (SELECT TOP 1 TICKER_BLOOMBERG FROM ACT_VALEUR WHERE ISIN = data.ISIN) As 'Ticker'"
        '    ' Prevent doublons in datagrid.
        '    sql = sql & " ,(SELECT TOP 1 LIBELLE FROM ACT_VALEUR WHERE ISIN = data.ISIN) As 'Company Name'"
        '    sql = sql & " ,data.currency As 'Crncy'"
        '    sql = sql & " ,data.liquidity_test As 'liquidity'"
        '    sql = sql & " ,CASE WHEN val.EXCLUSION = 1 THEN 'X' ELSE NULL END AS exclu"
        '    ' Prevent doublons in datagrid.
        '    sql = sql & " ,(SELECT TOP 1 id FROM ACT_VALEUR WHERE ISIN = data.ISIN) AS 'id_valeur'"
        '    sql = sql & " ,FLOOR(rank_quant / ((	"
        '    sql = sql & "                           SELECT CAST(COUNT(*) + 1 AS float)"
        '    sql = sql & "                           FROM ACT_DATA_FACTSET"
        '    sql = sql & "                           WHERE date=data.date AND fga_sector = data.fga_sector"
        '    sql = sql & "                       ) / 5) + 1"
        '    sql = sql & "   ) As 'Quint Quant'"
        '    sql = sql & " ,data.GARPN_TOTAL_S AS 'Note Quant'"
        '    sql = sql & " ,FLOOR(rank_qual / ((	"
        '    sql = sql & "                           SELECT CAST(COUNT(*) + 1 AS float)"
        '    sql = sql & "                           FROM ACT_DATA_FACTSET"
        '    sql = sql & "                           WHERE date=data.date AND fga_sector = data.fga_sector"
        '    sql = sql & "                       ) / 5) + 1"
        '    sql = sql & "   ) As 'Quint Qual'"
        '    sql = sql & " , data.GARPN_NOTE_S AS 'Note Qual'"
        '    sql = sql & " , CAST(TOTAL_NOTE AS float) AS 'total'"

        '    sql = sql & colNote
        '    sql = sql & " FROM ACT_VALEUR val"
        '    sql = sql & " LEFT OUTER JOIN ACT_DATA_FACTSET data ON data.ISIN = val.ISIN"
        '    sql = sql & " INNER JOIN ("
        '    sql = sql & "               SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_TOTAL_S DESC) AS rank_quant"
        '    sql = sql & "               FROM ACT_DATA_FACTSET f"
        '    sql = sql & "               WHERE date='" & CbDateGeneral.Text & "' AND fga_sector = " & id_fga
        '    sql = sql & "            ) rank_quant ON rank_quant.ISIN = data.ISIN"
        '    sql = sql & " INNER JOIN ("
        '    sql = sql & "               SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_NOTE_S DESC) AS rank_qual"
        '    sql = sql & "               FROM ACT_DATA_FACTSET f"
        '    sql = sql & "               WHERE date='" & CbDateGeneral.Text & "' AND fga_sector = " & id_fga
        '    sql = sql & "            ) rank_qual ON rank_qual.ISIN = data.ISIN"
        '    sql = sql & " LEFT OUTER JOIN #notes note ON note.id_valeur=val.id"
        '    sql = sql & " WHERE val.ID_FGA=" & id_fga
        '    If CbDateGeneral.Text <> "" Then
        '        sql = sql & " AND data.DATE='" & CbDateGeneral.Text & "'"
        '    End If
        '    sql = sql & " ORDER BY 'quint qual', data.GARPN_NOTE_S DESC "

        '    sql = sql & " DROP TABLE #notes"

        '    da.RowHeaderCell(DValeursNote)
        '    clearGrid(DValeursNote)
        '    DValeursNote.DataSource = co.LoadDataGridByString(sql)

        '    ' Add separator next total.
        '    addColumnSeparator(DValeursNote,
        '                       DValeursNote.Columns.IndexOf(DValeursNote.Columns("Total")) + 1,
        '                       True)

        '    DValeursNote.DefaultCellStyle.Format = "n2"
        '    'DValeursNote.Width = 50
        '    DValeursNote.DefaultCellStyle.Font = New Font(Control.DefaultFont.FontFamily.ToString, 12)

        '    DValeursNote.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        '    DValeursNote.Columns("Ticker").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '    DValeursNote.Columns("Ticker").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("Ticker").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        '    DValeursNote.Columns("Ticker").ReadOnly = True
        '    DValeursNote.Columns("Company Name").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        '    DValeursNote.Columns("Company Name").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("Company Name").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        '    DValeursNote.Columns("Company Name").Frozen = True
        '    DValeursNote.Columns("Company Name").ReadOnly = True
        '    DValeursNote.Columns("id_valeur").Visible = False
        '    DValeursNote.Columns("id_valeur").ReadOnly = True
        '    DValeursNote.Columns("liquidity").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("liquidity").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        '    DValeursNote.Columns("liquidity").Width = 42
        '    DValeursNote.Columns("liquidity").ReadOnly = True
        '    DValeursNote.Columns("exclu").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("exclu").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        '    DValeursNote.Columns("exclu").Width = 42
        '    DValeursNote.Columns("exclu").ReadOnly = True
        '    DValeursNote.Columns("Quint Quant").Visible = False
        '    DValeursNote.Columns("Quint Quant").ReadOnly = True
        '    DValeursNote.Columns("Quint Qual").Visible = False
        '    DValeursNote.Columns("Quint Qual").ReadOnly = True
        '    DValeursNote.Columns("Note Quant").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("Note Quant").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        '    DValeursNote.Columns("Note Quant").ReadOnly = True
        '    DValeursNote.Columns("Note Qual").DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '    DValeursNote.Columns("Note Qual").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        '    DValeursNote.Columns("Note Qual").ReadOnly = True
        '    DValeursNote.Columns("total").ReadOnly = True

        '    If DValeursNote.Columns.Contains("Crncy") Then
        '        DValeursNote.Columns("Crncy").Visible = False

        '        For Each row As DataGridViewRow In DValeursNote.Rows
        '            If Not IsDBNull(row.Cells("Crncy").Value) Then
        '                'ligne currency
        '                If row.Cells("Crncy").Value = "USD" Then
        '                    row.Cells("Ticker").Style.BackColor = Color.LightGray
        '                    row.Cells("Company Name").Style.BackColor = Color.LightGray
        '                End If
        '            End If
        '            ' Apply format for company name and ticker
        '            row.Cells("Ticker").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        '            row.Cells("Company Name").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        '        Next
        '    End If

        '    For Each dico In colDico
        '        If Not dico("is_activated") Then
        '            DValeursNote.Columns(dico("nom").ToString).DefaultCellStyle.BackColor = Color.FromArgb(192, 192, 192)
        '        End If

        '        If Not dico("is_note") Then
        '            ' Comment
        '            DValeursNote.Columns(dico("nom").ToString).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        '            DValeursNote.Columns(dico("nom").ToString).DefaultCellStyle.Font = New Font(Control.DefaultFont, FontStyle.Regular)
        '            DValeursNote.Columns(dico("nom").ToString).DefaultCellStyle.ForeColor = Color.Blue
        '        End If
        '    Next

        '    Dim rows As New List(Of DataGridViewRow)
        '    For Each row In DValeursNote.Rows
        '        rows.Add(row)
        '    Next

        '    computeTotalNotes(secteurFGA, rows)

        '    ' Use colour to distinct quintils. 
        '    Color_quintile(DValeursNote, "Quint Quant", New List(Of String)({"Note Quant"}))
        '    Color_quintile(DValeursNote, "Quint Qual", New List(Of String)({"Note Qual"})) ', "liquidity", "Company Name", "Ticker"}))
        'End Sub

        ''' <summary>
        ''' Récupère les colonnes associées à un secteur FGA
        ''' </summary>
        Private Function getValeurColonneNotes(ByVal secteurFGA As String) As List(Of Dictionary(Of String, Object))
            Dim sqlColonnes As String

            ' Get columns id & nom
            sqlColonnes = "SELECT col.id_column AS id, col.nom AS nom, col.is_activated, col.is_note, col.coef, col.position"
            sqlColonnes = sqlColonnes & " FROM ACT_NOTE_COLUMN col"
            sqlColonnes = sqlColonnes & " INNER JOIN ACT_NOTE_TABLE tab ON tab.id_table=col.id_table"
            sqlColonnes = sqlColonnes & " WHERE tab.nom='" & secteurFGA.Replace("'", "''") & "'"
            sqlColonnes = sqlColonnes & " ORDER BY position"

            Return co.sqlToListDico(sqlColonnes)
        End Function

        ''' <summary>
        ''' Calcul les notes totales de datagrid.
        ''' </summary>
        Private Sub computeTotalNotes(ByVal secteurFGA As String,
                                     ByVal rows As List(Of DataGridViewRow))
            Dim listDicoColonnes = getValeurColonneNotes(secteurFGA)
            Dim sum_coef As Double = 0
            Dim columnCoefs As New Dictionary(Of String, Double)
            Dim failColumns As New List(Of String)
            Dim hasChanged As Boolean = False

            ' Get columns with grade
            For Each dico In listDicoColonnes
                If dico("is_activated") And dico("is_note") Then
                    If dico.ContainsKey("coef") Then
                        columnCoefs.Add(dico("nom"), dico("coef"))
                        sum_coef += dico("coef")
                    Else
                        columnCoefs.Add(dico("nom"), 1)
                        sum_coef += 1
                    End If
                End If
            Next

            For Each row As DataGridViewRow In rows
                Dim total As Double? = Nothing
                failColumns.AddRange(computeNoteRowTotal(row, columnCoefs, total))
                total /= sum_coef

                If total IsNot Nothing Then
                    If IsDBNull(row.Cells("total").Value) OrElse row.Cells("total").Value <> total Then
                        row.Cells("total").Value = total
                        hasChanged = True
                    End If
                ElseIf Not IsDBNull(row.Cells("total").Value) Then
                    row.Cells("total").Value = DBNull.Value
                    hasChanged = True
                End If
            Next

            ' Prevent for non valid grads.
            If failColumns.Count > 0 Then
                Dim names As String = ""
                For Each col As String In failColumns
                    names &= vbNewLine & vbTab & "- " & col
                Next

                MessageBox.Show("Certaines notes ne sont pas reconnues dans les colonnes suivantes : " & names)
            End If
        End Sub

        ''' <summary>
        ''' Calcul la note total d'une ligne et retourne les colonnes où la note n'est pas reconnue.
        ''' </summary>
        Private Function computeNoteRowTotal(ByVal row As DataGridViewRow,
                                             ByVal columnCoefs As Dictionary(Of String, Double),
                                             ByRef sum As Double?) As List(Of String)
            Dim total As Double = 0
            Dim notes As New Dictionary(Of String, Double)
            Dim failColumns As New List(Of String)

            ' get grade for each column
            For Each col_coef In columnCoefs
                If Not IsDBNull(row.Cells(col_coef.Key).Value) Then
                    Dim value As String = row.Cells(col_coef.Key).Value

                    If value Is Nothing _
                        OrElse Double.TryParse(value, Nothing) _
                        OrElse value.Count(AddressOf IsNote) = value.Length Then

                        notes.Add(col_coef.Key,
                                getNoteValue(value))
                    Else
                        ' prevent if grade is not a grade
                        row.Cells(col_coef.Key).Style.ForeColor = Color.Red
                        failColumns.Add(col_coef.Key)
                    End If
                End If
            Next

            ' compute total
            If notes.Count > 0 Then
                For Each col In columnCoefs.Keys
                    If notes.ContainsKey(col) Then
                        total += notes(col) * columnCoefs(col)
                    Else
                        total += getNoteValue("") * columnCoefs(col)
                    End If
                Next

                sum = total
            Else
                sum = Nothing
            End If

            Return failColumns
        End Function

        ''' <summary>
        ''' Retourne Vrai si un caractere est reconnu en tant que note qualitative.
        ''' </summary>
        Private Function IsNote(ByVal c As Char) As Boolean
            Return c = "-"c _
                OrElse c = "+"c _
                OrElse c = "="c
        End Function

        ''' <summary>
        ''' Renvoie la valeur d'une note passée sous forme de chaîne.
        ''' </summary>
        Private Function getNoteValue(ByVal value As String) As Double
            Dim res As Double = 5

            If value IsNot Nothing Then
                If Not Double.TryParse(value, res) Then
                    res = 5

                    res += 2.5 * value.Count(AddressOf isPlus)
                    res -= 2.5 * value.Count(AddressOf isMinus)
                End If
            End If

            Return res
        End Function

        Private Sub saveTotalNote(ByVal id_valeur As Integer, ByVal total As Double)
            co.Update("ACT_VALEUR",
                      New List(Of String)({"TOTAL_NOTE"}),
                      New List(Of Object)({total}),
                      "ID",
                      id_valeur)
        End Sub

        Private Sub DatagridNote_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
            Dim grid As DataGridView = TryCast(sender, DataGridView)
            If grid Is Nothing Then
                Return
            End If

            saveNotes(New List(Of DataGridViewCell)({grid.CurrentCell}))
        End Sub

        Private Sub saveNotes(ByVal cells As List(Of DataGridViewCell))
            Dim total_changed As Boolean = False
            Dim rows As New List(Of DataGridViewRow)

            For Each Cell In cells
                If IsSaveNote(Cell) Then
                    total_changed = True
                    If Not rows.Contains(Cell.OwningRow) Then
                        rows.Add(Cell.OwningRow)
                    End If
                End If
            Next

            If total_changed Then
                ' Compute relative grads if changed.
                co.ProcedureStockée("ACT_Update_Note_Valeur",
                                    New List(Of String)({"@Date",
                                                         "@id_fga"}),
                                    New List(Of Object)({CbDateGeneral.SelectedValue,
                                                         co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", CbSectorFGA.Text).FirstOrDefault}))
            End If
        End Sub

        ''' <summary>
        ''' Compute new cell's total note and return true if total changed.
        ''' </summary>
        Private Function IsSaveNote(ByVal cell As DataGridViewCell) As Boolean
            Dim row As DataGridViewRow = cell.OwningRow
            Dim id_column As Integer
            Dim is_note As Boolean
            Dim id_valeur As Integer = row.Cells("id_valeur").Value
            Dim note As String = cell.Value.ToString
            Dim id_records As New List(Of Object)

            id_column = co.SelectWhere("ACT_NOTE_COLUMN", "id_column", "nom",
                                        cell.OwningColumn.Name.Replace("'", "''")).FirstOrDefault()
            is_note = co.SelectWhere("ACT_NOTE_COLUMN", "is_note", "id_column", id_column).FirstOrDefault

            id_records = co.SelectDistinctWheres("ACT_NOTE_RECORD",
                                                    "id_record",
                                                    New List(Of String)({"id_column", "id_valeur"}),
                                                    New List(Of Object)({id_column, id_valeur}))

            If note = "" Then
                If id_records.Count <> 0 Then
                    co.DeleteWhere("ACT_NOTE_RECORD",
                                    "id_record",
                                    id_records.FirstOrDefault)
                Else
                    Return False
                End If
            ElseIf Not is_note OrElse (Double.TryParse(note, 5) _
                                        OrElse co.SelectSimple("ACT_NOTE", "id").Contains(note)) Then
                If id_records.Count = 0 Then
                    co.Insert("ACT_NOTE_RECORD",
                            New List(Of String)({"id_column", "id_valeur", "note"}),
                            New List(Of Object)({id_column, id_valeur, note}))
                Else
                    co.Updates("ACT_NOTE_RECORD",
                                New List(Of String)({"note"}),
                                New List(Of Object)({note}),
                                New List(Of String)({"id_record"}),
                                New List(Of Object)({id_records.FirstOrDefault}))
                End If

                'TODO: remove bonus
                If note.ToLowerInvariant() = "coucou" Then
                    MessageBox.Show("Cette fonctionnalité n'est pas un chat. Merci de ne pas en abuser.")
                End If
            Else
                MessageBox.Show("La note suivante n'est pas reconnue : '" & note & "' !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
                Return False
            End If

            Dim old_total As Double = 5
            Dim total As Double = 5

            If Not IsDBNull(row.Cells("total").Value) Then
                old_total = row.Cells("total").Value
            End If

            computeTotalNotes(CbSectorFGA.Text, New List(Of DataGridViewRow)({row}))

            If Not IsDBNull(row.Cells("total").Value) Then
                total = row.Cells("total").Value
            End If

            If old_total <> total Then
                saveTotalNote(id_valeur, total)
                Return True
            End If

            Return False
        End Function

        Private Sub BExcelBlendValeur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelBlendValeur.Click
            da.DataGridsToNewExcel(New List(Of DataGridView)(New DataGridView() {DValeursBlend}), New List(Of String)(New String() {"Scrore"}), New List(Of Integer)(New Integer() {1, 1}), CbDateGeneral.Text)
        End Sub

#End Region

#Region "Europe - News Flow"
        'Private Sub saveNews(ByVal icb As String, ByVal fga As String, ByVal val As String)
        '    'On fait l'update du bon commentaire 
        '    'caractéristique de la table à charger
        '    Dim table As String = ""
        '    Dim titre As String = ""
        '    Dim donnee As String = ""
        '    If (icb <> "" And fga = "" And val = "") Then
        '        table = "ACT_SUPERSECTOR_NEWS"
        '        titre = "id_secteur"
        '        donnee = co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", icb).FirstOrDefault
        '    ElseIf (icb <> "" And fga <> "" And val = "") Then
        '        table = "ACT_FGA_SECTOR_NEWS"
        '        titre = "id_secteur"
        '        donnee = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", fga).FirstOrDefault
        '    Else
        '        table = "ACT_ISIN_NEWS"
        '        titre = "isin"
        '        donnee = co.SelectDistinctWhere("ACT_DATA_FACTSET", "isin", "company_name", val).FirstOrDefault
        '    End If

        '    If donnee Is Nothing Then
        '        Return
        '    End If

        '    If co.SelectDistinctWhere(table, "libelle", titre, donnee).Count = 0 Then
        '        co.Insert(table, New List(Of String)(New String() {titre, "libelle"}), New List(Of Object)(New Object() {donnee, TCommentaireNews.Rtf}))
        '    Else
        '        co.Update(table, New List(Of String)(New String() {"libelle"}), New List(Of Object)(New Object() {TCommentaireNews.Rtf}), titre, donnee)
        '    End If

        'End Sub

        Private Class CommentParameters
            Public supersecteur As String
            Public secteurFGA As String
            Public entreprise As String
            Public result As String
        End Class

        Public Sub loadComentary()
            'Lance le chargement des commentaires dans un nouveau thread.
            'Dim t As New Threading.Thread(AddressOf computeCommentary)
            't.IsBackground = True
            't.Start()

            'Utilise un BackgroundWorker pour executer le chargement des commentaires dans un autre thread.
            'If BWLoadComments.IsBusy Then
            '    BWLoadComments.CancelAsync()
            'End If

            'Dim params As CommentParameters = New CommentParameters
            'params.supersecteur = CbSuperSector.Text
            'params.secteurFGA = CbSectorFGA.Text
            'params.entreprise = CbValeur.Text
            'Try
            '    BWLoadComments.RunWorkerAsync(params)
            'Catch ex As Exception

            'End Try

            'TCommentaireNews.Rtf = Nothing
            'computeCommentary(CbSuperSector.Text, CbSectorFGA.Text, CbValeur.Text)
        End Sub

        Private Function checkSaveCB() As Boolean
            If commentHasChanged Then
                Dim ret As Integer = askForSave()

                If ret < 0 Then
                    'Cancel
                    Return False
                Else
                    If ret = 0 Then
                        'Save
                        saveAllOldTextBox()
                    End If
                    commentHasChanged = False
                End If
            End If

            Return True
        End Function

        Private Function checkSaveRecoText() As Boolean
            If commentHasChanged Then
                Dim ret As Integer = askForSave()

                If ret < 0 Then
                    'Cancel
                    Return False
                Else
                    If ret = 0 Then
                        'Save
                        saveCommentValeurReco()
                    End If
                    commentHasChanged = False
                End If
            End If

            Return True
        End Function

        'Private Sub computeCommentary(ByVal superSecteur As String, ByVal secteurFGA As String, ByVal valeur As String)
        '    'On charge le bon commentaire = la bonne table 
        '    'caractéristique de la table à charger
        '    Dim table As String = ""
        '    Dim titre As String = ""
        '    Dim donnee As String = ""
        '    If (superSecteur <> "" And secteurFGA = "" And valeur = "") Then
        '        table = "ACT_SUPERSECTOR_NEWS"
        '        titre = "id_secteur"
        '        donnee = co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", superSecteur).FirstOrDefault
        '    ElseIf (superSecteur <> "" And secteurFGA <> "" And valeur = "") Then
        '        table = "ACT_FGA_SECTOR_NEWS"
        '        titre = "id_secteur"
        '        donnee = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", secteurFGA).FirstOrDefault
        '    Else
        '        table = "ACT_ISIN_NEWS"
        '        titre = "isin"
        '        donnee = co.SelectDistinctWhere("ACT_DATA_FACTSET", "isin", "company_name", valeur).FirstOrDefault
        '    End If

        '    If donnee <> Nothing And titre <> Nothing And table <> Nothing Then
        '        Dim rtf As String = co.SelectDistinctWhere(table, "libelle", titre, donnee).FirstOrDefault
        '        TCommentaireNews.Rtf = rtf
        '        commentHasChanged = False
        '        TCommentaireNews.Font = New Font(TCommentaireNews.Font.Name, 10)
        '    End If
        'End Sub

        'Private Class Comment
        '    ''' <summary>
        '    ''' Dictionnaire des associations textbox/pair(nom, ismodified)
        '    ''' </summary>
        '    Private rtfs As New Dictionary(Of vbMaf.Windows.Forms.RichEditBox, KeyValuePair(Of String, Boolean))

        '    ''' <summary>
        '    ''' initialise le dictionnaire
        '    ''' </summary>
        '    ''' <param name="boxes"></param>
        '    ''' <remarks></remarks>
        '    Sub New(ByVal boxes As List(Of KeyValuePair(Of vbMaf.Windows.Forms.RichEditBox, String)))
        '        For Each pair As KeyValuePair(Of vbMaf.Windows.Forms.RichEditBox, String) In boxes
        '            rtfs(pair.Key) = New KeyValuePair(Of String, Boolean)(pair.Value, False)
        '        Next
        '    End Sub

        '    ''' <summary>
        '    ''' Définit la textbox renseignée comme étant modifiée
        '    ''' </summary>
        '    ''' <param name="editBox">nom de la textbox</param>
        '    Sub setContentChange(ByRef editBox As vbMaf.Windows.Forms.RichEditBox)
        '        rtfs(editBox) = New KeyValuePair(Of String, Boolean)(rtfs(editBox).Key, True)
        '    End Sub

        '    ''' <summary>
        '    ''' Retourne la liste des textbox dont le champs RTF a été modifié
        '    ''' </summary>
        '    Function getModified() As List(Of vbMaf.Windows.Forms.RichEditBox)
        '        Dim boxs As New List(Of vbMaf.Windows.Forms.RichEditBox)

        '        For Each editbox As vbMaf.Windows.Forms.RichEditBox In rtfs.Keys
        '            If rtfs(editbox).Value Then
        '                boxs.Add(editbox)
        '            End If
        '        Next

        '        Return boxs
        '    End Function

        '    ''' <summary>
        '    ''' Ouvre une nouvelle fenêtre pour demander confirmation de sauvegarde du commentaire
        '    ''' </summary>
        '    ''' <returns>La valeur de l'action</returns>
        '    ''' <remarks>annuler:-1, Oui:0, Non:1</remarks>
        '    Function askForSave(ByVal editbox As vbMaf.Windows.Forms.RichEditBox) As Integer
        '        Dim popup As New PopupWindowSave

        '        popup.setLocation(rtfs(editbox).Key)
        '        popup.ShowDialog()

        '        Return popup.getResult()
        '    End Function

        '    ''' <summary>
        '    ''' Met à jour toutes les associations textbox/pair(nom, ismodified) dans le dictionnaire
        '    ''' </summary>
        '    Sub refreshAll()
        '        For i As Integer = 0 To rtfs.Count - 1
        '            Dim editbox As vbMaf.Windows.Forms.RichEditBox = rtfs.Keys.ElementAt(i)
        '            refresh(editbox)
        '        Next
        '    End Sub

        '    ''' <summary>
        '    ''' Met à jour l'association textbox/pair(nom, ismodified) pour une valeur donnée
        '    ''' </summary>
        '    ''' <param name="editbox">la textbox de l'association à modifier</param>
        '    Sub refresh(ByVal editbox As vbMaf.Windows.Forms.RichEditBox)
        '        rtfs(editbox) = New KeyValuePair(Of String, Boolean)(rtfs(editbox).Key, False)
        '    End Sub

        'End Class


        ''' <summary>
        ''' Ouvre une nouvelle fenêtre pour demander confirmation de sauvegarde
        ''' </summary>
        ''' <returns>La valeur de l'action</returns>
        ''' <remarks>annuler:-1, Oui:0, Non:1</remarks>
        Function askForSave() As Integer
            Dim popup As New PopupWindowSave

            popup.setLocation()
            popup.ShowDialog()

            Return popup.getResult()
        End Function

        Private Sub BWLoadComments_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWLoadComments.DoWork
            'Récupération des paramètres
            If Not TypeOf e.Argument Is CommentParameters Then
                Return
            End If

            Dim p As CommentParameters = e.Argument

            'On charge le bon commentaire = la bonne table 
            'caractéristique de la table à charger
            Dim table As String = ""
            Dim titre As String = ""
            Dim donnee As String = ""
            If (p.supersecteur <> "" And p.secteurFGA = "" And p.entreprise = "") Then
                table = "ACT_SUPERSECTOR_NEWS"
                titre = "id_secteur"
                donnee = co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", p.supersecteur).FirstOrDefault
            ElseIf (p.supersecteur <> "" And p.secteurFGA <> "" And p.entreprise = "") Then
                table = "ACT_FGA_SECTOR_NEWS"
                titre = "id_secteur"
                donnee = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", p.secteurFGA).FirstOrDefault
            Else
                table = "ACT_ISIN_NEWS"
                titre = "isin"
                donnee = co.SelectDistinctWhere("ACT_DATA_FACTSET", "isin", "company_name", p.entreprise).FirstOrDefault
            End If

            If donnee <> Nothing And titre <> Nothing And table <> Nothing Then
                Try
                    e.Result = co.SelectDistinctWhere(table, "libelle", titre, donnee).FirstOrDefault
                Catch ex As Exception

                End Try


            End If
        End Sub

        Private Sub BWLoadComments_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWLoadComments.RunWorkerCompleted
            TCommentaireNews.Rtf = e.Result
            commentHasChanged = False
            TCommentaireNews.Font = New System.Drawing.Font(TCommentaireNews.Font.Name, 10)
        End Sub
#End Region

#Region "Europe - Recommandations"

        Public Sub FillGridSecteurReco()
            Cursor.Current = Cursors.WaitCursor
            'Tables contenant les recommandations FGA et ICB
            Dim secteurs As String = " Select id_secteur, Max(date) As date into #reco_FGA FROM ACT_RECO_SECTOR WHERE type = 'FGA' GROUP BY id_secteur"
            secteurs = secteurs & " Select id_secteur, Max(date) As date into #reco_ICB FROM ACT_RECO_SECTOR WHERE type = 'ICB' GROUP BY id_secteur"

            'Table liant les secteurs FGA à leurs supersecteurs
            secteurs = secteurs & " Select distinct"
            secteurs = secteurs & " ss.code as  id_supersector, ss.label As libelle_supersector, "
            secteurs = secteurs & " fga.code AS id_fga ,fga.label As libelle_fga"
            secteurs = secteurs & " into #sectors_FGA"
            secteurs = secteurs & " from ref_security.SECTOR ss"
            secteurs = secteurs & " inner join  ref_security.SECTOR s on s.id_parent=ss.id"
            secteurs = secteurs & " inner join ref_security.SECTOR_TRANSCO tr on tr.id_sector1=s.id"
            secteurs = secteurs & " inner join ref_security.SECTOR fga on fga.id=tr.id_sector2"
            secteurs = secteurs & " where fga.class_name='FGA_EU'"
            'secteurs = secteurs & " sup.id_federis As id_supersector, sup.libelle As libelle_supersector, "
            'secteurs = secteurs & " fga.id AS id_fga ,fga.libelle As libelle_fga"
            'secteurs = secteurs & " into #sectors_FGA"
            'secteurs = secteurs & " from ACT_SUPERSECTOR sup"
            'secteurs = secteurs & " RIGHT OUTER JOIN ACT_SECTOR sec on sup.id = sec.id_supersector"
            'secteurs = secteurs & " RIGHT OUTER JOIN ACT_SUBSECTOR sub on sec.id = sub.id_sector"
            'secteurs = secteurs & " RIGHT OUTER JOIN ACT_FGA_SECTOR fga on sub.id_fga_sector = fga.id"

            'Sélection des secteurs FGA et ICB associés à leurs recommandations respectives (ou rien si vide)
            secteurs = secteurs & " Select"
            secteurs = secteurs & " CONVERT(VARCHAR, sFGA.id_supersector) As 'id ICB', sFGA.libelle_supersector As 'Secteur ICB',"
            secteurs = secteurs & " sFGA.id_fga AS 'id FGA', sFGA.libelle_fga As 'Secteur FGA',"
            secteurs = secteurs & " CONVERT(VARCHAR, recFGA.date, 103) As 'Date', reco.recommandation As 'reco'"
            secteurs = secteurs & " FROM #sectors_FGA sFGA"
            secteurs = secteurs & " LEFT OUTER JOIN #reco_FGA recFGA on sFGA.id_fga = recFGA.id_secteur"
            secteurs = secteurs & " LEFT OUTER JOIN ACT_RECO_SECTOR reco on reco.id_secteur = recFGA.id_secteur and reco.type = 'FGA' and reco.date = recFGA.date"
            secteurs = secteurs & " UNION"
            secteurs = secteurs & " SELECT "
            'secteurs = secteurs & " sup.id_federis As 'id ICB', sup.libelle As 'Secteur ICB',"
            'secteurs = secteurs & " NULL AS 'id FGA', NULL As 'Secteur FGA',"
            'secteurs = secteurs & " CONVERT(VARCHAR, recICB.date, 103), reco.recommandation As 'reco'"
            'secteurs = secteurs & " FROM ACT_SUPERSECTOR sup"
            'secteurs = secteurs & " LEFT OUTER JOIN #reco_ICB recICB on sup.id_federis = recICB.id_secteur"
            'secteurs = secteurs & " LEFT OUTER JOIN ACT_RECO_SECTOR reco on reco.id_secteur = recICB.id_secteur and reco.type = 'ICB' and reco.date = recICB.date"
            'secteurs = secteurs & " ORDER BY 'id ICB', 'id FGA'"
            secteurs = secteurs & " sup.code As 'id ICB', sup.label As 'Secteur ICB',"
            secteurs = secteurs & " NULL AS 'id FGA', NULL As 'Secteur FGA',"
            secteurs = secteurs & " CONVERT(VARCHAR, recICB.date, 103), reco.recommandation As 'reco'"
            secteurs = secteurs & " FROM ref_security.SECTOR sup"
            secteurs = secteurs & " LEFT OUTER JOIN #reco_ICB recICB on sup.code = recICB.id_secteur"
            secteurs = secteurs & " LEFT OUTER JOIN ACT_RECO_SECTOR reco on reco.id_secteur = recICB.id_secteur and reco.type = 'ICB' and reco.date = recICB.date"
            secteurs = secteurs & " WHERE sup.class_name='GICS' and sup.level=0"
            secteurs = secteurs & " ORDER BY 'id ICB', 'id FGA'"

            secteurs = secteurs & " drop table #sectors_FGA"
            secteurs = secteurs & " drop table #reco_FGA"
            secteurs = secteurs & " drop table #reco_ICB"

            DSecteursReco.DataSource = co.LoadDataGridByString(secteurs)
            DSecteursReco.DefaultCellStyle.Format = "n0"
            DSecteursReco.Columns("id ICB").AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
            DSecteursReco.Columns("id ICB").Width = 45
            DSecteursReco.Columns("id FGA").AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
            DSecteursReco.Columns("id FGA").Width = 45
            DSecteursReco.Columns("reco").AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
            DSecteursReco.Columns("reco").Width = 35
            DSecteursReco.Columns("reco").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            DSecteursReco.Columns("reco").DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12)
            DSecteursReco.Columns("Date").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            DSecteursReco.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ColoriageRecommandation(DSecteursReco, False)
            selectCurrent(DSecteursReco, False)

            Cursor.Current = Cursors.Default
        End Sub

        Private Sub fillGridValeurReco()
            'construction seulement si l'onglet est visible
            If Not TOngletEurope.SelectedTab Is TValeurReco Then
                Return
            End If

            Cursor.Current = Cursors.WaitCursor
            ' Insert les recommandations des valeurs et du secteur FGA associé
            Dim valeurs As String

            If CbSectorFGA.Text = "" Then
                If CbSuperSector.Text = "" Then
                    Return
                End If
                Dim supersecteur As String = "'" & Replace(CbSuperSector.Text, "'", "''") & "'"

                ' Table répertoriant les secteurs FGA du supersecteur
                valeurs = " SELECT distinct sect.date, sect.id_secteur, fga.label"
                valeurs = valeurs & " INTO #secteurs"
                valeurs = valeurs & " FROM ref_security.SECTOR ss"
                valeurs = valeurs & " INNER JOIN ref_security.SECTOR s ON s.id_parent=ss.id"
                valeurs = valeurs & " INNER JOIN ref_security.SECTOR_TRANSCO tr ON tr.id_sector1=s.id"
                valeurs = valeurs & " INNER JOIN ref_security.SECTOR fga ON fga.id=tr.id_sector2"
                valeurs = valeurs & " INNER JOIN (Select id_secteur, Max(date) As date FROM ACT_RECO_SECTOR"
                valeurs = valeurs & "               WHERE type = 'FGA'"
                valeurs = valeurs & "               GROUP BY id_secteur) sect on fga.code=sect.id_secteur"
                valeurs = valeurs & " WHERE tr.class_name='FGA_ALL' AND ss.label=" & supersecteur

                ' Récupération des secteurs et valeurs associées au supersecteur
                valeurs = valeurs & " SELECT distinct CONVERT(VARCHAR, reco.date, 103) as Date, sect.id_secteur AS 'Secteur FGA', NULL As ISIN, sect.label As Ticker, NULL as Libelle, NULL as liquidity, NULL AS exclu, NULL AS ""Note Quant"", 0 AS ""Quint Quant"", agr.MXEU as ""Poids MXEU"", agr.MXEM as ""Poids MXEM"", agr.MXEUM as ""Poids MXEUM"", agr.MXUSLC as ""Poids MXUSLC"", NULL AS ""reco MXEM"", NULL AS ""reco MXEUM"", reco.recommandation AS ""reco MXEU"""
                valeurs = valeurs & " FROM #secteurs sect"
                valeurs = valeurs & " LEFT OUTER JOIN ACT_RECO_SECTOR reco on sect.id_secteur = reco.id_secteur AND reco.type='FGA' AND reco.date=sect.date"
                valeurs = valeurs & " LEFT OUTER JOIN DATA_FACTSET agr on agr.fga_sector=sect.id_secteur"
                valeurs = valeurs & " WHERE agr.date=(SELECT MAX(date) FROM DATA_FACTSET) and agr.GICS_SECTOR is null and agr.MXEU is not null and MXUSLC is null"
                valeurs = valeurs & " UNION"
                valeurs = valeurs & " SELECT distinct CONVERT(VARCHAR, reco.date, 103) as Date, fga.fga_sector AS 'Secteur FGA', fac.ISIN, fac.Ticker AS Ticker, fac.company_name as Libelle, fac.liquidity_test as liquidity, CASE WHEN fac.ESG is null THEN 'X' ELSE NULL END AS exclu"
                valeurs = valeurs & " , fac.GARPN_TOTAL_S AS ""Note Quant"""
                valeurs = valeurs & " , NULL As 'Quint Quant'"

                valeurs = valeurs & " , fac.MXEU as ""Poids MXEU"", fac.MXEM as ""Poids MXEM"", fac.MXEUM as ""Poids MXEUM"", fac.MXUSLC as ""Poids MXUSLC"", reco_SXXE AS ""reco MXEM"", reco_SXXA AS ""reco MXEUM"", reco_SXXP AS ""reco MXEU"""
                valeurs = valeurs & " FROM DATA_FACTSET fac"
                valeurs = valeurs & " INNER JOIN DATA_FACTSET fga ON fga.GICS_SUBINDUSTRY=fac.SECTOR"
                valeurs = valeurs & " INNER JOIN #secteurs sect on sect.id_secteur=fga.FGA_sector"
                valeurs = valeurs & " LEFT OUTER JOIN ( "
                valeurs = valeurs & "                       SELECT ISIN, date, rank_total"
                valeurs = valeurs & " 							, FLOOR(rank_quant / ((	"
                valeurs = valeurs & " 											SELECT CAST(COUNT(*) + 1 AS float)"
                valeurs = valeurs & " 											FROM ACT_VALEUR_RANK r2"
                valeurs = valeurs & " 											INNER JOIN ACT_VALEUR v2 ON r2.id_value = v2.id"
                valeurs = valeurs & " 											WHERE date=r.date AND id_fga =v.id_fga"
                valeurs = valeurs & " 									) / 5) + 1"
                valeurs = valeurs & " 							)AS quintil_quant"
                valeurs = valeurs & " 						FROM ACT_VALEUR_RANK r"
                valeurs = valeurs & " 						INNER JOIN ACT_VALEUR v ON v.ID = r.id_value"
                valeurs = valeurs & "                 ) rank ON rank.ISIN = fac.ISIN  AND rank.date = fac.date"
                valeurs = valeurs & " LEFT OUTER JOIN ACT_VALEUR val on fac.ISIN = val.ISIN"
                ' table ACT_DATA_LIQUIDITY inutile car l indicateur est maintenant dans les donnees factset
                '                valeurs = valeurs & " LEFT OUTER JOIN ACT_DATA_LIQUIDITY liq on fac.ISIN = liq.ISIN"
                valeurs = valeurs & " LEFT OUTER JOIN ACT_RECO_VALEUR reco on fac.ISIN = reco.ISIN AND reco.date = (SELECT MAX(tmp.date) FROM ACT_RECO_VALEUR tmp WHERE tmp.ISIN = reco.ISIN)"
                valeurs = valeurs & " WHERE fac.date = (SELECT MAX(date) FROM ACT_DATA_FACTSET) AND fga.GICS_SUBINDUSTRY is not null"
                valeurs = valeurs & " ORDER BY 'Secteur FGA', ""Quint Quant"", ""Note Quant"" DESC"

                ' Suppression de la table temporaire.
                valeurs = valeurs & " DROP TABLE #secteurs"
            Else
                ' Récupération du secteur FGA et de ses valeurs associées
                Dim secteur_fga As String = "'" & Replace(CbSectorFGA.Text, "'", "''") & "'"
                Dim id_fga As Integer = co.SelectDistinctWhere("DATA_FACTSET", "FGA_SECTOR", "sector_label", CbSectorFGA.Text).FirstOrDefault

                Dim request As String
                request = "declare @nbTitres as Int" & _
                " set @nbTitres = (SELECT COUNT(*) FROM DATA_FACTSET where date='{0}' and SECTOR in ( select GICS_SUBINDUSTRY from DATA_FACTSET where fga_sector = '{1}' AND GICS_SUBINDUSTRY is not null and date = '{0}') and {2} is not null )" & _
                " SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_TOTAL_S DESC) AS rank_quant" & _
                " INTO #RANK_QUANT" & _
                " FROM DATA_FACTSET" & _
                " where SECTOR in ( select GICS_SUBINDUSTRY from DATA_FACTSET where fga_sector = '{1}' and date='{0}' ) and date='{0}' AND {2} is not null"

                If CbIndiceGeneral.Text = "Europe" Then
                    valeurs = String.Format(request, CbDateGeneral.Text, id_fga, "MXEU")
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    valeurs = String.Format(request, CbDateGeneral.Text, id_fga, "MXUSLC")
                End If

                valeurs = " SELECT distinct CONVERT(VARCHAR, reco.date, 103) as Date, fga.code AS 'Secteur FGA', NULL As ISIN, fga.label As Ticker, NULL as Libelle, NULL as liquidity, NULL AS exclu, NULL AS ""Note Quant"", 0 AS ""Quint Quant"","
                If CbIndiceGeneral.Text = "Europe" Then
                    valeurs = valeurs & " agr.MXEU as ""Poids MXEU"", agr.MXEM as ""Poids MXEM"", agr.MXEUM as ""Poids MXEUM"", NULL AS ""reco MXEM"", NULL AS ""reco MXEUM"", reco.recommandation AS ""reco MXEU""" ', NULL AS ""Note Quant"", 0 AS ""Quint Quant"", NULL AS ""Note Qual"", 0 AS ""Quint Qual"", NULL AS ""Note Totale"", 0 AS ""Quint Total"""
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    valeurs = valeurs & " agr.MXUSLC as ""Poids MXUSLC"", reco.recommandation AS ""reco MXUSLC"""
                End If
                valeurs = valeurs & " FROM ref_security.SECTOR fga "
                valeurs = valeurs & " LEFT OUTER JOIN ACT_RECO_SECTOR reco on reco.id_secteur=fga.code AND reco.type='FGA'"
                valeurs = valeurs & " LEFT OUTER JOIN DATA_FACTSET agr on agr.fga_sector=fga.code"
                valeurs = valeurs & " WHERE reco.date = (SELECT MAX(date) FROM ACT_RECO_SECTOR WHERE id_secteur='" & id_fga & "' AND type='FGA')"
                valeurs = valeurs & " AND agr.date='" & CbDateGeneral.Text & "'"
                If CbIndiceGeneral.Text = "Europe" Then
                    valeurs = valeurs & " AND fga.class_name='FGA_EU' AND agr.MXEU is not null AND agr.MXUSLC is null AND fga.code=" & id_fga
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    valeurs = valeurs & " AND fga.class_name='FGA_US' AND agr.MXEU is null AND agr.MXUSLC is not null AND fga.code=" & id_fga
                End If
                valeurs = valeurs & " UNION"
                valeurs = valeurs & " SELECT distinct CONVERT(VARCHAR, reco.date, 103) AS Date, fga.code AS 'Secteur FGA', fac.ISIN, fac.TICKER AS Ticker, fac.company_name as Libelle, fac.liquidity_test as liquidity, CASE WHEN fac.ESG is null THEN 'X' ELSE NULL END AS exclu"
                valeurs = valeurs & "       , fac.GARPN_TOTAL_S AS ""Note Quant"""
                valeurs = valeurs & "       ,FLOOR(rank_quant / (@nbTitres / 5) + 1 ) As 'Quint Quant'"

                If CbIndiceGeneral.Text = "Europe" Then
                    valeurs = valeurs & "       , MXEU as ""Poids MXEU"", MXEM as ""Poids MXEM"", MXEUM as ""Poids MXEUM"", reco_SXXE AS ""reco MXEM"", reco_SXXA AS ""reco MXEUM"", reco_SXXP AS ""reco MXEU"""
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    valeurs = valeurs & "       , MXUSLC as ""Poids MXUSLC"", reco_MXUSLC AS ""reco MXUSLC"""
                End If
                valeurs = valeurs & " FROM DATA_FACTSET fac"
                valeurs = valeurs & "     INNER JOIN  #RANK_QUANT as rank_quant ON rank_quant.ISIN = fac.ISIN"
                valeurs = valeurs & "     LEFT OUTER JOIN ACT_RECO_VALEUR reco on fac.ISIN = reco.ISIN AND reco.date = (	SELECT MAX(tmp.date) FROM ACT_RECO_VALEUR tmp WHERE tmp.ISIN = reco.ISIN)"
                valeurs = valeurs & "     INNER JOIN ref_security.SECTOR fga ON fga.label=" & secteur_fga & " AND fga.class_name='FGA_EU'"
                valeurs = valeurs & "     INNER JOIN ref_security.SECTOR_TRANSCO tr ON tr.id_sector1=fga.id"
                valeurs = valeurs & "     INNER JOIN ref_security.SECTOR ss ON ss.id=tr.id_sector2"
                valeurs = valeurs & " WHERE"
                valeurs = valeurs & "   fac.date='" & CbDateGeneral.Text & "'"
                If CbIndiceGeneral.Text = "Europe" Then
                    valeurs = valeurs & "   AND fac.sector=ss.code AND fac.MXEU is not null"
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    valeurs = valeurs & "   AND fac.sector=ss.code AND fac.MXUSLC is not null"
                End If
                valeurs = valeurs & " ORDER BY 'Secteur FGA', ""Quint Quant"", ""Note Quant"" DESC"
                valeurs = valeurs & " DROP TABLE #RANK_QUANT"
            End If

                DValeursReco.DataSource = co.LoadDataGridByString(valeurs)
                DValeursReco.DefaultCellStyle.Format = "n1"
                DValeursReco.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DValeursReco.Columns("Secteur FGA").Visible = False
                DValeursReco.Columns("ISIN").Visible = False
                DValeursReco.Columns("Date").Visible = False
                DValeursReco.Columns("Ticker").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                DValeursReco.Columns("Libelle").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                DValeursReco.Columns("liquidity").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursReco.Columns("liquidity").Width = 42
                DValeursReco.Columns("liquidity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DValeursReco.Columns("exclu").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursReco.Columns("exclu").Width = 42
                DValeursReco.Columns("exclu").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DValeursReco.Columns("Note Quant").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursReco.Columns("Note Quant").Width = 42
                DValeursReco.Columns("Note Quant").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                DValeursReco.Columns("Quint Quant").Visible = False
                If CbIndiceGeneral.Text = "Europe" Then
                    DValeursReco.Columns("Poids MXEUM").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("Poids MXEUM").DefaultCellStyle.Format = "F2"
                    DValeursReco.Columns("Poids MXEUM").Width = 48
                    DValeursReco.Columns("Poids MXEUM").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("Poids MXEM").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("Poids MXEM").DefaultCellStyle.Format = "F2"
                    DValeursReco.Columns("Poids MXEM").Width = 42
                    DValeursReco.Columns("Poids MXEM").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("Poids MXEU").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("Poids MXEU").DefaultCellStyle.Format = "F2"
                    DValeursReco.Columns("Poids MXEU").Width = 42
                    DValeursReco.Columns("Poids MXEU").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXEM").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("reco MXEM").Width = 42
                    DValeursReco.Columns("reco MXEM").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXEM").DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12)
                    DValeursReco.Columns("reco MXEUM").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("reco MXEUM").Width = 48
                    DValeursReco.Columns("reco MXEUM").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXEUM").DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12)
                    DValeursReco.Columns("reco MXEU").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("reco MXEU").Width = 42
                    DValeursReco.Columns("reco MXEU").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXEU").DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12)
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    DValeursReco.Columns("Poids MXUSLC").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("Poids MXUSLC").DefaultCellStyle.Format = "F2"
                    DValeursReco.Columns("Poids MXUSLC").Width = 52
                    DValeursReco.Columns("Poids MXUSLC").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXUSLC").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    DValeursReco.Columns("reco MXUSLC").Width = 52
                    DValeursReco.Columns("reco MXUSLC").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    DValeursReco.Columns("reco MXUSLC").DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12)
                End If

                ColoriageRecommandation(DValeursReco, True)

                Color_quintile(DValeursReco, "Quint Quant", New List(Of String)({"Note Quant"}))

                If CbIndiceGeneral.Text = "Europe" Then
                    For Each row As DataGridViewRow In DValeursReco.Rows
                        If IsDBNull(row.Cells("Poids MXEU").Value) OrElse row.Cells("Poids MXEU").Value Is Nothing Then
                            row.Cells("Poids MXEU").Style.BackColor = Color.Black
                        End If
                    Next
                ElseIf CbIndiceGeneral.Text = "USA" Then
                    For Each row As DataGridViewRow In DValeursReco.Rows
                        If IsDBNull(row.Cells("Poids MXUSLC").Value) OrElse row.Cells("Poids MXUSLC").Value Is Nothing Then
                            row.Cells("Poids MXUSLC").Style.BackColor = Color.Black
                        End If
                    Next
                End If

                selectCurrent(DValeursReco, True)
                addValeursRecoInfoBulle()

                Cursor.Current = Cursors.Default
        End Sub

        Private Sub addValeursRecoInfoBulle()
            If CbIndiceGeneral.Text = "Europe" Then
                For Each row As DataGridViewRow In DValeursReco.Rows
                    row.Cells("Poids MXEU").ToolTipText = _
                        "MXEM: " & row.Cells("Poids MXEM").Value.ToString _
                        & "\nMXEUM: " & row.Cells("Poids MXEUM").Value.ToString _
                        & "\nMXEU: " & row.Cells("Poids MXEU").Value.ToString
                Next
            ElseIf CbIndiceGeneral.Text = "USA" Then
                For Each row As DataGridViewRow In DValeursReco.Rows
                    row.Cells("Poids MXUSLC").ToolTipText = _
                        "MXUSLC: " & row.Cells("Poids MXUSLC").Value.ToString
                Next
            End If
        End Sub

        Private Sub selectCurrent(ByVal grid As DataGridView, ByVal isValeur As Boolean)
            ' check combobox and select sector in grid
            Dim secteur As String = "*"
            Dim valeur As String = "*"

            If isValeur Then
                If CbSectorFGA.Text <> "" And Not CbSectorFGA.Text.StartsWith("*") Then
                    secteur = CbSectorFGA.Text
                    valeur = CbValeur.Text
                Else
                    Return
                End If
            Else
                If CbSuperSector.Text <> "" And Not CbSuperSector.Text.StartsWith("*") Then
                    secteur = CbSuperSector.Text
                Else
                    Return
                End If
            End If


            For Each cells As DataGridViewCell In grid.SelectedCells
                cells.Selected = False
            Next

            If isValeur Then
                ' Sélection par valeur
                If valeur = "" Then
                    ' Sélection du secteur
                    For Each row As DataGridViewRow In grid.Rows
                        If row.Cells("Ticker").Value.ToString = secteur Then
                            row.Cells("Ticker").Selected = True
                        End If
                    Next
                Else
                    ' Sélection de la valeur.
                    For Each row As DataGridViewRow In grid.Rows
                        If row.Cells("Libelle").Value.ToString = valeur Then
                            row.Cells("Libelle").Selected = True
                        End If
                    Next
                End If
            Else
                ' Sélection sectorielle
                For Each row As DataGridViewRow In grid.Rows
                    If row.Cells("Secteur ICB").Value.ToString = secteur Then
                        If row.Cells("Secteur FGA").Value.ToString = "" Then
                            row.Cells("Secteur ICB").Selected = True
                        End If
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Changement de secteur pour afficher les détails
        ''' </summary>
        Public Sub fillSecteurRecoHisto()
            If (DSecteursReco.SelectedCells.Count > 0) Then
                Dim row As DataGridViewRow = DSecteursReco.SelectedCells(0).OwningRow
                Dim secteur_fga As String
                Dim reco As String

                secteur_fga = row.Cells("Secteur FGA").Value.ToString

                If secteur_fga <> "" Then
                    'Historique d'un secteur FGA
                    Dim id_secteur_fga = row.Cells.Item(DSecteursReco.Columns("id FGA").Index).Value

                    secteur_fga = "'" & Replace(secteur_fga, "'", "''") & "'"

                    reco = "SELECT type, date, " & secteur_fga & "As libelle, recommandation As reco, id_comment, id_comment_change"
                    reco = reco & " FROM ACT_RECO_SECTOR rec"
                    reco = reco & " WHERE rec.type = 'FGA' and rec.id_secteur = " & id_secteur_fga
                    reco = reco & " ORDER BY date DESC, type DESC"
                Else
                    'Historique d'un secteur ICB avec ses secteurs FGA
                    Dim secteur_icb As String = row.Cells.Item(DSecteursReco.Columns("Secteur ICB").Index).Value
                    Dim id_secteur_icb As String = row.Cells.Item(DSecteursReco.Columns("id ICB").Index).Value

                    secteur_icb = "'" & Replace(secteur_icb, "'", "''") & "'"

                    reco = "SELECT type, date, " & secteur_icb & " As libelle, recommandation As reco, id_comment, id_comment_change"
                    reco = reco & " FROM ACT_RECO_SECTOR rec"
                    reco = reco & " WHERE rec.type = 'ICB' and rec.id_secteur = " & id_secteur_icb
                    reco = reco & " UNION"
                    reco = reco & " SELECT type, date, fga.libelle As Libelle, recommandation As reco, id_comment, id_comment_change"
                    reco = reco & " FROM ACT_SUPERSECTOR sup"
                    reco = reco & " INNER JOIN ACT_SECTOR sec on sec.id_supersector = sup.id"
                    reco = reco & " INNER JOIN ACT_SUBSECTOR sub on sub.id_sector = sec.id"
                    reco = reco & " INNER JOIN ACT_FGA_SECTOR fga on fga.id = sub.id_fga_sector"
                    reco = reco & " INNER JOIN ACT_RECO_SECTOR rec on rec.id_secteur = fga.id and rec.type = 'FGA'"
                    reco = reco & " WHERE rec.type = 'FGA' and sup.id_federis = " & id_secteur_icb
                    reco = reco & " GROUP BY date, type, fga.libelle, recommandation, id_comment, id_comment_change"
                    reco = reco & " ORDER BY date DESC, type DESC"
                End If

                DSecteursRecoChange.DataSource = co.LoadDataGridByString(reco)
                DSecteursRecoChange.Columns("id_comment").Visible = False
                DSecteursRecoChange.Columns("id_comment_change").Visible = False
                DSecteursRecoChange.Columns("reco").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                If DSecteursRecoChange.RowCount > 0 Then
                    DSecteursRecoChange.Rows(0).Cells(0).Selected = True
                End If
            End If
        End Sub

        Private Sub fillValeursRecoHisto()
            If (DValeursReco.SelectedCells.Count > 0) Then
                Dim row As DataGridViewRow = DValeursReco.SelectedCells(0).OwningRow
                Dim isin As String
                Dim reco As String

                isin = row.Cells("ISIN").Value.ToString

                If isin = "" Then
                    'Historique d'un secteur FGA
                    Dim id_secteur_fga As Integer = row.Cells("Secteur FGA").Value
                    Dim secteur_fga As String = "'" & Replace(row.Cells("Ticker").Value.ToString, "'", "''") & "'"

                    reco = "SELECT date, " & secteur_fga & " As Libelle, recommandation As reco, id_comment, id_comment_change"
                    reco = reco & " FROM ACT_RECO_SECTOR rec"
                    reco = reco & " WHERE rec.type = 'FGA' and rec.id_secteur = " & id_secteur_fga
                    reco = reco & " ORDER BY date DESC"
                Else
                    'Historique d'une valeur
                    Dim valeur As String = "'" & Replace(row.Cells("Libelle").Value.ToString, "'", "''") & "'"
                    isin = "'" & Replace(isin, "'", "''") & "'"

                    reco = "SELECT date as Date, " & valeur & "As Libelle, reco_SXXE As 'Reco SXXE', reco_SXXA As 'Reco SXXA', reco_SXXP As 'reco MXEU', id_comment, id_comment_change"
                    reco = reco & " FROM ACT_RECO_VALEUR"
                    reco = reco & " WHERE isin = " & isin
                    reco = reco & " ORDER BY date DESC"
                End If

                DValeursRecoChange.DataSource = co.LoadDataGridByString(reco)
                DValeursRecoChange.Columns("id_comment").Visible = False
                DValeursRecoChange.Columns("id_comment_change").Visible = False
                DValeursRecoChange.Columns("Date").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                DValeursRecoChange.Columns("Libelle").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

                If DValeursRecoChange.RowCount > 0 Then
                    DValeursRecoChange.Rows(0).Cells(0).Selected = True
                End If
            End If
        End Sub

        ''' <summary>
        ''' Colorie la grille avec le code couleur des secteurs et colorie la colonne recommandation en fonction du signe.
        ''' </summary>
        ''' <param name="DataGrid">La grille à colorier</param>
        ''' <remarks></remarks>
        Public Sub ColoriageRecoHisto(ByVal DataGrid As DataGridView, ByVal isvaleur As Boolean)
            DataGrid.SuspendLayout()

            For Each col As DataGridViewColumn In DataGrid.Columns
                If col.HeaderText.ToLower.Contains("reco") Then
                    Dim names As New Dictionary(Of String, String)

                    For i = DataGrid.Rows.Count - 1 To 0 Step -1
                        Dim cell As DataGridViewCell = DataGrid.Rows(i).Cells("libelle")
                        Dim reco As DataGridViewCell = DataGrid.Rows(i).Cells.Item(col.Index)
                        Dim name As String

                        If isvaleur Then
                            name = cell.Value.ToString
                        Else
                            Dim type As String = DataGrid.Rows(i).Cells("type").Value.ToString

                            If type = "ICB" Then
                                'Ligne supersector
                                DataGrid.Rows(i).DefaultCellStyle.BackColor = Color.Tan
                                DataGrid.Rows(i).DefaultCellStyle.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, FontStyle.Bold)
                            End If
                            name = cell.Value.ToString + type
                        End If

                        reco.Style.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, FontStyle.Bold)

                        If names.ContainsKey(name) Then
                            Dim old_reco As Integer = getRecoValue(names(name))
                            Dim new_reco As Integer = getRecoValue(reco.Value.ToString)

                            If new_reco < old_reco Then
                                ' recommandation moins bonne
                                reco.Style.BackColor = Color.LightPink
                            ElseIf new_reco > old_reco Then
                                ' meilleure recommandation 
                                reco.Style.BackColor = Color.LightGreen
                            Else
                                reco.Style.BackColor = Color.White
                            End If
                            names(name) = reco.Value.ToString
                        Else
                            names.Add(name, reco.Value.ToString)
                        End If
                    Next
                End If
            Next

            DataGrid.ResumeLayout()
        End Sub

        Function getRecoValue(ByVal reco As String) As Double
            Dim res As Integer = 0

            If reco = "" Then
                Return Integer.MinValue
            End If

            res += 2 * reco.Count(AddressOf isPlus)
            res -= 2 * reco.Count(AddressOf isMinus)

            Return res
        End Function

        Private Function isPlus(ByVal c As Char) As Boolean
            Return c = "+"
        End Function

        Private Function isMinus(ByVal c As Char) As Boolean
            Return c = "-"
        End Function

        Private Sub ColoriageRecommandation(ByVal datagrid As DataGridView, ByVal isValeur As Boolean)
            datagrid.SuspendLayout()

            Dim bold_font As System.Drawing.Font = New System.Drawing.Font(DefaultFont, FontStyle.Bold)
            Dim big_font As System.Drawing.Font = New System.Drawing.Font(System.Windows.Forms.Control.DefaultFont.FontFamily.ToString, 12, FontStyle.Bold)
            For Each col As DataGridViewColumn In datagrid.Columns
                If col.HeaderText.Contains("reco") Then

                    For i = 0 To datagrid.Rows.Count - 1
                        Dim cell As DataGridViewCell = datagrid.Rows(i).Cells.Item(col.Index)
                        'cell.Style.Font = default_font

                        If isValeur Then
                            If IsDBNull(datagrid.Item(datagrid.Columns("Libelle").Index, i).Value) Then
                                'Ligne secteur FGA coloriée seulement sur une grille avec des valeurs
                                datagrid.Rows(i).DefaultCellStyle.BackColor = Color.PaleGoldenrod
                                datagrid.Rows(i).DefaultCellStyle.Font = bold_font

                                color_cell(cell)
                            End If
                        ElseIf IsDBNull(datagrid.Item(datagrid.Columns("Secteur FGA").Index, i).Value) Then
                            'Ligne supersector
                            datagrid.Rows(i).DefaultCellStyle.BackColor = Color.Tan
                            datagrid.Rows(i).DefaultCellStyle.Font = bold_font

                            color_cell(cell)
                        End If
                        ' Big size for reco.
                        cell.Style.Font = big_font
                    Next
                End If
            Next
            datagrid.ResumeLayout()
        End Sub

        Private Sub color_cell(ByRef cell As DataGridViewCell)
            If cell.Value.ToString = "--" Then
                'recommandation "-"
                cell.Style.BackColor = Color.Purple
            ElseIf cell.Value.ToString = "-" Then
                'recommandation "+"
                cell.Style.BackColor = Color.LightPink
            ElseIf cell.Value.ToString = "+" Then
                'recommandation "+"
                cell.Style.BackColor = Color.LightGreen
            ElseIf cell.Value.ToString = "++" Then
                'recommandation "+"
                cell.Style.BackColor = Color.Green
            End If
        End Sub

        'Private Sub BExcelSectoriel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelSectoriel.Click
        '    da.DataGridToNewExcel(DSecteurs, "Grille sectorielle " & TLastDate.Text)
        'End Sub
        Private Sub DSecteursReco_BindingContextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursReco.BindingContextChanged
            FillGridSecteurReco()
        End Sub

        Private Sub DValeursReco_BindingContextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DValeursReco.BindingContextChanged
            If CbSuperSector.Text <> "" Then
                fillGridValeurReco()
            End If
        End Sub

        Private Sub DSecteursReco_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursReco.SelectionChanged
            fillSecteurRecoHisto()
        End Sub

        Private Sub DValeursReco_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DValeursReco.SelectionChanged
            fillValeursRecoHisto()
        End Sub

        Private Sub DSecteursRecoChange_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DSecteursRecoChange.DataBindingComplete
            ColoriageRecoHisto(DSecteursRecoChange, False)
        End Sub

        Private Sub DValeursRecoChange_DataBindingComplete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles DValeursRecoChange.DataBindingComplete
            ColoriageRecoHisto(DValeursRecoChange, True)
        End Sub

        Private Sub DSecteursRecoChange_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursRecoChange.CurrentCellChanged
            If DSecteursRecoChange.CurrentRow IsNot Nothing Then
                DSecteursRecoChange.SuspendLayout()

                Dim id As Integer = DSecteursRecoChange.CurrentRow.Cells("id_comment").Value.ToString
                Dim rtf As List(Of Object) = co.SelectWhere("ACT_RECO_COMMENT", "comment", "id", id)

                If rtf Is Nothing Then
                    TCommentSecteursReco.Rtf = ""
                Else
                    TCommentSecteursReco.Rtf = rtf.FirstOrDefault
                End If
                TCommentSecteursReco.Font = New System.Drawing.Font(TCommentSecteursReco.Font.Name, 10)
                commentHasChanged = False

                'reco change
                id = DSecteursRecoChange.CurrentRow.Cells("id_comment_change").Value.ToString
                rtf = co.SelectWhere("ACT_RECO_COMMENT", "comment", "id", id)

                If rtf Is Nothing Then
                    TCommentSecteursRecoChange.Rtf = ""
                Else
                    TCommentSecteursRecoChange.Rtf = rtf.FirstOrDefault
                End If
                TCommentSecteursRecoChange.Font = New System.Drawing.Font(TCommentSecteursRecoChange.Font.Name, 10)
                commentHasChanged = False
                DSecteursRecoChange.ResumeLayout()
            End If
        End Sub

        Private Sub DValeursRecoChange_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DValeursRecoChange.CurrentCellChanged
            If DValeursRecoChange.CurrentRow IsNot Nothing Then

                DValeursRecoChange.SuspendLayout()

                'reco commentaire
                Dim id As Integer = DValeursRecoChange.CurrentRow.Cells("id_comment").Value.ToString
                Dim rtf As List(Of Object) = co.SelectWhere("ACT_RECO_COMMENT", "comment", "id", id)

                TCommentValeursReco.Visible = True
                If rtf Is Nothing Then
                    TCommentValeursReco.Rtf = ""
                Else
                    TCommentValeursReco.Rtf = rtf.FirstOrDefault
                End If
                TCommentValeursReco.Font = New System.Drawing.Font(TCommentValeursReco.Font.Name, 10)
                commentHasChanged = False

                'reco change
                id = DValeursRecoChange.CurrentRow.Cells("id_comment_change").Value.ToString
                rtf = co.SelectWhere("ACT_RECO_COMMENT", "comment", "id", id)

                TCommentValeursRecoChange.Visible = True
                If rtf Is Nothing Then
                    TCommentValeursRecoChange.Rtf = ""
                Else
                    TCommentValeursRecoChange.Rtf = rtf.FirstOrDefault
                End If
                TCommentValeursRecoChange.Font = New System.Drawing.Font(TCommentValeursRecoChange.Font.Name, 10)
                commentHasChanged = False
                DValeursRecoChange.ResumeLayout()
            Else
                Try
                    TCommentValeursRecoChange.Visible = False
                    TCommentValeursReco.Visible = False
                Catch ex As Exception

                End Try
            End If
        End Sub

        Private Sub TCommentValeursReco_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TCommentValeursReco.Leave, TCommentValeursRecoChange.Leave
            checkSaveRecoText()
        End Sub

        Private Sub DSecteursReco_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DSecteursReco.Sorted
            ColoriageRecommandation(DSecteursReco, False)
        End Sub

        Private Sub DValeursReco_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DValeursReco.Sorted
            ColoriageRecommandation(DValeursReco, True)
            Color_quintile(DValeursReco, "Quint Quant", New List(Of String)({"Note Quant"}))
            Color_quintile(DValeursReco, "Quint Qual", New List(Of String)({"Note Qual"}))
        End Sub

        Private Sub SplitContainer70Height_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SplitContainer2.Resize, SplitContainer5.Resize
            Dim container As SplitContainer = TryCast(sender, SplitContainer)
            If container Is Nothing Then
                Return
            End If

            'Move splitter to 70% of its container.
            Dim dist As Integer = container.Height * 70 / 100

            If (dist > container.Panel1MinSize) Then
                container.SplitterDistance = dist
            End If
        End Sub

        Private Sub SplitContainer40Width_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SplitContainer3.Resize, SplitContainer6.Resize
            Dim container As SplitContainer = TryCast(sender, SplitContainer)
            If container Is Nothing Then
                Return
            End If

            'Move splitter to 40% of its container.
            Dim dist As Integer = container.Width * 40 / 100

            If (dist > container.Panel1MinSize) Then
                container.SplitterDistance = dist
            End If
        End Sub

        Private Sub saveCommentSecteurReco()
            If DSecteursRecoChange.CurrentRow IsNot Nothing Then
                DSecteursRecoChange.SuspendLayout()

                'reco commentaire
                Dim id As Integer = DSecteursRecoChange.CurrentRow.Cells("id_comment").Value
                'Update du commentaire
                If id > 0 Then
                    co.Update("ACT_RECO_COMMENT",
                              New List(Of String)({"comment"}),
                              New List(Of Object)({TCommentSecteursReco.Rtf}),
                              "id",
                              id)
                End If

                'reco change
                id = DSecteursRecoChange.CurrentRow.Cells("id_comment_change").Value
                If id > 0 Then
                    co.Update("ACT_RECO_COMMENT",
                              New List(Of String)({"comment"}),
                              New List(Of Object)({TCommentSecteursRecoChange.Rtf}),
                              "id",
                              id)
                End If

                DSecteursRecoChange.ResumeLayout()
            End If
        End Sub

        Private Sub saveCommentValeurReco()
            If DValeursRecoChange.CurrentRow IsNot Nothing Then
                DValeursRecoChange.SuspendLayout()

                'reco commentaire
                Dim id As Integer = DValeursRecoChange.CurrentRow.Cells("id_comment").Value
                'Update du commentaire
                If id > 0 Then
                    co.Update("ACT_RECO_COMMENT",
                              New List(Of String)({"comment"}),
                              New List(Of Object)({TCommentValeursReco.Rtf}),
                              "id",
                              id)
                End If

                'reco change
                id = DValeursRecoChange.CurrentRow.Cells("id_comment_change").Value
                If id > 0 Then
                    co.Update("ACT_RECO_COMMENT",
                              New List(Of String)({"comment"}),
                              New List(Of Object)({TCommentValeursRecoChange.Rtf}),
                              "id",
                              id)
                End If

                DValeursRecoChange.ResumeLayout()
            End If
        End Sub

        Private Sub newSecteurReco()
            Dim recoWindow As BaseActionRecommandation
            Dim headers As New List(Of String)
            Dim options As New List(Of String)
            Dim values As New List(Of List(Of String))

            'construction de la liste des headers
            headers.Add("id ICB")
            headers.Add("Secteur ICB")
            headers.Add("id FGA")
            headers.Add("Secteur FGA")
            headers.Add("Date")
            headers.Add("reco")

            'construction de la liste des options
            options.Add("-")
            options.Add("=")
            options.Add("+")

            'construction de la liste des valeurs
            For Each row As DataGridViewRow In getICBRows(DSecteursReco)
                Dim fields As New List(Of String)

                For Each header As String In headers
                    fields.Add(row.Cells(header).Value.ToString)
                Next
                values.Add(fields)
            Next

            recoWindow = New BaseActionRecommandation(headers, options, values, False)
            recoWindow.ShowDialog()
        End Sub

        Private Sub NewValeurReco()
            Dim recoWindow As BaseActionRecommandation
            Dim headers As List(Of String)
            Dim options As New List(Of String)
            Dim values As New List(Of List(Of String))

            ' construction des headers
            headers = New List(Of String)
            headers.Add("ISIN")
            headers.Add("Libelle")
            headers.Add("Date")
            headers.Add("Indice")
            headers.Add("reco")

            ' construction de la liste des options
            options.Add("-")
            options.Add("=")
            options.Add("+")
            options.Add("++")
            options.Add("OUT")

            ' construction de la liste des valeurs
            For Each row As DataGridViewRow In getSelectedRows(DValeursReco)
                ' ajout d'une ligne par indice
                For Each col As DataGridViewColumn In DValeursReco.Columns
                    If col.HeaderText.Contains("reco") Then
                        Dim fields As New List(Of String)({row.Cells("ISIN").Value.ToString,
                                                           row.Cells("Libelle").Value.ToString,
                                                           row.Cells("Date").Value.ToString,
                                                           col.HeaderText.Substring("reco ".Length),
                                                           row.Cells(col.HeaderText).Value.ToString})
                        values.Add(fields)
                    End If
                Next
            Next

            recoWindow = New BaseActionRecommandation(headers, options, values, True)
            recoWindow.ShowDialog()
        End Sub

        Private Function getICBRows(ByVal grid As DataGridView) As List(Of DataGridViewRow)
            Dim rows As New List(Of DataGridViewRow)
            Dim ids As New List(Of Integer)
            Dim idICB As Integer

            For Each cell As DataGridViewCell In grid.SelectedCells
                idICB = cell.OwningRow.Cells("id ICB").Value
                If Not ids.Contains(idICB) Then
                    For Each row As DataGridViewRow In grid.Rows
                        If row.Cells("id ICB").Value = idICB Then
                            rows.Add(row)
                        End If
                    Next
                    ids.Add(idICB)
                End If
            Next

            Return rows
        End Function

        Private Function getSelectedRows(ByVal grid As DataGridView) As List(Of DataGridViewRow)
            Dim rows As New List(Of DataGridViewRow)
            Dim isins As New List(Of String)

            For Each cell As DataGridViewCell In grid.SelectedCells
                Dim isin As String = cell.OwningRow.Cells("ISIN").Value.ToString

                If Not isins.Contains(isin) Then
                    rows.Add(cell.OwningRow)
                    isins.Add(isin)
                End If
            Next

            Return rows
        End Function
#End Region

#Region "Europe - Blend"

        Private Sub CbGarp_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateGeneral.SelectedValueChanged
            If co.SelectDistinctWhere("DATA_FACTSET", "date", "date", CbDateGeneral.Text).Count > 0 Then

                DTPChart.Value = CbDateGeneral.Text
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ''''''''''''''''''''''''''''''''A DECOMMENTER SI ON VEUX REMETTRE LES SCORES DES SECTEURS''''''''''''''''''''''''''''''''
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '' '' ''da.RowHeaderCell(DSecteursBlend)

                '' '' ''clearGrid(DSecteursBlend)

                '' '' ''DSecteursBlend.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_DataGridBlend", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {CbDateGeneral.Text})))


                ' '' '' ''If DBlend.ColumnCount = 18 Then
                '' '' ''DSecteursBlend.DefaultCellStyle.Format = "n1"
                '' '' ''DSecteursBlend.Columns(1).Frozen = True
                '' '' ''DSecteursBlend.Columns("Code").DefaultCellStyle.Format = "## ## ##"
                ' '' '' ''DSecteursBlend.Columns("Old Score").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("FGA Sector").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                '' '' ''DSecteursBlend.Columns("Score").DefaultCellStyle.Format = "n2"
                ' '' '' ''Growth
                '' '' ''DSecteursBlend.Columns("Growth").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("Growth").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("Cr Sales %").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("Cr Sales %").DefaultCellStyle.Format = "#0.#\%"
                '' '' ''DSecteursBlend.Columns("Sales CV").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("Sales CV").DefaultCellStyle.Format = "n3"
                '' '' ''DSecteursBlend.Columns("Sales trend").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("Cr EPS %").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("Cr EPS %").DefaultCellStyle.Format = "#0.#\%"
                '' '' ''DSecteursBlend.Columns("EPS CV").DefaultCellStyle.BackColor = Color.LightBlue
                '' '' ''DSecteursBlend.Columns("EPS CV").DefaultCellStyle.Format = "n3"
                '' '' ''DSecteursBlend.Columns("EPS trend").DefaultCellStyle.BackColor = Color.LightBlue
                ' '' '' ''profit.
                '' '' ''DSecteursBlend.Columns("Profit.").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("Profit.").DefaultCellStyle.BackColor = Color.LightCoral
                '' '' ''DSecteursBlend.Columns("EBIT Margin trend").DefaultCellStyle.BackColor = Color.LightCoral
                '' '' ''DSecteursBlend.Columns("EBIT Margin CV").DefaultCellStyle.BackColor = Color.LightCoral
                '' '' ''DSecteursBlend.Columns("EBIT Margin CV").DefaultCellStyle.Format = "n3"
                '' '' ''DSecteursBlend.Columns("ROE %").DefaultCellStyle.BackColor = Color.LightCoral
                '' '' ''DSecteursBlend.Columns("ROE %").DefaultCellStyle.Format = "#0.#\%"
                ' '' '' ''value
                '' '' ''DSecteursBlend.Columns("Value").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("Value").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("Dvd Yld %").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("Dvd Yld %").DefaultCellStyle.Format = "#0.#\%"
                '' '' ''DSecteursBlend.Columns("PE x").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PE x").DefaultCellStyle.Format = "#0.#\x"
                '' '' ''DSecteursBlend.Columns("PE / Avg5Y").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PE / Avg5Y").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("PE rel / Avg5Y").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PE rel / Avg5Y").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("PB x").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PB x").DefaultCellStyle.Format = "#0.#\x"
                '' '' ''DSecteursBlend.Columns("PB / Avg5Y").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PB / Avg5Y").DefaultCellStyle.Format = "n2"
                '' '' ''DSecteursBlend.Columns("PB rel / Avg5Y").DefaultCellStyle.BackColor = Color.LightSlateGray
                '' '' ''DSecteursBlend.Columns("PB rel / Avg5Y").DefaultCellStyle.Format = "n2"
                ' '' '' ''End If

                '' '' ''For i = 0 To DSecteursBlend.Rows.Count - 1
                '' '' ''    If DSecteursBlend.Item(DSecteursBlend.Columns("Code").Index, i).Value.ToString = "25" Then
                '' '' ''        DSecteursBlend.Rows(i).Cells(DSecteursBlend.Columns("EBIT Margin trend").Index).Style.BackColor = Color.Chocolate
                '' '' ''        DSecteursBlend.Item(DSecteursBlend.Columns("EBIT Margin CV").Index, i).Style.BackColor = Color.Chocolate
                '' '' ''        DSecteursBlend.Refresh()
                '' '' ''    End If
                '' '' ''Next

                ' '' '' ''Mise à jour de la grille Valeurs/Analyse
                '' '' ''ValeurAnalyseBlend()

            Else
                MessageBox.Show("La table DATA_FACTSET est vide au " & CbDateGeneral.Text & " !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub


        ''' <summary>
        ''' BExcelGarp : Exporte la grille DGarp vers Excel
        ''' </summary>
        Private Sub BExcelGarp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelBlendSecteur.Click
            'da.DataGridToNewExcel(DSecteursBlend, "Grille Blend " & CbBlend.Text)
            da.DataGridsToNewExcel(New List(Of DataGridView)(New DataGridView() {DSecteursBlend, DSecteursNote}), New List(Of String)(New String() {"Scrore", "Note"}), New List(Of Integer)(New Integer() {1, 1}), CbDateGeneral.Text)
        End Sub

        ''' <summary>
        ''' BDeleteGarp : Vide la grille DGarp
        ''' </summary>
        Private Sub BDeleteGarp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            clearGrid(DSecteursBlend)
        End Sub


        Private Sub DSecteursNote_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DSecteursNote.CellEndEdit
            '        If e.KeyCode = Keys.Enter And DSecteursNote.RowCount > 0 Then 'si la touche controle est appuyée ainsi que la touche V

            If co.SelectSimple("ACT_NOTE", "id").Contains(DSecteursNote.CurrentCell.Value) Then

                Dim id_theme As String

                Select Case DSecteursNote.Columns(DSecteursNote.CurrentCell.ColumnIndex).HeaderText
                    Case "Sensibilité à amélioration macro"
                        id_theme = "sam"
                    Case "CA exposé aux émergents"
                        id_theme = "exEm"
                    Case "CA exposé aux US"
                        id_theme = "exU"
                    Case "Hausse du coût de refinancement"
                        id_theme = "hcr"
                    Case "Augmentation fiscalité"
                        id_theme = "af2"
                    Case "Sécurité du dividende"
                        id_theme = "sdd"
                    Case "Baisse de l'€ favorable"
                        id_theme = "beuro"
                    Case "Hausse de l'€ favorable"
                        id_theme = "heuro"
                    Case "Sensibilité à la hausse du prix de l'énergie"
                        id_theme = "she"
                    Case "Sensibilité à la hausse des coûts matières"
                        id_theme = "shmp"
                    Case "CA exposé en Europe"
                        id_theme = "exEu"
                    Case """Shareholder friendly"""
                        id_theme = "share"
                    Case Else
                        MsgBox("Le titre de la colonne n'est pas reconnu par l'application")
                        Exit Sub
                End Select

                Dim value As String = DSecteursNote.CurrentCell.Value
                'Dim id_secteur = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", DSecteursNote.Rows(DSecteursNote.CurrentCell.RowIndex).Cells(1).Value).FirstOrDefault()
                Dim id_secteur = DSecteursNote.CurrentRow.Cells(0).Value

                Dim datagrid As DataGridView = sender

                If co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, id_theme})).Count = 0 Then
                    'cas si pas encore de note dans le secteur
                    co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, id_theme, value}))
                    BeginInvoke(New MethodInvoker(AddressOf PopulateControl))
                Else
                    'cas si existe deja une note pour le secteur dans la base
                    If value <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, id_theme, co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, id_theme})).FirstOrDefault})).FirstOrDefault Then
                        Dim a As Integer = MessageBox.Show("Voulez vous vraiment changer la notation du secteur ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        If a = 6 Then
                            co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, id_theme, value}))
                            BeginInvoke(New MethodInvoker(AddressOf PopulateControl))
                        End If
                    End If
                End If
            ElseIf DSecteursNote.CurrentCell.Value Is DBNull.Value Then
            Else

                MessageBox.Show("Le type de note " & DSecteursNote.CurrentCell.Value & " n'est pas reconnu par l'application !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        Sub PopulateControl()
            DSecteursNote.DataSource = co.LoadDataGridByString("Execute ACT_Sectorielle_Note")
            'DSecteursNote.CurrentCell = DSecteursNote.Rows(0).Cells(1)
        End Sub

#End Region

#Region "Style - Value"

        ' ''' <summary>
        ' ''' BExcelValue : Exporte la grille DValue vers Excel
        ' ''' </summary>
        'Private Sub BExcelValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelValue.Click
        '    da.DataGridToNewExcel(DValue, "Grille Value " & CbDateGrowth.Text)
        'End Sub

        ' ''' <summary>
        ' ''' BDeleteValue : Vide la grille DValue
        ' ''' </summary>
        'Private Sub BViderValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BViderValue.Click
        '    clearGrid(DValue)
        'End Sub

        'Private Sub CbDateValue_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateValue.SelectedValueChanged
        '    If co.SelectDistinctWhere("DATA_FACTSET", "date", "date", CbDateValue.Text).Count > 0 Then
        '        DValue.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_DataGridValue", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {CbDateValue.Text})))
        '        da.PresentationDataGrid(DValue, 1)
        '        DValue.Columns(1).Frozen = True
        '        da.AutoFiltre(DValue, New List(Of Integer)(New Integer() {0, 1, 2, 3, 5, 6, 7}))
        '    Else
        '        MessageBox.Show("La table DATA_FACTSET est vide au " & CbDateValue.Text & " !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
        '    End If
        'End Sub

#End Region

#Region "Style - Growth"

        ' ''' <summary>
        ' ''' BDeleteGrowth : Vide la grille DGrowth
        ' ''' </summary>
        'Private Sub BViderGrowth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BViderGrowth.Click
        '    clearGrid(DGrowth)
        'End Sub

        ' ''' <summary>
        ' ''' BExcelGrowth : Exporte la grille DGrowth vers Excel
        ' ''' </summary>
        'Private Sub BExcelGrowth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelGrowth.Click
        '    da.DataGridToNewExcel(DGrowth, "Grille Growth " & CbDateGrowth.Text)
        'End Sub

        'Private Sub CbDateGrowth_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateGrowth.SelectedValueChanged
        '    If co.SelectDistinctWhere("DATA_FACTSET", "date", "date", CbDateGrowth.Text).Count > 0 Then
        '        DGrowth.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_DataGridGrowth", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {CbDateGrowth.Text})))
        '        da.PresentationDataGrid(DGrowth, 1)
        '        DGrowth.Columns(1).Frozen = True
        '        da.AutoFiltre(DGrowth, New List(Of Integer)(New Integer() {0, 1, 2, 3, 5, 6, 7}))
        '    Else
        '        MessageBox.Show("La table DATA_FACTSET est vide au " & CbDateGrowth.Text & " !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
        '    End If
        'End Sub

#End Region

#Region "Style - Radar"

        Private Sub RadarDGVMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadarDGVMenu.Click
            Dim id_fga As Integer =
            co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_ALL", 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault()
            Dim ticker As String = DValeursBlend.Rows(DValeursBlend.SelectedCells.Item(0).RowIndex).Cells("Ticker").Value
            Dim valeur As String = DValeursBlend.Rows(DValeursBlend.SelectedCells.Item(0).RowIndex).Cells("Company Name").Value

            Me.CbValeur.SelectedIndex = Me.CbValeur.FindStringExact(valeur + " | " + ticker)

            'Construction du radar Growth
            Dim growth1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Growth", ticker, id_fga.ToString, ""}))
            GrowthRadar.Series("Series1").Points.DataBindXY(growth1.Keys, growth1.Values)
            Dim growth3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Growth", ticker, id_fga.ToString, "AVG"}))
            GrowthRadar.Series("Series3").Points.DataBindXY(growth3.Keys, growth3.Values)

            'Construction du radar Value
            Dim value1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Value", ticker, id_fga.ToString, ""}))
            ValueRadar.Series("Series1").Points.DataBindXY(value1.Keys, value1.Values)
            Dim value3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Value", ticker, id_fga.ToString, "AVG"}))
            ValueRadar.Series("Series3").Points.DataBindXY(value3.Keys, value3.Values)

            'Construction du radar Qualite
            Dim qualite1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Qualite", ticker, id_fga.ToString, ""}))
            QualiteRadar.Series("Series1").Points.DataBindXY(qualite1.Keys, qualite1.Values)
            Dim qualite3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Qualite", ticker, id_fga.ToString, "AVG"}))
            QualiteRadar.Series("Series3").Points.DataBindXY(qualite3.Keys, qualite3.Values)

            'Construction du radar Growth
            Dim total1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Total", ticker, id_fga.ToString, ""}))
            ChartBar.Series("Series1").Points.DataBindXY(total1.Keys, total1.Values)
            ChartBar.Series("Series1").LegendText = ticker

        End Sub

        Private Sub ScoreMetricGrid(ByVal id_fga As Integer)
            If CbValeur.Text = "" Or CbValeur2.Text = "" Then
                Return
            End If

            Dim dateMax2 As String = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPChart.Value + "'"))(0))("date")
            Dim ticker1 As String = CbValeur.Text.Substring(CbValeur.Text.IndexOf("|") + 2)
            Dim ticker2 As String = CbValeur2.Text.Substring(CbValeur2.Text.IndexOf("|") + 2)

            DValeursChart.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_Radar_Value", New List(Of String)(New String() {"@date", "@date2", "@TICKER1", "@TICKER2", "@FGA"}), New List(Of Object)(New Object() {CbDateGeneral.Text, dateMax2, ticker1, ticker2, id_fga.ToString})))

            If DValeursChart.ColumnCount > 0 Then
                DValeursChart.DefaultCellStyle.Format = "n1"
                DValeursChart.Columns("GROWTH").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                DValeursChart.Columns("VALUE").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                DValeursChart.Columns("PROFIT").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

                DValeursChart.Columns(1).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(2).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(6).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(7).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(11).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(12).DefaultCellStyle.BackColor = Color.LightBlue
                DValeursChart.Columns(3).DefaultCellStyle.BackColor = Color.PaleGreen
                DValeursChart.Columns(4).DefaultCellStyle.BackColor = Color.PaleGreen
                DValeursChart.Columns(8).DefaultCellStyle.BackColor = Color.PaleGreen
                DValeursChart.Columns(9).DefaultCellStyle.BackColor = Color.PaleGreen
                DValeursChart.Columns(13).DefaultCellStyle.BackColor = Color.PaleGreen
                DValeursChart.Columns(14).DefaultCellStyle.BackColor = Color.PaleGreen

                DValeursChart.Columns(1).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(2).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(6).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(7).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(11).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(12).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(3).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(4).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(8).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(9).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(13).DefaultCellStyle.Format = "n2"
                DValeursChart.Columns(14).DefaultCellStyle.Format = "n2"

                DValeursChart.Columns("GROWTH").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursChart.Columns("VALUE").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursChart.Columns("PROFIT").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                DValeursChart.Columns("GROWTH").Width = 140
                DValeursChart.Columns("VALUE").Width = 140
                DValeursChart.Columns("PROFIT").Width = 140

            End If
        End Sub

        Private Sub CbValeur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbValeur.SelectedValueChanged
            If CbValeur.Text = "" Then
                Return
            End If

            Dim id_fga As Integer =
            co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_ALL", 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault()
            Dim ticker As String = CbValeur.Text.Substring(CbValeur.Text.IndexOf("|") + 2)

            'Construction du radar Growth
            Dim growth1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Growth", ticker, id_fga.ToString, ""}))
            GrowthRadar.Series("Series1").Points.DataBindXY(growth1.Keys, growth1.Values)

            'Construction du radar Value
            Dim value1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Value", ticker, id_fga.ToString, ""}))
            ValueRadar.Series("Series1").Points.DataBindXY(value1.Keys, value1.Values)

            'Construction du radar Qualite
            Dim qualite1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Qualite", ticker, id_fga.ToString, ""}))
            QualiteRadar.Series("Series1").Points.DataBindXY(qualite1.Keys, qualite1.Values)

            'Construction du radar Growth
            Dim total1 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Total", ticker, id_fga.ToString, ""}))
            ChartBar.Series("Series1").Points.DataBindXY(total1.Keys, total1.Values)
            If CbValeur.Text = CbValeur2.Text Then
                Dim stock As List(Of Dictionary(Of String, Object))
                stock = co.sqlToListDico("SELECT distinct DATE AS 'date' FROM DATA_FACTSET WHERE date<'" + DTPChart.Value + "'")
                If stock.Count = 0 Then
                    Return
                End If
                ChartBar.Series("Series1").LegendText = CbDateGeneral.Text
                ChartBar.Series("Series2").LegendText = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPChart.Value + "'"))(0))("date")
            Else
                ChartBar.Series("Series1").LegendText = ticker
            End If

            ScoreMetricGrid(id_fga)
        End Sub

        Private Sub CbValeur2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbValeur2.SelectedValueChanged

            Dim stock As List(Of Dictionary(Of String, Object))
            stock = co.sqlToListDico("SELECT distinct DATE AS 'date' FROM DATA_FACTSET WHERE date<'" + DTPChart.Value + "'")
            If CbValeur2.Text = "" Or stock.Count = 0 Then
                Return
            End If

            Dim dateMax As String = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPChart.Value + "'"))(0))("date")

            Dim id_fga As Integer =
            co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_ALL", 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault()
            Dim ticker As String = CbValeur2.Text.Substring(CbValeur2.Text.IndexOf("|") + 2)

            'Construction du radar Growth
            Dim growth2 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {dateMax, "Growth", ticker, id_fga.ToString, ""}))
            GrowthRadar.Series("Series2").Points.DataBindXY(growth2.Keys, growth2.Values)
            Dim growth3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Growth", ticker, id_fga.ToString, "AVG"}))
            GrowthRadar.Series("Series3").Points.DataBindXY(growth3.Keys, growth3.Values)

            'Construction du radar Value
            Dim value2 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {dateMax, "Value", ticker, id_fga.ToString, ""}))
            ValueRadar.Series("Series2").Points.DataBindXY(value2.Keys, value2.Values)
            Dim value3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Value", ticker, id_fga.ToString, "AVG"}))
            ValueRadar.Series("Series3").Points.DataBindXY(value3.Keys, value3.Values)

            'Construction du radar Qualite
            Dim qualite2 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {dateMax, "Qualite", ticker, id_fga.ToString, ""}))
            QualiteRadar.Series("Series2").Points.DataBindXY(qualite2.Keys, qualite2.Values)
            Dim qualite3 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {CbDateGeneral.Text, "Qualite", ticker, id_fga.ToString, "AVG"}))
            QualiteRadar.Series("Series3").Points.DataBindXY(qualite3.Keys, qualite3.Values)

            Dim total2 As Dictionary(Of Object, Object) = co.ProcedureStockéeDico("ACT_Radar_Growth", New List(Of String)(New String() {"@date", "@Pres", "@TICKER", "@FGA", "@MM"}), New List(Of Object)(New Object() {dateMax, "Total", ticker, id_fga.ToString, ""}))
            ChartBar.Series("Series2").Points.DataBindXY(total2.Keys, total2.Values)
            If CbValeur.Text = CbValeur2.Text Then
                ChartBar.Series("Series1").LegendText = CbDateGeneral.Text
                ChartBar.Series("Series2").LegendText = dateMax
            Else
                ChartBar.Series("Series2").LegendText = ticker
            End If

            ScoreMetricGrid(id_fga)
        End Sub

#End Region

#Region "ScoreChange"

        Sub WeeklyCheck(ByVal test As Integer)
            Dim stock As List(Of Dictionary(Of String, Object))
            Dim dateMax As String = CbDateGeneral.Text
            Dim dateOld As String = CbDateGeneral.Text

            stock = co.sqlToListDico("SELECT distinct DATE AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPcheck1.Value + "'")
            If stock.Count > 0 Then
                dateMax = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPcheck1.Value + "'"))(0))("date")
                stock.Clear()
            End If
            stock = co.sqlToListDico("SELECT distinct DATE AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPcheck2.Value + "'")
            If stock.Count > 0 Then
                dateOld = ((co.sqlToListDico("SELECT MAX(DATE) AS 'date' FROM DATA_FACTSET WHERE date<='" + DTPcheck2.Value + "'"))(0))("date")
                stock.Clear()
            End If

            Dim id_fga As Integer = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_ALL", 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault()
            Dim id_sec As Integer = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"GICS", 0, CbSuperSector.Text}.ToList, , ).FirstOrDefault()
            Dim period As Double = Math.Abs((DTPcheck2.Value - DTPcheck1.Value).Days)

            If test = 0 Then
                Dim stocks As List(Of Dictionary(Of String, Object))
                stocks = co.sqlToListDico("SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.DATE AS 'DATE1', fac.GARPN_RANKING_S AS 'Rank1'," _
                                            + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.DATE AS 'DATE2', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                            + " FROM DATA_FACTSET fac" _
                                            + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                            + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                            + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                            + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                            + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                            + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                            + " AND fac.GARPN_RANKING_S <> fac2.GARPN_RANKING_S AND fgafac.GICS_SECTOR is null" _
                                            + " ORDER BY INDUSTRY_FGA, COMPANY_NAME")
                If stocks.Count > 0 Then
                    Dim result = MessageBox.Show("Bonjour." + vbCrLf + vbCrLf + "Il y a des changements dans les scores des valeurs" + vbCrLf + "Voulez-vous les imprimer?", "Daily ScoreChange", MessageBoxButtons.YesNo)
                    Dim saveFileDialog1 As New SaveFileDialog()
                    If result = DialogResult.Yes Then
                        saveFileDialog1.ShowDialog()
                        If saveFileDialog1.FileName = "" Then
                            MsgBox("Enter Filename to create PDF")
                            Exit Sub
                        Else
                            ExportDataToPDFTableScoreChange(saveFileDialog1, dateMax, dateOld)
                        End If
                    End If
                End If
                stocks.Clear()
            End If

            If period <= 3 Then
                DScoreChange.DataSource = co.LoadDataGridByString("SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.DATE AS 'DATE1', fac.GARPN_RANKING_S AS 'Rank1'," _
                                        + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.DATE AS 'DATE2', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                        + " FROM DATA_FACTSET fac" _
                                        + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                        + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                        + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                        + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                        + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                        + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                        + " AND fac.GARPN_RANKING_S <> fac2.GARPN_RANKING_S AND fgafac.GICS_SECTOR is null" _
                                        + " ORDER BY INDUSTRY_FGA, COMPANY_NAME")
            ElseIf CbSuperSector.Text = "" Then
                DScoreChange.DataSource = co.LoadDataGridByString("SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.DATE AS 'DATE1', fac.GARPN_RANKING_S AS 'Rank1'," _
                                    + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.DATE AS 'DATE2', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                    + " FROM DATA_FACTSET fac" _
                                    + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                    + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                    + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                    + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                    + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                    + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                    + " AND ABS(fac.GARPN_RANKING_S-fac2.GARPN_RANKING_S) >= 5 AND fgafac.GICS_SECTOR is null" _
                                    + " AND ((fac.GARPN_QUINTILE_S < 3 AND fac2.GARPN_QUINTILE_S >= 3) OR (fac.GARPN_QUINTILE_S >= 3 AND fac2.GARPN_QUINTILE_S < 3))" _
                                    + " ORDER BY INDUSTRY_FGA, COMPANY_NAME")
            ElseIf CbSectorFGA.Text = "" Then
                DScoreChange.DataSource = co.LoadDataGridByString("SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.DATE AS 'DATE1', fac.GARPN_RANKING_S AS 'Rank1'," _
                                    + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.DATE AS 'DATE2', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                    + " FROM DATA_FACTSET fac" _
                                    + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                    + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                    + " INNER JOIN ref_security.SECTOR ss on ss.id=sec.id_parent" _
                                    + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                    + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                    + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                    + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                    + " AND ABS(fac.GARPN_RANKING_S-fac2.GARPN_RANKING_S) >= 5 AND ss.code='" + id_sec.ToString + "' AND fgafac.GICS_SECTOR is null" _
                                    + " AND ((fac.GARPN_QUINTILE_S < 3 AND fac2.GARPN_QUINTILE_S >= 3) OR (fac.GARPN_QUINTILE_S >= 3 AND fac2.GARPN_QUINTILE_S < 3))" _
                                    + " ORDER BY INDUSTRY_FGA, COMPANY_NAME")
            Else
                DScoreChange.DataSource = co.LoadDataGridByString("SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.DATE AS 'DATE1', fac.GARPN_RANKING_S AS 'Rank1'," _
                                    + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.DATE AS 'DATE2', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                    + " FROM DATA_FACTSET fac" _
                                    + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                    + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                    + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                    + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                    + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                    + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                    + " AND ABS(fac.GARPN_RANKING_S-fac2.GARPN_RANKING_S) >= 5 AND fga.code='" + id_fga.ToString + "' AND fgafac.GICS_SECTOR is null" _
                                    + " AND ((fac.GARPN_QUINTILE_S < 3 AND fac2.GARPN_QUINTILE_S >= 3) OR (fac.GARPN_QUINTILE_S >= 3 AND fac2.GARPN_QUINTILE_S < 3))" _
                                    + " ORDER BY INDUSTRY_FGA, COMPANY_NAME")
            End If
        End Sub


#End Region

#Region "gestion des sélections"

        ''' <summary>
        ''' gestion des sélections après un tri.
        ''' </summary>
        Private Sub DataGridView_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles DGenerale.MouseUp, DQualite.MouseUp, DCroissance.MouseUp, DValorisation.MouseUp, DMomentum.MouseUp
            'Récupére la grille sélectionnée.
            Dim grid As DataGridView = TryCast(sender, DataGridView)
            If grid Is Nothing Then
                Return
            End If

            'sélection des cellules anciennement sélectionnées avant le tri.
            If grid.HitTest(e.X, e.Y).Type = DataGridViewHitTestType.ColumnHeader And Not selectedCells Is Nothing Then
                'Récupère le nom de la colonne du nom des compagnies.
                Dim columnname As String = getColumnName(grid, New List(Of String)({"Company", "Company Name"}))
                If columnname Is Nothing Then
                    Return
                End If

                grid.SuspendLayout()
                grid.ClearSelection()

                For Each name As String In selectedCells.Keys
                    For Each row As DataGridViewRow In grid.Rows
                        Dim company As String = row.Cells(columnname).Value.ToString

                        'distingue les lignes de valeurs, secteur FGA, supersecteur et d'indice.
                        If company = "* SXXP *" Then
                            Dim fga As String = row.Cells("FGA Sector").Value.ToString
                            Dim super As String = row.Cells("ICB SuperSector").Value.ToString

                            If (name.StartsWith("FGA_") And fga = name.Substring(4)) _
                                Or (name.StartsWith("ICB_") And super = name.Substring(4) And fga = "") _
                                Or (company = name And fga = "" And super = "") Then
                                selectCells(name, row)
                            End If
                        ElseIf company = name Then
                            selectCells(name, row)
                        End If
                    Next
                Next

                grid.ResumeLayout()
            End If
        End Sub

        ''' <summary>
        ''' Met à jour les autres grilles lorsque la sélection actuelle change.
        ''' </summary>
        Private Sub DataGridView_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DGenerale.CurrentCellChanged, DQualite.CurrentCellChanged, DCroissance.CurrentCellChanged, DValorisation.CurrentCellChanged, DMomentum.CurrentCellChanged
            'Evite de rappeler la mise à jour des cellules des autres grilles si un update est déjà en cours.
            If Not updatingSelection Then
                'Récupération de la grille sélectionnée.
                Dim grid As DataGridView = TryCast(sender, DataGridView)
                If grid Is Nothing Then
                    Return
                End If

                InitSelectedCells(grid)
            End If
        End Sub

        ''' <summary>
        ''' Récupère la valeur d'une cellule
        ''' </summary>
        ''' <param name="row">la ligne contenant la cellule</param>
        ''' <param name="col_name">le nom de la colonne</param>
        ''' <param name="prefixed">Si vrai, un prefixe est ajouté aux champs ICB et FGA</param>
        ''' <returns>la valeur de la cellule à la ligne "row" dans la colonne "col_name"</returns>
        ''' <remarks>Les valeurs ICB et FGA sont respectivement préfixées par "ICB_" et "FGA_" si prefixed est vrai.</remarks>
        Private Function getCellValue(ByVal row As DataGridViewRow, ByVal col_name As String, ByVal prefixed As Boolean) As String
            Dim value As String = row.Cells(col_name).Value.ToString()

            If value = "* SXXP *" Then
                ' Recherche du secteur FGA
                value = row.Cells("FGA Sector").Value.ToString
                If value = "" Then
                    ' Recherche du superSecteur
                    value = row.Cells("ICB SuperSector").Value.ToString
                    If value = "" Then
                        ' Seul l'indice est sélectionné
                        value = "* SXXP *"
                    ElseIf prefixed Then
                        value = "ICB_" + value
                    End If
                ElseIf prefixed Then
                    value = "FGA_" + value
                End If
            End If

            Return value
        End Function

        ''' <summary>
        ''' sélectionne une ligne entière.
        ''' </summary>
        ''' <param name="grid">la grille contenant la ligne.</param>
        Private Sub selectRow(ByVal grid As DataGridView)
            'Récupère le nom de la colonne du nom des compagnies.
            Dim columnname As String = getColumnName(grid, New List(Of String)({"Company", "Company Name"}))
            If columnname Is Nothing Then
                Return
            End If

            grid.SuspendLayout()
            grid.ClearSelection()

            For Each name As String In selectedCells.Keys
                For Each row As DataGridViewRow In grid.Rows
                    Dim company As String = row.Cells(columnname).Value.ToString

                    'distingue les lignes de valeurs, secteur FGA, supersecteur et d'indice.
                    If company = "* SXXP *" Then
                        Dim fga As String = row.Cells("FGA Sector").Value.ToString
                        Dim super As String = row.Cells("ICB SuperSector").Value.ToString

                        If (name.StartsWith("FGA_") And fga = name.Substring(4)) _
                            Or (name.StartsWith("ICB_") And super = name.Substring(4) And fga = "") _
                            Or (company = name And fga = "" And super = "") Then
                            selectCellsInRow(row)
                        End If
                    ElseIf company = name Then
                        selectCellsInRow(row)
                    End If
                Next
            Next

            grid.ResumeLayout()
        End Sub

        ''' <summary>
        ''' Ajoute les lignes sélectionnées dans selectedCells.
        ''' </summary>
        ''' <param name="grid">La grille contenant les cellules sélectionnées.</param>
        ''' <remarks></remarks>
        Private Sub InitSelectedCells(ByVal grid As DataGridView)
            'Permet d'éviter de mettre à jour sur le chargement d'un onglet (bricolage).
            Try
                If grid.SelectedCells(0).RowIndex = 0 And grid.SelectedCells(0).ColumnIndex = 0 Then
                    Return
                End If
            Catch ex As Exception
                'pas de cellule sélectionnée
                Return
            End Try

            'Récupère le nom de la colonne du nom des compagnies.
            Dim columnname As String = getColumnName(grid, New List(Of String)({"Company", "Company Name"}))
            If columnname Is Nothing Then
                Return
            End If

            selectedCells.Clear()

            'Ajoute à selectedCells le nom des entreprises ou secteur contenues dans columnname pour les lignes sélectionnées.
            For Each cell As DataGridViewCell In grid.SelectedCells
                Dim key As String = getCellValue(cell.OwningRow, columnname, True)
                If selectedCells.ContainsKey(key) Then
                    selectedCells.Item(key).Add(cell.ColumnIndex)
                Else
                    selectedCells.Add(key, New List(Of Integer)({cell.ColumnIndex}))
                End If
            Next

            'sélection des lignes des autres grilles.
            updatingSelection = True
            For Each g As DataGridView In grids
                If Not g.Equals(grid) Then
                    selectRow(g)
                End If
            Next
            updatingSelection = False
        End Sub

        ''' <summary>
        ''' Sélectionne les cellules sauvegardées dans selectedCells
        ''' </summary>
        ''' <param name="name">le nom de la cellule correspondant à la clé dans selectedCells</param>
        ''' <param name="row">La ligne de la cellule</param>
        Private Sub selectCells(ByVal name As String, ByVal row As DataGridViewRow)
            For Each col As Integer In selectedCells.Item(name)
                If row.Cells.Count < col Then
                    row.Cells(col).Selected = True
                End If
            Next
        End Sub

        ''' <summary>
        ''' Sélectionne une ligne entière
        ''' </summary>
        ''' <param name="row">La ligne à sélectionner</param>
        Private Sub selectCellsInRow(ByVal row As DataGridViewRow)
            For Each cell As DataGridViewCell In row.Cells
                cell.Selected = True
            Next
        End Sub

        ''' <summary>
        ''' Récupère le nom d'une colonne
        ''' </summary>
        ''' <param name="grid">La grille contenant la colonne</param>
        ''' <param name="names">Les différents noms possibles pour la colonne</param>
        ''' <returns>Le nom correspondant à la colonne si trouvée, sinon Nothing.</returns>
        Private Function getColumnName(ByVal grid As DataGridView, ByVal names As List(Of String)) As String
            For Each name As String In names
                If grid.Columns.Contains(name) Then
                    Return name
                End If
            Next

            Return Nothing
        End Function

#End Region

#Region "Evènements"
        Private Sub Cb_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSuperSector.Enter, CbSectorFGA.Enter, CbIndiceGeneral.Enter, CbDateGeneral.Enter
            refreshOldCBValue()
        End Sub

        Private Sub BChargerGeneral_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BChargerGeneral.Click
            '---------------------------------------------------------------------------------------------------
            'updatingCombobox = True
            If Not updatingCombobox Then
                'Construction grille Valeur/analyse
                ValeurAnalyseBlend()

                'Construction grille valeurs/Reco
                If CbSuperSector.Text <> "" Then
                    fillGridValeurReco()
                    'CbSuperSector.SelectedIndex = 0
                    'CbSuperSector.SelectedIndex = 2
                End If

                'Charge les commentaires du super secteur
                'loadComentary()

                'Sélectionne le supersecteur dans la grille Secteurs/reco
                selectCurrent(DSecteursReco, False)
                selectCurrent(DValeursReco, True)
            End If
            Windows.Forms.Cursor.Current = Cursors.Default

            '---------------------------------------------------------------------------------------------------
            GrowthRadar.Series("Series1").Points.Clear()
            ValueRadar.Series("Series1").Points.Clear()
            QualiteRadar.Series("Series1").Points.Clear()
            ChartBar.Series("Series1").Points.Clear()
            GrowthRadar.Series("Series2").Points.Clear()
            ValueRadar.Series("Series2").Points.Clear()
            QualiteRadar.Series("Series2").Points.Clear()
            ChartBar.Series("Series2").Points.Clear()
            GrowthRadar.Series("Series3").Points.Clear()
            ValueRadar.Series("Series3").Points.Clear()
            QualiteRadar.Series("Series3").Points.Clear()
            ChartBar.Series("Series1").LegendText = ""
            ChartBar.Series("Series2").LegendText = ""
            Dim ds As New DataSet
            ds.Clear()
            DValeursChart.DataSource = ds
            updateCbSectorFGA()
        End Sub
        Private Sub Cb_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSectorFGA.SelectedValueChanged, CbDateGeneral.SelectedValueChanged, CbIndiceGeneral.SelectedValueChanged, CbSuperSector.SelectedValueChanged
            Dim cb As System.Windows.Forms.ComboBox = TryCast(sender, System.Windows.Forms.ComboBox)
            If cb Is Nothing Then
                Return
            End If

            SelectedValueChanged(cb)
        End Sub

        ''' <summary>
        ''' CbSuperSector : binde le bon secteur FGA, met à jour la grille Valeurs/Analyse 
        ''' et charge les commentaires associés au super secteur
        ''' </summary>
        Private Sub SelectedValueChanged(ByVal cb As System.Windows.Forms.ComboBox)
            If oldCBValue.ContainsKey(cb) AndAlso cb.Text = oldCBValue(cb) Then
                ' no change.
                Return
            End If

            If (checkSaveCB()) Then
                updateCB(cb)
            Else
                rollBackOldCBValue()
            End If
            refreshOldCBValue()
        End Sub

        Private Sub updateCB(ByVal cb As System.Windows.Forms.ComboBox)
            If cb Is CbSuperSector Then
                updateCbSuperSector()
            ElseIf cb Is CbSectorFGA Then
                updateCbSectorFGA()
            ElseIf cb Is CbValeur Then
                updateCBValeur()
            End If
        End Sub

        Private Sub updateCbSuperSector()
            'Réinitialise les listbox dépendantes de super secteur.
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            CbValeur.DataSource = Nothing
            TCommentaireNews.Rtf = Nothing
            Dim FGA_classname As String
            If CbIndiceGeneral.Text = "Europe" Then
                FGA_classname = "FGA_EU"
            ElseIf CbIndiceGeneral.Text = "USA" Then
                FGA_classname = "FGA_US"
            Else
                Return
            End If
            Dim sectors As New List(Of String)
            If CbSuperSector.Text <> "**Isin**" Then
                If CbSuperSector.Text <> "**SuperSectors**" And CbSuperSector.Text <> "" Then
                    Dim id_icb As Integer = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"label", "class_name"}.ToList, {CbSuperSector.Text, "GICS", 0}.ToList, , ).FirstOrDefault
                    For Each libelle_FGA In co.ProcedureStockéeList("ACT_SuperSectorvsSectorFGA", New List(Of String)(New String() {"@supersector", "@fga"}), New List(Of Object)(New Object() {id_icb, FGA_classname}))
                        'co.SelectDistinctWhere("ACT_SUPERSECTOR", "id", "libelle", CbSuperSector.Text).FirstOrDefault}))
                        sectors.Add(libelle_FGA)
                    Next
                End If
            End If

            sectors.Sort()
            sectors.Insert(0, "")
            CbSectorFGA.DataSource = sectors

            If sectors.Count = 3 Then
                CbSectorFGA.SelectedIndex = 2
            End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'updatingCombobox = True
            If Not updatingCombobox Then
                'Construction grille Valeur/analyse
                ValeurAnalyseBlend()

                'Construction grille valeurs/Reco
                If CbSuperSector.Text <> "" Then
                    fillGridValeurReco()
                End If

                'Charge les commentaires du super secteur
                'loadComentary()

                'Sélectionne le supersecteur dans la grille Secteurs/reco
                selectCurrent(DSecteursReco, False)
                selectCurrent(DValeursReco, True)
            End If
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Private Sub updateCbSectorFGA()

            Dim valeurs As New List(Of String)
            valeurs.Add("")
            Dim valeurs2 As New List(Of String)
            valeurs2.Add("")

            Dim id_fga As Integer = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"FGA_EU", 0, CbSectorFGA.Text}.ToList, , ).FirstOrDefault
            Dim id_sec As Integer = co.SelectDistinctWheres("ref_security.SECTOR", "code", {"class_name", "level", "label"}.ToList, {"GICS", 0, CbSuperSector.Text}.ToList, , ).FirstOrDefault

            If RadioButton_Chart2.Checked Then
                For Each subIndustry In co.SelectDistinctWheres("DATA_FACTSET", "GICS_SUBINDUSTRY", {"FGA_SECTOR", "DATE"}.ToList, {id_fga, CbDateGeneral.Text}.ToList, , )
                    If subIndustry IsNot DBNull.Value Then
                        For Each valeur In co.SelectDistinctWheres("DATA_FACTSET", "COMPANY_NAME", {"SECTOR", "DATE"}.ToList, {subIndustry, CbDateGeneral.Text}.ToList, , )
                            For Each ticker In co.SelectDistinctWheres("DATA_FACTSET", "TICKER", {"COMPANY_NAME", "DATE"}.ToList, {valeur, CbDateGeneral.Text}.ToList, , )
                                valeurs.Add(valeur + " | " + ticker)
                                valeurs2.Add(valeur + " | " + ticker)
                            Next
                        Next
                    End If
                Next
            Else
                For Each subIndustry In co.SelectDistinctWheres("DATA_FACTSET", "GICS_SUBINDUSTRY", {"GICS_SECTOR", "DATE"}.ToList, {id_sec, CbDateGeneral.Text}.ToList, , )
                    If subIndustry IsNot DBNull.Value Then
                        For Each valeur In co.SelectDistinctWheres("DATA_FACTSET", "COMPANY_NAME", {"SECTOR", "DATE"}.ToList, {subIndustry, CbDateGeneral.Text}.ToList, , )
                            For Each ticker In co.SelectDistinctWheres("DATA_FACTSET", "TICKER", {"COMPANY_NAME", "DATE"}.ToList, {valeur, CbDateGeneral.Text}.ToList, , )
                                valeurs.Add(valeur + " | " + ticker)
                                valeurs2.Add(valeur + " | " + ticker)
                            Next
                        Next
                    End If
                Next
            End If
            valeurs.Sort()
            valeurs2.Sort()
            CbValeur.DataSource = valeurs
            CbValeur2.DataSource = valeurs2

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'updatingCombobox = True
            If Not updatingCombobox Then
                'Mise à jour de la grille Valeurs/analyse
                ValeurAnalyseBlend()

                'Construction grille valeurs/Reco
                If CbSuperSector.Text <> "" Then
                    fillGridValeurReco()
                End If

                'Mise à jour des commentaires
                'loadComentary()

                'Sélectionne le supersecteur dans la grille Secteurs/reco
                selectCurrent(DSecteursReco, False)
                selectCurrent(DValeursReco, True)
            End If
        End Sub

        Private Sub updateCBValeur()
            'loadComentary()
            selectCurrent(DValeursReco, True)
        End Sub

        ''' <summary>
        ''' Met à jour les anciennes valeurs des différentes combobox
        ''' </summary>
        Private Sub refreshOldCBValue()
            oldCBValue(CbSuperSector) = CbSuperSector.Text
            oldCBValue(CbSectorFGA) = CbSectorFGA.Text
            oldCBValue(CbValeur) = CbValeur.Text
            oldCBValue(CbIndiceGeneral) = CbIndiceGeneral.Text
            oldCBValue(CbDateGeneral) = CbDateGeneral.Text
        End Sub

        Private Sub rollBackOldCBValue()
            CbSuperSector.Text = oldCBValue(CbSuperSector)
            CbSectorFGA.Text = oldCBValue(CbSectorFGA)
            CbValeur.Text = oldCBValue(CbValeur)
            CbIndiceGeneral.Text = oldCBValue(CbIndiceGeneral)
            CbDateGeneral.Text = oldCBValue(CbDateGeneral)
        End Sub

        Public Function nameColumnDataTable(ByVal dataTable As DataTable) As List(Of String)
            Dim res As List(Of String) = New List(Of String)
            For Each col In dataTable.Columns
                res.Add(col.ToString)
            Next
            Return res
        End Function

        Private Sub BDeleteValeur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            clearGrid(DValeursBlend)
        End Sub

        ''' <summary>
        ''' Evenement sur click simple de souris.
        ''' </summary>
        ''' <remarks>Tri descendant sur la grille pour un clic droit sur un header de colonne</remarks>
        Private Sub DataGridView_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles DGenerale.MouseClick, DCroissance.MouseClick, DQualite.MouseClick, DValorisation.MouseClick, DMomentum.MouseClick,
            DSecteursBlend.MouseClick, DValeursBlend.MouseClick, DSecteursNote.MouseClick, DSecteursReco.MouseClick, DSecteursRecoChange.MouseClick, DValeursReco.MouseClick, DValeursRecoChange.MouseClick
            'Récupération de la grille sélectionnée.
            Dim grid As DataGridView = TryCast(sender, DataGridView)
            If grid Is Nothing Then
                Return
            End If

            Dim info As DataGridView.HitTestInfo = grid.HitTest(e.X, e.Y)

            If e.Button = MouseButtons.Right Then
                If (info.Type = DataGridViewHitTestType.ColumnHeader) Then
                    'Tri descendant sur la colonne lors d'un click droit sur le header.
                    grid.Sort(grid.Columns(grid.HitTest(e.X, e.Y).ColumnIndex),
                              System.ComponentModel.ListSortDirection.Descending)
                ElseIf (info.Type = DataGridViewHitTestType.Cell) Then
                    Dim cell As DataGridViewCell = grid.Rows(info.RowIndex).Cells(info.ColumnIndex)

                    ' gestion multi-sélections
                    If Not My.Computer.Keyboard.CtrlKeyDown Then
                        grid.ClearSelection()
                    End If
                    cell.Selected = True

                    'Menu contextuel sur une cellule
                    If gridmenus.Keys.Contains(grid) Then
                        gridmenus(grid).popContextMenu(cell)
                    End If
                End If
            ElseIf (info.Type = DataGridViewHitTestType.Cell) Then
                Dim checkedCell As DataGridViewCheckBoxCell = TryCast(grid.Rows(info.RowIndex).Cells(info.ColumnIndex), DataGridViewCheckBoxCell)

                If checkedCell IsNot Nothing AndAlso Not checkedCell.ReadOnly Then
                    ' Checked event.
                    If IsDBNull(checkedCell.Value) Then
                        checkedCell.Value = True
                    Else
                        checkedCell.Value = Not checkedCell.Value
                    End If

                    co.Updates("ACT_VALEUR",
                               New List(Of String)({"exclusion"}),
                               New List(Of Object)({IIf(checkedCell.Value, 1, 0)}),
                               New List(Of String)({"ISIN"}),
                               New List(Of Object)({checkedCell.OwningRow.Cells("Isin").Value.ToString}))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Colorie les quintils après tri sur grille.
        ''' </summary>
        Private Sub DValeurBlend_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Color_quintile(DValeursBlend, "Quint Quant", New List(Of String)({"Note"})) ', "liquidity", "Poids", "Company Name", "Ticker"}))
        End Sub

        'Private Sub DValeurNote_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '    Color_quintile(DValeursNote, "Quint Quant", New List(Of String)({"Note Quant"}))
        '    Color_quintile(DValeursNote, "Quint Qual", New List(Of String)({"Note Qual"})) ', "liquidity", "Company Name", "Ticker"}))
        'End Sub

        Private Sub DValeurReco_Sorted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DValeursReco.Sorted
            Color_quintile(DValeursReco, "Quint Quant", New List(Of String)({"Note Quant"}))
            Color_quintile(DValeursReco, "Quint Qual", New List(Of String)({"Note Qual"}))
        End Sub

        ''' <summary>
        ''' Remplit les combobox de secteurs en fonction de la cellule sélectionnée
        ''' </summary>
        ''' <param name="cell">cellule sélectionnée</param>
        Private Sub fillCBSecteurs(ByVal cell As DataGridViewCell)
            'Remplit les différentes combobox en fonction de la catégorie de la cellule sélectionnée.
            Dim grid As DataGridView = cell.DataGridView
            Dim col As DataGridViewColumn = cell.OwningColumn
            Dim row As DataGridViewRow = cell.OwningRow

            'suspend les interactions avec la grille.
            grid.SuspendLayout()

            'Nom des colonnes
            Dim icb_col As String = getColumnName(grid, New List(Of String)({"ICB SuperSector"}))
            Dim fga_col As String = getColumnName(grid, New List(Of String)({"FGA Sector"}))
            Dim val_col As String = getColumnName(grid, New List(Of String)({"Company", "Company Name"}))

            Dim fga As String
            Dim icb As String

            If icb_col IsNot Nothing AndAlso col.HeaderText.Contains(icb_col) Then
                setICBFGAValeur(cell.Value.ToString,
                                "",
                                "")
                updateCbSuperSector()
            ElseIf fga_col IsNot Nothing AndAlso col.HeaderText.Contains(fga_col) Then
                setICBFGAValeur(getCellValue(row, icb_col, False),
                                cell.Value.ToString,
                                "")
                updateCbSectorFGA()
            ElseIf val_col IsNot Nothing AndAlso (col.HeaderText.Contains(val_col) Or col.HeaderText.Contains("Ticker")) Then
                If icb_col Is Nothing Then
                    icb = CbSuperSector.SelectedValue.ToString
                Else
                    icb = getCellValue(row, icb_col, False)
                End If

                If fga_col Is Nothing Then
                    fga = CbSectorFGA.SelectedValue.ToString
                Else
                    fga = getCellValue(row, fga_col, False)
                End If

                If cell.Value.ToString = "* SXXP *" Then
                    AfficheSynthese(getCellValue(row, icb_col, False),
                                    getCellValue(row, fga_col, False))
                Else
                    'nom d'une compagnie
                    setICBFGAValeur(icb,
                                    fga,
                                    row.Cells(val_col).Value.ToString)
                End If
            End If

            grid.ResumeLayout()
        End Sub

        ''' <summary>
        ''' Affiche les supersecteurs ou secteurs.
        ''' </summary>
        Private Sub AfficheSynthese(ByVal icb As String, ByVal fga As String)
            If fga = "" Then
                setICBFGAValeur("**SuperSectors**", "", "")
            Else
                setICBFGAValeur("", "**Sectors**", "")
            End If
        End Sub

        ''' <summary>
        ''' Met à jour les différentes combobox de secteurs et valeur
        ''' </summary>
        ''' <param name="icb">la valeur du champ ICB</param>
        ''' <param name="fga">la valeur du champ FGA</param>
        ''' <param name="valeur">la valeur du champ Valeur</param>
        ''' <remarks>Si une valeur de champ est invalide, la combobox ne mettre pas à jour ce champ.
        ''' Ceci garantie la bonne association des secteurs.</remarks>
        Private Sub setICBFGAValeur(ByVal icb As String, ByVal fga As String, ByVal valeur As String)
            refreshOldCBValue()
            updatingCombobox = True
            CbSuperSector.Text = icb
            CbSectorFGA.Text = fga
            updatingCombobox = False
            CbValeur.Text = valeur
        End Sub

        ''' <summary>
        ''' Sauvegarde les messages des commentaires et des recommandations
        ''' </summary>
        Private Sub BSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSave.Click
            saveAllTextBox()
        End Sub

        Private Sub saveAllTextBox()
            Dim icb As String = CbSuperSector.Text
            Dim fga As String = CbSectorFGA.Text
            Dim val As String = CbValeur.Text

            'saveNews(icb, fga, val)
            saveCommentSecteurReco()
            saveCommentValeurReco()
            commentHasChanged = False
        End Sub

        Private Sub saveAllOldTextBox()
            Dim icb As String = oldCBValue(CbSuperSector)
            Dim fga As String = oldCBValue(CbSectorFGA)
            Dim val As String = oldCBValue(CbValeur)

            'saveNews(icb, fga, val)
            commentHasChanged = False
        End Sub

        Private Sub AfficherLaideMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AfficherLaideMenuItem.Click
            Dim help As New HelpWindowAction()
            help.Show()
        End Sub

        Private Sub TOngletEurope_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles TOngletEurope.Selected
            Dim tab As System.Windows.Forms.TabControl = TryCast(sender, System.Windows.Forms.TabControl)
            If tab Is Nothing Then
                Return
            End If

            If tab.SelectedTab.Equals(TSecteurReco) Then
                FillGridSecteurReco()
            ElseIf tab.SelectedTab.Equals(TValeurReco) Then
                If CbSuperSector.Text <> "" Then
                    fillGridValeurReco()
                End If
            End If
        End Sub

        Private Sub Editbox_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TCommentaireNews.Enter, TCommentSecteursReco.Enter, TCommentSecteursRecoChange.Enter, TCommentValeursReco.Enter, TCommentValeursRecoChange.Enter
            commentHasChanged = True
        End Sub

        Private Sub General_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown, TOngletEurope.KeyDown
            Dim control As Object = FindForm().ActiveControl()
            If e.KeyCode = Keys.Delete Then
                If TypeOf control Is DataGridView Then
                    deleteCellContent(control)
                End If
            ElseIf e.Control Then
                ' Ctrl combinations
                If e.KeyCode = Keys.S Then
                    saveAllTextBox()
                End If
            ElseIf e.KeyCode = Keys.F1 Then
                help.Show()
            ElseIf e.KeyCode = Keys.F5 Then
                refreshGrid()
            End If
        End Sub

        Private Sub CoefficientsSecteursConfigure_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCoefSectorConfig.Click
            Dim window As New Action.Coefficient.BaseActionCoefIndice

            window.ShowDialog()
        End Sub

        Private Sub BValeurNoteConfig_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim window As New Action.Note.BaseActionNote(CbSectorFGA.Text)
            window.ShowDialog()

            If window.HasApply Then
                If CbSectorFGA.Text <> "" AndAlso Not CbSectorFGA.Text.StartsWith("**") Then
                    loadValeurNotes(CbSectorFGA.Text)
                End If
            End If
        End Sub

        Private Sub CoefficientsValeursConfigure_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCoefValeurConfig.Click
            Dim window As New Action.Coefficient.BaseActionCoefSecteur

            window.ShowDialog()
        End Sub

        Private Sub ShowFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowFilesToolStripMenuItem.Click
            If Stoxx Is Nothing Then
                'loadStoxx()
            End If

            Dim windowFile As New BaseActionShowFile(Stoxx)

            ' Assure une interopérabilité entre le winform et le wpf.
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(windowFile)
            windowFile.Show()
        End Sub
#End Region

#Region "menu contextuel"
        ''' <summary>
        ''' Recharge les grilles en fonction de l'onglet actuellement sélectionné
        ''' </summary>
        Private Sub refreshGrid()
            If TOngletEurope.SelectedTab Is TSecteurReco Then
                FillGridSecteurReco()
            ElseIf TOngletEurope.SelectedTab Is TValeurReco Then
                If CbSuperSector.Text <> "" Then
                    fillGridValeurReco()
                End If
            ElseIf TOngletEurope.SelectedTab Is TValeurAnalyse Then
                ValeurAnalyseBlend()
                loadValeurNotes(CbSectorFGA.Text)
            ElseIf TOngletEurope.SelectedTab Is TGenerale _
                    Or TOngletEurope.SelectedTab Is TCroissance _
                    Or TOngletEurope.SelectedTab Is TQualite _
                    Or TOngletEurope.SelectedTab Is TValorisation _
                    Or TOngletEurope.SelectedTab Is TMomentum Then
                'fillGridsGeneral()
            End If
        End Sub

        ''' <summary>
        ''' Supprime le contenu d'une cellule qui n'est pas en ReadOnly.
        ''' </summary>
        Private Sub deleteCellContent(ByVal grid As DataGridView)
            Dim cells As New List(Of DataGridViewCell)

            For Each cell As DataGridViewCell In grid.SelectedCells
                If Not cell.ReadOnly Then
                    cell.Value = DBNull.Value

                    'If grid Is DValeursNote Then
                    '    cells.Add(cell)
                    'End If
                End If
            Next

            If cells.Count > 0 Then
                saveNotes(cells)
            End If
        End Sub

        Private Sub NewsFlowDGVMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            fillCBSecteurs(DGVMenu.getcell)
            TOngletEurope.SelectTab(TNewsFlow)
        End Sub

        Private Sub SecteursRecoDGVMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SecteursRecoDGVMenu.Click
            fillCBSecteurs(DGVMenu.getcell)
            TOngletEurope.SelectTab(TSecteurReco)
        End Sub

        Private Sub ValeursRecoDGVMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValeursRecoDGVMenu.Click
            fillCBSecteurs(DGVMenu.getcell)
            TOngletEurope.SelectTab(TValeurReco)
        End Sub

        Private Sub RefreshRecoMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshRecoMenu.Click
            refreshGrid()
        End Sub

        Private Sub NewSecteurRecoMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewRecoMenu.Click
            If TOngletEurope.SelectedTab Is TSecteurReco Then
                newSecteurReco()
            ElseIf TOngletEurope.SelectedTab Is TValeurReco Then
                NewValeurReco()
            End If
        End Sub

        Private Sub RecoMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles RecoMenu.Opening
            NewRecoMenu.Visible = True
            If TOngletEurope.SelectedTab Is TValeurReco Then
                Dim menu As GridMenu = TryCast(sender, GridMenu)

                If menu IsNot Nothing AndAlso menu.getcell().OwningRow.Cells("Libelle").Value.ToString = "" Then
                    NewRecoMenu.Visible = False
                End If
            End If
        End Sub

        Private Sub BaseDeFichiersDGVMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaseDeFichiersRecoMenu.Click
            Dim menu As GridMenu = CType(sender, ToolStripMenuItem).Owner
            Dim cell = menu.getcell()
            Dim value As String = ""

            Dim icb = getColumnName(cell.DataGridView, New List(Of String)({"ICB SuperSector", "Secteur ICB"}))
            Dim fga = getColumnName(cell.DataGridView, New List(Of String)({"FGA Sector", "Secteur FGA"}))
            Dim val = getColumnName(cell.DataGridView, New List(Of String)({"Company", "Company Name"}))

            If cell.OwningColumn.Name = icb _
                OrElse cell.OwningColumn.Name = fga _
                OrElse cell.OwningColumn.Name = val Then

                value = cell.Value.ToString
            End If

            If (value = "" OrElse value.StartsWith("*")) AndAlso val IsNot Nothing Then
                value = cell.OwningRow.Cells(val).Value.ToString
            End If

            If (value = "" OrElse value.StartsWith("*")) AndAlso fga IsNot Nothing Then
                value = cell.OwningRow.Cells(fga).Value.ToString
            End If

            If (value = "" OrElse value.StartsWith("*")) AndAlso icb IsNot Nothing Then
                value = cell.OwningRow.Cells(icb).Value.ToString
            End If

            If Stoxx Is Nothing Then
                'loadStoxx()
            End If

            Dim windowFile As New BaseActionShowFile(Stoxx, value)

            ' Assure une interopérabilité entre le winform et le wpf.
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(windowFile)
            windowFile.Show()
        End Sub

        Private Sub BaseDesValeursToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaseDesValeursToolStripMenuItem.Click
            Dim window As New CorrectImportStockWindow()

            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
            window.Show()
        End Sub
#End Region

#Region "Others"

        Private Sub ISRToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            BaseActionImportation.UpdateIsr()
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        '' Idea is there but it does not work.
        'Private Sub TickerBBGToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TickerBBGToolStripMenuItem.Click
        '    Windows.Forms.Cursor.Current = Cursors.WaitCursor

        '    ' Ouvre le fichier avec la liste des tickers
        '    Dim app As Interop.Excel.Application = CreateObject("Excel.Application")
        '    Dim filename As String = My.Settings.PATH & "INPUT\ACTION\FACTSET\TickerIsin.xlsm"
        '    Dim book As Interop.Excel.Workbook = app.Workbooks.Open(filename)

        '    For Each window In book.Windows
        '        window.Caption = "Uses WithEvents"
        '    Next

        '    'Add an event handler for the WorkbookBeforeClose Event of the
        '    'Application object.
        '    AddHandler app.WorkbookBeforeClose, AddressOf BeforeBookClose

        '    app.Visible = True
        '    app.UserControl = True
        '    Windows.Forms.Cursor.Current = Cursors.Default
        'End Sub

        'Private Sub BeforeBookClose(ByVal Wb As Interop.Excel.Workbook, ByRef Cancel As Boolean)
        '    'This is called when you choose to close the workbook in Excel.
        '    BaseActionImportation.UpdateTicker()
        'End Sub

        Private Sub MiseAJourToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            MessageBox.Show("Pas encore fonctionnel. Veuillez ouvrir manuellement le fichier suivant :" & _
                            vbCrLf & My.Settings.PATH & "INPUT\ACTION\FACTSET\TickerIsin.xlsm")

            ' Does not work with Bloomberg refresh. Is library's missing ?
            '' Ouvre le fichier avec la liste des tickers
            'Dim app As Interop.Excel.Application = CreateObject("Excel.Application")
            'Dim filename As String = My.Settings.PATH & "INPUT\ACTION\FACTSET\TickerIsin.xlsm"
            'Dim book As Interop.Excel.Workbook = app.Workbooks.Open(filename)

            'app.Visible = True

            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Private Sub ImportationToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            BaseActionImportation.UpdateTicker("\INPUT\ACTION\FACTSET")

            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Private Sub NewScreenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewScreenToolStripMenuItem.Click
            Dim window As New BaseActionConsultation()
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
            window.Show()
        End Sub

#End Region

#Region "Evènements"
        Private Sub BPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPrint.Click
            Dim saveFileDialog1 As New SaveFileDialog()
            saveFileDialog1.ShowDialog()
            If saveFileDialog1.FileName = "" Then
                MsgBox("Enter Filename to create PDF")
                Exit Sub
            Else
                ExportDataToPDFTable(saveFileDialog1)
            End If
        End Sub
        Private Sub ExportDataToPDFTable(ByVal saveFileDialog1 As SaveFileDialog)
            Dim paragraph As New System.Windows.Documents.Paragraph
            Dim doc As New Document(iTextSharp.text.PageSize.A4, 40, 40, 40, 10)
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())
            Try
                Dim wri As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(saveFileDialog1.FileName + ".pdf", FileMode.Create))
                doc.Open()

                Dim font12BoldRed As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font12Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font12Normal As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                Dim font6Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font8Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 8.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font8BoldBlue As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.CYAN)
                Dim font8BoldGreen As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.GREEN)
                Dim font6Normal As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.FontFamily.COURIER, BaseColor.BLACK)

                'Dim reader As PdfReader = New PdfReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("\INPUT\ACTION\FACTSET\template.pdf"))
                'Dim reader As New PdfReader("C:\Users\G13LP00\Desktop\template2.pdf")
                'Dim Background As PdfTemplate = wri.GetImportedPage(reader, 1)

                'Dim pcb As PdfContentByte = wri.DirectContentUnder
                'pcb.AddTemplate(Background, 0, 0)

                'pcb.AddTemplate(Background, doc.PageSize.Width, doc.PageSize.Height)
                Dim p1 As New Phrase
                p1 = New Phrase(New Chunk("Comparison between : " + CbValeur.Text + "  and  " + CbValeur2.Text, font12Bold))
                doc.Add(p1)
                Dim p2 As New Phrase
                p2 = New Phrase(New Chunk("   at " + CbDateGeneral.Text + "  and  " + DTPChart.Value, font12Bold))
                doc.Add(p2)


                '------------------------------------------------------------------------------------
                'Create instance of the pdf table and set the number of column in that table
                Dim PdfTable As New PdfPTable(DValeursChart.ColumnCount)
                PdfTable.TotalWidth = 575.0F
                'fix the absolute width of the table
                PdfTable.LockedWidth = True
                'relative col widths in proportions - 1,4,1,1 and 1
                Dim widths As Single() = New Single() {18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F, 18.0F}

                PdfTable.SetWidths(widths)
                PdfTable.HorizontalAlignment = 1 ' 0 --> Left, 1 --> Center, 2 --> Right
                PdfTable.SpacingBefore = 2.0F

                'pdfCell Decleration
                Dim PdfPCell As PdfPCell = Nothing
                Dim i As Integer = 0
                For Each c As DataGridViewColumn In DValeursChart.Columns
                    'Assigning values to each cell as phrases
                    PdfPCell = New PdfPCell(New Phrase(New Chunk(c.HeaderText, font6Bold)))
                    'Alignment of phrase in the pdfcell
                    PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER
                    'If i = 1 Or i = 2 Or i = 6 Or i = 7 Or i = 11 Or i = 12 Then
                    '    PdfPCell.BackgroundColor = BaseColor.CYAN
                    'ElseIf i = 3 Or i = 4 Or i = 8 Or i = 9 Or i = 13 Or i = 14 Then
                    '    PdfPCell.BackgroundColor = BaseColor.GREEN
                    'End If
                    'Add pdfcell in pdftable
                    PdfTable.AddCell(PdfPCell)
                    i += 1
                Next

                Dim dt As DataTable = GetDataTable()
                If dt IsNot Nothing Then
                    'Now add the data from datatable to pdf table
                    Dim n As Integer = 1
                    For rows As Integer = 0 To dt.Rows.Count - 1
                        For column As Integer = 0 To dt.Columns.Count - 1
                            If column = 0 Or column = 5 Or column = 10 Then
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT
                                If ((dt.Rows).Item(rows)).Item(column) IsNot DBNull.Value AndAlso IsNumeric(((dt.Rows).Item(rows)).Item(column)) = False AndAlso ((dt.Rows).Item(rows)).Item(column) = "ISR" Then
                                    PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8Bold))
                                Else
                                    PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font6Normal))
                                End If
                                'PdfPCell.BackgroundColor = BaseColor.BLUE
                            Else
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT
                                'PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8Bold))
                                If column = 1 Or column = 2 Or column = 6 Or column = 7 Or column = 11 Or column = 12 Then
                                    PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8BoldBlue))
                                    'PdfPCell.BackgroundColor = BaseColor.CYAN
                                Else
                                    PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8BoldGreen))
                                    'PdfPCell.BackgroundColor = BaseColor.GREEN
                                End If
                            End If
                            PdfTable.AddCell(PdfPCell)
                        Next
                        'n += 1
                    Next
                    'Adding pdftable to the pdfdocument
                    PdfTable.WriteSelectedRows(0, -1, doc.PageSize.Width - 595.0F, doc.PageSize.Height - 100.0F, wri.DirectContent)
                    'doc.Add(PdfTable)
                End If

                'doc.Close()

                '------------------------------------------------------------------------------------
                'doc = New Document(iTextSharp.text.PageSize.A4, 40, 40, 40, 10)
                ''doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())
                'Dim reader As PdfReader = New PdfReader(saveFileDialog1.FileName + ".pdf")
                'Dim wri2 As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(saveFileDialog1.FileName + "2.pdf", FileMode.Create))
                'doc.Open()
                'Dim Background As PdfTemplate = wri2.GetImportedPage(reader, 1)
                'Dim pcb As PdfContentByte = wri2.DirectContentUnder
                'pcb.AddTemplate(Background, 0, 0)
                'doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())

                Dim chartimage1 As New MemoryStream()
                Dim chartimage2 As New MemoryStream()
                Dim chartimage3 As New MemoryStream()
                Dim chartimage4 As New MemoryStream()

                ChartBar.SaveImage(chartimage1, ChartImageFormat.Png)
                Dim picture As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(chartimage1.GetBuffer())
                'picture.RotationDegrees = 90.0F
                picture.ScalePercent(70)
                picture.SetAbsolutePosition(doc.PageSize.Width - 810.0F, doc.PageSize.Height - 320.0F)
                doc.Add(picture)
                GrowthRadar.SaveImage(chartimage2, ChartImageFormat.Png)
                picture = iTextSharp.text.Image.GetInstance(chartimage2.GetBuffer())
                'picture.RotationDegrees = 90.0F
                picture.ScalePercent(60)
                picture.SetAbsolutePosition(doc.PageSize.Width - 810.0F, doc.PageSize.Height - 550.0F)
                doc.Add(picture)
                ValueRadar.SaveImage(chartimage3, ChartImageFormat.Png)
                picture = iTextSharp.text.Image.GetInstance(chartimage3.GetBuffer())
                'picture.RotationDegrees = 90.0F
                picture.ScalePercent(60)
                picture.SetAbsolutePosition(doc.PageSize.Width - 530.0F, doc.PageSize.Height - 550.0F)
                doc.Add(picture)
                QualiteRadar.SaveImage(chartimage4, ChartImageFormat.Png)
                picture = iTextSharp.text.Image.GetInstance(chartimage4.GetBuffer())
                'picture.RotationDegrees = 90.0F
                picture.ScalePercent(60)
                picture.SetAbsolutePosition(doc.PageSize.Width - 260.0F, doc.PageSize.Height - 550.0F)
                doc.Add(picture)
                chartimage1.Close()
                chartimage2.Close()
                chartimage3.Close()
                chartimage4.Close()
                doc.Close()

                System.Diagnostics.Process.Start(saveFileDialog1.FileName + ".pdf")
            Catch
                MsgBox("Fichier PDF déjà ouvert! Veuillez le fermer d'abord ou changer de nom")
            End Try
        End Sub
        Private Function GetDataTable() As DataTable
            Dim dataTable As New DataTable("MyDataTable")

            Dim nameList As New List(Of String)
            For Each c As DataGridViewColumn In DValeursChart.Columns
                nameList.Add(c.HeaderText)

                'Create another DataColumn Name
                Dim dataColumn_1 As New DataColumn(c.HeaderText, GetType(String))
                dataTable.Columns.Add(dataColumn_1)
            Next

            'Now Add some row to newly created dataTable
            Dim dataRow As DataRow
            ''For i As Integer = 0 To Grid1.Children.Count - 1
            'dataRow = dataTable.NewRow()
            Dim RowGrid1 As RowDefinition = New RowDefinition()
            Dim d As Double
            Dim format As String = "# ##0.00;(##0.00)"
            For Each rowDef As DataGridViewRow In DValeursChart.Rows
                dataRow = dataTable.NewRow()
                For Each c As DataGridViewTextBoxCell In rowDef.Cells
                    If IsNumeric(c.Value) Then
                        d = c.Value
                        dataRow(c.ColumnIndex) = d.ToString(format)
                    Else
                        dataRow(c.ColumnIndex) = c.Value
                    End If
                Next
                dataTable.Rows.Add(dataRow)
            Next

            dataTable.AcceptChanges()
            Return dataTable
        End Function

        Private Sub ExportDataToPDFTableScoreChange(ByVal saveFileDialog1 As SaveFileDialog, ByVal dateMax As String, ByVal dateOld As String)
            Dim paragraph As New System.Windows.Documents.Paragraph
            Dim doc As New Document(iTextSharp.text.PageSize.A4, 40, 40, 40, 10)
            'doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())
            Try
                Dim wri As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(saveFileDialog1.FileName + ".pdf", FileMode.Create))
                doc.Open()

                Dim font12Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font12BoldBlue As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9.0F, iTextSharp.text.Font.BOLD, BaseColor.CYAN)
                Dim font12BoldGreen As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9.0F, iTextSharp.text.Font.BOLD, BaseColor.GREEN)
                Dim font12Normal As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                Dim font6Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font8Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 8.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font8BoldBlue As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.CYAN)
                Dim font8BoldGreen As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.BOLD, BaseColor.GREEN)
                Dim font6Normal As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 6.0F, iTextSharp.text.Font.FontFamily.COURIER, BaseColor.BLACK)

                Dim p1 As New Phrase
                p1 = New Phrase(New Chunk("ScoreChange between  ", font12Bold))
                doc.Add(p1)
                p1 = New Phrase(New Chunk(CbDateGeneral.Text, font12BoldBlue))
                doc.Add(p1)
                p1 = New Phrase(New Chunk("  and  ", font12Bold))
                doc.Add(p1)
                p1 = New Phrase(New Chunk(dateOld, font12BoldGreen))
                doc.Add(p1)

                '------------------------------------------------------------------------------------
                'Create instance of the pdf table and set the number of column in that table
                Dim PdfTable As New PdfPTable(8)
                PdfTable.TotalWidth = 450.0F
                'fix the absolute width of the table
                PdfTable.LockedWidth = True
                'relative col widths in proportions - 1,4,1,1 and 1
                Dim widths As Single() = New Single() {30.0F, 8.0F, 15.0F, 60.0F, 8.0F, 8.0F, 8.0F, 8.0F}

                PdfTable.SetWidths(widths)
                PdfTable.HorizontalAlignment = 1 ' 0 --> Left, 1 --> Center, 2 --> Right
                PdfTable.SpacingBefore = 2.0F

                'pdfCell Decleration
                Dim PdfPCell As PdfPCell = Nothing
                Dim dt As New DataTable
                Dim index As Integer = 0
                Dim sql As String = "SELECT distinct fga.label AS INDUSTRY_FGA, fgafac.SUIVI, fac.TICKER, fac.COMPANY_NAME, fac.GARPN_RANKING_S AS 'Rank1'," _
                                            + " fac.GARPN_QUINTILE_S AS 'Quint1', fac2.GARPN_RANKING_S AS 'Rank2', fac2.GARPN_QUINTILE_S AS 'Quint2'" _
                                            + " FROM DATA_FACTSET fac" _
                                            + " INNER JOIN DATA_FACTSET fac2 ON fac2.ISIN = fac.ISIN" _
                                            + " INNER JOIN ref_security.SECTOR sec ON sec.code=fac.SECTOR" _
                                            + " INNER JOIN ref_security.SECTOR_TRANSCO tr on tr.id_sector1=sec.id" _
                                            + " INNER JOIN ref_security.SECTOR fga on fga.id=tr.id_sector2" _
                                            + " INNER JOIN DATA_FACTSET fgafac on fgafac.FGA_SECTOR=fga.code AND fgafac.DATE=fac.DATE" _
                                            + " WHERE fac.DATE = '" + dateMax + "' AND fac2.DATE = '" + dateOld + "' AND fgafac.GICS_SECTOR is null" _
                                            + " AND fac.GARPN_RANKING_S <> fac2.GARPN_RANKING_S AND fgafac.GICS_SECTOR is null" _
                                            + " ORDER BY INDUSTRY_FGA, COMPANY_NAME"
                Dim myCommand As New SqlCommand(sql, Connection.coBase)
                Dim da As SqlDataAdapter = New SqlDataAdapter(myCommand)
                da.Fill(dt)

                For Each c As DataColumn In dt.Columns
                    'Assigning values to each cell as phrases
                    PdfPCell = New PdfPCell(New Phrase(New Chunk(c.ColumnName, font6Bold)))
                    'Alignment of phrase in the pdfcell
                    PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER
                    PdfTable.AddCell(PdfPCell)
                Next

                If dt IsNot Nothing Then
                    'Now add the data from datatable to pdf table
                    For rows As Integer = 0 To dt.Rows.Count - 1
                        For column As Integer = 0 To dt.Columns.Count - 1
                            If column = 4 Or column = 5 Then
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT
                                PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8BoldBlue))
                            ElseIf column = 6 Or column = 7 Then
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT
                                PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font8BoldGreen))
                            Else
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT
                                PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font6Normal))
                            End If
                            PdfTable.AddCell(PdfPCell)
                        Next
                        'n += 1
                    Next
                    'Adding pdftable to the pdfdocument
                    'PdfTable.WriteSelectedRows(0, -1, doc.PageSize.Width - 595.0F, doc.PageSize.Height - 100.0F, wri.DirectContent)
                    doc.Add(PdfTable)
                End If

                doc.Close()

                System.Diagnostics.Process.Start(saveFileDialog1.FileName + ".pdf")
            Catch
                MsgBox("Fichier PDF déjà ouvert! Veuillez le fermer d'abord ou changer de nom")
            End Try
        End Sub

        Private Sub ChartBar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChartBar.DoubleClick,
                                                                                            GrowthRadar.DoubleClick, ValueRadar.DoubleClick, QualiteRadar.DoubleClick
            Dim chartimage As New MemoryStream()
            sender.SaveImage(chartimage, ChartImageFormat.Bmp)
            Dim bm As Bitmap = New Bitmap(chartimage)
            Clipboard.SetImage(bm)
        End Sub

        Private valeurSize As Size
        Private chartSize As Size
        Private VCSize As Size
        Private Sub setSize()
            valeurSize.Height = Me.DValeursBlend.Size.Height
            valeurSize.Width = Me.DValeursBlend.Size.Width
            chartSize.Height = Me.GroupBoxRadar.Size.Height
            chartSize.Width = Me.GroupBoxRadar.Size.Width
            VCSize = valeurSize
            Me.DValeursChart.MaximumSize = VCSize
            Me.DValeursChart.MinimumSize = VCSize
            Me.DValeursChart.Size = VCSize
        End Sub

        Private Sub Label11_Click() Handles Label11.Click
            Dim size As Size
            If Me.GroupBoxRadar.Visible Then
                Me.GroupBoxRadar.Visible = False
                If Not Me.DValeursBlend.Visible AndAlso Me.DValeursChart.Visible Then
                ElseIf Me.DValeursBlend.Visible AndAlso Not Me.DValeursChart.Visible Then
                    size.Height = Me.LValeur.Size.Height - 150
                    size.Width = Me.DValeursBlend.Size.Width
                    Me.DValeursBlend.MaximumSize = size
                    Me.DValeursBlend.MinimumSize = size
                    Me.DValeursBlend.Size = size
                End If
            Else
                Me.GroupBoxRadar.Visible = True
                size.Height = 0
                size.Width = 0
                Me.DValeursChart.MaximumSize = VCSize
                Me.DValeursChart.MinimumSize = VCSize
                Me.DValeursChart.Size = VCSize
                Me.DValeursBlend.MaximumSize = size
                Me.DValeursBlend.MinimumSize = size
                Me.DValeursBlend.Size = valeurSize
            End If
            Me.LValeur.Update()
        End Sub

        Private Sub Label12_Click() Handles Label12.Click
            Dim size As Size
            If Me.DValeursBlend.Visible Then
                Me.DValeursBlend.Visible = False
                If Not Me.GroupBoxRadar.Visible AndAlso Me.DValeursChart.Visible Then
                End If
            Else
                Me.DValeursBlend.Visible = True
                If Not Me.GroupBoxRadar.Visible AndAlso Not Me.DValeursChart.Visible Then
                    size.Height = Me.LValeur.Size.Height - 150
                    size.Width = Me.DValeursBlend.Size.Width
                    Me.DValeursBlend.MaximumSize = size
                    Me.DValeursBlend.MinimumSize = size
                    Me.DValeursBlend.Size = size
                Else
                    size.Height = 0
                    size.Width = 0
                    Me.DValeursChart.MaximumSize = VCSize
                    Me.DValeursChart.MinimumSize = VCSize
                    Me.DValeursChart.Size = VCSize
                    Me.DValeursBlend.MaximumSize = size
                    Me.DValeursBlend.MinimumSize = size
                    Me.DValeursBlend.Size = valeurSize
                End If
            End If
            Me.LValeur.Update()
        End Sub

        Private Sub Label15_Click() Handles Label15.Click
            Dim size As Size
            If Me.DValeursChart.Visible Then
                Me.DValeursChart.Visible = False
                If Not Me.GroupBoxRadar.Visible AndAlso Me.DValeursBlend.Visible Then
                    size.Height = Me.LValeur.Size.Height - 150
                    size.Width = Me.DValeursBlend.Size.Width
                    Me.DValeursBlend.MaximumSize = size
                    Me.DValeursBlend.MinimumSize = size
                    Me.DValeursBlend.Size = size
                End If
            Else
                Me.DValeursChart.Visible = True
                If Not Me.GroupBoxRadar.Visible AndAlso Not Me.DValeursBlend.Visible Then
                Else
                    size.Height = 0
                    size.Width = 0
                    Me.DValeursChart.MaximumSize = VCSize
                    Me.DValeursChart.MinimumSize = VCSize
                    Me.DValeursChart.Size = VCSize
                    Me.DValeursBlend.MaximumSize = size
                    Me.DValeursBlend.MinimumSize = size
                    Me.DValeursBlend.Size = valeurSize
                End If
            End If
            Me.LValeur.Update()
        End Sub

#End Region

        Private Sub ScoreChangesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScoreChangesToolStripMenuItem.Click
            WeeklyCheck(0)
        End Sub

        Private Sub ScoreChangesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
            WeeklyCheck(1)
        End Sub

    End Class
End Namespace
