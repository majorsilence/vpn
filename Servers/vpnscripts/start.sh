#!/usr/bin/env bash

# See https://help.ubuntu.com/12.04/serverguide/openvpn.html
# multiple openvpn server setup https://forums.openvpn.net/topic7748.html

apt-get update
apt-get upgrade -y
apt-get autoremove -y

echo "ok=true  changed=true name='System Upgraded'" 
