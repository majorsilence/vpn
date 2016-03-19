#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configurebind()
{
	apt-get install -y bind9
	
	rm -rf /etc/bind/named.conf.options
	touch /etc/bind/named.conf.options

	echo "options {" >> /etc/bind/named.conf.options
	echo "directory \"/var/cache/bind\";" >> /etc/bind/named.conf.options
	echo "forwarders {" >> /etc/bind/named.conf.options
	echo "8.8.8.8;" >> /etc/bind/named.conf.options
	echo "8.8.4.4;" >> /etc/bind/named.conf.options
	echo "};" >> /etc/bind/named.conf.options
	echo "dnssec-validation auto;" >> /etc/bind/named.conf.options
	echo "auth-nxdomain no;" >> /etc/bind/named.conf.options
	echo "listen-on-v6 { any; };" >> /etc/bind/named.conf.options
	echo "};" >> /etc/bind/named.conf.options

}

configurebind
echo "ok=true  changed=true name='configurebind'" 
