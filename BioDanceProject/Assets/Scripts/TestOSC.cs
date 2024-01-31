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

    // OSC�A�h���X���w�肵�܂��B
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
        // ���b�Z�[�W�̓��e�Ɋ�Â��čs�����������������ɋL�q���܂��B
        Debug.Log($"Received OSC message: {message}");
    }

}