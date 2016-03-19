#!/usr/bin/env bash

ansible-playbook ./Ansible/vpn-playbook-test.yml -u "vagrant" --ask-pass -i ./Ansible/hosts -vvvv
