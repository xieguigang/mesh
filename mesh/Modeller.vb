Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Drawing3D

Public NotInheritable Class Modeller

    Private Sub New()
    End Sub

    Public Shared Function CreateMesh3D(sample As IEnumerable(Of MeasureData)) As Surface
        Dim layers As GeneralPath() = ContourLayer.GetContours(sample).ToArray


    End Function

End Class
