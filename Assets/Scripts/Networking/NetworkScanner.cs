using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetworkScanner : MonoBehaviour
{
    public bool run = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(run)
        {
            IPList();

            //LocalIPAddress();
            //  LanManager lm = new LanManager();
            // lm.ScanHost();
            // lm.StartServer(4222);

            run = false;
        }
    }
    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                Debug.Log(localIP);
                break;
            }
        }
        return localIP;
    }


    public string IPList()
    {
        var pingSender = new System.Net.NetworkInformation.Ping();
        string port = string.Empty;
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (!ip.IsDnsEligible)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Debug.Log(ip.Address);
                        // All IP Address in the LAN
                    }
                }
            }
        }
        return port;
    }


}
