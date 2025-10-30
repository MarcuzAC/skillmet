using System.DirectoryServices.Protocols;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace DepartmentalSystemAPI.Services
{
    public class AdService : IAdService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdService> _logger;
        private LdapConnection? _connection;

        public AdService(IConfiguration configuration, ILogger<AdService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AdUserInfo?> AuthenticateAsync(string username, string password)
        {
            LdapConnection? connection = null;
            try
            {
                var ldapConfig = _configuration.GetSection("Ldap");
                var server = ldapConfig["Server"];
                var port = int.Parse(ldapConfig["Port"] ?? "389");
                var domain = ldapConfig["Domain"];
                var searchBase = ldapConfig["SearchBase"];

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(domain))
                {
                    throw new InvalidOperationException("LDAP configuration is missing.");
                }

                // Create LDAP connection
                var identifier = new LdapDirectoryIdentifier(server, port);
                connection = new LdapConnection(identifier);

                // Set connection options
                connection.SessionOptions.ProtocolVersion = 3;
                connection.AuthType = AuthType.Basic;

                // Bind with user credentials to authenticate
                var credentials = new NetworkCredential($"{username}@{domain}", password);
                await Task.Run(() => connection.Bind(credentials));

                _logger.LogInformation("LDAP authentication successful for user: {Username}", username);

                // If bind succeeds, get user info
                var userInfo = await GetUserInfoInternalAsync(connection, username, searchBase);
                return userInfo;
            }
            catch (LdapException ex)
            {
                _logger.LogWarning("LDAP authentication failed for user: {Username} - Error: {ErrorCode}", username, ex.ErrorCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during LDAP authentication for user: {Username}", username);
                throw;
            }
            finally
            {
                connection?.Dispose();
            }
        }

        public async Task<AdUserInfo?> GetUserInfoAsync(string username)
        {
            LdapConnection? connection = null;
            try
            {
                var ldapConfig = _configuration.GetSection("Ldap");
                var server = ldapConfig["Server"];
                var port = int.Parse(ldapConfig["Port"] ?? "389");
                var bindDn = ldapConfig["BindDN"];
                var bindPassword = ldapConfig["BindPassword"];
                var searchBase = ldapConfig["SearchBase"];

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(bindDn) || string.IsNullOrEmpty(bindPassword))
                {
                    throw new InvalidOperationException("LDAP configuration is missing.");
                }

                var identifier = new LdapDirectoryIdentifier(server, port);
                connection = new LdapConnection(identifier);
                connection.SessionOptions.ProtocolVersion = 3;

                // Bind with service account
                var credentials = new NetworkCredential(bindDn, bindPassword);
                await Task.Run(() => connection.Bind(credentials));

                return await GetUserInfoInternalAsync(connection, username, searchBase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info from AD for user: {Username}", username);
                return null;
            }
            finally
            {
                connection?.Dispose();
            }
        }

        public async Task<List<AdUserInfo>> GetAllUsersAsync()
        {
            LdapConnection? connection = null;
            try
            {
                var ldapConfig = _configuration.GetSection("Ldap");
                var server = ldapConfig["Server"];
                var port = int.Parse(ldapConfig["Port"] ?? "389");
                var bindDn = ldapConfig["BindDN"];
                var bindPassword = ldapConfig["BindPassword"];
                var searchBase = ldapConfig["SearchBase"];

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(bindDn) || string.IsNullOrEmpty(bindPassword))
                {
                    throw new InvalidOperationException("LDAP configuration is missing.");
                }

                var identifier = new LdapDirectoryIdentifier(server, port);
                connection = new LdapConnection(identifier);
                connection.SessionOptions.ProtocolVersion = 3;

                // Bind with service account
                var credentials = new NetworkCredential(bindDn, bindPassword);
                await Task.Run(() => connection.Bind(credentials));

                return await GetAllUsersInternalAsync(connection, searchBase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users from AD");
                return new List<AdUserInfo>();
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private async Task<AdUserInfo?> GetUserInfoInternalAsync(LdapConnection connection, string username, string? searchBase)
        {
            return await Task.Run(() =>
            {
                var searchFilter = $"(sAMAccountName={username})";
                var attributes = new[] {
                    "givenName", "sn", "mail", "displayName",
                    "department", "title", "memberOf", "userAccountControl",
                    "sAMAccountName"
                };

                var searchRequest = new SearchRequest(
                    searchBase ?? "",
                    searchFilter,
                    SearchScope.Subtree,
                    attributes
                );

                var response = (SearchResponse)connection.SendRequest(searchRequest);

                if (response.Entries.Count == 0)
                {
                    return null;
                }

                var entry = response.Entries[0];
                var userInfo = new AdUserInfo
                {
                    Username = GetAttributeValue(entry, "sAMAccountName") ?? username,
                    FirstName = GetAttributeValue(entry, "givenName") ?? "",
                    LastName = GetAttributeValue(entry, "sn") ?? "",
                    Email = GetAttributeValue(entry, "mail") ?? "",
                    DisplayName = GetAttributeValue(entry, "displayName") ?? "",
                    Department = GetAttributeValue(entry, "department") ?? "",
                    Title = GetAttributeValue(entry, "title") ?? "",
                    Groups = GetUserGroups(entry),
                    IsActive = IsUserActive(entry)
                };

                return userInfo;
            });
        }

        private async Task<List<AdUserInfo>> GetAllUsersInternalAsync(LdapConnection connection, string? searchBase)
        {
            return await Task.Run(() =>
            {
                var users = new List<AdUserInfo>();

                // Search for all users (objectCategory=person and objectClass=user)
                var searchFilter = "(&(objectCategory=person)(objectClass=user))";
                var attributes = new[] {
                    "givenName", "sn", "mail", "displayName",
                    "department", "title", "memberOf", "userAccountControl",
                    "sAMAccountName"
                };

                var searchRequest = new SearchRequest(
                    searchBase ?? "",
                    searchFilter,
                    SearchScope.Subtree,
                    attributes
                );

                // Use paging to get all results
                var pageResultRequestControl = new PageResultRequestControl(1000);
                searchRequest.Controls.Add(pageResultRequestControl);

                while (true)
                {
                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        var userInfo = new AdUserInfo
                        {
                            Username = GetAttributeValue(entry, "sAMAccountName") ?? "",
                            FirstName = GetAttributeValue(entry, "givenName") ?? "",
                            LastName = GetAttributeValue(entry, "sn") ?? "",
                            Email = GetAttributeValue(entry, "mail") ?? "",
                            DisplayName = GetAttributeValue(entry, "displayName") ?? "",
                            Department = GetAttributeValue(entry, "department") ?? "",
                            Title = GetAttributeValue(entry, "title") ?? "",
                            Groups = GetUserGroups(entry),
                            IsActive = IsUserActive(entry)
                        };

                        // Only add users that have a username and are active
                        if (!string.IsNullOrEmpty(userInfo.Username) && userInfo.IsActive)
                        {
                            users.Add(userInfo);
                        }
                    }

                    // Check if there are more pages
                    var pageResponse = (PageResultResponseControl)response.Controls[0];
                    if (pageResponse.Cookie.Length == 0)
                        break;

                    pageResultRequestControl.Cookie = pageResponse.Cookie;
                }

                _logger.LogInformation("Found {Count} users in Active Directory", users.Count);
                return users;
            });
        }

        private string? GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            if (entry.Attributes.Contains(attributeName))
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute.Count > 0)
                {
                    return attribute[0] as string;
                }
            }
            return null;
        }

        private string[] GetUserGroups(SearchResultEntry entry)
        {
            var groups = new List<string>();

            if (entry.Attributes.Contains("memberOf"))
            {
                var memberOf = entry.Attributes["memberOf"];
                foreach (var groupDn in memberOf.GetValues(typeof(string)))
                {
                    if (groupDn is string groupDnString)
                    {
                        // Extract CN from DN (CN=GroupName,OU=Groups,DC=domain,DC=com)
                        var cnStart = groupDnString.IndexOf("CN=", StringComparison.OrdinalIgnoreCase);
                        if (cnStart >= 0)
                        {
                            var cnEnd = groupDnString.IndexOf(',', cnStart);
                            var groupName = cnEnd >= 0
                                ? groupDnString.Substring(cnStart + 3, cnEnd - cnStart - 3)
                                : groupDnString.Substring(cnStart + 3);
                            groups.Add(groupName);
                        }
                    }
                }
            }

            return groups.ToArray();
        }

        private bool IsUserActive(SearchResultEntry entry)
        {
            if (entry.Attributes.Contains("userAccountControl"))
            {
                var uacAttribute = entry.Attributes["userAccountControl"];
                if (uacAttribute.Count > 0 && uacAttribute[0] is string uacString && int.TryParse(uacString, out int uac))
                {
                    // Check if account is disabled (bit 2)
                    return (uac & 0x0002) == 0;
                }
            }
            return true;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}