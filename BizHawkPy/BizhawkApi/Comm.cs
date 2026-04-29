using System;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Comm
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["comm.getluafunctionslist"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
            },


            // -------------------------
            // HTTP
            // -------------------------
            ["comm.httpGet"] = (apis, bridge, args) =>
            {
                var url = Utils.Parse<string>(args, 0);
                var result = apis.Comm.HTTP?.ExecGet(url);
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.httpPost"] = (apis, bridge, args) =>
            {
                var url = Utils.Parse<string>(args, 0);
                var payload = Utils.Parse<string>(args, 1);
                var result = apis.Comm.HTTP?.ExecPost(url, payload);
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.httpGetGetUrl"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.HTTP?.GetUrl;
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.httpGetPostUrl"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.HTTP?.PostUrl;
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.httpSetGetUrl"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.HTTP == null)
                    throw new InvalidOperationException("HTTP API is not available");
                var url = Utils.Parse<string>(args, 0);
                apis.Comm.HTTP.GetUrl = url;
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.httpSetPostUrl"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.HTTP == null)
                    throw new InvalidOperationException("HTTP API is not available");
                var url = Utils.Parse<string>(args, 0);
                apis.Comm.HTTP.PostUrl = url;
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.httpSetTimeout"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.HTTP == null)
                    throw new InvalidOperationException("HTTP API is not available");
                var timeout = Utils.Parse<int>(args, 0);
                apis.Comm.HTTP.SetTimeout(timeout);
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.httpTest"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.HttpTest();
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.httpTestGet"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.HttpTestGet();
                bridge.CmdReturn(result, typeof(string));
            },

            // -------------------------
            // MMF
            // -------------------------
            ["comm.mmfCopyFromMemory"] = (apis, bridge, args) =>
            {
                var mmf_filename = Utils.Parse<string>(args, 0);
                var addr = Utils.Parse<long>(args, 1);
                var length = Utils.Parse<int>(args, 2);
                var domain = Utils.Parse<string>(args, 3);
                throw new NotImplementedException();
                //var result = apis.Comm.MMF.(filename, size);
                //bridge.CmdReturn(result, typeof(int));
            },
            ["comm.mmfCopyToMemory"] = (apis, bridge, args) =>
            {
                var mmf_filename = Utils.Parse<string>(args, 0);
                var addr = Utils.Parse<long>(args, 1);
                var length = Utils.Parse<int>(args, 2);
                var domain = Utils.Parse<string>(args, 3);
                throw new NotImplementedException();
                //var result = apis.Comm.MMF.(filename, size);
                //bridge.CmdReturn(null, typeof(void));
            },
            ["comm.mmfGetFilename"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.MMF.Filename;
                bridge.CmdReturn(result, typeof(string));
            },
            ["comm.mmfRead"] = (apis, bridge, args) =>
            {
                var mmf_filename = Utils.Parse<string>(args, 0);
                var expectedsize = Utils.Parse<int>(args, 1);
                var result = apis.Comm.MMF.ReadFromFile(mmf_filename, expectedsize);
                bridge.CmdReturn(result, typeof(string));
            },
            ["comm.mmfReadBytes"] = (apis, bridge, args) =>
            {
                var mmf_filename = Utils.Parse<string>(args, 0);
                var expectedsize = Utils.Parse<int>(args, 1);
                var result = apis.Comm.MMF.ReadBytesFromFile(mmf_filename, expectedsize);
                bridge.CmdReturn(result, typeof(byte[]));
            },
            ["comm.mmfScreenshot"] = (apis, bridge, args) =>
            {
                var result = apis.Comm.MMF.ScreenShotToFile();
                bridge.CmdReturn(result, typeof(int));
            },
            ["comm.mmfSetFilename"] = (apis, bridge, args) =>
            {
                var filename = Utils.Parse<string>(args, 0);
                apis.Comm.MMF.Filename = filename;
                bridge.CmdReturn(null, typeof(void));
            },
            ["comm.mmfWrite"] = (apis, bridge, args) =>
            {
                var filename = Utils.Parse<string>(args, 0);
                var data = Utils.Parse<string>(args, 1);
                var result = apis.Comm.MMF.WriteToFile(filename, data);
                bridge.CmdReturn(result, typeof(string));
            },
            ["comm.mmfWriteBytes"] = (apis, bridge, args) =>
            {
                var filename = Utils.Parse<string>(args, 0);
                var data = Utils.Parse<byte[]>(args, 1);
                var result = apis.Comm.MMF.WriteToFile(filename, data);
                bridge.CmdReturn(result, typeof(string));
            },

            // -------------------------
            // Socket
            // -------------------------
            ["comm.socketServerGetInfo"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.GetInfo();
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.socketServerGetIp"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.IP;
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.socketServerGetPort"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                {
                    bridge.CmdReturn(null, typeof(void));
                    return;
                }
                var result = apis.Comm.Sockets.Port;
                bridge.CmdReturn(result, typeof(int));
            },
            ["comm.socketServerIsConnected"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                {
                    bridge.CmdReturn(false, typeof(bool));
                    return;
                }
                var result = apis.Comm.Sockets.Connected;
                bridge.CmdReturn(result, typeof(bool));
            },
            ["comm.socketServerResponse"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.ReceiveString();
                bridge.CmdReturn(result, typeof(string));
            },
            ["comm.socketServerScreenShot"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.SendScreenshot();
                bridge.CmdReturn(result, typeof(string));
            },

            ["comm.socketServerScreenShotResponse"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.SendScreenshot();
                bridge.CmdReturn(result, typeof(string));
            },
            ["comm.socketServerSend"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var sendstring = Utils.Parse<string>(args, 0);
                var result = apis.Comm.Sockets.SendString(sendstring);
                bridge.CmdReturn(result, typeof(int));
            },
            ["comm.socketServerSendBytes"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var bytearray = Utils.Parse<byte[]>(args, 0);
                var result = apis.Comm.Sockets.SendBytes(bytearray);
                bridge.CmdReturn(result, typeof(int));
            },
            ["comm.socketServerSetIp"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var ip = Utils.Parse<string>(args, 0);
                apis.Comm.Sockets.IP = ip;
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.socketServerSetPort"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var port = Utils.Parse<ushort>(args, 0);
                apis.Comm.Sockets.Port = port;
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.socketServerSetTimeout"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var timeout = Utils.Parse<int>(args, 0);
                apis.Comm.Sockets.SetTimeout(timeout);
                bridge.CmdReturn(null, typeof(void));
            },

            ["comm.socketServerSuccessful"] = (apis, bridge, args) =>
            {
                if (apis?.Comm?.Sockets == null)
                    throw new InvalidOperationException("Sockets API is not available");
                var result = apis.Comm.Sockets.Successful;
                bridge.CmdReturn(result, typeof(bool));
            },

        };
    }
}