#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

sudo ansible-galaxy install marklee77.mariadb
