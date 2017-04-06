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

    Public Class Users

        Private Sub New()
            Throw New NotSupportedException()
        End Sub

        Public Const Output_Members = "members"
        Public Const Output_Members_UserId = "user_id"
        Public Const Output_Members_FullName = "full_name"
        Public Const Output_Members_Email = "email"
        Public Const Output_Members_IsBot = "is_bot"
        Public Const Output_Members_IsActive = "is_active"
        Public Const Output_Members_IsAdmin = "is_admin"
        Public Const Output_Members_AvatarUrl = "avatar_url"
        ''' <summary>If IsBot</summary>
        Public Const Output_Members_BotOwner = "bot_owner"

    End Class

    Public Class Streams

        Private Sub New()
            Throw New NotSupportedException()
        End Sub

        Public Const Input_IncludePublic = "include_public"
        Public Const Input_IncludeSubscribed = "include_subscribed"
        Public Const Input_IncludeAllActive = "include_all_active"
        Public Const Input_IncludeDefault = "include_default"

        Public Const Output_Streams = "streams"
        Public Const Output_Streams_StreamId = "stream_id"
        Public Const Output_Streams_InviteOnly = "invite_only"
        Public Const Output_Streams_Description = "description"
        Public Const Output_Streams_Name = "name"
        ''' <summary>If IncludeDefault</summary>
        Public Const Output_Streams_IsDefault = "is_default"

    End Class

End Class
