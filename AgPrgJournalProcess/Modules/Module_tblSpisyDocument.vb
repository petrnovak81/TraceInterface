Module Module_tblSpisyDocument
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_CE_DOCUMENTS_VW)
        Dim tblOra As List(Of CZ_CE_DOCUMENTS_VW)
        tblOra = _ora.CZ_CE_DOCUMENTS_VW.Distinct.ToList

        Dim list As New List(Of CZ_CE_DOCUMENTS_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyDocuments.FirstOrDefault(Function(e) e.IDSrcSpisyDocument = ora.CE_DOCUMENT_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.DocuSentComment, ora.DOCUMENT_DESCRIPTION) _
                 Or Comparison(sql.DocuLink, ora.DOCUMENT_LINK) _
                 Or DateDiff(sql.DocuSentDate, ora.CREATION_DATE)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_CE_DOCUMENTS_VW]->[tblSpisyDocument] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_CE_DOCUMENTS_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_CE_DOCUMENTS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyDocuments.FirstOrDefault(Function(e) e.IDSrcSpisyDocument = item.CE_DOCUMENT_ID)
            If model IsNot Nothing Then 'update
                model.DocuSentComment = item.DOCUMENT_DESCRIPTION
                model.DocuPrintPDF = item.DOCUMENT_LINK
                model.DocuLink = item.DOCUMENT_LINK
                model.DocuSentDate = item.CREATION_DATE
            Else 'insert
                model = New tblSpisyDocument
                model.IDSpisu = IDSpisu(_sql, item.ACCOUNT_ID)
                model.IDSrcSpisyDocument = item.CE_DOCUMENT_ID
                model.DocuSentComment = item.DOCUMENT_DESCRIPTION
                model.DocuPrintPDF = item.DOCUMENT_LINK
                model.DocuLink = item.DOCUMENT_LINK
                model.DocuSentDate = item.CREATION_DATE
                model.rr_Journal = 8

                _sql.tblSpisyDocuments.Add(model)
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
            log.WriteEntry("tblSpisyDocument: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
