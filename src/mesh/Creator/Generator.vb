Imports System.IO
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports stdNum = System.Math

Public Class Generator

    ReadOnly args As MeshArguments
    ReadOnly ions As List(Of Double)
    ReadOnly mass_range As DoubleRange

    Sub New(args As MeshArguments)
        Me.args = args
        Me.ions = New List(Of Double)
        Me.mass_range = New DoubleRange(args.massrange)
    End Sub

    Public Function GetExpressionMatrix() As Matrix
        Call ions.Clear()

        Return New Matrix With {
            .sampleID = args.sampleinfo _
                .Select(Function(sample) sample.ID) _
                .ToArray,
            .tag = "mesh_generator",
            .expression = GetIonExpressions.ToArray
        }
    End Function

    Private Iterator Function GetIonExpressions() As IEnumerable(Of DataFrameRow)
        Dim features As Integer = args.featureSize
        Dim mz As Double

        ' generates the metabolite ion feature at first
        For Each compound As MetaboliteAnnotation In args.metabolites.SafeQuery
            features -= 1
        Next

        Do While features > 0
            ' and generates the ion feature with no-names
            ' until the feature pool size decrease to zero
            mz = getIon()
            features -= 1

            Yield New DataFrameRow With {
                .geneID = $"MZ:${mz.ToString("F4")}",
                .experiments = getExpression()
            }
        Loop
    End Function

    Private Function getExpression() As Double()

    End Function

    Private Function getIon() As Double
        Do While True
            Dim mz As Double = randf.GetRandomValue(mass_range)
            ' check mz is duplicated or not
            Dim check As Boolean = Not ions _
                .Any(Function(mzi)
                         Return stdNum.Abs(mzi - mz) <= args.massdiff
                     End Function)

            If check Then
                ions.Add(mz)
                Return mz
            End If
        Loop

        Throw New InvalidDataException("this error will never happends!")
    End Function
End Class
