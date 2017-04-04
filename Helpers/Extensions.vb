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
        Await Task.Run(Sub() Self.BeginGetResponse(Sub(Ar) Res.SetResult(DirectCast(Self.EndGetResponse(Ar), Net.HttpWebResponse)), Nothing))
        Return Await Res.Task
    End Function

    <Extension()>
    Public Async Function GetResponseStreamAsync(ByVal Self As Net.WebResponse) As Task(Of IO.Stream)
        Return Await Task.Run(Function() Self.GetResponseStream())
    End Function

    Private Function LeastPowerOfTwoOnMin(ByVal Min As Integer) As Integer
        If Min < 1 Then
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

End Module
