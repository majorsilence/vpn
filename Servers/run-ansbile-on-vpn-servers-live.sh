#!/usr/bin/env bash

ansible-playbook ./Ansible/vpn-playbook-live.yml --user "root" -i ./Ansible/hosts -vvvv
