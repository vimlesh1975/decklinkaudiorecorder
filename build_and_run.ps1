# build_and_run.ps1
# Compile the project. The MSBuild targets inside decklinkaudiorecorder.vbproj
# will automatically stop any running instances before compiling and launch
# the newly generated timestamped executable afterwards.
#
# By not deleting bin/obj, .NET will perform incremental compilation, only
# rebuilding files that have changed and replacing what is new, speeding up builds.
Write-Host "Compiling project and launching application..." -ForegroundColor Yellow
dotnet build decklinkaudiorecorder.vbproj -c Release -p:Platform=x64
