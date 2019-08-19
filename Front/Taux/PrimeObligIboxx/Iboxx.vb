Imports System.IO
Imports WindowsApplication1
Imports WindowsApplication1.Referentiel

Namespace Taux.PrimeObligIboxx

    Public Class Iboxx

        Dim log As Login = New Login()
        Dim lo As Log = New Log()
        Dim co As Connection = New Connection()
        Dim excel As Excel = New Excel()
        Dim da As DGrid = New DGrid()

        Dim colName As List(Of String) = New List(Of String)
        Dim donnee As List(Of Object) = New List(Of Object)
        Dim pathiBoxx As String = My.Settings.PATH & "\INPUT\TAUX\IBOXX"

        Dim paysRisk As String = "SELECT c.isin, c.iso2, p.libelle_anglais into #tmp FROM TX_IBOXX_CNTRY_OF_RISK c LEFT OUTER JOIN pays p ON c.iso2=p.iso2      UPDATE TX_IBOXX SET Country = t.libelle_anglais FROM TX_IBOXX i, #tmp t WHERE i.isin = t.isin AND i.country <> t.libelle_anglais DROP TABLE #tmp"

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub Iboxx_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Tentative de connection : mabase + omega
            co.ToConnectBase()

            'Mettre les dates en gras que l'on peut observe
            MonthCalendar.MaxDate = DateTime.Now
            For Each f In co.SelectDistinctSimple("TX_IBOXX", "CONVERT(VARCHAR,date,103)", "DESC")
                MonthCalendar.AddBoldedDate(f)
            Next
            MonthCalendar.UpdateBoldedDates()

            CbSpread.DataSource = New List(Of String)(New String() {"Rating", "Pays", "Dette", "Maturité"})

            BExcel.Enabled = False
            BIboxx.Enabled = False
        End Sub

        ''' <summary>
        ''' CbFin : click dynamique
        ''' </summary>
        Private Sub CbFin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbFin.Click
            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbDébut.Text).Count > 0 Then
                'CbFin.DataSource = co.SelectDistinctWhere("TX_IBOXX", "CONVERT(VARCHAR,date,106)", "date", CbDébut.Text, ">")
                CbFin.DataSource = co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbDébut.Text, ">", "DESC")
            End If
        End Sub

        ''' <summary>
        ''' CbDébut : click dynamique
        ''' </summary>
        Private Sub CbDébut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDébut.Click
            'CbDébut.DataSource = co.SelectDistinctSimple("TX_IBOXX", "CONVERT(VARCHAR,date,106)")
            CbDébut.DataSource = co.SelectDistinctSimple("TX_IBOXX", "date", "DESC")
        End Sub

        Private Sub CbDateJ2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbADateJ2.Click
            CbADateJ2.DataSource = co.SelectDistinctSimple("TX_IBOXX", "date", "DESC")
        End Sub

        Private Sub CbDateJ1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbADateJ1.Click
            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbADateJ2.Text).Count > 0 Then
                CbADateJ1.DataSource = co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbADateJ2.Text, ">", "DESC")
            End If
        End Sub

        Private Sub CbDateJ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbADateJ.Click
            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbADateJ2.Text).Count > 0 And co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbADateJ1.Text).Count > 0 Then
                CbADateJ.DataSource = co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbADateJ1.Text, ">", "DESC")
            End If
        End Sub

        Private Sub CbPDateJ1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPDateJ1.Click
            CbPDateJ1.DataSource = co.SelectDistinctSimple("TX_IBOXX", "date", "DESC")
        End Sub

        Private Sub CbPDateJ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPDateJ.Click
            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbPDateJ1.Text).Count > 0 Then
                CbPDateJ.DataSource = co.SelectDistinctWhere("TX_IBOXX", "date", "date", CbPDateJ1.Text, ">", "DESC")
            End If
        End Sub

        ''' <summary>
        ''' Resize de l'ihm
        ''' </summary>
        Private Sub Iboxx_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Dim Feuille As Form
            Feuille = Me

            'If (Feuille.WindowState = WindowState.Minimized) Then
            'Exit Sub
            'End If

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
        ''' sqlAggre : Binde la DataGridViewAggr 
        ''' </summary>
        Private Function sqlAggre(ByVal debut As DateTime)
            Dim aggregation As String = String.Empty
            Dim c As Object
            For Each c In Me.GpAggrégation.Controls
                If TypeOf (c) Is RadioButton Then
                    Dim rb As RadioButton = CType(c, RadioButton)
                    If rb.Checked = True Then
                        aggregation = rb.Name
                        Exit For
                    End If
                End If
            Next

            Dim sql As String = "SELECT " & aggregation & ", ROUND(SUM(IndexWeight)/100,4) As 'Poids' FROM TX_IBOXX WHERE date='" & debut & "' GROUP BY " & aggregation & " ORDER BY poids"
            Return sql
        End Function

        ''' <summary>
        ''' GetFiltre : remplie 2 List en fonction des filtres 
        ''' </summary>
        Public Sub GetFiltre()
            'Recupération des differents filtres
            'If co.SelectDistinctWhere("TX_IBOXX", "date", "date", MonthCalendar.SelectionRange.Start.Date).Count > 0 Then

            colName.Add("date")
            donnee.Add(MonthCalendar.SelectionRange.Start.Date)

            If String.IsNullOrEmpty(CbLevel4.Text) = False Then
                colName.Add("level4")
                donnee.Add(CbLevel4.Text)
            End If
            If String.IsNullOrEmpty(TLibelle.Text) = False Then
                colName.Add("IssuerName")
                donnee.Add(" LIKE '%" & TLibelle.Text & "%'")
            End If
            If String.IsNullOrEmpty(CbCountry.Text) = False Then
                colName.Add("Country")
                donnee.Add(CbCountry.Text)
            End If
            If String.IsNullOrEmpty(CbRating.Text) = False Then
                colName.Add("rating")
                donnee.Add(CbRating.Text)
            End If
            If String.IsNullOrEmpty(CbTier.Text) = False Then
                colName.Add("tier")
                donnee.Add(CbTier.Text)
            End If
            If String.IsNullOrEmpty(CbDebt.Text) = False Then
                colName.Add("debt")
                donnee.Add(CbDebt.Text)
            End If
            'End If
        End Sub

        ''' <summary>
        ''' GetFiltre : remplie 2 List en fonction des filtres 
        ''' </summary>
        Public Function sqlIboxx(ByVal colWhere As List(Of String), ByVal donnee As List(Of Object)) As String
            Dim sql, type As String
            sql = "SELECT * FROM TX_IBOXX WHERE "

            For i = 0 To colWhere.Count - 1 Step 1
                type = TypeName(donnee(i))
                sql = sql & colWhere(i)
                If (Split(donnee(i), "LIKE")(0) <> " ") Then
                    sql = sql & " = "
                End If
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                sql = sql & donnee(i)
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                If (i <> colWhere.Count - 1) Then
                    sql = sql & " AND "
                End If
            Next

            Return sql
        End Function

        ''' <summary>
        ''' CbLevel4 : click dynamique 
        ''' </summary>
        Private Sub CbLevel4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbLevel4.Click
            GetFiltre()
            CbLevel4.DataSource = co.SelectDistinctWheres("TX_IBOXX", "Level4", colName, donnee)
            colName.Clear()
            donnee.Clear()
        End Sub

        ''' <summary>
        ''' CbPays : click dynamique 
        ''' </summary>
        Private Sub CbPays_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbCountry.Click
            GetFiltre()
            CbCountry.DataSource = co.SelectDistinctWheres("TX_IBOXX", "country", colName, donnee)
            colName.Clear()
            donnee.Clear()
        End Sub

        ''' <summary>
        ''' CbTier : click dynamique 
        ''' </summary>
        Private Sub CbTier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTier.Click
            GetFiltre()
            CbTier.DataSource = co.SelectDistinctWheres("TX_IBOXX", "Tier", colName, donnee)
            colName.Clear()
            donnee.Clear()
        End Sub

        ''' <summary>
        ''' CbRating : click dynamique 
        ''' </summary>
        Private Sub CbRating_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbRating.Click
            GetFiltre()
            CbRating.DataSource = co.SelectDistinctWheres("TX_IBOXX", "Rating", colName, donnee)
            colName.Clear()
            donnee.Clear()
        End Sub

        ''' <summary>
        ''' CbDebt : click dynamique 
        ''' </summary>
        Private Sub CbDebt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbDebt.Click
            GetFiltre()
            CbDebt.DataSource = co.SelectDistinctWheres("TX_IBOXX", "Debt", colName, donnee)
            colName.Clear()
            donnee.Clear()
        End Sub

        ''' <summary>
        ''' BExcel : écrire DataGridIboxx dans .csv
        ''' </summary>
        Private Sub BExcel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcel.Click
            da.DataGridToExcel(DataGridIboxx, SaveFileDialog, Me)
        End Sub

        ''' <summary>
        ''' BRapport : étude différence Iboxx et Prime Oblig 
        ''' </summary>
        Private Sub BRapport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRapport.Click
            Dim colNames As List(Of String) = New List(Of String)
            Dim donnees As List(Of Object) = New List(Of Object)
            colNames.Add("dateinventaire")
            colNames.Add("compte")
            donnees.Add(MonthCalendar.SelectionRange.Start.Date)
            donnees.Add("6100034")

            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", MonthCalendar.SelectionRange.Start.Date).Count > 0 Then
                'Remplir Cash et Prime oblig dans PTF_FGA
                If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count = 0 Then
                    Dim gt As GestionTable = New GestionTable()
                    gt.PortefeuilleFGA(MonthCalendar.SelectionRange.Start.Date, "PTF_FGA.sql", New String() {"6100034"})
                End If

                If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count > 0 Then
                    SaveFileDialog.FileName = "PO " & Replace(MonthCalendar.SelectionRange.Start.Date, "/", "-")
                    If SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                        'Faire le traitement dans TX_IBOXX_RAPPORT
                        colNames.Clear()
                        colNames.Add("@dateinventaire")
                        colNames.Add("@compte")
                        co.ProcedureStockée("IboxxRapport", colNames, donnees)

                        Try
                            Windows.Forms.Cursor.Current = Cursors.WaitCursor

                            'Rapport prime oblig
                            Dim sql1 As String = " SELECT niveau,code_titre As 'Code titre',libelle_titre As 'Libelle titre',"
                            sql1 = sql1 & " nominal As 'Nominal',"
                            sql1 = sql1 & " tx_rdt As 'Tx facial %',"
                            sql1 = sql1 & " cours As 'Cours close',"
                            sql1 = sql1 & " coupon_couru As 'Coupon couru',"
                            sql1 = sql1 & " evaluation As 'Evaluation',"
                            sql1 = sql1 & " vie_residuelle As 'Vie résiduelle',"
                            sql1 = sql1 & " duration As 'Duration',"
                            sql1 = sql1 & " sensibilite As 'Sensibilité',"
                            sql1 = sql1 & " rating As 'Rating',"
                            sql1 = sql1 & " actif As 'Actif %',"
                            sql1 = sql1 & " apport_sensibilite As 'Apport sensibilité'"
                            sql1 = sql1 & " FROM TX_IBOXX_RAPPORT_PRIME where date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                            sql1 = sql1 & " ORDER BY vie_residuelle"

                            'Rapport dette
                            Dim sql2 As String = "SELECT niveau,level2 As 'Secteur iBoxx niveau 2',Level4 As 'Secteur iBoxx niveau 4',debt As 'Type de dette',country As 'Pays',ROUND(poids_Iboxx,2) As 'IBOXX %',"
                            sql2 = sql2 & " poids_prime_oblig As 'Prime oblig %',"
                            sql2 = sql2 & " ecart_poids As 'Ecart %',"
                            sql2 = sql2 & " apport_Iboxx As 'Apport sensi IBOXX',"
                            sql2 = sql2 & " apport_prime_oblig As 'Apport sensi Prime oblig',"
                            sql2 = sql2 & " ecart_apport As 'Ecart apport sensi'"
                            sql2 = sql2 & " FROM TX_IBOXX_RAPPORT where date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                            sql2 = sql2 & " ORDER BY level2,level4,debt,country"

                            'Rapport rating
                            Dim sql3 As String = "SELECT niveau,groupe As 'Groupe',rating As 'Rating',"
                            sql3 = sql3 & " poids_Iboxx As 'IBOXX %',"
                            sql3 = sql3 & " poids_prime_oblig As 'Prime oblig %',"
                            sql3 = sql3 & " ecart_poids As 'Ecart %',"
                            sql3 = sql3 & " apport_Iboxx As 'Apport sensi IBOXX',"
                            sql3 = sql3 & " apport_prime_oblig As 'Apport sensi Prime oblig',"
                            sql3 = sql3 & " ecart_apport As 'Ecart apport sensi'"
                            sql3 = sql3 & " FROM TX_IBOXX_RAPPORT_RATING where date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                            sql3 = sql3 & " ORDER BY groupe,Rating"

                            'Rapport strates
                            Dim sql4 As String = "SELECT niveau,strates As 'Strates',"
                            sql4 = sql4 & " poids_Iboxx As 'IBOXX %',"
                            sql4 = sql4 & " poids_prime_oblig As 'Prime oblig %',"
                            sql4 = sql4 & " ecart_poids As 'Ecart %',"
                            sql4 = sql4 & " apport_Iboxx As 'Apport sensi IBOXX',"
                            sql4 = sql4 & " apport_prime_oblig As 'Apport sensi Prime oblig',"
                            sql4 = sql4 & " ecart_apport As 'Ecart apport sensi'"
                            sql4 = sql4 & " FROM TX_IBOXX_RAPPORT_STRATES where date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                            sql4 = sql4 & " ORDER BY strates"

                            'Rapport emission fga
                            Dim sql5 As String = "SELECT niveau, "
                            sql5 = sql5 & " emetteur As 'Emetteur',"
                            sql5 = sql5 & " isin_titre As 'Isin',"
                            sql5 = sql5 & " emission AS 'Emission',"
                            sql5 = sql5 & " pays_risque  As 'Pays de Risque',"
                            sql5 = sql5 & " poids_prime_oblig  As 'Prime oblig %',"
                            sql5 = sql5 & " poids_iboxx  As 'IBOXX %',"
                            sql5 = sql5 & " ecart_poids As 'Ecart %',"
                            sql5 = sql5 & " apport_prime_oblig  As 'Apport sensi Prime oblig',"
                            sql5 = sql5 & " apport_iboxx  As 'Apport sensi IBOXX',"
                            sql5 = sql5 & " ecart_apport As 'Ecart apport sensi'"
                            sql5 = sql5 & " FROM TX_IBOXX_RAPPORT_EMETTEUR"
                            sql5 = sql5 & " WHERE date='" & MonthCalendar.SelectionRange.Start.Date & "' "
                            sql5 = sql5 & " order by emetteur, emission"

                            'Rapport emetteur iboxx
                            Dim sql6 As String = "SELECT niveau,"
                            sql6 = sql6 & " emetteur As 'Emetteur',"
                            sql6 = sql6 & " isin_titre As 'Isin',"
                            sql6 = sql6 & " emission As 'Emission',"
                            sql6 = sql6 & " pays_risque  As 'Pays de Risque',"
                            sql6 = sql6 & " poids_iboxx As 'IBOXX %',"
                            sql6 = sql6 & " poids_prime_oblig As 'Prime oblig %',"
                            sql6 = sql6 & " ecart_poids As 'Ecart %',"
                            sql6 = sql6 & " apport_iboxx As 'Apport sensi IBOXX',"
                            sql6 = sql6 & " apport_prime_oblig As 'Apport sensi Prime oblig',"
                            sql6 = sql6 & " ecart_apport As 'Ecart apport sensi'"
                            sql6 = sql6 & " FROM TX_IBOXX_RAPPORT_EMETTEUR2"
                            sql6 = sql6 & " WHERE date='" & MonthCalendar.SelectionRange.Start.Date & "'"
                            sql6 = sql6 & " ORDER by emetteur"

                            'Rapport Pays
                            Dim sql7 As String = "SELECT niveau As 'Niveau',zone As 'Zone',"
                            sql7 = sql7 & " pays As 'Pays',"
                            sql7 = sql7 & " poids_iboxx 'IBOXX %',"
                            sql7 = sql7 & " CASE WHEN poids_prime_oblig IS NULL THEN 0 ELSE poids_prime_oblig END As 'Prime oblig %',"
                            sql7 = sql7 & " CASE WHEN ecart_poids IS NULL THEN -poids_iboxx ELSE ecart_poids END AS 'Ecart poids',"
                            sql7 = sql7 & " apport_iboxx AS 'Apport sensi IBOXX',"
                            sql7 = sql7 & " CASE WHEN apport_prime_oblig IS NULL THEN 0 ELSE apport_prime_oblig END As 'Apport sensi Prime oblig',"
                            sql7 = sql7 & " CASE WHEN ecart_apport IS NULL THEN -apport_iboxx ELSE ecart_apport END AS 'Ecart apport sensi'"
                            sql7 = sql7 & " FROM TX_IBOXX_RAPPORT_PAYS"
                            sql7 = sql7 & " Where date = '" & MonthCalendar.SelectionRange.Start.Date & "' "
                            sql7 = sql7 & " ORDER BY Zone,pays"

                            Dim sql As List(Of String) = New List(Of String)
                            sql.Add(sql1)
                            sql.Add(sql2)
                            sql.Add(sql3)
                            sql.Add(sql4)
                            sql.Add(sql5)
                            sql.Add(sql6)
                            sql.Add(sql7)

                            co.SqlToExcelEndSave(sql, SaveFileDialog.FileName)
                            PresentationRapport(SaveFileDialog.FileName, MonthCalendar.SelectionRange.Start.Date)
                            lo.Log(ELog.Information, "BRapport_Click", "Création rapport iboxx ")
                            Windows.Forms.Cursor.Current = Cursors.Default
                        Catch ex As Exception
                            MessageBox.Show("Un problème est survenu ! (fichier déjà ouvert " & SaveFileDialog.FileName & " ?)", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End Try
                    End If
                Else
                    MessageBox.Show("La table PTF_FGA ne contient pas de données au " & MonthCalendar.SelectionRange.Start.Date & " pour le compte 6100034", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Refaire la présentation du fichier excel
        ''' </summary>
        Public Sub PresentationRapport(ByVal chemin As String, ByVal datee As DateTime)
            Dim app As Microsoft.Office.Interop.Excel.Application = CreateObject("Excel.Application")
            Dim book As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(chemin)
            app.DisplayAlerts = False 'annule les messages
            Dim nbrLigne As Integer


            'PRESENTATION PRIME OBLIG
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = book.Worksheets(1)
            sheet.Activate()
            sheet.Rows("2:2").Select()
            app.ActiveWindow.FreezePanes = True
            sheet.Rows(1).Autofilter()
            Dim tx As Double = 0
            Dim totE7 As Double = 0
            Dim totF8 As Double = 0
            Dim totM13 As Double = 0
            Dim totN14 As Double = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            sheet.Columns(12).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "vie"
                        totE7 = totE7 + sheet.Cells(i, 7).Value()
                        totF8 = totF8 + sheet.Cells(i, 8).Value()
                        totM13 = totM13 + sheet.Cells(i, 13).Value()
                        totN14 = totN14 + sheet.Cells(i, 14).Value()
                        sheet.Range("A" & i & ":" & "N" & i).Interior.ColorIndex = 11 'fond bleu
                        sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                        'sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True 'gras
                    Case "titre"
                        tx = tx + sheet.Cells(i, 5).Value() * sheet.Cells(i, 8).Value()
                        'Case Else
                        'sheet.Rows(i).Font.Size = 11 'taille 13
                End Select
            Next
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                '.Font.Size = 11
                .Interior.ColorIndex = 2
                .RowHeight = 35
                .WrapText = True
            End With
            'With sheet.PageSetup
            '    .PrintArea = "$B:$N"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse Prime Oblig " & datee
            'End With
            sheet.Range("A1:N1").Interior.ColorIndex = 15
            sheet.Name = "Prime oblig"
            'ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 5).Value() = tx / totF8
            'SELECT SUM(couponRate*indexweight)/100 FROM TX_IBOXX where date='24/08/2011'
            ' la cellule contient le taux YTM moyen pour l IBOXX
            '        sheet.Cells(nbrLigne + 2, 5).Value() = co.SelectWhere("TX_IBOXX", "SUM(indexWeight*AnnualYield)/100", "date", MonthCalendar.SelectionRange.Start.Date).FirstOrDefault
            ' Taux facial moyen de l iBOXX
            sheet.Cells(nbrLigne + 2, 4).Value() = "iBoxx:"
            sheet.Cells(nbrLigne + 2, 5).Value() = co.SelectWhere("TX_IBOXX", "SUM(indexWeight*couponRate)/100", "date", MonthCalendar.SelectionRange.Start.Date).FirstOrDefault

            sheet.Cells(nbrLigne + 1, 7).Value() = totE7
            sheet.Cells(nbrLigne + 1, 8).Value() = totF8
            sheet.Cells(nbrLigne + 1, 13).Value() = totM13
            sheet.Cells(nbrLigne + 1, 14).Value() = totN14
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "N" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            sheet.Columns("D:D").NumberFormat = "# ### ##0" 'Nominal
            sheet.Columns("E:G").NumberFormat = "# ##0.00"   ' prix et coupon couru
            sheet.Columns("H:H").NumberFormat = "# ### ##0"  'Evaluation
            sheet.Columns("I:N").NumberFormat = "# ### ##0.00"
            sheet.Cells.Font.Size = 9
            sheet.Cells.EntireColumn.AutoFit()
            sheet.Columns(1).EntireColumn.Hidden = True
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE PRIME OBLIG : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("B1:N1").Interior.ColorIndex = 37


            'PRESENTATION TYPE DE DETTE
            sheet = book.Worksheets(2)
            sheet.Activate()
            sheet.Rows("2:2").Select()
            app.ActiveWindow.FreezePanes = True
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            Dim totF6 As Double = 0
            Dim totG7 As Double = 0
            Dim totI9 As Double = 0
            Dim totJ10 As Double = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            'Colonne pays à droite et italic
            sheet.Columns(5).Font.Italic = True
            sheet.Columns(5).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight
            For i As Integer = 2 To nbrLigne - 4
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "level2"
                        sheet.Range("A" & i & ":" & "K" & i).Interior.ColorIndex = 11 'fond bleu
                        sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                        'sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True 'gras
                        totF6 = totF6 + sheet.Cells(i, 6).Value()
                        totG7 = totG7 + sheet.Cells(i, 7).Value()
                        totI9 = totI9 + sheet.Cells(i, 9).Value()
                        totJ10 = totJ10 + sheet.Cells(i, 10).Value()
                        If sheet.Cells(i, 7).Value() = 0 Then
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 10).Value() = ""
                        End If
                    Case "level4"
                        sheet.Rows(i).Font.colorindex = 11
                        'sheet.Rows(i).Font.Size = 12
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                        sheet.Range("C" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        sheet.Range("C" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        If sheet.Cells(i, 7).Value() = 0 Then
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 10).Value() = ""
                        End If
                    Case "tier"
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                        sheet.Cells(i, 3).Font.colorindex = 2 'caractere blanc
                        'sheet.Rows(i).Font.Size = 11
                        sheet.Range("D" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        sheet.Range("D" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        sheet.Range("D" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        If sheet.Cells(i, 7).Value() = 0 Then
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 10).Value() = ""
                        End If
                    Case "country"
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                        sheet.Cells(i, 3).Font.colorindex = 2 'caractere blanc
                        sheet.Cells(i, 4).Font.colorindex = 2 'caractere blanc
                        sheet.Range("E" & i & ":" & "K" & i).Interior.ColorIndex = 34
                        sheet.Range("E" & i & ":" & "K" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        'sheet.Rows(i).Font.Size = 10.5
                        If sheet.Cells(i, 7).Value() = 0 Then
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 10).Value() = ""
                        End If
                    Case Else
                        'PENSEZ A DIMINUER LE FOR SI NEW ELSE (3)
                        totF6 = totF6 + sheet.Cells(i, 6).Value()
                        totG7 = totG7 + sheet.Cells(i, 7).Value()
                        totI9 = totI9 + sheet.Cells(i, 9).Value()
                        totJ10 = totJ10 + sheet.Cells(i, 10).Value()

                        sheet.Range("A" & i & ":" & "K" & i).Interior.ColorIndex = 11 'fond bleu
                        sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                        'sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True
                        If sheet.Cells(i, 7).Value() = 0 Then
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 10).Value() = ""
                        End If
                        sheet.Rows(i).Cut()
                        i = i - 1
                        sheet.Rows(nbrLigne + 1).Insert(Microsoft.Office.Interop.Excel.XlDirection.xlDown)
                        nbrLigne = nbrLigne - 1
                End Select
            Next
            'Cacher la colonne aide
            sheet.Columns(1).EntireColumn.Hidden = True
            'With sheet.PageSetup
            '    .PrintArea = "$B:$K"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    '.TopMargin = 36
            '    '.BottomMargin = 36
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse type de dette : iBoxx Corporate - Prime Oblig " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                '.Font.Size = 11
                .Interior.ColorIndex = 2
                .RowHeight = 35
                .WrapText = True
            End With
            sheet.Range("A1:K1").Interior.ColorIndex = 15
            sheet.Columns("B:K").ColumnWidth = 15
            'app.Cells.EntireColumn.AutoFit()
            'Ligne total
            sheet.Rows(nbrLigne + 4).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 4, 6).Value() = totF6
            sheet.Cells(nbrLigne + 4, 7).Value() = totG7
            sheet.Cells(nbrLigne + 4, 9).Value() = totI9
            sheet.Cells(nbrLigne + 4, 10).Value() = totJ10
            sheet.Cells(nbrLigne + 4, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 4 & ":" & "K" & nbrLigne + 4).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 4).Font.Size = 14
            sheet.Rows(nbrLigne + 4).Font.Bold = True
            sheet.Columns("F:K").NumberFormat = "## ##0.00"
            sheet.Cells.Font.Size = 9
            sheet.Name = "Type dette"
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE TYPE DE DETTE : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("B1:K1").Interior.ColorIndex = 37



            'PRESENTATION RATING
            sheet = book.Worksheets(3)
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            Dim totD4 As Double = 0
            Dim totE5 As Double = 0
            totG7 = 0
            Dim totH8 As Double = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "tot_rating", "liquidités", "futures"
                        sheet.Range("A" & i & ":" & "I" & i).Interior.ColorIndex = 11 'fond bleu
                        sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                        sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True 'gras
                        totD4 = totD4 + sheet.Cells(i, 4).Value()
                        totE5 = totE5 + sheet.Cells(i, 5).Value()
                        totG7 = totG7 + sheet.Cells(i, 7).Value()
                        totH8 = totH8 + sheet.Cells(i, 8).Value()
                        If sheet.Cells(i, 4).Value() = 0 Then
                            sheet.Cells(i, 4).Value() = ""
                            sheet.Cells(i, 6).Value() = ""
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 9).Value() = ""
                        End If
                    Case "rating"
                        sheet.Rows(i).Font.colorindex = 11 'caractere bleu
                        sheet.Rows(i).Font.Size = 12
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                        sheet.Range("C" & i & ":" & "I" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        sheet.Range("C" & i & ":" & "I" & i).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        If sheet.Cells(i, 4).Value() = 0 Then
                            sheet.Cells(i, 4).Value() = ""
                            sheet.Cells(i, 6).Value() = ""
                            sheet.Cells(i, 7).Value() = ""
                            sheet.Cells(i, 9).Value() = ""
                        End If
                End Select
            Next
            'Cacher la colonne aide
            sheet.Columns(1).EntireColumn.Hidden = True
            'With sheet.PageSetup
            '    .PrintArea = "$B:$I"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse rating : iBoxx Corporate - Prime Oblig " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                .Font.Size = 11
                .Interior.ColorIndex = 2
                .RowHeight = 35
                .WrapText = True
            End With
            'Ajouter ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 4).Value() = totD4
            sheet.Cells(nbrLigne + 1, 5).Value() = totE5
            sheet.Cells(nbrLigne + 1, 7).Value() = totG7
            sheet.Cells(nbrLigne + 1, 8).Value() = totH8
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "I" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            sheet.Range("A1:I1").Interior.ColorIndex = 15
            sheet.Columns("B:I").ColumnWidth = 15
            sheet.Columns("D:I").NumberFormat = "## ##0.00"
            sheet.Name = "Rating"
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE RATING : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("B1:I1").Interior.ColorIndex = 37




            'PRESENTATION STRATES
            sheet = book.Worksheets(4)
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            Dim totC3 As Double = 0
            totD4 = 0
            totF6 = 0
            totG7 = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                sheet.Range("A" & i & ":" & "H" & i).Interior.ColorIndex = 11 'fond bleu
                sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                sheet.Rows(i).Font.Size = 13 'taille 13
                sheet.Rows(i).Font.Bold = True 'gras
                totC3 = totC3 + sheet.Cells(i, 3).Value()
                totD4 = totD4 + sheet.Cells(i, 4).Value()
                totF6 = totF6 + sheet.Cells(i, 6).Value()
                totG7 = totG7 + sheet.Cells(i, 7).Value()
            Next
            'Ajouter ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 3).Value() = totC3
            sheet.Cells(nbrLigne + 1, 4).Value() = totD4
            sheet.Cells(nbrLigne + 1, 6).Value() = totF6
            sheet.Cells(nbrLigne + 1, 7).Value() = totG7
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "H" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            'Cacher la colonne aide
            sheet.Columns(1).EntireColumn.Hidden = True
            'With sheet.PageSetup
            '    .PrintArea = "$B:$H"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse strates : iBoxx Corporate - Prime Oblig " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                .Font.Size = 11
                .Interior.ColorIndex = 2
                .RowHeight = 35
                .WrapText = True
            End With
            sheet.Range("A1:H1").Interior.ColorIndex = 15
            sheet.Columns("B:H").ColumnWidth = 15
            sheet.Columns("C:H").NumberFormat = "## ##0.00"
            sheet.Name = "Strates"
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE STRATES : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("B1:H1").Interior.ColorIndex = 37


            'PRESENTATION EMISSION 
            sheet = book.Worksheets(5)
            sheet.Activate()
            sheet.Rows("2:2").Select()
            app.ActiveWindow.FreezePanes = True
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            totE5 = 0
            totF6 = 0
            totH8 = 0
            totI9 = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "emetteur"
                        sheet.Range("A" & i & ":" & "J" & i).Interior.ColorIndex = 34 'fond bleu
                        sheet.Rows(i).Font.colorindex = 11 'caractere blanc
                        'sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True 'gras
                        totE5 = totE5 + sheet.Cells(i, 5).Value()
                        totF6 = totF6 + sheet.Cells(i, 6).Value()
                        totH8 = totH8 + sheet.Cells(i, 8).Value()
                        totI9 = totI9 + sheet.Cells(i, 9).Value()
                    Case "emission"
                        'sheet.Rows(i).Font.colorindex = 11 'caractere bleu
                        'sheet.Rows(i).Font.Size = 12
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                End Select
            Next
            'Cacher la colonne aide
            sheet.Columns(1).EntireColumn.Hidden = True
            'With sheet.PageSetup
            '    .PrintArea = "$B:$J"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse Emission " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                '.Font.Size = 11
                .Interior.ColorIndex = 2
                .WrapText = True
            End With
            'Ajouter ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 5).Value() = totE5
            sheet.Cells(nbrLigne + 1, 6).Value() = totF6
            sheet.Cells(nbrLigne + 1, 8).Value() = totH8
            sheet.Cells(nbrLigne + 1, 9).Value() = totI9
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "J" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            sheet.Range("A1:J1").Interior.ColorIndex = 15
            sheet.Name = "Emission"
            sheet.Columns("E:G").NumberFormat = "## ##0.00"
            sheet.Columns("H:J").NumberFormat = "## ##0.000"
            sheet.Cells.Font.Size = 9
            sheet.Cells.EntireColumn.AutoFit()
            sheet.Columns(1).EntireColumn.Hidden = True
            sheet.Columns("H:H").insert()
            sheet.Columns("H:H").Interior.ColorIndex = 2
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE EMISSION : Prime Oblig - iBoxx Corporate " & datee
            sheet.Range("B1:K1").Interior.ColorIndex = 37




            'PRESENTATION EMETTEUR
            sheet = book.Worksheets(6)
            sheet.Activate()
            sheet.Rows("2:2").Select()
            app.ActiveWindow.FreezePanes = True
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            totE5 = 0
            totF6 = 0
            totH8 = 0
            totI9 = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "emetteur"
                        sheet.Range("A" & i & ":" & "J" & i).Interior.ColorIndex = 34 'fond bleu
                        sheet.Rows(i).Font.colorindex = 11 'caractere bleu
                        'sheet.Rows(i).Font.Size = 13 'taille 13
                        sheet.Rows(i).Font.Bold = True 'gras
                        totE5 = totE5 + sheet.Cells(i, 5).Value()
                        totF6 = totF6 + sheet.Cells(i, 6).Value()
                        totH8 = totH8 + sheet.Cells(i, 8).Value()
                        totI9 = totI9 + sheet.Cells(i, 9).Value()
                    Case "emission"
                        'sheet.Rows(i).Font.colorindex = 11 'caractere bleu
                        'sheet.Rows(i).Font.Size = 12
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                End Select
            Next
            'Cacher la colonne aide
            sheet.Columns(1).EntireColumn.Hidden = True
            'With sheet.PageSetup
            '    .PrintArea = "$B:$J"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse Emission " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                '.Font.Size = 11
                .Interior.ColorIndex = 2
                .WrapText = True
            End With
            'Ajouter ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 5).Value() = totE5
            sheet.Cells(nbrLigne + 1, 6).Value() = totF6
            sheet.Cells(nbrLigne + 1, 8).Value() = totH8
            sheet.Cells(nbrLigne + 1, 9).Value() = totI9
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "J" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            sheet.Range("A1:J1").Interior.ColorIndex = 15
            sheet.Name = "Emetteur"
            sheet.Columns("E:G").NumberFormat = "## ##0.00"
            sheet.Columns("H:J").NumberFormat = "## ##0.000"
            sheet.Cells.Font.Size = 9
            sheet.Cells.EntireColumn.AutoFit()
            sheet.Columns(1).EntireColumn.Hidden = True
            sheet.Columns("H:H").insert()
            sheet.Columns("H:H").Interior.ColorIndex = 2
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE EMETTEUR : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("B1:K1").Interior.ColorIndex = 37





            'PRESENTATION PAYS
            sheet = book.Worksheets(7)
            sheet.Activate()
            sheet.Rows("2:2").Select()
            app.ActiveWindow.FreezePanes = True
            sheet.Rows(1).Autofilter()
            nbrLigne = 0
            totD4 = 0
            totE5 = 0
            totG7 = 0
            totH8 = 0
            'Compte nombre de ligne
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                End If
            Next
            For i As Integer = 2 To nbrLigne
                sheet.Rows(i).Interior.ColorIndex = 2
                Select Case sheet.Cells(i, 1).Value()
                    Case "zone"
                        sheet.Range("B" & i & ":" & "I" & i).Interior.ColorIndex = 11 'fond bleu
                        sheet.Rows(i).Font.colorindex = 2 'caractere blanc
                        sheet.Rows(i).Font.Bold = True 'gras
                        sheet.Cells(i, 3).Value() = " "
                        totD4 = totD4 + sheet.Cells(i, 4).Value()
                        totE5 = totE5 + sheet.Cells(i, 5).Value()
                        totG7 = totG7 + sheet.Cells(i, 7).Value()
                        totH8 = totH8 + sheet.Cells(i, 8).Value()
                    Case "pays"
                        sheet.Rows(i).Font.colorindex = 11 'caractere bleu
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                        sheet.Range("C" & i & ":" & "I" & i).Interior.ColorIndex = 34 'fonds bleu ciel
                    Case "payshorsfi", "paysfi"
                        sheet.Cells(i, 2).Font.colorindex = 2 'caractere blanc
                End Select
            Next
            'With sheet.PageSetup
            '    .PrintArea = "$A:$I"
            '    .LeftMargin = 14.17
            '    .RightMargin = 14.17
            '    .FitToPagesWide = 1
            '    .FitToPagesTall = 1
            '    .CenterHeader = "Analyse pays " & datee
            'End With
            'Titre
            With sheet.Rows(1)
                .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                .Font.Bold = True
                .Font.Italic = False
                .Font.Size = 11
                .Interior.ColorIndex = 2
                .WrapText = True
            End With
            'Ajouter ligne total
            sheet.Rows(nbrLigne + 1).Interior.ColorIndex = 2
            sheet.Cells(nbrLigne + 1, 4).Value() = totD4
            sheet.Cells(nbrLigne + 1, 5).Value() = totE5
            sheet.Cells(nbrLigne + 1, 7).Value() = totG7
            sheet.Cells(nbrLigne + 1, 8).Value() = totH8
            sheet.Cells(nbrLigne + 1, 2).Value() = "TOTAL"
            sheet.Range("A" & nbrLigne + 1 & ":" & "I" & nbrLigne + 1).Interior.ColorIndex = 41 'fond bleu
            sheet.Rows(nbrLigne + 1).Font.Size = 14
            sheet.Rows(nbrLigne + 1).Font.Bold = True
            sheet.Range("A1:I1").Interior.ColorIndex = 15
            sheet.Name = "Pays"
            sheet.Columns("D:I").NumberFormat = "## ##0.00"
            sheet.Cells.Font.Size = 9
            sheet.Cells.EntireColumn.AutoFit()
            sheet.Rows(1).insert()
            sheet.Cells(1, 2).Value() = "ANALYSE PAYS : iBoxx Corporate - Prime Oblig " & datee
            sheet.Range("A1:I1").Interior.ColorIndex = 37
            sheet.Columns(1).EntireColumn.Hidden = True

            book.Sheets(1).Activate()

            app.ActiveWorkbook.SaveAs(chemin)
            app.DisplayAlerts = True
            app.Quit()
            app = Nothing
            book = Nothing
        End Sub

        ''' <summary>
        ''' BBloomberg : écrit dans un fichier excel les oblig en commun entre prime oblig et l'iboxx 
        ''' </summary>
        Private Sub BBloomberg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BBloomberg.Click

            Dim colNames As List(Of String) = New List(Of String)
            Dim donnees As List(Of Object) = New List(Of Object)
            colNames.Add("dateinventaire")
            colNames.Add("compte")
            donnees.Add(MonthCalendar.SelectionRange.Start.Date)
            donnees.Add("6100034")

            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", MonthCalendar.SelectionRange.Start.Date).Count > 0 Then
                'Remplir Cash et Prime oblig dans PTF_FGA
                If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count = 0 Then
                    Dim gt As GestionTable = New GestionTable()
                    gt.PortefeuilleFGA(MonthCalendar.SelectionRange.Start.Date, "PTF_FGA.sql", New String() {"6100034"})
                End If

                If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count > 0 Then
                    Try
                        Windows.Forms.Cursor.Current = Cursors.WaitCursor
                        SaveFileDialog.FileName = "PO Bloomberg.xls"
                        If SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                            co.SqlToExcelExistant("EXEC IboxxBloomberg '" & MonthCalendar.SelectionRange.Start.Date & "'", My.Settings.PATH & "\INPUT\TAUX", "Prime oblig.xls", SaveFileDialog.FileName, False, 2, , MonthCalendar.SelectionRange.Start.Date)
                            lo.Log(ELog.Information, "BBloomberg_Click", "Création rapport bloomberg")
                        End If
                        Windows.Forms.Cursor.Current = Cursors.Default
                    Catch ex As Exception
                        MessageBox.Show("Un problème est survenu ! (fichier déjà ouvert " & SaveFileDialog.FileName & " ?)", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End Try
                Else
                    MessageBox.Show("La table PTF_FGA ne contient pas de données au " & MonthCalendar.SelectionRange.Start.Date & " pour le compte 6100034", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If
            Else
                MessageBox.Show("La table TX_IBOXX ne contient pas de données au " & MonthCalendar.SelectionRange.Start.Date, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' BEffacer : efface tous les filtres 
        ''' </summary>
        Private Sub BGEffacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BGEffacer.Click
            'Vider les composants onglet1
            DataGridAgrr.DataSource = Nothing
            DataGridIboxx.DataSource = Nothing
            CbLevel4.SelectedIndex = -1
            TLibelle.Clear()
            CbCountry.SelectedIndex = -1
            CbRating.SelectedIndex = -1
            CbTier.SelectedIndex = -1
            CbDebt.SelectedIndex = -1
        End Sub

        ''' <summary>
        ''' BGCharger_Click : remplit les deux datagrid de l'onglet principale
        ''' </summary>
        Private Sub BGCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BGCharger.Click
            BGCharger.Enabled = False
            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            If co.SelectDistinctWhere("TX_IBOXX", "date", "date", MonthCalendar.SelectionRange.Start.Date).Count > 0 Then
                DataGridAgrr.DataSource = co.LoadDataGridByString(sqlAggre(MonthCalendar.SelectionRange.Start.Date))
                GetFiltre()
                DataGridIboxx.DataSource = co.LoadDataGridByString(sqlIboxx(colName, donnee))
                donnee.Clear()
                colName.Clear()
            End If

            BGCharger.Enabled = True
            BExcel.Enabled = True
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        ''' <summary>
        ''' BPEffacer_Click : Vide tous les composants de l'onglet principale
        ''' </summary>
        Private Sub BPEffacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPEffacer.Click
            'Vider les composants onglet2
            CbDébut.SelectedIndex = -1
            CbFin.SelectedIndex = -1
            DataGridIn.DataSource = Nothing
            DataGridOut.DataSource = Nothing
            DataGridZSpread.DataSource = Nothing
        End Sub

        ''' <summary>
        ''' BPCharger_Click : remplit tous les composants de l'onglet péridoe
        ''' </summary>
        Private Sub BPCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPCharger.Click
            'On a 2 dates VALIDES donc on peut faire des comparaisons
            If IsDate(CbDébut.Text) And IsDate(CbFin.Text) Then
                Dim paramName As List(Of String) = New List(Of String)()
                paramName.Add("@debut")
                paramName.Add("@fin")
                Dim paramDonnee As List(Of Object) = New List(Of Object)()
                paramDonnee.Add(CbDébut.Text)
                paramDonnee.Add(CbFin.Text)
                DataGridIn.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxIn", paramName, paramDonnee))
                DataGridOut.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxOut", paramName, paramDonnee))
                DataGridZSpread.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxZSpread", paramName, paramDonnee))
            End If
        End Sub

        ''' <summary>
        ''' BAEffacer_Click : Vide tous les composants de l'onglet période
        ''' </summary>
        Private Sub BAEffacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAEffacer.Click
            'Vider les composants onglet3
            CbADateJ.SelectedIndex = -1
            CbADateJ1.SelectedIndex = -1
            CbADateJ2.SelectedIndex = -1
            DataGridASM.DataSource = Nothing
            DataGridRating.DataSource = Nothing
            DataGridPays.DataSource = Nothing
            DataGridMaturite.DataSource = Nothing
        End Sub

        ''' <summary>
        ''' BAEffacer_Click : Vide tous les composants de l'onglet période
        ''' </summary>
        Private Sub BACharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BACharger.Click
            'On a 3 dates VALIDES donc on peut faire des comparaisons
            If IsDate(CbADateJ.Text) And IsDate(CbADateJ1.Text) And IsDate(CbADateJ2.Text) Then
                Dim paramName As List(Of String) = New List(Of String)()
                paramName.Add("@dateJ")
                paramName.Add("@dateJ1")
                paramName.Add("@dateJ2")
                paramName.Add("@durAdjusted")
                Dim paramDonnee As List(Of Object) = New List(Of Object)()
                paramDonnee.Add(CbADateJ.Text)
                paramDonnee.Add(CbADateJ1.Text)
                paramDonnee.Add(CbADateJ2.Text)
                If (Me.CheckBoxDurAdjusted.Checked) Then
                    paramDonnee.Add(True)
                Else
                    paramDonnee.Add(False)
                End If
                DataGridASM.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxASM", paramName, paramDonnee))
                DataGridRating.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxRating", paramName, paramDonnee))
                DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxPays", paramName, paramDonnee))
                DataGridMaturite.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IBoxxASM_MatPillar", paramName, paramDonnee))
                DataGridASM.DefaultCellStyle.Format = "n2"
                DataGridRating.DefaultCellStyle.Format = "n2"
                DataGridPays.DefaultCellStyle.Format = "n2"
                DataGridMaturite.DefaultCellStyle.Format = "n2"
            End If
        End Sub

        ''' <summary>
        ''' BPeCharger_Click : attribution de performance
        ''' </summary>
        Private Sub BPeCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPeCharger.Click
            Dim colNames As List(Of String) = New List(Of String)
            Dim donnees As List(Of Object) = New List(Of Object)
            colNames.Add("dateinventaire")
            colNames.Add("compte")
            donnees.Add(CbPDateJ1.Text)
            donnees.Add("6100034")

            'Remplir Cash et Prime oblig dans PTF_FGA
            If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count = 0 Then
                Dim gt As GestionTable = New GestionTable()
                gt.PortefeuilleFGA(CbPDateJ1.Text, "PTF_FGA.sql", New String() {"6100034"})
            End If

            If co.SelectDistinctWheres("PTF_FGA", "dateinventaire", colNames, donnees).Count > 0 Then
                'procedure stockéé
                colNames.Clear()
                donnees.Clear()
                colNames.Add("@compte")
                colNames.Add("@dateJ1")
                colNames.Add("@dateJ")
                donnees.Add("6100034")
                donnees.Add(CbPDateJ1.Text)
                donnees.Add(CbPDateJ.Text)
                DataGridAttributionSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("IboxxAttribPerf", colNames, donnees))
                DataGridAttributionSecteur.DefaultCellStyle.Format = "n2"
                da.AutoFiltre(DataGridAttributionSecteur)
            Else
                MessageBox.Show("La table PTF_FGA ne contient pas de données au " & CbPDateJ1.Text & " pour le compte 6100034", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

        End Sub

        ''' <summary>
        ''' BAEffacer_Click : Vider ihm performance
        ''' </summary>
        Private Sub BPeEffacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPeEffacer.Click
            'Vider les composants onglet4
            CbPDateJ.SelectedIndex = -1
            CbPDateJ1.SelectedIndex = -1
            DataGridAttributionSecteur.DataSource = Nothing
        End Sub

        ''' <summary>
        ''' BExcelASM_Click : Mettre DataGridASM dans excel
        ''' </summary>
        Private Sub BExcelASM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelASM.Click
            Dim datagrid As DataGridView = Nothing

            Select Case CbSpread.Text
                Case "Pays"
                    datagrid = DataGridPays
                Case "Rating"
                    datagrid = DataGridRating
                Case "Dette"
                    datagrid = DataGridASM
                Case "Maturité"
                    datagrid = DataGridMaturite

            End Select
            Dim dateJ As String = CbADateJ.Text.Substring(6, 4) + CbADateJ.Text.Substring(3, 2) + CbADateJ.Text.Substring(0, 2)

            SaveFileDialog.CreatePrompt = True
            SaveFileDialog.Filter = "Excel (*.csv)| *.csv"


            If (CheckBoxDurAdjusted.Checked) Then
                SaveFileDialog.FileName = "ASM_" + CbSpread.Text + "_DurationAdj_" + dateJ
            Else
                SaveFileDialog.FileName = "ASM_" + CbSpread.Text + "_" + dateJ
            End If

            da.DataGridToExcel(datagrid, SaveFileDialog, Me)
        End Sub

        ''' <summary>
        ''' BExcelPerf_Click : Mettre DataGridAttributionSecteur dans excel
        ''' </summary>
        Private Sub BExcelPerf_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcelPerf.Click
            da.DataGridToExcel(DataGridAttributionSecteur, SaveFileDialog, Me)
        End Sub


        Private Sub CbIboxx_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbIboxx.Click
            CbIboxx.Items.Clear()
            For Each fichier In Directory.GetFiles(pathiBoxx, "*.xls", SearchOption.TopDirectoryOnly)
                CbIboxx.Items.Add(New IO.FileInfo(fichier).Name())
            Next
        End Sub


        Private Sub CbIboxx_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbIboxx.SelectedValueChanged
            'reset du cache. qui evite de lire 2 fois le fichier
            excel.CacheReset()
            ' Lecture du fichier dans la combobox pour recuperer la cellule de la ligne 2 , colonne 3, qui est sensée etre une date
            Dim datee As String = excel.ExcelIboxxDate(pathiBoxx, CbIboxx.Text, "constituents_20077", 1, iLigne:=1, iCol:=1)
            If IsNothing(datee) Then
                excel.CacheReset()
                datee = excel.ExcelIboxxDate(pathiBoxx, CbIboxx.Text, "constituents_20077", 1, iLigne:=7, iCol:=2)
            End If

            If IsNothing(datee) Then
                MessageBox.Show("Le Fichier n'a pas le format attendu . Une date est attendue à la colonne C")
                BIboxx.Enabled = False
            ElseIf co.SelectDistinctWhere("TX_IBOXX", "date", "Date", datee).Count = 0 Then
                BIboxx.Enabled = True
            Else
                BIboxx.Enabled = False
            End If
        End Sub

        Private Sub BIboxx_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BIboxx.Click
            Try
                Windows.Forms.Cursor.Current = Cursors.WaitCursor
                BIboxx.Enabled = False

                excel.ExcelIboxxToSql(pathiBoxx, CbIboxx.Text, "TX_IBOXX", 1, 1, 1)

                co.RequeteSql(paysRisk)


                'Mettre les dates en gras que l'on peut observe
                MonthCalendar.MaxDate = DateTime.Now
                For Each f In co.SelectDistinctSimple("TX_IBOXX", "CONVERT(VARCHAR,date,103)", "DESC")
                    MonthCalendar.AddBoldedDate(f)
                Next
                MonthCalendar.UpdateBoldedDates()

                Windows.Forms.Cursor.Current = Cursors.Default
            Catch ex As Exception
                Windows.Forms.Cursor.Current = Cursors.Default
                MessageBox.Show("Le fichier Iboxx contient des données incorrectes", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End Try
        End Sub

        Private Sub BPaysRisk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPaysRisk.Click

            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            co.DeleteFrom("TX_IBOXX_CNTRY_OF_RISK")
            excel.ExcelToSql(My.Settings.PATH & "\INPUT\TAUX", "Prime oblig pays.xls", 2, "TX_IBOXX_CNTRY_OF_RISK")
            ' ne plus ecraser la colonne country de l iboxx car on n utilise plus la colonne pour les rapport 
            'co.RequeteSql(paysRisk)

            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        Private Sub PaysOfRiskToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PaysOfRiskToolStripMenuItem.Click
            excel.LectureFichierExcel(My.Settings.PATH & "\INPUT\TAUX", "Prime oblig pays.xls", 1)
        End Sub

        Private Sub POSToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles POSToolStripMenuItem.Click
            excel.LectureFichierExcel("G:\,FGA Front Office\04_Gestion_Taux\Gestion Crédit\PRIME OBLIG", "Prime oblig spread.xls", 1)
        End Sub

        Private Sub POToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles POToolStripMenuItem.Click
            Process.Start("G:\,FGA Front Office\04_Gestion_Taux\Gestion Crédit\PRIME OBLIG")
        End Sub

        Private Sub InputToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InputToolStripMenuItem.Click
            Process.Start("G:\,FGA Soft\INPUT\TAUX\IBOXX")
        End Sub
    End Class
End Namespace