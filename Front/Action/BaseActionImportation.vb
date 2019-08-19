Imports System.IO

Namespace Action

    Public Class BaseActionImportation

        Dim co As Connection = New Connection()
        Dim fichier As Fichier = New Fichier()
        Dim da As DGrid = New DGrid()
        Dim lo As Log = New Log()
        Dim excel As Excel = New Excel

        Public datee As DateTime

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub BaseActionImportation_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            BAjouter.Enabled = False

            'Tentative de connnection
            co.ToConnectBase()

            Rafraichir()
        End Sub

        ''' <summary>
        ''' Rafraichir : binde les composants graphiques
        ''' </summary>
        Private Sub Rafraichir()
            'Binder les combobox avec les bonnes dates 
            'Dim factsetDate As List(Of Object) = co.SelectDistinctSimple("DATA_FACTSET", "date", "DESC")
            '' '' '' ''CbDateCoef.DataSource = co.SelectDistinctSimple("ACT_DATA_FACTSET_COEF", "date", "DESC")
            CbDateLiquidity.DataSource = co.SelectDistinctSimple("DATA_FACTSET", "date", "DESC")
            CbVider.DataSource = co.SelectDistinctSimple("DATA_FACTSET", "date", "DESC")
            '' '' '' ''CbDateIsr.DataSource = co.SelectDistinctSimple("ISR_NOTE", "date", "DESC")
            DIsr.DefaultCellStyle.Format = "n2"
        End Sub

        ''' <summary>
        ''' CbFactSet_Click : ajout les fichiers dans le combobox
        ''' </summary>
        Private Sub CbFactSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbFichierFactSet.Click
            'CbFichierFacSet.Items.Clear()
            Dim mesFichiers As List(Of String) = New List(Of String)()
            For Each fichiers In Directory.GetFiles(My.Settings.PATH & "\INPUT\ACTION\FACTSET\DATA", "*.xls", SearchOption.AllDirectories)
                mesFichiers.Add(New IO.FileInfo(fichiers).Name())
            Next
            mesFichiers.Sort(New myReverserClass)
            CbFichierFactSet.DataSource = mesFichiers
        End Sub

        ''' <summary>
        ''' CbFactSet_SelectedValueChanged : check si la base est pleine 
        ''' </summary>
        Private Sub CbFactSet_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbFichierFactSet.SelectedValueChanged
            BAjouter.Enabled = False
            datee = excel.CellFichierExcel(My.Settings.PATH & "\INPUT\ACTION\FACTSET\DATA", CbFichierFactSet.Text, 1, 10, 1)
            TDate.Text = datee
            If co.SelectDistinctWhere("DATA_FACTSET", "date", "Date", datee).Count = 0 Then
                BAjouter.Enabled = True
            End If
        End Sub

        ''' <summary>
        ''' CbVider_Click : supprimer les données de ACT_DATA_FACSET, ACT_DATA_FACSET_COEF
        ''' </summary>
        Private Sub BVider_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BVider.Click
            'If CbVider.Text <> "" Then
            '    Dim a As Integer = MessageBox.Show("Voulez-vous supprimer les données dans DATA_FACTSET pour la date " & CbVider.Text & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            '    If a = 6 Then
            '        'co.DeleteWhere("ref_security.SECTOR_TRANSCO", "date", CbVider.Text)
            '        co.DeleteWhere("ref_security.ASSET_TO_SECTOR", "date", CbVider.Text)
            '        co.DeleteWhere("ref_security.ASSET", "MaturityDate", CbVider.Text)
            '        'co.DeleteWhere("ref_common.IDENTIFICATION", "MaturityDate", CbVider.Text)
            '        'co.DeleteWhere("ref_security.SECTOR", "date", CbVider.Text)
            '        co.DeleteWhere("DATA_FACTSET", "date", CbVider.Text)
            '        'co.DeleteWhere("ACT_DATA_FACTSET_AGR", "date", CbVider.Text)
            '        'co.commandeSqlnoInsert("DBCC CHECKIDENT('ref_common.IDENTIFICATION', RESEED, 0)")
            '        co.commandeSqlnoInsert("DBCC CHECKIDENT('ref_security.ASSET', RESEED, 0)")
            '        If co.SelectDistinctWhere("DATA_FACTSET", "date", "Date", datee).Count = 0 Then
            '            BAjouter.Enabled = True
            '        End If
            '    Else
            '        Exit Sub
            '    End If
            'End If
        End Sub

        ''' <summary>
        ''' BAjouter_Click : ajout les fichiers dans le combobox
        ''' </summary>
        Private Sub BAjouter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjouter.Click

            If IsNumeric(TWinsorized.Text) Then
                Windows.Forms.Cursor.Current = Cursors.WaitCursor

                'Mise à jour des coef des indicateurs
                'If co.SelectDistinctWhere("ACT_DATA_FACTSET_COEF", "date", "date", datee).Count > 0 Then
                '    co.DeleteWhere("ACT_DATA_FACTSET_COEF", "date", datee)
                'End If

                'Dim names As New List(Of String)(New String() {"PORTFOLIO", "DATE", "INDICATOR", "FINANCE", "OTHERS"})
                'For i = 0 To DataGridCoefGrowth.Rows.Count - 1
                '    Dim donnees As New List(Of Object)(New Object() {"Growth", datee, _
                '                                    If(IsDBNull(DataGridCoefGrowth.Rows(i).Cells(0)), 0, DataGridCoefGrowth.Rows(i).Cells(0).Value), _
                '                                    If(IsDBNull(DataGridCoefGrowth.Rows(i).Cells(1)) Or IsNumeric(DataGridCoefGrowth.Rows(i).Cells(1).Value) = False, 0, DataGridCoefGrowth.Rows(i).Cells(1).Value), _
                '                                    If(IsDBNull(DataGridCoefGrowth.Rows(i).Cells(2)) Or IsNumeric(DataGridCoefGrowth.Rows(i).Cells(2).Value) = False, 0, DataGridCoefGrowth.Rows(i).Cells(2).Value)})
                '    co.Insert("ACT_DATA_FACTSET_COEF", names, donnees)
                'Next i
                'For i = 0 To DataGridCoefValue.Rows.Count - 1
                '    Dim donnees As New List(Of Object)(New Object() {"Value", datee, _
                '                                    If(IsDBNull(DataGridCoefValue.Rows(i).Cells(0)), 0, DataGridCoefValue.Rows(i).Cells(0).Value), _
                '                                    If(IsDBNull(DataGridCoefValue.Rows(i).Cells(1)) Or IsNumeric(DataGridCoefValue.Rows(i).Cells(1).Value) = False, 0, DataGridCoefValue.Rows(i).Cells(1).Value), _
                '                                    If(IsDBNull(DataGridCoefValue.Rows(i).Cells(2)) Or IsNumeric(DataGridCoefValue.Rows(i).Cells(2).Value) = False, 0, DataGridCoefValue.Rows(i).Cells(2).Value)})
                '    co.Insert("ACT_DATA_FACTSET_COEF", names, donnees)
                'Next i
                'For i = 0 To DataGridCoefGarpO.Rows.Count - 1
                'Dim donnees As New List(Of Object)(New Object() {"Garp Old", datee, _
                '                               If(IsDBNull(DataGridCoefGarpO.Rows(i).Cells(0)), 0, DataGridCoefGarpO.Rows(i).Cells(0).Value), _
                '                              If(IsDBNull(DataGridCoefGarpO.Rows(i).Cells(1)) Or IsNumeric(DataGridCoefGarpO.Rows(i).Cells(1).Value) = False, 0, DataGridCoefGarpO.Rows(i).Cells(1).Value), _
                '                             If(IsDBNull(DataGridCoefGarpO.Rows(i).Cells(2)) Or IsNumeric(DataGridCoefGarpO.Rows(i).Cells(2).Value) = False, 0, DataGridCoefGarpO.Rows(i).Cells(2).Value)})
                'co.Insert("ACT_DATA_FACTSET_COEF", names, donnees)
                'Next i

                'names.Add("BANK") 'TODO: Ajouter BANK aux coefs précédents.
                'For i = 0 To DataGridCoefBlendValeur.Rows.Count - 1
                '    Dim donnees As New List(Of Object)(New Object() {"BlendValeur", datee, _
                '                                    If(IsDBNull(DataGridCoefBlendValeur.Rows(i).Cells(0)), 0, DataGridCoefBlendValeur.Rows(i).Cells(0).Value), _
                '                                    If(IsDBNull(DataGridCoefBlendValeur.Rows(i).Cells(1)) Or IsNumeric(DataGridCoefBlendValeur.Rows(i).Cells(1).Value) = False, 0, DataGridCoefBlendValeur.Rows(i).Cells(1).Value), _
                '                                    If(IsDBNull(DataGridCoefBlendValeur.Rows(i).Cells(2)) Or IsNumeric(DataGridCoefBlendValeur.Rows(i).Cells(2).Value) = False, 0, DataGridCoefBlendValeur.Rows(i).Cells(2).Value), _
                '                                    If(IsDBNull(DataGridCoefBlendValeur.Rows(i).Cells(3)) Or IsNumeric(DataGridCoefBlendValeur.Rows(i).Cells(3).Value) = False, 0, DataGridCoefBlendValeur.Rows(i).Cells(3).Value)})
                '    co.Insert("ACT_DATA_FACTSET_COEF", names, donnees)
                'Next i

                'Dim names2 As New List(Of String)(New String() {"PORTFOLIO", "DATE", "INDICATOR", "OTHERS"})
                'For i = 0 To DataGridCoefBlendSecteur.Rows.Count - 1
                '    Dim donnees As New List(Of Object)(New Object() {"BlendSecteur", datee, _
                '                                    If(IsDBNull(DataGridCoefBlendSecteur.Rows(i).Cells(0)), 0, DataGridCoefBlendSecteur.Rows(i).Cells(0).Value), _
                '                                    If(IsDBNull(DataGridCoefBlendSecteur.Rows(i).Cells(1)) Or IsNumeric(DataGridCoefBlendSecteur.Rows(i).Cells(1).Value) = False, 0, DataGridCoefBlendSecteur.Rows(i).Cells(1).Value)})
                '    co.Insert("ACT_DATA_FACTSET_COEF", names2, donnees)
                'Next i



                ''Ajout des notes ISR
                'If co.SelectDistinctWhere("ISR_NOTE", "date", "date", datee).Count > 0 Then
                'co.DeleteWhere("ISR_NOTE", "date", datee)
                'End If
                'Dim nameISR As New List(Of String)(New String() {"date", "isin", "sedol", "name", "europe", "euro", "ExEuro"})
                'For i = 0 To DIsr.Rows.Count - 1
                'Dim donnees As New List(Of Object)(New Object() {datee, DIsr.Rows(i).Cells(0).Value, DIsr.Rows(i).Cells(1).Value, DIsr.Rows(i).Cells(2).Value, DIsr.Rows(i).Cells(3).Value, DIsr.Rows(i).Cells(4).Value, DIsr.Rows(i).Cells(5).Value})
                'co.Insert("ISR_NOTE", nameISR, donnees)
                'Next i
                ' '' '' '' '' '' '' '' '' ''        [TestMethod]()
                ' '' '' '' '' '' '' '' '' ''public void LumenCSVReadValuesAndInsertIntoDest()
                ' '' '' '' '' '' '' '' '' ''{

                ' '' '' '' '' '' '' '' '' ''    TestContext.BeginTimer("LumenCSVReadValuesAndInsertIntoDest");

                ' '' '' '' '' '' '' '' '' ''    DataSet ds = new DataSet("ACT_PTF");
                ' '' '' '' '' '' '' '' '' ''    DBConnectionDelegate dest = new MSSQL2005_DBConnection(@"Data Source=FX027471M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60");
                ' '' '' '' '' '' '' '' '' ''    using (var reader = new CsvReader(new StreamReader(@"base_test_20131120.csv"), true, ';'))
                ' '' '' '' '' '' '' '' '' ''    {
                ' '' '' '' '' '' '' '' '' ''        CsvDataAdapter adapter = new CsvDataAdapter(reader);
                ' '' '' '' '' '' '' '' '' ''        adapter.Fill(dest, "dbo", "#ACT_PTF");

                ' '' '' '' '' '' '' '' '' ''        dest.Insert("dbo", "ACT_PTF", "dbo", "#ACT_PTF");
                ' '' '' '' '' '' '' '' '' ''    }
                ' '' '' '' '' '' '' '' '' ''    TestContext.EndTimer("LumenCSVReadValuesAndInsertIntoDest");
                ' '' '' '' '' '' '' '' '' ''}
                'Insertion des secteurs
                excel.ExcelToSqlForSecteur(datee, My.Settings.PATH & "\INPUT\ACTION\FACTSET", "Modele_Classification.xlsx", 1, "DATA_FACTSET")

                '' '' '' ''checkForMissingSectors(datee.ToShortDateString)
                ' + transco

                'Ajout des equities
                'Cas particulier pour tester le portefeuille
                If CbFichierFactSet.Text.Contains("value") Or CbFichierFactSet.Text.Contains("growth") Then
                    excel.ExcelToSql(My.Settings.PATH & "\INPUT\ACTION\FACTSET\DATA", CbFichierFactSet.Text, 1, "DATA_FACTSET")
                    'ElseIf CbFichierFacSet.Text = "factset 2011-11-18 quater NEW.xls" Or CbFichierFacSet.Text = "factset 2011-12-30.xls" Then
                    '   excel.ExcelToSql(My.Settings.PATH & "\INPUT\ACTION\FACTSET\DATA", CbFichierFacSet.Text, 1, "DATA_FACTSET", 5, , 2)
                Else
                    excel.ExcelToSql(My.Settings.PATH & "\INPUT\ACTION\FACTSET\DATA", CbFichierFactSet.Text, 1, "DATA_FACTSET", 2, , 2)
                End If

                '' '' '' ''co.ProcedureStockée("ACT_TickerConvert", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {datee}))

                '' '' '' ''checkForMissingEquities(datee.ToShortDateString)

                'Calcul des agrégats sectoriels à base de médianes
                '' '' '' ''co.ProcedureStockée("ACT_Agregats_Secteur", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {datee}))

                'Insertion des Notes ISR
                '' '' '' ''UpdateISR_NOTE()

                'Ajout des liquidity forcé
                'If co.SelectDistinctWhere("ACT_DATA_LIQUIDITY", "date", "date", datee).Count > 0 Then
                'co.DeleteWhere("ACT_DATA_LIQUIDITY", "date", datee)
                'End If
                'Dim union As String

                'Dim names2 As New List(Of String)(New String() {"date", "isin", "libelle", "defaut", "forcer", "unions"})
                'For i = 0 To DataGridViewLiquidity.Rows.Count - 1
                'If IsDBNull(DataGridViewLiquidity.Rows(i).Cells(3).Value) And IsDBNull(DataGridViewLiquidity.Rows(i).Cells(4).Value) Then
                'union = Nothing
                'Else
                'union = "X"
                'End If
                'Dim donnees As New List(Of Object)(New Object() {datee, _
                '                               DataGridViewLiquidity.Rows(i).Cells(1).Value, _
                '                               DataGridViewLiquidity.Rows(i).Cells(2).Value, _
                '                               DataGridViewLiquidity.Rows(i).Cells(3).Value, _
                '                               DataGridViewLiquidity.Rows(i).Cells(4).Value, _
                '                               union})
                'co.Insert("ACT_DATA_LIQUIDITY", names2, donnees)
                'Next i
                'For i = 0 To DataGridViewLiquidity.Rows.Count - 1
                '    If DataGridViewLiquidity.Rows(i).Cells(0).Value <> Nothing Then
                '        If co.SelectWhere2("DATA_FACTSET", "isin", "isin", DataGridViewLiquidity.Rows(i).Cells(0).Value, "date", datee, 1).Count = 1 Then
                '            co.Updates("DATA_FACTSET", New List(Of String)(New String() {"LIQUIDITY_TEST"}), New List(Of Object)(New Object() {"F"}), New List(Of String)(New String() {"date", "isin"}), New List(Of Object)(New Object() {datee, DataGridViewLiquidity.Rows(i).Cells(0).Value}))
                '        Else
                '            MsgBox("Le stock " & DataGridViewLiquidity.Rows(i).Cells(0).Value & " - " & DataGridViewLiquidity.Rows(i).Cells(1).Value & " n'a pas été forcé")
                '        End If
                '    End If
                'Next i


                'Traitement Interne
                '0)Gérer les secteurs fga avec les icb secteurs
                'co.RequeteSql("UPDATE DATA_FACTSET SET FGA_SECTOR = ss.id_fga_sector FROM DATA_FACTSET f, ACT_SUBSECTOR ss WHERE date='" & datee & "' and ss.id=f.ICB_SUBSECTOR")

                '1) CALCUL du ratio PE_VS_IND_ON_AVG5Y
                'Dim PE_ind As Double = excel.CellFichierExcel(My.Settings.PATH & "\INPUT\ACTION\FACTSET", "PE moyen 5 ans.xls", 2, 2, 4)
                'Dim PE_5m As Double = excel.CellFichierExcel(My.Settings.PATH & "\INPUT\ACTION\FACTSET", "PE moyen 5 ans.xls", 2, 1, 4)
                'Dim sql_PE As String = "UPDATE DATA_FACTSET  SET PE_VS_IND_NTM = PE_NTM/" & PE_ind & ",  PE_VS_IND_AVG5Y = PE_NTM_AVG5Y/" & PE_5m & ", PE_VS_IND_ON_AVG5Y =  (PE_NTM/" & PE_ind & ")/ (PE_NTM_AVG5Y/" & PE_5m & ") WHERE date='" & datee & "'"
                'co.RequeteSql(sql_PE)
                '=> Maintenant fait dans la macro du fichier excel


                '1)Inverse des ratios de valorisations
                'Dim toInverse As New List(Of String)
                'toInverse.Add("PE_NTM")
                'toInverse.Add("PB_NTM")
                'toInverse.Add("EV_SALES_NTM")
                'toInverse.Add("EV_EBITDA_NTM")
                'toInverse.Add("EV_EBIT_NTM")
                'toInverse.Add("PEG_NTM")
                'toInverse.Add("PE_NTM_AVG5Y")
                'toInverse.Add("PE_NTM_AVG10Y")
                'toInverse.Add("PE_VS_IND_ON_AVG5Y")
                'toInverse.Add("PB_VS_IND_ON_AVG5Y")
                'toInverse.Add("PB_NTM_AVG5Y")
                'toInverse.Add("PB_NTM_AVG10Y")
                'toInverse.Add("EV_SALES_NTM_AVG5Y")
                'toInverse.Add("EV_SALES_NTM_AVG10Y")
                'toInverse.Add("EV_EBITDA_NTM_AVG5Y")
                'toInverse.Add("EV_EBITDA_NTM_AVG10Y")
                'toInverse.Add("EV_EBIT_NTM_AVG5Y")
                'toInverse.Add("EV_EBIT_NTM_AVG10Y")
                'toInverse.Add("PE_ON_AVG5Y")
                'toInverse.Add("PE_ON_AVG10Y")
                'toInverse.Add("PB_ON_AVG5Y")
                'toInverse.Add("PB_ON_AVG10Y")
                'toInverse.Add("EV_SALES_ON_AVG5Y")
                'toInverse.Add("EV_SALES_ON_AVG10Y")
                'toInverse.Add("EV_EBITDA_ON_AVG5Y")
                'toInverse.Add("EV_EBITDA_ON_AVG10Y")
                'toInverse.Add("EV_EBIT_ON_AVG5Y")
                'toInverse.Add("EV_EBIT_ON_AVG10Y")
                'Dim sqlToInverse As String = ""
                'For Each indicator In toInverse
                '    sqlToInverse = ""
                '    sqlToInverse = "UPDATE DATA_FACTSET  SET " & indicator & "_INVERSE = CASE WHEN " & indicator & " = 0 THEN NULL ELSE 1/" & indicator & " END "
                '    sqlToInverse = sqlToInverse & "WHERE date='" & datee & "'"
                '    co.RequeteSql(sqlToInverse)
                'Next


                '1) Mise à jour des secteurs et associations


                '2) WINSORIZATION + NORMALISATION pour les portefeuille ABS
                ''Winsorize pour 90%
                '' '' '' ''co.ProcedureStockée("ACT_BlendScore_Valeur",
                '' '' '' ''                    New List(Of String)(New String() {"@date", "@winsor_coef", "@class_name"}),
                '' '' '' ''                    New List(Of Object)(New Object() {datee, Convert.ToDouble(TWinsorized.Text), "FGA_EU"}))
                '' '' '' ''co.ProcedureStockée("ACT_BlendScore_Valeur",
                '' '' '' ''                    New List(Of String)(New String() {"@date", "@winsor_coef", "@class_name"}),
                '' '' '' ''                    New List(Of Object)(New Object() {datee, Convert.ToDouble(TWinsorized.Text), "FGA_US"}))


                ''Ajout des agrégations dans la base 
                'co.DeleteWhere("ACT_DATA_FACTSET_AGR", "date", datee)
                'Dim sql As List(Of String) = New List(Of String)
                'Dim sql_agregation As String = Replace(fichier.LectureFichierSql(My.Settings.PATH & "\SQL_SCRIPTS\TEMPLATE\ACTION", "Agregation.sql"), "ma_date", datee)
                'sql.Add(sql_agregation)
                ''par indice 1*3
                'Dim indice As New List(Of String)(New String() {"SXXA", "SXXP", "SXXE"})
                'Dim sql_indice As String = Replace(fichier.LectureFichierSql(My.Settings.PATH & "\SQL_SCRIPTS\TEMPLATE\ACTION", "Agregation_indice.sql"), "ma_date", datee)
                'For Each indices In indice
                '    sql.Add(Replace(sql_indice, "mon_indice", indices))
                'Next
                ''par supersector 3*19
                'Dim sql_supersector As String = Replace(fichier.LectureFichierSql(My.Settings.PATH & "\SQL_SCRIPTS\TEMPLATE\ACTION", "Agregation_supersector.sql"), "ma_date", datee)
                'For Each indices In indice
                '    For Each supersectors In co.SelectDistinctSimple("ACT_SUPERSECTOR", "id")
                '        sql.Add(Replace(Replace(sql_supersector, "mon_indice", indices), "mon_supersector", supersectors))
                '    Next
                'Next
                ''par sector
                'Dim sql_sector As String = Replace(fichier.LectureFichierSql(My.Settings.PATH & "\SQL_SCRIPTS\TEMPLATE\ACTION", "Agregation_sector.sql"), "ma_date", datee)
                'For Each indices In indice
                '    For Each supersectors In co.SelectDistinctSimple("ACT_SUPERSECTOR", "id")
                '        For Each sectors In co.SelectDistinctWhere("ACT_SECTOR", "id", "id_supersector", supersectors)
                '            sql.Add(Replace(Replace(Replace(sql_sector, "mon_indice", indices), "mon_supersector", supersectors), "mon_sector", sectors))
                '        Next
                '    Next
                'Next
                'sql.Add("DROP TABLE #ref DROP TABLE #factset")
                'co.RequeteSqls(sql)

                'Dim agregates As List(Of String) = New List(Of String)()
                'For Each fichiers In Directory.GetFiles(My.Settings.PATH & "\INPUT\ACTION\FACTSET\AGREGATE", "*.xls", SearchOption.AllDirectories)
                '    agregates.Add(New IO.FileInfo(fichiers).Name())
                'Next
                'agregates.Sort(New myReverserClass)


                co.ProcedureStockée("ACT_BlendScore", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {datee}))

                Windows.Forms.Cursor.Current = Cursors.Default
                Me.Close()
                Dim bs As New BaseAction
                bs.Show()
            Else
                MessageBox.Show("Le parametre de winsorisation n'est pas un nombre !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' CbDateCoef_SelectedValueChanged : binder la DataGridViewCoef
        ''' </summary>
        Private Sub CbDateCoef_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateCoef.SelectedValueChanged
            'plus grande date par défault
            Dim coef As String = "SELECT INDICATOR As 'Indicator', FINANCE AS 'Finance' , OTHERS AS 'Others' FROM ACT_DATA_FACTSET_COEF WHERE date = '" & CbDateCoef.Text & "' and portfolio='Growth'"
            DataGridCoefGrowth.DataSource = co.LoadDataGridByString(coef)

            coef = "SELECT INDICATOR As 'Indicator', FINANCE AS 'Finance', OTHERS AS 'Others' FROM ACT_DATA_FACTSET_COEF WHERE date = '" & CbDateCoef.Text & "' and portfolio='Value'"
            DataGridCoefValue.DataSource = co.LoadDataGridByString(coef)

            coef = "SELECT INDICATOR As 'Indicator', FINANCE AS 'Finance', OTHERS AS 'Others', BANK AS 'Bank' FROM ACT_DATA_FACTSET_COEF WHERE date = '" & CbDateCoef.Text & "' and portfolio='BlendValeur'"
            DataGridCoefBlendValeur.DataSource = co.LoadDataGridByString(coef)

            coef = "SELECT INDICATOR As 'Indicator', OTHERS AS 'Coef' FROM ACT_DATA_FACTSET_COEF WHERE date = '" & CbDateCoef.Text & "' and portfolio='BlendSecteur'"
            DataGridCoefBlendSecteur.DataSource = co.LoadDataGridByString(coef)

            If DataGridCoefGrowth.Columns.Count > 0 Then
                DataGridCoefGrowth.Columns(0).ReadOnly = True
                DataGridCoefValue.Columns(0).ReadOnly = True
                DataGridCoefBlendValeur.Columns(0).ReadOnly = True
                DataGridCoefBlendSecteur.Columns(0).ReadOnly = True
            End If
        End Sub

        ''' <summary>
        ''' BaseActionImportation_FormClosing : binder la DataGridViewLiquidity
        ''' </summary>
        Private Sub BaseActionImportation_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Dim bs As New BaseAction
            bs.Show()
        End Sub


        ''' <summary>
        ''' CbLiquidity_SelectedValueChanged : binder la DataGridViewLiquidity
        ''' </summary>
        Private Sub CbDateLiquidity_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateLiquidity.SelectedValueChanged
            'plus grande date par défault
            'Dim liqu As String = "SELECT 	l.isin As 'Isin', "
            'liqu = liqu & "l.libelle As 'Libellé', "
            'liqu = liqu & "l.defaut As 'Défaut', "
            'liqu = liqu & "l.forcer As 'Forcer', "
            'liqu = liqu & "s.libelle As 'Secteur' "
            'liqu = liqu & "FROM ACT_DATA_LIQUIDITY l "
            'liqu = liqu & "LEFT OUTER JOIN DATA_FACTSET d ON d.isin = l.isin and d.date= (SELECT TOP 1 date FROM DATA_FACTSET order by date) "
            'liqu = liqu & "LEFT OUTER JOIN ACT_SUPERSECTOR s ON s.id = d.icb_sector "
            'liqu = liqu & "WHERE l.date = '" & CbLiquidity.Text & "' "
            'liqu = liqu & "ORDER BY l.libelle "

            'mise en forme de la grille 
            DataGridViewLiquidity.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_Liquidity", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {CbDateLiquidity.Text})))
            'If DataGridViewLiquidity.Columns.Count > 0 Then
            'DataGridViewLiquidity.Columns(0).ReadOnly = True 'compteur
            'DataGridViewLiquidity.Columns(1).ReadOnly = True 'isin
            'DataGridViewLiquidity.Columns(2).ReadOnly = True 'libelle
            'DataGridViewLiquidity.Columns(DataGridViewLiquidity.Columns.Count - 1).ReadOnly = True 'secteur
            'End If
            'co.AutoFiltre(DataGridViewLiquidity, New List(Of Integer)(New Integer() {DataGridViewLiquidity.Columns.Count - 1}))
        End Sub


        ''' <summary>
        ''' CbIsr_SelectedIndexChanged : binder DIsr
        ''' </summary>
        Private Sub CbDateIsr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDateIsr.SelectedIndexChanged
            'mise en forme de la grille 
            Dim sqlIsr = "SELECT ISIN,SEDOL,NAME,EUROPE,EURO,EXEURO FROM ISR_NOTE where DATE='" & CbDateIsr.Text & "' AND EUROPE IS NOT NULL order by NAME"
            DIsr.DataSource = co.LoadDataGridByString(sqlIsr)
            da.AutoFiltre(DIsr, New List(Of Integer)(New Integer() {0, 1, 2}))

            'Dim sqlIsrPb = "SELECT isin INTO #tmp FROM DATA_FACTSET WHERE date='" & CbIsr.Text & "'"
            'sqlIsrPb = sqlIsrPb & " EXCEPT"
            'sqlIsrPb = sqlIsrPb & " SELECT isin FROM ACT_DATA_ISR WHERE date='" & CbIsr.Text & "' "
            'sqlIsrPb = sqlIsrPb & " SELECT DISTINCT d.isin, d.company_name FROM #tmp t, DATA_FACTSET d"
            'sqlIsrPb = sqlIsrPb & " WHERE t.isin = d.isin and d.date='" & CbIsr.Text & "'"
            'sqlIsrPb = sqlIsrPb & " DROP TABLE #tmp"

            DIsrPb.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ACT_Isr_Verification", New List(Of String)(New String() {"@date"}), New List(Of Object)(New Object() {CbDateIsr.Text})))
            da.RowHeaderCell(DIsrPb)
        End Sub

        Private Sub BExcelISR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelISR.Click
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            UpdateIsr()
            Dim dateIsr As DateTime = excel.CellFichierExcel("G:\,FGA ISR\Notation Fédéris", "Notation ISR.xls", 3, 3, 1)
            Dim sql = "SELECT ISIN,SEDOL,NAME,EUROPE,EURO,EXEURO FROM ISR_NOTE where DATE='" & dateIsr & "' AND EUROPE IS NOT NULL order by NAME"
            DIsr.DataSource = co.LoadDataGridByString(sql)
            CbDateIsr.DataSource = co.SelectDistinctSimple("ISR_NOTE", "DATE", "DESC")
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Public Shared Sub UpdateIsr()
            Dim lo As New Log()
            Dim excel As New Excel()
            Dim co As New Connection()

            Try
                Dim dateIsr As DateTime = excel.CellFichierExcel("G:\,FGA ISR\Notation Fédéris", "Notation ISR.xls", 3, 3, 1)
                co.DeleteWhere("ISR_NOTE", "DATE", dateIsr)
                excel.ExcelToSql("G:\,FGA ISR\Notation Fédéris", "Notation ISR.xls", 3, "ISR_NOTE")
                lo.Log(ELog.Information, "BExcelISR_Click", "Importation d'un fichier note isr")
            Catch ex As Exception
                MessageBox.Show("Le fichier 'Notation ISR.xls' n'est pas valide !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                Windows.Forms.Cursor.Current = Cursors.Default
            End Try
        End Sub

        Public Shared Sub UpdateISR_NOTE()
            Dim lo As New Log()
            Dim excel As New Excel()
            Dim co As New Connection()

            Try
                Dim dateIsr As DateTime = excel.CellFichierExcel("G:\,FGA ISR\Notation Fédéris", "Notation ISR.xls", 3, 3, 1)
                co.DeleteWhere("ISR_NOTE", "DATE", dateIsr)
                excel.ExcelToSql("G:\,FGA ISR\Notation Fédéris", "Notation ISR.xls", 3, "ISR_NOTE")
                lo.Log(ELog.Information, "AutoImportISR", "Importation d'un fichier note isr")
            Catch ex As Exception
                MessageBox.Show("Le fichier 'Notation ISR.xls' n'est pas valide !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                Windows.Forms.Cursor.Current = Cursors.Default
            End Try
        End Sub

        ''' <summary>
        ''' BExcel_Click : ajouter liquidity en base
        ''' </summary>
        Private Sub BExcel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            ''If OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            ''If co.CellFichierExcel(Split(OpenFileDialog.FileName, "\" & OpenFileDialog.SafeFileName)(0), OpenFileDialog.SafeFileName, 1, 1, 5) = "forcer" Then
            'Try
            'Windows.Forms.Cursor.Current = Cursors.WaitCursor
            ''supprime donnée
            'Dim dateLiquidity As DateTime = co.CellFichierExcel("G:\,FGA Front Office\02_Gestion_Actions\MODELE GROWTH VALUE\modèle V3", "02_Liquidity_Europe.xls", 4, 2, 1)
            'co.DeleteWhere("ACT_DATA_LIQUIDITY", "date", dateLiquidity)
            ''ajoute donnée dans base
            'co.ExcelToSql("G:\,FGA Front Office\02_Gestion_Actions\MODELE GROWTH VALUE\modèle V3", "02_Liquidity_Europe.xls", 4, "ACT_DATA_LIQUIDITY")
            'CbLiquidity.DataSource = co.SelectDistinctSimple("ACT_DATA_LIQUIDITY", "date", "DESC")
            'Windows.Forms.Cursor.Current = Cursors.Default
            'lo.Log(ELog.Information, "BExcel_Click", "Importation d'un fichier liquidity")
            'Catch ex As Exception
            ' MessageBox.Show("Le fichier '02_Liquidity_Europe.xls' n'est pas valide !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            'Windows.Forms.Cursor.Current = Cursors.Default
            'End Try


            ''End If
        End Sub

        Private Sub BTicker_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTicker.Click
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            UpdateTicker("\INPUT\ACTION\FACTSET")
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Public Shared Sub UpdateTicker(ByVal TickerISIN_base_path As String)
            Dim lo As New Log()
            Dim excel As New Excel()
            Dim co As New Connection()

            'supprime donnée
            Dim datee As DateTime = excel.CellFichierExcel(My.Settings.PATH & TickerISIN_base_path, "TickerISIN_base.xlsx", 1, 2, 1)
            co.DeleteWhere("ACT_TICKER", "date", datee)
            'excel.ExcelToSql(My.Settings.PATH & "\INPUT\ACTION\FACTSET", "TickerISIN_base.xlsx", 1, "ACT_TICKER")
            excel.ExcelBBGToSqlUpdate(My.Settings.PATH & TickerISIN_base_path,
                                      "TickerISIN_base.xlsx",
                                      1,
                                      "ACT_TICKER")

            ' Met à jour les tickers dans ACT_VALEUR
            ' TODO: mettre à jour directement à partir du fichier, sans passer par ACT_TICKER.
            co.RequeteSql("UPDATE ACT_VALEUR" +
                          " SET TICKER_BLOOMBERG = t.TICKER_BLOOMBERG" +
                          " FROM ACT_VALEUR v" +
                          " INNER JOIN ACT_TICKER t ON v.ISIN = t.ISIN AND (v.TICKER_BLOOMBERG IS NULL OR v.TICKER_BLOOMBERG <> t.TICKER_BLOOMBERG)" +
                          " WHERE t.date ='" + datee + "'")
        End Sub

        ''' <summary>
        ''' Simuler une importation d un fichier factset
        ''' </summary>
        Public Sub batchImportation(ByVal cmdArgs() As String)
            Dim co As Connection = New Connection()
            'Tentative de connection a la BDD
            fichier.LectureFichierLog("Login.INI")
            'chargment des tables de scoring
            Me.BaseActionImportation_Load(Nothing, Nothing)
            'recherche du dernier fichier factset 
            Me.CbFactSet_Click(Nothing, Nothing)
            'Ajouter à en simulant le click
            If (BAjouter.Enabled = True) Then
                Me.BAjouter_Click(Nothing, Nothing)
            End If
        End Sub

        Private Sub checkForMissingSectors(ByVal mdate As String)
            '-----------------------------------------------------------------------------------------------------------------------------------
            ' Ajout des secteurs FGA
            Dim sql As String
            sql = " INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name) " +
                  " SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_ALL'" +
                  " FROM DATA_FACTSET" +
                  " WHERE GICS_SECTOR IS NULL AND MXEU is not null AND MXUSLC is not null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_ALL')" +
                  " AND ISIN is NULL and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name) " +
                  " SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_EU'" +
                  " FROM DATA_FACTSET" +
                  " WHERE GICS_SECTOR IS NULL AND MXEU is not null AND MXUSLC is null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_EU')" +
                  " AND ISIN is NULL and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name) " +
                  " SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_US'" +
                  " FROM DATA_FACTSET" +
                  " WHERE GICS_SECTOR IS NULL AND MXEU is null AND MXUSLC is not null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_US')" +
                  " AND ISIN is NULL and date = '" + mdate + "'"
            co.RequeteSql(sql)


            '-----------------------------------------------------------------------------------------------------------------------------------
            ' Ajout des industrygrp en priorité pour référence dans les industries
            sql = " INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name) " +
                  " SELECT GICS_SECTOR, NULL, 0, SECTOR_LABEL, 'GICS'" +
                  " FROM DATA_FACTSET" +
                  " WHERE GICS_SECTOR IS NOT NULL AND GICS_SUBINDUSTRY IS NULL AND GICS_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR)" +
                  " AND ISIN is NULL and date = '" + mdate + "'"
            co.RequeteSql(sql)

            '-----------------------------------------------------------------------------------------------------------------------------------
            ' Ajout des subindustry
            sql = " INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name) " +
                  " SELECT fac.GICS_SUBINDUSTRY, sec.id, COALESCE(sec.level + 1, 0), fac.SECTOR_LABEL, 'GICS'" +
                  " FROM DATA_FACTSET fac" +
                  " LEFT OUTER JOIN ref_security.SECTOR sec ON sec.code = fac.GICS_SECTOR AND sec.class_name = 'GICS'" +
                  " WHERE GICS_SUBINDUSTRY IS NOT NULL AND GICS_SUBINDUSTRY NOT IN (SELECT code FROM ref_security.SECTOR)" +
                  " AND fac.ISIN is NULL and fac.date = '" + mdate + "'"
            co.RequeteSql(sql)

            '-----------------------------------------------------------------------------------------------------------------------------------
            ' Liaison entre secteurs FGA GICS
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'GICS'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY" +
                  " WHERE sec1.class_name='FGA_ALL' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'FGA_ALL'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR" +
                  " WHERE sec2.class_name='FGA_ALL' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_ALL')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'GICS'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY" +
                  " WHERE sec1.class_name='FGA_EU' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'FGA_EU'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR" +
                  " WHERE sec2.class_name='FGA_EU' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_EU')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'GICS'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY" +
                  " WHERE sec1.class_name='FGA_US' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)
            sql = " INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)" +
                  " SELECT sec1.ID, sec2.ID, 'FGA_US'" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY" +
                  " INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR" +
                  " WHERE sec2.class_name='FGA_US' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_US')" +
                  " and fac.ISIN is null and date = '" + mdate + "'"
            co.RequeteSql(sql)


        End Sub

        Private Sub checkForMissingEquities(ByVal mdate As String)
            Dim sql As String

            ' ----------------------------------
            ' Ajout des identifications d'assets
            ' ----------------------------------

            'sql = " INSERT into ref_common.IDENTIFICATION(ISIN, Name, MaturityDate, Country)" +
            '      " SELECT fac.ISIN, fac.COMPANY_NAME, fac.date, 'XX'" +
            '      " FROM DATA_FACTSET fac" +
            '      " WHERE fac.ISIN IS NOT NULL AND fac.ISIN NOT IN (SELECT ISIN FROM ref_common.IDENTIFICATION where MaturityDate=fac.date)" +
            '      " and fac.date = '" + mdate + "'"
            'co.RequeteSql(sql)

            ' -----------------------------
            ' Ajout des equities manquantes
            ' -----------------------------

            'sql = " INSERT into ref_security.ASSET(ISIN, FinancialInstrumentName, MaturityDate, IdentificationId, Discriminator)" +
            '      " SELECT fac.ISIN, fac.COMPANY_NAME, fac.date, iden.Id, 'Equity'" +
            '      " FROM DATA_FACTSET fac" +
            '      " LEFT OUTER JOIN ref_common.IDENTIFICATION iden ON iden.ISIN = fac.ISIN" +
            '      " WHERE fac.ISIN IS NOT NULL AND fac.ISIN NOT IN (SELECT ISIN FROM ref_security.ASSET)" +
            '      " and fac.date = '" + mdate + "'"

            'co.RequeteSql(sql)

            ' -----------------------------------
            ' Lien des equities avec les secteurs 
            ' -----------------------------------

            ' Liaison avec les SECTOR
            sql = " INSERT INTO ref_security.ASSET_TO_SECTOR(ID_ASSET, ID_SECTOR, CLASS_NAME, CODE_SECTOR, SOURCE, DATE)" +
                  " SELECT ass.ID, sec.ID, sec.CLASS_NAME, sec.CODE, 'Factset', fac.date" +
                  " FROM DATA_FACTSET fac" +
                  " INNER JOIN ref_security.ASSET ass ON ass.ISIN = fac.ISIN" +
                  " INNER JOIN ref_security.SECTOR sec ON sec.CODE = fac.SECTOR" +
                  " WHERE fac.ISIN IS NOT NULL and ass.ID not in (SELECT id_asset FROM ref_security.ASSET_TO_SECTOR where date = '" + mdate + "')" +
                  " and fac.SECTOR IS NOT NULL and date = '" + mdate + "'"
            co.RequeteSql(sql)

        End Sub

    End Class

    Public Class myReverserClass
        Implements IComparer(Of String)

        ' Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        Public Function Compare(ByVal x As String, ByVal y As String) As Integer _
           Implements IComparer(Of String).Compare
            Return String.Compare(y, x)
        End Function 'IComparer.Compare
    End Class 'myReverserClass
End Namespace
