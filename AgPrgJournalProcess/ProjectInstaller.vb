Imports System.ComponentModel
Imports System.Configuration.Install

Public Class ProjectInstaller

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add initialization code after the call to InitializeComponent
    End Sub


    Protected Overrides Sub OnCommitted(savedState As System.Collections.IDictionary)
        Dim sc As New ServiceProcess.ServiceController("TRACEInterface_test")
        sc.Start()
    End Sub

    Private Sub ServiceInstaller_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceInstaller.AfterInstall

    End Sub

    Private Sub ServiceProcessInstaller_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceProcessInstaller.AfterInstall

    End Sub
End Class
