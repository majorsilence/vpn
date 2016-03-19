#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configuremono()
{

	wget -O xamarin.gpg http://download.mono-project.com/repo/xamarin.gpg
	apt-key add xamarin.gpg
	rm -f xamarin.gpg
	
	rm -rf /etc/apt/sources.list.d/mono-xamarin.list
	echo "deb http://download.mono-project.com/repo/debian wheezy main" > /etc/apt/sources.list.d/mono-xamarin.list

	apt-get update
	apt-get install -y mono-complete sqlite3 unzip

	echo "configure /etc/mono/registry for use with MVC5"
	rm -rf /etc/mono/registry
	mkdir /etc/mono/registry
	mkdir /etc/mono/registry/LocalMachine
	chmod g+rwx /etc/mono/registry/
	chmod g+rwx /etc/mono/registry/LocalMachine

	mozroots --sync --machine
	# mozroots --import --sync

}


configuremono
echo "ok=true  changed=true name='configuremono'" 
