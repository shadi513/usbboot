Imports System.IO

Public Class IntelHEX
    Public Structure hexData
        Dim dataBytes() As Byte
        Dim Address As UInteger
        Dim dataLength As Byte
        Dim recordType As Byte
        Dim CRC As Byte
    End Structure

    Public Shared Sub LoadHEXFile(ByVal fileName As String, ByRef dataBuffer() As Byte)
        Dim lines() As String, i As UInteger, j As Byte, s As String = "", bts As New hexData, ret As Byte
        lines = File.ReadAllLines(fileName)
        For i = 0 To lines.Length - 1
            ret = getHEXLineData(lines(i), bts)
            If bts.recordType = 0 Then
                For j = 0 To bts.dataLength - 1
                    dataBuffer(bts.Address + j) = bts.dataBytes(j)
                Next j
            Else
                Exit For
            End If
        Next i
    End Sub

    Public Shared Function getHEXLineData(ByVal hexLine As String, ByRef retData As hexData) As Boolean
        Dim ptr As Byte = 2, i, tmpByte As Byte, tmpStr As String, crc As UInteger
        If Mid(hexLine, 1, 1) <> ":" Then Return False 'Not HEX Line
        tmpStr = Mid(hexLine, ptr, 2)
        tmpByte = CByte(Val("&H" & tmpStr))
        retData.dataLength = tmpByte
        crc = tmpByte
        ReDim retData.dataBytes(retData.dataLength - 1)
        ptr += 2

        tmpStr = Mid(hexLine, ptr, 2)
        tmpByte = CByte(Val("&H" & tmpStr))
        retData.Address = tmpByte * 256
        crc += tmpByte
        ptr += 2

        tmpStr = Mid(hexLine, ptr, 2)
        tmpByte = CByte(Val("&H" & tmpStr))
        retData.Address += tmpByte
        crc = crc + tmpByte
        ptr += 2

        tmpStr = Mid(hexLine, ptr, 2)
        tmpByte = CByte(Val("&H" & tmpStr))
        retData.recordType = tmpByte
        crc += tmpByte
        ptr += 2

        Select Case tmpByte 'Record Type
            Case 0 'Data
                For i = 0 To retData.dataLength - 1
                    tmpStr = Mid(hexLine, ptr, 2)
                    tmpByte = CByte(Val("&H" & tmpStr))
                    retData.dataBytes(i) = tmpByte
                    crc = crc + tmpByte
                    ptr += 2
                Next i
            Case 1 'End Of File
            Case 2 'Extended Segment Address
            Case 3 'Start Segment Address
            Case 4 'Extended Linear Address
            Case 5 'Start Linear Address
        End Select

        tmpStr = Mid(hexLine, ptr, 2)
        tmpByte = CByte(Val("&H" & tmpStr))
        retData.CRC = tmpByte

        If crc Mod 256 = ((Not tmpByte) + 1) Then
            Return True
        Else
            Return False
        End If

    End Function



End Class
