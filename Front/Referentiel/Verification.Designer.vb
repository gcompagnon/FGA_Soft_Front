<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Verification
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Verification))
        Me.MonthCalendar = New System.Windows.Forms.MonthCalendar()
        Me.Gp = New System.Windows.Forms.GroupBox()
        Me.RbNiv2 = New System.Windows.Forms.RadioButton()
        Me.RbNiv1 = New System.Windows.Forms.RadioButton()
        Me.BCharger = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.DataGridSecteur = New System.Windows.Forms.DataGridView()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.DataGridTypeActif = New System.Windows.Forms.DataGridView()
        Me.BEffacer = New System.Windows.Forms.Button()
        Me.Gp.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.DataGridSecteur, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.DataGridTypeActif, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MonthCalendar
        '
        Me.MonthCalendar.Location = New System.Drawing.Point(181, 5)
        Me.MonthCalendar.Name = "MonthCalendar"
        Me.MonthCalendar.TabIndex = 40
        '
        'Gp
        '
        Me.Gp.Controls.Add(Me.RbNiv2)
        Me.Gp.Controls.Add(Me.RbNiv1)
        Me.Gp.Location = New System.Drawing.Point(398, 38)
        Me.Gp.Name = "Gp"
        Me.Gp.Size = New System.Drawing.Size(118, 87)
        Me.Gp.TabIndex = 41
        Me.Gp.TabStop = False
        Me.Gp.Text = "Niveau"
        '
        'RbNiv2
        '
        Me.RbNiv2.AutoSize = True
        Me.RbNiv2.Checked = True
        Me.RbNiv2.Location = New System.Drawing.Point(22, 53)
        Me.RbNiv2.Name = "RbNiv2"
        Me.RbNiv2.Size = New System.Drawing.Size(68, 17)
        Me.RbNiv2.TabIndex = 1
        Me.RbNiv2.TabStop = True
        Me.RbNiv2.Text = "Niveau 2"
        Me.RbNiv2.UseVisualStyleBackColor = True
        '
        'RbNiv1
        '
        Me.RbNiv1.AutoSize = True
        Me.RbNiv1.Location = New System.Drawing.Point(22, 30)
        Me.RbNiv1.Name = "RbNiv1"
        Me.RbNiv1.Size = New System.Drawing.Size(68, 17)
        Me.RbNiv1.TabIndex = 0
        Me.RbNiv1.Text = "Niveau 1"
        Me.RbNiv1.UseVisualStyleBackColor = True
        '
        'BCharger
        '
        Me.BCharger.BackgroundImage = CType(resources.GetObject("BCharger.BackgroundImage"), System.Drawing.Image)
        Me.BCharger.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BCharger.Location = New System.Drawing.Point(566, 38)
        Me.BCharger.Name = "BCharger"
        Me.BCharger.Size = New System.Drawing.Size(123, 27)
        Me.BCharger.TabIndex = 42
        Me.BCharger.Text = "Charger"
        Me.BCharger.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(1, 155)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1168, 545)
        Me.TabControl1.TabIndex = 44
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.TabPage1.Controls.Add(Me.DataGridSecteur)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1160, 519)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Secteur"
        '
        'DataGridSecteur
        '
        Me.DataGridSecteur.AllowUserToAddRows = False
        Me.DataGridSecteur.AllowUserToDeleteRows = False
        Me.DataGridSecteur.AllowUserToOrderColumns = True
        Me.DataGridSecteur.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridSecteur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridSecteur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridSecteur.Location = New System.Drawing.Point(1, 4)
        Me.DataGridSecteur.Name = "DataGridSecteur"
        Me.DataGridSecteur.ReadOnly = True
        Me.DataGridSecteur.Size = New System.Drawing.Size(1151, 510)
        Me.DataGridSecteur.TabIndex = 39
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.TabPage2.Controls.Add(Me.DataGridTypeActif)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1160, 519)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Type Actif"
        '
        'DataGridTypeActif
        '
        Me.DataGridTypeActif.AllowUserToAddRows = False
        Me.DataGridTypeActif.AllowUserToDeleteRows = False
        Me.DataGridTypeActif.AllowUserToOrderColumns = True
        Me.DataGridTypeActif.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridTypeActif.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridTypeActif.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridTypeActif.Location = New System.Drawing.Point(0, 4)
        Me.DataGridTypeActif.Name = "DataGridTypeActif"
        Me.DataGridTypeActif.ReadOnly = True
        Me.DataGridTypeActif.Size = New System.Drawing.Size(1152, 510)
        Me.DataGridTypeActif.TabIndex = 40
        '
        'BEffacer
        '
        Me.BEffacer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BEffacer.Location = New System.Drawing.Point(566, 73)
        Me.BEffacer.Name = "BEffacer"
        Me.BEffacer.Size = New System.Drawing.Size(123, 27)
        Me.BEffacer.TabIndex = 45
        Me.BEffacer.Text = "Effacer"
        Me.BEffacer.UseVisualStyleBackColor = True
        '
        'Verification
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1158, 700)
        Me.Controls.Add(Me.BEffacer)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.BCharger)
        Me.Controls.Add(Me.Gp)
        Me.Controls.Add(Me.MonthCalendar)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Verification"
        Me.Text = "Verification"
        Me.Gp.ResumeLayout(False)
        Me.Gp.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        CType(Me.DataGridSecteur, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        CType(Me.DataGridTypeActif, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MonthCalendar As System.Windows.Forms.MonthCalendar
    Friend WithEvents Gp As System.Windows.Forms.GroupBox
    Friend WithEvents RbNiv2 As System.Windows.Forms.RadioButton
    Friend WithEvents RbNiv1 As System.Windows.Forms.RadioButton
    Friend WithEvents BCharger As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents DataGridSecteur As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridTypeActif As System.Windows.Forms.DataGridView
    Friend WithEvents BEffacer As System.Windows.Forms.Button
End Class
