Imports System.Collections.ObjectModel
Imports System.Text

Namespace Action.Coefficient

    Public Class CoefViewModelSecteur
        Inherits CoefViewModel

        Private _date As String

        Public Sub New()
            MyBase.New()

            initWatchers(New KeyValuePair(Of String, Object)("OtherSecteurs",
                                                             New ObservableCollection(Of SecteurView)),
                         New KeyValuePair(Of String, Object)("OtherCriteres",
                                                             New ObservableCollection(Of SecteurView)))
            MyBase._is_sect = True
            setDate()
            populateOtherSecteurs()
            populateOtherCriteres()
        End Sub

        Public Sub New(isSimul As Boolean)
            MyBase.New(isSimul)

            initWatchers(New KeyValuePair(Of String, Object)("OtherSecteurs",
                                                             New ObservableCollection(Of SecteurView)),
                         New KeyValuePair(Of String, Object)("OtherCriteres",
                                                             New ObservableCollection(Of SecteurView)))
            MyBase._is_sect = True

            setDate()
            populateOtherSecteurs()
            populateOtherCriteres()

        End Sub

#Region "Properties"
        Public Property OtherSecteurs As ObservableCollection(Of SecteurView)
            Get
                Return getWatcher(Of ObservableCollection(Of SecteurView))("OtherSecteurs")
            End Get
            Set(ByVal value As ObservableCollection(Of SecteurView))
                setWatcher("OtherSecteurs", value)
            End Set
        End Property

        Public Property OtherCriteres As ObservableCollection(Of CritereView)
            Get
                Return getWatcher(Of ObservableCollection(Of CritereView))("OtherCriteres")
            End Get
            Set(ByVal value As ObservableCollection(Of CritereView))
                setWatcher("OtherCriteres", value)
            End Set
        End Property
#End Region

#Region "Methodes privees"
        Private Sub setDate()
            _date = co.RequeteSqlToList("SELECT MAX(date) FROM DATA_FACTSET").FirstOrDefault()
        End Sub
#End Region ' !Methodes privees


