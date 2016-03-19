#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

filterfiles()
{

	rm -rf /etc/fail2ban/filter.d/nginx-proxy.conf
	touch /etc/fail2ban/filter.d/nginx-proxy.conf

	echo "[Definition]" >> /etc/fail2ban/filter.d/nginx-proxy.conf
	echo "failregex = ^<HOST> -.*GET http.*" >> /etc/fail2ban/filter.d/nginx-proxy.conf
	echo "ignoreregex =" >> /etc/fail2ban/filter.d/nginx-proxy.conf


	rm -rf /etc/fail2ban/filter.d/nginx-noscript.conf
	touch /etc/fail2ban/filter.d/nginx-noscript.conf
	echo "[Definition]" >> /etc/fail2ban/filter.d/nginx-noscript.conf
	echo "failregex = ^<HOST> -.*GET.*(\.php|\.asp|\.exe|\.pl|\.cgi|\scgi|\.py)" >> /etc/fail2ban/filter.d/nginx-noscript.conf
	echo "ignoreregex =" >> /etc/fail2ban/filter.d/nginx-noscript.conf
	
	
	rm -rf /etc/fail2ban/filter.d/nginx-login.conf
	touch /etc/fail2ban/filter.d/nginx-login.conf
	echo "[Definition]" >> /etc/fail2ban/filter.d/nginx-login.conf
	echo "failregex = ^<HOST> -.*POST /sessions HTTP/1\..\" 200" >> /etc/fail2ban/filter.d/nginx-login.conf
	echo "ignoreregex =" >> /etc/fail2ban/filter.d/nginx-login.conf
	

	rm -rf /etc/fail2ban/filter.d/nginx-auth.conf
	touch /etc/fail2ban/filter.d/nginx-auth.conf
	echo "[Definition]" >> /etc/fail2ban/filter.d/nginx-auth.conf
	echo "failregex = no user/password was provided for basic authentication.*client: <HOST>" >> /etc/fail2ban/filter.d/nginx-auth.conf
	echo "user .* was not found in.*client: <HOST>" >> /etc/fail2ban/filter.d/nginx-auth.conf
	echo "user .* password mismatch.*client: <HOST>" >> /etc/fail2ban/filter.d/nginx-auth.conf
	echo "" >> /etc/fail2ban/filter.d/nginx-auth.conf
	echo "ignoreregex =" >> /etc/fail2ban/filter.d/nginx-auth.conf

}

