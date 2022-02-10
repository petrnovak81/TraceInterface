Imports System.ComponentModel
Imports System.Data.Entity.Core.Objects
Imports System.IO
Imports System.Net.Mail
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class service
    Private timer As System.Timers.Timer

    Private LoggingToWinLog_IsEnabled As Boolean
    Private ProcessingIntervalSeconds As Integer
    Private CriticalErrorsToEmail_EmailAdress As List(Of String)
    Private CriticalErrorsToEmail_IsEnabled As Boolean
    Private OneConfirmMailPerDay_EmailAddress As Boolean
    Private OneConfirmMailPerDay_IsEnabled As Boolean

    Private Email_EnableSsl As Boolean
    Private Email_Host As String
    Private Email_Port As Integer
    Private Email_CredentialsName As String
    Private Email_CredentialsPassword As String
    Private Email_From As String
    Private Email_Subject As String
    Private Email_DisplayName As String

    Public Sub WriteLog(msg As String, type As EventLogEntryType)
        On Error Resume Next
        If LoggingToWinLog_IsEnabled Then
            Me.EventLog.WriteEntry(msg, type)
        End If
    End Sub

    'event id 1
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.CanStop = True
        Me.CanPauseAndContinue = True

        'Setup logging
        Me.AutoLog = False

        DirectCast(Me.EventLog, ISupportInitialize).BeginInit()
        If Not EventLog.SourceExists(Me.ServiceName) Then
            EventLog.CreateEventSource(Me.ServiceName, "Application")
        End If
        DirectCast(Me.EventLog, ISupportInitialize).EndInit()

        Me.EventLog.Source = Me.ServiceName
        Me.EventLog.Log = "Application"

        AddHandler Me.EventLog.EntryWritten, AddressOf EventLogEntryWritten
        Me.EventLog.EnableRaisingEvents = True

        LoggingToWinLog_IsEnabled = My.Settings.LoggingToWinLog_IsEnabled
        ProcessingIntervalSeconds = My.Settings.ProcessingIntervalSeconds
        CriticalErrorsToEmail_EmailAdress = My.Settings.CriticalErrorsToEmail_EmailAdress.Cast(Of String)().ToList()
        CriticalErrorsToEmail_IsEnabled = My.Settings.CriticalErrorsToEmail_IsEnabled
        OneConfirmMailPerDay_EmailAddress = My.Settings.OneConfirmMailPerDay_EmailAddress
        OneConfirmMailPerDay_IsEnabled = My.Settings.OneConfirmMailPerDay_IsEnabled

        Email_EnableSsl = My.Settings.Email_EnableSsl
        Email_Host = My.Settings.Email_Host
        Email_Port = My.Settings.Email_Port
        Email_CredentialsName = My.Settings.Email_CredentialsName
        Email_CredentialsPassword = My.Settings.Email_CredentialsPassword
        Email_From = My.Settings.Email_From
        Email_Subject = My.Settings.Email_Subject
        Email_DisplayName = My.Settings.Email_DisplayName

        Using cn As New SqlEntities
            Dim i As New tblLogService
            i.L = "E"
            i.LogTime = Now
            cn.tblLogServices.Add(i)
            cn.SaveChanges()
        End Using

        'Me.timer = New System.Timers.Timer(ProcessingIntervalSeconds * 1000)
        Me.timer = New System.Timers.Timer(1000)
        Me.timer.AutoReset = True
        AddHandler Me.timer.Elapsed, AddressOf timer_Elapsed
        Me.timer.Enabled = True
        Me.timer.Start()
    End Sub

    Private Sub EventLogEntryWritten(sender As Object, e As EntryWrittenEventArgs)
        On Error Resume Next
        If LoggingToWinLog_IsEnabled Then
            If e.Entry.EntryType = EventLogEntryType.Error Then
                Using db As New SqlEntities
                    Dim msg = e.Entry.Message
                    Dim err As New tblError
                    err.ErrDate = Now
                    err.ErrSource = "Interface"
                    err.ErrText = Left(msg, 400)

                    db.tblErrors.Add(err)
                    db.SaveChanges()
                End Using
            End If
        Else
            Me.EventLog.EndInit()
        End If
    End Sub

    Private Sub restart()
        Dim proc As New Process()
        Dim psi As New ProcessStartInfo()

        psi.CreateNoWindow = True
        psi.FileName = "cmd.exe"
        psi.Arguments = "/C net stop TRACEInterface && net start TRACEInterface"
        psi.LoadUserProfile = False
        psi.UseShellExecute = False
        psi.WindowStyle = ProcessWindowStyle.Hidden
        proc.StartInfo = psi
        proc.Start()
    End Sub

    'event id 2
    'Private Function CheckOraConnection() As Boolean
    '    Dim conn As IDbConnection = Ora.Database.Connection
    '    Try
    '        conn.Open()
    '        WriteLog("Oracle connected", EventLogEntryType.Information)
    '        conn.Close()
    '        Return True
    '    Catch ex As Exception
    '        WriteLog("Oracle disconnected: " & ex.Message, EventLogEntryType.Error)
    '        Return False
    '    End Try
    'End Function

    'event id 3
    'Private Function CheckSqlConnection() As Boolean
    '    Dim conn As IDbConnection = Sql.Database.Connection
    '    Try
    '        conn.Open()
    '        WriteLog("SQL connected", EventLogEntryType.Information)
    '        conn.Close()
    '        Return True
    '    Catch ex As Exception
    '        WriteLog("SQL disconnected: " & ex.Message, EventLogEntryType.Error)
    '        Return False
    '    End Try
    'End Function

    'event id 4
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        WriteLog("OnStart", EventLogEntryType.Information)
    End Sub

    'event id 5
    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        WriteLog("OnStop", EventLogEntryType.Warning)
        'Me.timer.Stop()
    End Sub

    'event id 6
    Protected Overrides Sub OnPause()
        ' Add code here to perform any tear-down necessary to stop your service.
        WriteLog("OnPause", EventLogEntryType.Warning)
        'Me.timer.Stop()
    End Sub

    'event id 7
    Protected Overrides Sub OnContinue()
        ' Add code here to perform any tear-down necessary to stop your service.
        WriteLog("OnContinue", EventLogEntryType.Information)
        'Me.timer.Start()
    End Sub

    'event id 8
    Dim process_run = 0
    Private Sub timer_Elapsed(sender As Object, e As ElapsedEventArgs)
        Try
            If TimeOfDay.Hour = 0 Then
                timer.Stop()
                Threading.Thread.Sleep(7200000)
                restart()
            Else
                If process_run = 0 Then
                    process_run = 1
                    'Threading.Thread.Sleep(15000)

                    Dim row As String = ""
                    Dim sql As SqlEntities
                    Dim ora As OracleEntities

                    Try
                        sql = New SqlEntities
                        Dim tbl = sql.tblLogServices.OrderByDescending(Function(f) f.IDLog).First
                        If tbl IsNot Nothing Then
                            If tbl.L = "B" Then
                                'odeslat email pokud je cas begin > 5hodin
                                Exit Sub
                            End If
                        End If

                        Dim i As New tblLogService
                        i.L = "B"
                        i.LogTime = Now
                        sql.tblLogServices.Add(i)
                        sql.SaveChanges()

                        row = "DataTransferToORA"
                        sql = New SqlEntities
                        ora = New OracleEntities
                        DataTransferToORA(sql, ora)
                        row = "DataTransferToSQL"
                        sql = New SqlEntities
                        ora = New OracleEntities
                        DataTransferToSQL(sql, ora)


                        sql = New SqlEntities
                        i = New tblLogService
                        i.L = "E"
                        i.LogTime = Now
                        sql.tblLogServices.Add(i)
                        sql.SaveChanges()
                        sql.sp_Run_AfterSyncInterface()
                    Catch ex As Exception
                        'System.Diagnostics.Debugger.Launch()
                        'timer.Stop()
                        Dim st = New StackTrace(ex, True)
                        Dim frame = st.GetFrame(0)
                        Dim line = frame.GetFileLineNumber()
                        Dim method = frame.GetMethod()
                        Dim m = String.Concat(method.DeclaringType.FullName, ".", method.Name)

                        While ex.InnerException IsNot Nothing
                            ex = ex.InnerException
                        End While
                        WriteLog(row & ", declaring method: " & m & ", line: " & line & ", " & ex.Message, EventLogEntryType.Error)
                        'Threading.Thread.Sleep(600000)
                        'restart()
                        process_run = 0
                    End Try

                    process_run = 0
                End If
            End If
        Catch ex As Exception
            'timer.Stop()
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            WriteLog("2: " & ex.Message, EventLogEntryType.Error)
            process_run = 0
            'Threading.Thread.Sleep(600000)
            'restart()
        End Try
    End Sub

    Private Function GetLog() As List(Of LogEntry)
        Dim entry As EventLogEntry
        Dim data As New List(Of LogEntry)
        For Each entry In Me.EventLog.Entries.Cast(Of EventLogEntry).Where(Function(e) e.Source = Me.ServiceName)
            data.Add(New LogEntry With {.EntryType = entry.EntryType,
                     .TimeWritten = entry.TimeWritten,
                     .Message = entry.Message,
                     .Source = entry.Source})
        Next
        Return data
    End Function

    Private Function CreateHtmlEventLog(h3Title As String, p1 As String, entries As List(Of LogEntry)) As String
        Dim html As String = "<!DOCTYPE html><html><head><meta charset='utf-8'><title>TRACE Interface</title>"
        html += "<style>"
        html += "table { padding: 10px; border-collapse: collapse; }"
        html += "table th { color: #666666; background: #efeff2; padding: 3px; }"
        html += "table td { color: #444; padding: 3px; }"
        html += "table, th, td { border: 1px solid silver; }"
        html += "</style>"
        html += "</head><body><h3>" & h3Title & "</h3><p>" & p1 & "</p><table>"
        html += "<tr><th>Úroveň</th><th>Datum a čas</th><th>Zpráva</th></tr>"
        For Each i In entries.Take(100).OrderByDescending(Function(x) x.TimeWritten)
            Dim span As String = i.EntryType
            Select Case i.EntryType
                Case 1
                    span = "<div><span style='color: red;'>✋</span> Chyba</div>"
                Case 2
                    span = "<div><span style='color: orange;'>⚠</span> Upozornění</div>"
                Case 4
                    span = "<div><span style='color: blue;'>ⓘ</span> Informace</div>"
            End Select
            html += "<tr><td>" & span & "</td><td>" & i.TimeWritten.ToString("yyyy-MM-dd HH:mm:ss") & "</td><td>" & i.Message & "</td></tr>"
        Next
        html += "</table></body></html>"
        Return html
    End Function

    'event id 9
    Private Sub EmailSender(body As String, addressTo As List(Of String))
        If CriticalErrorsToEmail_IsEnabled Then
            Dim client As New SmtpClient()
            client.EnableSsl = Email_EnableSsl
            client.Host = Email_Host
            client.Port = Email_Port
            client.Credentials = New System.Net.NetworkCredential(Email_CredentialsName, Email_CredentialsPassword)

            Dim Message As New MailMessage()
            Message.Body = body
            Message.IsBodyHtml = True
            If Email_DisplayName IsNot Nothing Then
                Message.From = New MailAddress(Email_From, Email_DisplayName)
            Else
                Message.From = New MailAddress(Email_From)
            End If
            Message.Subject = Email_Subject
            For Each e As String In addressTo
                Message.[To].Add(New MailAddress(e))
            Next

            Try
                client.Send(Message)
                WriteLog("SUCCESS EmailSender: " & String.Join(", ", addressTo), EventLogEntryType.Information)
            Catch ex As Exception
                WriteLog("ERROR EmailSender: " & String.Join(", ", addressTo) & " - Exception: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If
    End Sub

    Private Function ReadWrite(dir As String) As String
        Try
            Dim f As String = Path.Combine(dir, "access")
            Using sw As New StreamWriter(f)
                sw.Write(Now.ToString("yyyy-MM-dd HH:mm:ss"))
            End Using
            Using sw As New StreamReader(f)
                Dim r = sw.ReadToEnd()
            End Using
            If File.Exists(f) Then
                File.Delete(f)
            End If
            Return Nothing
        Catch ex As UnauthorizedAccessException
            Return ex.Message
        End Try
    End Function


    ''' <summary>
    ''' [CZ_ACCOUNTS_VW]->[tblSpisy]
    ''' [CZ_CLIENTS_VW]->[tblCreditor]
    ''' [CZ_SUBSCRIBERS_VW]->[tblSpisyOsoby]
    ''' [CZ_ACC_SUBS_INT_VW]->[tblSpisyTypOsoby]
    ''' [CZ_SUBSCRIBER_ADDRESSES_VW]->[tblSpisyOsobyAdresy]
    ''' [CZ_SUBSCRIBER_CONTACTS_EML_VW]->[tblSpisyOsobyEmail]
    ''' [CZ_SUBSCRIBER_PHONES_VW]->[tblSpisyOsobyPhone]
    ''' [CZ_SUBSCRIBER_CONTACTS_EMP_VW]->[tblSpisyOsobyZam]
    ''' [CZ_ACCOUNT_DEBTS_VW]->[tblSpisyFinanceInfo]
    ''' [CZ_CE_DOCUMENTS_VW]->[tblSpisyDocument]
    ''' [CZ_ACCOUNT_INFO_XML_VW]->[tblOtherInfo]
    ''' [CZ_COLLENT_COMMENTS_VW]->[tblSpisyRecordHistory]
    ''' [CZ_PAYMENTS_VW]->[tblSpisyPlatbyDosle]
    ''' [CZ_PA_INSTALLMENTS_VW]->[tblSpisyDohody]
    ''' [CZ_PA_INSTALLMENT_HISTORY_VW]->[tblSpisyHistorieDohod]
    ''' </summary>
    Private Sub DataTransferToSQL(sql As SqlEntities, ora As OracleEntities)
        'On Error Resume Next
        'System.Diagnostics.Debugger.Launch()

        'Module_tblSpisyDohody v modelu vybrat tabulku vybrat sloupec s datumem a v nastaveni nastavit StroredGeneratePattern na Computed

        'update parameters
        Try
            Dim orr = ora.CZ_KOLLECTO_PARAMETERS_VW.FirstOrDefault(Function(e) e.CODE_PARAM = "DOC_PATH")
            If orr IsNot Nothing Then
                If orr.VALUE_PARAM.Length <= 150 Then
                    Dim rw As String = ReadWrite(orr.VALUE_PARAM)
                    If rw Is Nothing Then
                        Dim srr = sql.tblRegisterRestrictions.FirstOrDefault(Function(e) e.Register = "rr_DOC_PATH")
                        If srr IsNot Nothing Then
                            srr.Val = orr.VALUE_PARAM
                        Else
                            Dim rr As New tblRegisterRestriction
                            rr.Register = "rr_DOC_PATH"
                            rr.IDOrder = 1
                            rr.Val = orr.VALUE_PARAM
                            sql.tblRegisterRestrictions.Add(rr)
                        End If
                        sql.SaveChanges()
                        Me.EventLog.WriteEntry("DOC_PATH aktualizován.", EventLogEntryType.Information)
                    Else
                        Me.EventLog.WriteEntry(rw, EventLogEntryType.Error)
                    End If
                Else
                    Me.EventLog.WriteEntry("DOC_PATH je delší než 150 znaků 'tblRegisterRestrictions.rr_DOC_PATH'.", EventLogEntryType.Error)
                End If
            End If
        Catch ex As Exception
            Me.EventLog.WriteEntry("DOC_PATH error: " & ex.Message, EventLogEntryType.Error)
        End Try

        'Dim CZ_SUBSCRIBERS_VW = Module_tblSpisyOsoby.GetChanges(ora, sql, Me.EventLog)
        'If CZ_SUBSCRIBERS_VW.Count > 0 Then
        '    Module_tblSpisyOsoby.AddChanges(Me.EventLog, sql, CZ_SUBSCRIBERS_VW)
        'End If

        Dim CZ_CLIENTS_VW = Module_tblCreditor.GetChanges(ora, sql, Me.EventLog)
        If CZ_CLIENTS_VW.Count > 0 Then
            Module_tblCreditor.AddChanges(Me.EventLog, sql, CZ_CLIENTS_VW)
        End If

        'Dim CZ_ACCOUNTS_VW = Module_tblSpisy.GetChanges(ora, sql, Me.EventLog)
        'If CZ_ACCOUNTS_VW.Count > 0 Then
        '    Module_tblSpisy.AddChanges(Me.EventLog, sql, CZ_ACCOUNTS_VW)
        'End If

        'Dim CZ_ACC_SUBS_INT_VW = Module_tblSpisyTypOsoby.GetChanges(ora, sql, Me.EventLog)
        'If CZ_ACC_SUBS_INT_VW.Count > 0 Then
        '    Module_tblSpisyTypOsoby.AddChanges(Me.EventLog, sql, CZ_ACC_SUBS_INT_VW)
        'End If

        Dim CZ_SUBSCRIBER_ADDRESSES_VW = Module_tblSpisyOsobyAdresy.GetChanges(ora, sql, Me.EventLog)
        If CZ_SUBSCRIBER_ADDRESSES_VW.Count > 0 Then
            Dim states = Module_tblSpisyOsobyAdresy.AddChanges(Me.EventLog, sql, CZ_SUBSCRIBER_ADDRESSES_VW)
        End If

        'Dim CZ_SUBSCRIBER_CONTACTS_EML_VW = Module_tblSpisyOsobyEmail.GetChanges(ora, sql, Me.EventLog)
        'If CZ_SUBSCRIBER_CONTACTS_EML_VW.Count > 0 Then
        '    Dim states = Module_tblSpisyOsobyEmail.AddChanges(Me.EventLog, sql, CZ_SUBSCRIBER_CONTACTS_EML_VW)
        'End If

        Dim CZ_SUBSCRIBER_PHONES_VW = Module_tblSpisyOsobyPhone.GetChanges(ora, sql, Me.EventLog)
        If CZ_SUBSCRIBER_PHONES_VW.Count > 0 Then
            Dim states = Module_tblSpisyOsobyPhone.AddChanges(Me.EventLog, sql, CZ_SUBSCRIBER_PHONES_VW)
        End If

        Dim CZ_SUBSCRIBER_CONTACTS_EMP_VW = Module_tblSpisyOsobyZam.GetChanges(ora, sql, Me.EventLog)
        If CZ_SUBSCRIBER_CONTACTS_EMP_VW.Count > 0 Then
            Dim states = Module_tblSpisyOsobyZam.AddChanges(Me.EventLog, sql, CZ_SUBSCRIBER_CONTACTS_EMP_VW)
        End If

        'Dim CZ_ACCOUNT_DEBTS_VW = Module_tblSpisyFinanceInfo.GetChanges(ora, sql, Me.EventLog)
        'If CZ_ACCOUNT_DEBTS_VW.Count > 0 Then
        '    Dim states = Module_tblSpisyFinanceInfo.AddChanges(Me.EventLog, sql, CZ_ACCOUNT_DEBTS_VW)
        'End If

        Dim CZ_CE_DOCUMENTS_VW = Module_tblSpisyDocument.GetChanges(ora, sql, Me.EventLog)
        If CZ_CE_DOCUMENTS_VW.Count > 0 Then
            Dim states = Module_tblSpisyDocument.AddChanges(Me.EventLog, sql, CZ_CE_DOCUMENTS_VW)
        End If

        Dim CZ_ACCOUNT_INFO_XML_VW = Module_tblOtherInfo.GetChanges(ora, sql, Me.EventLog)
        If CZ_ACCOUNT_INFO_XML_VW.Count > 0 Then
            Dim states = Module_tblOtherInfo.AddChanges(Me.EventLog, sql, CZ_ACCOUNT_INFO_XML_VW)
        End If

        'Dim CZ_PAYMENTS_VW = Module_tblSpisyPlatbyDosle.GetChanges(ora, sql, Me.EventLog)
        'If CZ_PAYMENTS_VW.Count > 0 Then
        '    Dim states = Module_tblSpisyPlatbyDosle.AddChanges(Me.EventLog, sql, CZ_PAYMENTS_VW)
        'End If

        Dim CZ_PA_INSTALLMENTS_VW = Module_tblSpisyDohody.GetChanges(ora, sql, Me.EventLog)
        If CZ_PA_INSTALLMENTS_VW.Count > 0 Then
            Dim states = Module_tblSpisyDohody.AddChanges(Me.EventLog, sql, CZ_PA_INSTALLMENTS_VW)
        End If

        'Dim CZ_PA_INSTALLMENT_HISTORY_VW = Module_tblSpisyHistorieDohod.GetChanges(ora, sql, Me.EventLog)
        'If CZ_PA_INSTALLMENT_HISTORY_VW.Count > 0 Then
        '    Dim states = Module_tblSpisyHistorieDohod.AddChanges(Me.EventLog, sql, CZ_PA_INSTALLMENT_HISTORY_VW)
        'End If

        Dim CZ_PRINT_ADD_INFO_VW = Module_tblSpisyPrintInfo.GetChanges(ora, sql, Me.EventLog)
        If CZ_PRINT_ADD_INFO_VW.Count > 0 Then
            Module_tblSpisyPrintInfo.AddChanges(Me.EventLog, sql, CZ_PRINT_ADD_INFO_VW)
        End If

        'Dim CZ_PRINT_ADD_INFO_CEZ_VW = Module_tblSpisyPrintInfoCEZ.GetChanges(ora, sql, Me.EventLog)
        'If CZ_PRINT_ADD_INFO_CEZ_VW.Count > 0 Then
        '    Module_tblSpisyPrintInfoCEZ.AddChanges(Me.EventLog, sql, CZ_PRINT_ADD_INFO_CEZ_VW)
        'End If

        'CZ_PRINT_ADD_INFO_VW___tblSpisyPrintInfoes()

        sql.Database.ExecuteSqlCommand("UPDATE tblSpisy SET rr_State = 10 WHERE rr_State = 1")
    End Sub

    Private Sub DataTransferToORA(sql, ora)
        Dim NewIDSync As New ObjectParameter("NewIDSync", GetType(Long))
        Dim LastTStamp As New ObjectParameter("LastTStamp", GetType(Long))
        Dim NewTStamp As New ObjectParameter("NewTStamp", GetType(Long))

        Dim _NewIDSync As Long = 0
        Dim _LastTStamp As Long = 0
        Dim _NewTStamp As Long = 0

        Try
            sql.sp_Get_LastTimeStamp(NewIDSync, LastTStamp, NewTStamp)

            _NewIDSync = NewIDSync.Value
            _LastTStamp = LastTStamp.Value
            _NewTStamp = NewTStamp.Value


            Dim bol As Boolean = My.Settings.ZOZ
            If Not bol Then
                Module_CZ_SUBSCRIBER_PHONES_TBL.Update(Me.EventLog)
                Module_CZ_SUBSCRIBER_CONTACTS_EML_TBL.Update(Me.EventLog)
                Module_CZ_SUBSCRIBER_CONTACTS_EMP_TBL.Update(Me.EventLog)
                Module_CZ_SUBS_ADDRESSES_TBL.Update(Me.EventLog)
                Module_CZ_CE_DOCUMENTS_TBL.Update(Me.EventLog)
                Module_CZ_PAYMENTS_TBL.Update(Me.EventLog)
                'Module_CZ_VISIT_REPORT_TBL.Update(Sql, Ora, Me.EventLog)
            End If

            Module_CZ_VISIT_REPORT_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            Module_CZ_SUBSCRIBER_PHONES_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            'Module_CZ_SUBSCRIBER_CONTACTS_EML_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            Module_CZ_SUBSCRIBER_CONTACTS_EMP_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            Module_CZ_SUBS_ADDRESSES_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            Module_CZ_CE_DOCUMENTS_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)
            Module_CZ_PAYMENTS_TBL.Transfer(_LastTStamp, _NewTStamp, sql, Me.EventLog)

            If Not Module_CZ_SUBSCRIBER_PHONES_TBL.isError And
               Not Module_CZ_SUBSCRIBER_CONTACTS_EMP_TBL.isError And
               Not Module_CZ_SUBS_ADDRESSES_TBL.isError And
               Not Module_CZ_CE_DOCUMENTS_TBL.isError And
               Not Module_CZ_PAYMENTS_TBL.isError And
               Not Module_CZ_VISIT_REPORT_TBL.isError Then
                WriteLog("sp_Do_FinishSync NewIDSync = " & _NewIDSync.ToString, EventLogEntryType.Information)
                sql.sp_Do_FinishSync(_NewIDSync)
            End If


        Catch ex As Entity.Validation.DbEntityValidationException
            For Each eve In ex.EntityValidationErrors
                Dim tbl = eve.Entry.Entity.GetType().Name
                Dim state = eve.Entry.State
                For Each ve In eve.ValidationErrors
                    Dim err = (state & " " & tbl & " Property: " & ve.PropertyName & ", Error: " & ve.ErrorMessage)
                    WriteLog(err, EventLogEntryType.Error)
                Next
            Next
            Exit Sub
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            WriteLog("DataTransferToORA: " & ex.Message, EventLogEntryType.Error)
            Exit Sub
        End Try
    End Sub

    Public Function IDSpisu(sql As SqlEntities, ACCOUNT_ID As Integer) As Integer
        Dim model = sql.tblSpisies.FirstOrDefault(Function(e) e.IDSrcSpisy = ACCOUNT_ID)
        If model IsNot Nothing Then
            Return model.IDSpisu
        End If
        Return 0
    End Function

    Public Sub CZ_PRINT_ADD_INFO_VW___tblSpisyPrintInfoes()
        Dim counter As Integer = 0,
            action As String = "",
            method = Reflection.MethodBase.GetCurrentMethod(),
            updated As Integer = 0,
            inserted As Integer = 0,
            sw As Stopwatch = New Stopwatch()
        sw.Start()
        Try
            Using ora As New OracleEntities
                Using sql As New SqlEntities
                    Dim model_oracle = ora.CZ_PRINT_ADD_INFO_VW.ToList
                    For Each polozka_oracle In model_oracle
                        Dim polozka_sql = sql.tblSpisyPrintInfoes.FirstOrDefault(Function(e) e.IDSrcACC = polozka_oracle.ACCOUNT_ID)
                        If polozka_sql IsNot Nothing Then
                            'update
                            Dim upd As Integer = 0
                            action = "UPDATE"
                            upd += Update(polozka_sql.CustomerID, polozka_oracle.CUSTOMER_ID)
                            upd += Update(polozka_sql.CustomerID2, polozka_oracle.CUSTOMER_ID_SECOND)
                            upd += Update(polozka_sql.RegisterInfo, polozka_oracle.REGISTER_INFO)
                            upd += Update(polozka_sql.ACC_Comment9, polozka_oracle.ACC_COMMENT9)
                            upd += Update(polozka_sql.ACC_comment13, polozka_oracle.ACC_COMMENT13)
                            upd += Update(polozka_sql.ACC_comment14, polozka_oracle.ACC_COMMENT14)
                            upd += Update(polozka_sql.ACC_comment17, polozka_oracle.ACC_COMMENT17)
                            upd += Update(polozka_sql.ACC_Comment23, polozka_oracle.ACC_COMMENT23)
                            upd += Update(polozka_sql.ACC_comment27, polozka_oracle.ACC_COMMENT27)
                            upd += Update(polozka_sql.PF_Agreement, polozka_oracle.PF_SMLOUVY)
                            upd += Update(polozka_sql.AmountByWords, polozka_oracle.SLOVY_TO)
                            upd += Update(polozka_sql.AmountByWordsHal, polozka_oracle.SLOVY_TO_HAL)
                            If upd > 0 Then
                                updated += 1
                            End If
                        Else
                            'insert
                            action = "INSERT"
                            polozka_sql = New tblSpisyPrintInfo
                            polozka_sql.IDSpisu = IDSpisu(sql, polozka_oracle.ACCOUNT_ID)
                            polozka_sql.IDSrcACC = polozka_oracle.ACCOUNT_ID
                            polozka_sql.CustomerID = polozka_oracle.CUSTOMER_ID
                            polozka_sql.CustomerID2 = polozka_oracle.CUSTOMER_ID_SECOND
                            polozka_sql.RegisterInfo = polozka_oracle.REGISTER_INFO
                            polozka_sql.ACC_Comment9 = polozka_oracle.ACC_COMMENT9
                            polozka_sql.ACC_comment13 = polozka_oracle.ACC_COMMENT13
                            polozka_sql.ACC_comment14 = polozka_oracle.ACC_COMMENT14
                            polozka_sql.ACC_comment17 = polozka_oracle.ACC_COMMENT17
                            polozka_sql.ACC_Comment23 = polozka_oracle.ACC_COMMENT23
                            polozka_sql.ACC_comment27 = polozka_oracle.ACC_COMMENT27
                            polozka_sql.PF_Agreement = polozka_oracle.PF_SMLOUVY
                            polozka_sql.AmountByWords = polozka_oracle.SLOVY_TO
                            polozka_sql.AmountByWordsHal = polozka_oracle.SLOVY_TO_HAL
                            sql.tblSpisyPrintInfoes.Add(polozka_sql)
                        End If
                        counter += 1
                        If counter > 99 Then
                            sql.SaveChanges()
                            counter = 0
                        End If
                    Next
                    If counter > 0 Then
                        sql.SaveChanges()
                    End If
                    System.Diagnostics.Debugger.Launch()
                    sw.Stop()
                    Me.EventLog.WriteEntry(method.Name & ": UPDATE[" & updated & "], INSERT[" & inserted & "], " & String.Format("TIME[{0}:{1}:{2}]", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds), EventLogEntryType.Information)
                End Using
            End Using
        Catch ex As Exception
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
            End While
            sw.Stop()
            Me.EventLog.WriteEntry(method.Name & " " & action & " Exception: " & ex.Message & ", UPDATE[" & updated & "], INSERT[" & inserted & ", " & String.Format("TIME[{0}:{1}:{2}]", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds), EventLogEntryType.Error)
        End Try
    End Sub

    Public Function Update(o1 As Object, o2 As Object) As Integer
        Dim upd As Boolean = False
        If IsDBNull(o1) And IsDBNull(o2) Then
            Return 0
        End If
        If IsDBNull(o1) And Not IsDBNull(o2) Then
            upd = True
        End If
        If Not IsDBNull(o1) And IsDBNull(o2) Then
            upd = True
        End If
        If o1 Is Nothing And o2 Is Nothing Then
            Return 0
        End If
        If o1 Is Nothing And o2 IsNot Nothing Then
            upd = True
        End If
        If o1 IsNot Nothing And o2 Is Nothing Then
            upd = True
        End If
        If IsNumeric(o1) And IsNumeric(o1) Then
            If o1.GetType() = GetType(Double) Or o1.GetType() = GetType(Decimal) Or o1.GetType() = GetType(Long) Then
                o1 = Math.Round(CDbl(o1), 4)
            End If
            If o2.GetType() = GetType(Double) Or o2.GetType() = GetType(Decimal) Or o2.GetType() = GetType(Long) Then
                o2 = Math.Round(CDbl(o2), 4)
            End If
            upd = o1 <> o2
        Else
            If IsDate(o1) And IsDate(o2) Then
                upd = CDate(o1).ToString("yyyyMMdd") <> CDate(o2).ToString("yyyyMMdd")
            Else
                upd = o1.ToString.Trim.ToUpper <> o2.ToString.Trim.ToUpper
            End If
        End If
        If upd Then
            o1 = o2
            Return 1
        Else
            Return 0
        End If
    End Function

    Function IsXML(str As String) As Boolean
        If Not String.IsNullOrEmpty(str) And str.TrimStart().StartsWith("<") Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
