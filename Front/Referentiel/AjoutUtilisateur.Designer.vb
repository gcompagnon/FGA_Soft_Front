<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AjoutUtilisateur
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AjoutUtilisateur))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Administrateur = New System.Windows.Forms.CheckBox()
        Me.TLogin = New System.Windows.Forms.TextBox()
        Me.BAjouter = New System.Windows.Forms.Button()
        Me.BAnnuler = New System.Windows.Forms.Button()
        Me.CbTypeUser = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TNom = New System.Windows.Forms.TextBox()
        Me.TPrenom = New System.Windows.Forms.TextBox()
        Me.TEmail = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Login"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 114)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Type d'utilisateur"
        '
        'Administrateur
        '
        Me.Administrateur.AutoSize = True
        Me.Administrateur.Location = New System.Drawing.Point(301, 114)
        Me.Administrateur.Name = "Administrateur"
        Me.Administrateur.Size = New System.Drawing.Size(92, 17)
        Me.Administrateur.TabIndex = 4
        Me.Administrateur.Text = "Administrateur"
        Me.Administrateur.UseVisualStyleBackColor = True
        '
        'TLogin
        '
        Me.TLogin.Location = New System.Drawing.Point(100, 23)
        Me.TLogin.Name = "TLogin"
        Me.TLogin.Size = New System.Drawing.Size(181, 20)
        Me.TLogin.TabIndex = 1
        '
        'BAjouter
        '
        Me.BAjouter.BackgroundImage = CType(resources.GetObject("BAjouter.BackgroundImage"), System.Drawing.Image)
        Me.BAjouter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BAjouter.Location = New System.Drawing.Point(331, 187)
        Me.BAjouter.Name = "BAjouter"
        Me.BAjouter.Size = New System.Drawing.Size(91, 28)
        Me.BAjouter.TabIndex = 5
        Me.BAjouter.Text = "Ajouter"
        Me.BAjouter.UseVisualStyleBackColor = True
        '
        'BAnnuler
        '
        Me.BAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BAnnuler.Location = New System.Drawing.Point(243, 187)
        Me.BAnnuler.Name = "BAnnuler"
        Me.BAnnuler.Size = New System.Drawing.Size(82, 28)
        Me.BAnnuler.TabIndex = 6
        Me.BAnnuler.Text = "Annuler"
        Me.BAnnuler.UseVisualStyleBackColor = True
        '
        'CbTypeUser
        '
        Me.CbTypeUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTypeUser.FormattingEnabled = True
        Me.CbTypeUser.Location = New System.Drawing.Point(100, 109)
        Me.CbTypeUser.Name = "CbTypeUser"
        Me.CbTypeUser.Size = New System.Drawing.Size(181, 21)
        Me.CbTypeUser.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 83)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Nom"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 53)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Prénom"
        '
        'TNom
        '
        Me.TNom.Location = New System.Drawing.Point(100, 79)
        Me.TNom.Name = "TNom"
        Me.TNom.Size = New System.Drawing.Size(181, 20)
        Me.TNom.TabIndex = 3
        '
        'TPrenom
        '
        Me.TPrenom.Location = New System.Drawing.Point(100, 50)
        Me.TPrenom.Name = "TPrenom"
        Me.TPrenom.Size = New System.Drawing.Size(181, 20)
        Me.TPrenom.TabIndex = 2
        '
        'TEmail
        '
        Me.TEmail.Location = New System.Drawing.Point(100, 140)
        Me.TEmail.Name = "TEmail"
        Me.TEmail.Size = New System.Drawing.Size(181, 20)
        Me.TEmail.TabIndex = 5
        Me.TEmail.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 144)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(32, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Email"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(281, 143)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(70, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "@federisga.fr"
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.Panel1.Location = New System.Drawing.Point(355, 9)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(62, 56)
        Me.Panel1.TabIndex = 15
        '
        'AjoutUtilisateur
        '
        Me.AcceptButton = Me.BAjouter
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.CancelButton = Me.BAnnuler
        Me.ClientSize = New System.Drawing.Size(432, 231)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TEmail)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TPrenom)
        Me.Controls.Add(Me.TNom)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.CbTypeUser)
        Me.Controls.Add(Me.BAnnuler)
        Me.Controls.Add(Me.BAjouter)
        Me.Controls.Add(Me.TLogin)
        Me.Controls.Add(Me.Administrateur)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AjoutUtilisateur"
        Me.Text = "Ajout utilisateur"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Administrateur As System.Windows.Forms.CheckBox
    Friend WithEvents TLogin As System.Windows.Forms.TextBox
    Friend WithEvents BAjouter As System.Windows.Forms.Button
    Friend WithEvents BAnnuler As System.Windows.Forms.Button
    Friend WithEvents CbTypeUser As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TNom As System.Windows.Forms.TextBox
    Friend WithEvents TPrenom As System.Windows.Forms.TextBox
    Friend WithEvents TEmail As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
