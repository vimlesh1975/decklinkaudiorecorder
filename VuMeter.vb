Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Class VuMeter
    Inherits Control

    Private _dbValue As Double = -60.0
    Private _peakDbValue As Double = -60.0
    Private _peakHoldCount As Integer = 0
    Private Const PEAK_HOLD_MAX As Integer = 15 ' Number of ticks to hold peak (~450ms)
    Private Const DECAY_DB_PER_TICK As Double = 1.2 ' Rate of peak decay

    Private WithEvents decayTimer As Timer
    Private _channelLabel As String = "CH 1"

    Public Sub New()
        ' Optimize for custom double-buffered drawing to prevent flicker
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw, True)
        
        Me.BackColor = Color.FromArgb(20, 20, 22)
        Me.ForeColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        Me.Size = New Size(300, 24)

        ' Initialize internal animation timer (30ms per tick for ~33 fps)
        decayTimer = New Timer()
        decayTimer.Interval = 30
        decayTimer.Start()
    End Sub

    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property ChannelLabel As String
        Get
            Return _channelLabel
        End Get
        Set(value As String)
            _channelLabel = value
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Sets the current audio level in dBFS (range: -60.0 to 0.0)
    ''' </summary>
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property Value As Double
        Get
            Return _dbValue
        End Get
        Set(value As Double)
            ' Clamp value between -60dB and 0dB
            _dbValue = Math.Max(-60.0, Math.Min(0.0, value))
            
            ' Update peak value
            If _dbValue >= _peakDbValue Then
                _peakDbValue = _dbValue
                _peakHoldCount = 0
            End If
            
            Me.Invalidate()
        End Set
    End Property

    Private Sub decayTimer_Tick(sender As Object, e As EventArgs) Handles decayTimer.Tick
        Dim changed As Boolean = False

        ' Handle peak hold and decay
        If _peakHoldCount < PEAK_HOLD_MAX Then
            _peakHoldCount += 1
        Else
            If _peakDbValue > -60.0 Then
                _peakDbValue = Math.Max(-60.0, _peakDbValue - DECAY_DB_PER_TICK)
                changed = True
            End If
        End If

        ' Slow level decay if no update received
        If _dbValue > -60.0 Then
            _dbValue = Math.Max(-60.0, _dbValue - 2.5)
            changed = True
        End If

        If changed Then
            Me.Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Draw dark control background
        Using bgBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(bgBrush, Me.ClientRectangle)
        End Using

        ' Draw border
        Using borderPen As New Pen(Color.FromArgb(45, 45, 48), 1)
            Dim borderRect = Me.ClientRectangle
            borderRect.Width -= 1
            borderRect.Height -= 1
            g.DrawRectangle(borderPen, borderRect)
        End Using

        ' Dimensions for drawing
        Dim labelWidth As Integer = 50
        Dim barPaddingLeft As Integer = 8
        Dim barPaddingRight As Integer = 8
        Dim barPaddingTop As Integer = 4
        Dim barPaddingBottom As Integer = 4

        ' 1. Draw Channel Label text on the left
        Dim labelRect As New Rectangle(4, 0, labelWidth, Me.Height)
        Dim sf As New StringFormat() With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Center
        }
        Using textBrush As New SolidBrush(Color.FromArgb(200, 200, 205))
            g.DrawString(_channelLabel, Me.Font, textBrush, labelRect, sf)
        End Using

        ' 2. Calculate drawing bounds for the LED level bar
        Dim barX As Integer = labelWidth + barPaddingLeft
        Dim barY As Integer = barPaddingTop
        Dim barWidth As Integer = Me.Width - barX - barPaddingRight
        Dim barHeight As Integer = Me.Height - (barPaddingTop + barPaddingBottom)

        If barWidth <= 10 Then Return

        ' Draw LED Bar Background
        Dim barRect As New Rectangle(barX, barY, barWidth, barHeight)
        Using ledBgBrush As New SolidBrush(Color.FromArgb(10, 10, 12))
            g.FillRectangle(ledBgBrush, barRect)
        End Using

        ' Map dB values to percentages
        Dim fillPct As Double = (Value - (-60.0)) / 60.0
        Dim peakPct As Double = (_peakDbValue - (-60.0)) / 60.0

        ' We use a segmented hardware LED design (e.g., 30 segments)
        Dim segmentCount As Integer = 32
        Dim spacing As Integer = 2
        Dim totalSpacingWidth As Integer = spacing * (segmentCount - 1)
        Dim segmentWidth As Double = (barWidth - totalSpacingWidth) / segmentCount

        For i As Integer = 0 To segmentCount - 1
            Dim segX As Integer = barX + CInt(i * (segmentWidth + spacing))
            Dim segWidthInt As Integer = CInt(segmentWidth)
            If segWidthInt < 1 Then segWidthInt = 1

            Dim segRect As New Rectangle(segX, barY, segWidthInt, barHeight)
            Dim segCenterPct As Double = (i + 0.5) / segmentCount

            ' Determine color category of this segment
            Dim segColor As Color
            If segCenterPct < 0.7 Then
                ' Safe levels (Cyan)
                segColor = Color.FromArgb(0, 180, 216)
            ElseIf segCenterPct < 0.9 Then
                ' Warning levels (Yellow)
                segColor = Color.FromArgb(255, 183, 3)
            Else
                ' Peak warning (Magenta/Red)
                segColor = Color.FromArgb(224, 30, 90)
            End If

            ' Draw segment
            If segCenterPct <= fillPct Then
                ' Active segment: fully lit
                Using activeBrush As New SolidBrush(segColor)
                    g.FillRectangle(activeBrush, segRect)
                End Using
            Else
                ' Inactive segment: dim, dark background slot
                Dim dimColor As Color = Color.FromArgb(35, segColor.R \ 8, segColor.G \ 8, segColor.B \ 8)
                Using dimBrush As New SolidBrush(dimColor)
                    g.FillRectangle(dimBrush, segRect)
                End Using
            End If
        Next

    ' 3. Draw Peak Indicator Pin
    If peakPct > 0.0 AndAlso peakPct <= 1.0 Then
        Dim peakIndex As Integer = CInt(Math.Round(peakPct * segmentCount)) - 1
        If peakIndex < 0 Then peakIndex = 0
        If peakIndex >= segmentCount Then peakIndex = segmentCount - 1

        Dim peakX As Integer = barX + CInt(peakIndex * (segmentWidth + spacing))
        Dim peakWidthInt As Integer = CInt(segmentWidth)
        If peakWidthInt < 1 Then peakWidthInt = 1

        Dim peakRect As New Rectangle(peakX, barY, peakWidthInt, barHeight)
        
        Dim peakColor As Color
        Dim peakCenterPct As Double = (peakIndex + 0.5) / segmentCount
        If peakCenterPct < 0.7 Then
            peakColor = Color.FromArgb(144, 224, 239)
        ElseIf peakCenterPct < 0.9 Then
            peakColor = Color.FromArgb(252, 213, 129)
        Else
            peakColor = Color.FromArgb(255, 128, 160)
        End If

        Using peakBrush As New SolidBrush(peakColor)
            g.FillRectangle(peakBrush, peakRect)
        End Using
    End If
End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If decayTimer IsNot Nothing Then
                decayTimer.Stop()
                decayTimer.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class
