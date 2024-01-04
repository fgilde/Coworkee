using System;
using RazorEngineCore;
using Coworkee.Shared;
using System.Collections.Generic;

namespace Coworkee.Infrastructure.Models;

public class EmailTemplateCompiled
{
    public EmailTemplate Template { get; }
    private readonly IServiceProvider _serviceProvider;
    private readonly IRazorEngineCompiledTemplate<RazorEngineEmailTemplate> compiledTemplate;
    private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorEngineEmailTemplate>> compiledParts;

    public EmailTemplateCompiled(EmailTemplate template, IServiceProvider serviceProvider,
        IRazorEngineCompiledTemplate<RazorEngineEmailTemplate> compiledTemplate, Dictionary<string, IRazorEngineCompiledTemplate<RazorEngineEmailTemplate>> compiledParts)
    {
        Template = template;
        _serviceProvider = serviceProvider;
        this.compiledTemplate = compiledTemplate;
        this.compiledParts = compiledParts;
    }

    public string Run(object model)
    {
        return Run(compiledTemplate, model);
    }

    public string Run(IRazorEngineCompiledTemplate<RazorEngineEmailTemplate> template, object model)
    {
        RazorEngineEmailTemplate templateReference = null;

        string result = template.Run(instance =>
        {
            if (!(model is AnonymousTypeWrapper))
            {
                model = new AnonymousTypeWrapper(model);
            }

            instance.Model = model;
            instance.IncludeCallback = (key, includeModel) => Run(compiledParts[key], includeModel);
            instance.Initialize(_serviceProvider);
            templateReference = instance;
        });

        if (templateReference.Layout == null)
        {
            return result;
        }

        return compiledParts[templateReference.Layout].Run(instance =>
        {
            if (!(model is AnonymousTypeWrapper))
            {
                model = new AnonymousTypeWrapper(model);
            }

            instance.Model = model;
            instance.IncludeCallback = (key, includeModel) => Run(compiledParts[key], includeModel);
            instance.Initialize(_serviceProvider);
            instance.RenderBodyCallback = () => result;
        });
    }
}