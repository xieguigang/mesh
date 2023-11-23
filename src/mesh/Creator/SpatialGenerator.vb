Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Distributions.MathGamma
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports std = System.Math

Public Class SpatialGenerator : Inherits Generator

    ReadOnly ordinal As Vector
    ReadOnly ionization As Vector

    Sub New(args As MeshArguments)
        Call MyBase.New(args)

        Me.sample_groups = renderKernels(args) _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
        Me.ordinal = New Vector(Enumerable.Range(0, args.featureSize))
        Me.ionization = gamma(1 + ordinal ^ (1 / 3))
        ' Me.ionization = ionization / ionization.Sum
    End Sub

    ''' <summary>
    ''' if render by kernel, then the corresponding kernel value will
    ''' be attached to the sample via the sampleinfo color data.
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <returns></returns>
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

    Protected Overrides Iterator Function CalsMatrix(sample_group() As SampleInfo, maxinto As Double) As IEnumerable(Of Vector)
        Dim gauss As Vector
        Dim spot_index As Dictionary(Of String, SampleInfo) = sample_groups.Values _
            .IteratesALL _
            .ToDictionary(Function(s)
                              Return s.ID
                          End Function)

        ' use gauss kernel
        For Each spot As SampleInfo In sample_group
            Dim kernel As Double = If(spot_index.ContainsKey(spot.ID), Val(spot_index(spot.ID).color), 0.0)

            ' skip of the missing spot
            If kernel <= 0.0 Then
                Yield Vector.Zero([Dim]:=args.featureSize)
            Else
                gauss = Vector.rand(0.6, 0.99, args.featureSize)
                gauss = gauss * maxinto
                gauss = gauss * ionization

                Yield gauss
            End If
        Next
    End Function

    Protected Overrides Iterator Function SampleMatrix(sample_group() As SampleInfo) As IEnumerable(Of Vector)
        Dim kernel As New Vector(sample_group.Select(Function(s) Val(s.color)))
        Dim zero As Vector = Vector.Zero()
        Dim d As Integer = sample_group.Length / 20
        Dim i As i32 = 0
        Dim t0 As Date = Now
        Dim scale_range As New DoubleRange(kernel)
        Dim index_select As New DoubleRange(0, args.featureSize - 1)
        Dim x As New Vector(ordinal)

        If d = 0 Then
            d = 1
        End If

        x = (x / x.Max).Z
        kernel = (kernel / kernel.Max) * 2

        If Not args.linear_kernel Then
            kernel = kernel.Exp
        End If

        For Each spot As SampleInfo In sample_group
            Dim scale As Double = Val(spot.color)
            Dim offset As Integer = scale_range.ScaleMapping(scale, index_select)
            Dim mu As Double = x(offset)
            Dim sigma As Double = kernel(CInt(i)) ^ std.E
            Dim sample_data As Vector = pnorm.ProbabilityDensity(x, mu, sigma) * ionization

            sample_data += sample_data * Vector.rand(-0.25, 0.25, args.featureSize)

            If ++i Mod d = 0 Then
                Call VBDebugger.EchoLine($"  * [{((i / sample_group.Length) * 100).ToString("F2")}%, {(Now - t0).FormatTime}] {spot.sample_name}...")
            End If

            Yield sample_data * args.intensity_max
        Next
    End Function
End Class
