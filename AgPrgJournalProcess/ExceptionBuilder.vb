Imports System.Reflection

Module ExceptionBuilder
    Public Function Build(args As Object) As String
        Dim _StackTrace As New StackTrace
        Dim _MethodBase As MethodBase = _StackTrace.GetFrame(1).GetMethod()
        Dim arr As New List(Of String)
        For Each i As Object In _MethodBase.GetParameters().ToList
            Dim name = i.Name
            Dim value = i.GetType().GetProperty(i.Name).GetValue(i, Nothing)

            arr.Add("[" & name & "=" & value & "]")
        Next
        Return String.Join(", ", arr)
    End Function
End Module
