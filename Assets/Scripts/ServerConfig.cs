using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Serialization;

public class ServerConfig : MonoBehaviour
{
    public string address = "0.0.0.0";
    public ushort port = 7777;
    void Start()
    {
        // 设置服务器监听地址和端口
        // 0.0.0.0 表示监听所有网络接口
        InstanceFinder.TransportManager.Transport.SetServerBindAddress(address,IPAddressType.IPv4);
        InstanceFinder.TransportManager.Transport.SetPort(port);
        
        // 启动服务器
        InstanceFinder.ServerManager.StartConnection();
        Debug.Log($"服务器已启动，监听所有网络接口的{port}端口");
    }
}