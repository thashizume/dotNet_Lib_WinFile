Public Class FileList

    Private _directoryName As String = String.Empty
    Private _dt As System.Data.DataTable
    Private _progress_directory As String
    Private _progress_fileName As String
    Private _progress_count As Long
    Private _progress_max As Long

    Public Sub New()
        _dt = _createDataTable()
    End Sub

    Public ReadOnly Property ProgressDirectory As String
        Get
            Return Me._progress_directory
        End Get
    End Property

    Public ReadOnly Property ProgressFilename As String
        Get
            Return Me._progress_fileName
        End Get
    End Property

    Public ReadOnly Property ProgressCount As Long
        Get
            Return Me._progress_count
        End Get
    End Property

    Public ReadOnly Property ProgressMax As Long
        Get
            Return Me._progress_max
        End Get
    End Property

    Public Property DirectoryName As String
        Get
            Return Me._directoryName
        End Get
        Set(value As String)
            If (New System.IO.DirectoryInfo(value)).Exists Then
                Me._directoryName = value
            Else
                Throw New Exception("Directory Not Found [" & value & "]")
            End If
        End Set
    End Property

    Public Function Analyse(_dir As String, Optional _searchPattern As String = "*") _
        As System.Data.DataTable

        _dt = Me._createDataTable
        _getFiles(_dir, _searchPattern)

        Return _dt
    End Function

    Private Function __getFiles(_dir As String, _searchPattern As String) As String()
        Dim result As String() = Nothing
        Try
            result = System.IO.Directory.GetFiles(_dir, _searchPattern)
        Catch ex As Exception

        End Try
        Return result
    End Function


    Private Sub _getFiles(_dir As String, _searchPattern As String)

        Dim _files As String() = __getFiles(_dir, _searchPattern)
        Dim _subDir As String()

        If _files Is Nothing Then

        Else

            For Each _fName As String In _files
                Dim _dr As System.Data.DataRow = _dt.NewRow
                Dim _f As System.IO.FileInfo = New System.IO.FileInfo(_fName)
                _dr(0) = _f.Name
                _dr(1) = _f.DirectoryName
                _dr(2) = _f.FullName

                If _f.Extension.Length <= 0 Then
                    _dr(3) = "#n/a"

                Else
                    _dr(3) = _f.Extension.Replace(".", "").ToUpper

                End If

                _dr(4) = _f.CreationTime
                _dr(5) = _f.LastWriteTime
                _dr(6) = _f.LastAccessTime
                _dr(7) = _f.Length
                _dr(8) = ENUM_FILE_SELECT_VALUE.NOT_SELECT

                _dt.Rows.Add(_dr)
            Next

            _subDir = System.IO.Directory.GetDirectories(_dir)
            For Each _dName As String In _subDir
                Me._getFiles(_dName, _searchPattern)
            Next

        End If


    End Sub

    Public Function Move(path As String, dt As System.Data.DataTable, Optional mode As ENUM_COPY_MODE = ENUM_COPY_MODE.NOMAL)

        Return Nothing
    End Function

    Public Function Copy(path As String, dt As System.Data.DataTable, Optional mode As ENUM_COPY_MODE = ENUM_COPY_MODE.NOMAL)

        Return Nothing
    End Function

    Public Function Delete(dt As System.Data.DataTable)


        Return Nothing
    End Function

    Private Function _createDataTable()
        Dim dt As New System.Data.DataTable("FILE_LIST")

        dt.Columns.Add("FILE_NAME", GetType(String))
        dt.Columns.Add("DIRECTORY_NAME", GetType(String))
        dt.Columns.Add("FULL_NAME", GetType(String))
        dt.Columns.Add("EXT_NAME", GetType(String))
        dt.Columns.Add("CRATE_DATE", GetType(Date))
        dt.Columns.Add("MODIFY_DATE", GetType(Date))
        dt.Columns.Add("ACCESS_DATE", GetType(Date))
        dt.Columns.Add("SIZE", GetType(Long))
        dt.Columns.Add("IS_SELECT", GetType(Long))

        Return dt
    End Function

End Class

Public Enum ENUM_FILE_SELECT_VALUE As Long
    NOT_SELECT = 0
    COPY = 1
    MOVE = 2
    DELETE = 3

End Enum

Public Enum ENUM_COPY_MODE As Long
    NOMAL = 0
    FORCE = 1

End Enum