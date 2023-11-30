namespace Mint.Network;

internal struct PacketBind
{
    internal PacketBind(string registrar, PacketBindDelegate bindDelegate)
    {
        RegistrarName = registrar;
        BindDelegate = bindDelegate;
    }
    
    internal string RegistrarName;
    internal PacketBindDelegate BindDelegate;
}