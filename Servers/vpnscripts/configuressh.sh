#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

vpnsshuser=$1
vpnsshpassword=$2

configuressh()
{

	# Setup ssh server
	# probably already installed but lets make sure
	apt-get install -y openssh-server

	if [ ! -f /etc/ssh/sshd_config.factory-defaults ]; then
    	echo "create a read only copy of default settings"
    	cp /etc/ssh/sshd_config /etc/ssh/sshd_config.factory-defaults
		chmod a-w /etc/ssh/sshd_config.factory-defaults
	fi

	rm -rf /etc/ssh/sshd_config
	touch /etc/ssh/sshd_config

	echo "AllowUsers root $vpnsshuser vagrant" >> /etc/ssh/sshd_config
	echo "Port 22" >> /etc/ssh/sshd_config
	echo "Protocol 2" >> /etc/ssh/sshd_config
	echo "HostKey /etc/ssh/ssh_host_rsa_key" >> /etc/ssh/sshd_config
	echo "HostKey /etc/ssh/ssh_host_dsa_key" >> /etc/ssh/sshd_config
	echo "HostKey /etc/ssh/ssh_host_ecdsa_key" >> /etc/ssh/sshd_config
	echo "HostKey /etc/ssh/ssh_host_ed25519_key" >> /etc/ssh/sshd_config
	echo "UsePrivilegeSeparation yes" >> /etc/ssh/sshd_config
	echo "KeyRegenerationInterval 3600" >> /etc/ssh/sshd_config
	echo "ServerKeyBits 1024" >> /etc/ssh/sshd_config
	echo "SyslogFacility AUTH" >> /etc/ssh/sshd_config
	echo "LogLevel INFO" >> /etc/ssh/sshd_config
	echo "LoginGraceTime 120" >> /etc/ssh/sshd_config
	echo "PermitRootLogin without-password" >> /etc/ssh/sshd_config
	echo "StrictModes yes" >> /etc/ssh/sshd_config
	echo "RSAAuthentication yes" >> /etc/ssh/sshd_config
	echo "PubkeyAuthentication yes" >> /etc/ssh/sshd_config
	echo "IgnoreRhosts yes" >> /etc/ssh/sshd_config
	echo "RhostsRSAAuthentication no" >> /etc/ssh/sshd_config
	echo "HostbasedAuthentication no" >> /etc/ssh/sshd_config
	echo "PermitEmptyPasswords no" >> /etc/ssh/sshd_config
	echo "ChallengeResponseAuthentication no" >> /etc/ssh/sshd_config
	echo "PasswordAuthentication yes" >> /etc/ssh/sshd_config
	echo "X11Forwarding no" >> /etc/ssh/sshd_config
	echo "X11DisplayOffset 10" >> /etc/ssh/sshd_config
	echo "PrintMotd no" >> /etc/ssh/sshd_config
	echo "PrintLastLog yes" >> /etc/ssh/sshd_config
	echo "TCPKeepAlive yes" >> /etc/ssh/sshd_config
	echo "AcceptEnv LANG LC_*" >> /etc/ssh/sshd_config
	echo "Subsystem sftp /usr/lib/openssh/sftp-server" >> /etc/ssh/sshd_config

	if ! id -u $vpnsshuser > /dev/null 2>&1; then
		useradd -d /home/$vpnsshuser -m $vpnsshuser
	fi

	echo $vpnsshuser:$vpnsshpassword | chpasswd

	
	# add $vpnsshuser to the sudoer list
	adduser $vpnsshuser sudo
	# add $vpnsshuser to sudo (admin) group so ssh connection do not require password after login to run commands
	usermod -a -G sudo $vpnsshuser

	reload ssh
}

configuressh
echo "ok=true  changed=true name='configuressh'" 
