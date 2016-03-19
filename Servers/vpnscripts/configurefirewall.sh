#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configurefirewall()
{


	sed -i 's/DEFAULT_FORWARD_POLICY="DROP"/DEFAULT_FORWARD_POLICY="ACCEPT"/g' /etc/default/ufw

	yes | sudo ufw enable
	## ufw allow 22/tcp
	## ufw allow 1194/udp
	## ufw allow 1194/tcp
	sudo ufw allow 1723/udp # pptp
	sudo ufw allow 1723/tcp
	yes | sudo ufw allow ssh

	sudo ufw allow openvpn
	#sudo ufw allow pptpd

	# bind 9 on vpn subnets only
	sudo ufw allow from 10.8.0.0/24 to any port 53
	sudo ufw allow from 10.7.0.0/24 to any port 53
	sudo ufw allow from 10.6.0.0/24 to any port 53
	
	# BEGIN openvpn firewall rules *******************************************
	# configure openvpn tun adapter
	iptables -A INPUT -i tun+ -j ACCEPT

	# Routing all client traffic (including web-traffic) through the VPN
	# See configurevpnserver function
	iptables -t nat -A POSTROUTING -s 10.8.0.0/24 -o eth0 -j MASQUERADE
	iptables -t nat -A POSTROUTING -s 10.7.0.0/24 -o eth0 -j MASQUERADE

	# END openvpn firewall rules *******************************************
	
	# BEGIN pptp firewall rules *******************************************
	#iptables -I INPUT -p tcp --dport 1723 -m state --state NEW -j ACCEPT
	#iptables -I INPUT -p gre -j ACCEPT
	iptables -t nat -A POSTROUTING -s 10.6.0.0/24 -o eth0 -j MASQUERADE
	iptables -A FORWARD -p tcp --syn -s 10.6.0.0/24 -j TCPMSS --set-mss 1356
	#iptables -I FORWARD -p tcp --tcp-flags SYN,RST SYN -s 10.6.0.0/24 -j TCPMSS  --clamp-mss-to-pmtu
	# END pptp firewall rules *******************************************
	
	#enable packet forwarding (without this masquerade and web traffic does not work)
	
	rm -rf /etc/sysctl.conf
	touch /etc/sysctl.conf
	echo "net.ipv4.ip_forward=1" >> /etc/sysctl.conf
	sysctl -p /etc/sysctl.conf
	#sysctl -w net.ipv4.ip_forward=1

	# requires iptables-persistent is installed
	# See http://www.thomas-krenn.com/en/wiki/Saving_Iptables_Firewall_Rules_Permanently

	# fix errors in iptables-persistent package.  See https://forum.linode.com/viewtopic.php?t=9070&p=58732
	#sed -i 's/\(modprobe -q ip6\?table_filter\)/\1 || true/g' /var/lib/dpkg/info/iptables-persistent.postinst;

	echo iptables-persistent iptables-persistent/autosave_v4 boolean true | debconf-set-selections
	echo iptables-persistent iptables-persistent/autosave_v6 boolean true | debconf-set-selections

	apt-get install -y iptables-persistent	
	iptables-save > /etc/iptables/rules.v4
	/var/lib/dpkg/info/iptables-persistent.postinst;

	
}

configurefirewall
echo "ok=true  changed=true name='configurefirewall'" 
