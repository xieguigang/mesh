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

    Protected ReadOnly args As MeshArguments
    Protected ReadOnly ions As FeatureGenerator

    Protected sample_groups As Dictionary(Of String, SampleInfo())

    Sub New(args As MeshArguments)
        Me.args = args
        Me.ions = New FeatureGenerator(args)
        Me.sample_groups = args.sampleinfo _
            .GroupBy(Function(sample) sample.sample_info) _
            .ToDictionary(Function(group) group.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)
    End Sub

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
        Dim zero As Vector = Vector.Zero

        For Each sample_group In sample_groups
            Call VBDebugger.EchoLine("")
            Call VBDebugger.EchoLine($" Processing sample group: {sample_group.Key}...")
            Call VBDebugger.EchoLine($"    -> {sample_group.Value.Length} sample files...")
            Call VBDebugger.EchoLine("")

            sample_info.Add((sample_group.Key, sample_group.Value))

            For Each v As Vector In SampleMatrix(sample_group.Value)
                v(v.IsNaN) = zero
                sample_data.Add(v.ToArray)
            Next
        Next

        'For i As Integer = 0 To ions.Length - 1
        '    Dim ind As Integer = i
        '    Dim v As New Vector(sample_data.Select(Function(r) r(ind)))

        '    v = (((v / (v.Sum + 1)).Exp * 2.0) ^ 2) * args.intensity_max

        '    For j As Integer = 0 To sample_data.Count - 1
        '        sample_data(j)(i) = v(j)
        '    Next
        'Next

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

    Protected Const m_factor As Double = 5.3716
    Protected Const v_factor As Double = m_factor * 0.25

    Protected Overridable Iterator Function SampleMatrix(sample_group As SampleInfo()) As IEnumerable(Of Vector)
        ' get mean of each ion feature in current sample_group
        Dim mean_of_group As Vector = MathGamma.gamma(Vector.rand(args.featureSize) * m_factor) ^ 2
        ' various of each ion features in current sample_group
        Dim various As Double() = MathGamma.gamma(Vector.rand(args.featureSize) * v_factor) / 2.31
        Dim delta As Vector
        Dim sample_data As Vector
        Dim zero As Vector = Vector.Zero(1)
        Dim d As Integer = sample_group.Length / 20
        Dim i As i32 = 0
        Dim t0 As Date = Now

        If d = 0 Then
            d = 1
        End If

        For Each sample As SampleInfo In sample_group
            If ++i Mod d = 0 Then
                Call VBDebugger.EchoLine($"  * [{((i / sample_group.Length) * 100).ToString("F2")}%, {(Now - t0).FormatTime}] {sample.sample_name}...")
            End If

            delta = various _
                .Select(Function(x) randf.NextDouble(-x, x)) _
                .AsVector

            sample_data = mean_of_group + delta
            sample_data(sample_data < 0) = zero
            ' sample_data = sample_data.Log
            ' sample_data(sample_data < 1) = Vector.Zero
            ' sample_data = (sample_data / (sample_data.Sum + 1)) * args.intensity_max
            sample_data = sample_data * args.intensity_max

            Yield sample_data
        Next
    End Function
End Class
