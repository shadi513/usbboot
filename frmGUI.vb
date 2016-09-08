Public Class frmGUI

    Dim i As Byte, bootLoader As New BootLoader(Me, &HFFFF, &H1, 64, 64)
    Dim dataBuffer(65535) As Byte

    Public Sub OnPlugged(ByVal pHandle As Integer)
        If hidGetVendorID(pHandle) = &HFFFF And hidGetProductID(pHandle) = &H1 Then
            Label4.Text = bootLoader.getVersion
            Label4.ForeColor = Color.Green
            If ofd.FileName <> "" Then btnWriteAll.Enabled = True
        End If
    End Sub

    Public Sub OnUnplugged(ByVal pHandle As Integer)
        If hidGetVendorID(pHandle) = &HFFFF And hidGetProductID(pHandle) = &H1 Then
            hidSetReadNotify(hidGetHandle(&HFFFF, &H1), False)
            Label4.Text = "Not Connected"
            Label4.ForeColor = Color.Red
            btnWriteAll.Enabled = False
        End If
    End Sub

    Private Sub btnLoadHex_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadHex.Click
        ofd.Filter = "HEX Files (*.hex)|*.hex"
        ofd.FilterIndex = 0
        If ofd.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Label5.Text = IO.Path.GetFileName(ofd.FileName) & " Loaded"
            btnWriteAll.Enabled = True
        End If
    End Sub

    Private Sub clearBuffer(ByVal buffer() As Byte)
        Dim i As Long
        For i = LBound(buffer) To UBound(buffer)
            buffer(i) = 255
        Next
    End Sub

    Private Sub btnWriteAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWriteAll.Click
        Dim i As Integer, j As Byte, dataBuf(63) As Byte
        Label5.Text = "Reloading File"
        clearBuffer(dataBuffer)
        IntelHEX.LoadHEXFile(ofd.FileName, dataBuffer)
        dataBuffer(4) = dataBuffer(0)
        dataBuffer(5) = dataBuffer(1)
        dataBuffer(6) = dataBuffer(2)
        dataBuffer(7) = dataBuffer(3)
        dataBuffer(0) = &H33
        dataBuffer(1) = &HEF
        dataBuffer(2) = &H3B
        dataBuffer(3) = &HF0
        For i = 0 To 383
            For j = 0 To 63
                dataBuf(j) = dataBuffer((i * 64) + j)
            Next
            bootLoader.WritePage(i, dataBuf)
            pb.Value = i
            Label5.Text = "Writing " & Int((i / 383) * 100).ToString & "%"
            Application.DoEvents()
        Next i
        pb.Value = 0
        Label5.Text = "Resetting Device"
        Application.DoEvents()
        bootLoader.ResetDevice()
        Label5.Text = IO.Path.GetFileName(ofd.FileName) & " Loaded"
        Application.DoEvents()
    End Sub

    Private Sub showData()
        Dim i As Long, s As String = ""
        For i = 0 To UBound(dataBuffer) Step 2
            s &= (dataBuffer(i) + (dataBuffer(i + 1) * 256)).ToString("X4") & " "
        Next
        TextBox1.Text = s
    End Sub

    Private Sub frmGUI_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
        showData()
    End Sub

End Class


