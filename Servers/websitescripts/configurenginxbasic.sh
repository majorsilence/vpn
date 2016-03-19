#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

webaddress=$1

configurenginxbasic()
{

	echo "Installing nginx software start" 
	# See http://www.mono-project.com/FastCGI_Nginx
	# and http://wiki.nginx.org/Mono
	# and http://sourcecodebean.com/archives/serving-your-asp-net-mvc-site-on-nginx-fastcgi-mono-server4/1617
	# and https://github.com/ServiceStack/ServiceStack/wiki/Run-ServiceStack-in-Fastcgi-hosted-on-nginx
	apt-get install -y nginx mono-fastcgi-server acl
	echo "Installing nginx software finished" 

	if [ ! -d "/var/www/$webaddress" ]; then
		rm -rf /var/www
		mkdir /var/www
	fi

	
	if [ -d "/vpnsite" ]; then
  		# Control will enter here if $DIRECTORY exists.
  		# running on local vagrant
  		ln -fs /vpnsite /var/www/$webaddress
  	else
  		# live server

		if [ ! -d "/var/www/$webaddress" ]; then
			# create folder only if it does not exist
			mkdir /var/www/$webaddress
		fi
  		

  		# the following has no affect when running on a linked vagrant share
  		setfacl -m user:majorvpn:rwx /var/www/$webaddress
	fi

}

configuresitedefaults()
{

	echo "configuresitedefaults start" 

	# clear default site contents
	# cp /etc/nginx/sites-enabled/default /etc/nginx/sites-enabled/default-backup
	> /etc/nginx/sites-enabled/default

	# config server port 80 with mono
	echo "server {" >> /etc/nginx/sites-enabled/default
	echo "listen 80 default_server;" >> /etc/nginx/sites-enabled/default
	echo "listen [::]:80 default_server ipv6only=on;" >> /etc/nginx/sites-enabled/default
	echo "root /var/www/$webaddress/;" >> /etc/nginx/sites-enabled/default
	echo "index index.html index.htm default.aspx Default.aspx Index.aspx index.aspx;" >> /etc/nginx/sites-enabled/default
	echo "server_name $webaddress www.$webaddress;" >> /etc/nginx/sites-enabled/default
	echo "rewrite        ^ https://\$server_name\$request_uri? permanent;" >> /etc/nginx/sites-enabled/default
	echo "location / {" >> /etc/nginx/sites-enabled/default
	echo "root /var/www/$webaddress/;" >> /etc/nginx/sites-enabled/default
	echo "fastcgi_pass 127.0.0.1:9000;" >> /etc/nginx/sites-enabled/default
	echo "include /etc/nginx/fastcgi_params;" >> /etc/nginx/sites-enabled/default
	echo "index index.html index.htm default.aspx Default.aspx Index.aspx index.aspx;" >> /etc/nginx/sites-enabled/default
	echo "}" >> /etc/nginx/sites-enabled/default
	echo "}" >> /etc/nginx/sites-enabled/default
	#\ntry_files $uri $uri/ =404;

	# increase bucket size so more server options can go in sites-enabled/defaults
	# TODO: maybe we want to override the full nginx.conf file
	sed -i 's/# server_names_hash_bucket_size 64;/server_names_hash_bucket_size 64;/g' /etc/nginx/nginx.conf

	# Redirect www to non www
	# https://rtcamp.com/tutorials/nginx/www-non-www-redirection/

	service nginx reload

	echo "configuresitedefaults finished" 
}

