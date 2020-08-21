Param(
    [int]$max = 10000,
    [int]$numThreads = 8
)

Set-StrictMode -Version "latest"
$ErrorActionPreference = "stop"
$DebugPreference='continue'
$VerbosePreference = "continue"

$scriptblock = {
    Param(
        $pid_,
        $host_,
        [ref]$buffer_,
        $max_
    )

    Start-Sleep -Seconds 1

    $ThreadID = [appdomain]::GetCurrentThreadId()

    $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID starting, initial buffer value: " + $buffer_.Value)

    while($buffer_.Value -lt $max_){
        $buffer_.Value++
    }

    $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID ending, current buffer value: " + $buffer_.Value)
}

$runspaces = @()
$buffer = 1

for($i=1; $i -le $numThreads; $i++){
    "Creating thread # $i"

    #creates runspace
    $runspace = [PowerShell]::Create()
    $null = $runspace.AddScript($scriptBlock)
    $null = $runspace.AddArgument($PID)
    $null = $runspace.AddArgument($host)
    $null = $runspace.AddArgument([ref]$buffer)
    $null = $runspace.AddArgument($max)
     
    # Add runspace to runspaces collection and "start" it
    $runspaces += [PSCustomObject]@{ Pipe = $runspace; Status = $runspace.BeginInvoke() }
}

while ($runspaces.Status.IsCompleted -notcontains $true) {} # Wait for all runspaces to finish

$runspaces | %{$_.Pipe.EndInvoke($_.Status); $_.Pipe.Dispose();}

"Value of buffer at the end of invocation: $buffer"