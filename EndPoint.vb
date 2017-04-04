Friend Enum EndPoint

    FetchApiKey = 0
    RealmEmoji = 1
    Events = 2
    Messages = 3
    Register = 4
    Streams = 5
    UsersMeSubscriptions = 6
    Typing = 7
    Users = 8
    UsersMePointer = 9

End Enum

Partial Class Constants

    Public Shared ReadOnly EndPoints As String() =
        (Function() As String()
             Dim R = New String(10 - 1) {}
             R(EndPoint.FetchApiKey) = "fetch_api_key"
             R(EndPoint.RealmEmoji) = "realm/emoji"
             R(EndPoint.Events) = "events"
             R(EndPoint.Messages) = "messages"
             R(EndPoint.Register) = "register"
             R(EndPoint.Streams) = "streams"
             R(EndPoint.UsersMeSubscriptions) = "users/me/subscriptions"
             R(EndPoint.Typing) = "typing"
             R(EndPoint.Users) = "users"
             R(EndPoint.UsersMePointer) = "users/me/pointer"
             Return R
         End Function).Invoke()

End Class
