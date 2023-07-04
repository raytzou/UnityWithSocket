using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    private void Start()
    {
        TextBox.text = "";
        InputBox.text = "";
        GetComponent<CanvasGroup>().alpha = 0f;
        ChatMessageQueue = new();

        Main.GetSingleton.PrintNetInfo();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OpenChatToggler = !OpenChatToggler;

            if(!string.IsNullOrEmpty(InputBox.text))
                OnSubmitMessage();

            if (OpenChatToggler)
                GetComponent<CanvasGroup>().alpha = 1f;
            else
                GetComponent<CanvasGroup>().alpha = 0f;
        }

        if(Input.GetKeyDown(KeyCode.I)) Main.GetSingleton.PrintNetInfo();
    }

    public void OnSubmitMessage()
    {
        if (string.IsNullOrEmpty(InputBox.text)) return; // prevent Button onClick() with no text

        ChatMessageQueue.Enqueue(InputBox.text);
        Main.GetSingleton.SendMessage(InputBox.text);

        TextBox.text = "";
        if (ChatMessageQueue.Count != 0)
        {
            foreach (string message in ChatMessageQueue)
                TextBox.text += "\n" + Main.GetSingleton.Username + ": " + message;
        }
        if (ChatMessageQueue.Count == 13)
            ChatMessageQueue.Dequeue();

        OpenChatToggler = true;
        InputBox.text = "";
    }
}
