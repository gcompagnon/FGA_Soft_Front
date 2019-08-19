Imports System.IO
Imports WindowsApplication1.Taux.BaseEmetteurs

Namespace Referentiel

    Public Class Reconciliation

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()
        Dim da As DGrid = New DGrid()
        Dim excel As Excel = New Excel()
        Dim be As New BaseEmetteurs
        Private MyDataGridViewPrinter As New DataGridViewPrinter.DataGridViewPrinter(be)

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub Reconciliation_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Connection au différente base
            co.ToConnectBase()

            'DateTime.Format = DateTimePickerFormat.Custom
            'DateTime.CustomFormat = "dd/MM/yyyy"

            'Binder CbDataGrid
            CbDataGrid.Items.Clear()
            Dim list As List(Of String) = New List(Of String)
            list.Add("Quantité")
            list.Add("Cours")
            list.Add("Coupon couru")
            list.Add("Omega")
            list.Add("Chorus")
            CbDataGrid.DataSource = list
            BExcel.Enabled = False
            BPrint.Enabled = False

            Dim datee As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Computer.FileSystem.GetDirectories(My.Settings.PATH & "INPUT\MIDDLE\RECONCILIATION", FileIO.SearchOption.SearchTopLevelOnly)

            MonthCalendar.MaxDate = DateTime.Now
            'Mettre les dates en gras que l'on peut transpariser
            For Each d In datee
                If Split(d, "\")(Split(d, "\").Length - 1).Contains(".") = False Then
                    MonthCalendar.AddBoldedDate(Split(Split(d, "\")(Split(d, "\").Length - 1), "-")(2) & "/" & Split(Split(d, "\")(Split(d, "\").Length - 1), "-")(1) & "/" & Split(Split(d, "\")(Split(d, "\").Length - 1), "-")(0))
                End If
            Next

            MonthCalendar.UpdateBoldedDates()
        End Sub

        ''' <summary>
        ''' Remplacer données PTF_CHORUS ?
        ''' </summary>
        Private Sub RemplirChorus(ByVal datee As DateTime)
            If co.SelectDistinctWhere("PTF_CHORUS", "date_inventaire", "date_inventaire", datee).Count > 0 Then
                Dim a As Integer = MessageBox.Show("Voulez vous remplacer les données dans PTF_CHORUS pour la date " & datee & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 6 Then
                    co.DeleteWhere("PTF_CHORUS", "date_inventaire", datee)
                Else
                    Exit Sub
                End If
            End If
        End Sub

        ''' <summary>
        ''' Remplacer données PTF_FGA ? 
        ''' </summary>
        Private Sub RemplirFGA(ByVal datee As DateTime)
            If co.SelectWhere2("PTF_FGA", "dateinventaire", "dateinventaire", datee, "compte", "7201105", 1).Count > 0 Then
                Dim a As Integer = MessageBox.Show("Voulez vous remplacer les données dans PTF_FGA pour la date " & datee & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 6 Then
                    co.DeleteWhere("PTF_FGA", "dateinventaire", datee)
                Else
                    Exit Sub
                End If
            End If
        End Sub

        ''' <summary>
        ''' BCharger : remplir les DataGrids
        ''' </summary>
        Private Sub BCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCharger.Click
            'Dim directory As New System.IO.DirectoryInfo(dossier)
            'For Each fi In directory.GetFiles()
            'Check format excel 
            'If fi.Extension = ".csv" Or fi.Extension = ".xls" Or fi.Extension = ".xlsx" Then
            'End If
            'Next
            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date.Date.ToString

            Dim mois As String = datee.Date.Month.ToString
            If mois.Length = 1 Then
                mois = "0" & mois
            End If

            Dim jour As String = datee.Date.Day.ToString
            If jour.Length = 1 Then
                jour = "0" & mois
            End If

            Dim chemin As String = My.Settings.PATH & "INPUT\MIDDLE\RECONCILIATION" & "\" & datee.Date.Year & "-" & mois & "-" & jour

            If Directory.Exists(chemin) Then
                Windows.Forms.Cursor.Current = Cursors.WaitCursor
                Dim paramName As List(Of String) = New List(Of String)
                paramName.Add("@date")
                Dim paramDonneeQty As List(Of Object) = New List(Of Object)
                paramDonneeQty.Add(MonthCalendar.SelectionRange.Start.Date)
                Dim paramDonneeCours As List(Of Object) = New List(Of Object)
                paramDonneeCours.Add(MonthCalendar.SelectionRange.Start.Date)
                Dim paramDonneeCc As List(Of Object) = New List(Of Object)
                paramDonneeCc.Add(MonthCalendar.SelectionRange.Start.Date)

                'Remplir PTF_CHORUS
                RemplirChorus(datee)
                If co.SelectDistinctWhere("PTF_CHORUS", "date_inventaire", "date_inventaire", datee).Count = 0 Then
                    'Insérer dans base avec doublon
                    excel.ExcelToSql(chemin, "Chorus_ISIN_DEV.xls", 2, "PTF_DOUBLON_CHORUS")
                    'Supprimer les doublons par procédure stockée
                    co.ProcedureStockée("SuprimmerDoublonChorus", paramName, paramDonneeQty)
                End If

                'Remplir PTF_FGA
                RemplirFGA(datee)
                If co.SelectWhere2("PTF_FGA", "dateinventaire", "dateinventaire", datee, "compte", "7201105", 1).Count = 0 Then
                    co.DeleteWhere("PTF_FGA", "Dateinventaire", datee)
                    Dim omega As ConnectionOmega = New ConnectionOmega()
                    omega.ToConnectOmega()
                    omega.commandeSql("PTF_FGA", Replace(omega.LectureFichierSql("PTF_FGA.sql"), "***", datee), False)
                End If

                'Remplir les in et out chorus omega
                DataGridOmega.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("RéconciliationInOmega", paramName, paramDonneeQty))
                DataGridChorus.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("RéconciliationInChorus", paramName, paramDonneeCours))

                paramName.Add("@epsilon")
                If IsNumeric(TQty.Text) And IsNumeric(TCours.Text) And IsNumeric(TCc.Text) Then
                    paramDonneeQty.Add(Convert.ToDouble(TQty.Text))
                    paramDonneeCours.Add(Convert.ToDouble(TCours.Text))
                    paramDonneeCc.Add(Convert.ToDouble(TCc.Text))
                Else
                    MessageBox.Show("Une des sensibilités n'est pas un nombre !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    Exit Sub
                End If

                'Remplir les écart qty, cours_close, CC
                DataGridQty.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("RéconciliationQty", paramName, paramDonneeQty))
                DataGridCours.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("RéconciliationCours", paramName, paramDonneeCours))
                DataGridCc.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("RéconciliationCouponCouru", paramName, paramDonneeCc))

                If CbRapport.Checked Then
                    'Construction d'un rapport fichier excel dans le meme répertoire
                    Try
                        Dim sql1 As String = "SELECT"
                        sql1 = sql1 & " CASE WHEN Compte_OMEGA IS NULL THEN compte_CHORUS ELSE Compte_OMEGA END AS 'Compte',"
                        sql1 = sql1 & " Type_Pb As 'Type de problème',"
                        sql1 = sql1 & " Libelle_ptf As 'Libellé ptf',"
                        sql1 = sql1 & " code_titre As 'Code titre',"
                        sql1 = sql1 & " libelle_titre As 'Libellé titre',"
                        sql1 = sql1 & " type_produit As 'Type produit',"
                        sql1 = sql1 & " ecart As 'Ecart',"
                        sql1 = sql1 & " quantite_OMEGA As 'Quantité OMEGA',"
                        sql1 = sql1 & " quantite_CHORUS As 'Quantité CHORUS',"
                        sql1 = sql1 & " CASE WHEN quantite_CHORUS IS NOT NULL AND px_ref_CHORUS <> 0 THEN Vc_CHORUS/px_ref_CHORUS*100 ELSE ' ' END As 'Quantité CHORUS recalculée',"
                        sql1 = sql1 & " CASE WHEN quantite_CHORUS IS NOT NULL AND type_produit LIKE '%Obligation%' THEN Vc_CHORUS ELSE ' ' END As 'Valeur comptable CHORUS',"
                        sql1 = sql1 & " CASE WHEN quantite_CHORUS IS NOT NULL AND type_produit LIKE '%Obligation%' THEN px_ref_CHORUS ELSE ' ' END As 'Prix de référence CHORUS',"
                        sql1 = sql1 & " CASE WHEN compte_CHORUS IS NULL THEN ' ' ELSE compte_CHORUS END As 'Compte CHORUS',"
                        sql1 = sql1 & " '' As Commentaire "
                        sql1 = sql1 & " FROM PTF_CHORUS_RAPPORT"
                        sql1 = sql1 & " WHERE"
                        sql1 = sql1 & " date='" & MonthCalendar.SelectionRange.Start.Date & "' and type_pb IN('Existe dans OMEGA pas dans CHORUS','Existe dans CHORUS pas dans OMEGA','Ecart quantité')"
                        sql1 = sql1 & " ORDER BY libelle_titre"

                        Dim sql2 As String = " SELECT"
                        sql2 = sql2 & " Compte_OMEGA AS 'Compte OMEGA',"
                        sql2 = sql2 & " Compte_CHORUS AS 'Compte CHORUS',"
                        sql2 = sql2 & " Type_Pb As 'Type de problème',"
                        sql2 = sql2 & " Libelle_ptf As 'Libellé ptf',"
                        sql2 = sql2 & " code_titre As 'Code titre',"
                        sql2 = sql2 & " libelle_titre As 'Libellé titre',"
                        sql2 = sql2 & " type_produit As 'Type produit',"
                        sql2 = sql2 & " ecart As 'Ecart cours',"
                        sql2 = sql2 & " Cours_OMEGA As 'Cours OMEGA',"
                        sql2 = sql2 & " Cours_CHORUS As 'Cours CHORUS',"
                        sql2 = sql2 & " '' As Commentaire "
                        sql2 = sql2 & " FROM PTF_CHORUS_RAPPORT"
                        sql2 = sql2 & " WHERE"
                        sql2 = sql2 & " date='" & MonthCalendar.SelectionRange.Start.Date & "' and type_pb IN('Ecart cours')"
                        sql2 = sql2 & " ORDER BY Ecart DESC"

                        Dim sql3 As String = "SELECT"
                        sql3 = sql3 & " Compte_OMEGA AS 'Compte OMEGA', "
                        sql3 = sql3 & " Compte_CHORUS AS 'Compte CHORUS', "
                        sql3 = sql3 & " Type_Pb As 'Type de problème',		"
                        sql3 = sql3 & " Libelle_ptf As 'Libellé ptf',"
                        sql3 = sql3 & " code_titre As 'Code titre',"
                        sql3 = sql3 & " libelle_titre As 'Libellé titre',"
                        sql3 = sql3 & " type_produit As 'Type produit',"
                        sql3 = sql3 & " ecart As 'Ecart Coupon',"
                        sql3 = sql3 & " Cc_OMEGA As 'Coupon Couru OMEGA',"
                        sql3 = sql3 & " Cc_CHORUS As 'Coupon couru CHORUS',"
                        sql3 = sql3 & " '' As Commentaire "
                        sql3 = sql3 & " FROM PTF_CHORUS_RAPPORT WHERE "
                        sql3 = sql3 & " date='" & MonthCalendar.SelectionRange.Start.Date & "' and type_pb IN('Ecart coupon couru')"
                        sql3 = sql3 & " ORDER BY Ecart DESC"

                        Dim sql As List(Of String) = New List(Of String)
                        sql.Add(sql1)
                        sql.Add(sql2)
                        sql.Add(sql3)

                        co.SqlToExcelEndSave(sql, chemin & "\" & "Rapport.csv")

                        Dim onglet As List(Of String) = New List(Of String)
                        onglet.Add("In Out Qty")
                        onglet.Add("Cours")
                        onglet.Add("Coupon couru")
                        excel.PresentationExcel(chemin & "\" & "Rapport.csv", onglet)

                    Catch ex As Exception
                        MessageBox.Show("Un problème est survenu ! (fichier déjà ouvert " & SaveFileDialog.FileName & " ?)", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End Try
                End If

                BPrint.Enabled = True
                BExcel.Enabled = True
                Windows.Forms.Cursor.Current = Cursors.Default
            Else
                MessageBox.Show("Le dossier " & chemin & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' BExcel : envoyer les DataGrids vers Excel 
        ''' </summary>
        Private Sub BExcel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcel.Click
            'Choix de la DataGrid à mettre dans csv
            Select Case CbDataGrid.Text
                Case "Quantité"
                    da.DataGridToExcel(DataGridQty, SaveFileDialog, Me)
                Case "Cours"
                    da.DataGridToExcel(DataGridCours, SaveFileDialog, Me)
                Case "Coupon couru"
                    da.DataGridToExcel(DataGridCc, SaveFileDialog, Me)
                Case "Omega"
                    da.DataGridToExcel(DataGridOmega, SaveFileDialog, Me)
                Case "Chorus"
                    da.DataGridToExcel(DataGridChorus, SaveFileDialog, Me)

                Case Else
                    MsgBox("Le type de grille n'est pas reconnu par l'application !")
            End Select
        End Sub

        ''' <summary>
        ''' BPrint : imprimer les DataGrids 
        ''' </summary>
        Private Sub BPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPrint.Click
            Dim dataChoice As DataGridView = Nothing
            If String.IsNullOrEmpty(CbDataGrid.Text) = False Then
                Select Case CbDataGrid.Text
                    Case "Coupon couru"
                        dataChoice = DataGridCc
                    Case "Quantité"
                        dataChoice = DataGridQty
                    Case "Cours"
                        dataChoice = DataGridCours
                    Case "Omega"
                        dataChoice = DataGridOmega
                    Case "Chorus"
                        dataChoice = DataGridChorus
                    Case Else
                        MsgBox("Le type de grille n'est pas reconnu par l'application !")
                        Exit Sub
                End Select

                If MyDataGridViewPrinter.SetupThePrinting(dataChoice, myPrintDocument, Me.Text) Then
                    myPrintDocument.Print()
                End If

            Else
                MessageBox.Show("Il faut choisir la datgrid à imprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        Private Sub myPrintDocument_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles myPrintDocument.PrintPage
            Dim more As Boolean = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more = True Then
                e.HasMorePages = True
            End If
        End Sub

        ''' <summary>
        ''' BRefreshCorrespondance : met à jours la table PTF_CHORUS_CORRESPONDANCE
        ''' </summary>
        Private Sub BRefreshCorrespondance_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRefreshCorrespondance.Click
            co.DeleteFrom("PTF_CHORUS_CORRESPONDANCE")
            excel.ExcelToSql(My.Settings.PATH & "\IMPORT\MIDDLE\RECONCILIATION", "Correspondance compte OMEGA CHORUS.xls", 1, "PTF_CHORUS_CORRESPONDANCE")
        End Sub

    End Class
End Namespace
