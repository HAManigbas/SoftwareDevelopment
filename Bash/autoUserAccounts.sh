#!/bin/bash
set -u

# Hazel Ann Manigbas
# 1508630

#-------------------------------------------------------------------------------
# This script will read a file that contains the information of the new hires
# Before processing the file, it will test the file if there are errors
# If an error found in the file, the script will exit and output the error message
# If no error found, it will then create an account for the new hires
# and add the, on their respective group accounts
#---------------------------------------------------------------------------------



#------------------------------GLOBAL VARIABLES--------------------------------
userPath="/etc/passwd"
groupPath="/etc/group"
newHire_File=""
defaultPass="Password123"
FIELDNUM=5
declare -a _newHireInfos
declare -Arx FIELDNAMES=( [lname]=0 [fname]=1 [dept]=2 [group]=3 [phoneNum]=4 )



#------------------------------------------------------------------------------
# reads the csv file that contains the informations of the new hires
# then saves the informations to the array _newHireInfos
#------------------------------------------------------------------------------
function readFile(){
	readarray _newHireInfos < $newHire_File
}

#------------------------------------------------------------------------------
# exits the script and prints the error message
#------------------------------------------------------------------------------
function bail_out(){
	local errMsg=$1
	
	usage
	
	printf "MESSAGE: $errMsg \n" 1>&2

	exit 0
}

#------------------------------------------------------------------------------
# reminds the user of the correct syntax
#------------------------------------------------------------------------------
function usage(){
	printf "\n\nNOTE: must run as sudo \n" 1>&2
	printf "Usage: $0 [-t] [-f filename] \n\n" 1>&2
}


