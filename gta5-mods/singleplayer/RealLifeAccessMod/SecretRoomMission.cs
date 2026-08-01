using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace RealLifeAccessMod
{
    public class SecretRoomMission
    {
        private enum Stage
        {
            Idle,
            Started,
            GotKeycard,
            Hacked,
            InVault,
            Completed
        }

        private Stage _stage = Stage.Idle;
        private Blip _objectiveBlip;
        private Prop _keycardProp;
        private bool _rewardGiven;

        public void ToggleOrStart()
        {
            if (_stage == Stage.Idle || _stage == Stage.Completed)
            {
                Start();
            }
            else
            {
                Notification.PostTicker($"Missão em andamento: ~y~{_stage}", false);
                ShowCurrentObjective();
            }
        }

        public void OnTick()
        {
            DrawStartMarker();

            if (_stage == Stage.Idle || _stage == Stage.Completed) return;

            var ped = Game.Player.Character;

            switch (_stage)
            {
                case Stage.Started:
                    Screen.ShowSubtitle("Objetivo: roube o ~y~cartão classificado~w~ no FIB", 1);
                    if (ped.Position.DistanceTo(Config.MissionKeycard) < 2.0f)
                    {
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PICK_UP", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                        _stage = Stage.GotKeycard;
                        Notification.PostTicker("~g~Cartão classificado obtido!", true);
                        SetObjective(Config.MissionHackPoint, "Ponto de hack");
                    }
                    break;

                case Stage.GotKeycard:
                    Screen.ShowSubtitle("Objetivo: hackeie o painel ~b~IAA/FIB~w~", 1);
                    World.DrawMarker(MarkerType.VerticalCylinder, Config.MissionHackPoint, Vector3.Zero, Vector3.Zero, new Vector3(1.0f, 1.0f, 0.7f), Color.Cyan);
                    if (ped.Position.DistanceTo(Config.MissionHackPoint) < 1.8f)
                    {
                        Ui.DrawText("~w~[~y~E~w~] Hackear painel", 0.40f, 0.88f, 0.35f);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            Game.Player.Character.Task.StandStill(2500);
                            Script.Wait(2500);
                            _stage = Stage.Hacked;
                            Config.PlayerAccess = AccessLevel.Classified;
                            Notification.PostTicker("~g~Hack concluído!~w~ Acesso Classificado liberado.", true);
                            SetObjective(Config.MissionStart, "Entrada Secret Room");
                        }
                    }
                    break;

                case Stage.Hacked:
                    Screen.ShowSubtitle("Objetivo: entre na ~p~Secret Room~w~", 1);
                    if (ped.Position.DistanceTo(Config.MissionStart) < 2.0f)
                    {
                        Ui.DrawText("~w~[~y~E~w~] Entrar na Secret Room", 0.40f, 0.88f, 0.35f);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            ped.Position = Config.MissionVault;
                            _stage = Stage.InVault;
                            SetObjective(Config.MissionRewardSpawn, "Cofre");
                            Notification.PostTicker("~p~Secret Room aberta.", true);
                        }
                    }
                    break;

                case Stage.InVault:
                    Screen.ShowSubtitle("Objetivo: colete a ~g~recompensa do cofre", 1);
                    World.DrawMarker(MarkerType.VerticalCylinder, Config.MissionRewardSpawn, Vector3.Zero, Vector3.Zero, new Vector3(1.0f, 1.0f, 0.7f), Color.Gold);
                    if (ped.Position.DistanceTo(Config.MissionRewardSpawn) < 1.8f)
                    {
                        Ui.DrawText("~w~[~y~E~w~] Abrir cofre", 0.40f, 0.88f, 0.35f);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            Complete();
                        }
                    }
                    break;
            }
        }

        private void Start()
        {
            Cleanup();
            _stage = Stage.Started;
            _rewardGiven = false;
            Config.PlayerAccess = AccessLevel.Security; // progresso inicial da missão
            SetObjective(Config.MissionKeycard, "Cartão classificado");
            Notification.PostTicker("~y~MISSÃO: Secret Room~n~~w~Recupere o cartão, hackeie o painel e abra o vault.", true);
        }

        private void Complete()
        {
            if (!_rewardGiven)
            {
                Game.Player.Money += 250000;
                var model = new Model("kuruma2");
                model.Request(2000);
                if (model.IsLoaded)
                {
                    var rewardCar = World.CreateVehicle(model, Config.MissionRewardSpawn + new Vector3(3f, 0f, 0f), 90f);
                    if (rewardCar != null)
                    {
                        rewardCar.PlaceOnGround();
                        Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, rewardCar, "SECRET");
                    }
                    model.MarkAsNoLongerNeeded();
                }
                _rewardGiven = true;
            }

            _stage = Stage.Completed;
            Config.PlayerAccess = AccessLevel.Classified;
            ClearObjective();
            Notification.PostTicker("~g~MISSÃO CONCLUÍDA!~n~~w~+$250.000 | Kuruma blindado | Acesso Classificado permanente nesta sessão.", true);
        }

        private void DrawStartMarker()
        {
            if (_stage != Stage.Idle && _stage != Stage.Completed) return;

            World.DrawMarker(
                MarkerType.VerticalCylinder,
                Config.MissionStart,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(1.3f, 1.3f, 0.9f),
                Color.MediumPurple
            );

            if (Game.Player.Character.Position.DistanceTo(Config.MissionStart) < 2.0f)
            {
                Ui.DrawText("~w~[~y~F6~w~] Iniciar missão Secret Room", 0.38f, 0.88f, 0.35f);
            }
        }

        private void SetObjective(Vector3 pos, string name)
        {
            ClearObjective();
            _objectiveBlip = World.CreateBlip(pos);
            _objectiveBlip.Sprite = BlipSprite.Standard;
            _objectiveBlip.Color = BlipColor.Yellow;
            _objectiveBlip.Name = name;
            _objectiveBlip.ShowRoute = true;
        }

        private void ClearObjective()
        {
            if (_objectiveBlip != null && _objectiveBlip.Exists())
            {
                _objectiveBlip.Delete();
            }
            _objectiveBlip = null;
        }

        private void ShowCurrentObjective()
        {
            switch (_stage)
            {
                case Stage.Started: Notification.PostTicker("Vá até o FIB e pegue o cartão.", false); break;
                case Stage.GotKeycard: Notification.PostTicker("Hackeie o painel no FIB/IAA.", false); break;
                case Stage.Hacked: Notification.PostTicker("Entre na Secret Room.", false); break;
                case Stage.InVault: Notification.PostTicker("Abra o cofre.", false); break;
            }
        }

        public void Cleanup()
        {
            ClearObjective();
            if (_keycardProp != null && _keycardProp.Exists())
            {
                _keycardProp.Delete();
            }
            _keycardProp = null;
        }
    }
}
