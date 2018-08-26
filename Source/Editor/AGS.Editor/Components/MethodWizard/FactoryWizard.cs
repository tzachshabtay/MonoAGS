using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Editor
{
    public class FactoryWizard
    {
        private readonly AGSEditor _editor;
        private readonly Action<IPanel> _addUiExternal;
        private readonly Func<Dictionary<string, object>, Task<bool>> _validate;
        private readonly Action<Dictionary<string, object>> _setDefaults;
        private readonly IForm _parentForm;
        private readonly string _title;

        public FactoryWizard(IForm parentForm, string title, AGSEditor editor, Action<IPanel> addUiExternal, 
                             Func<Dictionary<string, object>, Task<bool>> validate,
                             Action<Dictionary<string, object>> setDefaults)
        {
            _title = title;
            _parentForm = parentForm;
            _editor = editor;
            _addUiExternal = addUiExternal;
            _validate = validate;
            _setDefaults = setDefaults;
        }

        public Task<(object result, MethodModel model, MethodWizardAttribute attr)> RunMethod(object factory, string methodName)
        {
            return run(factory, factory.GetType(), methodName);
        }

        public Task<(object result, MethodModel model, MethodWizardAttribute attr)> RunConstructor(Type factoryType)
        {
            return run(null, factoryType, null);
        }

        private async Task<(object result, MethodModel model, MethodWizardAttribute attr)> run(object factory, Type factoryType, string methodName)
        {
            var (method, methodAttribute) = getMethod(factoryType, methodName);
            HashSet<string> hideProperties = new HashSet<string>();
            Dictionary<string, object> overrideDefaults = new Dictionary<string, object>();
            foreach (var param in method.GetParameters())
            {
                var attr = param.GetCustomAttribute<MethodParamAttribute>();
                if (attr == null) continue;
                if (!attr.Browsable) hideProperties.Add(param.Name);
                if (attr.DefaultProvider != null)
                {
                    var resolver = _editor.GameResolver;
                    var provider = factoryType.GetMethod(attr.DefaultProvider);
                    if (provider == null)
                    {
                        throw new NullReferenceException($"Failed to find method with name: {attr.DefaultProvider ?? "null"}");
                    }
                    overrideDefaults[param.Name] = provider.Invoke(null, new[] { resolver });
                }
                else if (attr.Default != null) overrideDefaults[param.Name] = attr.Default;
            }

            _setDefaults?.Invoke(overrideDefaults);
            var wizard = new MethodWizard(_parentForm, _title, method, hideProperties, overrideDefaults, _addUiExternal, _editor, _validate);
            wizard.Load();
            var parameters = await wizard.ShowAsync();
            if (parameters == null) return (null, null, null);
            foreach (var param in overrideDefaults.Keys)
            {
                parameters[param] = get(param, parameters) ?? overrideDefaults[param];
            }
            (object result, MethodModel model) = runMethod(method, factory, parameters);
            return (result, model, methodAttribute);
        }

        private object get(string key, Dictionary<string, object> parameters) => parameters.TryGetValue(key, out var val) ? val : null;

        private (MethodBase, MethodWizardAttribute) getMethod(Type factoryType, string methodName)
        {
            if (methodName == null)
            {
                ConstructorInfo ctor = null;
                foreach (var method in factoryType.GetConstructors())
                {
                    if (ctor == null) ctor = method;
                    var attr = method.GetCustomAttribute<MethodWizardAttribute>();
                    if (attr == null) continue;
                    return (method, attr);
                }
                if (ctor != null) return (ctor, new MethodWizardAttribute());
                throw new InvalidOperationException($"Failed to find constructor in {factoryType}");
            }

            MethodInfo factory = null;
            foreach (var method in factoryType.GetMethods())
            {
                if (method.Name != methodName) continue;
                if (factory == null) factory = method;
                var attr = method.GetCustomAttribute<MethodWizardAttribute>();
                if (attr == null) continue;
                return (method, attr);
            }
            if (factory != null) return (factory, new MethodWizardAttribute());
            throw new InvalidOperationException($"Failed to find method name {methodName} (with MethodWizard attribute) in {factoryType}");
        }

        private (object, MethodModel) runMethod(MethodBase method, object factory, Dictionary<string, object> parameters)
        {
            var methodParams = method.GetParameters();
            object[] values = methodParams.Select(m => parameters.TryGetValue(m.Name, out object val) ?
                                                  val : MethodParam.GetDefaultValue(m.ParameterType)).ToArray();
            var returnType = method is MethodInfo methodInfo ? methodInfo.ReturnType : null;
            var model = new MethodModel { InstanceName = FactoryProvider.GetFactoryScriptName(factory, _editor.Game), Name = method.Name, Parameters = values, ReturnType = returnType };
            if (method is ConstructorInfo constructor) return (constructor.Invoke(values), model);
            return (method.Invoke(factory, values), model);
        }
    }
}