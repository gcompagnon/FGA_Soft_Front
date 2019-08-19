<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModelPortfolio
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModelPortfolio))
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle16 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle17 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle18 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.TabPortfolio = New System.Windows.Forms.TabControl()
        Me.TSecteur = New System.Windows.Forms.TabPage()
        Me.TCommentaireSecteur = New vbMaf.Windows.Forms.RichEditBox()
        Me.DRecoFga = New System.Windows.Forms.DataGridView()
        Me.DNews = New System.Windows.Forms.DataGridView()
        Me.DSecteursFga = New System.Windows.Forms.DataGridView()
        Me.CbIndiceSecteur = New System.Windows.Forms.ComboBox()
        Me.DSecteursIcb = New System.Windows.Forms.DataGridView()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TMaxDate = New System.Windows.Forms.TextBox()
        Me.BRevue = New System.Windows.Forms.Button()
        Me.BCommentaireSecteur = New System.Windows.Forms.Button()
        Me.BExcelSectorielle = New System.Windows.Forms.Button()
        Me.TIsin = New System.Windows.Forms.TabPage()
        Me.BCommentaireIsin = New System.Windows.Forms.Button()
        Me.TCommentaireIsin = New vbMaf.Windows.Forms.RichEditBox()
        Me.DHistoIsin = New System.Windows.Forms.DataGridView()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.CbIsinSelection = New System.Windows.Forms.ComboBox()
        Me.TabPortfolio.SuspendLayout()
        Me.TSecteur.SuspendLayout()
        CType(Me.DRecoFga, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DNews, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DSecteursFga, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DSecteursIcb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TIsin.SuspendLayout()
        CType(Me.DHistoIsin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabPortfolio
        '
        Me.TabPortfolio.Controls.Add(Me.TSecteur)
        Me.TabPortfolio.Controls.Add(Me.TIsin)
        Me.TabPortfolio.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabPortfolio.Location = New System.Drawing.Point(0, 0)
        Me.TabPortfolio.Name = "TabPortfolio"
        Me.TabPortfolio.SelectedIndex = 0
        Me.TabPortfolio.Size = New System.Drawing.Size(1284, 856)
        Me.TabPortfolio.TabIndex = 0
        '
        'TSecteur
        '
        Me.TSecteur.BackColor = System.Drawing.SystemColors.Control
        Me.TSecteur.Controls.Add(Me.TCommentaireSecteur)
        Me.TSecteur.Controls.Add(Me.DRecoFga)
        Me.TSecteur.Controls.Add(Me.DNews)
        Me.TSecteur.Controls.Add(Me.DSecteursFga)
        Me.TSecteur.Controls.Add(Me.CbIndiceSecteur)
        Me.TSecteur.Controls.Add(Me.DSecteursIcb)
        Me.TSecteur.Controls.Add(Me.Label10)
        Me.TSecteur.Controls.Add(Me.TMaxDate)
        Me.TSecteur.Controls.Add(Me.BRevue)
        Me.TSecteur.Controls.Add(Me.BCommentaireSecteur)
        Me.TSecteur.Controls.Add(Me.BExcelSectorielle)
        Me.TSecteur.Location = New System.Drawing.Point(4, 22)
        Me.TSecteur.Name = "TSecteur"
        Me.TSecteur.Padding = New System.Windows.Forms.Padding(3)
        Me.TSecteur.Size = New System.Drawing.Size(1276, 830)
        Me.TSecteur.TabIndex = 0
        Me.TSecteur.Text = "Secteur"
        '
        'TCommentaireSecteur
        '
        Me.TCommentaireSecteur.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TCommentaireSecteur.Location = New System.Drawing.Point(568, 406)
        Me.TCommentaireSecteur.Name = "TCommentaireSecteur"
        Me.TCommentaireSecteur.Rtf = "{\rtf1\ansi\ansicpg1252\deff0\deflang1036{\fonttbl{\f0\fnil\fcharset0 Microsoft S" & _
            "ans Serif;}}" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "\viewkind4\uc1\pard\f0\fs20\par" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "}" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.TCommentaireSecteur.ShowSelectionMargin = True
        Me.TCommentaireSecteur.ShowToolBar = False
        Me.TCommentaireSecteur.Size = New System.Drawing.Size(640, 416)
        Me.TCommentaireSecteur.TabIndex = 169
        '
        'DRecoFga
        '
        Me.DRecoFga.AllowUserToAddRows = False
        Me.DRecoFga.AllowUserToDeleteRows = False
        Me.DRecoFga.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DRecoFga.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DRecoFga.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DRecoFga.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DRecoFga.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DRecoFga.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DRecoFga.DefaultCellStyle = DataGridViewCellStyle2
        Me.DRecoFga.Location = New System.Drawing.Point(8, 406)
        Me.DRecoFga.Name = "DRecoFga"
        Me.DRecoFga.ReadOnly = True
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DRecoFga.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DRecoFga.RowHeadersVisible = False
        Me.DRecoFga.Size = New System.Drawing.Size(554, 416)
        Me.DRecoFga.TabIndex = 168
        '
        'DNews
        '
        Me.DNews.AllowUserToAddRows = False
        Me.DNews.AllowUserToDeleteRows = False
        Me.DNews.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DNews.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DNews.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DNews.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DNews.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.DNews.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DNews.DefaultCellStyle = DataGridViewCellStyle5
        Me.DNews.Location = New System.Drawing.Point(986, 3)
        Me.DNews.Name = "DNews"
        Me.DNews.ReadOnly = True
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DNews.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.DNews.RowHeadersVisible = False
        Me.DNews.Size = New System.Drawing.Size(87, 35)
        Me.DNews.TabIndex = 166
        '
        'DSecteursFga
        '
        Me.DSecteursFga.AllowUserToAddRows = False
        Me.DSecteursFga.AllowUserToDeleteRows = False
        Me.DSecteursFga.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DSecteursFga.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DSecteursFga.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursFga.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.DSecteursFga.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursFga.DefaultCellStyle = DataGridViewCellStyle8
        Me.DSecteursFga.Location = New System.Drawing.Point(566, 39)
        Me.DSecteursFga.Name = "DSecteursFga"
        Me.DSecteursFga.ReadOnly = True
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursFga.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.DSecteursFga.RowHeadersVisible = False
        Me.DSecteursFga.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DSecteursFga.Size = New System.Drawing.Size(779, 361)
        Me.DSecteursFga.TabIndex = 164
        '
        'CbIndiceSecteur
        '
        Me.CbIndiceSecteur.FormattingEnabled = True
        Me.CbIndiceSecteur.Location = New System.Drawing.Point(412, 12)
        Me.CbIndiceSecteur.Name = "CbIndiceSecteur"
        Me.CbIndiceSecteur.Size = New System.Drawing.Size(121, 21)
        Me.CbIndiceSecteur.TabIndex = 163
        '
        'DSecteursIcb
        '
        Me.DSecteursIcb.AllowUserToAddRows = False
        Me.DSecteursIcb.AllowUserToDeleteRows = False
        Me.DSecteursIcb.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DSecteursIcb.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DSecteursIcb.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursIcb.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle10
        Me.DSecteursIcb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursIcb.DefaultCellStyle = DataGridViewCellStyle11
        Me.DSecteursIcb.Location = New System.Drawing.Point(6, 41)
        Me.DSecteursIcb.Name = "DSecteursIcb"
        Me.DSecteursIcb.ReadOnly = True
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DSecteursIcb.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.DSecteursIcb.RowHeadersVisible = False
        Me.DSecteursIcb.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DSecteursIcb.Size = New System.Drawing.Size(554, 359)
        Me.DSecteursIcb.TabIndex = 162
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(18, 17)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(86, 13)
        Me.Label10.TabIndex = 160
        Me.Label10.Text = "Last modification"
        '
        'TMaxDate
        '
        Me.TMaxDate.Location = New System.Drawing.Point(128, 14)
        Me.TMaxDate.Name = "TMaxDate"
        Me.TMaxDate.ReadOnly = True
        Me.TMaxDate.Size = New System.Drawing.Size(139, 20)
        Me.TMaxDate.TabIndex = 159
        '
        'BRevue
        '
        Me.BRevue.Location = New System.Drawing.Point(312, 12)
        Me.BRevue.Name = "BRevue"
        Me.BRevue.Size = New System.Drawing.Size(75, 23)
        Me.BRevue.TabIndex = 158
        Me.BRevue.Text = "Revue"
        Me.BRevue.UseVisualStyleBackColor = True
        '
        'BCommentaireSecteur
        '
        Me.BCommentaireSecteur.BackgroundImage = CType(resources.GetObject("BCommentaireSecteur.BackgroundImage"), System.Drawing.Image)
        Me.BCommentaireSecteur.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BCommentaireSecteur.Location = New System.Drawing.Point(599, 9)
        Me.BCommentaireSecteur.Name = "BCommentaireSecteur"
        Me.BCommentaireSecteur.Size = New System.Drawing.Size(27, 28)
        Me.BCommentaireSecteur.TabIndex = 165
        Me.BCommentaireSecteur.UseVisualStyleBackColor = True
        '
        'BExcelSectorielle
        '
        Me.BExcelSectorielle.BackgroundImage = CType(resources.GetObject("BExcelSectorielle.BackgroundImage"), System.Drawing.Image)
        Me.BExcelSectorielle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BExcelSectorielle.Location = New System.Drawing.Point(566, 10)
        Me.BExcelSectorielle.Name = "BExcelSectorielle"
        Me.BExcelSectorielle.Size = New System.Drawing.Size(27, 27)
        Me.BExcelSectorielle.TabIndex = 161
        Me.BExcelSectorielle.UseVisualStyleBackColor = True
        '
        'TIsin
        '
        Me.TIsin.BackColor = System.Drawing.SystemColors.Control
        Me.TIsin.Controls.Add(Me.BCommentaireIsin)
        Me.TIsin.Controls.Add(Me.TCommentaireIsin)
        Me.TIsin.Controls.Add(Me.DHistoIsin)
        Me.TIsin.Controls.Add(Me.DataGridView1)
        Me.TIsin.Controls.Add(Me.CbIsinSelection)
        Me.TIsin.Location = New System.Drawing.Point(4, 22)
        Me.TIsin.Name = "TIsin"
        Me.TIsin.Padding = New System.Windows.Forms.Padding(3)
        Me.TIsin.Size = New System.Drawing.Size(1351, 830)
        Me.TIsin.TabIndex = 1
        Me.TIsin.Text = "Isin"
        '
        'BCommentaireIsin
        '
        Me.BCommentaireIsin.BackgroundImage = CType(resources.GetObject("BCommentaireIsin.BackgroundImage"), System.Drawing.Image)
        Me.BCommentaireIsin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BCommentaireIsin.Location = New System.Drawing.Point(391, 8)
        Me.BCommentaireIsin.Name = "BCommentaireIsin"
        Me.BCommentaireIsin.Size = New System.Drawing.Size(27, 28)
        Me.BCommentaireIsin.TabIndex = 167
        Me.BCommentaireIsin.UseVisualStyleBackColor = True
        '
        'TCommentaireIsin
        '
        Me.TCommentaireIsin.Location = New System.Drawing.Point(564, 558)
        Me.TCommentaireIsin.Name = "TCommentaireIsin"
        Me.TCommentaireIsin.Rtf = "{\rtf1\ansi\ansicpg1252\deff0\deflang1036{\fonttbl{\f0\fnil\fcharset0 Microsoft S" & _
            "ans Serif;}}" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "\viewkind4\uc1\pard\f0\fs20\par" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "}" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.TCommentaireIsin.ShowSelectionMargin = True
        Me.TCommentaireIsin.ShowToolBar = False
        Me.TCommentaireIsin.Size = New System.Drawing.Size(640, 264)
        Me.TCommentaireIsin.TabIndex = 166
        '
        'DHistoIsin
        '
        Me.DHistoIsin.AllowUserToAddRows = False
        Me.DHistoIsin.AllowUserToDeleteRows = False
        Me.DHistoIsin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DHistoIsin.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DHistoIsin.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DHistoIsin.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle13
        Me.DHistoIsin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DHistoIsin.DefaultCellStyle = DataGridViewCellStyle14
        Me.DHistoIsin.Location = New System.Drawing.Point(3, 558)
        Me.DHistoIsin.Name = "DHistoIsin"
        Me.DHistoIsin.ReadOnly = True
        DataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DHistoIsin.RowHeadersDefaultCellStyle = DataGridViewCellStyle15
        Me.DHistoIsin.RowHeadersVisible = False
        Me.DHistoIsin.Size = New System.Drawing.Size(554, 264)
        Me.DHistoIsin.TabIndex = 165
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle16
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.DefaultCellStyle = DataGridViewCellStyle17
        Me.DataGridView1.Location = New System.Drawing.Point(4, 36)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle18
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1200, 517)
        Me.DataGridView1.TabIndex = 164
        '
        'CbIsinSelection
        '
        Me.CbIsinSelection.FormattingEnabled = True
        Me.CbIsinSelection.Location = New System.Drawing.Point(14, 9)
        Me.CbIsinSelection.Name = "CbIsinSelection"
        Me.CbIsinSelection.Size = New System.Drawing.Size(255, 21)
        Me.CbIsinSelection.TabIndex = 163
        '
        'ModelPortfolio
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1284, 856)
        Me.Controls.Add(Me.TabPortfolio)
        Me.Name = "ModelPortfolio"
        Me.Text = "Portefeuille Model"
        Me.TabPortfolio.ResumeLayout(False)
        Me.TSecteur.ResumeLayout(False)
        Me.TSecteur.PerformLayout()
        CType(Me.DRecoFga, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DNews, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DSecteursFga, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DSecteursIcb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TIsin.ResumeLayout(False)
        CType(Me.DHistoIsin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabPortfolio As System.Windows.Forms.TabControl
    Friend WithEvents TSecteur As System.Windows.Forms.TabPage
    Friend WithEvents TIsin As System.Windows.Forms.TabPage
    Friend WithEvents DSecteursFga As System.Windows.Forms.DataGridView
    Friend WithEvents CbIndiceSecteur As System.Windows.Forms.ComboBox
    Friend WithEvents DSecteursIcb As System.Windows.Forms.DataGridView
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TMaxDate As System.Windows.Forms.TextBox
    Friend WithEvents BRevue As System.Windows.Forms.Button
    Friend WithEvents BCommentaireSecteur As System.Windows.Forms.Button
    Friend WithEvents BExcelSectorielle As System.Windows.Forms.Button
    Private WithEvents TCommentaireSecteur As vbMaf.Windows.Forms.RichEditBox
    Friend WithEvents DRecoFga As System.Windows.Forms.DataGridView
    Friend WithEvents DNews As System.Windows.Forms.DataGridView
    Friend WithEvents BCommentaireIsin As System.Windows.Forms.Button
    Private WithEvents TCommentaireIsin As vbMaf.Windows.Forms.RichEditBox
    Friend WithEvents DHistoIsin As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents CbIsinSelection As System.Windows.Forms.ComboBox
End Class
