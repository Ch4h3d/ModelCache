param (
	$solutionPath,
	$version = "alpha1"
)


Push-Location $solutionPath
$templateBuildPropsFilename = "./Directory.Build.props.template"
$buildPropsFilename = "./Directory.Build.props"

$gitDescribe = git describe
Write-Host "Git describe: $gitDescribe"
$gitDescribeArray = $gitDescribe -split "-"
$majorMinor = $gitDescribeArray[0]
$patch = $gitDescribeArray[1]
if($patch -eq $null){
	$patch = 0
}
$gitHash = $gitDescribeArray[2]
if($gitHash -eq $null){
	$gitHash = git rev-parse --short HEAD
}

$buildProps = Get-Content $templateBuildPropsFilename

if([string]::IsNullOrWhiteSpace($version))
{
	$buildProps = $buildProps -Replace "<PackageVersion></PackageVersion>", "<PackageVersion>$majorMinor.$patch</PackageVersion>"
}
else{
	Write-Host "Status: $version"
	$buildProps = $buildProps -Replace "<PackageVersion></PackageVersion>", "<PackageVersion>$majorMinor.$patch-$version</PackageVersion>"
}
$buildProps = $buildProps -Replace "<AssemblyVersion></AssemblyVersion>", "<AssemblyVersion>$majorMinor.$patch</AssemblyVersion>"
$buildProps = $buildProps -Replace "<FileVersion></FileVersion>", "<FileVersion>$majorMinor.$patch</FileVersion>"
$buildProps = $buildProps -Replace "<InformationalVersion></InformationalVersion>", "<InformationalVersion>$majorMinor.$patch+$gitHash</InformationalVersion>"

Set-Content -Path $buildPropsFilename -Value $buildProps

Pop-Location