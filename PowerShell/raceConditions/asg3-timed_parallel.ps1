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
        $file_,
        $max_,
        $mtxName_
    )

    $begin = Get-Date
    Start-Sleep -Seconds 1
          
    [int]$buffer_ = Get-Content $file_
    $mtx_ = [System.Threading.Mutex]::OpenExisting($mtxName_)

    while ($true) {
        if($mtx_.WaitOne()){
            if($buffer_ -lt $max_){
                #when buffer < max, increment buffer by 1
                #release mutex right away, so more than one Process can do the task
                $buffer_ ++
                $buffer_ > $file_
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
            $host_.ui.WriteVerboseLine("Mutex access denied")
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

$jobs = @()
$mtxName = "psJobsMutex"
$mtx = [System.Threading.Mutex]::new($false, $mtxName)

$file = "$PWD/buffer.txt"
0 > $file

for($i=1; $i -le $numThreads; $i++){
    "Creating psJob # $i"

    #creates jobs and start it
    $job = Start-Job -ScriptBlock $scriptblock -ArgumentList $PID, $Host, $file, $max, $mtxName
     
    # Add job to jobs collection
    $jobs += $job
}

$jobs | %{$null = Wait-Job $_} #wait for all the psJobs to finish
$completed = Get-Date

$spinup = @()   #array of begin time execution of all psJobs
$exit = @()     #array of end time execution of all psJobs
foreach($job in $jobs){
    $result = Receive-Job $job
    $spinup += $result[0] #recieves begin time execution of this psJob
    $exit += $result[1] #recieves end time execution of this psJob
}
$received = Get-Date
$mtx.Dispose()

#computations
$timeToLaunchArr = $spinup | %{($_ - $start).TotalMilliseconds} #get the time launch for each psJob
$timeToLaunch = GetAverage $timeToLaunchArr $numThreads         #then get the average

$timeToExitArr = $exit | %{($completed - $_).TotalMilliseconds} #get the time to exit for each psJob
$timeToExit = GetAverage $timeToExitArr $numThreads             #then get the average

$timeToRunCommandArr = 0..($numThreads-1) | %{($exit[$_] - $spinup[$_]).TotalMilliseconds} #get the time to run command for each psJob
$timeToRunCommand = GetAverage $timeToRunCommandArr $numThreads                            #then get the average

$timeToReceive = ($received - $completed).TotalMilliseconds

#output
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to set up background job', $timeToLaunch
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to run code', $timeToRunCommand
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to exit background job', $timeToExit
'{0,-30} : {1,10:#,##0.00} ms' -f 'Time to receive results', $timeToReceive

#get final buffer value
$buffer = Get-Content $file
"Value of buffer at the end of invocation: $buffer"
