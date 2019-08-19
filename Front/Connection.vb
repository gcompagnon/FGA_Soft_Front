Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Configuration

Public Class Connection
    'En attendant une combobox pour choisir la connection dans l'appli
    Public connectionString As String = "FGA_RW"
    'Dim connectionString As String = "FGA_PREPROD_RW"
    'Dim connectionString As String = "BENDB"

    Dim log As Log = New Log()
    Dim fichier As Fichier = New Fichier()
    Public Shared coBase As SqlConnection = New SqlConnection


    'On crée une deuxieme variable SQLConnection pour pouvoir exécuter 2 datareader en meme temps sur un meme base
    Public Shared coBaseBis As SqlConnection = New SqlConnection

    ''' <summary>
    ''' Permet de se connecter à une base de donnée
    ''' </summary>
    Public Sub ToConnectBase()
        Dim cmd As SqlCommand
        Dim da As SqlDataAdapter

        Try
            If coBase.State <> ConnectionState.Open Then
                Dim conString As String = ConfigurationManager.ConnectionStrings(Me.connectionString).ConnectionString
                'coBase.ConnectionString = "Persist Security Info=False;database=" & Ini.dataBaseNameBase & ";server=" & Ini.serverNameBase & ";User ID=" & Ini.userNameBase & ";Password=" & Ini.passwordBase & ";Connect Timeout=5000;"
                coBase.ConnectionString = conString
                cmd = New SqlCommand
                da = New SqlDataAdapter
                coBase.Open()
                cmd.Connection() = coBase
                log.Log(ELog.Information, "ToConnect", "Connection à la base " & Ini.dataBaseNameBase & " réussie !")
            End If

        Catch ex As Exception
            MsgBox("Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
            log.Log(ELog.Erreur, "ToConnect", "Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
        End Try
    End Sub

    ''' <summary>
    ''' Permet de se connecter à une base de donnée
    ''' On ajoute cette fonction pour avoir une deuxieme connection et pouvoir exécuter des scripts SQL via des StreamReader
    ''' SelectColonneName utilise un streamReader
    ''' On a ainsi une variable cobase et cobasebis en tant que connection SQL 
    ''' </summary>
    Public Sub ToConnectBasebis()
        'Dim cmd As SqlCommand
        'Dim da As SqlDataAdapter

        Try
            If coBaseBis.State <> ConnectionState.Open Then
                Dim conString As String = ConfigurationManager.ConnectionStrings(Me.connectionString).ConnectionString
                'coBaseBis.ConnectionString = "Persist Security Info=False;database=" & Ini.dataBaseNameBase & ";server=" & Ini.serverNameBase & ";User ID=" & Ini.userNameBase & ";Password=" & Ini.passwordBase & ";Connect Timeout=5000;"
                coBaseBis.ConnectionString = conString
                'cmd = New SqlCommand
                'da = New SqlDataAdapter
                coBaseBis.Open()
                'cmd.Connection() = coBaseBis
                log.Log(ELog.Information, "ToConnect", "Connection à la base " & Ini.dataBaseNameBase & " réussie !")
            End If

        Catch ex As Exception
            MsgBox("Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
            log.Log(ELog.Erreur, "ToConnect", "Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
        End Try
    End Sub

    ''' <summary>
    ''' Permet de se déconnecter d'une base de donnée
    ''' </summary>
    Public Sub ToDisconnect()
        Try
            If coBase.State = ConnectionState.Open Then
                coBase.Close()
                log.Log(ELog.Information, "ToDisconnect", "Succès lors de la déconnection de la base de données !")
            End If

        Catch ex As Exception
            MsgBox("Erreur(s) lors de la déconnection de la base de données !")
            log.Log(ELog.Erreur, "ToDisconnect", "Erreur(s) lors de la déconnection de la base de données !")
        End Try
    End Sub


