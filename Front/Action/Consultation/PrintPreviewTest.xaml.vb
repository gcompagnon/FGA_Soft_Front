Imports System.Windows.Markup
Imports System.Windows.Documents
Imports System.Windows.Controls
Imports System.Windows

Public Class PrintPreviewTest
    ' cree un "fixed" document
    Private monDocument As FixedDocument
    ' cree une imprimante 
    Private pd As PrintDialog

    Private Sub btnImprimeCanvas_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Selectionne une imprimante et definit mise en page
        pd = New PrintDialog()
        If (pd.ShowDialog Is Nothing) Then
            Return
        End If
        ' Cree un document
        monDocument = New FixedDocument()
        monDocument.DocumentPaginator.PageSize = New Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight)
        '1/ cree une page
        Dim page1 As FixedPage = New FixedPage()
        page1.Width = monDocument.DocumentPaginator.PageSize.Width
        page1.Height = monDocument.DocumentPaginator.PageSize.Height
        '2/ recupere le controle canvas
        Dim objCanvas As Canvas = Me.monCanvas
        '3/ necessaire supprimer canvas du grid pour l'ajouter à page1
        Me.monGrid.Children.Remove(objCanvas)
        objCanvas.Margin = New Thickness(96) ' 1 inch de marge
        '4/ ajoute canvas à page1
        page1.Children.Add(objCanvas)
        '5/ ajoute  page1 au document
        Dim page1Content As PageContent = New PageContent
        Dim Ipage1Content As IAddChild = CType(page1Content, IAddChild)
        Ipage1Content.AddChild(page1)
        monDocument.Pages.Add(page1Content)
        'REPETER LES ETAPES 1-5 POUR SECONDE PAGE
        'imprime le doc
        pd.PrintDocument(monDocument.DocumentPaginator, "My first document")
    End Sub

    Private Sub btnApercuCanvas(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Selectionne une imprimante et definit mise en page
        pd = New PrintDialog()
        If (pd.ShowDialog Is Nothing) Then
            Return
        End If
        ' Cree un document
        monDocument = New FixedDocument()
        monDocument.DocumentPaginator.PageSize = New Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight)
        '1/ cree une page
        Dim page1 As FixedPage = New FixedPage()
        page1.Width = monDocument.DocumentPaginator.PageSize.Width
        page1.Height = monDocument.DocumentPaginator.PageSize.Height
        Dim numPage As Integer = PageRangeSelection.UserPages
        'largeur & hauteur de Page Selectionnes 
        'conversion en millimetres (1 Inch=25.4 mm)
        'unites par defaut WPF exprimees = 96 dot/Inch 
        'unites en mm :u(mm)=u(wpf)*25.4/96
        Dim largPageSel As Double = pd.PrintableAreaWidth * 25.4 / 96
        Dim hautPageSel As Double = pd.PrintableAreaHeight * 25.4 / 96
        '2/ recupere le controle canvas 
        Dim objCanvas As Canvas = Me.monCanvas
        '3/memorise ses dimensions
        Dim ll As Double = objCanvas.ActualWidth
        Dim hh As Double = objCanvas.ActualHeight
        '4/ necessite de supprimer canvas du grid si on veut l'ajouter à page1
        Me.monGrid.Children.Remove(objCanvas)
        objCanvas.Margin = New Thickness(96) ' 1 inch de marge
        '5/ ajoute canvas à page1
        page1.Children.Add(objCanvas)
        '5-bis/ ajoute un titre 
        Dim monText As TextBox = New TextBox
        monText.Text = "mon canvas" & numPage.ToString & vbCrLf
        monText.Text = monText.Text & "Dimensions :" & largPageSel.ToString & "X" & hautPageSel.ToString
        Dim converter As New System.Windows.Media.BrushConverter
        'monText.Background = converter.ConvertFrom(Color.Goldenrod) 'Media.Color.FromArgb(Color.Goldenrod.A, Color.Goldenrod.R, Color.Goldenrod.G, Color.Goldenrod.B) 'New SolidBrush(Brushes.Goldenrod.
        monText.FontSize = 16
        '5-tris/ ajoute TextBox à page1
        page1.Children.Add(monText)
        '6/ ajoute  page1 au document en utilisant Interface IAddChild
        Dim page1Content As PageContent = New PageContent
        Dim Ipage1Content As IAddChild = CType(page1Content, IAddChild)
        Ipage1Content.AddChild(page1)
        monDocument.Pages.Add(page1Content)
        'Cree une fenetre Apercu et y envoie le document 
        Dim winPreview As Window = New Window
        winPreview.Title = "Apercu avant Impression par Mabrouki"
        winPreview.Content = monDocument
        winPreview.ShowDialog()
        'supprime le controle de la page(pour restauration dans grid)
        page1Content.Child.Children.Clear()
        'restaure le controle canvas avec dimensions initiales dans window
        objCanvas.Width = ll
        objCanvas.Height = hh
        objCanvas.HorizontalAlignment = Windows.HorizontalAlignment.Center
        objCanvas.VerticalAlignment = Windows.VerticalAlignment.Center
        Me.monGrid.Children.Add(objCanvas)
    End Sub
End Class
