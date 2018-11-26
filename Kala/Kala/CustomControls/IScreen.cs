namespace Kala
{
    public interface IScreen
    {
        void SetBacklight(float factor);

        void SetScreenOrientation(string Screenorientation);

        void ScreenSaver(long screensaver);

        void SetFullScreen(bool fullscreen);
    }
}