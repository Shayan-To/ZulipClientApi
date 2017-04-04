Friend Class JsonListObject
    Inherits JsonObject
    Implements IReadOnlyList(Of JsonObject)

    Public Sub New(ByVal Items As IEnumerable(Of JsonObject))
        Me.List = Items.ToArray()
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of JsonObject).Count
        Get
            Return Me.List.Length
        End Get
    End Property

    Default Public ReadOnly Property Item(Index As Integer) As JsonObject Implements IReadOnlyList(Of JsonObject).Item
        Get
            Return Me.List(Index)
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator(Of JsonObject) Implements IEnumerable(Of JsonObject).GetEnumerator
        Return DirectCast(Me.List, IReadOnlyList(Of JsonObject)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Private ReadOnly List As JsonObject()

End Class
