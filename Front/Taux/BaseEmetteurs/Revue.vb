Imports WindowsApplication1.Action
Namespace Taux.BaseEmetteurs
    Public Class Revue


        Dim log As Log = New Log()
        Dim co As Connection = New Connection()
        Dim id_secteur As String

        Private Sub Revue_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Connection a la base
            co.ToConnectBase()
            Rafraichir()
        End Sub

        Public Sub maj_secteur(ByVal secteur As String)
            cbSecteur.Text = secteur
        End Sub

        ''' <summary>
        ''' Rafraichir : binde les composants graphiques
        ''' </summary>
        Private Sub Rafraichir()
            'Binder les combobox 
            cbSecteur.DataSource = co.SelectDistinctSimple("ACT_FGA_SECTOR", "libelle", "ASC")

            CbexU.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "exU").FirstOrDefault = "1")) Then
                LExU.BackColor = Color.PaleTurquoise
            Else
                LExU.BackColor = Color.White
            End If

            CbExEm.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "exEm").FirstOrDefault = "1")) Then
                LexEm.BackColor = Color.PaleTurquoise
            Else
                LexEm.BackColor = Color.White
            End If

            Cbbeuro.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "beuro").FirstOrDefault = "1")) Then
                Lbeuro.BackColor = Color.PaleTurquoise
            Else
                Lbeuro.BackColor = Color.White
            End If

            Cbhcr.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "hcr").FirstOrDefault = "1")) Then
                Lhcr.BackColor = Color.PaleTurquoise
            Else
                Lhcr.BackColor = Color.White
            End If

            Cbsdd.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "sdd").FirstOrDefault = "1")) Then
                Lsdd.BackColor = Color.PaleTurquoise
            Else
                Lsdd.BackColor = Color.White
            End If

            Cbaf2.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "af2").FirstOrDefault = "1")) Then
                Laf2.BackColor = Color.PaleTurquoise
            Else
                Laf2.BackColor = Color.White
            End If

            Cbshe.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "she").FirstOrDefault = "1")) Then
                Lshe.BackColor = Color.PaleTurquoise
            Else
                Lshe.BackColor = Color.White
            End If

            Cbsam.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "sam").FirstOrDefault = "1")) Then
                Lsam.BackColor = Color.PaleTurquoise
            Else
                Lsam.BackColor = Color.White
            End If

            Cbheuro.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "heuro").FirstOrDefault = "1")) Then
                Lheuro.BackColor = Color.PaleTurquoise
            Else
                Lheuro.BackColor = Color.White
            End If

            Cbshmp.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "shmp").FirstOrDefault = "1")) Then
                Lshmp.BackColor = Color.PaleTurquoise
            Else
                Lshmp.BackColor = Color.White
            End If

            CbexEu.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "exEu").FirstOrDefault = "1")) Then
                LexEu.BackColor = Color.PaleTurquoise
            Else
                LexEu.BackColor = Color.White
            End If

            Cbshare.DataSource = co.SelectDistinctSimple("ACT_NOTE", "id", "DESC")
            If ((co.SelectDistinctWhere("ACT_THEME", "valide", "id", "share").FirstOrDefault = "1")) Then
                Lshare.BackColor = Color.PaleTurquoise
            Else
                Lshare.BackColor = Color.White
            End If
        End Sub

        Private Sub cbSecteur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbSecteur.SelectedIndexChanged, cbSecteur.SelectedIndexChanged

            'Binder toutes les infos déjà presente en base
            id_secteur = co.SelectDistinctWhere("ACT_FGA_SECTOR", "id", "libelle", cbSecteur.Text).FirstOrDefault
            If id_secteur = Nothing Then id_secteur = "1"

            'ACT_FGA_SECTOR_RECOMMANDATION
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur).Count <> 0 Then
                Select Case co.SelectWhere2("ACT_FGA_SECTOR_RECOMMANDATION", "id_recommandation", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault
                    Case "+"
                        RbPlus.Checked = True
                        RbNeutre.Checked = False
                        RbMoins.Checked = False
                    Case "="
                        RbPlus.Checked = False
                        RbNeutre.Checked = True
                        RbMoins.Checked = False
                    Case "-"
                        RbPlus.Checked = False
                        RbNeutre.Checked = False
                        RbMoins.Checked = True
                End Select
            Else
                RbNeutre.Checked = True
            End If

            'ACT_FGA_SECTOR_COMMENTAIRE
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_COMMENTAIRE", "MAX(date)", "id_secteur", id_secteur).Count <> 0 Then
                TCommentaire.Rtf = co.SelectWhere2("ACT_FGA_SECTOR_COMMENTAIRE", "commentaire", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_COMMENTAIRE", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault
            Else
                TCommentaire.Rtf = ""
            End If

            'ACT_FGA_SECTOR_NEWS
            'If co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).Count <> 0 Then
            'TNewsTitre.Text = co.SelectWhere2("ACT_FGA_SECTOR_NEWS", "titre", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault
            'TNewsLibelle.Text = co.SelectWhere2("ACT_FGA_SECTOR_NEWS", "libelle", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault
            'Else
            'TNewsTitre.Text = ""
            'TNewsLibelle.Text = ""
            'End If

            'ACT_FGA_SECTOR_NOTE
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exU", 1).Count <> 0 Then
                CbexU.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "exU", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exU", 1).FirstOrDefault})).FirstOrDefault
            Else
                CbexU.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exEm", 1).Count <> 0 Then
                CbExEm.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "exEm", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exEm", 1).FirstOrDefault})).FirstOrDefault
            Else
                CbExEm.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "beuro", 1).Count <> 0 Then
                Cbbeuro.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "beuro", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "beuro", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbbeuro.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "hcr", 1).Count <> 0 Then
                Cbhcr.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "hcr", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "hcr", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbhcr.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "sdd", 1).Count <> 0 Then
                Cbsdd.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "sdd", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "sdd", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbsdd.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "af2", 1).Count <> 0 Then
                Cbaf2.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "af2", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "af2", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbaf2.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "she", 1).Count <> 0 Then
                Cbshe.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "she", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "she", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbshe.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "sam", 1).Count <> 0 Then
                Cbsam.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "sam", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "sam", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbsam.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "heuro", 1).Count <> 0 Then
                Cbheuro.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "heuro", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "heuro", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbheuro.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "shmp", 1).Count <> 0 Then
                Cbshmp.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "shmp", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "shmp", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbshmp.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exEu", 1).Count <> 0 Then
                CbexEu.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "exEu", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "exEu", 1).FirstOrDefault})).FirstOrDefault
            Else
                CbexEu.Text = "="
            End If
            If co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "share", 1).Count <> 0 Then
                Cbshare.Text = co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "date"}), New List(Of Object)(New Object() {id_secteur, "share", co.SelectWhere2("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur, "id_theme", "share", 1).FirstOrDefault})).FirstOrDefault
            Else
                Cbshare.Text = "="
            End If

        End Sub

        Private Sub Revue_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
            Dim ba As New BaseAction
            Dim mp As New ModelPortfolio
            ba.Show()
            ba.TOngletEurope.SelectedTab = ba.TValeurAnalyse
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_NOTE", "id_secteur", "id_secteur", id_secteur).Count <> 0 Then
                'on peut selectionner le bon secteur de DSecteurs
                For i = 0 To mp.DSecteursIcb.Rows.Count - 1
                    If mp.DSecteursIcb.Rows(i).Cells(1).Value = cbSecteur.Text Then
                        mp.DSecteursIcb.CurrentCell = mp.DSecteursIcb.Rows(i).Cells(1)
                        mp.newClickFga()
                    End If
                Next
            End If
        End Sub

        Private Sub BSauvegarder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSauvegarder.Click

            'ACT_FGA_SECTOR_RECOMMANDATION
            Dim id_recommandation As String = ""
            If RbPlus.Checked = True Then
                id_recommandation = "+"
            ElseIf RbNeutre.Checked = True Then
                id_recommandation = "="
            ElseIf RbMoins.Checked = True Then
                id_recommandation = "-"
            End If
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur).Count = 0 Then
                co.Insert("ACT_FGA_SECTOR_RECOMMANDATION", New List(Of String)(New String() {"date", "id_secteur", "id_recommandation"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, id_recommandation}))
            ElseIf id_recommandation <> co.SelectWhere2("ACT_FGA_SECTOR_RECOMMANDATION", "id_recommandation", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_RECOMMANDATION", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_RECOMMANDATION", New List(Of String)(New String() {"date", "id_secteur", "id_recommandation"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, id_recommandation}))
            End If

            'ACT_FGA_SECTOR_COMMENTAIRE
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_COMMENTAIRE", "MAX(date)", "id_secteur", id_secteur).Count = 0 And TCommentaire.Text <> "" Then
                co.Insert("ACT_FGA_SECTOR_COMMENTAIRE", New List(Of String)(New String() {"date", "id_secteur", "commentaire"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, TCommentaire.Rtf}))
            ElseIf co.SelectDistinctWhere("ACT_FGA_SECTOR_COMMENTAIRE", "MAX(date)", "id_secteur", id_secteur).Count <> 0 Then
                If TCommentaire.Rtf <> co.SelectWhere2("ACT_FGA_SECTOR_COMMENTAIRE", "commentaire", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_COMMENTAIRE", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault Then
                    co.Insert("ACT_FGA_SECTOR_COMMENTAIRE", New List(Of String)(New String() {"date", "id_secteur", "commentaire"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, TCommentaire.Rtf}))
                End If
            End If

            'ACT_SUPERSECTOR_NEWS
            'If co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).Count = 0 And TNewsTitre.Text <> "" And TNewsLibelle.Text <> "" Then
            'co.Insert("ACT_FGA_SECTOR_NEWS", New List(Of String)(New String() {"date", "id_secteur", "titre", "libelle"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, TNewsTitre.Text, TNewsLibelle.Text}))
            'ElseIf co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).Count <> 0 Then
            'If TNewsTitre.Text <> co.SelectWhere2("ACT_FGA_SECTOR_NEWS", "titre", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault Or TNewsLibelle.Text <> co.SelectWhere2("ACT_FGA_SECTOR_NEWS", "libelle", "id_secteur", id_secteur, "date", co.SelectDistinctWhere("ACT_FGA_SECTOR_NEWS", "MAX(date)", "id_secteur", id_secteur).FirstOrDefault, 1).FirstOrDefault Then
            'co.Insert("ACT_FGA_SECTOR_NEWS", New List(Of String)(New String() {"date", "titre", "libelle", "id_secteur"}), New List(Of Object)(New Object() {DateTime.Now, TNewsTitre.Text, TNewsLibelle.Text, id_secteur}))
            'End If
            'End If

            'ACT_FGA_SECTOR_NOTE
            If co.SelectDistinctWhere("ACT_FGA_SECTOR_NOTE", "MAX(date)", "id_secteur", id_secteur).Count = 0 Then
                'nouvelle ligne dans tout le tableau
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exU", CbexU.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exEm", CbExEm.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "beuro", Cbbeuro.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "hcr", Cbhcr.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "sdd", Cbsdd.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "af2", Cbaf2.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "she", Cbshe.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "sam", Cbsam.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "heuro", Cbheuro.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "shmp", Cbshmp.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exEu", CbexEu.Text}))
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "share", Cbshare.Text}))
            End If
            If CbexU.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "exU", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "exU"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exU", CbexU.Text}))
            End If
            If CbExEm.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "exEm", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "exEm"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exEm", CbExEm.Text}))
            End If
            If Cbbeuro.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "beuro", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "beuro"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "beuro", Cbbeuro.Text}))
            End If
            If Cbhcr.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "hcr", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "hcr"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "hcr", Cbhcr.Text}))
            End If
            If Cbsdd.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "sdd", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "sdd"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "sdd", Cbsdd.Text}))
            End If
            If Cbaf2.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "af2", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "af2"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "af2", Cbaf2.Text}))
            End If
            If Cbshe.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "she", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "she"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "she", Cbshe.Text}))
            End If
            If Cbsam.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "sam", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "sam"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "sam", Cbsam.Text}))
            End If
            If Cbheuro.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "heuro", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "heuro"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "heuro", Cbheuro.Text}))
            End If
            If Cbshmp.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "shmp", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "shmp"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "shmp", Cbshmp.Text}))
            End If
            If CbexEu.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "exEu", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "exEu"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "exEu", CbexEu.Text}))
            End If
            If Cbshare.Text <> co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "id_note", New List(Of String)(New String() {"id_secteur", "id_theme", "Date"}), New List(Of Object)(New Object() {id_secteur, "share", co.SelectDistinctWheres("ACT_FGA_SECTOR_NOTE", "MAX(date)", New List(Of String)(New String() {"id_secteur", "id_theme"}), New List(Of Object)(New Object() {id_secteur, "share"})).FirstOrDefault})).FirstOrDefault Then
                co.Insert("ACT_FGA_SECTOR_NOTE", New List(Of String)(New String() {"date", "id_secteur", "id_theme", "id_note"}), New List(Of Object)(New Object() {DateTime.Now, id_secteur, "share", Cbshare.Text}))
            End If
        End Sub
    End Class
End Namespace

