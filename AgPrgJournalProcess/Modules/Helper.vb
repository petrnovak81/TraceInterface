Module Helper
    Public Function Comparison(val1 As Object, val2 As Object) As Object
        If IsNumeric(val1) And IsNumeric(val2) Then
            Return val1 <> val2
        Else
            Dim v1 As String = If(val1 IsNot Nothing, val1.ToString.Trim.ToUpper, "")
            Dim v2 As String = If(val2 IsNot Nothing, val2.ToString.Trim.ToUpper, "")
            Return v1 <> v2
        End If
    End Function

    Public Function DateDiff(date1 As Nullable(Of Date), date2 As Nullable(Of Date)) As Boolean
        Try
            If date1 Is Nothing And date2 Is Nothing Then
                Return False
            End If
            If date1 Is Nothing And date2 IsNot Nothing Then
                Return True
            End If
            If date1 IsNot Nothing And date2 Is Nothing Then
                Return True
            End If

            Dim v1 As String = date1.ToString("yyyyMMddHHmm")
            Dim v2 As String = date2.ToString("yyyyMMddHHmm")

            Return v1 <> v2
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function Compare(d As Object, m As Object) As Integer
        Try
            If d Is Nothing And m Is Nothing Then
                Return False
            End If
            If d Is Nothing And m IsNot Nothing Then
                Return True
            End If
            If d IsNot Nothing And m Is Nothing Then
                Return True
            End If
            Const decimalMin As Double = CDbl(Decimal.MinValue)
            Const decimalMax As Double = CDbl(Decimal.MaxValue)
            If d < decimalMin Then
                Return -1
            End If
            If d > decimalMax Then
                Return 1
            End If
            Return CDec(d).CompareTo(m) <> 0
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module
