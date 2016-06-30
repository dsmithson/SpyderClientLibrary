using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Client.Common
{
    public class Register : PropertyChangedBase, Spyder.Client.Common.IRegister, IEquatable<Register>
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

        public bool Equals(Register other)
        {
            if(other == null)
                return false;
            else if(this.type != other.type)
                return false;
            else if(this.name != other.name)
                return false;
            else if(this.registerID != other.registerID)
                return false;
            else if(this.lookupID != other.lookupID)
                return false;
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
                return false;

            var register = obj as Register;
            if (register == null)
                return false;
            else
                return this.Equals(register);
        }

        public override int GetHashCode()
        {
            return (((int)Type * 251) + RegisterID) * 251 + LookupID;
        }

        public static bool operator ==(Register register1, Register register2)
        {
            if (((object)register1 == null) || ((object)register2) == null)
                return object.Equals(register1, register2);

            return register1.Equals(register2);
        }

        public static bool operator !=(Register register1, Register register2)
        {
            if (((object)register1 == null) || ((object)register2) == null)
                return !object.Equals(register1, register2);

            return !register1.Equals(register2);
        }
    }
}
