Public Class UserCollection
    Implements IReadOnlyDictionary(Of Integer, User),
               IReadOnlyDictionary(Of String, User), ' For Emails
               IReadOnlyList(Of User)

    Public Sub New(ByVal Users As IEnumerable(Of User))
        Me.Items = Users.ToArray()
        Me.ById = New SimpleDictionary(Of Integer, User)(Me.Items.Select(Function(U) New KeyValuePair(Of Integer, User)(U.Id, U)))
        Me.ByEmail = New SimpleDictionary(Of String, User)(Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.Email, U)))
        Me.ByFullName = New SimpleDictionary(Of String, User)(Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.FullName, U)), RelaxSameKeysCheck:=True)
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of Integer, User)).Count, IReadOnlyCollection(Of KeyValuePair(Of String, User)).Count, IReadOnlyCollection(Of User).Count
        Get
            Return Me.Items.Count
        End Get
    End Property

    Public ReadOnly Property ItemById(Id As Integer) As User Implements IReadOnlyDictionary(Of Integer, User).Item
        Get
            Return Me.ById.Item(Id)
        End Get
    End Property

    Public ReadOnly Property ItemByEmail(Email As String) As User Implements IReadOnlyDictionary(Of String, User).Item
        Get
            Return Me.ByEmail.Item(Email)
        End Get
    End Property

    Public ReadOnly Property ItemByFullName(FullName As String) As User
        Get
            Return Me.ByFullName.Item(FullName)
        End Get
    End Property

    Public ReadOnly Property ItemAt(Index As Integer) As User Implements IReadOnlyList(Of User).Item
        Get
            Return Me.Items(Index)
        End Get
    End Property

    Public ReadOnly Property Ids As IEnumerable(Of Integer) Implements IReadOnlyDictionary(Of Integer, User).Keys
        Get
            Return Me.Items.Select(Function(U) U.Id)
        End Get
    End Property

    Public ReadOnly Property Emails As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, User).Keys
        Get
            Return Me.Items.Select(Function(U) U.Email)
        End Get
    End Property

    Public ReadOnly Property FullNames As IEnumerable(Of String)
        Get
            Return Me.Items.Select(Function(U) U.FullName)
        End Get
    End Property

    Private ReadOnly Property Values As IEnumerable(Of User) Implements IReadOnlyDictionary(Of Integer, User).Values, IReadOnlyDictionary(Of String, User).Values
        Get
            Return Me.Items
        End Get
    End Property

    Public Function ContainsId(Id As Integer) As Boolean Implements IReadOnlyDictionary(Of Integer, User).ContainsKey
        Return Me.ById.ContainsKey(Id)
    End Function

    Public Function ContainsEmail(Email As String) As Boolean Implements IReadOnlyDictionary(Of String, User).ContainsKey
        Return Me.ByEmail.ContainsKey(Email)
    End Function

    Public Function ContainsFullName(FullName As String) As Boolean
        Return Me.ByFullName.ContainsKey(FullName)
    End Function

    Public Function TryGetValueById(Id As Integer, ByRef Value As User) As Boolean Implements IReadOnlyDictionary(Of Integer, User).TryGetValue
        Return Me.ById.TryGetValue(Id, Value)
    End Function

    Public Function TryGetValueByEmail(Email As String, ByRef Value As User) As Boolean Implements IReadOnlyDictionary(Of String, User).TryGetValue
        Return Me.ByEmail.TryGetValue(Email, Value)
    End Function

    Public Function TryGetValueByFullName(FullName As String, ByRef Value As User) As Boolean
        Return Me.ByFullName.TryGetValue(FullName, Value)
    End Function

    Private Function IEnumerable_Ids_GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer, User)) Implements IEnumerable(Of KeyValuePair(Of Integer, User)).GetEnumerator
        Return Me.Items.Select(Function(U) New KeyValuePair(Of Integer, User)(U.Id, U)).GetEnumerator()
    End Function

    Private Function IEnumerable_Emails_GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, User)) Implements IEnumerable(Of KeyValuePair(Of String, User)).GetEnumerator
        Return Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.Email, U)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Public Function GetEnumerator() As IEnumerator(Of User) Implements IEnumerable(Of User).GetEnumerator
        Return DirectCast(Me.Items, IEnumerable(Of User)).GetEnumerator()
    End Function

    Private ReadOnly ById As SimpleDictionary(Of Integer, User)
    Private ReadOnly ByEmail As SimpleDictionary(Of String, User)
    Private ReadOnly ByFullName As SimpleDictionary(Of String, User)
    Private ReadOnly Items As User()

End Class
