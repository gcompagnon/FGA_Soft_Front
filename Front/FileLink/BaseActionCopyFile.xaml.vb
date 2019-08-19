Imports System.Collections.ObjectModel
Imports System.Windows.Media.Imaging
Imports System.Windows.Media
Imports System.Windows.Input
Imports System.Windows.Controls

Namespace FileLink

    Public Class BaseActionCopyFile
        Private _images As New ObservableCollection(Of FileLink.Image)
        Private _files As New ObservableCollection(Of FileLink.File)
        Private _stock As FileLink.StockViewModel
        Private _stockDir As String
        Private _oldIndex As Integer = -1
        Private _firstadd As Boolean = True

        Public Sub New(ByVal stock As FileLink.StockViewModel)
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            AddDragDropToControls(DGFile)
            AddDragDropToControls(LVImage)

            _stock = stock
            _stockDir = getDir(stock)
            MyBase.DataContext = Me
        End Sub

        Private Sub AddDragDropToControls(ByVal control As Control)
            While Not TypeOf control.Parent Is Grid
                control.AllowDrop = True
                AddHandler control.PreviewDragEnter, AddressOf LVImage_PreviewDrag
                AddHandler control.PreviewDragOver, AddressOf LVImage_PreviewDrag
                AddHandler control.PreviewDrop, AddressOf LVImage_PreviewDrop
                control = control.Parent
            End While
        End Sub

        Private Function getDir(ByVal stock As FileLink.StockViewModel) As String
            ' TODO : use config file for url
            Dim basedir As String = My.Resources.FILELINK_BASEDIR

            Return basedir & "\" & stock.getPath(False)
        End Function


#Region "Properties"

        Public Property Images() As ObservableCollection(Of FileLink.Image)
            Get
                Return _images
            End Get
            Set(ByVal value As ObservableCollection(Of FileLink.Image))
                _images = value
            End Set
        End Property

        Public Property Files() As ObservableCollection(Of FileLink.File)
            Get
                Return _files
            End Get
            Set(ByVal value As ObservableCollection(Of FileLink.File))
                _files = value
            End Set
        End Property

        Public Property Stock() As FileLink.StockViewModel
            Get
                Return _stock
            End Get
            Set(ByVal value As FileLink.StockViewModel)
                _stock = value
            End Set
        End Property
#End Region

