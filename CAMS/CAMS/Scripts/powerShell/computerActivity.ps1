#
# computerActivity.ps1
#


function Get-LoggedOnUser
 {
     [CmdletBinding()]
     param
     (
         [Parameter()]
         [ValidateScript({ Test-Connection -ComputerName $_ -Quiet -Count 1 })]
         [ValidateNotNullOrEmpty()]
         [string[]]$ComputerName = $env:COMPUTERNAME
     )
     foreach ($comp in $ComputerName)
     {
         $output = (Get-WmiObject -Class win32_computersystem -ComputerName $comp).UserName
         [PSCustomObject]$output
     }
 }

write-output @(Get-LoggedOnUser -ComputerName $compName)
write-output @(Test-Connection -BufferSize 32 -Count 1 -ComputerName $compName -Quiet)


#backround job
#$job = Test-Connection -ComputerName (Get-Content "Servers.txt") -AsJob
#if ($job.JobStateInfo.State -ne "Running") {$Results = Receive-Job $job}
