#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

remote_address=$1

allowremoteconnections()
{

	
	sed -i "s/bind-address.*/#bind-address = 127.0.0.1/g" /etc/mysql/my.cnf
	mysql -u "root" -p"Password123" -e "GRANT ALL PRIVILEGES ON *.* TO 'root'@'$remote_address' IDENTIFIED BY 'Password123' WITH GRANT OPTION;"
	

	service mysql restart
}

sqluserexists()
{


	while read User; do
	    if [[ "majorsql" == "$User" ]]; then
	        echo "true"
	        break
	    fi
	done < <(mysql -u "root" -p"Password123" -B -N -e 'use mysql; SELECT `user` FROM `user`;')

	if [[ "majorsql" != "$User" ]]; then
	    echo "false"
	fi

	return 0
}

setupsqluser()
{
	#read userexists junk <<< $(sqluserexists; echo $?)

	#if [ "$userexists" == 'false' ];
	#then
	#	mysql -u "root" -p"Password123" -e "CREATE USER 'majorsql'@'localhost' IDENTIFIED BY 'Password123';"
	#	mysql -u "root" -p"Password123" -e "GRANT ALL PRIVILEGES ON * . * TO 'majorsql'@'localhost';"
	#	mysql -u "root" -p"Password123" -e "FLUSH PRIVILEGES;"
	#fi
	return 0
}

configuremysql()
{


	if dpkg --list mariadb-server | egrep -q ^ii; then
		echo "mariadb-server already installed and configured"
		setupsqluser
    	return 0
	fi
	if dpkg --list mariadb-server | egrep -q ^hi; then
		echo "mariadb-server already installed and configured but held at its current version."
		setupsqluser
    	return 0
	fi

	apt-get install -y software-properties-common
	apt-key adv --recv-keys --keyserver hkp://keyserver.ubuntu.com:80 0xcbcb082a1bb943db
	add-apt-repository 'deb http://nyc2.mirrors.digitalocean.com/mariadb/repo/10.0/ubuntu trusty main'
	
	apt-get update

	# Install mysql, default in ubuntu instead of mariadb
	# See http://dba.stackexchange.com/questions/59317/install-mariadb-10-on-ubuntu-without-prompt-and-no-root-password
	export DEBIAN_FRONTEND=noninteractive
	debconf-set-selections <<< 'mariadb-server-10.0 mysql-server/root_password password Password123'
	debconf-set-selections <<< 'mariadb-server-10.0 mysql-server/root_password_again password Password123'
	#debconf-set-selections <<< 'mysql-server mysql-server/root_password Password123 Password123'
	#debconf-set-selections <<< 'mysql-server mysql-server/root_password_again Password123 Password123'
	#apt-get install -y mysql-server
	apt-get install -y mariadb-server


	#echo "mariadb-server hold" | dpkg --set-selections
	apt-mark hold mariadb-server
	apt-mark hold mariadb-server-10.0
	apt-mark hold mariadb-server-core-10.0
	# apt-mark unhold mariadb-server
	# apt-mark unhold mariadb-server-10.0
	# apt-mark unhold mariadb-server-core-10.0
	
	# display status of mariadb-server
	# dpkg --get-selections | grep "mariadb-server"

	allowremoteconnections
	setupsqluser

}

configuremysql
echo "ok=true  changed=true name='configuremysql'" 
