using Spyder.Client.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Client.Common
{
    public class Register : PropertyChangedBase, Spyder.Client.Common.IRegister
    {
        /// <summary>
        /// Type of Register instance
        /// </summary>
        private RegisterType type = RegisterType.Source;
        public virtual RegisterType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color registerColor;
        public Color RegisterColor
        {
            get { return registerColor; }
            set
            {
                if (registerColor != value)
                {
                    registerColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool registerColorDefined;
        public bool RegisterColorDefined
        {
            get { return registerColorDefined; }
            set
            {
                if (registerColorDefined != value)
                {
                    registerColorDefined = value;
                    OnPropertyChanged();
                }
            }
        }

        private int lookupID;
        public int LookupID
        {
            get { return lookupID; }
            set
            {
                if (lookupID != value)
                {
                    lookupID = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Item name
        /// </summary>
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
        
        /// <summary>
        /// Register location Index
        /// </summary>
        private int registerID;
        public int RegisterID
        {
            get { return registerID; }
            set
            {
                if (registerID != value)
                {
                    int lastPageIndex = registerID % 1000;
                    int newPageIndex = value % 1000;

                    registerID = value;
                    OnPropertyChanged();

                    if(lastPageIndex != newPageIndex)
                        OnPropertyChanged("PageIndex");
                }
            }
        }
        
        /// <summary>
        /// Page index for item
        /// </summary>
        public int PageIndex { get { return RegisterID / 1000; } }

        public virtual void CopyFrom(IRegister copyFrom)
        {
            Copy(copyFrom, this);
        }

        public static void Copy(IRegister source, IRegister destination)
        {
            destination.Name = source.Name;
            destination.RegisterID = source.RegisterID;
            destination.RegisterColor = source.RegisterColor;
            destination.RegisterColorDefined = source.RegisterColorDefined;
            destination.LookupID = source.LookupID;
            destination.Type = source.Type;
        }
    }
}
