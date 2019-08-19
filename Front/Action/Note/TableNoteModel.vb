Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Action.Note
    Public Class TableNoteModel
        Implements INotifyPropertyChanged

        Private _noteViewModel As NoteViewModel
        Private _tableNote As TableNote
        Private _columns As New ObservableCollection(Of ColumnNoteModel)
        Private _otherColumns As New ObservableCollection(Of ColumnNoteModel)

        Public Sub New(ByVal noteViewModel As NoteViewModel, ByVal tableNote As TableNote)
            _tableNote = tableNote
            _noteViewModel = noteViewModel
        End Sub

        Public Sub New(ByVal noteViewModel As NoteViewModel, ByVal id As Integer, ByVal name As String, ByVal first As Boolean)
            Me.New(noteViewModel,
                   New TableNote(id, name))
        End Sub

        
#Region "Property"
        Public Property Id() As Integer
            Get
                Return _tableNote.Id
            End Get
            Set(ByVal value As Integer)
                _tableNote.Id = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _tableNote.Name
            End Get
            Set(ByVal value As String)
                _tableNote.Name = value
                OnPropertyChanged("Name")
            End Set
        End Property

        Public Property Columns() As ObservableCollection(Of ColumnNoteModel)
            Get
                Return _columns
            End Get
            Set(ByVal value As ObservableCollection(Of ColumnNoteModel))
                _columns = value
                NoteViewModel.sortColumns(_columns, "Position", True)
            End Set
        End Property

        Public ReadOnly Property OtherColumns() As ObservableCollection(Of ColumnNoteModel)
            Get
                Return _otherColumns
            End Get
        End Property
#End Region '!Property

#Region "Méthodes publiques"

        ''' <summary>
        ''' Retourne True si le name n'est pas dans Columns.
        ''' </summary>
        ''' <remarks>La casse n'est pas prise en compte.</remarks>
        Public Function uniqueNameInColumns(ByVal name As String) As Boolean
            name = name.ToUpper

            For Each col As ColumnNoteModel In Columns
                If col.Name.ToUpper = name Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' Met à jour le nom d'une colonne si elle n'existe pas déjà.
        ''' </summary>
        Sub update(ByVal column As ColumnNoteModel)
            ' Check existing name
            If Not existName(_noteViewModel.ColumnCollection, column) Then
                _noteViewModel.addColumn(column)
            End If

            ' Remove from otherColumns
            If existName(OtherColumns, column) Then
                _otherColumns.Remove(column)
            End If
        End Sub

        ''' <summary>
        ''' Ajoute une colonne à la table.
        ''' </summary>
        Sub AddColumn(ByVal column As ColumnNoteModel)
            Dim name = column.Name.ToUpper

            Columns.Add(column)
            ' Remove column from otherColumns if exists.
            For i = 0 To OtherColumns.Count - 1
                Dim col = OtherColumns(i)
                If col IsNot Nothing AndAlso col.Name.ToUpper = name Then
                    OtherColumns.Remove(col)
                    Exit For
                End If
            Next

            ' Ajoute la colonne à la liste des colonnes.
            _noteViewModel.addColumn(column)
        End Sub

        ''' <summary>
        ''' Vérifie si le nom d'une colonne existe dans la collection.
        ''' </summary>
        Public Function existName(ByVal columnList As ObservableCollection(Of ColumnNoteModel),
                                   ByVal column As ColumnNoteModel) As Boolean
            Dim name = column.Name.ToUpper

            For Each col As ColumnNoteModel In columnList
                If col IsNot Nothing AndAlso col.Name.ToUpper = name Then
                    Return True
                End If
            Next

            Return False
        End Function

#End Region ' !Méthodes publiques

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
End Namespace