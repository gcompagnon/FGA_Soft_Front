﻿#ExternalChecksum("..\..\..\..\Action\Note\BaseActionNote.xaml","{8829d00f-11b8-4213-878b-770e8597ac16}","C8B55173BE6F8863E02EBFB04577162F91CEDB8159114BDF70255D676DD5D069")
'------------------------------------------------------------------------------
' <auto-generated>
'     Ce code a été généré par un outil.
'     Version du runtime :4.0.30319.42000
'
'     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
'     le code est régénéré.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Forms.Integration
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Shell
Imports WindowsApplication1.Action.Note
Imports WindowsApplication1.Utilities

Namespace Action.Note
    
    '''<summary>
    '''BaseActionNote
    '''</summary>
    <Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
    Partial Public Class BaseActionNote
        Inherits System.Windows.Window
        Implements System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",33)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents CBSelectedTable As System.Windows.Controls.ComboBox
        
        #End ExternalSource
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",42)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents CBSelectedColumn As System.Windows.Controls.ComboBox
        
        #End ExternalSource
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",46)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents BAddColumn As System.Windows.Controls.Button
        
        #End ExternalSource
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",49)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents DGColumns As System.Windows.Controls.DataGrid
        
        #End ExternalSource
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",117)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents BApply As System.Windows.Controls.Button
        
        #End ExternalSource
        
        
        #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",118)
        <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
        Friend WithEvents BCancel As System.Windows.Controls.Button
        
        #End ExternalSource
        
        Private _contentLoaded As Boolean
        
        '''<summary>
        '''InitializeComponent
        '''</summary>
        <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")>  _
        Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
            If _contentLoaded Then
                Return
            End If
            _contentLoaded = true
            Dim resourceLocater As System.Uri = New System.Uri("/Front;component/action/note/baseactionnote.xaml", System.UriKind.Relative)
            
            #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",1)
            System.Windows.Application.LoadComponent(Me, resourceLocater)
            
            #End ExternalSource
        End Sub
        
        <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
         System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
        Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
            If (connectionId = 1) Then
                Me.CBSelectedTable = CType(target,System.Windows.Controls.ComboBox)
                Return
            End If
            If (connectionId = 2) Then
                Me.CBSelectedColumn = CType(target,System.Windows.Controls.ComboBox)
                Return
            End If
            If (connectionId = 3) Then
                Me.BAddColumn = CType(target,System.Windows.Controls.Button)
                Return
            End If
            If (connectionId = 4) Then
                Me.DGColumns = CType(target,System.Windows.Controls.DataGrid)
                Return
            End If
            If (connectionId = 8) Then
                Me.BApply = CType(target,System.Windows.Controls.Button)
                Return
            End If
            If (connectionId = 9) Then
                Me.BCancel = CType(target,System.Windows.Controls.Button)
                Return
            End If
            Me._contentLoaded = true
        End Sub
        
        <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
         System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"),  _
         System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")>  _
        Sub System_Windows_Markup_IStyleConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IStyleConnector.Connect
            If (connectionId = 5) Then
                
                #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",94)
                AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.DG_BPosUp_Click)
                
                #End ExternalSource
            End If
            If (connectionId = 6) Then
                
                #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",97)
                AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.DG_BPosDown_Click)
                
                #End ExternalSource
            End If
            If (connectionId = 7) Then
                
                #ExternalSource("..\..\..\..\Action\Note\BaseActionNote.xaml",108)
                AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.DG_BDelete_Click)
                
                #End ExternalSource
            End If
        End Sub
    End Class
End Namespace
