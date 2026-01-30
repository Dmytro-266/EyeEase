interface API {
  GetSettings(): Promise<string>;
  SetSettings(settingJson: string);
  ResetSettings(): Promise<string>;
  RestNow(): Promise;
  Pause(): Promise;
  Resume(): Promise;
  Reset(): Promise;
  SetCurrentLanguage(lan: string): Promise;
  GetCurrentLanguage(): Promise<string>;
}
interface Window {
  chrome: {
    webview: {
      hostObjects: {
        sync: {
          api: API;
        };
        api: API;
      };
    };
  };
}
