Module Module_tblSpisyDohody
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_PA_INSTALLMENTS_VW)
        Dim tblOra As List(Of CZ_PA_INSTALLMENTS_VW)
        tblOra = _ora.CZ_PA_INSTALLMENTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_PA_INSTALLMENTS_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyDohodies.FirstOrDefault(Function(e) e.IDSrcSpisyDohody = ora.PA_INSTALLMENT_ID)
            If sql IsNot Nothing Then 'pro update
                If (DateDiff(sql.PaymentDate, ora.DUE_DATE) _
                 Or Comparison(sql.PaymentAmountAwait, ora.AMOUNT_INSTALLMENT) _
                 Or sql.PaymentAmountReal <> (ora.AMOUNT_INSTALLMENT - ora.UNPAID_AMOUNT_INSTALLMENT)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_PA_INSTALLMENTS_VW]->[tblSpisyDohody] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_PA_INSTALLMENTS_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function IDSpisu(_sql As SqlEntities, ACCOUNT_ID As Integer) As Integer
        Dim model = _sql.tblSpisies.FirstOrDefault(Function(e) e.IDSrcSpisy = ACCOUNT_ID)
        If model IsNot Nothing Then
            Return model.IDSpisu
        End If
        Return 0
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_PA_INSTALLMENTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyDohodies.FirstOrDefault(Function(e) e.IDSrcSpisyDohody = item.PA_INSTALLMENT_ID)
            If model IsNot Nothing Then 'update
                model.PaymentDate = item.DUE_DATE
                model.PaymentAmountAwait = item.AMOUNT_INSTALLMENT
                model.PaymentAmountReal = (item.AMOUNT_INSTALLMENT - item.UNPAID_AMOUNT_INSTALLMENT)
            Else 'insert
                Dim ID As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
                If ID > 0 Then
                    model = New tblSpisyDohody
                    model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                    model.IDSrcSpisyDohody = item.PA_INSTALLMENT_ID
                    model.PaymentDate = item.DUE_DATE
                    model.PaymentAmountAwait = item.AMOUNT_INSTALLMENT
                    model.PaymentAmountReal = (item.AMOUNT_INSTALLMENT - item.UNPAID_AMOUNT_INSTALLMENT)
                    _sql.tblSpisyDohodies.Add(model)
                Else
                    log.WriteEntry("tblSpisyDohody INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
                End If
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
            log.WriteEntry("tblSpisyDohody: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
