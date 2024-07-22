using StackExchange.Redis;

namespace Yo.StackExchange.Redis.Extensions.Enum;

/// <summary>
/// SET 命令的Redis存在规范
/// </summary>
public enum RedisSetModeEnum
{
    /// <summary>
    /// 无论Key是否存在，都应该执行Set
    /// </summary>
    None,

    /// <summary>
    /// 仅在Key不存在的时候执行Set
    /// </summary>
    Nx,

    /// <summary>
    /// 仅在Key存在的时候执行Set
    /// </summary>
    Xx
}

public static class RedisSetModeEnumExtensions
{
    public static When ConvertToWhen(this RedisSetModeEnum redisSetMode) => redisSetMode switch
    {
        RedisSetModeEnum.Nx => When.NotExists,
        RedisSetModeEnum.Xx => When.Exists,
        _ => When.Always
    };
}