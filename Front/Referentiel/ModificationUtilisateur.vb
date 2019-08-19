Public Class ModificationUtilisateur

    Dim co As Connection = New Connection()
    Dim log As Log = New Log()

    ''' <summary>
    ''' Initialisation de l'ihm
    ''' </summary>
    Private Sub Modification_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Tentative de connection a la BDD
        co.ToConnectBase()

        'Binder la liste des enum des metiers sur la listBox
        Dim i As Integer = 0
        For i = 0 To co.SelectSimple("TYPE_USER", "Libelle").Count - 1 Step 1
            CbType.Items.Add(co.SelectSimple("TYPE_USER", "TypeUser_Id")(i) & " - " & co.SelectSimple("TYPE_USER", "Libelle")(i))
        Next

        'Binder les utilisateurs
        Dim log As String = String.Empty
        Dim prenom As String = String.Empty
        Dim nom As String = String.Empty
        For i = 0 To co.SelectDistinctSimple("UTILISATEUR", "id").Count - 1 Step 1
            log = co.SelectDistinctSimple("UTILISATEUR", "id")(i)
            nom = co.SelectDistinctWhere("UTILISATEUR", "Nom", "id", log).FirstOrDefault
            prenom = co.SelectDistinctWhere("UTILISATEUR", "Prenom", "id", log).FirstOrDefault
            If log <> "OMEGA" Then
                CbLogin.Items.Add(log & " - " & nom & " " & prenom)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Bouton cancel de l'ihm
    ''' </summary>
    Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCancel.Click
        Me.Dispose()
    End Sub

    ''' <summary>
    ''' Bouton Update de l'ihm
    ''' </summary>
    Private Sub BAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAdd.Click
        'Recuperer login et userType du nouveau user
        Dim login As String = Split(CbLogin.Text.ToUpper, " -")(0)
        Dim nom As String = TNom.Text.ToUpper
        Dim prenom As String = TPrenom.Text
        Dim email As String = TEmail.Text
        Dim userType As String = Split(CbType.SelectedItem, " -")(0)
        Dim admin As Integer

        'Savoir si admin ou pas
        If (RbAdmin.Checked) Then
            admin = 1
        Else
            admin = 0
        End If

        'Check si toutes les info sont rempli
        If (String.IsNullOrEmpty(login) = False And String.IsNullOrEmpty(userType) = False And String.IsNullOrEmpty(nom) = False And String.IsNullOrEmpty(prenom) = False And String.IsNullOrEmpty(email) = False And checkMail(email) = True) Then
            'Check si la personne existe dans la bdd

            If (co.SelectSimple("UTILISATEUR", "Id").Contains(login)) Then
                'Modifaction de la personne dans la bdd
                Dim colNames As List(Of String) = New List(Of String)
                colNames.Add("Admin")
                colNames.Add("TypeUtilisateur")
                colNames.Add("Nom")
                colNames.Add("Prenom")
                colNames.Add("Email")
                Dim donnee As List(Of Object) = New List(Of Object)
                donnee.Add(admin)
                donnee.Add(userType)
                donnee.Add(nom)
                donnee.Add(prenom)
                donnee.Add(email)
                co.Update("UTILISATEUR", colNames, donnee, "id", login)

                log.Log(ELog.Information, "Button2_Click", "L'utilisateur  " & login & " - " & nom & " " & prenom & " a été modifié dans la BDD !")
                Me.Close()
            Else
                MessageBox.Show("Le login " & login & " - " & nom & " " & prenom & " n'existe pas dans la base de donnée !", "Echec de l'ajout de l'utilisateur " & login, MessageBoxButtons.OK, MessageBoxIcon.None)

            End If
        Else
            MessageBox.Show("Il manque des informations ou l'email n'est pas valide", "Echec de l'ajout de l'utilisateur " & CbLogin.Text, MessageBoxButtons.OK, MessageBoxIcon.None)
        End If
    End Sub

    ''' <summary>
    ''' CbLogin
    ''' </summary>
    Private Sub CbLogin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbLogin.TextChanged
        'Recupere le login a modifier
        Dim login As String = Split(CbLogin.Text, " -")(0)

        'Peuple les différents composants graphique
        TNom.Text = co.SelectDistinctWhere("Utilisateur", "Nom", "id", Login)(0).ToString.ToUpper
        TPrenom.Text = co.SelectDistinctWhere("Utilisateur", "prenom", "id", Login)(0)
        Dim id_type As String = co.SelectDistinctWhere("Utilisateur", "typeUtilisateur", "id", login)(0)
        TEmail.Text = co.SelectDistinctWhere("Utilisateur", "email", "id", login)(0)
        Dim libelle_type As String = co.SelectDistinctWhere("TYPE_USER", "Libelle", "TypeUser_id", id_type)(0)
        Dim type As String = id_type & " - " & libelle_type
        CbType.Text = type
        CbType.SelectedItem = type
        'Savoir si admin ou pas
        If (co.SelectDistinctWhere("Utilisateur", "Admin", "id", Login)(0)) Then
            RbAdmin.Checked = True
        Else
            RbAdmin.Checked = False
        End If
    End Sub

    ''' <summary>
    ''' Vérifie la valadité d'un mail
    ''' </summary>
    Public Function checkMail(ByVal email As String) As Boolean
        Dim pattern As String
        Dim res As Boolean = False
        pattern = "^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"
        If System.Text.RegularExpressions.Regex.IsMatch(email, pattern) Then
            res = True
        End If
        Return res
    End Function

    Private Sub BDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BDelete.Click
        'Check si le user a rentré toutes les informations
        If (String.IsNullOrEmpty(CbLogin.Text)) Then
            MessageBox.Show("Il manque des informations", "Echec ", MessageBoxButtons.OK, MessageBoxIcon.None)
            Exit Sub
        End If

        'Check si le login existe dans la BDD
        If (co.SelectSimple("UTILISATEUR", "Id").Contains(Split(CbLogin.Text.ToUpper, " -")(0))) Then
            'Suppression de l'utilisateur dans la BDD
            co.DeleteWhere("UTILISATEUR", "Id", Split(CbLogin.Text.ToUpper, " -")(0))
            log.Log(ELog.Information, "BDelete_Click", "L'utilisateur  " & CbLogin.SelectedText.ToUpper & " a été supprimé de la BDD !")
            Me.Close()
        Else
            MessageBox.Show("Le login " & CbLogin.Text & " n'existe pas dans la base de données !", "Echec de la supression de l'utilisateur " & CbLogin.Text, MessageBoxButtons.OK, MessageBoxIcon.None)
        End If
    End Sub
End Class