Imports System.Windows.Controls
Imports WindowsApplication1.Utilities
Imports Microsoft.Office
Imports System.Collections.ObjectModel

Namespace FileLink

    Public Class BaseActionShowFile
        ReadOnly _sectorTree As StockTreeViewModel
        Private co As New Connection()
        Private _gridMenu As GridMenu
        Dim _files As New ObservableCollection(Of FileLink.File)
        Dim _foundFiles As New ObservableCollection(Of FileLink.File)

        Sub New(ByRef root As Stock)
            InitializeComponent()

            _sectorTree = New StockTreeViewModel(root)

            MyBase.DataContext = _sectorTree
            DGFiles.DataContext = Me
            DGFilesFound.DataContext = Me
        End Sub

        Public Sub New(ByRef root As Stock, ByVal selectedName As String)
            Me.New(root)

            _sectorTree.SearchText = selectedName
            _sectorTree.SearchCommand.Execute(Nothing)
        End Sub

#Region "Properties"
        Public Property Files() As ObservableCollection(Of FileLink.File)
            Get
                Return _files
            End Get
            Set(ByVal value As ObservableCollection(Of FileLink.File))
                _files = value
            End Set
        End Property

        Public Property FoundFiles() As ObservableCollection(Of FileLink.File)
            Get
                Return _foundFiles
            End Get
            Set(ByVal value As ObservableCollection(Of FileLink.File))
                _foundFiles = value
            End Set
        End Property
#End Region

#Region "Stock TreeView"
        Private Sub TFindTreeView_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TFindTreeView.KeyDown
            If e.Key = Windows.Input.Key.Enter Then
                _sectorTree.SearchCommand.Execute(Nothing)
            End If
        End Sub

        Private Sub BFindTreeView_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BFindTreeView.Click
            _sectorTree.SearchCommand.Execute(Nothing)
        End Sub

        Private Sub TVGeneral_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles TVGeneral.SelectedItemChanged
            Dim newStock As StockViewModel = TryCast(e.NewValue, StockViewModel)
            If newStock Is Nothing Then
                Return
            End If

            fillDGFile(newStock)
        End Sub
#End Region

#Region "Stock's Files"

        Private Sub fillDGFile(ByVal stock As StockViewModel)
            Dim sql As String
            Dim idtype As String = getTypeId(stock.Type)
            Dim list As New List(Of Dictionary(Of String, Object))

            _files.Clear()

            If idtype Is Nothing Then
                Return
            End If

            EValeur.Header = stock.Name

            sql = "SELECT CONVERT(VARCHAR, l.date, 103) As 'Date', f.fname As 'Fichier', f.url As 'Dossier', f.description As 'Description', l.onglet As 'Onglet', f.id as 'Id'"
            sql = sql & " FROM ACT_FILE f"
            sql = sql & " INNER JOIN ACT_FILE_LINK l on f.id = l.id_file"
            sql = sql & " WHERE"
            sql = sql & " l." & idtype & " = " & stock.Id
            'sql = sql & " ORDER BY f.fname"

            list = co.sqlToListDico(sql)

            For Each row As Dictionary(Of String, Object) In list
                Dim description As String = ""

                If row.ContainsKey("Description") Then
                    description = row("Description")
                End If


                Dim file As New FileLink.File(row("Date"),
                                              row("Dossier"),
                                              row("Fichier"),
                                              description,
                                              row("Id"))

                _files.Add(file)
            Next

            EValeur.IsExpanded = False
            EValeur.IsExpanded = True
        End Sub

        Private Function getTypeId(ByVal stockType As StockType) As String
            Select Case stockType
                Case stockType.VALEUR
                    Return "id_value"
                Case stockType.FGA
                    Return "id_sector_fga"
                Case stockType.ICB
                    Return "id_sector_icb"
                Case Else
                    Return Nothing
            End Select
        End Function

#End Region

#Region "Find Files"
        Private Sub findFiles()
            Dim list As New List(Of Dictionary(Of String, Object))
            Dim sql As String
            Dim whereFile As String = " f.fname LIKE '%" & TFindFile.Text & "%'"
            Dim whereDir As String = " f.url LIKE '%" & TFindFile.Text & "%'"

            If Not CBFindDirectory.IsChecked And Not CBFindFile.IsChecked Then
                Return
            End If

            sql = "SELECT f.fname As 'Fichier', f.url As 'Dossier', f.description As 'Description', f.id as 'Id'"
            sql = sql & " FROM ACT_FILE f"
            If CBFindFile.IsChecked Then
                sql = sql & " WHERE " & whereFile
                If CBFindDirectory.IsChecked Then
                    sql = sql & " OR " & whereDir
                End If
            Else
                If CBFindDirectory.IsChecked Then
                    sql = sql & " WHERE " & whereDir
                End If
            End If
            sql = sql & " ORDER BY f.fname"

            FoundFiles.Clear()

            list = co.sqlToListDico(sql)

            For Each row As Dictionary(Of String, Object) In list
                Dim description As String = ""

                If row.ContainsKey("Description") Then
                    description = row("Description")
                End If

                Dim file As New FileLink.File(row("Dossier"),
                                              row("Fichier"),
                                              description,
                                              row("Id"))

                _foundFiles.Add(file)
            Next

            EValeur.IsExpanded = False
            EValeur.IsExpanded = True
        End Sub
