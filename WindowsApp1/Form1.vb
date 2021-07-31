Imports Microsoft.VisualBasic.Imaging.Drawing3D.Landscape

Public Class Form1

    Dim canvas As New Canvas

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        ElementHost1.Child = canvas

        Call canvas.BuildSolid(Vendor_3mf.IO.Open("E:\GCModeller\src\runtime\sciBASIC#\gr\build_3DEngine\Enviroment_GroupTrees.3mf"))
    End Sub
End Class
