DO NOT USE. Playground code for experimenting with. It may not build. If it builds it may not run.

VPN and DNS servers.

See sub folders for specific project details.

# Basic Technology Overview

- [Ubuntu](http://www.ubuntu.com/)
  - 12.04 - operating system for vpn server and dns server.
  - 14.04 - web site server (newer mono version)
- VPN
  - [OpenVPN](https://help.ubuntu.com/12.04/serverguide/openvpn.html) - VPN server software and can be used as client.
  - [PPTP](https://help.ubuntu.com/community/PPTPServer) - Point to point tunneling protocol
- [BIND 9 DNS](https://www.isc.org/downloads/bind/) - DNS and DSNsec server software.
- [C#/.net5](http://www.mono-project.com/Main_Page) - cross platform .net 5.
- [AspNetCore](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0) -
  - [nginx](http://nginx.org/) - web server
- [Dapper ORM](https://github.com/StackExchange/dapper-dot-net) - Awesome micro orm
- [MySql](http://www.mysql.com)/[Mariadb](https://mariadb.org/) - Use with website to track user data, accounts, certs, etc.
- [Vagrant](http://www.vagrantup.com/) - create and configure reproducible development and live environments.
  - Should also look into docker.io
  - Provision plugins available for aws, azure, and digital ocean
- [Ansible](http://www.ansible.com) - configuration management
  - [Ansible on Windows](http://www.azavea.com/blogs/labs/2014/10/running-vagrant-with-ansible-provisioning-on-windows/) not supported
- [Bash](<http://en.wikipedia.org/wiki/Bash_(Unix_shell)>) - Easily manage ubuntu (linux) servers using bash shell scripts.
- [Stripe](https://stripe.com) - Process online payments
- Javascript

# Clone and checkout

```bash
git clone https://github.com/majorsilence/vpn.git
```

# Ubuntu Dev

- Install [rider](https://www.jetbrains.com/rider/).

# Build Server

http://build01.majorsilence.com:8080/

- Jenkins CI build server

# Start Working with Vagrant

Run the following command to start all vms.

```bash
vagrant up
```

To load only one vm specifiy the name of the vm from the vagrant file after the word up. For example:

```bash
vagrant up website
vagrant up vpnauthority
```

Can connect to running website on your local computer in your broswer with:

- http://localhost:8081

# Login

Run the following command to ssh into the virtual machine. Requires **C:\Program Files (x86)\Git\bin**
is in your path on windows or if you are using linux that ssh is installed.

```bash
vagrant ssh
```

# End Virtual Machine

Run any of the three commands depending on what action you want.

```bash
vagrant suspend
vagrant halt
vagrant destroy
```

# Rerun bootstrap provision script

```bash
vagrant provision
```

# Restart and Reload Bootstrap Provision

```bash
vagrant reload --provision
```

Which will quickly restart your virtual machine, skipping the initial import step. The provision flag on the reload command instructs Vagrant to run the provisioners, since usually Vagrant will only do this on the first vagrant up command.

# OpenVPN Error Diagnosis

Run the following command to look for openvpn server errors

```bash
grep vpn /var/log/syslog
```

# Reload Vagrant file

This is very useful if you had to add new configuration such as port forwarding.

```
vagrant halt
vagrant reload
```

This will shutdown the virtual machine, parse the vagrant file and apply all settings to the virtual machine but will not run
the boostrap file again.

# OpenVPN Client Connections

The open vpn client must be run as an admin for the connection to be established.

# Site Info

If site is not live (SiteInfo.LiveSite is false) it requires a beta key to sign up.

# Ansible

Use the --ask-pass option to use username and password instead of ssh key.

-f 10 parallelism level of 10 servers at once

```bash
ansible-playbook playbook.yml --ask-pass --user=majorvpn -i /path/to/hosts/file
```

## Ansible from Windows Host

https://github.com/vovimayhem/vagrant-guest_ansible

This should only be used in a dev environment. Do not use on live servers.

This involves an ugly hack to get ansible finding the scripts. In the vagrant file it copies the scripts and ansible
files to the running folder.

Must install plugin as admin to use this.

```powershell
vagrant plugin install vagrant-guest_ansible
```

# Failed Mysql/Mariadb Updates

If an error like the following occurs:

```
mariadb-server depends on mariadb-server-10.0 (= 10.0.15+maria-1~trusty)
```

It maybe because of a conflict between repositories. I remember the following.
_ Mariadb does not work if universe or multiverse in sources
_ But mono fast cgi requires universe.
_ Will probably need to split into two different machines.
_ Fix
_ disable universe and multiverse
_ upgrade mariadb
_ enable universe and multiver
_ reinstall mono fast-cgi if removed

# Ansible SSH problems

For example you get the message.
"""
FATAL: no hosts matched or all hosts have already failed -- aborting
"""
If you have already created the vm and destroyed it and recreated you probably this is probably caused by a host idenfication change.
SSH does not like it.

Test this out by running something like

```bash
ssh vagrant@192.168.40.4
```

If you get the **remote host identifcation has changed warning**, remove the host with the following command.

```bash
ssh-keygen -R 192.168.40.4
```
