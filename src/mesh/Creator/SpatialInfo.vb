Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Module SpatialInfo

    Const templ_spatial2D As String = "[raster-%y.raw] [MS1][Scan_%d][%x,%y] FTMS + p NSI Full ms [%min-%max]"
    Const templ_spatial3D As String = "[raster-%y.raw] [MS1][Scan_%d][%x,%y,%z] FTMS + p NSI Full ms [%min-%max]"

    <Extension>
    Public Iterator Function Spatial2D(x As Integer(), y As Integer(),
                                       tag As String,
                                       Optional labels As String() = Nothing,
                                       Optional template As String = templ_spatial2D) As IEnumerable(Of SampleInfo)
        Dim scan_id As String

        If labels.IsNullOrEmpty Then
            labels = If(tag, "spatial_2D").Replicate(x.Length).ToArray
        End If

        For i As Integer = 0 To x.Length - 1
            'If kernels Is Nothing Then
            '    scan_id = $"[{x(i)},{y(i)}] {labels(i)}_{i + 1}"
            'Else
            '    scan_id = $"[{x(i)},{y(i)}] {labels(i)}_{i + 1}; KERNEL={kernels(i).ToString("F3")}"
            'End If
            scan_id = FillScanId(template, x(i), y(i), 0, i + 1)

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

    Private Function FillScanId(template As String, x As Integer, y As Integer, z As Integer, i As Integer) As String
        Dim sb As New StringBuilder(template)

        sb.Replace("%y", y)
        sb.Replace("%x", x)
        sb.Replace("%z", z)
        sb.Replace("%d", i)

        Return sb.ToString
    End Function

    <Extension>
    Public Iterator Function Spatial3D(x As Integer(), y As Integer(), z As Integer(),
                                       tag As String,
                                       Optional labels As String() = Nothing,
                                       Optional template As String = templ_spatial3D) As IEnumerable(Of SampleInfo)
        Dim scan_id As String

        If labels.IsNullOrEmpty Then
            labels = If(tag, "spatial_3D").Replicate(x.Length).ToArray
        End If

        For i As Integer = 0 To x.Length - 1
            'If kernels Is Nothing Then
            '    scan_id = $"[{x(i)},{y(i)},{z(i)}] {labels(i)}_{i + 1}"
            'Else
            '    scan_id = $"[{x(i)},{y(i)},{z(i)}] {labels(i)}_{i + 1}; KERNEL={kernels(i).ToString("F3")}"
            'End If
            scan_id = FillScanId(template, x(i), y(i), z(i), i + 1)

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
