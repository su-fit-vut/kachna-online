// ServingConfiguration.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.App.Configuration;

public class ServingConfiguration
{
    public string PathBase { get; set; }
    public string ReverseProxyNetworkIp { get; set; }
    public int ReverseProxyNetworkPrefix { get; set; }
    public bool ServeStaticFiles { get; set; }
    public string StaticFilesPathBase { get; set; }
}
