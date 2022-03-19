﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using SysBot.Base;
using System.Diagnostics;
using static SysBot.Base.SwitchOffsetType;

namespace PLARNGGui
{
    public class BotBaseRoutines
    {
        public int MaximumTransferSize { get; set; } = 0x1C0;
        public int BaseDelay { get; set; } = 64;
        public int DelayFactor { get; set; } = 256;
        private int Read(byte[] buffer)
        {
            int br = Main.Connection.Receive(buffer, 0, 1, SocketFlags.None);
            while (buffer[br - 1] != (byte)'\n')
                br += Main.Connection.Receive(buffer, br, 1, SocketFlags.None);
            return br;
        }

        public async Task<int> SendAsync(byte[] buffer) => await Task.Run(() => Main.Connection.Send(buffer)).ConfigureAwait(false);

        private async Task<byte[]> ReadBytesFromCmdAsync(byte[] cmd, int length)
        {
            await SendAsync(cmd).ConfigureAwait(false);

            var buffer = new byte[(length * 2) + 1];
            var _ = Read(buffer);
            return SysBot.Base.Decoder.ConvertHexByteStringToBytes(buffer);
        }
        public async Task<byte[]> ReadBytesAsync(uint offset, int length ) => await Read(offset, length, Heap).ConfigureAwait(false);
        public async Task<byte[]> ReadBytesMainAsync(ulong offset, int length) => await Read(offset, length, SysBot.Base.SwitchOffsetType.Main).ConfigureAwait(false);
        public async Task<byte[]> ReadBytesAbsoluteAsync(ulong offset, int length) => await Read(offset, length, Absolute).ConfigureAwait(false);

