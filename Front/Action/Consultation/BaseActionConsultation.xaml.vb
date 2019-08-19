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
Imports System.Windows.Controls.Primitives

Namespace Action.Consultation
    Public Class BaseActionConsultation
        Property viewModel As BaseActionConsultationViewModel
        Private selectedTicker As String = ""

        Sub New()

            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            viewModel = Me.DataContext

            viewModel.load()
            'viewModel.initialize(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem)

            '''''''''''FIXME'''''''''''''''
            viewModel.TestDoublons()
            '''''''''''END FIXME'''''''''''''''

            'DGCoissance.ItemsSource = viewModel.DTCroissance.DefaultView
            'viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem)
            Me.DGGeneral.ItemsSource = Me.viewModel.GenTableView
            Me.DGCroissance.ItemsSource = Me.viewModel.CroTableView
            Me.DGQualite.ItemsSource = Me.viewModel.QuaTableView
            Me.DGValorisation.ItemsSource = Me.viewModel.ValTableView
            Me.DGMomentum.ItemsSource = Me.viewModel.MomTableView
            Me.DGSynthese.ItemsSource = Me.viewModel.SynTableView
        End Sub

        Private Sub DGFormats()
            ' Make a DataGridTableStyle to map this DataTable.
            Dim table_style As New DataGridTableStyle
            table_style.MappingName = viewModel.GenTable.TableName

            Dim quantity_style As New DataGridTextBoxColumn
            quantity_style.Alignment = HorizontalAlignment.Right
            quantity_style.Format = "n2"
            quantity_style.MappingName = "Quantity"
            quantity_style.HeaderText = "Qty"
            table_style.GridColumnStyles.Add(quantity_style)
            Dim i As Integer = 145
            Dim s As String
            s = i.ToString("N10 %")

            ' Add the DataGridTableStyle to the DataGrid
            ' so it knows how to map this table.
            'System.Windows.Controls.DataGrid
            'DGGeneral.Style.
            'DGGeneral.TableStyles.Add(table_style)
        End Sub

        Private Sub DoublonsMenu_Click()
            Dim window As New CorrectImportStockWindow()

            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
            window.Show()
        End Sub
        Private Sub ScoreRecoMenu_Click()
            Dim bs As New BaseAction
            bs.Show()
        End Sub
        Private Sub BLoad_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BLoad.Click

            Dim tabIndex As Integer = TabControl1.SelectedIndex
            If TabControl1.SelectedIndex = 0 Then
                TabControl1.SelectedIndex = 1
            Else
                TabControl1.SelectedIndex = 0
            End If
            TabControl1.SelectedIndex = tabIndex
            Me.UpdateLayout()

            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Gen")
            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Cro")
            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Qua")
            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Val")
            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Mom")
            viewModel.FindDatas(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem, "Syn")
            viewModel.setFormat()


            Me.DGGeneral.ItemsSource = Me.viewModel.GenTableView
            Me.DGCroissance.ItemsSource = Me.viewModel.CroTableView
            Me.DGQualite.ItemsSource = Me.viewModel.QuaTableView
            Me.DGValorisation.ItemsSource = Me.viewModel.ValTableView
            Me.DGMomentum.ItemsSource = Me.viewModel.MomTableView
            Me.DGSynthese.ItemsSource = Me.viewModel.SynTableView

            Me.DGGeneral.FrozenColumnCount = 5
            Me.DGCroissance.FrozenColumnCount = 5
            Me.DGQualite.FrozenColumnCount = 5
            Me.DGValorisation.FrozenColumnCount = 5
            Me.DGMomentum.FrozenColumnCount = 5
            Me.DGSynthese.FrozenColumnCount = 5

            Dim x As Integer = Me.viewModel.GenTable.Rows.Count

            Dim style As New Style
            style.Setters.Add(New Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Left))

            If TabControl1.SelectedIndex = 0 And Me.DGGeneral.Columns.Count > 0 Then
                Me.DGGeneral.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGGeneral.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGGeneral.Columns(6).Visibility = Windows.Visibility.Hidden
                DGGeneral.Columns(0).CellStyle = style
                DGGeneral.Columns(1).CellStyle = style
                DGGeneral.Columns(2).CellStyle = style
                DGGeneral.Columns(4).CellStyle = style
            ElseIf TabControl1.SelectedIndex = 1 And Me.DGCroissance.Columns.Count > 0 Then
                Me.DGCroissance.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGCroissance.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGCroissance.Columns(6).Visibility = Windows.Visibility.Hidden
                DGCroissance.Columns(0).CellStyle = style
                DGCroissance.Columns(1).CellStyle = style
                DGCroissance.Columns(2).CellStyle = style
                DGCroissance.Columns(4).CellStyle = style
            ElseIf TabControl1.SelectedIndex = 2 And Me.DGQualite.Columns.Count > 0 Then
                Me.DGQualite.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGQualite.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGQualite.Columns(6).Visibility = Windows.Visibility.Hidden
                DGQualite.Columns(0).CellStyle = style
                DGQualite.Columns(1).CellStyle = style
                DGQualite.Columns(2).CellStyle = style
                DGQualite.Columns(4).CellStyle = style
            ElseIf TabControl1.SelectedIndex = 3 And Me.DGValorisation.Columns.Count > 0 Then
                Me.DGValorisation.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGValorisation.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGValorisation.Columns(6).Visibility = Windows.Visibility.Hidden
                DGValorisation.Columns(0).CellStyle = style
                DGValorisation.Columns(1).CellStyle = style
                DGValorisation.Columns(2).CellStyle = style
                DGValorisation.Columns(4).CellStyle = style
            ElseIf TabControl1.SelectedIndex = 4 And Me.DGMomentum.Columns.Count > 0 Then
                Me.DGMomentum.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGMomentum.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGMomentum.Columns(6).Visibility = Windows.Visibility.Hidden
                DGMomentum.Columns(0).CellStyle = style
                DGMomentum.Columns(1).CellStyle = style
                DGMomentum.Columns(2).CellStyle = style
                DGMomentum.Columns(4).CellStyle = style
            ElseIf TabControl1.SelectedIndex = 5 And Me.DGSynthese.Columns.Count > 0 Then
                Me.DGSynthese.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGSynthese.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGSynthese.Columns(6).Visibility = Windows.Visibility.Hidden
                DGSynthese.Columns(0).CellStyle = style
                DGSynthese.Columns(1).CellStyle = style
                DGSynthese.Columns(2).CellStyle = style
                DGSynthese.Columns(4).CellStyle = style
            End If
        End Sub

        Private Sub MyTabControl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DGGeneral.CurrentCellChanged,
            DGCroissance.CurrentCellChanged, DGQualite.CurrentCellChanged, DGValorisation.CurrentCellChanged, DGMomentum.CurrentCellChanged, DGSynthese.CurrentCellChanged
            Dim style As New Style
            Dim n As Integer = 0

            style.Setters.Add(New Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Left))
            If TabControl1.SelectedIndex = 0 And Me.DGGeneral.Columns.Count > 0 Then
                Me.DGGeneral.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGGeneral.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGGeneral.Columns(6).Visibility = Windows.Visibility.Hidden
                DGGeneral.Columns(0).CellStyle = style
                DGGeneral.Columns(1).CellStyle = style
                DGGeneral.Columns(2).CellStyle = style
                DGGeneral.Columns(4).CellStyle = style
                If DGGeneral.CurrentItem IsNot Nothing AndAlso TypeOf (DGGeneral.CurrentItem) Is System.Data.DataRowView AndAlso DGGeneral.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGGeneral.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGGeneral.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGGeneral.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            ElseIf TabControl1.SelectedIndex = 1 And Me.DGCroissance.Columns.Count > 0 Then
                Me.DGCroissance.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGCroissance.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGCroissance.Columns(6).Visibility = Windows.Visibility.Hidden
                DGCroissance.Columns(0).CellStyle = style
                DGCroissance.Columns(1).CellStyle = style
                DGCroissance.Columns(2).CellStyle = style
                DGCroissance.Columns(4).CellStyle = style
                If DGCroissance.CurrentItem IsNot Nothing AndAlso TypeOf (DGCroissance.CurrentItem) Is System.Data.DataRowView AndAlso DGCroissance.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGCroissance.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGCroissance.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGCroissance.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            ElseIf TabControl1.SelectedIndex = 2 And Me.DGQualite.Columns.Count > 0 Then
                Me.DGQualite.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGQualite.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGQualite.Columns(6).Visibility = Windows.Visibility.Hidden
                DGQualite.Columns(0).CellStyle = style
                DGQualite.Columns(1).CellStyle = style
                DGQualite.Columns(2).CellStyle = style
                DGQualite.Columns(4).CellStyle = style
                If DGQualite.CurrentItem IsNot Nothing AndAlso TypeOf (DGQualite.CurrentItem) Is System.Data.DataRowView AndAlso DGQualite.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGQualite.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGQualite.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGQualite.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            ElseIf TabControl1.SelectedIndex = 3 And Me.DGValorisation.Columns.Count > 0 Then
                Me.DGValorisation.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGValorisation.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGValorisation.Columns(6).Visibility = Windows.Visibility.Hidden
                DGValorisation.Columns(0).CellStyle = style
                DGValorisation.Columns(1).CellStyle = style
                DGValorisation.Columns(2).CellStyle = style
                DGValorisation.Columns(4).CellStyle = style
                If DGValorisation.CurrentItem IsNot Nothing AndAlso TypeOf (DGValorisation.CurrentItem) Is System.Data.DataRowView AndAlso DGValorisation.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGValorisation.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGValorisation.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGValorisation.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            ElseIf TabControl1.SelectedIndex = 4 And Me.DGMomentum.Columns.Count > 0 Then
                Me.DGMomentum.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGMomentum.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGMomentum.Columns(6).Visibility = Windows.Visibility.Hidden
                DGMomentum.Columns(0).CellStyle = style
                DGMomentum.Columns(1).CellStyle = style
                DGMomentum.Columns(2).CellStyle = style
                DGMomentum.Columns(4).CellStyle = style
                If DGMomentum.CurrentItem IsNot Nothing AndAlso TypeOf (DGMomentum.CurrentItem) Is System.Data.DataRowView AndAlso DGMomentum.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGMomentum.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGMomentum.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGMomentum.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            ElseIf TabControl1.SelectedIndex = 5 And Me.DGSynthese.Columns.Count > 0 Then
                Me.DGSynthese.Columns(3).Visibility = Windows.Visibility.Hidden
                Me.DGSynthese.Columns(5).Visibility = Windows.Visibility.Hidden
                Me.DGSynthese.Columns(6).Visibility = Windows.Visibility.Hidden
                DGSynthese.Columns(0).CellStyle = style
                DGSynthese.Columns(1).CellStyle = style
                DGSynthese.Columns(2).CellStyle = style
                DGSynthese.Columns(4).CellStyle = style
                If DGSynthese.CurrentItem IsNot Nothing AndAlso TypeOf (DGSynthese.CurrentItem) Is System.Data.DataRowView AndAlso DGSynthese.CurrentItem.Row(0) IsNot DBNull.Value Then
                    Me.selectedTicker = DGSynthese.CurrentItem.Row(0)
                ElseIf Me.selectedTicker <> "" Then
                    For Each row As System.Data.DataRowView In DGSynthese.Items
                        If row.Row(0) IsNot DBNull.Value AndAlso row.Row(0) = Me.selectedTicker Then
                            DGSynthese.SelectedItem = row
                        End If
                        n += 1
                    Next
                End If
            End If
            Me.DGGeneral.ItemsSource = Me.viewModel.GenTableView
            Me.DGCroissance.ItemsSource = Me.viewModel.CroTableView
            Me.DGQualite.ItemsSource = Me.viewModel.QuaTableView
            Me.DGValorisation.ItemsSource = Me.viewModel.ValTableView
            Me.DGMomentum.ItemsSource = Me.viewModel.MomTableView
            Me.DGSynthese.ItemsSource = Me.viewModel.SynTableView
        End Sub

        Private Sub CBUnivers_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CBUniverse.SelectionChanged
            viewModel.SuperSecteurs.Clear()
            viewModel.Secteurs.Clear()
            viewModel.SelectedUniverse = CBUniverse.SelectedItem
            viewModel.loadSecteur()
        End Sub

        Private Sub CBSuperSecteur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CBSuperSecteur.SelectionChanged
            viewModel.FillSecteurs(CBSuperSecteur.SelectedItem)
            'viewModel.FillValeurs(CBSuperSecteur.SelectedItem)
        End Sub

        Private Sub CBSecteur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CBSecteur.SelectionChanged
            'viewModel.filterDTCroissance(CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem)
            'viewModel.FillValeurs(CBSecteur.SelectedItem)
        End Sub

        

        Private Sub dataGrid_LoadingRow(ByVal sender As Object, ByVal e As DataGridRowEventArgs)
            ' Get the DataRow corresponding to the DataGridRow that is loading.
            Dim item As DataRowView = TryCast(e.Row.Item, DataRowView)

            If item IsNot Nothing Then
                Dim row As DataRow = item.Row

                If row("SECTOR") Is DBNull.Value Then
                    e.Row.Background = New SolidColorBrush(Colors.Peru)
                ElseIf row("INDUSTRY") Is DBNull.Value Then
                    e.Row.Background = New SolidColorBrush(Colors.Tan)
                    e.Row.FontWeight = FontWeights.Bold
                ElseIf row("Isin") Is DBNull.Value Then
                    e.Row.Background = New SolidColorBrush(Colors.PaleGoldenrod)
                    e.Row.FontWeight = FontWeights.Bold
                Else
                    e.Row.Background = New SolidColorBrush(Colors.White)
                    e.Row.FontWeight = FontWeights.Normal
                End If
                e.Row.Header = e.Row.GetIndex
            End If
        End Sub

        Private Sub TestPrint(ByVal elementToPrint_l As FrameworkElement)
            '// Create the print dialog object and set options
            Dim printDialog_l As New PrintDialog()
            printDialog_l.PageRangeSelection = PageRangeSelection.AllPages
            printDialog_l.UserPageRangeEnabled = True

            '// Display the dialog. This returns true if the user presses the Print button.
            Dim print_l As Boolean? = printDialog_l.ShowDialog()
            If print_l = True Then
                '// Get selected printer capabilities.
                Dim capabilities_l As PrintCapabilities = printDialog_l.PrintQueue.GetPrintCapabilities(printDialog_l.PrintTicket)

                '// Compute the scale to apply to the visual in order for it to fit in the page.
                Dim scale_l As Double = Math.Min(capabilities_l.PageImageableArea.ExtentWidth / elementToPrint_l.ActualWidth,
                                          capabilities_l.PageImageableArea.ExtentHeight / elementToPrint_l.ActualHeight)

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
            End If
        End Sub

        Private Sub BReco_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BReco.Click
            viewModel.FindReco(CBDate.SelectedValue, CBUniverse.SelectedValue, CBSuperSecteur.SelectedItem, CBSecteur.SelectedItem)
            Dim window As New RecoPrint
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
            window.Show()

            Dim s As String
            Dim i As Integer = 0
            For Each row In Me.viewModel.RecoTable.Rows
                Dim rtb1 As New TextBox
                rtb1.Text = row("Secteur") + " | " + row("IndustryFGA") + " | " + row("AssetName")
                Dim rtb3 As New TextBlock
                rtb3.Inlines.Add(row("Secteur") + " | " + row("IndustryFGA") + " | ")
                rtb3.Inlines.Add(row("AssetName"))
                rtb3.Inlines.Add(" | ")
                '-----------------------------------------------------------------
                If row("MXEU") IsNot DBNull.Value AndAlso row("MXEU") <> "" Then
                    rtb3.Inlines.Add("MXEU: ")
                    rtb3.Inlines.Add(row("MXEU"))
                End If
                If row("MXEM") IsNot DBNull.Value AndAlso row("MXEM") <> "" Then
                    rtb3.Inlines.Add(" , MXEM: ")
                    rtb3.Inlines.Add(row("MXEM"))
                End If
                If row("MXEUM") IsNot DBNull.Value AndAlso row("MXEUM") <> "" Then
                    rtb3.Inlines.Add(" , MXEUM: ")
                    rtb3.Inlines.Add(row("MXEUM"))
                End If
                '-----------------------------------------------------------------
                If row("MXUSLC") IsNot DBNull.Value AndAlso ("MXUSLC") <> "" Then
                    rtb3.Inlines.Add(" | MXUSLC: ")
                    rtb3.Inlines.Add(row("MXUSLC"))
                End If

                Dim rtb2 As New RichTextBox
                s = row("Recommandation")
                Dim stream As MemoryStream = New MemoryStream(ASCIIEncoding.Default.GetBytes(s))
                rtb2.Selection.Load(stream, DataFormats.Rtf)
                Dim RowGrid1 As RowDefinition = New RowDefinition()
                Dim RowGrid2 As RowDefinition = New RowDefinition()
                window.Grid1.RowDefinitions.Add(RowGrid1)
                window.Grid1.RowDefinitions.Add(RowGrid2)
                rtb3.Background = New SolidColorBrush(Colors.PaleGoldenrod)
                Grid.SetRow(rtb3, i)
                i += 1
                Grid.SetRow(rtb2, i)
                i += 1
                window.Grid1.Children.Add(rtb3)
                window.Grid1.Children.Add(rtb2)
            Next

            window._name = "Recommandations" + " : {" + CBUniverse.Text + ", " + CBSuperSecteur.Text + "}"
        End Sub

        Private Sub btnFilter_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim i As Integer
            Me.viewModel.SelectedCol.Clear()
            Me.viewModel.ColValues.Clear()

            If TabControl1.SelectedIndex = 0 AndAlso DGGeneral.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGGeneral.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.GenTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Visible
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 1 AndAlso DGCroissance.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGCroissance.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.CroTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Visible
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 2 AndAlso DGQualite.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGQualite.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.QuaTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Visible
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 3 AndAlso DGValorisation.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGValorisation.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.ValTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Visible
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 4 AndAlso DGMomentum.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGMomentum.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.MomTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Visible
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 5 AndAlso DGSynthese.SelectedCells.Count <> 0 Then
                Me.viewModel.SelectedCol.Add(DGSynthese.SelectedCells.First.Column.Header.ToString)
                For Each row As DataRow In Me.viewModel.SynTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(True, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Visible
            Else
                Return
            End If

            Me.viewModel.ColValues.Add(New ColumnFilter(True, ""))
            PopUp.IsOpen = True
        End Sub
        Private Sub btnFilter_Click2()
            Dim i As Integer
            Me.viewModel.ColValues.Clear()

            If TabControl1.SelectedIndex = 0 AndAlso DGGeneral.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.GenTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Visible
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 1 AndAlso DGCroissance.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.CroTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Visible
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 2 AndAlso DGQualite.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.QuaTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Visible
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 3 AndAlso DGValorisation.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.ValTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Visible
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 4 AndAlso DGMomentum.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.MomTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Visible
                BValider_SynClick.Visibility = Windows.Visibility.Collapsed
            ElseIf TabControl1.SelectedIndex = 5 AndAlso DGSynthese.SelectedCells.Count <> 0 Then
                For Each row As DataRow In Me.viewModel.SynTable.Rows
                    If row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value Then
                        Dim c As New ColumnFilter(Me.viewModel.checkBoxSelectAll, row(Me.viewModel.SelectedCol.Item(0)))
                        i = 0
                        While Me.viewModel.ColValues.Count > 0 AndAlso i < Me.viewModel.ColValues.Count AndAlso Me.viewModel.ColValues(i).Value <> c.Value
                            i += 1
                        End While
                        If Me.viewModel.ColValues.Count = 0 Or i >= Me.viewModel.ColValues.Count Then
                            Me.viewModel.ColValues.Add(c)
                        End If
                    End If
                Next
                BValider_GenClick.Visibility = Windows.Visibility.Collapsed
                BValider_CroClick.Visibility = Windows.Visibility.Collapsed
                BValider_QuaClick.Visibility = Windows.Visibility.Collapsed
                BValider_ValClick.Visibility = Windows.Visibility.Collapsed
                BValider_MomClick.Visibility = Windows.Visibility.Collapsed
                BValider_SynClick.Visibility = Windows.Visibility.Visible
            Else
                Return
            End If

            Me.viewModel.ColValues.Add(New ColumnFilter(True, ""))
            PopUp.IsOpen = True
        End Sub
        Private Sub Valider_GenClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.GenTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.GenTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub Valider_CroClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.CroTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.CroTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub Valider_QuaClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.QuaTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.QuaTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub Valider_ValClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.ValTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.ValTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub Valider_MomClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.MomTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.MomTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub Valider_SynClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim toDelete As Boolean = True
            Dim i As Integer = 0
            Dim j As New List(Of Integer)
            For Each row As DataRow In Me.viewModel.SynTable.Rows
                toDelete = True
                For Each s As ColumnFilter In Me.viewModel.ColValues
                    If ((s.Value = "" AndAlso row(Me.viewModel.SelectedCol.Item(0)) Is DBNull.Value) Or
                        (row(Me.viewModel.SelectedCol.Item(0)) IsNot DBNull.Value AndAlso row(Me.viewModel.SelectedCol.Item(0)) = s.Value)) AndAlso s.Check Then
                        toDelete = False
                        Exit For
                    End If
                Next
                If toDelete Then
                    j.Add(i)
                End If
                i += 1
            Next
            PopUp.IsOpen = False
            For n = j.Count - 1 To 0 Step -1
                Me.viewModel.SynTable.Rows.RemoveAt(j(n))
            Next
            Me.UpdateLayout()
        End Sub
        Private Sub CheckBoxHeader_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            PopUp.IsOpen = False
            Me.viewModel.checkBoxSelectAll = True
            btnFilter_Click2()
            'For Each item As ColumnFilter In Me.viewModel.ColValues
            '    item.Check = True
            'Next
            'Me.PopUp.UpdateLayout()
            'Me.UpdateLayout()
            'Me.viewModel.OnPropertyChanged("ColValues")
        End Sub
        Private Sub UncheckBoxHeader_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            PopUp.IsOpen = False
            Me.viewModel.checkBoxSelectAll = False
            btnFilter_Click2()
        End Sub

        Private Sub columnHeader_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            'Dim columnHeader As DataGridColumnHeader = sender

            'If columnHeader IsNot DBNull.Value Then
            '    Me.viewModel.GenTable.DefaultView.Sort = columnHeader.Content '"COUNTRY" 'columnHeader.Content
            'End If
        End Sub

    End Class

End Namespace