Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection 
Imports Bloomberglp.Blpapi
Imports [Request] = Bloomberglp.Blpapi.Request
Imports [Service] = Bloomberglp.Blpapi.Service
Imports [Element] = Bloomberglp.Blpapi.Element

Public Class AllocationGrille

    Dim Connection As New Connection()
    Dim DGVC_poids As New DataGridViewColumn()
    Dim Cell_poids As New DataGridViewTextBoxCell()
    Dim dataSourceGlobal As New DataTable
    Dim SessionStarted As Boolean 'Savoir si une session Bloom est déja ouverte 

    Private Const SERVICEBLOOM As String = "//blp/mktdata" 'Service bloom cours temps réel
    Private Const APIREFDATA_SVC = "//blp/refdata" 'Service bloom cours historique
    Private Const NB_INDICES_REFERENCE = 9


#Region "Fonctions appel WinForm"


    ''' <summary>
    ''' Initialisation d'une liste dans laquelle on lui insère les indices de références
    ''' </summary>
    ''' <param name="MaList"></param>
    ''' <remarks></remarks>
    Sub InitilisationListIndice(ByRef MaList As List(Of List(Of String)))

        MaList.Add(New List(Of String))
        MaList.Add(New List(Of String))

        MaList(0).Add("SXXT Index")
        MaList(0).Add("SXXB Index")
        MaList(0).Add("SPTRTE Index")
        MaList(0).Add("MSDEPN Index")
        MaList(0).Add("MSDEEEMN Index")
        MaList(0).Add("BT5ATREU Index")
        MaList(0).Add("QW5A Index")
        MaList(0).Add(".FEDINF Index")
        MaList(0).Add("EONCAPL7 Index")

        'On initialise les valeurs de retour à nul
        For i = 0 To NB_INDICES_REFERENCE - 1
            MaList(1).Add(Nothing)
        Next

    End Sub

    ''' <summary>
    ''' Démarre les différents calculs
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CalculPerf_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalculPerf.Click
        PilotageCalcul()
    End Sub


    ''' <summary>
    ''' Se produit lorsque l'on change le nom du ptf 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Radio_Bloom_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Radio_Bloom.CheckedChanged
        'Quand on change la valeur du checkBox, on grise la date de Fin puisqu'on fonctionne en temps réel. 
        If Radio_Bloom.Checked Then
            CalFin.Visible = False
            lbl_DateFin.Visible = False
            CalDebut.Visible = False
            lbl_DateDeb.Visible = False
        Else
            CalFin.Visible = True
            lbl_DateFin.Visible = True
            CalDebut.Visible = True
            lbl_DateDeb.Visible = True
        End If

    End Sub

    Private Sub AllocationGrille_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Connection.ToConnectBase()
        'Par défault on se connecte à Omega
        Radio_Bloom.Checked = True
        'Dim DateDernierCours As Date
        'DateDernierCours = Connection.RequeteSqlToList("Select distinct max(date) from strat_valeur_indice").Item(0)

        'For Each DateAllocation In Connection.SelectDistinctSimple("strat_allocation", "Date")
        '    If DateAllocation <= DateDernierCours Then
        '        CalDebut.AddBoldedDate(DateAllocation)
        '        CalFin.AddBoldedDate(DateAllocation)
        '    End If
        'Next

        For Each DateAllocation In Connection.SelectDistinctSimple("strat_valorisation_Mandat", "Datedeb")
            CalDebut.AddBoldedDate(DateAllocation)
        Next
        For Each DateAllocation In Connection.SelectDistinctSimple("strat_valorisation_Mandat", "DateFin")
            CalFin.AddBoldedDate(DateAllocation)
        Next
        CalFin.UpdateBoldedDates()
        CalDebut.UpdateBoldedDates()

        Cb_Groupe.Items.Add("TOUS")
        For Each groupe In Connection.SelectDistinctSimple("strat_allocation", "Groupe")
            Cb_Groupe.Items.Add(groupe)
        Next
        Cb_Groupe.SelectedItem = "TOUS"
    End Sub

#End Region

    ''' <summary>
    ''' Permets de démarrer les calculs de performances en temps réel ou entre 2 dates 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PilotageCalcul()
        Dim Datedeb, DateFin As Date
        Dim result As List(Of Object) = New List(Of Object)
        Dim Requete As String

        Datedeb = CalDebut.SelectionRange.Start
        DateFin = CalFin.SelectionRange.Start

        If Radio_Bloom.Checked Then
            'On fonctionne en temps réel 
            Datedeb = Connection.RequeteSqlToList("Select distinct max(datefin) from strat_valorisation_mandat").Item(0)
            CalculPerfTempsReel(Datedeb)

        ElseIf DateFin = Datedeb Then
            'Sinon on vérifie la validité des dates
            MessageBox.Show("Veuillez saisir 2 dates différentes ", "Error", MessageBoxButtons.OK)
        ElseIf DateFin < Datedeb Then
            MessageBox.Show("Veuillez saisir une date de début inférieur à la date de fin", "Error", MessageBoxButtons.OK)
        Else

            'Requete = Connection.LectureFichierSql("Strat_TraitementIndice.sql")
            'Requete = Replace(Requete, "#paramdeb", Datedeb.ToString("dd/MM/yyyy"))
            'Requete = Replace(Requete, "#paramfin", DateFin.ToString("dd/MM/yyyy"))

            'On vérifie quel type de calcul on doit faire

            'Il s'agit d'une perf entre 2 date => Il s'agit d'affichage
            result = Connection.commandeSqlnoInsert("select groupe,id_grille,valeur_boursiere, Part_PTF,BenchReel,EcartBench, PerfPoche, Plue_Value from strat_valorisation_mandat where dateDeb = '" & Datedeb.ToString("dd/MM/yyyy") & "' and Datefin ='" & DateFin.ToString("dd/MM/yyyy") & "'order by groupe desc ")
            If result.Count = 0 Then
                'Il n'y a pas d'enregistrement en BDD -> On doit calculer la perf entre ces 2 dates
                Requete = Connection.LectureFichierSql("Strat_CalculPerfBetween2dates.sql")
                Requete = Replace(Requete, "#ParamDeb", Datedeb)
                Requete = Replace(Requete, "#ParamFin", DateFin)
                result = Connection.commandeSqlnoInsert(Requete)
            End If
            'Else
            'result = 


            AfficherResultat(result)
        End If
        DG_Allocation.Refresh()

    End Sub


    Sub AfficherResultat(ByRef ResultatRequete As List(Of Object))
        'Dim Mydataset As New DataSet
        Dim colname As New DataGridViewColumn
        Dim noRow, NoAttribut As Integer
        Dim row As DataRow
        Dim mydatatable As New DataTable
        Dim ValeursTpsReel As New List(Of List(Of String))
        Dim AnciennesValeurs As New List(Of List(Of String))

        DG_Allocation.DataSource = Nothing

        mydatatable.BeginLoadData()
        Try

            'On ajoute les titres des columnes
            For Each colonne In ResultatRequete(0)
                'DG_Allocation.Columns.Add(colonne, colonne)
                mydatatable.Columns.Add(colonne)
            Next
            mydatatable.Columns("BenchReel").ColumnName = mydatatable.Columns("BenchReel").ColumnName & vbCrLf & "(En %)"
            mydatatable.Columns("Part_Ptf").ColumnName = mydatatable.Columns("Part_Ptf").ColumnName & vbCrLf & "(En %)"
            mydatatable.Columns("EcartBench").ColumnName = mydatatable.Columns("EcartBench").ColumnName & vbCrLf & "(En %)"
            mydatatable.Columns("PerfPoche").ColumnName = mydatatable.Columns("PerfPoche").ColumnName & vbCrLf & "(En %)"
            mydatatable.Columns("Plue_Value").ColumnName = mydatatable.Columns("Plue_Value").ColumnName & vbCrLf & "(En point de base)"

            ResultatRequete.RemoveRange(0, 1)

            noRow = 0
            NoAttribut = 0
            'On ajoute les lignes
            For Each lignes In ResultatRequete
                row = mydatatable.NewRow()
                For Each attribut In lignes
                    If NoAttribut >= 2 And NoAttribut < 7 Then
                        ' Si c'est 0 on n'applique pas le format si on à 0 (pour ne pas avoir de 0.00)
                        If attribut <> 0 Then
                            row(NoAttribut) = FormatNumber(attribut)
                        Else
                            row(NoAttribut) = attribut
                        End If
                    Else
                        row(NoAttribut) = attribut
                    End If
                    NoAttribut = NoAttribut + 1
                Next
                mydatatable.Rows.Add(row)

                NoAttribut = 0

            Next
            mydatatable.EndLoadData()
            mydatatable.DefaultView.Sort = "Groupe, Id_grille"
            dataSourceGlobal = mydatatable

            DG_Allocation.DataSource = mydatatable
            DG_Allocation.Refresh()
        Catch ex As Exception
            MessageBox.Show("Impossible de calculer une performance ", " Error", MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub Cb_Groupe_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cb_Groupe.SelectedIndexChanged

        'Variable pour calcul total Action
        Dim Action As Boolean
        Dim ActionsExiste As Boolean
        Dim Act_ValAction As Double
        Dim Act_PartPtf As Single
        Dim Act_PartBench As Single
        Dim Act_EcartBench As Single
        Dim Act_PerfGrille As Single
        Dim Act_Plus_Value As Single
        Dim NoAct As Integer

        'Variable pour calcul total Oblig
        Dim Oblig As Boolean
        Dim ObligExiste As Boolean
        Dim Oblig_ValAction As Double
        Dim Oblig_PartPtf As Single
        Dim Oblig_PartBench As Single
        Dim Oblig_EcartBench As Single
        Dim Oblig_PerfGrille As Single
        Dim Oblig_Plus_Value As Single
        Dim NoOblig As Integer

        Dim fbold As New Font("Arial", 10, FontStyle.Bold)
        Dim row As DataRowView
        Dim newrow As DataRow
        Dim tableSource As New DataTable


        If DG_Allocation.Rows.Count = 0 Then Exit Sub

        'Initialisation variables action
        Action = False 'Variable permettant de savoir si il y  a au moins une grille action pour un PTF
        ActionsExiste = False 'Pour ne pas calculer à chaque fois la grille action
        Act_ValAction = 0
        Act_PartPtf = 0
        Act_PartBench = 0
        Act_EcartBench = 0
        Act_PerfGrille = 0
        Act_Plus_Value = 0
        'Initiatisation variable oblig
        Oblig = False
        ObligExiste = False
        Oblig_ValAction = 0
        Oblig_PartPtf = 0
        Oblig_PartBench = 0
        Oblig_EcartBench = 0
        Oblig_PerfGrille = 0
        Oblig_Plus_Value = 0

        NoAct = 0
        NoOblig = 0
        tableSource = dataSourceGlobal
        Dim filtre As New DataView(tableSource)

        If Cb_Groupe.Text.Equals("TOUS") Then
            filtre.RowFilter = ""
        Else
            filtre.RowFilter = "Groupe = '" & Cb_Groupe.Text & "'"
        End If
        DG_Allocation.DataSource = filtre


        For norow = 0 To filtre.Count - 1
            row = filtre.Item(norow)
            'Si type action alors on fait la somme
            If InStr(row(1), "Act") Then
                If Not row(1).Equals("Actions") Then
                    NoAct = NoAct + 1
                    Action = True
                    Act_ValAction = Act_ValAction + row(2)
                    Act_PartPtf = Act_PartPtf + row(3)
                    Act_PartBench = Act_PartBench + row(4)
                    Act_EcartBench = Act_EcartBench + row(5)
                    Act_PerfGrille = Act_PerfGrille + (row(6) * row(3))
                    Act_Plus_Value = Act_Plus_Value + row(7)
                Else
                    ActionsExiste = True
                End If
            ElseIf InStr(row(1), "Obl") Then
                If Not row(1).Equals("Obligations") Then
                    NoOblig = NoOblig + 1
                    Oblig = True
                    Oblig_ValAction = Oblig_ValAction + row(2)
                    Oblig_PartPtf = Oblig_PartPtf + row(3)
                    Oblig_PartBench = Oblig_PartBench + row(4)
                    Oblig_EcartBench = Oblig_EcartBench + row(5)
                    Oblig_PerfGrille = Oblig_PerfGrille + (row(6) * row(3))
                    Oblig_Plus_Value = Oblig_Plus_Value + row(7)
                Else
                    ObligExiste = True
                End If
            End If
        Next

        If DG_Allocation.Rows.Count > 0 And Action And Not ActionsExiste Then

            newrow = tableSource.NewRow
            newrow(0) = Cb_Groupe.Text
            newrow(1) = "Actions"
            newrow(2) = FormatNumber(Act_ValAction)
            newrow(3) = FormatNumber(Act_PartPtf)
            newrow(4) = FormatNumber(Act_PartBench)
            newrow(5) = FormatNumber(Act_EcartBench)
            newrow(6) = FormatNumber(Act_PerfGrille / Act_PartPtf)
            newrow(7) = (Act_EcartBench * (Act_PerfGrille / Act_PartPtf)) / 100
            tableSource.Rows.Add(newrow)

            'DG_Allocation.Rows("Actions").DefaultCellStyle.Font = fbold
            'DG_Allocation.DataSource = tableSource
        End If

        If DG_Allocation.Rows.Count > 0 And Oblig And Not ObligExiste Then

            newrow = tableSource.NewRow
            newrow(0) = Cb_Groupe.Text
            newrow(1) = "Obligations"
            newrow(2) = FormatNumber(Oblig_ValAction)
            newrow(3) = FormatNumber(Oblig_PartPtf)
            newrow(4) = FormatNumber(Oblig_PartBench)
            newrow(5) = FormatNumber(Oblig_EcartBench)
            newrow(6) = FormatNumber(Oblig_PerfGrille / Oblig_PartPtf)
            newrow(7) = (Oblig_EcartBench * (Oblig_PerfGrille / Oblig_PartPtf)) / 100
            tableSource.Rows.Add(newrow)

            'DG_Allocation.Rows("Actions").DefaultCellStyle.Font = fbold
            'DG_Allocation.DataSource = tableSource
        End If

        DG_Allocation.DataSource = filtre.ToTable
        'If Radio_Bloom.Checked Then
        'PilotageCalcul()
        'MessageBox.Show("Affichage des titres selon les cours de clotures d'Omega ", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        'End If

        If Not Cb_Groupe.Text.Equals("TOUS") Then
            refreshBold(NoAct, NoOblig)
        End If


    End Sub

    ''' <summary>
    ''' Cette fonction calcule la somme des différentes poches (Actions, Oblig, Monétaire) 
    ''' Ces lignes seront en gras
    ''' </summary>
    ''' <param name="noAction"></param>
    ''' <param name="noOblig"></param>
    ''' <remarks></remarks>
    Sub refreshBold(ByVal noAction As Integer, ByVal noOblig As Integer)

        Dim obligation, monetaire As Integer
        Dim tablesource As DataTable
        Dim fbold As New Font("Arial", 10, FontStyle.Bold)
        Dim i As Integer 'valeur pour trier par ordre
        Dim noLigne As Integer
        Dim ActionExiste, ObligExiste, MonetaireExiste As Boolean

        ActionExiste = False
        ObligExiste = False
        MonetaireExiste = False
        obligation = 0
        monetaire = 0
        noLigne = 0
        tablesource = DG_Allocation.DataSource
        Dim filtre As New DataView(tablesource)

        tablesource.Columns.Add("Tri", GetType(Double))
        DG_Allocation.Columns("Tri").Visible = False
        'Premiere boucle pour affecter des valeurs et pouvoir trier le Datagrid
        For i = 0 To filtre.Count - 1
            DG_Allocation.Rows(i).Cells("Tri").Value = i
            noLigne = noLigne + 1
            If DG_Allocation.Rows(i).Cells("id_grille").Value.Equals("Actions") Then
                ActionExiste = True
                tablesource.Rows(i).Item("Tri") = 0
            ElseIf DG_Allocation.Rows(i).Cells("id_grille").Value.Equals("Obligations") Then
                ObligExiste = True
                tablesource.Rows(i).Item("Tri") = noAction + 0.5 '0.5 pour avoir une valeur comprise entre le nb de ligne actions et la ligne obligation
            ElseIf DG_Allocation.Rows(i).Cells("id_grille").Value.Equals("Monetaire") Then
                MonetaireExiste = True
                monetaire = i
                tablesource.Rows(i).Item("Tri") = noAction + noOblig + 2 'Nb ligne action + nb ligne oblig + 2 lignes 'actions' et 'obligation' que l'on ajoute manuellement
            Else
                tablesource.Rows(i).Item("Tri") = noLigne
            End If

        Next
        DG_Allocation.Sort(DG_Allocation.Columns("Tri"), System.ComponentModel.ListSortDirection.Ascending)

        'Seconde boucle pour mettre en gras les principales lignes
        For i = 0 To filtre.Count - 1
            If DG_Allocation.Rows(i).Cells(1).Value.Equals("Actions") Or DG_Allocation.Rows(i).Cells(1).Value.Equals("Obligations") Or DG_Allocation.Rows(i).Cells(1).Value.Equals("Monetaire") Then
                DG_Allocation.Rows(i).DefaultCellStyle.Font = fbold
            End If
        Next

        DG_Allocation.Refresh()

    End Sub

