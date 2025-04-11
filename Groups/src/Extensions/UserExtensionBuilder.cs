using Amethyst.Players;
using Amethyst.Players.Extensions;

namespace Groups.Extensions;

public sealed class UserExtensionBuilder : IPlayerExtensionBuilder<UserExtension>
{
    public UserExtension Build(NetPlayer player) => new(player);

    public void Initialize() { }
}
