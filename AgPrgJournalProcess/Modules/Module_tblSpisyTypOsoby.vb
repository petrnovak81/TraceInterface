Imports System.IO

Module Module_tblSpisyTypOsoby
    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_ACC_SUBS_INT_VW)
        Dim tblOra As List(Of CZ_ACC_SUBS_INT_VW)
        tblOra = _ora.CZ_ACC_SUBS_INT_VW.ToList

        Dim list As New List(Of CZ_ACC_SUBS_INT_VW)
        Try

            For Each ora In tblOra

                Dim ID1 = IDSpisu(_sql, ora.ACCOUNT_ID)
                Dim ID2 = IDSpisyOsoby(_sql, ora.SUBSCRIBER_ID)
                Dim sql = _sql.tblSpisyTypOsobies.FirstOrDefault(Function(e) e.IDSpisu = ID1 And e.IDSpisyOsoby = ID2 And e.rr_PersType = ora.SUBSCRIBER_TYPE_ID And e.PersMain = ora.MAIN)

                If sql IsNot Nothing Then 'pro update
                    If (DateDiff(sql.DateSignSKUZ, ora.DEBT_ACCEPT_DATE)) Then
                        'Or sql.PersMain <> ora.MAIN) Then

                        list.Add(ora)
                    Else
                        'If ora.ACCOUNT_ID = 1967109 Then
                        '    log.WriteEntry("04 " & ", rr_PersType = " & ora.SUBSCRIBER_TYPE_ID & " jdu dál", EventLogEntryType.Information)
                        'End If

                    End If

                Else 'pro insert

                    'If ora.ACCOUNT_ID = 1967109 Then
                    '    log.WriteEntry("05 " & ", rr_PersType = " & ora.SUBSCRIBER_TYPE_ID & " Nenalezeno přidávám", EventLogEntryType.Information)
                    'End If

                    list.Add(ora)
                End If
            Next

            'Dim x = list.Where(Function(e) e.ACCOUNT_ID = 1967109).Count



            log.WriteEntry("UPDATE/INSERT [CZ_ACC_SUBS_INT_VW]->[tblSpisyTypOsoby] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)

        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            log.WriteEntry(ex.Message, EventLogEntryType.Error)
        End Try
        Return List
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_ACC_SUBS_INT_VW)) As List(Of Integer)
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

    Public Function IDSpisyOsoby(_sql As SqlEntities, SUBSCRIBER_ID As Integer) As Integer
        Dim model = _sql.tblSpisyOsobies.FirstOrDefault(Function(e) e.IDSrcSpisyOsoby = SUBSCRIBER_ID)
        If model IsNot Nothing Then
            Return model.IDSpisyOsoby
        End If
        Return 0
    End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_ACC_SUBS_INT_VW) As Integer
        Try
            Dim ID1 As Integer = IDSpisu(_sql, item.ACCOUNT_ID)
            Dim ID2 As Integer = IDSpisyOsoby(_sql, item.SUBSCRIBER_ID)
            Dim model = _sql.tblSpisyTypOsobies.FirstOrDefault(Function(e) e.IDSpisu = ID1 And e.IDSpisyOsoby = ID2 And e.rr_PersType = item.SUBSCRIBER_TYPE_ID And e.PersMain = item.MAIN)



            '            Ve view CZ_ACC_SUBS_INT_VW
            '            ACCOUNT_ID, SUBSCRIBER_ID, SUBSCRIBER_TYPE_ID, MAIN, DEBT_ACCEPT_DATE
            '            '1967109','5509414','13','0',null
            '            '1967109','5509414','7','0',null
            '            '1967109','5509414','8','0',null
            '            '1967109','5509414','1','1',null

            '            máme k jednomu ACC

            '           IDSpisu 810
            '           IDSpisyOsoby 1646

            If model IsNot Nothing Then 'update
                model.DateSignSKUZ = item.DEBT_ACCEPT_DATE
                'model.PersMain = item.MAIN

            Else
                If ID1 > 0 And ID2 > 0 Then
                    model = New tblSpisyTypOsoby
                    model.IDSpisu = ID1
                    model.IDSpisyOsoby = ID2
                    model.DateSignSKUZ = item.DEBT_ACCEPT_DATE
                    model.PersMain = item.MAIN
                    model.rr_PersType = item.SUBSCRIBER_TYPE_ID
                    _sql.tblSpisyTypOsobies.Add(model)
                Else
                    log.WriteEntry("tblSpisyTypOsoby INSERT: FOR ACCOUNT_ID = " & item.ACCOUNT_ID & " IDSpisu NOT FOUND ORFOR SUBSCRIBER_ID = " & item.SUBSCRIBER_ID & " IDSpisyOsoby NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisyTypOsoby: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
