Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Action.Consultation
    Public Class GidConfigRow
        Implements INotifyPropertyChanged

        Private _criteres As New ObservableCollection(Of String)

        Property Ticker As String
        Property Indice As String
        Property SuperSecteur As String
        Property SuperSecteurId As Integer = 0
        Property Secteur As String
        Property SecteurId As Integer = 0
        Property Valeur As String
        Property Isin As String = ""
        Property Crncy As String
        Property SXXP As Double
        Property Price As Double
        Property MktCap As Double
        Property SalesCY As Double
        Property SRIEUROPE As String
        Property SRIEURO As String
        Property SRIExEURO As String
        Property Liquidity As String

        Property Criteres As ObservableCollection(Of String)
            Get
                Return _criteres
            End Get
            Set(ByVal value As ObservableCollection(Of String))
                _criteres = value
                OnPropertyChanged("Criteres")
            End Set
        End Property

#Region "Events"

        Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Sub OnPropertyChanged(ByVal propertyName As String)
            If Not String.IsNullOrEmpty(propertyName) Then
                RaiseEvent PropertyChanged(Me,
                          New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
#End Region ' !Events

    End Class
End Namespace

