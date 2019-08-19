Imports System.Data.SqlClient
Imports System.IO


Public Class ConnectionOmega

    Dim log As Log = New Log()
    Dim coOmega As SqlConnection = New SqlConnection
    Dim co As Connection = New Connection()


#Region "Connection BDD"

    ''' <summary>
    ''' Permet de se connecter à la base OMEGA
    ''' </summary>
    Public Sub ToConnectOmega()
        Dim cmd As SqlCommand
        Dim da As SqlDataAdapter

        Try
            If coOmega.State <> ConnectionState.Open Then
                coOmega.ConnectionString = "Persist Security Info=False;database=" & Ini.dataBaseNameOmega &
                    ";server=" & Ini.serverNameOmega &
                    ";User ID=" & Ini.userNameOmega &
                    ";Password=" & Ini.passwordOmega &
                    ";Connect Timeout=60;" 'Par défaut 15seconds

                cmd = New SqlCommand
                da = New SqlDataAdapter
                coOmega.Open()
                cmd.Connection() = coOmega
                log.Log(ELog.Information, "ToConnect", "Connection à la base " & Ini.dataBaseNameOmega & " réussie !")
            End If

        Catch ex As Exception
            MsgBox("Erreur(s) lors de la connection à la base " & Ini.dataBaseNameOmega & " !")
            log.Log(ELog.Erreur, "ToConnect", "Erreur(s) lors de la connection à la base " & Ini.dataBaseNameOmega & " !")
        End Try
    End Sub

    ''' <summary>
    ''' Permet de se déconnecter d'une base de donnée
    ''' </summary>
    Public Sub ToDisconnectOmega()
        Try
            If coOmega.State = ConnectionState.Open Then
                coOmega.Close()
                log.Log(ELog.Information, "ToDisconnect", "Succès lors de la déconnection de la base de données !")
            End If

        Catch ex As Exception
            MsgBox("Erreur(s) lors de la déconnection de la base de données !")
            log.Log(ELog.Erreur, "ToDisconnect", "Erreur(s) lors de la déconnection de la base de données !")
        End Try
    End Sub

#End Region

#Region "Requêtes SQL"

    ''' <summary>
    ''' Execute une commande sql à partir  d'un Sting
    ''' </summary>
    Public Sub commandeSql(ByVal table As String, ByVal requete As String, Optional ByVal supprimer As Boolean = False)
        Dim myCommand As New SqlCommand(requete, coOmega)
        myCommand.CommandTimeout = 600
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(myReader.FieldCount - 1) {}
        Dim colName As List(Of String) = New List(Of String)
        Dim j As Integer = 0

        co.ToConnectBase()

        If supprimer = True Then
            co.DeleteFrom(table)
        End If

        While myReader.Read
            If j = 0 Then
                For i = 0 To row.Count - 1 Step 1
                    colName.Add(myReader.GetName(i))
                    j = j + 1
                Next
            End If
            myReader.GetValues(row)
            co.Insert(table, colName, row.ToList)
        End While
        myReader.Close()
    End Sub

    ''' <summary>
    ''' Execute une commande sql à partir  d'un Sting
    ''' </summary>
    Public Function commandeSqlToList(ByVal requete As String) As List(Of Object)
        Dim myCommand As New SqlCommand(requete, coOmega)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(myReader.FieldCount) {}
        Dim res As List(Of Object) = New List(Of Object)

        While myReader.Read
            myReader.GetValues(row)
            res.Add(row(0))
        End While
        myReader.Close()
        Return res
    End Function

    ''' <summary>
    ''' Execute une commande sql à partir  d'un Sting
    ''' </summary>
    Public Function commandeSqlToListReturn(ByVal requete As String) As List(Of Object)
        Dim myCommand As New SqlCommand(requete, coOmega)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(myReader.FieldCount) {}
        Dim res As List(Of Object) = New List(Of Object)

        While myReader.Read
            myReader.GetValues(row)
            res.Add(row.Clone)
        End While
        myReader.Close()
        Return res
    End Function
#End Region

#Region "Lire Fichier .txt"

    ''' <summary>
    ''' Permet de lire un fichier Sql en enlevant les lignes contenant --
    ''' </summary>
    Public Function LectureFichierSql(ByVal fichier As String) As String
        Dim contenu As String = String.Empty
        Dim path As String = My.Settings.PATH
        Dim chemin As String = path & "\SQL_SCRIPTS\OMEGA\" & fichier
        Try
            Dim monStreamReader As New StreamReader(chemin)
            Dim ligne As String
            Do
                ligne = monStreamReader.ReadLine
                If String.IsNullOrEmpty(ligne) = False Then
                    'si la ligne est en commentaire
                    If ligne.StartsWith("--") Then
                        Continue Do
                    End If
                    'si la ligne contient un commentaire
                    If ligne.Contains("--") Then
                        Dim instruction() As String = ligne.Split("--")
                        ligne = instruction(0)
                    End If
                    'concatener la ligne
                    contenu = contenu & ligne & " "
                End If
            Loop Until ligne Is Nothing
            monStreamReader.Close()
        Catch ex As Exception
            MsgBox("Une erreur est survenue au cours de l'accès en lecture du fichier " & chemin, "Erreur lors de l'ouverture d'un fichier")
            Return String.Empty
        End Try

        Return contenu
    End Function

    ''' <summary>
    ''' Permet de lire un fichier Sql tel quel
    ''' </summary>
    Public Function LectureFichierSqlOrigine(ByVal fichier As String) As String
        Dim contenu As String = String.Empty
        Dim path As String = My.Settings.PATH
        Dim chemin As String = path & "\SQL_SCRIPTS\OMEGA\" & fichier
        Try
            Dim monStreamReader As New StreamReader(chemin)
            contenu = monStreamReader.ReadToEnd()
            monStreamReader.Close()
        Catch ex As Exception
            MsgBox("Une erreur est survenue au cours de l'accès en lecture du fichier " & chemin, "Erreur lors de l'ouverture d'un fichier")
            Return String.Empty
        End Try

        Return contenu
    End Function

#End Region

End Class