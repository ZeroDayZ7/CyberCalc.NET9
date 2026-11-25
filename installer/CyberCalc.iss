#ifndef AppVersion
#error "AppVersion must be passed to ISCC: /DAppVersion=1.2.3"
#endif
#ifndef SourceDir
#error "SourceDir must be passed to ISCC"
#endif
#ifndef OutputDir
#error "OutputDir must be passed to ISCC"
#endif

#define AppName "CyberCalc.NET9"
#define AppPublisher "Skyline Systems"
#define AppExeName "WPF-CALC-NET-9.exe"

#ifndef NumericVersion
#define NumericVersion {#AppVersion}
#endif

[Setup]
AppId={{A8D14E72-8B5B-4E44-9E64-77B2F20F8A31}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL=https://github.com/ZeroDayZ7/CyberCalc.NET9
AppSupportURL=https://github.com/ZeroDayZ7/CyberCalc.NET9/issues
DefaultDirName={localappdata}\Programs\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=no
PrivilegesRequired=lowest
OutputDir={#OutputDir}
OutputBaseFilename=CyberCalc-Setup-{#AppVersion}-x64
SetupIconFile=..\CyberCalc.NET9\Assets\app_icon.ico
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#AppExeName}
VersionInfoVersion={#NumericVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription=CyberCalc.NET9 Calculator
VersionInfoCopyright=Copyright © 2025 Skyline Systems

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon; IconFilename: "{app}\{#AppExeName}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent

[InstallDelete]
Type: filesandordirs; Name: "{app}"
