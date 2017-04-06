Friend Class Constants

    Private Sub New()
        Throw New NotSupportedException()
    End Sub

    Public Const [True] = "true"
    Public Const [False] = "false"
    Public Const Null = "null"

    Public Const ContentType_FormUrlEncoded = "application/x-www-form-urlencoded"

    Public Class Common

        Private Sub New()
            Throw New NotSupportedException()
        End Sub

        Public Const Output_Result = "result"
        Public Const Output_Message = "msg"
        ''' <summary>Optional</summary>
        Public Const Output_Reason = "reason"

    End Class

    Public Class FetchApiKey

        Private Sub New()
            Throw New NotSupportedException()
        End Sub

        Public Const Input_UserName = "username"
        Public Const Input_Password = "password"

        Public Const Output_ApiKey = "api_key"
        Public Const Output_Email = "email"

    End Class

End Class
