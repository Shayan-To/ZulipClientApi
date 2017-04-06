Friend Class Utilities

    Private Sub New()
        Throw New NotSupportedException()
    End Sub

    Public Shared ReadOnly Utf8NoBomEncoding As Text.UTF8Encoding = New Text.UTF8Encoding(False)

End Class
