Imports System.Data.Entity.Core.Objects
Imports System.Text

Module Module_CZ_SUBS_ADDRESSES_TBL
    Public isError As Boolean = False

    Public Sub Transfer(_LastTStamp As Long, _NewTStamp As Long, Sql As SqlEntities, log As EventLog)
        Dim ID = 0

        Dim items = Sql.sp_INT_NewAddresses(_LastTStamp, _NewTStamp).ToList
        For Each m In items
            Try
                Using Ora As New OracleEntities
                    ID = m.IDSpisyOsobyAdresy

                    If m.rr_Journal = 3 Or m.rr_Journal = 4 Then
                        Dim exist = Ora.AGCZ_SUBS_ADDRESSES_TBL.FirstOrDefault(Function(e) e.TRC_ADDRESSES_ID = m.IDSrcSpisyOsobyAdresy)
                        If exist IsNot Nothing Then

                        Else
                            Dim model As New AGCZ_SUBS_ADDRESSES_TBL
                            If m.PersHouseNum IsNot Nothing Then
                                For i As Integer = m.PersHouseNum.Length To 1 Step -1
                                    Dim bytes As Byte = Encoding.UTF8.GetByteCount(m.PersHouseNum)
                                    If bytes > 10 Then
                                        m.PersHouseNum = Left(m.PersHouseNum, i)
                                    Else
                                        Exit For
                                    End If
                                Next
                            End If

                            If m.PersZipCode IsNot Nothing And Not m.PersZipCode = "" And Not IsDBNull(m.PersZipCode) Then
                                For i As Integer = m.PersZipCode.Length To 1 Step -1
                                    Dim bytes As Integer = Encoding.UTF8.GetByteCount(m.PersZipCode)
                                    If bytes > 10 Then
                                        m.PersZipCode = Left(m.PersZipCode, i)
                                    Else
                                        Exit For
                                    End If
                                Next
                            Else
                                m.PersZipCode = "00000"
                            End If

                            model.SUBSCRIBER_ID = m.IDSrcSpisyOsoby
                            model.SUBSCRIBER_ADDRESS_ID = m.IDSrcSpisyOsobyAdresy
                            model.STREET = m.PersStreet
                            model.STREET_NUMBER = m.PersHouseNum
                            model.CITY = m.PersCity
                            model.POSTAL_CODE = m.PersZipCode
                            model.ADDRESS_TYPE = m.rr_AddressType
                            model.MAIN_ADDRESS = m.PersMainAddress
                            model.PERS_REGION = m.PersRegion

                            model.VALID = m.rr_AddressValidity

                            model.CREATION_DATE = m.TimeCreated
                            model.LAST_UPDATE_DATE = m.TimeValidityUpdate

                            model.TRC_ADDRESSES_ID = m.IDSpisyOsobyAdresy
                            model.TRC_TSTAMP = Convert.ToDecimal(m.TStamp)
                            model.TRC_RR_JOURNAL = (m.rr_Journal - 2)

                            Ora.AGCZ_SUBS_ADDRESSES_TBL.Add(model)

                            Ora.SaveChanges()
                        End If
                    Else
                        Dim model As New AGCZ_SUBS_ADDRESSES_TBL
                        If m.PersHouseNum IsNot Nothing Then
                            For i As Integer = m.PersHouseNum.Length To 1 Step -1
                                Dim bytes As Byte = Encoding.UTF8.GetByteCount(m.PersHouseNum)
                                If bytes > 10 Then
                                    m.PersHouseNum = Left(m.PersHouseNum, i)
                                Else
                                    Exit For
                                End If
                            Next
                        End If

                        If m.PersZipCode IsNot Nothing And Not m.PersZipCode = "" And Not IsDBNull(m.PersZipCode) Then
                            For i As Integer = m.PersZipCode.Length To 1 Step -1
                                Dim bytes As Integer = Encoding.UTF8.GetByteCount(m.PersZipCode)
                                If bytes > 10 Then
                                    m.PersZipCode = Left(m.PersZipCode, i)
                                Else
                                    Exit For
                                End If
                            Next
                        Else
                            m.PersZipCode = "00000"
                        End If

                        model.SUBSCRIBER_ID = m.IDSrcSpisyOsoby
                        model.SUBSCRIBER_ADDRESS_ID = m.IDSrcSpisyOsobyAdresy
                        model.STREET = m.PersStreet
                        model.STREET_NUMBER = m.PersHouseNum
                        model.CITY = m.PersCity
                        model.POSTAL_CODE = m.PersZipCode
                        model.ADDRESS_TYPE = m.rr_AddressType
                        model.MAIN_ADDRESS = m.PersMainAddress
                        model.PERS_REGION = m.PersRegion

                        model.VALID = m.rr_AddressValidity

                        model.CREATION_DATE = m.TimeCreated
                        model.LAST_UPDATE_DATE = m.TimeValidityUpdate

                        model.TRC_ADDRESSES_ID = m.IDSpisyOsobyAdresy
                        model.TRC_TSTAMP = Convert.ToDecimal(m.TStamp)
                        model.TRC_RR_JOURNAL = m.rr_Journal

                        Ora.AGCZ_SUBS_ADDRESSES_TBL.Add(model)

                        Ora.SaveChanges()
                    End If

                End Using
            Catch ex As Entity.Validation.DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim tbl = eve.Entry.Entity.GetType().Name
                    Dim state = eve.Entry.State
                    For Each ve In eve.ValidationErrors
                        Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                        log.WriteEntry(err, EventLogEntryType.Error)
                    Next
                Next
                isError = True
            Catch ex As Exception
                'ORA-00001: unique constraint 
                While ex.InnerException IsNot Nothing
                    ex = ex.InnerException
                End While
                If Not ex.Message.Contains("ORA-00001") Then
                    log.WriteEntry("AGCZ_SUBS_ADDRESSES_TBL TRC_ADDRESSES_ID = " & ID & ": " & ex.Message, EventLogEntryType.Error)
                    isError = True
                End If
            End Try
        Next

        log.WriteEntry("AGCZ_SUBS_ADDRESSES_TBL INSERT " & items.Count & " RECORDS, LastTStamp = " & _LastTStamp.ToString & ", NewTStamp = " & _NewTStamp.ToString, EventLogEntryType.Information)
    End Sub

    Public Sub Update(log As EventLog)
        Try
            Dim Ora As New OracleEntities
            Dim Sql As New SqlEntities
            Dim tbl = Ora.AGCZ_SUBS_ADDRESSES_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 6 Or e.TRC_RR_JOURNAL = 7).ToList

            For Each i In tbl
                Dim model = Sql.tblSpisyOsobyAdresies.FirstOrDefault(Function(e) e.IDSpisyOsobyAdresy = i.TRC_ADDRESSES_ID)

                If i.SUBSCRIBER_ADDRESS_ID IsNot Nothing Then
                    If model IsNot Nothing Then
                        If i.TRC_RR_JOURNAL = 6 Then
                            model.IDSrcSpisyOsobyAdresy = i.SUBSCRIBER_ADDRESS_ID
                            If model.rr_Journal = 1 Then
                                model.rr_Journal = 8
                            ElseIf model.rr_Journal = 3 Then
                                model.rr_Journal = 2
                            End If
                        Else
                            If model.rr_Journal = 2 Then
                                model.rr_Journal = 8
                            ElseIf model.rr_Journal = 4 Then
                                model.rr_Journal = 2
                            End If
                        End If
                    End If

                    i.TRC_RR_JOURNAL = 8
                Else
                    i.TRC_RR_JOURNAL = 9
                End If
            Next
            Sql.SaveChanges()
            Ora.SaveChanges()

            Dim o = Ora.AGCZ_SUBS_ADDRESSES_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 8).ToList
            For Each i In o
                Ora.AGCZ_SUBS_ADDRESSES_TBL.Remove(i)
            Next
            Ora.SaveChanges()
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each eve In ex.EntityValidationErrors
                Dim tbl = eve.Entry.Entity.GetType().Name
                Dim state = eve.Entry.State
                For Each ve In eve.ValidationErrors
                    Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                    log.WriteEntry(err, EventLogEntryType.Error)
                Next
            Next
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            If Not ex.Message.Contains("ORA-00001") Then
                log.WriteEntry("AGCZ_SUBS_ADDRESSES_TBL UPDATE: " & ex.Message, EventLogEntryType.Error)
            End If
        End Try
    End Sub
End Module
