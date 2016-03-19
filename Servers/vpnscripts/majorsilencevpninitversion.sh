#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

majorsilencevpninitversion()
{
	rm -rf /etc/majorsilencevpn
	mkdir /etc/majorsilencevpn
	touch /etc/majorsilencevpn/version
	echo "1" > /etc/majorsilencevpn/version
}


majorsilencevpninitversion
echo "ok=true  changed=true name='majorsilencevpninitversion'" 
