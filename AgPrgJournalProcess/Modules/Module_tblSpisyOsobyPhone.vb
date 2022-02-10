Module Module_tblSpisyOsobyPhone
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_SUBSCRIBER_PHONES_VW)
        Dim tblOra As List(Of CZ_SUBSCRIBER_PHONES_VW)
        tblOra = _ora.CZ_SUBSCRIBER_PHONES_VW.Distinct.ToList

        Dim list As New List(Of CZ_SUBSCRIBER_PHONES_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyOsobyPhones.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyPhone = ora.SUBSCRIBER_PHONE_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.PersPhone, ora.PHONE_NUMBER) _
                 Or Comparison(sql.rr_PhoneValidity, ora.VALID) _
                 Or Comparison(sql.PersPhoneMain, ora.MAIN_PHONE)) And sql.rr_Journal = 8 Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_SUBSCRIBER_PHONES_VW]->[tblSpisyOsobyPhone] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_SUBSCRIBER_PHONES_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function IDSpisyOsoby(_sql As SqlEntities, SUBSCRIBER_ID As Integer) As Integer
        Dim model = _sql.tblSpisyOsobies.FirstOrDefault(Function(e) e.IDSrcSpisyOsoby = SUBSCRIBER_ID)
        If model IsNot Nothing Then
            Return model.IDSpisyOsoby
        End If
        Return 0
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_SUBSCRIBER_PHONES_VW) As Integer
        Try
            Dim model = _sql.tblSpisyOsobyPhones.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyPhone = item.SUBSCRIBER_PHONE_ID)
            If model IsNot Nothing Then 'update
                model.PersPhone = item.PHONE_NUMBER
                model.rr_PhoneValidity = item.VALID
                model.PersPhoneMain = item.MAIN_PHONE
                'log.WriteEntry("UPDATE tblCreditor", EventLogEntryType.SuccessAudit)
            Else 'insert
                model = New tblSpisyOsobyPhone
                model.IDSpisyOsoby = IDSpisyOsoby(_sql, item.SUBSCRIBER_ID)
                model.IDSrcSpisyOsobyPhone = item.SUBSCRIBER_PHONE_ID
                model.PersPhone = item.PHONE_NUMBER
                model.rr_PhoneValidity = item.VALID
                model.PersPhoneMain = item.MAIN_PHONE
                model.rr_Journal = 8
                model.TimeCreated = Now
                model.TimeValidityUpdate = Now
                _sql.tblSpisyOsobyPhones.Add(model)
                'log.WriteEntry("INSERT tblCreditor", EventLogEntryType.SuccessAudit)
            End If
            _sql.SaveChanges()
            Return 4
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each eve In ex.EntityValidationErrors
                Dim tbl = eve.Entry.Entity.GetType().Name
                Dim state = eve.Entry.State
                For Each ve In eve.ValidationErrors
                    Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                    log.WriteEntry(err, EventLogEntryType.Error)
                Next
            Next
            Return 3
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            log.WriteEntry("tblSpisyOsobyPhone: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