#Region "Methodes MustOverrides"
        ''' <summary>
        ''' Récupère l'ensemble des critères liés aux secteurs
        ''' </summary>
        Protected Overrides Function getSQLCritere(isSimul As Boolean) As String
            Dim sql As String



            If (isSimul) Then
                sql = "SELECT crit.id_critere, crit.id_parent, crit.nom, crit.position, crit.description, crit.CAP_min, crit.CAP_max, crit.format, crit.precision, crit.groupe, crit.inverse"
                sql = sql & " FROM ACT_COEF_CRITERE_SIMULATION crit"
                sql = sql & " INNER JOIN  ("
                sql = sql & "    SELECT Max(date) AS maxdate, id_critere "
                sql = sql & "    FROM ACT_COEF_SECTEUR_SIMULATION "
                sql = sql & "    GROUP BY id_critere"
                sql = sql & "    UNION"
                sql = sql & "    SELECT MAX(date) AS maxdate, root.id_critere"
                sql = sql & "    FROM ACT_COEF_CRITERE_SIMULATION root"
                sql = sql & "    LEFT OUTER JOIN ACT_COEF_SECTEUR_SIMULATION sect ON sect.id_critere = root.id_critere"
                sql = sql & "    WHERE id_parent IS NULL"
                sql = sql & "    GROUP BY root.id_critere"
                sql = sql & " ) ind ON ind.id_critere = crit.id_critere"
                sql = sql & " LEFT OUTER JOIN ("
                sql = sql & "    SELECT * FROM ACT_COEF_CRITERE_SIMULATION WHERE id_parent is NULL "
                sql = sql & " ) root ON root.id_critere = crit.id_critere"
                sql = sql & " WHERE crit.is_sector = 1"
                sql = sql & " ORDER BY crit.id_parent, crit.position"
            Else
                sql = "SELECT crit.id_critere, crit.id_parent, crit.nom, crit.position, crit.description, crit.CAP_min, crit.CAP_max, crit.format, crit.precision, crit.groupe, crit.inverse"
                sql = sql & " FROM ACT_COEF_CRITERE crit"
                sql = sql & " INNER JOIN  ("
                sql = sql & "    SELECT Max(date) AS maxdate, id_critere "
                sql = sql & "    FROM ACT_COEF_SECTEUR "
                sql = sql & "    GROUP BY id_critere"
                sql = sql & "    UNION"
                sql = sql & "    SELECT MAX(date) AS maxdate, root.id_critere"
                sql = sql & "    FROM ACT_COEF_CRITERE root"
                sql = sql & "    LEFT OUTER JOIN ACT_COEF_SECTEUR sect ON sect.id_critere = root.id_critere"
                sql = sql & "    WHERE id_parent IS NULL"
                sql = sql & "    GROUP BY root.id_critere"
                sql = sql & " ) ind ON ind.id_critere = crit.id_critere"
                sql = sql & " LEFT OUTER JOIN ("
                sql = sql & "    SELECT * FROM ACT_COEF_CRITERE WHERE id_parent is NULL "
                sql = sql & " ) root ON root.id_critere = crit.id_critere"
                sql = sql & " WHERE crit.is_sector = 1"
                sql = sql & " ORDER BY crit.id_parent, crit.position"
            End If

            Return sql
        End Function

        ''' <summary>
        ''' Récupère l'ensemble des critères numériques sur les valeurs.
        ''' </summary>
        Private Sub populateOtherCriteres()
            Dim others As New ObservableCollection(Of CritereView)
            ' Add Criteres
            For Each col As String In co.SelectColonneName("DATA_FACTSET", "float")
                Dim found = False

                For Each crit In CritereList
                    If crit.Name = col Then
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then
                    others.Add(New CritereView(Me, 0, Nothing, col, 0))
                End If
            Next

            OtherCriteres = others
        End Sub

        ''' <summary>
        ''' Récupère les secteurs avec des coefficients.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub populateSecteurs(isSimul As Boolean)
            Dim secteurs As New List(Of SecteurView)
            Dim sql As String
            Dim defaut As New SecteurView(0, "Defaut")

            secteurs.Add(defaut)

            ' Select sectors
            If (isSimul) Then
                sql = "SELECT coef.id_fga AS id, sect.label As name "
                sql = sql & " FROM ACT_COEF_SECTEUR_SIMULATION coef "
                sql = sql & " 	LEFT OUTER JOIN ref_security.SECTOR sect ON sect.code = coef.id_fga "
                sql = sql & " WHERE sect.class_name='FGA_EU'"
                sql = sql & " GROUP BY coef.id_fga, sect.label "
                sql = sql & " ORDER BY coef.id_fga"
            Else
                sql = "SELECT coef.id_fga AS id, sect.label As name "
                sql = sql & " FROM ACT_COEF_SECTEUR coef "
                sql = sql & " 	LEFT OUTER JOIN ref_security.SECTOR sect ON sect.code = coef.id_fga "
                sql = sql & " WHERE sect.class_name='FGA_EU'"
                sql = sql & " GROUP BY coef.id_fga, sect.label "
                sql = sql & " ORDER BY coef.id_fga"
            End If
            ' Add sectors
            For Each dico As Dictionary(Of String, Object) In co.sqlToListDico(sql)
                If dico.ContainsKey("name") Then
                    secteurs.Add(New SecteurView(dico("id"), dico("name")))
                End If
            Next

            SecteurList = secteurs
        End Sub

        ''' <summary>
        ''' Récupère l'ensemble des secteurs.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub populateOtherSecteurs()
            Dim others As New ObservableCollection(Of SecteurView)
            Dim sql As String

            ' Select sectors
            sql = "SELECT * FROM ref_security.SECTOR WHERE class_name='FGA_EU'"

            ' Add sectors
            For Each dico As Dictionary(Of String, Object) In co.sqlToListDico(sql)
                If dico.ContainsKey("label") Then
                    Dim found = False

                    For Each Secteur In SecteurList
                        If Secteur.Libelle = dico("label") Then
                            found = True
                            Exit For
                        End If
                    Next

                    If Not found Then
                        others.Add(New SecteurView(dico("code"), dico("label")))
                    End If
                End If
            Next

            OtherSecteurs = others
        End Sub

        ''' <summary>
        ''' Récupère les coefficients des différents secteurs sur un critère donnée.
        ''' </summary>
        Protected Overrides Function getCoefsFromCritere(ByVal elt As CritereView, isSimul As Boolean) As ObservableCollection(Of CoefView)
            Dim list As New ObservableCollection(Of CoefView)
            Dim sql As String

            For Each sect As SecteurView In SecteurList

                If (isSimul) Then
                    sql = "SELECT coef"
                    sql = sql & " FROM ACT_COEF_SECTEUR_SIMULATION coef"
                    sql = sql & " INNER JOIN (  SELECT id_critere, id_fga, MAX(date) AS maxdate "
                    sql = sql & "               FROM ACT_COEF_SECTEUR_SIMULATION"
                    sql = sql & "               WHERE id_critere=" & elt.Id & " AND id_fga" & IIf(sect.Id = 0,
                                                                                                  " IS NULL",
                                                                                                  "=" & sect.Id)
                    sql = sql & "               GROUP BY id_critere, id_fga"
                    sql = sql & "            ) new ON new.maxdate=coef.date AND new.id_critere=coef.id_critere"
                    sql = sql & " WHERE coef.id_critere=" & elt.Id & " AND coef.id_fga" & IIf(sect.Id = 0,
                                                                                              " IS NULL",
                                                                                              "=" & sect.Id)
                    sql = sql & " GROUP BY coef"
                Else
                    sql = "SELECT coef"
                    sql = sql & " FROM ACT_COEF_SECTEUR coef"
                    sql = sql & " INNER JOIN (  SELECT id_critere, id_fga, MAX(date) AS maxdate "
                    sql = sql & "               FROM ACT_COEF_SECTEUR"
                    sql = sql & "               WHERE id_critere=" & elt.Id & " AND id_fga" & IIf(sect.Id = 0,
                                                                                                  " IS NULL",
                                                                                                  "=" & sect.Id)
                    sql = sql & "               GROUP BY id_critere, id_fga"
                    sql = sql & "            ) new ON new.maxdate=coef.date AND new.id_critere=coef.id_critere"
                    sql = sql & " WHERE coef.id_critere=" & elt.Id & " AND coef.id_fga" & IIf(sect.Id = 0,
                                                                                              " IS NULL",
                                                                                              "=" & sect.Id)
                    sql = sql & " GROUP BY coef"
                End If

                Dim res As List(Of Object) = co.RequeteSqlToList(sql)

                If res.Count = 0 Then
                    list.Add(New CoefView(0, Me, sect, elt))
                Else
                    list.Add(New CoefView(res.FirstOrDefault, Me, sect, elt))
                End If
            Next

            Return list
        End Function

        ''' <summary>
        ''' Sauvegarde un coefficient.
        ''' </summary>
        Public Overrides Sub saveToBDD(ByVal coefView As CoefView, isSimul As Boolean)
            If (isSimul) Then
                co.ProcedureStockée("ACT_Add_Coef_Secteur_Simulation",
                                    New List(Of String)({"@id_critere",
                                                         "@id_fga",
                                                         "@coef",
                                                         "@date"}),
                                    New List(Of Object)({coefView.Critere.Id,
                                                         coefView.Secteur.Id,
                                                         coefView.Data,
                                                         _date}))
            Else
                co.ProcedureStockée("ACT_Add_Coef_Secteur",
                                    New List(Of String)({"@id_critere",
                                                         "@id_fga",
                                                         "@coef",
                                                         "@date"}),
                                    New List(Of Object)({coefView.Critere.Id,
                                                         coefView.Secteur.Id,
                                                         coefView.Data,
                                                         _date}))
            End If

        End Sub

        Protected Overrides Function getTabName(isSimul As Boolean) As String
            If (isSimul) Then
                Return "ACT_COEF_SECTEUR_SIMULATION"
            End If
            Return "ACT_COEF_SECTEUR"
        End Function

