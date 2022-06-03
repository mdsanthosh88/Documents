using System;
using System.IdentityModel.Services;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XLC.MyAnalysis2.DbAccess.CustomEventArgs;
using static XLC.MyAnalysis2.DbModels.DbConstants.Constants;
using XLC.MyAnalysis2.DbModels;
using XLC.MyAnalysis2.Logic;
using XLC.MyAnalysis2.Shared;
using XLC.MyAnalysis2.TrackerEnabledDbContext.Configuration;
using XLC.MyAnalysis2.WebPortal.Custom_Binders;
using XLC.MyAnalysis2.WebPortal.DataAnnotations;
using XLC.MyAnalysis2.WebPortal.Helpers;
using XLC.MyAnalysis2.WebPortal.Hubs;

namespace XLC.MyAnalysis2.WebPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "XLC.MyAnalysis2.Resources.WebPageResources";
            DefaultModelBinder.ResourceClassKey = "XLC.MyAnalysis2.Resources.WebPageResources";

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LogHelper.Configure();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            LogHelper.Info($"Starting web app v{version}");

            AutoMapperConfig.Configure(new AutoMapperProfile());

            // Set the claim type for the unique identifier for the anti-forgery tokens
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ConfigHelper.GetAppSetting("EDS_UniqueClaimTypeIdentifier_Common");

            // Ensure that the initial system administrator user exists 
            UserHelper.EnsureInitialAdminUser();

            // Overcome max Json length limitation
            IncreaseMaxJsonLength();

            //Data annotations to allow for reading of values from resource file residing in another assembly.
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalisedRequired), typeof(RequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalisedRange), typeof(RangeAttributeAdapter));

            // Override default MVC validation of decimals - does not allow for comma separation of dec >= 1000
            ModelBinders.Binders.Add(typeof(decimal?), new NullableDecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            //Auditing of entity activity
            GlobalTrackingConfig.Enabled = false;
            EntityTracker.OverrideTracking<User>().Enable();
            EntityTracker.OverrideTracking<UserRole>().Enable();
            EntityTracker.OverrideTracking<UserAccountLevelAccess>().Enable();
            EntityTracker.OverrideTracking<UserDivisionLevelAccess>().Enable();
            EntityTracker.OverrideTracking<UserSubDivisionLevelAccess>().Enable();
            EntityTracker.OverrideTracking<UserLocationLevelAccess>().Enable();
            EntityTracker.OverrideTracking<UserDocumentAccess>().Enable();

            DBLanguageTables();

            // register notification and hook up event hander to published event
            if (ConfigHelper.GetAppSettingBool("DashboardAlertsEnabled"))
            {
                SqlDependencyLogicSingleton.Instance.DoAlert += dependency_OnChange;
                SqlDependencyLogicSingleton.Instance.RegisterNotification(DateTime.Now.ToUniversalTime(), DbModels.DbEnums.NotificationTypeEnum.External);
                SqlDependencyLogicSingleton.Instance.RegisterNotification(DateTime.Now.ToUniversalTime(), DbModels.DbEnums.NotificationTypeEnum.Internal);
            }

            // Subscribe to WSFAM events
            FederatedAuthentication.WSFederationAuthenticationModule.SignInError +=
                new EventHandler<ErrorEventArgs>(WSFederationAuthenticationModule_SignInError);

            FederatedAuthentication.WSFederationAuthenticationModule.SignedIn += WSFederationAuthenticationModule_SignedIn;
        }

        /// <summary>
        /// Handle the WIF User Signed In Event
        /// Record the date/time that the user logged in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WSFederationAuthenticationModule_SignedIn(object sender, EventArgs e)
        {
            LogHelper.Info("WSFederationAuthenticationModule_SignedIn event fired");
            var userLogic = new UserLogic();
            var userData = UserHelper.GetCurrentlyAuthenticatedUser(localDataOnly: false);

            // 158695 - verify User against xlExtAppID
            if(userData.HasEdsRecord)
            {
                userLogic.RecordUserLogin(userData.ID);
            }
            else
            {
                LogHelper.Info($"No valid EDS record found for user {userData.EdsCn}");
                userLogic.UpdateUserStatus(userData, UserStatus.Pending, userData.EdsCn);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            if (Context.User != null)
            {
                GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, UserHelper.GetCurrentUsersRoles().ToArray());
                Context.User = gp;
            }
        }

        private void IncreaseMaxJsonLength()
        {
            // Increase max Json length (see http://www.c-sharpcorner.com/blogs/handle-max-json-length-propert-in-c-sharp)
            foreach (var factory in ValueProviderFactories.Factories)
            {
                if (factory is JsonValueProviderFactory)
                {
                    ValueProviderFactories.Factories.Remove(factory as JsonValueProviderFactory);
                    break;
                }
            }
            ValueProviderFactories.Factories.Add(new CustomJsonValueProviderFactory());
        }

        /// <summary>
        /// FS-02.1/FR3.11 - push the notification to the users via the hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void dependency_OnChange(object sender, SqlDependencyEventArgs args)
        {
            DashboardHub hub = new DashboardHub();
            hub.Show(args);
        }

        /// <summary>
        /// The following Error Handler resolves the issue whereby the SAML token given by an ADFS
        /// server has expired due to unsynced clocks. This could be, for example, the case where a user logs in at 12AM,
        /// is given a token and does not log out. They re-open their laptop after the token has expired,
        /// i.e. the following morning at 10AM and the Token cannot be read. The issue originally appeared here:
        /// https://support.softwaresolved.com/browse/P1164I-171 
        /// 
        /// Solution adapted from https://stackoverflow.com/a/15919551 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WSFederationAuthenticationModule_SignInError(object sender, ErrorEventArgs e)
        {
            // handle an intermittent error that most often occurs if you are redirected to the STS after a session expired,
            // and the user clicks back on the browser - second error very uncommon but this should fix
            if (e.Exception.Message.StartsWith(WSFederationAuthentication_SignInErrorIDs.Error_RedirectToSTSAfterSessionExpired) ||
                e.Exception.Message.StartsWith(WSFederationAuthentication_SignInErrorIDs.Error_RedirectToSTSUserBrowsesBackwards) ||
                e.Exception.Message.StartsWith(WSFederationAuthentication_SignInErrorIDs.Error_NotOnOrAfterNotSatisfied) ||
                e.Exception.Message.StartsWith(WSFederationAuthentication_SignInErrorIDs.Error_NotOnOrBeforeNotSatisfied) ||
                e.Exception.Message.StartsWith(WSFederationAuthentication_SignInErrorIDs.Error_InvalidSession))
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
                e.Cancel = true;
                Response.Redirect(ConfigHelper.GetAppSetting("EDS_AccessXL_AdminPage"));
            }
        }

        private void DBLanguageTables()
        {

            EntityTracker.OverrideTracking<DataFilterFieldI18N>().Enable();
            EntityTracker.OverrideTracking<DataFilterGroupI18N>().Enable();
            EntityTracker.OverrideTracking<DocumentTypeI18N>().Enable();
            EntityTracker.OverrideTracking<FireDepartmentTypeI18N>().Enable();
            EntityTracker.OverrideTracking<Frequencyi18N>().Enable();
            EntityTracker.OverrideTracking<GroupTypeI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentRestorationMethodI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentSeverityI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentStatusI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentSystemTypeI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentTicketOriginTypeI18N>().Enable();
            EntityTracker.OverrideTracking<ImpairmentTypeI18N>().Enable();
            EntityTracker.OverrideTracking<IndustryClassificationI18N>().Enable();
            EntityTracker.OverrideTracking<LossTypeI18N>().Enable();
            EntityTracker.OverrideTracking<ManagementProgramI18N>().Enable();
            EntityTracker.OverrideTracking<NotificationOptionI18N>().Enable();
            EntityTracker.OverrideTracking<NotificationTypeI18N>().Enable();
            EntityTracker.OverrideTracking<OverallRecommendationStatusI18N>().Enable();
            EntityTracker.OverrideTracking<PredominantConstructionTypeI18N1>().Enable();
            EntityTracker.OverrideTracking<RatingTypeI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationCategoryI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationClientIntentI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationImpactI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationProbabilityI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationResponseStatusI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationSourceI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationStatusI18N>().Enable();
            EntityTracker.OverrideTracking<RecommendationTypeI18N>().Enable();
            EntityTracker.OverrideTracking<ReportTypeI18N>().Enable();
            EntityTracker.OverrideTracking<RiskQualityCategoryI18N>().Enable();
            EntityTracker.OverrideTracking<RiskQualityRatingTypeI18N>().Enable();
            EntityTracker.OverrideTracking<RiskQualityRatingTypeDetailI18N>().Enable();
            EntityTracker.OverrideTracking<SummaryReportTypeI18N>().Enable();
            EntityTracker.OverrideTracking<SystemGridFieldI18N>().Enable();
            EntityTracker.OverrideTracking<SystemGridGroupI18N>().Enable();
            EntityTracker.OverrideTracking<ValueSourceTypeI18N>().Enable();
            EntityTracker.OverrideTracking<WhatIfOptimisationTypeI18N>().Enable();
        }
        
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null ||
                        filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(new
                               System.Web.Routing.RouteValueDictionary(new { controller = "WhatIf", action = "Index", area = "" }));
            }
        }
    }
}
