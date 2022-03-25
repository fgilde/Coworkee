param(
    [Parameter()]
    [String]$old,
    [String]$new,
    [String]$dir
)

$mask = "*.csproj *.cs *.xaml *.xml *.yml *.yaml *.json *.asax *.cshtml *.config *.js *.razor *.proto *.css *.html *.md *.razor.cs *.DotSettings";
# $mask = "*.*";

if (!$dir) {
    $dir = Get-Location;
}
Write-Output "We are working in Directory: $($dir)";

if (!$old) {
    $old = "CleanArchitectureBase";
    ($old,(Read-Host "Enter your old name or press Enter to use $($old)")) -match '\S' |% {$old = $_}
}

while (!$new) {    
    ($new,(Read-Host "Enter the new name:")) -match '\S' |% {$new = $_}
}

dotnet tool install -g vsrenamer
# dotnet tool install -g ModernRonin.ProjectRenamer

$cmd = "vsrenamer.exe -a -f $($old) -t $($new) -c -w $($dir) --rename true --replacecontent true -m '$($mask)'"
Invoke-Expression $cmd

# Write-Output $old
# Write-Output "to $new"