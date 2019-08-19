Imports System
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Data
Imports System.Windows.Forms

Namespace DataGridViewPrinter
    Public Class DataGridViewPrinter

        Private TheDataFont As Font
        Private TheDataColor As Color

        Private TheDataGridView As DataGridView
        ' The DataGridView Control which will be printed
        Private ThePrintDocument As PrintDocument
        ' The PrintDocument to be used for printing
        Private IsCenterOnPage As Boolean
        ' Determine if the report will be printed in the Top-Center of the page
        Private IsWithTitle As Boolean
        ' Determine if the page contain title text
        Private TheTitleText As String
        ' The title text to be printed in each page (if IsWithTitle is set to true)
        Private TheTitleFont As Font
        ' The font to be used with the title text (if IsWithTitle is set to true)
        Private TheTitleColor As Color
        ' The color to be used with the title text (if IsWithTitle is set to true)
        Private IsWithPaging As Boolean
        ' Determine if paging is used
        Shared CurrentRow As Integer
        ' A static parameter that keep track on which Row (in the DataGridView control) that should be printed
        Public Shared PageNumber As Integer

        Private PageWidth As Integer
        Private PageHeight As Integer
        Private LeftMargin As Integer
        Private TopMargin As Integer
        Private RightMargin As Integer
        Private BottomMargin As Integer

        Private CurrentY As Single
        ' A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate
        Private RowHeaderHeight As Single
        Private RowsHeight As List(Of Single)
        Private ColumnsWidth As List(Of Single)
        Private TheDataGridViewWidth As Single

        ' Maintain a generic list to hold start/stop points for the column printing
        ' This will be used for wrapping in situations where the DataGridView will not fit on a single page
        Private mColumnPoints As List(Of Integer())
        Private mColumnPointsWidth As List(Of Single)
        Private mColumnPoint As Integer

        Private Shared PrintTitle As String = ""               ' Header of pages
        Private Shared SelectedColumns As New List(Of String)  ' The Columns Selected by user to print.
        Private Shared AvailableColumns As New List(Of String) ' All Columns avaiable in DataGridView   
        Private Shared SelectedRows As New List(Of Integer) ' All Columns avaiable in DataGridView

        Private PrintRowColors As Boolean = False 'modification according mabrouklepoux

        Private Pere As PrintPreviewForm


        Public Sub New(ByVal Pere As PrintPreviewForm)
            Me.Pere = Pere
        End Sub
        ' The class constructor
        Public Sub New(ByVal aDataGridView As DataGridView, ByVal aPrintDocument As PrintDocument, ByVal CenterOnPage As Boolean, ByVal WithTitle As Boolean, ByVal aTitleText As String, ByVal aTitleFont As Font, _
                       ByVal aTitleColor As Color, ByVal aDataFont As Font, ByVal aDataColor As Color, ByVal WithPaging As Boolean)
            Init_Parameters(aDataGridView, aPrintDocument, CenterOnPage, WithTitle, aTitleText, aTitleFont, aTitleColor, aDataFont, aDataColor, WithPaging)
        End Sub

        '= Old Constructor
        Public Sub Init_Parameters(ByVal aDataGridView As DataGridView, ByVal aPrintDocument As PrintDocument, ByVal CenterOnPage As Boolean, ByVal WithTitle As Boolean, ByVal aTitleText As String, ByVal aTitleFont As Font, _
         ByVal aTitleColor As Color, ByVal aDataFont As Font, ByVal aDataColor As Color, ByVal WithPaging As Boolean)
            TheDataGridView = aDataGridView
            ThePrintDocument = aPrintDocument
            IsCenterOnPage = CenterOnPage
            IsWithTitle = WithTitle
            TheTitleText = aTitleText
            TheTitleFont = aTitleFont
            TheTitleColor = aTitleColor
            TheDataFont = aDataFont
            TheDataColor = aDataColor
            IsWithPaging = WithPaging

            PageNumber = 0

            RowsHeight = New List(Of Single)()
            ColumnsWidth = New List(Of Single)()

            mColumnPoints = New List(Of Integer())()
            mColumnPointsWidth = New List(Of Single)()

            ' Claculating the PageWidth and the PageHeight
            If Not ThePrintDocument.DefaultPageSettings.Landscape Then
                PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width
                PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height
            Else
                PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Width
                PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Height
            End If

            ' Claculating the page margins
            LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left
            TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top
            RightMargin = ThePrintDocument.DefaultPageSettings.Margins.Right
            BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom

            ' First, the current row to be printed is the first row in the DataGridView control
            CurrentRow = 0
        End Sub

        ' The function that calculate the height of each row (including the header row), the width of each column (according to the longest text in all its cells including the header cell), and the whole DataGridView width
        Private Sub Calculate(ByVal g As Graphics)
            If PageNumber = 0 Then
                ' Just calculate once
                Dim tmpSize As New SizeF()
                Dim tmpFont As Font
                Dim tmpWidth As Single

                TheDataGridViewWidth = 0
                For i As Integer = 0 To TheDataGridView.Columns.Count - 1
                    'tmpFont = TheDataGridView.ColumnHeadersDefaultCellStyle.Font
                    tmpFont = TheDataFont

                    If tmpFont Is Nothing Then
                        tmpFont = TheDataGridView.DefaultCellStyle.Font
                        ' If there is no special HeaderFont style, then use the default DataGridView font style
                    End If

                    tmpSize = g.MeasureString(TheDataGridView.Columns(i).HeaderText, tmpFont)
                    tmpWidth = tmpSize.Width
                    RowHeaderHeight = tmpSize.Height
                    For j As Integer = 0 To TheDataGridView.Rows.Count - 1
                        If Not TheDataGridView.Rows(j).IsNewRow Then
                            'tmpFont = TheDataGridView.Rows(j).DefaultCellStyle.Font
                            tmpFont = TheDataFont
                            If tmpFont Is Nothing Then
                                tmpFont = TheDataGridView.DefaultCellStyle.Font
                                ' If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
                            End If

                            tmpSize = g.MeasureString("Anything", tmpFont)
                            RowsHeight.Add(tmpSize.Height)

                            tmpSize = g.MeasureString(TheDataGridView.Rows(j).Cells(i).EditedFormattedValue.ToString(), tmpFont)
                            If tmpSize.Width > tmpWidth Then
                                tmpWidth = tmpSize.Width
                            End If
                        End If
                    Next
                    If TheDataGridView.Columns(i).Visible Then
                        TheDataGridViewWidth += tmpWidth
                    End If
                    ColumnsWidth.Add(tmpWidth)
                Next

                ' Define the start/stop column points based on the page width and the DataGridView Width
                ' We will use this to determine the columns which are drawn on each page and how wrapping will be handled
                ' By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
                Dim k As Integer

                Dim mStartPoint As Integer = 0
                For k = 0 To TheDataGridView.Columns.Count - 1
                    If TheDataGridView.Columns(k).Visible Then
                        mStartPoint = k
                        Exit For
                    End If
                Next

                Dim mEndPoint As Integer = TheDataGridView.Columns.Count
                For k = TheDataGridView.Columns.Count - 1 To 0 Step -1
                    If TheDataGridView.Columns(k).Visible Then
                        mEndPoint = k + 1
                        Exit For
                    End If
                Next

                Dim mTempWidth As Single = TheDataGridViewWidth
                Dim mTempPrintArea As Single = CSng(PageWidth) - CSng(LeftMargin) - CSng(RightMargin)

                ' We only care about handling where the total datagridview width is bigger then the print area
                If TheDataGridViewWidth > mTempPrintArea Then
                    mTempWidth = 0.0F
                    For k = 0 To TheDataGridView.Columns.Count - 1
                        If TheDataGridView.Columns(k).Visible Then
                            mTempWidth += ColumnsWidth(k)
                            ' If the width is bigger than the page area, then define a new column print range
                            If mTempWidth > mTempPrintArea Then
                                mTempWidth -= ColumnsWidth(k)
                                mColumnPoints.Add(New Integer() {mStartPoint, mEndPoint})
                                mColumnPointsWidth.Add(mTempWidth)
                                mStartPoint = k
                                mTempWidth = ColumnsWidth(k)
                            End If
                        End If
                        ' Our end point is actually one index above the current index
                        mEndPoint = k + 1
                    Next
                End If
                ' Add the last set of columns
                mColumnPoints.Add(New Integer() {mStartPoint, mEndPoint})
                mColumnPointsWidth.Add(mTempWidth)
                mColumnPoint = 0
            End If
        End Sub

        ' The funtion that print the title, page number, and the header row
        Private Sub DrawHeader(ByVal g As Graphics)
            CurrentY = CSng(TopMargin)

            ' Printing the page number (if isWithPaging is set to true)
            If IsWithPaging Then
                PageNumber += 1
                Dim PageString As String = "Page " + PageNumber.ToString()

                Dim PageStringFormat As New StringFormat()
                PageStringFormat.Trimming = StringTrimming.Word
                PageStringFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit Or StringFormatFlags.NoClip
                PageStringFormat.Alignment = StringAlignment.Far

                Dim PageStringFont As New Font("Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point)

                Dim PageStringRectangle As New RectangleF(CSng(LeftMargin), CurrentY, CSng(PageWidth) - CSng(RightMargin) - CSng(LeftMargin), g.MeasureString(PageString, PageStringFont).Height)

                g.DrawString(PageString, PageStringFont, New SolidBrush(Color.Black), PageStringRectangle, PageStringFormat)

                CurrentY += g.MeasureString(PageString, PageStringFont).Height
            End If

            ' Printing the title (if IsWithTitle is set to true)
            If IsWithTitle Then
                Dim TitleFormat As New StringFormat()
                TitleFormat.Trimming = StringTrimming.Word
                TitleFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit Or StringFormatFlags.NoClip
                If IsCenterOnPage Then
                    TitleFormat.Alignment = StringAlignment.Center
                Else
                    TitleFormat.Alignment = StringAlignment.Near
                End If

                Dim TitleRectangle As New RectangleF(CSng(LeftMargin), CurrentY, CSng(PageWidth) - CSng(RightMargin) - CSng(LeftMargin), g.MeasureString(TheTitleText, TheTitleFont).Height)

                g.DrawString(TheTitleText, TheTitleFont, New SolidBrush(TheTitleColor), TitleRectangle, TitleFormat)

                CurrentY += g.MeasureString(TheTitleText, TheTitleFont).Height
            End If

            ' Calculating the starting x coordinate that the printing process will start from
            Dim CurrentX As Single = CSng(LeftMargin)
            If IsCenterOnPage Then
                CurrentX += ((CSng(PageWidth) - CSng(RightMargin) - CSng(LeftMargin)) - mColumnPointsWidth(mColumnPoint)) / 2.0F
            End If

            ' Setting the HeaderFore style
            Dim HeaderForeColor As Color = TheDataGridView.ColumnHeadersDefaultCellStyle.ForeColor
            If HeaderForeColor.IsEmpty Then
                HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor
                ' If there is no special HeaderFore style, then use the default DataGridView style
            End If
            Dim HeaderForeBrush As New SolidBrush(HeaderForeColor)

            ' Setting the HeaderBack style
            Dim HeaderBackColor As Color = TheDataGridView.ColumnHeadersDefaultCellStyle.BackColor
            If HeaderBackColor.IsEmpty Then
                HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor
                ' If there is no special HeaderBack style, then use the default DataGridView style
            End If
            Dim HeaderBackBrush As New SolidBrush(HeaderBackColor)

            ' Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Dim TheLinePen As New Pen(TheDataGridView.GridColor, 1)

            ' Setting the HeaderFont style
            'Dim HeaderFont As Font = TheDataGridView.ColumnHeadersDefaultCellStyle.Font
            Dim HeaderFont = TheDataFont
            If HeaderFont Is Nothing Then
                HeaderFont = TheDataGridView.DefaultCellStyle.Font
                ' If there is no special HeaderFont style, then use the default DataGridView font style
            End If

            ' Calculating and drawing the HeaderBounds        
            Dim HeaderBounds As New RectangleF(CurrentX, CurrentY, mColumnPointsWidth(mColumnPoint), RowHeaderHeight)
            g.FillRectangle(HeaderBackBrush, HeaderBounds)

            ' Setting the format that will be used to print each cell of the header row
            Dim CellFormat As New StringFormat()
            CellFormat.Trimming = StringTrimming.Word
            CellFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit Or StringFormatFlags.NoClip

            ' Printing each visible cell of the header row
            Dim CellBounds As RectangleF
            Dim ColumnWidth As Single
            For i As Integer = CInt(mColumnPoints(mColumnPoint).GetValue(0)) To CInt(mColumnPoints(mColumnPoint).GetValue(1)) - 1
                If Not TheDataGridView.Columns(i).Visible Then
                    Continue For
                End If
                ' If the column is not visible then ignore this iteration
                ColumnWidth = ColumnsWidth(i)

                ' Check the CurrentCell alignment and apply it to the CellFormat
                If TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right") Then
                    CellFormat.Alignment = StringAlignment.Far
                ElseIf TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center") Then
                    CellFormat.Alignment = StringAlignment.Center
                Else
                    CellFormat.Alignment = StringAlignment.Near
                End If

                CellBounds = New RectangleF(CurrentX, CurrentY, ColumnWidth, RowHeaderHeight)

                ' Printing the cell text
                g.DrawString(TheDataGridView.Columns(i).HeaderText, HeaderFont, HeaderForeBrush, CellBounds, CellFormat)

                ' Drawing the cell bounds
                If TheDataGridView.RowHeadersBorderStyle <> DataGridViewHeaderBorderStyle.None Then
                    g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowHeaderHeight)
                    ' Draw the cell border only if the HeaderBorderStyle is not None
                End If

                CurrentX += ColumnWidth
            Next

            CurrentY += RowHeaderHeight
        End Sub

        ' The function that print a bunch of rows that fit in one page
        ' When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
        ' When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
        Private Function DrawRows(ByVal g As Graphics) As Boolean
            ' Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Dim TheLinePen As New Pen(TheDataGridView.GridColor, 1)

            ' The style paramters that will be used to print each cell
            Dim RowFont As Font
            Dim RowForeColor As Color
            Dim RowBackColor As Color
            Dim RowForeBrush As SolidBrush
            Dim RowBackBrush As SolidBrush
            Dim RowAlternatingBackBrush As SolidBrush

            ' Setting the format that will be used to print each cell
            Dim CellFormat As New StringFormat()
            CellFormat.Trimming = StringTrimming.Word
            CellFormat.FormatFlags = StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit

            ' Printing each visible cell
            Dim RowBounds As RectangleF
            Dim CurrentX As Single
            Dim ColumnWidth As Single
            While CurrentRow < TheDataGridView.Rows.Count
                If TheDataGridView.Rows(CurrentRow).Visible And Not TheDataGridView.Rows(CurrentRow).IsNewRow Then
                    ' Print the cells of the CurrentRow only if that row is visible
                    ' Setting the row font style
                    'RowFont = TheDataGridView.Rows(CurrentRow).DefaultCellStyle.Font
                    RowFont = TheDataFont
                    If RowFont Is Nothing Then
                        RowFont = TheDataGridView.DefaultCellStyle.Font
                        ' If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
                    End If

                    ' Setting the RowFore style
                    'RowForeColor = TheDataGridView.Rows(CurrentRow).DefaultCellStyle.ForeColor
                    RowForeColor = TheDataColor
                    If RowForeColor.IsEmpty Then
                        RowForeColor = TheDataGridView.DefaultCellStyle.ForeColor
                        ' If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
                    End If
                    RowForeBrush = New SolidBrush(RowForeColor)

                    ' Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
                    RowBackColor = TheDataGridView.Rows(CurrentRow).DefaultCellStyle.BackColor
                    If RowBackColor.IsEmpty Then
                        ' If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
                        RowBackBrush = New SolidBrush(TheDataGridView.DefaultCellStyle.BackColor)
                        RowAlternatingBackBrush = New SolidBrush(TheDataGridView.AlternatingRowsDefaultCellStyle.BackColor)
                    Else
                        ' If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
                        RowBackBrush = New SolidBrush(RowBackColor)
                        RowAlternatingBackBrush = New SolidBrush(RowBackColor)
                    End If

                    ' Calculating the starting x coordinate that the printing process will start from
                    CurrentX = CSng(LeftMargin)
                    If IsCenterOnPage Then
                        CurrentX += ((CSng(PageWidth) - CSng(RightMargin) - CSng(LeftMargin)) - mColumnPointsWidth(mColumnPoint)) / 2.0F
                    End If

                    ' Calculating the entire CurrentRow bounds                
                    RowBounds = New RectangleF(CurrentX, CurrentY, mColumnPointsWidth(mColumnPoint), RowsHeight(CurrentRow))

                    ' Filling the back of the CurrentRow
                    If Me.PrintRowColors = False Then 'modification according mabrouklepoux
                        If CurrentRow Mod 2 = 0 Then
                            g.FillRectangle(RowBackBrush, RowBounds)
                        Else
                            g.FillRectangle(RowAlternatingBackBrush, RowBounds)
                        End If
                    End If
                    For CurrentCell As Integer = CInt(mColumnPoints(mColumnPoint).GetValue(0)) To CInt(mColumnPoints(mColumnPoint).GetValue(1)) - 1

                        ' Printing each visible cell of the CurrentRow                
                        If Not TheDataGridView.Columns(CurrentCell).Visible Then
                            Continue For
                        End If
                        ' If the cell is belong to invisible column, then ignore this iteration
                        ' Check the CurrentCell alignment and apply it to the CellFormat
                        If TheDataGridView.Columns(CurrentCell).DefaultCellStyle.Alignment.ToString().Contains("Right") Then
                            CellFormat.Alignment = StringAlignment.Far
                        ElseIf TheDataGridView.Columns(CurrentCell).DefaultCellStyle.Alignment.ToString().Contains("Center") Then
                            CellFormat.Alignment = StringAlignment.Center
                        Else
                            CellFormat.Alignment = StringAlignment.Near
                        End If

                        ColumnWidth = ColumnsWidth(CurrentCell)
                        Dim CellBounds As New RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight(CurrentRow))
                        If PrintRowColors = True Then 'block added by mabrouklepoux
                            ' printing the cell backcolor 
                            g.FillRectangle(New SolidBrush(TheDataGridView.Rows(CurrentRow).Cells(CurrentCell).Style.BackColor), CellBounds)
                        End If

                        ' Printing the cell text
                        g.DrawString(TheDataGridView.Rows(CurrentRow).Cells(CurrentCell).EditedFormattedValue.ToString(), RowFont, RowForeBrush, CellBounds, CellFormat)

                        ' Drawing the cell bounds
                        If TheDataGridView.CellBorderStyle <> DataGridViewCellBorderStyle.None Then
                            g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowsHeight(CurrentRow))
                            ' Draw the cell border only if the CellBorderStyle is not None
                        End If

                        CurrentX += ColumnWidth
                    Next
                    CurrentY += RowsHeight(CurrentRow)

                    ' Checking if the CurrentY is exceeds the page boundries
                    ' If so then exit the function and returning true meaning another PagePrint action is required
                    If CInt(CurrentY) > (PageHeight - TopMargin - BottomMargin) Then
                        CurrentRow += 1
                        Return True
                    End If
                End If
                CurrentRow += 1
            End While

            CurrentRow = 0
            mColumnPoint += 1
            ' Continue to print the next group of columns
            If mColumnPoint = mColumnPoints.Count Then
                ' Which means all columns are printed
                mColumnPoint = 0
                Return False
            Else
                Return True
            End If
        End Function

        ' The method that calls all other functions
        Public Function DrawDataGridView(ByVal g As Graphics) As Boolean
            Try
                Calculate(g)
                DrawHeader(g)
                Dim bContinue As Boolean = DrawRows(g)
                Return bContinue
            Catch ex As Exception
                MessageBox.Show("Operation failed: " + ex.Message.ToString(), Application.ProductName + " - Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                Return False
            End Try
        End Function

        Public Function SetupThePrinting(ByVal myDataGridView As DataGridView, ByVal ThePrintDocument As PrintDocument, Optional ByVal TitleText As String = "") As Boolean
            Dim myDataGridViewPrint As New DataGridView
            Dim iRow As Integer
            Dim iCol As Integer = 0
            Dim RowCount As Integer = 0
            Dim IsMultiselected As Boolean = myDataGridView.SelectedRows.Count > 1

            AvailableColumns.Clear()
            SelectedColumns.Clear()
            SelectedRows.Clear()

            With myDataGridViewPrint
                With .ColumnHeadersDefaultCellStyle
                    .BackColor = Color.Navy
                    .ForeColor = Color.White
                    .Font = New Font(myDataGridViewPrint.Font, FontStyle.Bold)
                End With
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
                .CellBorderStyle = DataGridViewCellBorderStyle.Single
                .GridColor = Color.Black
                ' Set the selection background color for all the cells.
                .DefaultCellStyle.SelectionBackColor = Color.Yellow 'Color.White
                .DefaultCellStyle.SelectionForeColor = Color.Navy   'Color.Black

                ' Set RowHeadersDefaultCellStyle.SelectionBackColor so that its default
                ' value won't override DataGridView.DefaultCellStyle.SelectionBackColor.
                .RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty

                ' Set the background color for all rows and for alternating rows. 
                ' The value for alternating rows overrides the value for all rows. 
                .RowsDefaultCellStyle.BackColor = Color.LightGray
                .AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke  'Color.DarkGray
            End With

            'myDataGridViewPrint.Columns.Clear()
            'myDataGridViewPrint = myDataGridView1
            For Each c As DataGridViewColumn In myDataGridView.Columns
                AvailableColumns.Add(c.HeaderText)
            Next
            ' Show PrintOption Form
            Dim dlg As New PrintOptions(TitleText, AvailableColumns, Not IsMultiselected, Me.Pere)
            If dlg.ShowDialog() <> DialogResult.OK Then Exit Function

            SelectedColumns = dlg.GetSelectedColumns
            Dim SelectedColumnsName As New List(Of String)

            For Each col As String In SelectedColumns
                For Each column As DataGridViewColumn In myDataGridView.Columns
                    If column.HeaderText = col Then SelectedColumnsName.Add(column.Name)
                Next
            Next

            For iRow = 0 To myDataGridView.Rows.Count - 1
                If Not myDataGridView.Rows(iRow).IsNewRow Then
                    If Not dlg.PrintAllRows Then
                        If myDataGridView.Rows(iRow).Selected = True Then
                            SelectedRows.Add(iRow)
                            RowCount += 1
                        End If
                    Else
                        SelectedRows.Add(iRow)
                        RowCount += 1
                    End If
                End If
            Next
            myDataGridViewPrint.ColumnCount = SelectedColumns.Count
            myDataGridViewPrint.Rows.Add(RowCount)

            Dim iRow2 As Integer = 0
            For Each i As String In SelectedColumnsName  'Bug corrected SelectedColumns
                myDataGridViewPrint.Columns(iCol).Name = i
                myDataGridViewPrint.Columns(iCol).HeaderText = i
                For iRow = 0 To RowCount - 1
                    myDataGridViewPrint.Rows(iRow).Cells(iCol).Style = myDataGridView.Rows(SelectedRows(iRow)).Cells(i).Style
                    myDataGridViewPrint.Rows(iRow).Cells(iCol).Value = myDataGridView.Rows(SelectedRows(iRow)).Cells(i).Value
                Next
                iCol += 1
            Next

            Dim MyPrintDialog As New PrintDialog()
            MyPrintDialog.AllowCurrentPage = False
            MyPrintDialog.AllowPrintToFile = False
            MyPrintDialog.AllowSelection = False
            MyPrintDialog.AllowSomePages = False
            MyPrintDialog.PrintToFile = False
            MyPrintDialog.ShowHelp = False
            MyPrintDialog.ShowNetwork = False

            If MyPrintDialog.ShowDialog() <> DialogResult.OK Then
                Return False
            End If

            Me.PrintRowColors = dlg.PrintRowColors  'modification according mabrouklepoux

            ThePrintDocument.DocumentName = dlg.PrintTitle
            ThePrintDocument.PrinterSettings = MyPrintDialog.PrinterSettings
            ThePrintDocument.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings
            ThePrintDocument.DefaultPageSettings.Margins = New Margins(40, 40, 40, 40)

            Init_Parameters(myDataGridViewPrint, ThePrintDocument, dlg.PrintCenterReportOnPage, True, ThePrintDocument.DocumentName, New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, dlg.PrintFont, dlg.PrintFontColor, True)
            Return True
        End Function
    End Class
End Namespace