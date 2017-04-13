Public Structure TypingNotificationData

    Private Sub New(ByVal Action As TypingAction, ByVal Recipient As PrivateMessageRecipient)
        Me._Action = Action
        Me._Recipient = Recipient
    End Sub

    Public Shared Function Create(ByVal Action As TypingAction, ByVal Recipient As PrivateMessageRecipient) As TypingNotificationData
        Return New TypingNotificationData(Action, Recipient)
    End Function

    Friend Function GetDataForSendTypingNotification(ByVal Writer As JsonWriter) As Parameter()
        Return New Parameter() {
                   New Parameter(Constants.Typing.Input_Op, Constants.TypingActions(Me.Action)),
                   New Parameter(Constants.Typing.Input_To, Me.Recipient.GetDataAsJsonList(Writer))
               }
    End Function

#Region "Action Property"
    Private ReadOnly _Action As TypingAction

    Public ReadOnly Property Action As TypingAction
        Get
            Return Me._Action
        End Get
    End Property
#End Region

#Region "Recipient Read-Only Property"
    Private ReadOnly _Recipient As PrivateMessageRecipient

    Public ReadOnly Property Recipient As PrivateMessageRecipient
        Get
            Return Me._Recipient
        End Get
    End Property
#End Region

End Structure
