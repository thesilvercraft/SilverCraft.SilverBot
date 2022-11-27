namespace SilverBot.SysAdminModule
{
    internal class ScoopPackage : IPackage
    {
        public ScoopPackage(string name, string version)
        {
            Name = name;
            Version = version;
        }
        public ScoopPackage(string name, string version, string newversion):this(name,version)
        {
            NewVersion = newversion;
        }
        public ScoopPackage(string name, string version, string bucket, string date, string time)
        {
            Name = name;
            Version = version;
            Bucket = bucket;
            Date = date;
            Time = time;
        }
        public string Date { get; }
        public string Time { get; }

        public string Name { get; }

        public string Version { get; }
        public string? NewVersion { get; }

        public string Bucket { get; }
        public string? Source => Bucket;

        public string? Description => null;

        public string? FullDescription => throw new NotImplementedException();

    }
}