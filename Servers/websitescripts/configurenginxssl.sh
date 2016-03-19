#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

webaddress=$1

configurenginxssl()
{
	rm -rf /etc/nginx/ssl
	mkdir /etc/nginx/ssl
	touch /etc/nginx/ssl/majorsilencevpn_com.crt
	touch /etc/nginx/ssl/majorsilencevpn.key


	echo "This is were you need to add your ssl certs"

	echo "server {" >> /etc/nginx/sites-enabled/default
	echo "	listen 443 default_server;" >> /etc/nginx/sites-enabled/default
	echo "	listen [::]:443 default_server ipv6only=on;" >> /etc/nginx/sites-enabled/default
	echo "	root /var/www/$webaddress/;" >> /etc/nginx/sites-enabled/default
	echo "	index index.html index.htm default.aspx Default.aspx Index.aspx index.aspx;" >> /etc/nginx/sites-enabled/default
	echo "	server_name $webaddress www.$webaddress;" >> /etc/nginx/sites-enabled/default
	echo "	ssl on;" >> /etc/nginx/sites-enabled/default
	echo "	ssl_certificate /etc/nginx/ssl/majorsilencevpn_com.crt;" >> /etc/nginx/sites-enabled/default
	echo "	ssl_certificate_key /etc/nginx/ssl/majorsilencevpn.key;" >> /etc/nginx/sites-enabled/default
	echo "	location / {" >> /etc/nginx/sites-enabled/default
	echo "		root /var/www/$webaddress/;" >> /etc/nginx/sites-enabled/default
	echo "		fastcgi_pass 127.0.0.1:9000;" >> /etc/nginx/sites-enabled/default
	echo "		include /etc/nginx/fastcgi_params;" >> /etc/nginx/sites-enabled/default
	echo "		index index.html index.htm default.aspx Default.aspx Index.aspx index.aspx;" >> /etc/nginx/sites-enabled/default
	echo "		}" >> /etc/nginx/sites-enabled/default
	echo "}" >> /etc/nginx/sites-enabled/default



	service nginx restart
}

configurenginxssl
echo "ok=true  changed=true name='configurenginxssl'" 
