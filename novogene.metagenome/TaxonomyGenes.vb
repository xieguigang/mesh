Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class TaxonomyGenes

    Public Property species As String
    Public Property Detail_Taxonomy As String
    Public Property Gene_Num As Integer

    <Collection(NameOf(Gene_IDs), "; ")>
    Public Property Gene_IDs As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function Load(path$) As TaxonomyGenes()
        Dim csv As File = File.Load(path)

        For Each row As RowObject In csv.Skip(1)
            Dim ids$() = row _
                .Skip(3) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .ToArray
            row.Trim(3)
            row.Add(ids.JoinBy("; "))
        Next

        Call csv.Headers.Trim(4)

        Dim out As TaxonomyGenes() = csv.AsDataSource(Of TaxonomyGenes)
        Return out
    End Function
End Class
