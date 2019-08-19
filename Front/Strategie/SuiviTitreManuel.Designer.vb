<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SuiviTitreManuel
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
        Me.DG_TitreManuel = New System.Windows.Forms.DataGridView()
        Me.Btn_Supprimer = New System.Windows.Forms.Button()
        CType(Me.DG_TitreManuel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DG_TitreManuel
        '
        Me.DG_TitreManuel.AllowUserToAddRows = False
        Me.DG_TitreManuel.AllowUserToDeleteRows = False
        Me.DG_TitreManuel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DG_TitreManuel.Location = New System.Drawing.Point(29, 35)
        Me.DG_TitreManuel.Name = "DG_TitreManuel"
        Me.DG_TitreManuel.Size = New System.Drawing.Size(543, 315)
        Me.DG_TitreManuel.TabIndex = 0
        '
        'Btn_Supprimer
        '
        Me.Btn_Supprimer.Location = New System.Drawing.Point(239, 356)
        Me.Btn_Supprimer.Name = "Btn_Supprimer"
        Me.Btn_Supprimer.Size = New System.Drawing.Size(136, 31)
        Me.Btn_Supprimer.TabIndex = 1
        Me.Btn_Supprimer.Text = "Supprimer"
        Me.Btn_Supprimer.UseVisualStyleBackColor = True
        '
        'SuiviTitreManuel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(614, 397)
        Me.Controls.Add(Me.Btn_Supprimer)
        Me.Controls.Add(Me.DG_TitreManuel)
        Me.Name = "SuiviTitreManuel"
        Me.Text = "SuiviTitreManuel"
        CType(Me.DG_TitreManuel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DG_TitreManuel As System.Windows.Forms.DataGridView
    Friend WithEvents Btn_Supprimer As System.Windows.Forms.Button
End Class
