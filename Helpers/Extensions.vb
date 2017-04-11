Friend Module Extensions

    <Extension()>
    Public Async Function GetRequestStreamAsync(ByVal Self As Net.HttpWebRequest) As Task(Of IO.Stream)
        Dim Res = New TaskCompletionSource(Of IO.Stream)()
        Await Task.Run(Sub() Self.BeginGetRequestStream(Sub(Ar) Res.SetResult(Self.EndGetRequestStream(Ar)), Nothing))
        Return Await Res.Task
    End Function

    <Extension()>
    Public Async Function GetResponseAsync(ByVal Self As Net.HttpWebRequest) As Task(Of Net.HttpWebResponse)
        Dim Res = New TaskCompletionSource(Of Net.HttpWebResponse)()
        Await Task.Run(Sub() Self.BeginGetResponse(Sub(Ar)
                                                       Try
                                                           Res.SetResult(DirectCast(Self.EndGetResponse(Ar), Net.HttpWebResponse))
                                                       Catch ex As Net.WebException
                                                           Res.SetResult(DirectCast(ex.Response, Net.HttpWebResponse))
                                                       End Try
                                                   End Sub, Nothing))
        Return Await Res.Task
    End Function

    <Extension()>
    Public Async Function GetResponseStreamAsync(ByVal Self As Net.WebResponse) As Task(Of IO.Stream)
        Return Await Task.Run(Function() Self.GetResponseStream())
    End Function

    <Extension()>
    Public Function AsDictionary(ByVal Self As JsonObject) As JsonDictionaryObject
        Dim R = TryCast(Self, JsonDictionaryObject)
        Verify.False(R Is Nothing, "Item has to be a dictionary.")
        Return R
    End Function

    <Extension()>
    Public Function AsList(ByVal Self As JsonObject) As JsonListObject
        Dim R = TryCast(Self, JsonListObject)
        Verify.False(R Is Nothing, "Item has to be a list.")
        Return R
    End Function

    <Extension()>
    Private Function AsValue(ByVal Self As JsonObject, ByVal ErrorMessage As String) As JsonValueObject
        Dim R = TryCast(Self, JsonValueObject)
        Verify.False(R Is Nothing, ErrorMessage)
        Return R
    End Function

    <Extension()>
    Public Function GetString(ByVal Self As JsonObject) As String
        Assert.False(Self Is Nothing)
        Dim V = Self.AsValue("A string value was expected, not a list or dictionary.")

#If Not RelaxedStrings Then
        Verify.True(V.IsString, "Value must be a string.")
#End If

        Return V.Value
    End Function

    <Extension()>
    Public Function GetBoolean(ByVal Self As JsonObject) As Boolean
        Assert.False(Self Is Nothing)
        Dim V = Self.AsValue("A boolean value was expected, not a list or dictionary.")

#If Not RelaxedStrings Then
        Verify.False(V.IsString, "Value must be a boolean.")
#End If

        Verify.True(V.Value = Constants.True Or V.Value = Constants.False, "Value must be a boolean.")
        Return V.Value = Constants.True
    End Function

    <Extension()>
    Public Function GetInteger(ByVal Self As JsonObject) As Integer
        Assert.False(Self Is Nothing)
        Dim V = Self.AsValue("An integer value was expected, not a list or dictionary.")

#If Not RelaxedStrings Then
        Verify.False(V.IsString, "Value must be an integer.")
#End If

        Dim T = 0
        Verify.True(Integer.TryParse(V.Value, T), "Value must be an integer.")
        Return T
    End Function

    <Extension()>
    Public Function GetDouble(ByVal Self As JsonObject) As Double
        Assert.False(Self Is Nothing)
        Dim V = Self.AsValue("A number value was expected, not a list or dictionary.")

#If Not RelaxedStrings Then
        Verify.False(V.IsString, "Value must be a number.")
#End If

        Dim T = 0.0
        Verify.True(Double.TryParse(V.Value, T), "Value must be a number.")
        Return T
    End Function

    <Extension()>
    Public Function GetEnum(ByVal Self As JsonObject, ByVal Values As String()) As Integer
        Assert.False(Self Is Nothing)
        Dim V = Self.AsValue("A string value was expected, not a list or dictionary.")

#If Not RelaxedStrings Then
        Verify.True(V.IsString, "Value must be a string.")
#End If

        Dim I = Array.IndexOf(Values, V.Value)
        Verify.True(I <> -1, "Value must be from within the predefined values.")
        Return I
    End Function

    <Extension()>
    Public Function NothingIfEmpty(ByVal Self As String) As String
        If Self.Length = 0 Then
            Return Nothing
        End If
        Return Self
    End Function

    <Extension()>
    Public Function AsReadOnly(Of T)(ByVal Self As IList(Of T)) As ReadOnlyCollection(Of T)
        Return New ReadOnlyCollection(Of T)(Self)
    End Function

    <Extension()>
    Public Sub AddRange(Of T)(ByVal Self As IList(Of T), ByVal Items As IEnumerable(Of T))
        For Each I In Items
            Self.Add(I)
        Next
    End Sub

    Private Function LeastPowerOfTwoOnMin(ByVal Min As Integer) As Integer
        If Min <1 Then
            Return 1
        End If

        ' If Min is a power of two, we should return Min, otherwise, Min * 2
        Dim T = (Min - 1) And Min
        If T = 0 Then
            Return Min
        End If
        Min = T

        Do
            T = (Min - 1) And Min
            If T = 0 Then
                Return Min << 1
            End If
            Min = T
        Loop
    End Function

    ''' <summary>
    ''' Gets the interval in which Value resides in inside a sorted list.
    ''' </summary>
    ''' <param name="Value">The value to look for.</param>
    ''' <returns>
    ''' The tuple (start index, length).
    ''' Start index being the index of fist occurrance of Value, and length being the count of its occurrances.
    ''' If no occurrance of Value has been found, start index will be at the first element larger than Value.
    ''' </returns>
    <Extension()>
    Public Function BinarySearch(Of T)(ByVal Self As T(), ByVal Value As T, ByVal Comp As Comparison(Of T)) As VTuple(Of Integer, Integer)
        Dim Count = LeastPowerOfTwoOnMin(Self.Length + 1) \ 2
        Dim Offset1 = -1

        Do While Count > 0
            If Offset1 + Count < Self.Length Then
                Dim C = Comp.Invoke(Self(Offset1 + Count), Value)
                If C < 0 Then
                    Offset1 += Count
                ElseIf C = 0 Then
                    Exit Do
                End If
            End If
            Count \= 2
        Loop

        Dim Offset2 = Offset1
        If Count > 0 Then
            ' This should have been done in the ElseIf block in the previous loop before the Exit statement.
            Offset2 += Count

            Do While Count > 1
                Count \= 2
                If Offset1 + Count < Self.Length Then
                    If Comp.Invoke(Self(Offset1 + Count), Value) < 0 Then
                        Offset1 += Count
                    End If
                End If
                If Offset2 + Count < Self.Length Then
                    If Comp.Invoke(Self(Offset2 + Count), Value) <= 0 Then
                        Offset2 += Count
                    End If
                End If
            Loop
        End If

        Return VTuple.Create(Offset1 + 1, Offset2 - Offset1)
    End Function

    <Extension()>
    Public Function Implies(ByVal B1 As Boolean, ByVal B2 As Boolean) As Boolean
        Return Not B1 Or B2
    End Function

End Module
