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

Partial Public Class tblSpisyOsobyEmail
    Public Property IDSpisyOsobyEmail As Integer
    Public Property IDSpisyOsoby As Integer
    Public Property IDSrcSpisyOsobyEmail As Nullable(Of Long)
    Public Property PersEmail As String
    Public Property PersEmailValid As Nullable(Of Boolean)
    Public Property EmailFVisitComment As String
    Public Property New_PersEmailValid As String
    Public Property IntfcState As Byte
    Public Property TStamp As Byte()
    Public Property rr_Journal As Byte
    Public Property TimeValidityUpdate As Date
    Public Property TimeCreated As Date
    Public Property PersEmailMain As Boolean

    Public Overridable Property tblSpisyOsoby As tblSpisyOsoby

End Class
