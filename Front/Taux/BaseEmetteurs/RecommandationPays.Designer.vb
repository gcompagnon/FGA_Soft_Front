Namespace Taux.BaseEmetteurs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RecommandationPays
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RecommandationPays))
            Me.DataGridPays = New System.Windows.Forms.DataGridView()
            Me.CbPays = New System.Windows.Forms.ComboBox()
            Me.CbRecommandation = New System.Windows.Forms.ComboBox()
            Me.TLogin = New System.Windows.Forms.TextBox()
            Me.TCommentaire = New System.Windows.Forms.TextBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.BRecommandation = New System.Windows.Forms.Button()
            Me.DataGridHistorique = New System.Windows.Forms.DataGridView()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.labal = New System.Windows.Forms.Label()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.RbNégatif = New System.Windows.Forms.RadioButton()
            Me.RbNeutre = New System.Windows.Forms.RadioButton()
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridHistorique, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'DataGridPays
            '
            Me.DataGridPays.AllowUserToAddRows = False
            Me.DataGridPays.AllowUserToDeleteRows = False
            Me.DataGridPays.AllowUserToOrderColumns = True
            Me.DataGridPays.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridPays.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridPays.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridPays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridPays.Location = New System.Drawing.Point(6, 25)
            Me.DataGridPays.Name = "DataGridPays"
            Me.DataGridPays.ReadOnly = True
            Me.DataGridPays.RowHeadersVisible = False
            Me.DataGridPays.Size = New System.Drawing.Size(583, 559)
            Me.DataGridPays.TabIndex = 7
            '
            'CbPays
            '
            Me.CbPays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbPays.FormattingEnabled = True
            Me.CbPays.Location = New System.Drawing.Point(16, 658)
            Me.CbPays.Name = "CbPays"
            Me.CbPays.Size = New System.Drawing.Size(186, 21)
            Me.CbPays.TabIndex = 8
            '
            'CbRecommandation
            '
            Me.CbRecommandation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbRecommandation.FormattingEnabled = True
            Me.CbRecommandation.Location = New System.Drawing.Point(231, 680)
            Me.CbRecommandation.Name = "CbRecommandation"
            Me.CbRecommandation.Size = New System.Drawing.Size(105, 21)
            Me.CbRecommandation.TabIndex = 9
            '
            'TLogin
            '
            Me.TLogin.Location = New System.Drawing.Point(231, 632)
            Me.TLogin.Name = "TLogin"
            Me.TLogin.Size = New System.Drawing.Size(63, 20)
            Me.TLogin.TabIndex = 11
            '
            'TCommentaire
            '
            Me.TCommentaire.Location = New System.Drawing.Point(476, 629)
            Me.TCommentaire.Multiline = True
            Me.TCommentaire.Name = "TCommentaire"
            Me.TCommentaire.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.TCommentaire.Size = New System.Drawing.Size(683, 76)
            Me.TCommentaire.TabIndex = 12
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(13, 610)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(170, 13)
            Me.Label1.TabIndex = 13
            Me.Label1.Text = "Modification recommandation pays"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(13, 639)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(30, 13)
            Me.Label2.TabIndex = 14
            Me.Label2.Text = "Pays"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(473, 613)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(103, 13)
            Me.Label3.TabIndex = 15
            Me.Label3.Text = "Ancien commentaire"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(228, 613)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(33, 13)
            Me.Label4.TabIndex = 16
            Me.Label4.Text = "Login"
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(228, 661)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(133, 13)
            Me.Label6.TabIndex = 18
            Me.Label6.Text = "Ancienne recommandation"
            '
            'BRecommandation
            '
            Me.BRecommandation.Location = New System.Drawing.Point(1145, 597)
            Me.BRecommandation.Name = "BRecommandation"
            Me.BRecommandation.Size = New System.Drawing.Size(104, 26)
            Me.BRecommandation.TabIndex = 20
            Me.BRecommandation.Text = "Recommandation"
            Me.BRecommandation.UseVisualStyleBackColor = True
            '
            'DataGridHistorique
            '
            Me.DataGridHistorique.AllowUserToAddRows = False
            Me.DataGridHistorique.AllowUserToDeleteRows = False
            Me.DataGridHistorique.AllowUserToOrderColumns = True
            Me.DataGridHistorique.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.DataGridHistorique.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridHistorique.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridHistorique.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridHistorique.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridHistorique.Location = New System.Drawing.Point(595, 25)
            Me.DataGridHistorique.Name = "DataGridHistorique"
            Me.DataGridHistorique.ReadOnly = True
            Me.DataGridHistorique.RowHeadersVisible = False
            Me.DataGridHistorique.Size = New System.Drawing.Size(654, 559)
            Me.DataGridHistorique.TabIndex = 21
            '
            'Label5
            '
            Me.Label5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(591, 9)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(150, 13)
            Me.Label5.TabIndex = 22
            Me.Label5.Text = "Historique de recommandation"
            '
            'labal
            '
            Me.labal.AutoSize = True
            Me.labal.Location = New System.Drawing.Point(12, 9)
            Me.labal.Name = "labal"
            Me.labal.Size = New System.Drawing.Size(171, 13)
            Me.labal.TabIndex = 23
            Me.labal.Text = "Dernière recommandation par pays"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.RbNégatif)
            Me.GroupBox1.Controls.Add(Me.RbNeutre)
            Me.GroupBox1.Location = New System.Drawing.Point(366, 600)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(92, 108)
            Me.GroupBox1.TabIndex = 42
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "ISR"
            '
            'RbNégatif
            '
            Me.RbNégatif.AutoSize = True
            Me.RbNégatif.Location = New System.Drawing.Point(15, 52)
            Me.RbNégatif.Name = "RbNégatif"
            Me.RbNégatif.Size = New System.Drawing.Size(59, 17)
            Me.RbNégatif.TabIndex = 1
            Me.RbNégatif.Text = "Négatif"
            Me.RbNégatif.UseVisualStyleBackColor = True
            '
            'RbNeutre
            '
            Me.RbNeutre.AutoSize = True
            Me.RbNeutre.Checked = True
            Me.RbNeutre.Location = New System.Drawing.Point(15, 26)
            Me.RbNeutre.Name = "RbNeutre"
            Me.RbNeutre.Size = New System.Drawing.Size(57, 17)
            Me.RbNeutre.TabIndex = 0
            Me.RbNeutre.TabStop = True
            Me.RbNeutre.Text = "Neutre"
            Me.RbNeutre.UseVisualStyleBackColor = True
            '
            'RecommandationPays
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1254, 722)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.labal)
            Me.Controls.Add(Me.Label5)
            Me.Controls.Add(Me.DataGridHistorique)
            Me.Controls.Add(Me.BRecommandation)
            Me.Controls.Add(Me.Label6)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.TCommentaire)
            Me.Controls.Add(Me.TLogin)
            Me.Controls.Add(Me.CbRecommandation)
            Me.Controls.Add(Me.CbPays)
            Me.Controls.Add(Me.DataGridPays)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "RecommandationPays"
            Me.Text = "Recommandations par pays"
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridHistorique, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents DataGridPays As System.Windows.Forms.DataGridView
        Friend WithEvents CbPays As System.Windows.Forms.ComboBox
        Friend WithEvents CbRecommandation As System.Windows.Forms.ComboBox
        Friend WithEvents TLogin As System.Windows.Forms.TextBox
        Friend WithEvents TCommentaire As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents BRecommandation As System.Windows.Forms.Button
        Friend WithEvents DataGridHistorique As System.Windows.Forms.DataGridView
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents labal As System.Windows.Forms.Label
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents RbNégatif As System.Windows.Forms.RadioButton
        Friend WithEvents RbNeutre As System.Windows.Forms.RadioButton
    End Class
End Namespace
