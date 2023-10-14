Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

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
        Dim scale_range As New DoubleRange(kernel)
        Dim index_select As New DoubleRange(0, args.featureSize - 1)

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
            Dim sigma As Double = kernel(CInt(i))
            Dim sample_data As Vector = pnorm.ProbabilityDensity(x, mu, sigma)

            sample_data += sample_data * Vector.rand(-0.5, 0.5, sample_data.Dim)

            If ++i Mod d = 0 Then
                Call VBDebugger.EchoLine($"  * [{((i / sample_group.Length) * 100).ToString("F2")}%, {(Now - t0).FormatTime}] {spot.sample_name}...")
            End If

            Yield sample_data * args.intensity_max
        Next
    End Function
End Class
