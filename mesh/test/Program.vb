Imports System.Drawing
Imports mesh
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module Program

    Sub Main()
        Dim data As MeasureData() = {
            New MeasureData(1, 213, 10),
            New MeasureData(10, 19, 100),
            New MeasureData(122, 661, 1000),
            New MeasureData(441, 371, 20),
            New MeasureData(751, 81, 56),
            New MeasureData(201, 691, 1),
            New MeasureData(2231, 101, 888),
            New MeasureData(31, 1561, 230),
            New MeasureData(1555, 1111, 10000),
            New MeasureData(91, 132, 770)
        }

        Dim meshData As Surface() = Modeller.CreateMesh3D(data).ToArray

        g.SetDriver(Drivers.SVG)

        Dim view As New Camera() With {.screen = New Size(1400, 1200)}

        Call Modeller.Draw(meshData, view).Save("./test.svg")

        Pause()
    End Sub
End Module
