using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Utils
{
    public static class StateManager
    {
        public static void SaveState(this PhoneApplicationPage phoneApplicationPage, string key, object value)
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                phoneApplicationPage.State.Remove(key);
            }
            phoneApplicationPage.State.Add(key, value);
        }

        public static T LoadState<T>(this PhoneApplicationPage phoneApplicationPage, string key)
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                return (T)phoneApplicationPage.State[key];
            }
            return default(T);
        }

    }
}
