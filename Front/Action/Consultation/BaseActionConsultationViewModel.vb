Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Data.SqlClient
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Globalization

Namespace Action.Consultation
    Public Class BaseActionConsultationViewModel
        Implements INotifyPropertyChanged

#Region "Initialisation"
        Public Shared ReadOnly ALLSUPERSECTORS As String = "**Sectors GICS**"
        Public Shared ReadOnly ALLSECTORS As String = "**Industries FGA**"

        Private co As New Connection
        Private _dates As New ObservableCollection(Of String)
        Private _univers As New ObservableCollection(Of String)
        Private _supersecteurs As New ObservableCollection(Of Secteur)
        Private _secteurs As New ObservableCollection(Of Secteur)
        Private _genRows As New ObservableCollection(Of GenRow)
        Private _DTCroissance As New DataTable("Croissance")
        Public SelectedUniverse As String = ""
        Public checkBoxSelectAll As Boolean = True
        Private _selectedCol As New ObservableCollection(Of String)
        Private _colValues As New ObservableCollection(Of ColumnFilter)

        Property bool As Boolean = True

        Property SelectedCol As ObservableCollection(Of String)
            Get
                Return _selectedCol
            End Get
            Set(ByVal value As ObservableCollection(Of String))
                _selectedCol = value
                OnPropertyChanged("SelectedCol")
            End Set
        End Property
        Property ColValues As ObservableCollection(Of ColumnFilter)
            Get
                Return _colValues
            End Get
            Set(ByVal value As ObservableCollection(Of ColumnFilter))
                _colValues = value
                OnPropertyChanged("ColValues")
            End Set
        End Property

        Property Dates As ObservableCollection(Of String)
            Get
                Return _dates
            End Get
            Set(ByVal value As ObservableCollection(Of String))
                _dates = value
                OnPropertyChanged("Dates")
            End Set
        End Property

        Property Univers As ObservableCollection(Of String)
            Get
                Return _univers
            End Get
            Set(ByVal value As ObservableCollection(Of String))
                _univers = value
                OnPropertyChanged("Univers")
            End Set
        End Property

        Property SuperSecteurs As ObservableCollection(Of Secteur)
            Get
                Return _supersecteurs
            End Get
            Set(ByVal value As ObservableCollection(Of Secteur))
                _supersecteurs = value
                OnPropertyChanged("SuperSecteurs")
            End Set
        End Property

        Property Secteurs As ObservableCollection(Of Secteur)
            Get
                Return _secteurs
            End Get
            Set(ByVal value As ObservableCollection(Of Secteur))
                _secteurs = value
                OnPropertyChanged("Secteurs")
            End Set
        End Property

        'Property Valeurs As ObservableCollection(Of String)
        '    Get
        '        Return _valeurs
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of String))
        '        _valeurs = value
        '        OnPropertyChanged("Valeurs")
        '    End Set
        'End Property

        Property GenRows As ObservableCollection(Of GenRow)
            Get
                Return _genRows
            End Get
            Set(ByVal value As ObservableCollection(Of GenRow))
                _genRows = value
                OnPropertyChanged("GenRows")
            End Set
        End Property

        Property DTCroissance As DataTable
            Get
                Return _DTCroissance
            End Get
            Set(ByVal value As DataTable)
                _DTCroissance = value
                OnPropertyChanged("DTCroissance")
            End Set
        End Property
#End Region ' !Initialisation

#Region "Onglet General Rows"
        Private _genTable As DataTable
        Private GenAgr As New StringBuilder
        Private criteres As String = ""

        Public Property GenTable() As DataTable
            Get
                Return _genTable
            End Get
            Set(ByVal value As DataTable)
                _genTable = value
                OnPropertyChanged("GenTable")
                OnPropertyChanged("GenTableView")
            End Set
        End Property

        ReadOnly Property GenTableView As DataView
            Get
                Return If(Me.GenTable IsNot Nothing, Me.GenTable.DefaultView, Nothing)
            End Get
        End Property

        ReadOnly Property IsUniverse(ByVal row As DataRow) As Boolean
            Get
                Return row("SuperSecteurId") = 0
            End Get
        End Property

        ReadOnly Property IsSuperSector As Boolean '(ByVal row As DataRow) As Boolean
            Get
                'Return row("SuperSecteurId") <> 0 And row("SecteurId") = 0
                Return True
            End Get
        End Property

        ReadOnly Property IsSector(ByVal row As DataRow) As Boolean
            Get
                Return row("SecteurId") <> 0 And row("Isin") = ""
            End Get
        End Property

        ReadOnly Property IsValeur(ByVal row As DataRow) As Boolean
            Get
                Return row("Isin") <> ""
            End Get
        End Property

        Public Sub FillGenTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.GenTable = New DataTable()

            Me.GenTable.Columns.Add("TICKER")
            Me.GenTable.Columns.Add("COMPANY_NAME")
            Me.GenTable.Columns.Add("SECTOR")
            Me.GenTable.Columns.Add("SuperSecteurId")
            Me.GenTable.Columns.Add("INDUSTRY")
            Me.GenTable.Columns.Add("SecteurId")
            Me.GenTable.Columns.Add("Isin")
            Me.GenTable.Columns.Add("SUIVI")
            Me.GenTable.Columns.Add("COUNTRY")
            Me.GenTable.Columns.Add("CURRENCY")
            Me.GenTable.Columns.Add("PRICE")
            Me.GenTable.Columns.Add("PRICE_EUR")
            If SelectedUniverse = "ALL" Then
                Me.GenTable.Columns.Add("MXEU")
                Me.GenTable.Columns.Add("MXUSLC")
            ElseIf SelectedUniverse = "EUROPE" Then
                Me.GenTable.Columns.Add("MXEU")
            ElseIf SelectedUniverse = "USA" Then
                Me.GenTable.Columns.Add("MXUSLC")
            ElseIf SelectedUniverse = "EMU" Then
                Me.GenTable.Columns.Add("MXEM")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                Me.GenTable.Columns.Add("MXEUM")
            ElseIf SelectedUniverse = "FRANCE" Then
                Me.GenTable.Columns.Add("MXFR")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("6100001")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                Me.GenTable.Columns.Add("MXFR")
                Me.GenTable.Columns.Add("6100002")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("6100004")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                Me.GenTable.Columns.Add("MXUSLC")
                Me.GenTable.Columns.Add("6100024")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                Me.GenTable.Columns.Add("MXEU")
                Me.GenTable.Columns.Add("6100026")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("6100030")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("6100033")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                Me.GenTable.Columns.Add("MXEUM")
                Me.GenTable.Columns.Add("6100062")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("6100063")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("AVEURO")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                Me.GenTable.Columns.Add("MXEM")
                Me.GenTable.Columns.Add("AVEUROPE")
            End If
            Me.GenTable.Columns.Add("MARKET_CAP_EUR")
            Me.GenTable.Columns.Add("EV_NTM_EUR")
            Me.GenTable.Columns.Add("SALES_NTM_EUR")
            Me.GenTable.Columns.Add("ESG")
            Me.GenTable.Columns.Add("LIQUIDITY_TEST")
            Me.GenTable.Columns.Add("LIQUIDITY")

            Me.GenTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.GenAgr.Clear()
            Me.GenAgr.Append(" fac.COUNTRY,")
            Me.GenAgr.Append(" fac.CURRENCY,")
            Me.GenAgr.Append(" fac.PRICE,")
            Me.GenAgr.Append(" fac.PRICE_EUR,")
            If SelectedUniverse = "ALL" Then
                Me.GenAgr.Append(" fac.MXEU,")
                Me.GenAgr.Append(" fac.MXUSLC,")
            ElseIf SelectedUniverse = "EUROPE" Then
                Me.GenAgr.Append(" fac.MXEU,")
            ElseIf SelectedUniverse = "USA" Then
                Me.GenAgr.Append(" fac.MXUSLC,")
            ElseIf SelectedUniverse = "EMU" Then
                Me.GenAgr.Append(" fac.MXEM,")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                Me.GenAgr.Append(" fac.MXEUM,")
            ElseIf SelectedUniverse = "FRANCE" Then
                Me.GenAgr.Append(" fac.MXFR,")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.[6100001],")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                Me.GenAgr.Append(" fac.MXFR,")
                Me.GenAgr.Append(" fac.[6100002],")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.[6100004],")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                Me.GenAgr.Append(" fac.MXUSLC,")
                Me.GenAgr.Append(" fac.[6100024],")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                Me.GenAgr.Append(" fac.MXEU,")
                Me.GenAgr.Append(" fac.[6100026],")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.[6100030],")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.[6100033],")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                Me.GenAgr.Append(" fac.MXEUM,")
                Me.GenAgr.Append(" fac.[6100062],")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.[6100063],")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.AVEURO,")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                Me.GenAgr.Append(" fac.MXEM,")
                Me.GenAgr.Append(" fac.AVEUROPE,")
            End If
            Me.GenAgr.Append(" fac.MARKET_CAP_EUR,")
            Me.GenAgr.Append(" fac.EV_NTM_EUR,")
            Me.GenAgr.Append(" fac.SALES_NTM_EUR,")
            Me.GenAgr.Append(" fac.ESG,")
            Me.GenAgr.Append(" fac.LIQUIDITY_TEST,")
            Me.GenAgr.Append(" fac.LIQUIDITY ")
        End Sub

#End Region ' !Onglet General Rows

