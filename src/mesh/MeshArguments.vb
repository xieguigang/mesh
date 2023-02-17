Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class MeshArguments

    ''' <summary>
    ''' m/z mass range to generates the ions
    ''' [ion features]
    ''' </summary>
    ''' <returns></returns>
    Public Property massrange As Double() = {50, 1200}
    Public Property massdiff As Double = 0.005
    ''' <summary>
    ''' A vector of the sample labels to generates the matrix
    ''' [sample observes]
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleinfo As SampleInfo()
    ''' <summary>
    ''' Ion numbers in the generated expression matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property featureSize As Integer = 10000

    ''' <summary>
    ''' the metabolites that appears in the ions feature, 
    ''' this property value can be nothing
    ''' </summary>
    ''' <returns></returns>
    Public Property metabolites As MetaboliteAnnotation()
    ''' <summary>
    ''' adducts for evaluate ion m/z features from the <see cref="MetaboliteAnnotation.ExactMass"/> 
    ''' of the metabolites
    ''' </summary>
    ''' <returns></returns>
    Public Property adducts As MzCalculator()

    Public Function CreateMatrix() As Matrix
        Return New Generator(Me).GetExpressionMatrix
    End Function

End Class
