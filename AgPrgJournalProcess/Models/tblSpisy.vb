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

Partial Public Class tblSpisy
    Public Property IDSpisu As Integer
    Public Property IDSrcSpisy As Nullable(Of Long)
    Public Property IDUser As Integer
    Public Property ACC As String
    Public Property ActualDebit As Nullable(Of Double)
    Public Property IDCreditor As Nullable(Of Integer)
    Public Property Principal As Nullable(Of Double)
    Public Property Interest As Nullable(Of Double)
    Public Property Costs As Nullable(Of Double)
    Public Property YetPaid As Nullable(Of Double)
    Public Property rr_Currency As String
    Public Property KindOfEnforcement As String
    Public Property AccountNumber As String
    Public Property BankCode As String
    Public Property VariableSymbol As String
    Public Property SpecificSymbol As String
    Public Property LastPaymentDate As Nullable(Of Date)
    Public Property LastPaymentAmount As Nullable(Of Double)
    Public Property LastCallDate As Nullable(Of Date)
    Public Property LastCallText As String
    Public Property TaskForIP As String
    Public Property FileAction As String
    Public Property VisitDateSentFromMother As Nullable(Of Date)
    Public Property VisitDateReceiveFromMotherMax As Nullable(Of Date)
    Public Property VisitDateReceiveFromMother As Nullable(Of Date)
    Public Property VisitDateSentToMother As Nullable(Of Date)
    Public Property IDUserWhoClosedFile As Nullable(Of Integer)
    Public Property rr_ClosedFile As Nullable(Of Short)
    Public Property VisitDateD1Max As Nullable(Of Date)
    Public Property VisitDateD1OSN As Nullable(Of Date)
    Public Property FirstVisitExecuted As Boolean
    Public Property VisitDateDNMax As Nullable(Of Date)
    Public Property VisitDateDNOSN As Nullable(Of Date)
    Public Property VisitDateDXSK As Nullable(Of Date)
    Public Property DateReturnToCreditor As Nullable(Of Date)
    Public Property DateLapse As Nullable(Of Date)
    Public Property DateSignSKUZ As Nullable(Of Date)
    Public Property MaxCommission As Nullable(Of Double)
    Public Property FirstVisitFee As Nullable(Of Double)
    Public Property SumPaymentFromVisits As Nullable(Of Double)
    Public Property ConditionSK As String
    Public Property TypeSK As Nullable(Of Short)
    Public Property TotalCountSK As Nullable(Of Short)
    Public Property PardonCampaign As Boolean
    Public Property MaxCountPayments As Nullable(Of Short)
    Public Property rr_State As Short
    Public Property LL_LastLapse As Integer
    Public Property FirstFieldVisitUpToDay As Nullable(Of Short)
    Public Property NextFieldVisitUpToDay As Nullable(Of Short)
    Public Property IDSpisyRecordHistory_For61 As Nullable(Of Integer)
    Public Property DebtLastContact As Nullable(Of Date)
    Public Property DOHL As String
    Public Property ActualActions As String
    Public Property CountOfFV As Short
    Public Property FV_TerminationDate As Nullable(Of Date)
    Public Property InReturnRequest As Boolean
    Public Property TStamp As Byte()
    Public Property Commission As Double

    Public Overridable Property tblCashPayments As ICollection(Of tblCashPayment) = New HashSet(Of tblCashPayment)
    Public Overridable Property tblCreditor As tblCreditor
    Public Overridable Property tblOtherInfoes As ICollection(Of tblOtherInfo) = New HashSet(Of tblOtherInfo)
    Public Overridable Property tblSpisyDohodies As ICollection(Of tblSpisyDohody) = New HashSet(Of tblSpisyDohody)
    Public Overridable Property tblSpisyFinanceInfoes As ICollection(Of tblSpisyFinanceInfo) = New HashSet(Of tblSpisyFinanceInfo)
    Public Overridable Property tblSpisyHistorieDohods As ICollection(Of tblSpisyHistorieDohod) = New HashSet(Of tblSpisyHistorieDohod)
    Public Overridable Property tblSpisyPlatbyDosles As ICollection(Of tblSpisyPlatbyDosle) = New HashSet(Of tblSpisyPlatbyDosle)
    Public Overridable Property tblSpisyRecordHistories As ICollection(Of tblSpisyRecordHistory) = New HashSet(Of tblSpisyRecordHistory)
    Public Overridable Property tblSpisyTypOsobies As ICollection(Of tblSpisyTypOsoby) = New HashSet(Of tblSpisyTypOsoby)

End Class
