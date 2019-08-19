Namespace Utilities
    Public Class ExcelTool
        ' Used for timer.
        Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)
        Private Declare Function GetTickCount Lib "kernel32" () As Long

        ''' <summary>
        ''' By Chip Pearson, www.cpearson.com chip@cpearson.com
        ''' This function wait for file to close.
        ''' </summary>
        ''' <param name="FileName">The file to test.</param>
        ''' <param name="TestIntervalMilliseconds">The timer between two tests.</param>
        ''' <param name="TimeOutMilliseconds">The timeout after the function exits, no limit if negative.</param>
        Public Shared Sub WaitForFileClose(ByVal FileName As String, ByVal TestIntervalMilliseconds As Long, _
                                           Optional ByVal TimeOutMilliseconds As Long = -1,
                                           Optional ByVal doAction As System.Action = Nothing)
            Dim StartTickCount As Long
            Dim EndTickCount As Long
            Dim TickCountNow As Long
            Dim FileIsOpen As Boolean
            Dim Done As Boolean

            '''''''''''''''''''''''''''''''''''''''''''''''
            ' Before we do anything, first test if the file
            ' is open. If it is not, get out immediately.
            '''''''''''''''''''''''''''''''''''''''''''''''
            FileIsOpen = IsFileOpen(FileName:=FileName)
            If FileIsOpen = False Then
                Exit Sub
            End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' If TestIntervalMilliseconds <= 0, use a default value of 500.
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If TestIntervalMilliseconds <= 0 Then
                TestIntervalMilliseconds = 500
            End If

            '''''''''''''''''''''''''''''''
            ' Get the current tick count.
            '''''''''''''''''''''''''''''''
            StartTickCount = GetTickCount()
            If TimeOutMilliseconds <= 0 Then
                ''''''''''''''''''''''''''''''''''''''''
                ' If TimeOutMilliSeconds is negative,
                ' we'll wait forever.
                ''''''''''''''''''''''''''''''''''''''''
                EndTickCount = -1
            Else
                ''''''''''''''''''''''''''''''''''''''''
                ' If TimeOutMilliseconds > 0, get the
                ' tick count value at which we will
                ' give up on the wait and return
                ' false.
                ''''''''''''''''''''''''''''''''''''''''
                EndTickCount = StartTickCount + TimeOutMilliseconds
            End If

            Done = False
            Do Until Done
                ''''''''''''''''''''''''''''''''''''''''''''''''
                ' Test if the file is open. If it is closed,
                ' exit with a result of True.
                ''''''''''''''''''''''''''''''''''''''''''''''''
                If IsFileOpen(FileName:=FileName) = False Then
                    Exit Sub
                End If
                ''''''''''''''''''''''''''''''''''''''''''
                ' Go to sleep for TestIntervalMilliSeconds
                ' milliseconds.
                '''''''''''''''''''''''''''''''''''''''''
                TickCountNow = GetTickCount()
                If EndTickCount > 0 Then
                    '''''''''''''''''''''''''''''''''''''''''''''
                    ' If EndTickCount > 0, a specified timeout
                    ' value was provided. Test if we have
                    ' exceeded the time. Do one last test for
                    ' FileOpen, and exit.
                    '''''''''''''''''''''''''''''''''''''''''''
                    If TickCountNow >= EndTickCount Then
                        Exit Sub
                    Else
                        '''''''''''''''''''''''''''''''''''''''''
                        ' TickCountNow is less than EndTickCount,
                        ' so continue to wait.
                        '''''''''''''''''''''''''''''''''''''''''
                    End If
                Else
                    ''''''''''''''''''''''''''''''''
                    ' EndTickCount < 0, meaning wait
                    ' forever. Test if the file
                    ' is open. If the file is not
                    ' open, exit with a TRUE result.
                    ''''''''''''''''''''''''''''''''
                    If IsFileOpen(FileName:=FileName) = False Then
                        Exit Sub
                    End If

                End If
            Loop

            If doAction IsNot Nothing Then
                doAction()
            End If
        End Sub

        ''' <summary>
        ''' By Chip Pearson www.cpearson.com/excel chip@cpearson.com
        ''' This function determines whether a file is open by any program. Returns TRUE or FALSE
        ''' </summary>
        Private Shared Function IsFileOpen(ByVal FileName As String) As Boolean
            Dim FileNum As Integer
            Dim ErrNum As Integer

            On Error Resume Next   ' Turn error checking off.

            '''''''''''''''''''''''''''''''''''''''''''
            ' If we were passed in an empty string,
            ' there is no file to test so return FALSE.
            '''''''''''''''''''''''''''''''''''''''''''
            If FileName = vbNullString Then
                IsFileOpen = False
                Exit Function
            End If

            '''''''''''''''''''''''''''''''
            ' If the file doesn't exist,
            ' it isn't open so get out now.
            '''''''''''''''''''''''''''''''
            If Dir(FileName) = vbNullString Then
                IsFileOpen = False
                Exit Function
            End If
            ''''''''''''''''''''''''''
            ' Get a free file number.
            ''''''''''''''''''''''''''
            FileNum = FreeFile()
            '''''''''''''''''''''''''''
            ' Attempt to open the file
            ' and lock it.
            '''''''''''''''''''''''''''
            Err.Clear()
            ''''''''''''''''''''''''''''''''''''''
            ' Save the error number that occurred.
            ''''''''''''''''''''''''''''''''''''''
            ErrNum = Err.Number
            On Error GoTo 0        ' Turn error checking back on.
            ''''''''''''''''''''''''''''''''''''
            ' Check to see which error occurred.
            ''''''''''''''''''''''''''''''''''''
            Select Case ErrNum
                Case 0
                    '''''''''''''''''''''''''''''''''''''''''''
                    ' No error occurred.
                    ' File is NOT already open by another user.
                    '''''''''''''''''''''''''''''''''''''''''''
                    IsFileOpen = False

                Case 70
                    '''''''''''''''''''''''''''''''''''''''''''
                    ' Error number for "Permission Denied."
                    ' File is already opened by another user.
                    '''''''''''''''''''''''''''''''''''''''''''
                    IsFileOpen = True

                    '''''''''''''''''''''''''''''''''''''''''''
                    ' Another error occurred. Assume the file
                    ' cannot be accessed.
                    '''''''''''''''''''''''''''''''''''''''''''
                Case Else
                    IsFileOpen = True
            End Select
        End Function
    End Class
End Namespace