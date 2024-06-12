using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using CadDev.Extension.CustomAttribute;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using Orientation = System.Windows.Controls.Orientation;
using RibbonButton = Autodesk.Windows.RibbonButton;
using RibbonControl = Autodesk.Windows.RibbonControl;
using RibbonSplitButton = Autodesk.Windows.RibbonSplitButton;
using RibbonTab = Autodesk.Windows.RibbonTab;

namespace CadDev.Extension
{
    public abstract class ExtensionApplication : IExtensionApplication
    {
        private Assembly Assembly { get; set; }
        private string ProjectName { get; set; }
        #region
        public void Initialize()
        {
            OnStartup();
        }
        public void Terminate()
        {
            OnShutdown();
        }
        public abstract void OnStartup();

        public virtual void OnShutdown()
        {
        }
        #endregion

        [CommandMethod("Init_HC_Tools")]
        public void Init()
        {
            Assembly = Assembly.GetExecutingAssembly();
            ProjectName = Assembly.GetName().Name;
            InitializeTab();
        }
        private void InitializeTab()
        {
            Type[] allTypes = Assembly.GetTypes();

            foreach (var type in allTypes)
            {
                MethodInfo[] methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if ((RibbonButtonAttribute)Attribute.GetCustomAttribute(method, typeof(RibbonButtonAttribute)) is RibbonButtonAttribute attribute1)
                    {
                        ConstructorInfo commandConstructor = type.GetConstructor(Type.EmptyTypes);
                        Action commandAction = (Action)Delegate.CreateDelegate(typeof(Action), commandConstructor.Invoke(null), type.GetMethod(method.Name));

                        // Get the current ribbon control
                        RibbonControl ribbonControl = ComponentManager.Ribbon;
                        //create RibbonControl
                        if (ribbonControl != null)
                        {
                            RibbonTab rtab = ribbonControl.FindTab(attribute1.RibbonTabTitle);
                            if (rtab is not null)
                            {
                                List<RibbonPanel> ribbonPanels = new List<RibbonPanel>();
                                foreach (RibbonPanel panel in rtab.Panels)
                                {
                                    if (panel.Source.AutomationName == attribute1.RibbonPanelName)
                                    {
                                        AddRibbonButtonWithPanel(panel.Source, attribute1.RibbonPanelName, attribute1.CommandName, attribute1.Description,
                                                        attribute1.Image, attribute1.LargeImage, commandAction);
                                        break;
                                    }
                                    else
                                    {
                                        RibbonPanel ribbonPanel = AddRibbonButtonWithOutPanel(attribute1.RibbonPanelName, attribute1.CommandName, attribute1.Description,
                                                        attribute1.Image, attribute1.LargeImage, commandAction);
                                        ribbonPanels.Add(ribbonPanel);
                                        break;
                                    }
                                }
                                ribbonPanels.ForEach(rib => rtab.Panels.Add(rib));
                            }
                            else
                            {
                                rtab = new RibbonTab();
                                rtab.Title = attribute1.RibbonTabTitle;
                                rtab.Id = attribute1.RibbonTabTitle;
                                ribbonControl.Tabs.Add(rtab);
                                RibbonPanel ribbonPanel = AddRibbonButtonWithOutPanel(attribute1.RibbonPanelName, attribute1.CommandName, attribute1.Description,
                                                                            attribute1.Image, attribute1.LargeImage, commandAction);
                                rtab.Panels.Add(ribbonPanel);
                            }
                        }
                    }
                    if ((RibbonSplitButtonAttribute)Attribute.GetCustomAttribute(method, typeof(RibbonSplitButtonAttribute)) is RibbonSplitButtonAttribute attribute2)
                    {
                        ConstructorInfo commandConstructor = type.GetConstructor(Type.EmptyTypes);
                        Action commandAction = (Action)Delegate.CreateDelegate(typeof(Action), commandConstructor.Invoke(null), type.GetMethod(method.Name));

                        // Get the current ribbon control
                        RibbonControl ribbonControl = ComponentManager.Ribbon;
                        //create RibbonControl
                        if (ribbonControl != null)
                        {
                            RibbonTab rtab = ribbonControl.FindTab(attribute2.RibbonTabTitle);
                            if (rtab is not null)
                            {
                                List<RibbonPanel> ribbonPanels = new List<RibbonPanel>();
                                foreach (RibbonPanel panel in rtab.Panels)
                                {
                                    if (panel.Source.AutomationName == attribute2.RibbonPanelName)
                                    {
                                        foreach (var item in panel.Source.Items)
                                        {
                                            if (item is RibbonSplitButton ribbonSplitButton && item.AutomationName == attribute2.SplitRibbonName)
                                            {
                                                AddRibbonSplitButtonWithPanel(panel.Source, ribbonSplitButton, attribute2.CommandName,
                                                                              attribute2.Description, attribute2.Image, attribute2.LargeImage, commandAction);
                                            }
                                            break;
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        RibbonPanel ribbonPanel = AddRibbonSplitButtonWithOutPanel(attribute2.RibbonPanelName, attribute2.SplitRibbonName, attribute2.CommandName, attribute2.Description,
                                                        attribute2.Image, attribute2.LargeImage, commandAction);
                                        ribbonPanels.Add(ribbonPanel);
                                        break;
                                    }
                                }
                                ribbonPanels.ForEach(rib => rtab.Panels.Add(rib));
                            }
                            else
                            {
                                rtab = new RibbonTab();
                                rtab.Title = attribute2.RibbonTabTitle;
                                rtab.Id = attribute2.RibbonTabTitle;
                                ribbonControl.Tabs.Add(rtab);
                                RibbonPanel ribbonPanel = AddRibbonSplitButtonWithOutPanel(attribute2.RibbonPanelName, attribute2.SplitRibbonName, attribute2.CommandName, attribute2.Description, attribute2.Image, attribute2.LargeImage, commandAction);
                                rtab.Panels.Add(ribbonPanel);
                            }
                        }
                    }
                    if ((RibbonStackButtonAttribute)Attribute.GetCustomAttribute(method, typeof(RibbonStackButtonAttribute)) is RibbonStackButtonAttribute attribute3)
                    {
                        ConstructorInfo commandConstructor = type.GetConstructor(Type.EmptyTypes);
                        Action commandAction = (Action)Delegate.CreateDelegate(typeof(Action), commandConstructor.Invoke(null), type.GetMethod(method.Name));

                        // Get the current ribbon control
                        RibbonControl ribbonControl = ComponentManager.Ribbon;
                        //create RibbonControl
                        if (ribbonControl != null)
                        {
                            RibbonTab rtab = ribbonControl.FindTab(attribute3.RibbonTabTitle);
                            if (rtab is not null)
                            {
                                List<RibbonPanel> ribbonPanels = new List<RibbonPanel>();
                                foreach (RibbonPanel panel in rtab.Panels)
                                {
                                    if (panel.Source.AutomationName == attribute3.RibbonPanelName)
                                    {
                                        AddRibbonStackButtonWithPanel(panel.Source, attribute3.RowpanelName, attribute3.CommandName, attribute3.Description,
                                                        attribute3.Image, attribute3.LargeImage, commandAction);
                                        break;
                                    }
                                    else
                                    {
                                        RibbonPanel ribbonPanel = AddRibbonStackButtonWithOutPanel(attribute3.RibbonPanelName, attribute3.RowpanelName, attribute3.CommandName, attribute3.Description,
                                                        attribute3.Image, attribute3.LargeImage, commandAction);
                                        ribbonPanels.Add(ribbonPanel);
                                        break;
                                    }
                                }
                                ribbonPanels.ForEach(rib => rtab.Panels.Add(rib));
                            }
                            else
                            {
                                rtab = new RibbonTab();
                                rtab.Title = attribute3.RibbonTabTitle;
                                rtab.Id = attribute3.RibbonTabTitle;
                                ribbonControl.Tabs.Add(rtab);
                                RibbonPanel ribbonPanel = AddRibbonStackButtonWithOutPanel(attribute3.RibbonPanelName, attribute3.RowpanelName, attribute3.CommandName, attribute3.Description,
                                                                            attribute3.Image, attribute3.LargeImage, commandAction);
                                rtab.Panels.Add(ribbonPanel);
                            }
                        }
                    }
                    if ((RibbonToggleButtonAttribute)Attribute.GetCustomAttribute(method, typeof(RibbonToggleButtonAttribute)) is RibbonToggleButtonAttribute attribute4)
                    {
                        ConstructorInfo commandConstructor = type.GetConstructor(Type.EmptyTypes);
                        Action commandAction = (Action)Delegate.CreateDelegate(typeof(Action), commandConstructor.Invoke(null), type.GetMethod(method.Name));

                        // Get the current ribbon control
                        RibbonControl ribbonControl = ComponentManager.Ribbon;
                        //create RibbonControl
                        if (ribbonControl != null)
                        {
                            RibbonTab rtab = ribbonControl.FindTab(attribute4.RibbonTabTitle);
                            if (rtab is not null)
                            {
                                List<RibbonPanel> ribbonPanels = new List<RibbonPanel>();
                                foreach (RibbonPanel panel in rtab.Panels)
                                {
                                    if (panel.Source.AutomationName == attribute4.RibbonPanelName)
                                    {
                                        AddRibbonToggleButtonWithPanel(panel.Source, attribute4.RibbonPanelName, attribute4.CommandName, attribute4.Description,
                                                        attribute4.Image, attribute4.LargeImage, commandAction);
                                        break;
                                    }
                                    else
                                    {
                                        RibbonPanel ribbonPanel = AddRibbonToggleButtonWithOutPanel(attribute4.RibbonPanelName, attribute4.CommandName, attribute4.Description,
                                                        attribute4.Image, attribute4.LargeImage, commandAction);
                                        ribbonPanels.Add(ribbonPanel);
                                        break;
                                    }
                                }
                                ribbonPanels.ForEach(rib => rtab.Panels.Add(rib));
                            }
                            else
                            {
                                rtab = new RibbonTab();
                                rtab.Title = attribute4.RibbonTabTitle;
                                rtab.Id = attribute4.RibbonTabTitle;
                                ribbonControl.Tabs.Add(rtab);
                                RibbonPanel ribbonPanel = AddRibbonToggleButtonWithOutPanel(attribute4.RibbonPanelName, attribute4.CommandName, attribute4.Description,
                                                                            attribute4.Image, attribute4.LargeImage, commandAction);
                                rtab.Panels.Add(ribbonPanel);
                            }
                        }
                    }
                }
            }
        }
        //Ribbon button
        private RibbonPanel AddRibbonButtonWithOutPanel(string ribbonPanel, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonPanelSource srcPanel = new RibbonPanelSource();
            srcPanel.Title = ribbonPanel;

            RibbonPanel Panel = new RibbonPanel();
            Panel.Source = srcPanel;

            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;
            RibbonButton btnPythonShell = new RibbonButton
            {
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Name = commandName,
                ShowText = true,
                Text = commandName,
                Description = description,
                Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng(),
                LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng(),
                CommandHandler = new RelayCommand(action)
            };
            srcPanel.Items.Add(btnPythonShell);
            return Panel;
        }
        private void AddRibbonButtonWithPanel(RibbonPanelSource rps, string ribbonPanel, string commandName, string description, string image, string largeImage, Action action)
        {
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;

            RibbonButton btnPythonShell = new RibbonButton
            {
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Name = commandName,
                ShowText = true,
                Text = commandName,
                Description = description,
                Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng(),
                LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng(),
                CommandHandler = new RelayCommand(action)
            };
            rps.Items.Add(btnPythonShell);
        }
        //RibbonSplitButton
        private RibbonPanel AddRibbonSplitButtonWithOutPanel(string ribbonPanel, string splitRibbonName, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonPanelSource srcPanel = new RibbonPanelSource();
            srcPanel.Title = ribbonPanel;

            RibbonPanel Panel = new RibbonPanel();
            Panel.Source = srcPanel;

            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;
            RibbonButton ribbonButton = new RibbonButton
            {
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Standard,
                Name = commandName,
                ShowText = true,
                Text = commandName,
                Description = description,
                Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng(),
                LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng(),
                CommandHandler = new RelayCommand(action)
            };

            RibbonSplitButton ribSplitButton = new RibbonSplitButton();
            ribSplitButton.Name = splitRibbonName;
            ribSplitButton.Text = splitRibbonName;
            ribSplitButton.ShowText = true;
            ribSplitButton.IsSplit = true;
            ribSplitButton.Size = RibbonItemSize.Large;
            ribSplitButton.IsSynchronizedWithCurrentItem = true;
            ribSplitButton.Items.Add(ribbonButton);

            srcPanel.Items.Add(ribSplitButton);
            return Panel;
        }
        private void AddRibbonSplitButtonWithPanel(RibbonPanelSource rps, RibbonSplitButton ribSplitButton, string commandName, string description, string image, string largeImage, Action action)
        {
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;

            RibbonButton ribbonButton = new RibbonButton
            {
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Standard,
                Name = commandName,
                ShowText = true,
                Text = commandName,
                Description = description,
                Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng(),
                LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng(),
                CommandHandler = new RelayCommand(action)
            };

            ribSplitButton.Items.Add(ribbonButton);
        }
        //Stack Button
        private RibbonPanel AddRibbonStackButtonWithOutPanel(string ribbonPanel, string rowpanelName, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonPanelSource srcPanel = new RibbonPanelSource();
            srcPanel.Title = ribbonPanel;
            RibbonPanel Panel = new RibbonPanel();
            Panel.Source = srcPanel;

            RibbonRowPanel rowPanel = new RibbonRowPanel();
            rowPanel.Name = rowpanelName;
            rowPanel.Id = rowpanelName;
            srcPanel.Items.Add(rowPanel);

            RibbonRowBreak ribbonRowBreak = new RibbonRowBreak();
            ribbonRowBreak.AllowInStatusBar = false;
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;
            RibbonButton ribbonButton = new RibbonButton();
            ribbonButton.Orientation = Orientation.Horizontal;
            ribbonButton.AllowInStatusBar = true;
            ribbonButton.Size = RibbonItemSize.Standard;
            ribbonButton.Name = commandName;
            ribbonButton.ShowText = true;
            ribbonButton.Text = commandName;
            ribbonButton.Description = description;
            ribbonButton.ResizeStyle = RibbonItemResizeStyles.ResizeWidth;
            ribbonButton.Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng();
            ribbonButton.LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng();
            ribbonButton.CommandHandler = new RelayCommand(action);

            rowPanel.Items.Add(ribbonButton);
            rowPanel.Items.Add(ribbonRowBreak);

            return Panel;
        }
        private void AddRibbonStackButtonWithPanel(RibbonPanelSource rps, string rowpanelName, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonRowPanel rowPanel = null;
            foreach (var item in rps.Items)
            {
                if (item is RibbonRowPanel ribbonRowPanel && ribbonRowPanel.Id.Equals(rowpanelName))
                    rowPanel = ribbonRowPanel;
            }
            if (rowPanel is null)
            {
                rowPanel = new RibbonRowPanel();
                rowPanel.Name = rowpanelName;
                rowPanel.Id = rowpanelName;
                rps.Items.Add(rowPanel);
            }

            RibbonRowBreak ribbonRowBreak = new RibbonRowBreak();
            ribbonRowBreak.AllowInStatusBar = false;
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;
            RibbonButton ribbonButton = new RibbonButton();
            ribbonButton.Orientation = Orientation.Horizontal;
            ribbonButton.AllowInStatusBar = true;
            ribbonButton.Size = RibbonItemSize.Standard;
            ribbonButton.Name = commandName;
            ribbonButton.ShowText = true;
            ribbonButton.Text = commandName;
            ribbonButton.Description = description;
            ribbonButton.ResizeStyle = RibbonItemResizeStyles.ResizeWidth;
            ribbonButton.Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng();
            ribbonButton.LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng();
            ribbonButton.CommandHandler = new RelayCommand(action);

            rowPanel.Items.Add(ribbonButton);
            rowPanel.Items.Add(ribbonRowBreak);
        }
        //Toggle Button
        private RibbonPanel AddRibbonToggleButtonWithOutPanel(string ribbonPanel, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonPanelSource srcPanel = new RibbonPanelSource();
            srcPanel.Title = ribbonPanel;
            RibbonPanel Panel = new RibbonPanel();
            Panel.Source = srcPanel;
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;

            RibbonToggleButton button = new RibbonToggleButton();
            button.Text = commandName;
            button.Size = RibbonItemSize.Standard;
            button.Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng();
            button.LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng();
            button.ShowText = true;
            button.CommandParameter = "";
            button.CommandHandler = new RelayCommand(action);
            srcPanel.Items.Add(button);
            return Panel;
        }
        private void AddRibbonToggleButtonWithPanel(RibbonPanelSource rps, string ribbonPanel, string commandName, string description, string image, string largeImage, Action action)
        {
            RibbonPanel Panel = new RibbonPanel();
            Panel.Source = rps;
            image = string.IsNullOrEmpty(image) ? "logo_16.png" : image;
            largeImage = string.IsNullOrEmpty(largeImage) ? "logo_32.png" : largeImage;

            RibbonToggleButton button = new RibbonToggleButton();
            button.Text = commandName;
            button.Size = RibbonItemSize.Large;
            button.Image = ($"{ProjectName}.Resources.{image}").GetEmbeddedPng();
            button.LargeImage = ($"{ProjectName}.Resources.{largeImage}").GetEmbeddedPng();
            button.ShowText = true;
            button.CommandParameter = "";
            button.CommandHandler = new RelayCommand(action);
            rps.Items.Add(button);
        }
    }
}
