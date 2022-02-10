Module Module_tblSpisyHistorieDohod
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_PA_INSTALLMENT_HISTORY_VW)
        Dim tblOra As List(Of CZ_PA_INSTALLMENT_HISTORY_VW)
        tblOra = _ora.CZ_PA_INSTALLMENT_HISTORY_VW.Distinct.ToList

        Dim list As New List(Of CZ_PA_INSTALLMENT_HISTORY_VW)

        For Each ora In tblOra
            Dim ID = IDSpisu(_sql, ora.ACCOUNT_ID)
            Dim sql = _sql.tblSpisyHistorieDohods.FirstOrDefault(Function(e) e.IDSrcSpisyHistorieDohod = ora.PA_INSTANCE_ID And e.IDSpisu = ID)
            If sql IsNot Nothing Then 'pro update
                If (DateDiff(sql.DateSK, ora.CREATION_DATE) _
                 Or Comparison(sql.Amount, ora.AMOUNT_PA) _
                 Or Comparison(sql.CountOfPaymentsSK, ora.PA_INSTALLMENTS_COUNT) _
                 Or sql.rr_PAValidityType <> IDOrder(_sql, ora.STAUS) _
                 Or Comparison(sql.rr_PAType, ora.PA_TYPE_ID)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_PA_INSTALLMENT_HISTORY_VW]->[tblSpisyHistorieDohod] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_PA_INSTALLMENT_HISTORY_VW)) As List(Of Integer)
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

    Public Function IDOrder(_sql As SqlEntities, STAUS As String) As Short
        Dim model = _sql.tblRegisterRestrictions.FirstOrDefault(Function(e) e.Val2 = STAUS)
        If model IsNot Nothing Then
            Return CShort(model.IDOrder)
        End If
        Return 0
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_PA_INSTALLMENT_HISTORY_VW) As Integer
        Try
            Dim ID As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
            Dim model = _sql.tblSpisyHistorieDohods.FirstOrDefault(Function(e) e.IDSrcSpisyHistorieDohod = item.PA_INSTANCE_ID And e.IDSpisu = ID)
            If model IsNot Nothing Then 'update
                model.DateSK = item.CREATION_DATE
                model.Amount = item.AMOUNT_PA
                model.CountOfPaymentsSK = item.PA_INSTALLMENTS_COUNT
                model.rr_PAValidityType = IDOrder(_sql, item.STAUS)
                model.rr_PAType = item.PA_TYPE_ID
            Else 'insert
                If ID > 0 Then
                    model = New tblSpisyHistorieDohod
                    model.IDSpisu = ID
                    model.IDSrcSpisyHistorieDohod = item.PA_INSTANCE_ID
                    model.DateSK = item.CREATION_DATE
                    model.Amount = item.AMOUNT_PA
                    model.CountOfPaymentsSK = item.PA_INSTALLMENTS_COUNT
                    model.rr_PAValidityType = IDOrder(_sql, item.STAUS)
                    model.rr_PAType = item.PA_TYPE_ID
                    _sql.tblSpisyHistorieDohods.Add(model)
                Else
                    log.WriteEntry("tblSpisyHistorieDohod INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisyHistorieDohod: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
