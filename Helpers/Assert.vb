Public Class Assert

    <DebuggerHidden()>
    Public Shared Sub [True](ByVal T As Boolean, Optional ByVal Message As String = Nothing)
        If Not T Then
            Fail(Message)
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub [False](ByVal T As Boolean, Optional ByVal Message As String = Nothing)
        If T Then
            Fail(Message)
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub Fail(Optional ByVal Message As String = Nothing)
        Throw New AssertionException(Message)
    End Sub

End Class
