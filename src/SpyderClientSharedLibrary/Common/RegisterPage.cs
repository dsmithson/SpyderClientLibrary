﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class RegisterPage : PropertyChangedBase
    {
        private int pageIndex;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                if (pageIndex != value)
                {
                    pageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
