Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports novogene.metagenome

Module Module1

    Sub Main()

        Call "H:\result1\04.TaxAnnotation\matches\fungus.csv" _
            .LoadCsv(Of QueryArgument) _
            .Match(TaxonomyGenes.Load("H:\result1\04.TaxAnnotation\GeneNums\sp.csv")) _
            .ToArray _
            .SaveTo("H:\result1\04.TaxAnnotation\matches\fungus-GeneNums.csv")


        Call "H:\result1\04.TaxAnnotation\matches\prokaryotic.csv" _
            .LoadCsv(Of QueryArgument) _
            .Match(TaxonomyGenes.Load("H:\result1\04.TaxAnnotation\GeneNums\sp.csv")) _
            .ToArray _
            .SaveTo("H:\result1\04.TaxAnnotation\matches\prokaryotic-GeneNums.csv")


        Call "H:\result1\04.TaxAnnotation\matches\virus.csv" _
            .LoadCsv(Of QueryArgument) _
            .Match(TaxonomyGenes.Load("H:\result1\04.TaxAnnotation\GeneNums\sp.csv")) _
            .ToArray _
            .SaveTo("H:\result1\04.TaxAnnotation\matches\virus-GeneNums.csv")




        Call "H:\result1\04.TaxAnnotation\matches\fungus.csv" _
            .LoadCsv(Of QueryArgument) _
            .Match("H:\result1\04.TaxAnnotation\GeneNums.BetweenSamples\sp.csv".LoadCsv(Of TaxonomyAbundance)) _
            .ToArray _
            .SaveTo("H:\result1\04.TaxAnnotation\matches\fungus-GeneNums.BetweenSamples.csv")

        Call "H:\result1\04.TaxAnnotation\matches\prokaryotic.csv" _
    .LoadCsv(Of QueryArgument) _
    .Match("H:\result1\04.TaxAnnotation\GeneNums.BetweenSamples\sp.csv".LoadCsv(Of TaxonomyAbundance)) _
    .ToArray _
    .SaveTo("H:\result1\04.TaxAnnotation\matches\prokaryotic-GeneNums.BetweenSamples.csv")

        Call "H:\result1\04.TaxAnnotation\matches\virus.csv" _
    .LoadCsv(Of QueryArgument) _
    .Match("H:\result1\04.TaxAnnotation\GeneNums.BetweenSamples\sp.csv".LoadCsv(Of TaxonomyAbundance)) _
    .ToArray _
    .SaveTo("H:\result1\04.TaxAnnotation\matches\virus-GeneNums.BetweenSamples.csv")

    End Sub
End Module
