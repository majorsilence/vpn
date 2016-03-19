#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

adminemail=$1
webaddress=$2
companyname=$3

getubuntuversion()
{

	b=$(cat /etc/issue)

	if [[ $b == *14.04* ]]; 
		then 
		echo '14.04'
	fi

	if [[ $b == *12.04* ]]; 
		then 
		echo '12.04'
	fi

}


configureopenvpnauthority()
{
	read ubuntuversion junk <<< $(getubuntuversion; echo $?)

	echo "ubuntu version is: $ubuntuversion"

	apt-get install -y openvpn

	if [ "$ubuntuversion" == '14.04' ];
	then
		apt-get install -y easy-rsa
	fi

	# BEGIN setup server certificate authority *******************
	if [ ! -d /etc/openvpn/easy-rsa/ ]; then
  		mkdir /etc/openvpn/easy-rsa/

		if [ "$ubuntuversion" == '14.04' ];
		then
			cp -r /usr/share/easy-rsa/* /etc/openvpn/easy-rsa/
			sed -i "s/KEY_ALTNAMES=\"\$KEY_CN\"/KEY_ALTNAMES=\"DNS:\${KEY_CN}\"/g" /etc/openvpn/easy-rsa/pkitool
		else
			cp -r /usr/share/doc/openvpn/examples/easy-rsa/2.0/* /etc/openvpn/easy-rsa/
		fi

		
	fi

	#edit /etc/openvpn/easy-rsa/vars adjusting the following to your environment:

	rm -rf /etc/openvpn/easy-rsa/vars
	touch /etc/openvpn/easy-rsa/vars

	cd /etc/openvpn/easy-rsa/
	
	echo "export EASY_RSA=\"\`pwd\`\"" >> /etc/openvpn/easy-rsa/vars
	echo "export OPENSSL=\"openssl\"" >> /etc/openvpn/easy-rsa/vars
	echo "export PKCS11TOOL=\"pkcs11-tool\"" >> /etc/openvpn/easy-rsa/vars
	echo "export GREP=\"grep\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_CONFIG=\`\$EASY_RSA/whichopensslcnf \$EASY_RSA\`" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_DIR=\"\$EASY_RSA/keys\"" >> /etc/openvpn/easy-rsa/vars
	echo "export PKCS11_MODULE_PATH=\"dummy\"" >> /etc/openvpn/easy-rsa/vars
	echo "export PKCS11_PIN=\"dummy\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_SIZE=2048" >> /etc/openvpn/easy-rsa/vars
	echo "export CA_EXPIRE=3650" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_EXPIRE=3650" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_COUNTRY=\"CA\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_PROVINCE=\"NL\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_CITY=\"Gander\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_ORG=\"$companyname\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_EMAIL=\"$adminemail\"" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_EMAIL=$adminemail" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_CN=$webaddress" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_NAME=$webaddress" >> /etc/openvpn/easy-rsa/vars
	echo "export KEY_OU=$webaddress" >> /etc/openvpn/easy-rsa/vars
	echo "export PKCS11_MODULE_PATH=changeme" >> /etc/openvpn/easy-rsa/vars
	echo "export PKCS11_PIN=1234" >> /etc/openvpn/easy-rsa/vars

	# generate the master Certificate Authority (CA) certificate and key:
	cd /etc/openvpn/easy-rsa/

	# create link to openssl-1.0.0 to openssl.cnf because openssl.cnf is missing in
	# ubuntu 12.04 but openvpn "source vars" requires openssl.cnf to exit in the same 
	# dirctory as "vars"
	if [ ! -f openssl.cnf ]; then
    	ln -s openssl-1.0.0.cnf openssl.cnf
	fi

	if [ ! -f /etc/openvpn/majorsilencevpn.com.crt ]; then
		
		
		
		echo "open vpn keys and certs missing, creating now"

		touch /etc/openvpn/easy-rsa/setup-server-stuff
		
		echo "#!/usr/bin/env bash" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "cd /etc/openvpn/easy-rsa/" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/easy-rsa/keys/majorsilencevpn.com.crt" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/easy-rsa/keys/majorsilencevpn.com.key" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/easy-rsa/keys/ca.crt" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/majorsilencevpn.com.crt" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/majorsilencevpn.com.key" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "rm -f /etc/openvpn/ca.crt" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "source vars" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "./clean-all" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "./pkitool --initca # non interactive ./build-ca" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "./pkitool --server # non interactive ./build-key-server" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "# Diffie Hellman parameters must be generated for the OpenVPN server" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "./build-dh" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "# copy all generated keys to openvpn config folder" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "cd /etc/openvpn/easy-rsa/keys/" >> /etc/openvpn/easy-rsa/setup-server-stuff
		echo "cp ca.crt dh2048.pem majorsilencevpn.com.crt majorsilencevpn.com.key /etc/openvpn/" >> /etc/openvpn/easy-rsa/setup-server-stuff
		chmod +x /etc/openvpn/easy-rsa/setup-server-stuff
		sudo /etc/openvpn/easy-rsa/setup-server-stuff
		
		#rm -rf /etc/openvpn/easy-rsa/setup-server-stuff
		

	fi
	# END setup server certificate authority *******************
	
	# Create temp folder for downloading client certs
	cd /etc/openvpn
	rm -rf downloadclientcerts
	mkdir downloadclientcerts
}


configureopenvpnauthority
echo "ok=true  changed=true name='configureopenvpnauthority'" 