        public async Task<byte[]> ReadBytesMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes) => await ReadMulti(offsetSizes, Heap).ConfigureAwait(false);
        public async Task<byte[]> ReadBytesMainMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes) => await ReadMulti(offsetSizes, SysBot.Base.SwitchOffsetType.Main).ConfigureAwait(false);
        public async Task<byte[]> ReadBytesAbsoluteMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes) => await ReadMulti(offsetSizes, Absolute).ConfigureAwait(false);

        public async Task WriteBytesAsync(byte[] data, uint offset) => await Write(data, offset, Heap).ConfigureAwait(false);
        public async Task WriteBytesMainAsync(byte[] data, ulong offset) => await Write(data, offset, SysBot.Base.SwitchOffsetType.Main).ConfigureAwait(false);
        public async Task WriteBytesAbsoluteAsync(byte[] data, ulong offset) => await Write(data, offset, Absolute).ConfigureAwait(false);

        public async Task<ulong> GetMainNsoBaseAsync()
        {
            byte[] baseBytes = await ReadBytesFromCmdAsync(SwitchCommand.GetMainNsoBase(), sizeof(ulong)).ConfigureAwait(false);
            Array.Reverse(baseBytes, 0, 8);
            return BitConverter.ToUInt64(baseBytes, 0);
        }

        public async Task<ulong> GetHeapBaseAsync()
        {
            var baseBytes = await ReadBytesFromCmdAsync(SwitchCommand.GetHeapBase(), sizeof(ulong)).ConfigureAwait(false);
            Array.Reverse(baseBytes, 0, 8);
            return BitConverter.ToUInt64(baseBytes, 0);
        }

        public async Task<string> GetTitleID()
        {
            var bytes = await ReadRaw(SwitchCommand.GetTitleID(), 17).ConfigureAwait(false);
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        private async Task<byte[]> Read(ulong offset, int length, SwitchOffsetType type)
        {
            var method = type.GetReadMethod();
            if (length <= MaximumTransferSize)
            {
                var cmd = method(offset, length);
                return await ReadBytesFromCmdAsync(cmd, length).ConfigureAwait(false);
            }

            byte[] result = new byte[length];
            for (int i = 0; i < length; i += MaximumTransferSize)
            {
                int len = MaximumTransferSize;
                int delta = length - i;
                if (delta < MaximumTransferSize)
                    len = delta;

                var cmd = method(offset + (uint)i, len);
                var bytes = await ReadBytesFromCmdAsync(cmd, len).ConfigureAwait(false);
                bytes.CopyTo(result, i);
                await Task.Delay((MaximumTransferSize / DelayFactor) + BaseDelay).ConfigureAwait(false);
            }
            return result;
        }

        private async Task<byte[]> ReadMulti(IReadOnlyDictionary<ulong, int> offsetSizes, SwitchOffsetType type)
        {
            var method = type.GetReadMultiMethod();
            var cmd = method(offsetSizes);
            var totalSize = offsetSizes.Values.Sum();
            return await ReadBytesFromCmdAsync(cmd, totalSize).ConfigureAwait(false);
        }

        private async Task Write(byte[] data, ulong offset, SwitchOffsetType type)
        {
            var method = type.GetWriteMethod();
            if (data.Length <= MaximumTransferSize)
            {
                var cmd = method(offset, data);
                await SendAsync(cmd).ConfigureAwait(false);
                return;
            }
            int byteCount = data.Length;
            for (int i = 0; i < byteCount; i += MaximumTransferSize)
            {
                var slice =  data.SliceSafe(i, MaximumTransferSize);
                var cmd = method(offset + (uint)i, slice);
                await SendAsync(cmd).ConfigureAwait(false);
                await Task.Delay((MaximumTransferSize / DelayFactor) + BaseDelay).ConfigureAwait(false);
            }
        }

        public async Task<byte[]> ReadRaw(byte[] command, int length)
        {
            await SendAsync(command).ConfigureAwait(false);
            var buffer = new byte[length];
            var _ = Read(buffer);
            return buffer;
        }

        public async Task SendRaw(byte[] command)
        {
            await SendAsync(command).ConfigureAwait(false);
        }

        public async Task<byte[]> PointerPeek(int size, IEnumerable<long> jumps)
        {
            return await ReadBytesFromCmdAsync(SwitchCommand.PointerPeek(jumps, size), size).ConfigureAwait(false);
        }

        public async Task PointerPoke(byte[] data, IEnumerable<long> jumps)
        {
            await SendAsync(SwitchCommand.PointerPoke(jumps, data)).ConfigureAwait(false);
        }

        public async Task<ulong> PointerAll(IEnumerable<long> jumps)
        {
            var offsetBytes = await ReadBytesFromCmdAsync(SwitchCommand.PointerAll(jumps), sizeof(ulong)).ConfigureAwait(false);
            Array.Reverse(offsetBytes, 0, 8);
            return BitConverter.ToUInt64(offsetBytes, 0);
        }

        public async Task<ulong> PointerRelative(IEnumerable<long> jumps)
        {
            var offsetBytes = await ReadBytesFromCmdAsync(SwitchCommand.PointerRelative(jumps), sizeof(ulong)).ConfigureAwait(false);
            Array.Reverse(offsetBytes, 0, 8);
            return BitConverter.ToUInt64(offsetBytes, 0);
        }
        public async Task Click(SwitchButton b, int delay)
        {
            await SendAsync(SwitchCommand.Click(b, true)).ConfigureAwait(false);
            await Task.Delay(delay).ConfigureAwait(false);
        }

        public async Task PressAndHold(SwitchButton b, int hold, int delay)
        {
            await SendAsync(SwitchCommand.Hold(b, true)).ConfigureAwait(false);
            await Task.Delay(hold).ConfigureAwait(false);
            await SendAsync(SwitchCommand.Release(b, true)).ConfigureAwait(false);
            await Task.Delay(delay).ConfigureAwait(false);
        }

        public async Task DaisyChainCommands(int delay, IEnumerable<SwitchButton> buttons)
        {
            SwitchCommand.Configure(SwitchConfigureParameter.mainLoopSleepTime, delay, true);
            var commands = buttons.Select(z => SwitchCommand.Click(z, true)).ToArray();
            var chain = commands.SelectMany(x => x).ToArray();
            await SendAsync(chain).ConfigureAwait(false);
            SwitchCommand.Configure(SwitchConfigureParameter.mainLoopSleepTime, 0, true);
        }

        public async Task SetStick(SwitchStick stick, short x, short y, int delay)
        {
            var cmd = SwitchCommand.SetStick(stick, x, y, true);
            await SendAsync(cmd).ConfigureAwait(false);
            await Task.Delay(delay).ConfigureAwait(false);
        }

        public async Task DetachController()
        {
            await SendAsync(SwitchCommand.DetachController(true)).ConfigureAwait(false);
        }

        public async Task SetScreen(ScreenState state)
        {
            await SendAsync(SwitchCommand.SetScreen(state, true)).ConfigureAwait(false);
        }

        public async Task EchoCommands(bool value)
        {
            var cmd = SwitchCommand.Configure(SwitchConfigureParameter.echoCommands, value ? 1 : 0, true);
            await SendAsync(cmd).ConfigureAwait(false);
        }

        /// <inheritdoc cref="ReadUntilChanged(ulong,byte[],int,int,bool,bool,CancellationToken)"/>
        public async Task<bool> ReadUntilChanged(uint offset, byte[] comparison, int waitms, int waitInterval, bool match) =>
            await ReadUntilChanged(offset, comparison, waitms, waitInterval, match, false).ConfigureAwait(false);

        /// <summary>
        /// Reads an offset until it changes to either match or differ from the comparison value.
        /// </summary>
        /// <returns>If <see cref="match"/> is set to true, then the function returns true when the offset matches the given value.<br>Otherwise, it returns true when the offset no longer matches the given value.</br></returns>
        public async Task<bool> ReadUntilChanged(ulong offset, byte[] comparison, int waitms, int waitInterval, bool match, bool absolute)
        {
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                var task = absolute
                    ? ReadBytesAbsoluteAsync(offset, comparison.Length)
                    : ReadBytesAsync((uint)offset, comparison.Length);
                var result = await task.ConfigureAwait(false);
                if (match == result.SequenceEqual(comparison))
                    return true;

                await Task.Delay(waitInterval).ConfigureAwait(false);
            } while (sw.ElapsedMilliseconds < waitms);
            return false;
        }
    }
}
