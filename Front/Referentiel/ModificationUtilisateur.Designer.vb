<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModificationUtilisateur
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModificationUtilisateur))
        Me.CbType = New System.Windows.Forms.ComboBox()
        Me.BCancel = New System.Windows.Forms.Button()
        Me.BAdd = New System.Windows.Forms.Button()
        Me.RbAdmin = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TPrenom = New System.Windows.Forms.TextBox()
        Me.TNom = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CbLogin = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TEmail = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BDelete = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CbType
        '
        Me.CbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbType.FormattingEnabled = True
        Me.CbType.Location = New System.Drawing.Point(125, 148)
        Me.CbType.Name = "CbType"
        Me.CbType.Size = New System.Drawing.Size(177, 21)
        Me.CbType.TabIndex = 14
        '
        'BCancel
        '
        Me.BCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BCancel.Location = New System.Drawing.Point(314, 249)
        Me.BCancel.Name = "BCancel"
        Me.BCancel.Size = New System.Drawing.Size(72, 24)
        Me.BCancel.TabIndex = 13
        Me.BCancel.Text = "Cancel"
        Me.BCancel.UseVisualStyleBackColor = True
        '
        'BAdd
        '
        Me.BAdd.Location = New System.Drawing.Point(315, 219)
        Me.BAdd.Name = "BAdd"
        Me.BAdd.Size = New System.Drawing.Size(71, 24)
        Me.BAdd.TabIndex = 12
        Me.BAdd.Text = "Update"
        Me.BAdd.UseVisualStyleBackColor = True
        '
        'RbAdmin
        '
        Me.RbAdmin.AutoSize = True
        Me.RbAdmin.Location = New System.Drawing.Point(27, 216)
        Me.RbAdmin.Name = "RbAdmin"
        Me.RbAdmin.Size = New System.Drawing.Size(92, 17)
        Me.RbAdmin.TabIndex = 10
        Me.RbAdmin.Text = "Administrateur"
        Me.RbAdmin.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(24, 150)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Type d'utilisateur"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(24, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(103, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Saisir login à modifer"
        '
        'TPrenom
        '
        Me.TPrenom.Location = New System.Drawing.Point(125, 87)
        Me.TPrenom.Name = "TPrenom"
        Me.TPrenom.Size = New System.Drawing.Size(177, 20)
        Me.TPrenom.TabIndex = 18
        '
        'TNom
        '
        Me.TNom.Location = New System.Drawing.Point(125, 116)
        Me.TNom.Name = "TNom"
        Me.TNom.Size = New System.Drawing.Size(177, 20)
        Me.TNom.TabIndex = 17
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(24, 94)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Prénom"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(24, 123)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "Nom"
        '
        'CbLogin
        '
        Me.CbLogin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbLogin.FormattingEnabled = True
        Me.CbLogin.Location = New System.Drawing.Point(27, 42)
        Me.CbLogin.Name = "CbLogin"
        Me.CbLogin.Size = New System.Drawing.Size(164, 21)
        Me.CbLogin.TabIndex = 20
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(24, 182)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(32, 13)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Email"
        '
        'TEmail
        '
        Me.TEmail.Location = New System.Drawing.Point(123, 179)
        Me.TEmail.Name = "TEmail"
        Me.TEmail.Size = New System.Drawing.Size(179, 20)
        Me.TEmail.TabIndex = 25
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.Panel1.Location = New System.Drawing.Point(340, 19)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(46, 44)
        Me.Panel1.TabIndex = 26
        '
        'BDelete
        '
        Me.BDelete.BackgroundImage = CType(resources.GetObject("BDelete.BackgroundImage"), System.Drawing.Image)
        Me.BDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BDelete.Location = New System.Drawing.Point(213, 37)
        Me.BDelete.Name = "BDelete"
        Me.BDelete.Size = New System.Drawing.Size(89, 28)
        Me.BDelete.TabIndex = 27
        Me.BDelete.Text = "Delete"
        Me.BDelete.UseVisualStyleBackColor = True
        '
        'ModificationUtilisateur
        '
        Me.AcceptButton = Me.BAdd
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BCancel
        Me.ClientSize = New System.Drawing.Size(400, 284)
        Me.Controls.Add(Me.BDelete)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TEmail)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.CbLogin)
        Me.Controls.Add(Me.TPrenom)
        Me.Controls.Add(Me.TNom)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.CbType)
        Me.Controls.Add(Me.BCancel)
        Me.Controls.Add(Me.BAdd)
        Me.Controls.Add(Me.RbAdmin)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ModificationUtilisateur"
        Me.Text = "Modification Utilisateur"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CbType As System.Windows.Forms.ComboBox
    Friend WithEvents BCancel As System.Windows.Forms.Button
    Friend WithEvents BAdd As System.Windows.Forms.Button
    Friend WithEvents RbAdmin As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TPrenom As System.Windows.Forms.TextBox
    Friend WithEvents TNom As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CbLogin As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TEmail As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents BDelete As System.Windows.Forms.Button
End Class
