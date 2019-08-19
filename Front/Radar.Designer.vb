<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Radar
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
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series3 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series4 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Radar))
        Me.GraphRadar = New System.Windows.Forms.DataVisualization.Charting.Chart()
        CType(Me.GraphRadar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GraphRadar
        '
        Me.GraphRadar.BackColor = System.Drawing.Color.Gainsboro
        Me.GraphRadar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.GraphRadar.BorderSkin.BackColor = System.Drawing.Color.Red
        ChartArea1.AxisX.LabelAutoFitStyle = CType(((((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) _
                    Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) _
                    Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) _
                    Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45) _
                    Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep90) _
                    Or System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap), System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)
        ChartArea1.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        ChartArea1.BorderColor = System.Drawing.Color.Red
        ChartArea1.Name = "ChartArea1"
        ChartArea1.Position.Auto = False
        ChartArea1.Position.Height = 100.0!
        ChartArea1.Position.Width = 100.0!
        Me.GraphRadar.ChartAreas.Add(ChartArea1)
        Me.GraphRadar.Dock = System.Windows.Forms.DockStyle.Fill
        Legend1.BorderColor = System.Drawing.Color.Gainsboro
        Legend1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot
        Legend1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Legend1.IsTextAutoFit = False
        Legend1.Name = "Legend1"
        Me.GraphRadar.Legends.Add(Legend1)
        Me.GraphRadar.Location = New System.Drawing.Point(0, 0)
        Me.GraphRadar.Name = "GraphRadar"
        Series1.BackImageTransparentColor = System.Drawing.Color.White
        Series1.BackSecondaryColor = System.Drawing.Color.White
        Series1.BorderColor = System.Drawing.Color.SaddleBrown
        Series1.BorderWidth = 4
        Series1.ChartArea = "ChartArea1"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Radar
        Series1.Color = System.Drawing.Color.White
        Series1.CustomProperties = "RadarDrawingStyle=Marker, AreaDrawingStyle=Polygon, CircularLabelsStyle=Horizonta" & _
            "l, LabelStyle=Left"
        Series1.Legend = "Legend1"
        Series1.MarkerBorderColor = System.Drawing.Color.SaddleBrown
        Series1.MarkerColor = System.Drawing.Color.SaddleBrown
        Series1.Name = "Indice"
        Series1.SmartLabelStyle.CalloutLineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot
        Series1.SmartLabelStyle.CalloutLineWidth = 6
        Series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.[String]
        Series1.YValueMembers = "1"
        Series2.BorderColor = System.Drawing.Color.Tan
        Series2.BorderWidth = 4
        Series2.ChartArea = "ChartArea1"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Radar
        Series2.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Series2.CustomProperties = "RadarDrawingStyle=Marker, AreaDrawingStyle=Polygon, CircularLabelsStyle=Horizonta" & _
            "l, LabelStyle=Left"
        Series2.Legend = "Legend1"
        Series2.MarkerBorderColor = System.Drawing.Color.Tan
        Series2.MarkerColor = System.Drawing.Color.Tan
        Series2.Name = "Secteur ICB"
        Series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.[String]
        Series2.YValueMembers = "1"
        Series3.BorderColor = System.Drawing.Color.Khaki
        Series3.BorderWidth = 4
        Series3.ChartArea = "ChartArea1"
        Series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Radar
        Series3.Color = System.Drawing.Color.Khaki
        Series3.CustomProperties = "RadarDrawingStyle=Marker, AreaDrawingStyle=Polygon, LabelStyle=Left"
        Series3.LabelBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Series3.Legend = "Legend1"
        Series3.MarkerColor = System.Drawing.Color.Khaki
        Series3.Name = "Secteur FGA"
        Series3.SmartLabelStyle.CalloutLineColor = System.Drawing.Color.Red
        Series3.SmartLabelStyle.CalloutLineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot
        Series3.SmartLabelStyle.CalloutStyle = System.Windows.Forms.DataVisualization.Charting.LabelCalloutStyle.Box
        Series3.XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary
        Series3.YValueMembers = "1"
        Series4.BorderColor = System.Drawing.Color.Lime
        Series4.BorderWidth = 4
        Series4.ChartArea = "ChartArea1"
        Series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Radar
        Series4.Color = System.Drawing.Color.Lime
        Series4.CustomProperties = "RadarDrawingStyle=Line"
        Series4.Legend = "Legend1"
        Series4.Name = "Valeur"
        Me.GraphRadar.Series.Add(Series1)
        Me.GraphRadar.Series.Add(Series2)
        Me.GraphRadar.Series.Add(Series3)
        Me.GraphRadar.Series.Add(Series4)
        Me.GraphRadar.Size = New System.Drawing.Size(789, 531)
        Me.GraphRadar.TabIndex = 3
        Me.GraphRadar.Text = "Chart1"
        Title1.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer))
        Title1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Title1.Name = "Title1"
        Title1.Position.Auto = False
        Title1.Position.Height = 3.530918!
        Title1.Position.Width = 20.0!
        Title1.Position.X = 3.0!
        Title1.Position.Y = 3.0!
        Title1.Text = "Radar"
        Me.GraphRadar.Titles.Add(Title1)
        '
        'Radar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 531)
        Me.Controls.Add(Me.GraphRadar)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Radar"
        Me.Text = "Radar"
        CType(Me.GraphRadar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GraphRadar As System.Windows.Forms.DataVisualization.Charting.Chart
End Class
