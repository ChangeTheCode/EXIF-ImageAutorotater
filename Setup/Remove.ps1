# this powershell script remove the rotation application from the registry.

#Store the current working location
Push-Location

#Set HKEY_CLASSES_ROOT as ps drive 
New-PSDrive -PSProvider registry -Root HKEY_CLASSES_ROOT -Name HKCR

#Change the current working location to the appropriate registry drive 
Set-Location HKCR:

# remove registry entry 
Remove-Item -Path "HKCR:\Directory\shell\RotationTool" -Recurse

# return to the starting working location.
Pop-Location
