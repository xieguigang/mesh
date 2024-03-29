
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports pip = SMRUCC.Rsharp.Runtime.Internal.Object.pipeline
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
    ''' <param name="features">
    ''' set the number of the ion features in the generated dataset, 
    ''' andalso you could set the ion set manually from this 
    ''' parameter.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mesh")>
    <RApiReturn(GetType(MeshArguments))>
    Public Function meshArgs(<RRawVectorArgument(GetType(Double))>
                             Optional mass_range As Object = "50,1200",
                             <RRawVectorArgument>
                             Optional features As Object = 10000,
                             Optional mzdiff As Double = 0.005,
                             <RRawVectorArgument(GetType(String))>
                             Optional adducts As Object = "[M+H]+|[M+Na]+|[M+K]+|[M+NH4]+|[M+H2O+H]+|[M-H2O+H]+",
                             Optional intensity_max As Double = 100000.0,
                             Optional source_tag As String = Generator.source_tag,
                             Optional env As Environment = Nothing) As MeshArguments

        Dim range As Double() = CLRVector.asNumeric(mass_range)
        Dim precursors As MzCalculator() = Math.GetPrecursorTypes(adducts, env)
        Dim ionSet As Double() = CLRVector.asNumeric(features)

        Return New MeshArguments With {
            .mass_range = range,
            .featureSize = If(ionSet.Length > 1, ionSet.Length, CInt(ionSet(Scan0))),
            .massdiff = mzdiff,
            .intensity_max = intensity_max,
            .adducts = precursors,
            .opts = SearchOption.SmallMolecule(DNPOrWileyType.Wiley, True),
            .ionSet = If(ionSet.Length > 1, ionSet, Nothing),
            .source_tag = source_tag
        }
    End Function

    ''' <summary>
    ''' Get the ion feature set based on the configed mesh argument
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <returns></returns>
    <ExportAPI("featureSet")>
    Public Function featureSet(mesh As MeshArguments) As Double()
        Return New FeatureGenerator(mesh) _
            .Clear() _
            .CreateIonFeatures() _
            .Shuffles _
            .ToArray
    End Function

    <ExportAPI("formula")>
    Public Function formulaGenerator(mesh As MeshArguments,
                                     <RRawVectorArgument(GetType(Double))> Optional C As Object = "1,9",
                                     <RRawVectorArgument(GetType(Double))> Optional H As Object = "0,60",
                                     <RRawVectorArgument(GetType(Double))> Optional O As Object = "0,18",
                                     <RRawVectorArgument(GetType(Double))> Optional N As Object = "0,10",
                                     <RRawVectorArgument(GetType(Double))> Optional P As Object = "0,8",
                                     <RRawVectorArgument(GetType(Double))> Optional S As Object = "0,8",
                                     <RRawVectorArgument(GetType(Double))> Optional F As Object = "0,15",
                                     <RRawVectorArgument(GetType(Double))> Optional Cl As Object = "0,8",
                                     <RRawVectorArgument(GetType(Double))> Optional Br As Object = "0,5",
                                     <RRawVectorArgument(GetType(Double))> Optional Si As Object = "0,8") As MeshArguments

        mesh.opts = New SearchOption(Short.MinValue, Short.MaxValue) _
            .AddElement("C", CLRVector.asInteger(C)) _
            .AddElement("H", CLRVector.asInteger(H)) _
            .AddElement("O", CLRVector.asInteger(O)) _
            .AddElement("N", CLRVector.asInteger(N)) _
            .AddElement("P", CLRVector.asInteger(P)) _
            .AddElement("S", CLRVector.asInteger(S)) _
            .AddElement("F", CLRVector.asInteger(F)) _
            .AddElement("Cl", CLRVector.asInteger(Cl)) _
            .AddElement("Br", CLRVector.asInteger(Br)) _
            .AddElement("Si", CLRVector.asInteger(Si))

        Return mesh
    End Function

    ''' <summary>
    ''' Set sample labels and group labels information
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="sampleinfo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' set sampleinfo value to the mesh sampleinfo property
    ''' </remarks>
    <ExportAPI("samples")>
    <RApiReturn(GetType(MeshArguments))>
    <Extension>
    Public Function setSamples(mesh As MeshArguments,
                               <RRawVectorArgument>
                               sampleinfo As Object,
                               Optional env As Environment = Nothing) As Object
#Disable Warning
        Dim samples As SampleInfo() = REnv.asVector(Of SampleInfo)(sampleinfo)
        mesh.sampleinfo = samples
#Enable Warning
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
    ''' <remarks>
    ''' the sampleinfo color was used as the total ion in each spatial spot
    ''' </remarks>
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
                                      Optional template As String = "[raster-%y.raw][Scan_%d][%x,%y] FTMS + p NSI Full ms [%min-%max]",
                                      Optional linear_kernel As Boolean = False,
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

        Call processTemplateString(mesh, template)

        If zi.IsNullOrEmpty Then
            ' spatial 2d
            sampleinfo = SpatialInfo.Spatial2D(xi, yi, Nothing, labels, template).ToArray
        Else
            If xi.Length <> zi.Length Then
                Return Internal.debug.stop("invalid spatial x,y,z", env)
            End If

            ' spatial 3d
            sampleinfo = SpatialInfo.Spatial3D(xi, yi, zi, Nothing, labels, template).ToArray
        End If

        mesh.spatial = True
        mesh.linear_kernel = linear_kernel

        Return mesh.setSamples(sampleinfo, env)
    End Function

    <Extension>
    Private Function processTemplateString(mesh As MeshArguments, ByRef template As String) As String
        template = template.Replace("%min", mesh.mass_range.Min.ToString("F4"))
        template = template.Replace("%max", mesh.mass_range.Max.ToString("F4"))
        Return template
    End Function

    <ExportAPI("sample.cal_spatial")>
    Public Function sample_cal_spatial(mesh As MeshArguments,
                                       <RRawVectorArgument> x As Object,
                                       <RRawVectorArgument> y As Object,
                                       level As Double,
                                       Optional template As String = "[raster-%y.raw][Scan_%d][%x,%y] FTMS + p NSI Full ms [%min-%max]",
                                       Optional env As Environment = Nothing) As Object

        Dim px As Integer() = CLRVector.asInteger(x)
        Dim py As Integer() = CLRVector.asInteger(y)
        Dim level_factor As String = level.ToString

        mesh.processTemplateString(template)
        mesh.cals = mesh.cals.JoinIterates(
            SpatialInfo.Spatial2D(px, py, $"cal-{level}P", Nothing, template) _
                .Select(Function(si)
                            si.color = level_factor
                            Return si
                        End Function)
        ) _
        .ToArray

        Return mesh
    End Function

    ''' <summary>
    ''' Create a spatial sample via the given raster matrix
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="raster"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("samples.raster")>
    Public Function samplesRaster(mesh As MeshArguments, raster As RasterScaler,
                                  <RRawVectorArgument>
                                  Optional label As Object = Nothing,
                                  Optional kernel_cutoff As Double = 0.0001,
                                  Optional linear_kernel As Boolean = False,
                                  Optional template As String = "[raster-%y.raw][Scan_%d][%x,%y] FTMS + p NSI Full ms [%min-%max]",
                                  Optional env As Environment = Nothing) As Object

        Dim pixels As PixelData() = raster.GetRasterData _
            .Where(Function(a) a.Scale > 0) _
            .ToArray
        Dim x As Integer() = pixels.Select(Function(p) p.X).ToArray
        Dim y As Integer() = pixels.Select(Function(p) p.Y).ToArray
        Dim kernels As Vector = pixels.Select(Function(p) p.Scale).AsVector
        Dim labels As String() = CLRVector.asCharacter(label)
        ' Dim trim As Double = kernels.FindThreshold(q:=TrIQ)

        ' kernels(kernels > trim) = Vector.Scalar(trim)
        kernels = kernels / kernels.Max
        kernels(kernels < kernel_cutoff) = Vector.Zero

        If kernels.Dim > 1 AndAlso labels.TryCount = 1 Then
            ' is a segment
            mesh.processTemplateString(template)
            mesh.sampleinfo = mesh.sampleinfo.JoinIterates(
                SpatialInfo.Spatial2D(x, y, labels(Scan0), Nothing, template)
            ) _
            .ToArray
            mesh.kernel = mesh.kernel _
                .JoinIterates(kernels) _
                .ToArray
        Else
            ' is sample
            mesh.setSpatialSamples(
                x, y,
                kernel:=kernels,
                group:=labels,
                env:=env,
                linear_kernel:=linear_kernel,
                template:=template
            )
            mesh.kernel = kernels
            mesh.linear_kernel = linear_kernel
        End If

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
    ''' 
    ''' this parameter value could be the annotation abstract model: <see cref="MetaboliteAnnotation"/>,
    ''' or the kegg compound model <see cref="Compound"/> from the GCModeller package.
    ''' </param>
    ''' <param name="adducts"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("metabolites")>
    <RApiReturn(GetType(MeshArguments))>
    Public Function setMetabolites(mesh As MeshArguments,
                                   <RRawVectorArgument> metabolites As Object,
                                   <RRawVectorArgument(GetType(String))>
                                   Optional adducts As Object = "[M+H]+|[M+Na]+|[M+K]+|[M+NH4]+|[M+H2O+H]+|[M-H2O+H]+",
                                   Optional env As Environment = Nothing) As Object

        Dim list As pip = pip.TryCreatePipeline(Of MetaboliteAnnotation)(metabolites, env, suppress:=True)

        If list.isError Then
            ' try kegg compound model
            list = pip.TryCreatePipeline(Of Compound)(metabolites, env)

            If list.isError Then
                Return list.getError
            End If

            list = list.populates(Of Compound)(env) _
                .Select(Function(c)
                            Return New MetaboliteAnnotation With {
                                .CommonName = c.commonNames.DefaultFirst(c.entry),
                                .ExactMass = FormulaScanner.EvaluateExactMass(c.formula),
                                .Formula = c.formula,
                                .Id = c.entry
                            }
                        End Function) _
                .DoCall(AddressOf pip.CreateFromPopulator)
        End If

        mesh.metabolites = list.populates(Of MetaboliteAnnotation)(env).ToArray
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
                          Optional q As Double = 0.7,
                          Optional spatial As Boolean = False) As Object

        Dim sim As Generator

        If mesh.spatial Then
            sim = New SpatialGenerator(mesh)
        Else
            sim = New Generator(mesh)
        End If

        If mzpack Then
            Return sim _
                .GetExpressionMatrix _
                .toMzPack(
                    spatial:=spatial,
                    mesh:=mesh,
                    q:=q
                )
        Else
            Return sim.GetExpressionMatrix
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
                             Optional mesh As MeshArguments = Nothing,
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
        Dim current As ScanMS1
        Dim d As Integer = scans.expression.Length / 25
        Dim p As i32 = 0

        If Not mesh Is Nothing AndAlso (mesh.sample_groups.Length > 1 OrElse Not mesh.cals.IsNullOrEmpty) Then
            Dim infoSource = mesh.sampleinfo.JoinIterates(mesh.cals).ToArray
            Dim sampleinfo As Dictionary(Of String, (SampleInfo(), DataFrameRow())) = infoSource _
                .Zip(scans.expression()) _
                .GroupBy(Function(si) si.First.ID) _
                .ToDictionary(Function(si) si.Key,
                              Function(s)
                                  Dim pie = s.Select(Function(si) si.First).ToArray
                                  Dim conv = s.Select(Function(si) si.Second).ToArray

                                  Return (pie, conv)
                              End Function)

            mz = mz.Shuffles.AsVector

            ' processing mutliple layer sample data
            For Each sample In sampleinfo ' .OrderByDescending(Function(si) si.Value.Item2.Length)
                t += dt
                current = MsData.PopulateMs1Scan(sample.Key, t, q, mz, sample.Value.Item1, sample.Value.Item2)

                If Not current Is Nothing Then
                    Call scan1.Add(current)
                End If
                If (++p Mod d) = 0 Then
                    If scan1.Count > 0 Then
                        Call VBDebugger.EchoLine($"[{p}/{scans.expression.Length}] {(p / scans.expression.Length * 100).ToString("F2")}% ... {scan1.Last.scan_id}")
                    End If
                End If
            Next
        Else
            Dim empty As New Dictionary(Of String, SampleInfo)

            If Not mesh Is Nothing Then
                empty = mesh.sampleinfo.ToDictionary(Function(i) i.ID)
            End If

            ' processing simple sample data
            For Each sample As DataFrameRow In scans.expression
                t += dt
                current = sample.PopulateMs1Scan(t, q, spatial, mz, sampleinfo:=empty)

                If Not current Is Nothing Then
                    Call scan1.Add(current)
                End If
                If (++p Mod d) = 0 Then
                    If scan1.Count > 0 Then
                        Call VBDebugger.EchoLine($"[{p}/{scans.expression.Length}] {(p / scans.expression.Length * 100).ToString("F2")}% ... {scan1.Last.scan_id}")
                    End If
                End If
            Next
        End If

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
