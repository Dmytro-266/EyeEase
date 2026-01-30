using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace EyeNurse.Services
{
    public class HotkeyService
    {
        private const int WM_HOTKEY = 0x0312;
        private IntPtr _windowHandle;
        private IntPtr _oldWindowHandle;
        private int _currentId;
        private Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private bool _isInitialized = false;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public void Initialize(IntPtr windowHandle)
        {
            // Unregister all hotkeys with old handle first
            if (_oldWindowHandle != IntPtr.Zero)
            {
                foreach (var id in _hotkeyActions.Keys)
                {
                    UnregisterHotKey(_oldWindowHandle, id);
                }
            }
            
            _oldWindowHandle = _windowHandle;
            _windowHandle = windowHandle;
            
            // Only subscribe once
            if (!_isInitialized)
            {
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
                _isInitialized = true;
            }
        }

        public void Register(string hotkeyStr, Action action)
        {
            if (string.IsNullOrEmpty(hotkeyStr)) return;

            try
            {
                uint modifiers = 0;
                // Simple parsing
                var parts = hotkeyStr.Split('+');
                uint vk = 0;

                foreach (var part in parts)
                {
                    var p = part.Trim();
                    if (string.Equals(p, "Ctrl", StringComparison.OrdinalIgnoreCase) || string.Equals(p, "Control", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0002;
                    else if (string.Equals(p, "Alt", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0001;
                    else if (string.Equals(p, "Shift", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0004;
                    else if (string.Equals(p, "Win", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0008;
                    else
                    {
                        // Parse key - handle numeric keys specially
                        if (p.Length == 1 && char.IsDigit(p[0]))
                        {
                            // For digits 0-9, the Key enum uses D0-D9
                            if (Enum.TryParse("D" + p, true, out Key key))
                            {
                                vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                            }
                        }
                        else if (Enum.TryParse(p, true, out Key key))
                        {
                            vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                        }
                    }
                }

                if (vk != 0)
                {
                    _currentId++;
                    bool result = RegisterHotKey(_windowHandle, _currentId, modifiers, vk);
                    System.Diagnostics.Debug.WriteLine($"RegisterHotKey for '{hotkeyStr}' (vk={vk}, mod={modifiers}, handle={_windowHandle}): {result}");
                    if (result)
                    {
                        _hotkeyActions[_currentId] = action;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to parse hotkey: {hotkeyStr}");
                }
            }
            catch (Exception ex)
            {
                // Handle parsing errors silently or log
                System.Diagnostics.Debug.WriteLine($"Failed to register hotkey {hotkeyStr}: {ex.Message}");
            }
        }

        public void UnregisterAll()
        {
            foreach (var id in _hotkeyActions.Keys)
            {
                UnregisterHotKey(_windowHandle, id);
            }
            _hotkeyActions.Clear();
            _currentId = 0;
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY)
            {
                int id = (int)msg.wParam;
                if (_hotkeyActions.ContainsKey(id))
                {
                    _hotkeyActions[id]?.Invoke();
                    handled = true;
                }
            }
        }
    }
}
