Imports System
Imports System.IO
Imports System.Windows.Forms

Friend Module Program
    <STAThread()>
    Friend Sub Main(args As String())
        ' Hook up crash logs
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
        AddHandler Application.ThreadException, AddressOf Application_ThreadException

        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Try
            Application.Run(New MainForm())
        Catch ex As Exception
            LogCrash(ex)
        End Try
    End Sub

    Private Sub Application_ThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
        LogCrash(e.Exception)
        Application.Exit()
    End Sub

    Private Sub CurrentDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        If TypeOf e.ExceptionObject Is Exception Then
            LogCrash(CType(e.ExceptionObject, Exception))
        Else
            LogCrashString(e.ExceptionObject?.ToString())
        End If
        Application.Exit()
    End Sub

    Private Sub LogCrash(ex As Exception)
        LogCrashString(ex?.ToString() & Environment.NewLine & ex?.StackTrace)
    End Sub

    Private Sub LogCrashString(text As String)
        Try
            Dim path As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_log.txt")
            File.WriteAllText(path, text)
        Catch
        End Try
    End Sub
End Module
