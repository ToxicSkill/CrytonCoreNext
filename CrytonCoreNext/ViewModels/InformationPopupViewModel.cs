﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using System.Windows.Controls;
using System.Windows.Media;

namespace CrytonCoreNext.ViewModels
{
    public class InformationPopupViewModel : ViewModelBase
    {
        private bool _showPopup = false;

        private Color _defaultColor = EPopopColor.Information;

        public Color BackgroundColor { get; set; }

        public string InformationString { get; init; }

        public ScrollBarVisibility VerticalScrollBarVisbility { get; set; }

        public bool ShowPopup 
        {
            get => _showPopup;
            set
            { 
                _showPopup = value;
                OnPropertyChanged(nameof(ShowPopup)); 
            } 
        }

        public InformationPopupViewModel(string informationString = "", Color color = default)
        {
            if (Color.Equals(color, default))
            {
                color = _defaultColor;
            }

            BackgroundColor = color;
            InformationString = informationString;
            VerticalScrollBarVisbility = ScrollBarVisibility.Hidden;
        }
    }
}