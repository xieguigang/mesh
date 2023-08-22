Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Module SpatialInfo

    <Extension>
    Public Iterator Function Spatial2D(x As Integer(), y As Integer(),
                                       kernels As Double(),
                                       Optional labels As String() = Nothing) As IEnumerable(Of SampleInfo)
        Dim scan_id As String

        If labels.IsNullOrEmpty Then
            labels = "spatial_2D".Replicate(x.Length).ToArray
        End If

        For i As Integer = 0 To x.Length - 1
            If kernels Is Nothing Then
                scan_id = $"[{x(i)},{y(i)}] {labels(i)}_{i + 1}"
            Else
                scan_id = $"[{x(i)},{y(i)}] {labels(i)}_{i + 1}; KERNEL={kernels(i).ToString("F3")}"
            End If

            Yield New SampleInfo With {
                .ID = $"{x(i)},{y(i)}",
                .batch = 1,
                .color = "black",
                .injectionOrder = i + 1,
                .sample_info = labels(i),
                .sample_name = scan_id,
                .shape = "rect"
            }
        Next
    End Function

    <Extension>
    Public Iterator Function Spatial3D(x As Integer(), y As Integer(), z As Integer(),
                                       kernels As Double(),
                                       Optional labels As String() = Nothing) As IEnumerable(Of SampleInfo)

        Dim scan_id As String

        If labels.IsNullOrEmpty Then
            labels = "spatial_3D".Replicate(x.Length).ToArray
        End If

        For i As Integer = 0 To x.Length - 1
            If kernels Is Nothing Then
                scan_id = $"[{x(i)},{y(i)},{z(i)}] {labels(i)}_{i + 1}"
            Else
                scan_id = $"[{x(i)},{y(i)},{z(i)}] {labels(i)}_{i + 1}; KERNEL={kernels(i).ToString("F3")}"
            End If

            Yield New SampleInfo With {
                .batch = 1,
                .color = "black",
                .ID = $"{x(i)},{y(i)},{z(i)}",
                .injectionOrder = i + 1,
                .sample_info = labels(i),
                .sample_name = scan_id,
                .shape = "rect"
            }
        Next
    End Function

End Module
