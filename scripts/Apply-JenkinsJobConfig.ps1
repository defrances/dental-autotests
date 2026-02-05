# Apply Jenkins job "test execution" config via API.
# Usage: .\Apply-JenkinsJobConfig.ps1 [-JenkinsUrl "http://localhost:8080"] [-User "admin"] [-Password "admin"] [-ConfigPath "..\jenkins-job-config.xml"]
param(
    [string] $JenkinsUrl = "http://localhost:8080",
    [string] $User = "admin",
    [string] $Password = "admin",
    [string] $JobName = "test execution",
    [string] $ConfigPath = (Join-Path $PSScriptRoot "..\jenkins-job-config.xml")
)
$ErrorActionPreference = "Stop"
$pair = "${User}:${Password}"
$b64 = [Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))
$base = $JenkinsUrl.TrimEnd("/")
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
# Get crumb (same session for cookie)
$crumbJson = Invoke-RestMethod -Uri "$base/crumbIssuer/api/json" -Headers @{ Authorization = "Basic $b64" } -WebSession $session
$crumb = $crumbJson.crumb
# Load config and apply (crumb in URL for POST)
$config = Get-Content -Path $ConfigPath -Raw
$jobEnc = [System.Web.HttpUtility]::UrlEncode($JobName)
$uri = "$base/job/$jobEnc/config.xml?Jenkins-Crumb=$([System.Web.HttpUtility]::UrlEncode($crumb))"
Invoke-WebRequest -Uri $uri -Method Post -Headers @{ Authorization = "Basic $b64"; "Content-Type" = "application/xml" } -WebSession $session -Body $config -UseBasicParsing | Out-Null
Write-Host "OK: Job '$JobName' config applied from $ConfigPath"
