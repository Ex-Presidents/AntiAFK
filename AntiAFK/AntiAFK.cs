using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Core.Plugins;
using SDG.Unturned;
using UnityEngine;
using Rocket.API.Collections;
using Rocket.Unturned.Player;

namespace AntiAFK
{
    public class AntiAFK : RocketPlugin<Configuration>
    {
        private Dictionary<SteamPlayer, DateTime> PlayerInfo = new Dictionary<SteamPlayer, DateTime>();

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "kick_reason", "AFK Timeout" }
        };

        void Update()
        {
            List<SteamPlayer> remove = new List<SteamPlayer>();
            List<SteamPlayer> mod = new List<SteamPlayer>(); // Fuck you .NET

            if (PlayerInfo.Count != Provider.clients.Count)
            {
                for(int i = 0; i < Provider.clients.Count; i++)
                    if (!PlayerInfo.ContainsKey(Provider.clients[i]) && !UnturnedPlayer.FromSteamPlayer(Provider.clients[i]).HasPermission("antiafk"))
                        PlayerInfo.Add(Provider.clients[i], DateTime.Now);
                foreach(SteamPlayer player in PlayerInfo.Keys)
                    if (!Provider.clients.Contains(player))
                        remove.Add(player);
            }

            foreach(SteamPlayer player in PlayerInfo.Keys)
            {
                if (player.player.movement.isMoving || player.player.stance.stance == EPlayerStance.DRIVING || Provider.pending.Contains(Provider.pending.Find(c => c.playerID == player.playerID)))
                    mod.Add(player);

                if((DateTime.Now - PlayerInfo[player]).TotalSeconds >= Configuration.Instance.SecondsUntilKick)
                {
                    Provider.kick(player.playerID.steamID, Translations.Instance["kick_reason"]);
                    remove.Add(player);
                }
            }
            while (remove.Count > 0)
            {
                PlayerInfo.Remove(remove[0]);
                remove.RemoveAt(0);
            }
            while(mod.Count > 0)
            {
                PlayerInfo[mod[0]] = DateTime.Now;
                mod.RemoveAt(0);
            }
        }
    }
}
