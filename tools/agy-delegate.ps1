# Sends a task to the Antigravity CLI (agy) in headless mode. Tries each model
# in order. When a model fails for any reason except a missing agy / sign-in
# (quota, permission, incomplete run, other error), the task is sent to the
# next model in the list.
#
# Hand-off between models: if the failed model had already started (it has a
# conversation id or it changed files), the next model first tries to continue
# that same conversation. If that is not possible, it gets the task again with
# a note telling it to look at `git diff` and continue where the previous
# model stopped.
#
# Write permission: headless agy cannot ask for permission, and folder
# patterns in permissions.allow do not match. For each run the script adds a
# write_file(<path>) rule for every full file path inside this repo that the
# task mentions, and restores settings.json afterwards. So the task must list
# the full path of every file to create or edit. The script never passes
# --dangerously-skip-permissions and always keeps useG1Credits false.
#
# Exit codes:
#   0  a model finished the task (status SUCCESS and files changed)
#   10 every model failed and all failures were quota errors -> Claude does the task itself
#   3  agy is not installed or not signed in -> Claude does the task itself
#   When every model failed but not all for quota, the code of the last
#   non-quota failure is returned (Claude reports to the user, does not code):
#   4  agy was waiting for a permission it does not have (AGY_NEEDS_PERMISSION)
#   5  agy returned before its work finished, or reported SUCCESS without
#      changing any file (AGY_INCOMPLETE)
#   1  any other agy error (AGY_ERROR)
#
# On success the first lines are:
#   AGY_SUCCESS
#   MODEL=<slug>
#   CONVERSATION_ID=<id>
# followed by the agent's response. Pass CONVERSATION_ID back with
# -Conversation to send a fix request into the same conversation.
#
# Usage (prefer -TaskFile: a long prompt with quotes breaks on the
# powershell.exe -File command line):
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -TaskFile <prompt.md>
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -Task "Fix: ..." -Conversation <id>
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -TaskFile <prompt.md> -Models gemini-3.1-pro-high,gemini-3.8-flash-high
#   -AllowNoChanges: treat SUCCESS without file changes as success (e.g. a review-only task)

param(
    [string]$Task = "",
    [string]$TaskFile = "",
    [string[]]$Models = @(),
    [string]$Conversation = "",
    [string]$Timeout = "15m",
    [switch]$AllowNoChanges
)

$QuotaPattern = '429|RESOURCE_EXHAUSTED|quota|rate.?limit|capacity|credits|usage limit'
$AuthPattern = 'authentication required|not signed in|login'
$PermissionPattern = 'permission that headless mode cannot prompt|auto-denied'
$HandoffNote = "Önceki model işi yarıda bıraktı, git diff'e bak ve kaldığı yerden devam et."
$ModelsFile = Join-Path $PSScriptRoot "agy-models.txt"
$RepoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$SettingsFile = Join-Path $env:USERPROFILE ".gemini\antigravity-cli\settings.json"

if ($TaskFile) { $Task = Get-Content -Raw -Encoding UTF8 $TaskFile }
if (-not $Task) {
    Write-Output "AGY_ERROR: pass -Task or -TaskFile."
    exit 1
}

if (-not (Get-Command agy -ErrorAction SilentlyContinue)) {
    Write-Output "AGY_UNAVAILABLE: agy is not installed or not on PATH."
    exit 3
}

function Get-ModelOrder {
    if ($Models.Count -gt 0) { return $Models }
    if (Test-Path $ModelsFile) {
        return Get-Content $ModelsFile | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith("#") }
    }
    return & agy models 2>$null | ForEach-Object { ($_ -split '\s+')[0] } | Where-Object { $_ -and $_ -ne "..." }
}

# Windows PowerShell 5.1 (and 7.0-7.2) does not escape embedded double quotes
# when it passes an argument to a native program, so agy would see the prompt
# split into several arguments. Escape them the way the C runtime expects.
function ConvertTo-NativeArgument([string]$value) {
    if ($PSVersionTable.PSVersion.Major -ge 7 -and (Get-Variable PSNativeCommandArgumentPassing -ErrorAction SilentlyContinue) -and $PSNativeCommandArgumentPassing -ne 'Legacy') {
        return $value
    }
    $value = $value -replace '(\\*)"', '$1$1\"'
    return $value -replace '(\\+)$', '$1$1'
}

