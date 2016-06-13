#define ApplicationVersion GetFileVersion('..\Binaries\Release\Gwen.dll')

[Setup]
AppName=Gwen.Net Extended Layout
AppVersion={#ApplicationVersion}
AppVerName=Gwen.Net Extended Layout v{#ApplicationVersion}
DefaultDirName={pf}\GwenNetEx
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\Gwen.XmlDesigner.OpenTK.exe
SourceDir=..\Binaries\Release
OutputDir=Output

[Files]
Source: "Gwen.XmlDesigner.OpenTK.exe"; DestDir: "{app}"
Source: "Gwen.dll"; DestDir: "{app}"
Source: "Gwen.xml"; DestDir: "{app}"
Source: "Gwen.Platform.Windows.dll"; DestDir: "{app}"
Source: "Gwen.Renderer.OpenTK.dll"; DestDir: "{app}"
Source: "Gwen.XmlDesigner.dll"; DestDir: "{app}"
Source: "OpenTK.dll"; DestDir: "{app}"
Source: "OpenTK.dll.config"; DestDir: "{app}"
Source: "Skins\*"; DestDir: "{app}\Skins"

[Icons]
Name: "{commonprograms}\GwenNetEx"; Filename: "{app}\Gwen.XmlDesigner.OpenTK.exe"
