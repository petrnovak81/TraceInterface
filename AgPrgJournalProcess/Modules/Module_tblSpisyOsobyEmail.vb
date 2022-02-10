Module Module_tblSpisyOsobyEmail
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_SUBSCRIBER_CONTACTS_EML_VW)
        Dim tblOra As List(Of CZ_SUBSCRIBER_CONTACTS_EML_VW)
        tblOra = _ora.CZ_SUBSCRIBER_CONTACTS_EML_VW.Distinct.ToList

        Dim list As New List(Of CZ_SUBSCRIBER_CONTACTS_EML_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyOsobyEmails.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyEmail = ora.SUBSCRIBER_CONTACT_ID)

            'TODO PersEmailValid = Boolean, EMAIL_FLAG = Decimal

            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.PersEmail, ora.EMAIL) And sql.rr_Journal = 8) Then

                    'Or sql.PersEmailValid <> ora.EMAIL_FLAG)

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_SUBSCRIBER_CONTACTS_EML_VW]->[tblSpisyOsobyEmail] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_SUBSCRIBER_CONTACTS_EML_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_SUBSCRIBER_CONTACTS_EML_VW) As Integer
        Try
            Dim model = _sql.tblSpisyOsobyEmails.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyEmail = item.SUBSCRIBER_CONTACT_ID)
            If model IsNot Nothing Then 'update
                model.PersEmail = item.EMAIL
                model.PersEmailValid = If(item.EMAIL_FLAG < 1, 0, 1)
                'log.WriteEntry("UPDATE tblCreditor", EventLogEntryType.SuccessAudit)
            Else 'insert
                model = New tblSpisyOsobyEmail
                model.IDSpisyOsoby = IDSpisyOsoby(_sql, item.SUBSCRIBER_ID)
                model.IDSrcSpisyOsobyEmail = item.SUBSCRIBER_CONTACT_ID
                model.PersEmail = item.EMAIL
                model.PersEmailValid = If(item.EMAIL_FLAG < 1, 0, 1)
                model.rr_Journal = 8
                model.TimeCreated = Now
                model.TimeValidityUpdate = Now
                model.PersEmailMain = 0

                _sql.tblSpisyOsobyEmails.Add(model)
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
            log.WriteEntry("tblSpisyOsobyEmail: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