jailfile()
{
	rm -rf /etc/fail2ban/jail.local
	touch /etc/fail2ban/jail.local

	echo "[DEFAULT]" >> /etc/fail2ban/jail.local
	echo "ignoreip = 127.0.0.1/8" >> /etc/fail2ban/jail.local
	echo "bantime  = 600" >> /etc/fail2ban/jail.local
	echo "findtime = 600" >> /etc/fail2ban/jail.local
	echo "maxretry = 3" >> /etc/fail2ban/jail.local
	echo "backend = auto" >> /etc/fail2ban/jail.local
	echo "usedns = warn" >> /etc/fail2ban/jail.local
	echo "destemail = root@localhost" >> /etc/fail2ban/jail.local
	echo "sendername = Fail2Ban" >> /etc/fail2ban/jail.local
	echo "banaction = iptables-multiport" >> /etc/fail2ban/jail.local
	echo "mta = sendmail" >> /etc/fail2ban/jail.local
	echo "protocol = tcp" >> /etc/fail2ban/jail.local
	echo "chain = INPUT" >> /etc/fail2ban/jail.local
	echo "action_ = %(banaction)s[name=%(__name__)s, port=\"%(port)s\", protocol=\"%(protocol)s\", chain=\"%(chain)s\"]" >> /etc/fail2ban/jail.local
	echo "action_mw = %(banaction)s[name=%(__name__)s, port=\"%(port)s\", protocol=\"%(protocol)s\", chain=\"%(chain)s\"]" >> /etc/fail2ban/jail.local
	echo "	%(mta)s-whois[name=%(__name__)s, dest=\"%(destemail)s\", protocol=\"%(protocol)s\", chain=\"%(chain)s\", sendername=\"%(sendername)s\"]" >> /etc/fail2ban/jail.local
	echo "action_mwl = %(banaction)s[name=%(__name__)s, port=\"%(port)s\", protocol=\"%(protocol)s\", chain=\"%(chain)s\"]" >> /etc/fail2ban/jail.local
	echo "	%(mta)s-whois-lines[name=%(__name__)s, dest=\"%(destemail)s\", logpath=%(logpath)s, chain=\"%(chain)s\", sendername=\"%(sendername)s\"]" >> /etc/fail2ban/jail.local
	echo "action = %(action_)s" >> /etc/fail2ban/jail.local
	echo "[ssh]" >> /etc/fail2ban/jail.local
	echo "enabled  = true" >> /etc/fail2ban/jail.local
	echo "port     = ssh" >> /etc/fail2ban/jail.local
	echo "filter   = sshd" >> /etc/fail2ban/jail.local
	echo "logpath  = /var/log/auth.log" >> /etc/fail2ban/jail.local
	echo "maxretry = 6" >> /etc/fail2ban/jail.local
	echo "" >> /etc/fail2ban/jail.local
	echo "[ssh-ddos]" >> /etc/fail2ban/jail.local
	echo "enabled  = true" >> /etc/fail2ban/jail.local
	echo "port     = ssh" >> /etc/fail2ban/jail.local
	echo "filter   = sshd-ddos" >> /etc/fail2ban/jail.local
	echo "logpath  = /var/log/auth.log" >> /etc/fail2ban/jail.local
	echo "maxretry = 6" >> /etc/fail2ban/jail.local
	echo "[nginx-http-auth]" >> /etc/fail2ban/jail.local
	echo "enabled = true" >> /etc/fail2ban/jail.local
	echo "filter  = nginx-http-auth" >> /etc/fail2ban/jail.local
	echo "port    = http,https" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx/error.log" >> /etc/fail2ban/jail.local
	echo "[nginx-auth]" >> /etc/fail2ban/jail.local
	echo "enabled = true" >> /etc/fail2ban/jail.local
	echo "filter = nginx-auth" >> /etc/fail2ban/jail.local
	echo "action = iptables-multiport[name=NoAuthFailures, port=\"http,https\"]" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx*/*error*.log" >> /etc/fail2ban/jail.local
	echo "bantime = 600" >> /etc/fail2ban/jail.local
	echo "maxretry = 6" >> /etc/fail2ban/jail.local
	echo "[nginx-login]" >> /etc/fail2ban/jail.local
	echo "enabled = true" >> /etc/fail2ban/jail.local
	echo "filter = nginx-login" >> /etc/fail2ban/jail.local
	echo "action = iptables-multiport[name=NoLoginFailures, port=\"http,https\"]" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx*/*access*.log" >> /etc/fail2ban/jail.local
	echo "bantime = 600" >> /etc/fail2ban/jail.local
	echo "maxretry = 6" >> /etc/fail2ban/jail.local
	echo "[nginx-badbots]" >> /etc/fail2ban/jail.local
	echo "enabled  = true" >> /etc/fail2ban/jail.local
	echo "filter = apache-badbots" >> /etc/fail2ban/jail.local
	echo "action = iptables-multiport[name=BadBots, port=\"http,https\"]" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx*/*access*.log" >> /etc/fail2ban/jail.local
	echo "bantime = 86400" >> /etc/fail2ban/jail.local
	echo "maxretry = 1" >> /etc/fail2ban/jail.local
	echo "[nginx-noscript]" >> /etc/fail2ban/jail.local
	echo "enabled = true" >> /etc/fail2ban/jail.local
	echo "action = iptables-multiport[name=NoScript, port=\"http,https\"]" >> /etc/fail2ban/jail.local
	echo "filter = nginx-noscript" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx*/*access*.log" >> /etc/fail2ban/jail.local
	echo "maxretry = 6" >> /etc/fail2ban/jail.local
	echo "bantime  = 86400" >> /etc/fail2ban/jail.local
	echo "[nginx-proxy]" >> /etc/fail2ban/jail.local
	echo "enabled = true" >> /etc/fail2ban/jail.local
	echo "action = iptables-multiport[name=NoProxy, port=\"http,https\"]" >> /etc/fail2ban/jail.local
	echo "filter = nginx-proxy" >> /etc/fail2ban/jail.local
	echo "logpath = /var/log/nginx*/*access*.log" >> /etc/fail2ban/jail.local
	echo "maxretry = 0" >> /etc/fail2ban/jail.local
	echo "bantime  = 86400" >> /etc/fail2ban/jail.local


}

configurefail2ban()
{
	# fail2ban - protect ssh
	# See https://www.digitalocean.com/community/articles/how-to-protect-ssh-with-fail2ban-on-ubuntu-12-04 if you want to make any edits to the config
	apt-get install -y fail2ban
	
	filterfiles
	jailfile

	service fail2ban restart

	fail2ban-client status
}

configurefail2ban
echo "ok=true  changed=true name='configurefail2ban'" 
