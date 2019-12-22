using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myna.Core.Extensions {
    public static class AppExtensions {

        public static void AssertIsNotNull<T>(this T obj, string name, string message = null)
            where T: class {
            if (obj is null)
                ThrowArgumentNullException<T>(obj, name, message);
        }

        public static void AssertIsNotEmpty<T>(this T obj, string name, string message = null, 
            T defaultValue = null) where T: class {

            if (obj == defaultValue
                || (obj is string str && string.IsNullOrWhiteSpace(str))
                || (obj is IEnumerable list && !list.Cast<object>().Any())
            )
                ThrowArgumentNullException<T>(obj, name, message);
        }

        public static void AssertStringIsNotNullOrEmpty(this string str, string name, string message = null) {
            if (string.IsNullOrWhiteSpace(str))
                ThrowArgumentNullException<string>(str, name, message);
        }


        private static void ThrowArgumentNullException<T>(T obj, string name, string message = null) {
            throw new ArgumentNullException($"{name}: {typeof(T)}", message);
        }
    }
}
