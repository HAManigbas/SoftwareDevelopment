#!/bin/bash
set -u

#================GLOBAL VARIABLES=========================
MAX=255		#Max Number for an octet
MIN=0		#Min Number for an octet
NUM_OCTET=4	#Number of octets
NUM_PARAM=2	#Number of parameters for this script
namedconf_file="/etc/named.conf"
backup_file=`date +%F_%T`
backup_file="${namedconf_file}.${backup_file}"


#=======================================================================
# function: green()
# -- change the text color to green
#=======================================================================
function green(){
	printf "\033[;32m"
}


#=========================================================================
# function resetColor()
# -- reset the text color into white
#=========================================================================
function resetColor(){
	printf "\033[0m"
}


#=========================================================================
# function: bail_out()
# -- when an error occurs, it will display the error message then exit the script
# parameters: errMsg (error message)
#==========================================================================
function bail_out(){
	local errMsg=$1

	printf "Usage: $0 [Zone Name] [IP Address] \n"
	printf "ERROR MESSAGE: $errMsg \n" 1>&2

	exit 0
}


#============================================================================
# function: valid_ip()
# -- checks if the input IP Address is valid
# parameter: ip (input IP Address)
#============================================================================
function valid_ip(){
	local ip=$1
	local ip_Arr=(${ip//./ })

	if [[ ${#ip_Arr[@]} -ne $NUM_OCTET ]]; then
		bail_out "Invalid IP Address: $ip"
	fi

	for (( i=0;i<${#ip_Arr[@]};i++ )); do
		if [[ ${ip_Arr[$i]} -gt $MAX || ${ip_Arr[$i]} -lt $MIN ]]; then
		bail_out "Invalid IP Address: $ip"
		fi
	done	
}



#MAIN

# check if the number of parameter is valid
if [[ $# -gt $NUM_PARAM || $# -lt $NUM_PARAM ]]; then
	bail_out "Invalid number of parameters."
fi

#=====================Input Parameters========================================
zoneName=$1
ipAddress=$2

valid_ip $ipAddress

# check if DNS Server for the domain already exist
isZoneExist=$(grep "^zone" $namedconf_file | grep $zoneName | wc -l)
if [[ $isZoneExist -gt 0 ]]; then
	bail_out "Zone \"$zoneName\" already exist in $namedconf_file"
fi


# recheck again if the DNS server is already setup
isZoneExist=$(named-checkconf -z $namedconf_file | grep $zoneName | wc -l)
if [[ $isZoneExist -gt 0 ]]; then
        bail_out "Domain already setup for DNS on server \"$zoneName\""
fi

# start named service
`systemctl start named`
`systemctl enable named`

# check status for named service
if [[ $(systemctl status named) ]]; then
	green
	printf "\nnamed Service is actively running\n"
	resetColor	
else
	bail_out "named service is not actively running."
fi

# copy configuration file into the back up file
`cp $namedconf_file $backup_file`

green
printf "\n$namedconf_file copied into $backup_file \n"
resetColor

zoneFile="${zoneName}.zone"

# add the zone into the configuration file
cat >> $namedconf_file  << End-Of-Message

zone "$zoneName" IN {
	type master;
	file "$zoneFile";
	allow-update {none;};
};
End-Of-Message

green
printf "\nZone \"$zoneName\" added to the $namedconf_file \n"
resetColor

zoneFile="/var/named/$zoneFile"

# create a zone file for the zone
cat > $zoneFile << End-Of-Message
\$TTL 1D
@ IN SOA ${zoneName}. boss.${zoneName}.cs.johnabbott.qc.ca. (
	1 ; serial
	1D ; refresh
	1H ; retry
	1W ; expire
	3H );
@ IN NS ns1.cs.johnabbott.qc.ca.

@ IN A $ipAddress
www IN CNAME ${zoneName}.
ftp IN CNAME ${zoneName}.
mail IN CNAME ${zoneName}.
End-Of-Message

green
printf "\nZone file for \"$zoneName\" is created  \n\n"

named-checkzone forward $zoneFile
printf "\n"
named-checkconf -z $namedconf_file
printf "\n"
`chown root:named $zoneFile`
`service named restart`

if [[ $(service named status) ]]; then
        printf "\nnamed Service is actively running\n"
else
	resetColor
	`cp $backup_file $namedconf_file`
	bail_out "Unsuccsessful creation of DNS Server for $zoneName"
fi

printf "\nDNS Server for $zoneName is successfully created\n"
resetColor


# Hazel Ann Manigbas
# 1508630
 
