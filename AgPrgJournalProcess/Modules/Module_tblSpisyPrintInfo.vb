Module Module_tblSpisyPrintInfo
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_PRINT_ADD_INFO_VW)
        Dim tblOra As List(Of CZ_PRINT_ADD_INFO_VW)
        tblOra = _ora.CZ_PRINT_ADD_INFO_VW.Distinct.ToList

        Dim list As New List(Of CZ_PRINT_ADD_INFO_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyPrintInfoes.FirstOrDefault(Function(e) e.IDSrcACC = ora.ACCOUNT_ID)
            If sql IsNot Nothing Then 'pro update
                If (sql.CustomerID <> ora.CUSTOMER_ID _
        Or sql.CustomerID2 <> ora.CUSTOMER_ID_SECOND _
        Or Comparison(sql.RegisterInfo, ora.REGISTER_INFO) _
        Or Comparison(sql.ACC_Comment9, ora.ACC_COMMENT9) _
        Or Comparison(sql.ACC_comment13, ora.ACC_COMMENT13) _
        Or Comparison(sql.ACC_comment14, ora.ACC_COMMENT14) _
        Or Comparison(sql.ACC_comment17, ora.ACC_COMMENT17) _
        Or Comparison(sql.ACC_Comment23, ora.ACC_COMMENT23) _
        Or Comparison(sql.ACC_comment27, ora.ACC_COMMENT27) _
        Or Comparison(sql.PF_Agreement, ora.PF_SMLOUVY) _
        Or Comparison(sql.AmountByWords, ora.SLOVY_TO) _
        Or Comparison(sql.AmountByWordsHal, ora.SLOVY_TO_HAL)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_PRINT_ADD_INFO_VW]->[tblSpisyPrintInfo] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_PRINT_ADD_INFO_VW)) As List(Of Integer)
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

    'Public Property IDSpisyPrintInfo As Integer
    'Public Property IDSpisu As Nullable(Of Integer)
    'Public Property IDSrcACC As String
    'Public Property CustomerID As String
    'Public Property CustomerID2 As String
    'Public Property RegisterInfo As String
    'Public Property ACC_Comment9 As String
    'Public Property ACC_comment13 As String
    'Public Property ACC_comment14 As String
    'Public Property ACC_comment17 As String
    'Public Property ACC_Comment23 As String
    'Public Property ACC_comment27 As String
    'Public Property PF_Agreement As String
    'Public Property AmountByWords As String
    'Public Property AmountByWordsHal As String

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_PRINT_ADD_INFO_VW) As Integer
        Try
            Dim model = _sql.tblSpisyPrintInfoes.FirstOrDefault(Function(e) e.IDSrcACC = item.ACCOUNT_ID)
            If model IsNot Nothing Then 'update
                model.CustomerID = item.CUSTOMER_ID
                model.CustomerID2 = item.CUSTOMER_ID_SECOND
                model.RegisterInfo = item.REGISTER_INFO
                model.ACC_Comment9 = item.ACC_COMMENT9
                model.ACC_comment13 = item.ACC_COMMENT13
                model.ACC_comment14 = item.ACC_COMMENT14
                model.ACC_comment17 = item.ACC_COMMENT17
                model.ACC_Comment23 = item.ACC_COMMENT23
                model.ACC_comment27 = item.ACC_COMMENT27
                model.PF_Agreement = item.PF_SMLOUVY
                model.AmountByWords = item.SLOVY_TO
                model.AmountByWordsHal = item.SLOVY_TO_HAL
            Else 'insert
                model = New tblSpisyPrintInfo
                model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                model.IDSrcACC = item.ACCOUNT_ID
                model.CustomerID = item.CUSTOMER_ID
                model.CustomerID2 = item.CUSTOMER_ID_SECOND
                model.RegisterInfo = item.REGISTER_INFO
                model.ACC_Comment9 = item.ACC_COMMENT9
                model.ACC_comment13 = item.ACC_COMMENT13
                model.ACC_comment14 = item.ACC_COMMENT14
                model.ACC_comment17 = item.ACC_COMMENT17
                model.ACC_Comment23 = item.ACC_COMMENT23
                model.ACC_comment27 = item.ACC_COMMENT27
                model.PF_Agreement = item.PF_SMLOUVY
                model.AmountByWords = item.SLOVY_TO
                model.AmountByWordsHal = item.SLOVY_TO_HAL

                _sql.tblSpisyPrintInfoes.Add(model)
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
