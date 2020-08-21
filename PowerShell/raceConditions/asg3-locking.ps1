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
        $max_,
        $mtx_
    )

    Start-Sleep -Seconds 1

    $ThreadID = [appdomain]::GetCurrentThreadId()

    $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID starting, initial buffer value: " + $buffer_.Value)

    while ($true) {
        $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID wants to access the Mutex, current buffer value: " + $buffer_.Value)

        if($mtx_.WaitOne()){
            $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID Mutex access permitted, current buffer value: " + $buffer_.Value)

            if($buffer_.Value -lt $max_){
                #when buffer < max, increment buffer by 1
                #release mutex right away, so more than one thread can do the task
                $buffer_.Value++
                $null = $mtx_.ReleaseMutex()
                $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID releases Mutex, current buffer value: " + $buffer_.Value)
            }
            else{
                #do nothing with buffer when buffer == max
                #just release the mutex then force exit the loop
                $null = $mtx_.ReleaseMutex()
                $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID releases Mutex, $max_ == " + $buffer_.Value + " (max == buffer)")
                break
            }
        }
        else {
            $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID Mutex access denied")
        }
    }

    $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID ending, current buffer value: " + $buffer_.Value)
}

$runspaces = @()
$buffer = 1
$mtx = New-Object System.Threading.Mutex

for($i=1; $i -le $numThreads; $i++){
    "Creating thread # $i"

    #creates runspace
    $runspace = [PowerShell]::Create()
    $null = $runspace.AddScript($scriptBlock)
    $null = $runspace.AddArgument($PID)
    $null = $runspace.AddArgument($host)
    $null = $runspace.AddArgument([ref]$buffer)
    $null = $runspace.AddArgument($max)
    $null = $runspace.AddArgument($mtx)
     
    # Add runspace to runspaces collection and "start" it
    $runspaces += [PSCustomObject]@{ Pipe = $runspace; Status = $runspace.BeginInvoke() }
}


while ($runspaces.Status.IsCompleted -notcontains $true) {} # Wait for all runspaces to finish

$runspaces | %{$_.Pipe.EndInvoke($_.Status); $_.Pipe.Dispose();}

$mtx.Dispose()
rv runspaces

"Value of buffer at the end of invocation: $buffer"