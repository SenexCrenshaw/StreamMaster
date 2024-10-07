namespace RemoveContollersAndHubs
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            DirectoryInfo di = new("../../../../StreamMaster.Application/");
            FileInfo[] files = di.GetFiles("ControllerAndHub.cs", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (file.FullName.Contains("StreamGroups"))
                {
                    continue;
                }
                File.Delete(file.FullName);
            }
            files = di.GetFiles("IControllerAndHub.cs", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (file.FullName.Contains("StreamGroups"))
                {
                    continue;
                }
                File.Delete(file.FullName);
            }
        }
    }
}
