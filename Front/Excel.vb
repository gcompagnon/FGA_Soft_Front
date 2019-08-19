Imports System.Text

Public Class Excel

    Dim co As Connection = New Connection()
    Dim lo As Log = New Log()
    Dim fi As Fichier = New Fichier()


    ''' <summary>
    ''' Lire un fichier excel
    ''' Indice Feuille == 1 si premiere feuille ...
    ''' </summary>
    Public Sub LectureFichierExcel(ByVal chemin As String, ByVal fichier As String, ByVal indiceFeuille As Integer) 'As Microsoft.Office.Interop.Excel.Worksheet
        fichier = chemin & "\" & fichier

        If fi.Existe(fichier) Then

            Dim app As Microsoft.Office.Interop.Excel.Application
            Try
                app = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                app = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(fichier, , False)
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille)
            app.Visible = True

        Else
            MessageBox.Show("Le fichier excel  n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If

    End Sub

    ''' <summary>
    ''' Lire un fichier excel
    ''' Indice Feuille == 1 si premiere feuille ...
    ''' </summary>
    Public Function CellFichierExcel(ByVal chemin As String, ByVal fichier As String, ByVal indiceFeuille As Integer, ByVal iLigne As Integer, ByVal jCol As Integer) As Object
        fichier = chemin & "\" & fichier

        ' Lister les Processus Excel
        Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
        Dim pidExcel As New List(Of Integer)
        For Each p As Process In pProcess
            pidExcel.Add(p.Id)
        Next

        Dim app As Microsoft.Office.Interop.Excel.Application
        Try
            app = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
        Catch ex As Exception
            app = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
        End Try
        'Déclaration des variables
        'Dim app As Microsoft.Office.Interop.Excel.Application = CreateObject("Excel.Application")
        'Ne pas afficher les messages venant des macros
        app.DisplayAlerts = False
        'Faire en sorte que la fenêtre Excel soit en icone et minimisé
        app.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMinimized
        'Desactiver l execution de macros en automatique (utile pour l addin Bloomberg)
        app.AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable

        'Dim facset_installe As RegistryKey
        'facset_installe = Registry.LocalMachine.OpenSubKey("SOFTWARE\\FactSet")
        'If (facset_installe IsNot Nothing) Then
        '    'MsgBox("Factset installe")
        '    'Faire en sorte qu'excel soit visible (utile avec l'addin Factset )
        '    app.Visible = True

        '    'For Each ai As Microsoft.Office.Interop.Excel.AddIn In app.AddIns
        '    '    'MsgBox(ai.FullName)
        '    '    'Threading.Thread.Sleep(100)
        '    '    If (ai.FullName.Contains("factset")) Then
        '    '        Threading.Thread.Sleep(100)
        '    '        'ai.Installed = False
        '    '    End If
        '    'Next
        'End If

        Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(fichier, , True)

        Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille)
        Dim res As Object = sheet.Cells(iLigne, jCol).Value()

        sheet = Nothing
        wbExcel.Close(False)
        wbExcel = Nothing
        ' Le Quit provoquerait une erreur sur Excel et Factset
        '        app.Quit()
        app.DisplayAlerts = True
        app = Nothing ' libère la memoire

        ' Supprimer le Processus Excel avec un Kill
        pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
        Dim pidExcelFinal As New List(Of Integer)
        For Each p As Process In pProcess
            pidExcelFinal.Add(p.Id)
        Next

        For Each pid As Integer In pidExcelFinal.Except(pidExcel)
            Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
            p.CloseMainWindow()
            'p.Close()
        Next
        GC.Collect()


        Return res
    End Function

    ''' <summary>
    ''' Exporter un fichier excel vers une base de données
    ''' </summary>
    Public Sub ExcelToSqlForSecteur(ByVal datee As DateTime, ByVal cheminExcel As String, ByVal excelName As String, ByVal indiceFeuille As Integer, ByVal tabName As String, Optional ByVal iLigneStart As Integer = 0, Optional ByVal iColStart As Integer = 0, Optional ByVal iRowToDelete As Integer = 0)

        If fi.Existe(cheminExcel & "\" & excelName) Then

            'Lister les Processus Excel
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcel As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcel.Add(p.Id)
            Next

            Dim colName As List(Of String) = New List(Of String)
            Dim chiffre As List(Of Object) = New List(Of Object)

            'Déclaration des variables
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'Application Excel
            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel


            'Replacer le fichier en case (1,1) sans saut de ligne
            If iLigneStart <> 0 Then
                For ii = 1 To iLigneStart - 1 Step 1
                    sheet.Rows(1).delete()
                Next
            End If
            If iColStart <> 0 Then
                For jj = 1 To iColStart - 1 Step 1
                    sheet.Columns(1).delete()
                Next
            End If
            If iRowToDelete <> 0 Then
                sheet.Rows(iRowToDelete).delete()
            End If
            Dim nbrCol As Integer = 0
            For Each f In sheet.Rows(1).Value()
                If IsNothing(f) = False Then
                    'buff.Append(Replace(Replace(f, " ", "_"), "%", "pct") + ", ")
                    colName.Add(Replace(Replace(f, " ", "_"), "%", "pct"))
                    nbrCol = nbrCol + 1
                End If
            Next
            'colName.Add("DATE")

            Dim nbrLigne As Integer = 0
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                Else
                    Exit For
                End If
            Next
            Dim i As Integer = 0
            Dim inserts As New List(Of String)

            For index As Integer = 2 To nbrLigne
                i = 0
                chiffre.Clear()
                For Each f In sheet.Rows(index).Value()
                    i = i + 1
                    If i < nbrCol + 1 Then
                        If TypeOf f Is Integer AndAlso f = My.Resources.EXCEL_ERR_VALEUR Then ' #VALEUR!
                            chiffre.Add(Nothing)
                        Else
                            chiffre.Add(f)
                        End If
                    End If
                Next
                'chiffre.Add(datee)
                ' remonter une erreur si les 2 tableaux n ont pas les memes tailles
                If colName.Count <> chiffre.Count Then
                    MsgBox("Problème d'insertion de la ligne " & index & " dans la base " & tabName & Chr(10) & _
                           co.InsertToSql(tabName, colName, chiffre))
                    Continue For
                End If

                inserts.Add(co.InsertToSql(tabName, colName, chiffre))
            Next
            co.RequeteSqls(inserts)
            lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)
            appExcel.DisplayAlerts = False
            wbExcel.Close(False)
            'appExcel.Quit()
            appExcel.DisplayAlerts = True
            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing

            ' Supprimer le Processus Excel avec un Kill
            pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcelFinal As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcelFinal.Add(p.Id)
            Next

            For Each pid As Integer In pidExcelFinal.Except(pidExcel)
                Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
                p.CloseMainWindow()
                'p.Close()
            Next
            GC.Collect()

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    ''' <summary>
    ''' Exporter un fichier excel vers une base de données
    ''' </summary>
    Public Sub ExcelToSqlBulk(ByVal cheminExcel As String, ByVal excelName As String, ByVal indiceFeuille As Integer, ByVal tabName As String, ByVal colSize As List(Of Integer), Optional ByVal iLigneStart As Integer = 0, Optional ByVal iColStart As Integer = 0, Optional ByVal iRowToDelete As Integer = 0)

        If fi.Existe(cheminExcel & "\" & excelName) Then

            'Lister les Processus Excel
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcel As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcel.Add(p.Id)
            Next

            Dim colName As List(Of String) = New List(Of String)
            Dim colType As List(Of Type)
            Dim cell As Object

            'Déclaration des variables
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'Application Excel
            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel


            'Replacer le fichier en case (1,1) sans saut de ligne
            If iLigneStart <> 0 Then
                For ii = 1 To iLigneStart - 1 Step 1
                    sheet.Rows(1).delete()
                Next
            End If
            If iColStart <> 0 Then
                For jj = 1 To iColStart - 1 Step 1
                    sheet.Columns(1).delete()
                Next
            End If
            If iRowToDelete <> 0 Then
                sheet.Rows(iRowToDelete).delete()
            End If
            Dim nbrCol As Integer = 0
            For Each f In sheet.Rows(1).Value()
                If IsNothing(f) = False Then
                    'buff.Append(Replace(Replace(f, " ", "_"), "%", "pct") + ", ")
                    colName.Add(Replace(Replace(f, " ", "_"), "%", "pct"))
                    nbrCol = nbrCol + 1
                End If
            Next

            colType = New List(Of Type)(colName.Count)
            For i As Integer = 0 To colName.Count - 1
                cell = sheet.Cells(2, i + 1).Value()
                If (IsNothing(cell)) Then
                    colType.Add(System.Type.GetType("System.String"))
                Else
                    colType.Add(cell.GetType())
                End If

            Next

            Dim nbrLigne As Integer = 0
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    If (CStr(f).Length > 0) Then
                        nbrLigne = nbrLigne + 1
                    End If
                Else
                    Exit For
                End If
            Next

            'configuration de la datatable contenant les donnees à bulker
            Dim dataTab As New DataTable(tabName)
            For i As Integer = 0 To colName.Count - 1
                Dim dc As DataColumn = New DataColumn(colName(i))
                dc.DataType = colType(i)
                dataTab.Columns.Add(dc)
            Next

            'Lecture du contenu

            For index As Integer = 2 To nbrLigne
                Dim row As DataRow
                row = dataTab.NewRow()

                For i As Integer = 0 To colName.Count - 1
                    cell = sheet.Cells(index, i + 1).Value()

                    If IsNothing(cell) Or (TypeOf cell Is Integer AndAlso cell = My.Resources.EXCEL_ERR_VALEUR) Then ' #VALEUR!
                        row(colName(i)) = DBNull.Value
                    ElseIf TypeOf cell Is String And (CStr(cell).Length > colSize(i)) Then
                        row(colName(i)) = CStr(cell).Substring(0, colSize(i))
                    Else
                        row(colName(i)) = cell
                    End If
                Next
                dataTab.Rows.Add(row)

            Next
            dataTab.AcceptChanges()

            co.RequeteBulk(dataTab, tabName)

            lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)
            appExcel.DisplayAlerts = False
            wbExcel.Close(False)
            'appExcel.Quit()
            appExcel.DisplayAlerts = True
            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing

            ' Supprimer le Processus Excel avec un Kill
            pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcelFinal As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcelFinal.Add(p.Id)
            Next

            For Each pid As Integer In pidExcelFinal.Except(pidExcel)
                Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
                p.CloseMainWindow()
                'p.Close()
            Next
            GC.Collect()

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    ''' <summary>
    ''' Exporter un fichier excel vers une base de données
    ''' </summary>
    Public Sub ExcelToSql(ByVal cheminExcel As String, ByVal excelName As String, ByVal indiceFeuille As Integer, ByVal tabName As String, Optional ByVal iLigneStart As Integer = 0, Optional ByVal iColStart As Integer = 0, Optional ByVal iRowToDelete As Integer = 0)

        If fi.Existe(cheminExcel & "\" & excelName) Then

            'Lister les Processus Excel
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcel As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcel.Add(p.Id)
            Next

            Dim colName As List(Of String) = New List(Of String)
            Dim chiffre As List(Of Object) = New List(Of Object)

            'Déclaration des variables
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'Application Excel
            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel


            'Replacer le fichier en case (1,1) sans saut de ligne
            If iLigneStart <> 0 Then
                For ii = 1 To iLigneStart - 1 Step 1
                    sheet.Rows(1).delete()
                Next
            End If
            If iColStart <> 0 Then
                For jj = 1 To iColStart - 1 Step 1
                    sheet.Columns(1).delete()
                Next
            End If
            If iRowToDelete <> 0 Then
                sheet.Rows(iRowToDelete).delete()
            End If
            Dim nbrCol As Integer = 0
            For Each f In sheet.Rows(1).Value()
                If IsNothing(f) = False Then
                    'buff.Append(Replace(Replace(f, " ", "_"), "%", "pct") + ", ")
                    colName.Add(Replace(Replace(f, " ", "_"), "%", "pct"))
                    nbrCol = nbrCol + 1
                End If
            Next

            Dim nbrLigne As Integer = 0
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                Else
                    Exit For
                End If
            Next
            Dim i As Integer = 0

            Dim inserts As New List(Of String)

            For index As Integer = 2 To nbrLigne
                i = 0
                chiffre.Clear()
                For Each f In sheet.Rows(index).Value()
                    i = i + 1
                    If i < nbrCol + 1 Then
                        If TypeOf f Is Integer AndAlso f = My.Resources.EXCEL_ERR_VALEUR Then ' #VALEUR!
                            chiffre.Add(Nothing)
                        Else
                            chiffre.Add(f)
                        End If
                    End If
                Next
                'remonter une erreur si les 2 tableaux n ont pas les memes tailles
                If colName.Count <> chiffre.Count Then
                    MsgBox("Problème d'insertion de la ligne " & index & " dans la base " & tabName & Chr(10) & _
                           co.InsertToSql(tabName, colName, chiffre))
                    Continue For
                End If

                inserts.Add(co.InsertToSql(tabName, colName, chiffre))
            Next
            co.RequeteSqls(inserts)

            lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)
            appExcel.DisplayAlerts = False
            wbExcel.Close(False)
            'appExcel.Quit()
            appExcel.DisplayAlerts = True
            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing

            ' Supprimer le Processus Excel avec un Kill
            pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcelFinal As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcelFinal.Add(p.Id)
            Next

            For Each pid As Integer In pidExcelFinal.Except(pidExcel)
                Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
                p.CloseMainWindow()
                'p.Close()
            Next
            GC.Collect()

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    ''' <summary>
    ''' Exporter un fichier excel vers une base de données et met à jour les champs existants.
    ''' </summary>
    Public Sub ExcelToSqlUpdate(ByVal cheminExcel As String, ByVal excelName As String, ByVal indiceFeuille As Integer, ByVal tabName As String, Optional ByVal iLigneStart As Integer = 0, Optional ByVal iColStart As Integer = 0, Optional ByVal iRowToDelete As Integer = 0)
        If fi.Existe(cheminExcel & "\" & excelName) Then

            'Lister les Processus Excel
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcel As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcel.Add(p.Id)
            Next

            Dim colName As List(Of String) = New List(Of String)
            Dim chiffre As List(Of Object) = New List(Of Object)

            'Déclaration des variables
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'Application Excel
            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel


            'Replacer le fichier en case (1,1) sans saut de ligne
            For ii = 1 To iLigneStart - 1 Step 1
                sheet.Rows(1).delete()
            Next
            For jj = 1 To iColStart - 1 Step 1
                sheet.Columns(1).delete()
            Next
            If iRowToDelete <> 0 Then
                sheet.Rows(iRowToDelete).delete()
            End If

            Dim nbrCol As Integer = 0
            For Each f In sheet.Rows(1).Value()
                If Not IsNothing(f) Then
                    colName.Add(Replace(Replace(f, " ", "_"), "%", "pct"))
                    nbrCol = nbrCol + 1
                End If
            Next
            Dim nbrLigne As Integer = 0
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                Else
                    Exit For
                End If
            Next
            Dim i As Integer = 0
            For index As Integer = 2 To nbrLigne
                For Each f In sheet.Rows(index).Value()
                    i = i + 1
                    If i < nbrCol + 1 Then
                        If TypeOf f Is Integer AndAlso f = My.Resources.EXCEL_ERR_VALEUR Then ' #VALEUR!
                            chiffre.Add(Nothing)
                        Else
                            chiffre.Add(f)
                        End If
                    End If
                Next
                ' remonter une erreur si les 2 tableaux n ont pas les memes tailles
                If colName.Count <> chiffre.Count Then
                    MsgBox("Problème de mise à jour de la ligne " & index & " dans la base " & tabName)
                    Continue For
                End If

                If co.Select2DistinctWheres(tabName, colName, colName, chiffre).Count > 0 Then
                    ' Update
                Else
                    ' Insert
                    co.Insert(tabName, colName, chiffre)
                End If
                i = 0
                chiffre.Clear()
            Next
            lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)
            appExcel.DisplayAlerts = False
            wbExcel.Close(False)
            'appExcel.Quit()
            appExcel.DisplayAlerts = True
            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing

            ' Supprimer le Processus Excel avec un Kill
            pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcelFinal As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcelFinal.Add(p.Id)
            Next

            For Each pid As Integer In pidExcelFinal.Except(pidExcel)
                Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
                p.CloseMainWindow()
                'p.Close()
            Next
            GC.Collect()

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    ''' <summary>
    ''' Importer un fichier excel avec des liens Bloomberg dans la base de données et met à jour les champs existants.
    ''' </summary>
    Public Sub ExcelBBGToSqlUpdate(ByVal cheminExcel As String, ByVal excelName As String, ByVal indiceFeuille As Integer, ByVal tabName As String, Optional ByVal iLigneStart As Integer = 0, Optional ByVal iColStart As Integer = 0, Optional ByVal iRowToDelete As Integer = 0)
        If fi.Existe(cheminExcel & "\" & excelName) Then

            'Lister les Processus Excel
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcel As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcel.Add(p.Id)
            Next

            Dim colName As List(Of String) = New List(Of String)
            Dim chiffre As List(Of Object) = New List(Of Object)

            'Déclaration des variables
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'Application Excel
            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel


            'Replacer le fichier en case (1,1) sans saut de ligne
            For ii = 1 To iLigneStart - 1 Step 1
                sheet.Rows(1).delete()
            Next
            For jj = 1 To iColStart - 1 Step 1
                sheet.Columns(1).delete()
            Next
            If iRowToDelete <> 0 Then
                sheet.Rows(iRowToDelete).delete()
            End If

            Dim nbrCol As Integer = 0
            For Each f In sheet.Rows(1).Value()
                If Not IsNothing(f) Then
                    colName.Add(Replace(Replace(f, " ", "_"), "%", "pct"))
                    nbrCol = nbrCol + 1
                End If
            Next
            Dim nbrLigne As Integer = 0
            For Each f In sheet.Columns(1).Value()
                If IsNothing(f) = False Then
                    nbrLigne = nbrLigne + 1
                Else
                    Exit For
                End If
            Next

            For index As Integer = 2 To nbrLigne
                Dim message As String = ""
                Dim i As Integer = 0
                Dim done = False
                For Each f As Object In sheet.Rows(index).Value()
                    i = i + 1
                    If i < nbrCol + 1 Then
                        If TypeOf f Is Integer AndAlso f = My.Resources.EXCEL_ERR_VALEUR Then ' #VALEUR!
                            chiffre.Add(Nothing)
                        ElseIf TypeOf f Is String AndAlso (CType(f, String).StartsWith("#")) Then
                            ' ignore current row
                            message = "colonne " & i & " contient " & f.ToString() & " = > ligne " & index & " ignorée"
                            Exit For
                        Else
                            chiffre.Add(f)
                        End If
                    End If
                Next

                ' remonter une erreur si les 2 tableaux n ont pas les memes tailles
                If colName.Count <> chiffre.Count Then
                    If MsgBox("Problème de mise à jour de la ligne " & index & ": " & message & ", dans la base " & excelName, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then
                        Exit Sub
                    End If
                    i = 0
                    chiffre.Clear()
                    Continue For
                End If

                If co.Select2DistinctWheres(tabName, colName, colName, chiffre).Count > 0 Then
                    ' Update
                Else
                    ' Insert
                    co.Insert(tabName, colName, chiffre)
                End If
                i = 0
                chiffre.Clear()
            Next
            lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)
            appExcel.DisplayAlerts = False
            wbExcel.Close(False)
            'appExcel.Quit()
            appExcel.DisplayAlerts = True
            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing

            ' Supprimer le Processus Excel avec un Kill
            pProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            Dim pidExcelFinal As New List(Of Integer)
            For Each p As Process In pProcess
                pidExcelFinal.Add(p.Id)
            Next

            For Each pid As Integer In pidExcelFinal.Except(pidExcel)
                Dim p As Process = System.Diagnostics.Process.GetProcessById(pid)
                p.CloseMainWindow()
                'p.Close()
            Next
            GC.Collect()

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    'retourne les donnees en colonnes
    Function ExcelIboxxSheet(ByVal cheminExcel As String, ByVal excelName As String, ByVal tabName As String, ByVal indiceFeuille As Integer, ByRef colName As List(Of String), Optional ByVal startRow As Integer = 1, Optional ByVal startCol As Integer = 1, Optional ByVal premiereLigne As Boolean = False) As List(Of List(Of Object))


        Dim appExcel As Microsoft.Office.Interop.Excel.Application = Nothing
        Try
            If fi.Existe(cheminExcel & "\" & excelName) Then
                Dim retMatrix As New List(Of List(Of Object))
                Dim chiffre As List(Of Object)

                'Déclaration des variables
                appExcel = CreateObject("Excel.Application") 'Application Excel
                appExcel.DisplayAlerts = False
                Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(Filename:=cheminExcel & "\" & excelName, [ReadOnly]:=True) 'Classeur Excel
                Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(indiceFeuille) 'Feuille Excel

                'recuperer le nb de ligne et de colonnes max. et le nom des colonnes
                Dim nbrCol As Integer = 0
                Dim nbrLigne As Integer = 0
                Dim csvFileToSplit = False ' flag pour savoir si le fichier ne contient qu'une seule colonne, car il faudra analyser le contenu avec le point virgule

                Dim f As Object
                Dim splitedCell() As String

                If Not (_colName Is Nothing) Then
                    colName = _colName
                    nbrCol = _colName.Count
                    csvFileToSplit = _csvFileToSplit
                Else
                    For ii As Integer = startCol To sheet.Columns.Count
                        f = sheet.Cells(startRow, ii).Value()
                        If IsNothing(f) = False Then
                            If (CStr(f).Length > 0) Then
                                splitedCell = Split(f.ToString, ",")
                                If (splitedCell.Count = 1) Then
                                    colName.Add(Replace(f.ToString, " ", "_"))
                                    nbrCol = nbrCol + 1
                                Else
                                    nbrCol = splitedCell.Count
                                    colName = splitedCell.ToList
                                    csvFileToSplit = True
                                    Exit For
                                End If
                            End If
                        Else
                            Exit For
                        End If
                    Next
                End If
                If (premiereLigne) Then
                    nbrLigne = 1
                Else
                    For ii As Integer = startRow + 1 To sheet.Rows.Count
                        f = sheet.Cells(ii, startCol).Value()
                        If IsNothing(f) = False Then
                            If (CStr(f).Length > 0) Then
                                nbrLigne = nbrLigne + 1
                            End If
                        Else
                            Exit For
                        End If
                    Next
                End If
                '-----------------------------------------------------
                For rowNb As Integer = startRow + 1 To startRow + nbrLigne
                    chiffre = New List(Of Object)(nbrCol)
                    For columnNb As Integer = startCol To startCol + nbrCol - 1
                        f = sheet.Cells(rowNb, columnNb).Value()

                        If (csvFileToSplit = True) Then
                            splitedCell = Split(f.ToString, ",")
                            For Each c As String In splitedCell
                                chiffre.Add(interpretiBoxxCell(c))
                            Next
                            Exit For ' passer à la ligne suivante
                        End If

                        ' soit une date , soit une notation en 1E-X
                        chiffre.Add(interpretiBoxxCell(f))

                    Next

                    retMatrix.Add(chiffre)

                Next 'fin lecture de la lignes

                'mettre en cache le resultat de l analyse fichier
                _colName = colName
                _csvFileToSplit = csvFileToSplit

                Return retMatrix
            Else
                Return Nothing
            End If
        Finally
            If (Not (appExcel Is Nothing)) Then
                appExcel.DisplayAlerts = False
                appExcel.Quit()
                appExcel = Nothing ' libère la memoire
            End If
        End Try
    End Function

    Private _colName As List(Of String) = Nothing
    Private _csvFileToSplit As Boolean

    Public Sub CacheReset()
        _colName = Nothing
        _csvFileToSplit = False
    End Sub

    Private Function interpretiBoxxCell(ByRef cell As String) As Object
        ' soit une date , soit une notation en 1E-X
        Dim resultDouble As Double
        Dim resultDate As Date
            If (Double.TryParse(cell, resultDouble)) Then
                Return resultDouble
            ElseIf Date.TryParse(cell, resultDate) Then
                Return resultDate
            End If
        Return cell
    End Function

    Private Function interpretiBoxxCell(ByRef cell As Object) As Object
        If (TypeOf (cell) Is String) Then
            Return interpretiBoxxCell(CStr(cell))
        End If
        Return cell
    End Function

    Public Function ExcelIboxxDate(ByVal cheminExcel As String, ByVal excelName As String, ByVal tabName As String, ByVal indiceFeuille As Integer, Optional ByVal iLigne As Integer = 1, Optional ByVal iCol As Integer = 1) As String
        Dim colName As List(Of String) = New List(Of String)
        Dim matrix As List(Of List(Of Object))
        Dim sDate As String = Nothing

        matrix = ExcelIboxxSheet(cheminExcel, excelName, tabName, indiceFeuille, colName, iLigne, iCol, premiereLigne:=True)
        Try
            Dim datee As Object = matrix(0)(2)
            If Not (IsNothing(datee)) Then

                If (TypeOf datee Is Date) Then
                    Dim datee_ As Date = datee
                    sDate = datee_.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                ElseIf (TypeOf datee Is String) Then
                    If datee.Contains("-") Then
                        Dim mois As String = String.Empty
                        Select Case Split(datee, "-")(1)
                            Case "Jan"
                                mois = "01"
                            Case "Feb"
                                mois = "02"
                            Case "Mar"
                                mois = "03"
                            Case "Apr"
                                mois = "04"
                            Case "May"
                                mois = "05"
                            Case "Jun"
                                mois = "06"
                            Case "Jul"
                                mois = "07"
                            Case "Aug"
                                mois = "08"
                            Case "Sep"
                                mois = "09"
                            Case "Oct"
                                mois = "10"
                            Case "Nov"
                                mois = "11"
                            Case "Dec"
                                mois = "12"
                        End Select
                        sDate = Split(datee, "-")(0) & "/" & mois & "/" & Split(datee, "-")(2)
                    End If
                End If
            End If
        Catch ex As Exception
            Return Nothing
        End Try
        Return sDate
    End Function

    ''' <summary>
    ''' Exporter un fichier excel vers une base de données
    ''' </summary>
    Public Sub ExcelIboxxToSql(ByVal cheminExcel As String, ByVal excelName As String, ByVal tabName As String, ByVal indiceFeuille As Integer, Optional ByVal iLigne As Integer = 1, Optional ByVal iCol As Integer = 1)

        'Déclaration des variables
        Dim colName As List(Of String) = New List(Of String)

        Dim matrix As List(Of List(Of Object))
        matrix = ExcelIboxxSheet(cheminExcel, excelName, tabName, indiceFeuille, colName, iLigne, iCol)

        If (matrix Is Nothing) Then
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            Exit Sub
        End If

        For Each chiffre As List(Of Object) In matrix
            co.Insert(tabName, colName, chiffre)
        Next

        lo.Log(ELog.Information, "ExcelToSql", "Le fichier excel " & excelName & " a été transféré dans la base de donnée " & tabName)

    End Sub

    ''' <summary>
    ''' Change les titres et les onglets
    ''' </summary>
    Public Sub PresentationExcel(ByVal chemin As String, ByVal onglet As List(Of String), Optional ByVal autoSize As Boolean = True)
        Dim app As Microsoft.Office.Interop.Excel.Application = CreateObject("Excel.Application")
        Dim book As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(chemin)
        Dim sheet As Microsoft.Office.Interop.Excel.Worksheet
        Dim nbCol As Integer = 0
        app.DisplayAlerts = False 'annule les messages

        For i = 1 To book.Worksheets.Count Step 1
            sheet = book.Worksheets(i)
            sheet.Name = onglet(i - 1)
            For Each f In sheet.Rows(1).Value()
                If IsNothing(f) = False Then
                    nbCol = nbCol + 1
                End If
            Next
            sheet.Range(sheet.Cells(1, 1), sheet.Cells(1, nbCol)).Interior.ColorIndex = 15
            sheet.Range(sheet.Cells(1, 1), sheet.Cells(1, nbCol)).Font.Bold = True
            sheet.Range(sheet.Cells(1, 1), sheet.Cells(1, nbCol)).Font.Size = 11
            If autoSize = True Then sheet.Cells.EntireColumn.AutoFit()
            nbCol = 0
        Next

        book.Sheets(1).Activate()

        app.ActiveWorkbook.SaveAs(chemin)
        app.DisplayAlerts = True
        app.Quit()
        app = Nothing
        book = Nothing
    End Sub


End Class
