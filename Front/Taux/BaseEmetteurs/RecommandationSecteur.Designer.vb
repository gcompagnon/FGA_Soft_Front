Namespace Taux.BaseEmetteurs

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RecommandationSecteur
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RecommandationSecteur))
            Me.labal = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.DataGridHistorique = New System.Windows.Forms.DataGridView()
            Me.BRecommandation = New System.Windows.Forms.Button()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.TCommentaire = New System.Windows.Forms.TextBox()
            Me.TLogin = New System.Windows.Forms.TextBox()
            Me.CbRecommandation = New System.Windows.Forms.ComboBox()
            Me.CbSousSecteur = New System.Windows.Forms.ComboBox()
            Me.DataGridSousSecteur = New System.Windows.Forms.DataGridView()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.CbSecteur = New System.Windows.Forms.ComboBox()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.RbNégatif = New System.Windows.Forms.RadioButton()
            Me.RbNeutre = New System.Windows.Forms.RadioButton()
            CType(Me.DataGridHistorique, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'labal
            '
            Me.labal.AutoSize = True
            Me.labal.Location = New System.Drawing.Point(1, 6)
            Me.labal.Name = "labal"
            Me.labal.Size = New System.Drawing.Size(209, 13)
            Me.labal.TabIndex = 38
            Me.labal.Text = "Dernière recommandation par sous secteur"
            '
            'Label5
            '
            Me.Label5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(644, 6)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(150, 13)
            Me.Label5.TabIndex = 37
            Me.Label5.Text = "Historique de recommandation"
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
            Me.DataGridHistorique.Location = New System.Drawing.Point(647, 22)
            Me.DataGridHistorique.Name = "DataGridHistorique"
            Me.DataGridHistorique.ReadOnly = True
            Me.DataGridHistorique.RowHeadersVisible = False
            Me.DataGridHistorique.Size = New System.Drawing.Size(608, 574)
            Me.DataGridHistorique.TabIndex = 36
            '
            'BRecommandation
            '
            Me.BRecommandation.Location = New System.Drawing.Point(1143, 624)
            Me.BRecommandation.Name = "BRecommandation"
            Me.BRecommandation.Size = New System.Drawing.Size(108, 26)
            Me.BRecommandation.TabIndex = 35
            Me.BRecommandation.Text = "Recommandation"
            Me.BRecommandation.UseVisualStyleBackColor = True
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(222, 661)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(133, 13)
            Me.Label6.TabIndex = 34
            Me.Label6.Text = "Ancienne recommandation"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(222, 612)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(33, 13)
            Me.Label4.TabIndex = 33
            Me.Label4.Text = "Login"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(498, 612)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(103, 13)
            Me.Label3.TabIndex = 32
            Me.Label3.Text = "Ancien commentaire"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(9, 660)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(69, 13)
            Me.Label2.TabIndex = 31
            Me.Label2.Text = "Sous secteur"
            '
            'TCommentaire
            '
            Me.TCommentaire.Location = New System.Drawing.Point(501, 628)
            Me.TCommentaire.Multiline = True
            Me.TCommentaire.Name = "TCommentaire"
            Me.TCommentaire.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.TCommentaire.Size = New System.Drawing.Size(596, 92)
            Me.TCommentaire.TabIndex = 29
            '
            'TLogin
            '
            Me.TLogin.Location = New System.Drawing.Point(225, 631)
            Me.TLogin.Name = "TLogin"
            Me.TLogin.Size = New System.Drawing.Size(63, 20)
            Me.TLogin.TabIndex = 28
            '
            'CbRecommandation
            '
            Me.CbRecommandation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbRecommandation.FormattingEnabled = True
            Me.CbRecommandation.Location = New System.Drawing.Point(225, 679)
            Me.CbRecommandation.Name = "CbRecommandation"
            Me.CbRecommandation.Size = New System.Drawing.Size(105, 21)
            Me.CbRecommandation.TabIndex = 27
            '
            'CbSousSecteur
            '
            Me.CbSousSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSousSecteur.FormattingEnabled = True
            Me.CbSousSecteur.Location = New System.Drawing.Point(12, 679)
            Me.CbSousSecteur.Name = "CbSousSecteur"
            Me.CbSousSecteur.Size = New System.Drawing.Size(204, 21)
            Me.CbSousSecteur.TabIndex = 26
            '
            'DataGridSousSecteur
            '
            Me.DataGridSousSecteur.AllowUserToAddRows = False
            Me.DataGridSousSecteur.AllowUserToDeleteRows = False
            Me.DataGridSousSecteur.AllowUserToOrderColumns = True
            Me.DataGridSousSecteur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridSousSecteur.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridSousSecteur.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridSousSecteur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridSousSecteur.Location = New System.Drawing.Point(2, 22)
            Me.DataGridSousSecteur.Name = "DataGridSousSecteur"
            Me.DataGridSousSecteur.ReadOnly = True
            Me.DataGridSousSecteur.RowHeadersVisible = False
            Me.DataGridSousSecteur.Size = New System.Drawing.Size(638, 574)
            Me.DataGridSousSecteur.TabIndex = 25
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(9, 617)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(44, 13)
            Me.Label7.TabIndex = 40
            Me.Label7.Text = "Secteur"
            '
            'CbSecteur
            '
            Me.CbSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSecteur.FormattingEnabled = True
            Me.CbSecteur.Location = New System.Drawing.Point(12, 631)
            Me.CbSecteur.Name = "CbSecteur"
            Me.CbSecteur.Size = New System.Drawing.Size(204, 21)
            Me.CbSecteur.TabIndex = 39
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.RbNégatif)
            Me.GroupBox1.Controls.Add(Me.RbNeutre)
            Me.GroupBox1.Location = New System.Drawing.Point(375, 612)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(92, 108)
            Me.GroupBox1.TabIndex = 41
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
            'RecommandationSecteur
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1263, 732)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.Label7)
            Me.Controls.Add(Me.CbSecteur)
            Me.Controls.Add(Me.labal)
            Me.Controls.Add(Me.Label5)
            Me.Controls.Add(Me.DataGridHistorique)
            Me.Controls.Add(Me.BRecommandation)
            Me.Controls.Add(Me.Label6)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.TCommentaire)
            Me.Controls.Add(Me.TLogin)
            Me.Controls.Add(Me.CbRecommandation)
            Me.Controls.Add(Me.CbSousSecteur)
            Me.Controls.Add(Me.DataGridSousSecteur)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "RecommandationSecteur"
            Me.Text = "Recommandation par sous secteur"
            CType(Me.DataGridHistorique, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents labal As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents DataGridHistorique As System.Windows.Forms.DataGridView
        Friend WithEvents BRecommandation As System.Windows.Forms.Button
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents TCommentaire As System.Windows.Forms.TextBox
        Friend WithEvents TLogin As System.Windows.Forms.TextBox
        Friend WithEvents CbRecommandation As System.Windows.Forms.ComboBox
        Friend WithEvents CbSousSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents DataGridSousSecteur As System.Windows.Forms.DataGridView
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents CbSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents RbNégatif As System.Windows.Forms.RadioButton
        Friend WithEvents RbNeutre As System.Windows.Forms.RadioButton
    End Class
End Namespace