1. Click on network manager
2. Click __VPN Connections__
3. Click __Configure VPN...__
4. Click __Add__
5. Select the connection type __Point-to-Point Tunneling Protocol (PPTP)__
6. Click __Create__
7. On the VPN tab fill in the Gateway with the IP of your vpn connection from https://vpn.majorsilence.com/setup
    * For example: 107.170.154.114
8. Fill in your __User name__
9. Fill in your __Password__
10. Click __Advanced__
    * Check on:
    * __MSCHAP__
    * __MSCHAPv2__
    * __Use Point-to-Point encryption (MPPE)__
    * __Allow BSD data compression__
    * __Allow Deflate data compression__
    * __Use TCP header compression__
11. Click __OK__

You can now connect by

1. Click on network manager
2. Click __VPN Connections__
3. Select the vpn you created above

# Screenshot tutorial

![Network Manager Icon in Menu Bar](/assets/knowledgebase/pptp/ubuntu-networking/0001-network-manager.png)

![Add New Network Connection](/assets/knowledgebase/pptp/ubuntu-networking/0002-network-manager.png)

![Select Point-to-Point Tunneling Protocol (PPTP) Network Option](/assets/knowledgebase/pptp/ubuntu-networking/0003-network-manager.png)

![Enter PPTP Connection Details](/assets/knowledgebase/pptp/ubuntu-networking/0004-network-manager.png)

![Enter PPTP Advanced Connection Details](/assets/knowledgebase/pptp/ubuntu-networking/0005-network-manager.png)