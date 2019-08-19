Imports System.IO
Namespace Taux.BaseEmetteurs
    Public Class ModificationSignature

        Dim log As Log = New Log()
        Dim co As Connection = New Connection()
        Dim dossier As Dossier = New Dossier()
        Dim fi As Fichier = New Fichier()
        Dim oldName As String
        Dim changeDate As Boolean = False
        Private checkPrint As Integer

        ''' <summary>
        ''' Load de l'ihm
        ''' </summary>
        Private Sub ModificationSignature_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Connection a la BDD
            co.ToConnectBase()
            TCommentaire.AutoSize = True
            TCommentaire2.AutoSize = True
            TDescription.AutoSize = True

            'Binder Secteur, Sous Secteur, Pays
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            CbSecteur.DataSource = co.SelectDistinctSimple("SECTEUR", "libelle")
            CbSecteur.Text = co.SelectDistinctWhere("SECTEUR", "libelle", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_secteur", "code", code).First).First
            If co.SelectDistinctWhere("TX_SIGNATURE", "id_sous_secteur", "code", code).Count > 0 Then
                CbSousSecteur.Text = co.SelectDistinctWhere("SOUS_SECTEUR", "libelle", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_sous_secteur", "code", code).First).First
            End If
            CbPays.DataSource = co.SelectDistinctSimple("PAYS", "libelle")
            CbPays.Text = co.SelectDistinctWhere("PAYS", "libelle", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_pays", "code", code).First).First

            'Binder les CbRating    
            CbMoodysCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Moodys", "type_rating", "CT", 1, "ordre ASC")
            CbMoodysCT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Moo_CT", "code", code).First).First
            CbMoodysLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Moodys", "type_rating", "LT", 1, "ordre ASC")
            CbMoodysLT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Moo_LT", "code", code).First).First
            CbSandPCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1, "ordre ASC")
            CbSandPCT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Sp_CT", "code", code).First).First
            CbSandPLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1, "ordre ASC")
            CbSandPLT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Sp_LT", "code", code).First).First
            CbInternCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "CT", 1, "ordre ASC")
            CbInternCT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_In_CT", "code", code).First).First
            CbInternLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "StandardandPoors", "type_rating", "LT", 1, "ordre ASC")
            CbInternLT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_In_LT", "code", code).First).First
            CbFitchLT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "LT", 1, "ordre ASC")
            CbFitchLT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Fi_LT", "code", code).First).First
            CbFitchCT.DataSource = co.SelectWhere2("TX_RATING", "rating", "agence", "Fitch", "type_rating", "CT", 1, "ordre ASC")
            CbFitchCT.Text = co.SelectDistinctWhere("TX_RATING", "rating", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Fi_CT", "code", code).First).First

            'Binder recommandation
            Dim recommandation As Object = co.SelectDistinctWhere("TX_SIGNATURE", "id_recommandation", "code", code).First
            If recommandation = "Neg" Then
                RbNegatif.Checked = True
            ElseIf recommandation = "Ne" Then
                RbNeutre.Checked = True
            ElseIf recommandation = "Ne-" Then
                RbNeutre_.Checked = True
            ElseIf recommandation = "Pos" Then
                RbPositif.Checked = True
            ElseIf recommandation = "Rev" Then
                RbReview.Checked = True
            Else
                RbNa.Checked = True
            End If

            'Binder groupe et commentaire et note_isr
            If co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).Count = 1 Then
                TGroupe.Text = co.SelectDistinctWhere("TX_GROUPE", "libelle", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code)(0)).First
            Else
                TGroupe.Clear()
            End If
            If co.SelectDistinctWhere("TX_SIGNATURE", "commentaire", "code", code).Count = 1 Then
                TCommentaire.Rtf = co.SelectDistinctWhere("TX_SIGNATURE", "commentaire", "code", code)(0)
            Else
                TCommentaire.Text = ""
            End If
            If co.SelectDistinctWhere("TX_SIGNATURE", "commentaire2", "code", code).Count = 1 Then
                TCommentaire2.Rtf = co.SelectDistinctWhere("TX_SIGNATURE", "commentaire2", "code", code)(0)
            Else
                TCommentaire2.Text = ""
            End If
            If co.SelectDistinctWhere("TX_SIGNATURE", "description", "code", code).Count = 1 Then
                TDescription.Rtf = co.SelectDistinctWhere("TX_SIGNATURE", "description", "code", code)(0)
            Else
                TDescription.Text = ""
            End If
            If co.SelectDistinctWhere("TX_SIGNATURE", "recommandation", "code", code).Count = 1 Then
                TRecommandation.Text = co.SelectDistinctWhere("TX_SIGNATURE", "recommandation", "code", code)(0)
            Else
                TRecommandation.Text = ""
            End If
            If co.SelectDistinctWhere("TX_SIGNATURE", "note_isr", "code", code).Count = 1 Then
                TNoteISR.Text = co.SelectDistinctWhere("TX_SIGNATURE", "note_isr", "code", code)(0)
            Else
                TNoteISR.Clear()
            End If


            'Binder Histo Commentaire, SousSecteur, Pays
            Dim colName As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            colName.Add("@libelle")
            donnee.Add(CbSousSecteur.Text)
            DataGridSousSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastSousSecteurRecommandation", colName, donnee))
            colName.Clear()
            donnee.Clear()
            colName.Add("@libelle")
            donnee.Add(CbPays.Text)
            DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastPaysRecommandation", colName, donnee))
            colName.Clear()
            donnee.Clear()
            DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id ORDER BY date desc")
            DataGridRating.DataSource = co.LoadDataGridByString("SELECT thr.date As 'Date', tr.agence As 'Agence', tr.rating As 'Rating', tr.type_rating As 'Horizon' FROM TX_HISTO_RATING thr, TX_RATING tr where thr.id_rating=tr.id AND thr.code_signature='" & code & "' ORDER BY 'Date' desc")
            DataGridRecom.DataSource = co.LoadDataGridByString("SELECT thr.date As 'Date', tr.libelle As 'Recommandation' FROM TX_HISTO_RECOMMANDATION thr, TX_RECOMMANDATION tr where thr.id_recommandation=tr.id AND thr.code_signature='" & code & "' ORDER BY 'Date' desc")

            If Utilisateur.admin = False Then
                BChanger.Enabled = False
                BSupprimer.Enabled = False
            End If

            DataGridFile.Columns(0).ReadOnly = True
            DataGridFile.Columns(2).ReadOnly = True
            DataGridFile.Columns(4).ReadOnly = True

            ' Set the CustomFormat string.
            DatePicker.CustomFormat = "dd'/'MM'/'yyyy"
            DatePicker.Format = DateTimePickerFormat.Custom
        End Sub

        ''' <summary>
        ''' Affiche le libellé choisi
        ''' </summary>
        Protected Friend Sub maj_libelle(ByVal text As String)
            TLibelle.Text = text
        End Sub

        ''' <summary>
        ''' BChanger : refresh signature dans la base
        ''' </summary>
        Private Sub BChanger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BChanger.Click
            If String.IsNullOrEmpty(TLibelle.Text) = False And String.IsNullOrEmpty(CbSecteur.Text) = False And String.IsNullOrEmpty(CbPays.Text) = False And String.IsNullOrEmpty(CbFitchCT.Text) = False And String.IsNullOrEmpty(CbFitchLT.Text) = False And String.IsNullOrEmpty(CbInternCT.Text) = False And String.IsNullOrEmpty(CbInternLT.Text) = False And String.IsNullOrEmpty(CbMoodysCT.Text) = False And String.IsNullOrEmpty(CbMoodysLT.Text) = False And String.IsNullOrEmpty(CbSandPLT.Text) = False And String.IsNullOrEmpty(CbSandPCT.Text) = False Then
                Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
                Dim datee As DateTime = DateTime.Now
                Dim changement As Boolean = False
                Dim colName As List(Of String) = New List(Of String)()
                Dim donnee As List(Of Object) = New List(Of Object)()

                'Modification dans TX_SIGNATURE
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
                If recommandation <> co.SelectDistinctWhere("TX_SIGNATURE", "id_recommandation", "code", code).First Then
                    colName.Add("id_recommandation")
                    donnee.Add(recommandation)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_recommandation")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(recommandation)
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RECOMMANDATION", colNameRec, donneeRec)
                End If
                'If co.SelectDistinctWhere("TX_SIGNATURE", "commentaire", "code", code).Count = 1 Then
                If TCommentaire.Rtf <> co.SelectDistinctWhere("TX_SIGNATURE", "commentaire", "code", code).FirstOrDefault Then
                    colName.Add("commentaire")
                    donnee.Add(TCommentaire.Rtf)
                    changement = True
                End If
                If TCommentaire2.Rtf <> co.SelectDistinctWhere("TX_SIGNATURE", "commentaire2", "code", code).FirstOrDefault Then
                    colName.Add("commentaire2")
                    donnee.Add(TCommentaire2.Rtf)
                    changement = True
                End If
                If TDescription.Rtf <> co.SelectDistinctWhere("TX_SIGNATURE", "description", "code", code).FirstOrDefault Then
                    colName.Add("description")
                    donnee.Add(TDescription.Rtf)
                    changement = True
                End If
                If TRecommandation.Text <> co.SelectDistinctWhere("TX_SIGNATURE", "recommandation", "code", code).FirstOrDefault Then
                    colName.Add("recommandation")
                    donnee.Add(TRecommandation.Text)
                    changement = True
                End If
                'changement note isr 
                If co.SelectDistinctWhere("TX_SIGNATURE", "note_isr", "code", code).Count = 0 Then
                    If TNoteISR.Text <> co.SelectDistinctWhere("TX_SIGNATURE", "note_isr", "code", code).FirstOrDefault Then
                        If IsNumeric(TNoteISR.Text) Then
                            colName.Add("note_isr")
                            donnee.Add(Convert.ToDouble(TNoteISR.Text))
                            changement = True
                        End If
                    End If
                Else
                    If TNoteISR.Text <> co.SelectDistinctWhere("TX_SIGNATURE", "note_isr", "code", code).First.ToString Then
                        If IsNumeric(TNoteISR.Text) Then
                            colName.Add("note_isr")
                            donnee.Add(Convert.ToDouble(TNoteISR.Text))
                            changement = True
                        End If
                    End If
                End If
                'End If
                If co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_secteur", "code", code).First Then
                    colName.Add("id_secteur")
                    donnee.Add(co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First())
                    changement = True
                End If
                If co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).Count > 0 Then
                    If co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_sous_secteur", "code", code).FirstOrDefault Then
                        colName.Add("id_sous_secteur")
                        donnee.Add(co.SelectDistinctWhere("SOUS_SECTEUR", "id", "libelle", CbSousSecteur.Text).First())
                        changement = True
                    End If
                End If
                If co.SelectDistinctWhere("PAYS", "id", "libelle", CbPays.Text).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_pays", "code", code).First Then
                    colName.Add("id_pays")
                    donnee.Add(co.SelectDistinctWhere("PAYS", "id", "libelle", CbPays.Text).First())
                    changement = True
                End If
                Dim nameRating As List(Of String) = New List(Of String)()
                nameRating.Add("agence")
                nameRating.Add("rating")
                nameRating.Add("type_rating")
                Dim donneeRating As List(Of Object) = New List(Of Object)()

                donneeRating.Add("Moodys")
                donneeRating.Add(CbMoodysCT.Text)
                donneeRating.Add("CT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Moo_CT", "code", code).First Then
                    colName.Add("id_rating_Moo_CT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"M" & CbMoodysCT.Text & "CT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("Moodys")
                donneeRating.Add(CbMoodysLT.Text)
                donneeRating.Add("LT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Moo_LT", "code", code).First Then
                    colName.Add("id_rating_Moo_LT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"M" & CbMoodysLT.Text & "LT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("StandardandPoors")
                donneeRating.Add(CbSandPCT.Text)
                donneeRating.Add("CT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Sp_CT", "code", code).First Then
                    colName.Add("id_rating_Sp_CT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"S" & CbSandPCT.Text & "CT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("StandardandPoors")
                donneeRating.Add(CbSandPLT.Text)
                donneeRating.Add("LT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Sp_LT", "code", code).First Then
                    colName.Add("id_rating_Sp_LT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"S" & CbSandPLT.Text & "LT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("Fitch")
                donneeRating.Add(CbFitchCT.Text)
                donneeRating.Add("CT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Fi_CT", "code", code).First Then
                    colName.Add("id_rating_Fi_CT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"F" & CbFitchCT.Text & "CT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("Fitch")
                donneeRating.Add(CbFitchLT.Text)
                donneeRating.Add("LT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_Fi_LT", "code", code).First Then
                    colName.Add("id_rating_Fi_LT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"F" & CbFitchLT.Text & "LT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("Interne")
                donneeRating.Add(CbInternCT.Text)
                donneeRating.Add("CT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_In_CT", "code", code).First Then
                    colName.Add("id_rating_In_CT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"I" & CbInternCT.Text & "CT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                donneeRating.Clear()
                donneeRating.Add("Interne")
                donneeRating.Add(CbInternLT.Text)
                donneeRating.Add("LT")
                If co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First <> co.SelectDistinctWhere("TX_SIGNATURE", "id_rating_In_LT", "code", code).First Then
                    colName.Add("id_rating_In_LT")
                    donnee.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First)
                    changement = True
                    Dim colNameRec As List(Of String) = New List(Of String)()
                    Dim donneeRec As List(Of Object) = New List(Of Object)()
                    colNameRec.Add("id_rating")
                    colNameRec.Add("code_signature")
                    colNameRec.Add("date")
                    donneeRec.Add(co.SelectDistinctWheres("TX_RATING", "id", nameRating, donneeRating).First) '"I" & CbInternLT.Text & "LT")
                    donneeRec.Add(code)
                    donneeRec.Add(datee)
                    co.Insert("TX_HISTO_RATING", colNameRec, donneeRec)
                End If

                If changement Then
                    co.Update("TX_SIGNATURE", colName, donnee, "code", code)
                End If

                'Modification dans TX_GROUPE
                If String.IsNullOrEmpty(TGroupe.Text) = False Then
                    If co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).Count = 1 Then
                        If TGroupe.Text <> co.SelectDistinctWhere("TX_GROUPE", "libelle", "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).First).First Then
                            colName.Clear()
                            donnee.Clear()
                            colName.Add("libelle")
                            donnee.Add(TGroupe.Text)
                            co.Update("TX_GROUPE", colName, donnee, "id", co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).First)
                        End If
                    End If
                    'Création d'un groupe par l'utilisateur puis le brancher sur la signature
                    If co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).Count = 0 Then
                        colName.Clear()
                        donnee.Clear()
                        colName.Add("id")
                        colName.Add("libelle")
                        colName.Add("id_utilisateur")
                        Dim rnd As System.Random = New System.Random
                        Dim clé As String = "FGA-" & code & "-" & rnd.Next(0, 1000)
                        donnee.Add(clé)
                        donnee.Add(TGroupe.Text)
                        donnee.Add(Utilisateur.login)
                        co.Insert("TX_GROUPE", colName, donnee)
                        co.Update("TX_SIGNATURE", New List(Of String)(New String() {"id_groupe"}), New List(Of Object)(New Object() {clé}), "code", code)
                    End If
                End If


                'Me.Close()
                'BaseEmetteurs.Show()
            Else
                MessageBox.Show("Il manque des informations avant d'archiver la signature dans TX_SIGNATURE", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' CbSecteur : proposer les bons sous secteurs
        ''' </summary>
        Private Sub CbSecteur_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSecteur.SelectedValueChanged
            If String.IsNullOrEmpty(CbSecteur.Text) = False Then
                Dim id_secteur As Object = co.SelectDistinctWhere("SECTEUR", "id", "libelle", CbSecteur.Text).First
                ' filtrer les lignes avec un code %_OLD , car on ne fait pas choisir un ancien code de sous secteur
                Dim clauseWhereParam As List(Of String) = New List(Of String)
                Dim clauseWhereValue As List(Of Object) = New List(Of Object)
                clauseWhereParam.Add("id_secteur")
                clauseWhereValue.Add(id_secteur)
                clauseWhereParam.Add("id")
                clauseWhereValue.Add("NOT LIKE '%_OLD'")
                'Populer la combo box avec les libellés des sous secteur valide
                CbSousSecteur.DataSource = co.SelectDistinctWheres("SOUS_SECTEUR", "libelle", clauseWhereParam, clauseWhereValue)

            End If
        End Sub

        ''' <summary>
        ''' BSuprimer : suprimme signature base + disque
        ''' </summary>
        Private Sub BSupprimer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSupprimer.Click
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            Dim a As Integer = MessageBox.Show("Etes vous sures de supprimer la signature " & TLibelle.Text & " et tous les fichiers presents sur le disque " & co.SelectDistinctSimple("TX_RACINE", "chemin").First.ToString & "\" & code.ToString & " ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            If a = 1 Then
                'Supression de la BDD
                co.DeleteWhere("TX_FICHIER", "code_signature", code)
                co.DeleteWhere("TX_HISTO_RECOMMANDATION", "code_signature", code)
                co.DeleteWhere("TX_HISTO_RATING", "code_signature", code)
                Dim id_groupe As String = co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "code", code).FirstOrDefault
                co.DeleteWhere("TX_SIGNATURE", "code", code)
                If String.IsNullOrEmpty(id_groupe) = False Then
                    If co.SelectDistinctWhere("TX_SIGNATURE", "id_groupe", "id_groupe", id_groupe).Count = 0 Then
                        co.DeleteWhere("TX_GROUPE", "id", id_groupe)
                    End If
                End If
                'Ne rien faire pour TX_GROUPE car il peut avoir des signature qui l'utilise


                'Supression des fichiers sur le disque
                dossier.SupprimerDossier(co.SelectDistinctSimple("TX_RACINE", "chemin").First & "\" & TLibelle.Text)

                Me.Close()
                Dim be As New BaseEmetteurs
                be.Show()
                log.Log(ELog.Information, "BSupprimer_Click", "la signature " & TLibelle.Text & " a été suprimmer de TX_SIGNATURE, TX_HISTO_RECOMMANDATION, TX_HISTO_RATING, TX_COMMENTAIRE,TX_SIGNATURE !")
            End If
        End Sub

        ''' <summary>
        ''' Resize de l'ihm
        ''' </summary>
        Private Sub ModificationSignature_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
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
        ''' Ouvre ihm BaseEmetteur
        ''' </summary>
        Private Sub ModificationSignature2_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Dim be As New BaseEmetteurs
            be.Show()
        End Sub


        'Private Sub BPolice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '    If Not TCommentaire.SelectionFont Is Nothing Then
        '        FontDialog.Font = TCommentaire.SelectionFont
        '    Else
        '        FontDialog.Font = Nothing
        '    End If

        '    FontDialog.ShowApply = True

        '    If FontDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
        '        TCommentaire.SelectionFont = FontDialog.Font
        '        TCommentaire.SelectionColor = FontDialog.Color
        '    End If

        '    'Dim fnt As New FontDialog
        '    'fnt.ShowColor = True
        '    'If fnt.ShowDialog() <> Windows.Forms.DialogResult.OK Then
        '    ' Exit Sub
        '    'End If
        '    'RichTextBox1.SelectionFont = FontDialog1.Font
        'End Sub

        ''' <summary>
        ''' CbSousSecteur : Récupere derniere recommandation secteur
        ''' </summary>
        Private Sub CbSousSecteur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbSousSecteur.SelectedIndexChanged
            Dim colName As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            colName.Add("@libelle")
            donnee.Add(CbSousSecteur.Text)
            DataGridSousSecteur.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastSousSecteurRecommandation", colName, donnee))
        End Sub

        ''' <summary>
        ''' CbPays : Récupere derniere recommandation pays
        ''' </summary>
        Private Sub CbPays_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPays.SelectedIndexChanged
            Dim colName As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            colName.Add("@libelle")
            donnee.Add(CbPays.Text)
            DataGridPays.DataSource = co.LoadDataGridByProcedureStockée(co.ProcedureStockéeForDataGrid("GetLastPaysRecommandation", colName, donnee))
        End Sub

        ''' <summary>
        ''' Ouvre fichier 
        ''' </summary>
        Private Sub DataGridFile_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridFile.DoubleClick
            Dim fichier = DataGridFile.CurrentRow.Cells.Item(1).Value.ToString
            Dim cheminFichier As String = co.SelectSimple("TX_RACINE", "chemin")(0) & "\" & TLibelle.Text & "\" & fichier
            If (fi.Existe(cheminFichier)) Then
                Process.Start(cheminFichier)
            Else
                MessageBox.Show("Le fichier " & cheminFichier & " n'arrive pas a être ouvert !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' Supprime fichier disque + base
        ''' </summary>
        Private Sub DataGridFile_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DataGridFile.KeyDown
            'Si appuie sur supprimer du clavier
            If e.KeyCode = 46 Then
                Dim libelle As String = TLibelle.Text
                Dim fichier As String = DataGridFile.CurrentRow.Cells.Item(1).Value.ToString
                Dim a As Integer = MessageBox.Show("Etes vous sures de supprimer le fichier " & fichier & " de la signature " & libelle & " ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 1 Then
                    Dim cheminFichier As String = co.SelectSimple("TX_RACINE", "chemin")(0) & "\" & libelle & "\" & fichier
                    If (fi.Existe(cheminFichier)) Then
                        Try
                            'Supprimer de window
                            Kill(cheminFichier)
                            Dim donnee As List(Of Object) = New List(Of Object)()
                            donnee.Add(co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", libelle).First)
                            donnee.Add(fichier)
                            Dim colName As List(Of String) = New List(Of String)()
                            colName.Add("code_signature")
                            colName.Add("nom")
                            co.DeleteWheres("TX_FICHIER", colName, donnee)
                            DataGridFile.Rows.RemoveAt(DataGridFile.CurrentRow.Index)
                        Catch ex As Exception
                            MessageBox.Show("Le fichier " & cheminFichier & " est ouvert il ne peut pas etre supprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End Try
                    Else
                        MessageBox.Show("Le fichier " & cheminFichier & " n'existe pas dans Window !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Update la base fichier
        ''' </summary>
        Private Sub DataGridFile_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridFile.CellValueChanged
            Dim libelle As String = TLibelle.Text
            Dim nom As String = DataGridFile.CurrentRow.Cells.Item(1).Value.ToString
            Dim note As String = DataGridFile.CurrentRow.Cells.Item(3).Value.ToString
            Dim colName1 As List(Of String) = New List(Of String)()
            Dim donnee1 As List(Of Object) = New List(Of Object)()
            'Changement de la note
            If e.ColumnIndex = 3 Then
                colName1.Add("note")
                donnee1.Add(note)
            End If
            'Changement du nom de fichier
            If e.ColumnIndex = 1 Then
                colName1.Add("nom")
                donnee1.Add(nom)
                Try
                    My.Computer.FileSystem.RenameFile(co.SelectDistinctSimple("TX_RACINE", "chemin")(0) & "\" & TLibelle.Text & "\" & oldName, nom)
                Catch ex As Exception
                    If DataGridFile.CurrentRow.Cells.Item(1).Value <> oldName Then
                        MessageBox.Show("Le nom fichier " & nom & " n'est pas correct ou le fichier est ouvert ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        DataGridFile.CurrentRow.Cells.Item(1).Value = oldName
                    End If
                    Exit Sub
                End Try
            End If
            Dim colName2 As List(Of String) = New List(Of String)()
            Dim donnee2 As List(Of Object) = New List(Of Object)()
            colName2.Add("code_signature")
            colName2.Add("nom")
            donnee2.Add(co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", libelle).First)
            If e.ColumnIndex <> 1 Then
                donnee2.Add(nom)
            Else
                donnee2.Add(oldName)
            End If
            co.Updates("TX_FICHIER", colName1, donnee1, colName2, donnee2)
        End Sub

        Private Sub DataGridFile_CellBeginEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles DataGridFile.CellBeginEdit
            If e.ColumnIndex = 1 Then
                oldName = DataGridFile.CurrentRow.Cells.Item(1).Value.ToString
            End If
        End Sub

        ''' <summary>
        ''' Filtre TNotre DataGridFichier
        ''' </summary>
        Private Sub TNote_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TNote.TextChanged
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            If changeDate = False Then
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' ORDER BY date desc")
            Else
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' AND CONVERT(CHAR(10), fi.date, 103)='" & DatePicker.Value.Date & "' ORDER BY date desc")
            End If
        End Sub

        ''' <summary>
        ''' Filtre TNom DataGridFichier
        ''' </summary>
        Private Sub TNom_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TNomFichier.TextChanged
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            If changeDate = False Then
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' ORDER BY date desc")
            Else
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' AND CONVERT(CHAR(10), fi.date, 103)='" & DatePicker.Value.Date & "' ORDER BY date desc")
            End If
        End Sub

        ''' <summary>
        ''' Filtre TEmetteur DataGridFichier
        ''' </summary>
        Private Sub TEmetteur_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TEmetteurFichier.TextChanged
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            If changeDate = False Then
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' ORDER BY date desc")
            Else
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' AND CONVERT(CHAR(10), fi.date, 103)='" & DatePicker.Value.Date & "' ORDER BY date desc")
            End If
        End Sub

        ''' <summary>
        ''' Filtre TLogin DataGridFichier
        ''' </summary>
        Private Sub TLogin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TLogin.TextChanged
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            If changeDate = False Then
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' ORDER BY date desc")
            Else
                DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' AND CONVERT(CHAR(10), fi.date, 103)='" & DatePicker.Value.Date & "' ORDER BY date desc")
            End If
        End Sub

        ''' <summary>
        ''' Filtre DatePicker dataGridFichier
        ''' </summary>
        Private Sub DatePicker_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DatePicker.ValueChanged
            changeDate = True
            Dim code As String = co.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", TLibelle.Text).First
            DataGridFile.DataSource = co.LoadDataGridByString("SELECT fi.date As 'Date', fi.nom As 'Nom fichier',  ef.libelle As 'Emetteur fichier', fi.note As 'Note', fi.id_utilisateur As 'Utilisateur' FROM TX_FICHIER fi, TX_EMETTEUR_FICHIER ef where fi.code_signature='" & code & "' AND fi.id_emetteur_fichier=ef.id AND note like '%" & TNote.Text & "%' AND nom like '%" & TNomFichier.Text & "%'  AND ef.libelle like '%" & TEmetteurFichier.Text & "%' AND fi.id_utilisateur like '%" & TLogin.Text & "%' AND CONVERT(CHAR(10), fi.date, 103)='" & DatePicker.Value.Date & "' ORDER BY date desc")
        End Sub

        'Private Sub BDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '    Dim a As Integer = MessageBox.Show("Etes-vous sûr de vouloir effacer le contenu de comentaire ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        '    If a = 1 Then
        '        TCommentaire.Clear()
        '    End If
        'End Sub

        Private Sub PrintDocument1_BeginPrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PrintDocument.BeginPrint
            checkPrint = 0
        End Sub

        'Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument.PrintPage
        '    ' Print the content of the RichTextBox. Store the last character printed.
        '    checkPrint = TCommentaire.Print(checkPrint, TCommentaire.TextLength, e)

        '    ' Look for more pages
        '    If checkPrint < TCommentaire.TextLength Then
        '        e.HasMorePages = True
        '    Else
        '        e.HasMorePages = False
        '    End If
        'End Sub

        ''' <summary>
        ''' BImpression : imprime TCommentaire
        ''' </summary>
        Private Sub BImpression_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If PrintDialog.ShowDialog() = DialogResult.OK Then
                PrintDocument.Print()
            End If
        End Sub


        Private Sub TCommentaire_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
            '    If e.Control AndAlso e.KeyCode = Keys.V Then 'si la touche controle est appuyée ainsi que la touche V
            '        Dim d As New DataObject
            '        Dim desiredFormat As String = DataFormats.Rtf
            '        d = My.Computer.Clipboard.GetDataObject
            '        If d.GetDataPresent(desiredFormat) Then
            '            Dim data() As Byte = Nothing
            '            data = TryCast(d.GetData(desiredFormat), Byte())
            '        End If
            '        'Dim data As New DataObject
            '        ' Add the data in various formats.
            '        'data.SetData(DataFormats.Text, rchSource.Text)
            '        'My.Computer.Clipboard.SetDataObject(data)
            '    End If
            'TCommentaire.SelectAll()
            'TCommentaire. = TCommentaire.Font.size()
        End Sub


        Private Sub BDelete2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim a As Integer = MessageBox.Show("Etes-vous sûr de vouloir effacer le contenu de comentaire2 ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            If a = 1 Then
                TCommentaire2.Text = ""
            End If
        End Sub

        'Private Sub BPolice2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If Not TCommentaire2.SelectionFont Is Nothing Then
        '    FontDialog.Font = TCommentaire2.SelectionFont
        'Else
        '    FontDialog.Font = Nothing
        'End If

        'FontDialog.ShowApply = True

        'If FontDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
        '    TCommentaire2.SelectionFont = FontDialog.Font
        '    TCommentaire2.SelectionColor = FontDialog.Color
        'End If
        'End Sub

        Private Sub TCommentaire_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            If e.Button = MouseButtons.Right Then
                Return
            End If
        End Sub
    End Class
End Namespace
