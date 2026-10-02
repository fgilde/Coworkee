# Renames the app: "MyApp" -> "<New>" and "myapp" -> "<new>" in file contents, file names and folder names (one pass, case-sensitive).
# Usage: pwsh ./rename.ps1 -new Contoso [-old MyApp] [-dir .]
param(
    [String]$old,
    [String]$new,
    [String]$dir
)

if (-not $dir) { $dir = Get-Location }
if (-not $old) {
    $old = Get-ChildItem -Path $dir -Filter *.slnx | Select-Object -First 1 | ForEach-Object { $_.BaseName }
    if (-not $old) { $old = "MyApp" }
}
while (-not $new) { $new = Read-Host "Enter the new name" }
if ($new -notmatch '^[A-Za-z][A-Za-z0-9]*$') { throw "Use letters and digits only, starting with a letter." }

Write-Host "Rename '$old' to '$new' in $dir"
$skip = '[\\/](\.git|bin|obj|node_modules|\.data)([\\/]|$)'
$text = @('.cs', '.razor', '.csproj', '.props', '.targets', '.slnx', '.json', '.yml', '.yaml', '.md', '.ps1', '.css', '.js', '.html', '.xml', '.editorconfig', '.config', '.http')
$lowerOld = $old.ToLowerInvariant()
$lowerNew = $new.ToLowerInvariant()

function Rename-Text([string]$value) {
    # case-sensitive: "MyApp" -> "Contoso" for types and projects, "myapp" -> "contoso" for resource, scope and connection names
    return $value -creplace [regex]::Escape($old), $new -creplace [regex]::Escape($lowerOld), $lowerNew
}

$files = Get-ChildItem -Path $dir -Recurse -File -Force | Where-Object { $_.FullName -notmatch $skip }
foreach ($file in $files | Where-Object { $text -contains $_.Extension -or $_.Name -eq 'Dockerfile' }) {
    if ($file.FullName -eq $PSCommandPath) { continue }
    $content = Get-Content -LiteralPath $file.FullName -Raw
    if ($null -ne $content) {
        $renamed = Rename-Text $content
        if ($renamed -cne $content) {
            Set-Content -LiteralPath $file.FullName -Value $renamed -NoNewline
            Write-Host "content: $($file.FullName)"
        }
    }
}

foreach ($file in $files | Where-Object { $_.Name -cmatch [regex]::Escape($old) }) {
    Rename-Item -LiteralPath $file.FullName -NewName (Rename-Text $file.Name)
}

# deepest folders first, so parents are renamed after their children
Get-ChildItem -Path $dir -Recurse -Directory -Force | Where-Object { $_.FullName -notmatch $skip -and $_.Name -cmatch [regex]::Escape($old) } |
    Sort-Object { $_.FullName.Length } -Descending | ForEach-Object { Rename-Item -LiteralPath $_.FullName -NewName (Rename-Text $_.Name) }

Write-Host "Done. Build with: dotnet build $new.slnx"
