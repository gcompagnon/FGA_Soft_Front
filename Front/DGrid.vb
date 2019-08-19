Imports DataGridViewAutoFilter
Imports System.Text

Public Class DGrid

    Dim lo As Log = New Log


    Class EventDataGridHandler
        Public dataGridView As DataGridView
        Public Sub handler_rowCellHearder_Numbering(ByVal sender As System.Object, ByVal e As System.EventArgs)
            dataGridView.RowHeadersWidth = 50
            For i As Integer = 0 To dataGridView.RowCount - 1
                dataGridView.Rows(i).HeaderCell.Value = (i + 1).ToString()
            Next
        End Sub

    End Class



    Public Sub RowHeaderCell(ByVal dataGridViewParam As DataGridView)
        Dim h As EventDataGridHandler = New EventDataGridHandler
        h.dataGridView = dataGridViewParam

        AddHandler dataGridViewParam.DataBindingComplete, AddressOf h.handler_rowCellHearder_Numbering
        AddHandler dataGridViewParam.Sorted, AddressOf h.handler_rowCellHearder_Numbering
    End Sub

    ''' <summary>
    ''' Applique des filtres sur chaque colonne de la datagrille
    ''' </summary>
    Public Sub AutoFiltre(ByVal dataGridView As DataGridView, Optional ByVal colAFiltre As List(Of Integer) = Nothing)
        If colAFiltre Is Nothing Then
            For Each col As DataGridViewColumn In dataGridView.Columns
                col.HeaderCell = New DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell)
            Next
        Else
            For Each col In colAFiltre
                dataGridView.Columns(col).HeaderCell = New DataGridViewAutoFilterColumnHeaderCell(dataGridView.Columns(col).HeaderCell)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Export une datagrid vers un nouveau fichier excel
    ''' </summary>
    Public Sub DataGridToNewExcelByPast(ByVal maDataGrid As DataGridView, ByVal monTitre As String) ', ByVal monTitre As String) ', ByVal dataGrid2 As DataGridView)
        'dataGrid1.DefaultCellStyle.Format = "#####.#"
        Try
            If maDataGrid.RowCount > 0 Then

                'NEW FICHIER EXCEL
                'Dim xl As New Microsoft.Office.Interop.Excel.Application
                'xl.Workbooks.Add()
                'xl.Visible = True

                'OUVRIR FICHIER MODELE
                Dim appExcel As Microsoft.Office.Interop.Excel.Application
                Try
                    appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
                Catch ex As Exception
                    appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
                End Try

                Dim xl As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(My.Settings.PATH & "\IMPORT\Modèle Grille.xls", , True) 'Classeur Excel
                appExcel.Visible = False

                'COPIE CLIPBOARD
                maDataGrid.SelectAll()
                Dim dataObject As DataObject = New DataObject
                dataObject = maDataGrid.GetClipboardContent()
                maDataGrid.ClearSelection()
                Clipboard.SetDataObject(dataObject, False)
                xl.Sheets(1).Paste(xl.Sheets(1).cells(1, 1), False)

                'PRESENTATION
                If maDataGrid.RowHeadersVisible = True Then
                    xl.Sheets(1).columns(1).delete()
                End If
                'figer les volet l1
                'xl.Sheets(1).rows(1).RowHeight = 15
                'xl.Sheets(1).Cells.EntireColumn.AutoFit()
                xl.Sheets(1).Activate()
                xl.Sheets(1).Rows("2:2").Select()
                appExcel.ActiveWindow.FreezePanes = True
                xl.Sheets(1).cells(1, 1).Select()
                xl.Sheets(1).Rows(1).Font.Size = 11 'taille 11
                xl.Sheets(1).Rows(1).Font.Bold = True 'gras
                'filtre automatique
                xl.Sheets(1).Rows(1).Autofilter()
                xl.Sheets(1).Range("A2:IV65536").WrapText = False
                'autofit de la sheet1
                xl.Sheets(1).Cells.EntireColumn.AutoFit()
                'Mode IMPRESSION PAYSAGE
                'xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(dataGrid1.RowCount, dataGrid1.ColumnCount)).Select()
                Dim i As Integer = maDataGrid.RowCount + 1 ' Pour les titres
                Dim j As Integer = maDataGrid.ColumnCount
                xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(i, j)).Select()
                With xl.Sheets(1).PageSetup
                    .LeftMargin = 14.17
                    .RightMargin = 14.17
                    .BottomMargin = 14.17
                    .TopMargin = 14.17
                    .Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape
                    .Zoom = False
                    .FitToPagesWide = 1
                    .FitToPagesTall = 1
                    .LeftFooter = "&""Arial,Gras""Fédéris Gestion d'Actifs"
                    .CenterFooter = monTitre
                    '.RightFooter = "page &P&G"
                    '.RightFooter = "&G"
                    '.RightFooterPicture.Filename = My.Settings.PATH & "IMAGES\F.jpg"
                End With

                appExcel.Visible = True
                lo.Log(ELog.Information, "ToExcel", "Exportation de la datagrid dans un fichier excel!")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'NEW FICHIER EXCEL
        'Dim xl As New Microsoft.Office.Interop.Excel.Application

    End Sub

    ''' <summary>
    ''' Export une datagrid vers un nouveau fichier excel
    ''' </summary>
    Public Sub DataGridToNewExcel(ByVal maDataGrid As DataGridView, ByVal monTitre As String, Optional ByVal page As Integer = 1) ', ByVal monTitre As String) ', ByVal dataGrid2 As DataGridView)
        'dataGrid1.DefaultCellStyle.Format = "#####.#"
        Try
            If maDataGrid.RowCount > 0 Then

                'NEW FICHIER EXCEL
                'Dim xl As New Microsoft.Office.Interop.Excel.Application
                'xl.Workbooks.Add()
                'xl.Visible = True

                Windows.Forms.Cursor.Current = Cursors.WaitCursor

                'OUVRIR FICHIER MODELE
                Dim appExcel As Microsoft.Office.Interop.Excel.Application
                Try
                    appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
                Catch ex As Exception
                    appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
                End Try

                Dim xl As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(My.Settings.PATH & "\IMPORT\Modèle Grille.xls", , True) 'Classeur Excel
                'appExcel.Visible = False

                Dim numbColumn As Integer = maDataGrid.ColumnCount + 1
                'COPIE DATAGRID
                For titre As Integer = 0 To maDataGrid.ColumnCount - 1
                    xl.Sheets(1).cells(1, titre + 2) = maDataGrid.Columns(titre).HeaderText
                Next
                For ligne As Integer = 0 To maDataGrid.RowCount - 1
                    'couleur des lignes
                    Dim startCell = xl.Sheets(1).cells(ligne + 2, 1)
                    Dim endCell = xl.Sheets(1).cells(ligne + 2, numbColumn)
                    If (maDataGrid.Rows(ligne).DefaultCellStyle.BackColor.IsNamedColor) Then
                        xl.Sheets(1).Range(startCell, endCell).Interior.Color = RGB(
                                            maDataGrid.Rows(ligne).DefaultCellStyle.BackColor.R,
                                            maDataGrid.Rows(ligne).DefaultCellStyle.BackColor.G,
                                            maDataGrid.Rows(ligne).DefaultCellStyle.BackColor.B)
                    Else
                        xl.Sheets(1).Range(startCell, endCell).Interior.Color = RGB(255, 255, 255)
                    End If
                    With xl.Sheets(1).Range(startCell, endCell).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom)
                        .LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                        .ColorIndex = 16
                    End With
                    For colonne As Integer = 0 To maDataGrid.ColumnCount - 1
                        xl.Sheets(1).cells(ligne + 2, colonne + 2) = maDataGrid.Rows(ligne).Cells(colonne).FormattedValue
                    Next
                    xl.Sheets(1).cells(ligne + 2, 1) = maDataGrid.Rows(ligne).HeaderCell.Value
                Next


                'PRESENTATION
                Dim i As Integer = maDataGrid.RowCount + 1 ' Pour les titres
                Dim j As Integer = maDataGrid.ColumnCount + 1
                xl.Sheets(1).Activate()
                'filtre automatique
                xl.Sheets(1).Rows(1).Autofilter()
                'saut de ligne
                xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(i, j)).WrapText = False
                xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(1, j)).WrapText = True
                'mise en place des volets
                xl.Sheets(1).Rows("2:2").Select()
                appExcel.ActiveWindow.FreezePanes = True
                'xl.Sheets(1).cells(1, 1).Select()
                'taille des titres
                xl.Sheets(1).Rows(1).Font.Size = 10.5 'taille 11
                xl.Sheets(1).Rows(1).Font.Bold = True 'gras
                'autofit des colonnes
                xl.Sheets(1).Cells.EntireColumn.AutoFit()
                'Mode IMPRESSION PAYSAGE
                xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(i, j)).Select()
                With xl.Sheets(1).PageSetup
                    .LeftMargin = 14.17
                    .RightMargin = 14.17
                    .BottomMargin = 14.17
                    .TopMargin = 14.17
                    .Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape
                    .Zoom = False
                    .FitToPagesWide = page
                    .FitToPagesTall = 1
                    .LeftFooter = "&""Arial,Gras""Fédéris Gestion d'Actifs"
                    .CenterFooter = monTitre
                    '.RightFooter = "page &P&G"
                    '.RightFooter = "&G"
                    '.RightFooterPicture.Filename = My.Settings.PATH & "IMAGES\F.jpg"
                End With
                xl.Sheets(1).Range(xl.Sheets(1).Cells(2, 1), xl.Sheets(1).Cells(i, j)).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight
                'xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(i, j)).NumberFormat = "# ##0,0"

                Windows.Forms.Cursor.Current = Cursors.Default

                appExcel.Visible = True
                lo.Log(ELog.Information, "ToExcel", "Exportation de la datagrid dans un fichier excel!")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'NEW FICHIER EXCEL
        'Dim xl As New Microsoft.Office.Interop.Excel.Application

    End Sub


    ''' <summary>
    ''' Export DES datagrids vers un nouveau fichier excel
    ''' </summary>
    Public Sub DataGridsToNewExcel(ByVal datagrids As List(Of DataGridView), ByVal mesTitres As List(Of String), ByVal mesPages As List(Of Integer), ByVal libelle As String) ', ByVal dataGrid2 As DataGridView)
        'dataGrid1.DefaultCellStyle.Format = "#####.#"

        If datagrids.Count = mesPages.Count And datagrids.Count = mesTitres.Count Then
            'Try

            'OUVRIR FICHIER MODELE
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            Dim xl As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(My.Settings.PATH & "\IMPORT\Modèle Grille.xls", , True) 'Classeur Excel
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            appExcel.Visible = False

            For igrid = 0 To datagrids.Count - 1

                If datagrids(igrid).RowCount > 0 Then

                    'NEW FICHIER EXCEL
                    'Dim xl As New Microsoft.Office.Interop.Excel.Application
                    'xl.Workbooks.Add()
                    'xl.Visible = True


                    Dim numbColumn As Integer = datagrids(igrid).ColumnCount + 1
                    'COPIE DATAGRID
                    ' headers
                    For titre As Integer = 0 To datagrids(igrid).ColumnCount - 1
                        xl.Sheets(igrid + 1).cells(1, titre + 2) = datagrids(igrid).Columns(titre).HeaderText
                    Next

                    ' values
                    For ligne As Integer = 0 To datagrids(igrid).RowCount - 1
                        'couleur des lignes
                        Dim startCell = xl.Sheets(igrid + 1).cells(ligne + 2, 1)
                        Dim endCell = xl.Sheets(igrid + 1).cells(ligne + 2, numbColumn)
                        If (datagrids(igrid).Rows(ligne).DefaultCellStyle.BackColor.IsNamedColor) Then
                            xl.Sheets(igrid + 1).Range(startCell, endCell).Interior.Color = RGB(
                                                datagrids(igrid).Rows(ligne).DefaultCellStyle.BackColor.R,
                                                datagrids(igrid).Rows(ligne).DefaultCellStyle.BackColor.G,
                                                datagrids(igrid).Rows(ligne).DefaultCellStyle.BackColor.B)
                        Else
                            xl.Sheets(igrid + 1).Range(startCell, endCell).Interior.Color = RGB(255, 255, 255)
                        End If
                        With xl.Sheets(igrid + 1).Range(startCell, endCell).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom)
                            .LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                            .ColorIndex = 16
                        End With
                        For colonne As Integer = 0 To datagrids(igrid).ColumnCount - 1
                            xl.Sheets(igrid + 1).cells(ligne + 2, colonne + 2) = datagrids(igrid).Rows(ligne).Cells(colonne).FormattedValue
                        Next
                        xl.Sheets(igrid + 1).cells(ligne + 2, 1) = datagrids(igrid).Rows(ligne).HeaderCell.Value
                    Next


                    'PRESENTATION
                    Dim i As Integer = datagrids(igrid).RowCount + 1 ' Pour les titres
                    Dim j As Integer = datagrids(igrid).ColumnCount + 1
                    xl.Sheets(igrid + 1).Activate()
                    'nom de la feuille
                    xl.Sheets(igrid + 1).Name = mesTitres(igrid)
                    'filtre automatique
                    xl.Sheets(igrid + 1).Rows(1).Autofilter()
                    'saut de ligne
                    xl.Sheets(igrid + 1).Range(xl.Sheets(igrid + 1).Cells(1, 1), xl.Sheets(igrid + 1).Cells(i, j)).WrapText = False
                    xl.Sheets(igrid + 1).Range(xl.Sheets(igrid + 1).Cells(1, 1), xl.Sheets(igrid + 1).Cells(1, j)).WrapText = True
                    'mise en place des volets
                    xl.Sheets(igrid + 1).Rows("2:2").Select()
                    appExcel.ActiveWindow.FreezePanes = True
                    'xl.Sheets(1).cells(1, 1).Select()
                    'taille des titres
                    xl.Sheets(igrid + 1).Rows(1).Font.Size = 10.5 'taille 11
                    xl.Sheets(igrid + 1).Rows(1).Font.Bold = True 'gras
                    'autofit des colonnes
                    xl.Sheets(igrid + 1).Cells.EntireColumn.AutoFit()
                    'Mode IMPRESSION PAYSAGE
                    xl.Sheets(igrid + 1).Range(xl.Sheets(igrid + 1).Cells(1, 1), xl.Sheets(igrid + 1).Cells(i, j)).Select()
                    With xl.Sheets(igrid + 1).PageSetup
                        .LeftMargin = 14.17
                        .RightMargin = 14.17
                        .BottomMargin = 14.17
                        .TopMargin = 14.17
                        .Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape
                        .Zoom = False
                        .FitToPagesWide = mesPages(igrid)
                        .FitToPagesTall = 1
                        .LeftFooter = "&""Arial,Gras""Fédéris Gestion d'Actifs"
                        .CenterFooter = mesTitres(igrid) & " " & libelle
                        '.RightFooter = "page &P&G"
                        '.RightFooter = "&G"
                        '.RightFooterPicture.Filename = My.Settings.PATH & "IMAGES\F.jpg"
                    End With
                    xl.Sheets(igrid + 1).Range(xl.Sheets(igrid + 1).Cells(2, 1), xl.Sheets(igrid + 1).Cells(i, j)).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight
                    'xl.Sheets(1).Range(xl.Sheets(1).Cells(1, 1), xl.Sheets(1).Cells(i, j)).NumberFormat = "# ##0,0"

                    lo.Log(ELog.Information, "ToExcel", "Exportation de la datagrid " & datagrids(igrid).Name & " dans un fichier excel!")
                End If

            Next

            appExcel.Visible = True
            Windows.Forms.Cursor.Current = Cursors.Default

            'Catch ex As Exception
            '   MsgBox(ex.Message)
            'End Try

        End If

        'NEW FICHIER EXCEL
        'Dim xl As New Microsoft.Office.Interop.Excel.Application

    End Sub


    ''' <summary>
    ''' Copier datagridview dans un fichier excel
    ''' </summary>
    Public Sub DataGridToExcel(ByVal dataGrid As DataGridView, ByVal saveFile As SaveFileDialog, ByVal ihm As Windows.Forms.Form)
        Try
            'Choix d'un fichier de destination
            If saveFile.ShowDialog(ihm) = System.Windows.Forms.DialogResult.OK Then
                'On valide l'édition de la DataGridView
                dataGrid.EndEdit()
                'On se prépare une mémoire des données formatées a écrire dans le fichier
                Dim ToSave As New StringBuilder()
                'On vas y mettre les en têtes tant demandées :-)
                Dim Headers As String = String.Empty
                For index As Integer = 0 To dataGrid.Columns.Count - 1
                    Headers &= dataGrid.Columns(index).HeaderText & ";"
                Next
                'La boucle ajoute un ";" a la fin qui est inutile
                Headers = Headers.Remove(Headers.LastIndexOf(";"), 1)
                'Maintenant qu'il est "propre" on le stocke dans la mémoire
                ToSave.AppendLine(Headers)

                'On boucle sur toutes les lignes disponibles
                For i As UInt64 = 0 To dataGrid.Rows.Count - 1
                    'On se fait une variable pour y stocké une ligne
                    Dim OneRow As String = String.Empty
                    'On peut faire une boucle sur toutes les colonnes disponible si la ligne n'est pas vide
                    If dataGrid.Rows(i).IsNewRow = False Then
                        For j As Integer = 0 To dataGrid.Rows(i).Cells.Count - 1
                            OneRow &= dataGrid.Rows(i).Cells(j).Value & ";"
                        Next
                        'La boucle ajoute un ";" a la fin qui est inutile
                        OneRow = OneRow.Remove(OneRow.LastIndexOf(";"), 1)
                        'Maintenant qu'il est "propre" on le stocke dans la mémoire
                        ToSave.AppendLine(OneRow)
                    End If
                Next
                'Tout est bien qui finit bien ? essayons maintenant d'écrire le fichier
                IO.File.WriteAllText(saveFile.FileName, ToSave.ToString(), Encoding.Default)
            End If

        Catch ex As Exception
            MessageBox.Show(String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
        End Try
    End Sub

    ''' <summary>
    ''' Aligne les chifres à droites et les autres caractère à gauche
    ''' </summary>
    Public Sub PresentationDataGrid(ByVal dataGrid As DataGridView, ByVal nombreChiffreAprèsVirgule As Integer)
        Dim i As Double
        If dataGrid.RowCount <> 0 Then
            For j = 0 To dataGrid.Columns.Count - 1
                i = 0

                While (IsDBNull(dataGrid.Rows(i).Cells(j).Value) And dataGrid.RowCount < i - 1)
                    If dataGrid.Columns.Count - 1 > i Then
                        i = i + 1
                        'dataGrid.rowheader()
                    Else
                        Exit While
                    End If
                End While

                'Changement des formats + alignement colonne
                If dataGrid.Columns(j).HeaderText.Contains("Ticker") Then
                    dataGrid.Columns(j).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                ElseIf dataGrid.Columns(j).HeaderText.Contains("Liquidité") Then
                    dataGrid.Columns(j).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                ElseIf IsNumeric(dataGrid.Rows(i).Cells(j).Value) Then
                    dataGrid.Columns(j).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    If dataGrid.Rows(i).Cells(j).Value / 100 < 1 Then
                        dataGrid.Columns(j).DefaultCellStyle.Format = "n" & nombreChiffreAprèsVirgule
                    Else
                        dataGrid.Columns(j).DefaultCellStyle.Format = "n0"
                    End If
                Else
                    dataGrid.Columns(j).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                End If
            Next
        End If
    End Sub

End Class