configureupstartservice()
{

	echo "configureupstartservice start" 

	# upstart service script
	rm -rf /etc/nginx/startvpnsite
	touch /etc/nginx/startvpnsite
	echo "#!/bin/bash" >> /etc/nginx/startvpnsite
	echo "fastcgi-mono-server4 /applications=/:/var/www/$webaddress/ /socket=tcp:127.0.0.1:9000" >> /etc/nginx/startvpnsite
	
	# group www-data will need access and execute permissions for the upstart service to be able to bring up fast cgi
	# See https://github.com/ServiceStack/ServiceStack/wiki/Run-ServiceStack-in-Fastcgi-hosted-on-nginx
	chgrp www-data /etc/nginx/startvpnsite
	chmod g+rwx /etc/nginx/startvpnsite

	#fastcgi-mono-server4 /applications=/:/var/www/majorsilencevpn.com/ /socket=unix:/tmp/fastcgi.socket
	#fastcgi-mono-server4 /applications=/:/var/www/majorsilencevpn.com/ /socket=tcp:127.0.0.1:9000
	rm -rf /etc/init/majorsilencevpn.conf
	touch /etc/init/majorsilencevpn.conf
	echo "#!upstart" >> /etc/init/majorsilencevpn.conf
	echo "description \"majorsilence solutions vpn services\"" >> /etc/init/majorsilencevpn.conf
	echo "author \"Peter\"" >> /etc/init/majorsilencevpn.conf
	echo "" >> /etc/init/majorsilencevpn.conf
	echo "start on started networking" >> /etc/init/majorsilencevpn.conf
	echo "stop on shutdown" >> /etc/init/majorsilencevpn.conf
	echo "" >> /etc/init/majorsilencevpn.conf
	echo "respawn" >> /etc/init/majorsilencevpn.conf
	echo "" >> /etc/init/majorsilencevpn.conf
	echo "script" >> /etc/init/majorsilencevpn.conf
	echo "echo \$\$ > /var/run/majorsilencevpn.pid" >> /etc/init/majorsilencevpn.conf
	echo "exec /etc/nginx/startvpnsite 2>&1" >> /etc/init/majorsilencevpn.conf
	echo "end script" >> /etc/init/majorsilencevpn.conf
	echo "" >> /etc/init/majorsilencevpn.conf
	echo "pre-start script" >> /etc/init/majorsilencevpn.conf
	echo "echo \"[`date`] (sys) Starting\" >> /var/log/majorsilencevpn.sys.log" >> /etc/init/majorsilencevpn.conf
	echo "end script" >> /etc/init/majorsilencevpn.conf
	echo "" >> /etc/init/majorsilencevpn.conf
	echo "pre-stop script" >> /etc/init/majorsilencevpn.conf
	echo "rm /var/run/majorsilencevpn.pid" >> /etc/init/majorsilencevpn.conf
	echo "echo \"[`date -u +%Y-%m-%dT%T.%3NZ`] (sys) Stopping\" >> /var/log/majorsilencevpn.sys.log" >> /etc/init/majorsilencevpn.conf
	echo "end script" >> /etc/init/majorsilencevpn.conf


	majorvpnstatus="$(service majorsilencevpn status)"

	if [ ! "$majorvpnstatus" == "majorsilencevpn stop/waiting" ]; then
		service majorsilencevpn stop
	fi
	
	service majorsilencevpn start	
	#bash /etc/nginx/startvpnsite &
	service nginx start

	echo "configureupstartservice finished" 
}


createnginxconf()
{

	rm -rf /etc/nginx/nginx.conf
	touch /etc/nginx/nginx.conf

	echo "user www-data;" >> /etc/nginx/nginx.conf
	echo "worker_processes 4;" >> /etc/nginx/nginx.conf
	echo "pid /run/nginx.pid;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "events { " >> /etc/nginx/nginx.conf
	echo "	worker_connections 768;" >> /etc/nginx/nginx.conf
	echo "}" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "http {" >> /etc/nginx/nginx.conf
	echo "	ssl_protocols  TLSv1 TLSv1.1 TLSv1.2;  # don’t use SSLv3 ref: POODLE" >> /etc/nginx/nginx.conf
	echo "	ssl_prefer_server_ciphers on;" >> /etc/nginx/nginx.conf
	echo "	ssl_ciphers \"ECDH+AESGCM:DH+AESGCM:ECDH+AES256:DH+AES256:ECDH+AES128:DH+AES:ECDH+3DES:DH+3DES:RSA+AESGCM:RSA+AES:RSA+3DES:!aNULL:!MD5:!DSS\";" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	sendfile on;" >> /etc/nginx/nginx.conf
	echo "	tcp_nopush on;" >> /etc/nginx/nginx.conf
	echo "	tcp_nodelay on;" >> /etc/nginx/nginx.conf
	echo "	keepalive_timeout 65;" >> /etc/nginx/nginx.conf
	echo "	types_hash_max_size 2048;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	server_names_hash_bucket_size 64;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	include /etc/nginx/mime.types;" >> /etc/nginx/nginx.conf
	echo "	default_type application/octet-stream;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	access_log /var/log/nginx/access.log;" >> /etc/nginx/nginx.conf
	echo "	error_log /var/log/nginx/error.log;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	gzip on;" >> /etc/nginx/nginx.conf
	echo "	gzip_disable \"msie6\";" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "	include /etc/nginx/conf.d/*.conf;" >> /etc/nginx/nginx.conf
	echo "	include /etc/nginx/sites-enabled/*;" >> /etc/nginx/nginx.conf
	echo "" >> /etc/nginx/nginx.conf
	echo "}" >> /etc/nginx/nginx.conf
	
		
	# reload settings
	service nginx reload

}

configurenginxbasic
configuresitedefaults
configureupstartservice
createnginxconf
echo "ok=true  changed=true name='configurenginxbasic'" 

