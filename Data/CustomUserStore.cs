namespace Flügger.Data
{
    using Microsoft.AspNetCore.Identity;
    using Npgsql;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly string _connectionString;

        public CustomUserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgreSQL");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync(cancellationToken);

                user.Email = user.UserName + "@ucl.flügger.dk";

                // Ensure required properties are not null
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Username, Email, and PasswordHash must not be null." });
                }

                // Hash the password before saving
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                var cmd = new NpgsqlCommand(@"
            INSERT INTO users (username, email, password_hash, security_stamp, concurrency_stamp)
            VALUES (@username, @email, @password, @stamp, @concurrency)", conn);

                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value); // Ensure it's not null
                cmd.Parameters.AddWithValue("@password", user.PasswordHash);
                cmd.Parameters.AddWithValue("@stamp", user.SecurityStamp ?? "");
                cmd.Parameters.AddWithValue("@concurrency", user.ConcurrencyStamp ?? "");

                int result = await cmd.ExecuteNonQueryAsync(cancellationToken);

                if (result > 0)
                    return IdentityResult.Success;

                return IdentityResult.Failed(new IdentityError { Description = "User creation failed, no rows affected." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return IdentityResult.Failed(new IdentityError { Description = $"Exception: {ex.Message}" });
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);

            var cmd = new NpgsqlCommand("SELECT id, username, email, password_hash FROM users WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@username", normalizedUserName);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return new ApplicationUser
                {
                    Id = reader["id"].ToString(),
                    UserName = reader["username"].ToString(),
                    Email = reader["email"].ToString(),
                    PasswordHash = reader["password_hash"].ToString()
                };
            }
            return null;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public void Dispose() { }

        // Other required methods for IUserStore
        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) { user.UserName = userName; return Task.CompletedTask; }
        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName.ToUpper());
        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
