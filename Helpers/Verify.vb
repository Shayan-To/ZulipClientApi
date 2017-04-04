Public Class Verify

    <DebuggerHidden()>
    Public Shared Sub [True](ByVal T As Boolean, Optional ByVal Message As String = Nothing, Optional ByVal InnerException As Exception = Nothing)
        If Not T Then
            Fail(Message, InnerException)
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub [False](ByVal T As Boolean, Optional ByVal Message As String = Nothing, Optional ByVal InnerException As Exception = Nothing)
        If T Then
            Fail(Message, InnerException)
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub NonNullArg(Of T As Class)(ByVal O As T, Optional ByVal Name As String = Nothing, Optional ByVal Message As String = Nothing)
        If O Is Nothing Then
            If Name Is Nothing Then
                Throw New ArgumentNullException()
            ElseIf Message Is Nothing Then
                Throw New ArgumentNullException(Name)
            Else
                Throw New ArgumentNullException(Name, Message)
            End If
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub TrueArg(ByVal T As Boolean, Optional ByVal Name As String = Nothing, Optional ByVal Message As String = Nothing)
        If Not T Then
            If Name Is Nothing And Message Is Nothing Then
                Throw New ArgumentException()
            Else
                Throw New ArgumentException(Message, Name)
            End If
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub FalseArg(ByVal T As Boolean, Optional ByVal Name As String = Nothing, Optional ByVal Message As String = Nothing)
        If T Then
            If Name Is Nothing And Message Is Nothing Then
                Throw New ArgumentException()
            Else
                Throw New ArgumentException(Message, Name)
            End If
        End If
    End Sub

    <DebuggerHidden()>
    Public Shared Sub Fail(Optional ByVal Message As String = Nothing, Optional ByVal InnerException As Exception = Nothing)
        Throw New InvalidOperationException(Message, InnerException)
    End Sub

    <DebuggerHidden()>
    Public Shared Sub FailArg(Optional ByVal Name As String = Nothing, Optional ByVal Message As String = Nothing)
        If Name Is Nothing And Message Is Nothing Then
            Throw New ArgumentException()
        Else
            Throw New ArgumentException(Message, Name)
        End If
    End Sub

End Class
