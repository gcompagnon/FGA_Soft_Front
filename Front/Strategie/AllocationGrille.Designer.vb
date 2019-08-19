<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AllocationGrille
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
        Me.DG_Allocation = New System.Windows.Forms.DataGridView()
        Me.CalDebut = New System.Windows.Forms.MonthCalendar()
        Me.CalFin = New System.Windows.Forms.MonthCalendar()
        Me.lbl_DateDeb = New System.Windows.Forms.Label()
        Me.lbl_DateFin = New System.Windows.Forms.Label()
        Me.CalculPerf = New System.Windows.Forms.Button()
        Me.Cb_Groupe = New System.Windows.Forms.ComboBox()
        Me.Radio_Bloom = New System.Windows.Forms.RadioButton()
        Me.Radio_Omega = New System.Windows.Forms.RadioButton()
        CType(Me.DG_Allocation, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DG_Allocation
        '
        Me.DG_Allocation.AllowUserToAddRows = False
        Me.DG_Allocation.AllowUserToDeleteRows = False
        Me.DG_Allocation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DG_Allocation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DG_Allocation.Cursor = System.Windows.Forms.Cursors.Default
        Me.DG_Allocation.Location = New System.Drawing.Point(36, 287)
        Me.DG_Allocation.Name = "DG_Allocation"
        Me.DG_Allocation.ReadOnly = True
        Me.DG_Allocation.Size = New System.Drawing.Size(764, 465)
        Me.DG_Allocation.TabIndex = 0
        '
        'CalDebut
        '
        Me.CalDebut.Location = New System.Drawing.Point(123, 48)
        Me.CalDebut.Name = "CalDebut"
        Me.CalDebut.TabIndex = 1
        '
        'CalFin
        '
        Me.CalFin.Location = New System.Drawing.Point(439, 48)
        Me.CalFin.Name = "CalFin"
        Me.CalFin.TabIndex = 2
        '
        'lbl_DateDeb
        '
        Me.lbl_DateDeb.AutoSize = True
        Me.lbl_DateDeb.Location = New System.Drawing.Point(19, 48)
        Me.lbl_DateDeb.Name = "lbl_DateDeb"
        Me.lbl_DateDeb.Size = New System.Drawing.Size(75, 13)
        Me.lbl_DateDeb.TabIndex = 3
        Me.lbl_DateDeb.Text = "Date de début"
        '
        'lbl_DateFin
        '
        Me.lbl_DateFin.AutoSize = True
        Me.lbl_DateFin.Location = New System.Drawing.Point(332, 48)
        Me.lbl_DateFin.Name = "lbl_DateFin"
        Me.lbl_DateFin.Size = New System.Drawing.Size(59, 13)
        Me.lbl_DateFin.TabIndex = 4
        Me.lbl_DateFin.Text = "Date de fin"
        '
        'CalculPerf
        '
        Me.CalculPerf.Location = New System.Drawing.Point(674, 82)
        Me.CalculPerf.Name = "CalculPerf"
        Me.CalculPerf.Size = New System.Drawing.Size(153, 48)
        Me.CalculPerf.TabIndex = 5
        Me.CalculPerf.Text = "Lancer calcul"
        Me.CalculPerf.UseVisualStyleBackColor = True
        '
        'Cb_Groupe
        '
        Me.Cb_Groupe.FormattingEnabled = True
        Me.Cb_Groupe.Location = New System.Drawing.Point(123, 246)
        Me.Cb_Groupe.Name = "Cb_Groupe"
        Me.Cb_Groupe.Size = New System.Drawing.Size(205, 21)
        Me.Cb_Groupe.TabIndex = 6
        '
        'Radio_Bloom
        '
        Me.Radio_Bloom.AutoSize = True
        Me.Radio_Bloom.Location = New System.Drawing.Point(465, 225)
        Me.Radio_Bloom.Name = "Radio_Bloom"
        Me.Radio_Bloom.Size = New System.Drawing.Size(191, 17)
        Me.Radio_Bloom.TabIndex = 7
        Me.Radio_Bloom.TabStop = True
        Me.Radio_Bloom.Text = "Temps réel (BloomBerg nécessaire)"
        Me.Radio_Bloom.UseVisualStyleBackColor = True
        '
        'Radio_Omega
        '
        Me.Radio_Omega.AutoSize = True
        Me.Radio_Omega.Location = New System.Drawing.Point(465, 250)
        Me.Radio_Omega.Name = "Radio_Omega"
        Me.Radio_Omega.Size = New System.Drawing.Size(124, 17)
        Me.Radio_Omega.TabIndex = 8
        Me.Radio_Omega.TabStop = True
        Me.Radio_Omega.Text = "Cours cloture Omega"
        Me.Radio_Omega.UseVisualStyleBackColor = True
        '
        'AllocationGrille
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(857, 764)
        Me.Controls.Add(Me.Radio_Omega)
        Me.Controls.Add(Me.Radio_Bloom)
        Me.Controls.Add(Me.Cb_Groupe)
        Me.Controls.Add(Me.CalculPerf)
        Me.Controls.Add(Me.lbl_DateFin)
        Me.Controls.Add(Me.lbl_DateDeb)
        Me.Controls.Add(Me.CalFin)
        Me.Controls.Add(Me.CalDebut)
        Me.Controls.Add(Me.DG_Allocation)
        Me.Name = "AllocationGrille"
        Me.Text = "Resultat stratégie"
        CType(Me.DG_Allocation, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DG_Allocation As System.Windows.Forms.DataGridView
    Friend WithEvents CalDebut As System.Windows.Forms.MonthCalendar
    Friend WithEvents CalFin As System.Windows.Forms.MonthCalendar
    Friend WithEvents lbl_DateDeb As System.Windows.Forms.Label
    Friend WithEvents lbl_DateFin As System.Windows.Forms.Label
    Friend WithEvents CalculPerf As System.Windows.Forms.Button
    Friend WithEvents Cb_Groupe As System.Windows.Forms.ComboBox
    Friend WithEvents Radio_Bloom As System.Windows.Forms.RadioButton
    Friend WithEvents Radio_Omega As System.Windows.Forms.RadioButton
End Class
