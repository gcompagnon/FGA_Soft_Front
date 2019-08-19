Imports System.ComponentModel
Imports WindowsApplication1.Watcher

Namespace Action.Note

    Public Class ColumnNoteModel

        Private _tableNoteModel As TableNoteModel
        Private _columnNote As ColumnNoteSupervisor

        Public Sub New(ByVal model As TableNoteModel, ByVal columnNote As ColumnNoteSupervisor)
            _tableNoteModel = model
            _columnNote = columnNote
        End Sub

        Public Sub New(ByVal model As TableNoteModel, ByVal id As Integer, ByVal name As String, ByVal is_activated As Boolean, ByVal is_note As Boolean, ByVal coef As Double?, ByVal position As Integer)
            Me.New(model, New ColumnNoteSupervisor(id, name, is_activated, is_note, coef, position))
        End Sub

#Region "Property"
        Public Property Id() As Integer
            Get
                Return _columnNote.Id
            End Get
            Set(ByVal value As Integer)
                _columnNote.Id = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _columnNote.Name
            End Get
            Set(ByVal value As String)
                If value <> Name Then
                    If _tableNoteModel.uniqueNameInColumns(value) Then
                        _columnNote.Name = value
                        _tableNoteModel.update(Me)
                    Else
                        MessageBox.Show("Le nom du critère existe déjà dans la table.")
                    End If
                End If
            End Set
        End Property

        Public Property Position As Integer
            Get
                Return _columnNote.Position
            End Get
            Set(ByVal value As Integer)
                _columnNote.Position = value
            End Set
        End Property

        Public Property Is_Activated() As Boolean
            Get
                Return _columnNote.Is_Activated
            End Get
            Set(ByVal value As Boolean)
                _columnNote.Is_Activated = value
            End Set
        End Property

        Public Property Is_Note() As Boolean
            Get
                Return _columnNote.Is_Note
            End Get
            Set(ByVal value As Boolean)
                _columnNote.Is_Note = value
            End Set
        End Property

        Public Property Coef As Double?
            Get
                Return _columnNote.Coef
            End Get
            Set(ByVal value As Double?)
                Dim newCoef As Double? = 0

                If value IsNot Nothing Then
                    Double.TryParse(value, newCoef)
                Else
                    newCoef = Nothing
                End If

                _columnNote.Coef = newCoef
            End Set
        End Property

        Public ReadOnly Property IsCoefCorrect As Boolean
            Get
                If Is_Note Then
                    Return Coef IsNot Nothing
                Else
                    Return True
                End If
            End Get
        End Property

        ReadOnly Property HasChanged As Boolean
            Get
                Return _columnNote.HasChanged
            End Get
        End Property

#End Region ' !Property

#Region "Méthodes publiques"
        ''' <summary>
        ''' Met à jour tous les champs
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub update()
            _columnNote.update()
        End Sub
#End Region ' !Méthodes publiques

    End Class
End Namespace