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

            var eyeNurseService = IocService.GetService<EyeNurseService>()!;
            eyeNurseService.Init();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var eyeNurseService = IocService.GetService<EyeNurseService>()!;
            
            //初始化托盘，写在构造函数防止空提示
            // Init tray in OnStartup to ensure Resources are loaded
            string iconPath = Path.Combine(eyeNurseService.ApptEntryDir, "Assets\\Img\\logo.png");

            Menu = new()
            {
                Width = 150
            };

            _aboutMenuItem = new MenuItem();
            _aboutMenuItem.Click += AboutMenuItem_Click;
            _settingMenuItem = new MenuItem();
            _settingMenuItem.Click += SettingMenuItem_Click;
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
            ShowCountdownWindow();
        }

        #region public
        public void UpdateNotifyIconText(string? lan = null)
        {
            if (lan != null)
                LanService.UpdateCulture(lan);

            _notifyIcon?.Dispatcher.BeginInvoke(() =>
            {
                if (_aboutMenuItem != null) _aboutMenuItem.Header = LanService.Get("about");
                if (_settingMenuItem != null) _settingMenuItem.Header = LanService.Get("setting");
                if (_pauseMenuItem != null) _pauseMenuItem.Header = LanService.Get("pause");
                if (_resumeMenuItem != null) _resumeMenuItem.Header = LanService.Get("resume");
                if (_resetMenuItem != null) _resetMenuItem.Header = LanService.Get("reset");
                if (_restNowMenuItem != null) _restNowMenuItem.Header = LanService.Get("rest_now");
                if (_exitMenuItem != null) _exitMenuItem.Header = LanService.Get("exit");
            });
        }

        #endregion

        #region private
        #region callback
 
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
