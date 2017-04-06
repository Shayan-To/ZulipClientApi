Public Class Freezable

    Public Sub Freeze()
        If Me.FreezeCalled Then
            Exit Sub
        End If
        Me.FreezeCalled = True

        Me.OnFreezing()
        Me._IsFrozen = True
        Me.OnFrozen()
    End Sub

    Protected Overridable Sub OnFreezing()

    End Sub

    Protected Overridable Sub OnFrozen()

    End Sub

    Protected Sub VerifyWrite()
        Verify.False(Me.IsFrozen, "Cannot change a frozen object.")
    End Sub

    Protected Sub VerifyFrozen()
        Verify.True(Me.IsFrozen, "The object has to be frozen to perform this operation.")
    End Sub

#Region "IsFrozen Property"
    Private _IsFrozen As Boolean

    Public ReadOnly Property IsFrozen As Boolean
        Get
            Return Me._IsFrozen
        End Get
    End Property
#End Region

    Private FreezeCalled As Boolean

End Class
