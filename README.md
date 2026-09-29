# 🥽 Oculus Driver Reworked

> **A universal Oculus driver solution** — decompiled, enhanced, and perfected to work seamlessly across all hardware configurations and installation types.

## ⚡ Overview

This project provides a reworked version of the original `oculus-driver.exe` that **actually works** reliably, regardless of your hardware specifications or existing drivers. Say goodbye to installation headaches and compatibility issues!

✨ **Works correctly on all devices** — No more driver conflicts or failed installations

## 🚀 Quick Start

### Installation

1. **Download the latest release**
   ```
   https://github.com/ovsky/oculus-driver-reworked/releases/
   ```
   Download and extract `oculus-driver.zip`

2. **Replace the original driver files**
   ```
   C:\Program Files\Oculus\Support\oculus-drivers\
   ```
   Replace with the binaries from this repository

3. **Register the Windows Service**
   ```cmd
   "C:\Program Files\Oculus\Support\oculus-runtime\OVRServiceLauncher.exe" -install -start
   ```
   Run as Administrator, or do it manually if preferred

4. **Install Oculus**
   Launch the Oculus app installer and enjoy a smooth setup experience!

## 📋 Requirements

- **Language:** C#
- **Platform:** Windows
- **Oculus SDK/Runtime** installed

## ⚠️ Troubleshooting

If you're unable to start the Oculus Software installation at all, you can use alternative Oculus Software versions maintained by the community:
- 📁 [Google Drive - Oculus Software Archives](https://drive.google.com/drive/folders/1ZC5_...) (maintained by Lazy_Ad5967)

## 🔧 Technical Details

- **Decompiled & Enhanced** version of original Oculus driver
- **Universal Hardware Support** — tested across various system configurations
- **Reliable Service Management** — proper Windows service integration
- **Driver Compatibility** — works alongside existing drivers without conflicts

## 📝 License & Credits

Built by the community for the community.

---

**Need help?** Check the [Issues](https://github.com/ovsky/oculus-driver-reworked/issues) page or submit a bug report.

**Have improvements?** Contributions are welcome! Feel free to fork and submit pull requests.
