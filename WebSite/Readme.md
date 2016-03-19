The web site allows users to create accounts and pay (activate). 

Once an account is activated the web site will ssh into the vpn server, create a client cert and 
make it available to download/use by the user.


# SSL

See /certs folder for script to generate csr.  

This will need to be re-written

# NUnit Tests

Run all tests from command line:
```bash
nunit-console "./WebSite/SiteTestsFast/bin/Debug/SiteTestsFast.dll"
```

Run a specific test from command line:
```bash
nunit-console -run:SiteTestsFast.LiveSite.SiteInfo "./WebSite/SiteTestsFast/bin/Debug/SiteTestsFast.dll"
```
