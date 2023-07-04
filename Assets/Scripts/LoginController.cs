using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    TMP_InputField Inputbox;

    public void OnLogin()
    {
        if(!string.IsNullOrEmpty(Inputbox.text))
            Main.GetSingleton.Username = Inputbox.text;
        
        SceneManager.LoadScene("Assets/Scenes/ChatScene.unity", LoadSceneMode.Single);
    }
}
