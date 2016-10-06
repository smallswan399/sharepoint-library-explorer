object SharePointIntegration: TSharePointIntegration
  Left = 0
  Top = 0
  BorderStyle = bsDialog
  Caption = 'SharePoint Integration'
  ClientHeight = 335
  ClientWidth = 505
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object memInfo: TMemo
    Left = 257
    Top = 0
    Width = 248
    Height = 335
    Align = alRight
    Lines.Strings = (
      'memInfo')
    ScrollBars = ssVertical
    TabOrder = 0
  end
  object pgControl: TPageControl
    Left = 0
    Top = 0
    Width = 257
    Height = 335
    ActivePage = tsUpload
    Align = alClient
    TabOrder = 1
    object tsBasic: TTabSheet
      Caption = 'Basic'
      ExplicitLeft = -12
      ExplicitTop = -2
      ExplicitWidth = 281
      ExplicitHeight = 165
      object btnConfig: TButton
        Left = 30
        Top = 54
        Width = 150
        Height = 25
        Caption = 'Config'
        TabOrder = 0
        OnClick = btnConfigClick
      end
      object btnDetect: TButton
        Left = 30
        Top = 94
        Width = 150
        Height = 25
        Caption = 'FileExists'
        TabOrder = 1
        OnClick = btnDetectClick
      end
      object edtSpin: TSpinEdit
        Left = 30
        Top = 15
        Width = 110
        Height = 22
        MaxValue = 0
        MinValue = 0
        TabOrder = 2
        Value = 0
      end
      object btnDotDot: TButton
        Left = 144
        Top = 14
        Width = 42
        Height = 25
        Caption = '...'
        TabOrder = 3
        OnClick = btnDotDotClick
      end
    end
    object tsDownload: TTabSheet
      Caption = 'Download'
      ImageIndex = 1
      ExplicitLeft = -2
      ExplicitTop = 17
      object lbl1: TLabel
        Left = 31
        Top = 81
        Width = 105
        Height = 13
        Caption = 'LoadFromDMS Result:'
      end
      object lbl2: TLabel
        Left = 30
        Top = 183
        Width = 70
        Height = 13
        Caption = 'GetFile Result:'
      end
      object btnLoadFromDMS: TButton
        Left = 31
        Top = 44
        Width = 150
        Height = 25
        Caption = 'LoadFromDMS'
        TabOrder = 0
        OnClick = btnLoadFromDMSClick
      end
      object edtDownload: TEdit
        Left = 31
        Top = 201
        Width = 150
        Height = 21
        TabOrder = 1
      end
      object edtURL: TEdit
        Left = 31
        Top = 98
        Width = 150
        Height = 21
        TabOrder = 2
      end
      object cbSelect: TCheckBox
        Left = 31
        Top = 17
        Width = 150
        Height = 17
        Caption = 'Select 2 files'
        TabOrder = 3
      end
      object btnGetFile: TButton
        Left = 30
        Top = 148
        Width = 150
        Height = 25
        Caption = 'GetFile'
        TabOrder = 4
        OnClick = btnGetFileClick
      end
    end
    object tsUpload: TTabSheet
      Caption = 'Upload'
      ImageIndex = 2
      object lbl3: TLabel
        Left = 38
        Top = 28
        Width = 94
        Height = 13
        Caption = 'Filename to upload:'
      end
      object edtFile2: TEdit
        Left = 38
        Top = 46
        Width = 150
        Height = 21
        TabOrder = 0
      end
      object btnSaveDisk: TButton
        Left = 38
        Top = 140
        Width = 150
        Height = 25
        Caption = 'SaveDisk'
        TabOrder = 1
        OnClick = btnSaveDiskClick
      end
      object btnDotDot2: TButton
        Left = 190
        Top = 43
        Width = 54
        Height = 25
        Caption = '...'
        TabOrder = 2
        OnClick = btnDotDot2Click
      end
    end
  end
  object dlgOpen: TOpenDialog
    Left = 87
    Top = 124
  end
end
