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

Partial Public Class tblSpisyOsobyAdresy
    Public Property IDSpisyOsobyAdresy As Integer
    Public Property IDSpisyOsoby As Integer
    Public Property IDSrcSpisyOsobyAdresy As Nullable(Of Long)
    Public Property PersCity As String
    Public Property PersHouseNum As String
    Public Property PersStreet As String
    Public Property PersZipCode As String
    Public Property PersRegion As String
    Public Property PersAddressVisited As Nullable(Of Boolean)
    Public Property PersMainAddress As Nullable(Of Boolean)
    Public Property PersAddressForItinerary As Nullable(Of Boolean)
    Public Property VisitDatePlan As Nullable(Of Date)
    Public Property VisitDatePlanNoChange As Boolean
    Public Property rr_AddressValidity As Nullable(Of Short)
    Public Property rr_AddressType As Nullable(Of Short)
    Public Property GPSLat As String
    Public Property GPSLng As String
    Public Property GPSValid As Boolean
    Public Property PersAddrFull As String
    Public Property AddrFVisitComment As String
    Public Property NCName As String
    Public Property NCSurname As String
    Public Property NCEmail As String
    Public Property NCEmailValid As Nullable(Of Boolean)
    Public Property NCPhone As String
    Public Property NCPhoneValid As Nullable(Of Short)
    Public Property NCrr_PersType As Nullable(Of Short)
    Public Property NextContact As Boolean
    Public Property New_PersMainAddress As Nullable(Of Boolean)
    Public Property New_rr_AddressType As Nullable(Of Short)
    Public Property New_rr_AddressValidity As Nullable(Of Short)
    Public Property New_PersAddressVisited As Nullable(Of Boolean)
    Public Property rr_Journal As Byte
    Public Property TStamp As Byte()
    Public Property LastUpdate As Nullable(Of Date)
    Public Property GPSValidDetail As Byte
    Public Property TimeValidityUpdate As Date
    Public Property TimeCreated As Date

End Class
