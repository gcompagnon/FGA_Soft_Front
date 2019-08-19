Namespace Referentiel
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GestionTable
        Inherits System.Windows.Forms.Form

        'Form remplace la méthode Dispose pour nettoyer la liste des composants.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Requise par le Concepteur Windows Form
        Private components As System.ComponentModel.IContainer

        'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
        'Ne la modifiez pas à l'aide de l'éditeur de code.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GestionTable))
            Me.CbFGA = New System.Windows.Forms.CheckBox()
            Me.CbProxy = New System.Windows.Forms.CheckBox()
            Me.BViderA = New System.Windows.Forms.Button()
            Me.BRemplirPort = New System.Windows.Forms.Button()
            Me.BToutVider = New System.Windows.Forms.Button()
            Me.CbCorrespondances = New System.Windows.Forms.CheckBox()
            Me.CbLot = New System.Windows.Forms.CheckBox()
            Me.BAjouterLot = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.cbTrans1 = New System.Windows.Forms.CheckBox()
            Me.BAjoutA = New System.Windows.Forms.Button()
            Me.TabControl = New System.Windows.Forms.TabControl()
            Me.TabPagePortefeuille = New System.Windows.Forms.TabPage()
            Me.CbTrans3 = New System.Windows.Forms.CheckBox()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.CbDate = New System.Windows.Forms.ComboBox()
            Me.CbTrans2 = New System.Windows.Forms.CheckBox()
            Me.TabPageGeneral = New System.Windows.Forms.TabPage()
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.BRefreshSecteur = New System.Windows.Forms.Button()
            Me.BRefreshPays = New System.Windows.Forms.Button()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.CbSecteur = New System.Windows.Forms.CheckBox()
            Me.CbPays = New System.Windows.Forms.CheckBox()
            Me.CbZoneGéographique = New System.Windows.Forms.CheckBox()
            Me.BRemplirGeneral = New System.Windows.Forms.Button()
            Me.TabPageTaux = New System.Windows.Forms.TabPage()
            Me.BRefreshRating = New System.Windows.Forms.Button()
            Me.CbRatingEmetteur = New System.Windows.Forms.CheckBox()
            Me.CbCorrespondanceIboxx = New System.Windows.Forms.CheckBox()
            Me.Panel3 = New System.Windows.Forms.Panel()
            Me.BRefreshSignature = New System.Windows.Forms.Button()
            Me.iBoxx = New System.Windows.Forms.ComboBox()
            Me.CbiBoxx = New System.Windows.Forms.CheckBox()
            Me.CbEmetteurFichier = New System.Windows.Forms.CheckBox()
            Me.CbRacine = New System.Windows.Forms.CheckBox()
            Me.CbEmetteur = New System.Windows.Forms.CheckBox()
            Me.CbRecommandation = New System.Windows.Forms.CheckBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.CbRating = New System.Windows.Forms.CheckBox()
            Me.BRemplirTaux = New System.Windows.Forms.Button()
            Me.TabPageAction = New System.Windows.Forms.TabPage()
            Me.BRemplirAct = New System.Windows.Forms.Button()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.CbActionSecteur = New System.Windows.Forms.CheckBox()
            Me.CbTrans4 = New System.Windows.Forms.CheckBox()
            Me.TabControl.SuspendLayout()
            Me.TabPagePortefeuille.SuspendLayout()
            Me.TabPageGeneral.SuspendLayout()
            Me.TabPageTaux.SuspendLayout()
            Me.TabPageAction.SuspendLayout()
            Me.SuspendLayout()
            '
            'CbFGA
            '
            Me.CbFGA.AutoSize = True
            Me.CbFGA.Location = New System.Drawing.Point(15, 49)
            Me.CbFGA.Name = "CbFGA"
            Me.CbFGA.Size = New System.Drawing.Size(102, 17)
            Me.CbFGA.TabIndex = 0
            Me.CbFGA.Text = "Portefeuille FGA"
            Me.CbFGA.UseVisualStyleBackColor = True
            '
            'CbProxy
            '
            Me.CbProxy.AutoSize = True
            Me.CbProxy.Location = New System.Drawing.Point(15, 72)
            Me.CbProxy.Name = "CbProxy"
            Me.CbProxy.Size = New System.Drawing.Size(107, 17)
            Me.CbProxy.TabIndex = 1
            Me.CbProxy.Text = "Portefeuille Proxy"
            Me.CbProxy.UseVisualStyleBackColor = True
            '
            'BViderA
            '
            Me.BViderA.Location = New System.Drawing.Point(281, 219)
            Me.BViderA.Name = "BViderA"
            Me.BViderA.Size = New System.Drawing.Size(72, 26)
            Me.BViderA.TabIndex = 5
            Me.BViderA.Text = "Vider à"
            Me.BViderA.UseVisualStyleBackColor = True
            '
            'BRemplirPort
            '
            Me.BRemplirPort.Location = New System.Drawing.Point(361, 187)
            Me.BRemplirPort.Name = "BRemplirPort"
            Me.BRemplirPort.Size = New System.Drawing.Size(70, 26)
            Me.BRemplirPort.TabIndex = 6
            Me.BRemplirPort.Text = "Remplir"
            Me.BRemplirPort.UseVisualStyleBackColor = True
            '
            'BToutVider
            '
            Me.BToutVider.Location = New System.Drawing.Point(361, 219)
            Me.BToutVider.Name = "BToutVider"
            Me.BToutVider.Size = New System.Drawing.Size(70, 26)
            Me.BToutVider.TabIndex = 8
            Me.BToutVider.Text = "Tout Vider"
            Me.BToutVider.UseVisualStyleBackColor = True
            '
            'CbCorrespondances
            '
            Me.CbCorrespondances.AutoSize = True
            Me.CbCorrespondances.Location = New System.Drawing.Point(15, 116)
            Me.CbCorrespondances.Name = "CbCorrespondances"
            Me.CbCorrespondances.Size = New System.Drawing.Size(133, 17)
            Me.CbCorrespondances.TabIndex = 9
            Me.CbCorrespondances.Text = "Correspondances FGA"
            Me.CbCorrespondances.UseVisualStyleBackColor = True
            '
            'CbLot
            '
            Me.CbLot.AutoSize = True
            Me.CbLot.Location = New System.Drawing.Point(15, 139)
            Me.CbLot.Name = "CbLot"
            Me.CbLot.Size = New System.Drawing.Size(110, 17)
            Me.CbLot.TabIndex = 10
            Me.CbLot.Text = "Lot de portefeuille"
            Me.CbLot.UseVisualStyleBackColor = True
            '
            'BAjouterLot
            '
            Me.BAjouterLot.Location = New System.Drawing.Point(121, 135)
            Me.BAjouterLot.Name = "BAjouterLot"
            Me.BAjouterLot.Size = New System.Drawing.Size(52, 22)
            Me.BAjouterLot.TabIndex = 11
            Me.BAjouterLot.Text = "Ajouter"
            Me.BAjouterLot.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(7, 19)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(250, 13)
            Me.Label1.TabIndex = 12
            Me.Label1.Text = "Cochez la ou les case(s) pour effectuer une action :"
            '
            'cbTrans1
            '
            Me.cbTrans1.AutoSize = True
            Me.cbTrans1.Location = New System.Drawing.Point(15, 95)
            Me.cbTrans1.Name = "cbTrans1"
            Me.cbTrans1.Size = New System.Drawing.Size(71, 17)
            Me.cbTrans1.TabIndex = 13
            Me.cbTrans1.Text = "Transp. 1"
            Me.cbTrans1.UseVisualStyleBackColor = True
            '
            'BAjoutA
            '
            Me.BAjoutA.Location = New System.Drawing.Point(281, 187)
            Me.BAjoutA.Name = "BAjoutA"
            Me.BAjoutA.Size = New System.Drawing.Size(74, 26)
            Me.BAjoutA.TabIndex = 16
            Me.BAjoutA.Text = "Ajouter à"
            Me.BAjoutA.UseVisualStyleBackColor = True
            '
            'TabControl
            '
            Me.TabControl.Controls.Add(Me.TabPagePortefeuille)
            Me.TabControl.Controls.Add(Me.TabPageGeneral)
            Me.TabControl.Controls.Add(Me.TabPageTaux)
            Me.TabControl.Controls.Add(Me.TabPageAction)
            Me.TabControl.Location = New System.Drawing.Point(-4, -3)
            Me.TabControl.Name = "TabControl"
            Me.TabControl.SelectedIndex = 0
            Me.TabControl.Size = New System.Drawing.Size(474, 302)
            Me.TabControl.TabIndex = 17
            '
            'TabPagePortefeuille
            '
            Me.TabPagePortefeuille.BackColor = System.Drawing.SystemColors.Control
            Me.TabPagePortefeuille.Controls.Add(Me.CbTrans4)
            Me.TabPagePortefeuille.Controls.Add(Me.CbTrans3)
            Me.TabPagePortefeuille.Controls.Add(Me.Panel1)
            Me.TabPagePortefeuille.Controls.Add(Me.CbDate)
            Me.TabPagePortefeuille.Controls.Add(Me.CbTrans2)
            Me.TabPagePortefeuille.Controls.Add(Me.Label1)
            Me.TabPagePortefeuille.Controls.Add(Me.BAjoutA)
            Me.TabPagePortefeuille.Controls.Add(Me.CbFGA)
            Me.TabPagePortefeuille.Controls.Add(Me.CbProxy)
            Me.TabPagePortefeuille.Controls.Add(Me.BViderA)
            Me.TabPagePortefeuille.Controls.Add(Me.cbTrans1)
            Me.TabPagePortefeuille.Controls.Add(Me.BRemplirPort)
            Me.TabPagePortefeuille.Controls.Add(Me.BAjouterLot)
            Me.TabPagePortefeuille.Controls.Add(Me.BToutVider)
            Me.TabPagePortefeuille.Controls.Add(Me.CbLot)
            Me.TabPagePortefeuille.Controls.Add(Me.CbCorrespondances)
            Me.TabPagePortefeuille.Location = New System.Drawing.Point(4, 22)
            Me.TabPagePortefeuille.Name = "TabPagePortefeuille"
            Me.TabPagePortefeuille.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPagePortefeuille.Size = New System.Drawing.Size(466, 276)
            Me.TabPagePortefeuille.TabIndex = 0
            Me.TabPagePortefeuille.Text = "Portefeuille"
            '
            'CbTrans3
            '
            Me.CbTrans3.AutoSize = True
            Me.CbTrans3.Location = New System.Drawing.Point(153, 95)
            Me.CbTrans3.Name = "CbTrans3"
            Me.CbTrans3.Size = New System.Drawing.Size(71, 17)
            Me.CbTrans3.TabIndex = 20
            Me.CbTrans3.Text = "Transp. 3"
            Me.CbTrans3.UseVisualStyleBackColor = True
            '
            'Panel1
            '
            Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
            Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel1.Location = New System.Drawing.Point(401, 6)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(53, 49)
            Me.Panel1.TabIndex = 19
            '
            'CbDate
            '
            Me.CbDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.CbDate.FormattingEnabled = True
            Me.CbDate.Location = New System.Drawing.Point(306, 72)
            Me.CbDate.Name = "CbDate"
            Me.CbDate.Size = New System.Drawing.Size(122, 21)
            Me.CbDate.TabIndex = 18
            '
            'CbTrans2
            '
            Me.CbTrans2.AutoSize = True
            Me.CbTrans2.Location = New System.Drawing.Point(83, 95)
            Me.CbTrans2.Name = "CbTrans2"
            Me.CbTrans2.Size = New System.Drawing.Size(71, 17)
            Me.CbTrans2.TabIndex = 17
            Me.CbTrans2.Text = "Transp. 2"
            Me.CbTrans2.UseVisualStyleBackColor = True
            '
            'TabPageGeneral
            '
            Me.TabPageGeneral.BackColor = System.Drawing.SystemColors.Control
            Me.TabPageGeneral.Controls.Add(Me.Panel2)
            Me.TabPageGeneral.Controls.Add(Me.BRefreshSecteur)
            Me.TabPageGeneral.Controls.Add(Me.BRefreshPays)
            Me.TabPageGeneral.Controls.Add(Me.Label2)
            Me.TabPageGeneral.Controls.Add(Me.CbSecteur)
            Me.TabPageGeneral.Controls.Add(Me.CbPays)
            Me.TabPageGeneral.Controls.Add(Me.CbZoneGéographique)
            Me.TabPageGeneral.Controls.Add(Me.BRemplirGeneral)
            Me.TabPageGeneral.Location = New System.Drawing.Point(4, 22)
            Me.TabPageGeneral.Name = "TabPageGeneral"
            Me.TabPageGeneral.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPageGeneral.Size = New System.Drawing.Size(466, 276)
            Me.TabPageGeneral.TabIndex = 1
            Me.TabPageGeneral.Text = "Général"
            '
            'Panel2
            '
            Me.Panel2.BackgroundImage = CType(resources.GetObject("Panel2.BackgroundImage"), System.Drawing.Image)
            Me.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel2.Location = New System.Drawing.Point(401, 6)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(53, 49)
            Me.Panel2.TabIndex = 21
            '
            'BRefreshSecteur
            '
            Me.BRefreshSecteur.BackgroundImage = CType(resources.GetObject("BRefreshSecteur.BackgroundImage"), System.Drawing.Image)
            Me.BRefreshSecteur.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRefreshSecteur.Location = New System.Drawing.Point(152, 86)
            Me.BRefreshSecteur.Name = "BRefreshSecteur"
            Me.BRefreshSecteur.Size = New System.Drawing.Size(26, 25)
            Me.BRefreshSecteur.TabIndex = 20
            Me.BRefreshSecteur.UseVisualStyleBackColor = True
            '
            'BRefreshPays
            '
            Me.BRefreshPays.BackgroundImage = CType(resources.GetObject("BRefreshPays.BackgroundImage"), System.Drawing.Image)
            Me.BRefreshPays.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRefreshPays.Location = New System.Drawing.Point(152, 43)
            Me.BRefreshPays.Name = "BRefreshPays"
            Me.BRefreshPays.Size = New System.Drawing.Size(26, 25)
            Me.BRefreshPays.TabIndex = 19
            Me.BRefreshPays.UseVisualStyleBackColor = True
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(7, 19)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(250, 13)
            Me.Label2.TabIndex = 18
            Me.Label2.Text = "Cochez la ou les case(s) pour effectuer une action :"
            '
            'CbSecteur
            '
            Me.CbSecteur.AutoSize = True
            Me.CbSecteur.Location = New System.Drawing.Point(15, 93)
            Me.CbSecteur.Name = "CbSecteur"
            Me.CbSecteur.Size = New System.Drawing.Size(136, 17)
            Me.CbSecteur.TabIndex = 16
            Me.CbSecteur.Text = "Secteur - Sous Secteur"
            Me.CbSecteur.UseVisualStyleBackColor = True
            '
            'CbPays
            '
            Me.CbPays.AutoSize = True
            Me.CbPays.Location = New System.Drawing.Point(15, 49)
            Me.CbPays.Name = "CbPays"
            Me.CbPays.Size = New System.Drawing.Size(83, 17)
            Me.CbPays.TabIndex = 15
            Me.CbPays.Text = "Pays - Zone"
            Me.CbPays.UseVisualStyleBackColor = True
            '
            'CbZoneGéographique
            '
            Me.CbZoneGéographique.AutoSize = True
            Me.CbZoneGéographique.Location = New System.Drawing.Point(15, 70)
            Me.CbZoneGéographique.Name = "CbZoneGéographique"
            Me.CbZoneGéographique.Size = New System.Drawing.Size(121, 17)
            Me.CbZoneGéographique.TabIndex = 14
            Me.CbZoneGéographique.Text = "Zone Géographique"
            Me.CbZoneGéographique.UseVisualStyleBackColor = True
            '
            'BRemplirGeneral
            '
            Me.BRemplirGeneral.Location = New System.Drawing.Point(351, 216)
            Me.BRemplirGeneral.Name = "BRemplirGeneral"
            Me.BRemplirGeneral.Size = New System.Drawing.Size(69, 28)
            Me.BRemplirGeneral.TabIndex = 4
            Me.BRemplirGeneral.Text = "Remplir"
            Me.BRemplirGeneral.UseVisualStyleBackColor = True
            '
            'TabPageTaux
            '
            Me.TabPageTaux.BackColor = System.Drawing.SystemColors.Control
            Me.TabPageTaux.Controls.Add(Me.BRefreshRating)
            Me.TabPageTaux.Controls.Add(Me.CbRatingEmetteur)
            Me.TabPageTaux.Controls.Add(Me.CbCorrespondanceIboxx)
            Me.TabPageTaux.Controls.Add(Me.Panel3)
            Me.TabPageTaux.Controls.Add(Me.BRefreshSignature)
            Me.TabPageTaux.Controls.Add(Me.iBoxx)
            Me.TabPageTaux.Controls.Add(Me.CbiBoxx)
            Me.TabPageTaux.Controls.Add(Me.CbEmetteurFichier)
            Me.TabPageTaux.Controls.Add(Me.CbRacine)
            Me.TabPageTaux.Controls.Add(Me.CbEmetteur)
            Me.TabPageTaux.Controls.Add(Me.CbRecommandation)
            Me.TabPageTaux.Controls.Add(Me.Label3)
            Me.TabPageTaux.Controls.Add(Me.CbRating)
            Me.TabPageTaux.Controls.Add(Me.BRemplirTaux)
            Me.TabPageTaux.Location = New System.Drawing.Point(4, 22)
            Me.TabPageTaux.Name = "TabPageTaux"
            Me.TabPageTaux.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPageTaux.Size = New System.Drawing.Size(466, 276)
            Me.TabPageTaux.TabIndex = 2
            Me.TabPageTaux.Text = "Taux"
            '
            'BRefreshRating
            '
            Me.BRefreshRating.BackgroundImage = CType(resources.GetObject("BRefreshRating.BackgroundImage"), System.Drawing.Image)
            Me.BRefreshRating.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRefreshRating.Location = New System.Drawing.Point(193, 156)
            Me.BRefreshRating.Name = "BRefreshRating"
            Me.BRefreshRating.Size = New System.Drawing.Size(26, 25)
            Me.BRefreshRating.TabIndex = 35
            Me.BRefreshRating.UseVisualStyleBackColor = True
            '
            'CbRatingEmetteur
            '
            Me.CbRatingEmetteur.AutoSize = True
            Me.CbRatingEmetteur.Location = New System.Drawing.Point(15, 164)
            Me.CbRatingEmetteur.Name = "CbRatingEmetteur"
            Me.CbRatingEmetteur.Size = New System.Drawing.Size(101, 17)
            Me.CbRatingEmetteur.TabIndex = 34
            Me.CbRatingEmetteur.Text = "Rating emetteur"
            Me.CbRatingEmetteur.UseVisualStyleBackColor = True
            '
            'CbCorrespondanceIboxx
            '
            Me.CbCorrespondanceIboxx.AutoSize = True
            Me.CbCorrespondanceIboxx.Location = New System.Drawing.Point(15, 188)
            Me.CbCorrespondanceIboxx.Name = "CbCorrespondanceIboxx"
            Me.CbCorrespondanceIboxx.Size = New System.Drawing.Size(132, 17)
            Me.CbCorrespondanceIboxx.TabIndex = 33
            Me.CbCorrespondanceIboxx.Text = "Correspondance iBoxx"
            Me.CbCorrespondanceIboxx.UseVisualStyleBackColor = True
            '
            'Panel3
            '
            Me.Panel3.BackgroundImage = CType(resources.GetObject("Panel3.BackgroundImage"), System.Drawing.Image)
            Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.Panel3.Location = New System.Drawing.Point(401, 6)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(53, 49)
            Me.Panel3.TabIndex = 32
            '
            'BRefreshSignature
            '
            Me.BRefreshSignature.BackgroundImage = CType(resources.GetObject("BRefreshSignature.BackgroundImage"), System.Drawing.Image)
            Me.BRefreshSignature.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BRefreshSignature.Location = New System.Drawing.Point(193, 90)
            Me.BRefreshSignature.Name = "BRefreshSignature"
            Me.BRefreshSignature.Size = New System.Drawing.Size(26, 25)
            Me.BRefreshSignature.TabIndex = 31
            Me.BRefreshSignature.UseVisualStyleBackColor = True
            '
            'iBoxx
            '
            Me.iBoxx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.iBoxx.FormattingEnabled = True
            Me.iBoxx.Location = New System.Drawing.Point(81, 210)
            Me.iBoxx.Name = "iBoxx"
            Me.iBoxx.Size = New System.Drawing.Size(203, 21)
            Me.iBoxx.TabIndex = 30
            '
            'CbiBoxx
            '
            Me.CbiBoxx.AutoSize = True
            Me.CbiBoxx.Location = New System.Drawing.Point(15, 212)
            Me.CbiBoxx.Name = "CbiBoxx"
            Me.CbiBoxx.Size = New System.Drawing.Size(51, 17)
            Me.CbiBoxx.TabIndex = 29
            Me.CbiBoxx.Text = "iBoxx"
            Me.CbiBoxx.UseVisualStyleBackColor = True
            '
            'CbEmetteurFichier
            '
            Me.CbEmetteurFichier.AutoSize = True
            Me.CbEmetteurFichier.Location = New System.Drawing.Point(15, 141)
            Me.CbEmetteurFichier.Name = "CbEmetteurFichier"
            Me.CbEmetteurFichier.Size = New System.Drawing.Size(99, 17)
            Me.CbEmetteurFichier.TabIndex = 28
            Me.CbEmetteurFichier.Text = "Emetteur fichier"
            Me.CbEmetteurFichier.UseVisualStyleBackColor = True
            '
            'CbRacine
            '
            Me.CbRacine.AutoSize = True
            Me.CbRacine.Location = New System.Drawing.Point(15, 118)
            Me.CbRacine.Name = "CbRacine"
            Me.CbRacine.Size = New System.Drawing.Size(129, 17)
            Me.CbRacine.TabIndex = 27
            Me.CbRacine.Text = "Racine de l'archivage"
            Me.CbRacine.UseVisualStyleBackColor = True
            '
            'CbEmetteur
            '
            Me.CbEmetteur.AutoSize = True
            Me.CbEmetteur.Location = New System.Drawing.Point(15, 95)
            Me.CbEmetteur.Name = "CbEmetteur"
            Me.CbEmetteur.Size = New System.Drawing.Size(71, 17)
            Me.CbEmetteur.TabIndex = 26
            Me.CbEmetteur.Text = "Signature"
            Me.CbEmetteur.UseVisualStyleBackColor = True
            '
            'CbRecommandation
            '
            Me.CbRecommandation.AutoSize = True
            Me.CbRecommandation.Location = New System.Drawing.Point(15, 72)
            Me.CbRecommandation.Name = "CbRecommandation"
            Me.CbRecommandation.Size = New System.Drawing.Size(109, 17)
            Me.CbRecommandation.TabIndex = 25
            Me.CbRecommandation.Text = "Recommandation"
            Me.CbRecommandation.UseVisualStyleBackColor = True
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(7, 19)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(250, 13)
            Me.Label3.TabIndex = 24
            Me.Label3.Text = "Cochez la ou les case(s) pour effectuer une action :"
            '
            'CbRating
            '
            Me.CbRating.AutoSize = True
            Me.CbRating.Location = New System.Drawing.Point(15, 49)
            Me.CbRating.Name = "CbRating"
            Me.CbRating.Size = New System.Drawing.Size(57, 17)
            Me.CbRating.TabIndex = 21
            Me.CbRating.Text = "Rating"
            Me.CbRating.UseVisualStyleBackColor = True
            '
            'BRemplirTaux
            '
            Me.BRemplirTaux.Location = New System.Drawing.Point(351, 216)
            Me.BRemplirTaux.Name = "BRemplirTaux"
            Me.BRemplirTaux.Size = New System.Drawing.Size(69, 28)
            Me.BRemplirTaux.TabIndex = 19
            Me.BRemplirTaux.Text = "Remplir"
            Me.BRemplirTaux.UseVisualStyleBackColor = True
            '
            'TabPageAction
            '
            Me.TabPageAction.BackColor = System.Drawing.SystemColors.Control
            Me.TabPageAction.Controls.Add(Me.BRemplirAct)
            Me.TabPageAction.Controls.Add(Me.Label4)
            Me.TabPageAction.Controls.Add(Me.CbActionSecteur)
            Me.TabPageAction.Location = New System.Drawing.Point(4, 22)
            Me.TabPageAction.Name = "TabPageAction"
            Me.TabPageAction.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPageAction.Size = New System.Drawing.Size(466, 276)
            Me.TabPageAction.TabIndex = 3
            Me.TabPageAction.Text = "Action"
            '
            'BRemplirAct
            '
            Me.BRemplirAct.Location = New System.Drawing.Point(351, 220)
            Me.BRemplirAct.Name = "BRemplirAct"
            Me.BRemplirAct.Size = New System.Drawing.Size(69, 28)
            Me.BRemplirAct.TabIndex = 33
            Me.BRemplirAct.Text = "Remplir"
            Me.BRemplirAct.UseVisualStyleBackColor = True
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(7, 19)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(250, 13)
            Me.Label4.TabIndex = 25
            Me.Label4.Text = "Cochez la ou les case(s) pour effectuer une action :"
            '
            'CbActionSecteur
            '
            Me.CbActionSecteur.AutoSize = True
            Me.CbActionSecteur.Location = New System.Drawing.Point(15, 49)
            Me.CbActionSecteur.Name = "CbActionSecteur"
            Me.CbActionSecteur.Size = New System.Drawing.Size(63, 17)
            Me.CbActionSecteur.TabIndex = 17
            Me.CbActionSecteur.Text = "Secteur"
            Me.CbActionSecteur.UseVisualStyleBackColor = True
            '
            'CbTrans4
            '
            Me.CbTrans4.AutoSize = True
            Me.CbTrans4.Location = New System.Drawing.Point(225, 95)
            Me.CbTrans4.Name = "CbTrans4"
            Me.CbTrans4.Size = New System.Drawing.Size(71, 17)
            Me.CbTrans4.TabIndex = 21
            Me.CbTrans4.Text = "Transp. 4"
            Me.CbTrans4.UseVisualStyleBackColor = True
            '
            'GestionTable
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(465, 298)
            Me.Controls.Add(Me.TabControl)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "GestionTable"
            Me.Text = "Importation des tables"
            Me.TabControl.ResumeLayout(False)
            Me.TabPagePortefeuille.ResumeLayout(False)
            Me.TabPagePortefeuille.PerformLayout()
            Me.TabPageGeneral.ResumeLayout(False)
            Me.TabPageGeneral.PerformLayout()
            Me.TabPageTaux.ResumeLayout(False)
            Me.TabPageTaux.PerformLayout()
            Me.TabPageAction.ResumeLayout(False)
            Me.TabPageAction.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents CbFGA As System.Windows.Forms.CheckBox
        Friend WithEvents CbProxy As System.Windows.Forms.CheckBox
        Friend WithEvents BViderA As System.Windows.Forms.Button
        Friend WithEvents BRemplirPort As System.Windows.Forms.Button
        Friend WithEvents BToutVider As System.Windows.Forms.Button
        Friend WithEvents CbCorrespondances As System.Windows.Forms.CheckBox
        Friend WithEvents CbLot As System.Windows.Forms.CheckBox
        Friend WithEvents BAjouterLot As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents cbTrans1 As System.Windows.Forms.CheckBox
        Friend WithEvents BAjoutA As System.Windows.Forms.Button
        Friend WithEvents TabControl As System.Windows.Forms.TabControl
        Friend WithEvents TabPagePortefeuille As System.Windows.Forms.TabPage
        Friend WithEvents TabPageGeneral As System.Windows.Forms.TabPage
        Friend WithEvents BRemplirGeneral As System.Windows.Forms.Button
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents CbSecteur As System.Windows.Forms.CheckBox
        Friend WithEvents CbPays As System.Windows.Forms.CheckBox
        Friend WithEvents CbZoneGéographique As System.Windows.Forms.CheckBox
        Friend WithEvents CbTrans2 As System.Windows.Forms.CheckBox
        Friend WithEvents TabPageTaux As System.Windows.Forms.TabPage
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents CbRating As System.Windows.Forms.CheckBox
        Friend WithEvents BRemplirTaux As System.Windows.Forms.Button
        Friend WithEvents CbEmetteur As System.Windows.Forms.CheckBox
        Friend WithEvents CbRecommandation As System.Windows.Forms.CheckBox
        Friend WithEvents CbRacine As System.Windows.Forms.CheckBox
        Friend WithEvents CbEmetteurFichier As System.Windows.Forms.CheckBox
        Friend WithEvents CbiBoxx As System.Windows.Forms.CheckBox
        Friend WithEvents iBoxx As System.Windows.Forms.ComboBox
        Friend WithEvents BRefreshSecteur As System.Windows.Forms.Button
        Friend WithEvents BRefreshSignature As System.Windows.Forms.Button
        Friend WithEvents CbDate As System.Windows.Forms.ComboBox
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents CbCorrespondanceIboxx As System.Windows.Forms.CheckBox
        Friend WithEvents BRefreshPays As System.Windows.Forms.Button
        Friend WithEvents BRefreshRating As System.Windows.Forms.Button
        Friend WithEvents CbRatingEmetteur As System.Windows.Forms.CheckBox
        Friend WithEvents TabPageAction As System.Windows.Forms.TabPage
        Friend WithEvents BRemplirAct As System.Windows.Forms.Button
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents CbActionSecteur As System.Windows.Forms.CheckBox
        Friend WithEvents CbTrans3 As System.Windows.Forms.CheckBox
        Friend WithEvents CbTrans4 As System.Windows.Forms.CheckBox
    End Class
End Namespace
