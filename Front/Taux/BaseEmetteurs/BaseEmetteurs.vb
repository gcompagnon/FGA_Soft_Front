Imports System.IO
Imports System.Reflection
Namespace Taux.BaseEmetteurs
    Public Class BaseEmetteurs
        Implements PrintPreviewForm

        Dim front As Connection = New Connection()
        Dim omega As ConnectionOmega = New ConnectionOmega()
        Dim dossier As Dossier = New Dossier()
        Dim fi As Fichier = New Fichier()
        Dim log As Log = New Log()
        Dim oldName As String
        Private MyDataGridViewPrinter As New DataGridViewPrinter.DataGridViewPrinter(Me)

        ''' <summary>
        ''' load de l'ihm
        ''' </summary>
        Public Sub BaseEmetteurs_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Tentative de connnection
            front.ToConnectBase()
            omega.ToConnectOmega()

            TChemin.Text = front.SelectDistinctSimple("TX_RACINE", "chemin").First
            CbDataGrid.Items.Clear()
            CbDataGrid.Items.Add("Pays")
            CbDataGrid.Items.Add("Signature")
            CbDataGrid.Items.Add("Sous secteur")
            CbDataGrid.SelectedIndex = 1
            CEmetteur.DataSource = front.SelectDistinctSimple("TX_EMETTEUR_FICHIER", "libelle")


            Rafraichir()

            BChemin.Enabled = False
            If Utilisateur.admin = False Then
                BAjouter.Enabled = False
                BCreer.Enabled = False
            End If

        End Sub

        ''' <summary>
        ''' This function will get the Folderdialog as parameter and return the Selected
        ''' Folders UNC path. 
        ''' Ex: \\Server1\TestFolder
        ''' </summary>
        ''' <param name="oFolderBrowserDialog"></param>
        ''' <returns>it will return the Selected Folders UNC Path</returns>
        ''' <remarks></remarks>
        Public Shared Function GetNetworkFolders(ByVal oFolderBrowserDialog As FolderBrowserDialog) _
                                As String
            'Get type of Folder Dialog bog
            Dim type As Type = oFolderBrowserDialog.[GetType]
            'Get Fieldinfo for rootfolder.
            Dim fieldInfo As Reflection.FieldInfo = type.GetField("rootFolder", _
                            BindingFlags.NonPublic Or BindingFlags.Instance)
            'Now set the value for Folder Dialog using DirectCast
            '18 = Network Neighborhood is the root
            'fieldInfo.SetValue(oFolderBrowserDialog, DirectCast(18, Environment.SpecialFolder))
            'if user click on Ok, then it will return the selected folder.
            'otherwise return the blank string.
            If oFolderBrowserDialog.ShowDialog() = DialogResult.OK Then
                Return oFolderBrowserDialog.SelectedPath
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' BChemin : Changer le chemin de la racine de la BDD
        ''' </summary>
        Private Sub BChemin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BChemin.Click
            TChemin.Text = GetNetworkFolders(objFolderDialog)

            If String.IsNullOrEmpty(TChemin.Text) = False Then
                'check si annulé
                If (front.SelectDistinctSimple("TX_RACINE", "chemin").First <> TChemin.Text) Then
                    'Déplacer le dossier entier
                    dossier.CopyDirectory(front.SelectDistinctSimple("TX_RACINE", "chemin").First, TChemin.Text)
                    'Supprimer le dossier ancien
                    dossier.SupprimerDossier(front.SelectDistinctSimple("TX_RACINE", "chemin").First)
                    'Sauver le nouveau chemin dans TX_RACINE
                    Dim name As List(Of String) = New List(Of String)()
                    Dim donnee As List(Of Object) = New List(Of Object)()
                    donnee.Clear()
                    name.Clear()
                    name.Add("chemin")
                    donnee.Add(TChemin.Text)
                    front.Update("TX_RACINE", name, donnee, "chemin", front.SelectDistinctSimple("TX_RACINE", "chemin").First)
                    log.Log(ELog.Information, "BChemin_Click", "Le chemin de la base " & TChemin.Text & " a été changé dans TX_RACINE !")

                Else
                    MessageBox.Show("Le nouveau chemin est le même que l'ancien", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Tlibelle : Load de la frame
        ''' </summary>
        Private Sub Tlibelle_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TSignature.TextChanged
            Dim name As List(Of String) = New List(Of String)()
            Dim donnee As List(Of Object) = New List(Of Object)()
            name.Add("@text")
            donnee.Add(" '%" & TSignature.Text & "%'")
            CbLibelleBase.DataSource = front.ProcedureStockéeList("GetSignatureLibelle", name, donnee)
            CbLibelleOmega.DataSource = front.ProcedureStockéeList("GetSignatureLibelleOmega", name, donnee)
        End Sub

        ''' <summary>
        ''' BCharger : charge une signature de TX_SIGNATURE
        ''' </summary>
        Private Sub BCharger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCharger.Click
            If String.IsNullOrEmpty(CbLibelleBase.Text) = False Then
                If front.SelectDistinctWhere("TX_SIGNATURE", "libelle", "libelle", CbLibelleBase.Text).Count = 1 Then
                    Dim modif As New ModificationSignature
                    modif.maj_libelle(CbLibelleBase.Text)
                    Me.Close()
                    modif.Show()
                Else
                    MessageBox.Show("Le libelle " & CbLibelleBase.Text & " n'existe pas dans la table TX_SIGNATURE !", " ", MessageBoxButtons.OK, MessageBoxIcon.None)
                End If
            Else
                MessageBox.Show("Aucun libellé est renseigné !", " ", MessageBoxButtons.OK, MessageBoxIcon.None)
            End If
        End Sub

        ''' <summary>
        ''' Preview de TOUTES les impressions
        ''' </summary>
        Public Sub Preview(ByVal PrintCenterReportOnPage As Boolean, ByVal PrintFont As Font, ByVal PrintFontColor As Color) Implements PrintPreviewForm.Preview
            Dim dataChoice As DataGridView = Nothing
            Select Case CbDataGrid.Text
                Case "Pays"
                    dataChoice = DataGridPays
                Case "Sous secteur"
                    dataChoice = DataGridSousSecteur
                Case "Signature"
                    dataChoice = DataGridSignature
                Case Else
                    MsgBox("Cette datagrid n'existe pas")
            End Select

            MyDataGridViewPrinter.Init_Parameters(dataChoice, myPrintDocument, PrintCenterReportOnPage, True, myPrintDocument.DocumentName, New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, PrintFont, PrintFontColor, True)
            'If MyDataGridViewPrinter.SetupThePrinting(myDataGridView, MyPrintDocument) Then 'Me.Text) Then
            Dim MyPrintPreviewDialog As New PrintPreviewDialog()
            MyPrintPreviewDialog.Document = myPrintDocument
            MyPrintPreviewDialog.ShowDialog()
            'End If

        End Sub

        ''' <summary>
        ''' Impression
        ''' </summary>
        Private Sub myPrintDocument_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles myPrintDocument.PrintPage
            Dim more As Boolean = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more = True Then
                e.HasMorePages = True
            End If
        End Sub

        ''' <summary>
        ''' Resize de l'ihm
        ''' </summary>
        Private Sub BaseEmetteurs_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Dim Feuille As Form
            Feuille = Me
            Static Longueur As Long
            Static Hauteur As Long
            Dim PropLongueur As Single
            Dim PropHauteur As Single
            If ((Longueur > 0) And (Hauteur > 0)) Then
                PropLongueur = Feuille.Width / Longueur
                PropHauteur = Feuille.Height / Hauteur
                Dim Ctrl As Control
                On Error Resume Next
                For Each Ctrl In Feuille.Controls
                    Ctrl.Left = CInt(Ctrl.Left * PropLongueur)
                    Ctrl.Top = CInt(Ctrl.Top * PropHauteur)
                    Ctrl.Width = CInt(Ctrl.Width * PropLongueur)
                    Ctrl.Height = CInt(Ctrl.Height * PropHauteur)
                Next
                On Error GoTo 0
            End If
            Longueur = Feuille.Width
            Hauteur = Feuille.Height
        End Sub

        ''' <summary>
        ''' Bouton BRafraichir : check différence base et disc
        ''' </summary>
        Private Sub BRafraichir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRafraichirFichier.Click
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            Rafraichir()
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        ''' <summary>
        ''' BAjouter : ajoute les fichiers non enregistrés en base
        ''' </summary>
        Private Sub BAjouter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAjouter.Click
            Dim libelle As List(Of Object) = New List(Of Object)()
            Dim colNames As List(Of String) = New List(Of String)()
            Dim donnees As List(Of Object) = New List(Of Object)()

            For i = 0 To DataGridNewFile.Rows.Count - 1
                If libelle.Contains(DataGridNewFile.Rows(i).Cells(0).Value.ToString) = False Then
                    libelle.Add(DataGridNewFile.Rows(i).Cells(0).Value.ToString)
                End If
                Dim code As String = front.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", DataGridNewFile.Rows(i).Cells(0).Value).First
                colNames.Add("code_signature")
                colNames.Add("nom")
                colNames.Add("date")
                colNames.Add("id_utilisateur")
                colNames.Add("id_emetteur_fichier")
                colNames.Add("note")
                donnees.Add(code)
                donnees.Add(DataGridNewFile.Rows(i).Cells(1).Value)
                donnees.Add(DateTime.Now)
                donnees.Add(Utilisateur.login)
                donnees.Add(front.SelectDistinctWhere("TX_EMETTEUR_FICHIER", "id", "libelle", DataGridNewFile.Rows(i).Cells(2).Value).First)
                donnees.Add(DataGridNewFile.Rows(i).Cells(3).Value)
                front.Insert("TX_FICHIER", colNames, donnees)
                colNames.Clear()
                donnees.Clear()
            Next i

            If CbEmail.Checked = True And libelle.Count() > 0 Then
                'envoyer un email avec les modif libelle
                Dim sentTo As String = "bauge@federisga.fr"
                Dim title As String = "Ajout de fichier(s) dans la base émetteur"
                Dim body As String = "Ajout de nouveau(x) fichier(s) pour la/les signature(s) : "
                For i = 0 To libelle.Count - 1
                    body = body & libelle(i).ToString
                    If i <> libelle.Count - 1 Then
                        body = body & ", "
                    End If
                Next

                Dim email As Email = New Email()
                'email.sendMail(sentTo, title, body)
            End If

            'Suprimmer les lignes du tab
            For ligne As Integer = 0 To DataGridNewFile.Rows.Count - 1
                DataGridNewFile.Rows.RemoveAt(0)
            Next

            DataGridNewFile.ClearSelection()
            DataGridNewFile.DataSource = Nothing

            'Binder DataGridFile = fichier ajouter depuis la dernière déconexion de l'utilisateur
            DataGridFile.ClearSelection()
            Dim name As List(Of String) = New List(Of String)()
            name.Add("@login")
            Dim donnee As List(Of Object) = New List(Of Object)()
            donnee.Add(Utilisateur.login)
            DataGridFile.DataSource = front.LoadDataGridByProcedureStockée(front.ProcedureStockéeForDataGrid("GetDataGridFileRefresh", name, donnee))

        End Sub

        ''' <summary>
        ''' DataGridFile : ouvre fichier archivé
        ''' </summary>
        Private Sub DataGridFile_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridFile.DoubleClick
            Dim cheminFichier = front.SelectSimple("TX_RACINE", "chemin")(0) & "\" & DataGridFile.CurrentRow.Cells.Item(0).Value.ToString & "\" & DataGridFile.CurrentRow.Cells.Item(2).Value.ToString
            If (fi.Existe(cheminFichier)) Then
                Process.Start(cheminFichier)
            Else
                MessageBox.Show("Le fichier " & cheminFichier & " n'arrive pas a être ouvert !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' DataGridNewFile : ouvre fichier archivé
        ''' </summary>
        Private Sub DataGridNewFile_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridNewFile.DoubleClick
            Dim cheminFichier = front.SelectSimple("TX_RACINE", "chemin").First & "\" & DataGridNewFile.CurrentRow.Cells.Item(0).Value.ToString & "\" & DataGridNewFile.CurrentRow.Cells.Item(1).Value.ToString
            If (fi.Existe(cheminFichier)) Then
                Process.Start(cheminFichier)
            Else
                MessageBox.Show("Le fichier " & cheminFichier & " n'arrive pas a être ouvert !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' Rafraichie toutes les datagrilles de l'ihm
        ''' </summary>
        Public Sub Rafraichir()
            Dim nbMessages As Integer = 0

            'Binder DataGridFile = fichier ajouter depuis la dernière déconexion de l'utilisateur
            DataGridFile.ClearSelection()
            Dim name As List(Of String) = New List(Of String)()
            name.Add("@login")
            Dim donnee As List(Of Object) = New List(Of Object)()
            donnee.Add(Utilisateur.login)
            DataGridFile.DataSource = front.LoadDataGridByProcedureStockée(front.ProcedureStockéeForDataGrid("GetDataGridFileRefresh", name, donnee))
            DataGridFile.Columns(0).ReadOnly = True
            DataGridFile.Columns(1).ReadOnly = True
            DataGridFile.Columns(3).ReadOnly = True
            DataGridFile.Columns(5).ReadOnly = True

            'Binder DataGridNewFile = différence entre base et window
            For ligne As Integer = 0 To DataGridNewFile.Rows.Count - 1
                DataGridNewFile.Rows.RemoveAt(0)
            Next
            Dim base As String = front.SelectDistinctSimple("TX_RACINE", "chemin").First
            For Each code In front.SelectDistinctSimple("TX_SIGNATURE", "code")
                Dim libelleDuCode As String = front.SelectDistinctWhere("TX_SIGNATURE", "libelle", "code", code).First.ToString
                Dim fichiers As System.Array = Array.CreateInstance(GetType(String), 0)
                Dim cheminComplet As String = base & "\" & libelleDuCode.Replace("/", "")


                Try
                    fichiers = Directory.GetFiles(cheminComplet, "*.*", SearchOption.AllDirectories)
                Catch ex As Exception
                    If (nbMessages < 4) Then
                        nbMessages = nbMessages + 1
                        MessageBox.Show("Le repertoire: " & cheminComplet & " n'arrive pas a être ouvert !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If

                End Try

                Dim fichierBase As List(Of Object) = front.SelectDistinctWhere("TX_FICHIER", "nom", "code_signature", code)
                For Each fichier In fichiers
                    'On compare les différences entre la base et window
                    If fichierBase.Contains(New IO.FileInfo(fichier).Name()) = False Then
                        DataGridNewFile.Rows.Add(libelleDuCode, New IO.FileInfo(fichier).Name(), "", "")
                    End If
                Next
            Next

            'Binder DataGridSignature = derniere recommandation pour chaque signature
            name.Clear()
            donnee.Clear()
            DataGridSignature.ClearSelection()
            DataGridSignature.DataSource = front.LoadDataGridByProcedureStockée(front.ProcedureStockéeForDataGrid("DataGridSignature", name, donnee))
            If DataGridSignature.Rows.Count <> 0 Then
                DataGridSignature.Columns(1).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSignature.Columns(2).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSignature.Columns(3).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSignature.Columns(4).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSignature.Columns(5).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSignature.Columns(6).HeaderCell.Style.BackColor = Color.Aqua

                DataGridSignature.Columns(8).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(9).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(10).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(11).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(12).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(13).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSignature.Columns(14).HeaderCell.Style.BackColor = Color.PaleGreen
            End If

            'Binder DataGridSousSecteur = derniere recommandation pour chaque sous secteur
            DataGridSousSecteur.ClearSelection()
            DataGridSousSecteur.DataSource = front.LoadDataGridByProcedureStockée(front.ProcedureStockéeForDataGrid("DataGridSousSecteur", name, donnee))
            If DataGridSousSecteur.Rows.Count <> 0 Then
                DataGridSousSecteur.Columns(2).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSousSecteur.Columns(3).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSousSecteur.Columns(4).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSousSecteur.Columns(5).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSousSecteur.Columns(6).HeaderCell.Style.BackColor = Color.Aqua
                DataGridSousSecteur.Columns(7).HeaderCell.Style.BackColor = Color.Aqua

                DataGridSousSecteur.Columns(9).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSousSecteur.Columns(10).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSousSecteur.Columns(11).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSousSecteur.Columns(12).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSousSecteur.Columns(13).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridSousSecteur.Columns(14).HeaderCell.Style.BackColor = Color.PaleGreen
            End If

            'Binder DataGridPays = derniere recommandation pour chaque Pays
            DataGridPays.ClearSelection()
            DataGridPays.DataSource = front.LoadDataGridByProcedureStockée(front.ProcedureStockéeForDataGrid("DataGridPays", name, donnee))
            If DataGridPays.Rows.Count <> 0 Then
                DataGridPays.Columns(1).HeaderCell.Style.BackColor = Color.Aqua
                DataGridPays.Columns(2).HeaderCell.Style.BackColor = Color.Aqua
                DataGridPays.Columns(3).HeaderCell.Style.BackColor = Color.Aqua
                DataGridPays.Columns(4).HeaderCell.Style.BackColor = Color.Aqua
                DataGridPays.Columns(5).HeaderCell.Style.BackColor = Color.Aqua
                DataGridPays.Columns(6).HeaderCell.Style.BackColor = Color.Aqua

                DataGridPays.Columns(8).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridPays.Columns(9).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridPays.Columns(10).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridPays.Columns(11).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridPays.Columns(12).HeaderCell.Style.BackColor = Color.PaleGreen
                DataGridPays.Columns(13).HeaderCell.Style.BackColor = Color.PaleGreen
            End If
        End Sub

        ''' <summary>
        ''' DataGridNewFile : supprime fichier du disque
        ''' </summary>
        Private Sub DataGridNewFile_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DataGridNewFile.KeyDown
            'Si appuie sur supprimer du clavier
            If e.KeyCode = 46 Then
                Dim libelle As String = DataGridNewFile.CurrentRow.Cells.Item(0).Value.ToString
                Dim fichier As String = DataGridNewFile.CurrentRow.Cells.Item(1).Value.ToString
                Dim a As Integer = MessageBox.Show("Etes vous sures de supprimer le fichier " & fichier & " de la signature " & libelle & " ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 1 Then
                    Dim cheminFichier As String = front.SelectSimple("TX_RACINE", "chemin")(0) & "\" & libelle & "\" & fichier
                    If (fi.Existe(cheminFichier)) Then
                        Try
                            'Supprimer de window
                            Kill(cheminFichier)
                            DataGridNewFile.Rows.RemoveAt(DataGridNewFile.CurrentRow.Index)
                        Catch ex As Exception
                            MessageBox.Show("Le fichier " & cheminFichier & " est ouvert il ne peut pas etre supprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End Try
                    Else
                        MessageBox.Show("Le fichier " & cheminFichier & " n'existe pas dans Window !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Ouvre AjoutSignature
        ''' </summary>
        Private Sub CréerSignatureToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CréerSignatureToolStripMenuItem.Click
            Me.Close()
            Dim ajoutS As New AjouterSignature
            ajoutS.Show()
        End Sub

        ''' <summary>
        ''' Ouvre RecommandationSecteur
        ''' </summary>
        Private Sub RecommandationSecteurToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecommandationSecteurToolStripMenuItem.Click
            Me.Close()
            Dim recoS As New RecommandationSecteur
            recoS.Show()
        End Sub

        ''' <summary>
        ''' Ouvre RecommandationPays
        ''' </summary>
        Private Sub RecommandationPaysToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecommandationPaysToolStripMenuItem.Click
            Me.Close()
            Dim recoP As New RecommandationPays
            recoP.Show()
        End Sub

        ''' <summary>
        ''' DataGridFile : supprime un fichier archivé (disque + base)
        ''' </summary>
        Private Sub DataGridFile_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DataGridFile.KeyDown
            'Si appuie sur supprimer du clavier
            If e.KeyCode = 46 Then
                Dim libelle As String = DataGridFile.CurrentRow.Cells.Item(0).Value.ToString
                Dim fichier As String = DataGridFile.CurrentRow.Cells.Item(2).Value.ToString
                Dim a As Integer = MessageBox.Show("Etes vous sures de supprimer le fichier " & fichier & " de la signature " & libelle & " ?", "Confirmation.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                If a = 1 Then
                    Dim cheminFichier As String = front.SelectSimple("TX_RACINE", "chemin")(0) & "\" & libelle & "\" & fichier
                    If (fi.Existe(cheminFichier)) Then
                        Try
                            'Supprimer de window
                            Kill(cheminFichier)
                            Dim donnee As List(Of Object) = New List(Of Object)()
                            donnee.Add(front.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", libelle).First)
                            donnee.Add(fichier)
                            Dim colName As List(Of String) = New List(Of String)()
                            colName.Add("code_signature")
                            colName.Add("nom")
                            front.DeleteWheres("TX_FICHIER", colName, donnee)
                            DataGridFile.Rows.RemoveAt(DataGridFile.CurrentRow.Index)
                        Catch ex As Exception
                            MessageBox.Show("Le fichier " & cheminFichier & " est ouvert il ne peut pas etre supprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        End Try
                    Else
                        MessageBox.Show("Le fichier " & cheminFichier & " n'existe pas dans Window !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If
                End If
            End If
        End Sub


        ''' <summary>
        ''' DataGridFile : Récupere les changement dans la grille
        ''' </summary>
        Private Sub DataGridFile_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridFile.CellValueChanged
            Dim libelle As String = DataGridFile.CurrentRow.Cells.Item(0).Value.ToString
            Dim fichier As String = DataGridFile.CurrentRow.Cells.Item(2).Value.ToString
            Dim note As String = DataGridFile.CurrentRow.Cells.Item(4).Value.ToString
            Dim colName1 As List(Of String) = New List(Of String)()
            Dim donnee1 As List(Of Object) = New List(Of Object)()
            'Changement de la note
            If e.ColumnIndex = 4 Then
                colName1.Add("note")
                donnee1.Add(note)
            End If
            'Changement du nom de fichier
            If e.ColumnIndex = 2 Then
                colName1.Add("nom")
                donnee1.Add(fichier)
                Try
                    My.Computer.FileSystem.RenameFile(front.SelectDistinctSimple("TX_RACINE", "chemin")(0) & "\" & libelle & "\" & oldName, fichier)
                Catch ex As Exception
                    If fichier <> oldName Then
                        MessageBox.Show("Le nom fichier " & fichier & " n'est pas correct ou le fichier est ouvert ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                        DataGridFile.CurrentRow.Cells.Item(2).Value = oldName
                    End If
                    Exit Sub
                End Try
            End If
            Dim colName2 As List(Of String) = New List(Of String)()
            Dim donnee2 As List(Of Object) = New List(Of Object)()
            colName2.Add("code_signature")
            colName2.Add("nom")
            donnee2.Add(front.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", libelle).First)
            If e.ColumnIndex <> 2 Then
                donnee2.Add(fichier)
            Else
                donnee2.Add(oldName)
            End If
            front.Updates("TX_FICHIER", colName1, donnee1, colName2, donnee2)
        End Sub

        ''' <summary>
        ''' DataGridFile : Mémorise l'ancien nom du ficher
        ''' </summary>
        Private Sub DataGridFile_CellBeginEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles DataGridFile.CellBeginEdit
            If e.ColumnIndex = 2 Then
                oldName = DataGridFile.CurrentRow.Cells.Item(2).Value.ToString
            End If
        End Sub

        ''' <summary>
        ''' BRafraichirOmega : remplie la table TMP_SIGNATURE_OMEGA
        ''' </summary>
        Private Sub BRafraichirOmega_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BRafraichirOmega.Click
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            omega.commandeSql("TMP_SIGNATURE_OMEGA", omega.LectureFichierSql("TMP_SIGNATURE_OMEGA.sql"), True)
            front.RequeteSql("EXECUTE TX_rating_note")
            Windows.Forms.Cursor.Current = Cursors.Default
        End Sub

        ''' <summary>
        ''' BCréer : ajoute une signature OMEGA dans la base
        ''' </summary>
        Private Sub BCreer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BCreer.Click
            Dim libelle As String = RTrim(CbLibelleOmega.Text)

            If String.IsNullOrEmpty(libelle) = False Then

                Dim code As String = front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "code", "libelle", libelle)(0)

                If front.SelectDistinctWhere("TX_SIGNATURE", "code", "libelle", libelle).Count = 0 And front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "libelle", "libelle", libelle).Count > 0 Then
                    Dim colSignature As List(Of String) = New List(Of String)()
                    Dim donneeSignature As List(Of Object) = New List(Of Object)()
                    Dim colName As List(Of String) = New List(Of String)()
                    Dim donnee As List(Of Object) = New List(Of Object)()

                    'Check si nouveau groupe
                    Dim id_groupe As String = front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "id_groupe", "code", code)(0)
                    If front.SelectDistinctWhere("TX_GROUPE", "id", "id", id_groupe).Count = 0 Then
                        'TX_GROUPE(id, libelle, utilisateur)
                        colName.Add("id")
                        colName.Add("libelle")
                        colName.Add("id_utilisateur")
                        donnee.Add(id_groupe)
                        donnee.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "libelle_groupe", "id_groupe", id_groupe)(0))
                        donnee.Add("OMEGA")
                        front.Insert("TX_GROUPE", colName, donnee)
                        colName.Clear()
                        donnee.Clear()
                    End If

                    'Check si secteur
                    Dim id_secteur As String = front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "id_secteur", "code", code)(0)
                    If front.SelectDistinctWhere("SECTEUR", "id", "id", id_secteur).Count = 0 Then
                        'SECTEUR(id, libelle, utilisateur)
                        colName.Add("id")
                        colName.Add("libelle")
                        colName.Add("id_utilisateur")
                        donnee.Add(id_secteur)
                        donnee.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "libelle_secteur", "id_secteur", id_secteur)(0))
                        donnee.Add("OMEGA")
                        front.Insert("SECTEUR", colName, donnee)
                        colName.Clear()
                        donnee.Clear()
                    End If

                    'Check si nouveau sous secteur
                    Dim id_sous_secteur As String = front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "id_sous_secteur", "code", code)(0)
                    If front.SelectDistinctWhere("SOUS_SECTEUR", "id", "id", id_sous_secteur).Count = 0 Then
                        'SOUS_SECTEUR(id, id_secteur, libelle, utilisateur)
                        colName.Add("id")
                        colName.Add("id_secteur")
                        colName.Add("libelle")
                        colName.Add("id_utilisateur")
                        donnee.Add(id_sous_secteur)
                        donnee.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "id_secteur", "id_sous_secteur", id_sous_secteur)(0))
                        donnee.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "libelle_sous_secteur", "id_sous_secteur", id_sous_secteur)(0))
                        donnee.Add("OMEGA")
                        front.Insert("SOUS_SECTEUR", colName, donnee)
                        colName.Clear()
                        donnee.Clear()
                    End If

                    'Check si nouveau pays + association
                    Dim id_pays As String = front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "id_pays", "code", code)(0)
                    If front.SelectDistinctWhere("PAYS", "id", "id", id_pays).Count = 0 Then
                        'PAYS(id, libelle, utilisateur)
                        colName.Add("id")
                        colName.Add("libelle")
                        colName.Add("id_utilisateur")
                        donnee.Add(id_pays)
                        donnee.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "libelle_pays", "id_pays", id_pays)(0))
                        donnee.Add("OMEGA")
                        front.Insert("PAYS", colName, donnee)
                        colName.Clear()
                        donnee.Clear()
                        'ASSOCIATION_PAYS_ZONE(id_pays, id_zone)
                        colName.Add("id_pays")
                        colName.Add("id_zone")
                        donnee.Add(id_pays)
                        donnee.Add("NA")
                        front.Insert("ASSOCIATION_PAYS_ZONE", colName, donnee)
                        colName.Clear()
                        donnee.Clear()
                    End If

                    'Ajout dans TX_SIGNATURE
                    colSignature.Add("code")
                    donneeSignature.Add(code)
                    colSignature.Add("libelle")
                    donneeSignature.Add(libelle)
                    colSignature.Add("id_groupe")
                    donneeSignature.Add(id_groupe)
                    colSignature.Add("id_recommandation")
                    Dim id_recommandation As String = "Na"
                    donneeSignature.Add(id_recommandation)
                    colSignature.Add("id_secteur")
                    donneeSignature.Add(id_secteur)
                    colSignature.Add("id_sous_secteur")
                    donneeSignature.Add(id_sous_secteur)
                    colSignature.Add("id_pays")
                    donneeSignature.Add(id_pays)
                    colSignature.Add("id_rating_Moo_CT")
                    donneeSignature.Add("MNA  CT")
                    colSignature.Add("id_rating_Moo_LT")
                    donneeSignature.Add("MNA  LT")
                    colSignature.Add("id_rating_Sp_CT")
                    donneeSignature.Add("SNA  CT")
                    colSignature.Add("id_rating_Sp_LT")
                    donneeSignature.Add("SNA  LT")
                    colSignature.Add("id_rating_Fi_CT")
                    donneeSignature.Add("FNA  CT")
                    colSignature.Add("id_rating_Fi_LT")
                    donneeSignature.Add("FNA  LT")
                    colSignature.Add("id_rating_In_CT")
                    Dim id_interneCT As String = "I" & front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "interneCT", "code", code)(0).PadRight(4, " ") & "CT"
                    donneeSignature.Add(id_interneCT)
                    colSignature.Add("id_rating_In_LT")
                    Dim id_interneLT As String = "I" & front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "interneLT", "code", code)(0).PadRight(4, " ") & "LT"
                    donneeSignature.Add(id_interneLT)
                    colSignature.Add("id_utilisateur")
                    donneeSignature.Add(Utilisateur.login)
                    colSignature.Add("note_isr")
                    donneeSignature.Add(front.SelectDistinctWhere("TMP_SIGNATURE_OMEGA", "note_isr", "code", code).FirstOrDefault)
                    front.Insert("TX_SIGNATURE", colSignature, donneeSignature)

                    'Création du dossier si il existe pas
                    If Directory.Exists(front.SelectDistinctSimple("TX_RACINE", "chemin").First & "\" & libelle) = False Then
                        Directory.CreateDirectory(front.SelectDistinctSimple("TX_RACINE", "chemin").First & "\" & libelle)
                    End If

                    'Mise à jour de la table TX_HISTO_RATING
                    'TX_HISTO_RATING(id_rating, code_signature, date, id_utilisateur)
                    colName.Add("id_rating")
                    colName.Add("code_signature")
                    colName.Add("date")
                    colName.Add("id_utilisateur")
                    donnee.Add(id_interneLT)
                    donnee.Add(code)
                    donnee.Add(DateTime.Now)
                    donnee.Add(Utilisateur.login)
                    front.Insert("TX_HISTO_RATING", colName, donnee)
                    donnee.Clear()
                    donnee.Add(id_interneCT)
                    donnee.Add(code)
                    donnee.Add(DateTime.Now)
                    donnee.Add(Utilisateur.login)
                    front.Insert("TX_HISTO_RATING", colName, donnee)
                    donnee.Clear()
                    colName.Clear()

                    'Mise à jour de la table TX_HISTO_RECOMMANDATION
                    'TX_HISTO_RECOMMANDATION(id_recommandation, code_signature, date, id_utilisateur)
                    colName.Add("id_recommandation")
                    colName.Add("code_signature")
                    colName.Add("date")
                    colName.Add("id_utilisateur")
                    donnee.Add(id_recommandation)
                    donnee.Add(code)
                    donnee.Add(DateTime.Now)
                    donnee.Add(Utilisateur.login)
                    front.Insert("TX_HISTO_RECOMMANDATION", colName, donnee)
                    colName.Clear()
                    donnee.Clear()


                    colName.Add("@text")
                    donnee.Add(" '%" & TSignature.Text & "%'")
                    CbLibelleBase.DataSource = front.ProcedureStockéeList("GetSignatureLibelle", colName, donnee)
                    CbLibelleOmega.DataSource = front.ProcedureStockéeList("GetSignatureLibelleOmega", colName, donnee)
                    Rafraichir()
                Else
                    MessageBox.Show("La signature " & libelle & " existe déja dans la base ou n'existe pas dans la base OMEGA ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                End If

            Else
                MessageBox.Show("Aucune signature est sélectionnée !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        ''' <summary>
        ''' DataGridNewFile : renome les fichiers
        ''' </summary>
        Private Sub DataGridNewFile_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridNewFile.CellValueChanged
            If DataGridNewFile.RowCount > 0 Then
                Dim libelle As String = DataGridNewFile.CurrentRow.Cells.Item(0).Value.ToString
                Dim fichier As String = DataGridNewFile.CurrentRow.Cells.Item(1).Value.ToString

                'Changement du nom de fichier
                If e.ColumnIndex = 1 Then
                    Try
                        My.Computer.FileSystem.RenameFile(front.SelectDistinctSimple("TX_RACINE", "chemin")(0) & "\" & libelle & "\" & oldName, fichier)
                    Catch ex As Exception
                        If fichier <> oldName Then
                            MessageBox.Show("Le nom fichier " & fichier & " n'est pas correct ou le fichier est ouvert ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                            DataGridNewFile.CurrentRow.Cells.Item(1).Value = oldName
                        End If
                        Exit Sub
                    End Try
                End If
            End If
        End Sub

        ''' <summary>
        ''' DataGridNewFile : sauvegarde l'ancien non des fichiers
        ''' </summary>
        Private Sub DataGridNewFile_CellBeginEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles DataGridNewFile.CellBeginEdit
            If e.ColumnIndex = 1 Then
                oldName = DataGridNewFile.CurrentRow.Cells.Item(1).Value.ToString
            End If
        End Sub


        ''' <summary>
        ''' Bouton BPrint : imprime une des 3 datatgrildview
        ''' </summary>
        Private Sub BPrint_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPrint.Click
            Dim dataChoice As DataGridView = Nothing
            If String.IsNullOrEmpty(CbDataGrid.Text) = False Then
                Select Case CbDataGrid.Text
                    Case "Pays"
                        dataChoice = DataGridPays
                    Case "Sous secteur"
                        dataChoice = DataGridSousSecteur
                    Case "Signature"
                        dataChoice = DataGridSignature
                    Case Else
                        MsgBox("Cette datagrid n'existe pas")
                End Select

                If MyDataGridViewPrinter.SetupThePrinting(dataChoice, myPrintDocument, Me.Text) Then
                    myPrintDocument.Print()
                End If

            Else
                MessageBox.Show("Il faut choisir la datagrid à imprimer !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
            End If
        End Sub

        Private Sub GroupBox2_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox2.Enter
            If Utilisateur.admin = True Then
                BChemin.Enabled = True
            End If
        End Sub

        Private Sub DataGridSignature_ColumnAdded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewColumnEventArgs)
            'DataGridSignature.Columns(e.Column.Index).HeaderCell.Style.ForeColor = Color.Gray
            'e.Column.HeaderCell.Style.BackColor = Color.Maroon
        End Sub

    End Class
End Namespace