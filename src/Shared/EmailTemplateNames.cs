using System.Globalization;

namespace Coworkee.Shared;

public partial class EmailTemplate
{    
    public static class Names
    {
        public const string Activated = "Activated.cshtml";      
        public const string Footer = "Footer.cshtml";      
        public const string ForgotPassword = "Forgot-Password.cshtml";      
        public const string Register = "Register.cshtml";      
        public const string Layout = "Layout.cshtml";      
    }

    public static EmailTemplate Activated(CultureInfo culture = null) => Get(Names.Activated, culture); 
    public static EmailTemplate Activated(string culture) => Get(Names.Activated, culture); 
    public static EmailTemplate Footer(CultureInfo culture = null) => Get(Names.Footer, culture); 
    public static EmailTemplate Footer(string culture) => Get(Names.Footer, culture); 
    public static EmailTemplate ForgotPassword(CultureInfo culture = null) => Get(Names.ForgotPassword, culture); 
    public static EmailTemplate ForgotPassword(string culture) => Get(Names.ForgotPassword, culture); 
    public static EmailTemplate Register(CultureInfo culture = null) => Get(Names.Register, culture); 
    public static EmailTemplate Register(string culture) => Get(Names.Register, culture); 
    public static EmailTemplate Layout(CultureInfo culture = null) => Get(Names.Layout, culture); 
    public static EmailTemplate Layout(string culture) => Get(Names.Layout, culture); 
        
    // Nested classes for each culture
    public static class DE_DE
    {
            public static EmailTemplate Activated => Get(EmailTemplate.Names.Activated, "de-DE");
            public static EmailTemplate Footer => Get(EmailTemplate.Names.Footer, "de-DE");
            public static EmailTemplate ForgotPassword => Get(EmailTemplate.Names.ForgotPassword, "de-DE");
            public static EmailTemplate Register => Get(EmailTemplate.Names.Register, "de-DE");
        
    }

    public static class EN_US
    {
            public static EmailTemplate Activated => Get(EmailTemplate.Names.Activated, "en-US");
            public static EmailTemplate Footer => Get(EmailTemplate.Names.Footer, "en-US");
            public static EmailTemplate ForgotPassword => Get(EmailTemplate.Names.ForgotPassword, "en-US");
            public static EmailTemplate Register => Get(EmailTemplate.Names.Register, "en-US");
        
    }

    public static class SHARED
    {
            public static EmailTemplate Layout => Get(EmailTemplate.Names.Layout, "Shared");
        
    }

        
}