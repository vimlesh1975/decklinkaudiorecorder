Imports System.Runtime.InteropServices
Imports DeckLinkAPI

Public Class DeckLinkAudioRecorder
    Implements IDeckLinkInputCallback
    Implements IDisposable

    ' Events for communication with the MainForm UI
    Public Event LogReceived(message As String)
    Public Event AudioSamplesCaptured(rawBytes() As Byte, length As Integer, peakLevels() As Double)

    Private deckLink As IDeckLink
    Private deckLinkInput As IDeckLinkInput
    Private isCapturing As Boolean = False
    Private isRecording As Boolean = False
    Private wavWriter As WavWriter
    Private channelCount As Integer = 2
    Private sampleType As _BMDAudioSampleType = _BMDAudioSampleType.bmdAudioSampleType16bitInteger
    Private disposedValue As Boolean = False

    ''' <summary>
    ''' Enumerates all available DeckLink input devices on the system.
    ''' </summary>
    Public Shared Function EnumerateDevices() As List(Of String)
        Dim list As New List(Of String)
        Dim iterator As CDeckLinkIteratorClass = Nothing
        Try
            iterator = New CDeckLinkIteratorClass()
            Do
                Dim device As IDeckLink = Nothing
                Try
                    iterator.Next(device)
                Catch
                    Exit Do
                End Try
                
                If device Is Nothing Then Exit Do
                
                Dim displayName As String = Nothing
                device.GetDisplayName(displayName)
                If Not String.IsNullOrEmpty(displayName) Then
                    list.Add(displayName)
                End If
                Marshal.ReleaseComObject(device)
            Loop
        Catch ex As Exception
            ' If driver is not installed or COM initialization fails, return empty list
        Finally
            If iterator IsNot Nothing Then
                Marshal.ReleaseComObject(iterator)
            End If
        End Try
        Return list
    End Function

    ''' <summary>
    ''' Queries the supported video display modes for the specified device.
    ''' </summary>
    Public Function GetDisplayModes(deviceName As String) As List(Of (Mode As _BMDDisplayMode, Name As String))
        Dim list As New List(Of (Mode As _BMDDisplayMode, Name As String))
        Dim targetDevice As IDeckLink = FindDevice(deviceName)
        If targetDevice Is Nothing Then Return list

        Dim input As IDeckLinkInput = Nothing
        Try
            input = CType(targetDevice, IDeckLinkInput)
            Dim modeIterator As IDeckLinkDisplayModeIterator = Nothing
            input.GetDisplayModeIterator(modeIterator)
            If modeIterator IsNot Nothing Then
                Do
                    Dim displayMode As IDeckLinkDisplayMode = Nothing
                    modeIterator.Next(displayMode)
                    If displayMode Is Nothing Then Exit Do

                    Dim modeName As String = Nothing
                    displayMode.GetName(modeName)
                    Dim m As _BMDDisplayMode = displayMode.GetDisplayMode()
                    list.Add((m, modeName))
                    Marshal.ReleaseComObject(displayMode)
                Loop
                Marshal.ReleaseComObject(modeIterator)
            End If
        Catch ex As Exception
            RaiseEvent LogReceived($"Error enumerating display modes: {ex.Message}")
        Finally
            If input IsNot Nothing Then Marshal.ReleaseComObject(input)
            Marshal.ReleaseComObject(targetDevice)
        End Try
        Return list
    End Function

    Private Shared Function FindDevice(deviceName As String) As IDeckLink
        Dim iterator As CDeckLinkIteratorClass = Nothing
        Try
            iterator = New CDeckLinkIteratorClass()
            Do
                Dim device As IDeckLink = Nothing
                Try
                    iterator.Next(device)
                Catch
                    Exit Do
                End Try
                
                If device Is Nothing Then Exit Do
                
                Dim displayName As String = Nothing
                device.GetDisplayName(displayName)
                If String.Equals(displayName, deviceName, StringComparison.OrdinalIgnoreCase) Then
                    Return device
                End If
                Marshal.ReleaseComObject(device)
            Loop
        Catch
        Finally
            If iterator IsNot Nothing Then Marshal.ReleaseComObject(iterator)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Starts capturing audio and video (video is required by SDK to clock the device).
    ''' </summary>
    Public Sub StartCapture(deviceName As String,
                            connection As _BMDAudioConnection,
                            displayMode As _BMDDisplayMode,
                            sampleRate As _BMDAudioSampleRate,
                            samplesPerSampleType As _BMDAudioSampleType,
                            channels As Integer)
        If isCapturing Then StopCapture()

        deckLink = FindDevice(deviceName)
        If deckLink Is Nothing Then
            Throw New InvalidOperationException($"DeckLink device '{deviceName}' not found.")
        End If

        deckLinkInput = CType(deckLink, IDeckLinkInput)

        ' Try to configure the audio input connection
        Try
            Dim config As IDeckLinkConfiguration = CType(deckLink, IDeckLinkConfiguration)
            config.SetInt(_BMDDeckLinkConfigurationID.bmdDeckLinkConfigAudioInputConnection, CType(connection, Long))
            RaiseEvent LogReceived($"Audio input connection configured: {connection}")
        Catch ex As Exception
            RaiseEvent LogReceived($"Audio connection configuration warning (using driver default): {ex.Message}")
        End Try

        channelCount = channels
        sampleType = samplesPerSampleType

        ' Set this class as the callback handler
        deckLinkInput.SetCallback(Me)

        ' Enable Video Input - Required because DeckLink clocks audio sampling rate from the video reference signal
        Try
            deckLinkInput.EnableVideoInput(displayMode, _BMDPixelFormat.bmdFormat8BitYUV, _BMDVideoInputFlags.bmdVideoInputFlagDefault)
            RaiseEvent LogReceived($"Video input enabled with display mode: {displayMode}")
        Catch ex As Exception
            RaiseEvent LogReceived($"Failed to enable video input: {ex.Message}. Falling back to PAL mode.")
            deckLinkInput.EnableVideoInput(_BMDDisplayMode.bmdModePAL, _BMDPixelFormat.bmdFormat8BitYUV, _BMDVideoInputFlags.bmdVideoInputFlagDefault)
        End Try

        ' Enable Audio Input
        deckLinkInput.EnableAudioInput(sampleRate, samplesPerSampleType, CUInt(channels))
        RaiseEvent LogReceived($"Audio input enabled: {sampleRate} Hz, {samplesPerSampleType} bit, {channels} channels.")

        ' Start streaming
        deckLinkInput.StartStreams()
        isCapturing = True
        RaiseEvent LogReceived("DeckLink capture streams started successfully.")
    End Sub

    ''' <summary>
    ''' Stops the active capture streams.
    ''' </summary>
    Public Sub StopCapture()
        StopRecording()
        isCapturing = False

        If deckLinkInput IsNot Nothing Then
            Try
                deckLinkInput.StopStreams()
                deckLinkInput.SetCallback(Nothing)
                deckLinkInput.DisableAudioInput()
                deckLinkInput.DisableVideoInput()
            Catch ex As Exception
                RaiseEvent LogReceived($"Error during stream stop: {ex.Message}")
            Finally
                Marshal.ReleaseComObject(deckLinkInput)
                deckLinkInput = Nothing
            End Try
        End If

        If deckLink IsNot Nothing Then
            Try
                Marshal.ReleaseComObject(deckLink)
            Catch
            Finally
                deckLink = Nothing
            End Try
        End If

        RaiseEvent LogReceived("DeckLink capture streams stopped.")
    End Sub

    ''' <summary>
    ''' Starts recording incoming audio to a WAV file.
    ''' </summary>
    Public Sub StartRecording(filePath As String)
        If Not isCapturing Then
            Throw New InvalidOperationException("Cannot record audio when capture streams are not running.")
        End If

        Dim bitsPerSample As Integer = If(sampleType = _BMDAudioSampleType.bmdAudioSampleType16bitInteger, 16, 32)
        wavWriter = New WavWriter(filePath, 48000, bitsPerSample, channelCount)
        isRecording = True
        RaiseEvent LogReceived($"Recording started to WAV file: '{filePath}'")
    End Sub

    ''' <summary>
    ''' Stops writing audio to the WAV file.
    ''' </summary>
    Public Sub StopRecording()
        If isRecording Then
            isRecording = False
            Dim writer = wavWriter
            wavWriter = Nothing
            If writer IsNot Nothing Then
                Dim bytesRecorded As Long = writer.RecordedBytes
                writer.Close()
                RaiseEvent LogReceived($"Recording stopped. Saved {FormatBytes(bytesRecorded)} to disk.")
            End If
        End If
    End Sub

    Public ReadOnly Property IsCapturingActive As Boolean
        Get
            Return isCapturing
        End Get
    End Property

    Public ReadOnly Property IsRecordingActive As Boolean
        Get
            Return isRecording
        End Get
    End Property

    Public ReadOnly Property RecordedBytesCount As Long
        Get
            If wavWriter IsNot Nothing Then
                Return wavWriter.RecordedBytes
            End If
            Return 0
        End Get
    End Property

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

    ' --- IDeckLinkInputCallback Implementation ---

    Public Sub VideoInputFrameArrived(videoFrame As IDeckLinkVideoInputFrame, audioPacket As IDeckLinkAudioInputPacket) Implements IDeckLinkInputCallback.VideoInputFrameArrived
        Try
            If audioPacket IsNot Nothing Then
                Dim sampleFrameCount As Integer = audioPacket.GetSampleFrameCount()
                If sampleFrameCount > 0 Then
                    Dim bufferPtr As IntPtr = IntPtr.Zero
                    audioPacket.GetBytes(bufferPtr)

                    Dim bytesPerSample As Integer = If(sampleType = _BMDAudioSampleType.bmdAudioSampleType16bitInteger, 2, 4)
                    Dim byteCount As Integer = sampleFrameCount * channelCount * bytesPerSample

                    Dim rawBytes(byteCount - 1) As Byte
                    Marshal.Copy(bufferPtr, rawBytes, 0, byteCount)

                    ' Calculate live peaks
                    Dim peaks() As Double = ComputePeaks(rawBytes, sampleFrameCount, channelCount, sampleType)

                    ' Write to disk if recording
                    Dim currentWriter = wavWriter
                    If isRecording AndAlso currentWriter IsNot Nothing Then
                        Try
                            currentWriter.WriteSamples(rawBytes, byteCount)
                        Catch ex As Exception
                            isRecording = False
                            RaiseEvent LogReceived($"WAV Recording failed: {ex.Message}")
                        End Try
                    End If

                    ' Raise callback events back to Form GUI
                    RaiseEvent AudioSamplesCaptured(rawBytes, byteCount, peaks)
                End If
            End If
        Catch ex As Exception
            ' Callback runs on high priority real-time thread, handle errors safely
        Finally
            ' COM Object cleanup is critical to avoid Native leaks
            If videoFrame IsNot Nothing Then
                Marshal.ReleaseComObject(videoFrame)
            End If
            If audioPacket IsNot Nothing Then
                Marshal.ReleaseComObject(audioPacket)
            End If
        End Try
    End Sub

    Public Sub VideoInputFormatChanged(notificationEvents As _BMDVideoInputFormatChangedEvents, newDisplayMode As IDeckLinkDisplayMode, detectedSignalFlags As _BMDDetectedVideoInputFormatFlags) Implements IDeckLinkInputCallback.VideoInputFormatChanged
        Dim name As String = Nothing
        If newDisplayMode IsNot Nothing Then
            newDisplayMode.GetName(name)
            RaiseEvent LogReceived($"Video signal format change detected: {name}")
        End If
    End Sub

    ''' <summary>
    ''' Computes the peak amplitude per channel in decibels relative to full scale (dBFS).
    ''' Clamps bottom values to -60.0 dBFS.
    ''' </summary>
    Private Shared Function ComputePeaks(rawBytes() As Byte, sampleCount As Integer, channelCount As Integer, sampleType As _BMDAudioSampleType) As Double()
        Dim peaks(channelCount - 1) As Double
        For i As Integer = 0 To channelCount - 1
            peaks(i) = -60.0
        Next

        If rawBytes Is Nothing OrElse rawBytes.Length = 0 Then Return peaks

        If sampleType = _BMDAudioSampleType.bmdAudioSampleType16bitInteger Then
            Dim maxVals(channelCount - 1) As Short
            For s As Integer = 0 To sampleCount - 1
                For c As Integer = 0 To channelCount - 1
                    Dim offset As Integer = (s * channelCount + c) * 2
                    If offset + 1 < rawBytes.Length Then
                        Dim sample As Short = BitConverter.ToInt16(rawBytes, offset)
                        Dim absSample As Short = If(sample = Short.MinValue, Short.MaxValue, Math.Abs(sample))
                        If absSample > maxVals(c) Then maxVals(c) = absSample
                    End If
                Next
            Next
            For c As Integer = 0 To channelCount - 1
                If maxVals(c) > 0 Then
                    Dim dBFS As Double = 20.0 * Math.Log10(CDbl(maxVals(c)) / 32768.0)
                    peaks(c) = Math.Max(-60.0, Math.Min(0.0, dBFS))
                End If
            Next
        Else
            Dim maxVals(channelCount - 1) As Integer
            For s As Integer = 0 To sampleCount - 1
                For c As Integer = 0 To channelCount - 1
                    Dim offset As Integer = (s * channelCount + c) * 4
                    If offset + 3 < rawBytes.Length Then
                        Dim sample As Integer = BitConverter.ToInt32(rawBytes, offset)
                        Dim absSample As Integer = If(sample = Integer.MinValue, Integer.MaxValue, Math.Abs(sample))
                        If absSample > maxVals(c) Then maxVals(c) = absSample
                    End If
                Next
            Next
            For c As Integer = 0 To channelCount - 1
                If maxVals(c) > 0 Then
                    Dim dBFS As Double = 20.0 * Math.Log10(CDbl(maxVals(c)) / 2147483648.0)
                    peaks(c) = Math.Max(-60.0, Math.Min(0.0, dBFS))
                End If
            Next
        End If

        Return peaks
    End Function

    ' --- IDisposable Implementation ---

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                StopCapture()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
