Public Class Verification

    Dim lo As Log = New Log()
    Dim co As Connection = New Connection()

    Private Sub Verification_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        co.ToConnectBase()

        'Mettre les dates en gras que l'on peut transpariser
        MonthCalendar.MaxDate = DateTime.Now
        For Each f In co.SelectDistinctSimple("PTF_TRANSPARISE", "dateinventaire")
            MonthCalendar.AddBoldedDate(f)
        Next

        MonthCalendar.UpdateBoldedDates()
    End Sub

    ''' <summary>
    ''' BCharger : remplir les datagrids
    ''' </summary>
    Private Sub BCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCharger.Click
        'Connaitre niveau
        Dim niv As Double
        If RbNiv1.Checked Then
            niv = 1
        Else
            niv = 2
        End If

        'Savoir si date existe dans la base
        If co.SelectWhere2("PTF_TRANSPARISE", "dateinventaire", "dateinventaire", MonthCalendar.SelectionRange.Start.Date, "numero_niveau", niv, 1).Count > 0 Then
            Dim name As List(Of String) = New List(Of String)
            Dim donnee As List(Of Object) = New List(Of Object)
            name.Add("@date")
            name.Add("@niveau")
            donnee.Add(MonthCalendar.SelectionRange.Start.Date)
            donnee.Add(niv)
            DataGridSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ControleSecteur", name, donnee))
            DataGridTypeActif.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("ControleTypeActif", name, donnee))
        End If
    End Sub

    Private Sub BEffacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BEffacer.Click
        DataGridSecteur.DataSource = Nothing
        DataGridTypeActif.DataSource = Nothing
    End Sub
End Class