using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private Queue<Action> actions = new Queue<Action>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        document.rootVisualElement.Q<TextField>("email_field").value = PlayerPrefs.GetString("username");
        document.rootVisualElement.Q<TextField>("password_field").value = PlayerPrefs.GetString("password");
        document.rootVisualElement.Q<TextField>("name_field").value = PlayerPrefs.GetString("name");
        document.rootVisualElement.Q<Button>("login_button").clicked += Onclicked;
    }

    private void Onclicked()
    {
        Task.Run(Signup);
    }

    private async Task Signup()
    {
        var email = document.rootVisualElement.Q<TextField>("email_field").value;
        var password = document.rootVisualElement.Q<TextField>("password_field").value;
        var name = document.rootVisualElement.Q<TextField>("name_field").value;
        
        actions.Enqueue(() => PlayerPrefs.SetString("username", email));
        actions.Enqueue(() => PlayerPrefs.SetString("password", password));
        actions.Enqueue(() => PlayerPrefs.SetString("name", name));
        
        var id = await DatabaseManager.Instance.SignIn(email, password, name);

        actions.Enqueue(() =>
        {
            var label = document.rootVisualElement.Q<Label>("character_label");
            label.text = id;
        });

    }

    // Update is called once per frame
    void Update()
    {
        while (actions.Count > 0) actions.Dequeue().Invoke();
        //document.rootVisualElement.Q<Label>("character_label").text = Time.timeSinceLevelLoad.ToString("F2");
    }
}
