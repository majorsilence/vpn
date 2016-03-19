#!/usr/bin/env bash


vpnsshuser=$1
configuresudo()
{
	# make sudo not require a password.
	# this is required for open vpn cert creation to work
	
	rm -rf /etc/sudoers.d/$vpnsshuser
	touch /etc/sudoers.d/$vpnsshuser
	echo "$vpnsshuser ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/$vpnsshuser


}

configuresudo
echo "ok=true  changed=true name='configuresudo'" 
