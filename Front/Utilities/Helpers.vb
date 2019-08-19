Public Class Helpers
    ''' <summary>
    ''' Utilitaire pour la presentation de chiffres au format String
    ''' </summary>
    ''' <param name="numberAsString"> le chiffre à modifier</param>
    ''' <param name="nbAfterDot">nb de chiffre apres la virgule voulue </param>
    ''' <param name="separator"> </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function DisplayRoundedNumber(ByRef numberAsString As String, ByRef nbAfterDot As UInteger, Optional ByRef separator As Char = ".") As String
        Dim dot As Integer = numberAsString.IndexOf(separator)
        If (dot = -1) Then
            DisplayRoundedNumber = numberAsString
        ElseIf (nbAfterDot + dot + 1 >= numberAsString.Length) Then
            DisplayRoundedNumber = numberAsString
        Else
            DisplayRoundedNumber = numberAsString.Substring(0, nbAfterDot + dot + 1)
        End If
    End Function
End Class
