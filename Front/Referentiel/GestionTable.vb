Imports System.IO
Namespace Referentiel

    Public Class GestionTable

        Dim co As Connection = New Connection()
        Dim omega As ConnectionOmega = New ConnectionOmega()
        Dim excel As Excel = New Excel()

        Dim pathiBoxx As String = My.Settings.PATH & "\INPUT\TAUX\IBOXX"
        Dim dateFacSet As Date

        ''' <summary>
        ''' Ihm ouverture
        ''' </summary>
        Private Sub Gestiontable_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Gestion droits pour les onglets Integration : pour les admin des groupes Dev et Dir
            If Not (Utilisateur.metier = "Dev" Or Utilisateur.metier = "Dir") Or
                Utilisateur.admin = False Then
                Dim OngletPortefeuille As TabPage
                Dim OngletGeneral As TabPage
                OngletPortefeuille = TabControl.TabPages("TabPagePortefeuille")
                OngletGeneral = TabControl.TabPages("TabPageGeneral")
                TabControl.TabPages.Remove(OngletPortefeuille)
                TabControl.TabPages.Remove(OngletGeneral)
            End If
            If Not (Utilisateur.metier = "Act" Or Utilisateur.metier = "Dev" Or Utilisateur.metier = "Dir") Or
                Utilisateur.admin = False Then
                Dim OngletAction As TabPage
                OngletAction = TabControl.TabPages("TabPageAction")
                TabControl.TabPages.Remove(OngletAction)
            End If
            If Not (Utilisateur.metier = "Tx" Or Utilisateur.metier = "Dev" Or Utilisateur.metier = "Dir") Or
                Utilisateur.admin = False Then
                Dim OngletTaux As TabPage
                OngletTaux = TabControl.TabPages("TabPageTaux")
                TabControl.TabPages.Remove(OngletTaux)
            End If

            'Connection a la BDD
            co.ToConnectBase()
            omega.ToConnectOmega()

            'Set the CustomFormat string.
            'DateTime.CustomFormat = "MM'/'dd'/'yyyy"
            'DateTime.Format = DateTimePickerFormat.Custom
            CbFGA.Checked = True
            CbProxy.Checked = False

            'Binder les dates transparisable
            CbDate.DataSource = omega.commandeSqlToList(omega.LectureFichierSql("GetDateTrans.sql"))

            'Check si on peut ajouter des tables générales
            If co.SelectDistinctSimple("PAYS", "id").Count > 0 And co.SelectDistinctSimple("ZONE", "id").Count() > 0 And co.SelectDistinctSimple("ASSOCIATION_PAYS_ZONE", "id_pays").Count() > 0 Then
                CbPays.Enabled = False
            End If
            If co.SelectDistinctSimple("ZONE_GEOGRAPHIQUE", "pays").Count > 0 Then
                CbZoneGéographique.Enabled = False
            End If
            If co.SelectDistinctSimple("SECTEUR", "id").Count > 0 And co.SelectDistinctSimple("SOUS_SECTEUR", "id").Count > 0 Then
                CbSecteur.Enabled = False
            End If

            'Check si on peut ajouter des tables taux
            If co.SelectDistinctSimple("TX_RATING", "id").Count > 0 Then
                CbRating.Enabled = False
            End If
            If co.SelectDistinctSimple("TMP_SIGNATURE_OMEGA", "code").Count > 0 Then
                CbEmetteur.Enabled = False
            End If
            If co.SelectDistinctSimple("TX_RECOMMANDATION", "id").Count > 0 Then
                CbRecommandation.Enabled = False
            End If
            If co.SelectDistinctSimple("TX_RACINE", "chemin").Count() > 0 Then
                CbRacine.Enabled = False
            End If
            If co.SelectDistinctSimple("TX_EMETTEUR_FICHIER", "id").Count() > 0 Then
                CbEmetteurFichier.Enabled = False
            End If
            If co.SelectDistinctSimple("TX_RATING_EMETTEUR", "id_emetteur").Count() > 0 Then
                CbRatingEmetteur.Enabled = False
            End If
            If co.SelectDistinctSimple("TX_IBOXX_CORRESPONDANCE", "secteur").Count() > 0 Then
                CbCorrespondanceIboxx.Enabled = False
            End If
            CbiBoxx.Enabled = False

            If co.SelectDistinctSimple("PTF_TYPE_ACTIF", "Produit").Count > 0 And co.SelectDistinctSimple("PTF_LOT", "id_lot").Count > 0 Then
                BRemplirPort.Enabled = False
            End If


            'Check si on peut ajouter des tables taux
            If co.SelectDistinctSimple("ACT_SUPERSECTOR", "id").Count > 0 Then
                CbActionSecteur.Enabled = False
            End If

            'Autorise a modifié la base que si utilasateur est admin
            If Utilisateur.admin = False Then
                BAjoutA.Enabled = False
                BViderA.Enabled = False
                BRemplirPort.Enabled = False
                BToutVider.Enabled = False
                BRemplirGeneral.Enabled = False
                BRemplirTaux.Enabled = False
                BRemplirAct.Enabled = False
            End If

        End Sub

#Region "Portefeuille"

        ''' <summary>
        ''' BRemplir : remplit les caractéristique des portefeuilles 
        ''' </summary>
        Private Sub BRemplir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRemplirPort.Click
            BRemplirPort.Enabled = False
            BToutVider.Enabled = False
            BViderA.Enabled = False
            BAjouterLot.Enabled = False
            BAjoutA.Enabled = False

            If CbCorrespondances.Checked Or CbLot.Checked Then

                If CbCorrespondances.Checked Then
                    'Remplir les 3 tables de correspondances portefeuille FGA
                    If (co.SelectSimple("PTF_TYPE_ACTIF", "Produit").Count = 0 And co.SelectSimple("PTF_CARAC_OPCVM", "Libelle").Count = 0 And co.SelectSimple("PTF_TYPE_DE_DETTE", "Libelle").Count = 0) Then
                        Dim path As String = My.Settings.PATH & "\IMPORT\MIDDLE\TRANSPARENCE"
                        'Insertion dans PTF_Type_actif
                        excel.ExcelToSql(path, "Base_FGA_CORRESPONDANCES.xls", 1, "PTF_TYPE_ACTIF")
                        'Insertion dans PTF_carc_Opcvm
                        excel.ExcelToSql(path, "Base_FGA_CORRESPONDANCES.xls", 2, "PTF_CARAC_OPCVM")
                        'Insertion dans PTF_TYPE_DE_DETTE
                        excel.ExcelToSql(path, "Base_FGA_CORRESPONDANCES.xls", 3, "PTF_TYPE_DE_DETTE")
                    Else
                        MessageBox.Show("La table de correspondance FGA est déjà pleine", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
                    End If
                End If

                If CbLot.Checked Then
                    'Remplir les 3 tables de portefeuille lot
                    If (co.SelectSimple("PTF_LOT", "id_lot").Count = 0 And co.SelectSimple("PTF_LOT_GROUPE", "id_lot").Count = 0 And co.SelectSimple("PTF_LOT_COMPTE", "id_lot").Count = 0) Then
                        Dim path As String = My.Settings.PATH & "\IMPORT\MIDDLE\TRANSPARENCE"
                        Dim nameExcel As String = "BASE_LOT.xls"
                        'Insertion dans PTF_LOT
                        excel.ExcelToSql(path, nameExcel, 1, "PTF_LOT")
                        'Insertion dans PTF_LOT_GROUPE
                        excel.ExcelToSql(path, nameExcel, 2, "PTF_LOT_GROUPE")
                        'Insertion dans PTF_LOT_COMPTE
                        excel.ExcelToSql(path, nameExcel, 3, "PTF_LOT_COMPTE")
                    Else
                        MessageBox.Show("La table de lot de portefeuille est déjà pleine", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
                    End If
                End If

            Else
                MessageBox.Show("Cochez au moins la case correspondance et/ou lot pour remplir une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

            BRemplirPort.Enabled = True
            BToutVider.Enabled = True
            BViderA.Enabled = True
            BAjoutA.Enabled = True
            BAjouterLot.Enabled = True
        End Sub

        ''' <summary>
        ''' BVider : vide une table avec un where (date)
        ''' </summary>
        Private Sub BViderA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BViderA.Click
            Dim datee As DateTime = CbDate.Text
            BRemplirPort.Enabled = False
            BToutVider.Enabled = False
            BViderA.Enabled = False
            BAjoutA.Enabled = False
            BAjouterLot.Enabled = False
            Dim flagNiv1 As Boolean = False


            If CbFGA.Checked Or CbProxy.Checked Or cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked Then

                If CbFGA.Checked Then
                    If (co.SelectDistinctWhere("PTF_FGA", "Dateinventaire", "Dateinventaire", datee).Count > 0) Then
                        co.DeleteWhere("PTF_FGA", "Dateinventaire", datee)
                    Else
                        MessageBox.Show("La table PTF_FGA  ne contient pas d'informations à la date " & datee, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                    If (co.SelectDistinctWhere("PTF_AN", "date", "date", datee).Count > 0) Then
                        co.DeleteWhere("PTF_AN", "date", datee)
                    Else
                        MessageBox.Show("La table PTF_AN  ne contient pas d'informations à la date " & datee, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If

                If CbProxy.Checked Then
                    If (co.SelectDistinctWhere("PTF_PROXY", "Date", "Date", datee).Count > 0) Then
                        co.DeleteWhere("PTF_PROXY", "Date", datee)
                    Else
                        MessageBox.Show("La table PTF_PROXY ne contient pas d'informations à la date " & datee, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                    If (co.SelectDistinctWhere("PTF_PARAM_PROXY", "Date", "Date", datee).Count > 0) Then
                        co.DeleteWhere("PTF_PARAM_PROXY", "Date", datee)
                    Else
                        MessageBox.Show("La table PTF_PARAM_PROXY ne contient pas d'informations à la date " & datee, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If

                If cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked Then
                    Dim colWhere As List(Of String) = New List(Of String)()
                    colWhere.Add("dateinventaire")
                    colWhere.Add("numero_niveau")
                    Dim donnee As List(Of Object) = New List(Of Object)()
                    If (CbTrans4.Checked) Then
                        donnee.Add(datee)
                        donnee.Add(4)
                        If (co.SelectDistinctWheres("PTF_TRANSPARISE", "dateinventaire", colWhere, donnee).Count > 0) Then
                            co.DeleteWheres("PTF_TRANSPARISE", colWhere, donnee)
                        Else
                            MessageBox.Show("La table PTF_TRANSPARISE ne contient pas d'informations pour " & donnee.ToArray.ToString, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End If

                    End If
                    donnee.Clear()

                    If (CbTrans3.Checked) Then
                        donnee.Add(datee)
                        donnee.Add(3)
                        If (co.SelectDistinctWheres("PTF_TRANSPARISE", "dateinventaire", colWhere, donnee).Count > 0) Then
                            co.DeleteWheres("PTF_TRANSPARISE", colWhere, donnee)
                        Else
                            MessageBox.Show("La table PTF_TRANSPARISE ne contient pas d'informations pour " & donnee.ToArray.ToString, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End If

                    End If
                    donnee.Clear()
                    If (CbTrans2.Checked) Then
                        donnee.Add(datee)
                        donnee.Add(2)
                        If (co.SelectDistinctWheres("PTF_TRANSPARISE", "dateinventaire", colWhere, donnee).Count > 0) Then
                            co.DeleteWheres("PTF_TRANSPARISE", colWhere, donnee)
                        Else
                            MessageBox.Show("La table PTF_TRANSPARISE ne contient pas d'informations pour " & donnee.ToArray.ToString, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End If
                    End If
                    donnee.Clear()
                    If (cbTrans1.Checked) Then
                        donnee.Add(datee)
                        donnee.Add(1)
                        If (co.SelectDistinctWheres("PTF_TRANSPARISE", "dateinventaire", colWhere, donnee).Count > 0) Then
                            co.DeleteWheres("PTF_TRANSPARISE", colWhere, donnee)
                        Else
                            MessageBox.Show("La table PTF_TRANSPARISE ne contient pas d'informations pour " & donnee.ToArray.ToString, "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End If
                    End If
                End If
            Else
                MessageBox.Show("Cochez au moins portefeuille FGA, Proxy, et/ou transparence 1-2-3-4 pour vider une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

            BRemplirPort.Enabled = True
            BToutVider.Enabled = True
            BAjoutA.Enabled = True
            BAjouterLot.Enabled = True
            BViderA.Enabled = True
        End Sub

        ''' <summary>
        ''' BToutVider : vide une table ENTIEREMENT
        ''' </summary>
        Private Sub BToutVider_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BToutVider.Click
            BRemplirPort.Enabled = False
            BToutVider.Enabled = False
            BViderA.Enabled = False
            BAjouterLot.Enabled = False
            BAjoutA.Enabled = False

            If CbFGA.Checked Or CbProxy.Checked Or CbCorrespondances.Checked Or CbLot.Checked Or cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked Then

                Dim a As Integer = MessageBox.Show("Etes vous sures de supprimer toutes les données ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 1 Then

                    If CbFGA.Checked Then
                        co.DeleteFrom("PTF_FGA")
                        co.DeleteFrom("PTF_AN")
                    End If
                    If CbProxy.Checked Then
                        co.DeleteFrom("PTF_PROXY")
                        co.DeleteFrom("PTF_PARAM_PROXY")
                    End If
                    If CbCorrespondances.Checked Then
                        co.DeleteFrom("PTF_TYPE_ACTIF")
                        co.DeleteFrom("PTF_CARAC_OPCVM")
                        co.DeleteFrom("PTF_TYPE_DE_DETTE")
                    End If
                    If CbLot.Checked Then
                        co.DeleteFrom("PTF_LOT_GROUPE")
                        co.DeleteFrom("PTF_LOT_COMPTE")
                        co.DeleteFrom("PTF_LOT")
                    End If
                    If cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked Then
                        'Supprimer que les lignes niv2 de la table PTF_FGA
                        Dim colWhere As List(Of String) = New List(Of String)()
                        Dim donnee As List(Of Object) = New List(Of Object)()
                        colWhere.Add("numero_niveau")
                        If (cbTrans1.Checked) Then
                            donnee.Add(1)
                        ElseIf (CbTrans2.Checked) Then
                            donnee.Add(2)
                        ElseIf (CbTrans3.Checked) Then
                            donnee.Add(3)
                        ElseIf (CbTrans4.Checked) Then
                            donnee.Add(4)
                        End If

                        co.DeleteWheres("PTF_TRANSPARISE", colWhere, donnee)
                    End If

                End If
            Else
                MessageBox.Show("Cochez au moins une case pour tout vider une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

            BAjouterLot.Enabled = True
            BAjoutA.Enabled = True
            BRemplirPort.Enabled = True
            BToutVider.Enabled = True
            BViderA.Enabled = True
        End Sub

        ''' <summary>
        ''' Effectue une requete sur la base Omega pour remplir la table PTF_FGA(inventaire)  et PTF_AN (liste des actifs nets)
        ''' </summary>
        ''' <param name="datee">la date d extraction</param>
        ''' <param name="nomFichierPTF_FGA">le nom du fichier contenant la requete sql</param>
        ''' <param name="comptes">optionel: liste des comptes sur lequels la requete porte</param>
        ''' <remarks></remarks>
        '''         
        Public Sub PortefeuilleFGA(ByVal datee As DateTime, ByVal nomFichierPTF_FGA As String, Optional ByVal comptes As String() = Nothing)
            co.ToConnectBase()
            omega.ToConnectOmega()

            'Remplie tous les OPCVM et MANDAT puis les PTF_AN
            If (co.SelectDistinctWhere("PTF_AN", "date", "date", datee).Count > 0) Then
                Dim a As Integer = MessageBox.Show("Voulez vous remplacer les données dans PTF_FGA et PTF_AN pour la date " & datee & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 6 Then
                    'If nomFichierPTF_FGA <> "PTF_FGA_Prime_Oblig.sql" Then
                    co.DeleteWhere("PTF_FGA", "Dateinventaire", datee)
                    co.DeleteWhere("PTF_AN", "date", datee)
                    'Else
                    'Dim colWhere As List(Of String) = New List(Of String)
                    'colWhere.Add("dateinventaire")
                    'colWhere.Add("compte")
                    'Dim donnee As List(Of Object) = New List(Of Object)
                    'donnee.Add(datee)
                    'donnee.Add("6100034")
                    'co.DeleteWhere("PTF_AN", "date", datee)
                    'End If
                Else
                    Exit Sub
                End If
            End If

            ' mise à jour de la table PTF_FGA avec les inventaires Omega
            Dim requeteAExecuter As String
            requeteAExecuter = omega.LectureFichierSqlOrigine(nomFichierPTF_FGA)

            If (Not (comptes Is Nothing)) Then
                If comptes.Count > 0 Then
                    Dim clauseComptes = "v._compte in ( "
                    For Each c In comptes
                        clauseComptes += "'" + c + "',"
                    Next
                    ' retirer la dernier virgule
                    clauseComptes = clauseComptes.Remove(clauseComptes.Length - 1)
                    clauseComptes += ") and"
                    ' remplacer le commentaire pour ajouter une clause
                    requeteAExecuter = requeteAExecuter.Replace("--<CLAUSE_COMPLEMENTAIRE>", clauseComptes)
                End If
            End If

            requeteAExecuter = requeteAExecuter.Replace("***", datee)
            omega.commandeSql("PTF_FGA", requeteAExecuter, False)

            ' mise à jour de la table PTF_AN avec les actifs nets
            requeteAExecuter = omega.LectureFichierSql("PTF_AN.sql")
            requeteAExecuter = requeteAExecuter.Replace("***", datee)
            omega.commandeSql("PTF_AN", requeteAExecuter, False)

            'Remplir les lignes particuliere de Cash OPCVM et Mandat
            Dim paramName As List(Of String) = New List(Of String)
            paramName.Add("@date")
            Dim paramDonnee As List(Of Object) = New List(Of Object)
            paramDonnee.Add(datee)
            co.ProcedureStockée("GetCashFga2", paramName, paramDonnee)
            'Dim groupe As String
            'Dim chiffre As List(Of Object) = New List(Of Object)
            'For Each f In co.ProcedureStockéeDico("GetCashFga", paramName, paramDonnee)
            '    groupe = co.SelectWhere("PTF_FGA", "Groupe", "compte", f.Key).First.ToString
            '    'groupe
            '    chiffre.Add(groupe)
            '    'dateinventaire
            '    chiffre.Add(datee)
            '    'compte
            '    chiffre.Add(f.Key)
            '    'isin_ptf
            '    If (co.SelectWhere("PTF_AN", "ISIN_Ptf", "compte", f.Key).Count > 0) Then
            '        chiffre.Add(co.SelectWhere("PTF_AN", "ISIN_Ptf", "compte", f.Key).First.ToString())
            '    Else
            '        chiffre.Add(Nothing)
            '    End If
            '    'libelle_ptf
            '    chiffre.Add(co.SelectWhere("PTF_AN", "Libelle_Ptf", "compte", f.Key).First.ToString)
            '    'check Mandat ou OPCVN
            '    If groupe <> "OPCVM" Then
            '        groupe = "Mandat"
            '    End If
            '    'code_titre
            '    chiffre.Add("Cash " & groupe)
            '    'isin_titre
            '    chiffre.Add("Cash " & groupe)
            '    'libelle_titre
            '    chiffre.Add("Liquidité(" & groupe & ")")
            '    'valeur_boursiere
            '    chiffre.Add(f.Value)
            '    chiffre.Add(0)
            '    'valeur_comptable
            '    chiffre.Add(f.Value)
            '    chiffre.Add(0)
            '    chiffre.Add(0)
            '    chiffre.Add(0)
            '    'type_produit
            '    chiffre.Add("Cash")
            '    'devise_titre
            '    chiffre.Add("EUR")
            '    'secteur
            '    chiffre.Add("Liquidité")
            '    'sous_secteur
            '    chiffre.Add("Liquidité")
            '    'Pays
            '    chiffre.Add("France")
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    chiffre.Add(Nothing)
            '    co.Insert("PTF_FGA", co.SelectColonneName("PTF_FGA"), chiffre)
            '    chiffre.Clear()
            'Next
        End Sub

        ''' <summary>
        ''' BAjoutA : transparise pour un niveau donnée et une date donnéé
        ''' </summary>
        Private Sub BAjoutA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjoutA.Click
            BRemplirPort.Enabled = False
            BToutVider.Enabled = False
            BViderA.Enabled = False
            BAjouterLot.Enabled = False
            BAjoutA.Enabled = False
            Dim datee As DateTime = CbDate.Text

            If CbFGA.Checked Or CbProxy.Checked Or cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked Then

                Dim colName As List(Of String) = New List(Of String)
                Dim chiffre As List(Of Object) = New List(Of Object)
                Dim fermer As Boolean = True
                Windows.Forms.Cursor.Current = Cursors.WaitCursor

                If CbFGA.Checked Then
                    PortefeuilleFGA(datee, "PTF_FGA.sql")
                End If


                If CbProxy.Checked Then
                    'Remplir la table PROXY
                    If ((co.SelectDistinctWhere("PTF_PARAM_PROXY", "date", "date", datee).Count > 0) Or
                        (co.SelectDistinctWhere("PTF_PROXY", "date", "date", datee).Count > 0)) Then
                        Dim a As DialogResult = MessageBox.Show("Voulez vous remplacer les données dans PTF_PROXY et PTF_PARAM_PROXY pour la date " & datee & " ?", "Confirmation.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        If a = DialogResult.Yes Then
                            co.DeleteWhere("PTF_PARAM_PROXY", "date", datee)
                            co.DeleteWhere("PTF_PROXY", "date", datee)
                        Else
                            Exit Sub
                        End If
                    End If

                    'Remplir la table PTF_PROXY et PTF_PARAM_PROXY (avec fichier, ou recopier la base ?)
                    Dim path As String = My.Settings.PATH & "\INPUT\MIDDLE\TRANSPARENCE"
                    Dim nameExcel As String = "Base_PROXY.xls"
                    'Dim excel As Microsoft.Office.Interop.Excel.Worksheet = co.LectureFichierExcel(path, nameExcel, 2) 'date de PTF_PARAM_PROXY
                    Dim dateeExcel As DateTime = excel.CellFichierExcel(path, nameExcel, 2, 2, 1)
                    Dim paramName As List(Of String) = New List(Of String)
                    paramName.Add("@date")
                    Dim paramDonnee As List(Of Object) = New List(Of Object)
                    paramDonnee.Add(datee)

                    If datee = dateeExcel Or co.SelectDistinctSimple("PTF_PARAM_PROXY", "source").Count = 0 Then
                        'on remplit avec excel PTF_PROXY puis PTF_PARAM_PROXY
                        excel.ExcelToSqlBulk(path, "Base_PROXY.xls", 1, "PTF_PROXY", colSize:=New List(Of Integer)(New Integer() {16, 20, 60, 20, 60, 8, 8, 60, 3, 60, 60, 30, 60, 4, 60, 16, 8, 8}))
                        'co.ExcelToSql(path, "Base_PROXY.xls", 2, "PTF_PARAM_PROXY")
                        colName.Clear()
                        Dim codeProxy As String = String.Empty
                        colName = co.SelectColonneName("PTF_PARAM_PROXY", order:=False)
                        Dim nbrLigne As Integer
                        Dim app As Microsoft.Office.Interop.Excel.Application = CreateObject("Excel.Application")
                        Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(path & "/" & "Base_PROXY.xls", , True)
                        Dim excell As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(2)

                        For Each f In excell.Columns(1).Value()
                            If IsNothing(f) = False Then
                                nbrLigne = nbrLigne + 1
                            End If
                        Next
                        For index As Integer = 2 To nbrLigne
                            For Each f In excell.Rows(index).Value()
                                If IsNothing(f) = False Then
                                    chiffre.Add(f)
                                End If
                            Next
                            If co.SelectDistinctWhere("PTF_PROXY", "code_proxy", "Date", datee).Contains(chiffre(3)) Or chiffre(5) <> "PROXY" Then
                                co.Insert("PTF_PARAM_PROXY", colName, chiffre)
                            Else
                                'Mesage d'info a l'utilisateur pour lui informer de la supression de la table
                                If codeProxy <> chiffre(3) Then
                                    codeProxy = chiffre(3)
                                    MessageBox.Show("Le code proxy " & chiffre(3) & " n'a pas été ajouté dans PTF_PARAM_PROXY car la table PTF_PROXY ne contient pas ses informations !", "Problème ", MessageBoxButtons.OK, MessageBoxIcon.None)
                                End If
                            End If
                            chiffre.Clear()
                        Next
                        app.Quit()
                        app = Nothing

                        'Ajout des lignes de CASH pour les proxy_proxy
                        co.ProcedureStockée("GetCashProxy2", paramName, paramDonnee)
                        'colName.Clear()
                        'colName = co.SelectColonneName("PTF_PROXY")
                        'For Each f In co.ProcedureStockéeDico("GetCashProxy", paramName, paramDonnee)
                        '    chiffre.Add(dateeExcel)
                        '    chiffre.Add(f.Key)
                        '    chiffre.Add(co.SelectWhere("PTF_PARAM_PROXY", "libellé_proxy", "code_proxy", f.Key).First.ToString())
                        '    chiffre.Add("Liquidité(OPCVM)")
                        '    chiffre.Add("Liquidité(OPCVM)")
                        '    chiffre.Add(f.Value)
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add("Cash")
                        '    chiffre.Add("EUR")
                        '    chiffre.Add("Liquidité")
                        '    chiffre.Add("Liquidité")
                        '    chiffre.Add("France")
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add(Nothing)
                        '    chiffre.Add(Nothing)
                        '    co.Insert("PTF_PROXY", colName, chiffre)
                        '    chiffre.Clear()
                        'Next

                    Else
                        'on remplit avec la base = procédure stocké
                        co.ProcedureStockée("GetProxyInfo", paramName, paramDonnee)
                    End If


                End If

                If (cbTrans1.Checked Or CbTrans2.Checked Or CbTrans3.Checked Or CbTrans4.Checked) Then
                    If (co.SelectDistinctWhere("PTF_FGA", "Dateinventaire", "Dateinventaire", datee).Count > 0 And co.SelectDistinctWhere("PTF_AN", "date", "date", datee).Count > 0 And co.SelectDistinctWhere("PTF_PARAM_PROXY", "date", "date", datee).Count > 0 And co.SelectDistinctWhere("PTF_PROXY", "date", "date", datee).Count > 0) Then
                        If (co.SelectDistinctSimple("PTF_TYPE_ACTIF", "Types").Count > 0 And co.SelectDistinctSimple("ZONE_GEOGRAPHIQUE", "Zone").Count > 0 And co.SelectDistinctSimple("PTF_CARAC_OPCVM", "Types").Count > 0 And co.SelectDistinctSimple("PTF_LOT", "id_lot").Count > 0 And co.SelectDistinctSimple("PTF_TYPE_DE_DETTE", "libelle").Count > 0) Then

                            Dim paramDonnee As List(Of Object) = New List(Of Object)
                            paramDonnee.Add(datee)
                            Dim paramName As List(Of String) = New List(Of String)
                            paramName.Add("@date")

                            If (cbTrans1.Checked) Then
                                'On transparise le niveau 1 pour tous les comptes existants dans la bdd
                                co.ProcedureStockée("Trans1", paramName, paramDonnee)
                            End If

                            If (CbTrans2.Checked) Then
                                co.ProcedureStockée("Trans2", paramName, paramDonnee)
                            End If

                            If (CbTrans3.Checked) Then
                                co.ProcedureStockée("Trans3", paramName, paramDonnee)
                            End If

                            If (CbTrans4.Checked) Then
                                co.ProcedureStockée("Trans4", paramName, paramDonnee)
                            End If
                        Else
                            MessageBox.Show("Impossible de transpariser le niveau  1 et/ou 2 et/ou 3 et/ou 4 à la date " & datee & " car il n'y a pas de données pour cette date dans les tables de concordance PTF_TYPE_ACTIF et/ou PTF_CARAC_OPCVM et/ou PTF_LOT et/ou PTF_TYPE_DE_DETTE et/ou ZONE_GEOGRAPHIQUE", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End If
                    Else
                        MessageBox.Show("Impossible de transpariser le niveau  1 et/ou 2 et/ou 3 et/ou 4 à la date " & datee & " car il n'y a pas de données pour cette date dans la table PTF_FGA et/ou PTF_AN et/ou PTF_PROXY et/ou PTF_PARAM_PROXY", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If

                Windows.Forms.Cursor.Current = Cursors.Default
                Me.Dispose()
            Else
                MessageBox.Show("Cochez au moins une case pour remplir une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

            BRemplirPort.Enabled = True
            BToutVider.Enabled = True
            BViderA.Enabled = True
            BAjouterLot.Enabled = True
            BAjoutA.Enabled = True
        End Sub

        ''' <summary>
        ''' BAjouterLot : ouvre AjoutLot
        ''' </summary>
        Private Sub BAjouter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjouterLot.Click
            Dim al As New AjoutLot
            al.Show()
        End Sub

        ''' <summary>
        ''' Cette fonction supprime n'importe quel caractère du string strA passé en premier argument
        ''' exemple :
        ''' toto="azè-e(r(tè-y"
        ''' toto=Strip(toto,"(","è","-")
        ''' donne "azerty"
        ''' </summary>
        Function Strip(ByVal strA As String, ByVal ParamArray varZoek() As String)
            Dim intTel As Integer
            Dim strNew As String = String.Empty
            Dim varteken As Object
            Dim strTekst As String

            strTekst = strA
            For Each varteken In varZoek
                strNew = ""
                For intTel = 1 To Len(strTekst)
                    If Mid(strTekst, intTel, 1) <> varteken Then
                        strNew = strNew & Mid(strTekst, intTel, 1)
                    End If
                Next
                strTekst = strNew
            Next
            Strip = strNew
        End Function

        ''' <summary>
        ''' MouseEnter de BAjoutA
        ''' </summary>
        Private Sub BAjoutA_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjoutA.MouseEnter
            CbFGA.Enabled = True
            CbProxy.Enabled = True
            cbTrans1.Enabled = True
            CbTrans2.Enabled = True
            CbTrans3.Enabled = True
            CbCorrespondances.Enabled = False
            CbLot.Enabled = False
        End Sub

        ''' <summary>
        ''' MouseEnter de BToutVider
        ''' </summary>
        Private Sub BToutVider_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BToutVider.MouseEnter
            CbFGA.Enabled = True
            CbProxy.Enabled = True
            cbTrans1.Enabled = True
            CbTrans2.Enabled = True
            CbTrans3.Enabled = True
            CbCorrespondances.Enabled = True
            CbLot.Enabled = True
        End Sub

        ''' <summary>
        ''' MouseEnter de BViderA
        ''' </summary>
        ''' 
        Private Sub BViderA_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BViderA.MouseEnter
            CbFGA.Enabled = True
            CbProxy.Enabled = True
            cbTrans1.Enabled = True
            CbTrans2.Enabled = True
            CbTrans3.Enabled = True
            CbCorrespondances.Enabled = False
            CbLot.Enabled = False
        End Sub

        ''' <summary>
        ''' MouseEnter de BRemplir
        ''' </summary>
        Private Sub BRemplir_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRemplirPort.MouseEnter
            CbFGA.Enabled = False
            CbProxy.Enabled = False
            cbTrans1.Enabled = False
            CbTrans2.Enabled = False
            CbTrans3.Enabled = False
            CbCorrespondances.Enabled = True
            CbLot.Enabled = True
        End Sub

#End Region

#Region "Général"

        ''' <summary>
        ''' BRemplirGeneral : ajoute des tables générales : secteur, zone géographique ....
        ''' </summary>
        Private Sub BRemplirGeneral_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRemplirGeneral.Click
            If CbPays.Checked Or CbZoneGéographique.Checked Or CbSecteur.Checked Then

                Dim path As String = My.Settings.PATH & "\IMPORT"
                Dim nameExcel As String = "BASE_GENERALE.xls"

                If (CbPays.Checked) Then
                    If co.SelectDistinctSimple("PAYS", "id").Count() = 0 Then
                        excel.ExcelToSql(path, nameExcel, 2, "PAYS")
                    End If
                    If co.SelectDistinctSimple("ZONE", "id").Count() = 0 Then
                        excel.ExcelToSql(path, nameExcel, 3, "ZONE")
                    End If
                    If co.SelectDistinctSimple("ASSOCIATION_PAYS_ZONE", "id_pays").Count() = 0 Then
                        excel.ExcelToSql(path, nameExcel, 4, "ASSOCIATION_PAYS_ZONE")
                    End If
                End If

                If (CbZoneGéographique.Checked) Then
                    If co.SelectDistinctSimple("ZONE_GEOGRAPHIQUE", "pays").Count() = 0 Then
                        excel.ExcelToSql(path, nameExcel, 1, "ZONE_GEOGRAPHIQUE")
                    End If
                End If

                If (CbSecteur.Checked) Then
                    If co.SelectDistinctSimple("SECTEUR", "id").Count() = 0 And co.SelectDistinctSimple("SOUS_SECTEUR", "id").Count() = 0 Then
                        omega.commandeSql("SECTEUR", omega.LectureFichierSql("TMP_SECTEUR_OMEGA.sql"), False)
                        omega.commandeSql("SOUS_SECTEUR", omega.LectureFichierSql("TMP_SOUS_SECTEUR_OMEGA.sql"), False)
                    End If
                End If

            Else
                MessageBox.Show("Cochez au moins une case pour remplir une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' Bouton BRefreshPays : ajoute les nouveaux pays de OMEGA dans la base front
        ''' </summary>
        Private Sub BRefreshPays_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRefreshPays.Click
            'écrase les pays de OMEGA
            omega.commandeSql("TMP_PAYS_OMEGA", omega.LectureFichierSql("TMP_PAYS_OMEGA.sql"), True)
            'check si nouveau pays venant de OMEGA
            If co.SelectDistinctSimple("PAYS", "id").Count < co.SelectDistinctSimple("TMP_PAYS_OMEGA", "id").Count Then
                Dim colPays As List(Of String) = New List(Of String)()
                colPays.Add("id")
                colPays.Add("libelle")
                colPays.Add("libelle_anglais")
                colPays.Add("id_utilisateur")
                Dim colAsso As List(Of String) = New List(Of String)()
                colAsso.Add("id_pays")
                colAsso.Add("id_zone")
                Dim donnee As List(Of Object) = New List(Of Object)()
                For Each newPays In co.SelectDistinctSimple("TMP_PAYS_OMEGA", "id").Except(co.SelectDistinctSimple("PAYS", "id"))
                    donnee.Add(newPays)
                    donnee.Add(RTrim(co.SelectDistinctWhere("TMP_PAYS_OMEGA", "libelle", "id", newPays)(0)))
                    donnee.Add("Not Available")
                    donnee.Add("OMEGA")
                    co.Insert("PAYS", colPays, donnee)
                    MessageBox.Show("Le pays " & RTrim(co.SelectDistinctWhere("TMP_PAYS_OMEGA", "libelle", "id", newPays)(0)) & " a été ajouté !", "Ajout pays.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    donnee.Clear()
                    'Ajout dans la table d'association des pays et zone en tant que non renseigner NA
                    If co.SelectDistinctWhere("ASSOCIATION_PAYS_ZONE", "id_pays", "id_pays", newPays).Count = 0 Then
                        donnee.Add(newPays)
                        donnee.Add("NA")
                        co.Insert("ASSOCIATION_PAYS_ZONE", colAsso, donnee)
                        donnee.Clear()
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' BRefreshSecteur : ajoute les nouveaux secteur ou sous secteur de OMEGA dan sla base front
        ''' </summary>
        Private Sub BRefreshSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRefreshSecteur.Click
            'écrase les secteurs et sous secteur de OMEGA
            omega.commandeSql("TMP_SECTEUR_OMEGA", omega.LectureFichierSql("TMP_SECTEUR_OMEGA.sql"), True)
            omega.commandeSql("TMP_SOUS_SECTEUR_OMEGA", omega.LectureFichierSql("TMP_SOUS_SECTEUR_OMEGA.sql"), True)

            'check si nouveau secteur venant de OMEGA
            If co.SelectDistinctSimple("SECTEUR", "id").Count < co.SelectDistinctSimple("TMP_SECTEUR_OMEGA", "id").Count Then
                Dim colSecteur As List(Of String) = New List(Of String)()
                colSecteur.Add("id")
                colSecteur.Add("libelle")
                colSecteur.Add("id_utilisateur")
                Dim donnee As List(Of Object) = New List(Of Object)()
                For Each newSector In co.SelectDistinctSimple("TMP_SECTEUR_OMEGA", "id").Except(co.SelectDistinctSimple("SECTEUR", "id"))
                    donnee.Add(newSector)
                    donnee.Add(RTrim(co.SelectDistinctWhere("TMP_SECTEUR_OMEGA", "libelle", "id", newSector)(0)))
                    donnee.Add("OMEGA")
                    co.Insert("SECTEUR", colSecteur, donnee)
                    MessageBox.Show("Le secteur " & RTrim(co.SelectDistinctWhere("TMP_SECTEUR_OMEGA", "libelle", "id", newSector)(0)) & " a été ajouté !", "Ajout secteur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    donnee.Clear()
                Next
            End If

            'check si nouveau secteur venant de OMEGA
            If co.SelectDistinctSimple("SOUS_SECTEUR", "id").Count < co.SelectDistinctSimple("TMP_SOUS_SECTEUR_OMEGA", "id").Count Then
                Dim colSecteur As List(Of String) = New List(Of String)()
                colSecteur.Add("id")
                colSecteur.Add("id_secteur")
                colSecteur.Add("libelle")
                colSecteur.Add("id_utilisateur")
                Dim donnee As List(Of Object) = New List(Of Object)()
                For Each newSector In co.SelectDistinctSimple("TMP_SOUS_SECTEUR_OMEGA", "id").Except(co.SelectDistinctSimple("SOUS_SECTEUR", "id"))
                    donnee.Add(newSector)
                    donnee.Add(co.SelectDistinctWhere("TMP_SOUS_SECTEUR_OMEGA", "id_secteur", "id", newSector)(0))
                    donnee.Add(RTrim(co.SelectDistinctWhere("TMP_SOUS_SECTEUR_OMEGA", "libelle", "id", newSector)(0)))
                    donnee.Add("OMEGA")
                    co.Insert("SOUS_SECTEUR", colSecteur, donnee)
                    MessageBox.Show("Le sous secteur " & RTrim(co.SelectDistinctWhere("TMP_SOUS_SECTEUR_OMEGA", "libelle", "id", newSector)(0)) & " a été ajouté !", "Ajout secteur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    donnee.Clear()
                Next
            End If

        End Sub

#End Region

#Region "Taux"

        ''' <summary>
        ''' BTauxRemplir : ajoute des tables taux : recommandation, rating ....
        ''' </summary>
        Private Sub BTauxRemplir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRemplirTaux.Click
            Dim path As String = My.Settings.PATH & "\IMPORT\TAUX\"
            Dim baseEmetteur As String = "TX_BASE_EMETTEURS.xls"

            If CbRating.Checked Or CbRecommandation.Checked Or CbEmetteur.Checked Or CbRacine.Checked Or CbEmetteurFichier.Checked Or CbiBoxx.Checked Or CbCorrespondanceIboxx.Checked Or CbRatingEmetteur.Checked Then
                Windows.Forms.Cursor.Current = Cursors.WaitCursor

                If (CbRating.Checked) Then
                    If co.SelectDistinctSimple("TX_RATING", "id").Count() = 0 Then
                        excel.ExcelToSql(path, baseEmetteur, 1, "TX_RATING")
                    End If
                End If
                If (CbEmetteur.Checked) Then
                    If co.SelectDistinctSimple("TMP_SIGNATURE_OMEGA", "code").Count() = 0 Then
                        omega.commandeSql("TMP_SIGNATURE_OMEGA", omega.LectureFichierSql("TMP_SIGNATURE_OMEGA.sql"))
                    End If
                End If
                If (CbRecommandation.Checked) Then
                    If co.SelectDistinctSimple("TX_RECOMMANDATION", "id").Count() = 0 Then
                        excel.ExcelToSql(path, baseEmetteur, 2, "TX_RECOMMANDATION")
                    End If
                End If
                If (CbEmetteurFichier.Checked) Then
                    If co.SelectDistinctSimple("TX_EMETTEUR_FICHIER", "id").Count() = 0 Then
                        excel.ExcelToSql(path, baseEmetteur, 3, "TX_EMETTEUR_FICHIER")
                    End If
                End If
                If (CbRacine.Checked) Then
                    If co.SelectDistinctSimple("TX_RACINE", "chemin").Count() = 0 Then
                        excel.ExcelToSql(path, baseEmetteur, 4, "TX_RACINE")
                    End If
                End If
                If (CbRatingEmetteur.Checked) Then
                    If co.SelectDistinctSimple("TX_RATING_EMETTEUR", "id_emetteur").Count() = 0 Then
                        excel.ExcelToSql(path, baseEmetteur, 5, "TX_RATING_EMETTEUR")
                    End If
                End If
                If (CbiBoxx.Checked) Then
                    'co.ExcelToSql(pathiBoxx, iBoxx.Text, 1, "TX_IBOXX")
                    Try
                        excel.ExcelIboxxToSql(pathiBoxx, iBoxx.Text, "TX_IBOXX", 1, 7, 2)

                        Dim paysRisk As String = "SELECT c.isin, c.iso2, p.libelle_anglais"
                        paysRisk = paysRisk & " into #tmp"
                        paysRisk = paysRisk & " FROM TX_IBOXX_CNTRY_OF_RISK c LEFT OUTER JOIN pays p ON c.iso2=p.iso2 "

                        paysRisk = paysRisk & " UPDATE TX_IBOXX"
                        paysRisk = paysRisk & " SET Country = t.libelle_anglais"
                        paysRisk = paysRisk & " FROM TX_IBOXX i, #tmp t"
                        paysRisk = paysRisk & " WHERE i.isin = t.isin AND i.country <> t.libelle_anglais"

                        paysRisk = paysRisk & " DROP TABLE #tmp"


                        co.RequeteSql(paysRisk)

                    Catch ex As Exception
                        MessageBox.Show("Le fichier Iboxx contient des données incorrectes", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End Try
                End If
                If CbCorrespondanceIboxx.Checked Then
                    excel.ExcelToSql(path, "TX_IBOXX_CORRESPONDANCE.xls", 1, "TX_IBOXX_CORRESPONDANCE")
                End If

                Windows.Forms.Cursor.Current = Cursors.Default
                Me.Close()
            Else
                MessageBox.Show("Cochez au moins une case pour remplir une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' BRefreshSignature : remplie une table temporaire avec toutes les infos des signatures OMEGA
        ''' </summary>
        Private Sub BRefreshSignature_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRefreshSignature.Click
            omega.commandeSql("TMP_SIGNATURE_OMEGA", omega.LectureFichierSql("TMP_SIGNATURE_OMEGA.sql"), True)
        End Sub

        ''' <summary>
        ''' iBoxx SelectedValueChanged : check si ajout possible dans base
        ''' </summary>
        Private Sub iBoxx_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles iBoxx.SelectedValueChanged
            'Dim excel As Microsoft.Office.Interop.Excel.Worksheet = co.LectureFichierExcel(pathiBoxx, iBoxx.Text, 1)
            ' Lecture du fichier dans la combobox pour recuperer la cellule de la ligne 2 , colonne 3, qui est sensée etre une date
            Dim datee As String = excel.ExcelIboxxDate(pathiBoxx, CbiBoxx.Text, 1, 2, 3)

            If IsNothing(datee) Then
                MessageBox.Show("Le Fichier n'a pas le format attendu . Une date est attendue à la colonne C, Ligne 2")
                CbiBoxx.Enabled = False
            ElseIf co.SelectDistinctWhere("TX_IBOXX", "date", "Date", datee).Count = 0 Then
                CbiBoxx.Enabled = True
            Else
                CbiBoxx.Enabled = False
            End If
        End Sub

        ''' <summary>
        ''' iBoxx Click
        ''' </summary>
        Private Sub iBoxx_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles iBoxx.Click
            CbiBoxx.Enabled = False
            iBoxx.Items.Clear()
            For Each fichier In Directory.GetFiles(pathiBoxx, "*.xls", SearchOption.TopDirectoryOnly)
                iBoxx.Items.Add(New IO.FileInfo(fichier).Name())
            Next
        End Sub

        'TODO : A suprimer quand rating dans OMEGA
        ''' <summary>
        ''' BRefreshRating : re-remplie la table
        ''' </summary>
        Private Sub BRefreshRating_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRefreshRating.Click
            co.DeleteFrom("TX_RATING_EMETTEUR")
            excel.ExcelToSql(My.Settings.PATH & "\IMPORT\TAUX", "TX_BASE_EMETTEURS.xls", 5, "TX_RATING_EMETTEUR")
        End Sub

#End Region

#Region "Action"

        Private Sub BRemplirAct_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRemplirAct.Click
            If CbActionSecteur.Checked Then
                Windows.Forms.Cursor.Current = Cursors.WaitCursor

                If (CbActionSecteur.Checked) Then
                    If co.SelectDistinctSimple("ACT_SUPERSECTOR", "id").Count() = 0 Then
                        excel.ExcelToSql(My.Settings.PATH & "\IMPORT\ACTION", "Base Action.xls", 1, "ACT_SUPERSECTOR")
                    End If
                End If

                Windows.Forms.Cursor.Current = Cursors.Default
                Me.Close()
            Else
                MessageBox.Show("Cochez au moins une case pour remplir une table", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

#End Region

    End Class
End Namespace
