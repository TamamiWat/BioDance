using UnityEngine;
using System.Collections;
using extOSC;

public class ReceiveOSC : MonoBehaviour
{

    //[SerializeField] int port = 6969;
    //OSCReceiver receiver;

    // Use this for initialization
    //void Start()
    //{
    //    receiver = gameObject.AddComponent<OSCReceiver>();
    //    receiver.LocalPort = port;
    //    receiver.Bind("/extOSC/sample", MessageReceived);
    //}

    //void MessageReceived(OSCMessage message)
    //{
    //    Debug.Log($"extOSC receive: {message.Values[0].StringValue} {message.Values[1].FloatValue} {message.Values[2].IntValue}");
    //}

    // OSCアドレスを指定します。
    [SerializeField] string Address = "/example/osc";
    [SerializeField] int ReceiverPort = 7000;

    private OSCReceiver _receiver;

    void Start()
    {
        _receiver = gameObject.AddComponent<OSCReceiver>();
        _receiver.LocalPort = ReceiverPort;

        _receiver.Bind(Address, ReceivedMessage);
    }

    private void ReceivedMessage(OSCMessage message)
    {
        // メッセージの内容に基づいて行いたい処理をここに記述します。
        Debug.Log($"Received OSC message: {message}");
    }

}