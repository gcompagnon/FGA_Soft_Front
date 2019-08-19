<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AccueilTitreGrille
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
        Me.lbl_titre = New System.Windows.Forms.Label()
        Me.DG_Titre = New System.Windows.Forms.DataGridView()
        Me.Btn_Rafraichir = New System.Windows.Forms.Button()
        Me.lbl_NbTitre = New System.Windows.Forms.Label()
        CType(Me.DG_Titre, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbl_titre
        '
        Me.lbl_titre.AutoSize = True
        Me.lbl_titre.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!)
        Me.lbl_titre.Location = New System.Drawing.Point(34, 21)
        Me.lbl_titre.Name = "lbl_titre"
        Me.lbl_titre.Size = New System.Drawing.Size(212, 18)
        Me.lbl_titre.TabIndex = 2
        Me.lbl_titre.Text = "Titres non affectés à une grille :"
        '
        'DG_Titre
        '
        Me.DG_Titre.AllowUserToAddRows = False
        Me.DG_Titre.AllowUserToDeleteRows = False
        Me.DG_Titre.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DG_Titre.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DG_Titre.Cursor = System.Windows.Forms.Cursors.Default
        Me.DG_Titre.Location = New System.Drawing.Point(37, 50)
        Me.DG_Titre.Name = "DG_Titre"
        Me.DG_Titre.ReadOnly = True
        Me.DG_Titre.Size = New System.Drawing.Size(775, 385)
        Me.DG_Titre.TabIndex = 3
        '
        'Btn_Rafraichir
        '
        Me.Btn_Rafraichir.Location = New System.Drawing.Point(359, 440)
        Me.Btn_Rafraichir.Name = "Btn_Rafraichir"
        Me.Btn_Rafraichir.Size = New System.Drawing.Size(145, 35)
        Me.Btn_Rafraichir.TabIndex = 4
        Me.Btn_Rafraichir.Text = "Rafraichir"
        Me.Btn_Rafraichir.UseVisualStyleBackColor = True
        '
        'lbl_NbTitre
        '
        Me.lbl_NbTitre.AutoSize = True
        Me.lbl_NbTitre.Location = New System.Drawing.Point(74, 451)
        Me.lbl_NbTitre.Name = "lbl_NbTitre"
        Me.lbl_NbTitre.Size = New System.Drawing.Size(39, 13)
        Me.lbl_NbTitre.TabIndex = 5
        Me.lbl_NbTitre.Text = "Label1"
        '
        'AccueilTitreGrille
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(846, 481)
        Me.Controls.Add(Me.lbl_NbTitre)
        Me.Controls.Add(Me.Btn_Rafraichir)
        Me.Controls.Add(Me.DG_Titre)
        Me.Controls.Add(Me.lbl_titre)
        Me.Name = "AccueilTitreGrille"
        Me.Text = "AffectationTitreGrille"
        CType(Me.DG_Titre, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbl_titre As System.Windows.Forms.Label
    Friend WithEvents DG_Titre As System.Windows.Forms.DataGridView
    Friend WithEvents Btn_Rafraichir As System.Windows.Forms.Button
    Friend WithEvents lbl_NbTitre As System.Windows.Forms.Label
End Class