# Full file paths inside the repo that the task mentions (outside .git).
function Get-TaskFilePaths([string]$text) {
    $rootPrefix = $RepoRoot.TrimEnd('\') + '\'
    [regex]::Matches($text, '[A-Za-z]:\\[^\s"''`<>|*?,;()]+\.[A-Za-z0-9]+') | ForEach-Object {
        try { [System.IO.Path]::GetFullPath($_.Value) } catch { $null }
    } | Where-Object {
        $_ -and $_.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase) -and
        $_ -notmatch '\\\.git\\'
    } | Sort-Object -Unique
}

function Write-Settings($settings) {
    $json = $settings | ConvertTo-Json -Depth 10
    [System.IO.File]::WriteAllText($SettingsFile, $json, (New-Object System.Text.UTF8Encoding $false))
}

function Set-UseG1CreditsFalse($settings) {
    if ($settings.PSObject.Properties.Name -contains 'useG1Credits') { $settings.useG1Credits = $false }
    else { $settings | Add-Member -NotePropertyName useG1Credits -NotePropertyValue $false }
}

# A fingerprint of the working tree, to see whether a run changed any file.
function Get-TreeFingerprint {
    $state = (& git -C $RepoRoot status --porcelain=v1 -uall 2>$null) + (& git -C $RepoRoot diff HEAD 2>$null) | Out-String
    $sha = [System.Security.Cryptography.SHA256]::Create()
    return [BitConverter]::ToString($sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($state)))
}

function Invoke-Agy([string]$model, [string]$conversationId, [string]$prompt) {
    $agyArgs = @("-p", (ConvertTo-NativeArgument $prompt), "--output-format", "json", "--print-timeout", $Timeout, "--model", $model)
    if ($conversationId) { $agyArgs += @("--conversation", $conversationId) }

    $before = Get-TreeFingerprint
    $stderrFile = [System.IO.Path]::GetTempFileName()
    try {
        $stdout = & agy @agyArgs 2> $stderrFile | Out-String
        $exitCode = $LASTEXITCODE
        $stderr = Get-Content -Raw -ErrorAction SilentlyContinue $stderrFile
    } finally {
        Remove-Item -ErrorAction SilentlyContinue $stderrFile
    }

    $result = $null
    try { $result = $stdout | ConvertFrom-Json } catch { }

    return [pscustomobject]@{
        ExitCode       = $exitCode
        Result         = $result
        Raw            = $stdout
        Status         = if ($result) { $result.status } else { "" }
        ConversationId = if ($result) { $result.conversation_id } else { "" }
        Changed        = (Get-TreeFingerprint) -ne $before
        Error          = (@($result.error, $stderr) | Where-Object { $_ }) -join " "
    }
}

# Classifies a finished run: SUCCESS, QUOTA, AUTH, PERMISSION, INCOMPLETE or ERROR.
function Get-RunOutcome($run, [bool]$changedSinceStart) {
    $denied = @($run.Result.denied_actions | Where-Object { $_ } | ForEach-Object { $_.display_name })
    if ($run.Status -eq "SUCCESS" -and -not $run.Result.response -and $denied.Count -gt 0) { return "PERMISSION" }
    if ($run.Status -eq "SUCCESS" -and $run.Result.response -match 'Awaiting subagent') { return "INCOMPLETE" }
    if ($run.ExitCode -eq 0 -and $run.Status -eq "SUCCESS") {
        if (-not $changedSinceStart -and -not $AllowNoChanges) { return "INCOMPLETE" }
        return "SUCCESS"
    }
    if ($run.Error -match $QuotaPattern) { return "QUOTA" }
    if ($run.Error -match $AuthPattern) { return "AUTH" }
    if ($run.Status -eq "WAITING" -or $run.Error -match $PermissionPattern) { return "PERMISSION" }
    return "ERROR"
}

$modelOrder = @(Get-ModelOrder)
if ($modelOrder.Count -eq 0) {
    Write-Output "AGY_UNAVAILABLE: no models found (agy models returned nothing)."
    exit 3
}