#End Region

#Region "Events"

        Private Sub BFindFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BFindFile.Click
            findFiles()
        End Sub

        Private Sub TFindFile_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TFindFile.KeyDown
            If e.Key = Windows.Input.Key.Enter Then
                findFiles()
            End If
        End Sub

        Private Sub BAddFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAddFile.Click
            If TVGeneral.SelectedValue Is Nothing OrElse CType(TVGeneral.SelectedValue, FileLink.StockViewModel).Id <= 0 Then
                Return
            End If

            Dim openFileDialog As New OpenFileDialog()

            ' TODO openFileDialog.InitialDirectory = "folder in config file"

            openFileDialog.SupportMultiDottedExtensions = True
            openFileDialog.Multiselect = True
            openFileDialog.RestoreDirectory = True

            If openFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Try
                    CType(TVGeneral.SelectedValue, FileLink.StockViewModel).linkFilesToStock(openFileDialog.FileNames)
                Catch ex As Exception
                    MessageBox.Show("Cannot read file from disk. Original error: " & ex.Message)
                End Try

                fillDGFile(TVGeneral.SelectedValue)
            End If

        End Sub

        Private Sub BCopyFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCopyFile.Click
            If TVGeneral.SelectedValue Is Nothing OrElse CType(TVGeneral.SelectedValue, FileLink.StockViewModel).Id <= 0 Then
                Return
            End If

            Dim copyWindow As New BaseActionCopyFile(TVGeneral.SelectedValue)

            copyWindow.ShowDialog()

            fillDGFile(TVGeneral.SelectedValue)
        End Sub

        Private Sub BDelFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BDelFile.Click
            Dim stock As StockViewModel = TVGeneral.SelectedValue
            If stock Is Nothing Then
                Return
            End If

            Dim list = DGFiles.SelectedItems

            While list.Count > 0
                deleteFileLink(list(0), stock)
            End While
        End Sub

        Private Sub DG_BDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim file As FileLink.File = CType(sender, Windows.FrameworkElement).DataContext

            deleteFileLink(file, TVGeneral.SelectedValue)
        End Sub

        Private Sub deleteFileLink(ByVal file As FileLink.File, ByVal stock As StockViewModel)
            file.deleteFileLink(stock)
            _files.Remove(file)
        End Sub

        Private Sub DataGrid_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles DGFiles.MouseDoubleClick, DGFilesFound.MouseDoubleClick
            Dim grid As DataGrid = TryCast(sender, DataGrid)
            If grid Is Nothing Then
                Return
            End If

            If grid.SelectedValue Is Nothing Then
                Return
            End If

            Dim file As FileLink.File = grid.SelectedValue
            Dim path As String = file.Url & "\" & file.Name

            openLinkedFile(path, file.Tab)
        End Sub

        Public Sub openLinkedFile(ByVal filename As String, ByVal onglet As Integer)
            Dim ext As String = System.IO.Path.GetExtension(filename)

            If ext.EndsWith(".xls") Or ext.EndsWith(".csv") Or ext.EndsWith(".xlsx") Then
                ' Fichier Excel
                Dim app As New Interop.Excel.Application
                Dim book As Interop.Excel.Workbook = app.Workbooks.Open(filename)

                If onglet >= 0 AndAlso onglet < book.Sheets.Count Then
                    Dim sheet As Interop.Excel.Worksheet = book.Sheets(onglet)
                    sheet.Activate()
                End If

                app.Visible = True
            Else
                Try
                    Process.Start(New ProcessStartInfo(filename))
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Private Sub DGFilesFound_RowEditEnding(ByVal sender As System.Object, ByVal e As System.Windows.Controls.DataGridRowEditEndingEventArgs) Handles DGFilesFound.RowEditEnding, DGFiles.RowEditEnding
            Dim editedfile As FileLink.File = e.Row.Item

            If editedfile.Name <> editedfile.Source Then
                If IO.File.Exists(editedfile.Url & "\" & editedfile.Source) Then
                    ' move file
                    IO.File.Move(editedfile.Url & "\" & editedfile.Source, editedfile.Url & "\" & editedfile.Name)
                    editedfile.Source = editedfile.Name

                    'update File in DB
                    co.Update("ACT_FILE",
                              New List(Of String)({"fname", "url", "description"}),
                              New List(Of Object)({editedfile.Source, editedfile.Url, editedfile.Description}),
                              "id",
                              editedfile.Id)
                Else
                    ' cannot remove link because if user does not have right to access folder, he will erase all links...
                End If
            End If

            
        End Sub
#End Region
    End Class
End Namespace