#Region "Requêtes SQL"

    ''' <summary>
    ''' Execute une requete sql sans return
    ''' </summary>    
    Public Sub RequeteSql(ByVal sql As String)
        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Try
            command.ExecuteNonQuery()
            'log.Log(ELog.Information, "RequeteSql", "Execution d'une requete sql automatique")
        Catch ex As Exception
            MessageBox.Show(caption:="Erreur(s) d'une requete sql : " + ex.Message, text:=command.CommandText,
                            buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Execute une requete sql sans return
    ''' </summary>
    <STAThread()>
    Public Sub RequeteSqls(ByVal sqls As List(Of String))
        Dim requete As String = Nothing
        Dim command As SqlCommand = Nothing
        Dim buff As New StringBuilder

        Try
            For Each s In sqls
                buff.Append(s + " ")
            Next

            command = New SqlCommand(buff.ToString(), coBase)
            command.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(caption:="Erreur(s) d'une requete sql", text:=IIf(command Is Nothing, "", command.CommandText),
                            buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Execute une requete en bulk sans return
    ''' </summary>
    <STAThread()>
    Public Sub RequeteBulk(ByRef dt As DataTable, ByVal dn As String)
        Using bulkCopy As System.Data.SqlClient.SqlBulkCopy = New System.Data.SqlClient.SqlBulkCopy(coBase)
            bulkCopy.DestinationTableName = dn
            Try
                ' Write from the source to the destination.
                bulkCopy.WriteToServer(dt)
            Catch ex As Exception
                MessageBox.Show(caption:="Erreur(s) d'une requete sql", text:=ex.Message,
                            buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Execute une procedure stockée sans return
    ''' </summary>    
    Public Sub ProcedureStockée(ByVal nameProcedure As String, ByVal paramName As List(Of String), ByVal paramDonnee As List(Of Object))
        Try

            Dim sql As String = String.Empty
            If (paramName.Count > 0) Then
                For i = 0 To paramName.Count - 1 Step 1
                    sql = sql & " DECLARE " & paramName(i) & " "
                    If paramDonnee(i).GetType.Name.ToString = "String" Then
                        sql = sql & " VARCHAR(150) "
                    ElseIf paramDonnee(i).GetType.Name.ToString = "Int32" _
                        OrElse paramDonnee(i).GetType.Name.ToString = "Double" Then
                        sql = sql & " FLOAT "
                    Else
                        sql = sql & paramDonnee(i).GetType.Name.ToString()
                    End If
                Next
                For i = 0 To paramName.Count - 1 Step 1
                    sql = sql & " SET " & paramName(i) & " = "
                    If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") Then
                        sql = sql & "'"
                    End If
                    sql = sql & Replace(paramDonnee(i), "'", "''")
                    If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") Then
                        sql = sql & "'"
                    End If
                Next
                sql = sql & " EXECUTE " & nameProcedure & " "
                For i = 0 To paramName.Count - 1 Step 1
                    sql = sql & paramName(i)
                    If (i <> paramName.Count - 1) Then
                        sql = sql & ", "
                    End If
                Next
            Else
                sql = "EXECUTE " & nameProcedure
            End If

            Dim command As SqlCommand = New SqlCommand(sql, coBase)
            command.CommandTimeout = 600 ' Fix time out to 10 minutes.
            Dim reader As SqlDataReader = command.ExecuteReader
            reader.Close()
            log.Log(ELog.Information, "ProcedureStockée", "Execution de la procédure stockée " & nameProcedure)

        Catch ex As Exception
            MessageBox.Show(caption:="Erreur(s) de l'execution de la procédure stockée " & nameProcedure, text:=ex.Message,
                buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Execute une procedure stockée avec return
    ''' </summary>   
    Public Function ProcedureStockéeForDataGrid(ByVal nameProcedure As String, ByVal paramName As List(Of String), ByVal paramDonnee As List(Of Object)) As SqlCommand
        Dim sql As String = String.Empty
        If (paramName.Count > 0) Then
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & " DECLARE " & paramName(i) & " "
                If paramDonnee(i).GetType.Name.ToString = "String" Then
                    sql = sql & " VARCHAR(150) "
                ElseIf paramDonnee(i).GetType.Name.ToString() = "Int32" Then
                    sql = sql & "INTEGER "
                ElseIf paramDonnee(i).GetType.Name.ToString = "Decimal" Or paramDonnee(i).GetType.Name.ToString = "Double" Then
                    sql = sql & " FLOAT "
                ElseIf paramDonnee(i).GetType.Name.ToString = "Boolean" Then
                    sql = sql & " BIT "
                Else
                    sql = sql & paramDonnee(i).GetType.Name.ToString()
                End If
            Next
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & " SET " & paramName(i) & " = "
                If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime" Or paramDonnee(i).GetType.Name.ToString = "Boolean") Then
                    sql = sql & "'"
                End If
                sql = sql & Replace(paramDonnee(i), "'", "''")
                If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime" Or paramDonnee(i).GetType.Name.ToString = "Boolean") Then
                    sql = sql & "'"
                End If
            Next
            sql = sql & " EXECUTE " & nameProcedure & " "
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & paramName(i)
                If (i <> paramName.Count - 1) Then
                    sql = sql & ", "
                End If
            Next
        Else
            sql = "EXECUTE " & nameProcedure
        End If

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        log.Log(ELog.Information, "ProcedureStockée", "Execution de la procédure stockée " & nameProcedure)
        Return command
    End Function

    ''' <summary>
    ''' Execute une procedure stockée avec return
    ''' </summary>    
    Public Function ProcedureStockéeDico(ByVal nameProcedure As String, ByVal paramName As List(Of String), ByVal paramDonnee As List(Of Object)) As Dictionary(Of Object, Object)
        Dim sql As String = String.Empty
        If (paramName.Count > 0) Then
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & " DECLARE " & paramName(i) & " "
                If paramDonnee(i).GetType.Name.ToString = "String" Then
                    sql = sql & " VARCHAR(150) "
                ElseIf paramDonnee(i).GetType.Name.ToString = "Decimal" Or paramDonnee(i).GetType.Name.ToString = "Double" Then
                    sql = sql & " FLOAT "
                Else
                    sql = sql & paramDonnee(i).GetType.Name.ToString()
                End If
            Next
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & " SET " & paramName(i) & " = "
                If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") Then
                    sql = sql & "'"
                End If
                sql = sql & Replace(paramDonnee(i), "'", "''")
                If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") Then
                    sql = sql & "'"
                End If
            Next
            sql = sql & " EXECUTE " & nameProcedure & " "
            For i = 0 To paramName.Count - 1 Step 1
                sql = sql & paramName(i)
                If (i <> paramName.Count - 1) Then
                    sql = sql & ", "
                End If
            Next
        Else
            sql = "EXECUTE " & nameProcedure
        End If

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader

        Dim dico As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        Dim row As Object() = Nothing

        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i), row(i + 1))
                End If
                i = i + 2
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "ProcedureStockéeDico", "Execution de la procédure stockée " & nameProcedure)
        Return dico
    End Function

    ''' <summary>
    ''' Execute une procedure stockée avec return
    ''' </summary>    
    Public Function ProcedureStockéeList(ByVal nameProcedure As String, ByVal paramName As List(Of String), ByVal paramDonnee As List(Of Object)) As List(Of Object)
        Dim sql As String = String.Empty
        For i = 0 To paramName.Count - 1 Step 1
            sql = sql & " DECLARE " & paramName(i) & " "
            If paramDonnee(i).GetType.Name.ToString = "String" Then
                sql = sql & " VARCHAR(150) "
            ElseIf paramDonnee(i).GetType.Name.ToString() = "Int32" Then
                sql = sql & "INTEGER "
            Else
                sql = sql & paramDonnee(i).GetType.Name.ToString()
            End If

        Next
        For i = 0 To paramName.Count - 1 Step 1
            sql = sql & " SET " & paramName(i) & " = "
            If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") And Split(paramDonnee(i), "'%")(0) <> " " Then
                sql = sql & "'"
            End If
            If paramDonnee(i).ToString.Contains("%") Then
                sql = sql & paramDonnee(i)
            Else
                sql = sql & Replace(paramDonnee(i), "'", "''")
            End If
            If (paramDonnee(i).GetType.Name.ToString = "String" Or paramDonnee(i).GetType.Name.ToString = "DateTime") And Split(paramDonnee(i), "'%")(0) <> " " Then
                sql = sql & "'"
            End If
        Next
        sql = sql & " EXECUTE " & nameProcedure & " "
        For i = 0 To paramName.Count - 1 Step 1
            sql = sql & paramName(i)
            If (i <> paramName.Count - 1) Then
                sql = sql & ", "
            End If
        Next

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader

        Dim dico As List(Of Object) = New List(Of Object)()
        Dim row As Object() = Nothing

        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "ProcedureStockéeList", "Execution de la procédure stockée " & nameProcedure)
        Return dico
    End Function



    Public Function RequeteSqlToList(ByVal sql As String) As List(Of Object)

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader

        Dim dico As List(Of Object) = New List(Of Object)()
        Dim row As Object() = Nothing

        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        Return dico
    End Function


    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd
    ''' </summary>
    Public Function SelectSimple(ByVal table As String, ByVal colName As String) As List(Of Object)

        Dim command As SqlCommand = New SqlCommand("SELECT " & colName & " FROM " & table, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneFromTab", "Commande sql select * from réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd avec un distinct
    ''' </summary>
    Public Function SelectDistinctSimple(ByVal table As String, ByVal colName As String, Optional ByVal ordre As String = "ASC") As List(Of Object)
        Dim sql As String = "SELECT DISTINCT " & colName & " FROM " & table & " ORDER BY " & colName
        If ordre = "DESC" Then
            sql = sql & " DESC"
        End If
        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)

        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneFromTab", "Commande sql select * from réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd avec 1 where
    ''' </summary>
    Public Function SelectWhere(ByVal table As String, ByVal colName As String, ByVal colWhere As String, ByVal resWhere As Object) As List(Of Object)
        Dim sql, type As String
        sql = "SELECT " & colName & " FROM " & table & " WHERE " & colWhere & " = "
        type = TypeName(resWhere)
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
            sql = sql & Replace(resWhere, "'", "''")
            sql = sql & "'"
        Else
            sql = sql & resWhere
        End If

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select * from réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd avec une double condition where
    ''' </summary>
    Public Function SelectWhere2(ByVal table As String, ByVal colName As String, ByVal colWhere1 As String, ByVal resWhere1 As Object, ByVal colWhere2 As String, ByVal resWhere2 As Object, ByVal andVSor As Integer, Optional ByVal orderby As String = Nothing) As List(Of Object)
        Dim sql, type As String
        sql = "SELECT " & colName & " FROM " & table & " WHERE " & colWhere1 & " = "

        type = TypeName(resWhere1)
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If
        sql = sql & resWhere1
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If

        If (andVSor = 1) Then
            sql = sql & " AND " & colWhere2 & " = "
        Else
            sql = sql & " OR " & colWhere2 & " = "
        End If

        type = TypeName(resWhere2)
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If
        sql = sql & resWhere2
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If

        If orderby <> Nothing Then
            sql = sql & " order by " & orderby
        End If

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select * from réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd avec une condition where IS NOT NULL
    ''' </summary>
    Public Function SelectWhereNotNull(ByVal table As String, ByVal colName As String, ByVal colWhere1 As String) As List(Of Object)
        Dim sql As String
        sql = "SELECT " & colName & " FROM " & table & " WHERE " & colWhere1 & " IS NOT NULL"

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select réussie sur la table " & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne une colonne d'une table d'une bdd avec une condition where et un deuxieme chanps non vide
    ''' </summary>
    Public Function SelectDoubleWhereNotNull(ByVal table As String, ByVal colName As String, ByVal colWhere1 As String, ByVal resWhere1 As Object, ByVal colWhere2 As String, ByVal andVSor As Integer) As List(Of Object)
        Dim sql, type As String
        sql = "SELECT " & colName & " FROM " & table & " WHERE " & colWhere1 & " = "

        type = TypeName(resWhere1)
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If
        sql = sql & resWhere1
        If type = "String" Or type = "Date" Then
            sql = sql & "'"
        End If

        If (andVSor = 1) Then
            sql = sql & " AND " & colWhere2 & " IS NOT NULL"
        Else
            sql = sql & " OR " & colWhere2 & " IS NOT NULL"
        End If
        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select réussie sur la table " & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne distinct une colonne d'une table d'une bdd avec 1 where
    ''' </summary>
    Public Function SelectDistinctWhere(ByVal table As String, ByVal colName As String, ByVal colWhere As String, ByVal resWhere As Object, Optional ByVal operation As String = "=", Optional ByVal ordre As String = "ASC") As List(Of Object)
        Dim sql, type As String
        sql = "SELECT DISTINCT " & colName & " FROM " & table & " WHERE " & colWhere

        'If (Split(resWhere, "LIKE")(0) <> " ") Then
        If operation = "=" Then
            sql = sql & " = "
        Else
            sql = sql & " > "
        End If

        'End If

        type = TypeName(resWhere)

        If (type = "String" Or type = "Date") Then
            sql = sql & "'"
        End If
        'If Split(resWhere, "LIKE")(0) <> " " Then
        sql = sql & Replace(resWhere, "'", "''")
        'Else
        'sql = sql & resWhere
        'End If

        If (type = "String" Or type = "Date") Then
            sql = sql & "'"
        End If


        If ordre = "DESC" Then
            sql = sql & " ORDER BY " & colName & " DESC"
        End If

        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        'log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select double where réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne distinct une colonne d'une table d'une bdd avec plusieurs where
    ''' </summary>
    Public Function SelectDistinctWheres(ByVal table As String, ByVal colName As String, ByVal colWhere As List(Of String), ByVal donnee As List(Of Object), Optional ByVal colOR As String = "", Optional ByVal donneeOR As List(Of Object) = Nothing) As List(Of Object)
        Dim sql, type As String
        sql = "SELECT DISTINCT " & colName & " FROM " & table & " WHERE "

        For i = 0 To colWhere.Count - 1 Step 1
            type = TypeName(donnee(i))
            sql &= colWhere(i)
            ' tester si la valeur est sous la forme d un LIKE ou d un NOT LIKE
            Dim Like_Forme As String = Split(donnee(i), "LIKE")(0)
            Dim is_Like As Boolean = Like_Forme <> donnee(i)

            If (Not is_Like) Then
                If (type = "String" Or type = "Date") Then
                    sql &= " = '" & Replace(donnee(i), "'", "''") & "'"
                Else
                    sql &= " = " & donnee(i)
                End If
            Else ' en like : reprendre la valeur sans modification, tel quelle
                sql &= " " & donnee(i)
            End If

            If (i <> colWhere.Count - 1) Then
                sql &= " AND "
            End If
        Next

        If (colOR <> "") Then
            sql &= " AND " & colOR & " IN ("
            For i = 0 To donneeOR.Count - 1 Step 1
                type = TypeName(donneeOR(i))
                If (type = "String" Or type = "Date") Then
                    sql &= "'" & donneeOR(i) & "'"
                Else
                    sql &= donneeOR(i)
                End If

                If (i <> donneeOR.Count - 1) Then
                    sql &= ","
                End If
            Next
            sql &= ")"
        End If

        sql &= " ORDER BY " & colName
        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of Object)


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        'log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select distinct where réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Selectionne distinct 2 colonne d'une table d'une bdd avec plusieurs where
    ''' </summary>
    Public Function Select2Wheres(ByVal nameTab As String, ByVal colSelect As List(Of String), ByVal colWhere As List(Of String), ByVal donnee As List(Of Object)) As Dictionary(Of Object, Object)
        Dim sql As String = "SELECT "

        For i = 0 To colSelect.Count - 1 Step 1
            sql = sql & colSelect(i)
            If (i <> colSelect.Count - 1) Then
                sql = sql & " , "
            End If
        Next

        sql = sql & " FROM " & nameTab & " WHERE "

        Dim type As String
        For i = 0 To colWhere.Count - 1 Step 1

            type = TypeName(donnee(i))
            sql = sql & colWhere(i)

            If (Split(donnee(i), " ")(0) <> "LIKE") Then
                sql = sql & " = "
            End If

            If (type = "String" Or type = "Date") And Split(donnee(i), " ")(0) <> "LIKE" Then
                sql = sql & "'"
            End If
            sql = sql & donnee(i)
            If type = "String" Or type = "Date" And Split(donnee(i), " ")(0) <> "LIKE" Then
                sql = sql & "'"
            End If

            If (i <> colWhere.Count - 1) Then
                sql = sql & " AND "
            End If

        Next

        Dim dico As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        Dim myCommand As New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = Nothing


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i), row(i + 1))
                End If
                i = i + 2
            End While
        End While
        reader.Close()

        log.Log(ELog.Information, "GetDicoSelect", " ")
        Return dico
    End Function

    ''' <summary>
    ''' Selectionne distinct 2 colonne d'une table d'une bdd avec plusieurs where
    ''' </summary>
    Public Function Select2DistinctWheres(ByVal nameTab As String, ByVal colSelect As List(Of String), ByVal colWhere As List(Of String), ByVal donnee As List(Of Object)) As Dictionary(Of Object, Object)
        Dim sql As String = "SELECT "

        For i = 0 To colSelect.Count - 1 Step 1
            sql = sql & colSelect(i)
            If (i <> colSelect.Count - 1) Then
                sql = sql & " , "
            End If
        Next

        sql = sql & " FROM " & nameTab & " WHERE "

        Dim type As String
        For i = 0 To colWhere.Count - 1 Step 1

            type = TypeName(donnee(i))

            If (type <> "Nothing") Then
                sql = sql & colWhere(i)

                If (Split(donnee(i), " ")(0) <> "LIKE") Then
                    sql = sql & " = "
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), " ")(0) <> "LIKE" Then
                    sql = sql & "'"
                End If
                sql = sql & donnee(i)
                If type = "String" Or type = "Date" And Split(donnee(i), " ")(0) <> "LIKE" Then
                    sql = sql & "'"
                End If

                If (i <> colWhere.Count - 1) Then
                    sql = sql & " AND "
                End If
            End If

        Next

        Dim dico As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        Dim myCommand As New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = Nothing


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i), row(i + 1))
                End If
                i = i + 2
            End While
        End While
        reader.Close()

        log.Log(ELog.Information, "Select2DistinctWheres", " ")
        Return dico
    End Function

    ''' <summary>
    ''' Supprime une ligne d'une table à l'aide de son id EN STRING
    ''' </summary> 
    Public Sub DeleteWhere(ByVal tabName As String, ByVal colName As String, ByVal idToDelete As Object)
        Dim sqlCheck As String
        Dim sqlWork As String
        Dim sqlCheck2 As String

        'Vérifier si la supression est possible
        sqlCheck = "SELECT * FROM " & tabName & " WHERE " & colName & " = " & "'" & idToDelete & "'"
        Dim myCommand As New SqlCommand(sqlCheck, coBase)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        myReader.Read()

        If myReader.HasRows Then

            'Supression de la case
            myReader.Close()
            sqlWork = "DELETE FROM " & tabName & " WHERE " & colName & " = '" & idToDelete & "'"
            Dim myCommand2 As New SqlCommand(sqlWork, coBase)
            Dim myReader2 As SqlDataReader = myCommand2.ExecuteReader()
            myReader2.Read()
            myReader2.Close()

            'Check de la suppression
            sqlCheck2 = "SELECT * FROM " & tabName & " WHERE " & colName & " = '" & idToDelete & "'"
            Dim myCommand3 As New SqlCommand(sqlCheck2, coBase)
            Dim myReader3 As SqlDataReader = myCommand3.ExecuteReader()
            myReader3.Read()

            If myReader3.HasRows Then
                log.Log(ELog.Erreur, "Delete", "La commande sql DELETE n'a pas marchée !")
            End If
            myReader3.Close()

        Else
            log.Log(ELog.Information, "Delete", "La commande sql DELETE n'a pas trouvé l'id " & idToDelete & " !")
        End If
        myReader.Close()
    End Sub

    ''' <summary>
    ''' Supprimer toute les lignes d'une table 
    ''' </summary>
    Public Sub DeleteFrom(ByVal tabName As String)
        Dim sqlAdd As String
        'Ajout de la case
        sqlAdd = "DELETE FROM " & tabName
        Dim myCommand As New SqlCommand(sqlAdd, coBase)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        myReader.Read()
        myReader.Close()

        log.Log(ELog.Information, "DeleteFrom", "La commande sql DELETEFROM a supprimer tout le contenu de la table " & tabName & " !")

    End Sub

    ''' <summary>
    ''' Supprime une ligne d'une table 
    ''' </summary>
    Public Sub DeleteWheres(ByVal tabName As String, ByVal colWhere As List(Of String), ByVal donnee As List(Of Object))
        Dim sql As String
        sql = "DELETE FROM " & tabName & " WHERE "
        Dim type As String = String.Empty
        For i = 0 To colWhere.Count - 1 Step 1
            type = TypeName(donnee(i))
            sql = sql & colWhere(i)

            If donnee(i) Is Nothing Then
                sql &= " IS NULL"
            Else
                If (Split(donnee(i), "LIKE")(0) <> " ") Then
                    sql = sql & " = "
                End If

                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
                sql = sql & donnee(i)
                If (type = "String" Or type = "Date") And Split(donnee(i), "LIKE")(0) <> " " Then
                    sql = sql & "'"
                End If
            End If

            If (i < colWhere.Count - 1) Then
                sql = sql & " AND "
            End If
        Next

        Dim myCommand As New SqlCommand(sql, coBase)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        myReader.Read()
        myReader.Close()
        log.Log(ELog.Information, "DeleteWheres", "La commande sql DELETEWhere a supprimer tout le contenu de la table " & tabName & " !")
    End Sub

    ''' <summary>
    ''' Ajoute une ligne dans une tab 
    ''' </summary>
    Public Sub Insert(ByVal tabName As String, ByVal colNames As List(Of String), ByVal donnee As List(Of Object))
        Dim sqlAdd, type As String
        sqlAdd = Nothing
        Try
            If colNames.Count <> donnee.Count Then
                MsgBox("Problème d'insertion d'une ligne dans la base " & tabName & " (nombre de colonnes et donnees incoherentes)")
                Return
            End If

            ' Add column's index to ignoredList on unknown column.
            Dim columns As List(Of String) = SelectColonneName(tabName, order:=False)
            Dim ignored As New List(Of Integer)

            For i = 0 To columns.Count - 1
                columns(i) = columns(i).ToUpper
            Next

            'Construction de la chaine de facon dynamique
            sqlAdd = "INSERT INTO " & tabName & " ("
            For i = 0 To colNames.Count - 1 Step 1
                If Not columns.Contains(colNames(i).ToUpper) Then
                    ignored.Add(i)
                Else
                    sqlAdd = sqlAdd & colNames(i) & ", "
                End If
            Next

            ' Replace last "," with ")"
            If Right(sqlAdd, 2) = ", " Then
                sqlAdd = sqlAdd.Remove(sqlAdd.Count - 2, 2) & ") "
            End If

            sqlAdd = sqlAdd & " VALUES ("
            For i = 0 To colNames.Count - 1 Step 1
                If ignored.Contains(i) Then
                    Continue For
                End If
                type = TypeName(donnee(i))

                If (type = "String" Or type = "Date") Then
                    If donnee(i).ToString <> "" Then
                        sqlAdd = sqlAdd & "'"
                    Else
                        sqlAdd = sqlAdd & "NULL"
                    End If
                End If

                If IsNothing(donnee(i)) Then
                    sqlAdd = sqlAdd & "NULL"
                End If

                If IsDBNull(donnee(i)) Then
                    sqlAdd = sqlAdd & "NULL"
                Else
                    sqlAdd = sqlAdd & RTrim(Replace(donnee(i), "'", "''"))
                End If

                If (type = "String" Or type = "Date") Then
                    If donnee(i).ToString <> "" Then
                        sqlAdd = sqlAdd & "'"
                    End If
                End If

                sqlAdd = sqlAdd & ", "
            Next

            ' Replace last "," with ")"
            If Right(sqlAdd, 2) = ", " Then
                sqlAdd = sqlAdd.Remove(sqlAdd.Count - 2, 2) & ") "
            End If

            Dim myCommand2 As New SqlCommand(sqlAdd, coBase)
            Dim myReader2 As SqlDataReader = myCommand2.ExecuteReader()
            myReader2.Read()
            myReader2.Close()

        Catch ex As Exception
            If sqlAdd Is Nothing Then
                sqlAdd = "Aucun requete"
            End If
            MessageBox.Show(caption:="Problème d'insertion d'une ligne dans la base " & tabName, text:=sqlAdd & Chr(13) & ex.Message,
                buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Error)

        End Try
    End Sub

    ''' <summary>
    ''' Ajoute une ligne dans une tab 
    ''' </summary>
    Public Function InsertToSql(ByVal tabName As String, ByRef colNames As List(Of String), ByVal donnees As List(Of Object)) As String
        Return InsertColumnsToSql(tabName, colNames) + InsertDatasToSql(colNames, donnees)
    End Function

    Public Function InsertColumnsToSql(ByVal tabName As String, ByRef colNames As List(Of String)) As String
        Dim sqlAdd As New StringBuilder
        Dim columns As List(Of String) = SelectColonneName(tabName, order:=False)
        Dim i

        For i = 0 To columns.Count - 1
            columns(i) = columns(i).ToUpper
        Next

        'Construction de la chaine de facon dynamique
        sqlAdd.Append("INSERT INTO " & tabName & " (")
        i = 0
        Dim test As String
        While i < colNames.Count
            test = colNames(i)
            test = colNames(i).ToString
            If Not columns.Contains(colNames(i).ToUpper) Then
                colNames(i) = Nothing
                i += 1
            Else
                If IsNumeric(colNames(i)) Then
                    sqlAdd.Append("[" + colNames(i) & "], ")
                Else
                    sqlAdd.Append(colNames(i) & ", ")
                End If
                i += 1
            End If
        End While

        ' Remove last ","
        sqlAdd.Remove(sqlAdd.Length - 2, 1)
        sqlAdd.Append(")  VALUES ")

        Return sqlAdd.ToString
    End Function

    Public Function InsertDatasToSql(ByVal colNames As List(Of String), ByVal donnee As List(Of Object)) As String
        Dim sqlAdd As New StringBuilder("(")
        Dim type As String

        Try
            For i = 0 To colNames.Count - 1 Step 1
                If colNames(i) Is Nothing Then
                    Continue For
                End If

                type = TypeName(donnee(i))

                If (type = "String" Or type = "Date") Then
                    If donnee(i).ToString <> "" Then
                        sqlAdd.Append("'")
                    Else
                        sqlAdd.Append("NULL")
                    End If
                End If

                If IsNothing(donnee(i)) Or IsDBNull(donnee(i)) Then
                    sqlAdd.Append("NULL")
                Else
                    sqlAdd.Append(RTrim(Replace(donnee(i), "'", "''")))
                End If

                If (type = "String" Or type = "Date") Then
                    If donnee(i).ToString <> "" Then
                        sqlAdd.Append("'")
                    End If
                End If

                sqlAdd.Append(", ")
            Next

            ' Replace last "," with ")"
            If Right(sqlAdd.ToString, 2) = ", " Then
                sqlAdd.Remove(sqlAdd.Length - 2, 2)
                sqlAdd.Append(") ")
            End If

        Catch ex As Exception

        End Try
        Return sqlAdd.ToString
    End Function


    ''' <summary>
    ''' Update ligne dans une tab 
    ''' </summary>
    Public Sub Update(ByVal tabName As String, ByVal colNames As List(Of String), ByVal donnee As List(Of Object), ByVal colWhere As String, ByVal donneeWhere As Object)
        Dim sqlAdd, type As String

        'Construction de la chaine de facon dynamique
        sqlAdd = "UPDATE " & tabName & " SET "

        For i = 0 To donnee.Count - 1 Step 1
            sqlAdd = sqlAdd & colNames(i) & "="
            type = TypeName(donnee(i))

            If (type = "String" Or type = "Date") Then
                If donnee(i).ToString <> "" Then
                    sqlAdd = sqlAdd & "'"
                Else
                    sqlAdd = sqlAdd & "NULL"
                End If
            End If

            If IsNothing(donnee(i)) Then
                sqlAdd = sqlAdd & "NULL"
            End If

            If IsDBNull(donnee(i)) Then
                sqlAdd = sqlAdd & "NULL"
            Else
                sqlAdd = sqlAdd & RTrim(Replace(donnee(i), "'", "''"))
            End If

            If (type = "String" Or type = "Date") Then
                If donnee(i).ToString <> "" Then
                    sqlAdd = sqlAdd & "'"
                End If
            End If

            sqlAdd = sqlAdd & ", "
        Next

        ' Replace last "," with ")"
        If Right(sqlAdd, 2) = ", " Then
            sqlAdd = sqlAdd.Remove(sqlAdd.Count - 2, 2) & " "
        End If

        sqlAdd = sqlAdd & " WHERE " & colWhere & " = " & "'" & donneeWhere & "'"

        Dim myCommand2 As New SqlCommand(sqlAdd, coBase)
        Dim myReader2 As SqlDataReader = myCommand2.ExecuteReader()
        myReader2.Read()
        myReader2.Close()
    End Sub

    ''' <summary>
    ''' Updates ligne dans une tab avec plusieurs where 
    ''' </summary>
    Public Sub Updates(ByVal tabName As String, ByVal colNames As List(Of String), ByVal donnee As List(Of Object), ByVal colWhere As List(Of String), ByVal donneeWhere As List(Of Object))
        Dim sqlAdd, type As String

        'Construction de la chaine de facon dynamique
        sqlAdd = "UPDATE " & tabName & " SET "

        For i = 0 To donnee.Count - 1 Step 1
            sqlAdd = sqlAdd & colNames(i) & "="
            type = TypeName(donnee(i))

            If (type = "String" Or type = "Date") Then
                If donnee(i).ToString <> "" Then
                    sqlAdd = sqlAdd & "'"
                Else
                    sqlAdd = sqlAdd & "NULL"
                End If
            End If

            If IsNothing(donnee(i)) Then
                sqlAdd = sqlAdd & "NULL"
            End If

            If IsDBNull(donnee(i)) Then
                sqlAdd = sqlAdd & "NULL"
            Else
                sqlAdd = sqlAdd & RTrim(Replace(donnee(i), "'", "''"))
            End If

            If (type = "String" Or type = "Date") Then
                If donnee(i).ToString <> "" Then
                    sqlAdd = sqlAdd & "'"
                End If
            End If

            sqlAdd = sqlAdd & ", "
        Next

        ' Replace last "," with ")"
        If Right(sqlAdd, 2) = ", " Then
            sqlAdd = sqlAdd.Remove(sqlAdd.Count - 2, 2) & " "
        End If

        sqlAdd = sqlAdd & " WHERE "

        For i = 0 To colWhere.Count - 1 Step 1
            sqlAdd = sqlAdd & colWhere(i) & "="
            type = TypeName(donneeWhere(i))
            If type = "String" Or type = "Date" Then
                sqlAdd = sqlAdd & "'"
            End If
            If (i = donneeWhere.Count - 1) Then
                sqlAdd = sqlAdd & donneeWhere(i)
                If type = "String" Or type = "Date" Then
                    sqlAdd = sqlAdd & "'"
                End If
                Exit For
            End If
            sqlAdd = sqlAdd & donneeWhere(i)
            If type = "String" Or type = "Date" Then
                sqlAdd = sqlAdd & "'"
            End If
            sqlAdd = sqlAdd & " AND "
        Next

        Dim myCommand As New SqlCommand(sqlAdd, coBase)
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        myReader.Read()
        myReader.Close()
    End Sub

    ''' <summary>
    ''' Selectionne le nom des colonnes d'une table du type spécifié
    ''' </summary>
    Public Function SelectColonneName2() As List(Of String)
        Dim sql As New StringBuilder
        Dim res As New List(Of String)

        sql.Append("SELECT name FROM sys.columns")
        sql.Append(" WHERE object_id =  OBJECT_ID('DATA_FACTSET')")
        sql.Append(" ORDER BY name")

        Try
            Dim myCommand As New SqlCommand(sql.ToString, coBase)
            Dim reader As SqlDataReader = myCommand.ExecuteReader()
            Dim row As Object() = New Object(reader.FieldCount) {}

            While reader.Read
                'Copie d'une ligne entière de la table
                reader.GetValues(row)
                Dim i As Integer = 0
                'Traitement de la ligne
                While i < row.GetLength(0)
                    If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                        res.Add(row(i))
                    End If
                    i = i + 1
                End While
            End While
            reader.Close()
            'log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select * from réussie sur la table" & "DATA_FACTSET" & " !")
        Catch ex As Exception
            'MsgBox("Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
            'log.Log(ELog.Erreur, "ToConnect", "Erreur(s) lors de la connection à la base " & Ini.dataBaseNameBase & " !")
        End Try

        Return res
    End Function
    ''' <summary>
    ''' Selectionne le nom des colonnes d'une table du type spécifié
    ''' </summary>
    Public Function SelectColonneName(ByVal table As String, Optional ByVal data_type As String = Nothing, Optional ByVal order As Boolean = True) As List(Of String)
        Dim sql As String = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" & table & "'"
        If data_type IsNot Nothing Then
            sql &= " AND DATA_TYPE='" & data_type & "'"
        End If
        If order = True Then
            sql &= " ORDER BY COLUMN_NAME"
        End If


        Dim command As SqlCommand = New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = command.ExecuteReader
        Dim row As Object() = Nothing
        Dim res As New List(Of String)

        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    res.Add(row(i))
                End If
                i = i + 1
            End While
        End While
        reader.Close()
        log.Log(ELog.Information, "SelectColonneWhere", "Commande sql select * from réussie sur la table" & table & " !")
        Return res
    End Function

    ''' <summary>
    ''' Renvoie un select dans un dico 
    ''' </summary>
    Public Function GetDicoSelect(ByVal nameTab As String, ByVal colName1 As String, ByVal colName2 As String) As Dictionary(Of Object, Object)
        Dim sql As String = "SELECT " & colName1 & "," & colName2 & " FROM " & nameTab
        Dim dico As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        Dim myCommand As New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = Nothing


        While reader.Read
            If row Is Nothing Then
                row = New Object(reader.FieldCount) {}
            End If
            'Copie d'une ligne entière de la table
            reader.GetValues(row)
            Dim i As Integer = 0
            'Traitement de la ligne
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(row(i), row(i + 1))
                End If
                i = i + 2
            End While
        End While
        reader.Close()

        log.Log(ELog.Information, "GetDicoSelect", " ")
        Return dico
    End Function

    ''' <summary>
    ''' Remplie un Data Grid à l'aide d'un string
    ''' </summary> 
    Public Function LoadDataGridByString(ByVal requeteSQL As String) As BindingSource
        Dim myCommand As New SqlCommand(requeteSQL, coBase)
        Dim adpt As New Data.SqlClient.SqlDataAdapter(myCommand)
        Dim dt As New DataTable
        adpt.Fill(dt)
        Return New BindingSource(dt, Nothing)
    End Function

    ''' <summary>
    ''' Remplie un Data Grid à l'aide d'un string
    ''' </summary> 
    Public Function RequeteSqlToDataSet(ByVal requeteSQL As String, ByVal dataSetName As String) As DataSet
        Dim myCommand As New SqlCommand(requeteSQL, coBase)
        Dim adpt As New Data.SqlClient.SqlDataAdapter(myCommand)
        Dim oDataSet As New DataSet(dataSetName)
        adpt.Fill(oDataSet, dataSetName)

        'oDataSet.Tables("Categories").Rows(i)(0)
        Return oDataSet
    End Function


    Public Function RequeteSqlToDataSetAutomatic(ByVal nameTab As String, ByVal colSelect As List(Of String), ByVal colWhere As List(Of String), ByVal donnee As List(Of Object), ByVal dataSetName As String) As DataSet
        Dim sql As String = "SELECT "

        For i = 0 To colSelect.Count - 1 Step 1
            sql = sql & colSelect(i)
            If (i <> colSelect.Count - 1) Then
                sql = sql & " , "
            End If
        Next

        sql = sql & " FROM " & nameTab & " WHERE "

        Dim type As String
        For i = 0 To colWhere.Count - 1 Step 1

            type = TypeName(donnee(i))
            sql = sql & colWhere(i)

            If (Split(donnee(i), " ")(0) <> "LIKE") And (Split(donnee(i), " ")(0) <> "IS") Then
                sql = sql & " = "
            End If

            If (type = "String" Or type = "Date") And (Split(donnee(i), " ")(0) <> "LIKE" And Split(donnee(i), " ")(0) <> "IS") Then
                sql = sql & "'"
            End If
            sql = sql & donnee(i)
            If (type = "String" Or type = "Date") And (Split(donnee(i), " ")(0) <> "LIKE" And Split(donnee(i), " ")(0) <> "IS") Then
                sql = sql & "'"
            End If

            If (i <> colWhere.Count - 1) Then
                sql = sql & " AND "
            End If

        Next

        Dim myCommand As New SqlCommand(sql, coBase)
        Dim adpt As New Data.SqlClient.SqlDataAdapter(myCommand)
        Dim oDataSet As New DataSet(dataSetName)
        adpt.Fill(oDataSet, dataSetName)

        'oDataSet.Tables("Categories").Rows(i)(0)
        Return oDataSet

    End Function

    ''' <summary>
    ''' Remplie un Data Grid à l'aide d'une procédure stockée
    ''' </summary> 
    Public Function LoadDataGridByProcedureStockée(ByVal sql As SqlCommand) As BindingSource
        Dim adpt As New Data.SqlClient.SqlDataAdapter(sql)
        Dim dt As New DataTable
        adpt.Fill(dt)
        Return New BindingSource(dt, Nothing)
    End Function

    ''' <summary>
    ''' Export une requete sql vers un fichier excel non existant
    ''' </summary>
    Public Sub SqlToExcelEndSave(ByVal sql As List(Of String), ByVal chemin As String, Optional ByVal j As Integer = 0, Optional ByVal iligne As Integer = 1)

        Dim xl As New Microsoft.Office.Interop.Excel.Application
        xl.DisplayAlerts = False 'annule les messages
        xl.Visible = False
        xl.Workbooks.Add()


        For nbrSql = 0 To sql.Count - 1 Step 1

            If (nbrSql >= 3) Then
                xl.Sheets.Add(After:=xl.Worksheets(xl.Worksheets.Count))
                'xl.Worksheets.Add()
                'xl.Sheets.Add()
            End If
            Dim command As SqlCommand = New SqlCommand(sql(nbrSql), coBase)
            Dim reader As SqlDataReader = command.ExecuteReader
            Dim row As Object() = Nothing
            iligne = 1
            Dim jcol As Integer
            'Dim j As Integer
            j = 0
            jcol = 0

            While reader.Read
                If row Is Nothing Then
                    row = New Object(reader.FieldCount) {}
                End If
                reader.GetValues(row)
                If j = 0 Then
                    For i = 0 To row.Count - 2 Step 1
                        xl.Sheets(nbrSql + 1).Cells(1, jcol + 1) = reader.GetName(i)
                        jcol = jcol + 1
                        j = j + 1
                    Next
                End If
                jcol = 0
                iligne = iligne + 1
                'Traitement de la ligne
                While jcol < row.GetLength(0)
                    If Not row(jcol) Is Nothing AndAlso Not (row(jcol) Is DBNull.Value) Then
                        xl.Sheets(nbrSql + 1).Cells(iligne, jcol + 1).formula = row(jcol)
                    End If
                    jcol = jcol + 1
                End While
            End While
            reader.Close()
        Next

        'xl.Sheets(numeroFeuille).Name = nomFeuille
        'Enregistrement du fichier XLS
        xl.ActiveWorkbook.SaveAs(chemin)
        xl.DisplayAlerts = True
        xl.Quit() ' quitte l'application
        xl = Nothing ' libère la memoire
        log.Log(ELog.Information, "SqlToExcel", "Export d'une requete sql vers excel !")

    End Sub


    ''' <summary>
    ''' Export une requete sql vers un nouveau fichier excel et le laisse ouvert
    ''' </summary>
    Public Sub SqlToNewExcel(ByVal sql As List(Of String), Optional ByVal iligne As Integer = 1)

        Dim xl As New Microsoft.Office.Interop.Excel.Application
        xl.DisplayAlerts = False 'annule les messages
        xl.Visible = False
        xl.Workbooks.Add()

        Dim maLigne = iligne


        For nbrSql = 0 To sql.Count - 1 Step 1

            If (nbrSql >= 3) Then
                xl.Sheets.Add(After:=xl.Worksheets(xl.Worksheets.Count))
                'xl.Worksheets.Add()
                'xl.Sheets.Add()
            End If
            Dim command As SqlCommand = New SqlCommand(sql(nbrSql), coBase)
            Dim reader As SqlDataReader = command.ExecuteReader
            Dim row As Object() = Nothing
            iligne = maLigne
            Dim jcol As Integer
            'Dim j As Integer
            Dim titre As Boolean = True
            jcol = 0

            While reader.Read
                If row Is Nothing Then
                    row = New Object(reader.FieldCount) {}
                End If
                reader.GetValues(row)
                If titre Then
                    For i = 0 To row.Count - 2 Step 1
                        xl.Sheets(nbrSql + 1).Cells(1, jcol + 1) = reader.GetName(i)
                        jcol = jcol + 1
                        titre = False
                    Next
                End If
                jcol = 0
                iligne = iligne + 1
                'Traitement de la ligne
                While jcol < row.GetLength(0)
                    If Not row(jcol) Is Nothing AndAlso Not (row(jcol) Is DBNull.Value) Then
                        xl.Sheets(nbrSql + 1).Cells(iligne, jcol + 1).formula = row(jcol)
                    End If
                    jcol = jcol + 1
                End While
            End While
            reader.Close()

            'Filtre + Volet du fichier excel
            xl.Sheets(nbrSql + 1).Activate()
            xl.Sheets(nbrSql + 1).Rows(maLigne + 1).Select()
            xl.ActiveWindow.FreezePanes = True 'Volet
            xl.Sheets(nbrSql + 1).Rows(maLigne).Font.Size = 11 'taille 11
            xl.Sheets(nbrSql + 1).Rows(maLigne).Font.Bold = True 'gras
            xl.Sheets(nbrSql + 1).Rows(maLigne).Autofilter() 'Filtre
            xl.Sheets(nbrSql + 1).Cells.EntireColumn.AutoFit()
        Next

        xl.Visible = True
        log.Log(ELog.Information, "SqlToExcel", "Export d'une requete sql vers excel !")

    End Sub

    ''' <summary>
    ''' Export une requete sql vers un fichier excel existant
    ''' </summary>
    Public Sub SqlToExcelExistant(ByVal sql As String, ByVal cheminExcel As String, ByVal excelName As String, ByVal newChemin As String, Optional ByVal titre As Boolean = True, Optional ByVal ligneDebut As Integer = 1, Optional ByVal finCol As Integer = 3, Optional ByVal libelle As String = Nothing)

        If fichier.Existe(cheminExcel & "\" & excelName) Then
            Dim appExcel As Microsoft.Office.Interop.Excel.Application
            Try
                appExcel = CType(GetObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            Catch ex As Exception
                appExcel = CType(CreateObject("Excel.Application"), Microsoft.Office.Interop.Excel.Application)  'Application Excel
            End Try

            'appExcel.Visible = False
            'Desactiver l execution de macros en automatique (utile pour l addin Bloomberg)
            appExcel.AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable

            Dim wbExcel As Microsoft.Office.Interop.Excel.Workbook = appExcel.Workbooks.Open(cheminExcel & "\" & excelName, , True) 'Classeur Excel
            Dim sheet As Microsoft.Office.Interop.Excel.Worksheet = wbExcel.Worksheets(1) 'Feuille Excel

            If String.IsNullOrEmpty(libelle) = False Then
                'ajout d'un ' pour eviter l interpretation par excel en date format americain mm/dd/yyyy
                sheet.Cells(1, 1) = "'" + libelle
            End If

            If finCol <> 0 Then
                sheet.Range(sheet.Cells(ligneDebut + 1, 1), sheet.Cells(65536, finCol)).ClearContents()
            End If

            Dim command As SqlCommand = New SqlCommand(sql, coBase)
            Dim reader As SqlDataReader = command.ExecuteReader
            Dim row As Object() = Nothing

            Dim jcol As Integer
            'Dim j As Integer
            'j = 0
            jcol = 0

            While reader.Read
                If row Is Nothing Then
                    row = New Object(reader.FieldCount) {}
                End If
                reader.GetValues(row)
                If titre = True Then
                    For i = 0 To row.Count - 2 Step 1
                        sheet.Cells(ligneDebut, jcol + 1) = reader.GetName(i)
                        jcol = jcol + 1
                        titre = False
                    Next
                End If
                jcol = 0
                ligneDebut = ligneDebut + 1
                'Traitement de la ligne
                While jcol < row.GetLength(0)
                    If Not row(jcol) Is Nothing AndAlso Not (row(jcol) Is DBNull.Value) Then
                        sheet.Cells(ligneDebut, jcol + 1).formula = row(jcol)
                    End If
                    jcol = jcol + 1
                End While
            End While
            reader.Close()

            appExcel.DisplayAlerts = False 'annule les messages
            wbExcel.SaveAs(newChemin)
            wbExcel.Close(cheminExcel & "\" & excelName)
            appExcel.DisplayAlerts = True
            appExcel.Quit() ' quitte l'application

            'sheet.ActiveWorkbook.SaveAs(chemin)

            appExcel = Nothing ' libère la memoire
            wbExcel = Nothing
            sheet = Nothing
            log.Log(ELog.Information, "SqlToExcel", "Export d'une requete sql vers excel !")

        Else
            MessageBox.Show("Le fichier excel " & excelName & " n'existe pas !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End If
    End Sub

    ''' <summary>
    ''' Retourne une liste de dictionnaire dont chaque dictionnaire correspond à une ligne d'une
    ''' table et ses colonnes.
    ''' </summary>
    ''' <param name="sql">Requête SQL contenant le select</param>
    ''' <returns>La liste de dictionnaire ayant comme clé la colonne de la table et comme valeur 
    ''' le champs de la ligne</returns>
    Public Function sqlToListDico(ByVal sql As String) As List(Of Dictionary(Of String, Object))
        Dim listDico As New List(Of Dictionary(Of String, Object))
        Dim index As Integer = 0
        Dim myCommand As New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(reader.FieldCount) {}

        While reader.Read
            Dim dico As New Dictionary(Of String, Object)

            'Copie d'une ligne entière de la table
            reader.GetValues(row)

            'Traitement de la ligne
            Dim i As Integer = 0
            While i < row.GetLength(0)
                If Not row(i) Is Nothing AndAlso Not (row(i) Is DBNull.Value) Then
                    dico.Add(reader.GetName(i), row(i))
                End If
                i = i + 1
            End While
            listDico.Add(dico)
        End While
        reader.Close()

        ' Disable log... Too much pain.
        'log.Log(ELog.Information, "sqlToListDico", " ")
        Return listDico
    End Function

    ''' <summary>
    ''' Requete SQL pour remplir la partie General
    ''' </summary>
    ''' <param name="table">Nom de la table</param>
    ''' <param name="columns">List de String avec le nom des columns à afficher</param>
    ''' <param name="conditions">Table de Hachage avec les conditions</param>
    ''' <returns>s</returns>
    ''' <remarks>r</remarks>
    Public Function getSqlMultipleFields(ByVal table As String, ByVal columns As Dictionary(Of String, String), ByVal conditions As Dictionary(Of String, Object)) As String
        Dim sql As New StringBuilder

        sql.Append("SELECT ")
        For Each col As String In columns.Keys
            If columns(col) Is Nothing Then
                sql.Append(col + ",")
            Else
                sql.Append(col + " As """ + columns(col) + """,")
            End If
        Next

        sql.Remove(sql.Length - 1, 1)
        sql.Append(" FROM " + table)
        sql.Append(" WHERE ")

        For Each c As String In conditions.Keys
            If conditions(c) Is Nothing Then
                sql.Append(c + " is null AND ")
                Continue For
            End If

            sql.Append(c + " = ")
            If conditions(c).GetType.Name.ToString = "Decimal" Or conditions(c).GetType.Name.ToString = "Double" Or conditions(c).GetType.Name.ToString = "Int32" Then
                sql.Append(conditions(c))
            ElseIf conditions(c).GetType.Name.ToString = "Boolean" Then
                If conditions(c) Then
                    sql.Append("1")
                Else
                    sql.Append("0")
                End If
            Else
                sql.Append("'" + conditions(c).ToString.Replace("'", "''") + "'")
            End If

            sql.Append(" AND ")
        Next
        sql.Append("1 = 1")

        Return sql.ToString
    End Function

    ''' <summary>
    ''' Retourne une liste d'objet de type T à partir d'une requête SQL.
    ''' </summary>
    ''' <remarks>
    ''' Le constructeur du générique doit être précisé
    ''' Les colonnes dans la requête SQL doivent être identiques aux champs de l'objet T.
    ''' </remarks>
    Public Function sqlToListObject(Of T)(ByVal sql As String, ByVal ctor As Func(Of T)) As List(Of T)
        Dim list As New List(Of T)
        Dim index As Integer = 0
        Dim myCommand As New SqlCommand(sql, coBase)
        Dim reader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(reader.FieldCount) {}

        While reader.Read
            Dim obj As T = ctor()

            'Copie d'une ligne entière de la table
            reader.GetValues(row)

            'Traitement de la ligne
            For i = 0 To row.GetLength(0) - 2
                Try
                    If row(i) Is DBNull.Value Then
                        obj.GetType().GetProperty(reader.GetName(i)).SetValue(obj, Nothing, Nothing)
                    Else
                        Dim name = reader.GetName(i)
                        Dim prop As Reflection.PropertyInfo = obj.GetType.GetProperty(reader.GetName(i))
                        Dim val As Object = Convert.ChangeType(row(i), prop.PropertyType)
                        obj.GetType().GetProperty(reader.GetName(i)).SetValue(obj, val, Nothing)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.ToString)
                End Try
            Next

            list.Add(obj)
        End While
        reader.Close()

        ' Disable log... Too much pain.
        'log.Log(ELog.Information, "sqlToListObject", " ")
        Return list
    End Function

    ''' <summary>
    ''' Execute une commande sql à partir  d'un Sting
    ''' </summary>
    Public Sub commandeSql(ByVal table As String, ByVal requete As String, Optional ByVal supprimer As Boolean = False)
        Dim myCommand As New SqlCommand(requete, coBaseBis)
        myCommand.CommandTimeout = 600
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(myReader.FieldCount - 1) {}
        Dim colName As List(Of String) = New List(Of String)
        Dim colrow As List(Of Object) = New List(Of Object)
        Dim j As Integer = 0



        If supprimer = True Then
            Me.DeleteFrom(table)
        End If

        While myReader.Read
            If j = 0 Then
                For i = 0 To row.Count - 1 Step 1
                    colName.Add(myReader.GetName(i))
                    j = j + 1
                Next
            End If
            myReader.GetValues(row)
            Me.Insert(table, colName, row.ToList)
        End While
        myReader.Close()
    End Sub

    ''' <summary>
    ''' Execute une commande sql à partir  d'un String sans insertion dans une table de la BDD
    ''' </summary>
    Public Function commandeSqlnoInsert(ByVal requete As String)
        Dim myCommand As New SqlCommand(requete, coBase)
        myCommand.CommandTimeout = 600
        Dim myReader As SqlDataReader = myCommand.ExecuteReader()
        Dim row As Object() = New Object(myReader.FieldCount - 1) {}
        Dim colName As List(Of String) = New List(Of String)
        Dim colresult As List(Of Object) = New List(Of Object)
        Dim j As Integer = 0


        While myReader.Read
            If j = 0 Then
                For i = 0 To row.Count - 1 Step 1
                    colName.Add(myReader.GetName(i))
                    j = j + 1
                Next
                colresult.Add(colName)
            End If
            myReader.GetValues(row)
            colresult.Add(row.Clone)
        End While
        myReader.Close()
        Return colresult
    End Function

    ''' <summary>
    ''' Permet de lire un fichier Sql en enlevant les lignes contenant --
    ''' </summary>
    Public Function LectureFichierSql(ByVal fichier As String) As String
        Dim contenu As String = String.Empty
        Dim path As String = My.Settings.PATH
        Dim chemin As String = path & "\SQL_SCRIPTS\Strategie\" & fichier
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
            Return contenu
        Catch ex As Exception
            MsgBox("Une erreur est survenue au cours de l'accès en lecture du fichier " & chemin, "Erreur lors de l'ouverture d'un fichier")
            Return String.Empty
        End Try

        Return contenu
    End Function


#End Region

End Class