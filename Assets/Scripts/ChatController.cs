using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class ChatController : MonoBehaviour
{
    private bool OpenChatToggler = false;
    private Queue<string> ChatMessageQueue;

    [SerializeField]
    TMP_Text TextBox;
    [SerializeField]
    TMP_InputField InputBox;
    [SerializeField]
    Button Button;
    [SerializeField]
    TMP_Text SignalTextBox;
    [SerializeField]
    Image Signal;

    private void Start()
    {
        TextBox.text = "";
        InputBox.text = "";
        GetComponent<CanvasGroup>().alpha = 0f;
        ChatMessageQueue = new();

        Main.GetSingleton.PrintNetInfo();
    }

    private async void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OpenChatToggler = !OpenChatToggler;

            if (!string.IsNullOrEmpty(InputBox.text))
                OnSubmitMessage();

            if (OpenChatToggler)
            {
                GetComponent<CanvasGroup>().alpha = 1f;
                InputBox.ActivateInputField();
            }
            else
                GetComponent<CanvasGroup>().alpha = 0f;
        }

        if(Input.GetKeyDown(KeyCode.I) && !OpenChatToggler) Main.GetSingleton.PrintNetInfo();


        await Main.GetSingleton.ConnectToServer();
        //Debug.LogError(Main.GetSingleton.IsOnline);

        if (Main.GetSingleton.IsOnline)
        {
            Signal.GetComponent<Image>().color = Color.green;
            SignalTextBox.text = "Status: Online";
            await Main.GetSingleton.ReceiveMessage();
        }
        else // bad... this not gonna happen lol
        {
            Signal.GetComponent<Image>().color = Color.red;
            SignalTextBox.text = "Status: Offline";
        }
    }

    public async void OnSubmitMessage()
    {
        if (string.IsNullOrEmpty(InputBox.text)) return; // prevent Button onClick() with no text

        ChatMessageQueue.Enqueue(InputBox.text);
        await Main.GetSingleton.SendMessage(InputBox.text);

        UpdateChatBox();

        OpenChatToggler = true;
        InputBox.text = "";
        //InputBox.ActivateInputField();
    }

    private void UpdateChatBox()
    {
        TextBox.text = "";
        if (ChatMessageQueue.Count != 0)
        {
            foreach (string message in ChatMessageQueue)
                TextBox.text += "\n" + Main.GetSingleton.Username + ": " + message;
        }
        if (ChatMessageQueue.Count == 13)
            ChatMessageQueue.Dequeue();
    }
}
