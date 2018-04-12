namespace Kala
{
    public interface IPlatformInfo
    {
        string GetModel();

        string GetVersion();

        string GetWifiMacAddress();
    }
}