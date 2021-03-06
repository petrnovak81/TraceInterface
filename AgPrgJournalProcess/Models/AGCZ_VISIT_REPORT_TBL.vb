'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class AGCZ_VISIT_REPORT_TBL
    Public Property ID_VISIT_REPORT As Long
    Public Property ID_USER As Nullable(Of Long)
    Public Property RR_VISITTYPE As Nullable(Of Integer)
    Public Property ID_SPISU As Nullable(Of Long)
    Public Property ID_SRC_SPISY As Nullable(Of Long)
    Public Property ID_SPISY_OSOBY As Nullable(Of Long)
    Public Property ID_SRC_SPISY_OSOBY As Nullable(Of Long)
    Public Property VR1_DATE_OSN As Nullable(Of Date)
    Public Property VR1_DATE As Nullable(Of Date)
    Public Property VR1_GPS_LAT As String
    Public Property VR1_GPS_LNG As String
    Public Property VR2_ID_SPISY_OSOBY_ADRESY As Nullable(Of Long)
    Public Property VR3_RR_VISIT_RESULT As Nullable(Of Boolean)
    Public Property VR3_COMMENT As String
    Public Property VR41_CASH_GAIN As Nullable(Of Boolean)
    Public Property VR41_CASH_DATE As Nullable(Of Date)
    Public Property VR41_CASH_DOC_NUMBER As Nullable(Of Decimal)
    Public Property VR41_CASH_AMOUNT As Nullable(Of Decimal)
    Public Property VR41_CASH_CURRENCY As String
    Public Property VR41_CASH_COMMENT As String
    Public Property VR41_AGREEMENT_FOR_NEXT_CASH As Nullable(Of Boolean)
    Public Property VR41_DATE_FOR_NEXT_CASH As Nullable(Of Date)
    Public Property VR5_PROTOCOL_PHONE_DIRECT As Nullable(Of Byte)
    Public Property VR51_PHONE_DATE As Nullable(Of Date)
    Public Property VR51_ID_SPISY_OSOBY_PHONE As Nullable(Of Long)
    Public Property VR51_PHONE_NUMBER As Nullable(Of Boolean)
    Public Property VR51_PHONE_COMMENT As String
    Public Property VR6_RR_PROTOCOL_MAIL_DIRECT As Nullable(Of Byte)
    Public Property VR61_EMAIL_DATE As Nullable(Of Date)
    Public Property VR61_ID_SPISY_OSOBY_EMAIL As Nullable(Of Long)
    Public Property VR61_EMAIL As String
    Public Property VR61_COMMENT As String
    Public Property VR312_RR_VISIT_OTHER_PERSON As Nullable(Of Integer)
    Public Property VR314_COMMENT As String
    Public Property VR511_RR_V_CASE_PHONE_RES_OUT As Nullable(Of Integer)
    Public Property VR511A_RR_V_CASE_PHONE_RES_IN As Nullable(Of Integer)
    Public Property VR54_RR_CASE_AFTER_DENY As Nullable(Of Integer)
    Public Property VR3132_RR_RESULT_3132 As Nullable(Of Integer)
    Public Property VR3132_NEW_ADDRESS_ADDED As Nullable(Of Boolean)
    Public Property VR3132_ID_SPISY_OSOBY_ADRESY As Nullable(Of Long)
    Public Property VR3132_2_RR_HOUSING_PROPERTY As Nullable(Of Integer)
    Public Property VR3132_2_RR_HOUS_PROP_OWNER As Nullable(Of Boolean)
    Public Property VR3132_2_RR_HOUS_PROP_FAMILY As Nullable(Of Boolean)
    Public Property VR3132_2_RR_HOUS_PROP_RENT As Nullable(Of Boolean)
    Public Property VR3132_2_CAR_OWNER As Nullable(Of Boolean)
    Public Property VR3132_2_OTHER_OWNER As Nullable(Of Boolean)
    Public Property VR3132_2_OTHER_OWNER_COMMENT As String
    Public Property VR3132_2_LOW_STAND_OF_LIVING As Nullable(Of Boolean)
    Public Property VR3132_3_DENY As Nullable(Of Boolean)
    Public Property VR3132_3_INCOMELESS As Nullable(Of Boolean)
    Public Property VR3132_3_CONFISCATION As Nullable(Of Boolean)
    Public Property VR3132_3_RR_PERS_TYPE As Nullable(Of Integer)
    Public Property VR3132_3_PERS_TYPE_COMMENT As String
    Public Property VR3132_3_EMPLOYMENT As Nullable(Of Boolean)
    Public Property VR3132_3_BRIGADE As Nullable(Of Boolean)
    Public Property VR3132_3_MATERNITY_LEAVE As Nullable(Of Boolean)
    Public Property VR3132_3_EMPL_DEPARTMENT As Nullable(Of Boolean)
    Public Property VR3132_3_PENSION As Nullable(Of Decimal)
    Public Property VR3132_3_SOCIAL_BENEFITS As Nullable(Of Decimal)
    Public Property VR3132_3_OTHER_INCOME As Nullable(Of Decimal)
    Public Property VR3132_3_OTHER_INC_COMMENT As String
    Public Property VR3132_3_COMENT As String
    Public Property VR3136_RR_NEXT_ACTIVITY As Nullable(Of Integer)
    Public Property VR3136_1_NEXT_ID_SPISY_OS_ADR As Nullable(Of Long)
    Public Property VR3136_1_FV_DATE_PLANNED As Nullable(Of Date)
    Public Property VR3136_2_RR_CASE_NEXT_ACTIVITY As Nullable(Of Integer)
    Public Property VR3136_2_CASE_NEXT_ACT_COMMENT As String
    Public Property VR3136_2_DEADLINE As Nullable(Of Date)
    Public Property VR3137_PAYMENT_AGREEMENT_CANC As Nullable(Of Boolean)
    Public Property VR3137_DATE_AGREEMENT_CANCEL As Nullable(Of Date)
    Public Property VR3137_RR_ANOTHER_PERS_NXT_ACT As Nullable(Of Integer)
    Public Property VR3137_1_RR_PAYMENT_AGREEMENT As Nullable(Of Integer)
    Public Property VR3137_12_COMMENT As String
    Public Property VR3137_12_PAYMENT_DATE As Nullable(Of Date)
    Public Property VR3137_12_SKUZ_SIGNED As Nullable(Of Boolean)
    Public Property VR3137_13_GPS_LAT As String
    Public Property VR3137_13_GPS_LNG As String
    Public Property VR3137_13_ONE_AMOUNT As Nullable(Of Decimal)
    Public Property VR3137_13_COUNT_OF_PAYMENTS As Nullable(Of Integer)
    Public Property VR3137_13_DATE_FIRST_PAYMENT As Nullable(Of Date)
    Public Property VR3137_13_AMOUNT_FIRST_PAYMENT As Nullable(Of Decimal)
    Public Property VR3137_13_DAY_NEXT_PAYMENT As Nullable(Of Integer)
    Public Property VR3137_13_CURRENCY As String
    Public Property VR3137_13_COMMENT As String
    Public Property VR3137_17_COMMENT As String
    Public Property VR3137_3_REQUIRE_DOC As Nullable(Of Boolean)
    Public Property VR3137_3_COMMENT As String
    Public Property VR7_RR_CANCEL_SK As Nullable(Of Integer)
    Public Property VR341_LETTER_LEAVED As Nullable(Of Boolean)
    Public Property VR_HISTORY As String
    Public Property T_STAMP As Nullable(Of Decimal)
    Public Property RR_JOURNAL As Nullable(Of Integer)
    Public Property CREATION_DATE As Nullable(Of Date)

End Class
