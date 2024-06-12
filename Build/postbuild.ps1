#Copyright PhamNinh ＼（＾_＾）／
param ($SolutionDir, $MSBuildProjectDirectory)
$projectName = (Get-Item $MSBuildProjectDirectory).Name

# Đường dẫn tới file XML cần chỉnh sửa
$xmlFilePath = "$SolutionDir\PackageContents.xml"

# Kiểm tra xem đường dẫn có tồn tại hay không
if (Test-Path $xmlFilePath) 
{}
else 
{
	$xmlFilePath = "$SolutionDir$projectName\PackageContents.xml"
}

# Đọc nội dung của file XML
[xml]$xmlContent = Get-Content -Path $xmlFilePath

# Lặp qua các nút ApplicationPackage và thay đổi giá trị của thuộc tính Name
foreach ($appPackage in $xmlContent.SelectNodes('//ApplicationPackage')) 
{
    $appPackage.SetAttribute('Name', $projectName)
    $appPackage.SetAttribute('AutodeskProduct', $projectName)
    $appPackage.SetAttribute('AppName', $projectName)
}
foreach ($test in $xmlContent.ApplicationPackage.Components.ComponentEntry) 
{
    $test.AppName = $projectName
    if ($test.ModuleName -match './21')
    {
        $test.ModuleName = "./21/$projectName.dll"
    }
    if ($test.ModuleName -match './22')
    {
        $test.ModuleName = "./22/$projectName.dll"
    }
    if ($test.ModuleName -match './23')
    {
        $test.ModuleName = "./23/$projectName.dll"
    }
    if ($test.ModuleName -match './24')
    {
        $test.ModuleName = "./24/$projectName.dll"
    }
}
# Ghi lại các thay đổi vào file XML
$xmlContent.Save($xmlFilePath)

Write-Host "Done!"
