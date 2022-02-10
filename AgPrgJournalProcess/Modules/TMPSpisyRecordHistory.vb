Public Class TMPSpisyRecordHistory
    Dim dbSql As SqlEntities
    Dim dbOra As OracleEntities

    Sub New()
        dbSql = New SqlEntities
        dbOra = New OracleEntities
    End Sub

    Public Sub Insert(log As EventLog)
        Try
            log.WriteEntry("CALL sp_INT_Truncate_tblTMPSpisyRecordHistory", EventLogEntryType.Information)
            dbSql.sp_INT_Truncate_tblTMPSpisyRecordHistory()

            log.WriteEntry("GET ORACLE DATA", EventLogEntryType.Information)
            Dim data = dbOra.CZ_COLLENT_COMMENTS_VW.ToList
            log.WriteEntry("INSERT " & data.Count & " ROWS", EventLogEntryType.Information)

            For Each i In Data
                Dim r As New tblTMPSpisyRecordHistory
                r.ACCOUNT_ID = i.ACCOUNT_ID
                r.COLLENT_COMMENT_ID = i.COLLENT_COMMENT_ID
                r.CREATION_DATE = i.CREATION_DATE
                r.COMMENT_TYPE_NAME = i.COMMENT_TYPE_NAME
                r.USER_COMMENT = i.USER_COMMENT
                dbSql.tblTMPSpisyRecordHistories.Add(r)
                dbSql.SaveChanges()
            Next

            log.WriteEntry("CALL sp_INT_O2S_tblSpisyRecordHistory", EventLogEntryType.Information)
            dbSql.sp_INT_O2S_tblSpisyRecordHistory()

            log.WriteEntry("SUCCESS", EventLogEntryType.Information)
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            log.WriteEntry("INSERT ERROR: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class
