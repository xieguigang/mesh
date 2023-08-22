Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math

Public Class SpatialGenerator : Inherits Generator

    Sub New(args As MeshArguments)
        Call MyBase.New(args)

        Me.sample_groups = renderKernels(args) _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
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

    Protected Overrides Iterator Function SampleMatrix(sample_group() As SampleInfo) As IEnumerable(Of Vector)
        Dim kernel As New Vector(sample_group.Select(Function(s) Val(s.color)))
        Dim zero As Vector = Vector.Zero()
        Dim d As Integer = sample_group.Length / 20
        Dim i As i32 = 0
        Dim t0 As Date = Now
        Dim x As New Vector(Enumerable.Range(0, args.featureSize))
        Dim max As Double = kernel.Max

        If d = 0 Then
            d = 1
        End If

        kernel = (kernel / kernel.Max * 2).Exp

        For Each spot As SampleInfo In sample_group
            Dim mu As Double = kernel(++i)
            Dim sigma As Double = randf.NextGaussian(mu:=std.Exp(Val(spot.color) / max))
            Dim sample_data As Vector = pnorm.ProbabilityDensity(x, mu, sigma)
            Dim various As Vector = MathGamma.gamma(Vector.rand(min:=-3, max:=3, args.featureSize) * v_factor) / 2.31

            sample_data = sample_data + various
            sample_data(sample_data < 0) = zero

            If ++i Mod d = 0 Then
                Call VBDebugger.EchoLine($"  * [{((i / sample_group.Length) * 100).ToString("F2")}%, {(Now - t0).FormatTime}] {spot.sample_name}...")
            End If

            Yield sample_data * args.intensity_max
        Next
    End Function
End Class
