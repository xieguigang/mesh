Imports mesh
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Landscape

Public Class Form1

    Dim canvas As New Canvas

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        ElementHost1.Child = canvas

        Call canvas.BuildSolid(Vendor_3mf.IO.Open("E:\GCModeller\src\runtime\sciBASIC#\gr\build_3DEngine\Enviroment_GroupTrees.3mf"))

        'Dim data As MeasureData() = {
        '    New MeasureData(1, 213, 10),
        '    New MeasureData(10, 19, 100),
        '    New MeasureData(122, 661, 1000),
        '    New MeasureData(441, 371, 20),
        '    New MeasureData(751, 81, 56),
        '    New MeasureData(201, 691, 1),
        '    New MeasureData(2231, 101, 888),
        '    New MeasureData(31, 1561, 230),
        '    New MeasureData(1555, 1111, 10000),
        '    New MeasureData(91, 132, 770)
        '}

        'Dim meshData As Surface() = Modeller.CreateMesh3D(data).ToArray

        'Call canvas.BuildMesh(meshData)
    End Sub
End Class
