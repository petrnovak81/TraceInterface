Module Module_tblSpisyOsoby
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_SUBSCRIBERS_VW)
        Dim tblOra As List(Of CZ_SUBSCRIBERS_VW)
        tblOra = _ora.CZ_SUBSCRIBERS_VW.Distinct.ToList

        Dim list As New List(Of CZ_SUBSCRIBERS_VW)

        For Each ora In tblOra
            Dim sql = _sql.tblSpisyOsobies.FirstOrDefault(Function(e) e.IDSrcSpisyOsoby = ora.SUBSCRIBER_ID)
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.IDSrcSpisyOsoby, ora.SUBSCRIBER_ID) _
Or Comparison(sql.PersSurname, ora.LAST_NAME) _
Or Comparison(sql.PersSurname2, ora.LAST_NAME2) _
Or Comparison(sql.PersName, ora.FIRST_NAME) _
Or DateDiff(sql.PersBirthDate, ora.BIRTH_DATE) _
Or Comparison(sql.PersRC, ora.SSN) _
Or Comparison(sql.PersIC, ora.J_ATTRIBUTE) _
Or Comparison(sql.BonitaKatastr, ora.BONITA_KATASTR) _
Or Comparison(sql.BonitaPocetExe, ora.BONITA_POCET_EXE) _
Or DateDiff(sql.BonitaDatLustrace, ora.BONITA_DATUM_LUSTRACE)) Then

                    list.Add(ora)
                End If
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_SUBSCRIBERS_VW]->[tblSpisyOsoby] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_SUBSCRIBERS_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_SUBSCRIBERS_VW) As Integer
        Try
            Dim model = _sql.tblSpisyOsobies.FirstOrDefault(Function(e) e.IDSrcSpisyOsoby = item.SUBSCRIBER_ID)
            If model IsNot Nothing Then 'update
                model.IDSrcSpisyOsoby = item.SUBSCRIBER_ID
                model.PersSurname = item.LAST_NAME
                model.PersSurname2 = item.LAST_NAME2
                model.PersName = item.FIRST_NAME
                model.PersBirthDate = item.BIRTH_DATE
                model.PersRC = item.SSN
                model.PersIC = item.J_ATTRIBUTE
                model.BonitaKatastr = item.BONITA_KATASTR
                model.BonitaPocetExe = item.BONITA_POCET_EXE
                model.BonitaDatLustrace = item.BONITA_DATUM_LUSTRACE
            Else 'insert
                model = New tblSpisyOsoby
                model.IDSrcSpisyOsoby = item.SUBSCRIBER_ID
                model.PersSurname = item.LAST_NAME
                model.PersSurname2 = item.LAST_NAME2
                model.PersName = item.FIRST_NAME
                model.PersBirthDate = item.BIRTH_DATE
                model.PersRC = item.SSN
                model.PersIC = item.J_ATTRIBUTE
                model.BonitaKatastr = item.BONITA_KATASTR
                model.BonitaPocetExe = item.BONITA_POCET_EXE
                model.BonitaDatLustrace = item.BONITA_DATUM_LUSTRACE

                _sql.tblSpisyOsobies.Add(model)
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
            log.WriteEntry("tblSpisyOsoby: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
