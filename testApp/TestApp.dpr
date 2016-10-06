program TestApp;

uses
  Vcl.Forms,
  cpsTest in 'cpsTest.pas' {SharePointIntegration};

//utDmsSharePoint in '..\..\..\..\..\workxe3\dms_dxe2\sharepoint\utDmsSharePoint.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TSharePointIntegration, SharePointIntegration);
  Application.Run;
end.
