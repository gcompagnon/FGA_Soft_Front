Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Action.Consultation
    Public Class GenRow
        Implements INotifyPropertyChanged

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

        Property propCollection As New PropertyCollection
        Property dico As New Dictionary(Of String, Object)

        Private _criteres As New ObservableCollection(Of String)

        Property Criteres As ObservableCollection(Of String)
            Get
                Return _criteres
            End Get
            Set(ByVal value As ObservableCollection(Of String))
                _criteres = value
                OnPropertyChanged("Criteres")
            End Set
        End Property

        ReadOnly Property IsUniverse As Boolean
            Get
                Return Me.SuperSecteurId = 0
            End Get
        End Property

        ReadOnly Property IsSuperSector As Boolean
            Get
                Return Me.SuperSecteurId <> 0 And Me.SecteurId = 0
            End Get
        End Property

        ReadOnly Property IsSector As Boolean
            Get
                Return Me.SecteurId <> 0 And Me.Isin = ""
            End Get
        End Property

        ReadOnly Property IsValeur As Boolean
            Get
                Return Me.Isin <> ""
            End Get
        End Property

        Sub New()
            'Dim Ticker As String = ""
            'Dim Indice As String = ""
            'Dim SuperSecteur As String = ""
            'Dim SuperSecteurId As Integer = 0
            'Dim Secteur As String = ""
            'Dim SecteurId As Integer = 0
            'Dim Valeur As String = ""
            'Dim Isin As String = ""
            'Dim Crncy As String = ""
            'Dim SXXP As String = ""
            'Dim Price As String = ""
            'Dim MktCap As String = ""
            'Dim SalesCY As String = ""
            'Dim SRIEUROPE As String = ""
            'Dim SRIEURO As String = ""
            'Dim SRIExEURO As String = ""
            'Dim Liquidity As String = ""

            dico.Add("Ticker", "")
            dico.Add("Indice", "")
            dico.Add("SuperSecteur", "")
            dico.Add("SuperSecteurId", 0)
            dico.Add("Secteur", "")
            propCollection.Add("TimeStamp", DateTime.Now)
            'propCollection.Add("Ticker", Ticker)
            'propCollection.Add("Indice", Indice)
            'propCollection.Add("SuperSecteur", SuperSecteur)
            'propCollection.Add("SuperSecteurId", 0)
            'propCollection.Add("Secteur", Secteur)
            'propCollection.Add("SecteurId", 0)
            'propCollection.Add("Valeur", Valeur)
            'propCollection.Add("Isin", Isin)
            'propCollection.Add("Crncy", Crncy)
            'propCollection.Add("SXXP", SXXP)
            'propCollection.Add("Price", Price)
            'propCollection.Add("MktCap", MktCap)
            'propCollection.Add("SalesCY", SalesCY)
            'propCollection.Add("SRIEUROPE", SRIEUROPE)
            'propCollection.Add("SRIEURO", SRIEURO)
            'propCollection.Add("Liquidity", Liquidity)
        End Sub

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
