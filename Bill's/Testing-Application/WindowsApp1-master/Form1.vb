Public Class Form1
    Dim StrSerialIn As String
    Dim Home1 As String = "$H"
    Dim A10 As String = "X-7Y0"
    Dim B10 As String = "x-6y0"
    Dim C10 As String = "x-5y0"
    Dim D10 As String = "x-4y0"
    Dim E10 As String = "x-3y0"
    Dim F10 As String = "x-2y0"
    Dim G10 As String = "x-1y0"
    Dim H10 As String = "x-0y0"

    Dim A20 As String = "x-7y1"
    Dim B20 As String = "x-6y1"
    Dim C20 As String = "x-5y1"
    Dim D20 As String = "x-4y1"
    Dim E20 As String = "x-3y1"
    Dim F20 As String = "x-2y1"
    Dim G20 As String = "x-1y1"
    Dim H20 As String = "x-0y1"

    Dim A30 As String = "x-7y2"
    Dim B30 As String = "x-6y2"
    Dim C30 As String = "x-5y2"
    Dim D30 As String = "x-4y2"
    Dim E30 As String = "x-3y2"
    Dim F30 As String = "x-2y2"
    Dim G30 As String = "x-1y2"
    Dim H30 As String = "x-0y2"

    Dim A40 As String = "x-7y3"
    Dim B40 As String = "x-6y3"
    Dim C40 As String = "x-5y3"
    Dim D40 As String = "x-4y3"
    Dim E40 As String = "x-3y3"
    Dim F40 As String = "x-2y3"
    Dim G40 As String = "x-1y3"
    Dim H40 As String = "x-0y3"

    Dim A50 As String = "x-7y4"
    Dim B50 As String = "x-6y4"
    Dim C50 As String = "x-5y4"
    Dim D50 As String = "x-4y4"
    Dim E50 As String = "x-3y4"
    Dim F50 As String = "x-2y4"
    Dim G50 As String = "x-1y4"
    Dim H50 As String = "x-0y4"

    Dim A60 As String = "x-7y5"
    Dim B60 As String = "x-6y5"
    Dim C60 As String = "x-5y5"
    Dim D60 As String = "x-4y5"
    Dim E60 As String = "x-3y5"
    Dim F60 As String = "x-2y5"
    Dim G60 As String = "x-1y5"
    Dim H60 As String = "x-0y5"

    Dim A70 As String = "x-7y6"
    Dim B70 As String = "x-6y6"
    Dim C70 As String = "x-5y6"
    Dim D70 As String = "x-4y6"
    Dim E70 As String = "x-3y6"
    Dim F70 As String = "x-2y6"
    Dim G70 As String = "x-1y6"
    Dim H70 As String = "x-0y6"

    Dim A80 As String = "x-7y7"
    Dim B80 As String = "x-6y7"
    Dim C80 As String = "x-5y7"
    Dim D80 As String = "x-4y7"
    Dim E80 As String = "x-3y7"
    Dim F80 As String = "x-2y7"
    Dim G80 As String = "x-1y7"
    Dim H80 As String = "x-0y7"



    Private Sub ButtonScanPort_Click(sender As Object, e As EventArgs) Handles ButtonScanPort.Click
        ConnectionPanel.Focus()
        If LabelStatus.Text = "Status : Connected" Then
            MsgBox("Conncetion in progress, please Disconnect to scan the new port.", MsgBoxStyle.Critical, "Warning !!!")
            Return
        End If
        ComboBoxPort.Items.Clear()
        Dim myPort As Array
        Dim i As Integer
        myPort = IO.Ports.SerialPort.GetPortNames()
        ComboBoxPort.Items.AddRange(myPort)
        i = ComboBoxPort.Items.Count
        i = i - i
        Try
            ComboBoxPort.SelectedIndex = i
            ButtonConnect.Enabled = True
        Catch ex As Exception
            MsgBox("Com port not detected", MsgBoxStyle.Critical, "Warning !!!")
            ComboBoxPort.Text = ""
            ComboBoxPort.Items.Clear()
            Return
        End Try
        ComboBoxPort.DroppedDown = True
    End Sub



    Private Sub ComboBoxPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxPort.SelectedIndexChanged
        ConnectionPanel.Focus()
    End Sub

    Private Sub ComboBoxPort_DropDown(sender As Object, e As EventArgs) Handles ComboBoxPort.DropDown
        ConnectionPanel.Focus()
    End Sub

    Private Sub LinkLabel_LinkClicked_1(sender As Object, e As LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub ComboBoxPort_Click(sender As Object, e As EventArgs) Handles ComboBoxPort.Click
        If LabelStatus.Text = "Status : Connected" Then
            MsgBox("Connection in progress, please Disconnect to change COM.", MsgBoxStyle.Critical, "Warning !!!")
            Return
        End If
    End Sub


    Private Sub ComboBoxBaudRate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxBaudRate.SelectedIndexChanged
        ConnectionPanel.Focus()
    End Sub




    Private Sub ComboBoxBaudRate_DropDown(sender As Object, e As EventArgs) Handles ComboBoxBaudRate.DropDown
        ConnectionPanel.Focus()
    End Sub



    Private Sub ComboBoxBaudRate_Click(sender As Object, e As EventArgs) Handles ComboBoxBaudRate.Click
        If LabelStatus.Text = "Status : Connected" Then
            MsgBox("Conncetion in progress, please Disconnect to change Baud Rate.", MsgBoxStyle.Critical, "Warning !!!")
            Return
        End If
    End Sub


    Private Sub ButtonConnect_Click(sender As Object, e As EventArgs) Handles ButtonConnect.Click
        ConnectionPanel.Focus()
        Try
            SerialPort1.BaudRate = ComboBoxBaudRate.SelectedItem
            'SerialPort2.BaudRate = ComboBoxBaudRate2.SelectedItem
            SerialPort1.PortName = ComboBoxPort.SelectedItem
            'SerialPort2.PortName = ComboBoxPort2.SelectedItem
            'SerialPort2.Open()
            SerialPort1.Open()
            Timer1.Start()

            LabelStatus.Text = "Status : Connected"
            ButtonConnect.SendToBack()
            ButtonDisconnect.BringToFront()
            PictureBoxConnectionStatus.BackColor = Color.Green
        Catch ex As Exception
            MsgBox("Please check the Hardware, COM, Baud Rate and try again.", MsgBoxStyle.Critical, "Connection failed !!!")
        End Try
    End Sub
    Private Sub ButtonDisconnect_Click(sender As Object, e As EventArgs) Handles ButtonDisconnect.Click
        ConnectionPanel.Focus()
        Timer1.Stop()
        SerialPort1.Close()
        'SerialPort2.Close()
        ButtonDisconnect.SendToBack()
        ButtonConnect.BringToFront()
        LabelStatus.Text = "Status : Disconnect"
        PictureBoxConnectionStatus.Visible = True
        PictureBoxConnectionStatus.BackColor = Color.Red
    End Sub
    Private Sub ButtomHome_Click(sender As Object, e As EventArgs) Handles Home.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine("$H")
        'SerialPort2.WriteLine("1")
    End Sub
    Private Sub TimerSerial_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            StrSerialIn = SerialPort1.ReadExisting  '--> Read incoming serial data

            Dim TB As New TextBox
            TB.Multiline = True
            TB.Text = StrSerialIn   '--> Enter serial data into the textbox

            If TB.Lines.Count > 0 Then

            End If

            '-----------If the connection is successful and running, PictureBoxConnectionStatus will blink----
            If PictureBoxConnectionStatus.Visible = True Then
                PictureBoxConnectionStatus.Visible = False
            ElseIf PictureBoxConnectionStatus.Visible = False Then
                PictureBoxConnectionStatus.Visible = True
            End If
            '------------------------------------------------------------------------------------------------------
        Catch ex As Exception
            Timer1.Stop()
            SerialPort1.Close()
            'SerialPort2.Close()
            LabelStatus.Text = "Status : Disconnect"
            ButtonDisconnect.SendToBack()
            ButtonConnect.BringToFront()
            PictureBoxConnectionStatus.BackColor = Color.Red
            MsgBox("Please check the Hardware and Please connect again." & ex.Message, MsgBoxStyle.Critical, "Connection failed !!!")
            Return
        End Try
    End Sub
    '------------------------------------------------------------------------------------------------------
    '------------------------------------------------------------------------------------------------------
    Private Sub A1_Click(sender As Object, e As EventArgs) Handles A1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A10)
    End Sub
    Private Sub A2_Click(sender As Object, e As EventArgs) Handles A2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A20)
    End Sub
    Private Sub A3_Click(sender As Object, e As EventArgs) Handles A3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A30)
    End Sub
    Private Sub A4_Click(sender As Object, e As EventArgs) Handles A4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A40)
    End Sub
    Private Sub A5_Click(sender As Object, e As EventArgs) Handles A5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A50)
    End Sub
    Private Sub A6_Click(sender As Object, e As EventArgs) Handles A6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A60)
    End Sub
    Private Sub A7_Click(sender As Object, e As EventArgs) Handles A7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(A70)
        'SerialPort2.WriteLine("2")
    End Sub
    Private Sub A8_Click(sender As Object, e As EventArgs) Handles A8.Click
        ConnectionPanel.Focus()
        'SerialPort1.WriteLine(A80)
        SerialPort1.WriteLine(A80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub B1_Click(sender As Object, e As EventArgs) Handles B1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B10)
    End Sub
    Private Sub B2_Click(sender As Object, e As EventArgs) Handles B2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B20)
    End Sub
    Private Sub B3_Click(sender As Object, e As EventArgs) Handles B3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B30)
    End Sub
    Private Sub B4_Click(sender As Object, e As EventArgs) Handles B4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B40)
    End Sub
    Private Sub B5_Click(sender As Object, e As EventArgs) Handles B5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B50)
    End Sub
    Private Sub B6_Click(sender As Object, e As EventArgs) Handles B6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B60)
    End Sub
    Private Sub B7_Click(sender As Object, e As EventArgs) Handles B7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B70)
    End Sub
    Private Sub B8_Click(sender As Object, e As EventArgs) Handles B8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(B80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub C1_Click(sender As Object, e As EventArgs) Handles C1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C10)
    End Sub
    Private Sub C2_Click(sender As Object, e As EventArgs) Handles C2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C20)
    End Sub
    Private Sub C3_Click(sender As Object, e As EventArgs) Handles C3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C30)
    End Sub
    Private Sub C4_Click(sender As Object, e As EventArgs) Handles C4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C40)
    End Sub
    Private Sub C5_Click(sender As Object, e As EventArgs) Handles C5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C50)
    End Sub
    Private Sub C6_Click(sender As Object, e As EventArgs) Handles C6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C60)
    End Sub
    Private Sub C7_Click(sender As Object, e As EventArgs) Handles C7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C70)
    End Sub
    Private Sub C8_Click(sender As Object, e As EventArgs) Handles C8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(C80)
    End Sub

    '------------------------------------------------------------------------------------------------------
    Private Sub D1_Click(sender As Object, e As EventArgs) Handles D1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D10)
    End Sub
    Private Sub D2_Click(sender As Object, e As EventArgs) Handles D2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D20)
    End Sub
    Private Sub D3_Click(sender As Object, e As EventArgs) Handles D3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D30)
    End Sub
    Private Sub D4_Click(sender As Object, e As EventArgs) Handles D4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D40)
    End Sub
    Private Sub D5_Click(sender As Object, e As EventArgs) Handles D5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D50)
    End Sub
    Private Sub D6_Click(sender As Object, e As EventArgs) Handles D6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D60)
    End Sub
    Private Sub D7_Click(sender As Object, e As EventArgs) Handles D7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D70)
    End Sub
    Private Sub D8_Click(sender As Object, e As EventArgs) Handles D8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(D80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub E1_Click(sender As Object, e As EventArgs) Handles E1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E10)
    End Sub
    Private Sub E2_Click(sender As Object, e As EventArgs) Handles E2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E20)
    End Sub
    Private Sub E3_Click(sender As Object, e As EventArgs) Handles E3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E30)
    End Sub
    Private Sub E4_Click(sender As Object, e As EventArgs) Handles E4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E40)
    End Sub
    Private Sub E5_Click(sender As Object, e As EventArgs) Handles E5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E50)
    End Sub
    Private Sub E6_Click(sender As Object, e As EventArgs) Handles E6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E60)
    End Sub
    Private Sub E7_Click(sender As Object, e As EventArgs) Handles E7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E70)
    End Sub
    Private Sub E8_Click(sender As Object, e As EventArgs) Handles E8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(E80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub F1_Click(sender As Object, e As EventArgs) Handles F1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F10)
    End Sub
    Private Sub F2_Click(sender As Object, e As EventArgs) Handles F2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F20)
    End Sub
    Private Sub F3_Click(sender As Object, e As EventArgs) Handles F3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F30)
    End Sub
    Private Sub F4_Click(sender As Object, e As EventArgs) Handles F4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F40)
    End Sub
    Private Sub F5_Click(sender As Object, e As EventArgs) Handles F5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F50)
    End Sub
    Private Sub F6_Click(sender As Object, e As EventArgs) Handles F6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F60)
    End Sub
    Private Sub F7_Click(sender As Object, e As EventArgs) Handles F7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F70)
    End Sub
    Private Sub F8_Click(sender As Object, e As EventArgs) Handles F8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(F80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub G1_Click(sender As Object, e As EventArgs) Handles G1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G10)
    End Sub
    Private Sub G2_Click(sender As Object, e As EventArgs) Handles G2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G20)
    End Sub
    Private Sub G3_Click(sender As Object, e As EventArgs) Handles G3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G30)
    End Sub
    Private Sub G4_Click(sender As Object, e As EventArgs) Handles G4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G40)
    End Sub
    Private Sub G5_Click(sender As Object, e As EventArgs) Handles G5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G50)
    End Sub
    Private Sub G6_Click(sender As Object, e As EventArgs) Handles G6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G60)
    End Sub
    Private Sub G7_Click(sender As Object, e As EventArgs) Handles G7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G70)
    End Sub
    Private Sub G8_Click(sender As Object, e As EventArgs) Handles G8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(G80)
    End Sub
    '------------------------------------------------------------------------------------------------------
    Private Sub H1_Click(sender As Object, e As EventArgs) Handles H1.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H10)
    End Sub
    Private Sub H2_Click(sender As Object, e As EventArgs) Handles H2.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H20)
    End Sub
    Private Sub H3_Click(sender As Object, e As EventArgs) Handles H3.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H30)
    End Sub
    Private Sub H4_Click(sender As Object, e As EventArgs) Handles H4.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H40)
    End Sub
    Private Sub H5_Click(sender As Object, e As EventArgs) Handles H5.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H50)
    End Sub
    Private Sub H6_Click(sender As Object, e As EventArgs) Handles H6.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H60)
    End Sub
    Private Sub H7_Click(sender As Object, e As EventArgs) Handles H7.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H70)
    End Sub
    Private Sub H8_Click(sender As Object, e As EventArgs) Handles H8.Click
        ConnectionPanel.Focus()
        SerialPort1.WriteLine(H80)
        'StrSerialIn = SerialPort1.ReadExisting
        'If StrSerialIn == "H1" Then
        'H8.BackColor = Color.Red
        ' End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Status_Click(sender As Object, e As EventArgs) Handles LabelStatus.Click

    End Sub

    Private Sub Port_Click(sender As Object, e As EventArgs) Handles ButtonScanPort.Click

    End Sub

    Private Sub ConnectionPanel_Paint(sender As Object, e As PaintEventArgs) Handles ConnectionPanel.Paint

    End Sub
End Class
