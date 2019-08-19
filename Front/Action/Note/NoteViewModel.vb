Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Action.Note
    Public Class NoteViewModel
        Implements INotifyPropertyChanged

        Private co As New Connection()
        Private _tables As New ObservableCollection(Of TableNoteModel)
        Private _columnDico As New Dictionary(Of String, ColumnSlot)

        Public Sub New()
            loadTables()
        End Sub

#Region "Properties"
        Property SelectedTable As TableNoteModel

        Property Tables As ObservableCollection(Of TableNoteModel)
            Get
                Return _tables
            End Get
            Set(ByVal value As ObservableCollection(Of TableNoteModel))
                _tables = value
                OnPropertyChanged("Tables")
            End Set
        End Property

        ReadOnly Property ColumnCollection As ObservableCollection(Of ColumnNoteModel)
            Get
                Dim collection As New ObservableCollection(Of ColumnNoteModel)
                For Each col In _columnDico.Values
                    collection.Add(col.Column)
                Next

                Return collection
            End Get
        End Property

        Private Property ColumnToRemove As New List(Of ColumnSlot)

#End Region ' !Properties

#Region "Méthodes publiques"
        ''' <summary>
        ''' Charge les tables et leurs colonnes
        ''' </summary>
        Private Sub loadTables()
            Dim list As List(Of TableNote)
            Dim sql As String

            ' Fill list with tables in BDD
            sql = "SELECT id_table as Id, nom as Name"
            sql = sql & " FROM ACT_NOTE_TABLE"

            list = co.sqlToListObject(sql,
                                      Function() New TableNote)

            ' Add corresponding columns to each table
            For Each t As TableNote In list
                Dim tableModel As TableNoteModel = New TableNoteModel(Me, t)
                fillColumnsInTable(tableModel)
                Tables.Add(tableModel)
            Next

            ' Create table for the other FGAsectors
            sql = "SELECT libelle as Name"
            sql = sql & " FROM ACT_FGA_SECTOR"

            list = co.sqlToListObject(sql,
                                      Function() New TableNote)

            For Each t As TableNote In list
                Dim found As Boolean = False
                Dim name As String = t.Name.ToUpper

                For Each table As TableNoteModel In Tables
                    If table.Name.ToUpper = name Then
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then
                    Tables.Add(New TableNoteModel(Me, t))
                End If
            Next

            NoteViewModel.sortColumns(Tables, "Name", True)
        End Sub

        ''' <summary>
        ''' Récupére les colonnes d'une table donnée.
        ''' </summary>
        Private Sub fillColumnsInTable(ByVal table As TableNoteModel)
            Dim list As List(Of ColumnNoteSupervisor)
            Dim sql As String

            sql = "SELECT id_column as Id, nom as Name, is_activated as Is_Activated, is_note as Is_Note, coef as Coef, position as Position"
            sql = sql & " FROM ACT_NOTE_COLUMN "
            sql = sql & " WHERE id_table = " & table.Id

            list = co.sqlToListObject(sql,
                                      Function() New ColumnNoteSupervisor)

            For Each c As ColumnNoteSupervisor In list
                table.AddColumn(New ColumnNoteModel(table, c))
            Next
        End Sub

        ''' <summary>
        ''' Remplit la liste de colonne avec les différentes colonnes contenues dans toutes les tables
        ''' </summary>
        Public Sub fillColumnList()
            For Each table As TableNoteModel In Tables
                For Each colonne As ColumnNoteModel In table.Columns
                    addColumn(colonne)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Met à jour OtherColumns à partir de ColumnList en enlevant les colonnes de la table actuelle.
        ''' </summary>
        ''' <remarks>OtherColumns est trié par ordre alphabétique.</remarks>
        Sub fillOtherColumns(ByVal table As TableNoteModel)
            Dim newList As New ObservableCollection(Of ColumnNoteModel)

            For Each colGeneral As ColumnNoteModel In ColumnCollection
                Dim is_new = True

                ' Ignore NULL column
                If colGeneral Is Nothing Then
                    Continue For
                End If

                ' Ignore columns in secteur
                For Each colTable As ColumnNoteModel In table.Columns
                    If colGeneral.Name.ToUpper = colTable.Name.ToUpper Then
                        is_new = False
                        Exit For
                    End If
                Next

                ' Ignore columns already in Othercolumns.
                If is_new AndAlso Not table.existName(newList, colGeneral) Then
                    newList.Add(colGeneral)
                End If
            Next

            ' Sort OtherColumns
            NoteViewModel.sortColumns(newList, "Name", True)
            table.OtherColumns.Clear()
            table.OtherColumns.Add(Nothing)

            For Each col As ColumnNoteModel In newList
                table.OtherColumns.Add(col)
            Next
        End Sub

        ''' <summary>
        ''' Définit la table actuellement sélectionnée.
        ''' </summary>
        Public Sub selectTable(ByVal name As String)
            For Each tab As TableNoteModel In Tables
                If tab.Name = name Then
                    SelectedTable = tab
                    Return
                End If
            Next
        End Sub


        ''' <summary>
        ''' Get Bit From Boolean (True = 1)
        ''' </summary>
        Private Function getBit(ByVal bool As Boolean) As Integer
            If bool Then
                Return 1
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' Ajoute une colonne à la liste de colonnes.
        ''' </summary>
        Sub addColumn(ByVal column As ColumnNoteModel)
            If _columnDico.ContainsKey(column.Name) Then
                _columnDico(column.Name).Occurence += 1
            Else
                _columnDico.Add(column.Name, New ColumnSlot(column, 1))
            End If
        End Sub

        ''' <summary>
        ''' Supprime une colonne d'une table en mettant à jour les listes de colonnes.
        ''' </summary>
        Sub removeColumn(ByVal table As TableNoteModel, ByVal column As ColumnNoteModel)
            If _columnDico.ContainsKey(column.Name) Then
                ' Add column To removed List
                _columnDico(column.Name).Table = table
                Me.ColumnToRemove.Add(_columnDico(column.Name))

                ' update ColumnDico and add column to OtherList if still existing.
                _columnDico(column.Name).Occurence -= 1
                If _columnDico(column.Name).Occurence < 1 Then
                    _columnDico.Remove(column.Name)
                Else
                    table.OtherColumns.Add(column)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Enregistre tous les changements des tables et des colonnes.
        ''' </summary>
        Sub saveAll()
            ' Remove deleted columns
            For Each col In Me.ColumnToRemove
                removeFromDB(col)
            Next

            ' Save changes
            For Each t As TableNoteModel In Tables
                ' save unexisting table
                If t.Id = 0 Then
                    co.Insert("ACT_NOTE_TABLE",
                              New List(Of String)({"nom"}),
                              New List(Of Object)({t.Name}))
                    t.Id = co.SelectWhere("ACT_NOTE_TABLE",
                                          "id_table",
                                          "nom",
                                          t.Name).FirstOrDefault
                End If

                ' save columns
                For Each col As ColumnNoteModel In t.Columns
                    If col.Name = "" Then
                        ' Do not save on empty name.
                        MessageBox.Show("Un critère sans nom ne peut pas être sauvegardé dans le secteur """ & t.Name & """")
                        Continue For
                    End If
                    If col.Id = 0 Then
                        ' New column
                        co.Insert("ACT_NOTE_COLUMN",
                                  New List(Of String)({"id_table", "nom", "is_activated", "is_note", "coef", "position"}),
                                  New List(Of Object)({t.Id, col.Name, getBit(col.Is_Activated), getBit(col.Is_Note), col.Coef, col.Position}))
                        col.Id = co.SelectDistinctWheres("ACT_NOTE_COLUMN",
                                                         "id_column",
                                                         New List(Of String)({"id_table", "nom"}),
                                                         New List(Of Object)({t.Id, col.Name})).FirstOrDefault
                    Else
                        If col.HasChanged Then
                            ' Update column
                            co.Update("ACT_NOTE_COLUMN",
                                      New List(Of String)({"id_table", "nom", "is_activated", "is_note", "coef", "position"}),
                                      New List(Of Object)({t.Id, col.Name, getBit(col.Is_Activated), getBit(col.Is_Note), col.Coef, col.Position}),
                                      "id_column",
                                      col.Id)
                        End If
                    End If

                    col.update()
                Next
            Next
        End Sub

        ''' <summary>
        ''' Remove column and its record from database.
        ''' </summary>
        Private Sub removeFromDB(ByVal columnSlot As ColumnSlot)
            co.DeleteWheres("ACT_NOTE_RECORD",
                            New List(Of String)({"id_column"}),
                            New List(Of Object)({columnSlot.Column.Id}))

            co.DeleteWheres("ACT_NOTE_COLUMN",
                            New List(Of String)({"id_table", "id_column"}),
                            New List(Of Object)({columnSlot.Table.Id, columnSlot.Column.Id}))
        End Sub
#End Region ' !Méthodes publiques

#Region "Méthodes statiques"
        ''' <summary>
        ''' Tri sur une collection en fonction d'une propriété de T.
        ''' </summary>
        ''' <param name="property_name">nom de la propriété</param>
        ''' <param name="ascending">ordre du tri</param>
        Public Shared Sub sortColumns(Of T)(ByVal collection As System.Collections.ObjectModel.Collection(Of T),
                                      ByVal property_name As String,
                                      ByVal ascending As Boolean)

            If ascending Then
                For Each col In (From c In collection Order By c.GetType().GetProperty(property_name).GetValue(c, Nothing) Ascending)
                    collection.Remove(col)
                    collection.Add(col)
                Next
            Else
                For Each col In (From c In collection Order By c.GetType().GetProperty(property_name).GetValue(c, Nothing) Descending)
                    collection.Remove(col)
                    collection.Add(col)
                Next
            End If

        End Sub
#End Region ' !Méthodes statiques

#Region "Event"
        Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            If Not String.IsNullOrEmpty(propertyName) Then
                RaiseEvent PropertyChanged(Me,
                          New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
#End Region ' !Event

    End Class

    Class ColumnSlot

        Property Column As ColumnNoteModel
        Property Occurence As Integer
        Property Table As TableNoteModel

        Sub New(ByVal column As ColumnNoteModel, ByVal occurence As Integer)
            Me.Column = column
            Me.Occurence = occurence
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is ColumnSlot Then
                Return CType(obj, ColumnSlot).Column.Name = Me.Column.Name
            ElseIf TypeOf obj Is ColumnNoteModel Then
                Return CType(obj, ColumnNoteModel).Name = Me.Column.Name
            ElseIf TypeOf obj Is ColumnNoteSupervisor Then
                Return CType(obj, ColumnNoteSupervisor).Name = Me.Column.Name
            End If

            Return False
        End Function
    End Class
End Namespace
