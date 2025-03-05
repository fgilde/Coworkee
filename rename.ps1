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
# Definiere die Dateimasken für textbasierte Dateien
$mask = "*.csproj *.cs *.xaml *.xml *.yml *.yaml *.json *.asax *.cshtml *.config *.js *.razor *.proto *.css *.html *.md *.razor.cs *.DotSettings *.user Dockerfile *.g.cs *.editorconfig *.sln"
#endregion Options

#region Param Handling
if (-not $dir) {
    $dir = Get-Location
}
$solutionFileName = Get-ChildItem -Path $dir -Filter *.sln | ForEach-Object { $_.Name.Replace(".sln","") }

Write-Output "Working in Directory: $($dir)"

if (-not $old) {
    if ($solutionFileName) {
        $old = $solutionFileName
    } else {
        $old = "CleanArchitectureBase"
    }
    if (-not $autoName) {
        ($old, (Read-Host "Enter your old name or press Enter to use '$old'")) -match '\S' | ForEach-Object { $old = $_ }
    }
}

while (-not $new) {    
    ($new, (Read-Host "Enter the new name:")) -match '\S' | ForEach-Object { $new = $_ }
}

if (-not $old) {
    throw 'Invalid old name'
    exit
}

Write-Host "Rename from '$old' to '$new'"
#endregion Param Handling

#region Executing
if ($compile) {
    Invoke-Expression "dotnet build .\$($solutionFileName).sln"
}


Invoke-Expression "dotnet tool install -g vsrenamer"
$cmd = "vsrenamer.exe -a -c -f $($old) -t $($new) -w $($dir) --rename true --replacecontent true -m '$($mask)'"
Invoke-Expression $cmd

# Aktualisiere .csproj Dateien für Namespace- und AssemblyName-Änderungen
$files = Get-ChildItem -Path $dir -Filter *.csproj -Recurse
foreach ($f in $files) {
    $ns = "$($new)." + $f.Name.Replace(".csproj","")
    Write-Host "Updating file: $($f.FullName) with namespace: $ns"

    $xml = New-Object XML
    $xml.Load($f.FullName)
    
    # Aktualisiere RootNamespace
    $rootNamespaceNode = $xml.SelectSingleNode("//RootNamespace")
    if ($null -ne $rootNamespaceNode -and $rootNamespaceNode.PSObject.Properties.Match("InnerText")) {
        $rootNamespaceNode.InnerText = $ns
    } else {
        Write-Warning "RootNamespace-Element nicht gefunden oder nicht änderbar in Datei $($f.FullName)"
    }
    
    # Aktualisiere AssemblyName
    $assemblyNameNode = $xml.SelectSingleNode("//AssemblyName")
    if ($null -ne $assemblyNameNode -and $assemblyNameNode.PSObject.Properties.Match("InnerText")) {
        $assemblyNameNode.InnerText = $ns
    } else {
        Write-Warning "AssemblyName-Element nicht gefunden oder nicht änderbar in Datei $($f.FullName)"
    }
    
    $xml.Save($f.FullName)
}

# Ersetze in allen textbasierten Dateien den alten Namen durch den neuen
$extensions = $mask.Split(" ")
foreach ($ext in $extensions) {
    Get-ChildItem -Path $dir -Filter $ext -Recurse | ForEach-Object {
        try {
            (Get-Content $_.FullName -ErrorAction Stop) -replace [regex]::Escape($old), $new | Set-Content $_.FullName
            Write-Host "Updated content in file: $($_.FullName)"
        }
        catch {
            Write-Warning "Fehler beim Verarbeiten der Datei: $($_.FullName) - $_"
        }
    }
}

# Benenne Dateien um, die den alten Namen beinhalten
Get-ChildItem -Path $dir -Recurse -File | ForEach-Object {
    if ($_.Name -like "*$old*") {
        $newFileName = $_.Name -replace [regex]::Escape($old), $new
        $newFilePath = Join-Path -Path $_.DirectoryName -ChildPath $newFileName
        Rename-Item -Path $_.FullName -NewName $newFileName
        Write-Host "Renamed file: $($_.FullName) to $newFilePath"
    }
}

# Benenne Ordner um, die den alten Namen beinhalten (von tiefster Ebene beginnend)
Get-ChildItem -Path $dir -Recurse -Directory | Sort-Object FullName -Descending | ForEach-Object {
    if ($_.Name -like "*$old*") {
        $newDirName = $_.Name -replace [regex]::Escape($old), $new
        $newDirPath = Join-Path -Path $_.Parent.FullName -ChildPath $newDirName
        Rename-Item -Path $_.FullName -NewName $newDirName
        Write-Host "Renamed directory: $($_.FullName) to $newDirPath"
    }
}

if ($compile) {
    Invoke-Expression "dotnet build .\$($new).sln"
}
#endregion Executing
