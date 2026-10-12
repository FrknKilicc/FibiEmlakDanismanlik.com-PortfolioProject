# Sends a task to the Antigravity CLI (agy) in headless mode. Tries each model
# in order. When a model fails for any reason except a missing agy / sign-in
# (quota, permission, incomplete run, other error), the same task is sent to
# the next model in the list.
#
# Exit codes:
#   0  a model finished the task (status SUCCESS)
#   10 every model failed and all failures were quota errors -> Claude does the task itself
#   3  agy is not installed or not signed in -> Claude does the task itself
#   When every model failed but not all for quota, the code of the last
#   non-quota failure is returned (Claude reports to the user, does not code):
#   4  agy was waiting for a permission it does not have (AGY_NEEDS_PERMISSION)
#   5  agy returned before its work finished (AGY_INCOMPLETE)
#   1  any other agy error (AGY_ERROR)
#
# A failed model may leave partial file changes behind; the next model starts
# from that working tree. Check `git diff` after every run.
#
# On success the first lines are:
#   AGY_SUCCESS
#   MODEL=<slug>
#   CONVERSATION_ID=<id>
# followed by the agent's response. Pass CONVERSATION_ID back with
# -Conversation to send a fix request into the same conversation.
#
# Usage:
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -Task "..."
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -Task "Fix: ..." -Conversation <id>
#   powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -Task "..." -Models gemini-3.1-pro-high,gemini-3.8-flash-high

param(
    [Parameter(Mandatory = $true)][string]$Task,
    [string[]]$Models = @(),
    [string]$Conversation = "",
    [string]$Timeout = "15m"
)

$QuotaPattern = '429|RESOURCE_EXHAUSTED|quota|rate.?limit|capacity|credits|usage limit'
$AuthPattern = 'authentication required|not signed in|login'
$ModelsFile = Join-Path $PSScriptRoot "agy-models.txt"

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

function Invoke-Agy([string]$model, [string]$conversationId) {
    $agyArgs = @("-p", $Task, "--output-format", "json", "--print-timeout", $Timeout, "--model", $model)
    if ($conversationId) { $agyArgs += @("--conversation", $conversationId) }

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
        ExitCode = $exitCode
        Result   = $result
        Raw      = $stdout
        Status   = if ($result) { $result.status } else { "" }
        Error    = (@($result.error, $stderr) | Where-Object { $_ }) -join " "
    }
}

$modelOrder = @(Get-ModelOrder)
if ($modelOrder.Count -eq 0) {
    Write-Output "AGY_UNAVAILABLE: no models found (agy models returned nothing)."
    exit 3
}

$exhausted = @()
$lastFailureCode = 0
foreach ($model in $modelOrder) {
    $run = Invoke-Agy $model $Conversation

    $denied = @($run.Result.denied_actions | Where-Object { $_ } | ForEach-Object { $_.display_name })
    if ($run.Status -eq "SUCCESS" -and -not $run.Result.response -and $denied.Count -gt 0) {
        Write-Output "AGY_NEEDS_PERMISSION (model=$model): denied actions: $($denied -join ', ')"
        Write-Output "CONVERSATION_ID=$($run.Result.conversation_id)"
        $lastFailureCode = 4
        continue
    }

    if ($run.Status -eq "SUCCESS" -and $run.Result.response -match 'Awaiting subagent') {
        Write-Output "AGY_INCOMPLETE (model=$model): agy returned before its subagent finished."
        Write-Output "CONVERSATION_ID=$($run.Result.conversation_id)"
        $lastFailureCode = 5
        continue
    }

    if ($run.ExitCode -eq 0 -and $run.Status -eq "SUCCESS") {
        Write-Output "AGY_SUCCESS"
        Write-Output "MODEL=$model"
        Write-Output "CONVERSATION_ID=$($run.Result.conversation_id)"
        if ($exhausted.Count -gt 0) { Write-Output "EXHAUSTED_MODELS=$($exhausted -join ',')" }
        if ($run.Result.response) {
            Write-Output $run.Result.response
        } else {
            Write-Output "AGY_EMPTY_RESPONSE: raw output follows"
            Write-Output $run.Raw
        }
        exit 0
    }

    if ($run.Error -match $QuotaPattern) {
        Write-Output "AGY_MODEL_QUOTA_EXHAUSTED: $model -> $($run.Error)"
        $exhausted += $model
        continue
    }

    if ($run.Error -match $AuthPattern) {
        Write-Output "AGY_UNAVAILABLE: $($run.Error)"
        exit 3
    }

    if ($run.Status -eq "WAITING") {
        Write-Output "AGY_NEEDS_PERMISSION (model=$model): $($run.Error)"
        $lastFailureCode = 4
        continue
    }

    Write-Output "AGY_ERROR (model=$model, status=$($run.Status), exit=$($run.ExitCode)): $($run.Error)"
    if ($run.Result.response) { Write-Output $run.Result.response }
    $lastFailureCode = 1
}

if ($lastFailureCode -ne 0) {
    Write-Output "AGY_ALL_MODELS_FAILED (last failure code $lastFailureCode); quota exhausted: $($exhausted -join ',')"
    exit $lastFailureCode
}

Write-Output "AGY_ALL_QUOTA_EXHAUSTED: $($exhausted -join ',')"
exit 10
