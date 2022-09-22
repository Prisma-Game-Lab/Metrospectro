using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authenticator : MonoBehaviour
{
    public static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        Logger.Instance.LogInfo($"Unity Services {UnityServices.State}");
        await SignInAnonymouslyAsync();
    }
    private static async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Logger.Instance.LogInfo($"Sessao autenticada!\nPlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {

            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {

            Debug.LogException(ex);
        }
    }
}
