using Windows.Win32.Foundation;
using Windows.Win32.Security;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[SuppressMessage("", "SH002")]
internal readonly struct UnsafeSidAndAttributes : IEquatable<UnsafeSidAndAttributes>
{
    public readonly PSID Sid;
    public readonly uint Attributes;

    private UnsafeSidAndAttributes(PSID sid, uint attributes)
    {
        Sid = sid;
        Attributes = attributes;
    }

    public readonly string StringSid
    {
        get
        {
            ConvertSidToStringSid(Sid, out PWSTR stringSid);
            return stringSid.ToString();
        }
    }

    public static implicit operator SID_AND_ATTRIBUTES(UnsafeSidAndAttributes sid) => new SID_AND_ATTRIBUTES
    {
        Sid = sid.Sid,
        Attributes = sid.Attributes,
    };

    public static explicit operator UnsafeSidAndAttributes(SID_AND_ATTRIBUTES sid) => Create(sid.Sid, sid.Attributes);

    public static bool operator ==(UnsafeSidAndAttributes left, UnsafeSidAndAttributes right) => left.Equals(right);

    public static bool operator !=(UnsafeSidAndAttributes left, UnsafeSidAndAttributes right) => !left.Equals(right);

    public static UnsafeSidAndAttributes Create(PSID sid, uint attributes)
    {
        return new(sid, attributes);
    }

    public static UnsafeSidAndAttributes Create(string stringSid, uint attributes)
    {
        ConvertStringSidToSid(stringSid, out PSID sid);
        return new(sid, attributes);
    }

    public bool Equals(UnsafeSidAndAttributes other)
    {
        return Sid == other.Sid && Attributes == other.Attributes;
    }

    public override bool Equals([NotNullWhen(true)]object? obj)
    {
        return obj is UnsafeSidAndAttributes other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Sid, Attributes);
    }
}
