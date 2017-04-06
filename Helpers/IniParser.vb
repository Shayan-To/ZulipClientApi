Friend Class IniParser

    Public Function Parse(ByVal Input As String) As SimpleDictionary(Of String, String)
        Me.Input = Input.ToCharArray()
        Me.Index = 0
        Dim Res = Me.Parse()
        Me.StringBuilder.Clear()
        Return Res
    End Function

    Private Function Parse() As SimpleDictionary(Of String, String)
        Dim Res = New List(Of KeyValuePair(Of String, String))()
        Dim Section = ""

        Do
            Dim Token As Token
            Do
                Token = Me.ReadToken()
                If Token.Type = TokenType.None Or Token.Type = TokenType.Operator Or Token.Type = TokenType.Value Then
                    Exit Do
                End If
            Loop

            If Token.Type = TokenType.None Then
                Exit Do
            End If

            If Token.Type = TokenType.Operator Then
                Verify.True(Token.Value = "[", "Invalid INI format. Unexpected token.")

                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.Value, "Invalid INI format. Expected section name.")

                Dim Name = Token.Value

                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.Operator And Token.Value = "]", "Invalid INI format. Expected ']'.")

                Token = Me.ReadToken()
                Verify.True((Token.Type And TokenType.NewLine) = TokenType.NewLine, "Invalid INI format. Expected end of line.")

                Section = Name & SectionDelimiter

                Continue Do
            End If

            Assert.True(Token.Type = TokenType.Value)

            Dim Key = Token.Value

            Token = Me.ReadToken()
            Verify.True(Token.Type = TokenType.Operator And Token.Value = "=", "Invalid INI format. Expected '='.")

            Token = Me.ReadToken()
            Verify.True(Token.Type = TokenType.Value, "Invalid INI format. Expected a value.")

            Dim Value = Token.Value

            Token = Me.ReadToken()
            Verify.True((Token.Type And TokenType.NewLine) = TokenType.NewLine, "Invalid INI format. Expected end of line.")

            Res.Add(New KeyValuePair(Of String, String)(Section & Key, Value))
        Loop

        Return New SimpleDictionary(Of String, String)(Res, StringComparer.OrdinalIgnoreCase)
    End Function

    Private Function ReadToken() As Token
        If Me.Index = Me.Input.Length Then
            Return Nothing
        End If

        Dim Ch = Me.Input(Me.Index)

        If Array.IndexOf(Operators, Ch) <> -1 Then
            If Ch = ";"c Then
                Dim I = Me.Index
                Dim L = Me.SkipLine()
                Return New Token(New String(Me.Input, I, L), TokenType.Comment)
            End If

            If Ch = CarriageReturn Or Ch = LineFeed Then
                Me.SkipLine()
                Return New Token("", TokenType.NewLine)
            End If

            Me.Index += 1
            Return New Token(Me.Input(Me.Index - 1), TokenType.Operator)
        End If

        Return New Token(Me.ReadValue(), TokenType.Value)
    End Function

    Private Function ReadValue() As String
        Dim Res = Me.StringBuilder.Clear()

        Dim PrevStart = Me.Index
        For I = Me.Index To Me.Input.Length - 1
            If Array.IndexOf(Operators, Me.Input(I)) <> -1 Then
                Res.Append(Me.Input, PrevStart, I - PrevStart)
                Me.Index = I
                Exit For
            End If

            If Me.Input(I) = "\"c Then
                Res.Append(Me.Input, PrevStart, I - PrevStart)

                I += 1
                Verify.True(I < Me.Input.Length, "Invalid INI format. Unexpected end of data.")

                If Me.Input(I) = "x"c Then
                    Res.Append(Me.GetCharFromHex(I + 1))
                    I += 4
                Else
                    Dim Ch As Char = Nothing
                    Verify.True(EscapeDic.TryGetValue(Me.Input(I), Ch), "Invalid INI format. Invalid escape sequence.")
                    Res.Append(Ch)
                End If

                PrevStart = I + 1

                Continue For
            End If
        Next

        Return Res.ToString()
    End Function

    Private Function GetCharFromHex(ByVal I As Integer) As Char
        Verify.True(I + 3 < Me.Input.Length, "Invalid INI format. Unexpected end of data.")

        Dim R = 0

        For I = I To I + 3
            Dim Ch = Me.Input(I)
            R *= 16
            If "0"c <= Ch And Ch <= "9"c Then
                R += Strings.AscW(Ch) - Strings.AscW("0"c)
            ElseIf "a"c <= Ch And Ch <= "f"c Then
                R += Strings.AscW(Ch) - Strings.AscW("a"c) + 10
            ElseIf "A"c <= Ch And Ch <= "F"c Then
                R += Strings.AscW(Ch) - Strings.AscW("A"c) + 10
            Else
                Verify.Fail("Invalid INI format. Invalid escape sequence.")
            End If
        Next

        Return Strings.ChrW(R)
    End Function

    Private Function SkipLine() As Integer
        Dim StartIndex = Me.Index

        Do While Me.Index < Me.Input.Length AndAlso (Me.Input(Me.Index) <> CarriageReturn And Me.Input(Me.Index) <> LineFeed)
            Me.Index += 1
        Loop

        Dim Length = Me.Index - StartIndex

        If Me.Index < Me.Input.Length Then
            Me.Index += 1
            If (Me.Input(Me.Index - 1) = CarriageReturn And Me.Index < Me.Input.Length) AndAlso Me.Input(Me.Index) = LineFeed Then
                Me.Index += 1
            End If
        End If

        Return Length
    End Function

    Private Const CarriageReturn = Strings.ChrW(&HD)
    Private Const LineFeed = Strings.ChrW(&HA)

    Private Shared ReadOnly EscapeDic As Dictionary(Of Char, Char) =
                (Function() New Dictionary(Of Char, Char) From {
                         {"\"c, "\"c},
                         {";"c, ";"c},
                         {"="c, "="c},
                         {"["c, "["c},
                         {"]"c, "]"c},
                         {"0"c, Strings.ChrW(&H0)},
                         {"a"c, Strings.ChrW(&H7)},
                         {"b"c, Strings.ChrW(&H8)},
                         {"n"c, Strings.ChrW(&HA)},
                         {"r"c, Strings.ChrW(&HD)},
                         {"t"c, Strings.ChrW(&H9)}
                     }).Invoke()
    Private Shared ReadOnly Operators As Char() = ("=[];" & CarriageReturn & LineFeed).ToCharArray()

    Private Const SectionDelimiter = "."c

    Private ReadOnly StringBuilder As Text.StringBuilder = New Text.StringBuilder()

    Private Input As Char()
    Private Index As Integer

    Private Structure Token

        Public Sub New(ByVal Value As String, ByVal Type As TokenType)
            Me.Value = Value
            Me.Type = Type
        End Sub

        Public ReadOnly Value As String
        Public ReadOnly Type As TokenType

    End Structure

    Private Enum TokenType

        None = 0
        Value = 1
        [Operator] = 2
        NewLine = 4
        Comment = 8 + 4

    End Enum

End Class
