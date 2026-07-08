Imports System.IO
Imports System.Runtime.InteropServices
Imports DeckLinkAPI

Public Class MainForm
    Private WithEvents recorder As DeckLinkAudioRecorder
    Private WithEvents uiTimer As Timer
    
    Private latestPeaks() As Double
    Private isCapturing As Boolean = False
    Private isRecording As Boolean = False
    Private recordingStartTime As DateTime
    Private vuControls As New List(Of VuMeter)()
    Private currentSettings As AppSettings

    Public Sub New()
        ' Initialize components (defined in MainForm.Designer.vb)
        InitializeComponent()
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set window title with date/time of the exe
        Try
            Dim exePath As String = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
            Dim exeName As String = System.IO.Path.GetFileNameWithoutExtension(exePath)
            Dim match As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(exeName, "_(\d{6})_(\d{6})$")
            If match.Success Then
                Dim dateStr As String = match.Groups(1).Value
                Dim timeStr As String = match.Groups(2).Value
                Dim formattedDateTime As String = $"{dateStr.Substring(0, 2)}/{dateStr.Substring(2, 2)}/{dateStr.Substring(4, 2)} {timeStr.Substring(0, 2)}:{timeStr.Substring(2, 2)}:{timeStr.Substring(4, 2)}"
                Me.Text = $"DeckLink Audio Recorder - {formattedDateTime}"
            Else
                Dim fileInfo As New System.IO.FileInfo(exePath)
                Me.Text = $"DeckLink Audio Recorder - {fileInfo.LastWriteTime:dd/MM/yy HH:mm:ss}"
            End If
        Catch ex As Exception
            Me.Text = "DeckLink Audio Recorder"
        End Try

        Log("Application started.")

        ' Load settings
        currentSettings = AppSettings.Load()

        ' 1. Enumerate available DeckLink Devices
        RefreshDeviceList()

        ' 2. Populate Connections
        cboAudioConnections.Items.Clear()
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionEmbedded, "Embedded (SDI/HDMI)"))
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionAnalog, "Analog (Default)"))
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionAESEBU, "AES/EBU Digital"))
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionAnalogXLR, "Analog XLR"))
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionAnalogRCA, "Analog RCA"))
        cboAudioConnections.Items.Add(New ConnectionItem(_BMDAudioConnection.bmdAudioConnectionMicrophone, "Microphone"))
        
        ' Select connection from settings
        Dim connIdx As Integer = 0
        For i As Integer = 0 To cboAudioConnections.Items.Count - 1
            Dim item = CType(cboAudioConnections.Items(i), ConnectionItem)
            If CType(item.Connection, Long) = currentSettings.AudioConnection Then
                connIdx = i
                Exit For
            End If
        Next
        cboAudioConnections.SelectedIndex = connIdx

        ' 3. Populate Channels
        cboChannels.Items.Clear()
        cboChannels.Items.Add(2)
        cboChannels.Items.Add(8)
        cboChannels.Items.Add(16)
        
        ' Select channels from settings
        Dim chanIdx As Integer = 0
        For i As Integer = 0 To cboChannels.Items.Count - 1
            If CInt(cboChannels.Items(i)) = currentSettings.Channels Then
                chanIdx = i
                Exit For
            End If
        Next
        cboChannels.SelectedIndex = chanIdx

        ' 4. Populate Sample Formats
        cboSampleFormats.Items.Clear()
        cboSampleFormats.Items.Add("16-bit Integer")
        cboSampleFormats.Items.Add("32-bit Integer")
        
        ' Select sample format from settings
        Dim fmtIdx As Integer = 0
        For i As Integer = 0 To cboSampleFormats.Items.Count - 1
            If cboSampleFormats.Items(i).ToString() = currentSettings.SampleFormat Then
                fmtIdx = i
                Exit For
            End If
        Next
        cboSampleFormats.SelectedIndex = fmtIdx

        ' 5. Default Recording Path
        txtRecordingPath.Text = currentSettings.OutputFolder
        txtFilename.Text = currentSettings.FilenameBase

        ' Select device from settings
        If cboDevices.Items.Count > 0 Then
            Dim devIdx As Integer = 0
            If Not String.IsNullOrEmpty(currentSettings.DeviceName) Then
                Dim found = cboDevices.FindStringExact(currentSettings.DeviceName)
                If found >= 0 Then devIdx = found
            End If
            cboDevices.SelectedIndex = devIdx
        End If

        ' 6. Initialize UI update timer (30ms interval for smooth VU rendering)
        uiTimer = New Timer()
        uiTimer.Interval = 30
        AddHandler uiTimer.Tick, AddressOf UiTimer_Tick
        uiTimer.Start()

        UpdateControlStates()
    End Sub

    Private Sub RefreshDeviceList()
        cboDevices.Items.Clear()
        Try
            Dim devices = DeckLinkAudioRecorder.EnumerateDevices()
            For Each dev In devices
                cboDevices.Items.Add(dev)
            Next

            If cboDevices.Items.Count > 0 Then
                cboDevices.SelectedIndex = 0
                Log($"Found {cboDevices.Items.Count} DeckLink device(s).")
            Else
                Log("No DeckLink devices detected on this system.")
            End If
        Catch ex As Exception
            Log($"Error scanning DeckLink devices: {ex.Message}")
        End Try
    End Sub

    Private Sub cboDevices_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDevices.SelectedIndexChanged
        If cboDevices.SelectedIndex < 0 Then Return

        ' Populate Video display modes for the selected device (required for clocking audio)
        cboDisplayModes.Items.Clear()
        Dim tempRecorder As New DeckLinkAudioRecorder()
        Dim modes = tempRecorder.GetDisplayModes(cboDevices.SelectedItem.ToString())
        tempRecorder.Dispose()

        For Each modeItem In modes
            cboDisplayModes.Items.Add(New DisplayModeItem(modeItem.Mode, modeItem.Name))
        Next

        If cboDisplayModes.Items.Count > 0 Then
            ' Select display mode from settings if available
            Dim modeIdx As Integer = 0
            If currentSettings IsNot Nothing Then
                For i As Integer = 0 To cboDisplayModes.Items.Count - 1
                    Dim item = CType(cboDisplayModes.Items(i), DisplayModeItem)
                    If CType(item.Mode, Long) = currentSettings.DisplayMode Then
                        modeIdx = i
                        Exit For
                    End If
                Next
            End If
            cboDisplayModes.SelectedIndex = modeIdx
        Else
            ' Fallback standard modes
            cboDisplayModes.Items.Add(New DisplayModeItem(_BMDDisplayMode.bmdModeHD1080p25, "1080p25 (Default Fallback)"))
            cboDisplayModes.Items.Add(New DisplayModeItem(_BMDDisplayMode.bmdModePAL, "PAL (SD Fallback)"))
            cboDisplayModes.SelectedIndex = 0
        End If
    End Sub

    Private Sub cboChannels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboChannels.SelectedIndexChanged
        RebuildVuMeters()
    End Sub

    ''' <summary>
    ''' Rebuilds the visual VU meters based on the selected channel count.
    ''' </summary>
    Private Sub RebuildVuMeters()
        pnlVuMeters.SuspendLayout()
        
        ' Dispose old controls
        For Each meter In vuControls
            pnlVuMeters.Controls.Remove(meter)
            meter.Dispose()
        Next
        vuControls.Clear()

        Dim numChannels As Integer = If(cboChannels.SelectedItem IsNot Nothing, CInt(cboChannels.SelectedItem), 2)
        latestPeaks = New Double(numChannels - 1) {}
        For i As Integer = 0 To numChannels - 1
            latestPeaks(i) = -60.0
        Next

        ' Add new VU meter controls
        For i As Integer = 1 To numChannels
            Dim meter As New VuMeter() With {
                .ChannelLabel = $"CH {i}",
                .Width = pnlVuMeters.Width - 25,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Margin = New Padding(0, 2, 0, 2)
            }
            vuControls.Add(meter)
            pnlVuMeters.Controls.Add(meter)
        Next

        pnlVuMeters.ResumeLayout()
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Select Folder to save recorded WAV files"
            fbd.SelectedPath = txtRecordingPath.Text
            If fbd.ShowDialog() = DialogResult.OK Then
                txtRecordingPath.Text = fbd.SelectedPath
            End If
        End Using
    End Sub

    Private Sub btnStartCapture_Click(sender As Object, e As EventArgs) Handles btnStartCapture.Click
        If isCapturing Then
            ' Stop Capture
            StopCaptureInternal()
        Else
            ' Start Capture
            StartCaptureInternal()
        End If
    End Sub

    Private Sub StartCaptureInternal()
        If cboDevices.SelectedIndex < 0 Then
            MessageBox.Show("Please select a valid DeckLink device.", "Capture Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            recorder = New DeckLinkAudioRecorder()
            AddHandler recorder.LogReceived, AddressOf Recorder_LogReceived
            AddHandler recorder.AudioSamplesCaptured, AddressOf Recorder_AudioSamplesCaptured

            Dim deviceName As String = cboDevices.SelectedItem.ToString()
            Dim connection = CType(cboAudioConnections.SelectedItem, ConnectionItem).Connection
            Dim mode = CType(cboDisplayModes.SelectedItem, DisplayModeItem).Mode
            Dim channels = CInt(cboChannels.SelectedItem)
            Dim sampleType = If(cboSampleFormats.SelectedIndex = 1,
                                _BMDAudioSampleType.bmdAudioSampleType32bitInteger,
                                _BMDAudioSampleType.bmdAudioSampleType16bitInteger)

            recorder.StartCapture(deviceName, connection, mode, _BMDAudioSampleRate.bmdAudioSampleRate48kHz, sampleType, channels)
            
            isCapturing = True
            lblStatus.Text = "Status: Capturing Live Audio"
            lblStatus.ForeColor = Color.FromArgb(0, 180, 216) ' Neon Blue
            Log("Live capture started.")
        Catch ex As Exception
            Log($"Failed to start capture: {ex.Message}")
            Log("Tip: If the device is busy, click the red 'Release' button to terminate other software holding the card.")
            MessageBox.Show($"Failed to start capture: {ex.Message}{Environment.NewLine}{Environment.NewLine}Tip: If the card is busy, try clicking the 'Release' button to stop other apps.", "DeckLink Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            StopCaptureInternal()
        End Try

        UpdateControlStates()
    End Sub

    Private Sub StopCaptureInternal()
        isCapturing = False
        isRecording = False

        If recorder IsNot Nothing Then
            recorder.StopCapture()
            RemoveHandler recorder.LogReceived, AddressOf Recorder_LogReceived
            RemoveHandler recorder.AudioSamplesCaptured, AddressOf Recorder_AudioSamplesCaptured
            recorder.Dispose()
            recorder = Nothing
        End If

        lblStatus.Text = "Status: Idle"
        lblStatus.ForeColor = Color.DarkGray
        Log("Capture stopped.")

        ' Reset VU meters to quiet
        For Each meter In vuControls
            meter.Value = -60.0
        Next

        UpdateControlStates()
    End Sub

    Private Sub btnRecord_Click(sender As Object, e As EventArgs) Handles btnRecord.Click
        If isRecording Then
            ' Stop Recording
            StopRecordingInternal()
        Else
            ' Start Recording
            StartRecordingInternal()
        End If
    End Sub

    Private Sub StartRecordingInternal()
        If Not isCapturing OrElse recorder Is Nothing Then Return

        ' Validate path
        Dim outputFolder As String = txtRecordingPath.Text.Trim()
        Dim filename As String = txtFilename.Text.Trim()

        If String.IsNullOrWhiteSpace(outputFolder) OrElse String.IsNullOrWhiteSpace(filename) Then
            MessageBox.Show("Please enter a valid folder path and filename.", "Recording Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            If Not Directory.Exists(outputFolder) Then
                Directory.CreateDirectory(outputFolder)
            End If

            ' Strip file extension if any
            Dim baseName As String = Path.GetFileNameWithoutExtension(filename)
            
            ' Strip existing trailing timestamp from baseName if it matches _ddMMyy_HHmmss
            If baseName.Length > 14 AndAlso baseName.Chars(baseName.Length - 14) = "_"c Then
                Dim suffixCandidate As String = baseName.Substring(baseName.Length - 13)
                Dim isMatch As Boolean = True
                For idx As Integer = 0 To suffixCandidate.Length - 1
                    Dim ch As Char = suffixCandidate.Chars(idx)
                    If idx = 6 Then
                        If ch <> "_"c Then isMatch = False
                    Else
                        If Not Char.IsDigit(ch) Then isMatch = False
                    End If
                Next
                If isMatch Then
                    baseName = baseName.Substring(0, baseName.Length - 14)
                End If
            End If

            ' Append current date/time suffix
            Dim timestampedName As String = baseName & "_" & DateTime.Now.ToString("ddMMyy_HHmmss") & ".wav"
            txtFilename.Text = timestampedName

            Dim fullPath As String = Path.Combine(outputFolder, timestampedName)
            recorder.StartRecording(fullPath)
            
            recordingStartTime = DateTime.Now
            isRecording = True
            lblStatus.Text = "Status: RECORDING AUDIO"
            lblStatus.ForeColor = Color.FromArgb(224, 30, 90) ' Neon pink/red
            Log($"Recording started -> {fullPath}")
        Catch ex As Exception
            Log($"Failed to start recording: {ex.Message}")
            MessageBox.Show($"Failed to start recording: {ex.Message}", "Recording Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            StopRecordingInternal()
        End Try

        UpdateControlStates()
    End Sub

    Private Sub StopRecordingInternal()
        If recorder IsNot Nothing Then
            recorder.StopRecording()
        End If

        isRecording = False
        lblStatus.Text = "Status: Capturing Live Audio"
        lblStatus.ForeColor = Color.FromArgb(0, 180, 216) ' Neon Blue
        Log("Recording saved.")
        
        ' Remove timestamp to restore back to base name
        Dim currentFilename As String = txtFilename.Text.Trim()
        Dim baseName As String = Path.GetFileNameWithoutExtension(currentFilename)
        If baseName.Length > 14 AndAlso baseName.Chars(baseName.Length - 14) = "_"c Then
            Dim suffixCandidate As String = baseName.Substring(baseName.Length - 13)
            Dim isMatch As Boolean = True
            For idx As Integer = 0 To suffixCandidate.Length - 1
                Dim ch As Char = suffixCandidate.Chars(idx)
                If idx = 6 Then
                    If ch <> "_"c Then isMatch = False
                Else
                    If Not Char.IsDigit(ch) Then isMatch = False
                End If
            Next
            If isMatch Then
                baseName = baseName.Substring(0, baseName.Length - 14)
            End If
        End If
        txtFilename.Text = baseName

        UpdateControlStates()
    End Sub

    Private Sub UpdateControlStates()
        cboDevices.Enabled = Not isCapturing
        cboAudioConnections.Enabled = Not isCapturing
        cboDisplayModes.Enabled = Not isCapturing
        cboChannels.Enabled = Not isCapturing
        cboSampleFormats.Enabled = Not isCapturing
        
        txtRecordingPath.Enabled = Not isRecording
        txtFilename.Enabled = Not isRecording
        btnBrowse.Enabled = Not isRecording

        ' Capture Button styling
        If isCapturing Then
            btnStartCapture.Text = "Stop Capture"
            btnStartCapture.BackColor = Color.FromArgb(64, 64, 66)
            btnRecord.Enabled = True
        Else
            btnStartCapture.Text = "Start Capture"
            btnStartCapture.BackColor = Color.FromArgb(0, 119, 182)
            btnRecord.Enabled = False
        End If

        ' Record Button styling
        If isRecording Then
            btnRecord.Text = "Stop Record"
            btnRecord.BackColor = Color.FromArgb(64, 64, 66)
        Else
            btnRecord.Text = "Record"
            btnRecord.BackColor = Color.FromArgb(217, 4, 41)
        End If
    End Sub

    ' --- Event Handlers from DeckLinkAudioRecorder ---

    Private Sub Recorder_LogReceived(message As String)
        Log(message)
    End Sub

    Private Sub Recorder_AudioSamplesCaptured(rawBytes() As Byte, length As Integer, peakLevels() As Double)
        ' Thread-safe assignment of peaks
        latestPeaks = peakLevels
    End Sub

    ' --- Animation & UI Timer Tick ---

    Private Sub UiTimer_Tick(sender As Object, e As EventArgs)
        ' 1. Update VU Meters from the background thread's latest peaks
        Dim peaks = latestPeaks
        If peaks IsNot Nothing Then
            For i As Integer = 0 To Math.Min(peaks.Length - 1, vuControls.Count - 1)
                vuControls(i).Value = peaks(i)
            Next
        End If

        ' 2. Update duration and file size if recording
        If isRecording AndAlso recorder IsNot Nothing Then
            Dim duration As TimeSpan = DateTime.Now - recordingStartTime
            lblDuration.Text = $"Duration: {duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}"
            
            Dim bytes As Long = recorder.RecordedBytesCount
            lblFileSize.Text = $"File Size: {FormatBytes(bytes)}"
        Else
            lblDuration.Text = "Duration: 00:00:00"
            lblFileSize.Text = "File Size: 0.00 KB"
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshDeviceList()
    End Sub

    Private Sub btnRelease_Click(sender As Object, e As EventArgs) Handles btnRelease.Click
        ReleaseCards()
    End Sub

    Private Sub ReleaseCards()
        Log("Attempting to release DeckLink cards by stopping holding processes...")
        Dim processesToKill = New String() {
            "MediaExpress",
            "Blackmagic Media Express",
            "decklinkplayer",
            "DeckLinkOutputHelper",
            "ffmpeg",
            "ffplay",
            "vmix",
            "vmix64",
            "obs",
            "obs64",
            "vlc",
            "Ceftodecklink",
            "FfmpegRecorder"
        }

        Dim killedCount As Integer = 0

        ' 1. Terminate specific media helper processes
        For Each procName In processesToKill
            Try
                For Each runningProcess In Process.GetProcessesByName(procName)
                    Try
                        If runningProcess.Id <> Process.GetCurrentProcess().Id Then
                            runningProcess.Kill(entireProcessTree:=True)
                            Log($"Terminated process: {procName} (PID: {runningProcess.Id})")
                            killedCount += 1
                        End If
                    Catch ex As Exception
                        Log($"Failed to terminate process {procName}: {ex.Message}")
                    Finally
                        runningProcess.Dispose()
                    End Try
                Next
            Catch
            End Try
        Next

        ' 2. Terminate any other process containing 'decklink' or 'ceftodecklink' (except ourselves)
        For Each runningProcess In Process.GetProcesses()
            Try
                Dim procName As String = runningProcess.ProcessName
                If (procName.Contains("decklink", StringComparison.OrdinalIgnoreCase) OrElse
                    procName.Contains("ceftodecklink", StringComparison.OrdinalIgnoreCase)) AndAlso
                    runningProcess.Id <> Process.GetCurrentProcess().Id Then
                    
                    runningProcess.Kill(entireProcessTree:=True)
                    Log($"Terminated process: {procName} (PID: {runningProcess.Id})")
                    killedCount += 1
                End If
            Catch
            Finally
                runningProcess.Dispose()
            End Try
        Next
        
        If killedCount > 0 Then
            Log($"Successfully terminated {killedCount} process(es) holding DeckLink cards.")
            ' Refresh device list to rebuild connections
            RefreshDeviceList()
        Else
            Log("No known media processes or locked cards were detected.")
        End If
    End Sub

    ' --- Logger utility ---

    Private Sub Log(message As String)
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() Log(message))
            Return
        End If

        Dim timestamp As String = DateTime.Now.ToString("HH:mm:ss.fff")
        txtLogs.AppendText($"[{timestamp}] {message}{Environment.NewLine}")
        txtLogs.SelectionStart = txtLogs.TextLength
        txtLogs.ScrollToCaret()
    End Sub

    Private Shared Function FormatBytes(bytes As Long) As String
        Dim kb As Double = bytes / 1024.0
        Dim mb As Double = kb / 1024.0
        If mb >= 1.0 Then
            Return $"{mb:F2} MB"
        ElseIf kb >= 1.0 Then
            Return $"{kb:F2} KB"
        Else
            Return $"{bytes} bytes"
        End If
    End Function

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        StopCaptureInternal()

        ' Save current settings
        If currentSettings IsNot Nothing Then
            If cboDevices.SelectedItem IsNot Nothing Then
                currentSettings.DeviceName = cboDevices.SelectedItem.ToString()
            End If
            If cboAudioConnections.SelectedItem IsNot Nothing Then
                currentSettings.AudioConnection = CType(CType(cboAudioConnections.SelectedItem, ConnectionItem).Connection, Long)
            End If
            If cboDisplayModes.SelectedItem IsNot Nothing Then
                currentSettings.DisplayMode = CType(CType(cboDisplayModes.SelectedItem, DisplayModeItem).Mode, Long)
            End If
            If cboChannels.SelectedItem IsNot Nothing Then
                currentSettings.Channels = CInt(cboChannels.SelectedItem)
            End If
            If cboSampleFormats.SelectedItem IsNot Nothing Then
                currentSettings.SampleFormat = cboSampleFormats.SelectedItem.ToString()
            End If
            currentSettings.OutputFolder = txtRecordingPath.Text.Trim()
            
            ' Strip trailing timestamp if present before saving to settings so the next launch has clean base name
            Dim baseName As String = Path.GetFileNameWithoutExtension(txtFilename.Text.Trim())
            If baseName.Length > 14 AndAlso baseName.Chars(baseName.Length - 14) = "_"c Then
                Dim suffixCandidate As String = baseName.Substring(baseName.Length - 13)
                Dim isMatch As Boolean = True
                For idx As Integer = 0 To suffixCandidate.Length - 1
                    Dim ch As Char = suffixCandidate.Chars(idx)
                    If idx = 6 Then
                        If ch <> "_"c Then isMatch = False
                    Else
                        If Not Char.IsDigit(ch) Then isMatch = False
                    End If
                Next
                If isMatch Then
                    baseName = baseName.Substring(0, baseName.Length - 14)
                End If
            End If
            currentSettings.FilenameBase = baseName
            
            currentSettings.Save()
        End If

        If uiTimer IsNot Nothing Then
            uiTimer.Stop()
            uiTimer.Dispose()
        End If
    End Sub

    ' --- Helper Combobox item structures ---

    Private Class ConnectionItem
        Public Connection As _BMDAudioConnection
        Public Name As String

        Public Sub New(conn As _BMDAudioConnection, nameStr As String)
            Connection = conn
            Name = nameStr
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    Private Class DisplayModeItem
        Public Mode As _BMDDisplayMode
        Public Name As String

        Public Sub New(m As _BMDDisplayMode, nameStr As String)
            Mode = m
            Name = nameStr
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Class
