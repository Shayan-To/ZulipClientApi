Public Structure LoginData

#Region "Method Property"
    Private _Method As LoginDataMethod

    Public Property Method As LoginDataMethod
        Get
            Return Me._Method
        End Get
        Set(ByVal Value As LoginDataMethod)
            If Value = LoginDataMethod.ApiKey Then
                Me.Password = Nothing
            ElseIf Value = LoginDataMethod.Password Then
                Me.ApiKey = Nothing
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
            Verify.True(Me.Method = LoginDataMethod.ApiKey, $"Cannot set {NameOf(Me.ApiKey)} when {NameOf(Me.Method)} is not {NameOf(LoginDataMethod.ApiKey)}.")
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
            Verify.True(Me.Method = LoginDataMethod.Password, $"Cannot set {NameOf(Me.Password)} when {NameOf(Me.Method)} is not {NameOf(LoginDataMethod.Password)}.")
            Me._Password = Value
        End Set
    End Property
#End Region

End Structure

Public Enum LoginDataMethod

    ApiKey = 0
    Password = 1

End Enum
