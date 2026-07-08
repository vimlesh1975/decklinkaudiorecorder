Imports System.IO

Public Class WavWriter
    Implements IDisposable

    Private fs As FileStream
    Private bw As BinaryWriter
    Private totalDataBytes As Long = 0
    Private isDisposed As Boolean = False

    Public Sub New(filePath As String, sampleRate As Integer, bitsPerSample As Integer, channels As Integer)
        fs = New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        bw = New BinaryWriter(fs)

        ' Write WAV header placeholder (44 bytes)
        bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"))
        bw.Write(CInt(0)) ' ChunkSize (file size - 8) - updated on Close
        bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"))
        bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "))
        bw.Write(CInt(16)) ' Subchunk1Size (16 for PCM)
        bw.Write(CUShort(1)) ' AudioFormat (1 = PCM)
        bw.Write(CUShort(channels))
        bw.Write(sampleRate)

        Dim bytesPerSample As Integer = bitsPerSample \ 8
        Dim byteRate As Integer = sampleRate * channels * bytesPerSample
        Dim blockAlign As UShort = CUShort(channels * bytesPerSample)

        bw.Write(byteRate)
        bw.Write(blockAlign)
        bw.Write(CUShort(bitsPerSample))
        bw.Write(System.Text.Encoding.ASCII.GetBytes("data"))
        bw.Write(CInt(0)) ' Subchunk2Size (data size) - updated on Close
    End Sub

    Public Sub WriteSamples(data() As Byte, length As Integer)
        If isDisposed Then Throw New ObjectDisposedException(NameOf(WavWriter))
        bw.Write(data, 0, length)
        totalDataBytes += length
    End Sub

    Public Sub Close()
        If Not isDisposed Then
            Try
                ' Seek and write the actual sizes
                fs.Seek(4, SeekOrigin.Begin)
                bw.Write(CInt(totalDataBytes + 36)) ' ChunkSize

                fs.Seek(40, SeekOrigin.Begin)
                bw.Write(CInt(totalDataBytes)) ' Subchunk2Size
            Finally
                bw.Close()
                fs.Close()
                bw.Dispose()
                fs.Dispose()
                isDisposed = True
            End Try
        End If
    End Sub

    Public ReadOnly Property RecordedBytes As Long
        Get
            Return totalDataBytes
        End Get
    End Property

    Public Sub Dispose() Implements IDisposable.Dispose
        Close()
    End Sub
End Class
