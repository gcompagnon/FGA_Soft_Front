Public Class PrintOptions

    Private m_PrintTitle As String
    Private m_PrintAllRows As Boolean = True
    Private m_Font As Font
    Private m_FontColor As Color
    Private m_CenterReportOnPage As Boolean
    Private m_PrintRowColors As Boolean = False
    Private IHMPreview As PrintPreviewForm


    Public Sub New(ByVal PrintTitle As String, ByVal availableFields As List(Of String), ByVal PrintAllRows As Boolean, ByVal IHMPreview As Form)
        InitializeComponent()
        For Each field As String In availableFields
            chklst.Items.Add(field, CheckState.Unchecked)
        Next
        m_PrintTitle = PrintTitle
        m_PrintAllRows = PrintAllRows
        'If PrintAllRows Then rdoSelectedRows.Enabled = False
        txtTitle.Text = PrintTitle
        Me.IHMPreview = IHMPreview
    End Sub

    Private Sub PrintOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' set default rows to print
        'rdoAllRows.Checked = m_PrintAllRows
        'rdoSelectedRows.Checked = Not rdoAllRows.Checked
        CheckBoxSelectColumns.CheckState = CheckState.Checked
        CheckAllItems()
        txtTitle.Text = "Mon impression"
    End Sub


    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        m_PrintTitle = txtTitle.Text
        'm_PrintAllRows = rdoAllRows.Checked
        m_CenterReportOnPage = Me.CheckBoxCenterReportOnPage.Checked
        m_PrintRowColors = Me.CheckBoxPrintRowColors.Checked
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub




    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Public Function GetSelectedColumns() As List(Of String)
        Dim lst As New List(Of String)
        For Each item As Object In chklst.CheckedItems
            lst.Add(item.ToString)
        Next
        Return lst
    End Function

    Public ReadOnly Property PrintTitle() As String
        Get
            Return m_PrintTitle
        End Get
    End Property

    Public ReadOnly Property PrintAllRows() As Boolean
        Get
            Return m_PrintAllRows
        End Get
    End Property

    Public ReadOnly Property PrintFont() As Font
        Get
            If m_Font Is Nothing Then
                m_Font = New Font("Courier", 7, FontStyle.Regular, GraphicsUnit.Point)
            End If

            Return m_Font
        End Get
    End Property

    Public ReadOnly Property PrintFontColor() As Color
        Get
            If m_FontColor = Nothing Then
                m_FontColor = Color.Black
            End If
            Return m_FontColor
        End Get
    End Property

    Public ReadOnly Property PrintCenterReportOnPage() As Boolean
        Get
            Return m_CenterReportOnPage
        End Get
    End Property

    Public ReadOnly Property PrintRowColors() As Boolean
        Get
            Return m_PrintRowColors
        End Get
    End Property

    Private Sub CheckBoxSelectColumns_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxSelectColumns.CheckedChanged
        If CheckBoxSelectColumns.CheckState = CheckState.Checked Then
            Call CheckAllItems()
        Else
            chklst.Enabled = True
            ButtonClearAll.Enabled = True
        End If
    End Sub

    Private Sub CheckAllItems()
        For item As Integer = 0 To chklst.Items.Count - 1
            chklst.SetItemCheckState(item, CheckState.Checked)
        Next
        chklst.Enabled = False
        ButtonClearAll.Enabled = False
        btnOK.Enabled = True
    End Sub

    Private Sub UnCheckAllItems()
        For item As Integer = 0 To chklst.Items.Count - 1
            chklst.SetItemCheckState(item, CheckState.Unchecked)
            ButtonClearAll.Enabled = True
            btnOK.Enabled = False
        Next
    End Sub

    Private Function IsItemChecked() As Boolean
        Dim ischecked As Boolean = False
        For item As Integer = 0 To chklst.Items.Count - 1
            ischecked = ischecked Or chklst.GetItemCheckState(item) = CheckState.Checked
        Next
    End Function

    Private Sub chklst_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chklst.SelectedValueChanged
        If chklst.CheckedItems.Count > 0 Then
            ButtonClearAll.Enabled = True
            btnOK.Enabled = True
        Else
            btnOK.Enabled = False
        End If

    End Sub

    Private Sub ButtonClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonClearAll.Click
        UnCheckAllItems()
    End Sub

    Private Sub btnPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrintPreview.Click

        IHMPreview.Preview(Me.m_CenterReportOnPage, Me.m_Font, Me.m_FontColor)
    End Sub


    Private Sub btnFont_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFont.Click
        Dim fnt As New FontDialog
        fnt.ShowColor = True
        fnt.Font = m_Font
        fnt.Color = m_FontColor
        If fnt.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        m_Font = fnt.Font
        m_FontColor = fnt.Color
    End Sub
End Class

Public Interface PrintPreviewForm
    Sub Preview(ByVal PrintCenterReportOnPage As Boolean, ByVal PrintFont As Font, ByVal PrintFontColor As Color)
End Interface
