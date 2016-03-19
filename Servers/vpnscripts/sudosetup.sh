#!/usr/bin/env bash

vpnsshuser=$1
configuresudo()
{
	# make sudo not require a password.
	# this is required for open vpn cert creation to work
	
	rm -rf /etc/sudoers.d/root
	touch /etc/sudoers.d/root
	chmod 0440 /etc/sudoers.d/root
	echo "root ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/root

	rm -rf /etc/sudoers.d/vagrant
	touch /etc/sudoers.d/vagrant
	chmod 0440 /etc/sudoers.d/vagrant
	echo "vagrant ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/vagrant

	rm -rf /etc/sudoers.d/$vpnsshuser
	touch /etc/sudoers.d/$vpnsshuser
	chmod 0440 /etc/sudoers.d/$vpnsshuser
	echo "$vpnsshuser ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/$vpnsshuser
}

configuresudo
echo "ok=true  changed=true name='configuresudo'" 
