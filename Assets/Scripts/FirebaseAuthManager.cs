
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;

/// <summary>
/// Attach to an empty GameObject in the Account scene.
/// Expose SignUp() and LogIn() methods for Button OnClick().
/// Requires Firebase Unity SDK (Core + Auth) and a configured Firebase project.
/// </summary>
public class FirebaseAuthManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI statusLabel;

    private FirebaseAuth auth;

    private async void Awake()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        var deps = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (deps == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Log("Firebase ready.");
        }
        else
        {
            LogError($"Firebase deps unresolved: {deps}");
        }
    }

    // --- called from UI ---
    public async void SignUp()
    {
        if (!ValidateFields()) return;

        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text);
            Log($"Signed up as {emailInput.text}");
        }
        catch (FirebaseException ex)
        {
            LogError($"Sign‑up failed: {(AuthError)ex.ErrorCode}");
        }
    }

    public async void LogIn()
    {
        if (!ValidateFields()) return;

        try
        {
            await auth.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text);
            Log($"Welcome, {auth.CurrentUser.Email}");
        }
        catch (FirebaseException ex)
        {
            LogError($"Login failed: {(AuthError)ex.ErrorCode}");
        }
    }

    // --- utils ---
    private bool ValidateFields()
    {
        if (string.IsNullOrWhiteSpace(emailInput.text) || !emailInput.text.Contains("@"))
        {
            LogError("Invalid email.");
            return false;
        }
        if (passwordInput.text.Length < 6)
        {
            LogError("Password ≥ 6 chars.");
            return false;
        }
        return true;
    }

    private void Log(string msg)
    {
        statusLabel.text = msg;
        Debug.Log(msg);
    }

    private void LogError(string msg)
    {
        statusLabel.text = $"<color=#FF5555>{msg}</color>";
        Debug.LogWarning(msg);
    }
}
