<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintOptions))
        Me.btnPrintPreview = New System.Windows.Forms.Button()
        Me.CheckBoxPrintRowColors = New System.Windows.Forms.CheckBox()
        Me.ButtonClearAll = New System.Windows.Forms.Button()
        Me.CheckBoxSelectColumns = New System.Windows.Forms.CheckBox()
        Me.CheckBoxCenterReportOnPage = New System.Windows.Forms.CheckBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.lblColumnsToPrint = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chklst = New System.Windows.Forms.CheckedListBox()
        Me.btnFont = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnPrintPreview
        '
        Me.btnPrintPreview.Location = New System.Drawing.Point(324, 293)
        Me.btnPrintPreview.Name = "btnPrintPreview"
        Me.btnPrintPreview.Size = New System.Drawing.Size(72, 23)
        Me.btnPrintPreview.TabIndex = 29
        Me.btnPrintPreview.Text = "Preview"
        Me.btnPrintPreview.UseVisualStyleBackColor = True
        '
        'CheckBoxPrintRowColors
        '
        Me.CheckBoxPrintRowColors.AutoSize = True
        Me.CheckBoxPrintRowColors.Location = New System.Drawing.Point(284, 223)
        Me.CheckBoxPrintRowColors.Name = "CheckBoxPrintRowColors"
        Me.CheckBoxPrintRowColors.Size = New System.Drawing.Size(90, 17)
        Me.CheckBoxPrintRowColors.TabIndex = 28
        Me.CheckBoxPrintRowColors.Text = "Couleur ligne "
        Me.CheckBoxPrintRowColors.UseVisualStyleBackColor = True
        '
        'ButtonClearAll
        '
        Me.ButtonClearAll.Location = New System.Drawing.Point(199, 5)
        Me.ButtonClearAll.Name = "ButtonClearAll"
        Me.ButtonClearAll.Size = New System.Drawing.Size(61, 21)
        Me.ButtonClearAll.TabIndex = 27
        Me.ButtonClearAll.Text = "Effacer"
        Me.ButtonClearAll.UseVisualStyleBackColor = True
        '
        'CheckBoxSelectColumns
        '
        Me.CheckBoxSelectColumns.AutoSize = True
        Me.CheckBoxSelectColumns.Location = New System.Drawing.Point(138, 8)
        Me.CheckBoxSelectColumns.Name = "CheckBoxSelectColumns"
        Me.CheckBoxSelectColumns.Size = New System.Drawing.Size(55, 17)
        Me.CheckBoxSelectColumns.TabIndex = 26
        Me.CheckBoxSelectColumns.Text = "toutes"
        Me.CheckBoxSelectColumns.UseVisualStyleBackColor = True
        '
        'CheckBoxCenterReportOnPage
        '
        Me.CheckBoxCenterReportOnPage.AutoSize = True
        Me.CheckBoxCenterReportOnPage.Location = New System.Drawing.Point(284, 206)
        Me.CheckBoxCenterReportOnPage.Name = "CheckBoxCenterReportOnPage"
        Me.CheckBoxCenterReportOnPage.Size = New System.Drawing.Size(95, 17)
        Me.CheckBoxCenterReportOnPage.TabIndex = 25
        Me.CheckBoxCenterReportOnPage.Text = "Centrer la grille"
        Me.CheckBoxCenterReportOnPage.UseVisualStyleBackColor = True
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(275, 98)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(120, 13)
        Me.lblTitle.TabIndex = 24
        Me.lblTitle.Text = "Titre de l'impression"
        '
        'txtTitle
        '
        Me.txtTitle.AcceptsReturn = True
        Me.txtTitle.Location = New System.Drawing.Point(275, 117)
        Me.txtTitle.Multiline = True
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(176, 75)
        Me.txtTitle.TabIndex = 23
        '
        'lblColumnsToPrint
        '
        Me.lblColumnsToPrint.AutoSize = True
        Me.lblColumnsToPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColumnsToPrint.Location = New System.Drawing.Point(10, 8)
        Me.lblColumnsToPrint.Name = "lblColumnsToPrint"
        Me.lblColumnsToPrint.Size = New System.Drawing.Size(124, 13)
        Me.lblColumnsToPrint.TabIndex = 21
        Me.lblColumnsToPrint.Text = "Colonnes à imprimer "
        '
        'btnOK
        '
        Me.btnOK.BackColor = System.Drawing.SystemColors.Control
        Me.btnOK.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnOK.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(178, Byte))
        Me.btnOK.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnOK.Image = CType(resources.GetObject("btnOK.Image"), System.Drawing.Image)
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnOK.Location = New System.Drawing.Point(402, 293)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnOK.Size = New System.Drawing.Size(56, 25)
        Me.btnOK.TabIndex = 19
        Me.btnOK.Text = "Print"
        Me.btnOK.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnCancel.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(178, Byte))
        Me.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnCancel.Image = CType(resources.GetObject("btnCancel.Image"), System.Drawing.Image)
        Me.btnCancel.Location = New System.Drawing.Point(262, 293)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnCancel.Size = New System.Drawing.Size(56, 23)
        Me.btnCancel.TabIndex = 20
        Me.btnCancel.Text = "Annuler"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'chklst
        '
        Me.chklst.CheckOnClick = True
        Me.chklst.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chklst.FormattingEnabled = True
        Me.chklst.Location = New System.Drawing.Point(10, 27)
        Me.chklst.Name = "chklst"
        Me.chklst.Size = New System.Drawing.Size(246, 289)
        Me.chklst.TabIndex = 18
        '
        'btnFont
        '
        Me.btnFont.BackgroundImage = CType(resources.GetObject("btnFont.BackgroundImage"), System.Drawing.Image)
        Me.btnFont.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.btnFont.Location = New System.Drawing.Point(278, 60)
        Me.btnFont.Name = "btnFont"
        Me.btnFont.Size = New System.Drawing.Size(27, 20)
        Me.btnFont.TabIndex = 84
        Me.btnFont.Text = " "
        Me.btnFont.UseVisualStyleBackColor = True
        '
        'PrintOptions
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(475, 327)
        Me.Controls.Add(Me.btnFont)
        Me.Controls.Add(Me.btnPrintPreview)
        Me.Controls.Add(Me.CheckBoxPrintRowColors)
        Me.Controls.Add(Me.ButtonClearAll)
        Me.Controls.Add(Me.CheckBoxSelectColumns)
        Me.Controls.Add(Me.CheckBoxCenterReportOnPage)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.txtTitle)
        Me.Controls.Add(Me.lblColumnsToPrint)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.chklst)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "PrintOptions"
        Me.Text = "Options d'impression"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents btnPrintPreview As System.Windows.Forms.Button
    Friend WithEvents CheckBoxPrintRowColors As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonClearAll As System.Windows.Forms.Button
    Friend WithEvents CheckBoxSelectColumns As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxCenterReportOnPage As System.Windows.Forms.CheckBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents txtTitle As System.Windows.Forms.TextBox
    Friend WithEvents lblColumnsToPrint As System.Windows.Forms.Label
    Protected WithEvents btnOK As System.Windows.Forms.Button
    Protected WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents chklst As System.Windows.Forms.CheckedListBox
    Friend WithEvents btnFont As System.Windows.Forms.Button
End Class
