Public Class AjoutUtilisateur

    Dim co As Connection = New Connection()
    Dim log As Log = New Log()

    ''' <summary>
    ''' Initialisation de l'ihm
    ''' </summary>
    Private Sub AjoutUtilisateur_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TLogin.Text = ""
        TNom.Text = ""
        TPrenom.Text = ""

        'Tentative de connection a la BDD
        co.ToConnectBase()

        'Binder la liste des enum des metiers sur la listBox
        CbTypeUser.Items.Clear()
        Dim i As Integer = 0
        For i = 0 To co.SelectSimple("TYPE_USER", "Libelle").Count - 1 Step 1
            CbTypeUser.Items.Add(co.SelectSimple("TYPE_USER", "TypeUser_Id")(i) & "-" & co.SelectSimple("TYPE_USER", "Libelle")(i))
        Next
    End Sub

    ''' <summary>
    ''' BCancel : ferme l'ihm
    ''' </summary>
    Private Sub BCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAnnuler.Click
        Me.Dispose()
    End Sub

    ''' <summary>
    ''' BAdd : ajout new user dans la base
    ''' </summary>
    Private Sub BAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjouter.Click

        'Recuperer login et userType du nouveau user
        Dim login As String = TLogin.Text.ToUpper
        Dim nom As String = TNom.Text.ToUpper
        Dim prenom As String = TPrenom.Text
        Dim email As String = TEmail.Text & "@federisga.fr"
        Dim userType As String = Split(CbTypeUser.SelectedItem, "-")(0)
        Dim admin As Integer

        'Savoir si admin ou pas
        If (Administrateur.Checked) Then
            admin = 1
        Else
            admin = 0
        End If

        'Check si toutes les info sont rempli
        Dim em As Email = New Email()
        If (String.IsNullOrEmpty(login) = False And String.IsNullOrEmpty(userType) = False And String.IsNullOrEmpty(nom) = False And String.IsNullOrEmpty(prenom) = False And String.IsNullOrEmpty(email) = False And em.checkMail(email) = True) Then
            'Check si la personne existe dans la bdd
            If (co.SelectSimple("UTILISATEUR", "Id").Contains(login)) Then
                MessageBox.Show("Le login " & login & " - " & nom & " " & prenom & " existe dejà dans la base de donnée !", "Echec de l'ajout de l'utilisateur " & login, MessageBoxButtons.OK, MessageBoxIcon.None)
            Else
                'Ajout de la personne dans la bdd
                Dim colNames As List(Of String) = New List(Of String)
                colNames.Add("Id")
                colNames.Add("Nom")
                colNames.Add("Prenom")
                colNames.Add("Admin")
                colNames.Add("TypeUtilisateur")
                colNames.Add("Email")
                Dim donnee As List(Of Object) = New List(Of Object)
                donnee.Add(login)
                donnee.Add(nom)
                donnee.Add(prenom)
                donnee.Add(admin)
                donnee.Add(userType)
                donnee.Add(email)
                co.Insert("UTILISATEUR", colNames, donnee)
                log.Log(ELog.Information, "Button2_Click", "L'utilisateur  " & login & " - " & nom & " " & prenom & " a été ajouté dans UTILISATEUR !")
                Me.Close()
            End If
        Else
            MessageBox.Show("Il manque des informations", "Echec de l'ajout de l'utilisateur " & login & " - " & nom & " " & prenom, MessageBoxButtons.OK, MessageBoxIcon.None)
        End If
    End Sub

End Class