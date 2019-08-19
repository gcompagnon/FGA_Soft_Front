Public Class PopupWindowSave
    ''' <summary>
    ''' Associe un numéro de retour pour chaque bouton.
    ''' </summary>
    Private res As Integer = Nothing

    ''' <summary>
    ''' Return le résultat
    ''' </summary>
    Public Function getResult() As Integer
        Return res
    End Function

    ''' <summary>
    ''' Modifie la valeur du label en spécifiant l'emplacement concerné.
    ''' </summary>
    Public Sub setLocation()
        message.Content = "Voulez-vous enregistrer les modifications apportées ?"
    End Sub

    ''' <summary>
    ''' Met à jour la valeur du résultat puis ferme la fenêtre.
    ''' </summary>
    ''' <param name="result"></param>
    ''' <remarks></remarks>
    Private Sub closePopUp(ByVal result As Integer)
        res = result
        Me.Close()
    End Sub

    Private Sub BOui_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BOui.Click
        closePopUp(0)
    End Sub

    Private Sub BNon_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BNon.Click
        closePopUp(1)
    End Sub

    Private Sub Bannuler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Bannuler.Click
        closePopUp(-1)
    End Sub

End Class
