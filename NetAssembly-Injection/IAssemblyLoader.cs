using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAssembly_Injection
{
    /// <summary>
    /// interface exposing .NET assembly loading methods for other classes to implement.
    /// new methods can be implemented here if any new techniques are found.
    /// </summary>
    interface IAssemblyLoader
    {
        void Load(string b64Assembly, string methodName, string[] arguments);

        void LoadByUrl(string assemblyURL, string methodName, string[] arguments);

        void LoadFromRemoteHost(string remoteAssemblyURL, string[] arguments);
    }
}