#Region "Temps réel (Bloom)"


    ''' <summary>
    ''' Recherche des données historiques de bloomberg
    ''' La date correspond à la date la plus récente la table Strat_valeur_Indice
    ''' </summary>
    ''' <param name="DateRecherche"></param>
    ''' <returns>Liste des indices reférence et leur valeur associée </returns>
    ''' <remarks></remarks>
    Function RechercheDonnéesAncienneBloom(ByVal DateRecherche As Date) As List(Of List(Of String))

        Dim result As New List(Of List(Of String))
        Dim serverHost As String = "localhost"
        Dim serverPort As Integer = 8194
        Dim sessionOptions As New SessionOptions()
        sessionOptions.ServerHost = serverHost
        sessionOptions.ServerPort = serverPort
        Dim Session As New Session(sessionOptions)
        Dim ExitWhile As Boolean

        InitilisationListIndice(result)

        'SIMULATION DE PRESENCE DE BLOOMBERG IF à SUPPRIMER

        SessionStarted = Session.Start
        'Connection à la session 
        If Not SessionStarted Then
            MessageBox.Show("Impossible de lancer une session", "Error", MessageBoxButtons.OK)
            Return Nothing
            Exit Function
        End If
        'Récupération des cours historiques 
        If Not Session.OpenService(APIREFDATA_SVC) Then
            MessageBox.Show("Impossible de se connecter à BloomBerg", "Error", MessageBoxButtons.OK)
            Return Nothing
            Exit Function
        End If


        Dim refDataService As Service = Session.GetService(APIREFDATA_SVC)

        Dim request As Request = refDataService.CreateRequest("HistoricalDataRequest")


        For i = 0 To NB_INDICES_REFERENCE - 1
            request.Append("securities", result(0).Item(i))
        Next

        request.Append("fields", "PX_LAST") ' Valeurs qui seront retournées

        request.Set("periodicitySelection", "DAILY") 'Fréquence d'intervalle de remonté des valeurs
        Dim startdate As String = DateRecherche.ToString("yyyyMMdd")
        Dim Enddate As String = (DateRecherche.AddDays(1)).ToString("yyyyMMdd")
        request.Set("startDate", startdate)
        request.Set("endDate", Enddate)

        Session.SendRequest(request, Nothing)

        While Not ExitWhile
            Dim eventObj As [Event] = Session.NextEvent()
            For Each msg As Message In eventObj.GetMessages()
                If eventObj.Type = [Event].EventType.PARTIAL_RESPONSE Or eventObj.Type = [Event].EventType.RESPONSE Then
                    Dim MyArray As Element
                    MyArray = msg.AsElement.Elements.ElementAt(0).Elements.ElementAt(3).Item(0)
                    Dim titre As String = msg.AsElement.Elements.ElementAt(0).Elements.ElementAt(0).Item(0)
                    Dim Indice As Integer = result(0).IndexOf(titre)
                    result(1).Item(Indice) = MyArray.Elements.ElementAt(1).Item(0).ToString

                    For i = 0 To NB_INDICES_REFERENCE - 1
                        'Par defaut on ne peut sortir du while.
                        'Si on a un champs de vide on sortira du for. 
                        ExitWhile = False
                        If result(1).Item(i) Is Nothing Then Exit For
                        ExitWhile = True
                    Next

                End If

            Next
        End While

        Return result
    End Function


    ''' <summary>
    ''' 'Fonction de connexion à Bloom et de récupération des derniers cours dans Bloomberg
    ''' </summary>
    ''' <param name="Resultat"></param>
    ''' <returns>
    ''' True si on récupérer TOUTES les valeurs
    ''' False sinon (pb de connexion, titre introuvable...
    ''' </returns>
    ''' <remarks></remarks>
    Function RechercheCoursTempsReel(ByRef Resultat As List(Of List(Of String))) As Boolean

        Try
            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194
            Dim sessionOptions As New SessionOptions()
            sessionOptions.ServerHost = serverHost
            sessionOptions.ServerPort = serverPort
            Dim Session As New Session(sessionOptions)
            'tableau à deux dimensions : une pour la valeur de l'indice  et l'autre pour la valeur du dernier cours

            Dim ExitWhile As Boolean


            'Initialisation du tableau de résultat
            'On ajoute 
            InitilisationListIndice(Resultat)

            'On lance la session
            SessionStarted = Session.Start
            'Connection à la session 
            If Not SessionStarted Then
                MessageBox.Show("Impossible de lancer une session", "Error", MessageBoxButtons.OK)
                Return False
                Exit Function
            End If

            'Connection au service Bloom pour le temps réel
            If Not Session.OpenService(SERVICEBLOOM) Then
                MessageBox.Show("Impossible de se connecter à Bloomberg", "Error", MessageBoxButtons.OK)
                Return False
                Exit Function
            End If

            'Récupération des cours en temps réel
            'Liste dans laquelle on va mettre les tickers auxquels ont souscrit
            Dim subscriptions As New List(Of Subscription)()
            'Dim security As String = "SXXT Index"
            Dim mySubscription As Subscription

            For i = 0 To NB_INDICES_REFERENCE - 1
                mySubscription = New Subscription(Resultat(0).Item(i), "LAST_PRICE", 0, New CorrelationID(Resultat(0).Item(i)))
                'Ajout de chaque titres auxquels on s'abonne
                subscriptions.Add(mySubscription)
            Next

            'on lance la souscription au service
            Session.Subscribe(subscriptions)

            While Not ExitWhile
                Dim eventObj As [Event] = Session.NextEvent()
                For Each msg As Message In eventObj.GetMessages()
                    'MessageBox.Show(msg.ToString)
                    Try
                        If eventObj.Type = [Event].EventType.SUBSCRIPTION_DATA And Not (msg.AsElement.GetElement("LAST_PRICE") Is Nothing) Then
                            Dim topic As String = DirectCast(msg.CorrelationID.[Object], String)
                            Dim Indice As Integer = Resultat(0).IndexOf(topic)
                            Resultat(1).Item(Indice) = msg.AsElement("LAST_PRICE").ToString
                            For i = 0 To NB_INDICES_REFERENCE - 1
                                'Par defaut on ne peut sortir du while.
                                'Si on a un champs de vide on sortira du for. 
                                ExitWhile = False
                                If Resultat(1).Item(i) Is Nothing Then Exit For
                                ExitWhile = True
                            Next
                            'Exit While
                        End If
                        'On boucle 8 fois pour savoir 

                    Catch ex As Exception

                    End Try
                    'MessageBox.Show(msg.GetElement("LAST_PRICE").ToString())
                Next
            End While

            Return True
            'Quand le traitement est terminé, on a récupéré la valeur des derniers indices


            Return True
        Catch ex As Exception
            MessageBox.Show("Impossible de lancer une session", "Error", MessageBoxButtons.OK)
            Return False
        Finally

        End Try

    End Function




    'Calcul de la performance entre les anciennes et nouvelles valeur
    Private Sub CalculPerfTempsReel(ByVal dateDeb As Date)
        'On récupère les cours en temps réel si besoin
        Dim AnciennesValeurs As New List(Of List(Of String))
        Dim ValeursTpsReel As New List(Of List(Of String))
        Dim Requete As String
        Dim Result As List(Of Object)

        AnciennesValeurs = RechercheDonnéesAncienneBloom(dateDeb)
        If AnciennesValeurs Is Nothing Then
            'On le bascule vers le "mode" Omega car il n'a pas d'accès à BloomBerg
            Radio_Omega.Checked = True
            MessageBox.Show("Impossible de récupérer les cours Bloomberg", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If Not RechercheCoursTempsReel(ValeursTpsReel) Then
            'Si il y  a une erreur on affiche les résultats en fonctions des cours de clotures (cours Omega)
            Radio_Omega.Checked = True
            MessageBox.Show("Affichage des titres selon les cours de clotures d'Omega ", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        'On a maintenant 2 tableaux avec les anciennes et les dernières valeurs de bloomBerg
        Requete = Connection.LectureFichierSql("Strat_TraitementIndicetpsReel.sql")
        For i = 0 To NB_INDICES_REFERENCE - 1
            Requete = Replace(Requete, "'ValueDeb " & AnciennesValeurs.Item(0).Item(i) & "'", AnciennesValeurs.Item(1).Item(i))
            Requete = Replace(Requete, "'ValueFin " & ValeursTpsReel.Item(0).Item(i) & "'", ValeursTpsReel.Item(1).Item(i))
        Next

        Requete = Replace(Requete, "#paramdeb", dateDeb.ToString("dd/MM/yyyy"))
        result = Connection.commandeSqlnoInsert(Requete)
        AfficherResultat(Result)
    End Sub

#End Region
End Class