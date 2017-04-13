Public Structure PrivateMessageRecipient

    Private Sub New(ByVal RecipientEmailAddresses As IEnumerable(Of String))
        Me._RecipientEmailAddresses = RecipientEmailAddresses.ToArray().AsReadOnly()
    End Sub

    Public Shared Function Create(ByVal RecipientEmailAddresses As IEnumerable(Of String)) As PrivateMessageRecipient
        Return New PrivateMessageRecipient(RecipientEmailAddresses)
    End Function

    Public Shared Function Create(ParamArray ByVal RecipientEmailAddresses As String()) As PrivateMessageRecipient
        Return New PrivateMessageRecipient(RecipientEmailAddresses)
    End Function

    Friend Function GetDataAsJsonList(ByVal Writer As JsonWriter) As String
        Using Writer.OpenList()
            For I = 0 To Me.RecipientEmailAddresses.Count - 1
                Writer.WriteValue(Me.RecipientEmailAddresses.Item(I), True)
            Next
        End Using

        Dim R = Writer.ToString()
        Writer.Reset()
        Return R
    End Function

#Region "RecipientEmailAddresses Read-Only Property"
    Private ReadOnly _RecipientEmailAddresses As IReadOnlyList(Of String)

    Public ReadOnly Property RecipientEmailAddresses As IReadOnlyList(Of String)
        Get
            Return Me._RecipientEmailAddresses
        End Get
    End Property
#End Region

End Structure
