Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Module MsData

    Public Function PopulateMs1Scan(sampleId As String, t As Double, q As Double, mz As Vector, sampleinfo As SampleInfo(), sample As DataFrameRow()) As ScanMS1

    End Function

    <Extension>
    Public Function PopulateMs1Scan(sample As DataFrameRow,
                                    t As Double,
                                    q As Double,
                                    spatial As Boolean,
                                    mz As Vector,
                                    sampleinfo As Dictionary(Of String, SampleInfo)) As ScanMS1

        Dim expression As New Vector(sample.experiments)
        Dim cut As Double = 0

        If q > 0 Then
            cut = sample.experiments.GKQuantile.Query(q)
        End If

        Dim i = expression > cut
        Dim mzi = mz(i)
        Dim into = expression(i)
        Dim scan_id As String
        Dim sample_data As SampleInfo = Nothing
        Dim s1 As ScanMS1

        If mzi.Length = 0 Then
            Return Nothing
        End If

        If sampleinfo.ContainsKey(sample.geneID) Then
            sample_data = sampleinfo(sample.geneID)
            scan_id = sample_data.sample_name
        Else
            scan_id = sample.geneID
        End If

        s1 = New ScanMS1 With {
            .mz = mzi.ToArray,
            .BPC = into.Max,
            .into = into.ToArray,
            .rt = t,
            .TIC = expression.Sum,
            .scan_id = $"[MS1] {scan_id}, { .mz.Length} ions; total_ions={ .into.Sum.ToString("G3")}, basePeak={ .into.Max.ToString("G3")}, basePeak_m/z={ .mz(which.Max(.into)).ToString("F3")}"
        }

        If spatial Then
            Dim xyz As String() = sample.geneID.Split(","c)

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
End Module
