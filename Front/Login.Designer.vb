<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Login
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Login))
        Me.LName = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.BSeConnecter = New System.Windows.Forms.Button()
        Me.BAnnuler = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LName
        '
        Me.LName.AutoSize = True
        Me.LName.Location = New System.Drawing.Point(19, 30)
        Me.LName.Name = "LName"
        Me.LName.Size = New System.Drawing.Size(35, 13)
        Me.LName.TabIndex = 2
        Me.LName.Text = "Name"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LName)
        Me.GroupBox1.Location = New System.Drawing.Point(16, 11)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(356, 83)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Login"
        '
        'BSeConnecter
        '
        Me.BSeConnecter.Location = New System.Drawing.Point(259, 117)
        Me.BSeConnecter.Name = "BSeConnecter"
        Me.BSeConnecter.Size = New System.Drawing.Size(113, 34)
        Me.BSeConnecter.TabIndex = 4
        Me.BSeConnecter.Text = "Se connecter"
        Me.BSeConnecter.UseVisualStyleBackColor = True
        '
        'BAnnuler
        '
        Me.BAnnuler.Location = New System.Drawing.Point(127, 117)
        Me.BAnnuler.Name = "BAnnuler"
        Me.BAnnuler.Size = New System.Drawing.Size(105, 33)
        Me.BAnnuler.TabIndex = 5
        Me.BAnnuler.Text = "Annuler"
        Me.BAnnuler.UseVisualStyleBackColor = True
        '
        'Login
        '
        Me.AcceptButton = Me.BSeConnecter
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(395, 165)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.BSeConnecter)
        Me.Controls.Add(Me.BAnnuler)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Login"
        Me.Text = "Connection"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LName As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents BSeConnecter As System.Windows.Forms.Button
    Friend WithEvents BAnnuler As System.Windows.Forms.Button

End Class
