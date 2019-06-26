namespace Ancient.ProjectSystem
{
    public class AncientProject
    {
        public string version { get; set; }
        /// <summary>
        /// <see cref="string"/> or <see cref="AncientAuthor"/>
        /// </summary>
        public object author { get; set; }
    }

    public class AncientAuthor
    {
        public string name;
        public string email;
    }
    public class AncientElement
    {

    }
}
