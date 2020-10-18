# Raspberry Debugger
Visual Studio Extension for debugging .NET Core applications remotely on a Raspberry Pi.

The Microsoft .NET Core platform is a nice way to develop cross-platform applications for Windows, OS/X and Linux.  .NET Core is also compatible with Raspberry Pi and its Linux based operating system: **Raspberry Pi OS**.

You can use Visual Studio Code to develop and debug .NET Core applications either directly on your Raspberry or remotely from another computer but until today, there's been no easy way to use regular Visual Studio to develop and debug applications for Raspberry.

The new **Raspberry Debugger** Visual Studio extension allows you to code your application on a Windows workstation and then build and debug it on a Raspberry by just pressing **F5 - Start Debugging**.

### Requirements

* Visual Studio (Community Edition or better)
* Raspberry Pi running Raspberry Pi OS 32-bit
* Raspberry user allowed to sudo

### Configure your Raspberry

After getting your Raspberry setup based on the instructions you received with it, you'll need to perform a couple additional steps to make it ready for remote debugging:

1. Enable SSH so the Raspberry Debugger will be able to connect to your Raspberry remotely.  Start the **Terminal** on your Raspberry and enter these commands:
   ```
   sudo systemctl enable ssh
   sudo systemctl start ssh
   ```

2. Ensure that your Raspberry is connected to the network via WiFi or wired ethernet.  You probably already already did this during the initial Raspberry setup.

3. You'll need to need to know the IP address for your Raspberry.  Go back to the **Terminal** and enter this command:
    ```
    ip -h address
    ```
    You'll see something like this:
    <br/>
    ![Screenshot](/Doc/Images/ip-address.png?raw=true)
    <br/>
    You're looking for an **inet** address.  In my case here, my Raspberry is connected to WiFi and so the connection information will be located under the **wlan0** network interface.  I've highlighted the interface and the internet address here.

    When your Raspberry is connected to a wired network, you'll see the IP address beneath the **eth0** network interface which I've also highlighted but there is no IP address listed because my Raspberry is not connected to a wired network.

    Make a note of your Raspberry's IP address, you'll need it to configure a connection in Visual Studio.

4. **Advanced:** Your Raspeberry's IP address may change from time-to-time, depending on your network settings.  I've configured my home router to assign reserve an IP address for my Raspberry so it won't change.  You may need to [configure a static IP address](https://www.raspberrypi.org/documentation/configuration/tcpip/) on the Raspberry itself.

That's all the configuration required on the Raspberry.

### Configure Visual Studio

On your Windows workstation:

1. Download the **RaspberryDebugger.vsix** file from the latest release on this GitHub repository and double-click it to install it on Visual Studio.  Alternatively, you can install this from the **Visual Studio Marketplace** from within Visual Studio via **Extensions/Manage Extensions**.

2. Start or restart Visual Studio.

3. Create a connection for your Raspberry:
   a. Choose the **Tools/Options...** menu and select the **Raspberry Debugger/Connections** panel.  You'll see this:
      ![Screenshot](/Doc/Images/ToolsOptions1.png?raw=true)
   b. Click **Add** to create a connection.  The connection dialog will be prepopulated with the default **pi** username and the default **raspberry** password.  You'll need to update these as required and also enter your Raspberry's IP address (or hostname).
      ![Screenshot](/Doc/Images/ToolsOptions2.png?raw=true)
   c. When you click **OK**, we'll connect to the Raspberry to validate your credentials and if that's successful, we'll configure the Raspberry by installing any required packages and also create and configure the SSH keypair that will be used for subsequent connections.  You connections should look something like this:
      ![Screenshot](/Doc/Images/ToolsOptions3.png?raw=true)
   d. Your new connection will look something like this on success:
      ![Screenshot](/Doc/Images/ToolsOptions4.png?raw=true)

4: Configure your .NET Core project for debugging.  Raspberry Debugger supports Console and ASPNET applications targeting .NET Core 3.1.x (we'll support .NET 5 when it's released).
   a. Open one of your project source files and choose the new **Project/Raspberry Debug** menu:
      ![Screenshot](/Doc/Images/RaspberryDebugMenu.png?raw=true)
   b. The settings dialog will look like this:
      ![Screenshot](/Doc/Images/RaspberryProjectSettings.png?raw=true)
   c. Click the **Target Raspberry** combo box and choose the Raspberry connection you created easlier or **[DEFAULT]** to select the connection with its **Default** box checked and click **OK** to close the dialog.

That's all there is to it: Just **press F5 to build and debug** your program remotely on the Raspberry.  We'll handle installing the correct SDK as well as the VSDBG debugger on your Raspberry.





### Disclosures

* _The Raspberry Debugger extension is compatible with Raspberry Pi_
* _Raspberry Pi is a trademark of the Raspberry Pi Foundation_