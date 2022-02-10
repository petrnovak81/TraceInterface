Imports System.Data.Entity.Core.Objects

Module Module_CZ_VISIT_REPORT_TBL
    Public isError As Boolean = False

    Public Sub Transfer(_LastTStamp As Long, _NewTStamp As Long, Sql As SqlEntities, log As EventLog)
        Dim ID = 0
        Dim items = Sql.sp_INT_NewVisitReport(_LastTStamp, _NewTStamp).ToList
        log.WriteEntry("AGCZ_VISIT_REPORT_TBL INSERT " & items.Count & " RECORDS, LastTStamp = " & _LastTStamp.ToString & ", NewTStamp = " & _NewTStamp.ToString, EventLogEntryType.Information)
        For Each m In items
            Try
                Using Ora As New OracleEntities
                    Dim model As New AGCZ_VISIT_REPORT_TBL

                    ID = m.IDVisitReport

                    model.ID_SPISU = m.IDSpisu
                    model.ID_SPISY_OSOBY = m.IDSpisyOsoby
                    model.ID_SRC_SPISY = m.IDSrcSpisy
                    model.ID_SRC_SPISY_OSOBY = m.IDSrcSpisyOsoby
                    model.ID_USER = m.IDUser
                    model.ID_VISIT_REPORT = m.IDVisitReport
                    model.RR_JOURNAL = m.rr_Journal
                    model.RR_VISITTYPE = m.rr_VisitType
                    model.T_STAMP = Convert.ToDecimal(m.TStamp)

                    model.VR1_DATE_OSN = m.VR1_DateOSN
                    model.VR1_DATE = m.VR1_Date
                    model.VR1_GPS_LAT = m.VR1_GPSLat
                    model.VR1_GPS_LNG = m.VR1_GPSLng
                    model.VR2_ID_SPISY_OSOBY_ADRESY = m.VR2_IDSpisyOsobyAdresy
                    model.VR3_RR_VISIT_RESULT = m.VR3_rr_VisitResult
                    model.VR3_COMMENT = m.VR3_Comment
                    model.VR41_CASH_GAIN = m.VR41_CashGain
                    model.VR41_CASH_DATE = m.VR41_CashDate
                    model.VR41_CASH_DOC_NUMBER = m.VR41_CashDocNumber
                    model.VR41_CASH_AMOUNT = m.VR41_CashAmount
                    model.VR41_CASH_CURRENCY = m.VR41_CashCurrency
                    model.VR41_CASH_COMMENT = m.VR41_CashComment
                    model.VR41_AGREEMENT_FOR_NEXT_CASH = m.VR41_AgreementForNextCash
                    model.VR41_DATE_FOR_NEXT_CASH = m.VR41_DateForNextCash
                    model.VR5_PROTOCOL_PHONE_DIRECT = m.VR5_rr_ProtocolPhoneDirect
                    model.VR51_PHONE_DATE = m.VR51_PhoneDate
                    model.VR51_ID_SPISY_OSOBY_PHONE = m.VR51_IDSpisyOsobyPhone
                    model.VR51_PHONE_NUMBER = m.VR51_PhoneNumber
                    model.VR51_PHONE_COMMENT = m.VR51_PhoneComment
                    model.VR6_RR_PROTOCOL_MAIL_DIRECT = m.VR6_rr_ProtocolMailDirect
                    model.VR61_EMAIL_DATE = m.VR61_EmailDate
                    model.VR61_ID_SPISY_OSOBY_EMAIL = m.VR61_IDSpisyOsobyEmail
                    model.VR61_EMAIL = m.VR61_Email
                    model.VR61_COMMENT = Left(m.VR61_Comment, 260)
                    model.VR312_RR_VISIT_OTHER_PERSON = m.VR312_rr_VisitOtherPerson
                    model.VR314_COMMENT = m.VR314_Comment
                    model.VR511_RR_V_CASE_PHONE_RES_OUT = m.VR511_rr_VisitCasePhoneResultOut
                    model.VR511A_RR_V_CASE_PHONE_RES_IN = m.VR511a_rr_VisitCasePhoneResultIncom
                    model.VR54_RR_CASE_AFTER_DENY = m.VR54_rr_CaseAfterDeny
                    model.VR3132_RR_RESULT_3132 = m.VR3132_rr_Result3132
                    model.VR3132_NEW_ADDRESS_ADDED = m.VR3132_NewAddressAdded
                    model.VR3132_ID_SPISY_OSOBY_ADRESY = m.VR3132_IDSpisyOsobyAdresy
                    model.VR3132_2_RR_HOUSING_PROPERTY = m.VR3132_2_rr_HousingProperty
                    model.VR3132_2_RR_HOUS_PROP_OWNER = m.VR3132_2_rr_HousingPropertyOwner
                    model.VR3132_2_RR_HOUS_PROP_FAMILY = m.VR3132_2_rr_HousingPropertyFamily
                    model.VR3132_2_RR_HOUS_PROP_RENT = m.VR3132_2_rr_HousingPropertyRent
                    model.VR3132_2_CAR_OWNER = m.VR3132_2_CarOwner
                    model.VR3132_2_OTHER_OWNER = m.VR3132_2_OtherOwner
                    model.VR3132_2_OTHER_OWNER_COMMENT = m.VR3132_2_OtherOwnerComment
                    model.VR3132_2_LOW_STAND_OF_LIVING = m.VR3132_2_LowStandardOfLiving
                    model.VR3132_3_DENY = m.VR3132_3_Deny
                    model.VR3132_3_INCOMELESS = m.VR3132_3_Incomeless
                    model.VR3132_3_CONFISCATION = m.VR3132_3_Confiscation
                    model.VR3132_3_RR_PERS_TYPE = m.VR3132_3_rr_PersType
                    model.VR3132_3_PERS_TYPE_COMMENT = m.VR3132_3_PersType_Comment
                    model.VR3132_3_EMPLOYMENT = m.VR3132_3_Employment
                    model.VR3132_3_BRIGADE = m.VR3132_3_Brigade
                    model.VR3132_3_MATERNITY_LEAVE = m.VR3132_3_MaternityLeave
                    model.VR3132_3_EMPL_DEPARTMENT = m.VR3132_3_EmplDepartment
                    model.VR3132_3_PENSION = m.VR3132_3_Pension
                    model.VR3132_3_SOCIAL_BENEFITS = m.VR3132_3_SocialBenefits
                    model.VR3132_3_OTHER_INCOME = m.VR3132_3_OtherIncome
                    model.VR3132_3_OTHER_INC_COMMENT = m.VR3132_3_OtherInc_Comment
                    model.VR3132_3_COMENT = m.VR3132_3_Coment
                    model.VR3136_RR_NEXT_ACTIVITY = m.VR3136_rr_NextActivity
                    model.VR3136_1_NEXT_ID_SPISY_OS_ADR = m.VR3136_1_NextIDSpisyOsobyAdresy
                    model.VR3136_1_FV_DATE_PLANNED = m.VR3136_1_FVDatePlanned
                    model.VR3136_2_RR_CASE_NEXT_ACTIVITY = m.VR3136_2_rr_CaseNextActivity
                    model.VR3136_2_CASE_NEXT_ACT_COMMENT = m.VR3136_2_CaseNextActivityComment
                    model.VR3136_2_DEADLINE = m.VR3136_2_DeadLine
                    model.VR3137_PAYMENT_AGREEMENT_CANC = m.VR3137_PaymentAgreementCancel
                    model.VR3137_DATE_AGREEMENT_CANCEL = m.VR3137_DateAgreementCancel
                    model.VR3137_RR_ANOTHER_PERS_NXT_ACT = m.VR3137_rr_AnotherPersNextActivity
                    model.VR3137_1_RR_PAYMENT_AGREEMENT = m.VR3137_1_rr_PaymentAgreement
                    model.VR3137_12_COMMENT = m.VR3137_12_Comment
                    model.VR3137_12_PAYMENT_DATE = m.VR3137_12_PaymentDate
                    model.VR3137_12_SKUZ_SIGNED = m.VR3137_12_SKUZSigned
                    model.VR3137_13_GPS_LAT = m.VR3137_13_GPSLat
                    model.VR3137_13_GPS_LNG = m.VR3137_13_GPSLng
                    model.VR3137_13_ONE_AMOUNT = m.VR3137_13_OneAmount
                    model.VR3137_13_COUNT_OF_PAYMENTS = m.VR3137_13_CountOfPayments
                    model.VR3137_13_DATE_FIRST_PAYMENT = m.VR3137_13_DateFirstPayment
                    model.VR3137_13_AMOUNT_FIRST_PAYMENT = m.VR3137_13_AmountFirstPayment
                    model.VR3137_13_DAY_NEXT_PAYMENT = m.VR3137_13_DayNextPayment
                    model.VR3137_13_CURRENCY = m.VR3137_13_Currency
                    model.VR3137_13_COMMENT = m.VR3137_13_Comment
                    model.VR3137_17_COMMENT = m.VR3137_17_Comment
                    model.VR3137_3_REQUIRE_DOC = m.VR3137_3_RequireDoc
                    model.VR3137_3_COMMENT = m.VR3137_3_Comment
                    model.VR7_RR_CANCEL_SK = m.VR7_rr_CancelSK
                    model.VR341_LETTER_LEAVED = m.VR341_LetterLeaved

                    If m.VR_History Is Nothing Then
                        Sql.sp_Do_VisitReportUpdateJournal(ID, 90)
                    Else
                        model.VR_HISTORY = m.VR_History.ToString
                        Ora.AGCZ_VISIT_REPORT_TBL.Add(model)
                        Ora.SaveChanges()

                        Sql.sp_Do_VisitReportUpdateJournal(ID, 8)
                        log.WriteEntry("sp_Do_VisitReportUpdateJournal: IDVisitReport: " & ID & ", rr_Journal: 8", EventLogEntryType.Information)
                    End If
                End Using
            Catch ex As Exception
                'ORA-00001: unique constraint 
                While ex.InnerException IsNot Nothing
                    ex = ex.InnerException
                End While
                If Not ex.Message.Contains("ORA-00001") Then
                    log.WriteEntry("Interface AGCZ_VISIT_REPORT_TBL.Transfer ID_VISIT_REPORT = " & ID & ": " & ex.Message, EventLogEntryType.Error)
                    isError = True
                End If
            End Try
        Next
    End Sub

    Private Sub Update(Sql As SqlEntities, Ora As OracleEntities, log As EventLog)
        'Try
        '    Dim tbl = Ora.AGCZ_VISIT_REPORT_TBL.Where(Function(e) e.RR_JOURNAL = 6 Or e.RR_JOURNAL = 7).ToList
        '    For Each i In tbl
        '        Dim model = Sql.tblVisitReports.FirstOrDefault(Function(e) e.IDSpisyOsoby = i.ID_SPISY_OSOBY)
        '        If model IsNot Nothing Then
        '            If i.RR_JOURNAL = 6 Then
        '                model.IDSrcSpisyOsoby = i.ID_SRC_SPISY_OSOBY
        '                If model.rr_Journal = 1 Then
        '                    model.rr_Journal = 8
        '                ElseIf model.rr_Journal = 3 Then
        '                    model.rr_Journal = 2
        '                End If
        '            Else
        '                If model.rr_Journal = 2 Then
        '                    model.rr_Journal = 8
        '                ElseIf model.rr_Journal = 4 Then
        '                    model.rr_Journal = 2
        '                End If
        '            End If
        '        End If

        '        i.RR_JOURNAL = 8
        '    Next
        '    Ora.SaveChanges()

        '    Dim o = Ora.AGCZ_VISIT_REPORT_TBL.Where(Function(e) e.RR_JOURNAL = 8).ToList
        '    For Each i In o
        '        Ora.AGCZ_VISIT_REPORT_TBL.Remove(i)
        '    Next
        '    Ora.SaveChanges()
        'Catch ex As Entity.Validation.DbEntityValidationException
        '    For Each eve In ex.EntityValidationErrors
        '        Dim tbl = eve.Entry.Entity.GetType().Name
        '        Dim state = eve.Entry.State
        '        For Each ve In eve.ValidationErrors
        '            Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
        '            log.WriteEntry(err, EventLogEntryType.Error)
        '        Next
        '    Next
        'Catch ex As Exception
        '    While ex.InnerException IsNot Nothing
        '        ex = ex.InnerException
        '    End While
        '    log.WriteEntry("AGCZ_VISIT_REPORT_TBL UPDATE: " & ex.Message, EventLogEntryType.Error)
        'End Try
    End Sub
End Module
