Friend Class JsonValueObject
    Inherits JsonObject

    Public Sub New(ByVal Value As String, ByVal IsString As Boolean)
        Me._Value = Value
        Me._IsString = IsString
    End Sub

    Public Function VerifyString() As JsonValueObject
        Verify.True(Me.IsString, "Value must be a string.")
        Return Me
    End Function

    Public Function VerifyNotString() As JsonValueObject
        Verify.False(Me.IsString, "Value must be a string.")
        Return Me
    End Function

#Region "IsString Read-Only Property"
    Private ReadOnly _IsString As Boolean

    Public ReadOnly Property IsString As Boolean
        Get
            Return Me._IsString
        End Get
    End Property
#End Region

#Region "Value Read-Only Property"
    Private ReadOnly _Value As String

    Public ReadOnly Property Value As String
        Get
            Return Me._Value
        End Get
    End Property
#End Region

End Class
