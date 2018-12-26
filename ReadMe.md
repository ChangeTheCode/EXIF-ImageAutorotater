# Image Rotate tool

This application will rotate all your images in the given directory. So you do not need that anymore by hand. 
If the image do not support "EXIF" information this tool will not be able to rotate these files correctly. 

## Setup
Read me to add **image rotater** to the context menu (manuelly)

1. Start the regedit.exe
> Open start and type "regedit.exe and start this application 
2. Navigate to "HKEY_CLASSES_ROOT\directory\shell
3. Add there a new "KEY" with you custome name. For example "Run with image rotater" 
> Optional: Change name in this automaticlly created default entry (will chnage the display name)
4. Click right click to this "KEY" and create a second one with the name **command** add to this "Default" entry the path of the Image Rotater executable.  

