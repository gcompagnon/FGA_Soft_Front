Imports System.IO

Public Class Fichier

    ''' <summary>
    ''' Permet de lire les caractéristiques du fichier de connection
    ''' </summary>
    Public Sub LectureFichierLog(ByVal fichier As String)
        Try
            Dim nbr_ligne As Integer = 0
            Dim ligne As String
            ''Dim path As String = Split(System.AppDomain.CurrentDomain.BaseDirectory(), "bin")(0)
            Dim sr As New StreamReader(My.Settings.PATH & "\INPUT\CONFIGURATION\" & fichier)

            While sr.Peek <> -1
                nbr_ligne += 1
                ligne = sr.ReadLine()

                If fichier = "Login.INI" Then
                    Select Case nbr_ligne
                        Case 1
                            Ini.userNameBase = Split(ligne, "=")(1)
                        Case 2
                            Ini.passwordBase = Split(ligne, "=")(1)
                        Case 3
                            Ini.serverNameBase = Split(ligne, "=")(1)
                        Case 4
                            Ini.dataBaseNameBase = Split(ligne, "=")(1)
                        Case 6
                            Ini.userNameOmega = Split(ligne, "=")(1)
                        Case 7
                            Ini.passwordOmega = Split(ligne, "=")(1)
                        Case 8
                            Ini.serverNameOmega = Split(ligne, "=")(1)
                        Case 9
                            Ini.dataBaseNameOmega = Split(ligne, "=")(1)
                    End Select
                End If

                If fichier = "****.INI" Then

                End If

            End While
        Catch ex As Exception
            MsgBox("Une erreur est survenue au cours de l'accès en lecture du fichier de configuration du logiciel." & vbCrLf & "Veuillez vérifier l'emplacement : " & fichier, MsgBoxStyle.Critical, "Erreur lors du l'ouverture du fichier conf...")
        End Try
    End Sub

    ''' <summary>
    ''' Permet de lire un fichier Txt
    ''' </summary>
    Public Function LectureFichierTxt(ByVal fichier As String) As String
        Dim path As String = My.Settings.PATH
        Dim chemin As String = path & fichier
        Dim contenu As New String(" ", FileLen(chemin))
        FileOpen(1, chemin, OpenMode.Binary)
        FileGet(1, contenu)
        FileClose()
        Return contenu
    End Function

    ''' <summary>
    ''' Permet de lire un fichier Sql en enlevant les lignes contenant --
    ''' </summary>
    Public Function LectureFichierSql(ByVal path As String, ByVal fichier As String) As String
        Dim contenu As String = String.Empty
        Dim chemin As String = path & "\" & fichier
        Try
            Dim monStreamReader As New StreamReader(chemin, System.Text.Encoding.Default)
            Dim ligne As String
            Do
                ligne = monStreamReader.ReadLine
                If String.IsNullOrEmpty(ligne) = False Then
                    If ligne.Contains("--") = False Then
                        contenu = contenu & ligne & " "
                    End If
                End If
            Loop Until ligne Is Nothing
            monStreamReader.Close()
            Return contenu
        Catch ex As Exception
            MsgBox("Une erreur est survenue au cours de l'accès en lecture du fichier " & chemin, "Erreur lors de l'ouverture d'un fichier")
            Return String.Empty
        End Try

        Return contenu
    End Function

    ''' <summary>
    ''' Vérifie si un fichier existe
    ''' </summary>
    Public Function Existe(ByVal Path As String) As Boolean
        If Dir(Path) = "" Then
            Existe = False
        Else
            Existe = True
        End If
    End Function

End Class
