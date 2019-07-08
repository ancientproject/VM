namespace rune.cmd.Internal
{
    using System;
    using System.Drawing;
    using System.IO;
    using Ancient.ProjectSystem;
    using etc;

    public abstract class WithProject
    {
        public bool Validate(string directory)
        {
            var projectFiles = Directory.GetFiles(directory, "*.rune.json");

            if (projectFiles.Length == 0)
            {
                Console.WriteLine($"{":fried_shrimp:".Emoji()} {"Couldn't".Nier().Color(Color.Red)} find a project to run. Ensure a project exists in {directory}.");
                return false;
            }
            if (projectFiles.Length > 1)
            {
                Console.WriteLine($"{":fried_shrimp:".Emoji()} {"Specify".Nier().Color(Color.Red)} which project file to use because this folder contains more than one project file..");
                return false;
            }
            return true;
        }
    }
}