//From https://mindofai.github.io/Launching-Apps-thru-URI-with-Xamarin.Forms/
using System.Threading.Tasks;

namespace Kala
{
    public interface IAppLauncher
    {
        Task<bool> Launch(string stringUri);
    }
}
