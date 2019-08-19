Imports System.Collections.Generic
Imports System.Text
Imports System.Windows.Documents
Imports System.Windows.Markup
Imports System.Windows.Controls
Imports System.Windows.Xps
Imports System.Windows
Imports System.Printing
Imports System.Windows.Media


''' <summary>
''' Various helpers for printing WPF UI to a printer
''' </summary>
Public Class WpfPrint
#Region "Supporting Types"
    '****************************************************************************************
    ' Suporting Types
    '****************************************************************************************

    ''' <summary>
    ''' Element flags define the way the elements print; OR them for multiple effects
    ''' </summary>
    <Flags()> _
    Public Enum ElementFlags
        ''' <summary>No special flags</summary>
        None = 0

        ''' <summary>Move to the next line after output</summary>
        NewLine = 1

        ''' <summary>if there isn't 2x room, then do new page first</summary>
        BottomCheck2 = 2

        ''' <summary>Center the item horizontally</summary>
        HorzCenter = 4

        ''' <summary>Right align the item (center overrides)</summary>
        HorzRight = 8
    End Enum

#End Region

#Region "Data"
    '****************************************************************************************
    ' Data
    '****************************************************************************************

    Private _fixedDocument As FixedDocument
    Private _curFixedPage As FixedPage
    Private _curCanvas As Canvas
    Private _marginX As Double
    Private _marginY As Double
    Private _infiniteSize As New Size(Double.PositiveInfinity, Double.PositiveInfinity)

#End Region

#Region "Properties"
    '****************************************************************************************
    ' Properties
    '****************************************************************************************

    ''' <summary>Current font family used for known objects</summary>
    Public Property CurrentFontFamily() As FontFamily
        Get
            Return m_CurrentFontFamily
        End Get
        Set(ByVal value As FontFamily)
            m_CurrentFontFamily = value
        End Set
    End Property
    Private m_CurrentFontFamily As FontFamily


    ''' <summary>Current font size used for known objects</summary>
    Public Property CurrentFontSize() As Double
        Get
            Return m_CurrentFontSize
        End Get
        Set(ByVal value As Double)
            m_CurrentFontSize = value
        End Set
    End Property
    Private m_CurrentFontSize As Double


    ''' <summary>Current font weight used for known objects</summary>
    Public Property CurrentFontWeight() As FontWeight
        Get
            Return m_CurrentFontWeight
        End Get
        Set(ByVal value As FontWeight)
            m_CurrentFontWeight = value
        End Set
    End Property
    Private m_CurrentFontWeight As FontWeight


    ''' <summary>Current font style used for known objects</summary>
    Public Property CurrentFontStyle() As FontStyle
        Get
            Return m_CurrentFontStyle
        End Get
        Set(ByVal value As FontStyle)
            m_CurrentFontStyle = value
        End Set
    End Property
    Private m_CurrentFontStyle As FontStyle


    ''' <summary>Current margin for known objects</summary>
    Public Property CurrentElementMargin() As Thickness
        Get
            Return m_CurrentElementMargin
        End Get
        Set(ByVal value As Thickness)
            m_CurrentElementMargin = value
        End Set
    End Property
    Private m_CurrentElementMargin As Thickness


    ''' <summary>Current background for known objects</summary>
    Public Property CurrentElementBackground() As Brush
        Get
            Return m_CurrentElementBackground
        End Get
        Set(ByVal value As Brush)
            m_CurrentElementBackground = value
        End Set
    End Property
    Private m_CurrentElementBackground As Brush


    ''' <summary>Current foreground for known objects</summary>
    Public Property CurrentElementForeground() As Brush
        Get
            Return m_CurrentElementForeground
        End Get
        Set(ByVal value As Brush)
            m_CurrentElementForeground = value
        End Set
    End Property
    Private m_CurrentElementForeground As Brush


    ''' <summary>Gets the current fixed document being worked on</summary>
    Public ReadOnly Property CurrentFixedDocument() As FixedDocument
        Get
            Return _fixedDocument
        End Get
    End Property


    ''' <summary>The current horizontal position</summary>
    Public Property CurX() As Double
        Get
            Return m_CurX
        End Get
        Set(ByVal value As Double)
            m_CurX = value
        End Set
    End Property
    Private m_CurX As Double


    ''' <summary>The current vertical position</summary>
    Public Property CurY() As Double
        Get
            Return m_CurY
        End Get
        Set(ByVal value As Double)
            m_CurY = value
        End Set
    End Property
    Private m_CurY As Double


    ''' <summary>The starting and ending X margins on the page</summary>
    Public Property MarginX() As Double
        Get
            Return _marginX
        End Get
        Set(ByVal value As Double)
            If value < 0 Then
                value = 0
            End If
            _marginX = value
            If CurX < _marginX Then
                CurX = _marginX
            End If
        End Set
    End Property


    ''' <summary>The starting and ending Y margins on the page</summary>
    Public Property MarginY() As Double
        Get
            Return _marginY
        End Get
        Set(ByVal value As Double)
            If value < 0 Then
                value = 0
            End If
            _marginY = value
            If CurY < _marginY Then
                CurY = _marginY
            End If
        End Set
    End Property


    ''' <summary>Gets the page size for the document minus the margins</summary>
    Public ReadOnly Property PageSizeUsed() As Size
        Get
            Dim sz As Size = CurrentFixedDocument.DocumentPaginator.PageSize
            sz.Width -= 2 * _marginX
            sz.Height -= 2 * _marginY
            Return sz
        End Get
    End Property

