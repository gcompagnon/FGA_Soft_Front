Imports System.Windows.Controls

Namespace Action
    Public Class BaseActionRecommandation

        Private recommandations As New List(Of Recommandation)
        Private signs As New List(Of String)
        Private colOptIndex As Integer
        Private idICBIndex As Integer
        Private idFGAIndex As Integer
        Private is_valeur As Boolean

        Sub New(ByVal headers As List(Of String),
                 ByVal options As List(Of String),
                 ByVal values As List(Of List(Of String)),
                 ByVal isValeur As Boolean)
            InitializeComponent()

            is_valeur = isValeur
            signs = options
            addHeader(headers, options)
            addValues(values)
            updateBoutonPosition()
        End Sub

        Private Sub addHeader(ByVal headers As List(Of String), ByVal options As List(Of String))
            Dim row As New RowDefinition()
            Dim rowIndex As Integer = 0
            Dim colIndex As Integer = 0

            'construction de la ligne des headers
            row.Height = New Windows.GridLength(0, Windows.GridUnitType.Auto)
            Grid.RowDefinitions.Insert(rowIndex, row)

            ' construction des colonnes
            For Each h As String In headers
                If h.StartsWith("id") Then
                    createColumn(False)
                    If h.ToLower().Contains("icb") Then
                        idICBIndex = colIndex
                    ElseIf h.ToLower().Contains("fga") Then
                        idFGAIndex = colIndex
                    End If
                Else
                    createColumn(True)
                End If

                addControltoGrid(createLabel(h), rowIndex, colIndex, 1)
                colIndex += 1
            Next

            colOptIndex = colIndex
            For Each opt As String In options
                createColumn(True)
                colIndex += 1
            Next

            addControltoGrid(createLabel("Nouvelle Recommandation"), rowIndex, colOptIndex, options.Count)

            'Ajout colonne "pas d'option"
            createColumn(True)
            addControltoGrid(createLabel("Pas de changement"), rowIndex, colIndex, 1)

        End Sub


        Private Sub addValues(ByVal values As List(Of List(Of String)))
            For i As Integer = 0 To values.Count - 1
                Dim fields As Dictionary(Of Integer, String) = createDico(values(i))
                Dim buttons As New List(Of RadioButton)

                'Création des options
                For Each sign As String In signs
                    buttons.Add(createRadioButton(sign, i, False))
                Next
                'option pas de changement
                buttons.Add(createRadioButton("", i, True))

                Dim reco As New Recommandation(fields, buttons)
                recommandations.Add(reco)
            Next

            CreateTableReco()
        End Sub

        Private Sub updateBoutonPosition()
            'Alignement des boutons
            Grid.Children.Remove(BValider)
            Grid.Children.Remove(BAnnuler)

            Dim gridColMid As Integer = Grid.ColumnDefinitions.Count / 2
            BValider.HorizontalAlignment = Windows.HorizontalAlignment.Center
            BValider.VerticalAlignment = Windows.VerticalAlignment.Top
            BAnnuler.HorizontalAlignment = Windows.HorizontalAlignment.Center
            BAnnuler.VerticalAlignment = Windows.VerticalAlignment.Top
            addControltoGrid(BValider, Grid.RowDefinitions.Count - 1, 0, gridColMid)
            addControltoGrid(BAnnuler, Grid.RowDefinitions.Count - 1, gridColMid, gridColMid)
        End Sub

        Private Sub createColumn(ByVal isShowing As Boolean)
            Dim col As New ColumnDefinition()
            If isShowing Then
                col.Width = New Windows.GridLength(0, Windows.GridUnitType.Auto)
            Else
                col.Width = New Windows.GridLength(0)
            End If
            Grid.ColumnDefinitions.Add(col)
        End Sub


        Private Sub CreateTableReco()
            For Each reco As Recommandation In recommandations
                If is_valeur Then
                    If reco.getField(3) = "SXXE" Then
                        addBlankRow()
                    End If
                ElseIf reco.getField(2) = "" Then
                    addBlankRow()
                End If
                AddRow(reco)
            Next
            addBlankRow()
        End Sub

        Private Sub addBlankRow()
            Dim row As New System.Windows.Controls.RowDefinition()

            row.Height = New Windows.GridLength(20)
            Grid.RowDefinitions.Insert(Grid.RowDefinitions.Count - 1, row)
        End Sub

        Private Sub AddRow(ByVal reco As Recommandation)
            Dim row As New System.Windows.Controls.RowDefinition()
            Dim rowIndex As Integer = Grid.RowDefinitions.Count - 1
            Dim colIndex As Integer = 0

            row.Height = New Windows.GridLength(0, Windows.GridUnitType.Auto)
            Grid.RowDefinitions.Insert(rowIndex, row)

            For i As Integer = 0 To colOptIndex - 1
                addControltoGrid(createLabel(reco.getField(i)), rowIndex, colIndex, 1)
                colIndex += 1
            Next

            colIndex = colOptIndex
            For Each b In reco.buttons
                addControltoGrid(b, rowIndex, colIndex, 1)
                colIndex += 1
            Next
        End Sub

        ''' <summary>
        ''' Créé et rempli un dictionnaire à partir d'une liste de valeur en suivant les headers de grid.
        ''' </summary>
        ''' <param name="list">La liste des valeurs pour remplir le dictionnaire</param>
        ''' <returns>Le dictionnaire</returns>
        ''' <remarks>Si la liste contient moins de champs que le nombre de header, les dernieres valeurs associées aux headers restant seront vides seront vides</remarks>
        Private Function createDico(ByVal list As List(Of String)) As Dictionary(Of Integer, String)
            Dim Dico As New Dictionary(Of Integer, String)
            Dim max As Integer = Math.Min(list.Count, colOptIndex) - 1

            For i As Integer = 0 To max
                Dico.Add(i, list(i))
            Next

            Return Dico
        End Function

        Private Function createRadioButton(ByVal sign As String, ByVal group As Integer, ByVal checked As Boolean)
            Dim button As New RadioButton()
            button.Content = sign
            button.FontSize = 14
            button.GroupName = group
            button.IsChecked = checked
            button.Margin = New System.Windows.Thickness(20, 0, 0, 0)
            button.HorizontalAlignment = Windows.HorizontalAlignment.Center
            button.VerticalAlignment = Windows.VerticalAlignment.Center

            Return button
        End Function

        Private Function createLabel(ByVal text As String) As Windows.Controls.Label
            Dim label = New Windows.Controls.Label()
            label.Content = text
            label.FontSize = 14
            label.HorizontalAlignment = Windows.HorizontalAlignment.Center
            label.VerticalAlignment = Windows.VerticalAlignment.Center
            Return label
        End Function

        Private Sub addControltoGrid(ByVal Control As System.Windows.UIElement,
                                     ByVal rowIndex As Integer,
                                     ByVal columnIndex As Integer,
                                     ByVal columnSpan As Integer)

            Windows.Controls.Grid.SetRow(Control, rowIndex)
            Windows.Controls.Grid.SetColumn(Control, columnIndex)
            Windows.Controls.Grid.SetColumnSpan(Control, columnSpan)
            Grid.Children.Add(Control)
        End Sub


        Private Class Recommandation
            ''' <summary>
            ''' Associe à chaque header sa valeur
            ''' </summary>
            Dim _fields As Dictionary(Of Integer, String)
            ''' <summary>
            ''' Liste des radioButtons associés à la recommandation
            ''' </summary>
            Dim _buttons As List(Of RadioButton)

            Sub New(ByVal fields As Dictionary(Of Integer, String),
                    ByVal blist As List(Of RadioButton))
                _fields = fields
                _buttons = blist
            End Sub

            Public ReadOnly Property fields() As Dictionary(Of Integer, String)
                Get
                    Return _fields
                End Get
            End Property

            Public ReadOnly Property buttons() As List(Of RadioButton)
                Get
                    Return _buttons
                End Get
            End Property

            Function getField(ByVal colIndex As Integer) As String
                If _fields.ContainsKey(colIndex) Then
                    Return _fields(colIndex)
                Else
                    Return ""
                End If
            End Function

        End Class

        Private Sub BValider_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BValider.Click
            ' update BDD
            If is_valeur Then
                updateValeurReco()
            Else
                Dim changes As New List(Of KeyValuePair(Of Recommandation, String))

                For Each reco As Recommandation In recommandations
                    For Each b As RadioButton In reco.buttons
                        If b.IsChecked And b.Content <> "" Then
                            changes.Add(New KeyValuePair(Of Recommandation, String)(reco, b.Content))
                        End If
                    Next
                Next

                updateSecteurReco(changes)
            End If

            Me.Close()
        End Sub

        Private Sub BAnnuler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BAnnuler.Click
            Me.Close()
        End Sub

        Private Sub updateSecteurReco(ByVal changes As List(Of KeyValuePair(Of Recommandation, String)))
            Dim co As New Connection()
            Dim id As String
            Dim type As String

            For Each pair As KeyValuePair(Of Recommandation, String) In changes
                id = pair.Key.getField(idFGAIndex)
                type = "FGA"
                If id = "" Then
                    id = pair.Key.getField(idICBIndex)
                    type = "ICB"
                End If

                If id <> "" Then
                    co.ProcedureStockée("ACT_Add_Reco_Secteur",
                                        New List(Of String)({"@id", "@type", "@reco"}),
                                        New List(Of Object)({id, type, pair.Value}))
                End If
            Next

        End Sub

        Private Sub updateValeurReco()
            Dim co As New Connection()
            Dim recos As New Dictionary(Of String, Valeur)

            ' Fusion des différentes valeurs en une seule classe
            For Each reco As Recommandation In recommandations
                Dim isin As String = reco.getField(0)

                If Not recos.ContainsKey(isin) Then
                    recos.Add(isin, New Valeur(isin))
                End If

                For Each b As RadioButton In reco.buttons
                    If b.IsChecked Then
                        Dim newreco As String = b.Content.ToString

                        If newreco = "" Then
                            ' Set old reco
                            newreco = reco.getField(4)
                        ElseIf newreco = "OUT" Then
                            newreco = ""
                        End If

                        Select Case reco.getField(3).ToUpper
                            Case "MXEU"
                                recos(isin).sxxa = newreco
                            Case "MXEM"
                                recos(isin).sxxe = newreco
                            Case "MXEUM"
                                recos(isin).sxxp = newreco
                            Case "MXUSLC"
                                recos(isin).mxuslc = newreco
                        End Select
                    End If
                Next
            Next

            ' Mise à jour des nouvelles recos
            For Each v As Valeur In recos.Values
                co.ProcedureStockée("ACT_Add_Reco_Valeur",
                                New List(Of String)({"@isin", "@reco_SXXE", "@reco_SXXA", "@reco_SXXP", "@reco_MXUSLC"}),
                                New List(Of Object)({v.isin, v.sxxe, v.sxxa, v.sxxp, v.mxuslc}))
            Next
        End Sub

        Private Class Valeur
            Private _isin As String = ""
            Private _sxxe As String = ""
            Private _sxxa As String = ""
            Private _sxxp As String = ""
            Private _mxuslc As String = ""

            Sub New(ByVal isin As String)
                _isin = isin
            End Sub

            Public ReadOnly Property isin() As String
                Get
                    Return _isin
                End Get
            End Property

            Public Property sxxe() As String
                Get
                    Return _sxxe
                End Get

                Set(ByVal value As String)
                    _sxxe = value
                End Set
            End Property

            Public Property sxxa() As String
                Get
                    Return _sxxa
                End Get

                Set(ByVal value As String)
                    _sxxa = value
                End Set
            End Property

            Public Property sxxp() As String
                Get
                    Return _sxxp
                End Get

                Set(ByVal value As String)
                    _sxxp = value
                End Set
            End Property

            Public Property mxuslc() As String
                Get
                    Return _mxuslc
                End Get

                Set(ByVal value As String)
                    _mxuslc = value
                End Set
            End Property

        End Class

    End Class
End Namespace