# this powershell script set the rotation application to the registry.
# so you can execute the application by navigate to a directory and use the context menu to start the application

# Parameter help description
Param(
    [Parameter(Mandatory=$True)]
    [string]$path
)

#Store the current working location
Push-Location

#Set HKEY_CLASSES_ROOT as ps drive 
New-PSDrive -PSProvider registry -Root HKEY_CLASSES_ROOT -Name HKCR

#Change the current working location to the appropriate registry drive 
Set-Location HKCR:

# add app to context menu of directories
New-Item –Path "HKCR:\Directory\shell" –Name "RotationTool"

# add icon for app 
New-ItemProperty –Path "HKCR:\Directory\shell\RotationTool" –Name "icon" -Value "C:\Source_Private\Github\EXIF-ImageAutorotater\icon.ico"

# create command directory 
New-Item –Path "HKCR:\Directory\shell\RotationTool" –Name "command"

# add path to the application exe
Set-Itemproperty –Path "HKCR:\Directory\shell\RotationTool\command" –Name "(Default)" -Value $path

# return to the starting working location.
Pop-Location
