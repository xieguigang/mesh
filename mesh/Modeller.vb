Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Public NotInheritable Class Modeller

    Private Sub New()
    End Sub

    Public Shared Iterator Function CreateMesh3D(sample As IEnumerable(Of MeasureData)) As IEnumerable(Of Surface)
        Dim layers As GeneralPath() = ContourLayer.GetContours(sample).ToArray
        Dim base As GeneralPath = layers(Scan0)

        For Each higher As GeneralPath In layers.Skip(1)
            Dim basePoints As PointF() = base.GetPolygons.IteratesALL.ToArray
            Dim higherPoints As PointF() = higher.GetPolygons.IteratesALL.ToArray

            ' each point in higher layer query two nearest points
            ' in base layer for create a triangle mesh
            For Each polygon As PointF() In higher.GetPolygons
                For Each point As PointF In polygon
                    Dim twoPoints = basePoints.OrderBy(Function(d) d.Distance(point)).Take(2).ToArray

                    Yield New Surface With {
                        .vertices = {
                            New Point3D(point.X, point.Y, higher.level),
                            New Point3D(twoPoints(0).X, twoPoints(0).Y, base.level),
                            New Point3D(twoPoints(1).X, twoPoints(1).Y, base.level)
                        }
                    }
                Next
            Next

            For Each polygon As PointF() In base.GetPolygons
                For Each point As PointF In polygon
                    Dim twoPoints = higherPoints.OrderBy(Function(d) d.Distance(point)).Take(2).ToArray

                    Yield New Surface With {
                        .vertices = {
                            New Point3D(point.X, point.Y, base.level),
                            New Point3D(twoPoints(0).X, twoPoints(0).Y, higher.level),
                            New Point3D(twoPoints(1).X, twoPoints(1).Y, higher.level)
                        }
                    }
                Next
            Next

            base = higher
        Next
    End Function

End Class
