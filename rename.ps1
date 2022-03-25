#region Params
param(
    [Parameter()]
    [String]$old,
    [String]$new,
    [String]$dir,
    [String]$autoName
)
#endregion Params

#region Options
$compile = $false;
$mask = "*.csproj *.cs *.xaml *.xml *.yml *.yaml *.json *.asax *.cshtml *.config *.js *.razor *.proto *.css *.html *.md *.razor.cs *.DotSettings *.user Dockerfile *.g.cs *.editorconfig";
#endregion Options

#region Param Handling
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
#endregion Param Handling


#region Executing
if($compile) {
    Invoke-Expression "dotnet build .\$($solutionFileName).sln";
}

Invoke-Expression "dotnet tool install -g vsrenamer";
# dotnet tool install -g ModernRonin.ProjectRenamer

$cmd = "vsrenamer.exe -a -c -f $($old) -t $($new) -w $($dir) --rename true --replacecontent true -m '$($mask)'"
Invoke-Expression $cmd

$files = Get-ChildItem -Path $dir -Filter *.csproj -Recurse
foreach ($f in $files){
    #Write-Host $f.FullName
    $ns = "$($new)." + $f.Name.Replace( ".csproj","")
    Write-Host $ns


    $xml = New-Object XML
    $xml.Load($f.FullName)
    $element =  $xml.SelectSingleNode("//RootNamespace")
    $element.InnerText = $ns
    $element =  $xml.SelectSingleNode("//AssemblyName")
    $element.InnerText = $ns
    $xml.Save($f.FullName)

}



if($compile) {
    Invoke-Expression "dotnet build .\$($new).sln";
}
#endregion Executing
