Public Class ScriptBDD

    ''' <summary>
    ''' Load de l'ihm pour lire le script de la bdd  
    ''' </summary>
    Private Sub ScriptBDD_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim co As Connection = New Connection()
        Dim fichier As Fichier = New Fichier()
        TextBox.Text = fichier.LectureFichierTxt("\SQL_SCRIPTS\Script.sql")
    End Sub
End Class