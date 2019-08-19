<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScriptBDD
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ScriptBDD))
        Me.TextBox = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'TextBox
        '
        Me.TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox.Location = New System.Drawing.Point(0, 0)
        Me.TextBox.Multiline = True
        Me.TextBox.Name = "TextBox"
        Me.TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox.Size = New System.Drawing.Size(764, 825)
        Me.TextBox.TabIndex = 0
        '
        'ScriptBDD
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(764, 825)
        Me.Controls.Add(Me.TextBox)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ScriptBDD"
        Me.Text = "Script de la base de données"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox As System.Windows.Forms.TextBox
End Class
