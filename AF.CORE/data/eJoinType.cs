namespace AF.DATA;

/// <summary>
/// Join Typen
/// </summary>
public enum eJoinType
{
    /// <summary>
    /// LEFT JOIN
    /// </summary>
    [Description("LEFT JOIN")]
    LeftJoin = 0,
    /// <summary>
    /// INNER JOIN
    /// </summary>
    [Description("INNER JOIN")]
    InnerJoin = 1,
    /// <summary>
    /// RIGHT JOIN
    /// </summary>
    [Description("RIGHT JOIN")]
    RightJoin = 2,
    /// <summary>
    /// OUTER JOIN
    /// </summary>
    [Description("OUTER JOIN")]
    FullOuterJoin = 3
}