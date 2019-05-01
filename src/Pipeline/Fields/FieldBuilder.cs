﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class FieldBuilder : MemberBuilder<FieldInfo, object>
    {
        #region Constructors

        public FieldBuilder(UnityContainer container)
            : base(container)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
            return type.GetDeclaredFields()
                       .Where(member => !member.IsFamily && !member.IsPrivate &&
                                        !member.IsInitOnly && !member.IsStatic);
        }

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        public override ISelect<FieldInfo> GetOrDefault(IPolicySet registration) => 
            registration.Get<ISelect<FieldInfo>>() ?? Defaults.FieldsSelector;

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(FieldInfo info, object? resolver)
        {
            return Expression.Assign(
                Expression.Field(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(BuilderContextExpression.Context,
                        BuilderContextExpression.ResolveFieldMethod,
                        Expression.Constant(info, typeof(FieldInfo)),
                        Expression.Constant(PreProcessResolver(info, resolver), typeof(object))),
                    info.FieldType));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object? resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, value));
                return context.Existing;
            };
        }

        #endregion
    }
}
