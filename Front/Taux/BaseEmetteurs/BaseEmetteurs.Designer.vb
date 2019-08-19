Namespace Taux.BaseEmetteurs

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BaseEmetteurs
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BaseEmetteurs))
            Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Me.BCharger = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.CbLibelleBase = New System.Windows.Forms.ComboBox()
            Me.BChemin = New System.Windows.Forms.Button()
            Me.TChemin = New System.Windows.Forms.TextBox()
            Me.TSignature = New System.Windows.Forms.TextBox()
            Me.Libellé = New System.Windows.Forms.Label()
            Me.objFolderDialog = New System.Windows.Forms.FolderBrowserDialog()
            Me.DataGridFile = New System.Windows.Forms.DataGridView()
            Me.myPrintDocument = New System.Drawing.Printing.PrintDocument()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.DataGridNewFile = New System.Windows.Forms.DataGridView()
            Me.CSignature = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.CNom = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.CEmetteur = New System.Windows.Forms.DataGridViewComboBoxColumn()
            Me.CNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.BRafraichirFichier = New System.Windows.Forms.Button()
            Me.CbEmail = New System.Windows.Forms.CheckBox()
            Me.BAjouter = New System.Windows.Forms.Button()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
            Me.ActionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.CréerSignatureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.RecommandationSecteurToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.RecommandationPaysToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.TabControl1 = New System.Windows.Forms.TabControl()
            Me.Général = New System.Windows.Forms.TabPage()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.CbDataGrid = New System.Windows.Forms.ComboBox()
            Me.BPrint = New System.Windows.Forms.Button()
            Me.BRafraichirOmega = New System.Windows.Forms.Button()
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.BCreer = New System.Windows.Forms.Button()
            Me.CbLibelleOmega = New System.Windows.Forms.ComboBox()
            Me.TabPage2 = New System.Windows.Forms.TabPage()
            Me.DataGridSignature = New System.Windows.Forms.DataGridView()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.TabPage1 = New System.Windows.Forms.TabPage()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.DataGridSousSecteur = New System.Windows.Forms.DataGridView()
            Me.TabPage3 = New System.Windows.Forms.TabPage()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.DataGridPays = New System.Windows.Forms.DataGridView()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.Label8 = New System.Windows.Forms.Label()
            Me.Label9 = New System.Windows.Forms.Label()
            Me.Label10 = New System.Windows.Forms.Label()
            Me.Label11 = New System.Windows.Forms.Label()
            Me.Label12 = New System.Windows.Forms.Label()
            CType(Me.DataGridFile, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.DataGridNewFile, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.MenuStrip1.SuspendLayout()
            Me.TabControl1.SuspendLayout()
            Me.Général.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            CType(Me.DataGridSignature, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabPage1.SuspendLayout()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabPage3.SuspendLayout()
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'BCharger
            '
            Me.BCharger.Location = New System.Drawing.Point(284, 124)
            Me.BCharger.Name = "BCharger"
            Me.BCharger.Size = New System.Drawing.Size(65, 23)
            Me.BCharger.TabIndex = 2
            Me.BCharger.Text = "Charger"
            Me.BCharger.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(20, 21)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(106, 13)
            Me.Label1.TabIndex = 3
            Me.Label1.Text = "Recherche signature"
            '
            'CbLibelleBase
            '
            Me.CbLibelleBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbLibelleBase.FormattingEnabled = True
            Me.CbLibelleBase.Location = New System.Drawing.Point(23, 124)
            Me.CbLibelleBase.Name = "CbLibelleBase"
            Me.CbLibelleBase.Size = New System.Drawing.Size(235, 21)
            Me.CbLibelleBase.TabIndex = 8
            '
            'BChemin
            '
            Me.BChemin.Location = New System.Drawing.Point(241, 73)
            Me.BChemin.Name = "BChemin"
            Me.BChemin.Size = New System.Drawing.Size(75, 24)
            Me.BChemin.TabIndex = 16
            Me.BChemin.Text = "Chemin"
            Me.BChemin.UseVisualStyleBackColor = True
            '
            'TChemin
            '
            Me.TChemin.Location = New System.Drawing.Point(17, 31)
            Me.TChemin.Name = "TChemin"
            Me.TChemin.Size = New System.Drawing.Size(299, 20)
            Me.TChemin.TabIndex = 17
            '
            'TSignature
            '
            Me.TSignature.Location = New System.Drawing.Point(23, 37)
            Me.TSignature.Name = "TSignature"
            Me.TSignature.Size = New System.Drawing.Size(235, 20)
            Me.TSignature.TabIndex = 18
            '
            'Libellé
            '
            Me.Libellé.AutoSize = True
            Me.Libellé.Location = New System.Drawing.Point(20, 81)
            Me.Libellé.Name = "Libellé"
            Me.Libellé.Size = New System.Drawing.Size(37, 13)
            Me.Libellé.TabIndex = 19
            Me.Libellé.Text = "Libellé"
            '
            'objFolderDialog
            '
            Me.objFolderDialog.Description = "Sélectionnez un dossier"
            Me.objFolderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer
            '
            'DataGridFile
            '
            Me.DataGridFile.AllowUserToAddRows = False
            Me.DataGridFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridFile.Location = New System.Drawing.Point(8, 368)
            Me.DataGridFile.Name = "DataGridFile"
            Me.DataGridFile.RowHeadersVisible = False
            Me.DataGridFile.Size = New System.Drawing.Size(808, 347)
            Me.DataGridFile.TabIndex = 20
            '
            'myPrintDocument
            '
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(6, 9)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(154, 13)
            Me.Label2.TabIndex = 27
            Me.Label2.Text = "Nouveaux fichiers sur le disque"
            '
            'DataGridNewFile
            '
            Me.DataGridNewFile.AllowUserToAddRows = False
            Me.DataGridNewFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridNewFile.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me.DataGridNewFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridNewFile.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.CSignature, Me.CNom, Me.CEmetteur, Me.CNote})
            Me.DataGridNewFile.Location = New System.Drawing.Point(8, 25)
            Me.DataGridNewFile.Name = "DataGridNewFile"
            Me.DataGridNewFile.RowHeadersVisible = False
            Me.DataGridNewFile.Size = New System.Drawing.Size(808, 318)
            Me.DataGridNewFile.TabIndex = 28
            '
            'CSignature
            '
            Me.CSignature.HeaderText = "Signature"
            Me.CSignature.Name = "CSignature"
            Me.CSignature.Width = 77
            '
            'CNom
            '
            Me.CNom.HeaderText = "Nom Fichier"
            Me.CNom.Name = "CNom"
            Me.CNom.Width = 81
            '
            'CEmetteur
            '
            Me.CEmetteur.HeaderText = "Emetteur fichier"
            Me.CEmetteur.Name = "CEmetteur"
            Me.CEmetteur.Width = 78
            '
            'CNote
            '
            Me.CNote.HeaderText = "Note"
            Me.CNote.Name = "CNote"
            Me.CNote.Width = 55
            '
            'BRafraichirFichier
            '
            Me.BRafraichirFichier.BackgroundImage = CType(resources.GetObject("BRafraichirFichier.BackgroundImage"), System.Drawing.Image)
            Me.BRafraichirFichier.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRafraichirFichier.Location = New System.Drawing.Point(1090, 25)
            Me.BRafraichirFichier.Name = "BRafraichirFichier"
            Me.BRafraichirFichier.Size = New System.Drawing.Size(98, 25)
            Me.BRafraichirFichier.TabIndex = 29
            Me.BRafraichirFichier.Text = "Fichier"
            Me.BRafraichirFichier.UseVisualStyleBackColor = True
            '
            'CbEmail
            '
            Me.CbEmail.AutoSize = True
            Me.CbEmail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.CbEmail.Checked = True
            Me.CbEmail.CheckState = System.Windows.Forms.CheckState.Checked
            Me.CbEmail.Location = New System.Drawing.Point(950, 30)
            Me.CbEmail.Name = "CbEmail"
            Me.CbEmail.Size = New System.Drawing.Size(54, 17)
            Me.CbEmail.TabIndex = 30
            Me.CbEmail.Text = " Email"
            Me.CbEmail.UseVisualStyleBackColor = True
            '
            'BAjouter
            '
            Me.BAjouter.BackgroundImage = CType(resources.GetObject("BAjouter.BackgroundImage"), System.Drawing.Image)
            Me.BAjouter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BAjouter.Location = New System.Drawing.Point(849, 25)
            Me.BAjouter.Name = "BAjouter"
            Me.BAjouter.Size = New System.Drawing.Size(95, 25)
            Me.BAjouter.TabIndex = 31
            Me.BAjouter.Text = "Ajouter"
            Me.BAjouter.UseVisualStyleBackColor = True
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(6, 352)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(222, 13)
            Me.Label3.TabIndex = 32
            Me.Label3.Text = "Dernier fichier depuis la derniere déconnexion"
            '
            'MenuStrip1
            '
            Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ActionToolStripMenuItem})
            Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
            Me.MenuStrip1.Name = "MenuStrip1"
            Me.MenuStrip1.Size = New System.Drawing.Size(1214, 24)
            Me.MenuStrip1.TabIndex = 33
            Me.MenuStrip1.Text = "MenuStrip1"
            '
            'ActionToolStripMenuItem
            '
            Me.ActionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CréerSignatureToolStripMenuItem, Me.RecommandationSecteurToolStripMenuItem, Me.RecommandationPaysToolStripMenuItem})
            Me.ActionToolStripMenuItem.Name = "ActionToolStripMenuItem"
            Me.ActionToolStripMenuItem.Size = New System.Drawing.Size(49, 20)
            Me.ActionToolStripMenuItem.Text = "Action"
            '
            'CréerSignatureToolStripMenuItem
            '
            Me.CréerSignatureToolStripMenuItem.Name = "CréerSignatureToolStripMenuItem"
            Me.CréerSignatureToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
            Me.CréerSignatureToolStripMenuItem.Text = "Créer signature"
            '
            'RecommandationSecteurToolStripMenuItem
            '
            Me.RecommandationSecteurToolStripMenuItem.Name = "RecommandationSecteurToolStripMenuItem"
            Me.RecommandationSecteurToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
            Me.RecommandationSecteurToolStripMenuItem.Text = "Recommandation Secteur"
            '
            'RecommandationPaysToolStripMenuItem
            '
            Me.RecommandationPaysToolStripMenuItem.Name = "RecommandationPaysToolStripMenuItem"
            Me.RecommandationPaysToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
            Me.RecommandationPaysToolStripMenuItem.Text = "Recommandation Pays"
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(Me.Général)
            Me.TabControl1.Controls.Add(Me.TabPage2)
            Me.TabControl1.Controls.Add(Me.TabPage1)
            Me.TabControl1.Controls.Add(Me.TabPage3)
            Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TabControl1.Location = New System.Drawing.Point(0, 24)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.Size = New System.Drawing.Size(1214, 752)
            Me.TabControl1.TabIndex = 36
            '
            'Général
            '
            Me.Général.BackColor = System.Drawing.SystemColors.Control
            Me.Général.Controls.Add(Me.GroupBox3)
            Me.Général.Controls.Add(Me.BRafraichirOmega)
            Me.Général.Controls.Add(Me.CbEmail)
            Me.Général.Controls.Add(Me.Panel2)
            Me.Général.Controls.Add(Me.GroupBox2)
            Me.Général.Controls.Add(Me.GroupBox1)
            Me.Général.Controls.Add(Me.DataGridNewFile)
            Me.Général.Controls.Add(Me.Label2)
            Me.Général.Controls.Add(Me.Label3)
            Me.Général.Controls.Add(Me.DataGridFile)
            Me.Général.Controls.Add(Me.BAjouter)
            Me.Général.Controls.Add(Me.BRafraichirFichier)
            Me.Général.Location = New System.Drawing.Point(4, 22)
            Me.Général.Name = "Général"
            Me.Général.Padding = New System.Windows.Forms.Padding(3)
            Me.Général.Size = New System.Drawing.Size(1206, 726)
            Me.Général.TabIndex = 1
            Me.Général.Text = "Général"
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.CbDataGrid)
            Me.GroupBox3.Controls.Add(Me.BPrint)
            Me.GroupBox3.Location = New System.Drawing.Point(830, 334)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(335, 118)
            Me.GroupBox3.TabIndex = 35
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Impression"
            '
            'CbDataGrid
            '
            Me.CbDataGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbDataGrid.FormattingEnabled = True
            Me.CbDataGrid.Location = New System.Drawing.Point(32, 34)
            Me.CbDataGrid.Name = "CbDataGrid"
            Me.CbDataGrid.Size = New System.Drawing.Size(278, 21)
            Me.CbDataGrid.TabIndex = 48
            '
            'BPrint
            '
            Me.BPrint.BackgroundImage = CType(resources.GetObject("BPrint.BackgroundImage"), System.Drawing.Image)
            Me.BPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BPrint.Location = New System.Drawing.Point(235, 76)
            Me.BPrint.Name = "BPrint"
            Me.BPrint.Size = New System.Drawing.Size(96, 27)
            Me.BPrint.TabIndex = 47
            Me.BPrint.Text = " Impression"
            Me.BPrint.UseVisualStyleBackColor = True
            '
            'BRafraichirOmega
            '
            Me.BRafraichirOmega.BackgroundImage = CType(resources.GetObject("BRafraichirOmega.BackgroundImage"), System.Drawing.Image)
            Me.BRafraichirOmega.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRafraichirOmega.Location = New System.Drawing.Point(1090, 56)
            Me.BRafraichirOmega.Name = "BRafraichirOmega"
            Me.BRafraichirOmega.Size = New System.Drawing.Size(98, 25)
            Me.BRafraichirOmega.TabIndex = 37
            Me.BRafraichirOmega.Text = "Omega"
            Me.BRafraichirOmega.UseVisualStyleBackColor = True
            '
            'Panel2
            '
            Me.Panel2.BackgroundImage = CType(resources.GetObject("Panel2.BackgroundImage"), System.Drawing.Image)
            Me.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel2.Location = New System.Drawing.Point(999, 26)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(29, 24)
            Me.Panel2.TabIndex = 36
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.BChemin)
            Me.GroupBox2.Controls.Add(Me.TChemin)
            Me.GroupBox2.Location = New System.Drawing.Point(830, 487)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(335, 118)
            Me.GroupBox2.TabIndex = 34
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Déplacement base"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Panel1)
            Me.GroupBox1.Controls.Add(Me.BCreer)
            Me.GroupBox1.Controls.Add(Me.CbLibelleOmega)
            Me.GroupBox1.Controls.Add(Me.Libellé)
            Me.GroupBox1.Controls.Add(Me.TSignature)
            Me.GroupBox1.Controls.Add(Me.CbLibelleBase)
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.BCharger)
            Me.GroupBox1.Location = New System.Drawing.Point(830, 112)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(366, 181)
            Me.GroupBox1.TabIndex = 33
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Chargement"
            '
            'Panel1
            '
            Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
            Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel1.Location = New System.Drawing.Point(280, 19)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(44, 44)
            Me.Panel1.TabIndex = 22
            '
            'BCreer
            '
            Me.BCreer.Location = New System.Drawing.Point(284, 95)
            Me.BCreer.Name = "BCreer"
            Me.BCreer.Size = New System.Drawing.Size(65, 23)
            Me.BCreer.TabIndex = 21
            Me.BCreer.Text = "Créer"
            Me.BCreer.UseVisualStyleBackColor = True
            '
            'CbLibelleOmega
            '
            Me.CbLibelleOmega.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbLibelleOmega.FormattingEnabled = True
            Me.CbLibelleOmega.Location = New System.Drawing.Point(23, 97)
            Me.CbLibelleOmega.Name = "CbLibelleOmega"
            Me.CbLibelleOmega.Size = New System.Drawing.Size(235, 21)
            Me.CbLibelleOmega.TabIndex = 20
            '
            'TabPage2
            '
            Me.TabPage2.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage2.Controls.Add(Me.Label8)
            Me.TabPage2.Controls.Add(Me.Label7)
            Me.TabPage2.Controls.Add(Me.DataGridSignature)
            Me.TabPage2.Controls.Add(Me.Label6)
            Me.TabPage2.Location = New System.Drawing.Point(4, 22)
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage2.Size = New System.Drawing.Size(1206, 726)
            Me.TabPage2.TabIndex = 2
            Me.TabPage2.Text = "Signature"
            '
            'DataGridSignature
            '
            Me.DataGridSignature.AllowUserToAddRows = False
            Me.DataGridSignature.AllowUserToDeleteRows = False
            Me.DataGridSignature.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.DataGridSignature.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridSignature.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            DataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            DataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.DataGridSignature.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle15
            Me.DataGridSignature.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridSignature.EnableHeadersVisualStyles = False
            Me.DataGridSignature.Location = New System.Drawing.Point(6, 21)
            Me.DataGridSignature.Name = "DataGridSignature"
            Me.DataGridSignature.ReadOnly = True
            Me.DataGridSignature.Size = New System.Drawing.Size(1192, 697)
            Me.DataGridSignature.TabIndex = 45
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(9, 5)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(54, 13)
            Me.Label6.TabIndex = 44
            Me.Label6.Text = "Historique"
            '
            'TabPage1
            '
            Me.TabPage1.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage1.Controls.Add(Me.Label9)
            Me.TabPage1.Controls.Add(Me.Label10)
            Me.TabPage1.Controls.Add(Me.Label5)
            Me.TabPage1.Controls.Add(Me.DataGridSousSecteur)
            Me.TabPage1.Location = New System.Drawing.Point(4, 22)
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage1.Size = New System.Drawing.Size(1206, 726)
            Me.TabPage1.TabIndex = 3
            Me.TabPage1.Text = "Secteur"
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(9, 5)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(173, 13)
            Me.Label5.TabIndex = 45
            Me.Label5.Text = "Historique recommandation secteur"
            '
            'DataGridSousSecteur
            '
            Me.DataGridSousSecteur.AllowUserToAddRows = False
            Me.DataGridSousSecteur.AllowUserToDeleteRows = False
            Me.DataGridSousSecteur.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.DataGridSousSecteur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridSousSecteur.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.DataGridSousSecteur.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle13
            Me.DataGridSousSecteur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridSousSecteur.EnableHeadersVisualStyles = False
            Me.DataGridSousSecteur.Location = New System.Drawing.Point(6, 21)
            Me.DataGridSousSecteur.Name = "DataGridSousSecteur"
            Me.DataGridSousSecteur.ReadOnly = True
            Me.DataGridSousSecteur.Size = New System.Drawing.Size(1192, 697)
            Me.DataGridSousSecteur.TabIndex = 44
            '
            'TabPage3
            '
            Me.TabPage3.BackColor = System.Drawing.SystemColors.Control
            Me.TabPage3.Controls.Add(Me.Label11)
            Me.TabPage3.Controls.Add(Me.Label12)
            Me.TabPage3.Controls.Add(Me.Label4)
            Me.TabPage3.Controls.Add(Me.DataGridPays)
            Me.TabPage3.Location = New System.Drawing.Point(4, 22)
            Me.TabPage3.Name = "TabPage3"
            Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage3.Size = New System.Drawing.Size(1206, 726)
            Me.TabPage3.TabIndex = 4
            Me.TabPage3.Text = "Pays"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(9, 5)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(160, 13)
            Me.Label4.TabIndex = 44
            Me.Label4.Text = "Historique recommandation pays"
            '
            'DataGridPays
            '
            Me.DataGridPays.AllowUserToAddRows = False
            Me.DataGridPays.AllowUserToDeleteRows = False
            Me.DataGridPays.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.DataGridPays.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridPays.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.DataGridPays.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle14
            Me.DataGridPays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridPays.EnableHeadersVisualStyles = False
            Me.DataGridPays.Location = New System.Drawing.Point(6, 21)
            Me.DataGridPays.Name = "DataGridPays"
            Me.DataGridPays.ReadOnly = True
            Me.DataGridPays.Size = New System.Drawing.Size(1192, 697)
            Me.DataGridPays.TabIndex = 43
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.BackColor = System.Drawing.Color.Aqua
            Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label7.ForeColor = System.Drawing.Color.Black
            Me.Label7.Location = New System.Drawing.Point(368, 5)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(143, 13)
            Me.Label7.TabIndex = 46
            Me.Label7.Text = "Recommandation simple"
            '
            'Label8
            '
            Me.Label8.AutoSize = True
            Me.Label8.BackColor = System.Drawing.Color.PaleGreen
            Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label8.ForeColor = System.Drawing.SystemColors.ControlText
            Me.Label8.Location = New System.Drawing.Point(843, 5)
            Me.Label8.Name = "Label8"
            Me.Label8.Size = New System.Drawing.Size(129, 13)
            Me.Label8.TabIndex = 47
            Me.Label8.Text = "Recommandation ISR"
            '
            'Label9
            '
            Me.Label9.AutoSize = True
            Me.Label9.BackColor = System.Drawing.Color.PaleGreen
            Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label9.ForeColor = System.Drawing.SystemColors.ControlText
            Me.Label9.Location = New System.Drawing.Point(843, 5)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(129, 13)
            Me.Label9.TabIndex = 49
            Me.Label9.Text = "Recommandation ISR"
            '
            'Label10
            '
            Me.Label10.AutoSize = True
            Me.Label10.BackColor = System.Drawing.Color.Aqua
            Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label10.ForeColor = System.Drawing.Color.Black
            Me.Label10.Location = New System.Drawing.Point(368, 5)
            Me.Label10.Name = "Label10"
            Me.Label10.Size = New System.Drawing.Size(143, 13)
            Me.Label10.TabIndex = 48
            Me.Label10.Text = "Recommandation simple"
            '
            'Label11
            '
            Me.Label11.AutoSize = True
            Me.Label11.BackColor = System.Drawing.Color.PaleGreen
            Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label11.ForeColor = System.Drawing.SystemColors.ControlText
            Me.Label11.Location = New System.Drawing.Point(843, 5)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(129, 13)
            Me.Label11.TabIndex = 49
            Me.Label11.Text = "Recommandation ISR"
            '
            'Label12
            '
            Me.Label12.AutoSize = True
            Me.Label12.BackColor = System.Drawing.Color.Aqua
            Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label12.ForeColor = System.Drawing.Color.Black
            Me.Label12.Location = New System.Drawing.Point(368, 5)
            Me.Label12.Name = "Label12"
            Me.Label12.Size = New System.Drawing.Size(143, 13)
            Me.Label12.TabIndex = 48
            Me.Label12.Text = "Recommandation simple"
            '
            'BaseEmetteurs
            '
            Me.AcceptButton = Me.BCharger
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(1214, 776)
            Me.Controls.Add(Me.TabControl1)
            Me.Controls.Add(Me.MenuStrip1)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MainMenuStrip = Me.MenuStrip1
            Me.Name = "BaseEmetteurs"
            Me.Text = "Base de données émetteurs"
            CType(Me.DataGridFile, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.DataGridNewFile, System.ComponentModel.ISupportInitialize).EndInit()
            Me.MenuStrip1.ResumeLayout(False)
            Me.MenuStrip1.PerformLayout()
            Me.TabControl1.ResumeLayout(False)
            Me.Général.ResumeLayout(False)
            Me.Général.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.TabPage2.ResumeLayout(False)
            Me.TabPage2.PerformLayout()
            CType(Me.DataGridSignature, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabPage1.ResumeLayout(False)
            Me.TabPage1.PerformLayout()
            CType(Me.DataGridSousSecteur, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabPage3.ResumeLayout(False)
            Me.TabPage3.PerformLayout()
            CType(Me.DataGridPays, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents BCharger As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents CbLibelleBase As System.Windows.Forms.ComboBox
        Friend WithEvents BChemin As System.Windows.Forms.Button
        Friend WithEvents TChemin As System.Windows.Forms.TextBox
        Friend WithEvents TSignature As System.Windows.Forms.TextBox
        Friend WithEvents Libellé As System.Windows.Forms.Label
        Friend WithEvents objFolderDialog As System.Windows.Forms.FolderBrowserDialog
        Friend WithEvents DataGridFile As System.Windows.Forms.DataGridView
        Friend WithEvents myPrintDocument As System.Drawing.Printing.PrintDocument
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents DataGridNewFile As System.Windows.Forms.DataGridView
        Friend WithEvents BRafraichirFichier As System.Windows.Forms.Button
        Friend WithEvents CbEmail As System.Windows.Forms.CheckBox
        Friend WithEvents BAjouter As System.Windows.Forms.Button
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
        Friend WithEvents ActionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CréerSignatureToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RecommandationSecteurToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RecommandationPaysToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents Général As System.Windows.Forms.TabPage
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents BRafraichirOmega As System.Windows.Forms.Button
        Friend WithEvents BCreer As System.Windows.Forms.Button
        Friend WithEvents CbLibelleOmega As System.Windows.Forms.ComboBox
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents CSignature As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents CNom As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents CEmetteur As System.Windows.Forms.DataGridViewComboBoxColumn
        Friend WithEvents CNote As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents DataGridSousSecteur As System.Windows.Forms.DataGridView
        Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents DataGridPays As System.Windows.Forms.DataGridView
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents CbDataGrid As System.Windows.Forms.ComboBox
        Friend WithEvents BPrint As System.Windows.Forms.Button
        Friend WithEvents DataGridSignature As System.Windows.Forms.DataGridView
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents Label10 As System.Windows.Forms.Label
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents Label12 As System.Windows.Forms.Label
    End Class
End Namespace
