
Module Module_tblSpisy
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_ACCOUNTS_VW)
        Dim list As New List(Of CZ_ACCOUNTS_VW)
        Dim tblOra As List(Of CZ_ACCOUNTS_VW)
        tblOra = _ora.CZ_ACCOUNTS_VW.Distinct.ToList

        'Dim ora = tblOra.FirstOrDefault()
        'Dim sql = _sql.tblSpisies.FirstOrDefault(Function(e) e.IDSrcSpisy = ora.ACCOUNT_ID)
        'If sql IsNot Nothing Then
        '    log.WriteEntry(sql.VisitDateSentFromMother & " " & ora.FV_PASSING_ON_DATE, EventLogEntryType.Information)
        'End If


        For Each ora In tblOra
            Dim sql = _sql.tblSpisies.FirstOrDefault(Function(e) e.IDSrcSpisy = ora.ACCOUNT_ID)
            If sql IsNot Nothing Then 'pro update
                If ora.COMMISSION Is Nothing Then
                    ora.COMMISSION = 0
                End If

                If (Comparison(sql.ACC, ora.ACCOUNT_ID) _
        Or Comparison(sql.IDUser, ora.IDUSR) _
        Or Comparison(sql.ActualDebit, ora.TOTAL_BALANCE) _
        Or Comparison(sql.Principal, ora.DEBT_BALANCE_SUM) _
        Or Comparison(sql.Interest, ora.INTEREST_BALANCE_SUM) _
        Or Comparison(sql.Costs, ora.COST_BALANCE_SUM) _
        Or Comparison(sql.YetPaid, ora.PAYMENTS_AMOUNT_SUM) _
        Or Comparison(sql.KindOfEnforcement, ora.PRODUCT_NUMBER) _
        Or Comparison(sql.AccountNumber, ora.BANK_ACCOUNT_NUM) _
        Or Comparison(sql.BankCode, ora.BANK_CODE) _
        Or Comparison(sql.VariableSymbol, ora.VARIABLE_SYMBOL) _
        Or Comparison(sql.SpecificSymbol, ora.SPECIFIC_SYMBOL) _
        Or DateDiff(sql.LastPaymentDate, ora.LAST_PAYMENT_DATE) _
        Or Comparison(sql.LastPaymentAmount, ora.LAST_PAYMENT_AMOUNT) _
        Or DateDiff(sql.LastCallDate, ora.LAST_CALL_DATE) _
        Or Comparison(sql.LastCallText, ora.LAST_CALL_TEXT) _
        Or Comparison(sql.TaskForIP, ora.LAST_INSTRUCTION_FOR_FV) _
        Or Comparison(sql.FileAction, ora.CAMPAIGN) _
        Or DateDiff(sql.VisitDateSentFromMother, ora.FV_PASSING_ON_DATE) _
        Or DateDiff(sql.VisitDateSentToMother, ora.TAKE_OVER_DATE) _
        Or Comparison(sql.FirstFieldVisitUpToDay, ora.FIRST_FV_DEADLINE_DAYS) _
        Or Comparison(sql.NextFieldVisitUpToDay, ora.NEXT_FV_DEADLINE_DAYS) _
        Or DateDiff(sql.VisitDateDXSK, ora.PA_INSTALMENT_BREAK_DATE_FV) _
        Or DateDiff(sql.DateReturnToCreditor, ora.CE_RETURN_DATE) _
        Or DateDiff(sql.DateLapse, ora.LAPSE_DATE) _
        Or Comparison(sql.SumPaymentFromVisits, ora.PAYMENTS_AMOUNT_FV) _
        Or Comparison(sql.ConditionSK, ora.PA_STATUS_FV) _
        Or Comparison(sql.TypeSK, ora.PA_TYPE_ID_FV) _
        Or Comparison(sql.TotalCountSK, ora.PA_COUNT) _
        Or sql.PardonCampaign <> If(ora.CE_DEBT_REMISSION_STATUS = "N", False, True) _
        Or Comparison(sql.MaxCountPayments, ora.PA_MAX_PERIODS) _
        Or Comparison(sql.DOHL, ora.DOHL) _
        Or Comparison(sql.ActualActions, ora.PROBIHAJICI_AKCE) _
        Or Comparison(sql.Commission, ora.COMMISSION) _
        Or DateDiff(sql.FV_TerminationDate, ora.FV_TERMINATION_DATE)) Then

                    'Dim date1 As Date = ora.FV_PASSING_ON_DATE
                    'Dim d1 As System.DateTime = New System.DateTime(date1.Year, date1.Month, date1.Day, date1.Hour, date1.Minute, date1.Second)

                    'Dim date2 As Date = sql.VisitDateSentFromMother
                    'Dim d2 As System.DateTime = New System.DateTime(date2.Year, date2.Month, date2.Day, date2.Hour, date2.Minute, date2.Second)

                    'Dim diff As System.TimeSpan = d1 - d2
                    'Dim min = diff.TotalMinutes

                    'log.WriteEntry(min.ToString, EventLogEntryType.Information)

                    list.Add(ora)
                End If
            Else 'pro insert
                If ora.COMMISSION Is Nothing Then
                    ora.COMMISSION = 0
                End If

                list.Add(ora)
            End If
        Next

        log.WriteEntry("UPDATE/INSERT [CZ_ACCOUNTS_VW]->[tblSpisy] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_ACCOUNTS_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function IDCreditor(_sql As SqlEntities, CLIENT_ID As Integer) As Integer
        Dim model = _sql.tblCreditors.FirstOrDefault(Function(e) e.IDSrcCreditor = CLIENT_ID)
        If model IsNot Nothing Then
            Return model.IDCreditor
        End If
        Return 0
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_ACCOUNTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisies.FirstOrDefault(Function(e) e.IDSrcSpisy = item.ACCOUNT_ID)
            If model IsNot Nothing Then 'update

                If item.COMMISSION Is Nothing Then
                    item.COMMISSION = 0
                End If

                model.ACC = item.ACCOUNT_ID
                model.IDUser = item.IDUSR
                model.ActualDebit = item.TOTAL_BALANCE
                model.IDCreditor = IDCreditor(_sql, item.CLIENT_ID)
                model.Principal = item.DEBT_BALANCE_SUM
                model.Interest = item.INTEREST_BALANCE_SUM
                model.Costs = item.COST_BALANCE_SUM
                model.YetPaid = item.PAYMENTS_AMOUNT_SUM
                model.KindOfEnforcement = item.PRODUCT_NUMBER
                model.AccountNumber = item.BANK_ACCOUNT_NUM
                model.BankCode = item.BANK_CODE
                model.VariableSymbol = item.VARIABLE_SYMBOL
                model.SpecificSymbol = item.SPECIFIC_SYMBOL
                model.LastPaymentDate = item.LAST_PAYMENT_DATE
                model.LastPaymentAmount = item.LAST_PAYMENT_AMOUNT
                model.LastCallDate = item.LAST_CALL_DATE
                model.LastCallText = item.LAST_CALL_TEXT
                model.TaskForIP = item.LAST_INSTRUCTION_FOR_FV
                model.FileAction = item.CAMPAIGN
                model.VisitDateSentFromMother = item.FV_PASSING_ON_DATE
                model.VisitDateSentToMother = item.TAKE_OVER_DATE
                model.FirstFieldVisitUpToDay = item.FIRST_FV_DEADLINE_DAYS
                model.NextFieldVisitUpToDay = item.NEXT_FV_DEADLINE_DAYS
                model.VisitDateDXSK = item.PA_INSTALMENT_BREAK_DATE_FV
                model.DateReturnToCreditor = item.CE_RETURN_DATE
                model.DateLapse = item.LAPSE_DATE
                model.SumPaymentFromVisits = item.PAYMENTS_AMOUNT_FV
                model.ConditionSK = item.PA_STATUS_FV
                model.TypeSK = item.PA_TYPE_ID_FV
                model.TotalCountSK = item.PA_COUNT
                model.PardonCampaign = If(item.CE_DEBT_REMISSION_STATUS = "N", False, True)
                model.MaxCountPayments = item.PA_MAX_PERIODS
                model.DOHL = item.DOHL
                model.ActualActions = item.PROBIHAJICI_AKCE
                model.FV_TerminationDate = item.FV_TERMINATION_DATE
                model.Commission = CDbl(item.COMMISSION)
            Else 'insert
                Dim ID As Integer = IDCreditor(_sql, item.CLIENT_ID)
                If ID > 0 Then
                    If item.COMMISSION Is Nothing Then
                        item.COMMISSION = 0
                    End If

                    model = New tblSpisy
                    model.IDSrcSpisy = item.ACCOUNT_ID
                    model.ACC = item.ACCOUNT_ID
                    model.IDUser = item.IDUSR
                    model.ActualDebit = item.TOTAL_BALANCE
                    model.IDCreditor = ID
                    model.Principal = item.DEBT_BALANCE_SUM
                    model.Interest = item.INTEREST_BALANCE_SUM
                    model.Costs = item.COST_BALANCE_SUM
                    model.YetPaid = item.PAYMENTS_AMOUNT_SUM
                    model.KindOfEnforcement = item.PRODUCT_NUMBER
                    model.AccountNumber = item.BANK_ACCOUNT_NUM
                    model.BankCode = item.BANK_CODE
                    model.VariableSymbol = item.VARIABLE_SYMBOL
                    model.SpecificSymbol = item.SPECIFIC_SYMBOL
                    model.LastPaymentDate = item.LAST_PAYMENT_DATE
                    model.LastPaymentAmount = item.LAST_PAYMENT_AMOUNT
                    model.LastCallDate = item.LAST_CALL_DATE
                    model.LastCallText = item.LAST_CALL_TEXT
                    model.TaskForIP = item.LAST_INSTRUCTION_FOR_FV
                    model.FileAction = item.CAMPAIGN
                    model.VisitDateSentFromMother = item.FV_PASSING_ON_DATE
                    model.VisitDateSentToMother = item.TAKE_OVER_DATE
                    model.FirstFieldVisitUpToDay = item.FIRST_FV_DEADLINE_DAYS
                    model.NextFieldVisitUpToDay = item.NEXT_FV_DEADLINE_DAYS
                    model.VisitDateDXSK = item.PA_INSTALMENT_BREAK_DATE_FV
                    model.DateReturnToCreditor = item.CE_RETURN_DATE
                    model.DateLapse = item.LAPSE_DATE
                    model.SumPaymentFromVisits = item.PAYMENTS_AMOUNT_FV
                    model.ConditionSK = item.PA_STATUS_FV
                    model.TypeSK = item.PA_TYPE_ID_FV
                    model.TotalCountSK = item.PA_COUNT
                    model.PardonCampaign = If(item.CE_DEBT_REMISSION_STATUS = "N", False, True)
                    model.MaxCountPayments = item.PA_MAX_PERIODS
                    model.rr_Currency = "CZK"
                    model.FirstVisitExecuted = 0
                    model.MaxCommission = 0
                    model.FirstVisitFee = 0
                    model.rr_State = 1
                    model.LL_LastLapse = 0
                    model.DOHL = item.DOHL
                    model.ActualActions = item.PROBIHAJICI_AKCE
                    model.FV_TerminationDate = item.FV_TERMINATION_DATE
                    model.Commission = CDbl(item.COMMISSION)

                    _sql.tblSpisies.Add(model)
                Else
                    log.WriteEntry("tblSpisy INSERT: FOR CLIENT_ID = " & item.CLIENT_ID & " IDCreditor NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisy: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
