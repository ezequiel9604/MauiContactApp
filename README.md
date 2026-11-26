# ContactApp

A contacts app built with **.NET MAUI** to practice cross-platform mobile development with C# and XAML. You can browse, search, add, edit and delete contacts, and share any contact as a QR code.

> ⚠️ **This app needs a backend.** It gets and saves contacts through the **[contact-app-backend](https://github.com/<your-username>/contact-app-backend)** API. Set up and start the backend before you run the app (see [Backend setup](#2-backend-setup)).

<!-- Add screenshots: create a /docs folder in the repo and put the images there -->
<!--
<p align="center">
  <img src="docs/contacts-list.png" width="250" />
  <img src="docs/contact-qr.png" width="250" />
</p>
-->

## Features

- 📇 Contact list with avatar initials
- 🔍 Live search as you type
- ➕ Add and edit contacts
- 👉 Swipe actions: **Edit**, **Delete** and **QR**
- 🔄 Pull to refresh
- 🗂️ Sorting
- 📱 Share a contact as a QR code (vCard), which any phone camera can scan to add the contact
- 🖼️ Share the QR as an image through the system share sheet
- 🫙 Empty state when there are no contacts

## Tech stack

| | |
|---|---|
| Framework | .NET MAUI |
| Language | C# / XAML |
| Architecture | MVVM |
| MVVM helpers | [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) |
| QR generation | [ZXing.Net.Maui.Controls](https://www.nuget.org/packages/ZXing.Net.Maui.Controls) |
| Backend | [contact-app-backend](https://github.com/<your-username>/contact-app-backend) (REST API) |

## Requirements

### Software

- **[.NET SDK](https://dotnet.microsoft.com/download)**: use the version that matches `<TargetFrameworks>` in `ContactApp.csproj` (for example, `net10.0-android` needs the .NET 10 SDK)
- **.NET MAUI workload**
- An IDE, either of these:
  - **Visual Studio 2022 (17.12 or later) or Visual Studio 2026** on Windows with the **.NET Multi-platform App UI development** workload
  - **VS Code** with the **C# Dev Kit** and **.NET MAUI** extensions

### Backend

- **[contact-app-backend](https://github.com/<your-username>/contact-app-backend)** running locally or deployed. Its own README lists what it needs

### To run on Android

- Android SDK (installed by Visual Studio with the MAUI workload)
- An **Android emulator** (Android 5.0 / API 21 or higher), or
- A **physical Android device** with [Developer options and USB debugging](https://developer.android.com/studio/debug/dev-options) turned on

### To run on iOS (optional)

- A **Mac** with **Xcode** installed. iOS builds need a Mac, either directly or paired with Visual Studio on Windows
- An Apple ID for deploying to a physical iPhone

> **Note:** Windows is a supported target for running the app, but this project is mainly developed and tested on **Android**.

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/ContactApp.git
cd ContactApp
```

### 2. Backend setup

Clone and start the backend first by following the steps in **[contact-app-backend](https://github.com/<your-username>/contact-app-backend)**:

```bash
git clone https://github.com/<your-username>/contact-app-backend.git
```

#### Connect the emulator to the backend (`adb reverse`)

With the emulator running, forward the backend's port from the emulator to your PC:

```bash
adb reverse tcp:BACKEND_PORT tcp:BACKEND_PORT
```

Replace `BACKEND_PORT` with the port your backend listens on. For example:

```bash
adb reverse tcp:3000 tcp:3000
```

After this, `localhost:BACKEND_PORT` inside the emulator reaches the backend on your PC.

> **You need to run this again every time the emulator restarts.** To check that it's active, run `adb reverse --list`.
>
> If `adb` isn't recognized, open it from Visual Studio through **Tools → Android → Android Adb Command Prompt**, or add the Android SDK `platform-tools` folder to your `PATH`.

#### Set the API base URL

Set the base URL in `<path/to/your/config-file>`:

```csharp
public const string BaseUrl = "http://localhost:BACKEND_PORT/api/";
```

Which address to use depends on where the app runs:

| Running on | Use |
|---|---|
| Android emulator, backend on your PC | `http://localhost:<port>/` **after running `adb reverse`** |
| Physical phone over USB | `http://localhost:<port>/` after running `adb reverse` (works the same way) |
| Physical phone over Wi-Fi | `http://<your-PC-local-IP>:<port>/` (same network, for example `http://192.168.1.20:3000/`) |
| Windows target | `http://localhost:<port>/` |
| Deployed backend | Its public `https://` URL |

> **Android and plain HTTP:** Android blocks `http://` traffic by default. For local development, allow it in `Platforms/Android/AndroidManifest.xml` with `android:usesCleartextTraffic="true"` on the `<application>` element, and use HTTPS in production.

### 3. Install the MAUI workload (if you don't have it)

```bash
dotnet workload install maui
```

To check that it's installed:

```bash
dotnet workload list
```

### 4. Restore packages

```bash
dotnet restore
```

### 5. Run the app

Before you start, make sure that:

- [ ] The backend is running
- [ ] The emulator is open and you've run `adb reverse tcp:BACKEND_PORT tcp:BACKEND_PORT`

Then:

**Visual Studio**

1. Open `ContactApp.sln`
2. In the debug target dropdown, select **Android Emulators** and pick an emulator, or select your connected device
3. Press **F5**

**Command line (Android)**

```bash
dotnet build -t:Run -f net10.0-android
```

> Replace `net10.0-android` with the target framework in your `.csproj`.

## Project structure

```
ContactApp/
├── Models/             # Contact model
├── ViewModels/         # ContactsViewModel (MVVM)
├── Views/              # ContactPage, ContactQrPage, etc.
├── Helpers/            # VCardHelper (contact → vCard text for QR codes)
├── Resources/
│   ├── Images/         # Icons (SVGs are converted to PNG at build time)
│   └── Styles/         # Colors and styles
├── Platforms/          # Platform-specific code and config (Android, iOS, ...)
└── MauiProgram.cs      # App startup, DI and library registration
```

## Troubleshooting

| Problem | Fix |
|---|---|
| Contact list is empty or shows a connection error | Check that **contact-app-backend** is running and the base URL is correct for your device (see [Backend setup](#2-backend-setup)) |
| App worked before but now can't reach the backend | The emulator was probably restarted. Run `adb reverse tcp:BACKEND_PORT tcp:BACKEND_PORT` again |
| `adb: error: no devices/emulators found` | Start the emulator first, then run `adb reverse` |
| `adb` is not recognized | Use **Tools → Android → Android Adb Command Prompt** in Visual Studio, or add `platform-tools` to your `PATH` |
| Works on the emulator but not on a phone over Wi-Fi | Use your PC's local IP, keep both devices on the same Wi-Fi, and allow the backend's port through your firewall. Or connect the phone by USB and use `adb reverse` |
| `Cleartext HTTP traffic not permitted` | Add `android:usesCleartextTraffic="true"` for local development, or use HTTPS |
| `The workload 'maui' is not installed` | Run `dotnet workload install maui` |
| Old images or icons still showing | **Clean** the solution, then **Rebuild** |
| No emulator listed | In Visual Studio go to **Tools → Android → Android Device Manager** and create one |
| Device not detected | Turn on USB debugging and accept the "Allow USB debugging" prompt on the phone |
| Build errors after pulling changes | Delete the `bin/` and `obj/` folders and rebuild |

## Roadmap

- [ ] Offline support (local SQLite cache synced with the backend)
- [ ] Contact photos
- [ ] Call, SMS and email actions
- [ ] Favorites
- [ ] Dark mode

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
