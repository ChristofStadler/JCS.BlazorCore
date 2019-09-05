using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Utilities
{
    public class LocalStorage
    {
        private readonly IJSRuntime JSRuntime;

        public LocalStorage(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }
        public void SetItem(string key, string value)
        {
            var result = JSRuntime.InvokeAsync<object>("localStorage.setItem", key, value);
        }
        public Task<string> GetItem(string key)
        {
            return JSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
    }
}
