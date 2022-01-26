using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShopStoreFrontend.Domain.Common
{
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper()
    : base(new SqlMapper.ITypeMap[]
        {
                    new CustomPropertyTypeMap(
                       typeof(T),
                       (type, columnName) =>
                           type.GetProperties().FirstOrDefault(prop =>
                               prop.GetCustomAttributes(false)
                                   .OfType<ColumnAttribute>()
                                   .Any(attr => attr.Name == columnName)
                               )
                       ),
                    new DefaultTypeMap(typeof(T))
        })
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class FallbackTypeMapper : SqlMapper.ITypeMap
    {
        private readonly IEnumerable<SqlMapper.ITypeMap> MAPPERS;

        public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
        {
            MAPPERS = mappers;
        }

        /// <summary>
        /// ConstructorInfo 用來探索函式的屬性，以及叫用函式。 藉由呼叫物件的 Invoke 或方法所傳回的來建立物件
        /// </summary>
        /// <param name="names"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public ConstructorInfo FindConstructor(string[]names, Type[] types)
        {
            foreach(var mapper in MAPPERS)
            {
                try
                {
                    ConstructorInfo result = mapper.FindConstructor(names, types);
                    
                    if(result != null)
                    {
                        return result;
                    }

                }
                catch (NotImplementedException)
                {     
                    
                }
            }

            return null;
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            foreach(var mapper in MAPPERS)
            {
                try
                {
                    var result = mapper.GetConstructorParameter(constructor, columnName);
                    
                    if(result != null)
                    {
                        return result;
                    }

                }
                catch (NotImplementedException)
                {

                }
            }

            return null;
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            foreach(var mapper in MAPPERS)
            {
                try
                {
                    var result = mapper.GetMember(columnName);

                    if(result != null)
                    {
                        return result;
                    }

                }
                catch (NotImplementedException)
                {
                    
                }
            }

            return null;
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return MAPPERS
                    .Select(mapper => mapper.FindExplicitConstructor())
                    .FirstOrDefault(result => result != null);
        }
    }
}
