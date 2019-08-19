Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Windows.Controls
Imports System.ComponentModel
Imports System.Windows.Media
Imports System.Windows
Imports System.IO
Imports System.IO.Packaging
Imports System.Object
Imports System.Printing
Imports System.Drawing.Printing
Imports System.Windows.Documents
Imports System.Collections.Generic
Imports System.Windows.Markup
Imports System.Windows.Xps
Imports System.Windows.Xps.Packaging
Imports iTextSharp
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.Data.Odbc
Imports System.Reflection

Namespace Action.Consultation
    Public Class RecoPrint

        Public _name As String

        Private Sub BPrint_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BPrint.Click
            'Dim printDlg As New PrintDialog
            'If (printDlg.ShowDialog() = True) Then
            'printDlg.PrintVisual(Me, "Commande " + "Test")
            'End If
            'printDlg.ShowDialog()
            'Dim window As New PrintPreviewTest()
            'System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
            'Me.TextBox1.Text = Me.viewModel.RecoTable.Rows.Item(0)(3).ToString
            'window.Show()
            'TestPrint(Me) '.Grid1)

            'Me.BPrint.Visibility = Windows.Visibility.Collapsed
            'TestPrint(Me.RecoGrid)
            'Me.BPrint.Visibility = Windows.Visibility.Visible

            Dim saveFileDialog1 As New SaveFileDialog()
            saveFileDialog1.ShowDialog()
            If saveFileDialog1.FileName = "" Then
                MsgBox("Enter Filename to create PDF")
                Exit Sub
            Else
                ExportDataToPDFTable(saveFileDialog1)
            End If

        End Sub

        Private Sub ExportDataToPDFTable(ByVal saveFileDialog1 As SaveFileDialog)
            Dim paragraph As New System.Windows.Documents.Paragraph
            Dim doc As New Document(iTextSharp.text.PageSize.A4, 40, 40, 40, 10)
            Dim wri As PdfWriter
            Try
                wri = PdfWriter.GetInstance(doc, New FileStream(saveFileDialog1.FileName + ".pdf", FileMode.Create))


                doc.Open()

                Dim font12BoldRed As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font12Bold As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                Dim font12Normal As New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 10.0F, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)

                'Dim reader As PdfReader = New PdfReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("\INPUT\ACTION\FACTSET\template.pdf"))
                'Dim reader As New PdfReader("C:\Users\G13LP00\Desktop\template.pdf")
                'Dim Background As PdfTemplate = wri.GetImportedPage(reader, 1)

                'Dim pcb As PdfContentByte = wri.DirectContentUnder
                'pcb.AddTemplate(Background, 0, 0)

                'pcb.AddTemplate(Background, doc.PageSize.Width, doc.PageSize.Height)
                Dim p1 As New Phrase
                p1 = New Phrase(New Chunk(" ", font12BoldRed))
                doc.Add(p1)

                'Create instance of the pdf table and set the number of column in that table
                Dim PdfTable As New PdfPTable(1)
                PdfTable.TotalWidth = 490.0F
                'fix the absolute width of the table
                PdfTable.LockedWidth = True
                'relative col widths in proportions - 1,4,1,1 and 1
                Dim widths As Single() = New Single() {10.0F}
                PdfTable.SetWidths(widths)
                PdfTable.HorizontalAlignment = 1 ' 0 --> Left, 1 --> Center, 2 --> Right
                PdfTable.SpacingBefore = 2.0F

                'pdfCell Decleration
                Dim PdfPCell As PdfPCell = Nothing

                'Assigning values to each cell as phrases
                PdfPCell = New PdfPCell(New Phrase(New Chunk(_name, font12Bold)))
                'Alignment of phrase in the pdfcell
                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER
                'Add pdfcell in pdftable
                PdfTable.AddCell(PdfPCell)

                Dim dt As DataTable = GetDataTable()
                If dt IsNot Nothing Then
                    'Now add the data from datatable to pdf table
                    Dim n As Integer = 0
                    For rows As Integer = 0 To dt.Rows.Count - 1
                        For column As Integer = 0 To dt.Columns.Count - 1
                            If n Mod 2 = 0 Then
                                PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font12Bold))
                                PdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY
                            Else
                                PdfPCell = New PdfPCell(New Phrase(dt.Rows(rows)(column).ToString(), font12Normal))
                            End If
                            If column = 0 Or column = 1 Then
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT
                            Else
                                PdfPCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT
                            End If
                            PdfTable.AddCell(PdfPCell)
                            'pcb.AddTemplate(Background, 0, 0)
                        Next
                        n += 1
                    Next
                    'Adding pdftable to the pdfdocument
                    doc.Add(PdfTable)
                    'pcb.AddTemplate(Background, 0, 0)
                End If
                doc.Close()
                System.Diagnostics.Process.Start(saveFileDialog1.FileName + ".pdf")
            Catch
                MsgBox("Fichier PDF déjà ouvert! Veuillez le fermer d'abord ou changer de nom")
            End Try
        End Sub

        Private Function GetDataTable() As DataTable
            Dim dataTable As New DataTable("MyDataTable")
            'Create another DataColumn Name
            Dim dataColumn_1 As New DataColumn(_name, GetType(String))
            dataTable.Columns.Add(dataColumn_1)
            'Now Add some row to newly created dataTable
            Dim dataRow As DataRow
            ''For i As Integer = 0 To Grid1.Children.Count - 1
            'dataRow = dataTable.NewRow()
            Dim RowGrid1 As RowDefinition = New RowDefinition()
            'Dim rtb3 As New TextBlock
            'Dim rtb2 As New RichTextBox
            Dim i As Integer = 0
            For Each rowDef As Object In Grid1.Children
                dataRow = dataTable.NewRow()
                If i Mod 2 = 0 Then
                    Dim rtb3 As New TextBlock
                    rtb3 = rowDef
                    dataRow(_name) = rtb3.Text
                Else
                    Dim rtb2 As New RichTextBox
                    rtb2 = rowDef
                    dataRow(_name) = rtb2.Selection.Text
                End If
                dataTable.Rows.Add(dataRow)
                i += 1
            Next

            dataTable.AcceptChanges()
            Return dataTable
        End Function

        Private Sub TestPrint(ByVal elementToPrint_l As FrameworkElement)
            '// Create the print dialog object and set options
            Dim printDialog_l As New PrintDialog()
            'Dim elementToPrint_l As FrameworkElement = Me
            printDialog_l.PageRangeSelection = PageRangeSelection.AllPages
            printDialog_l.UserPageRangeEnabled = True

            '// Display the dialog. This returns true if the user presses the Print button.
            Dim print_l As Boolean? = printDialog_l.ShowDialog()
            Dim n As Integer = 0
            Dim hs As Integer = 0

            If print_l = True Then
                'While n < 2 AndAlso Me.scroll.VerticalOffset <> Me.scroll.ScrollableHeight

                '// Get selected printer capabilities.
                Dim capabilities_l As PrintCapabilities = printDialog_l.PrintQueue.GetPrintCapabilities(printDialog_l.PrintTicket)

                '// Compute the scale to apply to the visual in order for it to fit in the page.
                Dim scale_l As Double = Math.Min(capabilities_l.PageImageableArea.ExtentWidth / elementToPrint_l.ActualWidth,
                                          capabilities_l.PageImageableArea.ExtentHeight / elementToPrint_l.ActualHeight)
                'Dim scale_l As Double = Math.Min(850 / elementToPrint_l.ActualWidth,
                '                          capabilities_l.PageImageableArea.ExtentHeight / elementToPrint_l.ActualHeight)

                '//Save the printed element old attributes in order to render it back as
                '//it was before printing (the printing mechanism modifying the actual
                '//element render).
                '//The transformation applied to the element to print.
                Dim oldTransform_l As Transform = elementToPrint_l.LayoutTransform
                '//The size of the element to print.
                Dim oldSize_l As Size = New Size(elementToPrint_l.ActualWidth, elementToPrint_l.ActualHeight)
                '//The position of the element to print inside its parent visual.
                Dim transform_l As GeneralTransform = elementToPrint_l.TransformToAncestor(elementToPrint_l)
                Dim topLeft_l As Point = transform_l.Transform(New Point(0, 0))

                '// Transform the Visual with the computed scale factor.
                elementToPrint_l.LayoutTransform = New ScaleTransform(scale_l, scale_l)

                '// Get the size of the printer page.
                Dim size_l As Size = New Size(capabilities_l.PageImageableArea.ExtentWidth,
                                      capabilities_l.PageImageableArea.ExtentHeight)
                'Dim size_l As Size = New Size(850, capabilities_l.PageImageableArea.ExtentHeight)

                '// Update the layout of the visual to the printer page size.
                elementToPrint_l.Measure(size_l)
                elementToPrint_l.Arrange(New Rect(
                                             New Point(capabilities_l.PageImageableArea.OriginWidth,
                                                       capabilities_l.PageImageableArea.OriginHeight),
                                             size_l))

                '//Print the visual.
                printDialog_l.PrintVisual(elementToPrint_l, "Printing active panel...")

                '// Reinitialize panel's attributes.
                elementToPrint_l.LayoutTransform = oldTransform_l
                elementToPrint_l.Measure(oldSize_l)
                elementToPrint_l.Arrange(New Rect(topLeft_l, oldSize_l))

                Me.scroll.PageDown()
                Me.UpdateLayout()
                'If hs = 0 Then
                '    hs = Me.scroll.VerticalOffset
                'End If
                'elementToPrint_l = Me
                'n += 1
                'End While
            End If
        End Sub
    End Class
End Namespace
