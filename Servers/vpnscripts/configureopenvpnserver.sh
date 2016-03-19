#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

configureopenvpnserver()
{
	# BEGIN openvpn server setup
	#cp /usr/share/doc/openvpn/examples/sample-config-files/server.conf.gz /etc/openvpn/
	#gzip -d /etc/openvpn/server.conf.gz
	#mv /etc/openvpn/server.conf "$1"
	#rm -f /etc/openvpn/server.conf.gz
	
	rm -rf "$1"
	touch "$1"

	echo "port 1194" >> "$1"

	if [ $2 = "tcp" ]; then
		echo "proto tcp" >> "$1"
		echo "server 10.7.0.0 255.255.255.0" >> "$1"

	else
		echo "proto udp" >> "$1"
		echo "server 10.8.0.0 255.255.255.0" >> "$1"

	fi

	
	echo "dev tun" >> "$1"
	echo "ca ca.crt" >> "$1"
	echo "cert majorsilencevpn.com.crt" >> "$1"
	echo "key majorsilencevpn.com.key" >> "$1"
	echo "dh dh2048.pem" >> "$1"
	
	echo "ifconfig-pool-persist ipp.txt" >> "$1"
	echo "push \"redirect-gateway def1 bypass-dhcp\"" >> "$1"
	echo "push \"dhcp-option DNS 8.8.8.8\"" >> "$1"
	echo "push \"dhcp-option DNS 8.8.4.4\"" >> "$1"
	echo "keepalive 10 120" >> "$1"
	echo "comp-lzo" >> "$1"
	echo "persist-key" >> "$1"
	echo "persist-tun" >> "$1"
	echo "status openvpn-status.log" >> "$1"
	echo "verb 3" >> "$1"
	
	
	/etc/init.d/openvpn start

	# END openvpn server setup
	


	# Load balance openvpn
	# https://openvpn.net/index.php/open-source/documentation/howto.html#loadbalance

}

configureopenvpnserver "/etc/openvpn/udp.conf" "udp"
echo "ok=true  changed=true name='configureopenvpnserver udp'" 

configureopenvpnserver "/etc/openvpn/tcp.conf" "tcp"
echo "ok=true  changed=true name='configureopenvpnserver tcp'" 
