using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class GarageMenu
    {
        private bool _open;
        private int _categoryIndex;
        private int _itemIndex;
        private Vehicle _lastVehicle;

        public void Toggle()
        {
            _open = !_open;
            _itemIndex = 0;
            if (_open)
            {
                Notification.PostTicker("Garagem aberta. ~y~←/→~w~ categoria | ~y~↑/↓~w~ item | ~y~Enter~w~ spawn | ~y~Backspace~w~ fecha", false);
            }
        }

        public void OnTick()
        {
            if (!_open) return;

            var category = Config.Categories[_categoryIndex];
            var vehicles = Config.Vehicles.Where(v => v.Category == category).ToList();
            if (vehicles.Count == 0)
            {
                vehicles.Add(new VehicleEntry
                {
                    Label = "Nenhum veículo",
                    Model = "issi2",
                    Fallback = "issi2",
                    Category = category,
                    Access = AccessLevel.Classified
                });
            }

            if (_itemIndex >= vehicles.Count) _itemIndex = 0;

            DrawPanel(category, vehicles);

            if (Game.IsControlJustPressed(Control.FrontendUp) || Game.IsKeyPressed(Keys.Up))
            {
                _itemIndex--;
                if (_itemIndex < 0) _itemIndex = vehicles.Count - 1;
                WaitKey();
            }
            else if (Game.IsControlJustPressed(Control.FrontendDown) || Game.IsKeyPressed(Keys.Down))
            {
                _itemIndex++;
                if (_itemIndex >= vehicles.Count) _itemIndex = 0;
                WaitKey();
            }
            else if (Game.IsControlJustPressed(Control.FrontendLeft) || Game.IsKeyPressed(Keys.Left))
            {
                _categoryIndex--;
                if (_categoryIndex < 0) _categoryIndex = Config.Categories.Length - 1;
                _itemIndex = 0;
                WaitKey();
            }
            else if (Game.IsControlJustPressed(Control.FrontendRight) || Game.IsKeyPressed(Keys.Right))
            {
                _categoryIndex++;
                if (_categoryIndex >= Config.Categories.Length) _categoryIndex = 0;
                _itemIndex = 0;
                WaitKey();
            }
            else if (Game.IsControlJustPressed(Control.FrontendAccept) || Game.IsKeyPressed(Keys.Enter))
            {
                Spawn(vehicles[_itemIndex]);
                WaitKey();
            }
            else if (Game.IsControlJustPressed(Control.FrontendCancel) || Game.IsKeyPressed(Keys.Back))
            {
                _open = false;
                WaitKey();
            }
        }

        private void DrawPanel(string category, List<VehicleEntry> vehicles)
        {
            var lines = new List<string>
            {
                "~y~GARAGEM VIDA REAL",
                $"Categoria: ~b~{category}",
                $"Seu acesso: ~g~{(int)Config.PlayerAccess} ({Config.PlayerAccess})",
                "---------------------------"
            };

            for (int i = 0; i < vehicles.Count && i < 12; i++)
            {
                var v = vehicles[i];
                var locked = Config.PlayerAccess < v.Access;
                var marker = i == _itemIndex ? "~y~> " : "  ";
                var status = locked ? "~r~[LOCK]" : "~g~[OK]";
                lines.Add($"{marker}{v.Label} (nv.{(int)v.Access}) {status}");
            }

            float y = 0.18f;
            foreach (var line in lines)
            {
                Ui.DrawText(line, 0.70f, y, 0.35f);
                y += 0.028f;
            }
        }

        private void Spawn(VehicleEntry entry)
        {
            if (Config.PlayerAccess < entry.Access)
            {
                Notification.PostTicker($"~r~Acesso insuficiente.~w~ Precisa do nível {(int)entry.Access}.", false);
                return;
            }

            var modelName = ResolveModel(entry);
            var model = new Model(modelName);
            model.Request(3000);
            if (!model.IsLoaded)
            {
                Notification.PostTicker("~r~Falha ao carregar o modelo do veículo.", false);
                return;
            }

            if (_lastVehicle != null && _lastVehicle.Exists())
            {
                _lastVehicle.Delete();
            }

            var ped = Game.Player.Character;
            var pos = ped.Position + ped.ForwardVector * 3.5f;
            _lastVehicle = World.CreateVehicle(model, pos, ped.Heading);
            if (_lastVehicle != null)
            {
                _lastVehicle.PlaceOnGround();
                ped.SetIntoVehicle(_lastVehicle, VehicleSeat.Driver);
                Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, _lastVehicle, "REALIFE");
                Notification.PostTicker($"~g~Spawn:~w~ {entry.Label} (~b~{modelName}~w~)", false);
            }

            model.MarkAsNoLongerNeeded();
        }

        private string ResolveModel(VehicleEntry entry)
        {
            var addon = new Model(entry.Model);
            if (addon.IsValid && addon.IsVehicle) return entry.Model;
            return entry.Fallback;
        }

        private void WaitKey()
        {
            Script.Wait(150);
        }
    }
}
