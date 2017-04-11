Public Class StreamSubscription
    Inherits Freezable

    Protected Overrides Sub OnFreezing()
        Me.Stream.Freeze()
        Me._SubscribersEmailAddresses = Me.SubscribersEmailAddresses.ToArray().AsReadOnly()
    End Sub

#Region "Stream Read-Only Property"
    Private ReadOnly _Stream As Stream = New Stream()

    Public ReadOnly Property Stream As Stream
        Get
            Return Me._Stream
        End Get
    End Property
#End Region

#Region "Color Property"
    Private _Color As Color

    Public Property Color As Color
        Get
            Return Me._Color
        End Get
        Set(ByVal Value As Color)
            Me.VerifyWrite()
            Me._Color = Value
        End Set
    End Property
#End Region

#Region "ShowInHomeView Property"
    Private _ShowInHomeView As Boolean

    ''' <summary>
    ''' This is equivalent to "Not IsMuted".
    ''' </summary>
    Public Property ShowInHomeView As Boolean
        Get
            Return Me._ShowInHomeView
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._ShowInHomeView = Value
        End Set
    End Property
#End Region

#Region "PinToTop Property"
    Private _PinToTop As Boolean

    Public Property PinToTop As Boolean
        Get
            Return Me._PinToTop
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._PinToTop = Value
        End Set
    End Property
#End Region

#Region "ShowAudibleNotifications Property"
    Private _ShowAudibleNotifications As Boolean

    Public Property ShowAudibleNotifications As Boolean
        Get
            Return Me._ShowAudibleNotifications
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._ShowAudibleNotifications = Value
        End Set
    End Property
#End Region

#Region "ShowDesktopNotifications Property"
    Private _ShowDesktopNotifications As Boolean

    Public Property ShowDesktopNotifications As Boolean
        Get
            Return Me._ShowDesktopNotifications
        End Get
        Set(ByVal Value As Boolean)
            Me.VerifyWrite()
            Me._ShowDesktopNotifications = Value
        End Set
    End Property
#End Region

#Region "SubscribersEmailAddresses Read-Only Property"
    Private _SubscribersEmailAddresses As IList(Of String) = New List(Of String)()

    Public ReadOnly Property SubscribersEmailAddresses As IList(Of String)
        Get
            Return Me._SubscribersEmailAddresses
        End Get
    End Property
#End Region

End Class
