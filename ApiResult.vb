Public Enum ApiResult

    Success = 0
    [Error] = 1

End Enum

Partial Class Constants

    Public Shared ReadOnly ApiResults As String() =
        (Function() As String()
             Dim R = New String(2 - 1) {}
             R(ApiResult.Success) = "success"
             R(ApiResult.Error) = "error"
             Return R
         End Function).Invoke()

End Class
