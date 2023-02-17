﻿Public Class MeshArguments

    ''' <summary>
    ''' m/z mass range to generates the ions
    ''' [ion features]
    ''' </summary>
    ''' <returns></returns>
    Public Property massRange As Double() = {50, 1200}
    ''' <summary>
    ''' A vector of the sample labels to generates the matrix
    ''' [sample observes]
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleLabels As String()
    ''' <summary>
    ''' Ion numbers in the generated expression matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property featureSize As Integer = 10000

End Class