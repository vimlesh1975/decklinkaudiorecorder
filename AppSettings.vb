Imports System.IO
Imports System.Text.Json

Public Class AppSettings
    Public Property DeviceName As String = ""
    Public Property AudioConnection As Long = 1 ' Default: Embedded
    Public Property DisplayMode As Long = 1215312437 ' Default: bmdModeHD1080p25
    Public Property Channels As Integer = 2
    Public Property SampleFormat As String = "16-bit Integer"
    Public Property OutputFolder As String = ""
    Public Property FilenameBase As String = "recording"

    Private Shared ReadOnly SettingsPath As String = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DeckLinkAudioRecorder",
        "settings.json"
    )

    Public Shared Function Load() As AppSettings
        Try
            If File.Exists(SettingsPath) Then
                Dim json As String = File.ReadAllText(SettingsPath)
                Return JsonSerializer.Deserialize(Of AppSettings)(json)
            End If
        Catch
        End Try
        
        Dim defaults As New AppSettings()
        defaults.OutputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeckLinkAudio")
        Return defaults
    End Function

    Public Sub Save()
        Try
            Dim dir As String = Path.GetDirectoryName(SettingsPath)
            If Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If
            
            Dim options As New JsonSerializerOptions() With {
                .WriteIndented = True
            }
            Dim json As String = JsonSerializer.Serialize(Me, options)
            File.WriteAllText(SettingsPath, json)
        Catch
        End Try
    End Sub
End Class
