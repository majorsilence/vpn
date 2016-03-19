#!/usr/bin/env bash

ansible-playbook ./Ansible/website-playbook-test.yml --user "vagrant" --ask-pass -i ./Ansible/hosts -vvvv
