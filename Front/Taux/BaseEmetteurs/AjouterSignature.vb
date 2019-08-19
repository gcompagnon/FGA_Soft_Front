Imports System.IO
Namespace Taux.BaseEmetteurs

    Public Class AjouterSignature

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()

        ''' <summary>
        ''' Load de la frame
        ''' </summary>
        Private Sub frmSample_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Tentative de connnection
            co.ToConnectBase()

            'Binder les CbRating    
            CbMoodysCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Moodys", "type_rating", "CT", 1, "ordre ASC")
            CbMoodysLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Moodys", "type_rating", "LT", 1, "ordre ASC")
            'CbMoodysLT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "Moodys", "type_rating", "LT", 1).Count - 2
            CbSandPCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1, "ordre ASC")
            'CbSandPCT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1).Count - 2
            CbSandBLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1, "ordre ASC")
            'CbSandBLT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1).Count - 2
            CbInternCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1, "ordre ASC")
            'CbInternCT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1).Count - 2
            CbInternLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1, "ordre ASC")
            'CbInternLT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1).Count - 2
            CbFitchLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "LT", 1, "ordre ASC")
            'CbFitchLT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "LT", 1).Count - 2
            CbFitchCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "CT", 1, "ordre ASC")
            'CbFitchCT.SelectedIndex = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "CT", 1).Count - 2

            'Changement des composants
            CbPays.DataSource = co.SelectDistinctSimple("PAYS", "libelle")

            'Dim name As 
            'Dim donnee As 
            CbSecteur.DataSource = co.SelectDistinctWheres("SECTEUR", "libelle", New List(Of String)(New String() {"id"}), New List(Of Object)(New Object() {" LIKE '%O %'"}))
            CbPays.SelectedIndex = -1
            CbSecteur.SelectedIndex = -1
            CbSousSecteur.SelectedIndex = -1


            If Utilisateur.admin = False Then
                BCréer.Enabled = False
            End If
        End Sub

        ''' <summary>
        ''' BCréer : ajout une nouvelle signature en base
        ''' </summary>
        Private Sub BCréer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCréer.Click
            If String.IsNullOrEmpty(TLibelle.Text) = False And String.IsNullOrEmpty(CbSecteur.Text) = False And String.IsNullOrEmpty(CbPays.Text) = False And String.IsNullOrEmpty(CbFitchCT.Text) = False And String.IsNullOrEmpty(CbFitchLT.Text) = False And String.IsNullOrEmpty(CbInternCT.Text) = False And String.IsNullOrEmpty(CbInternLT.Text) = False And String.IsNullOrEmpty(CbMoodysCT.Text) = False And String.IsNullOrEmpty(CbMoodysLT.Text) = False And String.IsNullOrEmpty(CbSandBLT.Text) = False And String.IsNullOrEmpty(CbSandPCT.Text) = False Then

                'Check si le code existe déja
                Dim name As List(Of String) = New List(Of String)()
                Dim donnee As List(Of Object) = New List(Of Object)()
                Dim datee As DateTime = DateTime.Now.ToString
                Dim code As String = String.Empty


                If co.SelectDistinctWhere("TX_SIGNATURE", "libelle", "libelle", TLibelle.Text).Count = 0 Then

                    'Ajout du groupe dans TX_GROUPE
                    If (String.IsNullOrEmpty(TGroupe.Text) = False) And co.SelectDistinctWhere("TX_GROUPE", "id", "libelle", TGroupe.Text).Count = 0 Then
                        name.Add("id")
                        name.Add("libelle")
                        name.Add("id_utilisateur")
                        donnee.Add(TGroupe.Text)
                        donnee.Add(TGroupe.Text)
                        donnee.Add(Utilisateur.login)
                        co.Insert("TX_GROUPE", name, donnee)
                        donnee.Clear()
                        name.Clear()
                    End If


                    'Ajout la nouvelle signature dans TX_SIGNATURE
                    code = TLibelle.Text
                    name.Add("code")
                    donnee.Add(code)
                    name.Add("libelle")
                    donnee.Add(TLibelle.Text)
                    name.Add("id_recommandation")
                    Dim recommandation As String
                    If RbNegatif.Checked Then
                        recommandation = "Neg"
                    ElseIf RbNeutre.Checked Then
                        recommandation = "Ne"
                    ElseIf RbNeutre_.Checked Then
                        recommandation = "Ne-"
                    ElseIf RbPositif.Checked Then
                        recommandation = "Pos"
                    ElseIf RbReview.Checked Then
                        recommandation = "Rev"
                    Else
                        recommandation = "Na"
                    End If
                    donnee.Add(recommandation)
                    name.Add("id_secteur")
                    donnee.Add(co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First)
                    If String.IsNullOrEmpty(CbSousSecteur.Text) = False Then
                        name.Add("id_sous_secteur")
                        donnee.Add(co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).First)
                    End If
                    If String.IsNullOrEmpty(TGroupe.Text) = False Then
                        name.Add("id_groupe")
                        donnee.Add(co.SelectDistinctWhere("TX_GROUPE", "id", "libelle", TGroupe.Text).First)
                    End If

                    name.Add("id_pays")
                    donnee.Add(co.SelectDistinctWhere("PAYS", "id", "libelle", CbPays.Text).First)
                    name.Add("id_rating_Moo_CT")
                    name.Add("id_rating_Moo_LT")
                    name.Add("id_rating_Sp_CT")
                    name.Add("id_rating_Sp_LT")
                    name.Add("id_rating_Fi_CT")
                    name.Add("id_rating_Fi_LT")
                    name.Add("id_rating_In_CT")
                    name.Add("id_rating_In_LT")

                    Dim id_rating As List(Of String) = New List(Of String)()
                    Dim nameRating As List(Of String) = New List(Of String)()
                    nameRating.Add("agence")
                    nameRating.Add("rating")
                    nameRating.Add("type_rating")
                    Dim donneeRating As List(Of Object) = New List(Of Object)()

                    donneeRating.Add("Moodys")
                    donneeRating.Add(CbMoodysCT.Text)
                    donneeRating.Add("CT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()
                    donneeRating.Add("Moodys")
                    donneeRating.Add(CbMoodysLT.Text)
                    donneeRating.Add("LT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()

                    donneeRating.Add("StandardandPoors")
                    donneeRating.Add(CbSandPCT.Text)
                    donneeRating.Add("CT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()
                    donneeRating.Add("StandardandPoors")
                    donneeRating.Add(CbSandBLT.Text)
                    donneeRating.Add("LT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()

                    donneeRating.Add("Fitch")
                    donneeRating.Add(CbFitchCT.Text)
                    donneeRating.Add("CT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()
                    donneeRating.Add("Fitch")
                    donneeRating.Add(CbFitchLT.Text)
                    donneeRating.Add("LT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()

                    donneeRating.Add("Interne")
                    donneeRating.Add(CbInternCT.Text)
                    donneeRating.Add("CT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()
                    donneeRating.Add("Interne")
                    donneeRating.Add(CbInternLT.Text)
                    donneeRating.Add("LT")
                    id_rating.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    donneeRating.Clear()

                    donnee.Add(id_rating(0))
                    donnee.Add(id_rating(1))
                    donnee.Add(id_rating(2))
                    donnee.Add(id_rating(3))
                    donnee.Add(id_rating(4))
                    donnee.Add(id_rating(5))
                    donnee.Add(id_rating(6))
                    donnee.Add(id_rating(7))

                    name.Add("commentaire")
                    donnee.Add(TCommentaire.Rtf)
                    name.Add("id_utilisateur")
                    donnee.Add(Utilisateur.login)

                    If TNoteISR.Text <> "" Then
                        If IsNumeric(TNoteISR.Text) Then
                            name.Add("note_isr")
                            donnee.Add(Convert.ToDouble(TNoteISR.Text))
                        End If
                    End If

                    co.Insert("TX_SIGNATURE", name, donnee)
                    name.Clear()
                    donnee.Clear()

                    'Ajout dans TX_HISTO_RECOMMANDATION
                    name.Add("id_recommandation")
                    name.Add("code_signature")
                    name.Add("date")
                    donnee.Add(recommandation)
                    donnee.Add(code)
                    donnee.Add(datee)
                    co.Insert("TX_HISTO_RECOMMANDATION", name, donnee)
                    donnee.Clear()
                    name.Clear()

                    'Ajout dans TX_HISTO_RATING
                    For Each f In id_rating
                        donnee.Clear()
                        name.Clear()
                        name.Add("id_rating")
                        name.Add("code_signature")
                        name.Add("date")
                        name.Add("id_utilisateur")
                        donnee.Add(f)
                        donnee.Add(code)
                        donnee.Add(datee)
                        donnee.Add(Utilisateur.login)
                        co.Insert("TX_HISTO_RATING", name, donnee)
                    Next

                    'Création du dossier si il existe pas
                    If Directory.Exists(co.SelectDistinctSimple("TX_RACINE", "chemin").First & "\" & TLibelle.Text) = False Then
                        Directory.CreateDirectory(co.SelectDistinctSimple("TX_RACINE", "chemin").First & "\" & TLibelle.Text)
                    End If

                    Me.Close()
                    Dim be As New BaseEmetteurs
                    be.Show()
                    log.Log(ELog.Information, "BCréer_Click", "la signature " & TLibelle.Text & " a été ajouté dans TX_SIGNATURE !")
                Else
                    MessageBox.Show("La signature " & TLibelle.Text & " existe déjà dans la table TX_SIGNATURE", "Echec de l'ajout de la nouvelle signature", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If
            Else
                MessageBox.Show("Il manque des informations avant d'archiver la signature dans TX_SIGNATURE", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' CbSousSecteur : derniere recommandation sous secteur
        ''' </summary>
        Private Sub CbSousSecteur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSousSecteur.SelectedIndexChanged
            Dim colName As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            colName.Add("@libelle")
            donnee.Add(CbSousSecteur.Text)
            DataGridSousSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastSousSecteurRecommandation", colName, donnee))
        End Sub

        ''' <summary>
        ''' CbPays : derniere recommandation pays
        ''' </summary>
        Private Sub CbPays_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPays.SelectedIndexChanged
            Dim colName As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            colName.Add("@libelle")
            donnee.Add(CbPays.Text)
            DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastPaysRecommandation", colName, donnee))
        End Sub

        ''' <summary>
        ''' CbSecteur : peupler sous secteur
        ''' </summary>
        Private Sub CbSecteur_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSecteur.SelectedValueChanged
            If String.IsNullOrEmpty(CbSecteur.Text) = False Then
                Dim id_secteur As Object = co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First
                CbSousSecteur.DataSource = co.SelectDistinctWhere("SOUS_SECTEUR", "libelle", "id_secteur", id_secteur)
            End If
        End Sub

        ''' <summary>
        ''' Resize de l'ihm
        ''' </summary>
        Private Sub AjoutSignature_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Dim Feuille As Form
            Feuille = Me
            Static Longueur As Long
            Static Hauteur As Long
            Dim PropLongueur As Single
            Dim PropHauteur As Single
            If ((Longueur > 0) And (Hauteur > 0)) Then
                PropLongueur = Feuille.Width / Longueur
                PropHauteur = Feuille.Height / Hauteur
                Dim Ctrl As Control
                On Error Resume Next
                For Each Ctrl In Feuille.Controls
                    Ctrl.Left = CInt(Ctrl.Left * PropLongueur)
                    Ctrl.Top = CInt(Ctrl.Top * PropHauteur)
                    Ctrl.Width = CInt(Ctrl.Width * PropLongueur)
                    Ctrl.Height = CInt(Ctrl.Height * PropHauteur)
                Next
                On Error GoTo 0
            End If
            Longueur = Feuille.Width
            Hauteur = Feuille.Height
        End Sub

        ''' <summary>
        ''' Ouvrir baseEmetteur et fermer Me
        ''' </summary>
        Private Sub AjouterSignature_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Dim be As New BaseEmetteurs
            be.Show()
        End Sub

        ''' <summary>
        ''' BPolice :  changement police de commentaire
        ''' </summary>
        Private Sub BPolice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPolice.Click
            If Not TCommentaire.SelectionFont Is Nothing Then
                FontDialog1.Font = TCommentaire.SelectionFont
            Else
                FontDialog1.Font = Nothing
            End If

            FontDialog1.ShowApply = True

            If FontDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                TCommentaire.SelectionFont = FontDialog1.Font
                TCommentaire.SelectionColor = FontDialog1.Color
            End If
        End Sub


    End Class
End Namespace