#------------------------------------------------------------------------------
# Checks each line of the file if there is a missing field on the new hire's infos
# checks for duplicates
# prints error messages
# exit if there are error
#-------------------------------------------------------------------------------
function testFile(){
	local errFlag=0
	local fNameIdx=${FIELDNAMES["fname"]}
	local lNameIdx=${FIELDNAMES["lname"]}

	# Column Headers @ index 0, so start @ index 1
	for (( j=1;j<${#_newHireInfos[@]};j++ )); do
		local curr=$(echo "${_newHireInfos[$j]}" | tr -dc "[:alnum:]," )
		local _arr=(${curr//,/ })

		local fieldCount=${#_arr[@]}
		
		if [[ $fieldCount -ne $FIELDNUM ]]; then
			errFlag=1
			printf "\nERROR: Missing information for ${_arr[*]} (entry #$j)" 1>&2
		fi

		checkForDuplicate $j ${_arr[$fNameIdx]} ${_arr[$lNameIdx]} "isDuplicate"

		if [[ $isDuplicate -eq 1 ]]; then
			errFlag=1
		fi
	done

	if [[ $errFlag -eq 1 ]]; then
		bail_out "Errors above needs to be fixed to process the file"
	fi
}


#--------------------------------------------------------------------------------
# checks if there is a duplicate
# compare the current new hire's first amd last name if it has exactly the same
#		to the other new hires
# sets the flag to duplicate to 1 if there are duplicates
#--------------------------------------------------------------------------------
function checkForDuplicate(){
	local currIdx=$1
	local fname=$2
	local lname=$3
	local toReturn=$4
	local duplicate_=0
	local idxFname=${FIELDNAMES["fname"]}
	local idxLname=${FIELDNAMES["lname"]}

	for (( n=1;n<${#_newHireInfos[@]};n++ )); do
		if [[ $n -ne $currIdx ]]; then
			local curr_=$(echo "${_newHireInfos[$n]}" | tr -dc "[:alnum:]," )
			local arr_=(${curr_//,/ })
				
			if [[ ${arr_[$idxLname]} == $lname && ${arr_[$idxFname]} == $fname ]]; then
				duplicate_=1
				printf "\nERROR: Duplicate found for $fname $lname (entry #$currIdx and entry #$n)" 1>&2
				break
			fi
		fi
	done

	eval "$toReturn=$duplicate_"
}




#------------------------------------------------------------------------------
#parameter: all information for the current new hire
#
# extracts the first and last name of the current hire
# then remove all unessecary special characters and spaces to their names
# set their username as "lastname.firstnameInitial" 
# or if the username exists, add a number after
#
# return: return the username for the current new hire
#------------------------------------------------------------------------------
function setUsername(){
	local newHire_=$1
	local toReturn=$2

	local arrHire=(${newHire_//,/ })
	local lastNameIndex=${FIELDNAMES["lname"]}
	local firstNameIndex=${FIELDNAMES["fname"]}

	local lastName=${arrHire[$lastNameIndex]}
	lastName=$(echo "$lastName" | awk '{print tolower($0)}')
	lastName=$(echo "$lastName" | tr -dc "[:alnum:]")

	local firstName=${arrHire[$firstNameIndex]}
	firstName=$(echo "$firstName" | awk '{print tolower($0)}')
	firstName=$(echo "$firstName" | tr -dc "[:alnum:]")
	
	local initial=$(echo "$firstName" | head -c 1)

	local username="$lastName.$initial"

	local num=0

	local exists=$(grep -c "^$username:" $userPath)

	while [[ $exists -ne 0 ]]; do
		num=$(( num+1 ))
		username="$username$num"
		exists=$(grep -c "^$username:" $userPath)
	done

	eval "$toReturn=$username"	
}


#--------------------------------------------------------------------------------
# parameter: info about the current new hire
#
# extracts the department name from the information of the current hire
# then check if the department exists, if not add a group for that department
# checks also if the directory for the department already exists, if not make the dir
# 
# return: the department name
#--------------------------------------------------------------------------------
function setDepartment(){
	local newHire_=$1
	local toReturn=$2

	local arr=(${newHire_//,/ })
	local deptIndex=${FIELDNAMES["dept"]}

	local dept=${arr[$deptIndex]}
	dept=$(echo "$dept" | awk '{print tolower($0)}')
	dept=$(echo "$dept" | tr -dc "[:alnum:]")

	local exists=$(grep -c "^$dept:" $groupPath)

	if [[ $exists -eq 0 ]]; then
		groupadd $dept
	fi

	checkDirectory "/home/$dept" 0
	checkDirectory "/etc/$dept" 1

	eval "$toReturn=$dept"
}


#--------------------------------------------------------------------------------
# parameter: info about the current new hire
#
# extracts the group name from the information of the current hire
# then check if the ggroup exists, if not add a group for that group
# checks also if the directory for the group already exists, if not make the dir
#
# return: the group name
#--------------------------------------------------------------------------------
function setGroup(){
	local newHire_=$1
	local dept_=$2
	local toReturn=$3

	local arr=(${newHire_//,/ })
	local groupIndex=${FIELDNAMES["group"]}

	local group=${arr[$groupIndex]}
	group=$(echo "$group" | awk '{print tolower($0)}')
	group=$(echo "$group" | tr -dc "[:alnum:]")

	local exists=$(grep -c "^$group:" $groupPath)

	if [[ $exists -eq 0 ]]; then
		groupadd $group
	fi

	checkDirectory "/home/$dept_/$group" 0
	
	eval "$toReturn=$group"
}


#---------------------------------------------------------------------------------
# checks if the directory for dept or the group exists
# if it does not exist, make the directory then change the permission
# 
# if the flag for skel directory is also set to 1
# then copy the files from /etc/skel to the skel directory of the current dept
#----------------------------------------------------------------------------------
function checkDirectory(){
	local path_=$1
	local isSkel=$2
	local skelPath="/etc/skel"

	if [[ -d $path_ ]]; then
		chmod 755 $path_	#make sure I have access to the directory, since it is owned by the root
	else
		mkdir -p $path_
		chmod 755 $path_

		if [[ $isSkel -eq 1 ]]; then
			cp -r "$skelPath" $path_
		fi
	fi
}


#---------------------------------------------------------------------------------------------
# creates account for the current hire
# add the current new hire to their respective dept and group
# make their own home directory
# set a default password and will also force them to change password on their first log in
# set the umask
#---------------------------------------------------------------------------------------------
function setUpTheUser(){
	local userInfo_=$1
	local theUser_=$2
	local theDept_=$3
	local theGroup_=$4

	local arr=(${userInfo_//,/ })
	local phoneIdx=${FIELDNAMES["phoneNum"]}
	local firstNameIdx=${FIELDNAMES["fname"]}
	local lastNameIdx=${FIELDNAMES["lname"]}
	
	local fname=${arr[$firstNameIdx]}
	local lname=${arr[$lastNameIdx]}
	local phoneNum=${arr[$phoneIdx]}

	local path_="/usr/local/$theDept_/bin:$PATH"	#change PATH in .bashrc file to this path
	local path_home="/home/$theDept_/$theGroup_"	#home directory of the user
	
	useradd -m -b $path_home $theUser_ -g $theGroup_ -G $theDept_ -k "/etc/$theDept_/skel" -c "$fname $lname $phoneNum"
	echo $defaultPass | passwd --stdin $theUser_
	chage -M 30 $theUser_
	passwd -e $theUser_

	#administration department will have different permissions on file and directories
	if [[ $theDept_ == "administration" ]]; then
		chmod 711 "$path_home/$theUser_"
		cd "$path_home/$theUser_"
		echo "PATH=$path_" >> .bashrc
		echo "umask 177" >> .bashrc
	else
		chmod 751 "$path_home/$theUser_"
		cd "$path_home/$theUser_"
		echo "PATH=$path_" >> .bashrc
		echo "umask 137" >> .bashrc
	fi

	display $fname $lname $theUser_ "$path_home/$theUser_"
}


#--------------------------------------------------------------------------------
#if account is successfully created, it prints some info about the acount that was created
#---------------------------------------------------------------------------------
function display(){
	local fname_=$1
	local lname_=$2
	local username_=$3
	local homeDir_=$4

	printf "\n\n\t $fname $lname \n"
	printf "\t Username: $username_ \n"
	printf "\t Home Directory: $homeDir_ \n\n"
}


#MAIN
isTesting=0	#flag to check if the user set the file to be tested first before processing
isFilePassed=0	#flag to see if the use pass the filename to be tested and to be processed

while getopts "tf:" opt; do
	case ${opt} in
		t)
			isTesting=1
		;;

		f)
			isFilePassed=1
			newHire_File=$OPTARG
			if [[ -e $newHire_File ]]; then
				if [[ -r $newHire_File ]]; then
					if [[ -s $newHire_File ]]; then
						readFile
					else
						bail_out "The file $newHire_File is empty"
					fi
				else
					bail_out "The file $newHire_File is not readable"
				fi
			else
				bail_out "The file $newHire_File does not exist"
			fi
		;;
		
		:) 
			bail_out "No parameters found"
		;;

		?)
			bail_out "Invalid option"
		;;		
	esac
done
shift $(( OPTIND-1 ))


#check if required option and parameters are present
if [[ $isTesting -eq 0 && $isFilePassed -eq 0 ]]; then
	bail_out "File to process and to be tested not found"
elif [[ $isTesting -eq 0 ]]; then
	bail_out "Cannot proceed without testing the file"
elif [[ $isFilePassed -eq 0 ]]; then
	bail_out "File to process is not found"
else
	testFile
fi 


#no error found on the file, start processing it
# Column Headers at index 0, so start at index 1
for (( i=1;i<${#_newHireInfos[@]};i++ )); do
	current=$(echo "${_newHireInfos[$i]}" | tr -dc "[:alnum:],")
	setUsername $current "uName"
	setDepartment $current "deptName"
	setGroup $current $deptName "groupName"
	setUpTheUser $current $uName $deptName $groupName
done


