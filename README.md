# JpegSync
This utility was created with the purpose of enabling the use of Windows Spotlight (lockscreen) images as desktop backgrounds. This function does not exist in Windows 10. However, the files are on your local disk, so it's just a matter of copying these to your wallpaper folder. This utility automates that process.

## Getting started
If you don't care about the source code, but just want the tool, browse to the <code>bin\release</code> folder and download <code>JpegSync.exe</code>. That's all you'll need!

## Command line options
<pre>
JpegSync
 -i path                 Shows info on all JPEG files at the specified location
 -s path_from path_to    Syncs JPEGs from first path to the second
</pre>

Example
<pre>
JpegSync.exe -s "%localappdata%\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets" "D:\User\Pictures\wallpapers"
</pre>

## Method
The application works the way that it reads all JPEGs at the <code>path_from</code> location and their dimension and file size. These attributes are compared to the files at the <code>path_to</code> location. If there is no match on these three attributes, the file is copied, hence adding the new lockscreen image to the wallpapers collection.

Currently, only landscape images in HD (1920x1080) are copied. This filter can easily be changed in the code in the <code>Program.SyncPaths</code> method.

## Automate
To run this automatically at startup, add a shortcut to the application in your startup folder
<pre>
C:\Users\[username]\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
</pre>
