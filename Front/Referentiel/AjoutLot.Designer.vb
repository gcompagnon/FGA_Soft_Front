<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AjoutLot
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AjoutLot))
        Me.BAjouter = New System.Windows.Forms.Button()
        Me.TIdLot = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.CheckedListGroupe = New System.Windows.Forms.CheckedListBox()
        Me.CheckedListCompte = New System.Windows.Forms.CheckedListBox()
        Me.RbCompte = New System.Windows.Forms.RadioButton()
        Me.RbGroupe = New System.Windows.Forms.RadioButton()
        Me.CbLibelleLot = New System.Windows.Forms.ComboBox()
        Me.BSupprimer = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BAjouter
        '
        Me.BAjouter.BackgroundImage = CType(resources.GetObject("BAjouter.BackgroundImage"), System.Drawing.Image)
        Me.BAjouter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BAjouter.Location = New System.Drawing.Point(308, 14)
        Me.BAjouter.Name = "BAjouter"
        Me.BAjouter.Size = New System.Drawing.Size(88, 26)
        Me.BAjouter.TabIndex = 12
        Me.BAjouter.Text = "Ajouter"
        Me.BAjouter.UseVisualStyleBackColor = True
        '
        'TIdLot
        '
        Me.TIdLot.Location = New System.Drawing.Point(101, 57)
        Me.TIdLot.Name = "TIdLot"
        Me.TIdLot.Size = New System.Drawing.Size(163, 20)
        Me.TIdLot.TabIndex = 11
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(24, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(51, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Libellé lot"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(45, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Id lot"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.CheckedListGroupe)
        Me.GroupBox1.Controls.Add(Me.CheckedListCompte)
        Me.GroupBox1.Controls.Add(Me.RbCompte)
        Me.GroupBox1.Controls.Add(Me.RbGroupe)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 128)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(529, 407)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Type de lot"
        '
        'CheckedListGroupe
        '
        Me.CheckedListGroupe.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckedListGroupe.FormattingEnabled = True
        Me.CheckedListGroupe.Location = New System.Drawing.Point(79, 16)
        Me.CheckedListGroupe.Name = "CheckedListGroupe"
        Me.CheckedListGroupe.Size = New System.Drawing.Size(171, 379)
        Me.CheckedListGroupe.TabIndex = 18
        '
        'CheckedListCompte
        '
        Me.CheckedListCompte.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckedListCompte.FormattingEnabled = True
        Me.CheckedListCompte.Location = New System.Drawing.Point(340, 14)
        Me.CheckedListCompte.Name = "CheckedListCompte"
        Me.CheckedListCompte.Size = New System.Drawing.Size(183, 379)
        Me.CheckedListCompte.TabIndex = 17
        '
        'RbCompte
        '
        Me.RbCompte.AutoSize = True
        Me.RbCompte.Location = New System.Drawing.Point(273, 16)
        Me.RbCompte.Name = "RbCompte"
        Me.RbCompte.Size = New System.Drawing.Size(61, 17)
        Me.RbCompte.TabIndex = 1
        Me.RbCompte.TabStop = True
        Me.RbCompte.Text = "Compte"
        Me.RbCompte.UseVisualStyleBackColor = True
        '
        'RbGroupe
        '
        Me.RbGroupe.AutoSize = True
        Me.RbGroupe.Location = New System.Drawing.Point(4, 23)
        Me.RbGroupe.Name = "RbGroupe"
        Me.RbGroupe.Size = New System.Drawing.Size(60, 17)
        Me.RbGroupe.TabIndex = 0
        Me.RbGroupe.TabStop = True
        Me.RbGroupe.Text = "Groupe"
        Me.RbGroupe.UseVisualStyleBackColor = True
        '
        'CbLibelleLot
        '
        Me.CbLibelleLot.FormattingEnabled = True
        Me.CbLibelleLot.Location = New System.Drawing.Point(101, 19)
        Me.CbLibelleLot.Name = "CbLibelleLot"
        Me.CbLibelleLot.Size = New System.Drawing.Size(165, 21)
        Me.CbLibelleLot.TabIndex = 16
        '
        'BSupprimer
        '
        Me.BSupprimer.BackgroundImage = CType(resources.GetObject("BSupprimer.BackgroundImage"), System.Drawing.Image)
        Me.BSupprimer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BSupprimer.Location = New System.Drawing.Point(411, 15)
        Me.BSupprimer.Name = "BSupprimer"
        Me.BSupprimer.Size = New System.Drawing.Size(94, 26)
        Me.BSupprimer.TabIndex = 17
        Me.BSupprimer.Text = "Supprimer"
        Me.BSupprimer.UseVisualStyleBackColor = True
        '
        'AjoutLot
        '
        Me.AcceptButton = Me.BAjouter
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(555, 560)
        Me.Controls.Add(Me.BSupprimer)
        Me.Controls.Add(Me.CbLibelleLot)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.BAjouter)
        Me.Controls.Add(Me.TIdLot)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AjoutLot"
        Me.Text = "Ajout d'un lot de portefeuille"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BAjouter As System.Windows.Forms.Button
    Friend WithEvents TIdLot As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RbCompte As System.Windows.Forms.RadioButton
    Friend WithEvents RbGroupe As System.Windows.Forms.RadioButton
    Friend WithEvents CheckedListCompte As System.Windows.Forms.CheckedListBox
    Friend WithEvents CheckedListGroupe As System.Windows.Forms.CheckedListBox
    Friend WithEvents CbLibelleLot As System.Windows.Forms.ComboBox
    Friend WithEvents BSupprimer As System.Windows.Forms.Button
End Class
