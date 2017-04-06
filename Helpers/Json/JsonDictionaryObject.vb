Friend Class JsonDictionaryObject
    Inherits JsonObject
    Implements IReadOnlyDictionary(Of String, JsonObject)

    Public Sub New(ByVal Items As IEnumerable(Of KeyValuePair(Of String, JsonObject)))
        Me.List = Items.ToArray()
        Array.Sort(Me.List, CompareKeyHash)
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, JsonObject)).Count
        Get
            Return Me.List.Length
        End Get
    End Property

    Default Public ReadOnly Property Item(Key As String) As JsonObject Implements IReadOnlyDictionary(Of String, JsonObject).Item
        Get
            Dim Value As JsonObject = Nothing
            Verify.True(Me.TryGetValue(Key, Value), "Key not found.")
            Return Value
        End Get
    End Property

    Public ReadOnly Property ItemOrDefault(Key As String) As JsonObject
        Get
            Dim Value As JsonObject = Nothing
            Me.TryGetValue(Key, Value)
            Return Value
        End Get
    End Property

    Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, JsonObject).Keys
        Get
            Return Me.List.Select(Function(KV) KV.Key)
        End Get
    End Property

    Public ReadOnly Property Values As IEnumerable(Of JsonObject) Implements IReadOnlyDictionary(Of String, JsonObject).Values
        Get
            Return Me.List.Select(Function(KV) KV.Value)
        End Get
    End Property

    Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, JsonObject).ContainsKey
        Dim Value As JsonObject = Nothing
        Return Me.TryGetValue(key, Value)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, JsonObject)) Implements IEnumerable(Of KeyValuePair(Of String, JsonObject)).GetEnumerator
        Return DirectCast(Me.List, IReadOnlyList(Of KeyValuePair(Of String, JsonObject))).GetEnumerator()
    End Function

    Public Function TryGetValue(Key As String, ByRef Value As JsonObject) As Boolean Implements IReadOnlyDictionary(Of String, JsonObject).TryGetValue
        Dim T = Me.List.BinarySearch(New KeyValuePair(Of String, JsonObject)(Key, Nothing), CompareKeyHash)
        For I = 0 To T.Item2 - 1
            If Me.List(T.Item1 + I).Key = Key Then
                Value = Me.List(T.Item1 + I).Value
                Return True
            End If
        Next
        Return False
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Private ReadOnly List As KeyValuePair(Of String, JsonObject)()

    Private Shared ReadOnly CompareKeyHash As Comparison(Of KeyValuePair(Of String, JsonObject)) = Function(A, B) A.Key.GetHashCode().CompareTo(B.Key.GetHashCode())

End Class
