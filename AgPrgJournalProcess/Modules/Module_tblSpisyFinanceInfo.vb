Module Module_tblSpisyFinanceInfo
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_ACCOUNT_DEBTS_VW)
        Dim tblOra As List(Of CZ_ACCOUNT_DEBTS_VW)
        tblOra = _ora.CZ_ACCOUNT_DEBTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_ACCOUNT_DEBTS_VW)


        'TODO InfoAmountBalance = Double, DEBT_BALANCE = Decimal
        'TODO InfoAmountBalance = Double, DEBT_BALANCE = Decimal
        'TODO InfoAmountBalanceUpdated = Double, DEBT_BALANCE_UPDATE = Decimal

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyFinanceInfoes.FirstOrDefault(Function(e) e.IDSrcInfo = ora.ACCOUNT_DEBT_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.InfoKind, ora.DEBT_SUBCLASS_NAME) _
                 Or Comparison(sql.InfoInvoice, ora.INVOICE_NUMBER) _
                 Or Comparison(sql.InfoAmount, ora.DEBT_AMOUNT) _
                 Or Comparison(sql.InfoAmountBalance, ora.DEBT_BALANCE) _
                 Or Comparison(sql.InfoAmountBalanceUpdated, ora.DEBT_BALANCE_UPDATE) _
                 Or DateDiff(sql.InfoDate, ora.DEBT_DATE) _
                 Or DateDiff(sql.InfoDueDate, ora.DEBT_DUE_DATE) _
                 Or Comparison(sql.InfoCurrency, ora.CURRENCY)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_ACCOUNT_DEBTS_VW]->[tblSpisyFinanceInfo] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_ACCOUNT_DEBTS_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_ACCOUNT_DEBTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyFinanceInfoes.FirstOrDefault(Function(e) e.IDSrcInfo = item.ACCOUNT_DEBT_ID)
            If model IsNot Nothing Then 'update
                model.InfoKind = item.DEBT_SUBCLASS_NAME
                model.InfoInvoice = item.INVOICE_NUMBER
                model.InfoAmount = item.DEBT_AMOUNT
                model.InfoAmountBalance = item.DEBT_BALANCE
                model.InfoAmountBalanceUpdated = item.DEBT_BALANCE_UPDATE
                model.InfoDate = item.DEBT_DATE
                model.InfoDueDate = item.DEBT_DUE_DATE
                model.InfoCurrency = item.CURRENCY
            Else 'insert
                Dim id As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
                If id > 0 Then
                    model = New tblSpisyFinanceInfo
                    model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                    model.IDSrcInfo = item.ACCOUNT_DEBT_ID
                    model.InfoKind = item.DEBT_SUBCLASS_NAME
                    model.InfoInvoice = item.INVOICE_NUMBER
                    model.InfoAmount = item.DEBT_AMOUNT
                    model.InfoAmountBalance = item.DEBT_BALANCE
                    model.InfoAmountBalanceUpdated = item.DEBT_BALANCE_UPDATE
                    model.InfoDate = item.DEBT_DATE
                    model.InfoDueDate = item.DEBT_DUE_DATE
                    model.InfoCurrency = item.CURRENCY
                    _sql.tblSpisyFinanceInfoes.Add(model)
                Else
                    log.WriteEntry("tblSpisyFinanceInfo INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
                    Return 2
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
            'Dim values = Build(item)
            log.WriteEntry("tblSpisyFinanceInfo: ACCOUNT_DEBT_ID = " & item.ACCOUNT_DEBT_ID & ",  ACCOUNT_ID = " & item.ACCOUNT_ID & " - " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
