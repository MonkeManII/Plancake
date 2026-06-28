using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PlancakeSerializer.BuiltIns
{
    /// <summary>
    /// Assigns a <see cref="long"/> hash to each provided type, and all of its descendants / implementers.
    /// </summary>
    public class InheritanceHash
    {
        readonly Dictionary<Type, long> hashes = [];
        readonly Dictionary<long, Type> invhashes = [];

        /// <summary>
        /// Attempts to get the <see cref="long"/> hash of a specific <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the hash of.</param>
        /// <param name="hash">The hash, if any.</param>
        /// <returns>Whether a hash was found for that type.</returns>
        public bool TryGetHash(Type type, out long hash)
        {
            return hashes.TryGetValue(type, out hash);
        }

        /// <summary>
        /// Attempts to get the <see cref="Type"/> of a specific <see cref="long"/> hash.
        /// </summary>
        /// <param name="hash">The hash to get the type of.</param>
        /// <param name="type">The <see cref="Type"/>, if any.</param>
        /// <returns>Whether a type was found for that hash.</returns>
        public bool TryGetType(long hash, [NotNullWhen(true)] out Type? type)
        {
            return invhashes.TryGetValue(hash, out type);
        }

        /// <summary>
        /// Creates a <see cref="InheritanceHash"/> that hashes the provided types and all of their descendants / implementers.
        /// </summary>
        /// <param name="types">All of the types to add to the hashmap.</param>
        public InheritanceHash(params Type[] types)
        {
            foreach (Type t in types)
            {
                foreach (Type c in AllThatExtendsOrImplements(t))
                {
                    HashType(c);
                }
            }
        }

        long HashType(Type t)
        {
            ArgumentNullException.ThrowIfNull(t);

            if (hashes.TryGetValue(t, out long key))
            {
                return key;
            }

            string str = t.AssemblyQualifiedName ?? t.Name;

            long r = StringHasher.HashString(str, t.Namespace?.Length ?? t.Name.Length);

            if (!invhashes.TryAdd(r, t))
            {
                invhashes.TryGetValue(r, out Type? collision);
                if (collision is not null) throw new Exception($"Hash collision between {t.Name} and {collision.Name}! (hash: {r})");
            }

            hashes.Add(t, r);

            return r;
        }

        static Type[] AllThatExtendsOrImplements(Type baseClass)
        {
            // TRIPLE NESTED FOREACH LOOPS!
            // you LOVE to see it!
            List<Type> tList = [];
            Assembly[] appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in appAssemblies)
            {
                Module[] mod = a.GetModules();
                foreach (Module m in mod)
                {
                    Type[] types = m.GetTypes();
                    foreach (Type t in types)
                    {
                        if (ImplementsOrExtends(baseClass, t))
                        {
                            tList.Add(t);
                        }
                    }
                }
            }
            return [.. tList];
        }

        static bool ImplementsOrExtends(Type tBase, Type tImplements)
        {
            if (tBase.IsInterface)
            {
                return tImplements.IsAssignableTo(tBase);
            }
            else
            {
                return tImplements == tBase || tImplements.IsSubclassOf(tBase);
            }
        }
    }
}
