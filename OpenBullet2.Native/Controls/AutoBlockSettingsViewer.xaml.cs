﻿using OpenBullet2.Native.Helpers;
using OpenBullet2.Native.ViewModels;
using RuriLib.Models.Blocks;
using RuriLib.Models.Blocks.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenBullet2.Native.Controls
{
    /// <summary>
    /// Interaction logic for AutoBlockSettingsViewer.xaml
    /// </summary>
    public partial class AutoBlockSettingsViewer : UserControl
    {
        private readonly AutoBlockSettingsViewerViewModel vm;

        public AutoBlockSettingsViewer(BlockViewModel blockVM)
        {
            if (blockVM.Block is not AutoBlockInstance)
            {
                throw new Exception("Wrong block type for this UC");
            }

            vm = new AutoBlockSettingsViewerViewModel(blockVM);
            DataContext = vm;

            InitializeComponent();
            CreateControls();
        }

        private void CreateControls()
        {
            foreach (var setting in vm.Block.Settings)
            {
                UserControl viewer = setting.Value.FixedSetting switch
                {
                    StringSetting => new StringSettingViewer { Setting = setting.Value },
                    IntSetting => new IntSettingViewer { Setting = setting.Value },
                    FloatSetting => new FloatSettingViewer { Setting = setting.Value },
                    EnumSetting => new EnumSettingViewer { Setting = setting.Value },
                    ByteArraySetting => new ByteArraySettingViewer { Setting = setting.Value },
                    BoolSetting => new BoolSettingViewer { Setting = setting.Value },
                    ListOfStringsSetting => new ListOfStringsSettingViewer { Setting = setting.Value },
                    DictionaryOfStringsSetting => new DictionaryOfStringsSettingViewer { Setting = setting.Value },
                    _ => null
                };

                if (viewer is not null)
                {
                    settingsPanel.Children.Add(viewer);
                }
            }

            foreach (var action in vm.Block.Descriptor.Actions)
            {
                var button = new Button
                {
                    Foreground = Brush.Get("ForegroundMain"),
                    Background = Brush.Get("BackgroundPrimary"),
                    Margin = new Thickness(0, 0, 0, 5),
                    Content = action.Name
                };

                // TODO: Call global refresh on everything that is displayed after action invocation to make it update changes
                button.Click += (sender, e) => action.Delegate.Invoke(vm.Block);
                actionsPanel.Children.Add(button);
            }

            // TODO: Add images
        }
    }

    public class AutoBlockSettingsViewerViewModel : BlockSettingsViewerViewModel
    {
        public AutoBlockInstance AutoBlock => Block as AutoBlockInstance;

        public bool SafeMode
        {
            get => AutoBlock.Safe;
            set
            {
                AutoBlock.Safe = value;
                OnPropertyChanged();
            }
        }

        public bool HasReturnValue => Block.Descriptor.ReturnType is not null;
        public string ReturnValueType => $"Output variable ({Block.Descriptor.ReturnType})";
        public bool HasActions => Block.Descriptor.Actions.Any();
        public bool HasImages => Block.Descriptor.Images.Any();

        public string OutputVariable
        {
            get => AutoBlock.OutputVariable;
            set
            {
                AutoBlock.OutputVariable = value;
                OnPropertyChanged();
            }
        }

        public bool IsCapture
        {
            get => AutoBlock.IsCapture;
            set
            {
                AutoBlock.IsCapture = value;
                OnPropertyChanged();
            }
        }

        public AutoBlockSettingsViewerViewModel(BlockViewModel block) : base(block)
        {
            
        }
    }

    public class BlockSettingsViewerViewModel : ViewModelBase
    {
        public BlockViewModel BlockVM { get; init; }
        public BlockInstance Block => BlockVM.Block;

        public string NameAndId => $"{BlockVM.Block.ReadableName} ({BlockVM.Block.Id})";

        public string Label
        {
            get => BlockVM.Label;
            set
            {
                BlockVM.Label = value;
                OnPropertyChanged();
            }
        }

        public BlockSettingsViewerViewModel(BlockViewModel block)
        {
            BlockVM = block;
        }
    }
}
