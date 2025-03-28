using Amethyst.Players;
using Amethyst.Players.Extensions;

namespace Amethyst.Groups.Extensions;

public sealed class UserExtensionBuilder : IPlayerExtensionBuilder<UserExtension>
{
    public UserExtension Build(NetPlayer player)
    {
        return new UserExtension(player);
    }

    public void Initialize() {}
}
