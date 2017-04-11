Public Class Stream
    Inherits Freezable

#Region "Id Property"
    Private _Id As Integer

    Public Property Id As Integer
        Get
            Return Me._Id
        End Get
        Set(ByVal Value As Integer)
            Me.VerifyWrite()
            Me._Id = Value
        End Set
    End Property
#End Region

#Region "IsInviteOnly Property"
    Private _IsInviteOnly As Boolean

    Public Property IsInviteOnly As Boolean
        Get
            Return Me._IsInviteOnly
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._IsInviteOnly = Value
        End Set
    End Property
#End Region

#Region "Description Property"
    Private _Description As String

    Public Property Description As String
        Get
            Return Me._Description
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._Description = Value
        End Set
    End Property
#End Region

#Region "Name Property"
    Private _Name As String

    Public Property Name As String
        Get
            Return Me._Name
        End Get
        Set(ByVal Value As String)
            Me.VerifyWrite()
            Me._Name = Value
        End Set
    End Property
#End Region

#Region "IsDefault Property"
    Private _IsDefault As Boolean?

    Public Property IsDefault As Boolean?
        Get
            Return Me._IsDefault
        End Get
        Set(ByVal Value As Boolean?)
            Me.VerifyWrite()
            Me._IsDefault = Value
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
            Me._EmailAddress = Value
        End Set
    End Property
#End Region

End Class
