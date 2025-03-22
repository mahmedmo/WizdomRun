using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    // Panels
    public GameObject MainMenu;
    public GameObject Auth;
    public GameObject LoginHeader;
    public GameObject AppLoad;

    // Signup UI elements
    public GameObject SignupHeader;
    public GameObject NameLabel;
    public GameObject NameField;
    public GameObject SignupNameInput;
    public GameObject SignupEmailInput;
    public GameObject SignupPassInput;
    public GameObject SignupButton;

    // Login UI elements
    public GameObject LoginEmailInput;
    public GameObject LoginPassInput;
    public GameObject LoginButton;

    // Error texts (assume these are TextMeshProUGUI objects)
    public GameObject NameErrorText;
    public GameObject EmailErrorText;
    public GameObject PasswordErrorText;
    public GameObject AuthErrorText;

    private Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MainMenu.SetActive(true);
        Auth.SetActive(false);
        AppLoad.SetActive(false);
        NameErrorText.SetActive(false);
        EmailErrorText.SetActive(false);
        PasswordErrorText.SetActive(false);
        AuthErrorText.SetActive(false);

        TMP_InputField sPassField = SignupPassInput.GetComponent<TMP_InputField>();
        TMP_InputField lpassField = LoginPassInput.GetComponent<TMP_InputField>();
        sPassField.contentType = TMP_InputField.ContentType.Password;
        lpassField.contentType = TMP_InputField.ContentType.Password;
        sPassField.ForceLabelUpdate();
        lpassField.ForceLabelUpdate();
    }

    public void OnSignupMenuClick()
    {
        MainMenu.SetActive(false);
        Auth.SetActive(true);
        LoginHeader.SetActive(false);
        SignupHeader.SetActive(true);
        NameLabel.SetActive(true);
        NameField.SetActive(true);

        SignupNameInput.SetActive(true);
        SignupEmailInput.SetActive(true);
        SignupPassInput.SetActive(true);
        SignupButton.SetActive(true);
        SignupNameInput.GetComponent<TMP_InputField>().text = "";
        SignupEmailInput.GetComponent<TMP_InputField>().text = "";
        SignupPassInput.GetComponent<TMP_InputField>().text = "";

        LoginEmailInput.SetActive(false);
        LoginPassInput.SetActive(false);
        LoginButton.SetActive(false);
        LoginEmailInput.GetComponent<TMP_InputField>().text = "";
        LoginPassInput.GetComponent<TMP_InputField>().text = "";

        EmailErrorText.SetActive(false);
        PasswordErrorText.SetActive(false);
        AuthErrorText.SetActive(false);
    }

    public void OnLoginMenuClick()
    {
        MainMenu.SetActive(false);
        Auth.SetActive(true);
        LoginHeader.SetActive(true);
        SignupHeader.SetActive(false);
        NameLabel.SetActive(false);
        NameField.SetActive(false);

        LoginEmailInput.SetActive(true);
        LoginPassInput.SetActive(true);
        LoginButton.SetActive(true);
        LoginEmailInput.GetComponent<TMP_InputField>().text = "";
        LoginPassInput.GetComponent<TMP_InputField>().text = "";

        SignupEmailInput.SetActive(false);
        SignupPassInput.SetActive(false);
        SignupButton.SetActive(false);
        SignupEmailInput.GetComponent<TMP_InputField>().text = "";
        SignupPassInput.GetComponent<TMP_InputField>().text = "";

        NameErrorText.SetActive(false);
        EmailErrorText.SetActive(false);
        PasswordErrorText.SetActive(false);
        AuthErrorText.SetActive(false);
    }

    public void OnSignupSubmit()
    {
        TMP_InputField nameField = SignupNameInput.GetComponent<TMP_InputField>();
        TMP_InputField emailField = SignupEmailInput.GetComponent<TMP_InputField>();
        TMP_InputField passField = SignupPassInput.GetComponent<TMP_InputField>();

        string name = nameField.text;
        string email = emailField.text;
        string password = passField.text;

        bool isValid = true;

        if (name.Length <= 1)
        {
            ShowError(NameErrorText, "Name must be longer than 2 characters.");
            isValid = false;
        }
        else
        {
            NameErrorText.SetActive(false);
        }

        if (!emailRegex.IsMatch(email))
        {
            ShowError(EmailErrorText, "Invalid email address.");
            isValid = false;
        }
        else
        {
            EmailErrorText.SetActive(false);
        }

        if (password.Length <= 6)
        {
            ShowError(PasswordErrorText, "Password must be longer than 6 characters.");
            isValid = false;
        }
        else
        {
            PasswordErrorText.SetActive(false);
        }

        ClearFirebaseError();

        if (isValid)
        {
            Debug.Log("MainMenuManager: Calling AuthManager.Signup with name: " + name + ", email: " + email);
            AuthManager.Instance.Signup(name, email, password);
        }
    }

    public void OnLoginSubmit()
    {
        TMP_InputField emailField = LoginEmailInput.GetComponent<TMP_InputField>();
        TMP_InputField passField = LoginPassInput.GetComponent<TMP_InputField>();

        string email = emailField.text;
        string password = passField.text;

        ClearFirebaseError();
        Debug.Log("MainMenuManager: Calling AuthManager.Login with email: " + email);
        AuthManager.Instance.Login(email, password);
    }

    public void OnBackButtonClick()
    {
        Auth.SetActive(false);
        MainMenu.SetActive(true);
    }
    public void OnLoadStart()
    {
        AppLoad!.SetActive(true);
    }
    public void OnLoadDone()
    {
        AppLoad!.SetActive(false);
    }

    // Helper method to show error messages on TMP error text objects.
    private void ShowError(GameObject errorTextObj, string message)
    {
        errorTextObj.SetActive(true);
        TMP_Text tmpText = errorTextObj.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = message;
        }
    }

    // Clears any Firebase error messages from PasswordErrorText.
    private void ClearFirebaseError()
    {
        AuthErrorText.SetActive(false);
        TMP_Text tmpText = AuthErrorText.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = "";
        }
    }

    // Method to display Firebase errors on PasswordErrorText.
    public void DisplayFirebaseError(string errorMsg)
    {
        Debug.Log("DISPLAYING" + errorMsg);
        ShowError(AuthErrorText, errorMsg);
    }

    // Loads the UserMenu scene after successful authentication.
    public void LoadUserMenu()
    {
        SceneManager.LoadScene("UserMenu");
    }
}