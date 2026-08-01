using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class AccessMenu
    {
        private bool _open;
        private int _index;

        private readonly string[] _options =
        {
            "Definir acesso: Público (0)",
            "Definir acesso: VIP (1)",
            "Definir acesso: Segurança (2)",
            "Definir acesso: Operações (3)",
            "Definir acesso: Classificado (4)",
            "Limpar procurado + ignorar policia militar",
            "Ativar modo acesso total (wanted 0 + invulnerável zonas)",
            "Desativar modo acesso total",
            "Mostrar zonas restritas no mapa",
            "Fechar"
        };

        private bool _totalAccessMode;
        private readonly List<Blip> _zoneBlips = new List<Blip>();

        public void Toggle()
        {
            _open = !_open;
            _index = 0;
        }

        public void OnTick()
        {
            if (_totalAccessMode)
            {
                Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);
                Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
                // Reduz agressividade em Fort Zancudo quando autorizado
                if (Config.PlayerAccess >= AccessLevel.Operations)
                {
                    Function.Call(Hash.SET_PLAYER_CAN_BE_HASSLED_BY_GANGS, Game.Player, false);
                }
            }

            if (!_open) return;

            Draw();

            if (Game.IsControlJustPressed(Control.FrontendUp) || Game.IsKeyPressed(Keys.Up))
            {
                _index--;
                if (_index < 0) _index = _options.Length - 1;
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendDown) || Game.IsKeyPressed(Keys.Down))
            {
                _index++;
                if (_index >= _options.Length) _index = 0;
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendAccept) || Game.IsKeyPressed(Keys.Enter))
            {
                Select();
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendCancel) || Game.IsKeyPressed(Keys.Back))
            {
                _open = false;
                Script.Wait(150);
            }
        }

        private void Draw()
        {
            float y = 0.18f;
            Ui.DrawText("~y~MENU DE ACESSOS", 0.08f, y, 0.40f);
            y += 0.035f;
            Ui.DrawText($"Atual: ~g~{(int)Config.PlayerAccess} ({Config.PlayerAccess})", 0.08f, y, 0.32f);
            y += 0.030f;

            for (int i = 0; i < _options.Length; i++)
            {
                var prefix = i == _index ? "~y~> " : "  ";
                Ui.DrawText(prefix + _options[i], 0.08f, y, 0.30f);
                y += 0.028f;
            }
        }

        private void Select()
        {
            switch (_index)
            {
                case 0: SetAccess(AccessLevel.Public); break;
                case 1: SetAccess(AccessLevel.Vip); break;
                case 2: SetAccess(AccessLevel.Security); break;
                case 3: SetAccess(AccessLevel.Operations); break;
                case 4: SetAccess(AccessLevel.Classified); break;
                case 5:
                    Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
                    Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);
                    Notification.PostTicker("~g~Procurado limpo. Polícia militar ignorada temporariamente.", false);
                    break;
                case 6:
                    _totalAccessMode = true;
                    Config.PlayerAccess = AccessLevel.Classified;
                    Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);
                    Notification.PostTicker("~g~Modo acesso total ATIVO~w~ (nível 4).", false);
                    break;
                case 7:
                    _totalAccessMode = false;
                    Function.Call(Hash.SET_MAX_WANTED_LEVEL, 5);
                    Notification.PostTicker("~o~Modo acesso total DESATIVADO.", false);
                    break;
                case 8:
                    ToggleZoneBlips();
                    break;
                case 9:
                    _open = false;
                    break;
            }
        }

        private void SetAccess(AccessLevel level)
        {
            Config.PlayerAccess = level;
            Notification.PostTicker($"Acesso definido para ~y~{(int)level} ({level})", false);
        }

        private void ToggleZoneBlips()
        {
            if (_zoneBlips.Count > 0)
            {
                foreach (var blip in _zoneBlips)
                {
                    if (blip != null && blip.Exists()) blip.Delete();
                }
                _zoneBlips.Clear();
                Notification.PostTicker("Marcadores de zonas removidos.", false);
                return;
            }

            foreach (var zone in Config.Zones)
            {
                var blip = World.CreateBlip(zone.Position);
                blip.Sprite = BlipSprite.Garage;
                blip.Color = zone.Required >= AccessLevel.Operations ? BlipColor.Red : BlipColor.Yellow;
                blip.Name = zone.Name;
                blip.IsShortRange = false;
                _zoneBlips.Add(blip);
            }

            Notification.PostTicker("~g~Zonas restritas marcadas no mapa.", false);
        }
    }
}
