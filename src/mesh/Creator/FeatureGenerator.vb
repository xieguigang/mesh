Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports stdNum = System.Math

''' <summary>
''' generates the metabolite ions feature m/z set
''' </summary>
Public Class FeatureGenerator : Implements Enumeration(Of Double)

    ReadOnly args As MeshArguments
    ReadOnly ions As List(Of Double)
    ReadOnly mass_range As DoubleRange

    Sub New(args As MeshArguments)
        Me.ions = New List(Of Double)
        Me.mass_range = New DoubleRange(args.mass_range)
        Me.args = args
    End Sub

    Public Function Clear() As FeatureGenerator
        Call ions.Clear()
        Return Me
    End Function

    ''' <summary>
    ''' get ion feature m/z value of a specific metabolite
    ''' </summary>
    ''' <param name="meta"></param>
    ''' <returns></returns>
    Private Function getIon(meta As MetaboliteAnnotation) As Double
        Dim ion As Double

        ' the order of the elements in the adducts list
        ' will affects the generated ion feature set
        For Each adduct As MzCalculator In args.adducts
            ion = adduct.CalcMZ(meta.ExactMass)

            If Not mass_range.IsInside(ion) Then
                Continue For
            End If

            If MissingIon(ion) Then
                ions.Add(ion)
                Return ion
            End If
        Next

        Return -1
    End Function

    ''' <summary>
    ''' Create ion features
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function CreateIonFeatures() As IEnumerable(Of Double)
        Dim features As Integer = args.featureSize
        Dim ion As Double

        For Each meta As MetaboliteAnnotation In args.metabolites.SafeQuery
            ion = getIon(meta)

            If mass_range.IsInside(ion) Then
                features -= 1
                Yield ion
            End If
        Next

        Do While features > 0
            ion = getIon()
            features -= 1

            Yield ion
        Loop
    End Function

    ''' <summary>
    ''' check mz is duplicated or not
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' TRUE - means the given ion <paramref name="mz"/> is not exists in the current feature set
    ''' FALSE - else means the given ion <paramref name="mz"/> is already exists in current feature set
    ''' </returns>
    Private Function MissingIon(mz As Double) As Boolean
        Return Not ions _
            .Any(Function(mzi)
                     Return stdNum.Abs(mzi - mz) <= args.massdiff
                 End Function)
    End Function

    ''' <summary>
    ''' Generate a new ion m/z value
    ''' </summary>
    ''' <returns></returns>
    Private Function getIon() As Double
        Do While True
            Dim mz As Double = randf.GetRandomValue(mass_range)
            ' check mz is duplicated or not
            Dim check As Boolean = MissingIon(mz)

            ' check = true means current mz is new ion
            ' then we populate current new ion mz
            If check Then
                ions.Add(mz)
                Return mz
            End If
        Loop

        Throw New InvalidDataException("this error will never happends!")
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Double) Implements Enumeration(Of Double).GenericEnumerator
        For Each ion As Double In ions
            Yield ion
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Double).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
