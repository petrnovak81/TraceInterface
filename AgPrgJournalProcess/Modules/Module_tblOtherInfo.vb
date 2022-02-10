Module Module_tblOtherInfo
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_ACCOUNT_INFO_XML_VW)
        Dim tblOra As List(Of CZ_ACCOUNT_INFO_XML_VW)
        tblOra = _ora.CZ_ACCOUNT_INFO_XML_VW.Distinct.ToList

        Dim list As New List(Of CZ_ACCOUNT_INFO_XML_VW)
        For Each ora In tblOra
            Dim ID As Integer = IDSpisu(_sql, ora.ACCOUNT_ID)
            Dim sql = _sql.tblOtherInfoes.FirstOrDefault(Function(e) e.IDSpisu = ID)
            If sql IsNot Nothing Then 'pro update
                Dim xm1 As XDocument = XDocument.Parse(If(sql.OtherInfoXML IsNot Nothing, sql.OtherInfoXML, "<ROWSET><ROW><INFO_VALUE /></ROW></ROWSET>"))
                Dim xm2 As XDocument = XDocument.Parse(If(ora.INFO_VALUE_XML IsNot Nothing, ora.INFO_VALUE_XML, "<ROWSET><ROW><INFO_VALUE /></ROW></ROWSET>"))
                If (Comparison(xm1.ToString, xm2.ToString)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_ACCOUNT_INFO_XML_VW]->[tblOtherInfo] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_ACCOUNT_INFO_XML_VW)) As List(Of Integer)
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

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_ACCOUNT_INFO_XML_VW) As Integer
        Try
            Dim ID As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
            Dim model = _sql.tblOtherInfoes.FirstOrDefault(Function(e) e.IDSpisu = ID)
            If model IsNot Nothing Then 'update
                model.OtherInfoXML = item.INFO_VALUE_XML
            Else 'insert
                If ID > 0 Then
                    model = New tblOtherInfo
                    model.IDSpisu = ID
                    model.OtherInfoXML = item.INFO_VALUE_XML
                    _sql.tblOtherInfoes.Add(model)
                Else
                    log.WriteEntry("tblOtherInfo INSERT: FOR ACC = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblOtherInfo: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