#Region "Button General"
        Private Sub BAdd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAdd.Click
            Dim openFileDialog As New OpenFileDialog()

            If _firstadd Then
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                _firstadd = False
            End If

            openFileDialog.SupportMultiDottedExtensions = True
            openFileDialog.Multiselect = True
            openFileDialog.RestoreDirectory = True

            If openFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                For Each filename As String In openFileDialog.FileNames
                    Dim file As New FileLink.File(filename)

                    _files.Add(file)
                Next
            End If
        End Sub

        Private Sub BDel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BDel.Click
            If LVImage.SelectedIndex >= 0 AndAlso LVImage.SelectedIndex < LVImage.Items.Count Then
                _images.RemoveAt(LVImage.SelectedIndex)
            End If
        End Sub

        Private Sub BSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BSave.Click
            If Not checkNames() Then
                MessageBox.Show("Un ou plusieurs fichiers ou images n'ont pas de noms valides. Veuillez les corriger.", "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If Not IO.Directory.Exists(_stockDir) Then
                IO.Directory.CreateDirectory(_stockDir)
            End If

            For Each file As FileLink.File In Files
                copyFile(file)
                Stock.linkFileToStock(file.Url, file.Name, file.Description)
            Next

            For Each image As FileLink.Image In Images
                createImageFile(image)
                Stock.linkFileToStock(image.Url, image.Name, image.Description)
            Next

            Me.Close()
        End Sub

        Private Function checkNames() As Boolean
            For Each file As FileLink.File In Files
                If file.Name = "" Then
                    Return False
                End If
            Next

            For Each Image As FileLink.Image In Images
                If Image.Name = "" Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Sub copyFile(ByVal file As FileLink.File)
            Dim sourcePath As String = file.Url & "\" & file.Source
            Dim destPath As String

            Try
                Dim resName As String = file.getNameInDestPath(_stockDir)

                If resName = "" Then
                    MessageBox.Show("Aucun nom valide n'est renseigné. Le fichier """ & file.Name & """ ne sera pas copié", "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                file.Name = resName
                destPath = _stockDir & "\" & file.Name
                IO.File.Copy(sourcePath, destPath)
                file.Url = _stockDir
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Sub

        Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCancel.Click
            Me.Close()
        End Sub
#End Region

#Region "Image"
        Private Sub LVImage_PreviewDrag(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs)
            Dim isCorrect As Boolean = False

            If e.Data.GetDataPresent(DataFormats.FileDrop, True) Then
                Dim filenames As String() = e.Data.GetData(DataFormats.FileDrop, True)

                isCorrect = True

                For Each filename As String In filenames
                    If Not IO.File.Exists(filename) Then
                        isCorrect = False
                        Exit For
                    End If

                Next
            ElseIf e.Data.GetDataPresent(DataFormats.Bitmap, True) Then
                isCorrect = True
            End If

            If isCorrect Then
                e.Effects = Windows.DragDropEffects.All
            Else
                e.Effects = Windows.DragDropEffects.None
            End If

            e.Handled = True
        End Sub

        Private Sub LVImage_PreviewDrop(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs)
            Dim filenames As String() = e.Data.GetData(DataFormats.FileDrop, True)
            Dim info As IO.FileInfo
            Dim imgFile As FileLink.Image
            Dim file As FileLink.File

            If filenames IsNot Nothing Then
                For Each filename As String In filenames
                    info = New IO.FileInfo(filename)
                    If Not isImageExtension(info.Extension) Then
                        ' drop non image file
                        file = New FileLink.File(filename)
                        _files.Add(file)
                    Else
                        ' drop image file
                        imgFile = New FileLink.Image(filename)
                        _images.Add(imgFile)
                    End If
                Next
            ElseIf e.Data.GetDataPresent(DataFormats.Bitmap) Then
                Dim stream As System.IO.MemoryStream = CType(e.Data.GetData(DataFormats.Bitmap, True), System.IO.MemoryStream)
                Dim bmp As System.Drawing.Bitmap = Bitmap.FromStream(stream)

                addImagefromBitMap(bmp)
            End If

            e.Handled = True
        End Sub

        Private Sub CommandBinding_Executed(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            If Clipboard.ContainsData(DataFormats.Bitmap) Then
                Dim bmp As System.Drawing.Bitmap = Clipboard.GetData(DataFormats.Bitmap)

                addImagefromBitMap(bmp)
            End If
        End Sub

        Private Function isImageExtension(ByVal ext As String) As Boolean
            ext = ext.ToLower

            Return ext = ".jpg" _
                OrElse ext = ".jpeg" _
                OrElse ext = ".png" _
                OrElse ext = ".bmp" _
                OrElse ext = ".ico" _
                OrElse ext = ".gif"
        End Function

        Private Sub addImagefromBitMap(ByVal bmp As Bitmap)
            Dim imgFile As New FileLink.Image()
            imgFile.Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap,
                                                                                         IntPtr.Zero,
                                                                                         System.Windows.Int32Rect.Empty,
                                                                                         System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions)
            _images.Add(imgFile)
        End Sub

        Private Sub createImageFile(ByVal image As FileLink.Image)
            If Not IO.Path.HasExtension(image.Name) Then
                image.Name = image.Name & ".jpg"
            End If

            Dim resName As String = image.getNameInDestPath(_stockDir)

            If resName = "" Then
                MessageBox.Show("Aucun nom valide n'est renseigné. L'image """ & image.Name & """ ne sera pas copiée", "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            image.Name = resName


            Using filestream As New IO.FileStream(_stockDir & "\" & image.Name, IO.FileMode.Create)
                Dim encoder As BitmapEncoder = New PngBitmapEncoder()

                encoder.Frames.Add(BitmapFrame.Create(image.Image))
                encoder.Save(filestream)
                image.Url = _stockDir
            End Using
        End Sub
#End Region

#Region "File"
        Private Sub DG_BDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim file As FileLink.File = CType(sender, Windows.FrameworkElement).DataContext

            _files.Remove(file)
        End Sub
#End Region


    End Class
End Namespace