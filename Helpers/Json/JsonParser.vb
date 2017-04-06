Friend Class JsonParser

    Public Function Parse(ByVal Input As String) As JsonObject
        Me.Input = Input.ToCharArray()
        Me.Index = 0
        Dim Res = Me.Parse()
        Verify.True(Me.ReadToken().Type = TokenType.None, "Invalid JSON format. Expected end of data.")
        Me.StringBuilder.Clear()
        Return Res
    End Function

    Private Function Parse() As JsonObject
        Dim Token = Me.ReadToken()

        Verify.False(Token.Type = TokenType.None, "Invalid JSON format. Unexpected end of data.")

        If (Token.Type And TokenType.Value) = TokenType.Value Then
            Return New JsonValueObject(Token.Value, Token.Type = TokenType.QuotedValue)
        End If

        Assert.True(Token.Type = TokenType.Operator)

        Verify.True(Token.Value = "{" Or Token.Value = "[", "Invalid JSON format. Unexpected token.")

        If Token.Value = "{" Then
            Dim List = New List(Of KeyValuePair(Of String, JsonObject))()

            Do
                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.QuotedValue, "Invalid JSON format. Key must be a string.")
                Dim Key = Token.Value

                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.Operator And Token.Value = ":", "Invalid JSON format. Expected ':'.")

                List.Add(New KeyValuePair(Of String, JsonObject)(Key, Me.Parse()))

                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.Operator And (Token.Value = "," Or Token.Value = "}"), "Invalid JSON format. Expected ',' or '}'.")

                If Token.Value = "}" Then
                    Exit Do
                End If
            Loop

            Return New JsonDictionaryObject(List)
        Else
            Dim List = New List(Of JsonObject)()

            Do
                List.Add(Me.Parse())

                Token = Me.ReadToken()
                Verify.True(Token.Type = TokenType.Operator And (Token.Value = "," Or Token.Value = "]"), "Invalid JSON format. Expected ',' or ']'.")

                If Token.Value = "]" Then
                    Exit Do
                End If
            Loop

            Return New JsonListObject(List)
        End If
    End Function

    Private Function ReadToken() As Token
        Me.SkipWhiteSpace()

        If Me.Index = Me.Input.Length Then
            Return Nothing
        End If

        Dim Ch As Char = Me.Input(Me.Index)

        If Array.IndexOf(Operators, Ch) <> -1 Then
            Me.Index += 1
            Return New Token(Me.Input(Me.Index - 1), TokenType.Operator)
        End If

        If Ch = """"c Then
            Return New Token(ReadQuotedValue(), TokenType.QuotedValue)
        End If

        Return New Token(ReadNonQuotedValue(), TokenType.Value)
    End Function

    Private Function ReadQuotedValue() As String
        Dim Res = Me.StringBuilder.Clear()

        Assert.True(Me.Input(Me.Index) = """"c)

        Dim PrevStart = Me.Index + 1
        For I = Me.Index + 1 To Me.Input.Length - 1
            If Me.Input(I) = """"c Then
                Res.Append(Me.Input, PrevStart, I - PrevStart)
                Me.Index = I + 1
                Exit For
            End If

            If Me.Input(I) = "\"c Then
                Res.Append(Me.Input, PrevStart, I - PrevStart)

                I += 1
                Verify.True(I < Me.Input.Length, "Invalid JSON format. Unexpected end of data.")

                If Me.Input(I) = "u"c Then
                    Res.Append(Me.GetCharFromHex(I + 1))
                    I += 4
                Else
                    Dim Ch As Char = Nothing
                    Verify.True(EscapeDic.TryGetValue(Me.Input(I), Ch), "Invalid JSON format. Invalid escape sequence.")
                    Res.Append(Ch)
                End If

                PrevStart = I + 1

                Continue For
            End If
        Next

        Return Res.ToString()
    End Function

    Private Function GetCharFromHex(ByVal I As Integer) As Char
        Verify.True(I + 3 < Me.Input.Length, "Invalid JSON format. Unexpected end of data.")

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
                Verify.Fail("Invalid JSON format. Invalid escape sequence.")
            End If
        Next

        Return Strings.ChrW(R)
    End Function

    Private Function ReadNonQuotedValue() As String
        Dim StartIndex = Me.Index

        For Me.Index = Me.Index To Me.Input.Length - 1
            If Char.IsWhiteSpace(Me.Input(Me.Index)) Then
                Exit For
            End If
            If Array.IndexOf(Operators, Me.Input(Me.Index)) <> -1 Then
                Exit For
            End If
        Next

        Return New String(Me.Input, StartIndex, Me.Index - StartIndex)
    End Function

    Private Sub SkipWhiteSpace()
        Do While Me.Index < Me.Input.Length AndAlso Char.IsWhiteSpace(Me.Input(Me.Index))
            Me.Index += 1
        Loop
    End Sub

    Private Shared ReadOnly EscapeDic As Dictionary(Of Char, Char) =
                (Function() New Dictionary(Of Char, Char) From {
                         {""""c, """"c},
                         {"/"c, "/"c},
                         {"\"c, "\"c},
                         {"b"c, Strings.ChrW(&H8)},
                         {"f"c, Strings.ChrW(&HC)},
                         {"n"c, Strings.ChrW(&HA)},
                         {"r"c, Strings.ChrW(&HD)},
                         {"t"c, Strings.ChrW(&H9)}
                     }).Invoke()
    Private Shared ReadOnly Operators As Char() = "{}[],:".ToCharArray()

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
        QuotedValue = 3
        [Operator] = 4

    End Enum

End Class
