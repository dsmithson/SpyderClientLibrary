using System;
namespace Spyder.Client.Common
{
    public interface IRegister
    {
        void CopyFrom(IRegister copyFrom);
        int LookupID { get; set; }
        string Name { get; set; }
        int PageIndex { get; }
        Knightware.Primitives.Color RegisterColor { get; set; }
        bool RegisterColorDefined { get; set; }
        int RegisterID { get; set; }
        RegisterType Type { get; set; }
    }
}
