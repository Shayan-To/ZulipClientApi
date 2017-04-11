Public Class Client

    Public Sub New(ByVal RealmAddress As String)
        If RealmAddress.EndsWith("/") Then
            RealmAddress = RealmAddress.Substring(0, RealmAddress.Length - 1)
            Verify.FalseArg(RealmAddress.EndsWith("/"), NameOf(RealmAddress), "Invalid URL.")
        End If

        Me._RealmAddress = RealmAddress
    End Sub

    Private Sub VerifyLoggedIn()
        Verify.True(Me.IsLoggedIn, $"`{NameOf(Client)}` must be logged in first to do this. Use `{NameOf(Me.LoginAsync)}` method.")
    End Sub

    Private Function ParametersGoInBody(ByVal HttpMethod As HttpMethod) As Boolean
        Return HttpMethod = HttpMethod.Post
    End Function

    Private Async Function RunApiAsync(ByVal EndPoint As EndPoint,
                                       ByVal HttpMethod As HttpMethod,
                                       ByVal Parameters As IEnumerable(Of Parameter),
                                       Optional ByVal UseAutentication As Boolean = True) As Task(Of JsonDictionaryObject)
        Dim Url = $"{Me.RealmAddress}/{RelativeBaseApiAddress}/{Constants.EndPoints(EndPoint)}"

        Dim QueryParamsBuilder = Me.StringBuilder.Value
        If Parameters IsNot Nothing Then
            Dim Bl = False
            For Each P In Parameters
                If Bl Then
                    QueryParamsBuilder.Append("&"c)
                End If
                Bl = True
                QueryParamsBuilder.Append(Net.WebUtility.UrlEncode(P.Key)).Append("="c).Append(Net.WebUtility.UrlEncode(P.Value))
            Next
        End If
        Dim QueryParams = QueryParamsBuilder.ToString()
        QueryParamsBuilder.Clear()

        If QueryParams.Length <> 0 And Not Me.ParametersGoInBody(HttpMethod) Then
            Url &= "?" & QueryParamsBuilder.ToString()
        End If

        Dim Request = Net.WebRequest.CreateHttp(Url)
        Request.Method = Constants.HttpMethods(HttpMethod)

        If UseAutentication Then
            Request.Headers.Item(Net.HttpRequestHeader.Authorization) = Me.AuthHeader
        End If

        If Me.ParametersGoInBody(HttpMethod) Then
            Request.ContentType = Constants.ContentType_FormUrlEncoded

            Using ReqStream = Await Request.GetRequestStreamAsync(),
                  Writer = New IO.StreamWriter(ReqStream, Utilities.Utf8NoBomEncoding)
                Await Writer.WriteAsync(QueryParams)
            End Using
        End If

        Using Response = Await Request.GetResponseAsync(),
              ResponseStream = Await Response.GetResponseStreamAsync(),
              Reader = New IO.StreamReader(ResponseStream, Utilities.Utf8NoBomEncoding)
            Dim Json = Await Reader.ReadToEndAsync()

            Dim Res As JsonDictionaryObject = Nothing
            Dim ApiResult As ApiResult = Nothing
            Dim Message As String = Nothing
            Try
                Res = Me.JsonParser.Value.Parse(Json).AsDictionary()
                ApiResult = DirectCast(Res.Item(Constants.Common.Output_Result).GetEnum(Constants.ApiResults), ApiResult)
                Message = Res.Item(Constants.Common.Output_Message).GetString()
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try

            If ApiResult = ApiResult.Error Then
                Dim Reason As String = Nothing
                Try
                    Reason = Res.ItemOrDefault(Constants.Common.Output_Reason)?.GetString()
                Catch ex As Exception
                    Verify.Fail("Invalid response.", ex)
                End Try

                If Reason IsNot Nothing Then
                    Verify.Fail($"API returned an error ({DirectCast(Response.StatusCode, Integer)} - {Response.StatusDescription}).{Environment.NewLine}Reason: {Reason}{Environment.NewLine}Message: {Message}")
                Else
                    Verify.Fail($"API returned an error ({DirectCast(Response.StatusCode, Integer)} - {Response.StatusDescription}).{Environment.NewLine}Message: {Message}")
                End If
            End If

            Return Res
        End Using
    End Function

    Public Async Function LoginAsync(ByVal Data As LoginData) As Task
        Verify.False(Me.IsLoggedIn, $"A single instance of `{NameOf(Client)}` cannot log-in two times.")

        Verify.TrueArg(Data.UserName IsNot Nothing, NameOf(Data), $"Invalid data. You must provide `{NameOf(Data.UserName)}`.")
        Verify.TrueArg((Data.Method = LoginMethod.Password).Implies(Data.Password IsNot Nothing), NameOf(Data), $"Invalid data. You must provide `{NameOf(Data.Password)}` when `{NameOf(Data.Method)}` is `{NameOf(LoginMethod.Password)}`.")
        Verify.TrueArg((Data.Method = LoginMethod.ApiKey).Implies(Data.ApiKey IsNot Nothing), NameOf(Data), $"Invalid data. You must provide `{NameOf(Data.Password)}` when `{NameOf(Data.Method)}` is `{NameOf(LoginMethod.Password)}`.")

        Dim UserName = Data.UserName
        Dim ApiKey = Data.ApiKey

        If Data.Method = LoginMethod.Password Then
            Dim T = Await Me.RunApiAsync(EndPoint.FetchApiKey, HttpMethod.Post, Data.GetDataForFetchApiKey(), False)
            Try
                ApiKey = T.Item(Constants.FetchApiKey.Output_ApiKey).GetString()
                T.Item(Constants.FetchApiKey.Output_Email).GetString()
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try
        End If

        Me._UserName = UserName
        Me._ApiKey = ApiKey

        Dim Auth = $"{Me.UserName}:{Me.ApiKey}"
        Me.AuthHeader = "Basic " & Convert.ToBase64String(Utilities.Utf8NoBomEncoding.GetBytes(Auth))

        Me._IsLoggedIn = True
    End Function

    Private Async Function RetrieveUsersAsync() As Task(Of UserCollection)
        Me.VerifyLoggedIn()

        Dim R = Await Me.RunApiAsync(EndPoint.Users, HttpMethod.Get, Nothing)
        Dim Members As JsonListObject = Nothing
        Try
            Members = R.Item(Constants.Users.Output_Members).AsList()
        Catch ex As Exception
            Verify.Fail("Invalid response.", ex)
        End Try

        Dim Res = New User(Members.Count - 1) {}

        For I = 0 To Members.Count - 1
            Dim U = New User()

            Try
                Dim T = Members.Item(I).AsDictionary()
                With U
                    '.Id = T.Item(Constants.Users.Output_Members_UserId).GetInteger()
                    .FullName = T.Item(Constants.Users.Output_Members_FullName).GetString()
                    .Email = T.Item(Constants.Users.Output_Members_Email).GetString()
                    .IsActive = T.Item(Constants.Users.Output_Members_IsActive).GetBoolean()
                    .IsAdmin = T.Item(Constants.Users.Output_Members_IsAdmin).GetBoolean()
                    .AvatarUrl = T.Item(Constants.Users.Output_Members_AvatarUrl).GetString()
                    .IsBot = T.Item(Constants.Users.Output_Members_IsBot).GetBoolean()
                    If .IsBot Then
                        .BotOwnerEmail = T.Item(Constants.Users.Output_Members_BotOwner).GetString()
                    End If
                End With
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try

            U.Freeze()
            Res(I) = U
        Next

        Return New UserCollection(Res)
    End Function

    Private Async Function RetrieveStreamsAsync(ByVal Data As StreamsRetrieveData) As Task(Of StreamCollection)
        Me.VerifyLoggedIn()

        Data = Data.Fix()

        Dim R = Await Me.RunApiAsync(EndPoint.Streams, HttpMethod.Get, Data.GetDataForRetrieveStreams())
        Dim Streams As JsonListObject = Nothing
        Try
            Streams = R.Item(Constants.Streams.Output_Streams).AsList()
        Catch ex As Exception
            Verify.Fail("Invalid response.", ex)
        End Try

        Dim Res = New Stream(Streams.Count - 1) {}

        For I = 0 To Streams.Count - 1
            Dim S = New Stream()

            Try
                Dim T = Streams.Item(I).AsDictionary()
                With S
                    .Id = T.Item(Constants.Streams.Output_Streams_StreamId).GetInteger()
                    .Name = T.Item(Constants.Streams.Output_Streams_Name).GetString()
                    .Description = T.Item(Constants.Streams.Output_Streams_Description).GetString()
                    .IsInviteOnly = T.Item(Constants.Streams.Output_Streams_InviteOnly).GetBoolean()
                    If Data.IncludeDefault Then
                        '.IsDefault = T.Item(Constants.Streams.Output_Streams_IsDefault).GetBoolean()
                    End If
                End With
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try

            S.Freeze()
            Res(I) = S
        Next

        Return New StreamCollection(Res)
    End Function