# Temporary write permission for the files the task names.
$settingsBackup = $null
if (Test-Path $SettingsFile) {
    $settingsBackup = Get-Content -Raw -Encoding UTF8 $SettingsFile | ConvertFrom-Json
    $settings = Get-Content -Raw -Encoding UTF8 $SettingsFile | ConvertFrom-Json
    if (-not $settings.permissions) { $settings | Add-Member -NotePropertyName permissions -NotePropertyValue ([pscustomobject]@{ allow = @() }) }
    $writeRules = @(Get-TaskFilePaths $Task | ForEach-Object { "write_file($_)" })
    $settings.permissions.allow = @($settings.permissions.allow) + @($writeRules | Where-Object { $settings.permissions.allow -notcontains $_ })
    Set-UseG1CreditsFalse $settings
    Write-Settings $settings
    if ($writeRules.Count -eq 0) {
        Write-Output "AGY_WARNING: the task names no file path inside $RepoRoot; agy will not be able to write files."
    }
}

$startFingerprint = Get-TreeFingerprint
$exhausted = @()
$lastFailureCode = 0
$handoffConversation = $Conversation
$needsHandoffNote = $false

try {
    foreach ($model in $modelOrder) {
        $run = $null
        $outcome = ""

        # Continue the previous model's conversation with this model first.
        if ($handoffConversation) {
            $run = Invoke-Agy $model $handoffConversation $Task
            $outcome = Get-RunOutcome $run ((Get-TreeFingerprint) -ne $startFingerprint)
            if ($outcome -eq "ERROR" -and $handoffConversation -ne $Conversation) {
                Write-Output "AGY_HANDOFF_FAILED (model=$model): could not continue conversation $handoffConversation; starting over with a hand-off note."
                $run = $null
            }
        }
        if (-not $run) {
            $prompt = if ($needsHandoffNote) { "$HandoffNote`n`n$Task" } else { $Task }
            $run = Invoke-Agy $model "" $prompt
            $outcome = Get-RunOutcome $run ((Get-TreeFingerprint) -ne $startFingerprint)
        }

        $started = [bool]$run.ConversationId -or $run.Changed
        if ($outcome -notin @("SUCCESS", "AUTH") -and $started) {
            if ($run.ConversationId) { $handoffConversation = $run.ConversationId }
            $needsHandoffNote = $needsHandoffNote -or $run.Changed
        }

        switch ($outcome) {
            "SUCCESS" {
                Write-Output "AGY_SUCCESS"
                Write-Output "MODEL=$model"
                Write-Output "CONVERSATION_ID=$($run.ConversationId)"
                if ($exhausted.Count -gt 0) { Write-Output "EXHAUSTED_MODELS=$($exhausted -join ',')" }
                if ($run.Result.response) {
                    Write-Output $run.Result.response
                } else {
                    Write-Output "AGY_EMPTY_RESPONSE: raw output follows"
                    Write-Output $run.Raw
                }
                exit 0
            }
            "QUOTA" {
                Write-Output "AGY_MODEL_QUOTA_EXHAUSTED: $model -> $($run.Error)"
                $exhausted += $model
            }
            "AUTH" {
                Write-Output "AGY_UNAVAILABLE: $($run.Error)"
                exit 3
            }
            "PERMISSION" {
                $denied = @($run.Result.denied_actions | Where-Object { $_ } | ForEach-Object { $_.display_name })
                Write-Output "AGY_NEEDS_PERMISSION (model=$model): $(if ($denied.Count) { 'denied actions: ' + ($denied -join ', ') } else { $run.Error })"
                $lastFailureCode = 4
            }
            "INCOMPLETE" {
                Write-Output "AGY_INCOMPLETE (model=$model): agy returned before its work finished or changed no file."
                $lastFailureCode = 5
            }
            default {
                Write-Output "AGY_ERROR (model=$model, status=$($run.Status), exit=$($run.ExitCode)): $($run.Error)"
                if ($run.Result.response) { Write-Output $run.Result.response }
                $lastFailureCode = 1
            }
        }
        if ($run.ConversationId) { Write-Output "CONVERSATION_ID=$($run.ConversationId)" }
    }
} finally {
    if ($settingsBackup) {
        Set-UseG1CreditsFalse $settingsBackup
        Write-Settings $settingsBackup
    }
}

if ($lastFailureCode -ne 0) {
    Write-Output "AGY_ALL_MODELS_FAILED (last failure code $lastFailureCode); quota exhausted: $($exhausted -join ',')"
    exit $lastFailureCode
}

Write-Output "AGY_ALL_QUOTA_EXHAUSTED: $($exhausted -join ',')"
exit 10
