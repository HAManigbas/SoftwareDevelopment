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

    $begin = Get-Date
    Start-Sleep -Seconds 1
    $ThreadID = [appdomain]::GetCurrentThreadId()

    while ($true) {
        if($mtx_.WaitOne()){
            if($buffer_.Value -lt $max_){
                #when buffer < max, increment buffer by 1
                #release mutex right away, so more than one thread can do the task
                $buffer_.Value++
                $null = $mtx_.ReleaseMutex()
            }
            else{
                #do nothing with buffer when buffer == max
                #just release the mutex then force exit the loop
                $null = $mtx_.ReleaseMutex()
                break
            }
        }
        else {
            $host_.ui.WriteVerboseLine("PID: $pid_, ThreadID: $ThreadID Mutex access denied")
        }
    }

    $end = Get-Date

    $begin
    $end
}

function GetAverage($items_, $size_){
    $sum_ = 0

    foreach($item in $items_){
        $sum_ += $item
    }

    $average_ = $sum_ / $size_
    $average_
}

$start = Get-Date

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
$completed = Get-Date

$spinup = @()   #array of begin time execution of all psJobs
$exit = @()     #array of end time execution of all psJobs
foreach($runspace in $runspaces){
    $result = $runspace.Pipe.EndInvoke($runspace.Status)
    $spinup += $result[0] #recieves begin time execution of this thread
    $exit += $result[1] #recieves end time execution of this thread
}
$received = Get-Date

#clean up runspaces and the mutex
$runspaces | %{$_.Pipe.Dispose();}
$mtx.Dispose()
rv runspaces
$cleanup = Get-Date

#computations
$timeToLaunchArr = $spinup | %{($_ - $start).TotalMilliseconds} #get the time launch for each thread
$timeToLaunch = GetAverage $timeToLaunchArr $numThreads         #then get the average

$timeToExitArr = $exit | %{($completed - $_).TotalMilliseconds} #get the time to exit for each thread
$timeToExit = GetAverage $timeToExitArr $numThreads             #then get the average

$timeToRunCommandArr = 0..($numThreads-1) | %{($exit[$_] - $spinup[$_]).TotalMilliseconds} #get the time to run command for each thread
$timeToRunCommand = GetAverage $timeToRunCommandArr $numThreads                            #then get the average

$timeToReceive = ($received - $completed).TotalMilliseconds
$timeToCleanup = ($cleanup - $received).TotalMilliseconds

#output
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to set up background job', $timeToLaunch
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to run code', $timeToRunCommand
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to exit background job', $timeToExit
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to receive results', $timeToReceive
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to cleanup runspace', $timeToCleanup

"Value of buffer at the end of invocation: $buffer"