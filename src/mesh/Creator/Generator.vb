Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' expression data matrix generator
''' </summary>
Public Class Generator

    ReadOnly args As MeshArguments
    ReadOnly sample_groups As Dictionary(Of String, SampleInfo())
    ReadOnly renderKernelProfiles As Boolean
    ReadOnly ions As FeatureGenerator

    Sub New(args As MeshArguments)
        Me.renderKernelProfiles = Not args.kernel.IsNullOrEmpty
        Me.sample_groups = renderKernels(args) _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
        Me.args = args
        Me.ions = New FeatureGenerator(args)
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

    ''' <summary>
    ''' creates the expression value based on the arguments
    ''' </summary>
    ''' <returns></returns>
    Public Function GetExpressionMatrix() As Matrix
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
        Dim ions As Double() = Me.ions.Clear().CreateIonFeatures().ToArray
        Dim sample_info As New List(Of (name As String, SampleInfo()))
        Dim sample_data As New List(Of Double())

        For Each sample_group In sample_groups
            Call VBDebugger.EchoLine("")
            Call VBDebugger.EchoLine($" Processing sample group: {sample_group.Key}...")
            Call VBDebugger.EchoLine($"    -> {sample_group.Value.Length} sample files...")
            Call VBDebugger.EchoLine("")

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
        Dim m_factor As Double = 11.3
        Dim v_factor As Double = m_factor * 0.25
        Dim mean_of_group As Vector = MathGamma.gamma(Vector.rand(args.featureSize) * m_factor)
        ' various of each ion features in current sample_group
        Dim various As Double() = MathGamma.gamma(Vector.rand(args.featureSize) * v_factor)
        Dim delta As Vector
        Dim sample_data As Vector
        Dim zero As Vector = Vector.Zero(1)
        Dim kernel As Double
        Dim d As Integer = sample_group.Length / 20
        Dim i As i32 = 0
        Dim t0 As Date = Now

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
            sample_data(sample_data < 10) = zero
            ' sample_data = sample_data.Log
            ' sample_data(sample_data < 1) = Vector.Zero
            sample_data = sample_data * args.intensity_max

            Yield sample_data
        Next
    End Function
End Class