#End Region

#Region "Construction"
    '****************************************************************************************
    ' Construction
    '****************************************************************************************

    ''' <summary>
    ''' Constructor for printing
    ''' </summary>
    ''' <param name="printQueue"></param>
    ''' <param name="printTicket"></param>
    Public Sub New(ByVal printQueue As PrintQueue, ByVal printTicket As PrintTicket)
        Dim capabilities As PrintCapabilities = printQueue.GetPrintCapabilities(printTicket)

        Dim sz As New Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight)
        _fixedDocument = New FixedDocument()
        _fixedDocument.DocumentPaginator.PageSize = sz

        StartPage()
    End Sub


    ''' <summary>
    ''' Constructor for XPS creation
    ''' </summary>
    ''' <param name="sz"></param>
    Public Sub New(ByVal sz As Size)
        _fixedDocument = New FixedDocument()
        _fixedDocument.DocumentPaginator.PageSize = sz

        StartPage()
    End Sub

#End Region

#Region "Interfaces"
    '****************************************************************************************
    ' Interfaces
    '****************************************************************************************

    ''' <summary>
    ''' Add a new page to the document (start a new page)
    ''' </summary>
    Public Sub StartPage()
        ' Create a new page content and fixed page
        Dim content As New PageContent()
        _fixedDocument.Pages.Add(content)
        _curFixedPage = New FixedPage()
        DirectCast(content, IAddChild).AddChild(_curFixedPage)

        ' Create a new drawing canvas for the page
        _curCanvas = New Canvas()
        _curCanvas.Width = _fixedDocument.DocumentPaginator.PageSize.Width
        _curCanvas.Height = _fixedDocument.DocumentPaginator.PageSize.Height
        _curFixedPage.Children.Add(_curCanvas)

        ' Reset the current position
        CurX = MarginX
        CurY = MarginY
    End Sub


    ''' <summary>
    ''' Adds a new element at the current position, and updates the current position
    ''' </summary>
    ''' <param name="element">New element to add</param>
    ''' <param name="flags">Print options</param>
    Public Sub AddUIElement(ByVal element As UIElement, ByVal flags As ElementFlags)
        element.Measure(_infiniteSize)
        If CurX > _fixedDocument.DocumentPaginator.PageSize.Width - MarginX Then
            CurY += element.DesiredSize.Height
            CurX = MarginX
        End If
        Dim extraCheck As Double = 0
        If (flags And ElementFlags.BottomCheck2) = ElementFlags.BottomCheck2 Then
            extraCheck = element.DesiredSize.Height
        End If
        If CurY > _fixedDocument.DocumentPaginator.PageSize.Height - MarginY - extraCheck Then
            StartPage()
        End If

        '_curCanvas.Children.Add(element)
        _curCanvas = element
        element.SetValue(Canvas.LeftProperty, CurX)
        element.SetValue(Canvas.TopProperty, CurY)

        CurX += element.DesiredSize.Width
        If (flags And ElementFlags.NewLine) = ElementFlags.NewLine Then
            CurX = MarginX
            CurY += element.DesiredSize.Height
        End If
    End Sub


    ''' <summary>
    ''' Add a current style TextBlock element at the current position
    ''' </summary>
    ''' <param name="text">Text to add</param>
    ''' <param name="width">Width of element</param>
    ''' <param name="height">Height of element</param>
    ''' <param name="flags">Print options</param>
    Public Sub AddTextBlock(ByVal text As String, ByVal width As Double, ByVal height As Double, ByVal flags As ElementFlags)
        Dim tb As New TextBlock()
        tb.Text = text
        tb.FontFamily = CurrentFontFamily
        tb.FontSize = CurrentFontSize
        tb.FontWeight = CurrentFontWeight
        tb.FontStyle = CurrentFontStyle
        tb.VerticalAlignment = VerticalAlignment.Center
        If (flags And ElementFlags.HorzCenter) = ElementFlags.HorzCenter Then
            tb.HorizontalAlignment = HorizontalAlignment.Center
        ElseIf (flags And ElementFlags.HorzRight) = ElementFlags.HorzRight Then
            tb.HorizontalAlignment = HorizontalAlignment.Right
        End If
        tb.Margin = CurrentElementMargin
        If CurrentElementForeground IsNot Nothing Then
            tb.Foreground = CurrentElementForeground
        End If
        If CurrentElementBackground IsNot Nothing Then
            tb.Background = CurrentElementBackground
        End If

        Dim grid As New Grid()
        If CurrentElementBackground IsNot Nothing Then
            grid.Background = CurrentElementBackground
        End If
        If width <> 0 Then
            grid.Width = width
        End If
        If height <> 0 Then
            grid.Height = height
        End If
        grid.Children.Add(tb)

        AddUIElement(grid, flags)
    End Sub


    ''' <summary>
    ''' Adds a current style TextBox element at the current position
    ''' </summary>
    ''' <param name="text">Text to add</param>
    ''' <param name="width">Width of element</param>
    ''' <param name="height">Height of element</param>
    ''' <param name="flags">Print options</param>
    Public Sub AddTextBox(ByVal text As String, ByVal width As Double, ByVal height As Double, ByVal flags As ElementFlags)
        Dim tb As New TextBox()
        tb.Text = text
        tb.FontFamily = CurrentFontFamily
        tb.FontSize = CurrentFontSize
        tb.FontWeight = CurrentFontWeight
        tb.FontStyle = CurrentFontStyle
        tb.VerticalAlignment = VerticalAlignment.Center
        tb.VerticalContentAlignment = VerticalAlignment.Center
        If (flags And ElementFlags.HorzCenter) = ElementFlags.HorzCenter Then
            tb.HorizontalContentAlignment = HorizontalAlignment.Center
        ElseIf (flags And ElementFlags.HorzRight) = ElementFlags.HorzRight Then
            tb.HorizontalContentAlignment = HorizontalAlignment.Right
        End If
        tb.Margin = CurrentElementMargin
        If CurrentElementBackground IsNot Nothing Then
            tb.Background = CurrentElementBackground
        End If
        If CurrentElementForeground IsNot Nothing Then
            tb.Foreground = CurrentElementForeground
        End If

        Dim grid As New Grid()
        If CurrentElementBackground IsNot Nothing Then
            grid.Background = CurrentElementBackground
        End If
        If width <> 0 Then
            grid.Width = width
        End If
        If height <> 0 Then
            grid.Height = height
        End If
        grid.Children.Add(tb)

        AddUIElement(grid, flags)
    End Sub


    ''' <summary>
    ''' Add a current style CheckBox element at the current position
    ''' </summary>
    ''' <param name="value">Checkbox value to add</param>
    ''' <param name="width">Width of element</param>
    ''' <param name="height">Height of element</param>
    ''' <param name="flags">Print options</param>
    Public Sub AddCheckBox(ByVal value As Boolean, ByVal width As Double, ByVal height As Double, ByVal flags As ElementFlags)
        Dim cb As New CheckBox()
        cb.IsChecked = value
        cb.VerticalAlignment = VerticalAlignment.Center
        If (flags And ElementFlags.HorzCenter) = ElementFlags.HorzCenter Then
            cb.HorizontalAlignment = HorizontalAlignment.Center
        ElseIf (flags And ElementFlags.HorzRight) = ElementFlags.HorzRight Then
            cb.HorizontalAlignment = HorizontalAlignment.Right
        End If
        If CurrentElementForeground IsNot Nothing Then
            cb.Foreground = CurrentElementForeground
        End If
        If CurrentElementBackground IsNot Nothing Then
            cb.Background = CurrentElementBackground
        End If

        Dim grid As New Grid()
        If CurrentElementBackground IsNot Nothing Then
            grid.Background = CurrentElementBackground
        End If
        If width <> 0 Then
            grid.Width = width
        End If
        If height <> 0 Then
            grid.Height = height
        End If
        grid.Children.Add(cb)

        AddUIElement(grid, flags)
    End Sub

#End Region
End Class
