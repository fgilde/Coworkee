using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Contracts.Services.Identity;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Infrastructure.Contexts;
using lib.Coworkee.Infrastructure.Models.Identity;
using lib.Coworkee.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nextended.Core.Attributes;
using Nextended.Core.Extensions;

namespace lib.Coworkee.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(IRoleClaimService), 0)]
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IStringLocalizer<RoleClaimService> _localizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _db;

        public RoleClaimService(
            IStringLocalizer<RoleClaimService> localizer,
            ICurrentUserService currentUserService,
            ApplicationDbContext db)
        {
            _localizer = localizer;
            _currentUserService = currentUserService;
            _db = db;
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
        {
            var roleClaims = await _db.RoleClaims.ToListAsync();
            var roleClaimsResponse = roleClaims.MapTo<List<RoleClaimResponse>>();
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _db.RoleClaims.CountAsync();
            return count;
        }

        public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
        {
            var roleClaim = await _db.RoleClaims
                .SingleOrDefaultAsync(x => x.Id == id);
            var roleClaimResponse = roleClaim.MapTo<RoleClaimResponse>();
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
        {
            var roleClaims = await _db.RoleClaims
                .Include(x => x.Role)
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
            var roleClaimsResponse = roleClaims.MapTo<List<RoleClaimResponse>>();
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                return await Result<string>.FailAsync(_localizer["Role is required."]);
            }

            if (request.Id == 0)
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.ClaimType && x.ClaimValue == request.ClaimValue);
                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role Claim already exists."]);
                }
                var roleClaim = request.MapTo<ApplicationRoleClaim>();
                await _db.RoleClaims.AddAsync(roleClaim);
                await _db.SaveChangesAsync(_currentUserService.UserId);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} created."], request.ClaimValue));
            }
            else
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .Include(x => x.Role)
                        .SingleOrDefaultAsync(x => x.Id == request.Id);
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync(_localizer["Role Claim does not exist."]);
                }

                existingRoleClaim.ClaimType = request.ClaimType;
                existingRoleClaim.ClaimValue = request.ClaimValue;
                existingRoleClaim.Group = request.Group;
                existingRoleClaim.Description = request.Description;
                existingRoleClaim.RoleId = request.RoleId;
                _db.RoleClaims.Update(existingRoleClaim);
                await _db.SaveChangesAsync(_currentUserService.UserId);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for Role {1} updated."], request.ClaimValue, existingRoleClaim.Role.Name));
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existingRoleClaim = await _db.RoleClaims
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRoleClaim != null)
            {
                _db.RoleClaims.Remove(existingRoleClaim);
                await _db.SaveChangesAsync(_currentUserService.UserId);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for {1} Role deleted."], existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
            }

            return await Result<string>.FailAsync(_localizer["Role Claim does not exist."]);
        }
    }
}