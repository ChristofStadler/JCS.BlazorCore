using System;
using System.Threading.Tasks;
using BlazorCore.Utilities;
using Microsoft.JSInterop;

namespace BlazorCore.Services
{
    public class AccountService
    {
        public async Task<string> GetUID(IJSRuntime jsRuntime)
        {
            string uid = await jsRuntime.InvokeAsync<string>("app.GetUID");

            if (String.IsNullOrEmpty(uid))
            {
                uid = Guid.NewGuid().ToString();
                await jsRuntime.InvokeAsync<object>("app.SetUID", uid);
            }

            return uid;
        }
    }
}
