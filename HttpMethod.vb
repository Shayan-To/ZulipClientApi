Friend Enum HttpMethod

    [Get]
    Post

End Enum

Partial Class Constants

    Public Shared ReadOnly HttpMethods As String() =
        (Function() As String()
             Dim R = New String(2 - 1) {}
             R(HttpMethod.Get) = "GET"
             R(HttpMethod.Post) = "POST"
             Return R
         End Function).Invoke()

End Class
