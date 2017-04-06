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
                  Writer = New IO.StreamWriter(ReqStream, Text.Encoding.UTF8)
                Await Writer.WriteAsync(QueryParams)
            End Using
        End If

        Using Response = Await Request.GetResponseAsync(),
              ResponseStream = Await Response.GetResponseStreamAsync(),
              Reader = New IO.StreamReader(ResponseStream, Text.Encoding.UTF8)
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
                    Verify.Fail($"API returned an error ({Reason.Value}). {Res.Item(Constants.Common.Output_Message).AsValue().Value}")
                Else
                    Verify.Fail($"API returned an error. {Res.Item(Constants.Common.Output_Message).AsValue().Value}")
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
        Me.AuthHeader = "Basic " & Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(Auth))

        Me._IsLoggedIn = True
    End Function

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
