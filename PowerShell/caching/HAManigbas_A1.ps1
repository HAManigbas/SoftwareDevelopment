set-strictmode -version latest
#$DebugPreference        = 'SilentlyContinue'# disable write-debug output
$DebugPreference        = 'Continue'        # enable write-debug output
$ErrorActionPreference  = 'Stop'            # https://stackoverflow.com/questions/15545429/erroractionpreference-and-erroraction-silentlycontinue-for-get-pssessionconfigur
                                            # The ErrorAction can be used to convert non-terminating errors to terminating errors using the parameter value Stop. 
                                            # It can't help you ignore terminating errors. If you want to ignore, use a try { Stop-Transcript } catch {}


Clear-Host

$TOTAL_OPS = 10000
$TOTAL_DATA_ITEMS = 10000
$NUM_REGISTERS = 32
$CACHE_SIZE = 1024

# Response time of storage methods
$REGISTER_ACCESS_TIME = 0.25
$CACHE_ACCESS_TIME = 2
$RAM_ACCESS_TIME = 100
$DISK_ACCESS_TIME = 40000

# Time to transfer a single 64 bit word
$CACHE_BANDWIDTH_TIME   = ([Math]::Pow(10,9) / (700*[Math]::Pow(1024,3) / 8))
$RAM_BANDWIDTH_TIME     = ([Math]::Pow(10,9) / (15*[Math]::Pow(1024,3) / 8))
$DISK_BANDWIDTH_TIME    = ([Math]::Pow(10,9) / (200*[Math]::Pow(1024,2) / 8))

$RAM_HIT_PERCENTAGE = 90
$CACHE_REPLACEMENT_POLICIES = @('FIFO','LFU')
$INPUTFILE = ".\dataset.txt"

# $storage is the ONLY permitted global variable (as opposed to global constants)
$storage = [PSCustomObject]@{
    Registers   = New-Object System.Collections.Generic.List[Int]
    Cache       = $null #will be set later to either list of int or hash depending on the cache replacement policy
} 

function setFIFO($storageType, $storageSize, $item_){

    $found = $storageType.Contains($item_)  
    if($found){ #item in memory
        $storageType.Remove($item_)
    }
    elseif ($storageType.count -eq $storageSize) { #memory full
        $storageType.RemoveAt(0)
    }

    $storageType.add($item_) #add the new item at the end of the queue
    return $found
}

function setRegisters($item_)
{
    # return true/false if the item is in the registers
    # ...also set the registers appropriately

    #write-debug "Looking for $item_ in registers"

    $found = setFIFO $storage.Registers $NUM_REGISTERS $item_
    return $found
}

function testSetRegisters() 
{
    # Arrays vs arraylists
    # https://www.jonathanmedd.net/2014/01/adding-and-removing-items-from-a-powershell-array.html
    $testData=New-Object System.Collections.ArrayList
    foreach($i in 1..$NUM_REGISTERS){
        $item = get-random -maximum $TOTAL_OPS
        $testData.add($item) | Out-Null   # add() is misbehaved
        setRegisters $item | Out-Null # don't need output
    }
    
    $a = Compare-Object -ReferenceObject ($testData | Select-Object -uniq) -DifferenceObject $storage.Registers -PassThru
    if($a){ # Is the register object in the format we expect?
        Write-Error "Argh! Broken setRegisters()"
        exit
    }

    $exists = setRegisters $testData[0]
    if(!$exists){ # i.e. item not found even though we added it
        Write-Error "Argh! Broken setRegisters()"
        exit
    }

    $newItem = get-random -maximum $TOTAL_DATA_ITEMS
    while($testData.Contains($newItem)){
        $newItem = get-random -maximum $TOTAL_DATA_ITEMS
    }

    $exists = setRegisters $newItem
    if($exists){ # i.e. item found even though it doesn't exist
        Write-Error "Argh! Broken setRegisters()"
        exit
    }
    
    if($storage.Registers[$NUM_REGISTERS - 1] -ne $newItem){
        Write-Error "Argh! Broken setRegisters()"
        exit
    }
}



