# DeckLink Audio Recorder

![DeckLink Audio Recorder Screenshot](Screenshot%202026-07-08%20134737.png)

**DeckLink Audio Recorder** is a professional, high-performance Windows desktop application built in VB.NET. It interfaces directly with **Blackmagic Design DeckLink** hardware capture cards using the official DeckLink COM SDK to capture, monitor, and record multi-channel broadcast-grade audio.

---

## 🌟 Key Features

*   **Multi-Channel Audio Ingestion:** Support for capturing 2, 8, or 16 channels of synchronized audio.
*   **Flexible Sample Bit Depths:** Supports both 16-bit and 32-bit Integer PCM audio sample formats (48 kHz sample rate).
*   **Live Audio VU Level Meters:** High-performance, double-buffered level meters for each channel featuring realistic Peak-Hold indicators and dbFS scale decay animations.
*   **Dynamic Hardware Scanning:** Automatic detection of installed DeckLink devices, input connection types (Embedded SDI/HDMI, Analog, AES/EBU, XLR, RCA, Microphone), and video display modes/clocks.
*   **WAV File Exporter:** Fast, non-blocking disk writing with automatic header patching to output valid, uncompressed RIFF/WAVE files.
*   **Persistent Configuration:** Automatically saves and loads settings (device selection, connection type, video format, channels, bit-depth, destination directories, etc.) using JSON serialization.
*   **Automated Deployment pipeline:** Embedded MSBuild targets stop any running processes, clean up older compiled binaries, copy the output into a timestamped file format (e.g., `DeckLinkAudioRecorder_ddMMyy_HHmmss.exe`), and clean up the original compiler output file.
*   **Global Exception Handling:** Comprehensive system-wide diagnostic monitoring that dumps unexpected failures into a localized `crash_log.txt`.

---

## 🏗️ Codebase Architecture

The project consists of the following key files:

| File | Component | Description |
| :--- | :--- | :--- |
| **[Program.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/Program.vb)** | Application Entry Point | Sets up DPI-awareness, configures WinForms visual styles, and attaches global thread/domain exception handling hooks to write `crash_log.txt`. |
| **[MainForm.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/MainForm.vb)** | Main UI Form Logic | Controls the layout, translates settings/COM inputs to GUI components, handles button clicks, drives progress clocks, and connects the audio capture event loops to the level meters. |
| **[MainForm.Designer.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/MainForm.Designer.vb)** | UI Form Layout | Houses all programmatic visual hierarchy, control layouts, and designer settings for a clean dark-themed interface. |
| **[DeckLinkAudioRecorder.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/DeckLinkAudioRecorder.vb)** | Hardware / SDK Interface | Implements the COM class interface `IDeckLinkInputCallback`. Manages streaming buffers, controls hardware locks, configures audio input/video clock references, and fires events with raw data/peaks. |
| **[WavWriter.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/WavWriter.vb)** | WAV File Handler | Creates output file streams, writes initial PCM headers, streams live audio packets to disk, and updates header size fields on closing. |
| **[VuMeter.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/VuMeter.vb)** | Custom VU Control | A double-buffered `Control` that performs dynamic graphics drawing of level bars using smooth anti-aliased gradients, decay timer animation ticks, and peak indicator pins. |
| **[AppSettings.vb](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/AppSettings.vb)** | Configuration Manager | Uses `System.Text.Json` to save and load runtime options inside local user application data (`%localappdata%/DeckLinkAudioRecorder/settings.json`). |
| **[decklinkaudiorecorder.vbproj](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/decklinkaudiorecorder.vbproj)** | Build Project File | Declares dependencies, references the SDK interop DLL, and encapsulates target compilation cleanup routines. |
| **[build_and_run.ps1](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/build_and_run.ps1)** | Build Automation Script | Runs a clean compilation pipeline using the `dotnet` CLI, creating a fresh timestamped executable execution on launch. |

---

## 🚀 Getting Started

### Prerequisites

1.  **Blackmagic Desktop Video Drivers:** Install Desktop Video from [Blackmagic Support](https://www.blackmagicdesign.com/support/). The drivers expose the core COM APIs.
2.  **.NET 10.0 Windows SDK:** Make sure you have the .NET 10.0 SDK installed on your system.

### Build and Launch

To compile the application and launch it immediately, run the following command in a PowerShell terminal:

```powershell
.\build_and_run.ps1
```

During execution, the custom MSBuild targets configured inside **[decklinkaudiorecorder.vbproj](file:///c:/Users/vimlesh/Documents/vimlesh/decklinkaudiorecorder/decklinkaudiorecorder.vbproj)** will automatically:
1.  Check for and force-close any running instances of the app.
2.  Compile the latest source changes using incremental compiler routines.
3.  Copy the compiled output `decklinkaudiorecorder.exe` to a new timestamped file (e.g. `DeckLinkAudioRecorder_080726_134416.exe`).
4.  **Always delete** the original `decklinkaudiorecorder.exe` file to prevent duplicate binary confusion.
5.  Clean up older dated copies in the build output folder.
6.  Launch the newly generated timestamped executable.
