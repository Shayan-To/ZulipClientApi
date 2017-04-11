Public Structure Color

    Private Sub New(ByVal Red As Byte, ByVal Green As Byte, ByVal Blue As Byte)
        Me._Red = Red
        Me._Green = Green
        Me._Blue = Blue
    End Sub

    Public Shared Function FromHexString(ByVal HexString As String) As Color
        If HexString.Chars(0) = "#"c Then
            HexString = HexString.Substring(1)
        End If
        Verify.TrueArg(HexString.Length = 6, NameOf(HexString), "Argument must consist of exactly 6 hex digits, 2 for each of red and green and blue.")
        Verify.TrueArg(HexString.Cast(Of Char).All(Function(D) ("0"c <= D And D <= "9"c) Or ("A"c <= D And D <= "F"c) Or ("a"c <= D And D <= "f"c)), NameOf(HexString), "Digits must all be from within the hexadecimal (base-16) digits.")

        Dim Red = CByte(Convert.ToInt32(HexString.Substring(0, 2), 16))
        Dim Green = CByte(Convert.ToInt32(HexString.Substring(2, 2), 16))
        Dim Blue = CByte(Convert.ToInt32(HexString.Substring(4, 2), 16))

        Return New Color(Red, Green, Blue)
    End Function

    Public Shared Function FromBytes(ByVal Red As Byte, ByVal Green As Byte, ByVal Blue As Byte) As Color
        Return New Color(Red, Green, Blue)
    End Function

#Region "Red Read-Only Property"
    Private ReadOnly _Red As Byte

    Public ReadOnly Property Red As Byte
        Get
            Return Me._Red
        End Get
    End Property
#End Region

#Region "Green Read-Only Property"
    Private ReadOnly _Green As Byte

    Public ReadOnly Property Green As Byte
        Get
            Return Me._Green
        End Get
    End Property
#End Region

#Region "Blue Read-Only Property"
    Private ReadOnly _Blue As Byte

    Public ReadOnly Property Blue As Byte
        Get
            Return Me._Blue
        End Get
    End Property
#End Region

End Structure
