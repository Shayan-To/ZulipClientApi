Friend Class JsonWriter

    Private Sub WriteEscaped(ByVal S As String)
        Dim PrevStart = 0
        Dim I = 0
        For I = 0 To S.Length - 1
            Dim Ch = S.Chars(I)
            Dim ECh = Ch
            If EscapeDic.TryGetValue(Ch, ECh) Then
                Me.Builder.Append(S, PrevStart, I - PrevStart).Append("\"c).Append(ECh)
                PrevStart = I + 1
            ElseIf Char.IsControl(Ch) Then
                Me.Builder.Append(S, PrevStart, I - PrevStart).Append("\u").Append(Convert.ToString(Strings.AscW(Ch), 16).PadLeft(4, "0"c))
                PrevStart = I + 1
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
        Verify.False(Me.State = WriterState.End, "Cannot write after write is finished.")
        Verify.True((Me.State = WriterState.Dictionary).Implies(Me.HasKeyBefore), $"Cannot write a value in place of a key in a dictionary. Use {NameOf(Me.WriteKey)} instead.")

        Me.WriteComma()
        If Quoted Then
            Me.Builder.Append(""""c)
            Me.WriteEscaped(Value)
            Me.Builder.Append(""""c)
        Else
            Me.Builder.Append(Value)
        End If

        If Me.State = WriterState.Begin Then
            Me.State = WriterState.End
        End If
        Me.HasKeyBefore = False
        Me.HasValueBefore = True
    End Sub

    Public Sub WriteKey(ByVal Name As String)
        Verify.False(Me.State = WriterState.End, "Cannot write after write is finished.")
        Verify.True(Me.State = WriterState.Dictionary, "Cannot write a key outside a dictionary.")
        Verify.False(Me.HasKeyBefore, "Cannot write a key immediately after another.")

        Me.WriteComma()
        Me.Builder.Append(""""c)
        Me.WriteEscaped(Name)
        Me.Builder.Append(""":")
        Me.HasValueBefore = False
        Me.HasKeyBefore = True
    End Sub

    Public Function OpenList() As Opening
        Verify.False(Me.State = WriterState.End, "Cannot write after write is finished.")

        Me.WriteComma()
        Me.Builder.Append("["c)

        Dim R = New Opening(Me, "]"c, If(Me.State = WriterState.Begin, WriterState.End, Me.State))

        Me.HasValueBefore = False
        Me.State = WriterState.List

        Return R
    End Function

    Public Function OpenDictionary() As Opening
        Verify.False(Me.State = WriterState.End, "Cannot write after write is finished.")

        Me.WriteComma()
        Me.Builder.Append("{"c)

        Dim R = New Opening(Me, "}"c, If(Me.State = WriterState.Begin, WriterState.End, Me.State))

        Me.HasValueBefore = False
        Me.State = WriterState.Dictionary

        Return R
    End Function

    Private Sub CloseOpening(ByVal ClosingChar As Char, ByVal ClosingState As WriterState)
        Verify.False(Me.HasKeyBefore, "Cannot close while a key is pending its value.")

        Me.Builder.Append(ClosingChar)
        Me.State = ClosingState
        Me.HasValueBefore = True
    End Sub

    Public Sub Reset()
        Me.Builder.Clear()
        Me.State = WriterState.Begin
        Me.HasValueBefore = False
        Me.HasKeyBefore = False
    End Sub

    Public Overrides Function ToString() As String
        Verify.True(Me.State = WriterState.End, "Write must be finished before JSON string can be got.")
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
    Private HasKeyBefore As Boolean
    Private State As WriterState = WriterState.Begin

    Public Structure Opening
        Implements IDisposable

        Friend Sub New(ByVal Writer As JsonWriter, ByVal ClosingChar As Char, ByVal ClosingState As WriterState)
            Me.Writer = Writer
            Me.ClosingChar = ClosingChar
            Me.ClosingState = ClosingState
        End Sub

        Friend Sub Dispose() Implements IDisposable.Dispose
            Me.Writer.CloseOpening(Me.ClosingChar, Me.ClosingState)
        End Sub

        Public Sub Close()
            Me.Dispose()
        End Sub

        Private ReadOnly ClosingState As WriterState
        Private ReadOnly ClosingChar As Char
        Private ReadOnly Writer As JsonWriter

    End Structure

    Friend Enum WriterState

        Begin
        Dictionary
        List
        [End]

    End Enum

End Class
