using PixelGameAssets.Scripts.Actor.Enemies.Base;

namespace PixelGameAssets.Scripts.UI.Tag.Model
{
    /// <summary>
    /// for <see cref="InfoBar"/>
    /// </summary>
    public class InfoData
    {
        public Enemies.Level Level;
        public string Name;

        public InfoData(Enemies.Level level, string name)
        {
            Level = level;
            Name = name;
        }
    }
}