Public Structure RetrievableData(Of T)

    Friend Sub New(ByVal RetrieveDelegate As Func(Of Task(Of T)))
        Me.RetrieveDelegate = RetrieveDelegate
    End Sub

    Public Async Sub Retrieve()
        Me._Value = Await Me.RetrieveDelegate.Invoke()
        Me._LastRetrieveTime = Date.UtcNow
    End Sub

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

End Structure

Public Structure RetrievableData(Of T, TIn)

    Friend Sub New(ByVal RetrieveDelegate As Func(Of TIn, Task(Of T)))
        Me.RetrieveDelegate = RetrieveDelegate
    End Sub

    Public Async Sub Retrieve(ByVal Input As TIn)
        Me._Value = Await Me.RetrieveDelegate.Invoke(Input)
        Me._LastRetrieveTime = Date.UtcNow
    End Sub

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

End Structure
