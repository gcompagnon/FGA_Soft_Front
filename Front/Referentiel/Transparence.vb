Imports System.Text
Namespace Referentiel

    Public Class Transparence
        Implements PrintPreviewForm

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()
        Private MyDataGridViewPrinter As New DataGridViewPrinter.DataGridViewPrinter(Me)

        ''' <summary>
        ''' Load de l'ihm transparence 
        ''' </summary>
        Private Sub Transparence_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Tentative de connnection
            co.ToConnectBase()

            MonthCalendar.MaxDate = DateTime.Now
            'Mettre les dates en gras que l'on peut transpariser
            For Each f In co.SelectDistinctSimple("PTF_FGA", "dateinventaire").Intersect(co.SelectDistinctSimple("PTF_PROXY", "date"))
                MonthCalendar.AddBoldedDate(f)
            Next
            MonthCalendar.UpdateBoldedDates()

            Dim rapport As List(Of String) = New List(Of String)
            rapport.Add("Monitoring groupe")
            rapport.Add("Monitoring emetteur")
            CbRapport.DataSource = rapport

            BExcel.Enabled = False
            BPrint.Enabled = False
            RbNiv1.Checked = True
            RbSecteurs.Checked = True
        End Sub

        ''' <summary>
        ''' BCharger : transparise et filtre si c'est possible
        ''' </summary>
        Private Sub BCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCharger.Click
            BCharger.Enabled = False
            BEffacer.Enabled = False

            'Dim datee As DateTime = DateTime.Value.Date
            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date

            'Test si toutes les donnees pour trasnpariser sont dans la BDD
            If (co.SelectDistinctWhere("PTF_FGA", "Dateinventaire", "Dateinventaire", datee).Count > 0 And co.SelectDistinctWhere("PTF_AN", "date", "date", datee).Count > 0 And co.SelectDistinctWhere("PTF_PARAM_PROXY", "date", "date", datee).Count > 0 And co.SelectDistinctWhere("PTF_PROXY", "date", "date", datee).Count > 0) Then
                If (co.SelectDistinctSimple("PTF_TYPE_ACTIF", "Types").Count > 0 And co.SelectDistinctSimple("ZONE_GEOGRAPHIQUE", "Zone").Count > 0 And co.SelectDistinctSimple("PTF_CARAC_OPCVM", "Types").Count > 0 And co.SelectDistinctSimple("PTF_LOT", "id_lot").Count > 0 And co.SelectDistinctSimple("PTF_TYPE_DE_DETTE", "libelle").Count > 0) Then
                    Windows.Forms.Cursor.Current = Cursors.WaitCursor
                    Dim paramDonnee As List(Of Object) = New List(Of Object)
                    paramDonnee.Add(datee)
                    Dim paramName As List(Of String) = New List(Of String)
                    paramName.Add("@date")

                    'On transparise le niveau 1 pour tous les comptes existants dans la bdd
                    If (RbNiv1.Checked = True) Then
                        If (co.SelectDoubleWhereNotNull("PTF_TRANSPARISE", "Groupe", "Dateinventaire", datee, "Numero_Niveau", 1).Count = 0 And RbNiv1.Checked = True) Then
                            co.ProcedureStockée("Trans1", paramName, paramDonnee)
                        End If
                    End If

                    'On transparise le niveau 1 puis le 2 pour tous les comptes existants dans la bdd
                    If (RbNiv2.Checked = True) Then
                        If (co.SelectDoubleWhereNotNull("PTF_TRANSPARISE", "Groupe", "Dateinventaire", datee, "Numero_Niveau", 1).Count = 0) Then
                            co.ProcedureStockée("Trans1", paramName, paramDonnee)
                        End If
                        If (co.SelectDoubleWhereNotNull("PTF_TRANSPARISE", "Groupe", "Dateinventaire", datee, "Numero_Niveau", 2).Count = 0 And RbNiv2.Checked = True) Then
                            co.ProcedureStockée("Trans2", paramName, paramDonnee)
                        End If
                    End If

                    'En fonction des choix de l'utilisateur on créer notre requete sql select ... where ...
                    Dim colSelect As List(Of String) = New List(Of String)
                    Dim nomBase As String
                    Dim colName As List(Of String) = New List(Of String)
                    Dim donnee As List(Of Object) = New List(Of Object)

                    If (RbNiv0.Checked = True) Then
                        nomBase = "PTF_FGA"
                        colSelect.Add("Groupe")
                        colSelect.Add("Libelle_Ptf AS 'Libelle portefeuille'")
                        colSelect.Add("isin_titre AS 'Isin titre'")
                        colSelect.Add("Libelle_Titre AS 'Libelle titre'")
                        'faire somme VB+CC
                        colSelect.Add("FLOOR(Valeur_Boursiere + Coupon_Couru) AS 'Valeur Boursiere'")
                        colSelect.Add("Type_Produit AS 'Type produit'")
                        colSelect.Add("Secteur")
                        colSelect.Add("Sous_Secteur AS 'Sous secteur'")
                        colSelect.Add("Pays")
                        colSelect.Add("Grp_Emetteur AS 'Groupe émetteur'")
                        colSelect.Add("Maturite AS 'Maturité'")
                        colSelect.Add("duration AS 'Duration'")
                        colSelect.Add("Rating")
                    Else
                        nomBase = "PTF_TRANSPARISE"
                        colSelect.Add("Groupe")
                        colSelect.Add("Libelle_Ptf AS 'Libelle ptf'")
                        colSelect.Add("isin_titre AS 'Isin titre'")
                        colSelect.Add("Libelle_Titre AS 'Libelle titre'")
                        'faire somme VB+CC
                        colSelect.Add("FLOOR(Valeur_Boursiere + Coupon_Couru) AS 'Valeur Boursiere'")
                        colSelect.Add("Type_Produit AS 'Type produit'")
                        colSelect.Add("Secteur")
                        colSelect.Add("Sous_Secteur AS 'Sous secteur'")
                        colSelect.Add("Pays")
                        colSelect.Add("Grp_Emetteur AS 'Groupe émetteur'")
                        colSelect.Add("Maturite AS 'Maturité'")
                        colSelect.Add("duration AS 'Duration'")
                        colSelect.Add("isin_origine_niv_1 AS 'Isin origine niveau 1'")
                        If (RbNiv2.Checked = True) Then
                            colSelect.Add("isin_origine_niv_2 AS 'Isin origine niveau 2'")
                        End If
                        colSelect.Add("Zone_Géo AS 'Zone géographique'")
                        colSelect.Add("Type_actif AS 'Type actif'")
                        colSelect.Add("Type_de_dette AS 'Type de dette'")
                        colSelect.Add("groupe_rating AS 'Groupe de rating'")
                        colSelect.Add("Tranche_de_maturite AS 'Tranche de maturité'")
                    End If

                    If (String.IsNullOrEmpty(datee) = False) Then
                        colName.Add("dateinventaire")
                        donnee.Add(datee)
                    End If

                    Dim colOr As String = String.Empty
                    Dim donneOr As List(Of Object) = New List(Of Object)
                    'Attention traitement spéciale pour lot de portefeuille
                    If (String.IsNullOrEmpty(CbLotPortefeuille.Text) = False) Then
                        Dim libelle_lot As String = CbLotPortefeuille.Text
                        Dim type_lot As String = co.SelectDistinctWhere("PTF_LOT", "type_lot", "libelle_lot", libelle_lot).First()
                        Dim id_lot As String = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", libelle_lot).First()

                        Select Case type_lot
                            Case "Grp"
                                colOr = "groupe"
                                donneOr = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", id_lot)
                            Case "Ptf"
                                colOr = "compte"
                                donneOr = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", id_lot)
                            Case Else
                                MessageBox.Show("Lot de portefeuille non reconnu par le portefeuille !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End Select


                    End If

                    If (String.IsNullOrEmpty(TIsinTitre.Text) = False) Then
                        colName.Add("isin_titre")
                        donnee.Add(TIsinTitre.Text)
                    End If
                    If (String.IsNullOrEmpty(CbSecteur.Text) = False) Then
                        colName.Add("Secteur")
                        donnee.Add(CbSecteur.Text)
                    End If
                    If (String.IsNullOrEmpty(CbSousSecteur.Text) = False) Then
                        colName.Add("Sous_Secteur")
                        donnee.Add(CbSousSecteur.Text)
                    End If
                    If (String.IsNullOrEmpty(CbPays.Text) = False) Then
                        colName.Add("Pays")
                        donnee.Add(CbPays.Text)
                    End If
                    If (RbNiv0.Checked = False) Then
                        If (String.IsNullOrEmpty(CbTrancheMaturite.Text) = False) Then
                            colName.Add("Tranche_de_maturite")
                            donnee.Add(CbTrancheMaturite.Text)
                        End If
                    End If
                    If (String.IsNullOrEmpty(TLibelleTitre.Text) = False) Then
                        colName.Add("libelle_titre")
                        donnee.Add(" LIKE '%" & TLibelleTitre.Text & "%'")
                    End If
                    If (RbNiv1.Checked = True) Then
                        colName.Add("Numero_Niveau")
                        donnee.Add(1)
                    End If
                    If (RbNiv2.Checked = True) Then
                        colName.Add("Numero_Niveau")
                        donnee.Add(2)
                    End If

                    'On remplie la grille en fonction des filtres de l'utilisateur
                    Dim sql As String = SelectDataGrid(colSelect, nomBase, colName, donnee, colOr, donneOr)
                    DataGridSelect.DataSource = co.LoadDataGridByString(sql)

                    Dim colAggregation As String = String.Empty
                    colSelect.Clear()
                    'On check quelle agregation souhaite l'utilisateur
                    If (RbGroupeEmetteur.Checked = True) Then
                        colAggregation = "Grp_Emetteur"
                        colSelect.Add("Grp_Emetteur AS 'Groupe émetteur'")
                    End If
                    If (RbSecteurs.Checked = True) Then
                        colAggregation = "Secteur"
                        colSelect.Add("Secteur")
                    End If
                    If (RbSousSecteurs.Checked = True) Then
                        colAggregation = "Sous_secteur"
                        colSelect.Add("Sous_secteur AS 'Sous secteur'")
                    End If
                    If (RbPays.Checked = True) Then
                        colAggregation = "Pays"
                        colSelect.Add("Pays")
                    End If
                    If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                        If (RbZoneGeographique.Checked = True) Then
                            colAggregation = "Zone_Géo"
                            colSelect.Add("Zone_Géo AS 'Zone géographique'")
                        End If
                    End If
                    If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                        If (RbTypeDeDette.Checked = True) Then
                            colAggregation = "Type_de_dette"
                            colSelect.Add("Type_de_dette AS 'Type de dette'")
                        End If
                    End If
                    If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                        If (RbTypeActif.Checked = True) Then
                            colAggregation = "Type_actif"
                            colSelect.Add("Type_actif AS 'Type actif'")
                        End If
                    End If
                    If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                        If (RbRating.Checked = True) Then
                            colAggregation = "Groupe_rating"
                            colSelect.Add("Groupe_rating AS 'Groupe rating'")
                        End If
                    End If
                    If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                        If (RbTrancheMaturite.Checked = True) Then
                            colAggregation = "Tranche_de_maturite"
                            colSelect.Add("Tranche_de_maturite AS 'Tranche de maturité'")
                        End If
                    End If
                    'faire somme VB+CC

                    'On remplie la grille en fonction de l'agregation de l'utilisateur
                    Dim sql2 As String = AggrDataGrid(colSelect, colAggregation, nomBase, colName, donnee, colOr, donneOr)
                    DataGridAggr.DataSource = co.LoadDataGridByString(sql2)
                    log.Log(ELog.Information, "BCharger_Click", "Affichage de la transparence + aggrégation dans la datagrid !")
                    Windows.Forms.Cursor.Current = Cursors.Default
                Else
                    MessageBox.Show("Impossible de transpariser le niveau  0 et/ou 1 et/ou 2 à la date " & datee & " car il n'y a pas de donées pour cette date dans les tables de concordance PTF_TYPE_ACTIF et/ou PTF_CARAC_OPCVM et/ou PTF_LOT et/ou PTF_TYPE_DE_DETTE et/ou ZONE_GEOGRAPHIQUE", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If
            Else
                MessageBox.Show("Impossible de transpariser le niveau  0 et/ou 1 et/ou 2 à la date " & datee & " car il n'y a pas de donées pour cette date dans la table PTF_FGA et/ou PTF_AN et/ou PTF_PROXY et/ou PTF_PARAM_PROXY", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If

            BCharger.Enabled = True
            BEffacer.Enabled = True
            BExcel.Enabled = True
            BPrint.Enabled = True
        End Sub

        ''' <summary>
        ''' BVider :  vide les composants graphiques
        ''' </summary>
        Private Sub BVider_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BEffacer.Click
            'Vider les deux datagrids
            DataGridSelect.DataSource = Nothing
            DataGridAggr.DataSource = Nothing

            'Vider les combobox
            CbLotPortefeuille.SelectedIndex = -1
            TIsinTitre.Clear()
            TLibelleTitre.Clear()
            CbSecteur.SelectedIndex = -1
            CbSousSecteur.SelectedIndex = -1
            CbPays.SelectedIndex = -1
            CbTrancheMaturite.SelectedIndex = -1
        End Sub

        ''' <summary>
        ''' Exporte une data grille vers un fichier excel
        ''' </summary>
        Private Sub ToExcel2(ByVal dataGridView1 As DataGridView, ByVal dataGridView2 As DataGridView)
            Try
                'Choix d'un fichier de destination
                If SaveFileDialog.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                    'On valide l'édition de la DataGridView
                    dataGridView1.EndEdit()
                    'On se prépare une mémoire des données formatées a écrire dans le fichier
                    Dim ToSave As New StringBuilder()
                    'On vas y mettre les en têtes tant demandées :-)
                    Dim Headers As String = String.Empty
                    For index As Integer = 0 To dataGridView1.Columns.Count - 1
                        Headers &= dataGridView1.Columns(index).HeaderText & ";"
                    Next
                    'La boucle ajoute un ";" a la fin qui est inutile
                    Headers = Headers.Remove(Headers.LastIndexOf(";"), 1)
                    'Maintenant qu'il est "propre" on le stocke dans la mémoire
                    ToSave.AppendLine(Headers)

                    'On boucle sur toutes les lignes disponibles
                    For i As UInt64 = 0 To dataGridView1.Rows.Count - 1
                        'On se fait une variable pour y stocké une ligne
                        Dim OneRow As String = String.Empty
                        'On peut faire une boucle sur toutes les colonnes disponible si la ligne n'est pas vide
                        If dataGridView1.Rows(i).IsNewRow = False Then
                            For j As Integer = 0 To dataGridView1.Rows(i).Cells.Count - 1
                                OneRow &= dataGridView1.Rows(i).Cells(j).Value & ";"
                            Next
                            'La boucle ajoute un ";" a la fin qui est inutile
                            OneRow = OneRow.Remove(OneRow.LastIndexOf(";"), 1)
                            'Maintenant qu'il est "propre" on le stocke dans la mémoire
                            ToSave.AppendLine(OneRow)
                        End If
                    Next
                    'Tout est bien qui finit bien ? essayons maintenant d'écrire le fichier
                    IO.File.WriteAllText(SaveFileDialog.FileName, ToSave.ToString(), Encoding.Default)


                    'On valide l'édition de la DataGridView
                    dataGridView2.EndEdit()
                    'On se prépare une mémoire des données formatées a écrire dans le fichier
                    Dim ToSave2 As New StringBuilder()
                    'On vas y mettre les en têtes tant demandées :-)
                    Dim Headers2 As String = String.Empty
                    For index As Integer = 0 To dataGridView2.Columns.Count - 1
                        Headers2 &= dataGridView2.Columns(index).HeaderText & ";"
                    Next
                    'La boucle ajoute un ";" a la fin qui est inutile
                    Headers2 = Headers2.Remove(Headers2.LastIndexOf(";"), 1)
                    'Maintenant qu'il est "propre" on le stocke dans la mémoire
                    ToSave2.AppendLine(Headers2)

                    'On boucle sur toutes les lignes disponibles
                    For i As UInt64 = 0 To dataGridView2.Rows.Count - 1
                        'On se fait une variable pour y stocké une ligne
                        Dim OneRow As String = String.Empty
                        'On peut faire une boucle sur toutes les colonnes disponible si la ligne n'est pas vide
                        If dataGridView2.Rows(i).IsNewRow = False Then
                            For j As Integer = 0 To dataGridView2.Rows(i).Cells.Count - 1
                                OneRow &= dataGridView2.Rows(i).Cells(j).Value & ";"
                            Next
                            'La boucle ajoute un ";" a la fin qui est inutile
                            OneRow = OneRow.Remove(OneRow.LastIndexOf(";"), 1)
                            'Maintenant qu'il est "propre" on le stocke dans la mémoire
                            ToSave2.AppendLine(OneRow)
                        End If
                    Next
                    'Tout est bien qui finit bien ? essayons maintenant d'écrire le fichier
                    Dim oldName As String = New IO.FileInfo(SaveFileDialog.FileName.ToString).Name
                    Dim newName As String = New IO.FileInfo(SaveFileDialog.FileName.ToString).DirectoryName & "\" & Split(oldName, ".")(0) & "(2)." & Split(oldName, ".")(1)
                    IO.File.WriteAllText(newName, ToSave2.ToString(), Encoding.Default)
                End If

            Catch ex As Exception
                MessageBox.Show(String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
            End Try
        End Sub

        ''' <summary>
        ''' BExcel : Exporter les données de la datagridSelect vers Excel
        ''' </summary>
        Private Sub BExcel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BExcel.Click
            BCharger.Enabled = False
            BEffacer.Enabled = False
            BExcel.Enabled = False

            'Transfert vers Excel
            ToExcel2(DataGridSelect, DataGridAggr)

            BCharger.Enabled = True
            BEffacer.Enabled = True
            BExcel.Enabled = True
        End Sub

        ''' <summary>
        ''' Interdire a l'utilisateur certaine aggrégation si niveau 0
        ''' </summary>
        Private Sub RbNiv0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbNiv0.CheckedChanged
            If (RbNiv0.Checked = True) Then
                RbRating.Enabled = False
                RbZoneGeographique.Enabled = False
                RbTypeActif.Enabled = False
                RbTypeDeDette.Enabled = False
                RbTrancheMaturite.Enabled = False
                CbTrancheMaturite.Enabled = False
            Else
                RbRating.Enabled = True
                RbZoneGeographique.Enabled = True
                RbTypeActif.Enabled = True
                RbTypeDeDette.Enabled = True
                RbTrancheMaturite.Enabled = True
                CbTrancheMaturite.Enabled = True
            End If
        End Sub

        ''' <summary>
        ''' Remplie le composant CbSecteur en fonction des autres pré-remplie
        ''' </summary>
        Private Sub CbSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSecteur.Click
            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date
            CbSecteur.DataSource = Nothing
            CbSecteur.Items.Clear()

            Dim niveau As Integer
            Dim nomTable As String
            If (RbNiv0.Checked = True) Then
                niveau = 0
                nomTable = "PTF_FGA"
            ElseIf (RbNiv1.Checked = True) Then
                niveau = 1
                nomTable = "PTF_TRANSPARISE"
            Else
                niveau = 2
                nomTable = "PTF_TRANSPARISE"
            End If

            'Recupération des differents filtres
            Dim colName As List(Of String) = New List(Of String)
            Dim donnee As List(Of Object) = New List(Of Object)
            If (String.IsNullOrEmpty(datee) = False) Then
                colName.Add("dateinventaire")
                donnee.Add(datee)
            End If
            Dim colOr As String = String.Empty
            Dim donneOr As List(Of Object) = New List(Of Object)
            'Attention traitement spéciale pour lot de portefeuille
            If (String.IsNullOrEmpty(CbLotPortefeuille.Text) = False) Then
                Dim libelle_lot As String = CbLotPortefeuille.Text
                Dim type_lot As String = co.SelectDistinctWhere("PTF_LOT", "type_lot", "libelle_lot", libelle_lot).First()
                Dim id_lot As String = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", libelle_lot).First()

                Select Case type_lot
                    Case "Grp"
                        colOr = "groupe"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", id_lot)
                    Case "Ptf"
                        colOr = "compte"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", id_lot)
                    Case Else
                        MessageBox.Show("Lot de portefeuille non reconnu par le portefeuille !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End Select
            End If
            If (String.IsNullOrEmpty(TIsinTitre.Text) = False) Then
                colName.Add("isin_titre")
                donnee.Add(TIsinTitre.Text)
            End If
            If (String.IsNullOrEmpty(CbSousSecteur.Text) = False) Then
                colName.Add("Sous_Secteur")
                donnee.Add(CbSousSecteur.Text)
            End If
            If (String.IsNullOrEmpty(CbPays.Text) = False) Then
                colName.Add("Pays")
                donnee.Add(CbPays.Text)
            End If
            If (RbNiv0.Checked = False) Then
                If (String.IsNullOrEmpty(CbTrancheMaturite.Text) = False) Then
                    colName.Add("Tranche_de_maturite")
                    donnee.Add(CbTrancheMaturite.Text)
                End If
            End If
            If (String.IsNullOrEmpty(TLibelleTitre.Text) = False) Then
                colName.Add("libelle_titre")
                donnee.Add(" LIKE '%" & TLibelleTitre.Text & "%'")
            End If
            If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                colName.Add("Numero_Niveau")
                donnee.Add(niveau)
            End If

            CbSecteur.DataSource = co.SelectDistinctWheres(nomTable, "Secteur", colName, donnee, colOr, donneOr)
        End Sub

        ''' <summary>
        ''' Remplie le composant CbSousSecteur en fonction des autres pré-remplie
        ''' </summary>
        Private Sub CbSousSecteur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSousSecteur.Click
            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date
            CbSousSecteur.DataSource = Nothing
            CbSousSecteur.Items.Clear()

            Dim niveau As Integer
            Dim nomTable As String
            If (RbNiv0.Checked = True) Then
                niveau = 0
                nomTable = "PTF_FGA"
            ElseIf (RbNiv1.Checked = True) Then
                niveau = 1
                nomTable = "PTF_TRANSPARISE"
            Else
                niveau = 2
                nomTable = "PTF_TRANSPARISE"
            End If

            'Recupération des differents filtres
            Dim colName As List(Of String) = New List(Of String)
            Dim donnee As List(Of Object) = New List(Of Object)
            If (String.IsNullOrEmpty(datee) = False) Then
                colName.Add("dateinventaire")
                donnee.Add(datee)
            End If
            Dim colOr As String = String.Empty
            Dim donneOr As List(Of Object) = New List(Of Object)
            'Attention traitement spéciale pour lot de portefeuille
            If (String.IsNullOrEmpty(CbLotPortefeuille.Text) = False) Then
                Dim libelle_lot As String = CbLotPortefeuille.Text
                Dim type_lot As String = co.SelectDistinctWhere("PTF_LOT", "type_lot", "libelle_lot", libelle_lot).First()
                Dim id_lot As String = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", libelle_lot).First()

                Select Case type_lot
                    Case "Grp"
                        colOr = "groupe"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", id_lot)
                    Case "Ptf"
                        colOr = "compte"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", id_lot)
                    Case Else
                        MessageBox.Show("Lot de portefeuille non reconnu par le portefeuille !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End Select
            End If
            If (String.IsNullOrEmpty(TIsinTitre.Text) = False) Then
                colName.Add("isin_titre")
                donnee.Add(TIsinTitre.Text)
            End If
            If (String.IsNullOrEmpty(CbSecteur.Text) = False) Then
                colName.Add("Secteur")
                donnee.Add(CbSecteur.Text)
            End If
            If (String.IsNullOrEmpty(CbPays.Text) = False) Then
                colName.Add("Pays")
                donnee.Add(CbPays.Text)
            End If
            If (RbNiv0.Checked = False) Then
                If (String.IsNullOrEmpty(CbTrancheMaturite.Text) = False) Then
                    colName.Add("Tranche_de_maturite")
                    donnee.Add(CbTrancheMaturite.Text)
                End If
            End If
            If (String.IsNullOrEmpty(TLibelleTitre.Text) = False) Then
                colName.Add("libelle_titre")
                donnee.Add(" LIKE '%" & TLibelleTitre.Text & "%'")
            End If
            If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                colName.Add("Numero_Niveau")
                donnee.Add(niveau)
            End If

            CbSousSecteur.DataSource = co.SelectDistinctWheres(nomTable, "Sous_Secteur", colName, donnee, colOr, donneOr)
        End Sub

        ''' <summary>
        ''' Remplie le composant CbPays en fonction des autres pré-remplie
        ''' </summary>
        Private Sub CbPays_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPays.Click
            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date
            CbPays.DataSource = Nothing
            CbPays.Items.Clear()

            Dim niveau As Integer
            Dim nomTable As String
            If (RbNiv0.Checked = True) Then
                niveau = 0
                nomTable = "PTF_FGA"
            ElseIf (RbNiv1.Checked = True) Then
                niveau = 1
                nomTable = "PTF_TRANSPARISE"
            Else
                niveau = 2
                nomTable = "PTF_TRANSPARISE"
            End If

            'Recupération des differents filtres
            Dim colName As List(Of String) = New List(Of String)
            Dim donnee As List(Of Object) = New List(Of Object)
            If (String.IsNullOrEmpty(datee) = False) Then
                colName.Add("dateinventaire")
                donnee.Add(datee)
            End If
            Dim colOr As String = String.Empty
            Dim donneOr As List(Of Object) = New List(Of Object)
            'Attention traitement spéciale pour lot de portefeuille
            If (String.IsNullOrEmpty(CbLotPortefeuille.Text) = False) Then
                Dim libelle_lot As String = CbLotPortefeuille.Text
                Dim type_lot As String = co.SelectDistinctWhere("PTF_LOT", "type_lot", "libelle_lot", libelle_lot).First()
                Dim id_lot As String = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", libelle_lot).First()

                Select Case type_lot
                    Case "Grp"
                        colOr = "groupe"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", id_lot)
                    Case "Ptf"
                        colOr = "compte"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", id_lot)
                    Case Else
                        MessageBox.Show("Lot de portefeuille non reconnu par le portefeuille !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End Select
            End If
            If (String.IsNullOrEmpty(TIsinTitre.Text) = False) Then
                colName.Add("isin_titre")
                donnee.Add(TIsinTitre.Text)
            End If
            If (String.IsNullOrEmpty(CbSecteur.Text) = False) Then
                colName.Add("Secteur")
                donnee.Add(CbSecteur.Text)
            End If
            If (String.IsNullOrEmpty(CbSousSecteur.Text) = False) Then
                colName.Add("Sous_Secteur")
                donnee.Add(CbSousSecteur.Text)
            End If
            If (RbNiv0.Checked = False) Then
                If (String.IsNullOrEmpty(CbTrancheMaturite.Text) = False) Then
                    colName.Add("Tranche_de_maturite")
                    donnee.Add(CbTrancheMaturite.Text)
                End If
            End If
            If (String.IsNullOrEmpty(TLibelleTitre.Text) = False) Then
                colName.Add("libelle_titre")
                donnee.Add(" LIKE '%" & TLibelleTitre.Text & "%'")
            End If
            If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                colName.Add("Numero_Niveau")
                donnee.Add(niveau)
            End If

            CbPays.DataSource = co.SelectDistinctWheres(nomTable, "pays", colName, donnee, colOr, donneOr)
        End Sub

        ''' <summary>
        ''' Remplie le composant CbTrancheDeMaturité en fonction des autres pré-remplie
        ''' </summary>
        Private Sub CbTrancheMaturite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTrancheMaturite.Click
            Dim datee As DateTime = MonthCalendar.SelectionRange.Start.Date
            CbTrancheMaturite.DataSource = Nothing
            CbTrancheMaturite.Items.Clear()

            Dim niveau As Integer
            Dim nomTable As String
            If (RbNiv0.Checked = True) Then
                niveau = 0
                nomTable = "PTF_FGA"
            ElseIf (RbNiv1.Checked = True) Then
                niveau = 1
                nomTable = "PTF_TRANSPARISE"
            Else
                niveau = 2
                nomTable = "PTF_TRANSPARISE"
            End If

            'Recupération des differents filtres
            Dim colName As List(Of String) = New List(Of String)
            Dim donnee As List(Of Object) = New List(Of Object)
            If (String.IsNullOrEmpty(datee) = False) Then
                colName.Add("dateinventaire")
                donnee.Add(datee)
            End If
            Dim colOr As String = String.Empty
            Dim donneOr As List(Of Object) = New List(Of Object)
            'Attention traitement spéciale pour lot de portefeuille
            If (String.IsNullOrEmpty(CbLotPortefeuille.Text) = False) Then
                Dim libelle_lot As String = CbLotPortefeuille.Text
                Dim type_lot As String = co.SelectDistinctWhere("PTF_LOT", "type_lot", "libelle_lot", libelle_lot).First()
                Dim id_lot As String = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", libelle_lot).First()

                Select Case type_lot
                    Case "Grp"
                        colOr = "groupe"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", id_lot)
                    Case "Ptf"
                        colOr = "compte"
                        donneOr = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", id_lot)
                    Case Else
                        MessageBox.Show("Lot de portefeuille non reconnu par le portefeuille !", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End Select
            End If
            If (String.IsNullOrEmpty(TIsinTitre.Text) = False) Then
                colName.Add("isin_titre")
                donnee.Add(TIsinTitre.Text)
            End If
            If (String.IsNullOrEmpty(CbSecteur.Text) = False) Then
                colName.Add("Secteur")
                donnee.Add(CbSecteur.Text)
            End If
            If (String.IsNullOrEmpty(CbSousSecteur.Text) = False) Then
                colName.Add("Sous_Secteur")
                donnee.Add(CbSousSecteur.Text)
            End If
            If (String.IsNullOrEmpty(CbPays.Text) = False) Then
                colName.Add("Pays")
                donnee.Add(CbPays.Text)
            End If
            If (String.IsNullOrEmpty(TLibelleTitre.Text) = False) Then
                colName.Add("libelle_titre")
                donnee.Add(" LIKE '%" & TLibelleTitre.Text & "%'")
            End If
            If (RbNiv1.Checked = True Or RbNiv2.Checked = True) Then
                colName.Add("Numero_Niveau")
                donnee.Add(niveau)
            End If

            CbTrancheMaturite.DataSource = co.SelectDistinctWheres(nomTable, "Tranche_de_maturite", colName, donnee, colOr, donneOr)
        End Sub

        ''' <summary>
        ''' Remplie le composant CbLotPortefeuille
        ''' </summary>
        Private Sub CbLotPortefeuille_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbLotPortefeuille.Click
            CbLotPortefeuille.DataSource = Nothing
            CbLotPortefeuille.Items.Clear()
            CbLotPortefeuille.DataSource = co.SelectDistinctSimple("PTF_LOT", "libelle_lot")
        End Sub

        ''' <summary>
        ''' Requete sql pour les filtres de la SelectDataGrid
        ''' </summary>
        Public Function SelectDataGrid(ByVal colSelect As List(Of String), ByVal nomBase As String, ByVal colNames As List(Of String), ByVal donnee As List(Of Object), Optional ByVal colOR As String = "", Optional ByVal donneeOR As List(Of Object) = Nothing) As String
            Dim sql, type As String

            'Construction de la chaine de facon dynamique
            sql = "SELECT "
            For i = 0 To colSelect.Count - 1 Step 1
                'If (i = 4) Then
                'sql = sql & " FLOOR(SUM(Valeur_Boursiere + Coupon_Couru)) AS 'Valeur Boursiere', "
                'End If
                If (i = colSelect.Count - 1) Then
                    sql = sql & colSelect(i) & "  FROM " & nomBase & " WHERE "
                    Exit For
                End If
                sql = sql & colSelect(i) & ", "
            Next

            For i = 0 To colNames.Count - 1 Step 1
                type = TypeName(donnee(i))
                sql = sql & colNames(i)
                If (Split(donnee(i), "LIKE")(0) <> " ") Then
                    sql = sql & " = "
                End If
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & Replace(donnee(i), "'", "''")
                Else
                    sql = sql & donnee(i)
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                If (i <> colNames.Count - 1) Then
                    sql = sql & " AND "
                End If
            Next
            'rajouter la deuxieme condition
            If (colOR <> "") Then
                sql = sql & " AND " & colOR & " IN ("
                For i = 0 To donneeOR.Count - 1 Step 1
                    type = TypeName(donneeOR(i))
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    sql = sql & donneeOR(i)
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    If (i <> donneeOR.Count - 1) Then
                        sql = sql & ","
                    End If
                Next
                sql = sql & ")"
            End If

            'sql = sql & " GROUP BY "

            'For i = 0 To colSelect.Count - 1 Step 1
            'If (i = colSelect.Count - 1) Then
            'sql = sql & Split(colSelect(i), " AS ")(0)
            'Exit For
            'End If
            'sql = sql & Split(colSelect(i), " AS ")(0) & ", "
            'Next

            'If (colOR <> "") Then
            'sql = sql & "," & colOR
            'End If

            Return sql
        End Function

        ''' <summary>
        ''' Requete sql pour l'aggregation de la AggrDataGrid
        ''' </summary>
        Public Function AggrDataGrid(ByVal colSelect As List(Of String), ByVal aggregation As String, ByVal nomBase As String, ByVal colNames As List(Of String), ByVal donnee As List(Of Object), Optional ByVal colOR As String = "", Optional ByVal donneeOR As List(Of Object) = Nothing) As String
            Dim sql, type As String

            'Example de commande voulu
            'declare @total AS FLOAT
            'SET @total = (SELECT SUM(Valeur_Boursiere + Coupon_Couru) FROM PTF_TRANSPARISE WHERE dateinventaire = '31/03/2011' AND libelle_titre LIKE '%a%' AND Numero_Niveau = 1)
            'SELECT Pays, SUM(Valeur_Boursiere + Coupon_Couru) AS 'Valeur Boursiere', SUM(Valeur_Boursiere + Coupon_Couru)/@total AS 'Poids' FROM PTF_TRANSPARISE WHERE dateinventaire = '31/03/2011' AND libelle_titre LIKE '%a%' AND Numero_Niveau = 1 Group BY Pays

            sql = "declare @total AS FLOAT SET @total = (SELECT FLOOR(SUM(Valeur_Boursiere + Coupon_Couru)) FROM " & nomBase & " WHERE "

            For i = 0 To colNames.Count - 1 Step 1
                type = TypeName(donnee(i))
                sql = sql & colNames(i)
                If (Split(donnee(i), "LIKE")(0) <> " ") Then
                    sql = sql & " = "
                End If
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & Replace(donnee(i), "'", "''")
                Else
                    sql = sql & donnee(i)
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                If (i <> colNames.Count - 1) Then
                    sql = sql & " AND "
                End If
            Next

            If (colOR <> "") Then
                sql = sql & " AND " & colOR & " IN ("
                For i = 0 To donneeOR.Count - 1 Step 1
                    type = TypeName(donneeOR(i))
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    sql = sql & donneeOR(i)
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    If (i <> donneeOR.Count - 1) Then
                        sql = sql & ","
                    End If
                Next
                sql = sql & ")"
            End If

            sql = sql & ") "

            'Construction de la chaine de facon dynamique
            sql = sql & "SELECT " & colSelect(0) & ", FLOOR(SUM(Valeur_Boursiere + Coupon_Couru)) AS 'Valeur Boursiere',ROUND(SUM(Valeur_Boursiere + Coupon_Couru)/@total,4) AS 'Poids%' FROM " & nomBase & " WHERE "

            For i = 0 To colNames.Count - 1 Step 1
                type = TypeName(donnee(i))
                sql = sql & colNames(i)
                If (Split(donnee(i), "LIKE")(0) <> " ") Then
                    sql = sql & " = "
                End If
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & Replace(donnee(i), "'", "''")
                Else
                    sql = sql & donnee(i)
                End If


                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                If (i <> colNames.Count - 1) Then
                    sql = sql & " AND "
                End If
            Next

            If (colOR <> "") Then
                sql = sql & " AND " & colOR & " IN ("
                For i = 0 To donneeOR.Count - 1 Step 1
                    type = TypeName(donneeOR(i))
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    sql = sql & donneeOR(i)
                    If (type = "String" Or type = "Date") Then
                        sql = sql & "'"
                    End If
                    If (i <> donneeOR.Count - 1) Then
                        sql = sql & ","
                    End If
                Next
                sql = sql & ")"
            End If

            sql = sql & " GROUP BY " & aggregation

            Return sql
        End Function

        Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles myPrintDocument.PrintPage
            Dim more As Boolean = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more = True Then
                e.HasMorePages = True
            End If
        End Sub

        ''' <summary>
        ''' BPrint : imprime SelectDataGrid
        ''' </summary>
        Private Sub BPrint_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPrint.Click
            If MyDataGridViewPrinter.SetupThePrinting(DataGridSelect, myPrintDocument, Me.Text) Then
                myPrintDocument.Print()
            End If
        End Sub

        ''' <summary>
        ''' BRapport : création rapport
        ''' </summary>
        Private Sub BRapport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRapport.Click
            Dim niv As Integer
            If RbNiv2.Checked Then
                niv = 2
            Else
                niv = 1
            End If

            If co.SelectWhere2("PTF_TRANSPARISE", "dateinventaire", "dateinventaire", MonthCalendar.SelectionRange.Start.Date, "numero_niveau", niv, 1).Count > 0 Then
                Try
                    Windows.Forms.Cursor.Current = Cursors.WaitCursor
                    SaveFileDialog.FileName = CbRapport.Text & " " & Replace(MonthCalendar.SelectionRange.Start.Date, "/", "-") & ".xls"
                    If SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then

                        Dim sql As List(Of String) = New List(Of String)
                        Select Case CbRapport.Text
                            Case "Monitoring groupe"
                                'TODO: systeme de lecture de fichier template sql (present dans PROJET/SQL_SCRIPTS/RAPPORT/outputMonitoringGroupe) pour renseigner les parametres et execution de la requete du fichier
                                'Affichage dans excel
                                Dim requete As String = "declare @date as datetime"
                                requete &= " set @date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                                requete &= " declare @rapportCle as char(20)"
                                requete &= " set @rapportCle = 'MonitoringGroupe'"
                                requete &= " declare @niveauInventaire as tinyint"
                                requete &= " set @niveauInventaire = " & niv
                                requete &= " declare @returnCode as int"
                                requete &= " declare @nb int"
                                requete &= " set @nb = (Select count(*) from PTF_RAPPORT where cle = @rapportCle and date = @date)"
                                requete &= " IF @nb = 0 "
                                requete &= " BEGIN"
                                requete &= " exec @returnCode = ReportMonitoringGroupe @date,@niveauInventaire, @rapportCle OUTPUT"
                                requete &= " END"
                                requete = requete & " select  "
                                requete = requete & " gr0.classementRubrique,"
                                requete = requete & " gr0.libelle, "
                                requete = requete & " gr1.valeur as 'MM AGIRC' ,"
                                requete = requete & " gr2.valeur as 'MM ARRCO',  "
                                requete = requete & " gr3.valeur as 'RETRAITE',"
                                requete = requete & " gr4.valeur as 'MMP',"
                                requete = requete & " gr5.valeur as 'INPR',"
                                requete = requete & " gr6.valeur as 'CAPREVAL',"
                                requete = requete & " gr7.valeur as 'CMAV',"
                                requete = requete & " gr8.valeur as 'MUT2M',"
                                requete = requete & " gr9.valeur as 'SAPREM',   "
                                requete = requete & " gr10.valeur as 'AUXIA',"
                                requete = requete & " gr11.valeur as 'QUATREM',"
                                requete = requete & " gr12.valeur as 'AUTRES',"
                                requete = requete & " gr13.valeur as 'ASSURANCE',"
                                requete = requete & " gr14.valeur as 'IRCEM',"
                                requete = requete & " gr0.valeur as 'GLOBAL'"
                                requete = requete & " from PTF_RAPPORT as gr0"
                                requete = requete & " left outer join PTF_RAPPORT as gr1 on gr1.rubrique = gr0.rubrique"
                                requete = requete & " and gr1.Groupe= 'MM AGIRC' and gr1.cle = @rapportCle and gr1.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr2 on gr2.rubrique = gr0.rubrique"
                                requete = requete & " and gr2.Groupe= 'MM ARRCO' and gr2.cle = @rapportCle and gr2.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr3 on gr3.rubrique = gr0.rubrique"
                                requete = requete & " and gr3.Groupe= 'RETRAITE' and gr3.cle = @rapportCle and gr3.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr4 on gr4.rubrique = gr0.rubrique"
                                requete = requete & " and gr4.Groupe= 'MMP' and gr4.cle = @rapportCle and gr4.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr5 on gr5.rubrique = gr0.rubrique"
                                requete = requete & " and gr5.Groupe= 'INPR' and gr5.cle = @rapportCle and gr5.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr6 on gr6.rubrique = gr0.rubrique"
                                requete = requete & " and gr6.Groupe= 'CAPREVAL' and gr6.cle = @rapportCle and gr6.date = @date  "
                                requete = requete & " left outer join PTF_RAPPORT as gr7 on gr7.rubrique = gr0.rubrique"
                                requete = requete & " and gr7.Groupe= 'CMAV' and gr7.cle = @rapportCle and gr7.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr8 on gr8.rubrique = gr0.rubrique"
                                requete = requete & " and gr8.Groupe= 'MUT2M' and gr8.cle = @rapportCle and gr8.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr9 on gr9.rubrique = gr0.rubrique"
                                requete = requete & " and gr9.Groupe= 'SAPREM' and gr9.cle = @rapportCle and gr9.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr10 on gr10.rubrique = gr0.rubrique"
                                requete = requete & " and gr10.Groupe= 'AUXIA' and gr10.cle = @rapportCle and gr10.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr11 on gr11.rubrique = gr0.rubrique"
                                requete = requete & " and gr11.Groupe= 'QUATREM' and gr11.cle = @rapportCle and gr11.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr12 on gr12.rubrique = gr0.rubrique"
                                requete = requete & " and gr12.Groupe= 'AUTRES' and gr12.cle = @rapportCle and gr12.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr13 on gr13.rubrique = gr0.rubrique"
                                requete = requete & " and gr13.Groupe= 'ASSURANCE' and gr13.cle = @rapportCle and gr13.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr14 on gr14.rubrique = gr0.rubrique"
                                requete = requete & " and gr14.Groupe= 'IRCEM' and gr14.cle = @rapportCle and gr14.date = @date"
                                requete = requete & " where gr0.Groupe='FGA'  and gr0.cle = @rapportCle and gr0.date = @date"
                                requete = requete & " order by case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique  else gr0.classementRubrique  end, gr0.libelle"
                                sql.Add(requete)
                                co.SqlToExcelEndSave(sql, SaveFileDialog.FileName)
                                sql.Clear()
                                sql.Add("Monitoring")
                            Case "Monitoring emetteur"
                                'Affichage dans excel
                                Dim requete As String = "declare @date as datetime "
                                requete = requete & " set @date = '" & MonthCalendar.SelectionRange.Start.Date & "'"
                                requete = requete & " declare @rapportCle as char(20)"
                                requete = requete & " set @rapportCle = 'ConcentEmprunts'"
                                requete = requete & " select DISTINCT"
                                requete = requete & " gr0.classementRubrique,  "
                                requete = requete & " gr0.libelle, "
                                requete = requete & " gr0.valeur as 'MM AGIRC' ,"
                                requete = requete & " gr1.valeur as 'MM ARRCO',  "
                                requete = requete & " gr2.valeur as 'RETRAITE',"
                                requete = requete & " gr3.valeur as 'POIDS',"
                                requete = requete & " gr4.valeur as 'MMP',"
                                requete = requete & " gr5.valeur as 'MUT2M',"
                                requete = requete & " gr6.valeur as 'CMAV',"
                                requete = requete & " gr7.valeur as 'QUATREM',"
                                requete = requete & " gr8.valeur as 'AUXIA',"
                                requete = requete & " gr9.valeur as 'CAPREVAL',   "
                                requete = requete & " gr10.valeur as 'INPR',"
                                requete = requete & " gr11.valeur as 'SAPREM',"
                                requete = requete & " gr12.valeur as 'AUTRES',"
                                requete = requete & " gr13.valeur as 'ASSURANCE',"
                                requete = requete & " gr14.valeur as 'POIDS',"
                                requete = requete & " gr15.valeur as 'IRCEM'"
                                requete = requete & " from PTF_RAPPORT as gr0"
                                requete = requete & " left outer join PTF_RAPPORT as gr1 on gr1.rubrique = gr0.rubrique"
                                requete = requete & "  and gr1.ordreGroupe= 1 and gr1.cle = @rapportCle and gr1.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr2 on gr2.rubrique = gr0.rubrique"
                                requete = requete & "  and gr2.ordreGroupe= 2 and gr2.cle = @rapportCle and gr2.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr3 on gr3.rubrique = gr0.rubrique"
                                requete = requete & "  and gr3.ordreGroupe= 3 and gr3.cle = @rapportCle and gr3.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr4 on gr4.rubrique = gr0.rubrique"
                                requete = requete & " and gr4.ordreGroupe= 4 and gr4.cle = @rapportCle and gr4.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr5 on gr5.rubrique = gr0.rubrique"
                                requete = requete & " and gr5.ordreGroupe= 5 and gr5.cle = @rapportCle and gr5.date = @date"
                                requete = requete & "   left outer join PTF_RAPPORT as gr6 on gr6.rubrique = gr0.rubrique"
                                requete = requete & " and gr6.ordreGroupe= 6 and gr6.cle = @rapportCle and gr6.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr7 on gr7.rubrique = gr0.rubrique"
                                requete = requete & " and gr7.ordreGroupe= 7 and gr7.cle = @rapportCle and gr7.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr8 on gr8.rubrique = gr0.rubrique"
                                requete = requete & " and gr8.ordreGroupe= 8 and gr8.cle = @rapportCle and gr8.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr9 on gr9.rubrique = gr0.rubrique"
                                requete = requete & " and gr9.ordreGroupe= 9 and gr9.cle = @rapportCle and gr9.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr10 on gr10.rubrique = gr0.rubrique"
                                requete = requete & " and gr10.ordreGroupe= 10 and gr10.cle = @rapportCle and gr10.date = @date"
                                requete = requete & "  left outer join PTF_RAPPORT as gr11 on gr11.rubrique = gr0.rubrique"
                                requete = requete & "  and gr11.ordreGroupe= 11 and gr11.cle = @rapportCle and gr11.date = @date"
                                requete = requete & "  left outer join PTF_RAPPORT as gr12 on gr12.rubrique = gr0.rubrique"
                                requete = requete & "  and gr12.ordreGroupe= 12 and gr12.cle = @rapportCle and gr12.date = @date"
                                requete = requete & "  left outer join PTF_RAPPORT as gr13 on gr13.rubrique = gr0.rubrique"
                                requete = requete & "  and gr13.ordreGroupe= 13 and gr13.cle = @rapportCle and gr13.date = @date"
                                requete = requete & " left outer join PTF_RAPPORT as gr14 on gr14.rubrique = gr0.rubrique"
                                requete = requete & " and gr14.ordreGroupe= 14 and gr14.cle = @rapportCle and gr14.date = @date  "
                                requete = requete & " left outer join PTF_RAPPORT as gr15 on gr15.rubrique = gr0.rubrique"
                                requete = requete & " and gr15.ordreGroupe= 15 and gr15.cle = @rapportCle and gr15.date = @date "
                                requete = requete & " where gr0.ordreGroupe= 0 and gr0.cle = @rapportCle and gr0.date = @date"
                                requete = requete & " and gr0.rubrique like 'PAYS_%'"
                                requete = requete & " and gr13.valeur > 40000000"
                                requete = requete & "  order by gr13.valeur DESC"
                                sql.Add(requete)
                                co.SqlToExcelEndSave(sql, SaveFileDialog.FileName)
                                sql.Clear()
                                sql.Add("Monitoring emprunts")
                            Case Else
                                MsgBox("Le type de rapport n'est pas reconnu par l'application !")
                                Exit Sub
                        End Select
                        PresentationMonitoring(SaveFileDialog.FileName, sql)
                        Windows.Forms.Cursor.Current = Cursors.Default
                        SaveFileDialog.FileName = ""
                    End If
                Catch ex As Exception
                    MessageBox.Show("Un problème est survenu ! (fichier déjà ouvert " & SaveFileDialog.FileName & " ?)", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End Try
            Else
                MessageBox.Show("Pas de transparence niveau 2 pour cette date donnée", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' Présentation rapport excel
        ''' </summary>
        Private Sub PresentationMonitoring(ByRef cheminExcel As String, ByRef sheets As List(Of String))
            Dim app As Microsoft.Office.Interop.Excel.Application = CreateObject("Excel.Application")
            Dim book As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(cheminExcel)
            app.DisplayAlerts = False 'annule les messages

            For s = 0 To sheets.Count - 1 Step 1
                Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = book.Worksheets(s + 1)
                Dim nbrLigne, nbCol As Integer

                For Each f In sheet.Columns(1).Value()
                    If IsNothing(f) = False Then
                        nbrLigne = nbrLigne + 1
                    End If
                Next
                For i As Integer = 1 To 60
                    If sheet.Cells(1, i).Value <> "" Then
                        nbCol = nbCol + 1
                        If sheet.Cells(1, i).Value.ToString.Contains("ASSURANCE") Or sheet.Cells(1, i).Value.ToString.Contains("RETRAITE") Or sheet.Cells(1, i).Value.ToString.Contains("POIDS") Then
                            sheet.Columns(i).Interior.colorindex = 35 'vert pal
                        End If
                    End If
                Next


                'Traitement titre
                With sheet.Rows(1)
                    .HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter
                    .Interior.colorindex = 34
                    .Font.Bold = True
                End With
                sheet.Cells(1, 2).value() = MonthCalendar.SelectionRange.Start.Date

                'Traitement couleur colonne + trait colonne
                sheet.Columns(2).Interior.colorindex = 35
                sheet.Columns(2).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous

                'Fixer les volets
                sheet.Range("C2").Activate()
                app.ActiveWindow.FreezePanes = True

                'Présentation chiffre
                sheet.Columns("C:V").NumberFormat = "# ##0"

                'Couleur ligne
                Dim group1 As List(Of String) = New List(Of String)
                Dim group2 As List(Of String) = New List(Of String)
                Dim niveau1 As Double = 1   'car   1,   2,   3, ...
                Dim niveau2 As Double = 100 'car 101, 102, 103
                For i As Integer = 2 To nbrLigne
                    Select Case sheet.Cells(i, 1).Value()
                        Case 0
                        Case niveau1
                            sheet.Rows(i).Font.Bold = True
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium
                            sheet.Rows(i).Interior.ColorIndex = 37 'bleu
                            niveau2 = niveau1 * 100 + 1
                            niveau1 = niveau1 + 1
                            group1.Add(i)
                            group2.Add(i)
                        Case niveau2
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                            sheet.Range(sheet.Cells(i, 1), sheet.Cells(i, nbCol)).Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous
                            sheet.Cells(i, 2).Interior.ColorIndex = 2 'gris
                            niveau2 = niveau2 + 1
                            'If sheet.Cells(i + 1, 1).Value() <> niveau1 Then
                            group2.Add(i)
                            'End If
                        Case Else
                            sheet.Rows(i).Font.colorindex = 16
                    End Select
                Next

                'création des groupes
                For i = 0 To group2.Count - 2 Step 1
                    If group2(i) + 1 < group2(i + 1) - 1 Then
                        sheet.Rows(group2(i) + 1 & ":" & group2(i + 1) - 1).Group()
                    End If
                Next
                If group2.Count > 0 Then
                    sheet.Rows(group2(group2.Count - 1) + 1 & ":" & nbrLigne).Group()
                End If

                For i = 0 To group1.Count - 2 Step 1
                    If group1(i) + 1 < group1(i + 1) - 1 Then
                        sheet.Rows(group1(i) + 1 & ":" & group1(i + 1) - 1).Group()
                    End If
                Next
                If group1.Count > 0 Then
                    sheet.Rows(group1(group1.Count - 1) + 1 & ":" & nbrLigne + 1).Group()
                End If


                'Taille + Zoom
                sheet.Cells.Font.Size = 10
                app.ActiveWindow.Zoom = 70
                sheet.Cells.EntireColumn.AutoFit()
                sheet.Columns(1).EntireColumn.Hidden = True
                sheet.Name = sheets(s)
            Next


            app.ActiveWorkbook.SaveAs(cheminExcel)
            app.Workbooks.Close()
            app.DisplayAlerts = True
            app.Quit()
            app = Nothing
        End Sub

        Public Sub Preview(ByVal PrintCenterReportOnPage As Boolean, ByVal PrintFont As Font, ByVal PrintFontColor As Color) Implements PrintPreviewForm.Preview
            MyDataGridViewPrinter.Init_Parameters(DataGridSelect, myPrintDocument, PrintCenterReportOnPage, True, myPrintDocument.DocumentName, New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, PrintFont, PrintFontColor, True)
            Dim MyPrintPreviewDialog As New PrintPreviewDialog()
            MyPrintPreviewDialog.Document = myPrintDocument
            MyPrintPreviewDialog.ShowDialog()
        End Sub

    End Class
End Namespace