function fetchEmptyStatsObject(){
    $stats = [PSCustomObject]@{ 
        'totalTime' = 0;    'regHits' = 0;      'cacheHits' = 0;    'ramHits' = 0;
        'diskHits'  = 0;     'regTime' = 0;     'cacheTime' = 0;    'ramTime' = 0;
        'diskTime'  = 0;
    }

    $stats | Add-Member -MemberType ScriptMethod -Name "show" -Force -Value {
        # Outputs the cache stats for a particular cache type

        'Overall Performance (ns): {0,45}' -f [math]::round($this.totalTime,2)
        'Cache Hit Ratio: {0,54:p2}' -f ($this.cacheHits / ($this.cacheHits+($TOTAL_DATA_ITEMS - $this.cacheHits - $this.regHits)))
        ''
        'Cache Hits: {0,50} ({1:p2})' -f $this.cacheHits, ($this.cacheHits/$TOTAL_DATA_ITEMS)
        'Cache Misses: {0,57}' -f ($TOTAL_DATA_ITEMS - $this.cacheHits - $this.regHits)
        ''
        'Register Hits: {0,49} ({1:p2})' -f $this.regHits, ($this.regHits/$TOTAL_DATA_ITEMS)
        'RAM Hits: {0,53} ({1:p2})' -f $this.ramHits, ($this.ramHits/$TOTAL_DATA_ITEMS)
        'Disk Hits: {0,53} ({1:p2})' -f $this.diskHits, ($this.diskHits/$TOTAL_DATA_ITEMS)
        ''
        'Register Time: {0,46} NS ({1:p2})' -f [math]::round($this.regTime,2), ($this.regTime/$this.totalTime)
        'Cache Time: {0,49} NS ({1:p2})' -f [math]::round($this.cacheTime,2), ($this.cacheTime/$this.totalTime)
        'RAM Time: {0,51} NS ({1:p2})' -f [math]::round($this.ramTime,2), ($this.ramTime/$this.totalTime)
        'Disk Time: {0,49} NS ({1:p2})' -f [math]::round($this.diskTime,2), ($this.diskTime/$this.totalTime)
    }

    return $stats
}


function setCache($cacheType_, $item_, $opNum_)
{
    # Return true on cache hit, else false.  Call correct caching function
    $found = $false
    if($cacheType_ -eq 'FIFO'){
        $found = setFIFOCache $item_
    }
    elseif($cacheType_ -eq 'LFU'){
        $found = setLFUCache $item_ $opNum_
    }
    return $found
}

function setFIFOCache($item_){
    # Given an item request, set the cache

    $found = setFIFO $storage.Cache $CACHE_SIZE $item_
    return $found
}



function testSetFIFOCache() 
{
    $testData=New-Object System.Collections.ArrayList
    
    foreach($head in 1..($CACHE_SIZE)){
        $item = get-random -maximum $TOTAL_DATA_ITEMS
        $testData.add($item) | Out-Null   # add() is misbehaved
        setFIFOCache $item | Out-Null
    }

    $a = Compare-Object -ReferenceObject ($testData | Select-Object -uniq) -DifferenceObject $storage.cache -PassThru
    if($a){ # Is the cache object in the format we expect?
        Write-Error "Argh! Broken SetFIFOCache()"
        exit
    }

    foreach($head in 1..($CACHE_SIZE)){ # do it again to cover duplicate items (i.e. cache not yet full condition)
        $item = get-random -maximum $TOTAL_DATA_ITEMS
        $testData.add($item) | Out-Null   # add() is misbehaved
        setFIFOCache $item | Out-Null
    }

    if($storage.cache.count -ne $CACHE_SIZE){
        Write-Error "Argh! Weird cache size, coincidence?"
        exit
    }
    
    $exists = setCache 'FIFO' $testData[-1]
    if(!$exists){ # i.e. item not found even though we added it
        Write-Error "Argh! Broken SetFIFOCache()"
        exit
    }

    $newItem = get-random -maximum $TOTAL_DATA_ITEMS
    while($testData.Contains($newItem)){
        $newItem = get-random -maximum $TOTAL_DATA_ITEMS
    }

    $exists = setCache 'FIFO' $newItem
    if($exists){ # i.e. item found even though it doesn't exist
        Write-Error "Argh! Broken SetFIFOCache()"
        exit
    }
    
    if($storage.cache[-1] -ne $newItem){
        Write-Error "Argh! Broken SetFIFOCache()"
        exit
    }
}



