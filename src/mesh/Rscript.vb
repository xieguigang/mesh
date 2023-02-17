
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' MSdata expression matrix simulator for test metabolomics analysis pipeline
''' </summary>
<Package("mesh")>
<RTypeExport("mesh", GetType(MeshArguments))>
Public Module Rscript

    <ExportAPI("mesh")>
    Public Function meshArgs(<RRawVectorArgument>
                             Optional mass_range As Object = "50,1200",
                             Optional feature_size As Integer = 10000,
                             Optional env As Environment = Nothing) As MeshArguments

        Dim range As Double() = CLRVector.asNumeric(mass_range)

        Return New MeshArguments With {
            .massrange = range,
            .featureSize = feature_size
        }
    End Function

    <ExportAPI("samples")>
    Public Function setSamples(mesh As MeshArguments,
                               <RRawVectorArgument>
                               sampleinfo As Object,
                               Optional env As Environment = Nothing) As Object

        Dim samples As SampleInfo() = REnv.asVector(Of SampleInfo)(sampleinfo)
        mesh.sampleinfo = samples
        Return mesh
    End Function

    <ExportAPI("metabolites")>
    Public Function setMetabolites(mesh As MeshArguments,
                                   metabolites As MetaboliteAnnotation(),
                                   <RRawVectorArgument>
                                   adducts As Object,
                                   Optional env As Environment = Nothing) As Object
        mesh.metabolites = metabolites
        mesh.adducts = Math.GetPrecursorTypes(adducts, env)
        Return mesh
    End Function

    <ExportAPI("expr1")>
    <RApiReturn(GetType(Matrix))>
    Public Function expr0(mesh As MeshArguments) As Object
        Return New Generator(mesh).GetExpressionMatrix
    End Function
End Module
