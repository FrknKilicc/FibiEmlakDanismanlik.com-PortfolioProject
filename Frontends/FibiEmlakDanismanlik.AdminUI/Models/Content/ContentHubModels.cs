#nullable enable
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.AdminUI.Models.Content;

/// <summary>
/// İçerik Merkezi (Hub) kart kutucuğu modeli.
/// </summary>
public sealed class ContentHubTile
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? SiteUrl { get; set; }
    public string Stat { get; set; } = string.Empty;
    public string? Warning { get; set; }
}

/// <summary>
/// İçerik Merkezi grup modeli.
/// </summary>
public sealed class ContentHubGroup
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ContentHubTile> Tiles { get; set; } = new();
}

/// <summary>
/// İçerik Merkezi sayfa görünüm modeli.
/// </summary>
public sealed class ContentHubViewModel
{
    public List<ContentHubGroup> Groups { get; set; } = new();
}
