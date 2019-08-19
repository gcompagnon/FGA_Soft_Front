Namespace FileLink

    Public Class File

        Protected _name As String = ""
        Protected _source As String = ""
        Protected _url As String = ""
        Protected _description As String = ""
        Protected _linkedDate As String = ""
        Protected _id As Integer = -1
        Protected _tab As Integer = -1
        Private _p1 As Object
        Private _p2 As Object
        Private _p3 As Object
        Private _p4 As Object

        Sub New()
            Me.New("", "")
        End Sub

        Sub New(ByVal fullpath As String)
            Me.New(IO.Path.GetDirectoryName(fullpath),
                   IO.Path.GetFileName(fullpath))
        End Sub

        Sub New(ByVal url As String, ByVal name As String)
            _url = url
            _name = name
            _source = _name
        End Sub

        Sub New(ByVal url As String, ByVal name As String, ByVal description As String, ByVal id As Integer)
            Me.New(Nothing, url, name, description, id)
        End Sub

        Sub New(ByVal LinkedDate As String, ByVal url As String, ByVal name As String, ByVal description As String, ByVal id As Integer)
            Me.New(url, name)

            _linkedDate = LinkedDate
            _description = description
            _id = id
            _tab = Tab
        End Sub

        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Source As String
            Get
                Return _source
            End Get
            Set(ByVal value As String)
                _source = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property LinkedDate() As String
            Get
                Return _linkedDate
            End Get
            Set(ByVal value As String)
                _linkedDate = value
            End Set
        End Property

        Public Property Tab() As Integer
            Get
                Return _tab
            End Get
            Set(ByVal value As Integer)
                _tab = value
            End Set
        End Property

        Public Function getNameInDestPath(ByVal destDir As String) As String
            Dim destPath As String = destDir & "\" & name
            Dim otherName As String

            While IO.File.Exists(destPath)
                otherName = findAlternativeName(destDir, Name)
                Dim dialogresult As DialogResult = MessageBox.Show("Le fichier """ & Name & """ existe déjà dans le répertoire de destination. Le nouveau nom du fichier sera """ & otherName & """. Voulez-vous conserver ce nouveau nom ?",
                                                                    "Le fichier existe déjà",
                                                                    MessageBoxButtons.YesNoCancel,
                                                                    MessageBoxIcon.Exclamation)

                If dialogresult = Windows.Forms.DialogResult.No Then
                    Dim newname As String = ""
                    newname = Interaction.InputBox("Veuillez entrer le nouveau nom du fichier",
                                                    "Changement de nom",
                                                    otherName)
                    If newname = "" Then
                        Return ""
                    End If
                    otherName = newname
                ElseIf dialogresult = Windows.Forms.DialogResult.Cancel Then
                    Return ""
                End If

                Name = otherName
                destPath = destDir & "\" & Name
            End While

            Return name
        End Function

        Private Function findAlternativeName(ByVal Dir As String, ByVal originName As String) As String
            Dim ext As String = IO.Path.GetExtension(originName)
            originName = IO.Path.GetFileNameWithoutExtension(originName)
            Dim alternativeName As String = originName
            Dim index As Integer = 1


            While IO.File.Exists(Dir & "\" & alternativeName & ext)
                index += 1
                alternativeName = originName & "(" & index & ")"
            End While

            Return alternativeName & ext
        End Function

        Private Function getFileNameWithoutVersion(ByVal name As String) As String
            name = IO.Path.GetFileNameWithoutExtension(name)
            Dim startindex As Integer = name.LastIndexOf("("c)
            Dim index As Integer = index

            While index < name.Length AndAlso name(index) >= "0"c AndAlso name(index) <= "9"c
                index += 1
            End While

            If index < name.Length AndAlso index - startindex > 1 AndAlso name(index) = ")"c Then
                Return name.Substring(0, startindex - 1)
            End If

            Return name
        End Function

        Public Sub deleteFileLink(ByVal stock As StockViewModel)
            Dim co As New Connection()
            Dim id_icb As Integer = -1
            Dim id_fga As Integer = -1
            Dim id_val As Integer = -1

            Select Case stock.Type
                Case StockType.VALEUR
                    id_val = stock.Id
                Case StockType.FGA
                    id_fga = stock.Id
                Case StockType.ICB
                    id_icb = stock.Id
                Case Else
                    Return
            End Select

            co.ProcedureStockée("ACT_Del_File_Link",
                                New List(Of String)({"@id_file", "@id_sector_icb", "@id_sector_fga", "@id_value"}),
                                New List(Of Object)({Id, id_icb, id_fga, id_val}))


            ' check if file is within default directory
            If Url.StartsWith(My.Resources.FILELINK_BASEDIR) Then
                ' check if file is not linked anymore.
                If co.SelectWhere("ACT_FILE_LINK", "id_file", "id_file", Id).Count = 0 Then
                    moveFileToTrash()
                End If
            End If
        End Sub

        Public Sub moveFileToTrash()
            Dim destdir As String = My.Resources.FILELINK_BASEDIR & "\Trash"
            Dim srcPath = Url & "\" & Name
            Dim destPath = destdir & "\" & getNameInDestPath(destdir)

            If Not IO.Directory.Exists(destdir) Then
                IO.Directory.CreateDirectory(destdir)
            End If

            Try
                IO.File.Move(srcPath, destPath)
            Catch ex As Exception
            End Try

        End Sub
    End Class

End Namespace
