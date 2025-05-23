# Network Connection Guide

## Connecting Across Computers

To connect between different computers, follow these steps:

### On the Host Computer (Server):

1. Make sure the AutoConnector has `CONNECTION_MODE = 1` (host mode)
2. Check the Unity console when running to see your computer's IP addresses
3. Note one of the IP addresses listed (usually starts with 192.168.x.x)
4. Make sure your firewall allows connections on port 7777

### On the Client Computers:

1. Change the AutoConnector script to have `CONNECTION_MODE = 0` (client mode)
2. Set the `serverIP` field to the IP address of the host computer (e.g., "192.168.1.5")
3. Keep the default port (7777)

### Common Connection Issues:

1. **"Failed to obtain _appData, can't resume connection"**:
   - This often happens when the client can't reach the server
   - Ensure you're using the correct IP address 
   - Check that both computers are on the same network
   - Verify the server's firewall isn't blocking the connection
   - Try pinging the server from the client to verify connectivity

2. **Timeout Issues**:
   - The improved AutoConnector script now has better timeout handling
   - If you still get timeouts, try increasing the `connectionTimeout` value

3. **Firewall Settings**:
   - Make sure Windows Firewall or macOS Firewall allows Unity and the game to accept incoming connections
   - You may need to add an exception for port 7777 (TCP and UDP)

4. **Using a Public IP**:
   - If connecting across the internet (not recommended for testing), you'll need to:
     - Configure port forwarding on your router to forward port 7777 to the host computer
     - Use your public IP address (get it from whatismyip.com)
     - This is much more complex and should only be attempted if LAN testing isn't possible

### Testing Connectivity:

If you're having issues, try these steps to test basic connectivity:

1. On the client computer, open a command prompt/terminal
2. Type: `ping [server-ip]` (e.g., `ping 192.168.1.5`)
3. You should see successful ping responses
4. If pings fail, there's a basic network issue to solve first