﻿using GraphQL.Builders;
using GraphQL.Types;

static class ArgumentAppender
{
    static QueryArgument<ListGraphType<WhereExpressionGraph>> whereArgument =
        new QueryArgument<ListGraphType<WhereExpressionGraph>>
        {
            Name = "where"
        };

    static QueryArgument<IntGraphType> skipArgument =
        new QueryArgument<IntGraphType>
        {
            Name = "skip"
        };

    static QueryArgument<IntGraphType> takeArgument =
        new QueryArgument<IntGraphType>
        {
            Name = "take"
        };

    public static void AddWhereArgument<TSource, TGraph>(this ConnectionBuilder<TGraph, TSource> builder)
        where TGraph : IGraphType
    {
        builder.Argument<ListGraphType<WhereExpressionGraph>>("where", null);
    }

    public static QueryArguments GetQueryArguments()
    {
        return new QueryArguments
        {
            whereArgument,
            skipArgument,
            takeArgument
        };
    }
}