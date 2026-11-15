using LootHub.Application.Common;
using LootHub.Domain.Entities;

namespace LootHub.Application.Contracts.Services;

public interface ITokenService
{
    AccessToken CreateToken(AppUser user, IList<string> roles);
}
