Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
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
    ReadOnly renderKernelProfiles As Boolean

    Sub New(args As MeshArguments)
        Me.sample_groups = renderKernels(args) _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
        Me.args = args
        Me.ions = New List(Of Double)
        Me.mass_range = New DoubleRange(args.mass_range)
        Me.renderKernelProfiles = Not args.kernel.IsNullOrEmpty
    End Sub

    Private Shared Function renderKernels(mesh As MeshArguments) As IEnumerable(Of SampleInfo)
        If mesh.kernel.IsNullOrEmpty Then
            Return mesh.sampleinfo
        Else
            Return mesh.sampleinfo _
                .Select(Function(si, i)
                            si = New SampleInfo(si)
                            si.color = mesh.kernel(i).ToString
                            Return si
                        End Function) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' creates the expression value based on the arguments
    ''' </summary>
    ''' <returns></returns>
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
            Call VBDebugger.EchoLine($"Processing sample group: {sample_group.Key}...")

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
        Dim mean_of_group As Vector = MathGamma.gamma(Vector.rand(args.featureSize) * 50)
        ' various of each ion features in current sample_group
        Dim various As Double() = MathGamma.gamma(Vector.rand(args.featureSize) * 20)
        Dim delta As Vector
        Dim sample_data As Vector
        Dim one As Vector = Vector.Ones(1)
        Dim kernel As Double
        Dim d As Integer = sample_group.Length / 20
        Dim i As i32 = 0
        Dim t0 As Date = Now
        Dim into_range As New DoubleRange(0, args.intensity_max)

        For Each sample As SampleInfo In sample_group
            delta = various _
                .Select(Function(x) randf.NextDouble(-x, x)) _
                .AsVector

            If ++i Mod d = 0 Then
                Call VBDebugger.EchoLine($"  * [{((i / sample_group.Length) * 100).ToString("F2")}%, {(Now - t0).FormatTime}] {sample.sample_name}...")
            End If

            If renderKernelProfiles Then
                kernel = Val(sample.color)
            Else
                kernel = 1
            End If

            sample_data = mean_of_group * kernel + delta
            sample_data(sample_data < 1) = one
            sample_data = sample_data.Log
            sample_data(sample_data < 1) = Vector.Zero

            Dim sample_range As New DoubleRange(sample_data)

            sample_data = sample_data _
                .Select(Function(si)
                            Return sample_range.ScaleMapping(si, into_range)
                        End Function) _
                .AsVector

            sample_data(sample_data < 1) = Vector.Zero

            Yield sample_data
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
