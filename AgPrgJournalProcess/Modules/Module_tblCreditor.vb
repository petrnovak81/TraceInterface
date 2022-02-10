Module Module_tblCreditor
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_CLIENTS_VW)
        Dim tblOra As List(Of CZ_CLIENTS_VW)
        tblOra = _ora.CZ_CLIENTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_CLIENTS_VW)
        For Each ora In tblOra
            Dim sql = _sql.tblCreditors.FirstOrDefault(Function(e) e.IDSrcCreditor = ora.CLIENT_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.CreditorName, ora.NAME) _
                 Or Comparison(sql.CreditorAddrFull, ora.ADDRESS)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_CLIENTS_VW]->[tblCreditor] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_CLIENTS_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_CLIENTS_VW) As Integer
        Try
            Dim model = _sql.tblCreditors.FirstOrDefault(Function(e) e.IDSrcCreditor = item.CLIENT_ID)
            If model IsNot Nothing Then 'update
                model.CreditorName = item.NAME
                model.CreditorAlias = If(IsDBNull(item.NAME_ALIAS), item.NAME, item.NAME_ALIAS)
                model.CreditorAddrFull = item.ADDRESS
            Else 'insert
                model = New tblCreditor
                model.IDSrcCreditor = item.CLIENT_ID
                model.CreditorName = item.NAME
                model.CreditorAlias = If(IsDBNull(item.NAME_ALIAS), item.NAME, item.NAME_ALIAS)
                model.CreditorAddrFull = item.ADDRESS

                _sql.tblCreditors.Add(model)
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
            log.WriteEntry("tblCreditor: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
