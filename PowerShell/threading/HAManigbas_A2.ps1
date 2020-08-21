Param(
    $maxConcurrentThreads = 16,
    $bufferSize = 2000
)

Set-StrictMode -Version latest
$ErrorActionPreference='stop'
$DebugPreference='continue'
$VerbosePreference = "continue"


#Clear-Host

$NUM_ITEMS = 200000
$INPUTFILE = ".\asg2-200000.txt"

$scriptBlock = {
    Param(
        $numbers_
    )

    function isPrime($num_){
        $prime = $true
    
        if($num_ -eq 1){
            $prime = $false   
        }
        elseif ($num_ -eq 2) {
            $prime = $true
        }
        elseif($num_ % 2 -eq 0){
            $prime = $false
        }
        else {
            for($i = 3; $i -le [System.Math]::Sqrt($num_); $i+=2){
                if($num_ % $i -eq 0){
                    $prime = $false
                    break
                }
            }
        }
        $prime
    }

    $sum = 0
    foreach($n in $numbers_){
        if(isPrime $n){
            $sum += $n
        }
    }
    $sum
}

$items = Get-Content $INPUTFILE  # All of the data items to process
$runspaces = @()
$currentIndex = 0

for($i = $bufferSize; $i -le $NUM_ITEMS; $i+=$bufferSize){
    $numbers = New-Object System.Collections.ArrayList
    $running=0
    if($runspaces){
        $running=@($runspaces | Where-Object { $_.Status.IsCompleted -eq $false }).Length
    }
    while($running -eq $maxConcurrentThreads){ # Already $maxConcurrentThreads running threads, wait until one finishes...
        Start-Sleep -Milliseconds 200
        $running=@($runspaces | Where-Object { $_.Status.IsCompleted -eq $false }).Length
    }

    for($numOfVal = 0; $numOfVal -lt $bufferSize; $numOfVal++){
        $numbers.Add($items[$currentIndex++]) | Out-Null
    }

    $runspace = [PowerShell]::Create()
    $null = $runspace.AddScript($scriptBlock)
    $null = $runspace.AddArgument($numbers)

    $runspaces += [PSCustomObject]@{
        Pipe = $runspace;
        Status = $runspace.BeginInvoke()
    }
}

#write-output ([string]$runspaces.Count + " total runspaces created")

while ($runspaces.Status.IsCompleted -contains $false) {} # Wait for all runspaces to finish

$totalSum = 0
foreach($runspace in $runspaces){
    #$result=($runspace.Pipe.EndInvoke($runspace.Status))[0]
    #Write-Output ("ThreadID: " + $result.tid + " Sum: " + $result.sum)
    $totalSum += ($runspace.Pipe.EndInvoke($runspace.Status))[0]
    $runspace.Pipe.Dispose()
}
Write-Output $totalSum
rv items
rv runspaces