Public Class UserCollection
    Implements IReadOnlyDictionary(Of String, User), ' For Emails
               IReadOnlyList(Of User)

    ' To get back ids dictionary, see history of this file.

    Public Sub New(ByVal Users As IEnumerable(Of User))
        Me.Items = Users.ToArray()
        Me.ByEmail = New SimpleDictionary(Of String, User)(Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.EmailAddress, U)))
        Me.ByFullName = New SimpleDictionary(Of String, User)(Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.FullName, U)), RelaxSameKeysCheck:=True)
    End Sub

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, User)).Count, IReadOnlyCollection(Of User).Count
        Get
            Return Me.Items.Count
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

    Public ReadOnly Property Emails As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, User).Keys
        Get
            Return Me.Items.Select(Function(U) U.EmailAddress)
        End Get
    End Property

    Public ReadOnly Property FullNames As IEnumerable(Of String)
        Get
            Return Me.Items.Select(Function(U) U.FullName)
        End Get
    End Property

    Private ReadOnly Property Values As IEnumerable(Of User) Implements IReadOnlyDictionary(Of String, User).Values
        Get
            Return Me.Items
        End Get
    End Property

    Public Function ContainsEmail(Email As String) As Boolean Implements IReadOnlyDictionary(Of String, User).ContainsKey
        Return Me.ByEmail.ContainsKey(Email)
    End Function

    Public Function ContainsFullName(FullName As String) As Boolean
        Return Me.ByFullName.ContainsKey(FullName)
    End Function

    Public Function TryGetValueByEmail(Email As String, ByRef Value As User) As Boolean Implements IReadOnlyDictionary(Of String, User).TryGetValue
        Return Me.ByEmail.TryGetValue(Email, Value)
    End Function

    Public Function TryGetValueByFullName(FullName As String, ByRef Value As User) As Boolean
        Return Me.ByFullName.TryGetValue(FullName, Value)
    End Function

    Private Function IEnumerable_Emails_GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, User)) Implements IEnumerable(Of KeyValuePair(Of String, User)).GetEnumerator
        Return Me.Items.Select(Function(U) New KeyValuePair(Of String, User)(U.EmailAddress, U)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

    Public Function GetEnumerator() As IEnumerator(Of User) Implements IEnumerable(Of User).GetEnumerator
        Return DirectCast(Me.Items, IEnumerable(Of User)).GetEnumerator()
    End Function

    Private ReadOnly ByEmail As SimpleDictionary(Of String, User)
    Private ReadOnly ByFullName As SimpleDictionary(Of String, User)
    Private ReadOnly Items As User()

End Class
