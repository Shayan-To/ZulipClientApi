Friend Class IniDictionary
    Implements IReadOnlyDictionary(Of String, String)

    Public Sub New(ByVal Items As IEnumerable(Of KeyValuePair(Of String, String)))
        Me.List = Items.ToArray()
        Array.Sort(Me.List, CompareKeyHash)
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, String)).Count
        Get
            Return Me.List.Length
        End Get
    End Property

    Default Public ReadOnly Property Item(Key As String) As String Implements IReadOnlyDictionary(Of String, String).Item
        Get
            Dim Value As String = Nothing
            Verify.True(Me.TryGetValue(Key, Value), "Key not found.")
            Return Value
        End Get
    End Property

    Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, String).Keys
        Get
            Return Me.List.Select(Function(KV) KV.Key)
        End Get
    End Property

    Public ReadOnly Property Values As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, String).Values
        Get
            Return Me.List.Select(Function(KV) KV.Value)
        End Get
    End Property

    Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, String).ContainsKey
        Dim Value As String = Nothing
        Return Me.TryGetValue(key, Value)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
        Return DirectCast(Me.List, IReadOnlyList(Of KeyValuePair(Of String, String))).GetEnumerator()
    End Function

    Public Function TryGetValue(Key As String, ByRef Value As String) As Boolean Implements IReadOnlyDictionary(Of String, String).TryGetValue
        Dim T = Me.List.BinarySearch(New KeyValuePair(Of String, String)(Key, Nothing), CompareKeyHash)
        For I = 0 To T.Item2 - 1
            If Me.List(T.Item1 + I).Key = Key Then
                Value = Me.List(I).Value
                Return True
            End If
        Next
        Return False
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Private ReadOnly List As KeyValuePair(Of String, String)()

    Private Shared ReadOnly CompareKeyHash As Comparison(Of KeyValuePair(Of String, String)) = Function(A, B) A.Key.GetHashCode() - B.Key.GetHashCode()

End Class
