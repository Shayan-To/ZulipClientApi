Imports Parameter = System.Collections.Generic.KeyValuePair(Of String, String)

Public Class Client

    Public Sub New(ByVal RealmAddress As String)
        Me._RealmAddress = RealmAddress
    End Sub

    Private Sub VerifyLoggedIn()
        Verify.True(Me.IsLoggedIn, $"`{NameOf(Client)}` must be logged in first to do this. Use `{NameOf(Me.LoginAsync)}` method.")
    End Sub

    Private Function ParametersGoInBody(ByVal HttpMethod As HttpMethod) As Boolean
        Return HttpMethod = HttpMethod.Post
    End Function

    Private Async Function RunApi(ByVal EndPoint As EndPoint,
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
            Dim ApiResult As String = Nothing
            Try
                Res = Me.JsonParser.Value.Parse(Json).AsDictionary()
                Res.Item(Constants.Common.Output_Result).AsValue().VerifyEnum(Constants.ApiResults)
                Res.Item(Constants.Common.Output_Message).AsValue().VerifyString()
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try

            ApiResult = Res.Item(Constants.Common.Output_Result).AsValue().VerifyString().Value
            If ApiResult = Constants.ApiResults(Zulip.ApiResult.Error) Then
                Dim Reason As JsonValueObject = Nothing
                Try
                    Reason = Res.ItemOrDefault(Constants.Common.Output_Reason).AsValue().VerifyString()
                Catch ex As Exception
                    Verify.Fail("Invalid response.", ex)
                End Try

                If Reason IsNot Nothing Then
                    Verify.Fail($"API returned an error ({DirectCast(Response.StatusCode, Integer)} - {Response.StatusDescription}).{Environment.NewLine}Reason: {Reason.Value}{Environment.NewLine}Message: {Res.Item(Constants.Common.Output_Message).AsValue().Value}")
                Else
                    Verify.Fail($"API returned an error ({DirectCast(Response.StatusCode, Integer)} - {Response.StatusDescription}).{Environment.NewLine}Message: {Res.Item(Constants.Common.Output_Message).AsValue().Value}")
                End If
            End If

            Return Res
        End Using
    End Function

    Public Async Function LoginAsync(ByVal LoginData As LoginData) As Task
        Verify.False(Me.IsLoggedIn, $"A single instance of `{NameOf(Client)}` cannot log-in two times.")

        Dim UserName = LoginData.UserName
        Dim ApiKey = LoginData.ApiKey

        If LoginData.Method = LoginMethod.Password Then
            Dim T = Await Me.RunApi(EndPoint.FetchApiKey, HttpMethod.Post, LoginData.GetDataForFetchApiKey(), False)
            Try
                ApiKey = T.Item(Constants.FetchApiKey.Output_ApiKey).AsValue().VerifyString().Value
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

    Private Async Function RetrieveUsers() As Task(Of SimpleDictionary(Of Integer, User))
        Me.VerifyLoggedIn()

        Dim R = Await Me.RunApi(EndPoint.Users, HttpMethod.Get, Nothing)
        Dim Members As JsonListObject = Nothing
        Try
            Members = R.Item(Constants.Users.Output_Members).AsList()
        Catch ex As Exception
            Verify.Fail("Invalid response.", ex)
        End Try

        Dim Res = New KeyValuePair(Of Integer, User)(Members.Count - 1) {}

        For I = 0 To Members.Count - 1
            Dim M As JsonDictionaryObject = Nothing
            Try
                M = Members.Item(I).AsDictionary()
                M.Item(Constants.Users.Output_Members_UserId).AsValue().VerifyInteger()
                M.Item(Constants.Users.Output_Members_FullName).AsValue().VerifyString()
                M.Item(Constants.Users.Output_Members_Email).AsValue().VerifyString()
                M.Item(Constants.Users.Output_Members_IsActive).AsValue().VerifyBoolean()
                M.Item(Constants.Users.Output_Members_IsAdmin).AsValue().VerifyBoolean()
                M.Item(Constants.Users.Output_Members_AvatarUrl).AsValue().VerifyString()
                If M.Item(Constants.Users.Output_Members_IsBot).AsValue().VerifyBoolean().Value = Constants.True Then
                    M.Item(Constants.Users.Output_Members_BotOwner).AsValue().VerifyString()
                End If
            Catch ex As Exception
                Verify.Fail("Invalid response.", ex)
            End Try

            Dim U = New User()
            With U
                .Id = Integer.Parse(M.Item(Constants.Users.Output_Members_UserId).AsValue().Value)
                .FullName = M.Item(Constants.Users.Output_Members_FullName).AsValue().Value
                .Email = M.Item(Constants.Users.Output_Members_Email).AsValue().Value
                .IsActive = M.Item(Constants.Users.Output_Members_IsActive).AsValue().Value = Constants.True
                .IsAdmin = M.Item(Constants.Users.Output_Members_IsAdmin).AsValue().Value = Constants.True
                .AvatarUrl = M.Item(Constants.Users.Output_Members_AvatarUrl).AsValue().Value
                .IsBot = M.Item(Constants.Users.Output_Members_IsBot).AsValue().Value = Constants.True
                If .IsBot Then
                    .BotOwnerEmail = M.Item(Constants.Users.Output_Members_BotOwner).AsValue().Value
                End If

                .Freeze()
            End With

            Res(I) = New KeyValuePair(Of Integer, User)(U.Id, U)
        Next

        Return New SimpleDictionary(Of Integer, User)(Res)
    End Function

#Region "Users Read-Only Property"
    Private _Users As RetrievableData(Of SimpleDictionary(Of Integer, User)) = New RetrievableData(Of SimpleDictionary(Of Integer, User))(AddressOf Me.RetrieveUsers)

    Public ReadOnly Property Users As RetrievableData(Of SimpleDictionary(Of Integer, User))
        Get
            Return Me._Users
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
