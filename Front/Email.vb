Imports System.Net.Mail
Imports System.Net
Imports System.Net.Sockets



Public Class Email

    Private Declare Function InternetGetConnectedState Lib "wininet" (ByRef conn As Long, ByVal val As Long) As Boolean

    ''' <summary>
    ''' Vérifie la synthaxe d'un email
    ''' </summary>
    Public Function checkMail(ByVal email As String) As Boolean
        Dim pattern As String
        Dim res As Boolean = False
        pattern = "^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"
        If System.Text.RegularExpressions.Regex.IsMatch(email, pattern) Then
            res = True
        End If
        Return res
    End Function

    ''' <summary>
    ''' Vérifie la connnection à internet
    ''' </summary>
    Private Function checkConnection() As Boolean
        Dim Out As Integer
        Dim res As Boolean = False
        If InternetGetConnectedState(Out, 0) = True Then
            res = True
        End If
    End Function

    ''' <summary>
    ''' Envoie un email
    ''' </summary>
    Public Sub sendMail(ByVal sentTo As String, ByVal title As String, ByVal body As String)
        '465 25 587
        'Dim proxy As New TcpClient()
        'proxy.Connect("Proxy-Google.si2m.tec", "8080")
        'checkConnection()


        Dim Client As New SmtpClient("MX1.si2m.tec", 25)
        Dim from As New MailAddress("fgafront@si2m.fr", "FGA Front Admin")
        Dim sendto As New MailAddress("gcompagnon@federisga.fr", "GC")
        Dim message As New MailMessage(from, sendto)

        message.Body = "This is a test e-mail message sent by an application. "
        message.Subject = "Test Email using Credentials"

        'Dim myCreds As New NetworkCredential("TQA", "*T4rgetC", "MALAKOFFMEDERIC")
        'Dim myCredentialCache As New CredentialCache()

        Try
            'myCredentialCache.Add("MX1.si2m.tec", 25, "Basic", myCreds)
            'myCredentialCache.Add("MX1.si2m.tec", 25, "NTLM", myCreds)

            'Client.Credentials = myCredentialCache.GetCredential("MX1.si2m.tec", 25, "NTLM")
            client.Send(message)
            Console.WriteLine("Goodbye.")
        Catch e As Exception
            Console.WriteLine("Exception is raised. ")
            Console.WriteLine("Message: {0} ", e.Message)
        End Try



        'Try
        '    If checkMail(sentTo) Then
        '        'If checkConnection() Then
        '        Dim mail As New MailMessage
        '        Dim client As New SmtpClient("MX1.si2m.tec", 25)
        '        Dim sentFrom As String = "fgafront@si2m.fr"
        '        'Dim sentCode As String = "GOOGLE1APP"
        '        mail.From = New MailAddress(sentFrom)
        '        mail.To.Add(sentTo)
        '        mail.Body = body
        '        mail.Subject = title
        '        'Dim basicAuthenticationInfo As New System.Net.NetworkCredential(sentFrom, sentCode)
        '        Dim basicAuthenticationInfo As New System.Net.NetworkCredential(sentFrom, sentCode)
        '        client.Host = "MX1.si2m.tec"
        '        client.Port = 25
        '        client.UseDefaultCredentials = True
        '        'client.UseDefaultCredentials = False
        '        'client.Credentials = basicAuthenticationInfo
        '        'client.EnableSsl = True
        '        client.Send(mail)
        '        'Else
        '        'MessageBox.Show("L'ordinateur n'est pas connecter à internet", "Erreur email", MessageBoxButtons.OK, MessageBoxIcon.None)
        '        'End If
        '    Else
        '        MessageBox.Show("L'email est invalide " & sentTo, "Erreur email", MessageBoxButtons.OK, MessageBoxIcon.None)
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show("Une erreur inconnu s'est produite lors d'un envoie de mail vers " & sentTo, "Erreur email", MessageBoxButtons.OK, MessageBoxIcon.None)
        'End Try
    End Sub


End Class
