using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authenticator : MonoBehaviour
{
    public static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymouslyAsync();
    }
    private static async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
