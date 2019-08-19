Imports System.Windows

Namespace Action.Coefficient

    Public Class BaseActionCoefIndice

        Private _coefViewModel As New CoefViewModelIndice

        Public Sub New()
            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Me.DataContext = _coefViewModel
        End Sub

        Private Sub BCompute_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCompute.Click
            _coefViewModel.ComputeCoefs()
        End Sub

        Private Sub BApply_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BApply.Click
            _coefViewModel.saveToBDD()
            Me.Close()
        End Sub

        Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BCancel.Click
            Me.Close()
        End Sub

        Private Sub BDescription_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            If ColumnDescription.Visibility = System.Windows.Visibility.Visible Then
                ColumnDescription.Visibility = System.Windows.Visibility.Hidden
            Else
                ColumnDescription.Visibility = System.Windows.Visibility.Visible
            End If
        End Sub

        Private Sub BExportExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
            _coefViewModel.ExportToExcel()
        End Sub

    End Class

End Namespace
