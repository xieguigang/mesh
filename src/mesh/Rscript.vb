
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' MSdata expression matrix simulator for metabolomics analysis pipeline development and test.
''' </summary>
<Package("mesh")>
<RTypeExport("mesh", GetType(MeshArguments))>
Public Module Rscript

    ''' <summary>
    ''' Create a mesh argument for run metabolomics expression matrix simulation
    ''' </summary>
    ''' <param name="mass_range"></param>
    ''' <param name="feature_size"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mesh")>
    <RApiReturn(GetType(MeshArguments))>
    Public Function meshArgs(<RRawVectorArgument(GetType(Double))>
                             Optional mass_range As Object = "50,1200",
                             Optional feature_size As Integer = 10000,
                             Optional mzdiff As Double = 0.005,
                             Optional intensity_max As Double = 1.0E+15,
                             Optional env As Environment = Nothing) As MeshArguments

        Dim range As Double() = CLRVector.asNumeric(mass_range)

        Return New MeshArguments With {
            .mass_range = range,
            .featureSize = feature_size,
            .massdiff = mzdiff,
            .intensity_max = intensity_max
        }
    End Function

    ''' <summary>
    ''' Set sample labels and group labels information
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="sampleinfo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("samples")>
    <RApiReturn(GetType(MeshArguments))>
    <Extension>
    Public Function setSamples(mesh As MeshArguments,
                               <RRawVectorArgument>
                               sampleinfo As Object,
                               Optional env As Environment = Nothing) As Object

        Dim samples As SampleInfo() = REnv.asVector(Of SampleInfo)(sampleinfo)
        mesh.sampleinfo = samples
        Return mesh
    End Function

    ''' <summary>
    ''' Set spatial id
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="z">
    ''' z axis of the spatial spot
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("samples.spatial")>
    <Extension>
    Public Function setSpatialSamples(mesh As MeshArguments,
                                      <RRawVectorArgument> x As Object,
                                      <RRawVectorArgument> y As Object,
                                      <RRawVectorArgument>
                                      Optional z As Object = Nothing,
                                      <RRawVectorArgument>
                                      Optional kernel As Object = Nothing,
                                      <RRawVectorArgument>
                                      Optional group As Object = Nothing,
                                      Optional env As Environment = Nothing) As Object

        Dim xi As Integer() = CLRVector.asInteger(x)
        Dim yi As Integer() = CLRVector.asInteger(y)
        Dim zi As Integer() = CLRVector.asInteger(z)
        Dim sampleinfo As SampleInfo()
        Dim labels As String() = CLRVector.asCharacter(group)

        If xi.Length <> yi.Length Then
            Return Internal.debug.stop("invalid spatial x,y", env)
        End If

        If Not kernel Is Nothing Then
            mesh.kernel = CLRVector.asNumeric(kernel)
        End If

        If zi.IsNullOrEmpty Then
            ' spatial 2d
            sampleinfo = SpatialInfo.Spatial2D(xi, yi, mesh.kernel, labels).ToArray
        Else
            If xi.Length <> zi.Length Then
                Return Internal.debug.stop("invalid spatial x,y,z", env)
            End If

            ' spatial 3d
            sampleinfo = SpatialInfo.Spatial3D(xi, yi, zi, mesh.kernel, labels).ToArray
        End If

        Return mesh.setSamples(sampleinfo, env)
    End Function

    ''' <summary>
    ''' Create a spatial sample via the given raster matrix
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="raster"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("samples.raster")>
    Public Function samplesRaster(mesh As MeshArguments, raster As RasterScaler, Optional env As Environment = Nothing) As Object
        Dim pixels As PixelData() = raster.GetRasterData.ToArray
        Dim x As Integer() = pixels.Select(Function(p) p.X).ToArray
        Dim y As Integer() = pixels.Select(Function(p) p.Y).ToArray
        Dim kernels As Vector = pixels.Select(Function(p) p.Scale).AsVector

        kernels = kernels / kernels.Max
        kernels(kernels < 1) = Vector.Zero

        mesh.setSpatialSamples(x, y, env:=env)
        mesh.kernel = kernels

        Return mesh
    End Function

    ''' <summary>
    ''' Set metabolite features
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="metabolites">
    ''' A collection of the metabolite annotation data model that contains 
    ''' the basic annotation metadata: 
    ''' 
    ''' 1. id, 
    ''' 2. name, 
    ''' 3. exact mass, 
    ''' 4. and formula data
    ''' </param>
    ''' <param name="adducts"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("metabolites")>
    <RApiReturn(GetType(MeshArguments))>
    Public Function setMetabolites(mesh As MeshArguments,
                                   metabolites As MetaboliteAnnotation(),
                                   <RRawVectorArgument>
                                   adducts As Object,
                                   Optional env As Environment = Nothing) As Object
        mesh.metabolites = metabolites
        mesh.adducts = Math.GetPrecursorTypes(adducts, env)
        Return mesh
    End Function

    ''' <summary>
    ''' Generate the metabolomics expression matrix object
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <returns>
    ''' this function returns a GCModeller expression matrix object or 
    ''' MZKit mzpack data object based on the parameter option of 
    ''' <paramref name="mzpack"/>.
    ''' </returns>
    <ExportAPI("expr1")>
    <RApiReturn(GetType(Matrix), GetType(mzPack))>
    Public Function expr0(mesh As MeshArguments,
                          Optional mzpack As Boolean = False,
                          Optional spatial As Boolean = False) As Object
        If mzpack Then
            Return New Generator(mesh) _
                .GetExpressionMatrix _
                .toMzPack(spatial:=spatial)
        Else
            Return New Generator(mesh).GetExpressionMatrix
        End If
    End Function

    ''' <summary>
    ''' Cast the data expression matrix as the mzkit mzpack object
    ''' </summary>
    ''' <param name="expr1"></param>
    ''' <param name="q"></param>
    ''' <param name="rt_range"></param>
    ''' <param name="spatial">Current expression matrix is a spatial matrix?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.mzPack")>
    <Extension>
    Public Function toMzPack(expr1 As Matrix,
                             Optional q As Double = 0.7,
                             <RRawVectorArgument(GetType(Double))>
                             Optional rt_range As Object = "1,840",
                             Optional spatial As Boolean = False,
                             Optional env As Environment = Nothing) As mzPack

        ' transpose to sample_id in rows and mz
        ' ions in column features
        Dim scans As Matrix = expr1.T
        Dim mz As Vector = scans.sampleID _
            .Select(Function(id) Val(id.Match("\d+(\.\d+)?"))) _
            .ToArray
        Dim scan1 As New List(Of ScanMS1)
        Dim dt As Double = CLRVector.asNumeric(rt_range).Range.Length / scans.expression.Length
        Dim t As Double = 0

        For Each sample As DataFrameRow In scans.expression
            Dim quantile As QuantileEstimationGK = sample.experiments.GKQuantile
            Dim cut As Double = quantile.Query(q)
            Dim expression As New Vector(sample.experiments)
            Dim i = expression > cut
            Dim mzi = mz(i)
            Dim into = expression(i)

            t += dt

            If mzi.Length = 0 Then
                Continue For
            End If

            scan1.Add(New ScanMS1 With {
                .mz = mzi.ToArray,
                .BPC = into.Max,
                .into = into.ToArray,
                .rt = t,
                .TIC = expression.Sum,
                .scan_id = $"[MS1] {sample.geneID}, { .mz.Length} ions; total_ions={ .into.Sum}, basePeak={ .into.Max}, basePeak_m/z={ .mz(which.Max(.into))}"
            })

            If spatial Then
                Dim xyz As String() = sample.geneID.Split(","c)
                Dim current As ScanMS1 = scan1.Last

                current.meta = New Dictionary(Of String, String)
                current.meta.Add("x", xyz(0))
                current.meta.Add("y", xyz(1))

                If xyz.Length > 2 Then
                    current.meta.Add("z", xyz(2))
                End If
            End If
        Next

        Return New mzPack With {
            .Application = If(
                spatial,
                FileApplicationClass.MSImaging,
                FileApplicationClass.LCMS
            ),
            .metadata = New Dictionary(Of String, String) From {
                {"source", expr1.tag}
            },
            .source = expr1.tag,
            .MS = scan1.ToArray
        }
    End Function
End Module
