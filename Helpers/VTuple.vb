Public NotInheritable Class VTuple

    Private Sub New()
        Throw New NotSupportedException()
    End Sub

    Public Shared Function Create(Of T1, T2)(ByVal Item1 As T1, ByVal Item2 As T2) As VTuple(Of T1, T2)
        Return New VTuple(Of T1, T2)(Item1, Item2)
    End Function

    Public Shared Function Create(Of T1, T2, T3)(ByVal Item1 As T1, ByVal Item2 As T2, ByVal Item3 As T3) As VTuple(Of T1, T2, T3)
        Return New VTuple(Of T1, T2, T3)(Item1, Item2, Item3)
    End Function

End Class

Public Structure VTuple(Of T1, T2)

    Public Sub New(ByVal Item1 As T1, ByVal Item2 As T2)
        Me._Item1 = Item1
        Me._Item2 = Item2
    End Sub

    Public Overrides Function ToString() As String
        Return String.Concat("VTuple{", Me._Item1, ", ", Me._Item2, "}")
    End Function

#Region "Item1 Property"
    Private ReadOnly _Item1 As T1

    Public ReadOnly Property Item1 As T1
        Get
            Return Me._Item1
        End Get
    End Property
#End Region

#Region "Item2 Property"
    Private ReadOnly _Item2 As T2

    Public ReadOnly Property Item2 As T2
        Get
            Return Me._Item2
        End Get
    End Property
#End Region

End Structure

Public Structure VTuple(Of T1, T2, T3)

    Public Sub New(ByVal Item1 As T1, ByVal Item2 As T2, ByVal Item3 As T3)
        Me._Item1 = Item1
        Me._Item2 = Item2
        Me._Item3 = Item3
    End Sub

    Public Overrides Function ToString() As String
        Return String.Concat("VTuple{", Me._Item1, ", ", Me._Item2, ", ", Me._Item3, "}")
    End Function

#Region "Item1 Property"
    Private ReadOnly _Item1 As T1

    Public ReadOnly Property Item1 As T1
        Get
            Return Me._Item1
        End Get
    End Property
#End Region

#Region "Item2 Property"
    Private ReadOnly _Item2 As T2

    Public ReadOnly Property Item2 As T2
        Get
            Return Me._Item2
        End Get
    End Property
#End Region

#Region "Item3 Property"
    Private ReadOnly _Item3 As T3

    Public ReadOnly Property Item3 As T3
        Get
            Return Me._Item3
        End Get
    End Property
#End Region

End Structure
