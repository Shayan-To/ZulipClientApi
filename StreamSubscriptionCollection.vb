Public Class StreamSubscriptionCollection
    Implements IReadOnlyDictionary(Of Integer, StreamSubscription),
               IReadOnlyDictionary(Of String, StreamSubscription),
               IReadOnlyList(Of StreamSubscription)

    Public Sub New(ByVal Streams As IEnumerable(Of StreamSubscription))
        Me.Items = Streams.ToArray()
        Me.ByStreamId = New SimpleDictionary(Of Integer, StreamSubscription)(Me.Items.Select(Function(S) New KeyValuePair(Of Integer, StreamSubscription)(S.Stream.Id, S)))
        Me.ByStreamName = New SimpleDictionary(Of String, StreamSubscription)(Me.Items.Select(Function(S) New KeyValuePair(Of String, StreamSubscription)(S.Stream.Name, S)))
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of Integer, StreamSubscription)).Count, IReadOnlyCollection(Of KeyValuePair(Of String, StreamSubscription)).Count, IReadOnlyCollection(Of StreamSubscription).Count
        Get
            Return Me.Items.Count
        End Get
    End Property

    Public ReadOnly Property ItemByStreamName(StreamName As String) As StreamSubscription Implements IReadOnlyDictionary(Of String, StreamSubscription).Item
        Get
            Return Me.ByStreamName.Item(StreamName)
        End Get
    End Property

    Public ReadOnly Property ItemByStreamId(StreamId As Integer) As StreamSubscription Implements IReadOnlyDictionary(Of Integer, StreamSubscription).Item
        Get
            Return Me.ByStreamId.Item(StreamId)
        End Get
    End Property

    Public ReadOnly Property ItemAt(Index As Integer) As StreamSubscription Implements IReadOnlyList(Of StreamSubscription).Item
        Get
            Return Me.Items(Index)
        End Get
    End Property

    Private ReadOnly Property StreamIds As IEnumerable(Of Integer) Implements IReadOnlyDictionary(Of Integer, StreamSubscription).Keys
        Get
            Return Me.Items.Select(Function(S) S.Stream.Id)
        End Get
    End Property

    Private ReadOnly Property StreamNames As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, StreamSubscription).Keys
        Get
            Return Me.Items.Select(Function(S) S.Stream.Name)
        End Get
    End Property

    Public ReadOnly Property Streams As IEnumerable(Of Stream)
        Get
            Return Me.Items.Select(Function(S) S.Stream)
        End Get
    End Property

    Private ReadOnly Property Values As IEnumerable(Of StreamSubscription) Implements IReadOnlyDictionary(Of Integer, StreamSubscription).Values, IReadOnlyDictionary(Of String, StreamSubscription).Values
        Get
            Return Me.Items
        End Get
    End Property

    Public Function ContainsStreamName(StreamName As String) As Boolean Implements IReadOnlyDictionary(Of String, StreamSubscription).ContainsKey
        Return Me.ByStreamName.ContainsKey(StreamName)
    End Function

    Public Function ContainsStreamId(StreamId As Integer) As Boolean Implements IReadOnlyDictionary(Of Integer, StreamSubscription).ContainsKey
        Return Me.ByStreamId.ContainsKey(StreamId)
    End Function

    Public Function TryGetValueByStreamName(StreamName As String, ByRef Value As StreamSubscription) As Boolean Implements IReadOnlyDictionary(Of String, StreamSubscription).TryGetValue
        Return Me.ByStreamName.TryGetValue(StreamName, Value)
    End Function

    Public Function TryGetValueByStreamId(StreamId As Integer, ByRef Value As StreamSubscription) As Boolean Implements IReadOnlyDictionary(Of Integer, StreamSubscription).TryGetValue
        Return Me.ByStreamId.TryGetValue(StreamId, Value)
    End Function

    Private Function IEnumerable_StreamIds_GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer, StreamSubscription)) Implements IEnumerable(Of KeyValuePair(Of Integer, StreamSubscription)).GetEnumerator
        Return Me.Items.Select(Function(S) New KeyValuePair(Of Integer, StreamSubscription)(S.Stream.Id, S)).GetEnumerator()
    End Function

    Private Function IEnumerable_StreamNames_GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, StreamSubscription)) Implements IEnumerable(Of KeyValuePair(Of String, StreamSubscription)).GetEnumerator
        Return Me.Items.Select(Function(S) New KeyValuePair(Of String, StreamSubscription)(S.Stream.Name, S)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Public Function GetEnumerator() As IEnumerator(Of StreamSubscription) Implements IEnumerable(Of StreamSubscription).GetEnumerator
        Return DirectCast(Me.Items, IEnumerable(Of StreamSubscription)).GetEnumerator()
    End Function

    Private ReadOnly ByStreamName As SimpleDictionary(Of String, StreamSubscription)
    Private ReadOnly ByStreamId As SimpleDictionary(Of Integer, StreamSubscription)
    Private ReadOnly Items As StreamSubscription()

End Class
