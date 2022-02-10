Module Module_tblSpisyRecordHistory
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_COLLENT_COMMENTS_VW)
        Dim tblOra As List(Of CZ_COLLENT_COMMENTS_VW)
        tblOra = _ora.CZ_COLLENT_COMMENTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_COLLENT_COMMENTS_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyRecordHistories.FirstOrDefault(Function(e) e.IDSrcSpisyRecordHistory = ora.COLLENT_COMMENT_ID And e.IDSrcSpisyRecordHistory2 = ora.ACCOUNT_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.RecordCommentTxt, ora.USER_COMMENT) _
                 Or Comparison(sql.RecordCommentType, ora.COMMENT_TYPE_NAME) _
                 Or DateDiff(sql.RecordDate.ToString(), ora.CREATION_DATE.ToString())) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_COLLENT_COMMENTS_VW]->[tblSpisyRecordHistory] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_COLLENT_COMMENTS_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_COLLENT_COMMENTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyRecordHistories.FirstOrDefault(Function(e) e.IDSrcSpisyRecordHistory = item.COLLENT_COMMENT_ID And e.IDSrcSpisyRecordHistory2 = item.ACCOUNT_ID)
            If model IsNot Nothing Then 'update
                model.RecordCommentTxt = item.USER_COMMENT
                model.RecordCommentType = item.COMMENT_TYPE_NAME
                model.RecordDate = item.CREATION_DATE
                model.RecordSended = 4
            Else 'insert
                Dim ID As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
                If ID > 0 Then
                    model = New tblSpisyRecordHistory
                    model.IDSpisu = ID
                    model.IDSrcSpisyRecordHistory = item.COLLENT_COMMENT_ID
                    model.IDSrcSpisyRecordHistory2 = item.ACCOUNT_ID
                    model.RecordCommentTxt = item.USER_COMMENT
                    model.RecordCommentType = item.COMMENT_TYPE_NAME
                    model.RecordDate = item.CREATION_DATE
                    model.RecordSended = 4
                    _sql.tblSpisyRecordHistories.Add(model)
                Else
                    log.WriteEntry("tblSpisyRecordHistory INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisyRecordHistory: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
