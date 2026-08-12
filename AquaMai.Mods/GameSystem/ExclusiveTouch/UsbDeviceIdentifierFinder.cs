using LibUsbDotNet.Main;

namespace AquaMai.Mods.GameSystem.ExclusiveTouch;

public sealed class UsbDeviceIdentifierFinder : UsbDeviceFinder
{
    private readonly UsbDeviceLocationFinder locationFinder;

    public UsbDeviceIdentifierFinder(int vid, int pid, string identifier)
        : base(vid, pid, identifier?.Trim())
    {
        locationFinder = new UsbDeviceLocationFinder(vid, pid, identifier);
    }

    public override bool Check(UsbRegistry usbRegistry)
    {
        return base.Check(usbRegistry) || locationFinder.Check(usbRegistry);
    }
}
