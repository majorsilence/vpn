#!/usr/bin/env bash

ansible-playbook ./Ansible/website-playbook-live.yml --user "root" -i ./Ansible/hosts -vvvv
