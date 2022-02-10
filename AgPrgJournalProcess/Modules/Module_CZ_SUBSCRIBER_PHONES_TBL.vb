Imports System.Data.Entity.Core.Objects

Module Module_CZ_SUBSCRIBER_PHONES_TBL
    Public isError As Boolean = False

    Public Sub Transfer(_LastTStamp As Long, _NewTStamp As Long, Sql As SqlEntities, log As EventLog)
        Dim ID = 0

        Dim items As List(Of sp_INT_NewPhones_Result) = Sql.sp_INT_NewPhones(_LastTStamp, _NewTStamp).ToList

        For Each m In items
            Try
                Using Ora As New OracleEntities
                    ID = m.IDSpisyOsobyPhone
                    If m.rr_Journal = 3 Or m.rr_Journal = 4 Then
                        Dim exist = Ora.AGCZ_SUBS_PHONES_TBL.FirstOrDefault(Function(e) e.TRC_PHONE_ID = m.IDSpisyOsobyPhone)
                        If exist IsNot Nothing Then

                        Else
                            Dim model As New AGCZ_SUBS_PHONES_TBL
                            model.SUBSCRIBER_ID = m.IDSrcSpisyOsoby
                            model.SUBSCRIBER_PHONE_ID = m.IDSrcSpisyOsobyPhone
                            model.PHONE_NUMBER = m.PersPhone
                            model.VALID = m.rr_PhoneValidity
                            model.MAIN_PHONE = If(m.PersPhoneMain, 1, 0)
                            model.VALID = m.rr_PhoneValidity
                            model.FV_ORIGIN = m.FV_ORIGIN
                            model.TRC_PHONE_ID = m.IDSpisyOsobyPhone
                            model.TRC_PHONE_COMMENT = m.PhoneFVisitComment
                            model.TRC_TSTAMP = Convert.ToDecimal(m.TStamp)
                            model.TRC_RR_JOURNAL = m.rr_Journal

                            model.CREATION_DATE = m.TimeCreated
                            model.LAST_UPDATE_DATE = m.TimeValidityUpdate

                            Ora.AGCZ_SUBS_PHONES_TBL.Add(model)

                            Ora.SaveChanges()
                        End If
                    Else
                        Dim model As New AGCZ_SUBS_PHONES_TBL
                        model.SUBSCRIBER_ID = m.IDSrcSpisyOsoby
                        model.SUBSCRIBER_PHONE_ID = m.IDSrcSpisyOsobyPhone
                        model.PHONE_NUMBER = m.PersPhone
                        model.VALID = m.rr_PhoneValidity
                        model.MAIN_PHONE = If(m.PersPhoneMain, 1, 0)
                        model.VALID = m.rr_PhoneValidity
                        model.FV_ORIGIN = m.FV_ORIGIN
                        model.TRC_PHONE_ID = m.IDSpisyOsobyPhone
                        model.TRC_PHONE_COMMENT = m.PhoneFVisitComment
                        model.TRC_TSTAMP = Convert.ToDecimal(m.TStamp)
                        model.TRC_RR_JOURNAL = m.rr_Journal

                        model.CREATION_DATE = m.TimeCreated
                        model.LAST_UPDATE_DATE = m.TimeValidityUpdate

                        Ora.AGCZ_SUBS_PHONES_TBL.Add(model)

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
                    log.WriteEntry("AGCZ_SUBS_PHONES_TBL TRC_PHONE_ID = " & ID & ": " & ex.Message, EventLogEntryType.Error)
                    isError = True
                End If
            End Try
        Next

        log.WriteEntry("AGCZ_SUBS_PHONES_TBL INSERT " & items.Count & " RECORDS, LastTStamp = " & _LastTStamp.ToString & ", NewTStamp = " & _NewTStamp.ToString, EventLogEntryType.Information)
    End Sub

    Public Sub Update(log As EventLog)
        Try
            Dim Ora As New OracleEntities
            Dim Sql As New SqlEntities

            Dim tbl = Ora.AGCZ_SUBS_PHONES_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 6 Or e.TRC_RR_JOURNAL = 7).ToList
            For Each i In tbl
                Dim model = Sql.tblSpisyOsobyPhones.FirstOrDefault(Function(e) e.IDSpisyOsobyPhone = i.TRC_PHONE_ID)
                If i.SUBSCRIBER_PHONE_ID IsNot Nothing Then
                    If model IsNot Nothing Then
                        If i.TRC_RR_JOURNAL = 6 Then
                            model.IDSrcSpisyOsobyPhone = i.SUBSCRIBER_PHONE_ID
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

            Dim o = Ora.AGCZ_SUBS_PHONES_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 8).ToList
            For Each i In o
                Ora.AGCZ_SUBS_PHONES_TBL.Remove(i)
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
                log.WriteEntry("AGCZ_SUBS_PHONES_TBL UPDATE: " & ex.Message, EventLogEntryType.Error)
            End If
        End Try
    End Sub
End Module
