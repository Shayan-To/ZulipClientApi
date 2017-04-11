Public Class User
    Inherits Freezable

#Region "FullName Property"
    Private _FullName As String

    Public Property FullName As String
        Get
            Return Me._FullName
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._FullName = Value
        End Set
    End Property
#End Region

#Region "EmailAddress Property"
    Private _EmailAddress As String

    Public Property EmailAddress As String
        Get
            Return Me._EmailAddress
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._EmailAddress = Value
        End Set
    End Property
#End Region

#Region "AvatarUrl Property"
    Private _AvatarUrl As String

    Public Property AvatarUrl As String
        Get
            Return Me._AvatarUrl
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._AvatarUrl = Value
        End Set
    End Property
#End Region

#Region "IsBot Property"
    Private _IsBot As Boolean

    Public Property IsBot As Boolean
        Get
            Return Me._IsBot
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._IsBot = Value
        End Set
    End Property
#End Region

#Region "IsActive Property"
    Private _IsActive As Boolean

    Public Property IsActive As Boolean
        Get
            Return Me._IsActive
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._IsActive = Value
        End Set
    End Property
#End Region

#Region "IsAdmin Property"
    Private _IsAdmin As Boolean

    Public Property IsAdmin As Boolean
        Get
            Return Me._IsAdmin
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._IsAdmin = Value
        End Set
    End Property
#End Region

#Region "BotOwnerEmailAddress Property"
    Private _BotOwnerEmailAddress As String

    Public Property BotOwnerEmailAddress As String
        Get
            Return Me._BotOwnerEmailAddress
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._BotOwnerEmailAddress = Value
        End Set
    End Property
#End Region

End Class
