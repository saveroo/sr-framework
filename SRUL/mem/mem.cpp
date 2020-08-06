export module snippet;

import <string>;
import <unordered_map>;
import <vector>;
import <iostream>;
import <cmath>;
import <limits>;
import <cctype>;
import <stdexcept>;
import <any>;
import stringhelper;
import stringbuilder;
import tangible_filesystem;

//C# TO C++ CONVERTER NOTE: Forward class declarations:
namespace Memories { class MEMORY_BASIC_INFORMATION32; }
namespace Memories { class MEMORY_BASIC_INFORMATION64; }
namespace Memories { class MEMORY_BASIC_INFORMATION; }
namespace Memories { class SYSTEM_INFO; }

// TODO: Convert memory.dll to C++ equivalent;

namespace Memories
{
	/// <summary>
	/// Memory.dll class. Full documentation at https://github.com/erfg12/memory.dll/wiki
	/// </summary>
	export class Meme
	{
		//        public System.Diagnostics.Process

		//		#region DllImports
		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
	public:
		__declspec(dllimport) static std::any OpenProcess(unsigned int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

#if defined(WINXP)
#else
		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
		__declspec(dllimport) static UIntPtr Native_VirtualQueryEx(std::any hProcess, UIntPtr lpAddress, MEMORY_BASIC_INFORMATION32& lpBuffer, UIntPtr dwLength);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
		__declspec(dllimport) static UIntPtr Native_VirtualQueryEx(std::any hProcess, UIntPtr lpAddress, MEMORY_BASIC_INFORMATION64& lpBuffer, UIntPtr dwLength);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static unsigned int GetLastError();

		virtual ~Meme()
		{
			delete theProc;
			delete mainModule;
		}

		UIntPtr VirtualQueryEx(std::any hProcess, UIntPtr lpAddress, MEMORY_BASIC_INFORMATION& lpBuffer)
		{
			UIntPtr retVal;

			// TODO: Need to change this to only check once.
			if (getIs64Bit() || std::any::Size == 8)
			{
				// 64 bit
				MEMORY_BASIC_INFORMATION64 tmp64 = MEMORY_BASIC_INFORMATION64();
				retVal = Native_VirtualQueryEx(hProcess, lpAddress, tmp64, UIntPtr(static_cast<unsigned int>(System::Runtime::InteropServices::Marshal::SizeOf(tmp64))));

				lpBuffer.BaseAddress = tmp64.BaseAddress;
				lpBuffer.AllocationBase = tmp64.AllocationBase;
				lpBuffer.AllocationProtect = tmp64.AllocationProtect;
				lpBuffer.RegionSize = static_cast<long long>(tmp64.RegionSize);
				lpBuffer.State = tmp64.State;
				lpBuffer.Protect = tmp64.Protect;
				lpBuffer.Type = tmp64.Type;

				return retVal;
			}

			MEMORY_BASIC_INFORMATION32 tmp32 = MEMORY_BASIC_INFORMATION32();

			retVal = Native_VirtualQueryEx(hProcess, lpAddress, tmp32, UIntPtr(static_cast<unsigned int>(System::Runtime::InteropServices::Marshal::SizeOf(tmp32))));

			lpBuffer.BaseAddress = tmp32.BaseAddress;
			lpBuffer.AllocationBase = tmp32.AllocationBase;
			lpBuffer.AllocationProtect = tmp32.AllocationProtect;
			lpBuffer.RegionSize = tmp32.RegionSize;
			lpBuffer.State = tmp32.State;
			lpBuffer.Protect = tmp32.Protect;
			lpBuffer.Type = tmp32.Type;

			return retVal;
		}

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static void GetSystemInfo(SYSTEM_INFO& lpSystemInfo);
#endif

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static std::any OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, unsigned int dwThreadId);
		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static unsigned int SuspendThread(std::any hThread);
		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static int ResumeThread(std::any hThread);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("dbghelp.dll")]
		__declspec(dllimport) static bool MiniDumpWriteDump(std::any hProcess, int ProcessId, std::any hFile, MINIDUMP_TYPE DumpType, std::any ExceptionParam, std::any UserStreamParam, std::any CallackParam);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("user32.dll", SetLastError = true)]
		__declspec(dllimport) static int GetWindowLong(std::any hWnd, int nIndex);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("user32.dll", CharSet = CharSet::Auto, SetLastError = false)]
		__declspec(dllimport) static std::any SendMessage(std::any hWnd, unsigned int Msg, std::any w, std::any l);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static bool WriteProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, const std::wstring& lpBuffer, UIntPtr nSize, std::any& lpNumberOfBytesWritten);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static int GetProcessId(std::any handle);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", CharSet = CharSet::Unicode)]
		__declspec(dllimport) static unsigned int GetPrivateProfileString(const std::wstring& lpAppName, const std::wstring& lpKeyName, const std::wstring& lpDefault, StringBuilder* lpReturnedString, unsigned int nSize, const std::wstring& lpFileName);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		__declspec(dllimport) static bool VirtualFreeEx(std::any hProcess, UIntPtr lpAddress, UIntPtr dwSize, unsigned int dwFreeType);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
	private:
		__declspec(dllimport) static bool ReadProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, std::vector<unsigned char>& lpBuffer, UIntPtr nSize, std::any lpNumberOfBytesRead);

		// Saveroo
//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
//[DllImport("kernel32.dll")]
		__declspec(dllimport) static UIntPtr RPM(std::any hProcess, UIntPtr& lpBaseAddress, std::vector<unsigned char>& lpBuffer, UIntPtr nSize, std::any lpNumberOfBytesRead);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static bool ReadProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, std::vector<unsigned char>& lpBuffer, UIntPtr nSize, unsigned long long& lpNumberOfBytesRead);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
		__declspec(dllimport) static bool ReadProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, std::any lpBuffer, UIntPtr nSize, unsigned long long& lpNumberOfBytesRead);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		__declspec(dllimport) static UIntPtr VirtualAllocEx(std::any hProcess, UIntPtr lpAddress, unsigned int dwSize, unsigned int flAllocationType, unsigned int flProtect);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", CharSet = CharSet::Ansi, ExactSpelling = true)]
	public:
		__declspec(dllimport) static UIntPtr GetProcAddress(std::any hModule, const std::wstring& procName);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
	private:
		__declspec(dllimport) static bool _CloseHandle(std::any hObject);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
	public:
		__declspec(dllimport) static int CloseHandle(std::any hObject);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll", CharSet = CharSet::Auto)]
		__declspec(dllimport) static std::any GetModuleHandle(const std::wstring& lpModuleName);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
		__declspec(dllimport) static int WaitForSingleObject(std::any handle, int milliseconds);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32.dll")]
	private:
		__declspec(dllimport) static bool WriteProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, std::vector<unsigned char>& lpBuffer, UIntPtr nSize, std::any lpNumberOfBytesWritten);

		// Added to avoid casting to UIntPtr