#Region "Users Property"
    Private _Users As RetrievableData(Of UserCollection) = New RetrievableData(Of UserCollection)(AddressOf Me.RetrieveUsersAsync, Sub(V) Me._Users = V)

    Public ReadOnly Property Users As RetrievableData(Of UserCollection)
        Get
            Return Me._Users
        End Get
    End Property
#End Region

#Region "Streams Read-Only Property"
    Private _Streams As RetrievableData(Of StreamCollection, StreamsRetrieveData) = New RetrievableData(Of StreamCollection, StreamsRetrieveData)(AddressOf Me.RetrieveStreamsAsync, Sub(V) Me._Streams = V)

    Public ReadOnly Property Streams As RetrievableData(Of StreamCollection, StreamsRetrieveData)
        Get
            Return Me._Streams
        End Get
    End Property
#End Region

#Region "UserName Read-Only Property"
    Private _UserName As String

    Public ReadOnly Property UserName As String
        Get
            Return Me._UserName
        End Get
    End Property
#End Region

#Region "ApiKey Read-Only Property"
    Private _ApiKey As String

    Public ReadOnly Property ApiKey As String
        Get
            Return Me._ApiKey
        End Get
    End Property
#End Region

#Region "IsLoggedIn Read-Only Property"
    Private _IsLoggedIn As Boolean

    Public ReadOnly Property IsLoggedIn As Boolean
        Get
            Return Me._IsLoggedIn
        End Get
    End Property
#End Region

#Region "RealmAddress Read-Only Property"
    Private ReadOnly _RealmAddress As String

    Public ReadOnly Property RealmAddress As String
        Get
            Return Me._RealmAddress
        End Get
    End Property
#End Region

    Friend Const RelativeBaseApiAddress = "api/v1"

    Private AuthHeader As String
    Private ReadOnly StringBuilder As Threading.ThreadLocal(Of Text.StringBuilder) = New Threading.ThreadLocal(Of Text.StringBuilder)(Function() New Text.StringBuilder(), False)
    Private ReadOnly JsonParser As Threading.ThreadLocal(Of JsonParser) = New Threading.ThreadLocal(Of JsonParser)(Function() New JsonParser(), False)
    Private ReadOnly JsonWriter As Threading.ThreadLocal(Of JsonWriter) = New Threading.ThreadLocal(Of JsonWriter)(Function() New JsonWriter(), False)

End Class
