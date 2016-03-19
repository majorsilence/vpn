#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configureautosecurityupdates()
{
	echo "configuring automatic security updates"
	# see https://help.ubuntu.com/community/AutomaticSecurityUpdates
	apt-get install -y unattended-upgrades update-notifier-common

	rm -rf /etc/apt/apt.conf.d/20auto-upgrades
	touch /etc/apt/apt.conf.d/20auto-upgrades
	echo "APT::Periodic::Update-Package-Lists \"1\";" >> /etc/apt/apt.conf.d/20auto-upgrades
	echo "APT::Periodic::Unattended-Upgrade \"1\";" >> /etc/apt/apt.conf.d/20auto-upgrades


	rm -rf /etc/apt/apt.conf.d/50auto-upgrades
	touch /etc/apt/apt.conf.d/50auto-upgrades
	echo "Unattended-Upgrade::Allowed-Origins {    " >> /etc/apt/apt.conf.d/50auto-upgrades
	echo "\"\${distro_id} stable\";" >> /etc/apt/apt.conf.d/50auto-upgrades
	echo "\"\${distro_id} \${distro_codename}-security\";" >> /etc/apt/apt.conf.d/50auto-upgrades
	echo "\"\${distro_id} \${distro_codename}-updates\";" >> /etc/apt/apt.conf.d/50auto-upgrades
	echo "};" >> /etc/apt/apt.conf.d/50auto-upgrades
	echo " " >> /etc/apt/apt.conf.d/50auto-upgrades
	echo "Unattended-Upgrade::Automatic-Reboot \"true\";" >> /etc/apt/apt.conf.d/50auto-upgrades


	
}

configureautosecurityupdates
echo "ok=true  changed=true name='configureautosecurityupdates'" 
