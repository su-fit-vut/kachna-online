// UserService.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Models.Kis;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using User = KachnaOnline.Business.Models.Users.User;
using UserEntity = KachnaOnline.Data.Entities.Users.User;
using UserRoleEntity = KachnaOnline.Data.Entities.Users.UserRole;

namespace KachnaOnline.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IOptionsMonitor<KisOptions> _kisOptions;
        private readonly IOptionsMonitor<JwtOptions> _jwtOptions;
        private readonly IKisService _kisService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IKisService kisService, IUnitOfWork unitOfWork, ILogger<UserService> logger,
            IMapper mapper, IOptionsMonitor<KisOptions> kisOptions, IOptionsMonitor<JwtOptions> jwtOptions)
        {
            _kisService = kisService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _kisOptions = kisOptions;
            _jwtOptions = jwtOptions;

            _userRepository = unitOfWork.Users;
            _roleRepository = unitOfWork.Roles;
        }

        /// <inheritdoc />
        public async Task<LoginResult> LoginSession(string kisSessionId)
        {
            var kisIdentity = await _kisService.GetIdentityFromSession(kisSessionId);
            return await this.LoginIdentity(kisIdentity);
        }

        /// <inheritdoc />
        public async Task<LoginResult> LoginToken(string kisRefreshToken)
        {
            var kisIdentity = await _kisService.GetIdentityFromRefreshToken(kisRefreshToken);
            return await this.LoginIdentity(kisIdentity);
        }

        /// <inheritdoc />
        public async Task<User> GetUser(int userId)
        {
            var entity = await _userRepository.Get(userId);
            return _mapper.Map<User>(entity);
        }

        /// <inheritdoc />
        public async Task SaveUser(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var entity = await _userRepository.Get(user.Id);

            entity.DiscordId = user.DiscordId;
            entity.Nickname = user.Nickname;

            await _unitOfWork.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetUserRoles(int userId)
        {
            var entity = await _userRepository.GetWithRoles(userId);
            if (entity is null)
                return null;

            return entity.Roles
                .Where(r => !r.ForceDisable)
                .Select(r => r.Role.Name);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RoleAssignment>> GetUserRoleDetails(int userId)
        {
            var entity = await _userRepository.GetWithRoles(userId);
            if (entity is null)
                return null;

            return entity.Roles
                .Select(r => new RoleAssignment()
                {
                    Role = r.Role.Name,
                    AssignedBy = _mapper.Map<User>(r.AssignedByUser),
                    ManuallyDisabled = r.ForceDisable
                });
        }

        /// <inheritdoc />
        public async Task<bool?> IsInRole(int userId, string role)
        {
            var entity = await _userRepository.GetWithRoles(userId);
            if (entity is null)
                return null;

            return entity.Roles.Any(r => r.Role.Name == role && !r.ForceDisable);
        }

        /// <summary>
        /// Checks the provided <paramref name="kisIdentity"/>, synchronizes user data
        /// and returns a JWT for the user.
        /// </summary>
        /// <param name="kisIdentity">A <see cref="KisIdentity"/> object.</param>
        /// <returns>A <see cref="LoginResult"/> structure with the JWT Bearer token or information about errors.</returns>
        /// <seealso cref="IUserService.LoginSession"/>
        /// <seealso cref="IUserService.LoginToken"/>
        private async Task<LoginResult> LoginIdentity(KisIdentity kisIdentity)
        {
            if (kisIdentity is null)
                return new LoginResult() {HasError = true};
            if (kisIdentity.UserData is null)
                return new LoginResult() {UserFound = false};

            var userEntity = await this.SynchronizeUser(kisIdentity);
            if (userEntity is null)
                return new LoginResult() {HasError = true};

            var token = this.MakeJwt(kisIdentity,
                userEntity.Roles
                    .Where(r => !r.ForceDisable)
                    .Select(r => r.Role?.Name)
                    .Distinct()
                    .Where(r => r != null)
                    .ToArray());

            return new LoginResult() {UserFound = true, AccessToken = token, KisAccessToken = kisIdentity.AuthToken};
        }

        /// <summary>
        /// Creates or updates a local user entity based on data stored in the provided <paramref name="kisIdentity"/>.
        /// Performs role mapping using <see cref="MapRoles"/>.
        /// </summary>
        /// <param name="kisIdentity">A <see cref="KisIdentity"/> object.</param>
        /// <returns>
        /// A <see cref="UserEntity"/> object that corresponds to the user identified
        /// by the <paramref name="kisIdentity"/>; or null if the identity is null or when a database error occurs.
        /// </returns>
        /// <seealso cref="MapRoles"/>
        private async Task<UserEntity> SynchronizeUser(KisIdentity kisIdentity)
        {
            if (kisIdentity is null or {UserData: null})
                return null;

            var kisData = kisIdentity.UserData;
            var id = kisData.Id;

            var userEntity = await _userRepository.GetWithRoles(id);
            if (userEntity is null)
            {
                // Create new user
                userEntity = new UserEntity
                {
                    Id = id,
                    Name = kisData.Name,
                    Email = kisData.Email,
                    Nickname = kisData.Nickname,
                    Roles = new List<UserRoleEntity>()
                };

                await _userRepository.Add(userEntity);
            }
            else
            {
                // Synchronize existing user entity with KIS
                userEntity.Name = kisData.Name;
                userEntity.Email = kisData.Email;

                // Only assign KIS nickname if the user hasn't set a custom nickname.
                userEntity.Nickname ??= kisData.Nickname;
            }

            await this.MapRoles(userEntity, kisData.Roles);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (DbException e)
            {
                await _unitOfWork.ClearTrackedChanges();
                _logger.LogError(e, "Cannot synchronize user {UserId}.", id);
                return null;
            }

            return userEntity;
        }

        /// <summary>
        /// Create a JWT based on a KIS <paramref name="identity"/> object and a set of local role names,
        /// signed using the configured symmetric key.
        /// </summary>
        /// <param name="identity">A <see cref="KisIdentity"/> object representing the user's identity.</param>
        /// <param name="roles">An array of local role names.</param>
        /// <returns>A JSON Web Token representing the user's identity.</returns>
        private string MakeJwt(KisIdentity identity, string[] roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = Encoding.ASCII.GetBytes(_jwtOptions.CurrentValue.Secret);
            var signingCredentials =
                new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new(IdentityConstants.IdClaim, identity.UserData.Id.ToString(), ClaimValueTypes.Integer),
                new(IdentityConstants.EmailClaim, identity.UserData.Email, ClaimValueTypes.Email),
                new(IdentityConstants.NameClaim, identity.UserData.Nickname ?? identity.UserData.Name),
                new(IdentityConstants.KisRefreshTokenClaim, identity.RefreshToken)
            };
            claims.AddRange(roles.Select(r => new Claim(IdentityConstants.RolesClaim, r)));

            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.CurrentValue.ValiditySeconds),
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        /// <summary>
        /// Modifies a <paramref name="userEntity"/>'s local roles so that they match the specified KIS roles
        /// based on the configured mappings. Roles that have been assigned manually are not affected.
        /// </summary>
        /// <remarks>
        /// The mappings are configured using <see cref="KisOptions"/>. If the local role isn't present in the database,
        /// it is skipped.
        /// </remarks>
        /// <param name="userEntity">A <see cref="UserEntity"/> entity object.</param>
        /// <param name="userKisRoles">An array of KIS role names to map to local roles.</param>
        private async Task MapRoles(UserEntity userEntity, string[] userKisRoles)
        {
            var mappings = _kisOptions.CurrentValue.RoleMappings;

            // Select all local roles corresponding to the user's KIS roles
            var targetRoles = mappings
                .Where(e => userKisRoles.Contains(e.Key))
                .SelectMany(e => e.Value)
                .Distinct()
                .ToList();

            // Add roles that KIS grants the user
            foreach (var targetRoleName in targetRoles)
            {
                var role = await _roleRepository.GetByName(targetRoleName);
                if (role is null)
                {
                    _logger.LogWarning("Cannot find local role {RoleName} mentioned in a configured KIS role mapping.",
                        targetRoleName);
                    continue;
                }

                var existingAssociation = userEntity.Roles.FirstOrDefault(r => r.RoleId == role.Id);
                if (existingAssociation is null)
                {
                    // The user doesn't have the role they should have
                    userEntity.Roles.Add(new UserRoleEntity {RoleId = role.Id});
                    _logger.LogInformation("Adding role {RoleName} to user {UserId} (based on a KIS mapping).",
                        role.Name, userEntity.Id);
                }
            }

            // Remove non-manually assigned roles from the user
            var toRemove = new List<UserRoleEntity>();
            foreach (var existingAssociation in userEntity.Roles)
            {
                if (existingAssociation.Role != null &&
                    !targetRoles.Contains(existingAssociation.Role.Name) &&
                    !existingAssociation.AssignedByUserId.HasValue)
                {
                    _logger.LogInformation("Removing role {RoleName} from user {UserId} (based on a KIS mapping).",
                        existingAssociation.Role.Name, userEntity.Id);
                    toRemove.Add(existingAssociation);
                }
            }

            foreach (var userRole in toRemove)
                userEntity.Roles.Remove(userRole);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetRoles()
        {
            return await _roleRepository.All()
                .Select(r => r.Name).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetUsers(string filter)
        {
            var users = filter is null
                ? await _userRepository.All().ToListAsync()
                : await _userRepository.GetFiltered(filter);

            return _mapper.Map<IEnumerable<User>>(users);
        }

        /// <inheritdoc />
        public async Task AssignRole(int userId, string role, int assignedByUserId)
        {
            var userEntity = await _userRepository.GetWithRoles(userId);
            var roleEntity = await _roleRepository.GetByName(role);

            if (userEntity is null)
                throw new UserNotFoundException(userId);
            if (await _userRepository.Get(assignedByUserId) == null)
                throw new UserNotFoundException(assignedByUserId);
            if (roleEntity is null)
                throw new RoleNotFoundException();

            var assignment = userEntity.Roles.FirstOrDefault(r => r.Role.Id == roleEntity.Id);

            if (assignment is null)
            {
                _logger.LogInformation("User {AssignedById} created a manual assignment of {Role} to {UserId}.",
                    assignedByUserId, role, userId);

                userEntity.Roles.Add(new UserRoleEntity()
                {
                    Role = roleEntity,
                    User = userEntity,
                    AssignedByUserId = assignedByUserId,
                    ForceDisable = false
                });
            }
            else
            {
                _logger.LogInformation(
                    "User {AssignedById} modified a manual assignment of {Role} to {UserId} (previously assigned: {Prev}, now assigned: {Now}).",
                    assignedByUserId, role, userId, !assignment.ForceDisable, true);

                assignment.AssignedByUserId = assignedByUserId;
                assignment.ForceDisable = false;
            }

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Role assignment failed.");
                await _unitOfWork.ClearTrackedChanges();
                throw new RoleManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task RevokeRole(int userId, string role, int revokedByUserId)
        {
            var userEntity = await _userRepository.GetWithRoles(userId);
            var roleEntity = await _roleRepository.GetByName(role);

            if (userEntity is null)
                throw new UserNotFoundException(userId);
            if (await _userRepository.Get(revokedByUserId) == null)
                throw new UserNotFoundException(revokedByUserId);
            if (roleEntity is null)
                throw new RoleNotFoundException();

            var assignment = userEntity.Roles.FirstOrDefault(r => r.Role.Id == roleEntity.Id);

            if (assignment is null)
            {
                _logger.LogInformation("User {AssignedById} created a manual revocation of {Role} to {UserId}.",
                    revokedByUserId, role, userId);

                userEntity.Roles.Add(new UserRoleEntity()
                {
                    Role = roleEntity,
                    User = userEntity,
                    AssignedByUserId = revokedByUserId,
                    ForceDisable = true
                });
            }
            else
            {
                _logger.LogInformation(
                    "User {AssignedById} modified a manual assignment of {Role} to {UserId} (previously assigned: {Prev}, now assigned: {Now}).",
                    revokedByUserId, role, userId, !assignment.ForceDisable, false);

                assignment.AssignedByUserId = revokedByUserId;
                assignment.ForceDisable = true;
            }

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Role revocation failed.");
                await _unitOfWork.ClearTrackedChanges();
                throw new RoleManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task ResetRole(int userId, string role, int resetByUserId)
        {
            var userEntity = await _userRepository.GetWithRoles(userId);
            if (userEntity is null)
                throw new UserNotFoundException(userId);

            if (await _userRepository.Get(resetByUserId) == null)
                throw new UserNotFoundException(resetByUserId);

            var assignment = userEntity.Roles.FirstOrDefault(r => r.Role.Name == role);
            if (assignment is null)
                return;

            _logger.LogInformation(
                "User {AssignedById} reset the manual assignment of {Role} to {UserId} (previously assigned: {Prev}).",
                resetByUserId, role, userId, !assignment.ForceDisable);

            userEntity.Roles.Remove(assignment);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Role reset failed.");
                await _unitOfWork.ClearTrackedChanges();
                throw new RoleManipulationFailedException();
            }
        }
    }
}
