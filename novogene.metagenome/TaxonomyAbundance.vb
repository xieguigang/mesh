Imports Microsoft.VisualBasic.Serialization.JSON

Public Class TaxonomyAbundance

    Public Property Taxonomy As String
    Public Property Samples As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function Others(data As IEnumerable(Of TaxonomyAbundance)) As TaxonomyAbundance
        For Each x In data
            If x.Taxonomy = NameOf(Others) Then
                Return x
            End If
        Next

        Return Nothing
    End Function
End Class
