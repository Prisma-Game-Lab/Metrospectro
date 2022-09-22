using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Authentication.Samples
{
    /// <summary>
    /// This sample requires the player to provide the facebook access token manually.
    /// You should integrate the facebook unity sdk to login and retrieve the access token.
    /// </summary>
    public class FacebookLoginUIExample : MonoBehaviour
    {
        [SerializeField]
        Button m_SignInAnonymouslyButton;
        [SerializeField]
        Button m_SignInFacebookButton;
        [SerializeField]
        Button m_LinkFacebookButton;
        [SerializeField]
        Button m_UnlinkFacebookButton;
        [SerializeField]
        Button m_SignOutButton;
        [SerializeField]
        Button m_ClearSessionButton;
        [SerializeField]
        Button m_GetPlayerInfoButton;

        [SerializeField]
        InputField m_TokenInputField;
        [SerializeField]
        Text m_PlayerIdText;
        [SerializeField]
        Text m_StatusText;
        [SerializeField]
        Text m_SessionText;
        [SerializeField]
        Text m_ExceptionText;
        [SerializeField]
        Text m_PlayerInfoText;

        public async void OnClickSignInAnonymously() => await SignInAnonymouslyAsync();
        public async void OnClickSignInFacebook() => await SignInWithFacebookAsync(m_TokenInputField.text);
        public async void OnClickLinkFacebook() => await LinkWithFacebookAsync(m_TokenInputField.text);
        public async void OnClickUnlinkFacebook() => await UnlinkFacebookAsync();
        public async void OnClickGetPlayerInfo() => await GetPlayerInfoAsync();
        public void OnClickSignOut() => SignOut();
        public void OnClickClearSessionToken() => ClearSessionToken();

        /// <summary>
        /// Initialize unity services and setup event handlers.
        /// </summary>
        async void Start()
        {
            await UnityServices.InitializeAsync();
            Debug.Log($"Unity services initialization: {UnityServices.State}");

            // Shows if a session token exists
            Debug.Log($"Session Token Exists: {AuthenticationService.Instance.SessionTokenExists}");

            AuthenticationService.Instance.SignedIn += () =>
            {
                //Shows how to get a playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

                //Shows how to get an access token
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            };

            AuthenticationService.Instance.SignInFailed += errorResponse =>
            {
                Debug.LogError($"Sign in failed with error code: {errorResponse.ErrorCode}");
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log($"Player signed out!");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log($"Player session expired!");
            };

            UpdateUI();
        }

        /// <summary>
        /// Signs in anonymously: uses the session token to login to an existing account if it exists, otherwise creates an account and caches the session token.
        /// </summary>
        async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                SetStatus("Signed in anonymously!");
                UpdateUI();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetStatus("Failed to sign in anonymously!");
                SetException(ex);
            }
        }

        /// <summary>
        /// When the player triggers the Facebook login by signing in or by creating a new player profile,
        /// and you have received the Facebook access token, call the following API to authenticate the player
        /// </summary>
        /// <param name="accessToken">The facebook user access token.</param>
        async Task SignInWithFacebookAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
                SetStatus("Signed in with Facebook!");
                UpdateUI();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetStatus("Failed to sign in with Facebook!");
                SetException(ex);
            }
        }

        /// <summary>
        /// When the player wants to upgrade from being anonymous to creating a Facebook social account and sign in using Facebook,
        /// the game should prompt the player to trigger the Facebook login and get the access token from Facebook.
        /// Then, call the following API to link the player to the Facebook Access token
        /// </summary>
        /// <param name="accessToken">The facebook user access token.</param>
        async Task LinkWithFacebookAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
                SetStatus("Linked with Facebook!");
                UpdateUI();
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                SetStatus("Linking failed. Account is already linked!");
                Debug.LogException(ex);
                SetException(ex);
            }
            catch (Exception ex)
            {
                SetStatus("Failed to link with Facebook!");
                Debug.LogException(ex);
                SetException(ex);
            }
        }

        /// <summary>
        /// The player can be offered to unlink his facebook account.
        /// The game should call this api.
        /// Unlinking requires the facebook player info to be present.
        /// </summary>
        async Task UnlinkFacebookAsync()
        {
            try
            {
                await AuthenticationService.Instance.UnlinkFacebookAsync();
                SetStatus("Unlinked Facebook account!");
                UpdateUI();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetStatus("Failed to unlink Facebook account!");
                SetException(ex);
            }
        }

        /// <summary>
        /// Signs the player out.
        /// </summary>
        void SignOut()
        {
            AuthenticationService.Instance.SignOut();
            SetStatus("Signed out!");
            UpdateUI();
        }

        /// <summary>
        /// Deletes the session token from the cache to allow logging in to a new account.
        /// </summary>
        void ClearSessionToken()
        {
            try
            {
                AuthenticationService.Instance.ClearSessionToken();
                SetStatus("Session Token Cleared!");
                UpdateUI();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                SetStatus("Failed to clear session token!");
                SetException(ex);
            }
        }

        /// <summary>
        /// Retrieves the player info, including the external ids.
        /// </summary>
        async Task GetPlayerInfoAsync()
        {
            try
            {
                await AuthenticationService.Instance.GetPlayerInfoAsync();
                UpdateUI();
                SetStatus("Obtained PlayerInfo!");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                SetStatus("Failed to get PlayerInfo!");
                SetException(ex);
            }
        }

        /// <summary>
        /// Returns Player info string if the player is authorized
        /// </summary>
        /// <returns>the player info summary text.</returns>
        string GetPlayerInfoText()
        {
            var playerInfo = AuthenticationService.Instance.PlayerInfo;


            if (playerInfo?.CreatedAt == null)
                return string.Empty;

            var dateTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(playerInfo.CreatedAt));

            var builder = new StringBuilder();

            builder.AppendLine($"CreatedAt: {dateTime.LocalDateTime}");
            builder.AppendLine($"Id: {playerInfo.Id}");
            builder.AppendLine();

            if (playerInfo.Identities?.Count > 0)
            {
                builder.AppendLine($"External Ids:");

                foreach (var externalId in playerInfo.Identities)
                {
                    builder.AppendLine($"{externalId.TypeId}: {externalId.UserId}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Updates the interactivity and display of the UI.
        /// </summary>
        void UpdateUI()
        {
            var isSignedIn = AuthenticationService.Instance.IsSignedIn;
            m_PlayerIdText.text = $"PlayerId: {(isSignedIn ? AuthenticationService.Instance.PlayerId : "")}";
            m_SessionText.text = AuthenticationService.Instance.SessionTokenExists ? "Session Token: Exists" : "Session Token: Not Found";
            m_SignInAnonymouslyButton.interactable = !isSignedIn;
            m_SignInFacebookButton.interactable = !isSignedIn;
            m_SignOutButton.interactable = isSignedIn;
            m_ClearSessionButton.interactable = !isSignedIn;
            m_LinkFacebookButton.interactable = isSignedIn;
            m_UnlinkFacebookButton.interactable = isSignedIn;
            m_GetPlayerInfoButton.interactable = isSignedIn;
            m_PlayerInfoText.text = isSignedIn ? GetPlayerInfoText() : "";
            SetException(null);
        }

        /// <summary>
        /// Updates the status text component.
        /// </summary>
        /// <param name="status">The status to display.</param>
        void SetStatus(string status)
        {
            m_StatusText.text = $"Status: {status}";
        }

        /// <summary>
        /// Updates the exception text component.
        /// </summary>
        /// <param name="ex">The exception to display.</param>
        void SetException(Exception ex)
        {
            m_ExceptionText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
        }
    }
}
