namespace CadDev.Extension.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RibbonSplitButtonAttribute(string ribbonTitle, string ribbonPanel, string splitRibbonName, string commandName, string description, string image = "", string largeImage = "") : Attribute
    {
        public string RibbonTabTitle { get; } = ribbonTitle;
        public string RibbonPanelName { get; } = ribbonPanel;
        public string SplitRibbonName { get; } = splitRibbonName;
        public string CommandName { get; } = commandName;
        public string Description { get; } = description;
        public string Image { get; } = image;
        public string LargeImage { get; } = largeImage;
    }
}