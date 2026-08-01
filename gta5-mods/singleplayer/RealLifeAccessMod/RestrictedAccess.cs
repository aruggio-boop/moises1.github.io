using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class RestrictedAccess
    {
        private AccessZone _currentZone;

        public void OnTick()
        {
            var ped = Game.Player.Character;
            _currentZone = null;

            foreach (var zone in Config.Zones)
            {
                var dist = ped.Position.DistanceTo(zone.Position);
                if (dist <= zone.Radius)
                {
                    _currentZone = zone;
                    HandleZone(zone);
                    break;
                }
            }

            if (_currentZone != null)
            {
                var ok = Config.PlayerAccess >= _currentZone.Required;
                var color = ok ? "~g~AUTORIZADO" : "~r~RESTRITO";
                Ui.DrawText(
                    $"{color}~w~ | {_currentZone.Name} (nv.{(int)_currentZone.Required})",
                    0.30f,
                    0.05f,
                    0.35f
                );
            }
        }

        private void HandleZone(AccessZone zone)
        {
            bool allowed = Config.PlayerAccess >= zone.Required;

            if (allowed)
            {
                // Com acesso: remove stars e reduz hostilidade local
                if (zone.Required >= AccessLevel.Operations)
                {
                    Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
                    Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);
                }
            }
            else if (zone.Restricted)
            {
                // Sem acesso em zona restrita: alerta e wanted progressivo
                if (zone.Required >= AccessLevel.Operations)
                {
                    if (Game.Player.WantedLevel < 3)
                    {
                        Game.Player.WantedLevel = 3;
                    }
                    Screen.ShowSubtitle("~r~ÁREA RESTRITA~w~ — eleve seu acesso no menu F8", 1);
                }
                else if (zone.Required >= AccessLevel.Security)
                {
                    Screen.ShowSubtitle("~o~Acesso de segurança necessário~w~ (F8)", 1);
                }
                else
                {
                    Screen.ShowSubtitle("~o~Acesso VIP necessário~w~ (F8)", 1);
                }
            }
        }
    }
}
