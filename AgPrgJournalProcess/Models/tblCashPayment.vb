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

Partial Public Class tblCashPayment
    Public Property IDCashPayment As Integer
    Public Property IDSpisu As Nullable(Of Integer)
    Public Property IDVisitReport As Nullable(Of Integer)
    Public Property IDPaymentOrder As Nullable(Of Integer)
    Public Property DatePaymentPickup As Nullable(Of Date)
    Public Property AmountPickup As Nullable(Of Double)
    Public Property AmountSended As Nullable(Of Double)
    Public Property rr_CashPaymState As Nullable(Of Byte)
    Public Property DatePaymentOnCentrale As Nullable(Of Date)
    Public Property IDSpisyPlatbyDosle As Nullable(Of Integer)
    Public Property CashDocNumber As Nullable(Of Integer)
    Public Property rr_Journal As Nullable(Of Byte)
    Public Property TStamp As Byte()
    Public Property AmountOnCentrale As Nullable(Of Double)

    Public Overridable Property tblSpisy As tblSpisy

End Class
