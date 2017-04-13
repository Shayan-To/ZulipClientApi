''' <summary>
''' Optional
''' </summary>
Public Structure StreamsRetrieveData

    Private Sub New(ByVal IncludePublic As Boolean,
                   ByVal IncludeSubscribed As Boolean,
                   ByVal IncludeAllActive As Boolean,
                   ByVal IncludeDefault As Boolean)
        Me._IncludePublic = IncludePublic
        Me._IncludeSubscribed = IncludeSubscribed
        Me._IncludeAllActive = IncludeAllActive
        Me._IncludeDefault = IncludeDefault
        Me.IsCreated = True
    End Sub

    Public Shared Function Create(Optional ByVal IncludePublic As Boolean = True,
                                  Optional ByVal IncludeSubscribed As Boolean = True,
                                  Optional ByVal IncludeAllActive As Boolean = False,
                                  Optional ByVal IncludeDefault As Boolean = False) As StreamsRetrieveData
        Return New StreamsRetrieveData(IncludePublic, IncludeSubscribed, IncludeAllActive, IncludeDefault)
    End Function

    Friend Function Fix() As StreamsRetrieveData
        If Me.IsCreated Then
            Return Me
        End If
        Return New StreamsRetrieveData()
    End Function

    Friend Function GetDataForRetrieveStreams() As Parameter()
        Return New Parameter() {
                   New Parameter(Constants.Streams.Input_IncludeAllActive, If(Me.IncludeAllActive, Constants.True, Constants.False)),
                   New Parameter(Constants.Streams.Input_IncludeDefault, If(Me.IncludeDefault, Constants.True, Constants.False)),
                   New Parameter(Constants.Streams.Input_IncludePublic, If(Me.IncludePublic, Constants.True, Constants.False)),
                   New Parameter(Constants.Streams.Input_IncludeSubscribed, If(Me.IncludeSubscribed, Constants.True, Constants.False))
               }
    End Function

#Region "IncludePublic Property"
    Private ReadOnly _IncludePublic As Boolean

    Public ReadOnly Property IncludePublic As Boolean
        Get
            Return Me._IncludePublic
        End Get
    End Property
#End Region

#Region "IncludeSubscribed Property"
    Private ReadOnly _IncludeSubscribed As Boolean

    Public ReadOnly Property IncludeSubscribed As Boolean
        Get
            Return Me._IncludeSubscribed
        End Get
    End Property
#End Region

#Region "IncludeAllActive Property"
    Private ReadOnly _IncludeAllActive As Boolean

    Public ReadOnly Property IncludeAllActive As Boolean
        Get
            Return Me._IncludeAllActive
        End Get
    End Property
#End Region

#Region "IncludeDefault Property"
    Private ReadOnly _IncludeDefault As Boolean

    Public ReadOnly Property IncludeDefault As Boolean
        Get
            Return Me._IncludeDefault
        End Get
    End Property
#End Region

    Private ReadOnly IsCreated As Boolean

End Structure
