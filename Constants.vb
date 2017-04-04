Friend Class Constants

    Private Sub New()
        Throw New NotSupportedException()
    End Sub

    Public Class FetchApiKey

        Private Sub New()
            Throw New NotSupportedException()
        End Sub

        Friend Const Input_UserName = "username"
        Friend Const Input_Password = "password"

        Friend Const Output_ApiKey = "api_key"

    End Class

End Class
