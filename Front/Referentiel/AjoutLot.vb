Public Class AjoutLot

    Dim co As Connection = New Connection()

    ''' <summary>
    ''' Load de l'ihm
    ''' </summary>
    Private Sub AjoutLot_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Autoriser ou non les composants
        RbGroupe.Checked = True
        CheckedListCompte.Enabled = False

        'Tentative de connection a la BDD
        co.ToConnectBase()

        'Binder les composants
        For i = 0 To co.SelectDistinctSimple("PTF_FGA", "compte").Count - 1 Step 1
            CheckedListCompte.Items.Add(co.SelectDistinctSimple("PTF_FGA", "compte")(i))
        Next
        For i = 0 To co.SelectDistinctSimple("PTF_FGA", "groupe").Count - 1 Step 1
            CheckedListGroupe.Items.Add(co.SelectDistinctSimple("PTF_FGA", "groupe")(i))
        Next
    End Sub


    ''' <summary>
    ''' Bouton Groupe
    ''' </summary>
    Private Sub RbGroupe_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbGroupe.CheckedChanged
        If (RbGroupe.Checked) Then
            CheckedListCompte.Enabled = False
            CheckedListGroupe.Enabled = True
        Else
            CheckedListCompte.Enabled = True
            CheckedListGroupe.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Bouton Compte 
    ''' </summary>
    Private Sub RbCompte_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbCompte.CheckedChanged
        If (RbCompte.Checked) Then
            CheckedListCompte.Enabled = True
            CheckedListGroupe.Enabled = False
        Else
            CheckedListCompte.Enabled = False
            CheckedListGroupe.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' BAjouter : modifie ou ajoute un lot en base 
    ''' </summary>
    Private Sub BAjouter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjouter.Click
        If (String.IsNullOrEmpty(TIdLot.Text) = False And String.IsNullOrEmpty(CbLibelleLot.Text) = False And (CheckedListCompte.Items.Count > 0 Or CheckedListCompte.Items.Count > 0)) Then

            Dim colLot As List(Of String) = New List(Of String)
            Dim donneeLot As List(Of Object) = New List(Of Object)
            Dim colLot2 As List(Of String) = New List(Of String)
            Dim donneeLot2 As List(Of Object) = New List(Of Object)
            Dim type_lot, tabName, col2 As String

            If co.SelectDistinctWhere("PTF_LOT", "id_lot", "id_Lot", TIdLot.Text).Count > 0 Then
                If co.SelectDistinctWhere("PTF_LOT", "type_lot", "id_Lot", TIdLot.Text)(0) = "Ptf" Then
                    co.DeleteWhere("PTF_LOT_COMPTE", "id_lot", TIdLot.Text)
                    co.DeleteWhere("PTF_LOT", "id_lot", TIdLot.Text)
                Else
                    co.DeleteWhere("PTF_LOT_GROUPE", "id_lot", TIdLot.Text)
                    co.DeleteWhere("PTF_LOT", "id_lot", TIdLot.Text)
                End If
            End If

            Dim checkedList As New CheckedListBox
            If RbCompte.Checked Then
                type_lot = "Ptf"
                tabName = "PTF_LOT_COMPTE"
                col2 = "compte"
                checkedList = CheckedListCompte
            Else
                type_lot = "Grp"
                tabName = "PTF_LOT_GROUPE"
                col2 = "groupe"
                checkedList = CheckedListGroupe
            End If

            For Each elem As Object In checkedList.CheckedItems
                colLot2.Add("id_lot")
                donneeLot2.Add(TIdLot.Text)
                colLot2.Add(col2)
                donneeLot2.Add(elem)

                If (co.SelectDistinctWhere("PTF_LOT", "id_lot", "id_Lot", TIdLot.Text).Count = 0) Then
                    'Rien existe dans la base => INSERT dans PTF_LOT et PTF_LOT_...
                    colLot.Add("id_lot")
                    donneeLot.Add(TIdLot.Text)
                    colLot.Add("libelle_lot")
                    donneeLot.Add(CbLibelleLot.Text)
                    colLot.Add("Type_lot")
                    donneeLot.Add(type_lot)
                    co.Insert("PTF_LOT", colLot, donneeLot)
                    co.Insert(tabName, colLot2, donneeLot2)
                Else
                    Dim selectCol As List(Of String) = New List(Of String)
                    selectCol.Add("id_lot")
                    selectCol.Add(col2)
                    If co.Select2Wheres(tabName, selectCol, colLot2, donneeLot2).Count = 0 Then
                        'Juste ajout dans la seconde table
                        co.Insert(tabName, colLot2, donneeLot2)
                    Else
                        'Existe déja dans les deux tables
                        MessageBox.Show("La donnée " & tabName & "(" & TIdLot.Text & "," & col2 & ") existe déjà dans la base " & tabName, "Echec de l'ajout du lot " & TIdLot.Text, MessageBoxButtons.OK, MessageBoxIcon.None)
                    End If
                End If

                colLot.Clear()
                donneeLot.Clear()
                colLot2.Clear()
                donneeLot2.Clear()
            Next

            Else
                MessageBox.Show("Il manque des informations", "Echec de l'ajout du lot " & CbLibelleLot.Text, MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
    End Sub

    ''' <summary>
    ''' BbLibelleLot SelectedIndexChanged
    ''' </summary>
    Private Sub CbLibelleLot_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbLibelleLot.SelectedIndexChanged
        TIdLot.Text = co.SelectDistinctWhere("PTF_LOT", "id_lot", "libelle_lot", CbLibelleLot.Text).First

        If co.SelectDistinctWhere("PTF_LOT", "type_lot", "Libelle_lot", CbLibelleLot.Text).First = "Grp" Then
            'c'est un lot groupe
            RbCompte.Checked = False
            RbGroupe.Checked = True
            CheckedListCompte.Enabled = False
            CheckedListGroupe.Enabled = True

            Dim j As Integer = 0
            For i = 0 To co.SelectDistinctSimple("PTF_FGA", "groupe").Count - 1 Step 1
                If (co.SelectDistinctSimple("PTF_FGA", "groupe")(i) = co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", TIdLot.Text)(j)) Then
                    CheckedListGroupe.SetItemChecked(i, True)
                    If co.SelectDistinctWhere("PTF_LOT_GROUPE", "groupe", "id_lot", TIdLot.Text).Count - 1 <> j Then
                        j = j + 1
                    End If
                Else
                    CheckedListGroupe.SetItemChecked(i, False)
                End If
            Next
        Else
            'c'est un lot compte
            RbCompte.Checked = True
            RbGroupe.Checked = False
            CheckedListCompte.Enabled = True
            CheckedListGroupe.Enabled = False
            Dim j As Integer = 0
            For i = 0 To co.SelectDistinctSimple("PTF_FGA", "compte").Count - 1 Step 1
                If (co.SelectDistinctSimple("PTF_FGA", "compte")(i) = co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", TIdLot.Text)(j)) Then
                    CheckedListCompte.SetItemChecked(i, True)
                    If co.SelectDistinctWhere("PTF_LOT_COMPTE", "compte", "id_lot", TIdLot.Text).Count - 1 <> j Then
                        j = j + 1
                    End If
                Else
                    CheckedListCompte.SetItemChecked(i, False)
                End If
            Next
        End If


    End Sub

    ''' <summary>
    ''' BSupprimer : supprime un lot de la base
    ''' </summary>
    Private Sub BSupprimer_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSupprimer.Click
        If co.SelectDistinctWhere("PTF_LOT", "type_lot", "id_lot", TIdLot.Text).Count > 0 Then
            Select Case co.SelectDistinctWhere("PTF_LOT", "type_lot", "id_lot", TIdLot.Text)(0)
                Case "Grp"
                    co.DeleteWhere("PTF_LOT_GROUPE", "id_lot", TIdLot.Text)
                    co.DeleteWhere("PTF_LOT", "id_lot", TIdLot.Text)
                    Me.Close()
                Case "Ptf"
                    co.DeleteWhere("PTF_LOT_COMPTE", "id_lot", TIdLot.Text)
                    co.DeleteWhere("PTF_LOT", "id_lot", TIdLot.Text)
                    Me.Close()
            End Select
        Else
            MessageBox.Show("Le lot " & CbLibelleLot.Text & " n'existe pas dans la table PTF_LOT !", "Echec de la supression du lot " & TIdLot.Text, MessageBoxButtons.OK, MessageBoxIcon.None)
        End If
    End Sub

    ''' <summary>
    ''' BbLibelleLot Click
    ''' </summary>
    Private Sub CbLibelleLot_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbLibelleLot.Click
        CbLibelleLot.Items.Clear()
        For i = 0 To co.SelectSimple("PTF_LOT", "libelle_lot").Count - 1 Step 1
            CbLibelleLot.Items.Add(co.SelectSimple("PTF_LOT", "libelle_lot")(i))
        Next
    End Sub

    
End Class