Imports System.Collections.ObjectModel

Imports System.Text
Imports System.Windows.Controls
Imports System.ComponentModel

Namespace Action

    Public Class CorrectImportStockWindow

        Private co As New Connection()
        Private log As New Log()

        Public Sub New()

            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Me.DataContext = Me
            fillWithDouble()

        End Sub

#Region "Properties"
        Public Property StockList As New ObservableCollection(Of S_Stock)
#End Region ' !Properties

#Region "events"

        Private Sub BKeep_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
            Dim stock As S_Stock = DGStock.SelectedItem()
            Dim deleted_stocks As New List(Of S_Stock)

            For Each s As S_Stock In DGStock.Items
                If (s.Name = stock.Name Or s.Isin = stock.Isin Or s.Ticker = stock.Ticker) And (stock <> s) Then
                    deleted_stocks.Add(s)
                Else
                    s.IsSelected = False
                End If
            Next

            For Each s As S_Stock In deleted_stocks                
                ReplaceWith(s, stock)
                StockList.Remove(s)
            Next

            StockList.Remove(stock)
        End Sub

        Private Sub ReplaceWith(ByVal oldStock As S_Stock, ByVal NewStock As S_Stock)
            'Throw New NotImplementedException


            Dim name As String = oldStock.Name.Replace("'", "''")
            Dim isin As String = oldStock.Isin.Replace("'", "''")
            Dim ticker As String = oldStock.Ticker.Replace("'", "''")
            Dim country As String = oldStock.Country.Replace("'", "''")
            Dim name2 As String = NewStock.Name.Replace("'", "''")
            Dim isin2 As String = NewStock.Isin.Replace("'", "''")
            Dim country2 As String = NewStock.Country.Replace("'", "''")
            Dim ticker2 As String = NewStock.Ticker.Replace("'", "''")
            Dim sql As String

            sql = "UPDATE DATA_FACTSET" _
                + " SET TICKER='" + ticker2 + "', COMPANY_NAME='" + name2 + "', ISIN='" + isin2 + "', COUNTRY ='" + country2 + "'" _
                + " WHERE COMPANY_NAME='" + name + "' and ISIN='" + isin + "' and TICKER='" + ticker + "'"
            co.RequeteSql(sql)

        End Sub

        Private Sub fillWithDouble()
            Dim stocks As List(Of Dictionary(Of String, Object))
            stocks = findDouble()

            StockList.Clear()

            ' Add each stock to StockList
            For Each stock In stocks
                StockList.Add(New S_Stock(stock("date"), stock("name"), stock("isin"), stock("country"), stock("ticker")))
            Next
        End Sub

        Private Sub DGStock_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DGStock.SelectionChanged
            Dim stock As S_Stock = DGStock.SelectedItem()

            For Each s As S_Stock In DGStock.Items
                If s.Name = stock.Name Or s.Isin = stock.Isin Or s.Ticker = stock.Ticker Then
                    s.IsSelected = True
                    Me.UpdateLayout()
                Else
                    s.IsSelected = False
                End If
            Next
        End Sub

        Private Sub BClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BClose.Click
            Me.Close()
        End Sub
#End Region ' !Events

#Region "Find double"

        Public Const FIND_DOUBLE_EQUITY_SQL As String = "" _
                    + " DECLARE @sql AS	VARCHAR(max)" _
                    + " SET @sql=" _
                    + "   N' SELECT distinct x.COMPANY_NAME, x.ISIN, x.TICKER' " _
                    + " + N' INTO ##tmpdoubles' " _
                    + " + N' FROM' " _
                    + " + N' (SELECT distinct a.TICKER, a.COMPANY_NAME, a.ISIN' " _
                    + " + N' FROM DATA_FACTSET a' " _
                    + " + N' INNER JOIN DATA_FACTSET b on a.ISIN = b.ISIN AND' " _
                    + " + N'				(a.TICKER<>b.TICKER OR a.COMPANY_NAME<>b.COMPANY_NAME)' " _
                    + " + N' WHERE a.ISIN Is Not null' " _
                    + " + N' UNION' " _
                    + " + N' SELECT distinct a.TICKER, a.COMPANY_NAME, a.ISIN' " _
                    + " + N' FROM DATA_FACTSET a' " _
                    + " + N' INNER JOIN DATA_FACTSET c on a.TICKER = c.TICKER AND ' " _
                    + " + N'				(a.ISIN<>c.ISIN OR a.COMPANY_NAME<>c.COMPANY_NAME)' " _
                    + " + N' WHERE a.ISIN Is Not null				' " _
                    + " + N' UNION' " _
                    + " + N' SELECT distinct a.TICKER, a.COMPANY_NAME, a.ISIN' " _
                    + " + N' FROM DATA_FACTSET a				' " _
                    + " + N' INNER JOIN DATA_FACTSET d on a.COMPANY_NAME=d.COMPANY_NAME AND' " _
                    + " + N'				(a.ISIN<>d.ISIN OR a.TICKER<>d.TICKER)' " _
                    + " + N' WHERE a.ISIN Is Not null) AS x' " _
                    + " + N' ORDER BY x.TICKER' " _
                    + " EXEC(@sql) " _
                    + " SET @sql= " _
                    + "   N' SELECT (SELECT MAX(h.DATE) FROM DATA_FACTSET h' " _
                    + " + N' 	WHERE j.TICKER=h.TICKER AND j.ISIN=h.ISIN AND j.COMPANY_NAME=h.COMPANY_NAME) as date,' " _
                    + " + N' j.COMPANY_NAME as name,' " _
                    + " + N' j.ISIN as isin,' " _
                    + " + N' (SELECT TOP(1) h.COUNTRY FROM DATA_FACTSET h' " _
                    + " + N'		WHERE j.TICKER=h.TICKER AND j.ISIN=h.ISIN AND j.COMPANY_NAME=h.COMPANY_NAME) as country,' " _
                    + " + N' j.TICKER as ticker' " _
                    + " + N' FROM ##tmpdoubles as j' " _
                    + " + N' ORDER BY country, j.TICKER' " _
                    + " EXEC(@sql) " _
                    + " DROP TABLE ##tmpdoubles "

        Private Function findDouble() As List(Of Dictionary(Of String, Object))
            Return co.sqlToListDico(FIND_DOUBLE_EQUITY_SQL)

        End Function

#End Region ' !Find double

#Region "S_Stock"

        Structure S_Stock
            Sub New(ByVal datee As String, Optional ByVal name As String = Nothing, Optional ByVal isin As String = Nothing, Optional ByVal country As String = Nothing, Optional ByVal ticker As String = Nothing)
                Me.Datee = datee
                Me.Name = name
                Me.Isin = isin
                Me.Country = country
                Me.Ticker = ticker
                Me.IsSelected = False
            End Sub

            Property Datee As String
            Property Name As String
            Property Isin As String
            Property Country As String
            Property Ticker As String
            Property IsSelected As Boolean

            Shared Operator =(ByVal s1 As S_Stock, ByVal s2 As S_Stock) As Boolean
                Return s1.Name = s2.Name And s1.Isin = s2.Isin And s1.Ticker = s2.Ticker And s1.Country = s2.Country
            End Operator

            Shared Operator <>(ByVal s1 As S_Stock, ByVal s2 As S_Stock) As Boolean
                Return Not s1 = s2
            End Operator
        End Structure

#End Region ' !Structure S_Stock

    End Class

End Namespace