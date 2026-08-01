using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class BuildingAccess
    {
        private bool _menuOpen;
        private int _index;
        private bool _inside;
        private Vector3 _returnPos;
        private string _currentBuilding = "";

        public void Toggle()
        {
            _menuOpen = !_menuOpen;
            _index = 0;
        }

        public void OnTick()
        {
            DrawEntranceMarkers();
            HandleProximityEnter();

            if (!_menuOpen) return;

            DrawMenu();

            if (Game.IsControlJustPressed(Control.FrontendUp) || Game.IsKeyPressed(Keys.Up))
            {
                _index--;
                if (_index < 0) _index = Config.Buildings.Count - 1;
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendDown) || Game.IsKeyPressed(Keys.Down))
            {
                _index++;
                if (_index >= Config.Buildings.Count) _index = 0;
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendAccept) || Game.IsKeyPressed(Keys.Enter))
            {
                TryEnter(Config.Buildings[_index], true);
                Script.Wait(150);
            }
            else if (Game.IsControlJustPressed(Control.FrontendCancel) || Game.IsKeyPressed(Keys.Back))
            {
                _menuOpen = false;
                Script.Wait(150);
            }
            else if (Game.IsKeyPressed(Keys.X) && _inside)
            {
                ExitBuilding();
                Script.Wait(200);
            }
        }

        private void DrawMenu()
        {
            float y = 0.18f;
            Ui.DrawText("~y~EDIFÍCIOS & ACESSOS", 0.40f, y, 0.40f);
            y += 0.035f;
            Ui.DrawText("Enter entrar | X sair do interior | Backspace fecha", 0.40f, y, 0.28f);
            y += 0.030f;

            for (int i = 0; i < Config.Buildings.Count; i++)
            {
                var b = Config.Buildings[i];
                var locked = Config.PlayerAccess < b.Required;
                var prefix = i == _index ? "~y~> " : "  ";
                var status = locked ? "~r~[LOCK]" : "~g~[OK]";
                Ui.DrawText($"{prefix}{b.Name} (nv.{(int)b.Required}) {status}", 0.40f, y, 0.30f);
                y += 0.028f;
                if (i == _index)
                {
                    Ui.DrawText($"   ~c~{b.Hint}", 0.40f, y, 0.26f);
                    y += 0.025f;
                }
            }
        }

        private void DrawEntranceMarkers()
        {
            foreach (var building in Config.Buildings)
            {
                var unlocked = Config.PlayerAccess >= building.Required;
                var color = unlocked ? new Color(80, 200, 120, 180) : new Color(220, 60, 60, 160);
                World.DrawMarker(
                    MarkerType.VerticalCylinder,
                    building.Entrance,
                    Vector3.Zero,
                    Vector3.Zero,
                    new Vector3(1.2f, 1.2f, 0.8f),
                    color
                );
            }

            if (_inside)
            {
                Ui.DrawText("~y~Pressione X para sair do edifício", 0.40f, 0.90f, 0.35f);
            }
        }

        private void HandleProximityEnter()
        {
            if (_inside) return;

            var ped = Game.Player.Character;
            foreach (var building in Config.Buildings)
            {
                if (ped.Position.DistanceTo(building.Entrance) < 1.8f)
                {
                    Ui.DrawText($"~w~[~y~E~w~] Entrar: {building.Name}", 0.40f, 0.88f, 0.35f);
                    if (Game.IsControlJustPressed(Control.Context))
                    {
                        TryEnter(building, false);
                    }
                }
            }
        }

        private void TryEnter(BuildingEntry building, bool fromMenu)
        {
            if (Config.PlayerAccess < building.Required)
            {
                Notification.PostTicker($"~r~Acesso negado.~w~ {building.Hint}", true);
                return;
            }

            var ped = Game.Player.Character;
            _returnPos = ped.Position;
            _inside = true;
            _currentBuilding = building.Name;
            ped.Position = building.Interior;
            Notification.PostTicker($"~g~Entrou:~w~ {building.Name}", false);
            _menuOpen = false;
        }

        private void ExitBuilding()
        {
            if (!_inside) return;
            Game.Player.Character.Position = _returnPos;
            Notification.PostTicker($"Saiu de ~y~{_currentBuilding}", false);
            _inside = false;
            _currentBuilding = "";
        }
    }
}
