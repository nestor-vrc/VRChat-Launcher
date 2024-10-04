# VRChat Launcher Optimization

Welcome to the **VRChat Launcher Optimization** project! This application is a modified version of **VRChat's `launch.exe`**. It is designed to enhance the launching experience for VRChat by streamlining the process and improving performance while prioritizing user privacy.

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> [!WARNING]  
> This project is not affiliated with VRChat or its developers. It is a personal project and is not endorsed by VRChat. Use at your own risk.

## Features

- **Optimized Launch Process**: Simplifies and speeds up the launch of VRChat.
- **Enhanced Privacy**: Removes tracking features to protect user data.
- **CPU Affinity Management**: Allocates resources more efficiently for better performance.
- **Process Priority Handling**: Adjusts priority settings to ensure a smoother VRChat experience.

## Installation

1. Download the latest release from the [releases page](https://github.com/nestor-vrc/VRChat-Launcher/releases).
2. Open the VRChat installation directory and rename your original `launch.exe` file to `launch.exe.bak`.
   
   ```bash
   steamapps\common\VRChat
   📂 .
   ├── 📁 EasyAntiCheat
   ├── 📁 TouchMenuIcons
   ├── 📁 VRChat_Data
   ├── ...
   ├── 📄 launch.exe.bak // (original)
   ├── ...
   └── 🎮 VRChat.exe
   ```
3. Copy `Launch.exe` Modified version you just loaded from the [releases page](https://github.com/nestor-vrc/VRChat-Launcher/releases) to the VRChat installation directory.
   
   ```bash
   steamapps\common\VRChat
   📂 .
   ├── 📁 EasyAntiCheat
   ├── 📁 TouchMenuIcons
   ├── 📁 VRChat_Data
   ├── ...
   ├── 📄 launch.exe // (modified)
   ├── 📄 launch.exe.bak // (original)
   ├── ...
   └── 🎮 VRChat.exe
   ```

4. Launch VRChat and enjoy the enhanced launch experience!

## Usage

To use the modified `launch.exe`, simply rename your original `launch.exe` file to `launch.exe.bak`, copy the modified version to the same location, and launch VRChat as usual.

> [!NOTE]  
> If you encounter any issues or have suggestions for improvement, please open an issue or submit a pull request on the [GitHub repository](https://github.com/nestor-vrc/VRChat-Launcher).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
