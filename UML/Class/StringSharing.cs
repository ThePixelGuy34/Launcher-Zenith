using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;

public static class StringSharing
{
    public static string? LoginMethod { get; set; }
    public static string? DiscordId { get; set; }
    public static string? Username { get; set; }
    public static string? AuthKey { get; set; }
    public static string? Hash { get; set; }
    public static string? LoginReason { get; set; }

    // I've seperated E+P logic for now as it's quite different. Soon I want to make an external authentication endpoint on the backend
    // that can still return a token and user data, but for now this works almost the exact same and will be updated in the near future.

    public static string? access_token { get; set; }
    public static int expires_in { get; set; }
    public static string? expires_at { get; set; }
    public static string? token_type { get; set; }
    public static string? refresh_token { get; set; }
    public static int refresh_expires { get; set; }
    public static string? refresh_expires_at { get; set; }
    public static string? account_id { get; set; }
    public static string? client_id { get; set; }
    public static bool internal_client { get; set; }
    public static string? client_service { get; set; }
    public static string? displayName { get; set; }
    public static string? app { get; set; }
    public static string? in_app_id { get; set; }
    public static string? device_id { get; set; }
    public static string? authToken { get; set; }
}

