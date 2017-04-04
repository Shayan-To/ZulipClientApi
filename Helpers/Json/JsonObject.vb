Friend MustInherit Class JsonObject

    Public Function AsDictionary() As JsonDictionaryObject
        Return DirectCast(Me, JsonDictionaryObject)
    End Function

    Public Function AsList() As JsonListObject
        Return DirectCast(Me, JsonListObject)
    End Function

    Public Function AsValue() As JsonValueObject
        Return DirectCast(Me, JsonValueObject)
    End Function

End Class
