Imports System.ComponentModel

Namespace Watcher

    Public MustInherit Class Supervisor
        Implements INotifyPropertyChanged

        Private _dicoWatchers As New Dictionary(Of String, IWatcher)

        Public Sub New()
            initWatchers()
        End Sub

        ''' <summary>
        ''' Instancie le supervisor avec un seul watcher.
        ''' </summary>
        Public Sub New(ByVal watch_name, ByVal value)
            initWatcher(watch_name, value)
        End Sub

#Region "Propriétés"
        ''' <summary>
        ''' Vérifie si un changement à eu lieu parmi les watchers du supervisor.
        ''' </summary>
        Public ReadOnly Property HasChanged
            Get
                Dim has_changed = False

                For Each watch As IWatcher In _dicoWatchers.Values
                    If watch.HasChanged Then
                        has_changed = True
                    End If
                Next

                Return has_changed
            End Get
        End Property

#End Region ' !Propriétés

#Region "Methodes publiques"
        ''' <summary>
        ''' Met à jour les changements sur tous les watchers.
        ''' </summary>
        Public Sub update()
            For Each watch As IWatcher In _dicoWatchers.Values
                watch.update()
            Next
        End Sub
#End Region ' !Methodes publiques

#Region "Methodes protégées"
        ''' <summary>
        ''' Méthode d'initialisation des watchers de la classe héritées
        ''' </summary>
        Protected Overloads Sub initWatchers(ByVal ParamArray params() As KeyValuePair(Of String, Object))
            For i = 0 To UBound(params)
                initWatcher(params(i).Key, params(i).Value)
            Next
        End Sub

        ''' <summary>
        ''' Ajoute un watcher au supervisor
        ''' </summary>
        Private Sub initWatcher(Of T)(ByVal watch_name As String, ByVal value As T)
            _dicoWatchers.Add(watch_name, New Watcher(Of T)(value))
            _dicoWatchers(watch_name).update()
        End Sub

        ''' <summary>
        ''' Récupère une donnée contenue dans un watcher
        ''' </summary>
        Protected Function getWatcher(Of T)(ByVal watch_name As String) As T
            If Not _dicoWatchers.ContainsKey(watch_name) Then
                ' Add new watcher
                initWatcher(Of T)(watch_name, Nothing)
            End If

            Return CType(_dicoWatchers(watch_name), Watcher(Of Object)).Data
        End Function

        ''' <summary>
        ''' Met à jour le contenu d'un watcher et appelle OnPropertyChanged
        ''' </summary>
        Protected Sub setWatcher(Of T)(ByVal watch_name As String, ByVal value As T)
            Dim watcher As IWatcher = _dicoWatchers(watch_name)

            If Not _dicoWatchers.ContainsKey(watch_name) Then
                ' Add new watcher
                initWatcher(Of T)(watch_name, value)
            Else
                Try
                    CType(_dicoWatchers(watch_name), Watcher(Of Object)).Data = value
                    OnPropertyChanged(watch_name)
                Catch ex As Exception

                End Try
            End If
        End Sub

        ''' <summary>
        ''' Met à jour le watcher spécifié.
        ''' </summary>
        Protected Sub updateWatcher(ByVal watch_name)
            _dicoWatchers(watch_name).update()
        End Sub

        ''' <summary>
        ''' Met à jour le contenu d'un watcher et met à jour ce changement.
        ''' </summary>
        Protected Sub updateWatcher(Of T)(ByVal watch_name As String, ByVal value As T)
            setWatcher(watch_name, value)
            _dicoWatchers(watch_name).update()
        End Sub

        ''' <summary>
        ''' Return weither the watcher value has changed.
        ''' </summary>
        Protected Function WatcherHasChanged(ByVal watch_name As String) As Boolean
            Return _dicoWatchers(watch_name).HasChanged
        End Function
#End Region ' !Methodes protégées

#Region "Events"
        Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            If Not String.IsNullOrEmpty(propertyName) Then
                RaiseEvent PropertyChanged(Me,
                          New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
#End Region ' !Events

    End Class

End Namespace