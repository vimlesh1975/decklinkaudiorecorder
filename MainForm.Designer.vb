<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        
        ' Panel Containers
        Me.pnlConfig = New System.Windows.Forms.Panel()
        Me.pnlVuOuter = New System.Windows.Forms.Panel()
        Me.pnlLogsOuter = New System.Windows.Forms.Panel()
        
        ' Title and Headers
        Me.lblAppTitle = New System.Windows.Forms.Label()
        Me.lblConfigHeader = New System.Windows.Forms.Label()
        Me.lblVuHeader = New System.Windows.Forms.Label()
        Me.lblLogsHeader = New System.Windows.Forms.Label()
        
        ' Configuration Controls
        Me.lblDevice = New System.Windows.Forms.Label()
        Me.cboDevices = New System.Windows.Forms.ComboBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnRelease = New System.Windows.Forms.Button()
        
        Me.lblConnection = New System.Windows.Forms.Label()
        Me.cboAudioConnections = New System.Windows.Forms.ComboBox()
        
        Me.lblDisplayMode = New System.Windows.Forms.Label()
        Me.cboDisplayModes = New System.Windows.Forms.ComboBox()
        
        Me.lblChannels = New System.Windows.Forms.Label()
        Me.cboChannels = New System.Windows.Forms.ComboBox()
        
        Me.lblSampleFormat = New System.Windows.Forms.Label()
        Me.cboSampleFormats = New System.Windows.Forms.ComboBox()
        
        Me.lblRecDir = New System.Windows.Forms.Label()
        Me.txtRecordingPath = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        
        Me.lblFilename = New System.Windows.Forms.Label()
        Me.txtFilename = New System.Windows.Forms.TextBox()
        
        ' Status & Action Controls
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblDuration = New System.Windows.Forms.Label()
        Me.lblFileSize = New System.Windows.Forms.Label()
        
        Me.btnStartCapture = New System.Windows.Forms.Button()
        Me.btnRecord = New System.Windows.Forms.Button()
        
        ' Flow Panel for VU Meters
        Me.pnlVuMeters = New System.Windows.Forms.FlowLayoutPanel()
        
        ' Log Controls
        Me.txtLogs = New System.Windows.Forms.TextBox()
        
        Me.pnlConfig.SuspendLayout()
        Me.pnlVuOuter.SuspendLayout()
        Me.pnlLogsOuter.SuspendLayout()
        Me.SuspendLayout()
        
        ' ----------------------------------------------------
        ' lblAppTitle (Global Header)
        ' ----------------------------------------------------
        Me.lblAppTitle.AutoSize = True
        Me.lblAppTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 16.0F, System.Drawing.FontStyle.Bold)
        Me.lblAppTitle.ForeColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(180, Byte), CType(216, Byte))
        Me.lblAppTitle.Location = New System.Drawing.Point(12, 10)
        Me.lblAppTitle.Name = "lblAppTitle"
        Me.lblAppTitle.Size = New System.Drawing.Size(370, 30)
        Me.lblAppTitle.TabIndex = 0
        Me.lblAppTitle.Text = "BLACKMAGIC DECKLINK RECORDER"
        
        ' ----------------------------------------------------
        ' pnlConfig (Left Card Panel)
        ' ----------------------------------------------------
        Me.pnlConfig.BackColor = System.Drawing.Color.FromArgb(CType(24, Byte), CType(24, Byte), CType(27, Byte))
        Me.pnlConfig.Controls.Add(Me.lblConfigHeader)
        Me.pnlConfig.Controls.Add(Me.lblDevice)
        Me.pnlConfig.Controls.Add(Me.cboDevices)
        Me.pnlConfig.Controls.Add(Me.btnRefresh)
        Me.pnlConfig.Controls.Add(Me.btnRelease)
        Me.pnlConfig.Controls.Add(Me.lblConnection)
        Me.pnlConfig.Controls.Add(Me.cboAudioConnections)
        Me.pnlConfig.Controls.Add(Me.lblDisplayMode)
        Me.pnlConfig.Controls.Add(Me.cboDisplayModes)
        Me.pnlConfig.Controls.Add(Me.lblChannels)
        Me.pnlConfig.Controls.Add(Me.cboChannels)
        Me.pnlConfig.Controls.Add(Me.lblSampleFormat)
        Me.pnlConfig.Controls.Add(Me.cboSampleFormats)
        Me.pnlConfig.Controls.Add(Me.lblRecDir)
        Me.pnlConfig.Controls.Add(Me.txtRecordingPath)
        Me.pnlConfig.Controls.Add(Me.btnBrowse)
        Me.pnlConfig.Controls.Add(Me.lblFilename)
        Me.pnlConfig.Controls.Add(Me.txtFilename)
        Me.pnlConfig.Controls.Add(Me.lblStatus)
        Me.pnlConfig.Controls.Add(Me.lblDuration)
        Me.pnlConfig.Controls.Add(Me.lblFileSize)
        Me.pnlConfig.Controls.Add(Me.btnStartCapture)
        Me.pnlConfig.Controls.Add(Me.btnRecord)
        Me.pnlConfig.Location = New System.Drawing.Point(12, 50)
        Me.pnlConfig.Name = "pnlConfig"
        Me.pnlConfig.Size = New System.Drawing.Size(420, 440)
        Me.pnlConfig.TabIndex = 1
        
        ' lblConfigHeader
        Me.lblConfigHeader.AutoSize = True
        Me.lblConfigHeader.Font = New System.Drawing.Font("Segoe UI", 10.0F, System.Drawing.FontStyle.Bold)
        Me.lblConfigHeader.ForeColor = System.Drawing.Color.White
        Me.lblConfigHeader.Location = New System.Drawing.Point(10, 8)
        Me.lblConfigHeader.Name = "lblConfigHeader"
        Me.lblConfigHeader.Size = New System.Drawing.Size(184, 19)
        Me.lblConfigHeader.Text = "HARDWARE CONFIGURATION"
        
        ' lblDevice / cboDevices / btnRefresh / btnRelease
        Me.lblDevice.AutoSize = True
        Me.lblDevice.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblDevice.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblDevice.Location = New System.Drawing.Point(10, 36)
        Me.lblDevice.Text = "DeckLink Device"
        
        Me.cboDevices.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.cboDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboDevices.ForeColor = System.Drawing.Color.White
        Me.cboDevices.Location = New System.Drawing.Point(10, 52)
        Me.cboDevices.Size = New System.Drawing.Size(210, 23)
        
        Me.btnRefresh.BackColor = System.Drawing.Color.FromArgb(CType(50, Byte), CType(50, Byte), CType(54, Byte))
        Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefresh.ForeColor = System.Drawing.Color.White
        Me.btnRefresh.Location = New System.Drawing.Point(230, 51)
        Me.btnRefresh.Size = New System.Drawing.Size(80, 25)
        Me.btnRefresh.Text = "Scan"
        Me.btnRefresh.UseVisualStyleBackColor = False

        Me.btnRelease.BackColor = System.Drawing.Color.FromArgb(CType(75, Byte), CType(30, Byte), CType(30, Byte))
        Me.btnRelease.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRelease.ForeColor = System.Drawing.Color.FromArgb(CType(255, Byte), CType(150, Byte), CType(150, Byte))
        Me.btnRelease.Location = New System.Drawing.Point(320, 51)
        Me.btnRelease.Size = New System.Drawing.Size(90, 25)
        Me.btnRelease.Text = "Release"
        Me.btnRelease.UseVisualStyleBackColor = False
        
        ' lblConnection / cboAudioConnections
        Me.lblConnection.AutoSize = True
        Me.lblConnection.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblConnection.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblConnection.Location = New System.Drawing.Point(10, 83)
        Me.lblConnection.Text = "Audio Input Connection"
        
        Me.cboAudioConnections.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.cboAudioConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAudioConnections.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboAudioConnections.ForeColor = System.Drawing.Color.White
        Me.cboAudioConnections.Location = New System.Drawing.Point(10, 99)
        Me.cboAudioConnections.Size = New System.Drawing.Size(395, 23)
        
        ' lblDisplayMode / cboDisplayModes
        Me.lblDisplayMode.AutoSize = True
        Me.lblDisplayMode.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblDisplayMode.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblDisplayMode.Location = New System.Drawing.Point(10, 130)
        Me.lblDisplayMode.Text = "Video Format (Required clock lock)"
        
        Me.cboDisplayModes.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.cboDisplayModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDisplayModes.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboDisplayModes.ForeColor = System.Drawing.Color.White
        Me.cboDisplayModes.Location = New System.Drawing.Point(10, 146)
        Me.cboDisplayModes.Size = New System.Drawing.Size(395, 23)
        
        ' lblChannels / cboChannels
        Me.lblChannels.AutoSize = True
        Me.lblChannels.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblChannels.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblChannels.Location = New System.Drawing.Point(10, 177)
        Me.lblChannels.Text = "Audio Channels"
        
        Me.cboChannels.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.cboChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboChannels.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboChannels.ForeColor = System.Drawing.Color.White
        Me.cboChannels.Location = New System.Drawing.Point(10, 193)
        Me.cboChannels.Size = New System.Drawing.Size(190, 23)
        
        ' lblSampleFormat / cboSampleFormats
        Me.lblSampleFormat.AutoSize = True
        Me.lblSampleFormat.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblSampleFormat.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblSampleFormat.Location = New System.Drawing.Point(215, 177)
        Me.lblSampleFormat.Text = "Sample Bit Depth"
        
        Me.cboSampleFormats.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.cboSampleFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSampleFormats.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSampleFormats.ForeColor = System.Drawing.Color.White
        Me.cboSampleFormats.Location = New System.Drawing.Point(215, 193)
        Me.cboSampleFormats.Size = New System.Drawing.Size(190, 23)
        
        ' lblRecDir / txtRecordingPath / btnBrowse
        Me.lblRecDir.AutoSize = True
        Me.lblRecDir.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblRecDir.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblRecDir.Location = New System.Drawing.Point(10, 226)
        Me.lblRecDir.Text = "Save Output Folder"
        
        Me.txtRecordingPath.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.txtRecordingPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtRecordingPath.ForeColor = System.Drawing.Color.White
        Me.txtRecordingPath.Location = New System.Drawing.Point(10, 242)
        Me.txtRecordingPath.Size = New System.Drawing.Size(295, 23)
        
        Me.btnBrowse.BackColor = System.Drawing.Color.FromArgb(CType(50, Byte), CType(50, Byte), CType(54, Byte))
        Me.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBrowse.ForeColor = System.Drawing.Color.White
        Me.btnBrowse.Location = New System.Drawing.Point(310, 241)
        Me.btnBrowse.Size = New System.Drawing.Size(95, 25)
        Me.btnBrowse.Text = "Browse..."
        Me.btnBrowse.UseVisualStyleBackColor = False
        
        ' lblFilename / txtFilename
        Me.lblFilename.AutoSize = True
        Me.lblFilename.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular)
        Me.lblFilename.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblFilename.Location = New System.Drawing.Point(10, 273)
        Me.lblFilename.Text = "WAV Filename (excluding .wav)"
        
        Me.txtFilename.BackColor = System.Drawing.Color.FromArgb(CType(38, Byte), CType(38, Byte), CType(40, Byte))
        Me.txtFilename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtFilename.ForeColor = System.Drawing.Color.White
        Me.txtFilename.Location = New System.Drawing.Point(10, 289)
        Me.txtFilename.Size = New System.Drawing.Size(395, 23)
        
        ' lblStatus
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold)
        Me.lblStatus.ForeColor = System.Drawing.Color.DarkGray
        Me.lblStatus.Location = New System.Drawing.Point(10, 325)
        Me.lblStatus.Text = "Status: Idle"
        
        ' lblDuration / lblFileSize
        Me.lblDuration.AutoSize = True
        Me.lblDuration.Font = New System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold)
        Me.lblDuration.ForeColor = System.Drawing.Color.FromArgb(CType(200, Byte), CType(200, Byte), CType(205, Byte))
        Me.lblDuration.Location = New System.Drawing.Point(10, 350)
        Me.lblDuration.Text = "Duration: 00:00:00"
        
        Me.lblFileSize.AutoSize = True
        Me.lblFileSize.Font = New System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold)
        Me.lblFileSize.ForeColor = System.Drawing.Color.FromArgb(CType(200, Byte), CType(200, Byte), CType(205, Byte))
        Me.lblFileSize.Location = New System.Drawing.Point(215, 350)
        Me.lblFileSize.Text = "File Size: 0.00 KB"
        
        ' btnStartCapture / btnRecord
        Me.btnStartCapture.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(119, Byte), CType(182, Byte))
        Me.btnStartCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStartCapture.Font = New System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold)
        Me.btnStartCapture.ForeColor = System.Drawing.Color.White
        Me.btnStartCapture.Location = New System.Drawing.Point(10, 385)
        Me.btnStartCapture.Size = New System.Drawing.Size(190, 42)
        Me.btnStartCapture.Text = "Start Capture"
        Me.btnStartCapture.UseVisualStyleBackColor = False
        
        Me.btnRecord.BackColor = System.Drawing.Color.FromArgb(CType(217, Byte), CType(4, Byte), CType(41, Byte))
        Me.btnRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRecord.Font = New System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold)
        Me.btnRecord.ForeColor = System.Drawing.Color.White
        Me.btnRecord.Location = New System.Drawing.Point(215, 385)
        Me.btnRecord.Size = New System.Drawing.Size(190, 42)
        Me.btnRecord.Text = "Record"
        Me.btnRecord.UseVisualStyleBackColor = False
        
        ' ----------------------------------------------------
        ' pnlVuOuter (Right Card Panel)
        ' ----------------------------------------------------
        Me.pnlVuOuter.BackColor = System.Drawing.Color.FromArgb(CType(24, Byte), CType(24, Byte), CType(27, Byte))
        Me.pnlVuOuter.Controls.Add(Me.lblVuHeader)
        Me.pnlVuOuter.Controls.Add(Me.pnlVuMeters)
        Me.pnlVuOuter.Location = New System.Drawing.Point(448, 50)
        Me.pnlVuOuter.Name = "pnlVuOuter"
        Me.pnlVuOuter.Size = New System.Drawing.Size(484, 440)
        Me.pnlVuOuter.TabIndex = 2
        
        ' lblVuHeader
        Me.lblVuHeader.AutoSize = True
        Me.lblVuHeader.Font = New System.Drawing.Font("Segoe UI", 10.0F, System.Drawing.FontStyle.Bold)
        Me.lblVuHeader.ForeColor = System.Drawing.Color.White
        Me.lblVuHeader.Location = New System.Drawing.Point(10, 8)
        Me.lblVuHeader.Name = "lblVuHeader"
        Me.lblVuHeader.Size = New System.Drawing.Size(167, 19)
        Me.lblVuHeader.Text = "LIVE AUDIO LEVEL METERS"
        
        ' pnlVuMeters (Flow Layout for the VU Channels)
        Me.pnlVuMeters.AutoScroll = True
        Me.pnlVuMeters.BackColor = System.Drawing.Color.FromArgb(CType(18, Byte), CType(18, Byte), CType(20, Byte))
        Me.pnlVuMeters.Location = New System.Drawing.Point(10, 30)
        Me.pnlVuMeters.Name = "pnlVuMeters"
        Me.pnlVuMeters.Size = New System.Drawing.Size(464, 397)
        Me.pnlVuMeters.TabIndex = 0
        
        ' ----------------------------------------------------
        ' pnlLogsOuter (Bottom Logs Panel)
        ' ----------------------------------------------------
        Me.pnlLogsOuter.BackColor = System.Drawing.Color.FromArgb(CType(24, Byte), CType(24, Byte), CType(27, Byte))
        Me.pnlLogsOuter.Controls.Add(Me.lblLogsHeader)
        Me.pnlLogsOuter.Controls.Add(Me.txtLogs)
        Me.pnlLogsOuter.Location = New System.Drawing.Point(12, 500)
        Me.pnlLogsOuter.Name = "pnlLogsOuter"
        Me.pnlLogsOuter.Size = New System.Drawing.Size(920, 100)
        Me.pnlLogsOuter.TabIndex = 3
        
        ' lblLogsHeader
        Me.lblLogsHeader.AutoSize = True
        Me.lblLogsHeader.Font = New System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold)
        Me.lblLogsHeader.ForeColor = System.Drawing.Color.FromArgb(CType(160, Byte), CType(160, Byte), CType(165, Byte))
        Me.lblLogsHeader.Location = New System.Drawing.Point(10, 5)
        Me.lblLogsHeader.Name = "lblLogsHeader"
        Me.lblLogsHeader.Size = New System.Drawing.Size(155, 13)
        Me.lblLogsHeader.Text = "DIAGNOSTICS & SYSTEM LOGS"
        
        ' txtLogs
        Me.txtLogs.BackColor = System.Drawing.Color.FromArgb(CType(12, Byte), CType(12, Byte), CType(14, Byte))
        Me.txtLogs.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLogs.Font = New System.Drawing.Font("Consolas", 8.25F)
        Me.txtLogs.ForeColor = System.Drawing.Color.FromArgb(CType(144, Byte), CType(224, Byte), CType(239, Byte))
        Me.txtLogs.Location = New System.Drawing.Point(10, 20)
        Me.txtLogs.Multiline = True
        Me.txtLogs.Name = "txtLogs"
        Me.txtLogs.ReadOnly = True
        Me.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLogs.Size = New System.Drawing.Size(900, 70)
        Me.txtLogs.TabIndex = 0
        
        ' ----------------------------------------------------
        ' MainForm (Base Settings)
        ' ----------------------------------------------------
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0F, 15.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(15, Byte), CType(15, Byte), CType(17, Byte))
        Me.ClientSize = New System.Drawing.Size(944, 612)
        Me.Controls.Add(Me.pnlLogsOuter)
        Me.Controls.Add(Me.pnlVuOuter)
        Me.Controls.Add(Me.pnlConfig)
        Me.Controls.Add(Me.lblAppTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0F)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DeckLink Multi-Channel Audio Recorder"
        
        Me.pnlConfig.ResumeLayout(False)
        Me.pnlConfig.PerformLayout()
        Me.pnlVuOuter.ResumeLayout(False)
        Me.pnlVuOuter.PerformLayout()
        Me.pnlLogsOuter.ResumeLayout(False)
        Me.pnlLogsOuter.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    ' Member Controls Declarations
    Friend WithEvents lblAppTitle As System.Windows.Forms.Label
    
    ' Left Panel
    Friend WithEvents pnlConfig As System.Windows.Forms.Panel
    Friend WithEvents lblConfigHeader As System.Windows.Forms.Label
    Friend WithEvents lblDevice As System.Windows.Forms.Label
    Friend WithEvents cboDevices As System.Windows.Forms.ComboBox
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents btnRelease As System.Windows.Forms.Button
    Friend WithEvents lblConnection As System.Windows.Forms.Label
    Friend WithEvents cboAudioConnections As System.Windows.Forms.ComboBox
    Friend WithEvents lblDisplayMode As System.Windows.Forms.Label
    Friend WithEvents cboDisplayModes As System.Windows.Forms.ComboBox
    Friend WithEvents lblChannels As System.Windows.Forms.Label
    Friend WithEvents cboChannels As System.Windows.Forms.ComboBox
    Friend WithEvents lblSampleFormat As System.Windows.Forms.Label
    Friend WithEvents cboSampleFormats As System.Windows.Forms.ComboBox
    Friend WithEvents lblRecDir As System.Windows.Forms.Label
    Friend WithEvents txtRecordingPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents lblFilename As System.Windows.Forms.Label
    Friend WithEvents txtFilename As System.Windows.Forms.TextBox
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblDuration As System.Windows.Forms.Label
    Friend WithEvents lblFileSize As System.Windows.Forms.Label
    Friend WithEvents btnStartCapture As System.Windows.Forms.Button
    Friend WithEvents btnRecord As System.Windows.Forms.Button
    
    ' Right Panel
    Friend WithEvents pnlVuOuter As System.Windows.Forms.Panel
    Friend WithEvents lblVuHeader As System.Windows.Forms.Label
    Friend WithEvents pnlVuMeters As System.Windows.Forms.FlowLayoutPanel
    
    ' Bottom Panel
    Friend WithEvents pnlLogsOuter As System.Windows.Forms.Panel
    Friend WithEvents lblLogsHeader As System.Windows.Forms.Label
    Friend WithEvents txtLogs As System.Windows.Forms.TextBox

End Class
