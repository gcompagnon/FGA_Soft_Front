<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Accueil
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Accueil))
        Me.MyMenu = New System.Windows.Forms.MenuStrip()
        Me.AdminMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.AjoutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ModificationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ScriptBDDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IntégrationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GestionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VérificationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MiddleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TransparenceToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.RéconciliationToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ActMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.TestToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.PortefeuilleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConsultationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DivMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.AffectationTitreGrilleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SuiviTitreManuelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AlimentationPTFFGAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllocationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TxMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnalyseIBoxxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.TestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TickerBBToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtractionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SolvencyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SimulationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CoefSecteursToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CoefValeurToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigNoteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DoublonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MyMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'MyMenu
        '
        Me.MyMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AdminMenu, Me.IntégrationToolStripMenuItem, Me.MiddleToolStripMenuItem, Me.ActMenu, Me.DivMenu, Me.TxMenu, Me.ReMenu, Me.SolvencyToolStripMenuItem, Me.DebugToolStripMenuItem})
        Me.MyMenu.Location = New System.Drawing.Point(0, 0)
        Me.MyMenu.Name = "MyMenu"
        Me.MyMenu.Size = New System.Drawing.Size(869, 24)
        Me.MyMenu.TabIndex = 0
        Me.MyMenu.Text = "MenuStrip1"
        '
        'AdminMenu
        '
        Me.AdminMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AjoutToolStripMenuItem, Me.ModificationToolStripMenuItem, Me.ToolStripSeparator1, Me.ScriptBDDToolStripMenuItem})
        Me.AdminMenu.Name = "AdminMenu"
        Me.AdminMenu.Size = New System.Drawing.Size(98, 20)
        Me.AdminMenu.Text = "Administrateur"
        '
        'AjoutToolStripMenuItem
        '
        Me.AjoutToolStripMenuItem.Image = CType(resources.GetObject("AjoutToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AjoutToolStripMenuItem.Name = "AjoutToolStripMenuItem"
        Me.AjoutToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.AjoutToolStripMenuItem.Text = "Ajout utilisateur"
        '
        'ModificationToolStripMenuItem
        '
        Me.ModificationToolStripMenuItem.Image = CType(resources.GetObject("ModificationToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ModificationToolStripMenuItem.Name = "ModificationToolStripMenuItem"
        Me.ModificationToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.ModificationToolStripMenuItem.Text = "Modification utilisateur"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(194, 6)
        '
        'ScriptBDDToolStripMenuItem
        '
        Me.ScriptBDDToolStripMenuItem.Name = "ScriptBDDToolStripMenuItem"
        Me.ScriptBDDToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.ScriptBDDToolStripMenuItem.Text = "Script BDD"
        '
        'IntégrationToolStripMenuItem
        '
        Me.IntégrationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GestionToolStripMenuItem, Me.VérificationToolStripMenuItem})
        Me.IntégrationToolStripMenuItem.Name = "IntégrationToolStripMenuItem"
        Me.IntégrationToolStripMenuItem.Size = New System.Drawing.Size(77, 20)
        Me.IntégrationToolStripMenuItem.Text = "Intégration"
        '
        'GestionToolStripMenuItem
        '
        Me.GestionToolStripMenuItem.Image = CType(resources.GetObject("GestionToolStripMenuItem.Image"), System.Drawing.Image)
        Me.GestionToolStripMenuItem.Name = "GestionToolStripMenuItem"
        Me.GestionToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.GestionToolStripMenuItem.Text = "Importation"
        '
        'VérificationToolStripMenuItem
        '
        Me.VérificationToolStripMenuItem.Image = CType(resources.GetObject("VérificationToolStripMenuItem.Image"), System.Drawing.Image)
        Me.VérificationToolStripMenuItem.Name = "VérificationToolStripMenuItem"
        Me.VérificationToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.VérificationToolStripMenuItem.Text = "Vérification"
        '
        'MiddleToolStripMenuItem
        '
        Me.MiddleToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TransparenceToolStripMenuItem1, Me.ToolStripSeparator2, Me.RéconciliationToolStripMenuItem1})
        Me.MiddleToolStripMenuItem.Name = "MiddleToolStripMenuItem"
        Me.MiddleToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.MiddleToolStripMenuItem.Text = "Middle"
        '
        'TransparenceToolStripMenuItem1
        '
        Me.TransparenceToolStripMenuItem1.Name = "TransparenceToolStripMenuItem1"
        Me.TransparenceToolStripMenuItem1.Size = New System.Drawing.Size(149, 22)
        Me.TransparenceToolStripMenuItem1.Text = "Transparence"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(146, 6)
        '
        'RéconciliationToolStripMenuItem1
        '
        Me.RéconciliationToolStripMenuItem1.Name = "RéconciliationToolStripMenuItem1"
        Me.RéconciliationToolStripMenuItem1.Size = New System.Drawing.Size(149, 22)
        Me.RéconciliationToolStripMenuItem1.Text = "Réconciliation"
        '
        'ActMenu
        '
        Me.ActMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TestToolStripMenuItem1, Me.PortefeuilleToolStripMenuItem, Me.ConsultationToolStripMenuItem})
        Me.ActMenu.Name = "ActMenu"
        Me.ActMenu.Size = New System.Drawing.Size(54, 20)
        Me.ActMenu.Text = "Action"
        '
        'TestToolStripMenuItem1
        '
        Me.TestToolStripMenuItem1.Image = Global.WindowsApplication1.My.Resources.Resources.socre
        Me.TestToolStripMenuItem1.Name = "TestToolStripMenuItem1"
        Me.TestToolStripMenuItem1.Size = New System.Drawing.Size(152, 22)
        Me.TestToolStripMenuItem1.Text = "Scores / Reco"
        '
        'PortefeuilleToolStripMenuItem
        '
        Me.PortefeuilleToolStripMenuItem.Name = "PortefeuilleToolStripMenuItem"
        Me.PortefeuilleToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.PortefeuilleToolStripMenuItem.Text = "Portefeuille"
        Me.PortefeuilleToolStripMenuItem.Visible = False
        '
        'ConsultationToolStripMenuItem
        '
        Me.ConsultationToolStripMenuItem.Image = Global.WindowsApplication1.My.Resources.Resources.F
        Me.ConsultationToolStripMenuItem.Name = "ConsultationToolStripMenuItem"
        Me.ConsultationToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ConsultationToolStripMenuItem.Text = "Consultation"
        '
        'DivMenu
        '
        Me.DivMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AffectationTitreGrilleToolStripMenuItem, Me.SuiviTitreManuelToolStripMenuItem, Me.AlimentationPTFFGAToolStripMenuItem, Me.AllocationToolStripMenuItem})
        Me.DivMenu.Name = "DivMenu"
        Me.DivMenu.Size = New System.Drawing.Size(65, 20)
        Me.DivMenu.Text = "Stratégie"
        '
        'AffectationTitreGrilleToolStripMenuItem
        '
        Me.AffectationTitreGrilleToolStripMenuItem.Name = "AffectationTitreGrilleToolStripMenuItem"
        Me.AffectationTitreGrilleToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.AffectationTitreGrilleToolStripMenuItem.Text = "Affectation Titre/Grille"
        '
        'SuiviTitreManuelToolStripMenuItem
        '
        Me.SuiviTitreManuelToolStripMenuItem.Name = "SuiviTitreManuelToolStripMenuItem"
        Me.SuiviTitreManuelToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.SuiviTitreManuelToolStripMenuItem.Text = "Suivi titre manuel "
        '
        'AlimentationPTFFGAToolStripMenuItem
        '
        Me.AlimentationPTFFGAToolStripMenuItem.Name = "AlimentationPTFFGAToolStripMenuItem"
        Me.AlimentationPTFFGAToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.AlimentationPTFFGAToolStripMenuItem.Text = "Alimentation PTF_FGA"
        '
        'AllocationToolStripMenuItem
        '
        Me.AllocationToolStripMenuItem.Name = "AllocationToolStripMenuItem"
        Me.AllocationToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.AllocationToolStripMenuItem.Text = "Allocation"
        '
        'TxMenu
        '
        Me.TxMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnalyseIBoxxToolStripMenuItem, Me.ToolStripSeparator3, Me.TestToolStripMenuItem, Me.TickerBBToolStripMenuItem})
        Me.TxMenu.Name = "TxMenu"
        Me.TxMenu.Size = New System.Drawing.Size(44, 20)
        Me.TxMenu.Text = "Taux"
        '
        'AnalyseIBoxxToolStripMenuItem
        '
        Me.AnalyseIBoxxToolStripMenuItem.Image = CType(resources.GetObject("AnalyseIBoxxToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AnalyseIBoxxToolStripMenuItem.Name = "AnalyseIBoxxToolStripMenuItem"
        Me.AnalyseIBoxxToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.AnalyseIBoxxToolStripMenuItem.Text = "Analyse iBoxx"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(151, 6)
        '
        'TestToolStripMenuItem
        '
        Me.TestToolStripMenuItem.Image = CType(resources.GetObject("TestToolStripMenuItem.Image"), System.Drawing.Image)
        Me.TestToolStripMenuItem.Name = "TestToolStripMenuItem"
        Me.TestToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.TestToolStripMenuItem.Text = "Base émetteurs"
        '
        'TickerBBToolStripMenuItem
        '
        Me.TickerBBToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ImportaToolStripMenuItem})
        Me.TickerBBToolStripMenuItem.Name = "TickerBBToolStripMenuItem"
        Me.TickerBBToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.TickerBBToolStripMenuItem.Text = "Ticker BBG"
        '
        'ImportaToolStripMenuItem
        '
        Me.ImportaToolStripMenuItem.Name = "ImportaToolStripMenuItem"
        Me.ImportaToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.ImportaToolStripMenuItem.Text = "Importation"
        '
        'ReMenu
        '
        Me.ReMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExtractionToolStripMenuItem})
        Me.ReMenu.Name = "ReMenu"
        Me.ReMenu.Size = New System.Drawing.Size(71, 20)
        Me.ReMenu.Text = "Reporting"
        '
        'ExtractionToolStripMenuItem
        '
        Me.ExtractionToolStripMenuItem.Name = "ExtractionToolStripMenuItem"
        Me.ExtractionToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ExtractionToolStripMenuItem.Text = "Extraction"
        '
        'SolvencyToolStripMenuItem
        '
        Me.SolvencyToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SimulationToolStripMenuItem})
        Me.SolvencyToolStripMenuItem.Name = "SolvencyToolStripMenuItem"
        Me.SolvencyToolStripMenuItem.Size = New System.Drawing.Size(66, 20)
        Me.SolvencyToolStripMenuItem.Text = "Solvency"
        '
        'SimulationToolStripMenuItem
        '
        Me.SimulationToolStripMenuItem.Name = "SimulationToolStripMenuItem"
        Me.SimulationToolStripMenuItem.Size = New System.Drawing.Size(131, 22)
        Me.SimulationToolStripMenuItem.Text = "Simulation"
        '
        'DebugToolStripMenuItem
        '
        Me.DebugToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CoefSecteursToolStripMenuItem, Me.CoefValeurToolStripMenuItem, Me.ConfigNoteToolStripMenuItem, Me.NewScreenToolStripMenuItem, Me.DoublonsToolStripMenuItem, Me.ImportationToolStripMenuItem})
        Me.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem"
        Me.DebugToolStripMenuItem.Size = New System.Drawing.Size(53, 20)
        Me.DebugToolStripMenuItem.Text = "debug"
        Me.DebugToolStripMenuItem.Visible = False
        '
        'CoefSecteursToolStripMenuItem
        '
        Me.CoefSecteursToolStripMenuItem.Name = "CoefSecteursToolStripMenuItem"
        Me.CoefSecteursToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.CoefSecteursToolStripMenuItem.Text = "CoefSecteurs"
        '
        'CoefValeurToolStripMenuItem
        '
        Me.CoefValeurToolStripMenuItem.Name = "CoefValeurToolStripMenuItem"
        Me.CoefValeurToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.CoefValeurToolStripMenuItem.Text = "CoefValeur"
        '
        'ConfigNoteToolStripMenuItem
        '
        Me.ConfigNoteToolStripMenuItem.Name = "ConfigNoteToolStripMenuItem"
        Me.ConfigNoteToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.ConfigNoteToolStripMenuItem.Text = "ConfigNote"
        '
        'NewScreenToolStripMenuItem
        '
        Me.NewScreenToolStripMenuItem.Name = "NewScreenToolStripMenuItem"
        Me.NewScreenToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.NewScreenToolStripMenuItem.Text = "New Screen"
        '
        'DoublonsToolStripMenuItem
        '
        Me.DoublonsToolStripMenuItem.Name = "DoublonsToolStripMenuItem"
        Me.DoublonsToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.DoublonsToolStripMenuItem.Text = "Doublons"
        '
        'ImportationToolStripMenuItem
        '
        Me.ImportationToolStripMenuItem.Name = "ImportationToolStripMenuItem"
        Me.ImportationToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.ImportationToolStripMenuItem.Text = "importation"
        '
        'Accueil
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(869, 583)
        Me.Controls.Add(Me.MyMenu)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MyMenu
        Me.MaximizeBox = False
        Me.Name = "Accueil"
        Me.Text = "Front Office - FGA"
        Me.MyMenu.ResumeLayout(False)
        Me.MyMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MyMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents AdminMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AjoutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TxMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ActMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ModificationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IntégrationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GestionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ScriptBDDToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AnalyseIBoxxToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MiddleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TransparenceToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RéconciliationToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VérificationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents DivMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExtractionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SolvencyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SimulationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PortefeuilleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CoefSecteursToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConfigNoteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CoefValeurToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    Friend WithEvents AffectationTitreGrilleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SuiviTitreManuelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AlimentationPTFFGAToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    Friend WithEvents TickerBBToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImportaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DoublonsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImportationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AllocationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConsultationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
