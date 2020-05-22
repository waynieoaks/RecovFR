using System;
using System.Runtime.InteropServices;

namespace RecovFR
{
    class MemoryClasses
    {
        public static class MemoryAccess
        {
            private static readonly IntPtr CRenderPhaseDeferredLighting_Patch1,
                CRenderPhaseDeferredLighting_Patch2;

            static MemoryAccess()
            {
                var pattern = new Pattern("\xE8\x00\x00\x00\x00\x80\x3D\x00\x00\x00\x00\x00\x48\x63\xC8", "x????xx?????xxx");

                var result = pattern.Get(0x1A);

                if (result != null)
                {
                    CRenderPhaseDeferredLighting_Patch1 = result;
                }

                result = pattern.Get(0x35);

                if (result != null)
                {
                    CRenderPhaseDeferredLighting_Patch2 = result;
                }
            }

            public static void SetSnowRendered(bool enabled)
            {
                if (enabled)
                {
                    Marshal.WriteByte(CRenderPhaseDeferredLighting_Patch1, 0xEB);
                    Marshal.Copy(new byte[] { 0x90, 0x90 }, 0, CRenderPhaseDeferredLighting_Patch2, 2);
                }

                else
                {
                    Marshal.WriteByte(CRenderPhaseDeferredLighting_Patch1, 0x75);
                    Marshal.Copy(new byte[] { 0x74, 0x13 }, 0, CRenderPhaseDeferredLighting_Patch2, 2);
                }
            }

            public static byte GetSnowRendered()
            {
                return Marshal.ReadByte(CRenderPhaseDeferredLighting_Patch1);
            }
        }

        // Pattern.cs:
        public sealed class Pattern
        {
            private string bytes, mask;

            public Pattern(string bytes, string mask)
            {
                this.bytes = bytes;
                this.mask = mask;
            }

            public unsafe IntPtr Get(string moduleName, int offset)
            {
                Win32Native.MODULEINFO module;

                Win32Native.GetModuleInformation(
                    Win32Native.GetCurrentProcess(),
                    Win32Native.GetModuleHandle(moduleName),
                    out module,
                    sizeof(Win32Native.MODULEINFO));

                var address = module.lpBaseOfDll.ToInt64();

                var end = address + module.SizeOfImage;

                for (; address < end; address++)
                {
                    if (bCompare((byte*)(address), bytes.ToCharArray(), mask.ToCharArray()))
                    {
                        return new IntPtr(address + offset);
                    }
                }

                return IntPtr.Zero;
            }

            public unsafe IntPtr Get(int offset = 0)
            {
                return Get(null, offset);
            }

            private unsafe bool bCompare(byte* pData, char[] bMask, char[] szMask)
            {
                int i = 0;

                for (; i < bMask.Length; i++)
                {
                    if (szMask[i] == 'x' && pData[i] != bMask[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static class Win32Native
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("psapi.dll", SetLastError = true)]
            public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, int cb);

            [StructLayout(LayoutKind.Sequential)]
            public struct MODULEINFO
            {
                public IntPtr lpBaseOfDll;
                public uint SizeOfImage;
                public IntPtr EntryPoint;
            }
        }
    }
}
// Call with MemoryAccess.SetSnowRendered(true);