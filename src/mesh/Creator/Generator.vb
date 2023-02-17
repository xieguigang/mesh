Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports stdNum = System.Math

Public Class Generator

    ReadOnly args As MeshArguments
    ReadOnly ions As List(Of Double)
    ReadOnly mass_range As DoubleRange
    ReadOnly sample_groups As Dictionary(Of String, SampleInfo())

    Sub New(args As MeshArguments)
        Me.sample_groups = args.sampleinfo _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
        Me.args = args
        Me.ions = New List(Of Double)
        Me.mass_range = New DoubleRange(args.massrange)
    End Sub

    Public Function GetExpressionMatrix() As Matrix
        Call ions.Clear()

        Return New Matrix With {
            .sampleID = sample_groups _
                .Select(Function(group) group.Value.Select(Function(sample) sample.ID)) _
                .IteratesALL _
                .ToArray,
            .tag = "mesh_generator",
            .expression = GetIonExpressions.ToArray
        }
    End Function

    Private Iterator Function GetIonExpressions() As IEnumerable(Of DataFrameRow)
        Dim ions As Double() = createIonFeatures().ToArray
        Dim sample_info As New List(Of (name As String, SampleInfo()))
        Dim sample_data As New List(Of Double())

        For Each sample_group In sample_groups
            sample_info.Add((sample_group.Key, sample_group.Value))
            sample_data.AddRange(SampleMatrix(sample_group.Value).Select(Function(v) v.ToArray))
        Next

#Disable Warning
        For i As Integer = 0 To ions.Length - 1
            Yield New DataFrameRow With {
                .geneID = $"MZ:{ions(i).ToString("F4")}",
                .experiments = sample_data _
                    .Select(Function(v) v(i)) _
                    .ToArray
            }
        Next
#Enable Warning
    End Function

    Private Iterator Function SampleMatrix(sample_group As SampleInfo()) As IEnumerable(Of Vector)
        ' get mean of each ion feature in current sample_group
        Dim mean_of_group As Vector = MathGamma.gamma(Vector.rand(args.featureSize) * 120)
        ' various of each ion features in current sample_group
        Dim various As Double() = MathGamma.gamma(Vector.rand(args.featureSize) * 5)
        Dim delta As Vector

        For Each sample As SampleInfo In sample_group
            delta = various _
                .Select(Function(x) randf.NextDouble(-x, x)) _
                .AsVector

            Yield mean_of_group + delta
        Next
    End Function

    Private Function getIon(meta As MetaboliteAnnotation) As Double
        Dim ion As Double

        For Each adduct As MzCalculator In args.adducts
            ion = adduct.CalcMZ(meta.ExactMass)

            If checkIon(ion) Then
                ions.Add(ion)
                Return ion
            End If
        Next

        Return ion
    End Function

    Private Iterator Function createIonFeatures() As IEnumerable(Of Double)
        Dim features As Integer = args.featureSize
        Dim ion As Double

        For Each meta As MetaboliteAnnotation In args.metabolites.SafeQuery
            ion = getIon(meta)
            features -= 1

            Yield ion
        Next

        Do While features > 0
            ion = getIon()
            features -= 1

            Yield ion
        Loop
    End Function

    ''' <summary>
    ''' check mz is duplicated or not
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns></returns>
    Private Function checkIon(mz As Double) As Boolean
        Return Not ions _
            .Any(Function(mzi)
                     Return stdNum.Abs(mzi - mz) <= args.massdiff
                 End Function)
    End Function

    Private Function getIon() As Double
        Do While True
            Dim mz As Double = randf.GetRandomValue(mass_range)
            ' check mz is duplicated or not
            Dim check As Boolean = checkIon(mz)

            If check Then
                ions.Add(mz)
                Return mz
            End If
        Loop

        Throw New InvalidDataException("this error will never happends!")
    End Function
End Class
