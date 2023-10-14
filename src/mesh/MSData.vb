Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Module MsData

    Public Function PopulateMs1Scan(sampleId As String, t As Double, q As Double, mz As Vector, sampleinfo As SampleInfo(), sample As DataFrameRow()) As ScanMS1
        Dim expression As Vector = sample.Sum

    End Function

    Private Function AssembleMSScanData(scan_id As String, geneId As String, t As Double, q As Double, spatial As Boolean,
                                        mz As Vector,
                                        expression As Vector,
                                        sample_data As SampleInfo) As ScanMS1
        Dim cut As Double = 0

        If q > 0 Then
            cut = expression.GKQuantile.Query(q)
        End If

        Dim i = expression > cut
        Dim mzi = mz(i)
        Dim into = expression(i)

        If mzi.Length = 0 Then
            Return Nothing
        Else
            Dim totalIons = into.Sum.ToString("G3")
            Dim baseInto = into.Max.ToString("G3")
            Dim basePeak = mzi(which.Max(into)).ToString("F3")

            scan_id = $"[MS1] {scan_id}, {mzi.Length} ions; total_ions={totalIons}, basePeak={baseInto}, basePeak_m/z={basePeak}"
        End If

        Dim s1 As New ScanMS1 With {
            .mz = mzi.ToArray,
            .BPC = into.Max,
            .into = into.ToArray,
            .rt = t,
            .TIC = expression.Sum,
            .scan_id = scan_id
        }

        If spatial Then
            Dim xyz As String() = geneId.Split(","c)

            s1.meta = New Dictionary(Of String, String) From {
                {"x", xyz(0)},
                {"y", xyz(1)}
            }

            If xyz.Length > 2 Then
                s1.meta.Add("z", xyz(2))
            End If
        End If
        If Not sample_data Is Nothing Then
            If s1.meta Is Nothing Then
                s1.meta = New Dictionary(Of String, String)
            End If

            s1.meta("sample") = sample_data.sample_info
        End If

        Return s1
    End Function

    <Extension>
    Public Function PopulateMs1Scan(sample As DataFrameRow,
                                    t As Double,
                                    q As Double,
                                    spatial As Boolean,
                                    mz As Vector,
                                    sampleinfo As Dictionary(Of String, SampleInfo)) As ScanMS1

        Dim expression As New Vector(sample.experiments)
        Dim sample_data As SampleInfo = Nothing
        Dim scan_id As String

        If sampleinfo.ContainsKey(sample.geneID) Then
            sample_data = sampleinfo(sample.geneID)
            scan_id = sample_data.sample_name
        Else
            scan_id = sample.geneID
        End If

        Return AssembleMSScanData(scan_id, sample.geneID, t, q, spatial, mz, expression, sample_data)
    End Function
End Module
