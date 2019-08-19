Imports System.IO

Public Class Dossier

    Public Sub CopyDirectory(ByVal sourcePath As String, ByVal destinationPath As String)
        CopyDirectory(New DirectoryInfo(sourcePath), New DirectoryInfo(destinationPath))
    End Sub

    ''' <summary>
    ''' Copier le contenu d'un dossier
    ''' </summary>
    Private Shared Sub CopyDirectory(ByVal source As DirectoryInfo, ByVal destination As DirectoryInfo)
        destination.Create()

        For Each file As FileInfo In source.GetFiles()
            file.CopyTo(Path.Combine(destination.FullName, file.Name))
        Next

        For Each subDirectory As DirectoryInfo In source.GetDirectories()
            CopyDirectory(subDirectory, destination.CreateSubdirectory(subDirectory.Name))
        Next
    End Sub

    ''' <summary>
    ''' Supprimer contenu d'un dossier
    ''' </summary>
    Sub SupprimerDossier(ByVal PathOfDirectory As String)
        If My.Computer.FileSystem.DirectoryExists(PathOfDirectory) = True Then
            My.Computer.FileSystem.DeleteDirectory(PathOfDirectory, FileIO.DeleteDirectoryOption.DeleteAllContents)
        Else
            MsgBox("Le dossier à supprimer n'existe pas")
        End If
    End Sub

End Class
