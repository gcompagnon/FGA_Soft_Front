Namespace Referentiel
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Transparence
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Transparence))
            Me.GpNiveau = New System.Windows.Forms.GroupBox()
            Me.RbNiv2 = New System.Windows.Forms.RadioButton()
            Me.RbNiv0 = New System.Windows.Forms.RadioButton()
            Me.RbNiv1 = New System.Windows.Forms.RadioButton()
            Me.DataGridSelect = New System.Windows.Forms.DataGridView()
            Me.BCharger = New System.Windows.Forms.Button()
            Me.CbSecteur = New System.Windows.Forms.ComboBox()
            Me.BEffacer = New System.Windows.Forms.Button()
            Me.GpFiltre = New System.Windows.Forms.GroupBox()
            Me.Tranche = New System.Windows.Forms.Label()
            Me.CbTrancheMaturite = New System.Windows.Forms.ComboBox()
            Me.CbPays = New System.Windows.Forms.ComboBox()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.TLibelleTitre = New System.Windows.Forms.TextBox()
            Me.TIsinTitre = New System.Windows.Forms.TextBox()
            Me.CbLotPortefeuille = New System.Windows.Forms.ComboBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.CbSousSecteur = New System.Windows.Forms.ComboBox()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.GbAggrégations = New System.Windows.Forms.GroupBox()
            Me.RbTrancheMaturite = New System.Windows.Forms.RadioButton()
            Me.RbRating = New System.Windows.Forms.RadioButton()
            Me.RbGroupeEmetteur = New System.Windows.Forms.RadioButton()
            Me.RbTypeDeDette = New System.Windows.Forms.RadioButton()
            Me.RbTypeActif = New System.Windows.Forms.RadioButton()
            Me.RbZoneGeographique = New System.Windows.Forms.RadioButton()
            Me.RbPays = New System.Windows.Forms.RadioButton()
            Me.RbSousSecteurs = New System.Windows.Forms.RadioButton()
            Me.RbSecteurs = New System.Windows.Forms.RadioButton()
            Me.BExcel = New System.Windows.Forms.Button()
            Me.DataGridAggr = New System.Windows.Forms.DataGridView()
            Me.MonthCalendar = New System.Windows.Forms.MonthCalendar()
            Me.myPrintDocument = New System.Drawing.Printing.PrintDocument()
            Me.BPrint = New System.Windows.Forms.Button()
            Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
            Me.BRapport = New System.Windows.Forms.Button()
            Me.CbRapport = New System.Windows.Forms.ComboBox()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.GpNiveau.SuspendLayout()
            CType(Me.DataGridSelect, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GpFiltre.SuspendLayout()
            Me.GbAggrégations.SuspendLayout()
            CType(Me.DataGridAggr, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'GpNiveau
            '
            Me.GpNiveau.Controls.Add(Me.RbNiv2)
            Me.GpNiveau.Controls.Add(Me.RbNiv0)
            Me.GpNiveau.Controls.Add(Me.RbNiv1)
            resources.ApplyResources(Me.GpNiveau, "GpNiveau")
            Me.GpNiveau.Name = "GpNiveau"
            Me.GpNiveau.TabStop = False
            '
            'RbNiv2
            '
            resources.ApplyResources(Me.RbNiv2, "RbNiv2")
            Me.RbNiv2.Name = "RbNiv2"
            Me.RbNiv2.TabStop = True
            Me.RbNiv2.UseVisualStyleBackColor = True
            '
            'RbNiv0
            '
            resources.ApplyResources(Me.RbNiv0, "RbNiv0")
            Me.RbNiv0.Name = "RbNiv0"
            Me.RbNiv0.TabStop = True
            Me.RbNiv0.UseVisualStyleBackColor = True
            '
            'RbNiv1
            '
            resources.ApplyResources(Me.RbNiv1, "RbNiv1")
            Me.RbNiv1.Name = "RbNiv1"
            Me.RbNiv1.TabStop = True
            Me.RbNiv1.UseVisualStyleBackColor = True
            '
            'DataGridSelect
            '
            Me.DataGridSelect.AllowUserToAddRows = False
            Me.DataGridSelect.AllowUserToDeleteRows = False
            Me.DataGridSelect.AllowUserToOrderColumns = True
            resources.ApplyResources(Me.DataGridSelect, "DataGridSelect")
            Me.DataGridSelect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            Me.DataGridSelect.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridSelect.Name = "DataGridSelect"
            Me.DataGridSelect.ReadOnly = True
            '
            'BCharger
            '
            resources.ApplyResources(Me.BCharger, "BCharger")
            Me.BCharger.Name = "BCharger"
            Me.BCharger.UseVisualStyleBackColor = True
            '
            'CbSecteur
            '
            Me.CbSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSecteur.FormattingEnabled = True
            resources.ApplyResources(Me.CbSecteur, "CbSecteur")
            Me.CbSecteur.Name = "CbSecteur"
            '
            'BEffacer
            '
            resources.ApplyResources(Me.BEffacer, "BEffacer")
            Me.BEffacer.Name = "BEffacer"
            Me.BEffacer.UseVisualStyleBackColor = True
            '
            'GpFiltre
            '
            Me.GpFiltre.Controls.Add(Me.Tranche)
            Me.GpFiltre.Controls.Add(Me.CbTrancheMaturite)
            Me.GpFiltre.Controls.Add(Me.CbPays)
            Me.GpFiltre.Controls.Add(Me.Label6)
            Me.GpFiltre.Controls.Add(Me.Label5)
            Me.GpFiltre.Controls.Add(Me.Label4)
            Me.GpFiltre.Controls.Add(Me.TLibelleTitre)
            Me.GpFiltre.Controls.Add(Me.TIsinTitre)
            Me.GpFiltre.Controls.Add(Me.CbLotPortefeuille)
            Me.GpFiltre.Controls.Add(Me.Label3)
            Me.GpFiltre.Controls.Add(Me.CbSousSecteur)
            Me.GpFiltre.Controls.Add(Me.Label2)
            Me.GpFiltre.Controls.Add(Me.Label1)
            Me.GpFiltre.Controls.Add(Me.CbSecteur)
            resources.ApplyResources(Me.GpFiltre, "GpFiltre")
            Me.GpFiltre.Name = "GpFiltre"
            Me.GpFiltre.TabStop = False
            '
            'Tranche
            '
            resources.ApplyResources(Me.Tranche, "Tranche")
            Me.Tranche.Name = "Tranche"
            '
            'CbTrancheMaturite
            '
            Me.CbTrancheMaturite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbTrancheMaturite.FormattingEnabled = True
            resources.ApplyResources(Me.CbTrancheMaturite, "CbTrancheMaturite")
            Me.CbTrancheMaturite.Name = "CbTrancheMaturite"
            '
            'CbPays
            '
            Me.CbPays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbPays.FormattingEnabled = True
            resources.ApplyResources(Me.CbPays, "CbPays")
            Me.CbPays.Name = "CbPays"
            '
            'Label6
            '
            resources.ApplyResources(Me.Label6, "Label6")
            Me.Label6.Name = "Label6"
            '
            'Label5
            '
            resources.ApplyResources(Me.Label5, "Label5")
            Me.Label5.Name = "Label5"
            '
            'Label4
            '
            resources.ApplyResources(Me.Label4, "Label4")
            Me.Label4.Name = "Label4"
            '
            'TLibelleTitre
            '
            resources.ApplyResources(Me.TLibelleTitre, "TLibelleTitre")
            Me.TLibelleTitre.Name = "TLibelleTitre"
            '
            'TIsinTitre
            '
            resources.ApplyResources(Me.TIsinTitre, "TIsinTitre")
            Me.TIsinTitre.Name = "TIsinTitre"
            '
            'CbLotPortefeuille
            '
            Me.CbLotPortefeuille.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbLotPortefeuille.FormattingEnabled = True
            resources.ApplyResources(Me.CbLotPortefeuille, "CbLotPortefeuille")
            Me.CbLotPortefeuille.Name = "CbLotPortefeuille"
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'CbSousSecteur
            '
            Me.CbSousSecteur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbSousSecteur.FormattingEnabled = True
            resources.ApplyResources(Me.CbSousSecteur, "CbSousSecteur")
            Me.CbSousSecteur.Name = "CbSousSecteur"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'GbAggrégations
            '
            Me.GbAggrégations.Controls.Add(Me.RbTrancheMaturite)
            Me.GbAggrégations.Controls.Add(Me.RbRating)
            Me.GbAggrégations.Controls.Add(Me.RbGroupeEmetteur)
            Me.GbAggrégations.Controls.Add(Me.RbTypeDeDette)
            Me.GbAggrégations.Controls.Add(Me.RbTypeActif)
            Me.GbAggrégations.Controls.Add(Me.RbZoneGeographique)
            Me.GbAggrégations.Controls.Add(Me.RbPays)
            Me.GbAggrégations.Controls.Add(Me.RbSousSecteurs)
            Me.GbAggrégations.Controls.Add(Me.RbSecteurs)
            resources.ApplyResources(Me.GbAggrégations, "GbAggrégations")
            Me.GbAggrégations.Name = "GbAggrégations"
            Me.GbAggrégations.TabStop = False
            '
            'RbTrancheMaturite
            '
            resources.ApplyResources(Me.RbTrancheMaturite, "RbTrancheMaturite")
            Me.RbTrancheMaturite.Name = "RbTrancheMaturite"
            Me.RbTrancheMaturite.TabStop = True
            Me.RbTrancheMaturite.UseVisualStyleBackColor = True
            '
            'RbRating
            '
            resources.ApplyResources(Me.RbRating, "RbRating")
            Me.RbRating.Name = "RbRating"
            Me.RbRating.TabStop = True
            Me.RbRating.UseVisualStyleBackColor = True
            '
            'RbGroupeEmetteur
            '
            resources.ApplyResources(Me.RbGroupeEmetteur, "RbGroupeEmetteur")
            Me.RbGroupeEmetteur.Name = "RbGroupeEmetteur"
            Me.RbGroupeEmetteur.TabStop = True
            Me.RbGroupeEmetteur.UseVisualStyleBackColor = True
            '
            'RbTypeDeDette
            '
            resources.ApplyResources(Me.RbTypeDeDette, "RbTypeDeDette")
            Me.RbTypeDeDette.Name = "RbTypeDeDette"
            Me.RbTypeDeDette.TabStop = True
            Me.RbTypeDeDette.UseVisualStyleBackColor = True
            '
            'RbTypeActif
            '
            resources.ApplyResources(Me.RbTypeActif, "RbTypeActif")
            Me.RbTypeActif.Name = "RbTypeActif"
            Me.RbTypeActif.TabStop = True
            Me.RbTypeActif.UseVisualStyleBackColor = True
            '
            'RbZoneGeographique
            '
            resources.ApplyResources(Me.RbZoneGeographique, "RbZoneGeographique")
            Me.RbZoneGeographique.Name = "RbZoneGeographique"
            Me.RbZoneGeographique.TabStop = True
            Me.RbZoneGeographique.UseVisualStyleBackColor = True
            '
            'RbPays
            '
            resources.ApplyResources(Me.RbPays, "RbPays")
            Me.RbPays.Name = "RbPays"
            Me.RbPays.TabStop = True
            Me.RbPays.UseVisualStyleBackColor = True
            '
            'RbSousSecteurs
            '
            resources.ApplyResources(Me.RbSousSecteurs, "RbSousSecteurs")
            Me.RbSousSecteurs.Name = "RbSousSecteurs"
            Me.RbSousSecteurs.TabStop = True
            Me.RbSousSecteurs.UseVisualStyleBackColor = True
            '
            'RbSecteurs
            '
            resources.ApplyResources(Me.RbSecteurs, "RbSecteurs")
            Me.RbSecteurs.Name = "RbSecteurs"
            Me.RbSecteurs.TabStop = True
            Me.RbSecteurs.UseVisualStyleBackColor = True
            '
            'BExcel
            '
            resources.ApplyResources(Me.BExcel, "BExcel")
            Me.BExcel.Name = "BExcel"
            Me.BExcel.UseVisualStyleBackColor = True
            '
            'DataGridAggr
            '
            Me.DataGridAggr.AllowUserToAddRows = False
            Me.DataGridAggr.AllowUserToDeleteRows = False
            Me.DataGridAggr.AllowUserToOrderColumns = True
            resources.ApplyResources(Me.DataGridAggr, "DataGridAggr")
            Me.DataGridAggr.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            Me.DataGridAggr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.DataGridAggr.Name = "DataGridAggr"
            Me.DataGridAggr.ReadOnly = True
            '
            'MonthCalendar
            '
            resources.ApplyResources(Me.MonthCalendar, "MonthCalendar")
            Me.MonthCalendar.MaxSelectionCount = 1
            Me.MonthCalendar.Name = "MonthCalendar"
            '
            'myPrintDocument
            '
            '
            'BPrint
            '
            resources.ApplyResources(Me.BPrint, "BPrint")
            Me.BPrint.Name = "BPrint"
            Me.BPrint.UseVisualStyleBackColor = True
            '
            'SaveFileDialog
            '
            Me.SaveFileDialog.CheckPathExists = False
            Me.SaveFileDialog.DefaultExt = "csv"
            '
            'BRapport
            '
            resources.ApplyResources(Me.BRapport, "BRapport")
            Me.BRapport.Name = "BRapport"
            Me.BRapport.UseVisualStyleBackColor = True
            '
            'CbRapport
            '
            Me.CbRapport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbRapport.FormattingEnabled = True
            resources.ApplyResources(Me.CbRapport, "CbRapport")
            Me.CbRapport.Name = "CbRapport"
            '
            'Label7
            '
            resources.ApplyResources(Me.Label7, "Label7")
            Me.Label7.Name = "Label7"
            '
            'Transparence
            '
            Me.AcceptButton = Me.BCharger
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Label7)
            Me.Controls.Add(Me.CbRapport)
            Me.Controls.Add(Me.BRapport)
            Me.Controls.Add(Me.BPrint)
            Me.Controls.Add(Me.MonthCalendar)
            Me.Controls.Add(Me.DataGridAggr)
            Me.Controls.Add(Me.BExcel)
            Me.Controls.Add(Me.GbAggrégations)
            Me.Controls.Add(Me.GpFiltre)
            Me.Controls.Add(Me.BEffacer)
            Me.Controls.Add(Me.BCharger)
            Me.Controls.Add(Me.DataGridSelect)
            Me.Controls.Add(Me.GpNiveau)
            Me.Name = "Transparence"
            Me.GpNiveau.ResumeLayout(False)
            Me.GpNiveau.PerformLayout()
            CType(Me.DataGridSelect, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GpFiltre.ResumeLayout(False)
            Me.GpFiltre.PerformLayout()
            Me.GbAggrégations.ResumeLayout(False)
            Me.GbAggrégations.PerformLayout()
            CType(Me.DataGridAggr, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents GpNiveau As System.Windows.Forms.GroupBox
        Friend WithEvents DataGridSelect As System.Windows.Forms.DataGridView
        Friend WithEvents BCharger As System.Windows.Forms.Button
        Friend WithEvents CbSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents BEffacer As System.Windows.Forms.Button
        Friend WithEvents RbNiv2 As System.Windows.Forms.RadioButton
        Friend WithEvents RbNiv0 As System.Windows.Forms.RadioButton
        Friend WithEvents RbNiv1 As System.Windows.Forms.RadioButton
        Friend WithEvents GpFiltre As System.Windows.Forms.GroupBox
        Friend WithEvents CbSousSecteur As System.Windows.Forms.ComboBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents CbLotPortefeuille As System.Windows.Forms.ComboBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents TLibelleTitre As System.Windows.Forms.TextBox
        Friend WithEvents TIsinTitre As System.Windows.Forms.TextBox
        Friend WithEvents CbPays As System.Windows.Forms.ComboBox
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents GbAggrégations As System.Windows.Forms.GroupBox
        Friend WithEvents RbTrancheMaturite As System.Windows.Forms.RadioButton
        Friend WithEvents RbRating As System.Windows.Forms.RadioButton
        Friend WithEvents RbGroupeEmetteur As System.Windows.Forms.RadioButton
        Friend WithEvents RbTypeDeDette As System.Windows.Forms.RadioButton
        Friend WithEvents RbTypeActif As System.Windows.Forms.RadioButton
        Friend WithEvents RbZoneGeographique As System.Windows.Forms.RadioButton
        Friend WithEvents RbPays As System.Windows.Forms.RadioButton
        Friend WithEvents RbSousSecteurs As System.Windows.Forms.RadioButton
        Friend WithEvents RbSecteurs As System.Windows.Forms.RadioButton
        Friend WithEvents BExcel As System.Windows.Forms.Button
        Friend WithEvents Tranche As System.Windows.Forms.Label
        Friend WithEvents CbTrancheMaturite As System.Windows.Forms.ComboBox
        Friend WithEvents DataGridAggr As System.Windows.Forms.DataGridView
        Friend WithEvents MonthCalendar As System.Windows.Forms.MonthCalendar
        Friend WithEvents myPrintDocument As System.Drawing.Printing.PrintDocument
        Friend WithEvents BPrint As System.Windows.Forms.Button
        Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
        Friend WithEvents BRapport As System.Windows.Forms.Button
        Friend WithEvents CbRapport As System.Windows.Forms.ComboBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
    End Class
End Namespace
