#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

apt-get update
apt-get upgrade -y
apt-get autoremove -y

echo "ok=true  changed=true name='start'" 