Public Structure LoginData

    Friend Function GetDataForFetchApiKey() As Parameter()
        Return New Parameter() {
                   New Parameter(Constants.FetchApiKey.Input_UserName, Me.UserName),
                   New Parameter(Constants.FetchApiKey.Input_Password, Me.Password)
               }
    End Function

#Region "Method Property"
    Private _Method As LoginMethod

    Public Property Method As LoginMethod
        Get
            Return Me._Method
        End Get
        Set(ByVal Value As LoginMethod)
            If Value = LoginMethod.ApiKey Then
                Me._Password = Nothing
            ElseIf Value = LoginMethod.Password Then
                Me._ApiKey = Nothing
            Else
                Verify.FailArg(NameOf(Me.Method), "Value must be from within the available values.")
            End If

            Me._Method = Value
        End Set
    End Property
#End Region

#Region "UserName Property"
    Private _UserName As String

    Public Property UserName As String
        Get
            Return Me._UserName
        End Get
        Set(ByVal Value As String)
            Me._UserName = Value
        End Set
    End Property
#End Region

#Region "ApiKey Property"
    Private _ApiKey As String

    Public Property ApiKey As String
        Get
            Return Me._ApiKey
        End Get
        Set(ByVal Value As String)
            Verify.True(Me.Method = LoginMethod.ApiKey, $"Cannot set {NameOf(Me.ApiKey)} when {NameOf(Me.Method)} is not {NameOf(LoginMethod.ApiKey)}.")
            Me._ApiKey = Value
        End Set
    End Property
#End Region

#Region "Password Property"
    Private _Password As String

    Public Property Password As String
        Get
            Return Me._Password
        End Get
        Set(ByVal Value As String)
            Verify.True(Me.Method = LoginMethod.Password, $"Cannot set {NameOf(Me.Password)} when {NameOf(Me.Method)} is not {NameOf(LoginMethod.Password)}.")
            Me._Password = Value
        End Set
    End Property
#End Region

End Structure

Public Enum LoginMethod

    ApiKey = 0
    Password = 1

End Enum
