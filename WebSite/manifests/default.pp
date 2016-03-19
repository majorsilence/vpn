exec { "apt-get update":
  path => "/usr/bin",
}
package { "apache2":
  ensure  => present,
  require => Exec["apt-get update"],
}

exec { "apt-get install libapache2-mod-mono":
  path => "/usr/bin",
  require => Package["apache2"]
}

exec { "apt-get install mono-apache-server2":
  path => "/usr/bin",
  require => Exec["apt-get install libapache2-mod-mono"]
}

exec { "a2enmod mod_mono":
  path => "/usr/bin",
  require => Exec["apt-get install mono-apache-server2"]
}

package { "libmono-sqlite4.0-cil":
  ensure  => present,
  require => Package["apache2"],
}


service { "apache2":
  ensure  => "running",
  require => Package["apache2"],
}
file { "/var/www/vpnsite":
  ensure  => "link",
  target  => "/vagrant/VpnSite/VpnSite/bin",
  require => Package["apache2"],
  notify  => Service["apache2"],
}