# Oculus-Driver-Reworked

If you have troubles with Oculus Software installation (mainly reinstall loop), try to stop it before the error window shows and replace this decompiled and slightly modified version of original oculus-driver.exe ✨that works correctly✨ regardless of hardware or drivers you already have.

Replace the: ```C:\Program Files\Oculus\Support\oculus-drivers\``` by binaries you can find in this repository.


Then add new Windows service that automaticly calls [or do it manually everytime]:
```C:\Program Files\Oculus\Support\oculus-runtime\OVRServiceLauncher.exe" -install -start```

Now you can install these drivers using Oculus Home app.

## Note
>If you completly can't start the Oculus Software installation you can use one of Oculus Software versions from Google Drive maintained by Lazy_Ad5967: https://drive.google.com/drive/folders/1ZC5_jeDcmR1NfT4yrmZ2Y2YW1K-VdEW8