Namespace Referentiel
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Reconciliation
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Reconciliation))
            Me.BCharger = New System.Windows.Forms.Button()
            Me.DataGridQty = New System.Windows.Forms.DataGridView()
            Me.DataGridCc = New System.Windows.Forms.DataGridView()
            Me.DataGridCours = New System.Windows.Forms.DataGridView()
            Me.BExcel = New System.Windows.Forms.Button()
            Me.CbDataGrid = New System.Windows.Forms.ComboBox()
            Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
            Me.TabControl = New System.Windows.Forms.TabControl()
            Me.TabPage1 = New System.Windows.Forms.TabPage()
            Me.TabPage2 = New System.Windows.Forms.TabPage()
            Me.TabPage3 = New System.Windows.Forms.TabPage()
            Me.Omega = New System.Windows.Forms.TabPage()
            Me.DataGridOmega = New System.Windows.Forms.DataGridView()
            Me.TabPage5 = New System.Windows.Forms.TabPage()
            Me.DataGridChorus = New System.Windows.Forms.DataGridView()
            Me.BPrint = New System.Windows.Forms.Button()
            Me.myPrintDocument = New System.Drawing.Printing.PrintDocument()
            Me.CbRapport = New System.Windows.Forms.CheckBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Gp = New System.Windows.Forms.GroupBox()
            Me.TCc = New System.Windows.Forms.TextBox()
            Me.TCours = New System.Windows.Forms.TextBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.TQty = New System.Windows.Forms.TextBox()
            Me.BRefreshCorrespondance = New System.Windows.Forms.Button()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.MonthCalendar = New System.Windows.Forms.MonthCalendar()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.Panel2 = New System.Windows.Forms.Panel()
            CType(Me.DataGridQty, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridCc, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridCours, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabControl.SuspendLayout()
            Me.TabPage1.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            Me.TabPage3.SuspendLayout()
            Me.Omega.SuspendLayout()
            CType(Me.DataGridOmega, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabPage5.SuspendLayout()
            CType(Me.DataGridChorus, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Gp.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.SuspendLayout()
            '
            'BCharger
            '
            Me.BCharger.BackgroundImage = CType(resources.GetObject("BCharger.BackgroundImage"), System.Drawing.Image)
            Me.BCharger.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BCharger.Location = New System.Drawing.Point(13, 22)
            Me.BCharger.Name = "BCharger"
            Me.BCharger.Size = New System.Drawing.Size(102, 27)
            Me.BCharger.TabIndex = 0
            Me.BCharger.Text = "Charger"
            Me.BCharger.UseVisualStyleBackColor = True
            '
            'DataGridQty
            '
            Me.DataGridQty.AllowUserToAddRows = False
            Me.DataGridQty.AllowUserToDeleteRows = False
            Me.DataGridQty.AllowUserToOrderColumns = True
            Me.DataGridQty.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridQty.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridQty.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridQty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridQty.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DataGridQty.Location = New System.Drawing.Point(3, 3)
            Me.DataGridQty.Name = "DataGridQty"
            Me.DataGridQty.ReadOnly = True
            Me.DataGridQty.Size = New System.Drawing.Size(1115, 753)
            Me.DataGridQty.TabIndex = 11
            '
            'DataGridCc
            '
            Me.DataGridCc.AllowUserToAddRows = False
            Me.DataGridCc.AllowUserToDeleteRows = False
            Me.DataGridCc.AllowUserToOrderColumns = True
            Me.DataGridCc.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridCc.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridCc.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridCc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridCc.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DataGridCc.Location = New System.Drawing.Point(3, 3)
            Me.DataGridCc.Name = "DataGridCc"
            Me.DataGridCc.ReadOnly = True
            Me.DataGridCc.Size = New System.Drawing.Size(1115, 753)
            Me.DataGridCc.TabIndex = 12
            '
            'DataGridCours
            '
            Me.DataGridCours.AllowUserToAddRows = False
            Me.DataGridCours.AllowUserToDeleteRows = False
            Me.DataGridCours.AllowUserToOrderColumns = True
            Me.DataGridCours.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridCours.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridCours.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridCours.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridCours.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DataGridCours.Location = New System.Drawing.Point(3, 3)
            Me.DataGridCours.Name = "DataGridCours"
            Me.DataGridCours.ReadOnly = True
            Me.DataGridCours.Size = New System.Drawing.Size(1115, 753)
            Me.DataGridCours.TabIndex = 13
            '
            'BExcel
            '
            Me.BExcel.BackgroundImage = CType(resources.GetObject("BExcel.BackgroundImage"), System.Drawing.Image)
            Me.BExcel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BExcel.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.BExcel.Location = New System.Drawing.Point(53, 58)
            Me.BExcel.Name = "BExcel"
            Me.BExcel.Size = New System.Drawing.Size(27, 25)
            Me.BExcel.TabIndex = 14
            Me.BExcel.Text = " "
            Me.BExcel.UseVisualStyleBackColor = True
            '
            'CbDataGrid
            '
            Me.CbDataGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbDataGrid.FormattingEnabled = True
            Me.CbDataGrid.Location = New System.Drawing.Point(20, 32)
            Me.CbDataGrid.Name = "CbDataGrid"
            Me.CbDataGrid.Size = New System.Drawing.Size(155, 21)
            Me.CbDataGrid.TabIndex = 15
            '
            'SaveFileDialog
            '
            Me.SaveFileDialog.DefaultExt = "csv"
            '
            'TabControl
            '
            Me.TabControl.Controls.Add(Me.TabPage1)
            Me.TabControl.Controls.Add(Me.TabPage2)
            Me.TabControl.Controls.Add(Me.TabPage3)
            Me.TabControl.Controls.Add(Me.Omega)
            Me.TabControl.Controls.Add(Me.TabPage5)
            Me.TabControl.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.TabControl.Location = New System.Drawing.Point(0, 175)
            Me.TabControl.Name = "TabControl"
            Me.TabControl.SelectedIndex = 0
            Me.TabControl.Size = New System.Drawing.Size(1129, 785)
            Me.TabControl.TabIndex = 19
            '
            'TabPage1
            '
            Me.TabPage1.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage1.Controls.Add(Me.DataGridQty)
            Me.TabPage1.Location = New System.Drawing.Point(4, 22)
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage1.Size = New System.Drawing.Size(1121, 759)
            Me.TabPage1.TabIndex = 0
            Me.TabPage1.Text = "Quantité"
            '
            'TabPage2
            '
            Me.TabPage2.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage2.Controls.Add(Me.DataGridCours)
            Me.TabPage2.Location = New System.Drawing.Point(4, 22)
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage2.Size = New System.Drawing.Size(1121, 759)
            Me.TabPage2.TabIndex = 1
            Me.TabPage2.Text = "Cours"
            '
            'TabPage3
            '
            Me.TabPage3.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage3.Controls.Add(Me.DataGridCc)
            Me.TabPage3.Location = New System.Drawing.Point(4, 22)
            Me.TabPage3.Name = "TabPage3"
            Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage3.Size = New System.Drawing.Size(1121, 759)
            Me.TabPage3.TabIndex = 2
            Me.TabPage3.Text = "Coupon couru"
            '
            'Omega
            '
            Me.Omega.BackColor = System.Drawing.SystemColors.Control
            Me.Omega.Controls.Add(Me.DataGridOmega)
            Me.Omega.Location = New System.Drawing.Point(4, 22)
            Me.Omega.Name = "Omega"
            Me.Omega.Padding = New System.Windows.Forms.Padding(3)
            Me.Omega.Size = New System.Drawing.Size(1121, 759)
            Me.Omega.TabIndex = 3
            Me.Omega.Text = "Omega"
            '
            'DataGridOmega
            '
            Me.DataGridOmega.AllowUserToAddRows = False
            Me.DataGridOmega.AllowUserToDeleteRows = False
            Me.DataGridOmega.AllowUserToOrderColumns = True
            Me.DataGridOmega.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridOmega.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridOmega.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridOmega.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridOmega.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DataGridOmega.Location = New System.Drawing.Point(3, 3)
            Me.DataGridOmega.Name = "DataGridOmega"
            Me.DataGridOmega.ReadOnly = True
            Me.DataGridOmega.Size = New System.Drawing.Size(1115, 753)
            Me.DataGridOmega.TabIndex = 13
            '
            'TabPage5
            '
            Me.TabPage5.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage5.Controls.Add(Me.DataGridChorus)
            Me.TabPage5.Location = New System.Drawing.Point(4, 22)
            Me.TabPage5.Name = "TabPage5"
            Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage5.Size = New System.Drawing.Size(1121, 759)
            Me.TabPage5.TabIndex = 4
            Me.TabPage5.Text = "Chorus"
            '
            'DataGridChorus
            '
            Me.DataGridChorus.AllowUserToAddRows = False
            Me.DataGridChorus.AllowUserToDeleteRows = False
            Me.DataGridChorus.AllowUserToOrderColumns = True
            Me.DataGridChorus.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridChorus.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridChorus.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridChorus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridChorus.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DataGridChorus.Location = New System.Drawing.Point(3, 3)
            Me.DataGridChorus.Name = "DataGridChorus"
            Me.DataGridChorus.ReadOnly = True
            Me.DataGridChorus.Size = New System.Drawing.Size(1115, 753)
            Me.DataGridChorus.TabIndex = 13
            '
            'BPrint
            '
            Me.BPrint.BackgroundImage = CType(resources.GetObject("BPrint.BackgroundImage"), System.Drawing.Image)
            Me.BPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BPrint.Location = New System.Drawing.Point(20, 58)
            Me.BPrint.Name = "BPrint"
            Me.BPrint.Size = New System.Drawing.Size(27, 25)
            Me.BPrint.TabIndex = 46
            Me.BPrint.Text = " Impression"
            Me.BPrint.UseVisualStyleBackColor = True
            '
            'myPrintDocument
            '
            '
            'CbRapport
            '
            Me.CbRapport.AutoSize = True
            Me.CbRapport.Location = New System.Drawing.Point(16, 60)
            Me.CbRapport.Name = "CbRapport"
            Me.CbRapport.Size = New System.Drawing.Size(64, 17)
            Me.CbRapport.TabIndex = 12
            Me.CbRapport.Text = "Rapport"
            Me.CbRapport.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(12, 33)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(58, 13)
            Me.Label1.TabIndex = 12
            Me.Label1.Text = "Epsilon qty"
            '
            'Gp
            '
            Me.Gp.Controls.Add(Me.TCc)
            Me.Gp.Controls.Add(Me.TCours)
            Me.Gp.Controls.Add(Me.Label3)
            Me.Gp.Controls.Add(Me.Label2)
            Me.Gp.Controls.Add(Me.TQty)
            Me.Gp.Controls.Add(Me.Label1)
            Me.Gp.Location = New System.Drawing.Point(572, 11)
            Me.Gp.Name = "Gp"
            Me.Gp.Size = New System.Drawing.Size(206, 144)
            Me.Gp.TabIndex = 47
            Me.Gp.TabStop = False
            Me.Gp.Text = "Sensibilité"
            '
            'TCc
            '
            Me.TCc.Location = New System.Drawing.Point(88, 85)
            Me.TCc.Name = "TCc"
            Me.TCc.Size = New System.Drawing.Size(93, 20)
            Me.TCc.TabIndex = 17
            Me.TCc.Text = "0.001"
            '
            'TCours
            '
            Me.TCours.Location = New System.Drawing.Point(88, 57)
            Me.TCours.Name = "TCours"
            Me.TCours.Size = New System.Drawing.Size(93, 20)
            Me.TCours.TabIndex = 16
            Me.TCours.Text = "0.001"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(13, 88)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(56, 13)
            Me.Label3.TabIndex = 15
            Me.Label3.Text = "Epsilon cc"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(12, 60)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(70, 13)
            Me.Label2.TabIndex = 14
            Me.Label2.Text = "Epsilon cours"
            '
            'TQty
            '
            Me.TQty.Location = New System.Drawing.Point(88, 29)
            Me.TQty.Name = "TQty"
            Me.TQty.Size = New System.Drawing.Size(93, 20)
            Me.TQty.TabIndex = 13
            Me.TQty.Text = "0.0001"
            '
            'BRefreshCorrespondance
            '
            Me.BRefreshCorrespondance.BackgroundImage = CType(resources.GetObject("BRefreshCorrespondance.BackgroundImage"), System.Drawing.Image)
            Me.BRefreshCorrespondance.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRefreshCorrespondance.Location = New System.Drawing.Point(160, 95)
            Me.BRefreshCorrespondance.Name = "BRefreshCorrespondance"
            Me.BRefreshCorrespondance.Size = New System.Drawing.Size(26, 27)
            Me.BRefreshCorrespondance.TabIndex = 48
            Me.BRefreshCorrespondance.UseVisualStyleBackColor = True
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(11, 109)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(149, 13)
            Me.Label4.TabIndex = 50
            Me.Label4.Text = "Mise à jours Correspondances"
            '
            'MonthCalendar
            '
            Me.MonthCalendar.Location = New System.Drawing.Point(116, 10)
            Me.MonthCalendar.MaxSelectionCount = 1
            Me.MonthCalendar.Name = "MonthCalendar"
            Me.MonthCalendar.TabIndex = 51
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.CbDataGrid)
            Me.GroupBox1.Controls.Add(Me.BPrint)
            Me.GroupBox1.Controls.Add(Me.BExcel)
            Me.GroupBox1.Location = New System.Drawing.Point(816, 11)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(199, 144)
            Me.GroupBox1.TabIndex = 52
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Sauvegarde"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Panel2)
            Me.GroupBox2.Controls.Add(Me.BCharger)
            Me.GroupBox2.Controls.Add(Me.CbRapport)
            Me.GroupBox2.Controls.Add(Me.BRefreshCorrespondance)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Location = New System.Drawing.Point(337, 11)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(195, 144)
            Me.GroupBox2.TabIndex = 53
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Action"
            '
            'Panel2
            '
            Me.Panel2.BackgroundImage = CType(resources.GetObject("Panel2.BackgroundImage"), System.Drawing.Image)
            Me.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel2.Location = New System.Drawing.Point(76, 55)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(29, 24)
            Me.Panel2.TabIndex = 51
            '
            'Reconciliation
            '
            Me.AcceptButton = Me.BCharger
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1129, 960)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.MonthCalendar)
            Me.Controls.Add(Me.Gp)
            Me.Controls.Add(Me.TabControl)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "Reconciliation"
            Me.Text = "Reconciliation"
            CType(Me.DataGridQty, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridCc, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridCours, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabControl.ResumeLayout(False)
            Me.TabPage1.ResumeLayout(False)
            Me.TabPage2.ResumeLayout(False)
            Me.TabPage3.ResumeLayout(False)
            Me.Omega.ResumeLayout(False)
            CType(Me.DataGridOmega, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabPage5.ResumeLayout(False)
            CType(Me.DataGridChorus, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Gp.ResumeLayout(False)
            Me.Gp.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents BCharger As System.Windows.Forms.Button
        Friend WithEvents DataGridQty As System.Windows.Forms.DataGridView
        Friend WithEvents DataGridCc As System.Windows.Forms.DataGridView
        Friend WithEvents DataGridCours As System.Windows.Forms.DataGridView
        Friend WithEvents BExcel As System.Windows.Forms.Button
        Friend WithEvents CbDataGrid As System.Windows.Forms.ComboBox
        Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
        Friend WithEvents TabControl As System.Windows.Forms.TabControl
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
        Friend WithEvents BPrint As System.Windows.Forms.Button
        Friend WithEvents myPrintDocument As System.Drawing.Printing.PrintDocument
        Friend WithEvents CbRapport As System.Windows.Forms.CheckBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Gp As System.Windows.Forms.GroupBox
        Friend WithEvents TQty As System.Windows.Forms.TextBox
        Friend WithEvents BRefreshCorrespondance As System.Windows.Forms.Button
        Friend WithEvents TCc As System.Windows.Forms.TextBox
        Friend WithEvents TCours As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Omega As System.Windows.Forms.TabPage
        Friend WithEvents DataGridOmega As System.Windows.Forms.DataGridView
        Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
        Friend WithEvents DataGridChorus As System.Windows.Forms.DataGridView
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents MonthCalendar As System.Windows.Forms.MonthCalendar
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
    End Class
End Namespace
