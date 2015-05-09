Public Class FileInfo

    Private _fileName As String = String.Empty
    Private _f As System.IO.FileInfo

    Public Sub New()

    End Sub

    Public Sub New(fileName As String)
        Me.FileName = fileName
    End Sub


    Public Property FileName As String
        Get
            Return Me._fileName
        End Get
        Set(value As String)
            If (New System.IO.FileInfo(value)).Exists Then
                Me._fileName = value
                _f = New System.IO.FileInfo(Me.FileName)
            Else

            End If

        End Set
    End Property

    Public ReadOnly Property DirectoryName As String
        Get
            Return _f.DirectoryName
        End Get
    End Property

    Public ReadOnly Property CreateDate As Date
        Get
            Return _f.CreationTime
        End Get
    End Property

    Public ReadOnly Property AccessDate As Date
        Get
            Return _f.LastAccessTime
        End Get
    End Property

    Public ReadOnly Property ModifyDate As Date
        Get
            Return _f.LastWriteTime
        End Get
    End Property

End Class
