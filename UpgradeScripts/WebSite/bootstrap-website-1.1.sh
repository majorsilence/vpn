#!/usr/bin/env bash


wget -O xamarin.gpg http://download.mono-project.com/repo/xamarin.gpg
apt-key add xamarin.gpg
rm -f xamarin.gpg

echo "deb http://download.mono-project.com/repo/debian wheezy main" > /etc/apt/sources.list.d/mono-xamarin.list

apt-get purge mono-runtime -y

apt-get update
apt-get upgrade -y
apt-get install -y mono-complete mono-fastcgi-server

mozroots --import --ask-remove --machine

service majorsilencevpn start	
service nginx stop
service nginx start	

echo "1.1" > /etc/majorsilencevpn/version



echo "majorsilence website upgrade to 1.1 finished"

exit
