namespace GodsEye.WEB.Helpers
{
    public class NavMenuItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class NavMenuGroup
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        // Se tiver filhos => menu com níveis
        public List<NavMenuItem> Items { get; set; } = new();

        // Se não tiver filhos => link direto
        public string Url { get; set; }

        public bool HasChildren => Items.Any();
    }
}
