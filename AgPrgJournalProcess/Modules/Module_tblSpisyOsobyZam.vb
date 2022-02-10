Module Module_tblSpisyOsobyZam
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_SUBSCRIBER_CONTACTS_EMP_VW)
        Dim tblOra As List(Of CZ_SUBSCRIBER_CONTACTS_EMP_VW)
        tblOra = _ora.CZ_SUBSCRIBER_CONTACTS_EMP_VW.Distinct.ToList

        Dim list As New List(Of CZ_SUBSCRIBER_CONTACTS_EMP_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyOsobyZams.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyZam = ora.SUBSCRIBER_CONTACT_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.EmployerName, ora.LAST_NAME) _
                 Or Comparison(sql.EmployerCity, ora.CITY) _
                 Or Comparison(sql.EmployerStreet, ora.STREET) _
                 Or Comparison(sql.EmployerZipCode, ora.POSTAL_CODE) _
                 Or Comparison(sql.EmployerContactEmail, ora.EMAIL) _
                 Or Comparison(sql.EmployerContactPhone, ora.PHONE1) _
                 Or Comparison(sql.EmployerAddrFull, ora.ADDRESS_FULL)) And sql.rr_Journal = 8 Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_SUBSCRIBER_CONTACTS_EMP_VW]->[tblSpisyOsobyZam] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_SUBSCRIBER_CONTACTS_EMP_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_SUBSCRIBER_CONTACTS_EMP_VW) As Integer
        Try
            Dim model = _sql.tblSpisyOsobyZams.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyZam = item.SUBSCRIBER_CONTACT_ID)
            If model IsNot Nothing Then 'update


                model.EmployerName = item.LAST_NAME
                model.EmployerCity = item.CITY
                model.EmployerStreet = item.STREET
                model.EmployerZipCode = Left(item.POSTAL_CODE, 10)
                model.EmployerContactEmail = item.EMAIL
                model.EmployerContactPhone = item.PHONE1
                model.EmployerAddrFull = item.ADDRESS_FULL

                model.EmployerType = 2
            Else 'insert
                If item.POSTAL_CODE IsNot Nothing AndAlso item.POSTAL_CODE.Length > 10 Then
                    item.POSTAL_CODE = Left(item.POSTAL_CODE, 10)
                End If


                model = New tblSpisyOsobyZam
                model.IDSpisyOsoby = IDSpisyOsoby(_sql, item.SUBSCRIBER_ID)
                model.IDSrcSpisyOsobyZam = item.SUBSCRIBER_CONTACT_ID
                model.EmployerName = item.LAST_NAME
                model.EmployerCity = item.CITY
                model.EmployerStreet = item.STREET
                model.EmployerZipCode = Left(item.POSTAL_CODE, 10)
                model.EmployerContactEmail = item.EMAIL
                model.EmployerContactPhone = item.PHONE1
                model.EmployerAddrFull = item.ADDRESS_FULL
                model.rr_AddressValidity = 2
                model.rr_Journal = 8
                model.TimeCreated = Now
                model.TimeValidityUpdate = Now

                model.EmployerType = 2

                _sql.tblSpisyOsobyZams.Add(model)
            End If

            'tblSpisyOsobyZam: Cannot insert the value NULL into column 'rr_Journal', table 'TRACE.dbo.tblSpisyOsobyEmail'; column does not allow nulls. INSERT fails.
            'The statement has been terminated.
            _sql.SaveChanges()
            Return 4
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each eve In ex.EntityValidationErrors
                Dim tbl = eve.Entry.Entity.GetType().Name
                Dim state = eve.Entry.State
                For Each ve In eve.ValidationErrors
                    Dim err = ("Module_tblSpisyOsobyZam " & state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                    log.WriteEntry(err, EventLogEntryType.Error)
                Next
            Next
            Return 3
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            log.WriteEntry("tblSpisyOsobyZam: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
