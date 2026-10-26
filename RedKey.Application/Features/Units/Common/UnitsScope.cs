namespace RedKey.Application.Features.Units.Common;

/// <summary>
/// Who is asking for units. Set by the controller (never by the client): the same query serves
/// the landing / customer website, the broker dashboard and the admin dashboard.
/// </summary>
public enum UnitsScope
{
    /// <summary>Website (visitors + customers): approved units only, no creator / commission data.</summary>
    Public = 1,

    /// <summary>Broker "available units": approved units with the commission.</summary>
    BrokerCatalog = 2,

    /// <summary>Broker "my listings": the broker's own units in every status.</summary>
    BrokerOwn = 3,

    /// <summary>Super admin: every unit + who created it.</summary>
    Admin = 4
}
