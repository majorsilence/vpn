# Ubuntu OpenVPN

Tested with Ubuntu 14.04

When logged into the site download your open vpn certificate from https://vpn.majorsilence.com/setup
by clicking the __Download OpenVPN Certificate__ button. This will download a file called Certs.zip.
Unzip this file.

Next install the ubuntu network manager openvpn plugin.

```bash
sudo apt-get install network-manager-openvpn-gnome
```

## Easy Way

1. Click on network manager
2. Click __VPN Connections__
3. Click __Configure VPN...__
4. Click __Add__
5. Select the connection type __Import a saved VPN Configuration...__
6. Click __Create...__
7. Browse to the certficate that you unzipped, folder __majorsilencevpn/ubuntu__ and select _
   _majorvpn-network-manager.ovpn__
8. Click save

You now have your vpn connection setup.

You can now connect by

1. Click on network manager
2. Click __VPN Connections__
3. Select the vpn you created above

## The Hard Way (if the easy way does not work)

1. Click on network manager
2. Click __VPN Connections__
3. Click __Configure VPN...__
4. Click __Add__
5. Select the connection type __OpenVPN__
6. Click __Create...__

On the VPN tab

1. On the VPN tab fill in the Gateway with the IP of your vpn connection from https://vpn.majorsilence.com/setup
    * For example: 107.170.154.114
2. Authention type: __Certificates (TLS)__
3. Select the __User Certificate__ from your download, something like [majorvpn].crt
4. Select the __CA Certificate__ from your download, ca.crt
5. Select the __Private Key__ from your download, [majorvpn].key
6. Click __Advanced__
7. Check on __Use LZO data compression__
8. Check on __Use TCP connection__
9. Click __OK__
10. Click __Save...__

You can now connect by

1. Click on network manager
2. Click __VPN Connections__
3. Select the vpn you created above

# Screenshot tutorial

![Network Manager Icon in Menu Bar](/assets/knowledgebase/openvpn/ubuntu-networking/0001-network-manager.png)

![Add New Network Connection](/assets/knowledgebase/openvpn/ubuntu-networking/0002-network-manager.png)

![Select OpenVPN Network Option](/assets/knowledgebase/openvpn/ubuntu-networking/0003-network-manager.png)

![Enter OpenVPN Connection Details and Select Certificates](/assets/knowledgebase/openvpn/ubuntu-networking/0004-network-manager.png)

![Enter OpenVPN Advanced Connection Details](/assets/knowledgebase/openvpn/ubuntu-networking/0005-network-manager.png)