Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class MeshArguments

    ''' <summary>
    ''' m/z mass range to generates the ions
    ''' [ion features]
    ''' </summary>
    ''' <returns></returns>
    Public Property mass_range As Double() = {50, 1200}
    Public Property massdiff As Double = 0.005
    Public Property linear_kernel As Boolean = False

    ''' <summary>
    ''' the intensity range conversion
    ''' </summary>
    ''' <returns></returns>
    Public Property intensity_max As Double = 10 ^ 15

    ''' <summary>
    ''' is spatial data?
    ''' </summary>
    ''' <returns></returns>
    Public Property spatial As Boolean = False

    ''' <summary>
    ''' A vector of the sample labels to generates the matrix
    ''' [sample observes]
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleinfo As SampleInfo()
    ''' <summary>
    ''' the expression level kernel, the value is corresponding with the <see cref="sampleinfo"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property kernel As Double()
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
    ''' <remarks>
    ''' the order of the adducts will affects the generated ion feature set
    ''' </remarks>
    Public Property adducts As MzCalculator()
    Public Property opts As SearchOption

    Public Function CreateMatrix() As Matrix
        Return New Generator(Me).GetExpressionMatrix
    End Function

End Class
