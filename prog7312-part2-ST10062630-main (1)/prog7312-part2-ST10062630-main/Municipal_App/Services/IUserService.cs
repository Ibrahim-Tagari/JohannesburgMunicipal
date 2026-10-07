using Municipal_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municipal_App.Services
{
    /// <summary>
    /// Interface for user management service
    /// Provides CRUD operations and authentication for users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Plain text password</param>
        /// <returns>User object if authentication successful, null otherwise</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Gets a user by their unique identifier
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User object or null if not found</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Gets a user by their username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User object or null if not found</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>User object or null if not found</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="user">User object with details</param>
        /// <returns>User object if creation successful, null otherwise</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user's information
        /// </summary>
        /// <param name="user">User object with updated details</param>
        /// <returns>Updated User object if update successful, null otherwise</returns>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Soft deletes a user (sets IsActive to false)
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>True if deletion successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Validates a user's password
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="password">Plain text password to validate</param>
        /// <returns>True if password is valid, false otherwise</returns>
        Task<bool> ValidatePasswordAsync(User user, string password);

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="currentPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if password changed successfully, false otherwise</returns>
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        /// <summary>
        /// Gets all users in the system
        /// </summary>
        /// <returns>List of all users</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets users by their role
        /// </summary>
        /// <param name="role">User role to filter by</param>
        /// <returns>List of users with the specified role</returns>
        Task<List<User>> GetUsersByRoleAsync(string role);

        /// <summary>
        /// Checks if a username or email address is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="email">Email address to check</param>
        /// <returns>True if username or email is available, false otherwise</returns>
        Task<bool> UserExistsAsync(string username, string email);

        /// <summary>
        /// Hashes a password using SHA256
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="hash">Password hash to verify against</param>
        /// <returns>True if password matches hash, false otherwise</returns>
        bool VerifyPassword(string password, string hash);
    }
}
