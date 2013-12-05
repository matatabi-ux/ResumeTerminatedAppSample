using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResumeTerminatedAppSample.Framework
{
    /// <summary>
    /// サービス格納コンテナ
    /// </summary>
    public class ServiceContainer : IServiceLocator
    {
        #region Privates

        /// <summary>
        /// コンストラクタ情報
        /// </summary>
        private readonly Dictionary<Type, ConstructorInfo> ConstructorInfos = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// 空引数
        /// </summary>
        private readonly object[] EmptyArguments = new object[0];

        /// <summary>
        /// デフォルトキー値
        /// </summary>
        private readonly string DefaultKey = Guid.NewGuid().ToString();

        /// <summary>
        /// ファクトリ
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, Delegate>> Factories = new Dictionary<Type, Dictionary<string, Delegate>>();

        /// <summary>
        /// インスタンス登録情報
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, object>> Registry = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// インタフェース登録情報
        /// </summary>
        private readonly Dictionary<Type, Type> InterfaceMap = new Dictionary<Type, Type>();

        /// <summary>
        /// 排他処理用オブジェクト
        /// </summary>
        private readonly object LockObject = new object();

        /// <summary>
        /// コンテナインスタンス
        /// </summary>
        private static ServiceContainer instance;

        #endregion //Privates

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static ServiceContainer Instance
        {
            get
            {
                return instance ?? (instance = new ServiceContainer());
            }
        }

        /// <summary>
        /// 指定したクラスが登録済みか判定します
        /// </summary>
        /// <typeparam name="T">指定したクラス</typeparam>
        /// <returns>登録済みの場合 true、それ以外は false</returns>
        public bool IsRegistered<T>()
        {
            var classType = typeof(T);
            return this.InterfaceMap.ContainsKey(classType);
        }

        /// <summary>
        /// 指定したキーで指定したクラスが登録済みか判定します
        /// </summary>
        /// <typeparam name="T">指定したクラス</typeparam>
        /// <param name="key">キー</param>
        /// <returns>登録済みの場合 true、それ以外は false</returns>
        public bool IsRegistered<T>(string key)
        {
            var classType = typeof(T);

            if (!this.InterfaceMap.ContainsKey(classType)
                || !this.Factories.ContainsKey(classType))
            {
                return false;
            }

            return this.Factories[classType].ContainsKey(key);
        }

        /// <summary>
        /// 指定したインタフェースをもつクラスをコンテナに登録します
        /// </summary>
        /// <typeparam name="TInterface">インタフェース</typeparam>
        /// <typeparam name="TClass">クラス</typeparam>
        public void Register<TInterface, TClass>()
            where TClass : class
            where TInterface : class
        {
            this.Register<TInterface, TClass>(false);
        }

        /// <summary>
        /// 指定したインタフェースをもつクラスをコンテナに登録します
        /// </summary>
        /// <typeparam name="TInterface">インタフェース</typeparam>
        /// <typeparam name="TClass">クラス</typeparam>
        /// <param name="createInstanceImmediately">直ちにインスタンス化する場合は true にします</param>
        public void Register<TInterface, TClass>(bool createInstanceImmediately)
            where TClass : class
            where TInterface : class
        {
            lock (this.LockObject)
            {
                var interfaceType = typeof(TInterface);
                var classType = typeof(TClass);

                if (!this.InterfaceMap.ContainsKey(interfaceType))
                {
                    this.InterfaceMap.Add(interfaceType, classType);
                    this.ConstructorInfos.Add(classType, GetConstructorInfo(classType));
                }

                Func<TInterface> factory = CreateInstance<TInterface>;
                this.Register(interfaceType, factory, this.DefaultKey);

                if (createInstanceImmediately)
                {
                    this.GetInstance<TInterface>();
                }
            }
        }

        /// <summary>
        /// 指定したクラスをコンテナに登録します
        /// </summary>
        /// <typeparam name="TClass">登録するクラス</typeparam>
        public void Register<TClass>()
            where TClass : class
        {
            this.Register<TClass>(false);
        }

        /// <summary>
        /// 指定したクラスをコンテナに登録します
        /// </summary>
        /// <typeparam name="TClass">登録するクラス</typeparam>
        /// <param name="createInstanceImmediately">直ちにインスタンス化する場合は true にします</param>
        public void Register<TClass>(bool createInstanceImmediately)
            where TClass : class
        {
            var classType = typeof(TClass);

            lock (this.LockObject)
            {
                if (!this.InterfaceMap.ContainsKey(classType))
                {
                    this.InterfaceMap.Add(classType, null);
                }

                this.ConstructorInfos.Add(classType, this.GetConstructorInfo(classType));
                Func<TClass> factory = CreateInstance<TClass>;
                this.Register(classType, factory, this.DefaultKey);

                if (createInstanceImmediately)
                {
                    this.GetInstance<TClass>();
                }
            }
        }

        #region Private Methods

        /// <summary>
        /// 指定したキーで指定したクラスをコンテナから取得します
        /// </summary>
        /// <param name="serviceType">クラスの型</param>
        /// <param name="key">キー</param>
        /// <returns>インスタンス</returns>
        private object GetService(Type serviceType, string key)
        {
            lock (this.LockObject)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = this.DefaultKey;
                }

                Dictionary<string, object> instances;

                if (!this.Registry.ContainsKey(serviceType))
                {
                    instances = new Dictionary<string, object>();
                    this.Registry.Add(serviceType, instances);
                }
                else
                {
                    instances = this.Registry[serviceType];
                }

                if (instances.ContainsKey(key))
                {
                    return instances[key];
                }

                object instance = null;

                if (this.Factories.ContainsKey(serviceType))
                {
                    if (this.Factories[serviceType].ContainsKey(key))
                    {
                        instance = this.Factories[serviceType][key].DynamicInvoke(null);
                    }
                    else
                    {
                        if (this.Factories[serviceType].ContainsKey(DefaultKey))
                        {
                            instance = this.Factories[serviceType][DefaultKey].DynamicInvoke(null);
                        }
                    }
                }
                instances.Add(key, instance);
                return instance;
            }
        }

        /// <summary>
        /// 指定したクラス、ファクトリ、キーをコンテナに登録します
        /// </summary>
        /// <typeparam name="TClass">クラス</typeparam>
        /// <param name="classType">クラスの型</param>
        /// <param name="factory">ファクトリメソッド</param>
        /// <param name="key">キー</param>
        private void Register<TClass>(Type classType, Func<TClass> factory, string key)
        {
            if (this.Factories.ContainsKey(classType))
            {
                if (this.Factories[classType].ContainsKey(key))
                {
                    return;
                }

                this.Factories[classType].Add(key, factory);
            }
            else
            {
                var list = new Dictionary<string, Delegate> { { key, factory } };
                this.Factories.Add(classType, list);
            }
        }

        /// <summary>
        /// 指定したクラスのコンストラクタ情報を取得します
        /// </summary>
        /// <param name="serviceType">クラスの型</param>
        /// <returns>コンストラクタ情報</returns>
        private ConstructorInfo GetConstructorInfo(Type serviceType)
        {
            Type resolveTo;

            if (this.InterfaceMap.ContainsKey(serviceType))
            {
                resolveTo = this.InterfaceMap[serviceType] ?? serviceType;
            }
            else
            {
                resolveTo = serviceType;
            }

            var constructorInfos = resolveTo.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic).ToArray();

            if (constructorInfos.Length > 1)
            {
                return constructorInfos.FirstOrDefault(i => i.Name != ".cctor");
            }

            return constructorInfos[0];
        }

        /// <summary>
        /// 指定したクラスのインスタンスを生成します
        /// </summary>
        /// <typeparam name="TClass">クラス</typeparam>
        /// <returns>インスタンス</returns>
        private TClass CreateInstance<TClass>()
        {
            var serviceType = typeof(TClass);

            var constructor = ConstructorInfos.ContainsKey(serviceType)
                                  ? ConstructorInfos[serviceType]
                                  : GetConstructorInfo(serviceType);

            var parameterInfos = constructor.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return (TClass)constructor.Invoke(EmptyArguments);
            }

            var parameters = new object[parameterInfos.Length];

            foreach (var parameterInfo in parameterInfos)
            {
                parameters[parameterInfo.Position] = GetService(parameterInfo.ParameterType, DefaultKey);
            }

            return (TClass)constructor.Invoke(parameters);
        }

        #endregion //Private Methods

        #region IServiceProvider

        /// <summary>
        /// 指定したクラスのすべてのインスタンスを取得します
        /// </summary>
        /// <param name="serviceType">クラスの型</param>
        /// <returns>インスタンス</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            lock (this.Factories)
            {
                if (this.Factories.ContainsKey(serviceType))
                {
                    foreach (var factory in Factories[serviceType])
                    {
                        this.GetInstance(serviceType, factory.Key);
                    }
                }
            }

            if (this.Registry.ContainsKey(serviceType))
            {
                return this.Registry[serviceType].Values;
            }
            return new List<object>();
        }

        /// <summary>
        /// 指定したクラスのすべてのインスタンスを取得します
        /// </summary>
        /// <typeparam name="TService">クラス</typeparam>
        /// <returns>インスタンス</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            var serviceType = typeof(TService);
            return this.GetAllInstances(serviceType)
                .Select(instance => (TService)instance);
        }

        /// <summary>
        /// 指定したクラスのインスタンスを取得します
        /// </summary>
        /// <param name="serviceType">クラスの型</param>
        /// <returns>インスタンス</returns>
        public object GetInstance(Type serviceType)
        {
            return this.GetService(serviceType, DefaultKey);
        }

        /// <summary>
        /// 指定したキーで指定したクラスのインスタンスを取得します
        /// </summary>
        /// <param name="serviceType">クラスの型</param>
        /// <param name="key">キー</param>
        /// <returns>インスタンス</returns>
        public object GetInstance(Type serviceType, string key)
        {
            return this.GetService(serviceType, key);
        }

        /// <summary>
        /// 指定したクラスのインスタンスを取得します
        /// </summary>
        /// <typeparam name="TService">クラス</typeparam>
        /// <returns>インスタンス</returns>
        public TService GetInstance<TService>()
        {
            return (TService)this.GetService(typeof(TService), DefaultKey);
        }

        /// <summary>
        /// 指定したキーで指定したクラスのインスタンスを取得します
        /// </summary>
        /// <typeparam name="TService">クラス</typeparam>
        /// <param name="key">キー</param>
        /// <returns>インスタンス</returns>
        public TService GetInstance<TService>(string key)
        {
            return (TService)this.GetService(typeof(TService), key);
        }

        #endregion //IServiceProvider
    }
}

