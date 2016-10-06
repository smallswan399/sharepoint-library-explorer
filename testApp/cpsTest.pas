unit cpsTest;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.StdCtrls, Vcl.Samples.Spin,
  Vcl.ComCtrls;

type
  TSharePointIntegration = class(TForm)
    memInfo: TMemo;
    pgControl: TPageControl;
    tsBasic: TTabSheet;
    btnConfig: TButton;
    btnDetect: TButton;
    edtSpin: TSpinEdit;
    btnDotDot: TButton;
    tsDownload: TTabSheet;
    btnLoadFromDMS: TButton;
    edtDownload: TEdit;
    tsUpload: TTabSheet;
    edtURL: TEdit;
    cbSelect: TCheckBox;
    edtFile2: TEdit;
    btnGetFile: TButton;
    btnSaveDisk: TButton;
    lbl1: TLabel;
    lbl2: TLabel;
    lbl3: TLabel;
    btnDotDot2: TButton;
    dlgOpen: TOpenDialog;
    procedure btnConfigClick(Sender: TObject);
    procedure btnDetectClick(Sender: TObject);
    procedure btnLoadFromDMSClick(Sender: TObject);
    procedure btnGetFileClick(Sender: TObject);
    procedure btnSaveDiskClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure btnDotDotClick(Sender: TObject);
    procedure btnDotDot2Click(Sender: TObject);
  private
  public
    sSharePointApi: string;
  end;

var
  SharePointIntegration: TSharePointIntegration;

implementation
uses
  ShellApi,
  ShlObj,
  ActiveX,

  ComObj,
  Registry,
  cpsInstaller,
  cpsGlobal;

const
  shp_config = 'config';
  //
  shp_loadfromdms = 'loadfromdms ';
  shp_download = 'downloadfromdms ';
  shp_savenewprofile = 'savenewprofile ';
  shp_savenewversion = 'savenewversion ';
  shp_getallversions = 'getallversions';
  //
  shp_reg = 'Software\Litera2\SharePoint';
  shp_dialogtitle = 'dialogtitle';
  shp_selecttwo = 'bselecttwo';
  shp_iresult = 'iresult';
  shp_url = 'url';
  shp_urldn = 'urldn';
  shp_urldm = 'urldm';
  shp_upload1 = 'upload1';
  shp_upload2 = 'upload2';
  shp_upload3 = 'upload3';

{$R *.dfm}

function SharePointPath: string;
begin
  Result :=
  //  'c:\Users\Administrator\ssptlib2\1bind\lcp_dmsst.exe';
  csFiles.GetProgramFilesShort + 'litera\sharepoint\lcp_dmsst.exe';
end;

function cpGetAppDir: string;
var
  idl: PItemIDList;
begin
  SHGetSpecialFolderLocation(0, CSIDL_LOCAL_APPDATA, IDL);
  try
    SetLength(Result, MAX_PATH);
    SHGetPathFromIDListW(IDL, PWideChar(Result));
  finally
    CoTaskMemFree(IDL);
  end;
  Result := PWideChar(Result);
  Result := IncludeTrailingPathDelimiter(Result);
end;

function GetDmsCheckOutDir(): string;
const
  core_DMSDOC = 'Litera\lcpdmsdoc\';
  //core_TEMP = 'Litera\temp\';
begin
  Result := IncludeTrailingPathDelimiter(cpGetAppDir()) + core_DMSDOC;
  if DirectoryExists(Result) = false then
    ForceDirectories(Result);
  //if not WideForceDirectories(Result) then
  //  Result := IncludeTrailingPathDelimiter(cpGetAppDir()) + core_TEMP;
end;

procedure TSharePointIntegration.FormCreate(Sender: TObject);
begin
  memInfo.Lines.Clear;
  btnDotDotClick(Sender);
  pgControl.ActivePageIndex := 0;
end;

procedure TSharePointIntegration.btnDotDot2Click(Sender: TObject);
begin
  if (dlgOpen.Execute) then
  begin
    edtFile2.Text := dlgOpen.FileName;
  end;
end;

procedure TSharePointIntegration.btnDotDotClick(Sender: TObject);
begin
  edtSpin.Value := Random(10000);
end;

procedure TSharePointIntegration.btnDetectClick(Sender: TObject);
var
  bResult: Boolean;
begin
  sSharePointApi := SharePointPath;
  bResult := FileExists(sSharePointApi); // This is web-service
  memInfo.Lines.Add('--------------------------------------');
  memInfo.Lines.Add('InstalledPath: ' + sSharePointApi);
  if (bResult) then
  begin
    memInfo.Lines.Add('Installed');
  end else begin
    memInfo.Lines.Add('Not Installed');
  end;
