using CardsTools.Data.Models;
using CardsTools.Data.Tools;

using Newtonsoft.Json;

namespace CardsTools.Data.Managers
{
    internal abstract class Keeper
    {

        public virtual void SaveDeskCardToStorage(DeskOfCards? desk)
        {
            File.WriteAllText(Path.Combine(FileHelper.GetStoragePath(), $"{desk?.Name}.json"), JsonConvert.SerializeObject(desk));
#pragma warning disable CS8604
            File.WriteAllText(Path.Combine(FileHelper.GetStorageBackupPath(desk?.Name), $"{desk.Name}_{DateTime.Now.ToFileTime()}.json"), JsonConvert.SerializeObject(desk));
#pragma warning restore CS8604
        }
        public virtual DeskOfCards? OpenDeskCardToStorage(string path)
        {
            return JsonConvert.DeserializeObject<DeskOfCards>(File.ReadAllText(path));
        }
    }
}
