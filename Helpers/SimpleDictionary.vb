Public Class SimpleDictionary(Of TKey, TValue)
    Implements IReadOnlyDictionary(Of TKey, TValue),
               IReadOnlyList(Of KeyValuePair(Of TKey, TValue))

    Public Sub New(ByVal Items As IEnumerable(Of KeyValuePair(Of TKey, TValue)), ByVal Comparer As IEqualityComparer(Of TKey))
        Me.List = Items.ToArray()
        Me.Comparer = Comparer
        Array.Sort(Me.List, Me.CompareKeyHash)
        For I = 0 To Me.List.Length - 2
            Verify.False(Me.Comparer.Equals(Me.List(I).Key, Me.List(I + 1).Key), "Cannot have two items with the same key.")
        Next
    End Sub

    Public Sub New(ByVal Items As IEnumerable(Of KeyValuePair(Of TKey, TValue)))
        Me.New(Items, EqualityComparer(Of TKey).Default)
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of TKey, TValue)).Count
        Get
            Return Me.List.Length
        End Get
    End Property

    Default Public ReadOnly Property Item(ByVal Key As TKey) As TValue Implements IReadOnlyDictionary(Of TKey, TValue).Item
        Get
            Dim Value As TValue = Nothing
            Verify.True(Me.TryGetValue(Key, Value), "Key not found.")
            Return Value
        End Get
    End Property

    Public ReadOnly Property ItemAt(ByVal Index As Integer) As KeyValuePair(Of TKey, TValue) Implements IReadOnlyList(Of KeyValuePair(Of TKey, TValue)).Item
        Get
            Return Me.List(Index)
        End Get
    End Property

    Public ReadOnly Property Keys As IEnumerable(Of TKey) Implements IReadOnlyDictionary(Of TKey, TValue).Keys
        Get
            Return Me.List.Select(Function(KV) KV.Key)
        End Get
    End Property

    Public ReadOnly Property Values As IEnumerable(Of TValue) Implements IReadOnlyDictionary(Of TKey, TValue).Values
        Get
            Return Me.List.Select(Function(KV) KV.Value)
        End Get
    End Property

    Public Function ContainsKey(key As TKey) As Boolean Implements IReadOnlyDictionary(Of TKey, TValue).ContainsKey
        Dim Value As TValue = Nothing
        Return Me.TryGetValue(key, Value)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, TValue)) Implements IEnumerable(Of KeyValuePair(Of TKey, TValue)).GetEnumerator
        Return DirectCast(Me.List, IReadOnlyList(Of KeyValuePair(Of TKey, TValue))).GetEnumerator()
    End Function

    Public Function TryGetValue(Key As TKey, ByRef Value As TValue) As Boolean Implements IReadOnlyDictionary(Of TKey, TValue).TryGetValue
        Dim T = Me.List.BinarySearch(New KeyValuePair(Of TKey, TValue)(Key, Nothing), Me.CompareKeyHash)
        For I = T.Item1 To T.Item1 + T.Item2 - 1
            If Me.Comparer.Equals(Me.List(I).Key, Key) Then
                Value = Me.List(I).Value
                Return True
            End If
        Next
        Return False
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Private ReadOnly List As KeyValuePair(Of TKey, TValue)()

    Private ReadOnly Comparer As IEqualityComparer(Of TKey)
    Private ReadOnly CompareKeyHash As Comparison(Of KeyValuePair(Of TKey, TValue)) = Function(A, B) Me.Comparer.GetHashCode(A.Key).CompareTo(Me.Comparer.GetHashCode(B.Key))

End Class
