using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSetup : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] private Button connectButton;

    //Int values for player name size
    [SerializeField] private int minNameSize = 1;
    [SerializeField] private int maxNameSize = 15;


    public const string PLAYER_NAME_KEY = "PlayerName";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        nameField.text = PlayerPrefs.GetString(PLAYER_NAME_KEY, string.Empty);
        HandleNameChange();
    }

    public void HandleNameChange()
    {
        //Set button interactable to match if the player has a valid name
        connectButton.interactable = nameField.text.Length >= minNameSize && nameField.text.Length <= maxNameSize;
    }


    public void Conect()
    {
        //Set player name
        PlayerPrefs.SetString(PLAYER_NAME_KEY, nameField.text);

        //load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
