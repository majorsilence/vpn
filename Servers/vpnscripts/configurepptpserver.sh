#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configurepptpserver()
{
 
	apt-get install -y pptpd
	

	rm -rf /etc/pptpd.conf
	touch /etc/pptpd.conf

	echo "option /etc/ppp/pptpd-options" >> /etc/pptpd.conf
	echo "logwtmp" >> /etc/pptpd.conf
	echo "localip 10.6.0.1" >> /etc/pptpd.conf
	echo "remoteip 10.6.0.100-200" >> /etc/pptpd.conf


	rm -rf /etc/ppp/pptpd-options
	touch /etc/ppp/pptpd-options

	echo "ms-dns 8.8.8.8" >> /etc/ppp/pptpd-options
	echo "ms-dns 8.8.4.4" >> /etc/ppp/pptpd-options
	echo "name pptpd" >> /etc/ppp/pptpd-options
	echo "refuse-pap" >> /etc/ppp/pptpd-options
	echo "refuse-chap" >> /etc/ppp/pptpd-options
	echo "refuse-mschap" >> /etc/ppp/pptpd-options
	echo "require-mschap-v2" >> /etc/ppp/pptpd-options
	echo "require-mppe-128" >> /etc/ppp/pptpd-options
	echo "proxyarp" >> /etc/ppp/pptpd-options
	echo "nodefaultroute" >> /etc/ppp/pptpd-options
	echo "lock" >> /etc/ppp/pptpd-options
	echo "nobsdcomp " >> /etc/ppp/pptpd-options
	
	/etc/init.d/pptpd restart

}

configurepptpserver
echo "ok=true  changed=true name='configurepptpserver'" 
