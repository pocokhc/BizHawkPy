using System;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Memory
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["memory.getcurrentmemorydomain"] = (apis, bridge, args) =>
            {
                var result = apis.Memory.GetCurrentMemoryDomain();
                bridge.CmdReturn(result, typeof(string));
            },

            ["memory.getcurrentmemorydomainsize"] = (apis, bridge, args) =>
            {
                var result = apis.Memory.GetCurrentMemoryDomainSize();
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.getmemorydomainlist"] = (apis, bridge, args) =>
            {
                var result = apis.Memory.GetMemoryDomainList();
                bridge.CmdReturn(result, typeof(object));
            },

            ["memory.getmemorydomainsize"] = (apis, bridge, args) =>
            {
                var name = Utils.Parse<string?>(args, 0);
                var result = apis.Memory.GetMemoryDomainSize(name);
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.hash_region"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var count = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                var result = apis.Memory.HashRegion(addr, count, domain);
                bridge.CmdReturn(result, typeof(string));
            },

            ["memory.read_bytes_as_array"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var length = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                var result = apis.Memory.ReadByteRange(addr, length, domain);
                bridge.CmdReturn(result, typeof(object));
            },

            ["memory.read_bytes_as_dict"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var length = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                var result = apis.Memory.ReadByteRange(addr, length, domain);

                var dict = new Dictionary<long, byte>(result.Count);
                for (int i = 0; i < result.Count; i++)
                {
                    dict[addr + i] = result[i];
                }

                bridge.CmdReturn(dict, typeof(object));
            },

            ["memory.read_s16_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var bytes = apis.Memory.ReadByteRange(addr, 2, domain);
                short result = (short)((bytes[0] << 8) | bytes[1]);
                bridge.CmdReturn(result, typeof(short));
            },

            ["memory.read_s16_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadS16(addr, domain);
                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_s24_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);

                var b = apis.Memory.ReadByteRange(addr, 3, domain);
                int result = (b[0] << 16) | (b[1] << 8) | b[2];

                // 符号拡張（24bit → 32bit）
                if ((result & 0x800000) != 0)
                    result |= unchecked((int)0xFF000000);

                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_s24_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadS24(addr, domain);
                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_s32_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);

                var b = apis.Memory.ReadByteRange(addr, 4, domain);
                int result = (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];

                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_s32_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadS32(addr, domain);
                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_s8"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadS8(addr, domain);
                bridge.CmdReturn(result, typeof(int));
            },
            ["memory.read_u16_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var bytes = apis.Memory.ReadByteRange(addr, 2, domain);
                ushort value = (ushort)((bytes[0] << 8) | bytes[1]);
                bridge.CmdReturn(value, typeof(ushort));
            },

            ["memory.read_u16_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadU16(addr, domain);
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.read_u24_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);

                var b = apis.Memory.ReadByteRange(addr, 3, domain);
                int result = (b[0] << 16) | (b[1] << 8) | b[2];

                // 符号拡張しない

                bridge.CmdReturn(result, typeof(int));
            },

            ["memory.read_u24_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadU24(addr, domain);
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.read_u32_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);

                var b = apis.Memory.ReadByteRange(addr, 4, domain);
                uint result = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];

                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.read_u32_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadU32(addr, domain);
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.read_u8"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadU8(addr, domain);
                bridge.CmdReturn(result, typeof(uint));
            },

            ["memory.readbyte"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);
                var result = apis.Memory.ReadByte(addr, domain);
                bridge.CmdReturn(result, typeof(uint));
            },

            // [deprecated]
            //["memory.readbyterange"] = (apis, bridge, args) =>
            //{
            //    var addr = Utils.Parse<long>(args, 0);
            //    var length = Utils.Parse<int>(args, 1);
            //    var domain = Utils.Parse<string?>(args, 2);
            //    var result = apis.Memory.ReadByteRange(addr, length, domain);
            //    bridge.CmdReturn($"{SerializeDict(result)}");
            //},

            ["memory.readfloat"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var bigEndian = Utils.Parse<bool>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                float value;
                if (bigEndian)
                {
                    var src = apis.Memory.ReadByteRange(addr, 4, domain);

                    var b = BitConverter.IsLittleEndian
                        ? new byte[] { src[3], src[2], src[1], src[0] }
                        : new byte[] { src[0], src[1], src[2], src[3] };

                    value = BitConverter.ToSingle(b, 0);
                }
                else
                {
                    value = apis.Memory.ReadFloat(addr, domain);
                }

                bridge.CmdReturn(value, typeof(float));
            },

            ["memory.usememorydomain"] = (apis, bridge, args) =>
            {
                var domain = Utils.Parse<string>(args, 0);
                var result = apis.Memory.UseMemoryDomain(domain);
                bridge.CmdReturn(result, typeof(bool));
            },

            ["memory.write_bytes_as_array"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var bytes_ = Utils.Parse<List<byte>>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                bridge._top.Log($"{bytes_}");
                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },
            ["memory.write_bytes_as_dict"] = (apis, bridge, args) =>
            {
                var addrmap = Utils.Parse<Dictionary<long, byte>>(args, 0);
                var domain = Utils.Parse<string?>(args, 1);

                foreach (var kv in addrmap)
                    apis.Memory.WriteByte(kv.Key, kv.Value, domain);

                bridge.CmdReturn(null, typeof(void));
            },
            ["memory.write_s16_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<short>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                var bytes_ = new List<byte>
                {
                    (byte)((value >> 8) & 0xFF), // hi
                    (byte)(value & 0xFF)         // lo
                };
                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_s16_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<short>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteS16(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },
            ["memory.write_s24_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                var bytes_ = new List<byte>
                {
                    (byte)((value >> 16) & 0xFF), // hi
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)          // lo
                };

                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_s24_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteS24(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_s32_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                var bytes_ = new List<byte>
                {
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)
                };

                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_s32_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<int>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteS32(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_s8"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<sbyte>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteS8(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u16_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<ushort>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                var bytes_ = new List<byte>
                {
                    (byte)((value >> 8) & 0xFF), // hi
                    (byte)(value & 0xFF)         // lo
                };

                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u16_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<ushort>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteU16(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u24_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<uint>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                var bytes_ = new List<byte>
                {
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)
                };

                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u24_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<uint>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                apis.Memory.WriteU24(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u32_be"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<uint>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);

                var bytes_ = new List<byte>
                {
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)(value & 0xFF)
                };

                apis.Memory.WriteByteRange(addr, bytes_, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u32_le"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<uint>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteU32(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.write_u8"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<byte>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteU8(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.writebyte"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<byte>(args, 1);
                var domain = Utils.Parse<string?>(args, 2);
                apis.Memory.WriteByte(addr, value, domain);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memory.writefloat"] = (apis, bridge, args) =>
            {
                var addr = Utils.Parse<long>(args, 0);
                var value = Utils.Parse<float>(args, 1);
                var bigendian = Utils.Parse<bool>(args, 2);
                var domain = Utils.Parse<string?>(args, 3);

                var bytes_ = BitConverter.GetBytes(value);

                // BitConverterは環境依存（通常LE）なので調整
                if (BitConverter.IsLittleEndian == bigendian)
                {
                    Array.Reverse(bytes_);
                }
                apis.Memory.WriteByteRange(addr, bytes_, domain);

                bridge.CmdReturn(null, typeof(void));
            },
        };
    }
}