#Region "Onglet Croissance Rows"
        Private _croTable As DataTable
        Private CroAgr As New StringBuilder
        Public Property CroTable() As DataTable
            Get
                Return _croTable
            End Get
            Set(ByVal value As DataTable)
                _croTable = value
                OnPropertyChanged("CroTable")
                OnPropertyChanged("CroTableView")
            End Set
        End Property

        ReadOnly Property CroTableView As DataView
            Get
                Return If(Me.CroTable IsNot Nothing, Me.CroTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillCroTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.CroTable = New DataTable()

            Me.CroTable.Columns.Add("Ticker")
            Me.CroTable.Columns.Add("COMPANY_NAME")
            Me.CroTable.Columns.Add("SECTOR")
            Me.CroTable.Columns.Add("SuperSecteurId")
            Me.CroTable.Columns.Add("INDUSTRY")
            Me.CroTable.Columns.Add("SecteurId")
            Me.CroTable.Columns.Add("Isin")
            Me.CroTable.Columns.Add("SUIVI")
            Me.CroTable.Columns.Add("COUNTRY")
            Me.CroTable.Columns.Add("CURRENCY")

            Me.CroTable.Columns.Add("IGROWTH_NTM")
            Me.CroTable.Columns.Add("EPS_CHG_NTM")
            Me.CroTable.Columns.Add("EPS_CHG_STM")
            Me.CroTable.Columns.Add("EBIT_CHG_NTM")
            Me.CroTable.Columns.Add("EBIT_CHG_STM")
            Me.CroTable.Columns.Add("EBIT_MARGIN_DIFF_NTM")
            Me.CroTable.Columns.Add("EBIT_MARGIN_DIFF_STM")
            Me.CroTable.Columns.Add("SALES_CHG_NTM")
            Me.CroTable.Columns.Add("SALES_CHG_STM")
            Me.CroTable.Columns.Add("CAPEX_CHG_NTM")
            Me.CroTable.Columns.Add("CAPEX_CHG_STM")
            Me.CroTable.Columns.Add("FCF_CHG_NTM")
            Me.CroTable.Columns.Add("FCF_CHG_STM")

            Me.CroTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.CroAgr.Clear()
            Me.CroAgr.Append(" fac.COUNTRY,")
            Me.CroAgr.Append(" fac.CURRENCY,")
            Me.CroAgr.Append(" fac.IGROWTH_NTM, ")
            Me.CroAgr.Append(" fac.EPS_CHG_NTM, ")
            Me.CroAgr.Append(" fac.EPS_CHG_STM, ")
            Me.CroAgr.Append(" fac.EBIT_CHG_NTM, ")
            Me.CroAgr.Append(" fac.EBIT_CHG_STM, ")
            Me.CroAgr.Append(" fac.EBIT_MARGIN_DIFF_NTM, ")
            Me.CroAgr.Append(" fac.EBIT_MARGIN_DIFF_STM, ")
            Me.CroAgr.Append(" fac.SALES_CHG_NTM, ")
            Me.CroAgr.Append(" fac.SALES_CHG_STM, ")
            Me.CroAgr.Append(" fac.CAPEX_CHG_NTM, ")
            Me.CroAgr.Append(" fac.CAPEX_CHG_STM, ")
            Me.CroAgr.Append(" fac.FCF_CHG_NTM, ")
            Me.CroAgr.Append(" fac.FCF_CHG_STM ")
        End Sub

#End Region ' !Onglet Croissance Rows

#Region "Onglet Qualite Rows"
        Private _quaTable As DataTable
        Private QuaAgr As New StringBuilder
        Public Property QuaTable() As DataTable
            Get
                Return _quaTable
            End Get
            Set(ByVal value As DataTable)
                _quaTable = value
                OnPropertyChanged("QuaTable")
                OnPropertyChanged("QuaTableView")
            End Set
        End Property

        ReadOnly Property QuaTableView As DataView
            Get
                Return If(Me.QuaTable IsNot Nothing, Me.QuaTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillQuaTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.QuaTable = New DataTable()

            Me.QuaTable.Columns.Add("Ticker")
            Me.QuaTable.Columns.Add("COMPANY_NAME")
            Me.QuaTable.Columns.Add("SECTOR")
            Me.QuaTable.Columns.Add("SuperSecteurId")
            Me.QuaTable.Columns.Add("INDUSTRY")
            Me.QuaTable.Columns.Add("SecteurId")
            Me.QuaTable.Columns.Add("Isin")
            Me.QuaTable.Columns.Add("SUIVI")
            Me.QuaTable.Columns.Add("COUNTRY")
            Me.QuaTable.Columns.Add("CURRENCY")

            Me.QuaTable.Columns.Add("EBIT_MARGIN_LTM")
            Me.QuaTable.Columns.Add("EBIT_MARGIN_NTM")
            Me.QuaTable.Columns.Add("EBIT_MARGIN_STM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.QuaTable.Columns.Add("PBT_RWA_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_RWA_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_RWA_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_RWA_DIFF_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_RWA_DIFF_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_SALES_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_SALES_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_SALES_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_SALES_DIFF_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("PBT_SALES_DIFF_STM") '----FINANCIAL
                End If
            End If
            Me.QuaTable.Columns.Add("ROE_LTM")
            Me.QuaTable.Columns.Add("ROE_NTM")
            Me.QuaTable.Columns.Add("ROE_STM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.QuaTable.Columns.Add("ROTE_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("ROTE_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("ROTE_STM") '----FINANCIAL
                Else

                    Me.QuaTable.Columns.Add("FCF_YLD_LTM")
                    Me.QuaTable.Columns.Add("FCF_YLD_NTM")
                    Me.QuaTable.Columns.Add("FCF_YLD_STM")
                    Me.QuaTable.Columns.Add("FCF_YLD_DIFF_NTM")
                    Me.QuaTable.Columns.Add("FCF_YLD_DIFF_STM")
                End If
            Else
                Me.QuaTable.Columns.Add("FCF_YLD_LTM")
                Me.QuaTable.Columns.Add("FCF_YLD_NTM")
                Me.QuaTable.Columns.Add("FCF_YLD_STM")
                Me.QuaTable.Columns.Add("FCF_YLD_DIFF_NTM")
                Me.QuaTable.Columns.Add("FCF_YLD_DIFF_STM")
            End If
            Me.QuaTable.Columns.Add("PAYOUT_LTM")
            Me.QuaTable.Columns.Add("PAYOUT_NTM")
            Me.QuaTable.Columns.Add("PAYOUT_STM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.QuaTable.Columns.Add("COST_INCOME_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("COST_INCOME_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("COST_INCOME_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("TIER_1_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("TIER_1_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("TIER_1_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("RORWA_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("RORWA_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("RORWA_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("COMBINED_RATIO_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("COMBINED_RATIO_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("COMBINED_RATIO_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("LOSS_RATIO_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("LOSS_RATIO_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("LOSS_RATIO_STM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("EXPENSE_RATIO_LTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("EXPENSE_RATIO_NTM") '----FINANCIAL
                    Me.QuaTable.Columns.Add("EXPENSE_RATIO_STM") '----FINANCIAL
                End If
            End If
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle <> "Financials" Then
                    Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_LTM")
                    Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_NTM")
                    Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_STM")
                    Me.QuaTable.Columns.Add("GEARING_LTM")
                    Me.QuaTable.Columns.Add("GEARING_NTM")
                    Me.QuaTable.Columns.Add("GEARING_STM")
                    Me.QuaTable.Columns.Add("CAPEX_SALES_LTM")
                    Me.QuaTable.Columns.Add("CAPEX_SALES_NTM")
                    Me.QuaTable.Columns.Add("CAPEX_SALES_STM")
                End If
            Else
                Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_LTM")
                Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_NTM")
                Me.QuaTable.Columns.Add("NET_DEBT_EBITDA_STM")
                Me.QuaTable.Columns.Add("GEARING_LTM")
                Me.QuaTable.Columns.Add("GEARING_NTM")
                Me.QuaTable.Columns.Add("GEARING_STM")
                Me.QuaTable.Columns.Add("CAPEX_SALES_LTM")
                Me.QuaTable.Columns.Add("CAPEX_SALES_NTM")
                Me.QuaTable.Columns.Add("CAPEX_SALES_STM")
            End If

            Me.QuaTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.QuaAgr.Clear()
            If superSecteur IsNot Nothing Then
                Me.QuaAgr.Append(" fac.COUNTRY,")
                Me.QuaAgr.Append(" fac.CURRENCY,")
                Me.QuaAgr.Append(" fac.EBIT_MARGIN_LTM,")
                Me.QuaAgr.Append(" fac.EBIT_MARGIN_NTM,")
                Me.QuaAgr.Append(" fac.EBIT_MARGIN_STM,")
                If superSecteur.Libelle = "Financials" Then
                    Me.QuaAgr.Append(" fac.PBT_RWA_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_RWA_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_RWA_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_RWA_DIFF_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_RWA_DIFF_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_SALES_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_SALES_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_SALES_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_SALES_DIFF_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.PBT_SALES_DIFF_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.ROTE_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.ROTE_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.ROTE_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COST_INCOME_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COST_INCOME_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COST_INCOME_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.TIER_1_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.TIER_1_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.TIER_1_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.RORWA_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.RORWA_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.RORWA_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COMBINED_RATIO_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COMBINED_RATIO_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.COMBINED_RATIO_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.LOSS_RATIO_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.LOSS_RATIO_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.LOSS_RATIO_STM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.EXPENSE_RATIO_LTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.EXPENSE_RATIO_NTM,") '----FINANCIAL
                    Me.QuaAgr.Append(" fac.EXPENSE_RATIO_STM,") '----FINANCIAL
                End If
            End If
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle <> "Financials" Then
                    Me.QuaAgr.Append(" fac.FCF_YLD_LTM,")
                    Me.QuaAgr.Append(" fac.FCF_YLD_NTM,")
                    Me.QuaAgr.Append(" fac.FCF_YLD_STM,")
                    Me.QuaAgr.Append(" fac.FCF_YLD_DIFF_NTM,")
                    Me.QuaAgr.Append(" fac.FCF_YLD_DIFF_STM,")
                    Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_LTM,")
                    Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_NTM,")
                    Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_STM,")
                    Me.QuaAgr.Append(" fac.GEARING_LTM,")
                    Me.QuaAgr.Append(" fac.GEARING_NTM,")
                    Me.QuaAgr.Append(" fac.GEARING_STM,")
                    Me.QuaAgr.Append(" fac.CAPEX_SALES_LTM,")
                    Me.QuaAgr.Append(" fac.CAPEX_SALES_NTM,")
                    Me.QuaAgr.Append(" fac.CAPEX_SALES_STM,")
                End If
            Else
                Me.QuaAgr.Append(" fac.FCF_YLD_LTM,")
                Me.QuaAgr.Append(" fac.FCF_YLD_NTM,")
                Me.QuaAgr.Append(" fac.FCF_YLD_STM,")
                Me.QuaAgr.Append(" fac.FCF_YLD_DIFF_NTM,")
                Me.QuaAgr.Append(" fac.FCF_YLD_DIFF_STM,")
                Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_LTM,")
                Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_NTM,")
                Me.QuaAgr.Append(" fac.NET_DEBT_EBITDA_STM,")
                Me.QuaAgr.Append(" fac.GEARING_LTM,")
                Me.QuaAgr.Append(" fac.GEARING_NTM,")
                Me.QuaAgr.Append(" fac.GEARING_STM,")
                Me.QuaAgr.Append(" fac.CAPEX_SALES_LTM,")
                Me.QuaAgr.Append(" fac.CAPEX_SALES_NTM,")
                Me.QuaAgr.Append(" fac.CAPEX_SALES_STM,")
            End If
            Me.QuaAgr.Append(" fac.ROE_LTM,")
            Me.QuaAgr.Append(" fac.ROE_NTM,")
            Me.QuaAgr.Append(" fac.ROE_STM,")
            Me.QuaAgr.Append(" fac.PAYOUT_LTM,")
            Me.QuaAgr.Append(" fac.PAYOUT_NTM,")
            Me.QuaAgr.Append(" fac.PAYOUT_STM ")
        End Sub

#End Region ' !Onglet Qualite Rows

#Region "Onglet Valorisation Rows"
        Private _valTable As DataTable
        Private ValAgr As New StringBuilder
        Public Property ValTable() As DataTable
            Get
                Return _valTable
            End Get
            Set(ByVal value As DataTable)
                _valTable = value
                OnPropertyChanged("ValTable")
                OnPropertyChanged("ValTableView")
            End Set
        End Property

        ReadOnly Property ValTableView As DataView
            Get
                Return If(Me.ValTable IsNot Nothing, Me.ValTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillValTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.ValTable = New DataTable()

            Me.ValTable.Columns.Add("Ticker")
            Me.ValTable.Columns.Add("COMPANY_NAME")
            Me.ValTable.Columns.Add("SECTOR")
            Me.ValTable.Columns.Add("SuperSecteurId")
            Me.ValTable.Columns.Add("INDUSTRY")
            Me.ValTable.Columns.Add("SecteurId")
            Me.ValTable.Columns.Add("Isin")
            Me.ValTable.Columns.Add("SUIVI")
            Me.ValTable.Columns.Add("COUNTRY")
            Me.ValTable.Columns.Add("CURRENCY")

            Me.ValTable.Columns.Add("DIV_YLD_NTM")
            Me.ValTable.Columns.Add("PE_LTM")
            Me.ValTable.Columns.Add("PE_NTM")
            Me.ValTable.Columns.Add("PE_STM")
            Me.ValTable.Columns.Add("PB_LTM")
            Me.ValTable.Columns.Add("PB_NTM")
            Me.ValTable.Columns.Add("PB_STM")
            Me.ValTable.Columns.Add("EV_EBIT_LTM")
            Me.ValTable.Columns.Add("EV_EBIT_NTM")
            Me.ValTable.Columns.Add("EV_EBIT_STM")
            Me.ValTable.Columns.Add("EV_EBITA_LTM")
            Me.ValTable.Columns.Add("EV_EBITA_NTM")
            Me.ValTable.Columns.Add("EV_EBITA_STM")
            Me.ValTable.Columns.Add("EV_EBITDA_LTM")
            Me.ValTable.Columns.Add("EV_EBITDA_NTM")
            Me.ValTable.Columns.Add("EV_EBITDA_STM")
            Me.ValTable.Columns.Add("EV_SALES_LTM")
            Me.ValTable.Columns.Add("EV_SALES_NTM")
            Me.ValTable.Columns.Add("EV_SALES_STM")
            Me.ValTable.Columns.Add("P_TBV_LTM")
            Me.ValTable.Columns.Add("P_TBV_NTM")
            Me.ValTable.Columns.Add("P_TBV_STM")
            Me.ValTable.Columns.Add("P_EMB_VALUE_LTM")
            Me.ValTable.Columns.Add("P_EMB_VALUE_NTM")
            Me.ValTable.Columns.Add("P_EMB_VALUE_STM")
            Me.ValTable.Columns.Add("PEG_NTM")
            Me.ValTable.Columns.Add("EV_EBIT_TO_G_NTM")
            Me.ValTable.Columns.Add("EV_EBITA_TO_G_NTM")
            Me.ValTable.Columns.Add("EV_EBITDA_TO_G_NTM")

            Me.ValTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.ValAgr.Clear()
            Me.ValAgr.Append(" fac.COUNTRY,")
            Me.ValAgr.Append(" fac.CURRENCY,")
            Me.ValAgr.Append(" fac.DIV_YLD_NTM,")
            Me.ValAgr.Append(" fac.PE_LTM,")
            Me.ValAgr.Append(" fac.PE_NTM,")
            Me.ValAgr.Append(" fac.PE_STM,")
            Me.ValAgr.Append(" fac.PB_LTM,")
            Me.ValAgr.Append(" fac.P_TBV_NTM,")
            Me.ValAgr.Append(" fac.PB_NTM,")
            Me.ValAgr.Append(" fac.PB_STM,")
            Me.ValAgr.Append(" fac.EV_EBIT_LTM,")
            Me.ValAgr.Append(" fac.EV_EBIT_NTM,")
            Me.ValAgr.Append(" fac.EV_EBIT_STM,")
            Me.ValAgr.Append(" fac.EV_EBITA_LTM,")
            Me.ValAgr.Append(" fac.EV_EBITA_NTM,")
            Me.ValAgr.Append(" fac.EV_EBITA_STM,")
            Me.ValAgr.Append(" fac.EV_EBITDA_LTM,")
            Me.ValAgr.Append(" fac.EV_EBITDA_NTM,")
            Me.ValAgr.Append(" fac.EV_EBITDA_STM,")
            Me.ValAgr.Append(" fac.EV_SALES_LTM,")
            Me.ValAgr.Append(" fac.EV_SALES_NTM,")
            Me.ValAgr.Append(" fac.EV_SALES_STM,")
            Me.ValAgr.Append(" fac.P_TBV_LTM,")
            Me.ValAgr.Append(" fac.P_TBV_STM,")
            Me.ValAgr.Append(" fac.P_EMB_VALUE_LTM,")
            Me.ValAgr.Append(" fac.P_EMB_VALUE_NTM,")
            Me.ValAgr.Append(" fac.P_EMB_VALUE_STM,")
            Me.ValAgr.Append(" fac.PEG_NTM,")
            Me.ValAgr.Append(" fac.EV_EBIT_TO_G_NTM,")
            Me.ValAgr.Append(" fac.EV_EBITA_TO_G_NTM,")
            Me.ValAgr.Append(" fac.EV_EBITDA_TO_G_NTM ")
        End Sub

#End Region ' !Onglet Valorisation Rows

#Region "Onglet Momentum Rows"
        Private _momTable As DataTable
        Private MomAgr As New StringBuilder
        Public Property MomTable() As DataTable
            Get
                Return _momTable
            End Get
            Set(ByVal value As DataTable)
                _momTable = value
                OnPropertyChanged("MomTable")
                OnPropertyChanged("MomTableView")
            End Set
        End Property

        ReadOnly Property MomTableView As DataView
            Get
                Return If(Me.MomTable IsNot Nothing, Me.MomTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillMomTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.MomTable = New DataTable()

            Me.MomTable.Columns.Add("Ticker")
            Me.MomTable.Columns.Add("COMPANY_NAME")
            Me.MomTable.Columns.Add("SECTOR")
            Me.MomTable.Columns.Add("SuperSecteurId")
            Me.MomTable.Columns.Add("INDUSTRY")
            Me.MomTable.Columns.Add("SecteurId")
            Me.MomTable.Columns.Add("Isin")
            Me.MomTable.Columns.Add("SUIVI")
            Me.MomTable.Columns.Add("CURRENCY")

            Me.MomTable.Columns.Add("PERF_MTD")
            Me.MomTable.Columns.Add("PERF_MTD_EUR")
            Me.MomTable.Columns.Add("PERF_YTD")
            Me.MomTable.Columns.Add("PERF_YTD_EUR")
            Me.MomTable.Columns.Add("PERF_1M")
            Me.MomTable.Columns.Add("PERF_1M_EUR")
            Me.MomTable.Columns.Add("PERF_1YR")
            Me.MomTable.Columns.Add("PERF_1YR_EUR")
            Me.MomTable.Columns.Add("EPS_CHG_1M")
            Me.MomTable.Columns.Add("EPS_CHG_3M")
            Me.MomTable.Columns.Add("EPS_CHG_1YR")
            Me.MomTable.Columns.Add("EPS_CHG_YTD")
            Me.MomTable.Columns.Add("BETA_1YR")
            Me.MomTable.Columns.Add("EPS_BROKER_UP_REV")
            Me.MomTable.Columns.Add("PRICE_BROKER_UP_REV")
            Me.MomTable.Columns.Add("PRICE")
            Me.MomTable.Columns.Add("TARGET")
            Me.MomTable.Columns.Add("UPSIDE")
            Me.MomTable.Columns.Add("RATING_POS_PCT")
            Me.MomTable.Columns.Add("RATING_TOT")
            Me.MomTable.Columns.Add("PRICE_52_HIGH")
            Me.MomTable.Columns.Add("PRICE_52_LOW")
            Me.MomTable.Columns.Add("VOL_1M")
            Me.MomTable.Columns.Add("VOL_3M")
            Me.MomTable.Columns.Add("VOL_1YR")
            Me.MomTable.Columns.Add("PRICE_PCTIL_1M")
            Me.MomTable.Columns.Add("PRICE_PCTIL_1YR")
            Me.MomTable.Columns.Add("PRICE_PCTIL_5YR")
            Me.MomTable.Columns.Add("COUNTRY")

            Me.MomTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.MomAgr.Clear()
            Me.MomAgr.Append(" fac.CURRENCY,")
            Me.MomAgr.Append(" fac.PERF_MTD,")
            'Me.MomAgr.Append(" fac.PERF_MTD_EUR,")
            Me.MomAgr.Append(" fac.PERF_YTD,")
            'Me.MomAgr.Append(" fac.PERF_YTD_EUR,")
            Me.MomAgr.Append(" fac.PERF_1M,")
            'Me.MomAgr.Append(" fac.PERF_1M_EUR,")
            Me.MomAgr.Append(" fac.PERF_1YR,")
            'Me.MomAgr.Append(" fac.PERF_1YR_EUR,")
            Me.MomAgr.Append(" fac.EPS_CHG_1M,")
            Me.MomAgr.Append(" fac.EPS_CHG_3M,")
            Me.MomAgr.Append(" fac.EPS_CHG_1YR,")
            Me.MomAgr.Append(" fac.EPS_CHG_YTD,")
            Me.MomAgr.Append(" fac.BETA_1YR,")
            Me.MomAgr.Append(" fac.EPS_BROKER_UP_REV,")
            Me.MomAgr.Append(" fac.PRICE_BROKER_UP_REV,")
            Me.MomAgr.Append(" fac.PRICE,")
            Me.MomAgr.Append(" fac.TARGET,")
            Me.MomAgr.Append(" fac.UPSIDE,")
            Me.MomAgr.Append(" fac.RATING_POS_PCT,")
            Me.MomAgr.Append(" fac.RATING_TOT,")
            Me.MomAgr.Append(" fac.PRICE_52_HIGH,")
            Me.MomAgr.Append(" fac.PRICE_52_LOW,")
            Me.MomAgr.Append(" fac.VOL_1M,")
            Me.MomAgr.Append(" fac.VOL_3M,")
            Me.MomAgr.Append(" fac.VOL_1YR,")
            Me.MomAgr.Append(" fac.PRICE_PCTIL_1M,")
            Me.MomAgr.Append(" fac.PRICE_PCTIL_1YR,")
            Me.MomAgr.Append(" fac.PRICE_PCTIL_5YR,")
            Me.MomAgr.Append(" fac.COUNTRY ")
        End Sub

#End Region ' !Onglet Momentum Rows

#Region "Onglet Synthese Rows"
        Private _synTable As DataTable
        Private SynAgr As New StringBuilder
        Public Property SynTable() As DataTable
            Get
                Return _synTable
            End Get
            Set(ByVal value As DataTable)
                _synTable = value
                OnPropertyChanged("SynTable")
                OnPropertyChanged("SynTableView")
            End Set
        End Property

        ReadOnly Property SynTableView As DataView
            Get
                Return If(Me.SynTable IsNot Nothing, Me.SynTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillSynTable(ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            Me.SynTable = New DataTable()

            Me.SynTable.Columns.Add("Ticker")
            Me.SynTable.Columns.Add("COMPANY_NAME")
            Me.SynTable.Columns.Add("SECTOR")
            Me.SynTable.Columns.Add("SuperSecteurId")
            Me.SynTable.Columns.Add("INDUSTRY")
            Me.SynTable.Columns.Add("SecteurId")
            Me.SynTable.Columns.Add("Isin")
            Me.SynTable.Columns.Add("SUIVI")
            Me.SynTable.Columns.Add("COUNTRY")
            Me.SynTable.Columns.Add("CURRENCY")


            If SelectedUniverse = "ALL" Then
                Me.SynTable.Columns.Add("MXEU")
                Me.SynTable.Columns.Add("MXUSLC")
            ElseIf SelectedUniverse = "EUROPE" Then
                Me.SynTable.Columns.Add("MXEU")
            ElseIf SelectedUniverse = "USA" Then
                Me.SynTable.Columns.Add("MXUSLC")
            ElseIf SelectedUniverse = "EMU" Then
                Me.SynTable.Columns.Add("MXEM")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                Me.SynTable.Columns.Add("MXEUM")
            ElseIf SelectedUniverse = "FRANCE" Then
                Me.SynTable.Columns.Add("MXFR")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("6100001")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                Me.SynTable.Columns.Add("MXFR")
                Me.SynTable.Columns.Add("6100002")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("6100004")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                Me.SynTable.Columns.Add("MXUSLC")
                Me.SynTable.Columns.Add("6100024")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                Me.SynTable.Columns.Add("MXEU")
                Me.SynTable.Columns.Add("6100026")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("6100030")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("6100033")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                Me.SynTable.Columns.Add("MXEUM")
                Me.SynTable.Columns.Add("6100062")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("6100063")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("AVEURO")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                Me.SynTable.Columns.Add("MXEM")
                Me.SynTable.Columns.Add("AVEUROPE")
            End If
            Me.SynTable.Columns.Add("EPS_CHG_NTM")
            Me.SynTable.Columns.Add("DIV_YLD_NTM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle <> "Financials" Then
                    Me.SynTable.Columns.Add("FCF_YLD_NTM") '---NORMAL
                End If
            End If
            Me.SynTable.Columns.Add("EBIT_MARGIN_NTM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.SynTable.Columns.Add("PBT_RWA_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("PBT_SALES_NTM") '----FINANCIAL
                End If
            End If
            Me.SynTable.Columns.Add("ROE_NTM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.SynTable.Columns.Add("ROTE_NTM") '----FINANCIAL
                End If
            End If
            Me.SynTable.Columns.Add("PE_NTM")
            Me.SynTable.Columns.Add("PB_NTM")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.SynTable.Columns.Add("P_TBV_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("RORWA_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("COST_INCOME_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("TIER_1_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("P_EMB_VALUE_NTM") '----FINANCIAL
                    Me.SynTable.Columns.Add("COMBINED_RATIO_NTM") '----FINANCIAL
                End If
            End If
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle <> "Financials" Then
                    Me.SynTable.Columns.Add("EV_EBIT_NTM") '---NORMAL
                    Me.SynTable.Columns.Add("EV_EBITDA_NTM") '---NORMAL
                    Me.SynTable.Columns.Add("EV_SALES_NTM") '---NORMAL
                    Me.SynTable.Columns.Add("PEG_NTM") '---NORMAL
                    Me.SynTable.Columns.Add("EV_EBIT_TO_G_NTM") '---NORMAL
                    Me.SynTable.Columns.Add("NET_DEBT_EBITDA_NTM") '---NORMAL
                End If
            End If
            Me.SynTable.Columns.Add("PRICE")
            Me.SynTable.Columns.Add("TARGET")
            Me.SynTable.Columns.Add("UPSIDE")
            Me.SynTable.Columns.Add("ESG")
            Me.SynTable.Columns.Add("GARPN_TOTAL_S")
            Me.SynTable.Columns.Add("GARPN_GROWTH_S")
            Me.SynTable.Columns.Add("GARPN_VALUE_S")
            Me.SynTable.Columns.Add("GARPN_YIELD_S")
            Me.SynTable.Columns.Add("GARPN_ISR_S")

            Me.SynTable.DefaultView.Sort = "SECTOR, INDUSTRY"

            Me.SynAgr.Clear()
            Me.SynAgr.Append(" fac.COUNTRY,")
            Me.SynAgr.Append(" fac.CURRENCY,")
            If SelectedUniverse = "ALL" Then
                Me.SynAgr.Append(" fac.MXEU,")
                Me.SynAgr.Append(" fac.MXUSLC,")
            ElseIf SelectedUniverse = "EUROPE" Then
                Me.SynAgr.Append(" fac.MXEU,")
            ElseIf SelectedUniverse = "USA" Then
                Me.SynAgr.Append(" fac.MXUSLC,")
            ElseIf SelectedUniverse = "EMU" Then
                Me.SynAgr.Append(" fac.MXEM,")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                Me.SynAgr.Append(" fac.MXEUM,")
            ElseIf SelectedUniverse = "FRANCE" Then
                Me.SynAgr.Append(" fac.MXFR,")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.[6100001],")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                Me.SynAgr.Append(" fac.MXFR,")
                Me.SynAgr.Append(" fac.[6100002],")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.[6100004],")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                Me.SynAgr.Append(" fac.MXUSLC,")
                Me.SynAgr.Append(" fac.[6100024],")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                Me.SynAgr.Append(" fac.MXEU,")
                Me.SynAgr.Append(" fac.[6100026],")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.[6100030],")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.[6100033],")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                Me.SynAgr.Append(" fac.MXEUM,")
                Me.SynAgr.Append(" fac.[6100062],")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.[6100063],")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.AVEURO,")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                Me.SynAgr.Append(" fac.MXEM,")
                Me.SynAgr.Append(" fac.AVEUROPE,")
            End If
            Me.SynAgr.Append(" fac.EPS_CHG_NTM,")
            Me.SynAgr.Append(" fac.DIV_YLD_NTM,")
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle = "Financials" Then
                    Me.SynAgr.Append(" fac.PBT_RWA_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.PBT_SALES_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.ROTE_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.P_TBV_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.RORWA_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.COST_INCOME_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.TIER_1_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.P_EMB_VALUE_NTM,") '----FINANCIAL
                    Me.SynAgr.Append(" fac.COMBINED_RATIO_NTM,") '----FINANCIAL
                End If
            End If
            If superSecteur IsNot Nothing Then
                If superSecteur.Libelle <> "Financials" Then
                    Me.SynAgr.Append(" fac.FCF_YLD_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.EV_EBIT_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.EV_EBITDA_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.EV_SALES_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.PEG_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.EV_EBIT_TO_G_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.NET_DEBT_EBITDA_NTM,") '---NORMAL
                    Me.SynAgr.Append(" fac.PRICE,")
                End If
            End If
            Me.SynAgr.Append(" fac.EBIT_MARGIN_NTM,")
            Me.SynAgr.Append(" fac.ROE_NTM,")
            Me.SynAgr.Append(" fac.PE_NTM,")
            Me.SynAgr.Append(" fac.PB_NTM,")
            Me.SynAgr.Append(" fac.TARGET,")
            Me.SynAgr.Append(" fac.UPSIDE,")
            Me.SynAgr.Append(" fac.ESG,")
            Me.SynAgr.Append(" fac.GARPN_TOTAL_S,")
            Me.SynAgr.Append(" fac.GARPN_GROWTH_S,")
            Me.SynAgr.Append(" fac.GARPN_VALUE_S,")
            Me.SynAgr.Append(" fac.GARPN_YIELD_S,")
            Me.SynAgr.Append(" fac.GARPN_ISR_S ")

        End Sub

#End Region ' !Onglet Synthese Rows

#Region "RecoPrint Rows"
        Private _recoTable As DataTable
        Private RecoAgr As New StringBuilder
        Public Property RecoTable() As DataTable
            Get
                Return _recoTable
            End Get
            Set(ByVal value As DataTable)
                _recoTable = value
                OnPropertyChanged("RecoTable")
                OnPropertyChanged("RecoTableView")
            End Set
        End Property

        ReadOnly Property RecoTableView As DataView
            Get
                Return If(Me.RecoTable IsNot Nothing, Me.RecoTable.DefaultView, Nothing)
            End Get
        End Property

        Public Sub FillRecoTable()
            Me.RecoTable = New DataTable()

            Me.RecoTable.Columns.Add("Industry")
            Me.RecoTable.Columns.Add("IndustryFGA")
            Me.RecoTable.Columns.Add("AssetName")
            Me.RecoTable.Columns.Add("Recommandation")
            Me.RecoTable.Columns.Add("MXEU")
            Me.RecoTable.Columns.Add("MXEM")
            Me.RecoTable.Columns.Add("MXEUM")
            Me.RecoTable.Columns.Add("MXUSLC")
            Me.RecoTable.DefaultView.Sort = "Industry, IndustryFGA"
        End Sub

#End Region ' !RecoPrint Rows

#Region "Fill"
        Sub ColumnConfigTOGenRow(ByVal col As ObservableCollection(Of ColumnConfig))
            For Each gen As GenRow In GenRows
                For Each c As ColumnConfig In col
                    gen.Criteres.Add(c.Name)
                Next
            Next
        End Sub

        Sub ColumnConfigTOGenTable(ByVal col As ObservableCollection(Of ColumnConfig), ByVal MyDataGrid As DataGrid)
            criteres = ""
            For Each c As ColumnConfig In col
                If c.ColumnName IsNot Nothing And c.ColumnName <> "" Then
                    If c.Name IsNot Nothing And c.Name <> "" Then
                        criteres += " fac." + c.ColumnName + " As '" + c.Name + "',"
                    Else
                        criteres += " fac." + c.ColumnName + " As '" + c.ColumnName + "',"
                    End If
                End If
            Next
        End Sub

        Sub load()
            FillGenTable(Nothing, Nothing)
            FillCroTable(Nothing, Nothing)
            FillQuaTable(Nothing, Nothing)
            FillValTable(Nothing, Nothing)
            FillMomTable(Nothing, Nothing)
            FillSynTable(Nothing, Nothing)
            FillRecoTable()
            FillDates()
            FillUnivers()
        End Sub

        Sub loadSecteur()
            If SelectedUniverse <> "" Then
                FillSuperSecteurs()
                FillSecteurs(New Secteur(0, ALLSUPERSECTORS))
            End If
        End Sub

        Sub initialize(ByVal mdate As String, ByVal univers As String, ByVal supersecteur As Secteur, ByVal secteur As Secteur)
            'FindDatas(mdate, univers, SECTOR, INDUSTRY)
            'fillDTCroissance(mdate, univers)
        End Sub


        Sub TestDoublons()
            Dim stocks As List(Of Dictionary(Of String, Object))
            stocks = co.sqlToListDico(CorrectImportStockWindow.FIND_DOUBLE_EQUITY_SQL)

            If stocks.Count > 0 Then
                MessageBox.Show("Bonjour." + vbCrLf + vbCrLf + "Il existe des doublons dans la base!" + vbCrLf + "Vous pouvez corriger cela grâce à l'utilitaire correspondant.")
            End If
            stocks.Clear()
        End Sub


        Sub fillDTCroissance(ByVal mdate As String, ByVal univers As String)
            Dim cmd As SqlCommand = co.ProcedureStockéeForDataGrid("ACT_DataGridGrowth", {"@date"}.ToList, New List(Of Object)({mdate}))
            Dim sda As SqlDataAdapter = New SqlDataAdapter(cmd)
            DTCroissance.Clear()
            sda.Fill(DTCroissance)
        End Sub

        Sub filterDTCroissance(ByVal supersecteur As Secteur, ByVal secteur As Secteur)
            DTCroissance.DefaultView.RowFilter = ""

            If DTCroissance.Columns.Count = 0 Then
                Return
            End If

            If secteur Is Nothing OrElse secteur.Libelle = "" Then
                DTCroissance.DefaultView.RowFilter = "[Secteur FGA] = ''"
            ElseIf secteur.Libelle = ALLSECTORS Then
                DTCroissance.DefaultView.RowFilter = "[Libellé] = ''"
            Else
                DTCroissance.DefaultView.RowFilter = "[Secteur FGA] = '" + secteur.Libelle + "'"
            End If
        End Sub
#End Region ' !Fill

#Region "UpdateGenRows"

        Sub FindReco(ByVal mDate As String, ByVal univers As String, ByVal superSecteur As Secteur, ByVal secteur As Secteur)
            If superSecteur Is Nothing Then
                Return
            End If

            Me.RecoTable.Clear()
            FillRecoTable()
            Dim sql As New StringBuilder
            sql.Append("select ss.label as Secteur, fga.label as IndustryFGA, fac.COMPANY_NAME as AssetName, rec_c.comment as Recommandation, rec_v.reco_SXXP as MXEU, rec_v.reco_SXXE as MXEM, rec_v.reco_SXXA as MXEUM, rec_v.reco_MXUSLC as MXUSLC")
            sql.Append(" from ref_security.SECTOR ss")
            sql.Append(" inner join ref_security.SECTOR s on s.id_parent=ss.id")
            sql.Append(" inner join ref_security.SECTOR_TRANSCO tr on tr.id_sector1=s.id")
            sql.Append(" inner join ref_security.SECTOR fga on fga.id=tr.id_sector2")
            'sql.Append(" inner join ref_security.ASSET_TO_SECTOR ass_sec on ass_sec.id_sector=s.id")
            'sql.Append(" inner join ref_security.ASSET ass on ass.Id=ass_sec.id_asset and ass.MaturityDate=ass_sec.date")
            sql.Append(" inner join DATA_FACTSET fac on fac.sector=s.code ")
            'sql.Append(" inner join ACT_RECO_VALEUR rec_v on rec_v.ISIN=ass.ISIN")
            sql.Append(" inner join ACT_RECO_VALEUR rec_v on rec_v.ISIN=fac.ISIN")
            sql.Append(" inner join ACT_RECO_COMMENT rec_c on rec_c.id=rec_v.id_comment")
            'sql.Append(" inner join DATA_FACTSET fac on fac.ISIN=ass.ISIN")
            sql.Append(" where ss.code=" + superSecteur.Id.ToString + " and fac.DATE='" + mDate + "' and s.class_name='GICS' and tr.class_name='FGA_ALL' and rec_c.comment not like ''")
            sql.Append(" and rec_v.id_comment=(select top(1) id_comment from ACT_RECO_VALEUR where ISIN=rec_v.ISIN order by date desc)")
            sql.Append(" AND ss.class_name = 'GICS'")
            If SelectedUniverse = "ALL" Then
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null")
            Else
                Return
            End If
            sql.Append(" order by ss.label, fga.label, fac.COMPANY_NAME")

            sqlToTable(sql.ToString, "Reco")
        End Sub

        Sub FindDatas(ByVal mDate As String, ByVal univers As String, ByVal superSecteur As Secteur, ByVal secteur As Secteur, ByVal grid As String)
            If grid = "Gen" Then
                Me.GenTable.Clear()
                FillGenTable(superSecteur, secteur)
            ElseIf grid = "Cro" Then
                Me.CroTable.Clear()
                FillCroTable(superSecteur, secteur)
            ElseIf grid = "Qua" Then
                Me.QuaTable.Clear()
                FillQuaTable(superSecteur, secteur)
            ElseIf grid = "Val" Then
                Me.ValTable.Clear()
                FillValTable(superSecteur, secteur)
            ElseIf grid = "Mom" Then
                Me.MomTable.Clear()
                FillMomTable(superSecteur, secteur)
            ElseIf grid = "Syn" Then
                Me.SynTable.Clear()
                FillSynTable(superSecteur, secteur)
            End If

            If mDate Is Nothing Or SelectedUniverse = "" Then
                Return
            End If

            If superSecteur Is Nothing Then
                If secteur Is Nothing Then
                    FindAll(mDate, univers, grid)
                ElseIf secteur.Libelle = "" Then
                    FindAll(mDate, univers, grid)
                End If
                Return
            ElseIf superSecteur.Libelle = "" Then
                If secteur Is Nothing Then
                    FindAll(mDate, univers, grid)
                ElseIf secteur.Libelle = "" Then
                    FindAll(mDate, univers, grid)
                End If
                Return
            End If

            If superSecteur.Libelle = ALLSUPERSECTORS Or (secteur IsNot Nothing AndAlso secteur.Libelle = ALLSECTORS) Then
                If superSecteur.Libelle = ALLSUPERSECTORS Then
                    FindAllSuperSectors(mDate, univers, grid)
                End If
                If secteur IsNot Nothing AndAlso secteur.Libelle = ALLSECTORS Then
                    FindAllSectors(mDate, univers, grid)
                End If

                Return
            End If

            If secteur Is Nothing Then
                FindGeneral(mDate, univers, IIf(superSecteur.Id < 0, Nothing, superSecteur.Id), Nothing, grid)
            Else
                FindGeneral(mDate, univers, IIf(superSecteur.Id < 0, Nothing, superSecteur.Id),
                                            IIf(secteur.Id < 0, Nothing, secteur.Id), grid)
            End If
        End Sub

        Public Sub setFormat()
            Dim sql As String = "SELECT Champs_FACTSET AS nom, format, precision, general, growth, value, quality, momentum, synthese  FROM ACT_AGR_FORMAT"
            Dim format As String
            Dim d As Double
            Dim nfi As NumberFormatInfo = New CultureInfo("en-US", False).NumberFormat
            nfi.NumberGroupSeparator = " "

            For Each dico In co.sqlToListDico(sql)
                format = "N0"
                If dico("format") <> 4 And dico("nom") <> "ESG" Then
                    'For index = dico("precision") To 1 Step -1
                    '    format &= "#"
                    'Next

                    Select Case (dico("format"))
                        Case 0
                            Select Case (dico("precision"))
                                Case 0
                                    format = "# ##0;(##0)"
                                Case 1
                                    format = "# ##0.0;(##0.0)"
                                Case 2
                                    format = "# ##0.00;(##0.00)"
                            End Select
                        Case 1
                            Select Case (dico("precision"))
                                Case 0
                                    format = "# ##0x;(##0x)"
                                Case 1
                                    format = "# ##0.0x;(##0.0x)"
                                Case 2
                                    format = "# ##0.00x;(##0.00x)"
                            End Select
                        Case 2
                            Select Case (dico("precision"))
                                Case 0
                                    format = "# ##0\%;(##0\%)"
                                Case 1
                                    format = "# ##0.0\%;(##0.0\%)"
                                Case 2
                                    format = "# ##0.00\%;(##0.00\%)"
                            End Select
                    End Select

                    If dico("general") = "X" Then
                        For Each row As DataRow In GenTable.Rows
                            If GenTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                    If dico("growth") = "X" Then
                        For Each row As DataRow In CroTable.Rows
                            If CroTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                    If dico("quality") = "X" Then
                        For Each row As DataRow In QuaTable.Rows
                            If QuaTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                    If dico("value") = "X" Then
                        For Each row As DataRow In ValTable.Rows
                            If ValTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                    If dico("momentum") = "X" Then
                        For Each row As DataRow In MomTable.Rows
                            If MomTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                    If dico("synthese") = "X" Then
                        For Each row As DataRow In SynTable.Rows
                            If SynTable.Columns.Contains(dico("nom")) Then
                                If row(dico("nom")) IsNot DBNull.Value Then
                                    d = row(dico("nom"))
                                    row(dico("nom")) = d.ToString(format, nfi)
                                End If
                            End If
                        Next
                    End If
                ElseIf dico("nom") = "ESG" Then
                    For Each row As DataRow In GenTable.Rows
                        If GenTable.Columns.Contains(dico("nom")) Then
                            If row(dico("nom")) IsNot DBNull.Value Then
                                Dim tmp As String = row(dico("nom"))
                                If tmp <> "EXCLU" Then
                                    row(dico("nom")) = Helpers.DisplayRoundedNumber(tmp, 2)
                                Else
                                    row(dico("nom")) = "EXCLU"
                                End If
                            End If
                        End If
                    Next

                End If
            Next
        End Sub

        Sub sqlToTable(ByVal sql As String, ByVal grid As String)

            Dim index As Integer = 0
            Dim myCommand As New SqlCommand(sql, Connection.coBase)
            'Dim reader As SqlDataReader = myCommand.ExecuteReader()
            'Dim row As Object() = New Object(reader.FieldCount) {}
            'Dim row2 As DataRow = Me.GenTable.NewRow()

            'reader.Close()
            Dim da As SqlDataAdapter = New SqlDataAdapter(myCommand)
            If grid = "Gen" Then
                da.Fill(Me.GenTable)
            ElseIf grid = "Cro" Then
                da.Fill(Me.CroTable)
            ElseIf grid = "Qua" Then
                da.Fill(Me.QuaTable)
            ElseIf grid = "Val" Then
                da.Fill(Me.ValTable)
            ElseIf grid = "Mom" Then
                da.Fill(Me.MomTable)
            ElseIf grid = "Reco" Then
                da.Fill(Me.RecoTable)
            ElseIf grid = "Syn" Then
                da.Fill(Me.SynTable)
            End If

        End Sub

        Sub FindAll(ByVal mDate As String, ByVal univers As String, ByVal grid As String)
            Dim sql As New StringBuilder

            FindAllSuperSectors(mDate, univers, grid)
            FindAllSectors(mDate, univers, grid)

            sql.Append("SELECT distinct fac.COMPANY_NAME As 'COMPANY_NAME',")
            sql.Append(Me.criteres)
            sql.Append(" ss.label As 'SECTOR',")
            sql.Append(" ss.code As 'SuperSecteurId',")
            sql.Append(" fga.label As 'INDUSTRY',")
            sql.Append(" fga.code As 'SecteurId',")
            sql.Append(" fgafac.SUIVI,")
            sql.Append(" fac.TICKER,")
            sql.Append(" fac.ISIN,")
            If grid = "Gen" Then
                sql.Append(Me.GenAgr.ToString)
            ElseIf grid = "Cro" Then
                sql.Append(Me.CroAgr.ToString)
            ElseIf grid = "Qua" Then
                sql.Append(Me.QuaAgr.ToString)
            ElseIf grid = "Val" Then
                sql.Append(Me.ValAgr.ToString)
            ElseIf grid = "Mom" Then
                sql.Append(Me.MomAgr.ToString)
            ElseIf grid = "Syn" Then
                sql.Append(Me.SynAgr.ToString)
            End If
            sql.Append(" FROM DATA_FACTSET fac")
            sql.Append(" INNER JOIN ref_security.SECTOR s on s.code = fac.SECTOR")
            sql.Append(" INNER JOIN ref_security.SECTOR ss on ss.id = s.id_parent")
            sql.Append(" INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id")
            sql.Append(" INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2")
            sql.Append(" INNER JOIN DATA_factset fgafac on fgafac.fga_sector = fga.code AND fgafac.DATE=fac.DATE")
            sql.Append(" WHERE fgafac.gics_sector is null AND fac.ISIN is not null AND fac.date='" + mDate + "' AND fga.class_name='FGA_ALL'")

            If SelectedUniverse = "ALL" Then
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null")
            Else
                Return
            End If
            sql.Append(" ORDER BY fac.COMPANY_NAME")

            sqlToTable(sql.ToString, grid)
        End Sub

        Sub FindAllSuperSectors(ByVal mDate As String, ByVal univers As String, ByVal grid As String)

            Dim sql As New StringBuilder
            sql.Append("SELECT ss.label As 'SECTOR', ss.code As 'SuperSecteurId',")
            sql.Append(Me.criteres)
            sql.Append(" fac.SUIVI,")
            If grid = "Gen" Then
                sql.Append(" fac.ISIN,")
                sql.Append(Me.GenAgr.ToString)
            ElseIf grid = "Cro" Then
                sql.Append(Me.CroAgr.ToString)
            ElseIf grid = "Qua" Then
                sql.Append(Me.QuaAgr.ToString)
            ElseIf grid = "Val" Then
                sql.Append(Me.ValAgr.ToString)
            ElseIf grid = "Mom" Then
                sql.Append(Me.MomAgr.ToString)
            ElseIf grid = "Syn" Then
                sql.Append(Me.SynAgr.ToString)
            End If
            sql.Append(" FROM ref_security.SECTOR ss")
            sql.Append(" INNER JOIN DATA_FACTSET fac on fac.GICS_SECTOR = ss.code")
            sql.Append(" WHERE [level] = 0")
            sql.Append(" AND ss.class_name = 'GICS' AND fac.GICS_SUBINDUSTRY is null AND fac.DATE='" + mDate + "'")
            If SelectedUniverse = "ALL" Then
                sql.Append(" AND fac.MXEU is not null AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null AND fac.MXUSLC is null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXEU is null AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null")
            Else
                Return
            End If
            sqlToTable(sql.ToString, grid)
        End Sub

        Sub FindAllSectors(ByVal mDate As String, ByVal univers As String, ByVal grid As String)

            Dim sql As New StringBuilder
            sql.Append("SELECT DISTINCT ss.label As 'SECTOR', ss.code As 'SuperSecteurId', s.label As 'INDUSTRY', s.code As 'SecteurId',")
            sql.Append(Me.criteres)
            sql.Append(" fac.SUIVI,")
            If grid = "Gen" Then
                sql.Append(" fac.ISIN,")
                sql.Append(Me.GenAgr.ToString)
            ElseIf grid = "Cro" Then
                sql.Append(Me.CroAgr.ToString)
            ElseIf grid = "Qua" Then
                sql.Append(Me.QuaAgr.ToString)
            ElseIf grid = "Val" Then
                sql.Append(Me.ValAgr.ToString)
            ElseIf grid = "Mom" Then
                sql.Append(Me.MomAgr.ToString)
            ElseIf grid = "Syn" Then
                sql.Append(Me.SynAgr.ToString)
            End If
            sql.Append(" FROM ref_security.SECTOR s")
            sql.Append(" INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id")
            sql.Append(" INNER JOIN ref_security.SECTOR fils on fils.id = st.id_sector2")
            sql.Append(" LEFT OUTER JOIN ref_security.SECTOR ss ON fils.id_parent = ss.id")
            sql.Append(" INNER JOIN DATA_FACTSET fac on fac.FGA_SECTOR = s.code")
            sql.Append(" WHERE fac.GICS_SECTOR is null AND ss.class_name = 'GICS' AND fac.DATE='" + mDate + "'")
            If SelectedUniverse = "ALL" Then
                sql.Append(" AND fac.MXEU is not null and fac.MXUSLC is not null AND s.class_name = 'FGA_ALL' ")
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null and fac.MXUSLC is null AND s.class_name = 'FGA_EU'")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXEU is null and fac.MXUSLC is not null AND s.class_name = 'FGA_US'")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null AND s.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null AND s.class_name = 'FGA_ALL'")
            Else
                Return
            End If

            sqlToTable(sql.ToString, grid)
        End Sub

        Sub FindAllValeurs(ByVal mDate As String, ByVal univers As String, ByVal superSector As Integer?, ByVal sector As Integer?, ByVal grid As String)

            Dim sql As New StringBuilder

            If sector Is Nothing Then
                sql.Append("SELECT distinct fac.COMPANY_NAME As 'COMPANY_NAME',")
                sql.Append(Me.criteres)
                sql.Append(" (select label from ref_security.SECTOR where code = " + superSector.ToString + ") As 'SECTOR',")
                sql.Append(" " + superSector.ToString + " As 'SuperSecteurId',")
                sql.Append(" fga.label As 'INDUSTRY',")
                sql.Append(" fga.code As 'SecteurId',")
                sql.Append(" fgafac.SUIVI,")
                sql.Append(" fac.TICKER,")
                sql.Append(" fac.ISIN,")
                If grid = "Gen" Then
                    sql.Append(Me.GenAgr.ToString)
                ElseIf grid = "Cro" Then
                    sql.Append(Me.CroAgr.ToString)
                ElseIf grid = "Qua" Then
                    sql.Append(Me.QuaAgr.ToString)
                ElseIf grid = "Val" Then
                    sql.Append(Me.ValAgr.ToString)
                ElseIf grid = "Mom" Then
                    sql.Append(Me.MomAgr.ToString)
                ElseIf grid = "Syn" Then
                    sql.Append(Me.SynAgr.ToString)
                End If
                sql.Append(" FROM ref_security.SECTOR s")
                If sector Is Nothing Then
                    sql.Append(" INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id")
                    sql.Append(" INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2")
                End If
                'sql.Append(" INNER JOIN ref_security.ASSET_TO_SECTOR assTOsec ON assTOsec.id_sector = s.id")
                'sql.Append(" INNER JOIN ref_security.ASSET ass ON ass.Id = assTOsec.id_asset and ass.MaturityDate=assTOsec.date")
                sql.Append(" INNER JOIN DATA_FACTSET fac ON fac.sector = s.code")
                'sql.Append(" INNER JOIN DATA_FACTSET fac ON fac.ISIN = ass.ISIN")
                sql.Append(" INNER JOIN DATA_factset fgafac on fgafac.fga_sector = fga.code AND fgafac.DATE=fac.DATE")
                If sector Is Nothing Then
                    sql.Append(" WHERE fgafac.gics_sector is null AND fac.DATE='" + mDate + "' AND s.id_parent = (select id from ref_security.SECTOR where code = " + superSector.ToString + ")")
                Else
                    sql.Append(" WHERE fgafac.gics_sector is null AND fac.DATE='" + mDate + "' AND s.code = " + sector.ToString)
                End If
            Else
                sql.Append("SELECT distinct fac.COMPANY_NAME As 'COMPANY_NAME',")
                sql.Append(Me.criteres)
                sql.Append(" (select label from ref_security.SECTOR where code = " + superSector.ToString + ") As 'SECTOR',")
                sql.Append(" " + superSector.ToString + " As 'SuperSecteurId',")
                sql.Append(" fga.label As 'INDUSTRY',")
                sql.Append(" fga.code As 'SecteurId',")
                sql.Append(" fgafac.SUIVI,")
                sql.Append(" fac.TICKER,")
                sql.Append(" fac.ISIN,")
                If grid = "Gen" Then
                    sql.Append(Me.GenAgr.ToString)
                ElseIf grid = "Cro" Then
                    sql.Append(Me.CroAgr.ToString)
                ElseIf grid = "Qua" Then
                    sql.Append(Me.QuaAgr.ToString)
                ElseIf grid = "Val" Then
                    sql.Append(Me.ValAgr.ToString)
                ElseIf grid = "Mom" Then
                    sql.Append(Me.MomAgr.ToString)
                ElseIf grid = "Syn" Then
                    sql.Append(Me.SynAgr.ToString)
                End If
                sql.Append(" FROM ref_security.SECTOR fga")
                sql.Append(" INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = fga.id")
                sql.Append(" INNER JOIN ref_security.SECTOR s on s.id = st.id_sector2")
                'sql.Append(" INNER JOIN ref_security.ASSET_TO_SECTOR assTOsec ON assTOsec.id_sector = s.id")
                sql.Append(" INNER JOIN DATA_FACTSET fac ON fac.sector = s.code")
                sql.Append(" INNER JOIN DATA_factset fgafac on fgafac.fga_sector = fga.code AND fgafac.DATE=fac.DATE")
                sql.Append(" WHERE fgafac.gics_sector is null AND fac.DATE='" + mDate + "' AND fga.code = " + sector.ToString)
            End If
            If SelectedUniverse = "ALL" Then
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null")
            Else
                Return
            End If
            sql.Append(" ORDER BY fac.COMPANY_NAME")

            sqlToTable(sql.ToString, grid)
        End Sub

        Sub FindGeneral(ByVal myDate As String, ByVal indice As String, ByVal superSector As Integer?, ByVal sector As Integer?, ByVal grid As String)

            If superSector Is Nothing Then
                Return
            End If

            Dim sql As New StringBuilder
            sql.Append("SELECT ss.label As 'SECTOR', ss.code As 'SuperSecteurId',")
            sql.Append(" fga.label As 'INDUSTRY', fga.code As 'SecteurId',")
            sql.Append(Me.criteres)
            sql.Append(" fac.SUIVI,")
            sql.Append(" fac.TICKER,")
            If grid = "Gen" Then
                sql.Append(" fac.ISIN,")
                sql.Append(Me.GenAgr.ToString)
            ElseIf grid = "Cro" Then
                sql.Append(Me.CroAgr.ToString)
            ElseIf grid = "Qua" Then
                sql.Append(Me.QuaAgr.ToString)
            ElseIf grid = "Val" Then
                sql.Append(Me.ValAgr.ToString)
            ElseIf grid = "Mom" Then
                sql.Append(Me.MomAgr.ToString)
            ElseIf grid = "Syn" Then
                sql.Append(Me.SynAgr.ToString)
            End If

            sql.Append(" FROM ref_security.SECTOR ss")
            sql.Append(" INNER JOIN ref_security.SECTOR s ON s.id_parent = ss.id")
            sql.Append(" INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id")
            sql.Append(" INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2")
            sql.Append(" INNER JOIN DATA_FACTSET fac on fac.FGA_SECTOR = fga.code")
            sql.Append(" WHERE ss.code = " + superSector.ToString)
            sql.Append(" AND fac.GICS_SECTOR is null AND fac.DATE='" + myDate + "'")
            If sector IsNot Nothing And sector <> -1 Then
                sql.Append(" AND fga.code = " + sector.ToString)
            End If
            sql.Append(" AND s.class_name = 'GICS'")
            If SelectedUniverse = "ALL" Then
                sql.Append(" AND fga.class_name = 'FGA_ALL' AND fac.MXEU is not null and fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fga.class_name = 'FGA_EU' AND fac.MXEU is not null and fac.MXUSLC is null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fga.class_name = 'FGA_US' AND fac.MXEU is null and fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null AND fga.class_name = 'FGA_ALL'")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null AND fga.class_name = 'FGA_ALL'")
            Else
                Return
            End If
            sql.Append(" UNION")
            sql.Append(" SELECT ss.label As 'SECTOR', ss.code As 'SuperSecteurId', null As 'INDUSTRY', null As 'SecteurId',")
            sql.Append(Me.criteres)
            sql.Append(" fac.SUIVI,")
            sql.Append(" fac.TICKER,")
            If grid = "Gen" Then
                sql.Append(" fac.ISIN,")
                sql.Append(Me.GenAgr.ToString)
            ElseIf grid = "Cro" Then
                sql.Append(Me.CroAgr.ToString)
            ElseIf grid = "Qua" Then
                sql.Append(Me.QuaAgr.ToString)
            ElseIf grid = "Val" Then
                sql.Append(Me.ValAgr.ToString)
            ElseIf grid = "Mom" Then
                sql.Append(Me.MomAgr.ToString)
            ElseIf grid = "Syn" Then
                sql.Append(Me.SynAgr.ToString)
            End If
            sql.Append(" FROM ref_security.SECTOR ss")
            sql.Append(" INNER JOIN DATA_FACTSET fac on fac.GICS_SECTOR = ss.code")
            sql.Append(" WHERE ss.code = " + superSector.ToString)
            sql.Append(" AND fac.GICS_SUBINDUSTRY is null AND fac.DATE='" + myDate + "'")
            If SelectedUniverse = "ALL" Then
                sql.Append(" AND fac.MXEU is not null AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EUROPE" Then
                sql.Append(" AND fac.MXEU is not null AND fac.MXUSLC is null")
            ElseIf SelectedUniverse = "USA" Then
                sql.Append(" AND fac.MXEU is null AND fac.MXUSLC is not null")
            ElseIf SelectedUniverse = "EMU" Then
                sql.Append(" AND fac.MXEM is not null")
            ElseIf SelectedUniverse = "EUROPE EX EMU" Then
                sql.Append(" AND fac.MXEUM is not null")
            ElseIf SelectedUniverse = "FRANCE" Then
                sql.Append(" AND fac.MXFR is not null")
            ElseIf SelectedUniverse = "FEDERIS ACTIONS" Then
                sql.Append(" AND fac.[6100001] is not null")
            ElseIf SelectedUniverse = "FEDERIS FRANCE ACTIONS" Then
                sql.Append(" AND fac.[6100002] is not null")
            ElseIf SelectedUniverse = "FEDERIS ISR EURO" Then
                sql.Append(" AND fac.[6100004] is not null")
            ElseIf SelectedUniverse = "FEDERIS NORTH AMERICA" Then
                sql.Append(" AND fac.[6100024] is not null")
            ElseIf SelectedUniverse = "FEDERIS EUROPE ACTIONS" Then
                sql.Append(" AND fac.[6100026] is not null")
            ElseIf SelectedUniverse = "FEDERIS EURO ACTIONS" Then
                sql.Append(" AND fac.[6100030] is not null")
            ElseIf SelectedUniverse = "FEDERIS IRC ACTIONS" Then
                sql.Append(" AND fac.[6100033] is not null")
            ElseIf SelectedUniverse = "FEDERIS EX EURO" Then
                sql.Append(" AND fac.[6100062] is not null")
            ElseIf SelectedUniverse = "FEDERIS CROISSANCE EURO" Then
                sql.Append(" AND fac.[6100063] is not null")
            ElseIf SelectedUniverse = "AVENIR EURO" Then
                sql.Append(" AND fac.AVEURO is not null")
            ElseIf SelectedUniverse = "FEDERIS VALUE EURO" Then
                sql.Append(" AND fac.AVEUROPE is not null")
            Else
                Return
            End If
            sqlToTable(sql.ToString, grid)

            If sector Is Nothing Then
                FindAllValeurs(myDate, indice, IIf(superSector < 0, Nothing, superSector), Nothing, grid)
            Else
                FindAllValeurs(myDate, indice, IIf(superSector < 0, Nothing, superSector),
                                            IIf(sector < 0, Nothing, sector), grid)
            End If
        End Sub

        Function GetSQLWhereConditions(ByVal conditions As Dictionary(Of String, Object)) As String
            Dim sql As New StringBuilder

            sql.Append(" WHERE ")

            For Each c As String In conditions.Keys
                If conditions(c) Is Nothing Then
                    sql.Append(c + " is null AND ")
                    Continue For
                End If

                sql.Append(c + " = ")
                If conditions(c).GetType.Name.ToString = "Decimal" Or conditions(c).GetType.Name.ToString = "Double" Or conditions(c).GetType.Name.ToString = "Int32" Then
                    sql.Append(conditions(c))
                ElseIf conditions(c).GetType.Name.ToString = "Boolean" Then
                    If conditions(c) Then
                        sql.Append("1")
                    Else
                        sql.Append("0")
                    End If
                Else
                    sql.Append("'" + conditions(c).ToString.Replace("'", "''") + "'")
                End If

                sql.Append(" AND ")
            Next
            sql.Append("1 = 1")

            Return sql.ToString
        End Function


#End Region ' !UpdateGenRows

#Region "DatabaseAccess"

        Sub FillDates()
            Dates.Clear()

            For Each d As Date In co.SelectDistinctSimple("DATA_FACTSET", "date", "DESC")
                Dates.Add(d.ToShortDateString)
            Next
        End Sub

        Sub FillUnivers()
            Univers.Clear()

            Univers.Add("")
            Univers.Add("ALL")
            Univers.Add("USA")
            Univers.Add("EUROPE")
            Univers.Add("EUROPE EX EMU")
            Univers.Add("EMU")
            Univers.Add("FRANCE")
            Univers.Add("")
            Univers.Add("FEDERIS NORTH AMERICA")
            Univers.Add("FEDERIS EUROPE ACTIONS")
            Univers.Add("FEDERIS EX EURO")
            Univers.Add("FEDERIS EURO ACTIONS")
            Univers.Add("AVENIR EURO")
            Univers.Add("FEDERIS ISR EURO")
            Univers.Add("FEDERIS ACTIONS")
            Univers.Add("FEDERIS IRC ACTIONS")
            Univers.Add("FEDERIS CROISSANCE EURO")
            Univers.Add("FEDERIS VALUE EURO")
            Univers.Add("FEDERIS FRANCE ACTIONS")
        End Sub


        Sub FillSuperSecteurs()
            Dim sql As String = "SELECT CODE AS Id, LABEL AS Libelle FROM ref_security.SECTOR WHERE"
            sql += " class_name = 'GICS'"
            sql += " and [level] = 0 ORDER BY label"
            SuperSecteurs.Clear()
            SuperSecteurs.Add(New Secteur(-1, ""))
            SuperSecteurs.Add(New Secteur(-1, ALLSUPERSECTORS))
            For Each s As Secteur In co.sqlToListObject(sql, Function() New Secteur)
                SuperSecteurs.Add(s)
            Next
        End Sub

        Sub FillSecteurs(ByVal supersecteur As Secteur)
            Dim sql As String
            If supersecteur IsNot Nothing Then
                If supersecteur.Id > 0 Then
                    sql = "SELECT distinct fga.code AS Id, fga.LABEL AS Libelle"
                    sql += " FROM ref_security.SECTOR s"
                    sql += " INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id"
                    sql += " INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2"
                    sql += " WHERE s.class_name = 'GICS' and s.id_parent = (select id from ref_security.SECTOR where code = " + supersecteur.Id.ToString + ")"
                    If SelectedUniverse = "ALL" Then
                        sql += " AND fga.class_name = 'FGA_ALL'"
                    ElseIf SelectedUniverse = "EUROPE" Then
                        sql += " AND fga.class_name = 'FGA_EU'"
                    ElseIf SelectedUniverse = "USA" Then
                        sql += " AND fga.class_name = 'FGA_US'"
                    Else
                        sql += " AND fga.class_name = 'FGA_ALL'"
                    End If
                    sql += " ORDER BY Libelle"
                Else
                    sql = "SELECT code AS Id, LABEL AS Libelle FROM ref_security.SECTOR WHERE"
                    If SelectedUniverse = "ALL" Then
                        sql += " class_name = 'FGA_ALL'"
                    ElseIf SelectedUniverse = "EUROPE" Then
                        sql += " class_name = 'FGA_EU'"
                    ElseIf SelectedUniverse = "USA" Then
                        sql += " class_name = 'FGA_US'"
                    Else
                        sql += " class_name = 'FGA_ALL'"
                    End If
                    sql += " AND [level] = 0 "
                    sql += " ORDER BY Libelle"
                End If

                Secteurs.Clear()
                Secteurs.Add(New Secteur(-1, ""))
                Secteurs.Add(New Secteur(-1, ALLSECTORS))
                For Each s As Secteur In co.sqlToListObject(sql, Function() New Secteur)
                    Secteurs.Add(s)
                Next
            End If
        End Sub

#End Region ' !DatabaseAccess

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