Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Linq

Public Module TaxonomyMatch

    <Extension>
    Public Iterator Function Match(query As QueryArgument, data As IEnumerable(Of TaxonomyAbundance)) As IEnumerable(Of TaxonomyAbundance)
        Dim expression As Expression = query.Expression.Build(Tokens.op_AND)

        For Each x As TaxonomyAbundance In data
            If expression.Match(x.Taxonomy) Then
                Yield x
            End If
        Next
    End Function

    <Extension>
    Public Iterator Function Match(query As IEnumerable(Of QueryArgument), data As IEnumerable(Of TaxonomyAbundance)) As IEnumerable(Of QueryArgument)
        Dim profiles = data.ToArray
        Dim LQuery = From q As QueryArgument
                     In query.AsParallel
                     Select q, m = q.Match(profiles)

        For Each result In LQuery
            Dim hits$() = result.m.Select(Function(x) x.Taxonomy).ToArray
            Call result.q.Data.Add("Taxonomy", hits.JoinBy("; "))

            Yield result.q
        Next
    End Function

    <Extension>
    Public Iterator Function Match(query As QueryArgument, data As IEnumerable(Of TaxonomyGenes)) As IEnumerable(Of TaxonomyGenes)
        Dim expression As Expression = query.Expression.Build(Tokens.op_AND)

        For Each x As TaxonomyGenes In data
            If expression.Match(x.Detail_Taxonomy) Then
                Yield x
            End If
        Next
    End Function

    <Extension>
    Public Iterator Function Match(query As IEnumerable(Of QueryArgument), data As IEnumerable(Of TaxonomyGenes)) As IEnumerable(Of QueryArgument)
        Dim profiles = data.ToArray
        Dim LQuery = From q As QueryArgument
                     In query.AsParallel
                     Select q, m = q.Match(profiles).ToArray

        For Each result In LQuery
            Dim hits$() = result.m _
                .Select(Function(x) x.Gene_IDs) _
                .IteratesALL _
                .ToArray
            Dim taxonomys = result.m _
                .Select(Function(x) Strings.Split(x.Detail_Taxonomy, "s__").Last) _
                .Distinct _
                .ToArray

            Call result.q.Data.Add("Taxonomy", taxonomys.JoinBy("; "))
            Call result.q.Data.Add("genes", hits.JoinBy("; "))

            Yield result.q
        Next
    End Function
End Module
