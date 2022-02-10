
Imports System.Data.Entity.Core.Objects

Module Module_tblSpisyOsobyAdresy

    Public Function GetChanges(_ora As OracleEntities, _sql As SqlEntities, log As EventLog) As List(Of CZ_SUBSCRIBER_ADDRESSES_VW)
        Dim tblOra As List(Of CZ_SUBSCRIBER_ADDRESSES_VW)
        tblOra = _ora.CZ_SUBSCRIBER_ADDRESSES_VW.Distinct.ToList

        Dim list As New List(Of CZ_SUBSCRIBER_ADDRESSES_VW)

        For Each ora In tblOra

            Dim sql = _sql.tblSpisyOsobyAdresies.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyAdresy = ora.SUBSCRIBER_ADDRESS_ID)

            'System.Diagnostics.Debugger.Launch()
            'Dim a = String.Join("", sql).ToLower
            'Dim b = String.Join("", ora).ToLower

            'TODO rr_AddressValidity = Short, VALID = Boolean
            If sql IsNot Nothing Then 'pro update
                If (Comparison(sql.PersCity, ora.CITY) _
                 Or Comparison(sql.PersHouseNum, ora.STREET_NUMBER) _
                 Or Comparison(sql.PersStreet, ora.STREET) _
                 Or Comparison(sql.PersZipCode, ora.POSTAL_CODE) _
                 Or Comparison(sql.PersMainAddress, ora.MAIN_ADDRESS) _
                 Or Comparison(sql.rr_AddressType, ora.ADDRESS_TYPE) _
                 Or sql.rr_AddressValidity <> If(ora.VALID, 2, 1)) _
                 And sql.GPSValid = False And sql.rr_Journal = 8 Then

                    list.Add(ora)
                End If
                'ElseIf sql.PersRegion = "xxx" Then
                '    list.Add(ora)
            Else 'pro insert
                list.Add(ora)
            End If
        Next
        log.WriteEntry("UPDATE/INSERT [CZ_SUBSCRIBER_ADDRESSES_VW]->[tblSpisyOsobyAdresy] " & list.Count & " z " & tblOra.Count, EventLogEntryType.Information)
        Return list
    End Function

    Public Function AddChanges(log As EventLog, _sql As SqlEntities, items As List(Of CZ_SUBSCRIBER_ADDRESSES_VW)) As List(Of Integer)
        Dim errCount As New List(Of Integer)
        For Each i In items
            UpdateOrInsert(log, _sql, i)
        Next
        Return errCount
    End Function

    Public Function IDSpisyOsoby(_sql As SqlEntities, SUBSCRIBER_ID As Integer) As Integer
        Dim model = _sql.tblSpisyOsobies.FirstOrDefault(Function(e) e.IDSrcSpisyOsoby = SUBSCRIBER_ID)
        If model IsNot Nothing Then
            Return model.IDSpisyOsoby
        End If
        Return 0
    End Function

    'Public Function PersHouseNum(STREET_NUMBER As String) As String
    '    Dim subString As String = Left(STREET_NUMBER, 10)
    '    Return subString
    'End Function

    Public Function UpdateOrInsert(log As EventLog, _sql As SqlEntities, item As CZ_SUBSCRIBER_ADDRESSES_VW) As Integer
        Dim act As String = Nothing
        Try
            Dim Region As New ObjectParameter("Region", GetType(String))
            Dim sp = _sql.sp_Get_Region(item.POSTAL_CODE, Region)
            Dim model = _sql.tblSpisyOsobyAdresies.FirstOrDefault(Function(e) e.IDSrcSpisyOsobyAdresy = item.SUBSCRIBER_ADDRESS_ID)
            If model IsNot Nothing Then 'update
                If Not model.GPSValid Then
                    model.IDSrcSpisyOsobyAdresy = item.SUBSCRIBER_ADDRESS_ID
                    model.PersCity = item.CITY
                    model.PersHouseNum = Left(item.STREET_NUMBER, 10)
                    model.PersStreet = item.STREET
                    model.PersZipCode = Left(item.POSTAL_CODE, 10)
                    model.PersRegion = If(Region.Value IsNot Nothing, Region.Value.ToString, "")
                    model.PersMainAddress = item.MAIN_ADDRESS

                    model.rr_AddressValidity = If(item.VALID, 2, 1)

                    model.rr_AddressType = item.ADDRESS_TYPE
                    model.GPSLat = Nothing
                    model.GPSLng = Nothing
                    model.GPSValid = False
                    model.GPSValidDetail = 0
                    model.PersAddrFull = Nothing
                    model.NextContact = False

                Else
                    model.PersRegion = If(Region.Value IsNot Nothing, Region.Value.ToString, "")
                End If
            Else 'insert
                Dim ID As Integer = IDSpisyOsoby(_sql, item.SUBSCRIBER_ID)
                If ID > 0 Then
                    model = New tblSpisyOsobyAdresy
                    model.IDSpisyOsoby = ID
                    model.IDSrcSpisyOsobyAdresy = item.SUBSCRIBER_ADDRESS_ID
                    model.PersCity = item.CITY
                    model.PersHouseNum = Left(item.STREET_NUMBER, 10)
                    model.PersStreet = item.STREET
                    model.PersZipCode = Left(item.POSTAL_CODE, 10)
                    model.PersRegion = If(Region.Value IsNot Nothing, Region.Value.ToString, "")
                    model.PersMainAddress = item.MAIN_ADDRESS

                    model.rr_AddressValidity = If(item.VALID, 2, 1)

                    model.rr_AddressType = item.ADDRESS_TYPE
                    model.rr_Journal = 8
                    model.NextContact = False
                    model.TimeCreated = Now
                    model.TimeValidityUpdate = Now

                    _sql.tblSpisyOsobyAdresies.Add(model)
                Else
                    log.WriteEntry("tblSpisyOsobyAdresy INSERT: FOR SUBSCRIBER_ID = " & item.SUBSCRIBER_ID & " IDSpisyOsoby NOT FOUND", EventLogEntryType.Warning)
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
            log.WriteEntry("tblSpisyOsobyAdresy: " & ex.Message, EventLogEntryType.Error)
            Return 3
        End Try
    End Function
End Module
