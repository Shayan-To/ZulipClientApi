Friend Class JsonWriter

    Private Sub WriteEscaped(ByVal S As String)
        Dim PrevStart = 0
        Dim I = 0
        For I = 0 To S.Length - 1
            Dim Ch = S.Chars(I)
            Dim ECh = Ch
            If EscapeDic.TryGetValue(Ch, ECh) Then
                Me.Builder.Append(S, PrevStart, I - PrevStart).Append("\"c).Append(ECh)
                PrevStart = I
            ElseIf Char.IsControl(Ch) Then
                Me.Builder.Append(S, PrevStart, I - PrevStart).Append("\u").Append(Convert.ToString(Strings.AscW(Ch), 16).PadLeft(4, "0"c))
                PrevStart = I
            End If
        Next

        Me.Builder.Append(S, PrevStart, I - PrevStart)
    End Sub

    Private Sub WriteComma()
        If Me.HasValueBefore Then
            Me.Builder.Append(","c)
        End If
    End Sub

    Public Sub WriteValue(ByVal Value As String, ByVal Quoted As Boolean)
        Me.WriteComma()
        If Quoted Then
            Me.Builder.Append(""""c)
            Me.WriteEscaped(Value)
            Me.Builder.Append(""""c)
        Else
            Me.Builder.Append(Value)
        End If
        Me.HasValueBefore = True
    End Sub

    Public Sub WriteKey(ByVal Name As String)
        Me.WriteComma()
        Me.Builder.Append(""""c)
        Me.WriteEscaped(Name)
        Me.Builder.Append(""""c)
        Me.HasValueBefore = False
    End Sub

    Public Function OpenList() As Opening
        Me.WriteComma()
        Me.Builder.Append("["c)
        Return New Opening(Me, "]"c)
        Me.HasValueBefore = False
    End Function

    Public Function OpenDictionary() As Opening
        Me.WriteComma()
        Me.Builder.Append("{"c)
        Return New Opening(Me, "}"c)
        Me.HasValueBefore = False
    End Function

    Private Sub CloseOpening(ByVal ClosingChar As Char)
        Me.Builder.Append(ClosingChar)
        Me.HasValueBefore = True
    End Sub

    Public Sub Clear()
        Me.Builder.Clear()
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Builder.ToString()
    End Function

    Private Shared ReadOnly EscapeDic As Dictionary(Of Char, Char) =
                (Function() New Dictionary(Of Char, Char) From {
                         {""""c, """"c},
                         {"/"c, "/"c},
                         {"\"c, "\"c},
                         {Strings.ChrW(&H8), "b"c},
                         {Strings.ChrW(&HC), "f"c},
                         {Strings.ChrW(&HA), "n"c},
                         {Strings.ChrW(&HD), "r"c},
                         {Strings.ChrW(&H9), "t"c}
                     }).Invoke()

    Private ReadOnly Builder As Text.StringBuilder = New Text.StringBuilder()
    Private HasValueBefore As Boolean

    Public Structure Opening
        Implements IDisposable

        Friend Sub New(ByVal Writer As JsonWriter, ByVal ClosingChar As Char)
            Me.Writer = Writer
            Me.ClosingChar = ClosingChar
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Writer.CloseOpening(Me.ClosingChar)
        End Sub

        Private ReadOnly ClosingChar As Char
        Private ReadOnly Writer As JsonWriter

    End Structure

End Class
