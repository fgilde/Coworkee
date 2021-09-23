using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.Generation;

namespace CleanArchitectureBase.Server
{
    public class CustomSchemaNameGenerator : ISchemaNameGenerator
    {
        public string Generate(Type type)
        {
            return ConstructSchemaId(type);
        }

        public string ConstructSchemaId(Type type)
        {
            var typeName = type.Name;
            if (type.IsGenericType)
            {
                var genericArgs = string.Join(", ", type.GetGenericArguments().Select(ConstructSchemaId));

                int index = typeName.IndexOf('`');
                var typeNameWithoutGenericArity = index == -1 ? typeName : typeName.Substring(0, index);

                return $"{typeNameWithoutGenericArity}<{genericArgs}>";
            }
            return typeName;
        }
    }

    public class CustomTypeNameGenerator : DefaultTypeNameGenerator
    {
        /// <inheritdoc />
        public override string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
        {
            if (string.IsNullOrEmpty(typeNameHint) && !string.IsNullOrEmpty(schema.DocumentPath))
            {
                typeNameHint = schema.DocumentPath.Replace("\\", "/").Split('/').Last();
            }

            return typeNameHint;
        }
    }
}