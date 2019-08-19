Namespace Taux.BaseEmetteurs
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AjouterSignature
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AjouterSignature))
            Me.DataGridPays = New System.Windows.Forms.DataGridView()
            Me.DataGridSousSecteur = New System.Windows.Forms.DataGridView()
            Me.CbSousSecteur = New System.Windows.Forms.ComboBox()
            Me.BCréer = New System.Windows.Forms.Button()
            Me.CbPays = New System.Windows.Forms.ComboBox()
            Me.CbSecteur = New System.Windows.Forms.ComboBox()
            Me.TLibelle = New System.Windows.Forms.TextBox()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.RbNa = New System.Windows.Forms.RadioButton()
            Me.RbNegatif = New System.Windows.Forms.RadioButton()
            Me.RbReview = New System.Windows.Forms.RadioButton()
            Me.RbNeutre = New System.Windows.Forms.RadioButton()
            Me.RbNeutre_ = New System.Windows.Forms.RadioButton()
            Me.RbPositif = New System.Windows.Forms.RadioButton()
            Me.Label17 = New System.Windows.Forms.Label()
            Me.Label16 = New System.Windows.Forms.Label()
            Me.Label15 = New System.Windows.Forms.Label()
            Me.CbInternLT = New System.Windows.Forms.ComboBox()
            Me.CbInternCT = New System.Windows.Forms.ComboBox()
            Me.CbMoodysLT = New System.Windows.Forms.ComboBox()
            Me.CbSandPCT = New System.Windows.Forms.ComboBox()
            Me.CbSandBLT = New System.Windows.Forms.ComboBox()
            Me.CbFitchCT = New System.Windows.Forms.ComboBox()
            Me.CbFitchLT = New System.Windows.Forms.ComboBox()
            Me.CbMoodysCT = New System.Windows.Forms.ComboBox()
            Me.Label11 = New System.Windows.Forms.Label()
            Me.Label12 = New System.Windows.Forms.Label()
            Me.Label9 = New System.Windows.Forms.Label()
            Me.Label10 = New System.Windows.Forms.Label()
            Me.Label8 = New System.Windows.Forms.Label()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label20 = New System.Windows.Forms.Label()
            Me.Label18 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label14 = New System.Windows.Forms.Label()
            Me.Label13 = New System.Windows.Forms.Label()
            Me.TCommentaire = New System.Windows.Forms.RichTextBox()
            Me.BPolice = New System.Windows.Forms.Button()
            Me.FontDialog1 = New System.Windows.Forms.FontDialog()
            Me.LineShape4 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape3 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape2 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape5 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape9 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape6 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape10 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape7 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape11 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape8 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape14 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.LineShape12 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.ShapeContainer2 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
            Me.TGroupe = New System.Windows.Forms.TextBox()
            Me.GroupBox5 = New System.Windows.Forms.GroupBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label29 = New System.Windows.Forms.Label()
            Me.TNoteISR = New System.Windows.Forms.TextBox()
            Me.ShapeContainer4 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
            Me.LineShape16 = New Microsoft.VisualBasic.PowerPacks.LineShape()
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox5.SuspendLayout()
            Me.SuspendLayout()
            '
            'DataGridPays
            '
            Me.DataGridPays.AllowUserToAddRows = False
            Me.DataGridPays.AllowUserToDeleteRows = False
            Me.DataGridPays.AllowUserToOrderColumns = True
            Me.DataGridPays.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridPays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridPays.Location = New System.Drawing.Point(18, 239)
            Me.DataGridPays.Name = "DataGridPays"
            Me.DataGridPays.ReadOnly = True
            Me.DataGridPays.RowHeadersVisible = False
            Me.DataGridPays.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
            Me.DataGridPays.Size = New System.Drawing.Size(617, 71)
            Me.DataGridPays.TabIndex = 50
            '
            'DataGridSousSecteur
            '
            Me.DataGridSousSecteur.AllowUserToAddRows = False
            Me.DataGridSousSecteur.AllowUserToDeleteRows = False
            Me.DataGridSousSecteur.AllowUserToOrderColumns = True
            Me.DataGridSousSecteur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridSousSecteur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridSousSecteur.Location = New System.Drawing.Point(18, 161)
            Me.DataGridSousSecteur.Name = "DataGridSousSecteur"
            Me.DataGridSousSecteur.ReadOnly = True
            Me.DataGridSousSecteur.RowHeadersVisible = False
            Me.DataGridSousSecteur.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
            Me.DataGridSousSecteur.Size = New System.Drawing.Size(617, 72)
            Me.DataGridSousSecteur.TabIndex = 49
            '
            'CbSousSecteur
            '
            Me.CbSousSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSousSecteur.FormattingEnabled = True
            Me.CbSousSecteur.Location = New System.Drawing.Point(117, 91)
            Me.CbSousSecteur.Name = "CbSousSecteur"
            Me.CbSousSecteur.Size = New System.Drawing.Size(246, 21)
            Me.CbSousSecteur.TabIndex = 2
            '
            'BCréer
            '
            Me.BCréer.Location = New System.Drawing.Point(699, 12)
            Me.BCréer.Name = "BCréer"
            Me.BCréer.Size = New System.Drawing.Size(88, 23)
            Me.BCréer.TabIndex = 14
            Me.BCréer.Text = "Créer"
            Me.BCréer.UseVisualStyleBackColor = True
            '
            'CbPays
            '
            Me.CbPays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbPays.FormattingEnabled = True
            Me.CbPays.Location = New System.Drawing.Point(117, 121)
            Me.CbPays.Name = "CbPays"
            Me.CbPays.Size = New System.Drawing.Size(246, 21)
            Me.CbPays.TabIndex = 3
            '
            'CbSecteur
            '
            Me.CbSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSecteur.FormattingEnabled = True
            Me.CbSecteur.Location = New System.Drawing.Point(117, 66)
            Me.CbSecteur.Name = "CbSecteur"
            Me.CbSecteur.Size = New System.Drawing.Size(246, 21)
            Me.CbSecteur.TabIndex = 1
            '
            'TLibelle
            '
            Me.TLibelle.Location = New System.Drawing.Point(117, 38)
            Me.TLibelle.Name = "TLibelle"
            Me.TLibelle.Size = New System.Drawing.Size(246, 20)
            Me.TLibelle.TabIndex = 0
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.RbNa)
            Me.GroupBox1.Controls.Add(Me.RbNegatif)
            Me.GroupBox1.Controls.Add(Me.RbReview)
            Me.GroupBox1.Controls.Add(Me.RbNeutre)
            Me.GroupBox1.Controls.Add(Me.RbNeutre_)
            Me.GroupBox1.Controls.Add(Me.RbPositif)
            Me.GroupBox1.Location = New System.Drawing.Point(664, 155)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(118, 164)
            Me.GroupBox1.TabIndex = 42
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Recommandation"
            '
            'RbNa
            '
            Me.RbNa.AutoSize = True
            Me.RbNa.Checked = True
            Me.RbNa.Location = New System.Drawing.Point(9, 135)
            Me.RbNa.Name = "RbNa"
            Me.RbNa.Size = New System.Drawing.Size(40, 17)
            Me.RbNa.TabIndex = 5
            Me.RbNa.TabStop = True
            Me.RbNa.Text = "NA"
            Me.RbNa.UseVisualStyleBackColor = True
            '
            'RbNegatif
            '
            Me.RbNegatif.AutoSize = True
            Me.RbNegatif.Location = New System.Drawing.Point(9, 89)
            Me.RbNegatif.Name = "RbNegatif"
            Me.RbNegatif.Size = New System.Drawing.Size(59, 17)
            Me.RbNegatif.TabIndex = 4
            Me.RbNegatif.Text = "Négatif"
            Me.RbNegatif.UseVisualStyleBackColor = True
            '
            'RbReview
            '
            Me.RbReview.AutoSize = True
            Me.RbReview.Location = New System.Drawing.Point(9, 112)
            Me.RbReview.Name = "RbReview"
            Me.RbReview.Size = New System.Drawing.Size(61, 17)
            Me.RbReview.TabIndex = 3
            Me.RbReview.Text = "Review"
            Me.RbReview.UseVisualStyleBackColor = True
            '
            'RbNeutre
            '
            Me.RbNeutre.AutoSize = True
            Me.RbNeutre.Location = New System.Drawing.Point(9, 43)
            Me.RbNeutre.Name = "RbNeutre"
            Me.RbNeutre.Size = New System.Drawing.Size(57, 17)
            Me.RbNeutre.TabIndex = 2
            Me.RbNeutre.Text = "Neutre"
            Me.RbNeutre.UseVisualStyleBackColor = True
            '
            'RbNeutre_
            '
            Me.RbNeutre_.AutoSize = True
            Me.RbNeutre_.Location = New System.Drawing.Point(9, 66)
            Me.RbNeutre_.Name = "RbNeutre_"
            Me.RbNeutre_.Size = New System.Drawing.Size(63, 17)
            Me.RbNeutre_.TabIndex = 1
            Me.RbNeutre_.Text = "Neutre -"
            Me.RbNeutre_.UseVisualStyleBackColor = True
            '
            'RbPositif
            '
            Me.RbPositif.AutoSize = True
            Me.RbPositif.Location = New System.Drawing.Point(9, 20)
            Me.RbPositif.Name = "RbPositif"
            Me.RbPositif.Size = New System.Drawing.Size(53, 17)
            Me.RbPositif.TabIndex = 0
            Me.RbPositif.Text = "Positif"
            Me.RbPositif.UseVisualStyleBackColor = True
            '
            'Label17
            '
            Me.Label17.AutoSize = True
            Me.Label17.Location = New System.Drawing.Point(549, 28)
            Me.Label17.Name = "Label17"
            Me.Label17.Size = New System.Drawing.Size(40, 13)
            Me.Label17.TabIndex = 71
            Me.Label17.Text = "Interne"
            '
            'Label16
            '
            Me.Label16.AutoSize = True
            Me.Label16.Location = New System.Drawing.Point(596, 56)
            Me.Label16.Name = "Label16"
            Me.Label16.Size = New System.Drawing.Size(20, 13)
            Me.Label16.TabIndex = 70
            Me.Label16.Text = "LT"
            '
            'Label15
            '
            Me.Label15.AutoSize = True
            Me.Label15.Location = New System.Drawing.Point(519, 56)
            Me.Label15.Name = "Label15"
            Me.Label15.Size = New System.Drawing.Size(21, 13)
            Me.Label15.TabIndex = 69
            Me.Label15.Text = "CT"
            '
            'CbInternLT
            '
            Me.CbInternLT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbInternLT.FormattingEnabled = True
            Me.CbInternLT.Location = New System.Drawing.Point(569, 76)
            Me.CbInternLT.Name = "CbInternLT"
            Me.CbInternLT.Size = New System.Drawing.Size(73, 21)
            Me.CbInternLT.TabIndex = 12
            '
            'CbInternCT
            '
            Me.CbInternCT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbInternCT.FormattingEnabled = True
            Me.CbInternCT.Location = New System.Drawing.Point(492, 76)
            Me.CbInternCT.Name = "CbInternCT"
            Me.CbInternCT.Size = New System.Drawing.Size(76, 21)
            Me.CbInternCT.TabIndex = 11
            '
            'CbMoodysLT
            '
            Me.CbMoodysLT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbMoodysLT.FormattingEnabled = True
            Me.CbMoodysLT.Location = New System.Drawing.Point(106, 76)
            Me.CbMoodysLT.Name = "CbMoodysLT"
            Me.CbMoodysLT.Size = New System.Drawing.Size(75, 21)
            Me.CbMoodysLT.TabIndex = 6
            '
            'CbSandPCT
            '
            Me.CbSandPCT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSandPCT.FormattingEnabled = True
            Me.CbSandPCT.Location = New System.Drawing.Point(182, 76)
            Me.CbSandPCT.Name = "CbSandPCT"
            Me.CbSandPCT.Size = New System.Drawing.Size(79, 21)
            Me.CbSandPCT.TabIndex = 7
            '
            'CbSandBLT
            '
            Me.CbSandBLT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSandBLT.FormattingEnabled = True
            Me.CbSandBLT.Location = New System.Drawing.Point(262, 76)
            Me.CbSandBLT.Name = "CbSandBLT"
            Me.CbSandBLT.Size = New System.Drawing.Size(78, 21)
            Me.CbSandBLT.TabIndex = 8
            '
            'CbFitchCT
            '
            Me.CbFitchCT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbFitchCT.FormattingEnabled = True
            Me.CbFitchCT.Location = New System.Drawing.Point(341, 76)
            Me.CbFitchCT.Name = "CbFitchCT"
            Me.CbFitchCT.Size = New System.Drawing.Size(75, 21)
            Me.CbFitchCT.TabIndex = 9
            '
            'CbFitchLT
            '
            Me.CbFitchLT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbFitchLT.FormattingEnabled = True
            Me.CbFitchLT.Location = New System.Drawing.Point(417, 76)
            Me.CbFitchLT.Name = "CbFitchLT"
            Me.CbFitchLT.Size = New System.Drawing.Size(74, 21)
            Me.CbFitchLT.TabIndex = 10
            '
            'CbMoodysCT
            '
            Me.CbMoodysCT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbMoodysCT.FormattingEnabled = True
            Me.CbMoodysCT.Location = New System.Drawing.Point(27, 76)
            Me.CbMoodysCT.Name = "CbMoodysCT"
            Me.CbMoodysCT.Size = New System.Drawing.Size(78, 21)
            Me.CbMoodysCT.TabIndex = 5
            '
            'Label11
            '
            Me.Label11.AutoSize = True
            Me.Label11.Location = New System.Drawing.Point(448, 56)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(20, 13)
            Me.Label11.TabIndex = 60
            Me.Label11.Text = "LT"
            '
            'Label12
            '
            Me.Label12.AutoSize = True
            Me.Label12.Location = New System.Drawing.Point(367, 56)
            Me.Label12.Name = "Label12"
            Me.Label12.Size = New System.Drawing.Size(21, 13)
            Me.Label12.TabIndex = 59
            Me.Label12.Text = "CT"
            '
            'Label9
            '
            Me.Label9.AutoSize = True
            Me.Label9.Location = New System.Drawing.Point(293, 55)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(20, 13)
            Me.Label9.TabIndex = 58
            Me.Label9.Text = "LT"
            '
            'Label10
            '
            Me.Label10.AutoSize = True
            Me.Label10.Location = New System.Drawing.Point(56, 56)
            Me.Label10.Name = "Label10"
            Me.Label10.Size = New System.Drawing.Size(21, 13)
            Me.Label10.TabIndex = 57
            Me.Label10.Text = "CT"
            '
            'Label8
            '
            Me.Label8.AutoSize = True
            Me.Label8.Location = New System.Drawing.Point(134, 56)
            Me.Label8.Name = "Label8"
            Me.Label8.Size = New System.Drawing.Size(20, 13)
            Me.Label8.TabIndex = 56
            Me.Label8.Text = "LT"
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(210, 56)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(21, 13)
            Me.Label7.TabIndex = 55
            Me.Label7.Text = "CT"
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(405, 28)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(30, 13)
            Me.Label6.TabIndex = 54
            Me.Label6.Text = "Fitch"
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(222, 28)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(103, 13)
            Me.Label5.TabIndex = 53
            Me.Label5.Text = "Standard and Poor's"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(86, 28)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(44, 13)
            Me.Label4.TabIndex = 52
            Me.Label4.Text = "Moodys"
            '
            'Label20
            '
            Me.Label20.AutoSize = True
            Me.Label20.Location = New System.Drawing.Point(15, 45)
            Me.Label20.Name = "Label20"
            Me.Label20.Size = New System.Drawing.Size(37, 13)
            Me.Label20.TabIndex = 76
            Me.Label20.Text = "Libellé"
            '
            'Label18
            '
            Me.Label18.AutoSize = True
            Me.Label18.Location = New System.Drawing.Point(15, 94)
            Me.Label18.Name = "Label18"
            Me.Label18.Size = New System.Drawing.Size(69, 13)
            Me.Label18.TabIndex = 75
            Me.Label18.Text = "Sous secteur"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(15, 124)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(30, 13)
            Me.Label3.TabIndex = 74
            Me.Label3.Text = "Pays"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(15, 69)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(44, 13)
            Me.Label2.TabIndex = 73
            Me.Label2.Text = "Secteur"
            '
            'Label14
            '
            Me.Label14.AutoSize = True
            Me.Label14.Location = New System.Drawing.Point(16, 448)
            Me.Label14.Name = "Label14"
            Me.Label14.Size = New System.Drawing.Size(68, 13)
            Me.Label14.TabIndex = 79
            Me.Label14.Text = "Commentaire"
            '
            'Label13
            '
            Me.Label13.AutoSize = True
            Me.Label13.Location = New System.Drawing.Point(385, 35)
            Me.Label13.Name = "Label13"
            Me.Label13.Size = New System.Drawing.Size(42, 13)
            Me.Label13.TabIndex = 78
            Me.Label13.Text = "Groupe"
            '
            'TCommentaire
            '
            Me.TCommentaire.Location = New System.Drawing.Point(12, 467)
            Me.TCommentaire.Name = "TCommentaire"
            Me.TCommentaire.Size = New System.Drawing.Size(770, 479)
            Me.TCommentaire.TabIndex = 13
            Me.TCommentaire.Text = ""
            '
            'BPolice
            '
            Me.BPolice.BackgroundImage = CType(resources.GetObject("BPolice.BackgroundImage"), System.Drawing.Image)
            Me.BPolice.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BPolice.Location = New System.Drawing.Point(754, 441)
            Me.BPolice.Name = "BPolice"
            Me.BPolice.Size = New System.Drawing.Size(27, 20)
            Me.BPolice.TabIndex = 82
            Me.BPolice.Text = " "
            Me.BPolice.UseVisualStyleBackColor = True
            '
            'LineShape4
            '
            Me.LineShape4.Name = "LineShape4"
            Me.LineShape4.X1 = 23
            Me.LineShape4.X2 = 638
            Me.LineShape4.Y1 = 81
            Me.LineShape4.Y2 = 81
            '
            'LineShape3
            '
            Me.LineShape3.Name = "LineShape3"
            Me.LineShape3.X1 = 24
            Me.LineShape3.X2 = 639
            Me.LineShape3.Y1 = 59
            Me.LineShape3.Y2 = 59
            '
            'LineShape2
            '
            Me.LineShape2.Name = "LineShape2"
            Me.LineShape2.X1 = 23
            Me.LineShape2.X2 = 638
            Me.LineShape2.Y1 = 31
            Me.LineShape2.Y2 = 31
            '
            'LineShape1
            '
            Me.LineShape1.Name = "LineShape1"
            Me.LineShape1.X1 = 23
            Me.LineShape1.X2 = 638
            Me.LineShape1.Y1 = 6
            Me.LineShape1.Y2 = 6
            '
            'LineShape5
            '
            Me.LineShape5.Name = "LineShape5"
            Me.LineShape5.X1 = 23
            Me.LineShape5.X2 = 23
            Me.LineShape5.Y1 = 7
            Me.LineShape5.Y2 = 79
            '
            'LineShape9
            '
            Me.LineShape9.Name = "LineShape9"
            Me.LineShape9.X1 = 102
            Me.LineShape9.X2 = 102
            Me.LineShape9.Y1 = 32
            Me.LineShape9.Y2 = 79
            '
            'LineShape6
            '
            Me.LineShape6.Name = "LineShape6"
            Me.LineShape6.X1 = 178
            Me.LineShape6.X2 = 178
            Me.LineShape6.Y1 = 7
            Me.LineShape6.Y2 = 79
            '
            'LineShape10
            '
            Me.LineShape10.Name = "LineShape10"
            Me.LineShape10.X1 = 258
            Me.LineShape10.X2 = 258
            Me.LineShape10.Y1 = 31
            Me.LineShape10.Y2 = 81
            '
            'LineShape7
            '
            Me.LineShape7.Name = "LineShape7"
            Me.LineShape7.X1 = 337
            Me.LineShape7.X2 = 337
            Me.LineShape7.Y1 = 7
            Me.LineShape7.Y2 = 79
            '
            'LineShape11
            '
            Me.LineShape11.Name = "LineShape11"
            Me.LineShape11.X1 = 413
            Me.LineShape11.X2 = 413
            Me.LineShape11.Y1 = 32
            Me.LineShape11.Y2 = 79
            '
            'LineShape8
            '
            Me.LineShape8.Name = "LineShape8"
            Me.LineShape8.X1 = 488
            Me.LineShape8.X2 = 488
            Me.LineShape8.Y1 = 7
            Me.LineShape8.Y2 = 79
            '
            'LineShape14
            '
            Me.LineShape14.Name = "LineShape14"
            Me.LineShape14.X1 = 565
            Me.LineShape14.X2 = 565
            Me.LineShape14.Y1 = 33
            Me.LineShape14.Y2 = 80
            '
            'LineShape12
            '
            Me.LineShape12.Name = "LineShape12"
            Me.LineShape12.X1 = 639
            Me.LineShape12.X2 = 639
            Me.LineShape12.Y1 = 7
            Me.LineShape12.Y2 = 79
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Label17)
            Me.GroupBox2.Controls.Add(Me.Label16)
            Me.GroupBox2.Controls.Add(Me.Label15)
            Me.GroupBox2.Controls.Add(Me.CbInternLT)
            Me.GroupBox2.Controls.Add(Me.CbInternCT)
            Me.GroupBox2.Controls.Add(Me.CbMoodysLT)
            Me.GroupBox2.Controls.Add(Me.CbSandPCT)
            Me.GroupBox2.Controls.Add(Me.CbSandBLT)
            Me.GroupBox2.Controls.Add(Me.CbFitchCT)
            Me.GroupBox2.Controls.Add(Me.CbFitchLT)
            Me.GroupBox2.Controls.Add(Me.CbMoodysCT)
            Me.GroupBox2.Controls.Add(Me.Label11)
            Me.GroupBox2.Controls.Add(Me.Label12)
            Me.GroupBox2.Controls.Add(Me.Label9)
            Me.GroupBox2.Controls.Add(Me.Label10)
            Me.GroupBox2.Controls.Add(Me.Label8)
            Me.GroupBox2.Controls.Add(Me.Label7)
            Me.GroupBox2.Controls.Add(Me.Label6)
            Me.GroupBox2.Controls.Add(Me.Label5)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Controls.Add(Me.ShapeContainer2)
            Me.GroupBox2.Location = New System.Drawing.Point(11, 325)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(663, 111)
            Me.GroupBox2.TabIndex = 85
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Rating"
            '
            'ShapeContainer2
            '
            Me.ShapeContainer2.Location = New System.Drawing.Point(3, 16)
            Me.ShapeContainer2.Margin = New System.Windows.Forms.Padding(0)
            Me.ShapeContainer2.Name = "ShapeContainer2"
            Me.ShapeContainer2.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape12, Me.LineShape14, Me.LineShape8, Me.LineShape11, Me.LineShape7, Me.LineShape10, Me.LineShape6, Me.LineShape9, Me.LineShape5, Me.LineShape4, Me.LineShape3, Me.LineShape2, Me.LineShape1})
            Me.ShapeContainer2.Size = New System.Drawing.Size(657, 92)
            Me.ShapeContainer2.TabIndex = 72
            Me.ShapeContainer2.TabStop = False
            '
            'TGroupe
            '
            Me.TGroupe.Location = New System.Drawing.Point(388, 57)
            Me.TGroupe.Multiline = True
            Me.TGroupe.Name = "TGroupe"
            Me.TGroupe.Size = New System.Drawing.Size(370, 80)
            Me.TGroupe.TabIndex = 4
            '
            'GroupBox5
            '
            Me.GroupBox5.Controls.Add(Me.Label1)
            Me.GroupBox5.Controls.Add(Me.Label29)
            Me.GroupBox5.Controls.Add(Me.TNoteISR)
            Me.GroupBox5.Controls.Add(Me.ShapeContainer4)
            Me.GroupBox5.Location = New System.Drawing.Point(682, 326)
            Me.GroupBox5.Name = "GroupBox5"
            Me.GroupBox5.Size = New System.Drawing.Size(99, 109)
            Me.GroupBox5.TabIndex = 139
            Me.GroupBox5.TabStop = False
            Me.GroupBox5.Text = "ISR"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(13, 27)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(30, 13)
            Me.Label1.TabIndex = 142
            Me.Label1.Text = "Note"
            '
            'Label29
            '
            Me.Label29.AutoSize = True
            Me.Label29.Location = New System.Drawing.Point(69, 67)
            Me.Label29.Name = "Label29"
            Me.Label29.Size = New System.Drawing.Size(19, 13)
            Me.Label29.TabIndex = 141
            Me.Label29.Text = "20"
            '
            'TNoteISR
            '
            Me.TNoteISR.Location = New System.Drawing.Point(16, 52)
            Me.TNoteISR.Name = "TNoteISR"
            Me.TNoteISR.Size = New System.Drawing.Size(39, 20)
            Me.TNoteISR.TabIndex = 137
            Me.TNoteISR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'ShapeContainer4
            '
            Me.ShapeContainer4.Location = New System.Drawing.Point(3, 16)
            Me.ShapeContainer4.Margin = New System.Windows.Forms.Padding(0)
            Me.ShapeContainer4.Name = "ShapeContainer4"
            Me.ShapeContainer4.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape16})
            Me.ShapeContainer4.Size = New System.Drawing.Size(93, 90)
            Me.ShapeContainer4.TabIndex = 140
            Me.ShapeContainer4.TabStop = False
            '
            'LineShape16
            '
            Me.LineShape16.Name = "LineShape13"
            Me.LineShape16.X1 = 67
            Me.LineShape16.X2 = 50
            Me.LineShape16.Y1 = 44
            Me.LineShape16.Y2 = 62
            '
            'AjouterSignature
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(799, 959)
            Me.Controls.Add(Me.GroupBox5)
            Me.Controls.Add(Me.TGroupe)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.BPolice)
            Me.Controls.Add(Me.TCommentaire)
            Me.Controls.Add(Me.Label14)
            Me.Controls.Add(Me.Label13)
            Me.Controls.Add(Me.Label20)
            Me.Controls.Add(Me.Label18)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.DataGridPays)
            Me.Controls.Add(Me.DataGridSousSecteur)
            Me.Controls.Add(Me.CbSousSecteur)
            Me.Controls.Add(Me.BCréer)
            Me.Controls.Add(Me.CbPays)
            Me.Controls.Add(Me.CbSecteur)
            Me.Controls.Add(Me.TLibelle)
            Me.Controls.Add(Me.GroupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "AjouterSignature"
            Me.Text = "Ajouter nouvelle signature"
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox5.ResumeLayout(False)
            Me.GroupBox5.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents DataGridPays As System.Windows.Forms.DataGridView
        Friend WithEvents DataGridSousSecteur As System.Windows.Forms.DataGridView
        Friend WithEvents CbSousSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents BCréer As System.Windows.Forms.Button
        Friend WithEvents CbPays As System.Windows.Forms.ComboBox
        Friend WithEvents CbSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents TLibelle As System.Windows.Forms.TextBox
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents RbNegatif As System.Windows.Forms.RadioButton
        Friend WithEvents RbReview As System.Windows.Forms.RadioButton
        Friend WithEvents RbNeutre As System.Windows.Forms.RadioButton
        Friend WithEvents RbNeutre_ As System.Windows.Forms.RadioButton
        Friend WithEvents RbPositif As System.Windows.Forms.RadioButton
        Friend WithEvents Label17 As System.Windows.Forms.Label
        Friend WithEvents Label16 As System.Windows.Forms.Label
        Friend WithEvents Label15 As System.Windows.Forms.Label
        Friend WithEvents CbInternLT As System.Windows.Forms.ComboBox
        Friend WithEvents CbInternCT As System.Windows.Forms.ComboBox
        Friend WithEvents CbMoodysLT As System.Windows.Forms.ComboBox
        Friend WithEvents CbSandPCT As System.Windows.Forms.ComboBox
        Friend WithEvents CbSandBLT As System.Windows.Forms.ComboBox
        Friend WithEvents CbFitchCT As System.Windows.Forms.ComboBox
        Friend WithEvents CbFitchLT As System.Windows.Forms.ComboBox
        Friend WithEvents CbMoodysCT As System.Windows.Forms.ComboBox
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents Label12 As System.Windows.Forms.Label
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents Label10 As System.Windows.Forms.Label
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label20 As System.Windows.Forms.Label
        Friend WithEvents Label18 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label14 As System.Windows.Forms.Label
        Friend WithEvents Label13 As System.Windows.Forms.Label
        Friend WithEvents TCommentaire As System.Windows.Forms.RichTextBox
        Friend WithEvents BPolice As System.Windows.Forms.Button
        Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
        Friend WithEvents LineShape4 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape3 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape2 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape5 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape9 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape6 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape10 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape7 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape11 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape8 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape14 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents LineShape12 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents ShapeContainer2 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
        Friend WithEvents TGroupe As System.Windows.Forms.TextBox
        Friend WithEvents RbNa As System.Windows.Forms.RadioButton
        Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label29 As System.Windows.Forms.Label
        Friend WithEvents TNoteISR As System.Windows.Forms.TextBox
        Friend WithEvents ShapeContainer4 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
        Friend WithEvents LineShape16 As Microsoft.VisualBasic.PowerPacks.LineShape
    End Class
End Namespace