end;

procedure TSharePointIntegration.btnConfigClick(Sender: TObject);
begin
  sSharePointApi := SharePointPath;
  memInfo.Lines.Add('--------------------------------------');
  memInfo.Lines.Add('InstalledPath: ' + sSharePointApi);
  if FileExists(sSharePointApi) = false then
  begin
    memInfo.Lines.Add('Not Installed');
  end else
  begin
    csInstall.ExecAndWait(sSharePointApi, shp_config);
  end;
end;

procedure TSharePointIntegration.btnLoadFromDMSClick(Sender: TObject);
var
  sFileName1, sFileName2: string;
  sDialogTitle: string; bSelectTwo: Boolean;
  sPath: string;
  sParam: string;
  reg: TRegistry;
  iResult: integer;
  iSession: Integer;
begin
  iSession := edtSpin.Value;
  iResult := 0;
  sPath := SharePointPath;
  //
  memInfo.Lines.Add('--------------------------------------');
  bSelectTwo := cbSelect.Checked;

  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  reg.WriteString(shp_dialogtitle, sDialogTitle);
  reg.WriteBool(shp_selecttwo, bSelectTwo);
  reg.Free;
  //
  sParam := shp_loadfromdms + IntToStr(iSession);
  csInstall.ExecAndWait(sPath, sParam);
  //
  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  if reg.ValueExists(shp_iresult) then
    iResult := reg.ReadInteger(shp_iresult);
  if reg.ValueExists(shp_url) then
  begin
    if bSelectTwo then
    begin
      sFilename2 := reg.ReadString(shp_url);
    end else begin
      sFilename1 := reg.ReadString(shp_url);
    end;
  end;
  reg.CloseKey;
  //reg.DeleteKey(shp_reg + '\' + IntToStr(iSession));
  reg.Free;
  //
  //if FileExists(sFilename1) = false then
  //  memInfo.Lines.Add('Cancel')
  //else
  //  memInfo.Lines.Add('URL: ' + sFilename1);
  if (bSelectTwo) then
  begin
    edtURL.Text := sFilename2;
  end else begin
    edtURL.Text := sFilename1;
  end;
end;

procedure TSharePointIntegration.btnGetFileClick(Sender: TObject);
var
  iSession: Integer;
  reg: TRegistry;
  sFilename,
  sPath,
    sParam: string;
  sDocID: string;
begin
  iSession := edtSpin.Value;
  sPath := SharePointPath;
  sDocID := edtURL.Text;
  //
  memInfo.Lines.Add('--------------------------------------');
  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  reg.WriteString(shp_url, sDocID);
  reg.WriteString(shp_urldn, GetDmsCheckOutDir());
  reg.Free;
  //
  sParam := shp_download + IntToStr(iSession);
  memInfo.Lines.Add('Session/Params: ' + sParam);
  csInstall.ExecAndWait(sPath, sParam);
  //
  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  if reg.ValueExists(shp_urldm) then
  begin
    sFilename := reg.ReadString(shp_urldm);
  end;
  reg.CloseKey;
  //reg.DeleteKey(shp_reg + '\' + IntToStr(iSession));
  reg.Free;
  //
  if FileExists(sFilename) = false then
    memInfo.Lines.Add('Cancel')
  else
    memInfo.Lines.Add('NO_Error' + shp_urldm);
  edtDownload.Text := shp_urldm;
end;

procedure TSharePointIntegration.btnSaveDiskClick(Sender: TObject);
var
  iResult: integer;
  reg: TRegistry;
  sPath, sParam: string;
  iSession: Integer;
  pFilename: string;
  pOrg,
  pMod: string;
begin
  iSession := edtSpin.Value;
  iResult := 0;
  sPath := SharePointPath;

  memInfo.Lines.Add('--------------------------------------');
  pFilename := edtFile2.Text;

  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  reg.WriteString(shp_upload1, pFileName);
  reg.WriteString(shp_upload2, pOrg);
  reg.WriteString(shp_upload3, pMod);
  reg.Free;
  //
  sParam := shp_savenewprofile + IntToStr(iSession);
  csInstall.ExecAndWait(sPath, sParam);
  //
  reg := TRegistry.Create(KEY_READ or KEY_WRITE);
  reg.OpenKey(shp_reg + '\' + IntToStr(iSession), true);
  if (reg.ValueExists(shp_iresult)) then
    iResult := reg.ReadInteger(shp_iresult);
  reg.CloseKey;
  //reg.DeleteKey(shp_reg + '\' + IntToStr(iSession));
  reg.Free;
  //Result := iResult;
end;

end.
