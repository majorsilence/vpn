#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configurefirewall()
{
	yes | sudo ufw enable
	sudo ufw allow 80/tcp
	sudo ufw allow 443/tcp
	# Let my dev machine connect to the vagrant boxes sql
	#sudo ufw allow from 192.168.10.0/24 to tcp port 3306 
	sudo ufw allow from 192.168.40.0/24 to any port 3306
	yes | sudo ufw allow ssh
	
	

	# requires iptables-persistent is installed
	# See http://www.thomas-krenn.com/en/wiki/Saving_Iptables_Firewall_Rules_Permanently
	echo iptables-persistent iptables-persistent/autosave_v4 boolean true | debconf-set-selections
	echo iptables-persistent iptables-persistent/autosave_v6 boolean true | debconf-set-selections

	apt-get install -y iptables-persistent
	iptables-save > /etc/iptables/rules.v4
	/var/lib/dpkg/info/iptables-persistent.postinst;
}

configurefirewall
echo "ok=true  changed=true name='configurefirewall'" 
