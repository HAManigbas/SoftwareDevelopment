Clear-Host

$userNames_Arr = @("Awan", "Bracken", "Burnie", "Cameron", "Conard", "Cordiano", "DiPasquale", 
    "Domingo", "Eichler","Fedele","Goral","Grant","GrippaVento","HansenAyulo","Lacroix","Lee",
    "Ma","Manigbas","Mattsson","Mayoff","Meath","Noul","Oiknine","Provost","Reinhart","ReyesGarcia",
    "Rogers","Singh","Stewart","TrudeauDumas","Tsitirides","Valliere","Wang","Yates")

$serverName = "Manigbas-DC.ManigbasDC.net"
$path_ = "OU=ITAdmins,OU=DCUsers,DC=ManigbasDC,DC=net"

#groupNames
$tchr = "teachers"
$stdnt = "students"

#default password for students
$defPass = "Password123"

function generateRandomPassword(){
    $num = Get-Random -Minimum 101 -Maximum 9999
    $pass = "user-DC@" + $num

    echo $pass
}

function createAccount($username_, $pass_, $group_){
    $principalName = $username_ + "@ManigbasDC.net"
    $identity = "CN=$username_,OU=ITAdmins,OU=DCUsers,DC=ManigbasDC,DC=net"
    $groupIdentity = "CN=$group_,OU=ITAdmins,OU=DCUsers,DC=ManigbasDC,DC=net"
    $Secure_String_Pwd = ConvertTo-SecureString $pass_ -AsPlainText -Force

    New-ADUser -DisplayName:$username_ -Name:$username_ -Path:$path_ -SamAccountName:$username_ -Server:$serverName -StreetAddress:$null -Title:$group_ -Type:"user" -UserPrincipalName:$principalName

    Set-ADAccountPassword -Identity:$identity -NewPassword:$Secure_String_Pwd -Reset:$true -Server:$serverName

    Enable-ADAccount -Identity:$identity -Server:"Manigbas-DC.ManigbasDC.net"

    Set-ADAccountControl -AccountNotDelegated:$false -AllowReversiblePasswordEncryption:$false -CannotChangePassword:$false -DoesNotRequirePreAuth:$false -Identity:$identity -PasswordNeverExpires:$false -Server:$serverName -UseDESKeyOnly:$false

    Set-ADUser -ChangePasswordAtLogon:$true -Identity:$identity -Server:$serverName -SmartcardLogonRequired:$false

    Add-ADPrincipalGroupMembership -Identity:$identity -MemberOf:$groupIdentity -Server:$serverName
}

function createGroup($groupName_){
    $identity = "CN=$groupName_,OU=ITAdmins,OU=DCUsers,DC=ManigbasDC,DC=net"
    New-ADGroup -GroupCategory:"Security" -GroupScope:"Global" -Name:$groupName_ -Path:$path_ -SamAccountName:$groupName_ -Server:$serverName
}


#main

#create groups
createGroup $tchr
createGroup $stdnt

#create a user 'Ivan'
$passForIvan = generateRandomPassword 'Ivan'
createAccount 'Ivan' $passForIvan $tchr
Add-ADPrincipalGroupMembership -Identity:"CN=Ivan,OU=ITAdmins,OU=DCUsers,DC=ManigbasDC,DC=net" -MemberOf:"CN=Domain Admins,CN=Users,DC=ManigbasDC,DC=net" -Server:$serverName

# create account for the students
foreach($userStdnt in $userNames_Arr){
    createAccount $userStdnt $defPass $stdnt
    echo "Account created for $userStdnt with default password"
}

#echo $passForIvan

#Hazel Ann Manigbas