//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
//[DllImport("kernel32.dll")]
		__declspec(dllimport) static bool WriteProcessMemory(std::any hProcess, UIntPtr lpBaseAddress, std::vector<unsigned char>& lpBuffer, UIntPtr nSize, std::any& lpNumberOfBytesWritten);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32")]
	public:
		__declspec(dllimport) static std::any CreateRemoteThread(std::any hProcess, std::any lpThreadAttributes, unsigned int dwStackSize, UIntPtr lpStartAddress, UIntPtr lpParameter, unsigned int dwCreationFlags, std::any& lpThreadId);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("kernel32")]
		__declspec(dllimport) static bool IsWow64Process(std::any hProcess, bool& lpSystemInfo);

		//C# TO C++ CONVERTER NOTE: The following .NET DllImport attribute was converted using the Microsoft-specific __declspec(dllimport):
		//[DllImport("user32.dll")]
		__declspec(dllimport) static bool SetForegroundWindow(std::any hWnd);

		// privileges
	private:
		static constexpr int PROCESS_CREATE_THREAD = 0x0002;
		static constexpr int PROCESS_QUERY_INFORMATION = 0x0400;
		static constexpr int PROCESS_VM_OPERATION = 0x0008;
		static constexpr int PROCESS_VM_WRITE = 0x0020;
		static constexpr int PROCESS_VM_READ = 0x0010;

		// used for memory allocation
		static constexpr unsigned int MEM_FREE = 0x10000;
		static constexpr unsigned int MEM_COMMIT = 0x00001000;
		static constexpr unsigned int MEM_RESERVE = 0x00002000;

		static constexpr unsigned int PAGE_READONLY = 0x02;
		static constexpr unsigned int PAGE_READWRITE = 0x04;
		static constexpr unsigned int PAGE_WRITECOPY = 0x08;
		static constexpr unsigned int PAGE_EXECUTE_READWRITE = 0x40;
		static constexpr unsigned int PAGE_EXECUTE_WRITECOPY = 0x80;
		static constexpr unsigned int PAGE_EXECUTE = 0x10;
		static constexpr unsigned int PAGE_EXECUTE_READ = 0x20;

		static constexpr unsigned int PAGE_GUARD = 0x100;
		static constexpr unsigned int PAGE_NOACCESS = 0x01;

		unsigned int MEM_PRIVATE = 0x20000;
		unsigned int MEM_IMAGE = 0x1000000;

		//		#endregion

				/// <summary>
				/// The process handle that was opened. (Use OpenProcess function to populate this variable)
				/// </summary>
	public:
		std::any pHandle;
	private:
		std::unordered_map<std::wstring, CancellationTokenSource*> FreezeTokenSrcs = std::unordered_map<std::wstring, CancellationTokenSource*>();
		std::unordered_map<std::wstring, CancellationTokenSource*> SRFreezeTokenSrcs = std::unordered_map<std::wstring, CancellationTokenSource*>();
	public:
		System::Diagnostics::Process* theProc = nullptr;

	public:
		enum class MINIDUMP_TYPE
		{
			MiniDumpNormal = 0x00000000,
			MiniDumpWithDataSegs = 0x00000001,
			MiniDumpWithFullMemory = 0x00000002,
			MiniDumpWithHandleData = 0x00000004,
			MiniDumpFilterMemory = 0x00000008,
			MiniDumpScanMemory = 0x00000010,
			MiniDumpWithUnloadedModules = 0x00000020,
			MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
			MiniDumpFilterModulePaths = 0x00000080,
			MiniDumpWithProcessThreadData = 0x00000100,
			MiniDumpWithPrivateReadWriteMemory = 0x00000200,
			MiniDumpWithoutOptionalData = 0x00000400,
			MiniDumpWithFullMemoryInfo = 0x00000800,
			MiniDumpWithThreadInfo = 0x00001000,
			MiniDumpWithCodeSegs = 0x00002000
		};

	private:
		bool IsDigitsOnly(const std::wstring& str)
		{
			for (auto c : str)
			{
				if (c < L'0' || c > L'9')
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Freeze a value to an address.
		/// </summary>
		/// <param name="address">Your address</param>
		/// <param name="type">byte, 2bytes, bytes, float, int, string, double or long.</param>
		/// <param name="value">Value to freeze</param>
		/// <param name="file">ini file to read address from (OPTIONAL)</param>
	public:
		void FreezeValue(const std::wstring& address, const std::wstring& type, const std::wstring& value, const std::wstring& file = L"")
		{
			CancellationTokenSource* cts = new CancellationTokenSource();

			if (FreezeTokenSrcs.find(address) != FreezeTokenSrcs.end())
			{
				Debug::WriteLine(L"Changing Freezing Address " + address + L" Value " + value);
				std::wcout << L"SURGA -> Changing Freezing Address " << address << L" Value " << value << std::endl;
				try
				{
					FreezeTokenSrcs[address]->Cancel();
					FreezeTokenSrcs.erase(address);
				}
				catch (...)
				{
					Debug::WriteLine(L"ERROR: Avoided a crash. Address " + address + L" was not frozen.");
				}
			}
			else
			{
				Debug::WriteLine(L"Adding Freezing Address " + address + L" Value " + value);
			}

			FreezeTokenSrcs.emplace(address, cts);

			Task::Factory->StartNew([&]()
			{
				while (!cts->Token.IsCancellationRequested)
				{
					WriteMemory(address, type, value, file);
					delay(25);
				}
			}, cts->Token);

			//C# TO C++ CONVERTER TODO TASK: A 'delete cts' statement was not added since cts was passed to a method or constructor. Handle memory management manually.
		}

		/// <summary>
		/// SR Freeze a value to an address.
		/// </summary>
		/// <param name="address">Your address</param>
		/// <param name="type">byte, 2bytes, bytes, float, int, string, double or long.</param>
		/// <param name="value">Value to freeze</param>
		/// <param name="file">ini file to read address from (OPTIONAL)</param>
		void SRFreezeValue(const std::wstring& address, const std::wstring& type, const std::wstring& value, const std::wstring& file = L"")
		{
			CancellationTokenSource* cts = new CancellationTokenSource();
			//C# TO C++ CONVERTER TODO TASK: There is no C++ equivalent to 'ToString':
			std::wstring realAddress = GetCode(address, file).ToUInt32().ToString(L"X");

			if (FreezeTokenSrcs.find(realAddress) != FreezeTokenSrcs.end())
			{
				Debug::WriteLine(L"Changing SRFreezing Address " + realAddress + L" Value " + value);
				try
				{
					FreezeTokenSrcs[realAddress]->Cancel();
					FreezeTokenSrcs.erase(realAddress);
				}
				catch (...)
				{
					Debug::WriteLine(L"ERROR: Avoided a crash. Address " + realAddress + L" was not frozen.");
				}
			}
			else
			{
				Debug::WriteLine(L"Adding SRFreezing Address " + realAddress + L" Value " + value);
			}

			FreezeTokenSrcs.emplace(realAddress, cts);

			Task::Factory->StartNew([&]()
			{
				while (!cts->Token.IsCancellationRequested)
				{
					WriteMemory(realAddress, type, value, file);
					delay(25);
				}
			}, cts->Token);

			//C# TO C++ CONVERTER TODO TASK: A 'delete cts' statement was not added since cts was passed to a method or constructor. Handle memory management manually.
		}

		/// <summary>
		/// Unfreeze a frozen value at an address
		/// </summary>
		/// <param name="address">address where frozen value is stored</param>
		void UnfreezeValue(const std::wstring& address)
		{
			Debug::WriteLine(L"Un-Freezing Address " + address);
			try
			{
				FreezeTokenSrcs[address]->Cancel();
				FreezeTokenSrcs.erase(address);
			}
			catch (...)
			{
				Debug::WriteLine(L"ERROR: Address " + address + L" was not frozen.");
			}
		}

		/// <summary>
		/// Open the PC game process with all security and access rights.
		/// </summary>
		/// <param name="proc">Use process name or process ID here.</param>
		/// <returns></returns>
		bool OpenProcess(int pid)
		{
			if (!IsAdmin())
			{
				Debug::WriteLine(L"WARNING: You are NOT running this program as admin! Visit https://github.com/erfg12/memory.dll/wiki/Administrative-Privileges");
				MessageBox::Show(L"WARNING: You are NOT running this program as admin!");
			}

			if (pid <= 0)
			{
				Debug::WriteLine(L"ERROR: OpenProcess given proc ID 0.");
				return false;
			}


			if (theProc != nullptr && theProc->Id == pid)
			{
				return true;
			}

			try
			{
				theProc = System::Diagnostics::Process::GetProcessById(pid);

				if (theProc != nullptr && !theProc->Responding)
				{
					Debug::WriteLine(L"ERROR: OpenProcess: Process is not responding or null.");
					return false;
				}

				pHandle = OpenProcess(0x1F0FFF, true, pid);
				System::Diagnostics::Process::EnterDebugMode();

				if (pHandle == std::any::Zero)
				{
					auto eCode = Marshal::GetLastWin32Error();
					Debug::WriteLine(L"ERROR: OpenProcess has failed opening a handle to the target process (GetLastWin32ErrorCode: " + std::to_wstring(eCode) + L")");
					System::Diagnostics::Process::LeaveDebugMode();
					theProc = nullptr;
					return false;
				}

				mainModule = theProc->MainModule;

				GetModules();

				// Lets set the process to 64bit or not here (cuts down on api calls)
				bool retVal;
				setIs64Bit(Environment::Is64BitOperatingSystem && (IsWow64Process(pHandle, retVal) && !retVal));

				Debug::WriteLine(L"Program is operating at Administrative level. Process #" + theProc + L" is open and modules are stored.");

				return true;
			}
			catch (...)
			{
				Debug::WriteLine(L"ERROR: OpenProcess has crashed. Are you trying to hack a x64 game? https://github.com/erfg12/memory.dll/wiki/64bit-Games");
				return false;
			}
		}


		/// <summary>
		/// Open the PC game process with all security and access rights.
		/// </summary>
		/// <param name="proc">Use process name or process ID here.</param>
		/// <returns></returns>
		bool OpenProcess(const std::wstring& proc)
		{
			return OpenProcess(GetProcIdFromName(proc));
		}

		/// <summary>
		/// Check if program is running with administrative privileges. Read about it here: https://github.com/erfg12/memory.dll/wiki/Administrative-Privileges
		/// </summary>
		/// <returns></returns>
		bool IsAdmin()
		{
			//C# TO C++ CONVERTER NOTE: The following 'using' block is replaced by its C++ equivalent:
			//ORIGINAL LINE: using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsIdentity identity = WindowsIdentity::GetCurrent();
				WindowsPrincipal* principal = new WindowsPrincipal(identity);

				delete principal;
				return principal->IsInRole(WindowsBuiltInRole::Administrator);
			}
		}

		/// <summary>
		/// Check if opened process is 64bit. Used primarily for getCode().
		/// </summary>
		/// <returns>True if 64bit false if 32bit.</returns>
	private:
		bool _is64Bit = false;
	public:
		bool getIs64Bit() const
		{
			return _is64Bit;
		}
	private:
		void setIs64Bit(bool value)
		{
			_is64Bit = value;
		}


		/// <summary>
		/// Builds the process modules dictionary (names with addresses).
		/// </summary>
		void GetModules()
		{
			if (theProc == nullptr)
			{
				return;
			}

			modules.clear();
			for (auto Module : *theProc->Modules)
			{
				if (!Module->ModuleName.empty() && modules.find(Module->ModuleName) == modules.end())
				{
					modules.emplace(Module->ModuleName, Module->BaseAddress);
				}
			}
		}

		void SetFocus()
		{
			//int style = GetWindowLong(procs.MainWindowHandle, -16);
			//if ((style & 0x20000000) == 0x20000000) //minimized
			//    SendMessage(procs.Handle, 0x0112, (IntPtr)0xF120, IntPtr.Zero);
			SetForegroundWindow(theProc->MainWindowHandle);
		}

		/// <summary>
		/// Get the process ID number by process name.
		/// </summary>
		/// <param name="name">Example: "eqgame". Use task manager to find the name. Do not include .exe</param>
		/// <returns></returns>
		int GetProcIdFromName(const std::wstring& name) //new 1.0.2 function
		{
			std::vector<System::Diagnostics::Process*> processlist = System::Diagnostics::Process::GetProcesses();

			if (StringHelper::toLower(name).find(L".exe") != std::wstring::npos)
			{
				name = StringHelper::replace(name, L".exe", L"");
			}
			if (StringHelper::toLower(name).find(L".bin") != std::wstring::npos) // test
			{
				name = StringHelper::replace(name, L".bin", L"");
			}

			for (auto theprocess : processlist)
			{
				//C# TO C++ CONVERTER TODO TASK: The following .NET 'String.Equals' reference is not converted:
				if (theprocess->ProcessName.Equals(name, StringComparison::CurrentCultureIgnoreCase)) //find (name).exe in the process list (use task manager to find the name)
				{
					return theprocess->Id;
				}
			}

			return 0; //if we fail to find it
		}



		/// <summary>
		/// Get code from ini file.
		/// </summary>
		/// <param name="name">label for address or code</param>
		/// <param name="file">path and name of ini file</param>
		/// <returns></returns>
		std::wstring LoadCode(const std::wstring& name, const std::wstring& file)
		{
			StringBuilder* returnCode = new StringBuilder(1024);
			unsigned int read_ini_result;

			if (file != L"")
			{
				read_ini_result = GetPrivateProfileString(L"codes", name, L"", returnCode, static_cast<unsigned int>(returnCode->capacity()), file);
			}
			else
			{
				returnCode->append(name);
			}

			//C# TO C++ CONVERTER TODO TASK: A 'delete returnCode' statement was not added since returnCode was passed to a method or constructor. Handle memory management manually.
			return returnCode->toString();
		}

	private:
		int LoadIntCode(const std::wstring& name, const std::wstring& path)
		{
			try
			{
				int intValue = Convert::ToInt32(LoadCode(name, path), 16);
				if (intValue >= 0)
				{
					return intValue;
				}
				else
				{
					return 0;
				}
			}
			catch (...)
			{
				Debug::WriteLine(L"ERROR: LoadIntCode function crashed!");
				return 0;
			}
		}

		/// <summary>
		/// Dictionary with our opened process module names with addresses.
		/// </summary>
	public:
		std::unordered_map<std::wstring, std::any> modules = std::unordered_map<std::wstring, std::any>();

		/// <summary>
		/// Make a named pipe (if not already made) and call to a remote function.
		/// </summary>
		/// <param name="func">remote function to call</param>
		/// <param name="name">name of the thread</param>
		void ThreadStartClient(const std::wstring& func, const std::wstring& name)
		{
			//ManualResetEvent SyncClientServer = (ManualResetEvent)obj;
//C# TO C++ CONVERTER NOTE: The following 'using' block is replaced by its C++ equivalent:
//ORIGINAL LINE: using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(name))
			{
				NamedPipeClientStream pipeStream = NamedPipeClientStream(name);
				if (!pipeStream.IsConnected)
				{
					pipeStream.Connect();
				}

				//MessageBox.Show("[Client] Pipe connection established");
//C# TO C++ CONVERTER NOTE: The following 'using' block is replaced by its C++ equivalent:
//ORIGINAL LINE: using (StreamWriter sw = new StreamWriter(pipeStream))
				{
					StreamWriter sw = StreamWriter(pipeStream);
					if (!sw.AutoFlush)
					{
						sw.AutoFlush = true;
					}
					sw.WriteLine(func);
				}
			}
		}

	private:
		ProcessModule* mainModule;

		/// <summary>
		/// Cut a string that goes on for too long or one that is possibly merged with another string.
		/// </summary>
		/// <param name="str">The string you want to cut.</param>
		/// <returns></returns>
	public:
		std::wstring CutString(const std::wstring& str)
		{
			StringBuilder* sb = new StringBuilder();
			for (auto c : str)
			{
				if (c >= L' ' && c <= L'~')
				{
					sb->append(c);
				}
				else
				{
					break;
				}
			}

			delete sb;
			return sb->toString();
		}

		/// <summary>
		/// Clean up a string that has bad characters in it.
		/// </summary>
		/// <param name="str">The string you want to sanitize.</param>
		/// <returns></returns>
		std::wstring SanitizeString(const std::wstring& str)
		{
			StringBuilder* sb = new StringBuilder();
			for (auto c : str)
			{
				if (c >= L' ' && c <= L'~')
				{
					sb->append(c);
				}
			}

			delete sb;
			return sb->toString();
		}

		//		#region readMemory
				/// <summary>
				/// Reads up to `length ` bytes from an address.
				/// </summary>
				/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
				/// <param name="length">The maximum bytes to read.</param>
				/// <param name="file">path and name of ini file.</param>
				/// <returns>The bytes read or null</returns>
		std::vector<unsigned char> ReadBytes(const std::wstring& code, long long length, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(length);
			UIntPtr theCode = GetCode(code, file);

			if (!ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(length), std::any::Zero))
			{
				return std::vector<unsigned char>();
			}

			return memory;
		}

		/// <summary>
		/// Read a float value from an address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <param name="round">Round the value to 2 decimal places</param>
		/// <returns></returns>
		float ReadFloat(const std::wstring& code, const std::wstring& file = L"", bool round = true)
		{
			std::vector<unsigned char> memory(4);

			UIntPtr theCode;
			theCode = GetCode(code, file);
			try
			{
				if (ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(4), std::any::Zero))
				{
					float address = BitConverter::ToSingle(memory, 0);
					float returnValue = static_cast<float>(address);
					if (round)
					{
						returnValue = static_cast<float>(std::round(address * std::pow(10, 2))) / std::pow(10, 2);
					}
					return returnValue;
				}
				else
				{
					return 0;
				}
			}
			catch (...)
			{
				return 0;
			}
		}

		/// <summary>
		/// Read a string value from an address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <param name="length">length of bytes to read (OPTIONAL)</param>
		/// <param name="zeroTerminated">terminate string at null char</param>
		/// <returns></returns>
		std::wstring ReadString(const std::wstring& code, const std::wstring& file = L"", int length = 32, bool zeroTerminated = true)
		{
			std::vector<unsigned char> memoryNormal(length);
			UIntPtr theCode;
			theCode = GetCode(code, file);
			if (ReadProcessMemory(pHandle, theCode, memoryNormal, static_cast<UIntPtr>(length), std::any::Zero))
			{
				return (zeroTerminated) ? StringHelper::split(Encoding::UTF8->GetString(memoryNormal), L'\0')[0] : Encoding::UTF8->GetString(memoryNormal);
			}
			else
			{
				return L"";
			}
		}

		/// <summary>
		/// Read a double value
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <param name="round">Round the value to 2 decimal places</param>
		/// <returns></returns>
		double ReadDouble(const std::wstring& code, const std::wstring& file = L"", bool round = true)
		{
			std::vector<unsigned char> memory(8);

			UIntPtr theCode;
			theCode = GetCode(code, file);
			try
			{
				if (ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(8), std::any::Zero))
				{
					double address = BitConverter::ToDouble(memory, 0);
					double returnValue = static_cast<double>(address);
					if (round)
					{
						returnValue = static_cast<double>(std::round(address * std::pow(10, 2))) / std::pow(10, 2);
					}
					return returnValue;
				}
				else
				{
					return 0;
				}
			}
			catch (...)
			{
				return 0;
			}
		}

		int ReadUIntPtr(UIntPtr code)
		{
			std::vector<unsigned char> memory(4);
			if (ReadProcessMemory(pHandle, code, memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Read an integer from an address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <returns></returns>
		int ReadInt(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			UIntPtr theCode;
			theCode = GetCode(code, file);
			if (ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Read a long value from an address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <returns></returns>
		long long ReadLong(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(16);
			UIntPtr theCode;

			theCode = GetCode(code, file);

			if (ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(16), std::any::Zero))
			{
				return BitConverter::ToInt64(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Read a UInt value from address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		/// <returns></returns>
		unsigned long long ReadUInt(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			UIntPtr theCode;
			theCode = GetCode(code, file);

			if (ReadProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				return BitConverter::ToUInt64(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Reads a 2 byte value from an address and moves the address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="moveQty">Quantity to move.</param>
		/// <param name="file">path and name of ini file (OPTIONAL)</param>
		/// <returns></returns>
		int Read2ByteMove(const std::wstring& code, int moveQty, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			UIntPtr theCode;
			theCode = GetCode(code, file);

			UIntPtr newCode = UIntPtr::Add(theCode, moveQty);

			if (ReadProcessMemory(pHandle, newCode, memory, static_cast<UIntPtr>(2), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Reads an integer value from address and moves the address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="moveQty">Quantity to move.</param>
		/// <param name="file">path and name of ini file (OPTIONAL)</param>
		/// <returns></returns>
		int ReadIntMove(const std::wstring& code, int moveQty, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			UIntPtr theCode;
			theCode = GetCode(code, file);

			UIntPtr newCode = UIntPtr::Add(theCode, moveQty);

			if (ReadProcessMemory(pHandle, newCode, memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Get UInt and move to another address by moveQty. Use in a for loop.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="moveQty">Quantity to move.</param>
		/// <param name="file">path and name of ini file (OPTIONAL)</param>
		/// <returns></returns>
		unsigned long long ReadUIntMove(const std::wstring& code, int moveQty, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(8);
			UIntPtr theCode;
			theCode = GetCode(code, file, 8);

			UIntPtr newCode = UIntPtr::Add(theCode, moveQty);

			if (ReadProcessMemory(pHandle, newCode, memory, static_cast<UIntPtr>(8), std::any::Zero))
			{
				return BitConverter::ToUInt64(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Read a 2 byte value from an address. Returns an integer.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and file name to ini file. (OPTIONAL)</param>
		/// <returns></returns>
		int Read2Byte(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memoryTiny(4);

			UIntPtr theCode;
			theCode = GetCode(code, file);

			if (ReadProcessMemory(pHandle, theCode, memoryTiny, static_cast<UIntPtr>(2), std::any::Zero))
			{
				return BitConverter::ToInt32(memoryTiny, 0);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Read 1 byte from address.
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and file name of ini file. (OPTIONAL)</param>
		/// <returns></returns>
		int ReadByte(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memoryTiny(1);

			UIntPtr theCode = GetCode(code, file);

			if (ReadProcessMemory(pHandle, theCode, memoryTiny, static_cast<UIntPtr>(1), std::any::Zero))
			{
				return memoryTiny[0];
			}

			return 0;
		}

		/// <summary>
		/// Reads a byte from memory and splits it into bits
		/// </summary>
		/// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		/// <param name="file">path and file name of ini file. (OPTIONAL)</param>
		/// <returns>Array of 8 booleans representing each bit of the byte read</returns>
		std::vector<bool> ReadBits(const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> buf(1);

			UIntPtr theCode = GetCode(code, file);

			std::vector<bool> ret(8);

			if (!ReadProcessMemory(pHandle, theCode, buf, static_cast<UIntPtr>(1), std::any::Zero))
			{
				return ret;
			}


			if (!BitConverter::IsLittleEndian)
			{
				throw std::runtime_error("Should be little endian");
			}

			for (auto i = 0; i < 8; i++)
			{
				ret[i] = static_cast<bool>(buf[0] & (1 << i));
			}

			return ret;

		}

		int ReadPByte(UIntPtr address, const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			if (ReadProcessMemory(pHandle, address + LoadIntCode(code, file), memory, static_cast<UIntPtr>(1), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		float ReadPFloat(UIntPtr address, const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			if (ReadProcessMemory(pHandle, address + LoadIntCode(code, file), memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				float spawn = BitConverter::ToSingle(memory, 0);
				return static_cast<float>(std::round(spawn * std::pow(10, 2))) / std::pow(10, 2);
			}
			else
			{
				return 0;
			}
		}

		int ReadPInt(UIntPtr address, const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			if (ReadProcessMemory(pHandle, address + LoadIntCode(code, file), memory, static_cast<UIntPtr>(4), std::any::Zero))
			{
				return BitConverter::ToInt32(memory, 0);
			}
			else
			{
				return 0;
			}
		}

		std::wstring ReadPString(UIntPtr address, const std::wstring& code, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memoryNormal(32);
			if (ReadProcessMemory(pHandle, address + LoadIntCode(code, file), memoryNormal, static_cast<UIntPtr>(32), std::any::Zero))
			{
				return CutString(System::Text::Encoding::ASCII->GetString(memoryNormal));
			}
			else
			{
				return L"";
			}
		}
		//		#endregion

		//		#region writeMemory
				///<summary>
				///Write to memory address. See https://github.com/erfg12/memory.dll/wiki/writeMemory() for more information.
				///</summary>
				///<param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
				///<param name="type">byte, 2bytes, bytes, float, int, string, double or long.</param>
				///<param name="write">value to write to address.</param>
				///<param name="file">path and name of .ini file (OPTIONAL)</param>
				///<param name="stringEncoding">System.Text.Encoding.UTF8 (DEFAULT). Other options: ascii, unicode, utf32, utf7</param>
		bool WriteMemory(const std::wstring& code, const std::wstring& type, const std::wstring& write, const std::wstring& file = L"", System::Text::Encoding* stringEncoding = nullptr)
		{
			std::vector<unsigned char> memory(4);
			int size = 4;

			UIntPtr theCode;
			theCode = GetCode(code, file);

			if (StringHelper::toLower(type) == L"float")
			{
				memory = BitConverter::GetBytes(std::stof(write));
				size = 4;
			}
			else if (StringHelper::toLower(type) == L"int")
			{
				memory = BitConverter::GetBytes(std::stoi(write));
				size = 4;
			}
			else if (StringHelper::toLower(type) == L"byte")
			{
				memory = std::vector<unsigned char>(1);
				memory[0] = Convert::ToByte(write, 16);
				size = 1;
			}
			else if (StringHelper::toLower(type) == L"2bytes")
			{
				memory = std::vector<unsigned char>(2);
				memory[0] = static_cast<unsigned char>(std::stoi(write) % 256);
				memory[1] = static_cast<unsigned char>(std::stoi(write) / 256);
				size = 2;
			}
			else if (StringHelper::toLower(type) == L"bytes")
			{
				if (write.find(L",") != std::wstring::npos || write.find(L" ") != std::wstring::npos) //check if it's a proper array
				{
					std::vector<std::wstring> stringBytes;
					if (write.find(L",") != std::wstring::npos)
					{
						stringBytes = StringHelper::split(write, L',');
					}
					else
					{
						stringBytes = StringHelper::split(write, L' ');
					}
					//Debug.WriteLine("write:" + write + " stringBytes:" + stringBytes);

					int c = stringBytes.Count();
					memory = std::vector<unsigned char>(c);
					for (int i = 0; i < c; i++)
					{
						memory[i] = Convert::ToByte(stringBytes[i], 16);
					}
					size = stringBytes.Count();
				}
				else //wasnt array, only 1 byte
				{
					memory = std::vector<unsigned char>(1);
					memory[0] = Convert::ToByte(write, 16);
					size = 1;
				}
			}
			else if (StringHelper::toLower(type) == L"double")
			{
				memory = BitConverter::GetBytes(std::stod(write));
				size = 8;
			}
			else if (StringHelper::toLower(type) == L"long")
			{
				memory = BitConverter::GetBytes(std::stoll(write));
				size = 8;
			}
			else if (StringHelper::toLower(type) == L"string")
			{
				if (stringEncoding == nullptr)
				{
					memory = System::Text::Encoding::UTF8->GetBytes(write);
				}
				else
				{
					memory = stringEncoding->GetBytes(write);
				}
				size = memory.size();
			}
			//Debug.Write("DEBUG: Writing bytes [TYPE:" + type + " ADDR:" + theCode + "] " + String.Join(",", memory) + Environment.NewLine);
			return WriteProcessMemory(pHandle, theCode, memory, static_cast<UIntPtr>(size), std::any::Zero);
		}

		/// <summary>
		/// Write to address and move by moveQty. Good for byte arrays. See https://github.com/erfg12/memory.dll/wiki/Writing-a-Byte-Array for more information.
		/// </summary>
		///<param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
		///<param name="type">byte, bytes, float, int, string or long.</param>
		/// <param name="write">byte to write</param>
		/// <param name="moveQty">quantity to move</param>
		/// <param name="file">path and name of .ini file (OPTIONAL)</param>
		/// <returns></returns>
		bool WriteMove(const std::wstring& code, const std::wstring& type, const std::wstring& write, int moveQty, const std::wstring& file = L"")
		{
			std::vector<unsigned char> memory(4);
			int size = 4;

			UIntPtr theCode;
			theCode = GetCode(code, file);

			if (type == L"float")
			{
				memory = std::vector<unsigned char>(write.length());
				memory = BitConverter::GetBytes(std::stof(write));
				size = write.length();
			}
			else if (type == L"int")
			{
				memory = BitConverter::GetBytes(std::stoi(write));
				size = 4;
			}
			else if (type == L"double")
			{
				memory = BitConverter::GetBytes(std::stod(write));
				size = 8;
			}
			else if (type == L"long")
			{
				memory = BitConverter::GetBytes(std::stoll(write));
				size = 8;
			}
			else if (type == L"byte")
			{
				memory = std::vector<unsigned char>(1);
				memory[0] = Convert::ToByte(write, 16);
				size = 1;
			}
			else if (type == L"string")
			{
				memory = std::vector<unsigned char>(write.length());
				memory = System::Text::Encoding::UTF8->GetBytes(write);
				size = write.length();
			}

			UIntPtr newCode = UIntPtr::Add(theCode, moveQty);

			Debug::Write(L"DEBUG: Writing bytes [TYPE:" + type + L" ADDR:[O]" + theCode + L" [N]" + newCode + L" MQTY:" + std::to_wstring(moveQty) + L"] " + std::wstring::Join(L",", memory) + L"\r\n");
			delay(1000);
			return WriteProcessMemory(pHandle, newCode, memory, static_cast<UIntPtr>(size), std::any::Zero);
		}

		/// <summary>
		/// Write byte array to addresses.
		/// </summary>
		/// <param name="code">address to write to</param>
		/// <param name="write">byte array to write</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		void WriteBytes(const std::wstring& code, std::vector<unsigned char>& write, const std::wstring& file = L"")
		{
			UIntPtr theCode;
			theCode = GetCode(code, file);
			WriteProcessMemory(pHandle, theCode, write, static_cast<UIntPtr>(write.size()), std::any::Zero);
		}

		/// <summary>
		/// Takes an array of 8 booleans and writes to a single byte
		/// </summary>
		/// <param name="code">address to write to</param>
		/// <param name="bits">Array of 8 booleans to write</param>
		/// <param name="file">path and name of ini file. (OPTIONAL)</param>
		void WriteBits(const std::wstring& code, std::vector<bool>& bits, const std::wstring& file = L"")
		{
			if (bits.size() != 8)
			{
				//C# TO C++ CONVERTER TODO TASK: This exception's constructor requires only one argument:
				//ORIGINAL LINE: throw new ArgumentException("Not enough bits for a whole byte", nameof(bits));
				throw std::invalid_argument("Not enough bits for a whole byte");
			}

			std::vector<unsigned char> buf(1);

			UIntPtr theCode = GetCode(code, file);

			for (auto i = 0; i < 8; i++)
			{
				if (bits[i])
				{
					buf[0] |= static_cast<unsigned char>(1 << i);
				}
			}

			WriteProcessMemory(pHandle, theCode, buf, static_cast<UIntPtr>(1), std::any::Zero);
		}

		/// <summary>
		/// Write byte array to address
		/// </summary>
		/// <param name="address">Address to write to</param>
		/// <param name="write">Byte array to write to</param>
		void WriteBytes(UIntPtr address, std::vector<unsigned char>& write)
		{
			std::any bytesRead;
			WriteProcessMemory(pHandle, address, write, static_cast<UIntPtr>(write.size()), bytesRead);
		}

		//		#endregion

				/// <summary>
				/// Convert code from string to real address. If path is not blank, will pull from ini file.
				/// </summary>
				/// <param name="name">label in ini file or code</param>
				/// <param name="path">path to ini file (OPTIONAL)</param>
				/// <param name="size">size of address (default is 8)</param>
				/// <returns></returns>
		UIntPtr GetCode(const std::wstring& name, const std::wstring& path = L"", int size = 8)
		{
			std::wstring theCode = L"";
			if (getIs64Bit())
			{
				//Debug.WriteLine("Changing to 64bit code...");
				if (size == 8)
				{
					size = 16; //change to 64bit
				}
				return Get64BitCode(name, path, size); //jump over to 64bit code grab
			}

			if (path != L"")
			{
				theCode = LoadCode(name, path);
			}
			else
			{
				theCode = name;
			}

			if (theCode == L"")
			{
				//Debug.WriteLine("ERROR: LoadCode returned blank. NAME:" + name + " PATH:" + path);
				return UIntPtr::Zero;
			}
			else
			{
				//Debug.WriteLine("Found code=" + theCode + " NAME:" + name + " PATH:" + path);
			}

			// remove spaces
			if (theCode.find(L" ") != std::wstring::npos)
			{
				StringHelper::replace(theCode, L" ", L"");
			}

			if (!theCode.find(L"+") != std::wstring::npos && !theCode.find(L",") != std::wstring::npos)
			{
				return UIntPtr(Convert::ToUInt32(theCode, 16));
			}

			std::wstring newOffsets = theCode;

			if (theCode.find(L"+") != std::wstring::npos)
			{
				newOffsets = theCode.substr((int)theCode.find(L'+') + 1);
			}

			std::vector<unsigned char> memoryAddress(size);

			if (newOffsets.find(L',') != std::wstring::npos)
			{
				std::vector<int> offsetsList;

				std::vector<std::wstring> newerOffsets = StringHelper::split(newOffsets, L',');
				for (auto oldOffsets : newerOffsets)
				{
					std::wstring test = oldOffsets;
					if (oldOffsets.Contains(L"0x"))
					{
						test = oldOffsets.Replace(L"0x", L"");
					}
					int preParse = 0;
					if (!oldOffsets.Contains(L"-"))
					{
						preParse = std::stoi(test, NumberStyles::AllowHexSpecifier);
					}
					else
					{
						test = StringHelper::replace(test, L"-", L"");
						preParse = std::stoi(test, NumberStyles::AllowHexSpecifier);
						preParse = preParse * -1;
					}
					offsetsList.push_back(preParse);
				}
				std::vector<int> offsets = offsetsList.ToArray();

				if (theCode.find(L"base") != std::wstring::npos || theCode.find(L"main") != std::wstring::npos)
				{
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(static_cast<int>(mainModule->BaseAddress) + offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}
				else if (!theCode.find(L"base") != std::wstring::npos && !theCode.find(L"main") != std::wstring::npos && theCode.find(L"+") != std::wstring::npos)
				{
					std::vector<std::wstring> moduleName = StringHelper::split(theCode, L'+');
					std::any altModule = std::any::Zero;
					if (!StringHelper::toLower(moduleName[0]).find(L".dll") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".exe") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".bin") != std::wstring::npos)
					{
						std::wstring theAddr = moduleName[0];
						if (theAddr.find(L"0x") != std::wstring::npos)
						{
							theAddr = StringHelper::replace(theAddr, L"0x", L"");
						}
						altModule = static_cast<std::any>(std::stoi(theAddr, NumberStyles::HexNumber));
					}
					else
					{
						try
						{
							altModule = modules[moduleName[0]];
						}
						catch (...)
						{
							Debug::WriteLine(L"Module " + moduleName[0] + L" was not found in module list!");
							Debug::WriteLine(L"Modules: " + std::wstring::Join(L",", modules));
						}
					}
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(static_cast<int>(altModule) + offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}
				else
				{
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}

				unsigned int num1 = BitConverter::ToUInt32(memoryAddress, 0); //ToUInt64 causes arithmetic overflow.

				UIntPtr base1 = static_cast<UIntPtr>(0);

				for (int i = 1; i < offsets.size(); i++)
				{
					base1 = UIntPtr(static_cast<unsigned int>(num1 + offsets[i]));
					ReadProcessMemory(pHandle, base1, memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
					num1 = BitConverter::ToUInt32(memoryAddress, 0); //ToUInt64 causes arithmetic overflow.
				}
				return base1;
			}
			else // no offsets
			{
				int trueCode = Convert::ToInt32(newOffsets, 16);
				std::any altModule = std::any::Zero;
				//Debug.WriteLine("newOffsets=" + newOffsets);
				if (StringHelper::toLower(theCode).find(L"base") != std::wstring::npos || StringHelper::toLower(theCode).find(L"main") != std::wstring::npos)
				{
					altModule = mainModule->BaseAddress;
				}
				else if (!StringHelper::toLower(theCode).find(L"base") != std::wstring::npos && !StringHelper::toLower(theCode).find(L"main") != std::wstring::npos && theCode.find(L"+") != std::wstring::npos)
				{
					std::vector<std::wstring> moduleName = StringHelper::split(theCode, L'+');
					if (!StringHelper::toLower(moduleName[0]).find(L".dll") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".exe") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".bin") != std::wstring::npos)
					{
						std::wstring theAddr = moduleName[0];
						if (theAddr.find(L"0x") != std::wstring::npos)
						{
							theAddr = StringHelper::replace(theAddr, L"0x", L"");
						}
						altModule = static_cast<std::any>(std::stoi(theAddr, NumberStyles::HexNumber));
					}
					else
					{
						try
						{
							altModule = modules[moduleName[0]];
						}
						catch (...)
						{
							Debug::WriteLine(L"Module " + moduleName[0] + L" was not found in module list!");
							Debug::WriteLine(L"Modules: " + std::wstring::Join(L",", modules));
						}
					}
				}
				else
				{
					altModule = modules[StringHelper::split(theCode, L'+')[0]];
				}
				return static_cast<UIntPtr>(static_cast<int>(altModule) + trueCode);
			}
		}

		/// <summary>
		/// Convert code from string to real address. If path is not blank, will pull from ini file.
		/// </summary>
		/// <param name="name">label in ini file OR code</param>
		/// <param name="path">path to ini file (OPTIONAL)</param>
		/// <param name="size">size of address (default is 16)</param>
		/// <returns></returns>
		UIntPtr Get64BitCode(const std::wstring& name, const std::wstring& path = L"", int size = 16)
		{
			std::wstring theCode = L"";
			if (path != L"")
			{
				theCode = LoadCode(name, path);
			}
			else
			{
				theCode = name;
			}

			if (theCode == L"")
			{
				return UIntPtr::Zero;
			}

			// remove spaces
			if (theCode.find(L" ") != std::wstring::npos)
			{
				StringHelper::replace(theCode, L" ", L"");
			}

			std::wstring newOffsets = theCode;
			if (theCode.find(L"+") != std::wstring::npos)
			{
				newOffsets = theCode.substr((int)theCode.find(L'+') + 1);
			}

			std::vector<unsigned char> memoryAddress(size);

			if (!theCode.find(L"+") != std::wstring::npos && !theCode.find(L",") != std::wstring::npos)
			{
				return UIntPtr(Convert::ToUInt64(theCode, 16));
			}

			if (newOffsets.find(L',') != std::wstring::npos)
			{
				std::vector<long long> offsetsList;

				std::vector<std::wstring> newerOffsets = StringHelper::split(newOffsets, L',');
				for (auto oldOffsets : newerOffsets)
				{
					std::wstring test = oldOffsets;
					if (oldOffsets.Contains(L"0x"))
					{
						test = oldOffsets.Replace(L"0x", L"");
					}
					long long preParse = 0;
					if (!oldOffsets.Contains(L"-"))
					{
						preParse = std::stoll(test, NumberStyles::AllowHexSpecifier);
					}
					else
					{
						test = StringHelper::replace(test, L"-", L"");
						preParse = std::stoll(test, NumberStyles::AllowHexSpecifier);
						preParse = preParse * -1;
					}
					offsetsList.push_back(preParse);
				}
				std::vector<long long> offsets = offsetsList.ToArray();

				if (theCode.find(L"base") != std::wstring::npos || theCode.find(L"main") != std::wstring::npos)
				{
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(static_cast<long long>(mainModule->BaseAddress) + offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}
				else if (!theCode.find(L"base") != std::wstring::npos && !theCode.find(L"main") != std::wstring::npos && theCode.find(L"+") != std::wstring::npos)
				{
					std::vector<std::wstring> moduleName = StringHelper::split(theCode, L'+');
					std::any altModule = std::any::Zero;
					if (!StringHelper::toLower(moduleName[0]).find(L".dll") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".exe") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".bin") != std::wstring::npos)
					{
						altModule = static_cast<std::any>(std::stoll(moduleName[0], System::Globalization::NumberStyles::HexNumber));
					}
					else
					{
						try
						{
							altModule = modules[moduleName[0]];
						}
						catch (...)
						{
							Debug::WriteLine(L"Module " + moduleName[0] + L" was not found in module list!");
							Debug::WriteLine(L"Modules: " + std::wstring::Join(L",", modules));
						}
					}
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(static_cast<long long>(altModule) + offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}
				else // no offsets
				{
					ReadProcessMemory(pHandle, static_cast<UIntPtr>(offsets[0]), memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
				}

				long long num1 = BitConverter::ToInt64(memoryAddress, 0);

				UIntPtr base1 = static_cast<UIntPtr>(0);

				for (int i = 1; i < offsets.size(); i++)
				{
					base1 = UIntPtr(static_cast<unsigned long long>(num1 + offsets[i]));
					ReadProcessMemory(pHandle, base1, memoryAddress, static_cast<UIntPtr>(size), std::any::Zero);
					num1 = BitConverter::ToInt64(memoryAddress, 0);
				}
				return base1;
			}
			else
			{
				long long trueCode = Convert::ToInt64(newOffsets, 16);
				std::any altModule = std::any::Zero;
				if (theCode.find(L"base") != std::wstring::npos || theCode.find(L"main") != std::wstring::npos)
				{
					altModule = mainModule->BaseAddress;
				}
				else if (!theCode.find(L"base") != std::wstring::npos && !theCode.find(L"main") != std::wstring::npos && theCode.find(L"+") != std::wstring::npos)
				{
					std::vector<std::wstring> moduleName = StringHelper::split(theCode, L'+');
					if (!StringHelper::toLower(moduleName[0]).find(L".dll") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".exe") != std::wstring::npos && !StringHelper::toLower(moduleName[0]).find(L".bin") != std::wstring::npos)
					{
						std::wstring theAddr = moduleName[0];
						if (theAddr.find(L"0x") != std::wstring::npos)
						{
							theAddr = StringHelper::replace(theAddr, L"0x", L"");
						}
						altModule = static_cast<std::any>(std::stoll(theAddr, NumberStyles::HexNumber));
					}
					else
					{
						try
						{
							altModule = modules[moduleName[0]];
						}
						catch (...)
						{
							Debug::WriteLine(L"Module " + moduleName[0] + L" was not found in module list!");
							Debug::WriteLine(L"Modules: " + std::wstring::Join(L",", modules));
						}
					}
				}
				else
				{
					altModule = modules[StringHelper::split(theCode, L'+')[0]];
				}
				return static_cast<UIntPtr>(static_cast<long long>(altModule) + trueCode);
			}
		}

		/// <summary>
		/// Close the process when finished.
		/// </summary>
		void CloseProcess()
		{
			if (pHandle == nullptr)
			{
				return;
			}

			CloseHandle(pHandle);
			theProc = nullptr;
		}

		/// <summary>
		/// Inject a DLL file.
		/// </summary>
		/// <param name="strDllName">path and name of DLL file.</param>
		void InjectDll(const std::wstring& strDllName)
		{
			std::any bytesout;

			for (auto pm : *theProc->Modules)
			{
				if (pm->ModuleName.StartsWith(L"inject", StringComparison::InvariantCultureIgnoreCase))
				{
					return;
				}
			}

			if (!theProc->Responding)
			{
				return;
			}

			int lenWrite = strDllName.length() + 1;
			UIntPtr allocMem = VirtualAllocEx(pHandle, static_cast<UIntPtr>(nullptr), static_cast<unsigned int>(lenWrite), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

			WriteProcessMemory(pHandle, allocMem, strDllName, static_cast<UIntPtr>(lenWrite), bytesout);
			UIntPtr injector = GetProcAddress(GetModuleHandle(L"kernel32.dll"), L"LoadLibraryA");

			if (injector == nullptr)
			{
				return;
			}

			std::any hThread = CreateRemoteThread(pHandle, static_cast<std::any>(nullptr), 0, injector, allocMem, 0, bytesout);
			if (hThread == nullptr)
			{
				return;
			}

			int Result = WaitForSingleObject(hThread, 10 * 1000);
			if (Result == 0x00000080LL || Result == 0x00000102LL)
			{
				if (hThread != nullptr)
				{
					CloseHandle(hThread);
				}
				return;
			}
			VirtualFreeEx(pHandle, allocMem, static_cast<UIntPtr>(0), 0x8000);

			if (hThread != nullptr)
			{
				CloseHandle(hThread);
			}

			return;
		}

#if defined(WINXP)
#else
		/// <summary>
		/// Creates a code cave to write custom opcodes in target process
		/// </summary>
		/// <param name="code">Address to create the trampoline</param>
		/// <param name="newBytes">The opcodes to write in the code cave</param>
		/// <param name="replaceCount">The number of bytes being replaced</param>
		/// <param name="allocationSize">size of the allocated region</param>
		/// <param name="file">ini file to look in</param>
		/// <remarks>Please ensure that you use the proper replaceCount
		/// if you replace halfway in an instruction you may cause bad things</remarks>
		/// <returns>UIntPtr to created code cave for use for later deallocation</returns>
		UIntPtr CreateCodeCave(const std::wstring& code, std::vector<unsigned char>& newBytes, int replaceCount, int size = 0x1000, const std::wstring& file = L"")
		{
			if (replaceCount < 5)
			{
				return UIntPtr::Zero; // returning UIntPtr.Zero instead of throwing an exception
			}
			// to better match existing code

			UIntPtr theCode;
			theCode = GetCode(code, file);
			UIntPtr address = theCode;

			// if x64 we need to try to allocate near the address so we dont run into the +-2GB limit of the 0xE9 jmp

			UIntPtr caveAddress = UIntPtr::Zero;
			UIntPtr prefered = address;

			for (auto i = 0; i < 10 && caveAddress == UIntPtr::Zero; i++)
			{
				caveAddress = VirtualAllocEx(pHandle, FindFreeBlockForRegion(prefered, static_cast<unsigned int>(size)), static_cast<unsigned int>(size), MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

				if (caveAddress == UIntPtr::Zero)
				{
					prefered = UIntPtr::Add(prefered, 0x10000);
				}
			}

			// Failed to allocate memory around the address we wanted let windows handle it and hope for the best?
			if (caveAddress == UIntPtr::Zero)
			{
				caveAddress = VirtualAllocEx(pHandle, UIntPtr::Zero, static_cast<unsigned int>(size), MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
			}

			int nopsNeeded = replaceCount > 5 ? replaceCount - 5 : 0;

			// (to - from - 5)
			int offset = static_cast<int>(static_cast<long long>(caveAddress) - static_cast<long long>(address) - 5);

			std::vector<unsigned char> jmpBytes(5 + nopsNeeded);
			jmpBytes[0] = 0xE9;
			BitConverter::GetBytes(offset).CopyTo(jmpBytes, 1);

			for (auto i = 5; i < jmpBytes.size(); i++)
			{
				jmpBytes[i] = 0x90;
			}
			WriteBytes(address, jmpBytes);

			std::vector<unsigned char> caveBytes(5 + newBytes.size());
			offset = static_cast<int>((static_cast<long long>(address) + jmpBytes.size()) - (static_cast<long long>(caveAddress) + newBytes.size()) - 5);

			newBytes.CopyTo(caveBytes, 0);
			caveBytes[newBytes.size()] = 0xE9;
			BitConverter::GetBytes(offset).CopyTo(caveBytes, newBytes.size() + 1);

			WriteBytes(caveAddress, caveBytes);

			return caveAddress;
		}

	private:
		UIntPtr FindFreeBlockForRegion(UIntPtr baseAddress, unsigned int size)
		{
			UIntPtr minAddress = UIntPtr::Subtract(baseAddress, 0x70000000);
			UIntPtr maxAddress = UIntPtr::Add(baseAddress, 0x70000000);

			UIntPtr ret = UIntPtr::Zero;
			UIntPtr tmpAddress = UIntPtr::Zero;

			SYSTEM_INFO si;
			GetSystemInfo(si);

			if (getIs64Bit())
			{
				if (static_cast<long long>(minAddress) > static_cast<long long>(si::maximumApplicationAddress) || static_cast<long long>(minAddress) < static_cast<long long>(si::minimumApplicationAddress))
				{
					minAddress = si::minimumApplicationAddress;
				}

				if (static_cast<long long>(maxAddress) < static_cast<long long>(si::minimumApplicationAddress) || static_cast<long long>(maxAddress) > static_cast<long long>(si::maximumApplicationAddress))
				{
					maxAddress = si::maximumApplicationAddress;
				}
			}
			else
			{
				minAddress = si::minimumApplicationAddress;
				maxAddress = si::maximumApplicationAddress;
			}

			MEMORY_BASIC_INFORMATION mbi;

			UIntPtr current = minAddress;
			UIntPtr previous = current;

			while (VirtualQueryEx(pHandle, current, mbi).ToUInt64() != 0)
			{
				if (static_cast<long long>(mbi.BaseAddress) > static_cast<long long>(maxAddress))
				{
					return UIntPtr::Zero; // No memory found, let windows handle
				}

				if (mbi.State == MEM_FREE && mbi.RegionSize > size)
				{
					if (static_cast<long long>(mbi.BaseAddress) % si::allocationGranularity > 0)
					{
						// The whole size can not be used
						tmpAddress = mbi.BaseAddress;
						int offset = static_cast<int>(si::allocationGranularity - (static_cast<long long>(tmpAddress) % si::allocationGranularity));

						// Check if there is enough left
						if ((mbi.RegionSize - offset) >= size)
						{
							// yup there is enough
							tmpAddress = UIntPtr::Add(tmpAddress, offset);

							if (static_cast<long long>(tmpAddress) < static_cast<long long>(baseAddress))
							{
								tmpAddress = UIntPtr::Add(tmpAddress, static_cast<int>(mbi.RegionSize - offset - size));

								if (static_cast<long long>(tmpAddress) > static_cast<long long>(baseAddress))
								{
									tmpAddress = baseAddress;
								}

								// decrease tmpAddress until its alligned properly
								tmpAddress = UIntPtr::Subtract(tmpAddress, static_cast<int>(static_cast<long long>(tmpAddress) % si::allocationGranularity));
							}

							// if the difference is closer then use that
							if (std::abs(static_cast<long long>(tmpAddress) - static_cast<long long>(baseAddress)) < std::abs(static_cast<long long>(ret) - static_cast<long long>(baseAddress)))
							{
								ret = tmpAddress;
							}
						}
					}
					else
					{
						tmpAddress = mbi.BaseAddress;

						if (static_cast<long long>(tmpAddress) < static_cast<long long>(baseAddress)) // try to get it the cloest possible
																  // (so to the end of the region - size and
																  // aligned by system allocation granularity)
						{
							tmpAddress = UIntPtr::Add(tmpAddress, static_cast<int>(mbi.RegionSize - size));

							if (static_cast<long long>(tmpAddress) > static_cast<long long>(baseAddress))
							{
								tmpAddress = baseAddress;
							}

							// decrease until aligned properly
							tmpAddress = UIntPtr::Subtract(tmpAddress, static_cast<int>(static_cast<long long>(tmpAddress) % si::allocationGranularity));
						}

						if (std::abs(static_cast<long long>(tmpAddress) - static_cast<long long>(baseAddress)) < std::abs(static_cast<long long>(ret) - static_cast<long long>(baseAddress)))
						{
							ret = tmpAddress;
						}
					}
				}

				if (mbi.RegionSize % si::allocationGranularity > 0)
				{
					mbi.RegionSize += si::allocationGranularity - (mbi.RegionSize % si::allocationGranularity);
				}

				previous = current;
				current = UIntPtr::Add(mbi.BaseAddress, static_cast<int>(mbi.RegionSize));

				if (static_cast<long long>(current) > static_cast<long long>(maxAddress))
				{
					return ret;
				}

				if (static_cast<long long>(previous) > static_cast<long long>(current))
				{
					return ret; // Overflow
				}
			}

			return ret;
		}
#endif

	public:
		//C# TO C++ CONVERTER NOTE: The following .NET attribute has no direct equivalent in C++:
		//ORIGINAL LINE: [Flags] public enum ThreadAccess : int
		enum class ThreadAccess : int
		{
			TERMINATE = (0x0001),
			SUSPEND_RESUME = (0x0002),
			GET_CONTEXT = (0x0008),
			SET_CONTEXT = (0x0010),
			SET_INFORMATION = (0x0020),
			QUERY_INFORMATION = (0x0040),
			SET_THREAD_TOKEN = (0x0080),
			IMPERSONATE = (0x0100),
			DIRECT_IMPERSONATION = (0x0200)
		};

	public:
		static void SuspendProcess(int pid)
		{
			auto process = System::Diagnostics::Process::GetProcessById(pid);

			if (process->ProcessName == L"")
			{
				return;
			}

			for (auto pT : *process->Threads)
			{
				std::any pOpenThread = OpenThread(ThreadAccess::SUSPEND_RESUME, false, static_cast<unsigned int>(pT->Id));
				if (pOpenThread == std::any::Zero)
				{
					continue;
				}

				SuspendThread(pOpenThread);
				CloseHandle(pOpenThread);
			}
		}

		static void ResumeProcess(int pid)
		{
			auto process = System::Diagnostics::Process::GetProcessById(pid);
			if (process->ProcessName == L"")
			{
				return;
			}

			for (auto pT : *process->Threads)
			{
				std::any pOpenThread = OpenThread(ThreadAccess::SUSPEND_RESUME, false, static_cast<unsigned int>(pT->Id));
				if (pOpenThread == std::any::Zero)
				{
					continue;
				}

				auto suspendCount = 0;
				do
				{
					suspendCount = ResumeThread(pOpenThread);
				} while (suspendCount > 0);
				CloseHandle(pOpenThread);
			}
		}

#if defined(WINXP)
#else
	private:
		//C# TO C++ CONVERTER TODO TASK: There is no equivalent in C++ to the 'async' keyword:
		//ORIGINAL LINE: async Task PutTaskDelay(int delay)
		Task* PutTaskDelay(int delay)
		{
			//C# TO C++ CONVERTER TODO TASK: There is no equivalent to 'await' in C++:
			await Task::Delay(delay);
		}
#endif

		void AppendAllBytes(const std::wstring& path, std::vector<unsigned char>& bytes)
		{
			//C# TO C++ CONVERTER NOTE: The following 'using' block is replaced by its C++ equivalent:
			//ORIGINAL LINE: using (var stream = new FileStream(path, FileMode.Append))
			{
				auto stream = FileStream(path, FileMode::Append);
				stream.Write(bytes, 0, bytes.size());
			}
		}

	public:
		std::vector<unsigned char> FileToBytes(const std::wstring& path, bool dontDelete = false)
		{
			std::vector<unsigned char> newArray = File::ReadAllBytes(path);
			if (!dontDelete)
			{
				File::Delete(path);
			}
			return newArray;
		}

		std::wstring MSize()
		{
			if (getIs64Bit())
			{
				return (L"x16");
			}
			else
			{
				return (L"x8");
			}
		}

		/// <summary>
		/// Convert a byte array to hex values in a string.
		/// </summary>
		/// <param name="ba">your byte array to convert</param>
		/// <returns></returns>
		static std::wstring ByteArrayToHexString(std::vector<unsigned char>& ba)
		{
			StringBuilder* hex = new StringBuilder(ba.size() * 2);
			int i = 1;
			for (auto b : ba)
			{
				if (i == 16)
				{
					hex->AppendFormat(L"{0:x2}{1}", b, L"\r\n");
					i = 0;
				}
				else
				{
					hex->AppendFormat(L"{0:x2} ", b);
				}
				i++;
			}

			delete hex;
			return hex->toString()->ToUpper();
		}

		static std::wstring ByteArrayToString(std::vector<unsigned char>& ba)
		{
			StringBuilder* hex = new StringBuilder(ba.size() * 2);
			for (auto b : ba)
			{
				hex->AppendFormat(L"{0:x2} ", b);
			}

			delete hex;
			return hex->toString();
		}

#if defined(WINXP)
#else

	public:
		class SYSTEM_INFO
		{
		public:
			unsigned short processorArchitecture = 0;
		private:
			unsigned short reserved = 0;
		public:
			unsigned int pageSize = 0;
			UIntPtr minimumApplicationAddress;
			UIntPtr maximumApplicationAddress;
			std::any activeProcessorMask;
			unsigned int numberOfProcessors = 0;
			unsigned int processorType = 0;
			unsigned int allocationGranularity = 0;
			unsigned short processorLevel = 0;
			unsigned short processorRevision = 0;
		};

	public:
		class MEMORY_BASIC_INFORMATION32
		{
		public:
			UIntPtr BaseAddress;
			UIntPtr AllocationBase;
			unsigned int AllocationProtect = 0;
			unsigned int RegionSize = 0;
			unsigned int State = 0;
			unsigned int Protect = 0;
			unsigned int Type = 0;
		};

	public:
		class MEMORY_BASIC_INFORMATION64
		{
		public:
			UIntPtr BaseAddress;
			UIntPtr AllocationBase;
			unsigned int AllocationProtect = 0;
			unsigned int __alignment1 = 0;
			unsigned long long RegionSize = 0;
			unsigned int State = 0;
			unsigned int Protect = 0;
			unsigned int Type = 0;
			unsigned int __alignment2 = 0;
		};

	public:
		class MEMORY_BASIC_INFORMATION
		{
		public:
			UIntPtr BaseAddress;
			UIntPtr AllocationBase;
			unsigned int AllocationProtect = 0;
			long long RegionSize = 0;
			unsigned int State = 0;
			unsigned int Protect = 0;
			unsigned int Type = 0;
		};

	public:
		unsigned long long GetMinAddress()
		{
			SYSTEM_INFO SI;
			GetSystemInfo(SI);
			return static_cast<unsigned long long>(SI.minimumApplicationAddress);
		}

		/// <summary>
		/// Dump memory page by page to a dump.dmp file. Can be used with Cheat Engine.
		/// </summary>
		bool DumpMemory(const std::wstring& file = L"dump.dmp")
		{
			//C# TO C++ CONVERTER TODO TASK: There is no C++ equivalent to 'ToString':
			Debug::Write(L"[DEBUG] memory dump starting... (" + DateTime::Now.ToString(L"h:mm:ss tt") + L")" + L"\r\n");
			SYSTEM_INFO sys_info = SYSTEM_INFO();
			GetSystemInfo(sys_info);

			UIntPtr proc_min_address = sys_info.minimumApplicationAddress;
			UIntPtr proc_max_address = sys_info.maximumApplicationAddress;

			// saving the values as long ints so I won't have to do a lot of casts later
			long long proc_min_address_l = static_cast<long long>(proc_min_address); //(Int64)procs.MainModule.BaseAddress;
			long long proc_max_address_l = static_cast<long long>(theProc->VirtualMemorySize64) + proc_min_address_l;

			//int arrLength = 0;
			if (FileSystem::fileExists(file))
			{
				File::Delete(file);
			}


			MEMORY_BASIC_INFORMATION memInfo = MEMORY_BASIC_INFORMATION();
			while (proc_min_address_l < proc_max_address_l)
			{
				VirtualQueryEx(pHandle, proc_min_address, memInfo);
				std::vector<unsigned char> buffer(static_cast<long long>(memInfo.RegionSize));
				UIntPtr test = static_cast<UIntPtr>(static_cast<long long>(memInfo.RegionSize));
				UIntPtr test2 = static_cast<UIntPtr>(static_cast<long long>(memInfo.BaseAddress));

				ReadProcessMemory(pHandle, test2, buffer, test, std::any::Zero);

				AppendAllBytes(file, buffer); //due to memory limits, we have to dump it then store it in an array.
				//arrLength += buffer.Length;

				proc_min_address_l += static_cast<long long>(memInfo.RegionSize);
				proc_min_address = UIntPtr(static_cast<unsigned long long>(proc_min_address_l));
			}


			//C# TO C++ CONVERTER TODO TASK: There is no C++ equivalent to 'ToString':
			Debug::Write(L"[DEBUG] memory dump completed. Saving dump file to " + file + L". (" + DateTime::Now.ToString(L"h:mm:ss tt") + L")" + L"\r\n");
			return true;
		}

		/// <summary>
		/// Array of byte scan.
		/// </summary>
		/// <param name="search">array of bytes to search for, OR your ini code label.</param>
		/// <param name="writable">Include writable addresses in scan</param>
		/// <param name="executable">Include executable addresses in scan</param>
		/// <param name="file">ini file (OPTIONAL)</param>
		/// <returns>IEnumerable of all addresses found.</returns>
		Task<std::vector<long long>>* AoBScan(const std::wstring& search, bool writable = false, bool executable = true, const std::wstring& file = L"")
		{
			return AoBScan(0, std::numeric_limits<long long>::max(), search, writable, executable, file);
		}

		/// <summary>
		/// Array of byte scan.
		/// </summary>
		/// <param name="search">array of bytes to search for, OR your ini code label.</param>
		/// <param name="readable">Include readable addresses in scan</param>
		/// <param name="writable">Include writable addresses in scan</param>
		/// <param name="executable">Include executable addresses in scan</param>
		/// <param name="file">ini file (OPTIONAL)</param>
		/// <returns>IEnumerable of all addresses found.</returns>
		Task<std::vector<long long>>* AoBScan(const std::wstring& search, bool readable, bool writable, bool executable, const std::wstring& file = L"")
		{
			return AoBScan(0, std::numeric_limits<long long>::max(), search, readable, writable, executable, file);
		}


		/// <summary>
		/// Array of Byte scan.
		/// </summary>
		/// <param name="start">Your starting address.</param>
		/// <param name="end">ending address</param>
		/// <param name="search">array of bytes to search for, OR your ini code label.</param>
		/// <param name="file">ini file (OPTIONAL)</param>
		/// <param name="writable">Include writable addresses in scan</param>
		/// <param name="executable">Include executable addresses in scan</param>
		/// <returns>IEnumerable of all addresses found.</returns>
		Task<std::vector<long long>>* AoBScan(long long start, long long end, const std::wstring& search, bool writable = false, bool executable = true, const std::wstring& file = L"")
		{
			// Not including read only memory was scan behavior prior.
			return AoBScan(start, end, search, false, writable, executable, file);
		}

		/// <summary>
		/// Array of Byte scan.
		/// </summary>
		/// <param name="start">Your starting address.</param>
		/// <param name="end">ending address</param>
		/// <param name="search">array of bytes to search for, OR your ini code label.</param>
		/// <param name="file">ini file (OPTIONAL)</param>
		/// <param name="readable">Include readable addresses in scan</param>
		/// <param name="writable">Include writable addresses in scan</param>
		/// <param name="executable">Include executable addresses in scan</param>
		/// <returns>IEnumerable of all addresses found.</returns>
		Task<std::vector<long long>>* AoBScan(long long start, long long end, const std::wstring& search, bool readable, bool writable, bool executable, const std::wstring& file = L"")
		{
			return Task::Run([&]()
			{
				auto memRegionList = std::vector<MemoryRegionResult*>();

				std::wstring memCode = LoadCode(search, file);

				std::vector<std::wstring> stringByteArray = StringHelper::split(memCode, L' ');

				std::vector<unsigned char> aobPattern(stringByteArray.size());
				std::vector<unsigned char> mask(stringByteArray.size());

				for (auto i = 0; i < stringByteArray.size(); i++)
				{
					std::wstring ba = stringByteArray[i];

					if (ba == L"??" || (ba.length() == 1 && ba == L"?"))
					{
						mask[i] = 0x00;
						stringByteArray[i] = L"0x00";
					}
					else if (std::isalnum(ba[0]) && ba[1] == L'?')
					{
						mask[i] = 0xF0;
						stringByteArray[i] = StringHelper::toString(ba[0]) + L"0";
					}
					else if (std::isalnum(ba[1]) && ba[0] == L'?')
					{
						mask[i] = 0x0F;
						stringByteArray[i] = L"0" + StringHelper::toString(ba[1]);
					}
					else
					{
						mask[i] = 0xFF;
					}
				}


				for (int i = 0; i < stringByteArray.size(); i++)
				{
					aobPattern[i] = static_cast<unsigned char>(Convert::ToByte(stringByteArray[i], 16) & mask[i]);
				}

				SYSTEM_INFO sys_info = SYSTEM_INFO();
				GetSystemInfo(sys_info);

				UIntPtr proc_min_address = sys_info.minimumApplicationAddress;
				UIntPtr proc_max_address = sys_info.maximumApplicationAddress;

				if (start < static_cast<long long>(proc_min_address.ToUInt64()))
				{
					start = static_cast<long long>(proc_min_address.ToUInt64());
				}

				if (end > static_cast<long long>(proc_max_address.ToUInt64()))
				{
					end = static_cast<long long>(proc_max_address.ToUInt64());
				}

				Debug::WriteLine(L"[DEBUG] memory scan starting... (start:0x" + start.ToString(MSize()) + L" end:0x" + end.ToString(MSize()) + L" time:" + DateTime::Now.ToString(L"h:mm:ss tt") + L")");
				UIntPtr currentBaseAddress = UIntPtr(static_cast<unsigned long long>(start));

				MEMORY_BASIC_INFORMATION memInfo = MEMORY_BASIC_INFORMATION();

				//Debug.WriteLine("[DEBUG] start:0x" + start.ToString("X8") + " curBase:0x" + currentBaseAddress.ToUInt64().ToString("X8") + " end:0x" + end.ToString("X8") + " size:0x" + memInfo.RegionSize.ToString("X8") + " vAloc:" + VirtualQueryEx(pHandle, currentBaseAddress, out memInfo).ToUInt64().ToString());

				while (VirtualQueryEx(pHandle, currentBaseAddress, memInfo).ToUInt64() != 0 && currentBaseAddress.ToUInt64() < static_cast<unsigned long long>(end) && currentBaseAddress.ToUInt64() + static_cast<unsigned long long>(memInfo.RegionSize) > currentBaseAddress.ToUInt64())
				{
					bool isValid = memInfo.State == MEM_COMMIT;
					isValid &= memInfo.BaseAddress.ToUInt64() < static_cast<unsigned long long>(proc_max_address.ToUInt64());
					isValid &= ((memInfo.Protect & PAGE_GUARD) == 0);
					isValid &= ((memInfo.Protect & PAGE_NOACCESS) == 0);
					isValid &= (memInfo.Type == MEM_PRIVATE) || (memInfo.Type == MEM_IMAGE);

					if (isValid)
					{
						bool isReadable = (memInfo.Protect & PAGE_READONLY) > 0;

						bool isWritable = ((memInfo.Protect & PAGE_READWRITE) > 0) || ((memInfo.Protect & PAGE_WRITECOPY) > 0) || ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) || ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

						bool isExecutable = ((memInfo.Protect & PAGE_EXECUTE) > 0) || ((memInfo.Protect & PAGE_EXECUTE_READ) > 0) || ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) || ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

						isReadable &= readable;
						isWritable &= writable;
						isExecutable &= executable;

						isValid &= isReadable || isWritable || isExecutable;
					}

					if (!isValid)
					{
						currentBaseAddress = UIntPtr(memInfo.BaseAddress.ToUInt64() + static_cast<unsigned long long>(memInfo.RegionSize));
						continue;
					}

					MemoryRegionResult* memRegion = new MemoryRegionResult();
					memRegion->CurrentBaseAddress = currentBaseAddress;
					memRegion->RegionSize = memInfo.RegionSize;
					memRegion->RegionBase = memInfo.BaseAddress;

					currentBaseAddress = UIntPtr(memInfo.BaseAddress.ToUInt64() + static_cast<unsigned long long>(memInfo.RegionSize));

					//Console.WriteLine("SCAN start:" + memRegion.RegionBase.ToString() + " end:" + currentBaseAddress.ToString());

					if (memRegionList.size() > 0)
					{
						auto previousRegion = memRegionList[memRegionList.size() - 1];

						if (static_cast<long long>(previousRegion->RegionBase) + previousRegion->RegionSize == static_cast<long long>(memInfo.BaseAddress))
						{
							MemoryRegionResult* tempVar = new MemoryRegionResult();
							tempVar->CurrentBaseAddress = previousRegion->CurrentBaseAddress;
							tempVar->RegionBase = previousRegion->RegionBase;
							tempVar->RegionSize = previousRegion->RegionSize + memInfo.RegionSize;
							memRegionList[memRegionList.size() - 1] = tempVar;

							delete tempVar;
							delete memRegion;
							continue;

							delete tempVar;
						}
					}

					memRegionList.push_back(memRegion);

					//C# TO C++ CONVERTER TODO TASK: A 'delete memRegion' statement was not added since memRegion was passed to a method or constructor. Handle memory management manually.
				}

				ConcurrentBag<long long>* bagResult = new ConcurrentBag<long long>();

				Parallel::ForEach(memRegionList, [&](item, parallelLoopState, index)
				{
					std::vector<long long> compareResults = CompareScan(item, aobPattern, mask);

					for (auto result : compareResults)
					{
						bagResult->Add(result);
					}
				});

				Debug::WriteLine(L"[DEBUG] memory scan completed. (time:" + DateTime::Now.ToString(L"h:mm:ss tt") + L")");

				delete bagResult;
				return bagResult->ToList()->OrderBy([&](std::any c)
				{
					//C# TO C++ CONVERTER TODO TASK: A 'delete bagResult' statement was not added since bagResult was used in a 'return' or 'throw' statement.
					return c;
				})->AsEnumerable();

				//C# TO C++ CONVERTER TODO TASK: A 'delete bagResult' statement was not added since bagResult was used in a 'return' or 'throw' statement.
			});
		}

		/// <summary>
		/// Array of bytes scan
		/// </summary>
		/// <param name="code">Starting address or ini label</param>
		/// <param name="end">ending address</param>
		/// <param name="search">array of bytes to search for or your ini code label</param>
		/// <param name="file">ini file</param>
		/// <returns>First address found</returns>
//C# TO C++ CONVERTER TODO TASK: There is no equivalent in C++ to the 'async' keyword:
//ORIGINAL LINE: public async Task<long> AoBScan(string code, long end, string search, string file = "")
		Task<long long>* AoBScan(const std::wstring& code, long long end, const std::wstring& search, const std::wstring& file = L"")
		{
			long long start = static_cast<long long>(GetCode(code, file).ToUInt64());

			//C# TO C++ CONVERTER TODO TASK: There is no equivalent to 'await' in C++:
			return (await AoBScan(start, end, search, true, true, true, file)).FirstOrDefault();
		}

	private:
		std::vector<long long> CompareScan(MemoryRegionResult* item, std::vector<unsigned char>& aobPattern, std::vector<unsigned char>& mask)
		{
			if (mask.size() != aobPattern.size())
			{
				throw std::invalid_argument(StringHelper::formatSimple(L"{0}.Length != {1}.Length", L"aobPattern", L"mask"));
			}

			std::any buffer = Marshal::AllocHGlobal(static_cast<int>(item->RegionSize));

			unsigned long long bytesRead;
			ReadProcessMemory(pHandle, item->CurrentBaseAddress, buffer, static_cast<UIntPtr>(item->RegionSize), bytesRead);

			int result = 0 - aobPattern.size();
			std::vector<long long> ret;
			//C# TO C++ CONVERTER TODO TASK: C# 'unsafe' code is not converted by C# to C++ Converter:
			//			unsafe
			//			{
			//				do
			//				{
			//
			//					result = FindPattern((byte * )buffer.ToPointer(), (int)bytesRead, aobPattern, mask, result + aobPattern.Length);
			//
			//					if (result >= 0)
			//						ret.Add((long)item.CurrentBaseAddress + result);
			//
			//				} while (result != -1);
			//			}

			Marshal::FreeHGlobal(buffer);

			return ret.ToArray();
		}

		int FindPattern(std::vector<unsigned char>& body, std::vector<unsigned char>& pattern, std::vector<unsigned char>& masks, int start = 0)
		{
			int foundIndex = -1;

			if (body.size() <= 0 || pattern.size() <= 0 || start > body.size() - pattern.size() || pattern.size() > body.size())
			{
				return foundIndex;
			}

			for (int index = start; index <= body.size() - pattern.size(); index++)
			{
				if (((body[index] & masks[0]) == (pattern[0] & masks[0])))
				{
					auto match = true;
					for (int index2 = 1; index2 <= pattern.size() - 1; index2++)
					{
						if ((body[index + index2] & masks[index2]) == (pattern[index2] & masks[index2]))
						{
							continue;
						}
						match = false;
						break;

					}

					if (!match)
					{
						continue;
					}

					foundIndex = index;
					break;
				}
			}

			return foundIndex;
		}

		//C# TO C++ CONVERTER TODO TASK: C# 'unsafe' code is not converted by C# to C++ Converter:
		//		private unsafe int FindPattern(byte * body, int bodyLength, byte[] pattern, byte[] masks, int start = 0)
		//		{
		//			int foundIndex = -1;
		//
		//			if (bodyLength <= 0 || pattern.Length <= 0 || start > bodyLength - pattern.Length || pattern.Length > bodyLength)
		//				return foundIndex;
		//
		//			for (int index = start; index <= bodyLength - pattern.Length; index++)
		//			{
		//				if (((body[index] & masks[0]) == (pattern[0] & masks[0])))
		//				{
		//					var match = true;
		//					for (int index2 = 1; index2 <= pattern.Length - 1; index2++)
		//					{
		//						if ((body[index + index2] & masks[index2]) == (pattern[index2] & masks[index2]))
		//							continue;
		//						match = false;
		//						break;
		//
		//					}
		//
		//					if (!match)
		//						continue;
		//
		//					foundIndex = index;
		//					break;
		//				}
		//			}
		//
		//			return foundIndex;
		//		}

#endif
	};
}

//Helper class added by C# to C++ Converter:

//----------------------------------------------------------------------------------------
//	Copyright © 2004 - 2020 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replace some string methods, including
//	conversions to or from strings.
//----------------------------------------------------------------------------------------
#include <string>
#include <sstream>
#include <vector>
#include <stdexcept>
#include <cctype>
#include <algorithm>

class StringHelper
{
public:
	static std::wstring toLower(std::wstring source)
	{
		std::transform(source.begin(), source.end(), source.begin(), [](unsigned char c) { return std::tolower(c); });
		return source;
	}

	static std::wstring toUpper(std::wstring source)
	{
		std::transform(source.begin(), source.end(), source.begin(), [](unsigned char c) { return std::toupper(c); });
		return source;
	}

	static std::wstring trimStart(std::wstring source, const std::wstring& trimChars = L" \t\n\r\v\f")
	{
		return source.erase(0, source.find_first_not_of(trimChars));
	}

	static std::wstring trimEnd(std::wstring source, const std::wstring& trimChars = L" \t\n\r\v\f")
	{
		return source.erase(source.find_last_not_of(trimChars) + 1);
	}

	static std::wstring trim(std::wstring source, const std::wstring& trimChars = L" \t\n\r\v\f")
	{
		return trimStart(trimEnd(source, trimChars), trimChars);
	}

	static std::wstring replace(std::wstring source, const std::wstring& find, const std::wstring& replace)
	{
		std::size_t pos = 0;
		while ((pos = source.find(find, pos)) != std::string::npos)
		{
			source.replace(pos, find.length(), replace);
			pos += replace.length();
		}
		return source;
	}

	static bool startsWith(const std::wstring& source, const std::wstring& value)
	{
		if (source.length() < value.length())
			return false;
		else
			return source.compare(0, value.length(), value) == 0;
	}

	static bool endsWith(const std::wstring& source, const std::wstring& value)
	{
		if (source.length() < value.length())
			return false;
		else
			return source.compare(source.length() - value.length(), value.length(), value) == 0;
	}

	static std::vector<std::wstring> split(const std::wstring& source, wchar_t delimiter)
	{
		std::vector<std::wstring> output;
		std::wistringstream ss(source);
		std::wstring nextItem;

		while (std::getline(ss, nextItem, delimiter))
		{
			output.push_back(nextItem);
		}

		return output;
	}

	template<typename T>
	static std::wstring toString(const T& subject)
	{
		std::wostringstream ss;
		ss << subject;
		return ss.str();
	}

	template<typename T>
	static T fromString(const std::wstring& subject)
	{
		std::wistringstream ss(subject);
		T target;
		ss >> target;
		return target;
	}

	static bool isEmptyOrWhiteSpace(const std::wstring& source)
	{
		if (source.length() == 0)
			return true;
		else
		{
			for (std::size_t index = 0; index < source.length(); index++)
			{
				if (!std::isspace(source[index]))
					return false;
			}

			return true;
		}
	}

	template<typename T>
	static std::wstring formatSimple(const std::wstring& input, T arg0)
	{
		std::wostringstream ss;
		std::size_t lastCloseBrace = std::string::npos;
		std::size_t openBrace = std::string::npos;
		while ((openBrace = input.find(L'{', openBrace + 1)) != std::string::npos)
		{
			if (openBrace + 1 < input.length())
			{
				if (input[openBrace + 1] == L'{')
				{
					openBrace++;
					continue;
				}

				std::size_t closeBrace = input.find(L'}', openBrace + 1);
				if (closeBrace != std::string::npos)
				{
					ss << input.substr(lastCloseBrace + 1, openBrace - lastCloseBrace - 1);
					lastCloseBrace = closeBrace;

					std::wstring index = trim(input.substr(openBrace + 1, closeBrace - openBrace - 1));
					if (index == L"0")
						ss << arg0;
					else
						throw std::runtime_error("Only simple positional format specifiers are handled by the 'formatSimple' helper method.");
				}
			}
		}

		if (lastCloseBrace + 1 < input.length())
			ss << input.substr(lastCloseBrace + 1);

		return ss.str();
	}

	template<typename T1, typename T2>
	static std::wstring formatSimple(const std::wstring& input, T1 arg0, T2 arg1)
	{
		std::wostringstream ss;
		std::size_t lastCloseBrace = std::string::npos;
		std::size_t openBrace = std::string::npos;
		while ((openBrace = input.find(L'{', openBrace + 1)) != std::string::npos)
		{
			if (openBrace + 1 < input.length())
			{
				if (input[openBrace + 1] == L'{')
				{
					openBrace++;
					continue;
				}

				std::size_t closeBrace = input.find(L'}', openBrace + 1);
				if (closeBrace != std::string::npos)
				{
					ss << input.substr(lastCloseBrace + 1, openBrace - lastCloseBrace - 1);
					lastCloseBrace = closeBrace;

					std::wstring index = trim(input.substr(openBrace + 1, closeBrace - openBrace - 1));
					if (index == L"0")
						ss << arg0;
					else if (index == L"1")
						ss << arg1;
					else
						throw std::runtime_error("Only simple positional format specifiers are handled by the 'formatSimple' helper method.");
				}
			}
		}

		if (lastCloseBrace + 1 < input.length())
			ss << input.substr(lastCloseBrace + 1);

		return ss.str();
	}

	template<typename T1, typename T2, typename T3>
	static std::wstring formatSimple(const std::wstring& input, T1 arg0, T2 arg1, T3 arg2)
	{
		std::wostringstream ss;
		std::size_t lastCloseBrace = std::string::npos;
		std::size_t openBrace = std::string::npos;
		while ((openBrace = input.find(L'{', openBrace + 1)) != std::string::npos)
		{
			if (openBrace + 1 < input.length())
			{
				if (input[openBrace + 1] == L'{')
				{
					openBrace++;
					continue;
				}

				std::size_t closeBrace = input.find(L'}', openBrace + 1);
				if (closeBrace != std::string::npos)
				{
					ss << input.substr(lastCloseBrace + 1, openBrace - lastCloseBrace - 1);
					lastCloseBrace = closeBrace;

					std::wstring index = trim(input.substr(openBrace + 1, closeBrace - openBrace - 1));
					if (index == L"0")
						ss << arg0;
					else if (index == L"1")
						ss << arg1;
					else if (index == L"2")
						ss << arg2;
					else
						throw std::runtime_error("Only simple positional format specifiers are handled by the 'formatSimple' helper method.");
				}
			}
		}

		if (lastCloseBrace + 1 < input.length())
			ss << input.substr(lastCloseBrace + 1);

		return ss.str();
	}
};

//Helper class added by C# to C++ Converter:

//----------------------------------------------------------------------------------------
//	Copyright © 2004 - 2020 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replace the .NET StringBuilder in C++.
//----------------------------------------------------------------------------------------
#include <string>
#include <sstream>

class StringBuilder
{
private:
	std::wstring privateString;

public:
	StringBuilder()
	{
	}

	StringBuilder(const std::wstring& initialString)
	{
		privateString = initialString;
	}

	StringBuilder(std::size_t capacity)
	{
		ensureCapacity(capacity);
	}

	StringBuilder(const std::wstring& initialString, std::size_t capacity)
	{
		privateString = initialString;
		ensureCapacity(capacity);
	}

	wchar_t operator [](std::size_t index)
	{
		return privateString[index];
	}

	StringBuilder* append(const std::wstring& toAppend)
	{
		privateString += toAppend;
		return this;
	}

	template<typename T>
	StringBuilder* append(const T& toAppend)
	{
		privateString += toString(toAppend);
		return this;
	}

	StringBuilder* appendLine()
	{
		privateString += L"\r\n";
		return this;
	}

	StringBuilder* appendLine(const std::wstring& toAppend)
	{
		privateString += toAppend + L"\r\n";
		return this;
	}

	StringBuilder* insert(std::size_t position, const std::wstring& toInsert)
	{
		privateString.insert(position, toInsert);
		return this;
	}

	template<typename T>
	StringBuilder* insert(std::size_t position, const T& toInsert)
	{
		privateString.insert(position, toString(toInsert));
		return this;
	}

	std::wstring toString()
	{
		return privateString;
	}

	std::wstring toString(std::size_t start, std::size_t length)
	{
		return privateString.substr(start, length);
	}

	std::size_t length()
	{
		return privateString.length();
	}

	void setLength(std::size_t newLength)
	{
		privateString.resize(newLength);
	}

	std::size_t capacity()
	{
		return privateString.capacity();
	}

	void ensureCapacity(std::size_t minimumCapacity)
	{
		privateString.reserve(minimumCapacity);
	}

	std::size_t maxCapacity()
	{
		return privateString.max_size();
	}

	void clear()
	{
		privateString.clear();
	}

	StringBuilder* remove(std::size_t start, std::size_t length)
	{
		privateString.erase(start, length);
		return this;
	}

	StringBuilder* replace(const std::wstring& oldString, const std::wstring& newString)
	{
		std::size_t pos = 0;
		while ((pos = privateString.find(oldString, pos)) != std::wstring::npos)
		{
			privateString.replace(pos, oldString.length(), newString);
			pos += newString.length();
		}
		return this;
	}

private:
	template<typename T>
	static std::wstring toString(const T& subject)
	{
		std::wostringstream ss;
		ss << subject;
		return ss.str();
	}
};

//Helper class added by C# to C++ Converter:

//----------------------------------------------------------------------------------------
//	Copyright © 2004 - 2020 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replace some static .NET file and folder method calls
//	with std::filesystem method calls.
//----------------------------------------------------------------------------------------
#include <string>
#include <filesystem>

class FileSystem
{
public:
	static void createDirectory(const std::wstring& path)
	{
		std::filesystem::create_directory(pathFromString(path));
	}

	static bool fileExists(const std::wstring& path)
	{
		return std::filesystem::is_regular_file(pathFromString(path));
	}

	static bool directoryExists(const std::wstring& path)
	{
		return std::filesystem::is_directory(pathFromString(path));
	}

	static std::wstring combine(const std::wstring& path1, const std::wstring& path2)
	{
		return (pathFromString(path1) / pathFromString(path2)).generic_wstring();
	}

	static bool isPathRooted(const std::wstring& path)
	{
		return pathFromString(path).has_root_path();
	}

	static std::wstring getFullPath(const std::wstring& path)
	{
		return std::filesystem::absolute(pathFromString(path)).generic_wstring();
	}

	static std::wstring getFileName(const std::wstring& path)
	{
		return std::filesystem::path(pathFromString(path)).filename().generic_wstring();
	}

	static std::wstring getDirectoryName(const std::wstring& path)
	{
		return std::filesystem::path(pathFromString(path)).parent_path().generic_wstring();
	}

	static std::wstring getCurrentDirectory()
	{
		return std::filesystem::current_path().generic_wstring();
	}

	static void copyFile(const std::wstring& path1, const std::wstring& path2)
	{
		std::filesystem::copy_file(pathFromString(path1), pathFromString(path2));
	}

	static void renamePath(const std::wstring& path1, const std::wstring& path2)
	{
		std::filesystem::rename(pathFromString(path1), pathFromString(path2));
	}

	static wchar_t preferredSeparator()
	{
		return std::filesystem::path::preferred_separator;
	}

private:
	static std::filesystem::path pathFromString(const std::wstring& path)
	{
		return std::filesystem::path(&path[0]);
	}
};
