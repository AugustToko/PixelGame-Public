namespace PixelGameAssets.Scripts.UI.MusicSystem
{
    /// <summary>
    /// 歌曲
    /// TODO: 完善
    /// </summary>
    public class Song
    {
        public string musicName;

        public string musicArtist;

        public string musicCoverPath;

        public Song(string musicName, string musicArtist, string musicCoverPath)
        {
            this.musicName = musicName;
            this.musicArtist = musicArtist;
            this.musicCoverPath = musicCoverPath;
        }
    }
}