function setLFUCache($item_, $opNum_){
    # Given an item request, set the cache

    $found = $false

    foreach($val in $storage.Cache){
        if($found = ($val.Value -eq $item_)){ #item in memory, update the Counter(Occurences) and OpNum(Age)
            $val.Counter++
            $val.OpNum = $opNum_
            break
        }
    }

    if(!$found){ #item not in memory
        if(!($storage.Cache.Count -eq $CACHE_SIZE)){ #memory not full, add the new item
            $storage.Cache.Add(@{
                Value = $item_
                OpNum = $opNum_
                Counter = 1
            })
        }
        else{ 
            #memory full, checks for the lowest counter that has the oldest age 
            #and then remove that item to give space for the new item

            $removeIndex = 0;   #item to be remove in memory
            $removeItem_OpNum = $storage.Cache[$removeIndex].OpNum #oldest age
            $removeItem_Counter = $storage.Cache[$removeIndex].Counter #most number of occurences

            foreach($i in 0..($CACHE_SIZE - 1)){
                $checkItem = $storage.Cache[$i] #current item to be check

                if($checkItem.Counter -eq $removeItem_Counter){
                    if($checkItem.OpNum -lt $removeItem_OpNum){
                        $removeIndex = $i #update the index of the item to remove
                        $removeItem_OpNum = $checkItem.OpNum #update the oldest age
                    }
                }
                elseif($checkItem.Counter -lt $removeItem_Counter){
                    $removeIndex = $i #update the index of the item to remove
                    $removeItem_Counter = $checkItem.Counter #update the number of most occurences
                }
            }

            $storage.Cache.RemoveAt($removeIndex) #remove the least frequency used item

            $storage.Cache.Add(@{ #add new item
                Value = $item_
                OpNum = $opNum_
                Counter = 1
            })
        }
    }

    return $found
}


function testSetLFUCache(){
    $item = 12
    $reps = 4

    for($i=0; $i -lt $reps; $i++){
        setCache 'LFU' $item | out-null
    }

    if($storage.cache[$reps-1][0] -ne $item){
        Write-Error "Argh! Weird cache values, broken setLFUCache()"
        exit
    }

    if($storage.cache[0].Count -ne 0 -or $storage.cache[1].Count -ne 0 -or $storage.cache[2].Count -ne 0){
        Write-Error "Argh! Weird cache values, broken setLFUCache()"
        exit
    }
}


function clearStorage(){
    # Clears the storage object so that it can be reused between cache types
    
    $storage.Registers.Clear()
    $storage.Cache = $null
}



# main body

$items = Get-Content $INPUTFILE  # All of the data items to process
Get-Random -maximum 100 -SetSeed 2019 | out-null

foreach($cacheType in $CACHE_REPLACEMENT_POLICIES){
    clearStorage
    $stats = fetchEmptyStatsObject

    if($cacheType -eq 'FIFO'){
        $storage.Cache = New-Object System.Collections.Generic.List[Int]
    }
    elseif ($cacheType -eq 'LFU') {
        $storage.Cache = New-Object System.Collections.Generic.List[PSObject]
    }

    Write-Output "________________________________________________________________________"
    Write-Output "${cacheType}:"

    for($opNum = 0; $opNum -lt $TOTAL_OPS; $opNum++){
        $currItem = $items[$opNum]
        

        if(setRegisters $currItem $opNum){ # item was in registers, we're done!
            $stats.regHits++
            $stats.regTime += $REGISTER_ACCESS_TIME
            $stats.totalTime += $REGISTER_ACCESS_TIME
            continue
        }

        if(setCache $cacheType $currItem $opNum){
            $stats.cacheHits++
            $stats.regTime += $REGISTER_ACCESS_TIME
            $stats.cacheTime += ($CACHE_ACCESS_TIME + $CACHE_BANDWIDTH_TIME)
            $stats.totalTime += ($REGISTER_ACCESS_TIME + $CACHE_ACCESS_TIME + $CACHE_BANDWIDTH_TIME)
            continue
        }

        if((Get-Random -maximum 100) -lt $RAM_HIT_PERCENTAGE){ # if in RAM...
            $stats.ramHits++
            $stats.regTime += $REGISTER_ACCESS_TIME
            $stats.cacheTime += ($CACHE_ACCESS_TIME + $CACHE_BANDWIDTH_TIME)
            $stats.ramTime += ($RAM_ACCESS_TIME + $RAM_BANDWIDTH_TIME)
            $stats.totalTime += ($REGISTER_ACCESS_TIME + $CACHE_ACCESS_TIME + $RAM_ACCESS_TIME + $CACHE_BANDWIDTH_TIME + $RAM_BANDWIDTH_TIME)
            continue
        }
        else {
            $stats.diskHits++
            $stats.regTime += $REGISTER_ACCESS_TIME
            $stats.cacheTime += ($CACHE_ACCESS_TIME + $CACHE_BANDWIDTH_TIME)
            $stats.ramTime += ($RAM_ACCESS_TIME + $RAM_BANDWIDTH_TIME)
            $stats.diskTime += ($DISK_ACCESS_TIME + $DISK_BANDWIDTH_TIME)
            $stats.totalTime += ($REGISTER_ACCESS_TIME + $CACHE_ACCESS_TIME + $RAM_ACCESS_TIME + $DISK_ACCESS_TIME + $CACHE_BANDWIDTH_TIME + $RAM_BANDWIDTH_TIME + $DISK_BANDWIDTH_TIME)
        }

    }

    $stats.show()
}
