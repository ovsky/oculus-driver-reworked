# Oculus-Driver-Reworked

Decompiled and slightly modified version of original oculus-driver.exe ✨that works correctly✨ regardless of hardware or drivers you already have. If you have troubles with Oculus Software installation (mainly reinstall loop), try to stop it before the error window shows and follow this instructions.

Replace the: ```C:\Program Files\Oculus\Support\oculus-drivers\``` by binaries you can find in this repository releases.

Then add new Windows service that automaticly calls [or do it manually]:  
```C:\Program Files\Oculus\Support\oculus-runtime\OVRServiceLauncher.exe" -install -start```

Now you can install Oculus app and drivers.

## Note
>If you completly can't start the Oculus Software installation you can use one of Oculus Software versions from Google Drive maintained by Lazy_Ad5967: https://drive.google.com/drive/folders/1ZC5_jeDcmR1NfT4yrmZ2Y2YW1K-VdEW8