#End Region ' !Methodes MustOverrides

#Region "Methodes publiques"
        Public Sub RemoveCritere(ByVal critere As CritereView)
            If critere.Parent IsNot Nothing Then
                critere.Parent.Children.Remove(critere)

                For Each crit In critere.Parent.Children.Where(Function(x) x.Position >= critere.Position)
                    crit.Position -= 1
                Next
            End If

            Me.CritereList.Remove(critere)

            If critere.IsLeaf Then
                ' Insert critere keeping otherCritere order.
                Dim pos = 0

                While pos < Me.OtherCriteres.Count _
                    AndAlso String.Compare(critere.Name, Me.OtherCriteres(pos).Name) > 0

                    pos += 1
                End While

                Me.OtherCriteres.Insert(pos, critere)
            End If
        End Sub


        ''' <summary>
        ''' export en feuille excel
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub ExportToExcel()
            Try
                ' recuperer la liste de CritereView et transformer en DataTable puis DataSet
                ' une liste d object : .Id,  et un Dictionnary clé est le  nom du secteur, et le coefficient en data
                Dim CritereExportExcel As List(Of ExcelView) = New List(Of ExcelView)
                'Fusionner la liste de la description des criteres CritereView avec les données des coefficients par secteur
                For Each elt As CritereView In CritereList
                    Dim CoefSecteurDico As Dictionary(Of String, String) = New Dictionary(Of String, String)
                    Dim c As CoefView = Nothing
                    For Each c1 As CoefView In elt.Coefs
                        c = c1
                        If c.Secteur Is Nothing Then
                            CoefSecteurDico.Add("Defaut", c.Data)
                        Else
                            CoefSecteurDico.Add(c.Secteur.Libelle, c.Data)
                        End If

                    Next
                    CritereExportExcel.Add(New ExcelView With {.view = elt, .data = CoefSecteurDico})

                Next


                Dim dt As System.Data.DataTable = ExportExcel.ConvertToDatatable(Of ExcelView)(CritereExportExcel, 1)

                Dim ds As DataSet = New DataSet("Criteres")
                ds.Tables.Add(dt)

                ExportExcel.ExportToExcel(ds)

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End Sub

        Private Class ExcelView
            Public view As CritereView
            Public data As Dictionary(Of String, String)
        End Class
#End Region ' !Methodes publiques

    End Class
End Namespace