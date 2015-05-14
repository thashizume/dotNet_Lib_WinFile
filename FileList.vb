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

    Public ReadOnly Property CountOfFiles As Long
        Get
            Return _dt.Rows.Count
        End Get
    End Property

    Public ReadOnly Property CountOfDirectory As Long
        Get
            Return Me.getDirectories.Rows.Count
        End Get
    End Property

    Public ReadOnly Property CountOfExtentType As Long
        Get
            Return Me.getExtents.Rows.Count
        End Get
    End Property

    Public ReadOnly Property CountOfSelectedFile As Long
        Get
            Dim dv1 As System.Data.DataView = New System.Data.DataView(Me.getFiles, "SELECTED = 1", String.Empty, DataViewRowState.CurrentRows)
            If dv1.Count > 0 Then Return CType(dv1.ToTable().Compute("count(size)", String.Empty), Long)
            Return 0
        End Get
    End Property

    Public ReadOnly Property CountOfSelectedDirectory As Long
        Get
            Dim dv1 As System.Data.DataView = New System.Data.DataView(Me.getDirectories, "SELECTED = 1", String.Empty, DataViewRowState.CurrentRows)
            If dv1.Count > 0 Then Return CType(dv1.ToTable().Compute("count(size)", String.Empty), Long)
            Return 0
        End Get
    End Property

    Public ReadOnly Property CountOfSelectedExtentType As Long
        Get
            Dim dv1 As System.Data.DataView = New System.Data.DataView(Me.getExtents, "SELECTED = 1", String.Empty, DataViewRowState.CurrentRows)
            If dv1.Count > 0 Then Return CType(dv1.ToTable().Compute("count(size)", String.Empty), Long)
            Return 0
        End Get
    End Property

    Public ReadOnly Property SumOfSize As Long
        Get
            Dim dv1 As System.Data.DataView = New System.Data.DataView(Me.getFiles, String.Empty, String.Empty, DataViewRowState.CurrentRows)
            If dv1.Count > 0 Then Return CType(dv1.ToTable().Compute("Sum(size)", String.Empty), Long) / (1024 * 1024)
            Return 0
        End Get
    End Property


    Public ReadOnly Property SumOfSelectedSize As Long
        Get
            Dim dv1 As System.Data.DataView = New System.Data.DataView(Me.getFiles, "SELECTED = 1", String.Empty, DataViewRowState.CurrentRows)
            If dv1.Count > 0 Then Return CType(dv1.ToTable().Compute("Sum(size)", String.Empty), Long) / (1024 * 1024)
            Return 0
        End Get
    End Property

    Public Function getFiles() As System.Data.DataTable
        If _dt Is Nothing Then Return Nothing
        Dim result As System.Data.DataTable = _dt.DefaultView.ToTable("FILE_LIST", True, "DIRECTORY_NAME,FILE_NAME,EXT_NAME,SIZE,CREATE_DATE,MODIFY_DATE,SELECTED".Split(","))
        Return result

    End Function

    Public Function getExtents() As System.Data.DataTable

        If _dt Is Nothing Then Return Nothing
        Dim result As System.Data.DataTable = _dt.DefaultView.ToTable("EXT_NAME", True, "EXT_NAME")
        result.Columns.Add("SIZE", GetType(Long))
        result.Columns.Add("COUNT", GetType(Long))
        result.Columns.Add("SELECTED", GetType(Long))

        For Each _row As System.Data.DataRow In result.Rows
            _row(1) = (_dt.Compute("Sum(size)", "EXT_NAME = '" & _row(0) & "'")) / (1024 * 1024)
            _row(1) = (_dt.Compute("Sum(size)", "EXT_NAME = '" & _row(0) & "'"))
            _row(2) = _dt.Compute("count(EXT_NAME)", "EXT_NAME = '" & _row(0) & "'")
            _row(3) = 0
        Next
        Return result
    End Function

    Function getDirectories() As System.Data.DataTable

        If _dt Is Nothing Then Return Nothing
        Dim result As System.Data.DataTable = _dt.DefaultView.ToTable("DIRECTORY_NAME", True, "DIRECTORY_NAME")
        result.Columns.Add("SIZE", GetType(Long))
        result.Columns.Add("COUNT", GetType(Long))
        result.Columns.Add("SELECTED", GetType(Long))
        For Each _row As System.Data.DataRow In result.Rows
            Dim _s As String = _row(0)
            _s = _s.Replace("'", "''")
            _row(1) = (_dt.Compute("Sum(size)", "DIRECTORY_NAME = '" & _s & "'")) / (1024 * 1024)
            _row(1) = (_dt.Compute("Sum(size)", "DIRECTORY_NAME = '" & _s & "'"))
            _row(2) = _dt.Compute("count(DIRECTORY_NAME)", "DIRECTORY_NAME = '" & _s & "'")
            _row(3) = 0
        Next
        Return result

    End Function

    Public Property DirectoryName As String
        Get
            Return Me._directoryName
        End Get
        Set(value As String)
            If (New System.IO.DirectoryInfo(value)).Exists Then
                Me._directoryName = (New System.IO.DirectoryInfo(value)).FullName
            Else
                Throw New Exception("Directory Not Found [" & value & "]")
            End If
        End Set
    End Property

    Public Function Analyse(_dir As String, Optional _searchPattern As String = "*") _
        As System.Data.DataTable

        _dt = Me._createDataTable
        Me.DirectoryName = _dir
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

    Private Function __chkFile(_fName As String) As Boolean
        Dim result As Boolean
        Dim _f As System.IO.FileInfo
        Try
            _f = New System.IO.FileInfo(_fName)
            result = True
        Catch ex As Exception
            result = False
        Finally
            _f = Nothing
        End Try

        Return result
    End Function


    Private Sub _getFiles(_dir As String, _searchPattern As String)
        Dim _files As String() = __getFiles(_dir, _searchPattern)
        Dim _subDir As String()
        If _files Is Nothing Then
        Else
            For Each _fName As String In _files
                If __chkFile(_fName) = True Then
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
                    _dr(9) = "." & _f.FullName.Replace(Me.DirectoryName, String.Empty)
                    _dt.Rows.Add(_dr)
                End If

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


    Public Function SeleteFile(_files() As String) As System.Data.DataTable

        If _dt Is Nothing Then Return Nothing


        For Each _row As System.Data.DataRow In _dt.Rows
            For Each _f As String In _files
                If _row("FILE_NAME") = _f Then
                    '_row.BeginEdit()
                    _row("SELECTED") = 1
                    '_row.AcceptChanges()
                End If
            Next

        Next

        _dt.AcceptChanges()
        Return Me.getFiles

    End Function

    Private Function _createDataTable()

        Dim dt As New System.Data.DataTable("FILE_LIST")
        dt.Columns.Add("FILE_NAME", GetType(String))
        dt.Columns.Add("DIRECTORY_NAME", GetType(String))
        dt.Columns.Add("FULL_NAME", GetType(String))
        dt.Columns.Add("EXT_NAME", GetType(String))
        dt.Columns.Add("CREATE_DATE", GetType(Date))
        dt.Columns.Add("MODIFY_DATE", GetType(Date))
        dt.Columns.Add("ACCESS_DATE", GetType(Date))
        dt.Columns.Add("SIZE", GetType(Long))
        dt.Columns.Add("SELECTED", GetType(Long))
        dt.Columns.Add("PATH_NAME", GetType(String))

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