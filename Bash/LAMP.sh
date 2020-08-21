#!/bin/bash
set -u

#------------------------------GLOBAL VARIABLES--------------------------------
NUM_PARAM=2
userPath="/etc/passwd"
configFile="/etc/httpd/conf/httpd.conf"
backup_file=`date +%F_%T`
backup_file="${configFile}.${backup_file}"


#=========================================================================
# function: bail_out()
# -- when an error occurs, it will display the error message then exit the script
# parameters: errMsg (error message)
#==========================================================================
function bail_out(){
        local errMsg=$1

        printf "Usage: $0 [Username] [Port Number] \n"
        printf "ERROR MESSAGE: $errMsg \n" 1>&2

        exit 0
}


#MAIN

# check if the number of parameter is valid
if [[ $# -gt $NUM_PARAM || $# -lt $NUM_PARAM ]]; then
        bail_out "Invalid number of parameters."
fi

#=====================Input Parameters========================================
user=$1
port=$2

homeDir="/home/$user"
documentRoot="$homeDir/www"
logPath="/home/$user/logs"

# check if the user is a valid user
user_exist=$(grep -c "^$user:" $userPath)
if [[ $user_exist -eq 0 ]]; then
	bail_out "The user $user does not exist."
fi

# check if the port is already in use
port_exist=$(grep "^Listen $port" $configFile | cut -d" " -f2)
if [[ $port_exist -eq $port ]]; then
	bail_out "Port number $port is already in use."
fi

# check if there is a configured virtual host for the port
virtualHost_exist=$(grep "DocumentRoot $documentRoot" $configFile | cut -d" " -f2)
if [[ $virtualHost_exist ==  $documentRoot ]]; then
	bail_out "The user $user already has a virtual host configured."
fi

# user exist, port not in use, no virtual host configured
# create a virtual host for the user

# start httpd service
`systemctl start httpd`

# check status for named service
if [[ $(systemctl status httpd) ]]; then
        printf "httpd Service is actively running\n"
else
        bail_out "httpd service is not actively running."
fi

#turn-off firewall
`service firewalld stop`

# copy configuration file into the back up file
`cp $configFile $backup_file`

`mkdir -p $documentRoot`
printf "Document root directory created for the user $user \n"
`chown $user $homeDir`
`chown $user $documentRoot`
`chmod 701 $homeDir`
printf "Virtual Path changed owner into the user $user \n"
`mkdir -p $logPath`
printf "Log directory created for the user $user \n"
`chown root $logPath`

index_file="$documentRoot/index.html"

`touch $index_file`
`chmod 755 $index_file`

# create index.html page for the user
cat > $index_file << End-Of-Message
<html>
	<head>
		<title>Index</title>
	</head>
	<body>
		<h1>Future Home of $user</h1>
	</body>
</html>
End-Of-Message

# add a virtual configuration for the user of their chosen port
cat >> $configFile << End-Of-Message

Listen $port

<VirtualHost *:$port>
DocumentRoot $documentRoot
ErrorLog $logPath/error_log
CustomLog $logPath/access_log common
</VirtualHost>


<Directory $documentRoot/>
AllowOverride all
Require all granted
</Directory>
End-Of-Message


# restart httpd service
`systemctl restart httpd`

# turn off firewall
`service firewalld stop`

# check status for named service
if [[ $(systemctl status httpd) ]]; then
        printf "httpd Service is actively running\n"
else
	`cp $backup_file $configFile`
        bail_out "Unsuccessful creation of Virtual Host for the user $user"
fi

printf "Virtual Host successfully created for $user \n"
