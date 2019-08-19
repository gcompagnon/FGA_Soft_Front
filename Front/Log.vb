Imports System.IO

Public Class Log



    ''' <summary>
    ''' Consigne les erreurs dans un fichier texte
    ''' </summary>
    Public Sub Log(ByVal enumInfo As ELog, ByVal nomMethode As String, ByVal description As String)
        Dim fp As Short
        Dim chemin As String

        fp = FreeFile()
        Dim path As String = My.Settings.PATH
        chemin = path & "\LOG\Log" & Utilisateur.login & ".txt"
        FileOpen(fp, chemin, OpenMode.Append, OpenAccess.Write, OpenShare.Shared)
        PrintLine(fp, My.Computer.Clock.LocalTime.ToString & " : " & enumInfo.ToString & " : " & nomMethode & " : " & description & ".")
        FileClose(fp)
    End Sub

    Public Sub LogSQLToFile(ByVal nomFichier As String, ByVal enumInfo As ELog, ByVal description As String)
        Dim fp As Integer
        Dim chemin As String

        fp = FreeFile()
        chemin = My.Settings.PATH & "\LOG\Log" & nomFichier & ".txt"
        FileOpen(fp, chemin, OpenMode.Append, OpenAccess.Write, OpenShare.Shared)
        PrintLine(fp, Utilisateur.login & " - " _
                  & My.Computer.Clock.LocalTime.ToString & " : " _
                  & enumInfo.ToString & " : " _
                  & description & ".")
        FileClose(fp)
    End Sub

    ' ''' <summary>
    ' ''' Supprime les enregistrements dans le fichier Log
    ' ''' </summary>
    'Public Sub DeleteFile()
    '    Dim path As String = My.Settings.PATH
    '    path = path & "\LOG\Log" & Utilisateur.login & ".txt"
    '    My.Computer.FileSystem.DeleteFile(path)
    'End Sub
End Class
