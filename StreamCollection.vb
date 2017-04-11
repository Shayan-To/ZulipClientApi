Public Class StreamCollection
    Implements IReadOnlyDictionary(Of Integer, Stream),
               IReadOnlyDictionary(Of String, Stream),
               IReadOnlyList(Of Stream)

    Public Sub New(ByVal Streams As IEnumerable(Of Stream))
        Me.Items = Streams.ToArray()
        Me.ById = New SimpleDictionary(Of Integer, Stream)(Me.Items.Select(Function(S) New KeyValuePair(Of Integer, Stream)(S.Id, S)))
        Me.ByName = New SimpleDictionary(Of String, Stream)(Me.Items.Select(Function(S) New KeyValuePair(Of String, Stream)(S.Name, S)))
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of Integer, Stream)).Count, IReadOnlyCollection(Of KeyValuePair(Of String, Stream)).Count, IReadOnlyCollection(Of Stream).Count
        Get
            Return Me.Items.Count
        End Get
    End Property

    Public ReadOnly Property ItemByName(Name As String) As Stream Implements IReadOnlyDictionary(Of String, Stream).Item
        Get
            Return Me.ByName.Item(Name)
        End Get
    End Property

    Public ReadOnly Property ItemById(Id As Integer) As Stream Implements IReadOnlyDictionary(Of Integer, Stream).Item
        Get
            Return Me.ById.Item(Id)
        End Get
    End Property

    Public ReadOnly Property ItemAt(Index As Integer) As Stream Implements IReadOnlyList(Of Stream).Item
        Get
            Return Me.Items(Index)
        End Get
    End Property

    Public ReadOnly Property Ids As IEnumerable(Of Integer) Implements IReadOnlyDictionary(Of Integer, Stream).Keys
        Get
            Return Me.Items.Select(Function(S) S.Id)
        End Get
    End Property

    Public ReadOnly Property Names As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, Stream).Keys
        Get
            Return Me.Items.Select(Function(S) S.Name)
        End Get
    End Property

    Private ReadOnly Property Values As IEnumerable(Of Stream) Implements IReadOnlyDictionary(Of Integer, Stream).Values, IReadOnlyDictionary(Of String, Stream).Values
        Get
            Return Me.Items
        End Get
    End Property

    Public Function ContainsName(Name As String) As Boolean Implements IReadOnlyDictionary(Of String, Stream).ContainsKey
        Return Me.ByName.ContainsKey(Name)
    End Function

    Public Function ContainsId(Id As Integer) As Boolean Implements IReadOnlyDictionary(Of Integer, Stream).ContainsKey
        Return Me.ById.ContainsKey(Id)
    End Function

    Public Function TryGetValueByName(Name As String, ByRef Value As Stream) As Boolean Implements IReadOnlyDictionary(Of String, Stream).TryGetValue
        Return Me.ByName.TryGetValue(Name, Value)
    End Function

    Public Function TryGetValueById(Id As Integer, ByRef Value As Stream) As Boolean Implements IReadOnlyDictionary(Of Integer, Stream).TryGetValue
        Return Me.ById.TryGetValue(Id, Value)
    End Function

    Private Function IEnumerable_Ids_GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer, Stream)) Implements IEnumerable(Of KeyValuePair(Of Integer, Stream)).GetEnumerator
        Return Me.Items.Select(Function(S) New KeyValuePair(Of Integer, Stream)(S.Id, S)).GetEnumerator()
    End Function

    Private Function IEnumerable_Names_GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, Stream)) Implements IEnumerable(Of KeyValuePair(Of String, Stream)).GetEnumerator
        Return Me.Items.Select(Function(S) New KeyValuePair(Of String, Stream)(S.Name, S)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Public Function GetEnumerator() As IEnumerator(Of Stream) Implements IEnumerable(Of Stream).GetEnumerator
        Return DirectCast(Me.Items, IEnumerable(Of Stream)).GetEnumerator()
    End Function

    Private ReadOnly ByName As SimpleDictionary(Of String, Stream)
    Private ReadOnly ById As SimpleDictionary(Of Integer, Stream)
    Private ReadOnly Items As Stream()

End Class
