Imports System.Deployment.Application
Imports System.Reflection
Imports System.Security.Principal

Imports WindowsApplication1.Referentiel
Imports WindowsApplication1.Action
Imports WindowsApplication1.Action.Consultation
Imports WindowsApplication1.Taux.BaseEmetteurs
Imports WindowsApplication1.Taux.PrimeObligIboxx



Public Class Accueil

    'Dim log As Login = New Login()
    Public co As Connection = New Connection()
    Dim excel As Excel = New Excel()
    Dim fi As Fichier = New Fichier()
    Dim incorrect = False

    ''' <summary>
    ''' Initilisation ihm Accueil
    ''' </summary>
    Public Sub AccueilIHM_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Affichage de la version dans le titre de la fenetre
        Me.Text = Me.Text + " ( version " + retourneVersionDeployee() + " )"

        'Tentative de connection
        fi.LectureFichierLog("Login.INI")
        co.ToConnectBase()


        Dim currentUser As WindowsIdentity = WindowsIdentity.GetCurrent()
        Dim login As String = Split(currentUser.Name.ToString, "\")(1).ToString.ToUpper

        'Checker si le login existe dans la BDD
        If (co.SelectSimple("UTILISATEUR", "Id").Contains(login)) Then
            Utilisateur.login = login
            Utilisateur.metier = co.SelectWhere("UTILISATEUR", "TypeUtilisateur", "Id", login).FirstOrDefault
            Utilisateur.admin = co.SelectWhere("UTILISATEUR", "admin", "Id", login).FirstOrDefault
            'Refresh champs derniere connexion d'un utilisateur
            co.Update("UTILISATEUR", New List(Of String)(New String() {"derniere_connexion"}), New List(Of Object)(New Object() {DateTime.Now.ToString}), "id", login)
        Else
            MessageBox.Show("Le nom d'utilisateur " & login & " est incorrect", "Echec d'authentification", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            incorrect = True
        End If

        'Affichiage du menu
        droitUtilisateur()

    End Sub


    Public Function retourneVersionDeployee() As String
        Dim titre As String
        Try
            titre = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString
        Catch
            titre = Assembly.GetExecutingAssembly().GetName().Version.ToString
        End Try
        Return titre
    End Function

    ''' <summary>
    ''' Menu ajouter un nouvelle utilisateur
    ''' </summary>
    Private Sub AjoutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AjoutToolStripMenuItem.Click
        Dim t As AjoutUtilisateur = New AjoutUtilisateur()
        t.ShowDialog()
        'AjoutUtilisateur.ShowDialog()
    End Sub

    ''' <summary>
    ''' Fermer ou non l'application
    ''' </summary>
    Private Sub AccueilIHM_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'If System.Windows.Forms.Application.OpenForms.Count = 1 Then
        If incorrect = False Then
            Dim a As Integer = MessageBox.Show("Etes-vous sûr de vouloir quitter et fermer toutes les fenêtres ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            If a = 1 Then
                'Update le champs derniere_deconnexion de l'utilisateur
                co.Update("UTILISATEUR", New List(Of String)(New String() {"derniere_deconnexion"}), New List(Of Object)(New Object() {DateTime.Now.ToString}), "id", Utilisateur.login)
                'log.Close()
                'Login.Close()
                co.ToDisconnect()
                'Application.Exit()
                'For Each oForm In System.Windows.Forms.Application.OpenForms
                'oForm.Exit()
                'Next oForm
                'For Each f In System.Windows.Forms.Application.OpenForms
                ' f.Close()
                ' Next
            End If
            If a = 2 Then
                e.Cancel = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Menu modification utilisateur
    ''' </summary>
    Private Sub ModificationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ModificationToolStripMenuItem.Click
        Dim modifU As New ModificationUtilisateur
        modifU.Show()
    End Sub

    ''' <summary>
    ''' Menu gestion des tables de la bdd
    ''' </summary>
    Private Sub GestionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GestionToolStripMenuItem.Click
        Dim GestionT As New GestionTable
        GestionT.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir script BDD
    ''' </summary>
    Private Sub ScriptBDDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScriptBDDToolStripMenuItem.Click
        Dim scriptBdd As New ScriptBDD
        scriptBdd.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir base émetteur
    ''' </summary>
    Private Sub TestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestToolStripMenuItem.Click
        Dim be As New BaseEmetteurs
        be.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir iboxx
    ''' </summary>
    Private Sub AnalyseIBoxxToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalyseIBoxxToolStripMenuItem.Click
        Dim ib As New Iboxx
        ib.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir transparence
    ''' </summary>
    Private Sub TransparenceToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TransparenceToolStripMenuItem1.Click
        Dim t As New Transparence
        t.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir réconciliation
    ''' </summary>
    Private Sub RéconciliationToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RéconciliationToolStripMenuItem1.Click
        Dim rec As New Reconciliation
        rec.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir base action Score
    ''' </summary>
    Private Sub TestToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestToolStripMenuItem1.Click
        Dim ba As New BaseAction
        ba.Show()
    End Sub

    ''' <summary>
    ''' Menu ouvrir base action Consultation
    ''' </summary>
    Private Sub ConsultationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConsultationToolStripMenuItem.Click
        Dim window As New BaseActionConsultation()
        System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
        window.Show()
    End Sub

    ''' <summary>
    ''' Menu reporting ouvrir extraction
    ''' </summary>
    Private Sub ExtractionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExtractionToolStripMenuItem.Click
        'Ouvrir IHM .....
    End Sub

    ''' <summary>
    ''' Gere les menus en fonction des droits : menu visible ou non
    ''' </summary>
    Private Sub droitUtilisateur()
        ActMenu.Visible = False
        TxMenu.Visible = False
        DivMenu.Visible = False
        ReMenu.Visible = False
        SolvencyToolStripMenuItem.Visible = False
        IntégrationToolStripMenuItem.Visible = False
        MiddleToolStripMenuItem.Visible = False

        'Récuperer les caractérisques de l'utilisateur = droit
        Select Case Utilisateur.metier
            Case "Act"
                ActMenu.Visible = True
                IntégrationToolStripMenuItem.Visible = True
            Case "Tx"
                TxMenu.Visible = True
                IntégrationToolStripMenuItem.Visible = True
                SolvencyToolStripMenuItem.Visible = True
            Case "Div"
                DivMenu.Visible = True
            Case "MO"
                MiddleToolStripMenuItem.Visible = True
                IntégrationToolStripMenuItem.Visible = True
            Case "Rep", "Ci"
                ReMenu.Visible = True
                TxMenu.Visible = True
            Case "Dir"
                ''équipe de direction
                ActMenu.Visible = True
                TxMenu.Visible = True
                DivMenu.Visible = True
                ReMenu.Visible = True
                SolvencyToolStripMenuItem.Visible = True
                IntégrationToolStripMenuItem.Visible = True
                MiddleToolStripMenuItem.Visible = True
            Case "Dev"
                ''équipe de dev : simulation
                ActMenu.Visible = True
                TxMenu.Visible = True
                DivMenu.Visible = True
                ReMenu.Visible = True
                SolvencyToolStripMenuItem.Visible = True
                IntégrationToolStripMenuItem.Visible = True
                MiddleToolStripMenuItem.Visible = True

            Case Else
                MsgBox("Votre type d'utilisateur n'est pas reconnu par l'application :" & co.SelectSimple("UTILISATEUR", "TypeUtilisateur")(0))
        End Select

        'Récuperer les caractéristique administratif
        Select Case Utilisateur.admin
            Case True
                ''admin droit à tout
                AdminMenu.Visible = True
            Case False
                AdminMenu.Visible = False
        End Select
    End Sub

    Private Sub VérificationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VérificationToolStripMenuItem.Click
        Dim v As New Verification
        v.Show()
    End Sub


    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim em As Email = New Email()
        em.sendMail(co.SelectDistinctWhere("UTILISATEUR", "email", "id", Utilisateur.login).FirstOrDefault, "efzf", "zezefdez")
    End Sub

    Private Sub SimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimulationToolStripMenuItem.Click
        Dim s As New Solv2
        s.Show()
    End Sub

    Private Sub Button1_Click_5(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'excel.ExcelToSql("G:\,FGA Soft\IMPORT\TAUX", "TX_BASE_SOLVENCY.xls", 3, "S2_TRANSITION")
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'excel.ExcelToSql("G:\,FGA Soft\INPUT\ACTION\FACTSET", "Factset_Agreg_1.xls", 1, "ACT_DATA_FACTSET_AGR", 7, 3)
    End Sub

    Private Sub Button1_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)
        excel.ExcelToSql("G:\,FGA Soft\INPUT\ACTION\FACTSET\data", "2012-05-08_Factset.xls", 2, "ACT_DATA_FACTSET_AGR")
    End Sub

    Private Sub Button1_Click_3(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub PortefeuilleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PortefeuilleToolStripMenuItem.Click
        Dim mp As New ModelPortfolio
        mp.Show()
    End Sub

    Private Sub Button1_Click_4(ByVal sender As System.Object, ByVal e As System.EventArgs)
        excel.ExcelToSql("G:\,FGA Systèmes\PROJETS\Modification sous secteur omega", "Copie benj de Titres - ss-secteurs.xls", 1, "TX_IBOXX_ISIN_SECTEUR_CORRESPONDANCE_tmp")
        '
    End Sub

    Private Sub CoefSecteursToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CoefSecteursToolStripMenuItem.Click
        Dim window As New Action.Coefficient.BaseActionCoefIndice

        window.ShowDialog()
    End Sub

    Private Sub CoefValeurToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CoefValeurToolStripMenuItem.Click
        Dim window As New Action.Coefficient.BaseActionCoefSecteur

        window.ShowDialog()
    End Sub

    Private Sub ConfigNoteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfigNoteToolStripMenuItem.Click
        Dim window As New Action.Note.BaseActionNote()

        window.ShowDialog()
    End Sub


    Private Sub ImportaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportaToolStripMenuItem.Click
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        BaseActionImportation.UpdateTicker("\INPUT\TAUX\CREDIT")

        Windows.Forms.Cursor.Current = Cursors.Default

    End Sub

    Private Sub AffectationTitreGrilleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AffectationTitreGrilleToolStripMenuItem.Click
        Dim act As New AccueilTitreGrille
        act.Show()
    End Sub

    Private Sub SuiviTitreManuelToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SuiviTitreManuelToolStripMenuItem.Click
        Dim st As New SuiviTitreManuel
        st.Show()
    End Sub

    Public Sub AlimentationPTFFGAToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlimentationPTFFGAToolStripMenuItem.Click
        Dim omega As ConnectionOmega = New ConnectionOmega()
        Dim DateTransparence As DateTime = "01-01-2099"
        Dim colWhere As List(Of String) = New List(Of String)()
        Dim donnee As List(Of Object) = New List(Of Object)()

        co.ToConnectBase()
        co.ToConnectBasebis()
        omega.ToConnectOmega()

        'Récupération des titres 
        omega.commandeSql("PTF_FGA", omega.LectureFichierSql("Strat_Alim_PTF_FGA.sql"), False)
        co.commandeSql("ptf_proxy", co.LectureFichierSql("Strat_ActulisationProxy.sql"), False)
        co.commandeSql("ptf_param_proxy", co.LectureFichierSql("Strat_ActualisationParamProxy.sql"), False)
        co.commandeSql("PTF_AN", co.LectureFichierSql("Strat_AN.sql"), False)
        'On transparise 

        If (co.SelectDistinctWhere("PTF_FGA", "Dateinventaire", "Dateinventaire", DateTransparence).Count > 0 And co.SelectDistinctWhere("PTF_PARAM_PROXY", "date", "date", DateTransparence).Count > 0 And co.SelectDistinctWhere("PTF_PROXY", "date", "date", DateTransparence).Count > 0) Then
            If (co.SelectDistinctSimple("PTF_TYPE_ACTIF", "Types").Count > 0 And co.SelectDistinctSimple("ZONE_GEOGRAPHIQUE", "Zone").Count > 0 And co.SelectDistinctSimple("PTF_CARAC_OPCVM", "Types").Count > 0 And co.SelectDistinctSimple("PTF_LOT", "id_lot").Count > 0 And co.SelectDistinctSimple("PTF_TYPE_DE_DETTE", "libelle").Count > 0) Then

                Dim paramDonnee As List(Of Object) = New List(Of Object)
                paramDonnee.Add(DateTransparence)
                Dim paramName As List(Of String) = New List(Of String)
                paramName.Add("@date")

                'On transparise le niveau 1 pour tous les comptes existants dans la bdd
                co.ProcedureStockée("Trans1", paramName, paramDonnee)
                co.ProcedureStockée("Trans2", paramName, paramDonnee)
                co.ProcedureStockée("Trans3", paramName, paramDonnee)
                co.ProcedureStockée("Trans4", paramName, paramDonnee)
                co.ProcedureStockée("Trans5", paramName, paramDonnee)
                colWhere.Add("date")
                donnee.Add(DateTransparence)
                co.DeleteWheres("Ptf_proxy", colWhere, donnee)
                co.DeleteWheres("Ptf_param_proxy", colWhere, donnee)
                co.DeleteWheres("Ptf_AN", colWhere, donnee)
                colWhere.Clear()
                colWhere.Add("dateinventaire")
                co.DeleteWheres("Ptf_fga", colWhere, donnee)
                co.commandeSql("STRAT_ALLOCATION", co.LectureFichierSql("Strat_CalculGrille.sql"), False)
                co.DeleteWheres("Ptf_Transparise", colWhere, donnee)

            Else
                MessageBox.Show("Impossible de transpariser le niveau  1 et/ou 2 et/ou 3 et/ou 4 à la date " & DateTransparence & " car il n'y a pas de données pour cette date dans les tables de concordance PTF_TYPE_ACTIF et/ou PTF_CARAC_OPCVM et/ou PTF_LOT et/ou PTF_TYPE_DE_DETTE et/ou ZONE_GEOGRAPHIQUE", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        Else
            MessageBox.Show("Impossible de transpariser le niveau  1 et/ou 2 et/ou 3 et/ou 4 à la date " & DateTransparence & " car il n'y a pas de données pour cette date dans la table PTF_FGA et/ou PTF_AN et/ou PTF_PROXY et/ou PTF_PARAM_PROXY", "Erreur.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    Private Sub DoublonsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DoublonsToolStripMenuItem.Click
        Dim window As New CorrectImportStockWindow()

        System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
        window.Show()
    End Sub

    ''' <summary>
    ''' Click sur le menu NewScreen : Ouvre la New Screen
    ''' </summary>
    Private Sub NewScreenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewScreenToolStripMenuItem.Click
        Dim window As New BaseActionConsultation()
        System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window)
        window.Show()
    End Sub

    Private Sub ImportationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportationToolStripMenuItem.Click
        Dim bai As New BaseActionImportation
        bai.Show()
    End Sub

    'Private Sub AllocationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllocationToolStripMenuItem.Click
    '    AllocationGrille.Show()
    'End Sub

    Private Sub AllocationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllocationToolStripMenuItem.Click
        Dim ag As New AllocationGrille
        ag.Show()
    End Sub

End Class