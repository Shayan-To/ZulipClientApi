Public Structure LoginData

    Private Sub New(ByVal Method As LoginMethod, ByVal UserName As String, ByVal Password As String, ByVal ApiKey As String)
        Assert.True((Method = LoginMethod.Password).Implies(ApiKey Is Nothing))
        Assert.True((Method = LoginMethod.ApiKey).Implies(Password Is Nothing))

        Me._Method = Method
        Me._UserName = UserName
        Me._Password = Password
        Me._ApiKey = ApiKey
    End Sub

    Public Shared Function CreateByPassword(ByVal UserName As String, ByVal Password As String) As LoginData
        Verify.NonNullArg(UserName, NameOf(UserName))
        Verify.NonNullArg(Password, NameOf(Password))

        Return New LoginData(LoginMethod.Password, UserName, Password, Nothing)
    End Function

    Public Shared Function CreateByApiKey(ByVal UserName As String, ByVal ApiKey As String) As LoginData
        Verify.NonNullArg(UserName, NameOf(UserName))
        Verify.NonNullArg(ApiKey, NameOf(ApiKey))

        Return New LoginData(LoginMethod.ApiKey, UserName, Nothing, ApiKey)
    End Function

    Friend Function GetDataForFetchApiKey() As Parameter()
        Return New Parameter() {
                   New Parameter(Constants.FetchApiKey.Input_UserName, Me.UserName),
                   New Parameter(Constants.FetchApiKey.Input_Password, Me.Password)
               }
    End Function

#Region "Method Read-Only Property"
    Private ReadOnly _Method As LoginMethod

    Public ReadOnly Property Method As LoginMethod
        Get
            Return Me._Method
        End Get
    End Property
#End Region

#Region "UserName Read-Only Property"
    Private ReadOnly _UserName As String

    Public ReadOnly Property UserName As String
        Get
            Return Me._UserName
        End Get
    End Property
#End Region

#Region "ApiKey Read-Only Property"
    Private ReadOnly _ApiKey As String

    Public ReadOnly Property ApiKey As String
        Get
            Return Me._ApiKey
        End Get
    End Property
#End Region

#Region "Password Read-Only Property"
    Private ReadOnly _Password As String

    Public ReadOnly Property Password As String
        Get
            Return Me._Password
        End Get
    End Property
#End Region

End Structure

Public Enum LoginMethod

    ApiKey = 0
    Password = 1

End Enum
