# EyeEase

**EyeEase** (formerly Eye Nurse) is a modern, unobtrusive application designed to help you protect your eyesight and handle computer fatigue. It reminds you to take regular breaks, enforcing rest periods to prevent eye strain and repetitive strain injury (RSI).

![EyeEase Logo](src/eyenurse-client-ui/public/icon.png)

## Features

-   **Smart Break Reminders**: Automatically locks the screen or notifies you when it's time to rest.
-   **Strict & Flexible Modes**: Choose whether you can skip breaks or must respect the timer.
-   **Modern User Interface**: A completely redesigned, clean UI built with Vue.js.
-   **Dark Mode**: Fully supported dark theme for comfortable use at night.
-   **Customizable**:
    -   Set work and rest durations.
    -   Configure hotkeys for pausing/resetting the timer.
    -   "Short Rest" (Rest Now) feature for immediate breaks.
-   **Unobtrusive**: Runs quietly in the system tray.
-   **Multi-language Support**: English, Chinese, and Ukrainian.

## Technology Stack

-   **Backend**: .NET 6 (WPF)
-   **Frontend**: Vue.js 3 + Vite + Tailwind CSS
-   **Integration**: Microsoft WebView2

## Getting Started

1.  Download the latest release.
2.  Install and run `EyeEase.exe`.
3.  Configure your preferred settings via the intuitive sidebar menu.

## Development

### Prerequisites
-   Node.js & npm
-   .NET 6 SDK
-   Visual Studio or VS Code

### Build
1.  **Frontend**:
    ```bash
    cd src/eyenurse-client-ui
    npm install
    npm run build
    ```
    This will copy the built UI assets to the WPF project assets folder.

2.  **Backend**:
    Open the solution in Visual Studio and build `EyeNurse` project, or run:
    ```bash
    dotnet build -c Release
    ```

## License

MIT License
