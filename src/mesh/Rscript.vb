
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mesh")>
Public Module Rscript

    <ExportAPI("expr1")>
    <RApiReturn(GetType(Matrix))>
    Public Function expr0(mesh As MeshArguments) As Object
        Return New Generator(mesh).GetExpressionMatrix
    End Function
End Module
