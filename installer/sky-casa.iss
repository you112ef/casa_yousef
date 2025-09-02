; Inno Setup script for Sky CASA
#define MyAppName "Sky CASA"
#define MyAppVersion "3.0"
#define MyAppPublisher "Sky CASA Project"
#define MyAppExeName "Sky_CASA.exe"

; BuildDir is passed from CI. Fallback to local bin path.
#ifndef BuildDir
  #define BuildDir "bin\\Release\\net472"
#endif

[Setup]
AppId={{B6A4F82D-3A55-4AE1-B6F3-0C0A42E1FBA9}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=no
DisableProgramGroupPage=yes
OutputBaseFilename=Sky-CASA-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
WizardStyle=modern

[Languages]
Name: "arabic"; MessagesFile: "compiler:Languages\\Arabic.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; Main application files
Source: "{#BuildDir}\\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; AI analysis scripts (if present in repo next to build dir)
Source: "ai_sperm_analysis\\*"; DestDir: "{app}\\ai_sperm_analysis"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
; OFFLINE ONNX shim + models
Source: "ai_sperm_onnx\\python.exe"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "ai_sperm_onnx\\models\\*"; DestDir: "{app}\\ai_sperm_onnx\\models"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
; Config and database if present
Source: "database.db"; DestDir: "{app}"; Flags: onlyifdoesntexist ignoreversion skipifsourcedoesntexist
Source: "Sky_CASA.exe.config"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\\backups"