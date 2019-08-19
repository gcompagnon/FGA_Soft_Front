<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AffecterTitreGrille
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
        Me.lbl_Titre = New System.Windows.Forms.Label()
        Me.Lbl_CodeTitre = New System.Windows.Forms.Label()
        Me.DG_Grille = New System.Windows.Forms.DataGridView()
        Me.Btn_Annuler = New System.Windows.Forms.Button()
        Me.Btn_Valider = New System.Windows.Forms.Button()
        CType(Me.DG_Grille, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbl_Titre
        '
        Me.lbl_Titre.Location = New System.Drawing.Point(12, 29)
        Me.lbl_Titre.Name = "lbl_Titre"
        Me.lbl_Titre.Size = New System.Drawing.Size(82, 25)
        Me.lbl_Titre.TabIndex = 0
        Me.lbl_Titre.Text = "Code Titre :"
        Me.lbl_Titre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Lbl_CodeTitre
        '
        Me.Lbl_CodeTitre.Location = New System.Drawing.Point(184, 29)
        Me.Lbl_CodeTitre.Name = "Lbl_CodeTitre"
        Me.Lbl_CodeTitre.Size = New System.Drawing.Size(110, 25)
        Me.Lbl_CodeTitre.TabIndex = 1
        Me.Lbl_CodeTitre.Text = "LibelleTitre"
        Me.Lbl_CodeTitre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DG_Grille
        '
        Me.DG_Grille.AllowUserToAddRows = False
        Me.DG_Grille.AllowUserToDeleteRows = False
        Me.DG_Grille.ColumnHeadersHeight = 25
        Me.DG_Grille.Cursor = System.Windows.Forms.Cursors.Default
        Me.DG_Grille.Location = New System.Drawing.Point(19, 64)
        Me.DG_Grille.Name = "DG_Grille"
        Me.DG_Grille.Size = New System.Drawing.Size(443, 225)
        Me.DG_Grille.TabIndex = 0
        '
        'Btn_Annuler
        '
        Me.Btn_Annuler.Location = New System.Drawing.Point(94, 335)
        Me.Btn_Annuler.Name = "Btn_Annuler"
        Me.Btn_Annuler.Size = New System.Drawing.Size(101, 35)
        Me.Btn_Annuler.TabIndex = 2
        Me.Btn_Annuler.Text = "Annuler"
        Me.Btn_Annuler.UseVisualStyleBackColor = True
        '
        'Btn_Valider
        '
        Me.Btn_Valider.Location = New System.Drawing.Point(340, 335)
        Me.Btn_Valider.Name = "Btn_Valider"
        Me.Btn_Valider.Size = New System.Drawing.Size(101, 35)
        Me.Btn_Valider.TabIndex = 3
        Me.Btn_Valider.Text = "Valider"
        Me.Btn_Valider.UseVisualStyleBackColor = True
        '
        'AffecterTitreGrille
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(523, 395)
        Me.Controls.Add(Me.Btn_Valider)
        Me.Controls.Add(Me.Btn_Annuler)
        Me.Controls.Add(Me.Lbl_CodeTitre)
        Me.Controls.Add(Me.lbl_Titre)
        Me.Controls.Add(Me.DG_Grille)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Name = "AffecterTitreGrille"
        Me.Text = "AffecterTitreGrille"
        CType(Me.DG_Grille, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DG_Grille As System.Windows.Forms.DataGridView
    Friend WithEvents lbl_Titre As System.Windows.Forms.Label
    Friend WithEvents Lbl_CodeTitre As System.Windows.Forms.Label
    Friend WithEvents Btn_Annuler As System.Windows.Forms.Button
    Friend WithEvents Btn_Valider As System.Windows.Forms.Button

End Class
