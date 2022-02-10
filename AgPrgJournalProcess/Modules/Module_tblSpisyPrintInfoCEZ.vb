Module Module_tblSpisyPrintInfoCEZ
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_PRINT_ADD_INFO_CEZ_VW)
        Dim tblOra As List(Of CZ_PRINT_ADD_INFO_CEZ_VW)
        tblOra = _ora.CZ_PRINT_ADD_INFO_CEZ_VW.Distinct.ToList

        Dim list As New List(Of CZ_PRINT_ADD_INFO_CEZ_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyPrintInfoCEZs.FirstOrDefault(Function(e) e.CEZ_IDSrcACC = ora.ACCOUNT_ID)
            If sql IsNot Nothing Then 'pro update
                If (
        Comparison(sql.CEZ_SuplyPlace, ora.ODBERNE_MISTO) _
        Or Comparison(sql.CEZ_VariableSymbol, ora.VARIABILNI_SYMBOL) _
        Or DateDiff(sql.CEZ_DueDate, ora.DATUM_SPLATNOSTI) _
        Or sql.CEZ_AmountTotal <> ora.FAKTUROVANA_CASTKA _
        Or sql.CEZ_AmountSupplement <> ora.VYSE_DOPLATKU) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_PRINT_ADD_INFO_CEZ_VW]->[tblSpisyPrintInfoCEZ] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_PRINT_ADD_INFO_CEZ_VW)) As List(Of Integer)
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

    'Public Property IDSpisyPrintInfoCEZ As Integer
    'Public Property IDSpisu As Nullable(Of Integer)
    'Public Property CEZ_IDSrcACC As String
    'Public Property CEZ_SuplyPlace As String
    'Public Property CEZ_VariableSymbol As String
    'Public Property CEZ_DueDate As Nullable(Of Date)
    'Public Property CEZ_AmountTotal As Nullable(Of Decimal)
    'Public Property CEZ_AmountSupplement As Nullable(Of Decimal)
    'Public Property TStamp As Byte()

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_PRINT_ADD_INFO_CEZ_VW) As Integer
        Try
            Dim model = _sql.tblSpisyPrintInfoCEZs.FirstOrDefault(Function(e) e.CEZ_IDSrcACC = item.ACCOUNT_ID)
            If model IsNot Nothing Then 'update
                model.CEZ_SuplyPlace = item.ODBERNE_MISTO
                model.CEZ_VariableSymbol = item.VARIABILNI_SYMBOL
                model.CEZ_DueDate = item.DATUM_SPLATNOSTI
                model.CEZ_AmountTotal = item.FAKTUROVANA_CASTKA
                model.CEZ_AmountSupplement = item.VYSE_DOPLATKU
            Else 'insert
                model = New tblSpisyPrintInfoCEZ
                model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                model.CEZ_IDSrcACC = item.ACCOUNT_ID
                model.CEZ_SuplyPlace = item.ODBERNE_MISTO
                model.CEZ_VariableSymbol = item.VARIABILNI_SYMBOL
                model.CEZ_DueDate = item.DATUM_SPLATNOSTI
                model.CEZ_AmountTotal = item.FAKTUROVANA_CASTKA
                model.CEZ_AmountSupplement = item.VYSE_DOPLATKU


                _sql.tblSpisyPrintInfoCEZs.Add(model)
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
            log.WriteEntry("tblSpisyPrintInfoCEZ: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
