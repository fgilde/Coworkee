param(
    [Parameter()]
    [String]$old,
    [String]$new,
    [String]$dir,
    [String]$autoName
)

$mask = "*.csproj *.cs *.xaml *.xml *.yml *.yaml *.json *.asax *.cshtml *.config *.js *.razor *.proto *.css *.html *.md *.razor.cs *.DotSettings *.user Dockerfile *.g.cs *.editorconfig";
# $mask = "*.*";

if (!$dir) {
    $dir = Get-Location;
}
$solutionFileName = Get-ChildItem -Path $dir -Filter *.sln | % { $_.Name.Replace( ".sln","") };

Write-Output "We are working in Directory: $($dir)";

if (!$old) {
    if($solutionFileName) {
        $old = $solutionFileName;
    } else {
        $old = "CleanArchitectureBase";
    }
    if (!$autoName) {
        ($old,(Read-Host "Enter your old name or press Enter to use $($old)")) -match '\S' |% {$old = $_}
    }
}

while (!$new) {    
    ($new,(Read-Host "Enter the new name:")) -match '\S' |% {$new = $_}
}

if(!$old) {
    throw 'Invalid old name';
    exit;
}

Write-Host "Rename from $($old) to $($new)";

Invoke-Expression "dotnet build .\$($solutionFileName).sln";

Invoke-Expression "dotnet tool install -g vsrenamer";
# dotnet tool install -g ModernRonin.ProjectRenamer

$cmd = "vsrenamer.exe -a -f $($old) -t $($new) -w $($dir) --rename true --replacecontent true -m '$($mask)'"
Invoke-Expression $cmd
Invoke-Expression "dotnet build .\$($new).sln";
