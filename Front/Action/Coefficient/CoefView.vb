Namespace Action.Coefficient

    Public Class CoefView
        Inherits Action.Cell

        Private _coef As Decimal
        Private _coefViewModel As CoefViewModel

        Sub New(ByVal coef As Decimal, ByVal coefViewModel As CoefViewModel, ByVal sect As SecteurView, ByVal crit As CritereView)
            MyBase.New(coef)

            _coef = coef
            _coefViewModel = coefViewModel
            Secteur = sect
            Critere = crit
        End Sub

        Public Overrides Property Data As String
            Get
                Return _coef
            End Get
            Set(ByVal value As String)
                Dim newCoef As Decimal
                Decimal.TryParse(value, newCoef)

                If newCoef <> _coef Then
                    _coef = newCoef

                    If _coefViewModel.AutoCompute Then
                        _coefViewModel.updateCoefsBottomUp(Me)
                    End If
                    OnPropertyChanged("Data")
                End If
            End Set
        End Property

        Public Property Secteur As SecteurView

        Public Property Critere As CritereView

    End Class

End Namespace
