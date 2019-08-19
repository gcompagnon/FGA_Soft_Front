Public Class AccueilTitreGrille

    Structure TitreOmega
        Public code_titre As String
        Public id_grille As String
        Public dategrille As String
        Public poids As Single
        Public source As String
    End Structure

    Structure TitreSousjacent
        Public Isin_Titre As String
        Public sous_jacent As String
    End Structure

    Structure result
        Public code_titre As String
        Public id_grille As String
        Public dategrille As String
        Public poids As Single
        Public source As String
        Public Isin_Titre As String
        Public sous_jacent As String
    End Structure

    Dim Connection As New Connection()

    Private Sub AffectationTitreGrille_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.SetDatasource()

    End Sub

    Delegate Sub SetDataSourceAsync()

    Public Sub SetDatasource()
        'Dim Connection As New Connection()
        Dim Marequete As String

        'Connection a la BDD
        Connection.ToConnectBase()

        Marequete = "Select distinct PTF.[code_titre],PTF.[Libelle_titre],PTF.Dateinventaire,PTF.[Type_Produit],PTF.[Secteur],PTF.[Sous_Secteur]"
        Marequete = Marequete & " From (SELECT  [code_titre],Max(Dateinventaire) as Madate FROM [PTF_FGA] Group by code_titre) Rqt"
        Marequete = Marequete & " INNER JOIN [PTF_FGA] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.Madate = PTF.dateinventaire Where Rqt.code_titre not in (select ISIN_titre from [STRAT_TITRE_GRILLE])"
        Marequete = Marequete & " UNION "
        Marequete = Marequete & " Select distinct prox.code_titre,Prox.[Libelle_titre],prox.[date],Prox.[Type_Produit],Prox.[Secteur],Prox.[Sous_Secteur]"
        Marequete = Marequete & " from (SELECT distinct [code_titre],max([date]) as madate FROM [PTF_PROXY] group by code_titre) tmp inner join ptf_proxy prox on prox.code_titre = tmp.code_titre and prox.date= tmp.madate"
        Marequete = Marequete & " Where tmp.code_titre not in (select ISIN_titre from [STRAT_TITRE_GRILLE])"
        Marequete = Marequete & " and tmp.code_titre not in (select code_titre from [PTF_FGA])"
        DG_Titre.DataSource = Connection.LoadDataGridByString(Marequete)
        lbl_NbTitre.Text = "Nombre de titres à affecter : " & DG_Titre.RowCount.ToString
        DG_Titre.Refresh()
    End Sub



    'Private Sub DG_Titre_CellMouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DG_Titre.CellMouseDoubleClick
    'AffecterTitreGrille.ShowDialog()
    'End Sub

    Private Sub Btn_Rafraichir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Rafraichir.Click
        'cette variable permet d'exécuter une procedure stockée sans erreur
        'Si on remplace Param par Nothing dans l'appel de la fonction, il y a une erreur
        'On a pas de paramètres a passé d'ou la déclaration d'une liste qui ne contiendra rien 
        Dim Param As New List(Of String)



        'Connection a la BDD
        Connection.ToConnectBase()
        Try
            Me.Cursor = Cursors.WaitCursor
            'Rafraichissement des itres de PTF_FGA et proxy
            Connection.ProcedureStockée("STRAT_Affecter_Titre_Grille", Param, Nothing)
            'Traitement des proxys (Si un titre à un proxy qui est décomposé à 100% selon différentes grilles)
            Connection.ProcedureStockée("STRAT_Traitement_Proxy", Param, Nothing)

            Recherchesousjacent()

            SetDatasource()
            'DG_Titre.oCslumns(0).DataGridView
            Me.Cursor = Cursors.Default

        Catch ex As Exception
            MessageBox.Show("Rafraichissement impossible")
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub DG_Titre_CellDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DG_Titre.CellDoubleClick
        Dim af As New AffecterTitreGrille
        af.ShowDialog()
    End Sub
    Private Sub Recherchesousjacent()

        Dim ConnOmega As New ConnectionOmega
        Dim RequeteOmega As String
        Dim resultOmega As List(Of Object)
        Dim RequeteBase As String
        Dim Resultat As List(Of Object)
        Dim Obj_Omega As New TitreOmega
        Dim Obj_Sousjacent As TitreSousjacent
        Dim listTitreSousjacent As New List(Of TitreSousjacent)
        Dim listTitreOmega As New List(Of TitreOmega)
        Dim colValue As List(Of Object)
        Dim UpdateSousJacent As New List(Of Object)
        Dim RequeteSousJacent As String

        'Recherche de nouveau sous_jacent
        ConnOmega.ToConnectOmega()
        RequeteOmega = "select p._codeprodui,p._sousjacent from com.produit p inner join com.produit p2 on p2._codeprodui = p._sousjacent where p._codeprodui in ("
        For i = 0 To DG_Titre.RowCount - 1
            RequeteOmega = RequeteOmega & "'" & DG_Titre.Rows(i).Cells(0).Value & "',"
        Next

        'Mise à jour des sous_jacent deja existant
        RequeteSousJacent = "select isin_titre from strat_titre_grille where source = 'Sous_Jacent'"
        UpdateSousJacent = Connection.RequeteSqlToList(RequeteSousJacent)
        'On supprime les anciennes lignes (pb pour la gestion des dates) On supprime puis on les réinsère avec la bonne grille
        Connection.DeleteWhere("strat_titre_grille", "Source", "Sous_jacent")
        For i = 0 To UpdateSousJacent.Count - 1
            RequeteOmega = RequeteOmega & "'" & UpdateSousJacent(i) & "',"
        Next

        RequeteOmega = RequeteOmega.Substring(0, RequeteOmega.Length - 1)
        RequeteOmega = RequeteOmega & ") and p._sousjacent is not null"
        resultOmega = ConnOmega.commandeSqlToListReturn(RequeteOmega)

        'On afecte le resultat à un objet omega
        For Each res In resultOmega
            Obj_Sousjacent = New TitreSousjacent()
            Obj_Sousjacent.Isin_Titre = res(0)
            Obj_Sousjacent.sous_jacent = res(1)
            listTitreSousjacent.Add(Obj_Sousjacent)
        Next

        RequeteBase = "Select isin_titre,id_grille,max(date),poids,source from strat_titre_grille where isin_titre in ( "
        For Each res In resultOmega
            RequeteBase = RequeteBase & "'" & res(1) & "',"
        Next
        RequeteBase = RequeteBase.Substring(0, RequeteBase.Length - 1)
        RequeteBase = RequeteBase & ") group by isin_titre,id_grille,source,poids"

        Resultat = Connection.RequeteSqlToList(RequeteBase)

        For i = 0 To Resultat.Count - 1 Step 5
            Obj_Omega = New TitreOmega()
            Obj_Omega.code_titre = Resultat(i)
            Obj_Omega.id_grille = Resultat(i + 1)
            Obj_Omega.dategrille = Resultat(i + 2)
            Obj_Omega.poids = Resultat(i + 3)
            Obj_Omega.source = "Sous_jacent"
            listTitreOmega.Add(Obj_Omega)
        Next

        'Dim query = _listTitreOmega.Join(listTitreSousjacent, _
        '                                Function(TitreSousjacent) sous_jacent, _
        '                                Function(TitreOmega) code_titre, _
        '                                Function(TitreSousjacent, TitreOmega) _
        '                                New With {.sous_jacent = .code_titre})
        Dim query = From t In listTitreSousjacent Join o In listTitreOmega On o.code_titre Equals t.sous_jacent
                    Select t.Isin_Titre, o.id_grille, o.dategrille, o.poids, o.source

        Dim colname As New List(Of String)
        colname.Add("Isin_titre")
        colname.Add("Id_grille")
        colname.Add("Date")
        colname.Add("Poids")
        colname.Add("Source")
        'On insère maintenant les lignes 
        For i = 0 To query.Count - 1
            colValue = New List(Of Object)
            colValue.Add(query(i).Isin_Titre)
            colValue.Add(query(i).id_grille)
            colValue.Add(query(i).dategrille)
            colValue.Add(query(i).poids)
            colValue.Add(query(i).source)
            Connection.Insert("strat_titre_grille", colname, colValue)
        Next

        RequeteBase = ""
    End Sub
End Class