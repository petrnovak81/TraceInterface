Imports System.Data.Entity.Core.Objects

Module Module_CZ_CE_DOCUMENTS_TBL
    Public isError As Boolean = False

    Public Sub Transfer(_LastTStamp As Long, _NewTStamp As Long, Sql As SqlEntities, log As EventLog)
        Dim ID = 0

        Dim items = Sql.sp_INT_NewDocuments(_LastTStamp, _NewTStamp).ToList

        For Each m In items
            Try
                Using Ora As New OracleEntities
                    Dim model As New AGCZ_CE_DOCUMENTS_TBL

                    ID = m.IDSpisyDocument

                    model.ACCOUNT_ID = m.IDSrcSpisy
                    model.CE_DOCUMENT_ID = m.IDSrcSpisyDocument
                    model.DOCUMENT_DESCRIPTION = m.DocuSentComment
                    model.DOCUMENT_LINK = m.DocuLink
                    model.CREATION_DATE = m.DocuSentDate

                    model.TRC_DOCUMENT_ID = m.IDSpisyDocument
                    model.TRC_GPS_LAT = m.GPSLat
                    model.TRC_GPS_LNG = m.GPSLng
                    model.TRC_TSTAMP = Convert.ToDecimal(m.TStamp)
                    model.TRC_RR_JOURNAL = m.rr_Journal
                    model.TRC_RR_DOCUMENT_KIND = m.rr_DocumentKind

                    Ora.AGCZ_CE_DOCUMENTS_TBL.Add(model)

                    Ora.SaveChanges()
                End Using
            Catch ex As Entity.Validation.DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim tbl = eve.Entry.Entity.GetType().Name
                    Dim state = eve.Entry.State
                    For Each ve In eve.ValidationErrors
                        Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                        log.WriteEntry(err, EventLogEntryType.Error)
                    Next
                Next
                isError = True
            Catch ex As Exception
                'ORA-00001: unique constraint 
                While ex.InnerException IsNot Nothing
                    ex = ex.InnerException
                End While
                If Not ex.Message.Contains("ORA-00001") Then
                    log.WriteEntry("AGCZ_CE_DOCUMENTS_TBL TRC_DOCUMENT_ID = " & ID & ": " & ex.Message, EventLogEntryType.Error)
                    isError = True
                End If
            End Try
        Next

        log.WriteEntry("AGCZ_CE_DOCUMENTS_TBL INSERT " & items.Count & " RECORDS, LastTStamp = " & _LastTStamp.ToString & ", NewTStamp = " & _NewTStamp.ToString, EventLogEntryType.Information)
    End Sub

    Public Sub Update(log As EventLog)
        Try
            Dim Ora As New OracleEntities
            Dim Sql As New SqlEntities

            Dim tbl = Ora.AGCZ_CE_DOCUMENTS_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 6 Or e.TRC_RR_JOURNAL = 7).ToList
            For Each i In tbl
                Dim model = Sql.tblSpisyDocuments.FirstOrDefault(Function(e) e.IDSpisyDocument = i.TRC_DOCUMENT_ID)
                If model IsNot Nothing Then
                    If i.TRC_RR_JOURNAL = 6 Then
                        model.IDSrcSpisyDocument = i.CE_DOCUMENT_ID
                        If model.rr_Journal = 1 Then
                            model.rr_Journal = 8
                        ElseIf model.rr_Journal = 3 Then
                            model.rr_Journal = 2
                        End If
                    Else
                        If model.rr_Journal = 2 Then
                            model.rr_Journal = 8
                        ElseIf model.rr_Journal = 4 Then
                            model.rr_Journal = 2
                        End If
                    End If
                End If

                i.TRC_RR_JOURNAL = 8
            Next
            Sql.SaveChanges()
            Ora.SaveChanges()

            Dim o = Ora.AGCZ_CE_DOCUMENTS_TBL.Where(Function(e) e.TRC_RR_JOURNAL = 8).ToList
            For Each i In o
                Ora.AGCZ_CE_DOCUMENTS_TBL.Remove(i)
            Next
            Ora.SaveChanges()
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each eve In ex.EntityValidationErrors
                Dim tbl = eve.Entry.Entity.GetType().Name
                Dim state = eve.Entry.State
                For Each ve In eve.ValidationErrors
                    Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                    log.WriteEntry(err, EventLogEntryType.Error)
                Next
            Next
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            If Not ex.Message.Contains("ORA-00001") Then
                log.WriteEntry("AGCZ_CE_DOCUMENTS_TBL UPDATE: " & ex.Message, EventLogEntryType.Error)
            End If
        End Try
    End Sub
End Module
