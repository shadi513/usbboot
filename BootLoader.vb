Public Class BootLoader

    Dim VID As UShort
    Dim PID As UShort
    Dim bufOutSize As Short
    Dim bufInSize As Short
    Dim BufferIn() As Byte, BufferOut() As Byte

    Public Enum LoaderCommand
        READ_PAGE = 1
        WRITE_PAGE = 2
        RESET_DEVICE = 3
        GET_VER = 4
    End Enum

    Public Enum Out_State
        STATE_SUCCESS = 1
        STATE_FAILED = 2
    End Enum

    Public Sub New(ByVal destForm As Form, ByVal VendorID As UShort, ByVal ProductID As UShort, ByVal BufferOutSize As Short, ByVal BufferInSize As Short)
        VID = VendorID
        PID = ProductID
        ReDim BufferIn(BufferInSize)
        ReDim BufferOut(BufferOutSize)
        BufferOut(0) = 0
        ConnectToHID(destForm)
    End Sub

    Protected Overrides Sub Finalize()
        DisconnectFromHID()
        MyBase.Finalize()
    End Sub

    Public Function readPage(ByVal pageAddress As UShort, ByRef ReadBuffer() As Byte) As Boolean
        Dim j As Byte, res As Boolean = False
        BufferOut(1) = LoaderCommand.READ_PAGE
        BufferOut(2) = pageAddress Mod 256
        BufferOut(3) = pageAddress \ 256
        hidWriteEx(VID, PID, BufferOut(0))
        res = hidReadEx(VID, PID, BufferIn(0))
        For j = 0 To 63
            ReadBuffer(j) = BufferIn(j + 1)
        Next j
        Return res
    End Function

    Public Function WritePage(ByVal pageAddress As UShort, ByVal pageData() As Byte) As Boolean
        Dim j As Byte, res As Boolean = False
        BufferOut(1) = LoaderCommand.WRITE_PAGE
        BufferOut(2) = pageAddress Mod 256
        BufferOut(3) = pageAddress \ 256
        hidWriteEx(VID, PID, BufferOut(0))
        For j = 0 To 63
            BufferOut(j + 1) = pageData(j)
        Next j
        hidWriteEx(VID, PID, BufferOut(0))
        res = hidReadEx(VID, PID, BufferIn(0))
        Return res
    End Function

    Public Sub ResetDevice()
        BufferOut(1) = LoaderCommand.RESET_DEVICE
        hidWriteEx(VID, PID, BufferOut(0))
    End Sub

    Public Function getVersion() As String
        Dim buf(63) As Byte, i As Byte
        BufferOut(1) = LoaderCommand.GET_VER
        hidWriteEx(VID, PID, BufferOut(0))
        hidReadEx(VID, PID, BufferIn(0))
        For i = 0 To 63
            buf(i) = BufferIn(i + 1)
        Next
        Return System.Text.Encoding.ASCII.GetString(buf)
    End Function

End Class
