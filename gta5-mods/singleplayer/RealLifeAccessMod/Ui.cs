using System.Drawing;
using GTA.UI;

namespace RealLifeAccessMod
{
    public static class Ui
    {
        public static void DrawText(string text, float x, float y, float scale = 0.35f)
        {
            var element = new TextElement(
                text,
                new PointF(x * 1920f, y * 1080f),
                scale * 48f,
                Color.White,
                Font.ChaletLondon,
                Alignment.Left
            )
            {
                Outline = true,
                Shadow = false
            };
            element.Draw();
        }

        public static void Notify(string message, bool blinking = false)
        {
            Notification.PostTicker(message, blinking);
        }
    }
}
