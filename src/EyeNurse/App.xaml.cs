using Common.Apps.Services;
using EyeNurse.Services;
using EyeNurse.ViewModels;
using HandyControl.Controls;
using HandyControl.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using MultiLanguageForXAML;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EyeNurse
{
    public partial class App : Application
    {
        private NotifyIcon? _notifyIcon;
        private MenuItem? _aboutMenuItem;
        private MenuItem? _settingMenuItem;
        private MenuItem? _darkModeMenuItem;
        private MenuItem? _runWhenStartsMenuItem;
        private MenuItem? _resetWhenUnlockMenuItem;
        private MenuItem? _pauseMenuItem;
        private MenuItem? _resumeMenuItem;
        private MenuItem? _resetMenuItem;
        private MenuItem? _restNowMenuItem;
        private MenuItem? _exitMenuItem;

        public static ContextMenu? Menu { private set; get; }

        public App()
        {
            //基础服务初始化
            var services = new ServiceCollection();
            services.AddSingleton<HotkeyService>();
            services.AddSingleton<EyeNurseService>();
            services.AddSingleton<EyeNurseViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton(this);
            IocService.Init(new InitServiceOption() { AppName = nameof(EyeNurse) }, services);

            //自定义控件
            //Xaml.CustomMaps.Add(typeof(TitleBar), TitleBar.TitleProperty);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var eyeNurseService = IocService.GetService<EyeNurseService>()!;
            eyeNurseService.Init();
            
            //初始化托盘，写在构造函数防止空提示
            // Init tray in OnStartup to ensure Resources are loaded
            string iconPath = Path.Combine(eyeNurseService.ApptEntryDir, "Assets\\Img\\logo.png");

            Menu = new()
            {
                Width = 200
            };

            _aboutMenuItem = new MenuItem();
            _aboutMenuItem.Click += AboutMenuItem_Click;
            _settingMenuItem = new MenuItem();
            _settingMenuItem.Click += SettingMenuItem_Click;
            
            // Quick settings toggles
            _darkModeMenuItem = new MenuItem() { IsCheckable = true };
            _darkModeMenuItem.Click += DarkModeMenuItem_Click;
            _runWhenStartsMenuItem = new MenuItem() { IsCheckable = true };
            _runWhenStartsMenuItem.Click += RunWhenStartsMenuItem_Click;
            _resetWhenUnlockMenuItem = new MenuItem() { IsCheckable = true };
            _resetWhenUnlockMenuItem.Click += ResetWhenUnlockMenuItem_Click;
            
            _pauseMenuItem = new MenuItem();
            _pauseMenuItem.Click += PauseMenuItem_Click;
            _resumeMenuItem = new MenuItem() { Visibility = Visibility.Collapsed };
            _resumeMenuItem.Click += ResumeMenuItem_Click;
            _resetMenuItem = new MenuItem();
            _resetMenuItem.Click += ResetMenuItem_Click;
            _restNowMenuItem = new MenuItem();
            _restNowMenuItem.Click += RestNowMenuItem_Click;
            _exitMenuItem = new MenuItem() { Command = ControlCommands.ShutdownApp };

            Menu.Items.Add(_aboutMenuItem);
            Menu.Items.Add(_settingMenuItem);
            Menu.Items.Add(new Separator());
            // Quick settings submenu
            Menu.Items.Add(_darkModeMenuItem);
            Menu.Items.Add(_runWhenStartsMenuItem);
            Menu.Items.Add(_resetWhenUnlockMenuItem);
            Menu.Items.Add(new Separator());
            Menu.Items.Add(_pauseMenuItem);
            Menu.Items.Add(_resumeMenuItem);
            Menu.Items.Add(_resetMenuItem);
            Menu.Items.Add(_restNowMenuItem);
            Menu.Items.Add(new Separator());
            Menu.Items.Add(_exitMenuItem);

            _notifyIcon = new NotifyIcon()
            {
                Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute))
                {
                    DecodePixelWidth = 300,
                    DecodePixelHeight = 300
                },
                ContextMenu = Menu
            };

            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            _notifyIcon.Init();

            UpdateNotifyIconText();
            LoadQuickSettings();
            ShowCountdownWindow();
        }

        #region public
        public void UpdateNotifyIconText(string? lan = null)
        {
            if (lan != null)
                LanService.UpdateCulture(lan);

            _notifyIcon?.Dispatcher.BeginInvoke(() =>
            {
                if (_aboutMenuItem != null) _aboutMenuItem.Header = GetSafeText("about", "About");
                if (_settingMenuItem != null) _settingMenuItem.Header = GetSafeText("setting", "Settings");
                if (_darkModeMenuItem != null) _darkModeMenuItem.Header = GetSafeText("dark_mode", "Dark Mode");
                if (_runWhenStartsMenuItem != null) _runWhenStartsMenuItem.Header = GetSafeText("run_when_starts", "Run When Starts");
                if (_resetWhenUnlockMenuItem != null) _resetWhenUnlockMenuItem.Header = GetSafeText("reset_when_unlock", "Reset After Unlock");
                if (_pauseMenuItem != null) _pauseMenuItem.Header = GetSafeText("pause", "Pause");
                if (_resumeMenuItem != null) _resumeMenuItem.Header = GetSafeText("resume", "Resume");
                if (_resetMenuItem != null) _resetMenuItem.Header = GetSafeText("reset", "Reset");
                if (_restNowMenuItem != null) _restNowMenuItem.Header = GetSafeText("rest_now", "Rest Now");
                if (_exitMenuItem != null) _exitMenuItem.Header = GetSafeText("exit", "Exit");
            });
        }

        public void RefreshQuickSettings()
        {
            LoadQuickSettings();
        }
        
        private string GetSafeText(string key, string fallback)
        {
            try
            {
                var text = LanService.Get(key);
                return string.IsNullOrEmpty(text) ? fallback : text;
            }
            catch
            {
                return fallback;
            }
        }

        #endregion

        #region private
        
        private void LoadQuickSettings()
        {
            var eyeNurseService = IocService.GetService<EyeNurseService>();
            if (eyeNurseService == null) return;
            
            var setting = eyeNurseService.LoadUserConfig<Models.UserConfigs.Setting>();
            
            if (_darkModeMenuItem != null) _darkModeMenuItem.IsChecked = setting.DarkMode;
            if (_runWhenStartsMenuItem != null) _runWhenStartsMenuItem.IsChecked = setting.RunWhenStarts;
            if (_resetWhenUnlockMenuItem != null) _resetWhenUnlockMenuItem.IsChecked = setting.ResetWhenSessionUnlock;
        }
        
        private void SaveQuickSettings(Action<Models.UserConfigs.Setting> updateAction)
        {
            var eyeNurseService = IocService.GetService<EyeNurseService>();
            if (eyeNurseService == null) return;
            
            var setting = eyeNurseService.LoadUserConfig<Models.UserConfigs.Setting>();
            updateAction(setting);
            eyeNurseService.SaveUserConfig(setting);
            eyeNurseService.ApplySetting(setting);
        }
        
        #region callback
 
        private void DarkModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _darkModeMenuItem?.IsChecked ?? false;
            SaveQuickSettings(s => s.DarkMode = isChecked);
        }
        
        private void RunWhenStartsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _runWhenStartsMenuItem?.IsChecked ?? false;
            SaveQuickSettings(s => s.RunWhenStarts = isChecked);
        }
        
        private void ResetWhenUnlockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _resetWhenUnlockMenuItem?.IsChecked ?? false;
            SaveQuickSettings(s => s.ResetWhenSessionUnlock = isChecked);
        }
        
        private void ResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.Reset();
        }

        private void ResumeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_resumeMenuItem != null) _resumeMenuItem.Visibility = Visibility.Collapsed;
            if (_pauseMenuItem != null) _pauseMenuItem.Visibility = Visibility.Visible;
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.Resume();
        }

        private void PauseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_resumeMenuItem != null) _resumeMenuItem.Visibility = Visibility.Visible;
            if (_pauseMenuItem != null) _pauseMenuItem.Visibility = Visibility.Collapsed;
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.Pause();
        }

        private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.ShowSetting();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.About();
        }
        private void RestNowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.RestNow();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowCountdownWindow();
        }

        private static void ShowCountdownWindow()
        {
            var vm = IocService.GetService<EyeNurseViewModel>();
            vm?.ShowCountdownWindow();
        }
        #endregion

        #endregion

    }
}
