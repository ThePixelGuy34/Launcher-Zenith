using DiscordRPC;
using DiscordRPC.Logging;
using System;
using UML.Services;

namespace UML.Class
{
    public class DiscordRPCManager
    {
        private static DiscordRpcClient _client;
        private const string ClientId = "1339093506546733117";
        private const string LargeImageKey = "zenith";
        private const string SmallImageKey = "";
        private const string DiscordInviteLink = "https://discord.gg/GGZ8D8NaPk";

        static DiscordRPCManager()
        {
            _client?.Dispose();
            _client = new DiscordRpcClient(ClientId);
            _client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            _client.OnReady += (sender, e) => {
                Logger.Log("DiscordRPC loaded!");
            };
            _client.OnError += (sender, e) => {
                Logger.Log($"DiscordRPC Error: {e.Code} - RPC-0001");
            };

            bool initialized = _client.Initialize();

            if (!initialized)
            {
                Logger.Log("Failed to initialize Discord RPC client. - RPC-0002");
            }
        }

        public static void SetPresence(string state, string details)
        {
            if (_client == null || !_client.IsInitialized)
            {
                Logger.Log("Discord RPC client not initialized when trying to set presence. - RPC-0003");
                return;
            }

            try
            {
                var presence = new RichPresence
                {
                    State = state,
                    Details = details,
                    Timestamps = Timestamps.Now,
                    Assets = new Assets
                    {
                        LargeImageKey = LargeImageKey,
                        LargeImageText = "Zenith Launcher",
                        SmallImageKey = SmallImageKey,
                        SmallImageText = "Hi cro"
                    },
                    Buttons = new DiscordRPC.Button[]
                    {
                        new DiscordRPC.Button
                        {
                            Label = "Join Discord Server!",
                            Url = DiscordInviteLink
                        }
                    }
                };

                _client.SetPresence(presence);
                Console.WriteLine("Rich presence set successfully.");
            }
            catch
            {
                Console.WriteLine($"Error setting Rich Presence - RPC-0004");
            }
        }

        public static void Dispose()
        {
            if (_client != null && _client.IsInitialized)
            {
                _client.ClearPresence();
                _client.Dispose();
                _client = null;
            }
        }
    }
}