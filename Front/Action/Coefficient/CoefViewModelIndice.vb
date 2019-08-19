Imports System.Collections.ObjectModel

Namespace Action.Coefficient

    Public Class CoefViewModelIndice
        Inherits CoefViewModel

#Region "Methodes MustOverrides"
        ''' <summary>
        ''' Récupère l'ensemble des critères liés aux indices
        ''' </summary>
        Protected Overrides Function getSQLCritere(isSimul As Boolean) As String
            Dim sql As String

            sql = "SELECT crit.id_critere, crit.id_parent, crit.nom, crit.position, crit.description"
            sql = sql & " FROM ACT_COEF_CRITERE crit"
            'sql = sql & " INNER JOIN  ("
            'sql = sql & "    SELECT Max(date) AS maxdate, id_critere "
            'sql = sql & "    FROM ACT_COEF_INDICE "
            'sql = sql & "    GROUP BY id_critere"
            'sql = sql & " ) ind ON ind.id_critere = crit.id_critere"
            sql = sql & " WHERE crit.is_sector = 0"
            sql = sql & " ORDER BY crit.id_parent, crit.position"

            Return sql
        End Function

        ''' <summary>
        ''' Récupère l'ensemble des secteurs.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub populateSecteurs(isSimul As Boolean)
            Dim secteurs As New List(Of SecteurView)
            Dim sql As String

            If (isSimul) Then
                ' Select sectors
                sql = "SELECT coef.id_indice AS id, ind.libelle As name"
                sql = sql & " FROM ACT_COEF_INDICE_SIMULATION coef"
                sql = sql & " LEFT OUTER JOIN ACT_INDICE ind ON ind.id = coef.id_indice"
                sql = sql & " GROUP BY coef.id_indice, ind.libelle"
                sql = sql & " ORDER BY coef.id_indice"
            Else
                ' Select sectors
                sql = "SELECT coef.id_indice AS id, ind.libelle As name"
                sql = sql & " FROM ACT_COEF_INDICE coef"
                sql = sql & " LEFT OUTER JOIN ACT_INDICE ind ON ind.id = coef.id_indice"
                sql = sql & " GROUP BY coef.id_indice, ind.libelle"
                sql = sql & " ORDER BY coef.id_indice"

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
        ''' Récupère les coefficients des différents secteurs sur un critère donnée.
        ''' </summary>
        Protected Overrides Function getCoefsFromCritere(ByVal elt As CritereView, isSimul As Boolean) As ObservableCollection(Of CoefView)
            Dim list As New ObservableCollection(Of CoefView)
            Dim sql As String

            For Each sect As SecteurView In SecteurList
                If (isSimul) Then
                    sql = "SELECT coef"
                    sql = sql & " FROM ACT_COEF_INDICE_SIMULATION coef"
                    sql = sql & " INNER JOIN (SELECT id_critere, id_indice, MAX(date) AS maxdate FROM ACT_COEF_INDICE_SIMULATION GROUP BY id_critere, id_indice) new ON new.maxdate=coef.date AND new.id_critere=coef.id_critere"
                    sql = sql & " WHERE coef.id_critere=" & elt.Id
                    If sect.Id = 0 Then
                        sql = sql & " AND coef.id_indice IS NULL"
                    Else
                        sql = sql & " AND coef.id_indice=" & sect.Id
                    End If
                    sql = sql & " GROUP BY coef"
                Else
                    sql = "SELECT coef"
                    sql = sql & " FROM ACT_COEF_INDICE coef"
                    sql = sql & " INNER JOIN (SELECT id_critere, id_indice, MAX(date) AS maxdate FROM ACT_COEF_INDICE GROUP BY id_critere, id_indice) new ON new.maxdate=coef.date AND new.id_critere=coef.id_critere"
                    sql = sql & " WHERE coef.id_critere=" & elt.Id
                    If sect.Id = 0 Then
                        sql = sql & " AND coef.id_indice IS NULL"
                    Else
                        sql = sql & " AND coef.id_indice=" & sect.Id
                    End If
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
            If coefView.Data = 0 Then
                If coefView.Secteur.Id = 0 Then
                    co.DeleteWheres("ACT_COEF_INDICE",
                                New List(Of String)({"id_critere",
                                                     "id_indice"}),
                                New List(Of Object)({coefView.Critere.Id,
                                                     Nothing}))
                Else
                    co.DeleteWheres("ACT_COEF_INDICE",
                                New List(Of String)({"id_critere",
                                                     "id_indice"}),
                                New List(Of Object)({coefView.Critere.Id,
                                                     coefView.Secteur.Id}))
                End If

            Else
                co.ProcedureStockée("ACT_Add_Coef_Indice",
                                    New List(Of String)({"@id_critere",
                                                         "@id_indice",
                                                         "@coef"}),
                                    New List(Of Object)({coefView.Critere.Id,
                                                         coefView.Secteur.Id,
                                                         coefView.Data}))
            End If
        End Sub

        Protected Overrides Function getTabName(isSimul As Boolean) As String
            If (isSimul) Then
                Return "ACT_COEF_INDICE_SIMULATION"
            End If
            Return "ACT_COEF_INDICE"
        End Function

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
#End Region ' !Methodes MustOverrides

    End Class
End Namespace