Public Structure RetrievableData(Of T)

    Friend Sub New(ByVal RetrieveDelegate As Func(Of Task(Of T)), ByVal Setter As Action(Of RetrievableData(Of T)))
        Me.RetrieveDelegate = RetrieveDelegate
        Me.Setter = Setter
        Me.LockObject = New Object()
    End Sub

    Public Async Function Retrieve() As Task(Of T)
        Dim R = Await Me.RetrieveDelegate.Invoke()
        SyncLock Me.LockObject
            Me._Value = R
            Me._LastRetrieveTime = Date.UtcNow
            Me.Setter.Invoke(Me)
        End SyncLock
        Return R
    End Function

#Region "Value Read-Only Property"
    Private _Value As T

    ''' <summary>
    ''' The last retrieved data. For getting or refreshing data, call `Retrieve`.
    ''' </summary>
    Public ReadOnly Property Value As T
        Get
            Return Me._Value
        End Get
    End Property
#End Region

#Region "LastRetrieveTime Read-Only Property"
    Private _LastRetrieveTime As Date

    ''' <summary>
    ''' Last UTC time data has been retrieved, or `Date.MaxValue` if it has not.
    ''' </summary>
    Public ReadOnly Property LastRetrieveTime As Date
        Get
            Return Me._LastRetrieveTime
        End Get
    End Property
#End Region

    Private ReadOnly RetrieveDelegate As Func(Of Task(Of T))
    Private ReadOnly Setter As Action(Of RetrievableData(Of T))
    Friend ReadOnly LockObject As Object

End Structure

Public Structure RetrievableData(Of T, TIn)

    Friend Sub New(ByVal RetrieveDelegate As Func(Of TIn, Task(Of T)), ByVal Setter As Action(Of RetrievableData(Of T, TIn)))
        Me.RetrieveDelegate = RetrieveDelegate
        Me.Setter = Setter
        Me.LockObject = New Object()
    End Sub

    Public Async Function Retrieve(ByVal Input As TIn) As Task(Of T)
        Dim R = Await Me.RetrieveDelegate.Invoke(Input)
        SyncLock Me.LockObject
            Me._Value = R
            Me._LastRetrieveTime = Date.UtcNow
            Me.Setter.Invoke(Me)
        End SyncLock
        Return R
    End Function

#Region "Value Read-Only Property"
    Private _Value As T

    ''' <summary>
    ''' The last retrieved data. For getting or refreshing data, call `Retrieve`.
    ''' </summary>
    Public ReadOnly Property Value As T
        Get
            Return Me._Value
        End Get
    End Property
#End Region

#Region "LastRetrieveTime Read-Only Property"
    Private _LastRetrieveTime As Date

    ''' <summary>
    ''' Last UTC time data has been retrieved, or `Date.MaxValue` if it has not.
    ''' </summary>
    Public ReadOnly Property LastRetrieveTime As Date
        Get
            Return Me._LastRetrieveTime
        End Get
    End Property
#End Region

    Private ReadOnly RetrieveDelegate As Func(Of TIn, Task(Of T))
    Private ReadOnly Setter As Action(Of RetrievableData(Of T, TIn))
    Friend ReadOnly LockObject As Object

End Structure
