'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class tblSpisyOsoby
    Public Property IDSpisyOsoby As Integer
    Public Property IDSrcSpisyOsoby As Nullable(Of Long)
    Public Property PersSurname As String
    Public Property PersName As String
    Public Property PersRC As String
    Public Property PersIC As String
    Public Property PersBirthDate As Nullable(Of Date)
    Public Property PersSurname2 As String
    Public Property BonitaKatastr As String
    Public Property BonitaPocetExe As Nullable(Of Short)
    Public Property BonitaDatLustrace As Nullable(Of Date)
    Public Property TStamp As Byte()

    Public Overridable Property tblSpisyOsobyEmails As ICollection(Of tblSpisyOsobyEmail) = New HashSet(Of tblSpisyOsobyEmail)
    Public Overridable Property tblSpisyOsobyPhones As ICollection(Of tblSpisyOsobyPhone) = New HashSet(Of tblSpisyOsobyPhone)
    Public Overridable Property tblSpisyOsobyZams As ICollection(Of tblSpisyOsobyZam) = New HashSet(Of tblSpisyOsobyZam)
    Public Overridable Property tblSpisyTypOsobies As ICollection(Of tblSpisyTypOsoby) = New HashSet(Of tblSpisyTypOsoby)

End Class
