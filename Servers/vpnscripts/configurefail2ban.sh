#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configurefail2ban()
{

	# fail2ban - protect ssh
	# See https://www.digitalocean.com/community/articles/how-to-protect-ssh-with-fail2ban-on-ubuntu-12-04 if you want to make any edits to the config
	apt-get install -y fail2ban
	cp -rf /etc/fail2ban/jail.conf /etc/fail2ban/jail.local

	service fail2ban restart
}

configurefail2ban
echo "ok=true  changed=true name='configurefail2ban'" 
