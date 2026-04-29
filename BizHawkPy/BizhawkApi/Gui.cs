using BizHawk.Client.Common;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BizHawkPy.BizhawkApi;

internal static class Gui
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {

            ["gui.addmessage"] = (apis, bridge, args) =>
            {
                var message = Utils.Parse<string>(args, 0);
                apis.Gui.AddMessage(message);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.clearGraphics"] = (apis, bridge, args) =>
            {
                var sur = Utils.Parse<string?>(args, 0);
                var surface = DisplaySurfaceIDParser.Parse(sur);
                apis.Gui.ClearGraphics(surface);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.clearImageCache"] = (apis, bridge, args) =>
            {
                apis.Gui.ClearImageCache();
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.cleartext"] = (apis, bridge, args) =>
            {
                apis.Gui.ClearText();
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.createcanvas"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var width =Utils.Parse<int>(args, 0);
                //var height =Utils.Parse<int>(args, 1);
                //var x =Utils.Parse<int?>(args, 2);
                //var y =Utils.Parse<int?>(args, 3);
                //var result = apis.Gui.CreateCanvas(width, height, x, y);
                //bridge.CmdReturn($"{SerializeDict(result)}");
            },

            ["gui.defaultBackground"] = (apis, bridge, args) =>
            {
                var color = Utils.ToColor(Utils.Parse<string?>(args, 0))
                    ?? throw new ArgumentNullException("color", "luacolor is null");
                apis.Gui.SetDefaultBackgroundColor(color);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.defaultForeground"] = (apis, bridge, args) =>
            {
                var color = Utils.ToColor(Utils.Parse<string?>(args, 0))
                    ?? throw new ArgumentNullException("color", "luacolor is null");
                apis.Gui.SetDefaultForegroundColor(color);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.defaultPixelFont"] = (apis, bridge, args) =>
            {
                var font = Utils.Parse<string>(args, 0);
                apis.Gui.SetDefaultPixelFont(font);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.defaultTextBackground"] = (apis, bridge, args) =>
            {
                var color = Utils.ToColor(Utils.Parse<string?>(args, 0))
                    ?? throw new ArgumentNullException("color", "luacolor is null");
                apis.Gui.SetDefaultTextBackground(color);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawAxis"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var size = Utils.Parse<int>(args, 2);
                var color = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var surface = Utils.Parse<string?>(args, 4);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawAxis(x, y, size, color, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawBezier"] = (apis, bridge, args) =>
            {
                var x1 = Utils.Parse<int>(args, 0);
                var y1 = Utils.Parse<int>(args, 1);
                var x2 = Utils.Parse<int>(args, 2);
                var y2 = Utils.Parse<int>(args, 3);
                var x3 = Utils.Parse<int>(args, 4);
                var y3 = Utils.Parse<int>(args, 5);
                var x4 = Utils.Parse<int>(args, 6);
                var y4 = Utils.Parse<int>(args, 7);
                var color = Utils.ToColor(Utils.Parse<string>(args, 8));
                var surface = Utils.Parse<string?>(args, 9);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawBezier(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4), color, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawBox"] = (apis, bridge, args) =>
            {
                var x1 = Utils.Parse<int>(args, 0);
                var y1 = Utils.Parse<int>(args, 1);
                var x2 = Utils.Parse<int>(args, 2);
                var y2 = Utils.Parse<int>(args, 3);
                var line = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var bg = Utils.ToColor(Utils.Parse<string?>(args, 5));
                var surface = Utils.Parse<string?>(args, 6);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawBox(x1, y1, x2, y2, line, bg, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawEllipse"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var w = Utils.Parse<int>(args, 2);
                var h = Utils.Parse<int>(args, 3);
                var line = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var bg = Utils.ToColor(Utils.Parse<string?>(args, 5));
                var surface = Utils.Parse<string?>(args, 6);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawEllipse(x, y, w, h, line, bg, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawIcon"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string>(args, 0);
                var x = Utils.Parse<int>(args, 1);
                var y = Utils.Parse<int>(args, 2);
                var w = Utils.Parse<int?>(args, 3);
                var h = Utils.Parse<int?>(args, 4);
                var surface = Utils.Parse<string?>(args, 5);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawIcon(path, x, y, w, h, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawImage"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var path = Utils.Parse<string>(args, 0);
                //var x = Utils.Parse<int>(args, 1);
                //var y = Utils.Parse<int>(args, 2);
                //var w = Utils.Parse<int?>(args, 3);
                //var h = Utils.Parse<int?>(args, 4);
                //var cache = Utils.Parse<bool?>(args, 5) ?? true;
                //var surface = Utils.Parse<string?>(args, 6);
                //var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                //        ?? bridge.APIState.surfaceID;
                //apis.Gui.DrawImage(path, x, y, w, h, cache, surfaceID);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawImageRegion"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var path = Utils.Parse<string>(args, 0);
                //var sx = Utils.Parse<int>(args, 1);
                //var sy = Utils.Parse<int>(args, 2);
                //var sw = Utils.Parse<int>(args, 3);
                //var sh = Utils.Parse<int>(args, 4);
                //var dx = Utils.Parse<int>(args, 5);
                //var dy = Utils.Parse<int>(args, 6);
                //var dw = Utils.Parse<int?>(args, 7);
                //var dh = Utils.Parse<int?>(args, 8);
                //var surface = Utils.Parse<string?>(args, 9);
                //var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                //        ?? bridge.APIState.surfaceID;
                //apis.Gui.DrawImageRegion(path, sx, sy, sw, sh, dx, dy, dw, dh, surfaceID);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawLine"] = (apis, bridge, args) =>
            {
                var x1 = Utils.Parse<int>(args, 0);
                var y1 = Utils.Parse<int>(args, 1);
                var x2 = Utils.Parse<int>(args, 2);
                var y2 = Utils.Parse<int>(args, 3);
                var color = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var surface = Utils.Parse<string?>(args, 5);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawLine(x1, y1, x2, y2, color, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawPie"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var w = Utils.Parse<int>(args, 2);
                var h = Utils.Parse<int>(args, 3);
                var start = Utils.Parse<int>(args, 4);
                var sweep = Utils.Parse<int>(args, 5);
                var line = Utils.ToColor(Utils.Parse<string?>(args, 6));
                var bg = Utils.ToColor(Utils.Parse<string?>(args, 7));
                var surface = Utils.Parse<string?>(args, 8);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawPie(x, y, w, h, start, sweep, line, bg, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawPixel"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var color = Utils.ToColor(Utils.Parse<string?>(args, 2));
                var surface = Utils.Parse<string?>(args, 3);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawPixel(x, y, color, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawPolygon"] = (apis, bridge, args) =>
            {
                var rawPoints = Utils.Parse<object>(args, 0);
                var points = Utils.ToPointArray(rawPoints);

                var ox = Utils.Parse<int?>(args, 1);
                var oy = Utils.Parse<int?>(args, 2);
                var line = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var bg = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var surface = Utils.Parse<string?>(args, 5);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawPolygon(points, line, bg, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawRectangle"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var w = Utils.Parse<int>(args, 2);
                var h = Utils.Parse<int>(args, 3);
                var line = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var bg = Utils.ToColor(Utils.Parse<string?>(args, 5));
                var surface = Utils.Parse<string?>(args, 6);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawRectangle(x, y, w, h, line, bg, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawString"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var msg = Utils.Parse<string>(args, 2);
                var fc = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var bc = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var size = Utils.Parse<int?>(args, 5);
                var family = Utils.Parse<string?>(args, 6);
                var style = Utils.Parse<string?>(args, 7);
                var ha = Utils.Parse<string?>(args, 8);
                var va = Utils.Parse<string?>(args, 9);
                var surface = Utils.Parse<string?>(args, 10);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawString(x, y, msg, fc, bc, size, family, style, ha, va, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.drawText"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var msg = Utils.Parse<string>(args, 2);
                var fc = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var bc = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var size = Utils.Parse<int?>(args, 5);
                var family = Utils.Parse<string?>(args, 6);
                var style = Utils.Parse<string?>(args, 7);
                var ha = Utils.Parse<string?>(args, 8);
                var va = Utils.Parse<string?>(args, 9);
                var surface = Utils.Parse<string?>(args, 10);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.DrawString(x, y, msg, fc, bc, size, family, style, ha, va, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.pixelText"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var msg = Utils.Parse<string>(args, 2);
                var fc = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var bc = Utils.ToColor(Utils.Parse<string?>(args, 4));
                var family = Utils.Parse<string?>(args, 5);
                var surface = Utils.Parse<string?>(args, 6);
                var surfaceID = DisplaySurfaceIDParser.Parse(surface)
                        ?? bridge.APIState.surfaceID;
                apis.Gui.PixelText(x, y, msg, fc, bc, family, surfaceID);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.text"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var msg = Utils.Parse<string>(args, 2);
                var fc = Utils.ToColor(Utils.Parse<string?>(args, 3));
                var anchor = Utils.Parse<string?>(args, 4);
                apis.Gui.Text(x, y, msg, fc, anchor);
                bridge.CmdReturn(null, typeof(void));
            },

            ["gui.use_surface"] = (apis, bridge, args) =>
            {
                var name = Utils.Parse<string>(args, 0);
                var parsed = DisplaySurfaceIDParser.Parse(name);
                if (parsed is not null)
                {
                    bridge.APIState.surfaceID = parsed.Value;
                }
                bridge.CmdReturn(null, typeof(void));
            },

        };
    }
}