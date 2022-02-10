Module Module_tblSpisyPlatbyDosle
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_PAYMENTS_VW)
        Dim tblOra As List(Of CZ_PAYMENTS_VW)
        tblOra = _ora.CZ_PAYMENTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_PAYMENTS_VW)

        'TODO Amount = Byte, BEFORE_FV_FLAG = Decimal
        'TODO rr_PaymentLabel = Byte, BEFORE_FV_FLAG = Decimal
        'TODO OverPayment = Double, SOLD = Decimal

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyPlatbyDosles.FirstOrDefault(Function(e) e.IDSrcSpisyPlatbyDosle = ora.PAYMENT_ID)
            If sql IsNot Nothing Then 'pro update
                If (sql.Amount.ToString <> ora.AMOUNT.ToString _
                 Or Comparison(sql.rr_PaymentLabel, ora.BEFORE_FV_FLAG) _
                 Or Comparison(sql.OverPayment, ora.SOLD.ToString)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_PAYMENTS_VW]->[tblSpisyPlatbyDosle] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_PAYMENTS_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_PAYMENTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyPlatbyDosles.FirstOrDefault(Function(e) e.IDSrcSpisyPlatbyDosle = item.PAYMENT_ID)
            If model IsNot Nothing Then 'update
                model.PaymentDate = item.PAYMENT_DATE
                model.Amount = item.AMOUNT
                model.rr_PaymentLabel = item.BEFORE_FV_FLAG
                model.OverPayment = item.SOLD
            Else 'insert
                Dim ID As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
                If ID > 0 Then
                    model = New tblSpisyPlatbyDosle
                    model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                    model.IDSrcSpisyPlatbyDosle = item.PAYMENT_ID
                    model.PaymentDate = item.PAYMENT_DATE
                    model.Amount = item.AMOUNT
                    model.rr_PaymentLabel = item.BEFORE_FV_FLAG
                    model.OverPayment = item.SOLD
                    _sql.tblSpisyPlatbyDosles.Add(model)
                Else
                    log.WriteEntry("tblSpisyPlatbyDosle INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisyPlatbyDosle: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
