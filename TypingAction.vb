Public Enum TypingAction

    Start = 0
    [Stop] = 1

End Enum

Partial Class Constants

    Public Shared ReadOnly TypingActions As String() =
        (Function() As String()
             Dim R = New String(2 - 1) {}
             R(TypingAction.Start) = "start"
             R(TypingAction.Stop) = "stop"
             Return R
         End Function).Invoke()

End Class
