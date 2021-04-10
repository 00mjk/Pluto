﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Pluto.Shellcode.Structures;

namespace Pluto.Shellcode
{
    internal static class ShellcodeAssembler
    {
        internal static Span<byte> AssembleSyscall32(SyscallDescriptor syscallDescriptor)
        {
            var shellcode = new List<byte>();

            // mov eax, Index

            shellcode.Add(0xB8);

            shellcode.AddRange(BitConverter.GetBytes(syscallDescriptor.Index));

            if (Environment.Is64BitOperatingSystem)
            {
                // call DWORD PTR ds:[Wow64Transition]

                shellcode.AddRange(new byte[] {0xFF, 0x15});

                shellcode.AddRange(BitConverter.GetBytes(NativeLibrary.GetExport(NativeLibrary.Load("ntdll.dll"), "Wow64Transition").ToInt32()));
            }

            else
            {
                // mov edx, esp

                shellcode.AddRange(new byte[] {0x89, 0xE2});

                // sysenter

                shellcode.AddRange(new byte[] {0xF, 0x34});
            }

            // ret

            shellcode.Add(0xC3);

            return CollectionsMarshal.AsSpan(shellcode);
        }

        internal static Span<byte> AssembleSyscall64(SyscallDescriptor syscallDescriptor)
        {
            var shellcode = new List<byte>();

            // mov r10, rcx

            shellcode.AddRange(new byte[] {0x4C, 0x8B, 0xD1});

            // mov eax, Index

            shellcode.Add(0xB8);

            shellcode.AddRange(BitConverter.GetBytes(syscallDescriptor.Index));

            // syscall

            shellcode.AddRange(new byte[] {0xF, 0x5});

            // ret

            shellcode.Add(0xC3);

            return CollectionsMarshal.AsSpan(shellcode);
        }
    }
}