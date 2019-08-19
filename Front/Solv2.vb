Imports System.IO

Public Class Solv2


    Dim co As Connection = New Connection()
    Dim fichier As Fichier = New Fichier()
    Dim excel As Excel = New Excel()
    Dim da As DGrid = New DGrid()


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSimulation.Click


        If IsNumeric(TNmc.Text) = False And IsNumeric(TACorpoWeight.Text) = False And IsNumeric(TBCorpoWeight.Text) = False And IsNumeric(TAGovieWeight.Text) = False And IsNumeric(TAAGovieWeight.Text) = False And IsNumeric(TBGovieWeight.Text) = False And IsNumeric(TSave.Text) = False Then
            MessageBox.Show("Les inputs ne sont pas valides !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            Exit Sub
        End If

        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Dim datee = CbDateSimulation.Text
        Dim Nmc As Integer = Convert.ToDouble(TNmc.Text)
        'En pourcentage 3% et 1.5%
        Dim ACorpoWeight As Double = Convert.ToDouble(TACorpoWeight.Text)
        Dim BCorpoWeight As Double = Convert.ToDouble(TBCorpoWeight.Text)
        Dim AGovieWeight As Double = Convert.ToDouble(TAGovieWeight.Text)
        Dim AAGovieWeight As Double = Convert.ToDouble(TAAGovieWeight.Text)
        Dim BGovieWeight As Double = Convert.ToDouble(TBGovieWeight.Text)


        'Dim Nindice As Integer = co.SelectDistinctWhere("TX_IBOXX", "COUNT(isin)", "date", datee).FirstOrDefault
        'Table de réference
        Try

            Dim Iisin As DataSet = co.RequeteSqlToDataSet(Replace(TSql.Text, "xxx", datee), "MonIndice")
            Dim Nindice As Integer = Iisin.Tables(0).Rows.Count - 1

            'Variable temporaire pour la construction du portefeuile
            Dim rating, issuerTicker, level, sqlAdd As String
            Dim weight, weighted, totalWeight, emetteurWeight As Double
            Dim nisin, nportfolio As Integer
            'Dim names As List(Of String) = New List(Of String)(New String() {"date", "portfolio", "isin", "weight", "issuerTicker", "rating"})


            'On suprime ou pas les nouvelles données
            If (co.SelectDistinctWhere("S2_PORT_COMPO", "date", "date", datee).Count > 0 Or co.SelectDistinctWhere("S2_PORT_TOTAL", "date", "date", datee).Count > 0 Or co.SelectDistinctWhere("S2_PORT_COMPO_TEMP", "date", "date", datee).Count > 0) Then
                Dim a As Integer = MessageBox.Show("Voulez vous suprimer les " & co.SelectDistinctWhere("S2_PORT_TOTAL", "COUNT(portfolio)", "date", datee).FirstOrDefault & " portefeuilles au " & datee & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 6 Then
                    co.DeleteWhere("S2_PORT_COMPO_TEMP", "date", datee)
                    co.DeleteWhere("S2_PORT_COMPO", "date", datee)
                    co.DeleteWhere("S2_PORT_TOTAL", "date", datee)
                End If
            End If

            'On lance la construction de Nmc portefeuille aléatoire
            'If (co.SelectDistinctWhere("S2_PORT_COMPO", "date", "date", datee).Count = 0) Then
            For port As Integer = 1 To Nmc

                'nportfolio = co.SelectDistinctWhere("S2_PORT_COMPO", "MAX(portfolio)", "date", datee).FirstOrDefault() + 1
                'nportfolio = co.SelectDistinctWhere("S2_PORT_COMPO_TEMP", "MAX(portfolio)", "date", datee).FirstOrDefault() + co.SelectDistinctWhere("S2_PORT_COMPO", "MAX(portfolio)", "date", datee).FirstOrDefault() + 1
                nportfolio = co.RequeteSqlToList("INSERT INTO S2_PORT_TOTAL (date) VALUES ('" & datee & "') SELECT @@IDENTITY AS 'Identity'").FirstOrDefault


                While totalWeight < 100

                    Randomize()
                    'On tire àléatoirement un chiffre [0;Nindice]
                    nisin = CInt(Int((Nindice * Rnd()) + 1)) - 1
                    rating = Iisin.Tables(0).Rows(nisin)("iboxx_rating")
                    issuerTicker = Iisin.Tables(0).Rows(nisin)("issuer")
                    level = Iisin.Tables(0).Rows(nisin)("sector_level1")
                    If level = "Sovereigns" Then
                        If rating = "AAA" Or rating = "AA" Then
                            weight = (Rnd() * AAGovieweight)
                            emetteurWeight = co.SelectDistinctWheres("S2_PORT_COMPO_TEMP", "SUM(weight)", New List(Of String)(New String() {"date", "portfolio", "IssuerTicker"}), New List(Of Object)(New Object() {CbDateSimulation.Text, nportfolio, issuerTicker})).FirstOrDefault
                            If emetteurWeight < AAGovieweight Then
                                weighted = Min(Min(weight, AAGovieweight - emetteurWeight), 100 - totalWeight)
                                sqlAdd = "INSERT INTO S2_PORT_COMPO_TEMP(date,portfolio,isin,weight,issuerTicker,rating) VALUES ('" & CbDateSimulation.Text & "'," & nportfolio & ",'" & Iisin.Tables(0).Rows(nisin)("isin") & "'," & weighted & ",'" & Replace(issuerTicker, "'", "''") & "','" & rating & "')"
                                co.RequeteSql(sqlAdd)
                                totalWeight = totalWeight + weighted
                            End If
                        ElseIf rating = "A" Then
                            weight = (Rnd() * AGovieweight)
                            emetteurWeight = co.SelectDistinctWheres("S2_PORT_COMPO_TEMP", "SUM(weight)", New List(Of String)(New String() {"date", "portfolio", "IssuerTicker"}), New List(Of Object)(New Object() {CbDateSimulation.Text, nportfolio, issuerTicker})).FirstOrDefault
                            If emetteurWeight < AGovieweight Then
                                weighted = Min(Min(weight, AGovieweight - emetteurWeight), 100 - totalWeight)
                                sqlAdd = "INSERT INTO S2_PORT_COMPO_TEMP(date,portfolio,isin,weight,issuerTicker,rating) VALUES ('" & CbDateSimulation.Text & "'," & nportfolio & ",'" & Iisin.Tables(0).Rows(nisin)("isin") & "'," & weighted & ",'" & Replace(issuerTicker, "'", "''") & "','" & rating & "')"
                                co.RequeteSql(sqlAdd)
                                totalWeight = totalWeight + weighted
                            End If
                        Else
                            'BBB et inférieur
                            weight = (Rnd() * BGovieweight)
                            emetteurWeight = co.SelectDistinctWheres("S2_PORT_COMPO_TEMP", "SUM(weight)", New List(Of String)(New String() {"date", "portfolio", "IssuerTicker"}), New List(Of Object)(New Object() {CbDateSimulation.Text, nportfolio, issuerTicker})).FirstOrDefault
                            If emetteurWeight < BGovieweight Then
                                weighted = Min(Min(weight, BGovieweight - emetteurWeight), 100 - totalWeight)
                                sqlAdd = "INSERT INTO S2_PORT_COMPO_TEMP(date,portfolio,isin,weight,issuerTicker,rating) VALUES ('" & CbDateSimulation.Text & "'," & nportfolio & ",'" & Iisin.Tables(0).Rows(nisin)("isin") & "'," & weighted & ",'" & Replace(issuerTicker, "'", "''") & "','" & rating & "')"
                                co.RequeteSql(sqlAdd)
                                totalWeight = totalWeight + weighted
                            End If
                        End If
                        'Cas Corparate et autres
                    Else
                        If rating = "AAA" Or rating = "AA" Or rating = "A" Then
                            'On tire àléatoirement un chiffre [0;3%]
                            weight = (Rnd() * ACorpoweight)
                            emetteurWeight = co.SelectDistinctWheres("S2_PORT_COMPO_TEMP", "SUM(weight)", New List(Of String)(New String() {"date", "portfolio", "IssuerTicker"}), New List(Of Object)(New Object() {CbDateSimulation.Text, nportfolio, issuerTicker})).FirstOrDefault
                            If emetteurWeight < ACorpoweight Then
                                weighted = Min(Min(weight, ACorpoweight - emetteurWeight), 100 - totalWeight)
                                sqlAdd = "INSERT INTO S2_PORT_COMPO_TEMP(date,portfolio,isin,weight,issuerTicker,rating) VALUES ('" & CbDateSimulation.Text & "'," & nportfolio & ",'" & Iisin.Tables(0).Rows(nisin)("isin") & "'," & weighted & ",'" & Replace(issuerTicker, "'", "''") & "','" & rating & "')"
                                co.RequeteSql(sqlAdd)
                                totalWeight = totalWeight + weighted
                            End If
                        Else
                            'BBB et inferieur
                            'On tire àléatoirement un chiffre [0;1.5%]
                            weight = (Rnd() * BCorpoweight)
                            emetteurWeight = co.SelectDistinctWheres("S2_PORT_COMPO_TEMP", "SUM(weight)", New List(Of String)(New String() {"date", "portfolio", "IssuerTicker"}), New List(Of Object)(New Object() {CbDateSimulation.Text, nportfolio, issuerTicker})).FirstOrDefault
                            If emetteurWeight < BCorpoweight Then
                                weighted = Min(Min(weight, BCorpoweight - emetteurWeight), 100 - totalWeight)
                                sqlAdd = "INSERT INTO S2_PORT_COMPO_TEMP(date,portfolio,isin,weight,issuerTicker,rating) VALUES ('" & CbDateSimulation.Text & "'," & nportfolio & ",'" & Iisin.Tables(0).Rows(nisin)("isin") & "'," & weighted & ",'" & Replace(issuerTicker, "'", "''") & "','" & rating & "')"
                                co.RequeteSql(sqlAdd)
                                totalWeight = totalWeight + weighted
                            End If
                        End If
                    End If
                End While

                'On enregistre tout en base et en passe et un nouveau portefeuille Nmc
                'co.RequeteSql(sqlAdd)
                totalWeight = 0
                'sqlAdd = ""

                If port Mod Convert.ToDouble(TSave.Text) = 0 And port <> 0 And rbSave.Checked Then
                    'Pour vier la base car consomme du temps
                    co.RequeteSql("EXECUTE S2_portefeuille_temp '" & datee & "'")
                End If

            Next port

            'On revide la base
            co.RequeteSql("EXECUTE S2_portefeuille_temp '" & datee & "'")
            'On calcul les caratéristiques des portefeuilles
            'co.RequeteSql("EXECUTE S2_portefeuille '" & datee & "'")


            CbDateCharger.DataSource = Nothing
            CbDateCharger.DataSource = co.SelectDistinctSimple("S2_PORT_TOTAL", "date", "DESC")


            'Dim sql As String = "DECLARE @date AS Datetime"
            'sql = sql & " SET @date = '" & datee & "'"
            'sql = sql & " SELECT s.date, s.portfolio, "
            'sql = sql & " SUM(s.weight*i.duration)/100 As duration, "
            'sql = sql & " SUM(s.weight*i.life)/100 As life,"
            'sql = sql & " SUM(s.weight*i.AnnualYield)/100 As yield,"
            'sql = sql & " CASE WHEN r.life < i.life THEN SUM(s.weight*r.scr*r.life)/100 ELSE SUM(s.weight*r.scr*i.life)/100 END As scr,"
            'sql = sql & " CASE WHEN i.rating = 'AAA' THEN SUM(s.weight) END As AAA,"
            'sql = sql & " CASE WHEN i.rating = 'AA' THEN SUM(s.weight) END As AA,"
            'sql = sql & " CASE WHEN i.rating = 'A' THEN SUM(s.weight) END As A,"
            'sql = sql & " CASE WHEN i.rating = 'BBB' THEN SUM(s.weight) END As BBB"
            'sql = sql & " INTO #PORTFOLIO_CARACT    "
            'sql = sql & " FROM S2_PORT_COMPO s "
            'sql = sql & " LEFT OUTER JOIN TX_IBOXX i ON i.date=@date and s.isin=i.isin"
            'sql = sql & " LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.rating"
            'sql = sql & " WHERE s.date=@date"
            'sql = sql & " GROUP BY s.date, s.portfolio,r.life ,i.life, i.rating"
            'sql = sql & " DELETE FROM S2_PORT_TOTAL where date='" & datee & "'"
            'sql = sql & " INSERT INTO S2_PORT_TOTAL(date,portfolio,duration,life,yield,scr,AAA,AA,A,BBB) "
            'sql = sql & " SELECT date, portfolio, sum(duration), sum(life), sum(yield) , sum(scr), SUM(AAA), SUM(AA), SUM(A), SUM(BBB) "
            'sql = sql & " FROM #PORTFOLIO_CARACT "
            'sql = sql & " GROUP BY date, portfolio"
            'sql = sql & " DROP TABLE #PORTFOLIO_CARACT"

            'Dim sql As String = "EXECUTE S2_Portefeuille '" & datee & "'"
            'co.RequeteSql(sql)

            'CbDateCharger.DataSource = Nothing
            'CbDateCharger.DataSource = co.SelectDistinctSimple("S2_PORT_TOTAL", "date", "DESC")

        Catch
            MessageBox.Show("La requête sql n'est pas valide ?", "Confirmation.", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End Try


        Windows.Forms.Cursor.Current = Cursors.Default

        'End If
    End Sub

    Private Sub Solv2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Ajout en base que pour les administrateurs dans le groupe Dev
        If Utilisateur.admin = False Or Utilisateur.metier <> "Dev" Then
            Dim OngletSimulation As TabPage
            OngletSimulation = TOnglet.TabPages("TSimulation")
            TOnglet.TabPages.Remove(OngletSimulation)
        Else
            'On remplie la combobox avec les fichiers
            Dim myFichier As List(Of String) = New List(Of String)
            For Each fi In Directory.GetFiles(My.Settings.PATH & "\INPUT\SOLVENCY", "*.xls", SearchOption.TopDirectoryOnly)
                myFichier.Add(New IO.FileInfo(fi).Name())
            Next

            CbFichier.DataSource = myFichier
            CbDateSimulation.DataSource = co.SelectDistinctSimple("S2_UNIVERS", "report_date", "DESC")
        End If

        CbDateCharger.DataSource = co.SelectDistinctSimple("S2_PORT_TOTAL", "date", "DESC")
        TOnglet.SelectedTab = TPortefeuille

    End Sub


    Public Function Min(ByVal un As Double, ByVal deux As Double)
        If (un < deux) Then
            Return un
        Else
            Return deux
        End If
    End Function


    Private Sub BCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCharger.Click

        If IsNumeric(TDurationMin.Text) = False And IsNumeric(TDurationMax.Text) = False And IsNumeric(TRendementMin.Text) = False And IsNumeric(TRendementMax.Text) = False And IsNumeric(TScrMin.Text) = False And IsNumeric(TScrMax.Text) = False Then
            MessageBox.Show("Les inputs ne sont pas valides !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            Exit Sub
        End If

        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        'On vide toutes les grilles
        DPortfolio.DataSource = Nothing
        DTitre.DataSource = Nothing
        DEmetteur.DataSource = Nothing
        DRating.DataSource = Nothing
        DSecteur.DataSource = Nothing

        'On remplie la grille portfolio
        Dim portfolio As String = "SELECT portfolio, duration, life, scr, scr_1yr As 'scr 1YR', yield, defaut, AAA, AA, A, BBB, HY, transition_1yr As 'transition 1YR', transition_5yr As 'transition 5YR' FROM S2_PORT_TOTAL where date='" & CbDateCharger.Text & "' and (duration BETWEEN " & TDurationMin.Text & " and " & TDurationMax.Text & ")  and (yield BETWEEN " & TRendementMin.Text & " and " & TRendementMax.Text & ") and (scr BETWEEN " & TScrMin.Text & " and " & TScrMax.Text & ") "
        DPortfolio.DataSource = co.LoadDataGridByString(portfolio)
        DPortfolio.DefaultCellStyle.Format = "n2"
        DPortfolio.Columns("portfolio").DefaultCellStyle.Format = "n0"

        'On trace le graphique
        graph.Series("yield").Points.Clear()
        Dim points As DataSet = co.RequeteSqlToDataSet("SELECT scr, yield FROM S2_PORT_TOTAL where date='" & CbDateCharger.Text & "' and (duration BETWEEN " & TDurationMin.Text & " and " & TDurationMax.Text & ")  and (yield BETWEEN " & TRendementMin.Text & " and " & TRendementMax.Text & ") and (scr BETWEEN " & TScrMin.Text & " and " & TScrMax.Text & ") ", "S2_PORT_TOTAL")
        For Each dataRow As DataRow In points.Tables("S2_PORT_TOTAL").Rows
            graph.Series("yield").Points.AddXY(dataRow("scr"), dataRow("yield"))
        Next

        'On trace le 2eme graphique
        graph.Series(1).Points.Clear()
        Dim points2 As DataSet = co.RequeteSqlToDataSet("SELECT scr, defaut FROM S2_PORT_TOTAL where date='" & CbDateCharger.Text & "' and (duration BETWEEN " & TDurationMin.Text & " and " & TDurationMax.Text & ")  and (yield BETWEEN " & TRendementMin.Text & " and " & TRendementMax.Text & ") and (scr BETWEEN " & TScrMin.Text & " and " & TScrMax.Text & ") ", "S2_PORT_TOTAL")
        For Each dataRow As DataRow In points2.Tables("S2_PORT_TOTAL").Rows
            graph.Series(1).Points.AddXY(dataRow("scr"), dataRow("defaut"))
        Next

        graph.ChartAreas(0).AxisY.Maximum = Convert.ToDouble(TRendementMax.Text) + 0.5
        graph.ChartAreas(0).AxisY.Minimum = Convert.ToDouble(TRendementMin.Text) - 0.5

        'On remplit le nbr portefeuille
        TGraph.Text = graph.Series("yield").Points.Count
        TMax.Text = co.SelectDistinctWhere("S2_PORT_TOTAL", "COUNT(portfolio)", "date", CbDateCharger.Text).FirstOrDefault()

        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub


    Private Sub BAgregation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAgregation.Click
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Dim sql As String = "EXECUTE S2_Portefeuille_temp '" & CbDateSimulation.Text & "'"
        co.RequeteSql(sql)

        CbDateCharger.DataSource = Nothing
        CbDateCharger.DataSource = co.SelectDistinctSimple("S2_PORT_TOTAL", "date", "DESC")

        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub


    Public Sub Composition()
        DTitre.DataSource = Nothing
        DEmetteur.DataSource = Nothing
        DRating.DataSource = Nothing
        DSecteur.DataSource = Nothing

        If DPortfolio.CurrentRow IsNot Nothing Then

            Dim id As String = DPortfolio.CurrentRow.Cells(0).Value

            Dim titre As String = "SELECT s.Isin, s.Issuer, SUM(p.weight) As Weight, s.Coupon_in_pct As 'Coupon', s.Annual_Yield As 'Yield', s.Maturity_Date as 'Maturity', s.Annual_Modified_Duration As 'Sensi', s.Years_To_maturity As 'Life' FROM S2_PORT_COMPO p"
            titre = titre & " LEFT OUTER JOIN S2_UNIVERS s ON s.report_date='" & CbDateCharger.Text & "' and p.isin=s.isin"
            titre = titre & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            titre = titre & " GROUP BY s.Isin, s.Issuer, s.Coupon_in_pct, s.Annual_Yield, s.Maturity_Date, s.Annual_Modified_Duration, s.Years_To_maturity ORDER BY SUM(p.weight) DESC"

            Dim emetteur As String = "SELECT s.Issuer, SUM(p.weight) As Weight FROM S2_PORT_COMPO p"
            emetteur = emetteur & " LEFT OUTER JOIN S2_UNIVERS s ON s.report_date='" & CbDateCharger.Text & "' and p.isin=s.isin"
            emetteur = emetteur & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            emetteur = emetteur & " GROUP BY s.issuer ORDER BY SUM(p.weight) DESC"

            Dim rating As String = "SELECT p.Rating, SUM(p.weight) As Weight FROM S2_PORT_COMPO p"
            rating = rating & " LEFT OUTER JOIN TX_RATING r ON r.agence='Interne' and r.type_rating='LT' and r.rating=p.rating"
            rating = rating & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            rating = rating & " GROUP BY p.rating, r.ordre"
            rating = rating & " ORDER BY CONVERT(float,r.ordre)"

            Dim secteur As String = "SELECT s.sector_Level4 As 'Level4', SUM(p.weight) As Weight FROM S2_PORT_COMPO p"
            secteur = secteur & " LEFT OUTER JOIN S2_UNIVERS s ON s.report_date='" & CbDateCharger.Text & "' and p.isin=s.isin"
            secteur = secteur & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            secteur = secteur & " GROUP BY s.sector_Level4 ORDER BY SUM(p.weight) DESC"

            Dim country As String = "SELECT c.name as 'Country', SUM(p.weight) As Weight FROM S2_PORT_COMPO p"
            country = country & " LEFT OUTER JOIN S2_UNIVERS s ON s.report_date='" & CbDateCharger.Text & "' and p.isin=s.isin"
            country = country & " LEFT OUTER JOIN ref.country c ON c.iso2=s.Country_Issue_ISO "
            country = country & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            country = country & " GROUP BY c.name ORDER BY SUM(p.weight) DESC"

            Dim tier As String = "SELECT s.Debt, s.Tier , SUM(p.weight) As Weight FROM S2_PORT_COMPO p"
            tier = tier & " LEFT OUTER JOIN S2_UNIVERS s ON s.report_date='" & CbDateCharger.Text & "' and p.isin=s.isin"
            tier = tier & " WHERE p.date='" & CbDateCharger.Text & "' and p.portfolio=" & id
            tier = tier & " GROUP BY s.debt, s.tier ORDER BY SUM(p.weight) DESC"

            DTitre.DataSource = co.LoadDataGridByString(titre)
            DTitre.Columns("Yield").DefaultCellStyle.Format = "#0.##\%"
            DTitre.Columns("Coupon").DefaultCellStyle.Format = "#0.##\%"
            DTitre.Columns("Weight").DefaultCellStyle.Format = "n2"
            'DTitre.Columns("duration").DefaultCellStyle.Format = "n2"
            DTitre.Columns("life").DefaultCellStyle.Format = "n2"
            DTitre.Columns("Sensi").DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DTitre)
            DEmetteur.DataSource = co.LoadDataGridByString(emetteur)
            DEmetteur.DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DEmetteur)
            DRating.DataSource = co.LoadDataGridByString(rating)
            DRating.DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DRating)
            DSecteur.DataSource = co.LoadDataGridByString(secteur)
            DSecteur.DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DSecteur)
            DCountry.DataSource = co.LoadDataGridByString(country)
            DCountry.DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DCountry)
            DTier.DataSource = co.LoadDataGridByString(tier)
            DTier.DefaultCellStyle.Format = "n2"
            da.AutoFiltre(DTier)
        End If
    End Sub

    Private Sub BComposition_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BComposition.Click
        Composition()
    End Sub

    Private Sub rbSave_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbSave.CheckedChanged
        If rbSave.Checked = True Then
            TSave.ReadOnly = False
        Else
            TSave.ReadOnly = True
        End If
    End Sub

    Private Sub DPortfolio_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DPortfolio.CurrentCellChanged
        Composition()
    End Sub

    Private Sub BImportFichier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BImportFichier.Click
        Dim isin As String = excel.CellFichierExcel(My.Settings.PATH & "\INPUT\SOLVENCY\", CbFichier.Text, 1, 17, 3)
        Dim report_date As DateTime = excel.CellFichierExcel(My.Settings.PATH & "\INPUT\SOLVENCY\", CbFichier.Text, 1, 17, 5)

        If (co.SelectWhere2("S2_UNIVERS", "isin", "isin", isin, "report_date", report_date, 1).Count = 0) Then
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            excel.ExcelToSql(My.Settings.PATH & "\INPUT\SOLVENCY", CbFichier.Text, 1, "S2_UNIVERS", 16, 2)
            CbDateSimulation.DataSource = co.SelectDistinctSimple("S2_UNIVERS", "report_date", "DESC")
            Windows.Forms.Cursor.Current = Cursors.Default
        Else
            MessageBox.Show("Le fichier " & CbFichier.Text & " est déjà dans la base", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If


    End Sub


End Class