Imports Microsoft.Office.Interop.Excel

Public Class ExportExcel

    ''' <summary>
    ''' methode qui permet de generer une System.Data.DataTable à partir d une liste d objet de type T
    ''' Ce type T peut être une structure de plusieurs types/classe utilisateurs(Vue) et aussi de Dictionnay(String,String)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="profondeur"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertToDatatable(Of T)(ByVal list As IList(Of T), Optional ByRef profondeur As Integer = 0) As System.Data.DataTable
        Dim table As System.Data.DataTable = New System.Data.DataTable()

        ' Specifier les types des colonnes
        BuildDataTableColumns(table, GetType(T), profondeur)

        'remplir le tableau
        For Each item As T In list
            Dim row As DataRow = table.NewRow()
            FillDataTable(row, table, item, GetType(T), 1)
            table.Rows.Add(row)
        Next
        Return table
    End Function
    ''' <summary>
    ''' Utilitaire permettant de prendre une DataSet ( dataset étant une ou plusieurs DataTable)
    ''' </summary>
    ''' <param name="dataSet"></param>
    ''' <param name="outputPath"></param>
    ''' <remarks></remarks>
    Public Shared Sub ExportToExcel(ByVal dataSet As DataSet, Optional ByVal outputPath As String = Nothing)
        ' Create the Excel Application object
        Dim appExcel As Microsoft.Office.Interop.Excel.Application
        Try
            appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
        Catch ex As Exception
            appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
        End Try

        ' Create a new Excel Workbook
        Dim excelWorkbook As Workbook = appExcel.Workbooks.Add(Type.Missing)

        Dim sheetIndex As Integer = 0
        Dim col, row As Integer
        Dim excelSheet As Worksheet

        ' Copy each DataTable as a new Sheet
        For Each dt As System.Data.DataTable In dataSet.Tables

            sheetIndex += 1

            ' Copy the DataTable to an object array
            Dim rawData(dt.Rows.Count, dt.Columns.Count - 1) As Object

            ' Copy the column names to the first row of the object array
            For col = 0 To dt.Columns.Count - 1
                rawData(0, col) = dt.Columns(col).ColumnName
            Next

            ' Copy the values to the object array
            For col = 0 To dt.Columns.Count - 1
                For row = 0 To dt.Rows.Count - 1
                    rawData(row + 1, col) = dt.Rows(row).ItemArray(col)
                Next
            Next

            ' Calculate the final column letter
            Dim finalColLetter As String = String.Empty
            Dim colCharset As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Dim colCharsetLen As Integer = colCharset.Length

            If dt.Columns.Count > colCharsetLen Then
                finalColLetter = colCharset.Substring( _
                 (dt.Columns.Count - 1) \ colCharsetLen - 1, 1)
            End If

            finalColLetter += colCharset.Substring( _
              (dt.Columns.Count - 1) Mod colCharsetLen, 1)

            ' Create a new Sheet
            excelSheet = CType( _
                excelWorkbook.Sheets.Add(excelWorkbook.Sheets(sheetIndex), _
                Type.Missing, 1, XlSheetType.xlWorksheet), Worksheet)

            excelSheet.Name = dt.TableName

            ' Fast data export to Excel
            Dim excelRange As String = String.Format("A1:{0}{1}", finalColLetter, dt.Rows.Count + 1)
            excelSheet.Range(excelRange, Type.Missing).Value2 = rawData

            ' Mark the first row as BOLD
            CType(excelSheet.Rows(1, Type.Missing), Range).Font.Bold = True

            excelSheet = Nothing
        Next

        If (Not outputPath Is Nothing) Then
            ' Save and Close the Workbook
            excelWorkbook.SaveAs(outputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, _
             Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, _
             Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)

            excelWorkbook.Close(True, Type.Missing, Type.Missing)

            excelWorkbook = Nothing

            ' Release the Application object
            appExcel.Quit()
            appExcel = Nothing

            ' Collect the unreferenced objects
            GC.Collect()
            GC.WaitForPendingFinalizers()
        Else

            appExcel.Visible = True
        End If
    End Sub


    ''' <summary>
    ''' Alimente une ligne d un tableau (row) de manière recursive (avec la protextion d un compteur: profondeur)
    ''' 
    ''' </summary>
    ''' <param name="row">la ligne courante, avec </param>
    ''' <param name="table"></param>
    ''' <param name="item"></param>
    ''' <param name="TypeColumn"></param>
    ''' <param name="profondeur"></param>
    ''' <remarks></remarks>
    Private Shared Sub FillDataTable(ByRef row As DataRow, ByRef table As System.Data.DataTable, ByRef item As Object, ByVal TypeColumn As Type, Optional ByRef profondeur As Integer = 0)
        Dim fields() As Reflection.FieldInfo = TypeColumn.GetFields()
        Dim properties() As Reflection.PropertyInfo = TypeColumn.GetProperties()
        'si le type est un dictionnary: faire autant de colonnes que de clés
        If TypeColumn.IsGenericType AndAlso TypeColumn.GetGenericTypeDefinition() = GetType(Dictionary(Of ,)) Then
            Dim dico As Dictionary(Of String, String) = item
            For Each cle In dico.Keys
                If Not (table.Columns.Contains(cle.ToString)) Then
                    table.Columns.Add(cle.ToString, GetType(String))
                End If
                row(cle.ToString) = dico(cle.ToString)
            Next
            Exit Sub
        End If
        'Dans le cas d'un objet avec des champs et proprietes: recuperer les valeurs pour mettre sur la ligne
        For Each field As Reflection.FieldInfo In fields
            Dim o As Object = field.GetValue(item)
            If (table.Columns.Contains(field.Name)) Then
                row(field.Name) = If(o, DBNull.Value)
            ElseIf (profondeur > 0) Then
                FillDataTable(row, table, o, field.FieldType, profondeur - 1)
            End If
        Next

        For Each prop As Reflection.PropertyInfo In properties
            Dim o As Object = prop.GetValue(item, Nothing)
            If (table.Columns.Contains(prop.Name)) Then
                row(prop.Name) = If(o, DBNull.Value)
            ElseIf (profondeur > 0) Then
                FillDataTable(row, table, o, prop.PropertyType, profondeur - 1)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Construit la structure de la table (metadata) avec une classe ou un dictionnary(String, String)
    ''' </summary>
    ''' <param name="table"></param>
    ''' <remarks></remarks>
    Private Shared Sub BuildDataTableColumns(ByRef table As System.Data.DataTable, ByVal TypeColumn As Type, ByVal profondeur As Integer)
        'Pour des Fields
        Dim fields() As Reflection.FieldInfo = TypeColumn.GetFields()
        For Each field As Reflection.FieldInfo In fields
            Dim t1 As Type = GetExportableType(field.FieldType)
            If Not t1 Is Nothing Then
                Dim col As DataColumn = table.Columns.Add(field.Name, t1)
                col.AllowDBNull = True
            ElseIf (profondeur > 0) Then
                BuildDataTableColumns(table, field.FieldType, profondeur - 1)
            End If

        Next

        'Pour des Properties
        Dim properties() As Reflection.PropertyInfo = TypeColumn.GetProperties()
        For Each prop As Reflection.PropertyInfo In properties
            Dim t1 As Type = GetExportableType(prop.PropertyType)
            If Not t1 Is Nothing Then
                Dim col As DataColumn = table.Columns.Add(prop.Name, t1)
                col.AllowDBNull = True
            ElseIf (profondeur > 0) Then
                BuildDataTableColumns(table, prop.PropertyType, profondeur - 1)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Retourne le type affichage pour un type comme Nullable(Of) 
    ''' </summary>
    ''' <param name="t1"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetExportableType(ByRef t1 As Type) As Type
        'cas particulier si le type est un nullable, decapsuler, car la datatable n accepte pas les nullable
        If (t1.IsGenericType AndAlso t1.GetGenericTypeDefinition() Is GetType(Nullable(Of ))) Then
            Dim underlyingT As Type = Nullable.GetUnderlyingType(t1)
            Return underlyingT
        ElseIf t1.IsPrimitive Or t1.Equals(GetType(String)) Then
            Return t1
        End If
        Return Nothing
    End Function

End Class
