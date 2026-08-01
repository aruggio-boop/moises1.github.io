using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class Main : Script
    {
        private readonly GarageMenu _garage;
        private readonly AccessMenu _access;
        private readonly BuildingAccess _buildings;
        private readonly RestrictedAccess _restricted;
        private readonly SecretRoomMission _mission;

        public Main()
        {
            _garage = new GarageMenu();
            _access = new AccessMenu();
            _buildings = new BuildingAccess();
            _restricted = new RestrictedAccess();
            _mission = new SecretRoomMission();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAborted;

            Notification.PostTicker("~y~RealLife Access Mod~w~ carregado.~n~F7 Garagem | F8 Acessos | F9 Edifícios | F6 Missão", true);
        }

        private void OnTick(object sender, EventArgs e)
        {
            _garage.OnTick();
            _access.OnTick();
            _buildings.OnTick();
            _restricted.OnTick();
            _mission.OnTick();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                _garage.Toggle();
            }
            else if (e.KeyCode == Keys.F8)
            {
                _access.Toggle();
            }
            else if (e.KeyCode == Keys.F9)
            {
                _buildings.Toggle();
            }
            else if (e.KeyCode == Keys.F6)
            {
                _mission.ToggleOrStart();
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _mission.Cleanup();
            Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);
            Function.Call(Hash.SET_MAX_WANTED_LEVEL, 5);
        }
    }
}
