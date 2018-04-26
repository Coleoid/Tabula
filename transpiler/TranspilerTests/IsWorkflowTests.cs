using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    // built to check the Workflow detection heuristics
    //[TestFixture] //  Not part of the normal test suite
    //public class IsWorkflowAnalysis
    //{
    //    WorkflowIntrospector introspector;
    //    List<Type> types;
    //    List<Type> accepted;
    //    List<Type> rejected;
    //    List<string> pre_accepted;  //  as strings to allow statically building lists without
    //    List<string> pre_rejected;  //  needing to statically link to the dll with the types

    //    [SetUp]
    //    public void SetUp()
    //    {
    //        SetUp_earlier_decisions();
    //        introspector = new WorkflowIntrospector();
    //        types = introspector.GetLoadedTypes();

    //        accepted = new List<Type>();
    //        rejected = new List<Type>();

    //        foreach (var type in types)
    //        {
    //            (introspector.IsWorkflow(type) ? accepted : rejected).Add(type);
    //        }
    //    }

    //    public void SetUp_earlier_decisions()
    //    {
    //        pre_accepted = new List<string>
    //        {
    //            "AccountablePropertyAddEdit", "AccountablePropertyList", "AccountablePropertyRecord",
    //            "AddEditDeparture", "AddOrEditCertificationApplication", "AdminShortcuts", "AnimalEdit",
    //            "AnimalList", "AnimalRead", "ApprovedCourseTitleEdit", "ApprovedCourseTitleRecords",
    //            "AssessmentTestPersistence", "ATManagement", "AuditCertApplications", "AuditTrailManagement",
    //            "AutomatedEmailEdit", "AutomatedEmailExpiringCertificationNotification",
    //            "AutomatedEmailLessonPlanRequestStatusChange", "AutomatedEmailList", "AutomatedTestingSetup",
    //            "AutoUpdateClassStatusManagement", "AwardsManagement", "CertApplicationApproval",
    //            "CertApplicationsPendingAudit", "CertificateManagement", "CertificationApplicationList",
    //            "CertificationList", "CertificationManagementFNH", "CertificationVerification",
    //            "ClassCertsConferral", "ClassCertsSelection", "ClassPerformanceReports",
    //            "ClassPerformanceReportsAdditionalInfo", "ClassRosterReportView", "ClassSearch",
    //            "ClassSetup", "CompletedTrainingMonitor", "ComplianceDashboard", "ComplianceLeftMenu",
    //            "ComplianceSetup", "ConfigurationManagement", "ConflictManagement", "CsvManagement",
    //            "CurriculumFNHManagement", "CurriculumRecordAuthorizedInstructorsBand",
    //            "CurriculumRecordAwardsBand", "CurriculumRecordCertApplicationsBand",
    //            "CurriculumRecordClassBillingBand", "CurriculumRecordClassesBand",
    //            "CurriculumRecordClassSchedulingBand", "CurriculumRecordDescriptiveInfoBand",
    //            "CurriculumRecordHelpers", "CurriculumRecordScheduleTemplatesBand",
    //            "CurriculumRecordStaffAssignmentsBand", "CustomerViews", "CustomerViewWorkflowBase",
    //            "CustomViewHelpers", "DepartureEeocReportView", "DepartureReasonAndReasonDetailMaintenance",
    //            "DiscussionsManagement", "DivisionManagement", "DocumentImportVerification",
    //            "DuplicateSearch", "DynamicLabeling", "EditOrViewApplicationRequirements",
    //            "EmailCertificatePreferences", "EmailInstructorAssignmentsView", "EmailManagement",
    //            "EmploymentActionEdit", "EnrollmentExport", "FacilityPersistence", "FirearmEdit",
    //            "FirearmList", "FirearmRecord", "GlobalSettingManagement", "GradeManagement",
    //            "GradeManagementFNH", "GradingScaleManagement", "GradingScalePersistence",
    //            "HousingFacilityShortcuts", "HousingManagement", "HousingReservationShortcuts",
    //            "HousingResidentShortcuts", "HumanResourceImportManagement", "ImplementationHelper",
    //            "InquiryCheckBox", "InquiryFreeform", "InquiryQuestion", "InServiceTrainingManagement",
    //            "InstructorManagement", "InstructorPersistence", "InstructorSearch",
    //            "InventoryComplianceSearchResults", "InventorySearchResults", "IssueCertification",
    //            "IssuedCertificationDetail", "LearningObjectiveManagement", "Licensing", "ListManagement",
    //            "ListManagementFNH", "LookupManagement", "MaintenanceTicketBaselineHelper",
    //            "MissedTrainingReportManagement", "MostCreditedQuestion", "MultiPartTestPartPersistence",
    //            "MVBaseControlWorkflow`1", "MVBaseExportHandlerWorkflow`1", "MVBaseHandlerWorkflow`1",
    //            "MVBaseWorkflow`1", "MvcSkadooshWorkflow`1", "MVCViewModelPropertyBase",
    //            "NonTrainingDayManagement", "ObservedTestManagement", "OhioVFDFView",
    //            "OnlineContentManagement", "OnlineEventDetailView", "OnlineEventManagement",
    //            "OnlineEventRosterExport", "OnlineTestSimulator", "OrganizationCertExpirationUpdate",
    //            "OrganizationFNHManagement", "OrganizationManagement", "OrganizationSearchCriteria",
    //            "OrganizationSearching", "OrganizationSearchResults", "PeopleDashboard", "PerformanceMonitor",
    //            "PermissionUser", "PersonEmploymentFNHManagement", "PersonEmploymentManagement",
    //            "PersonFNHManagement", "PersonManagement", "PersonPerformanceTester", "PersonTags",
    //            "PortalCertificationRequest", "PortalCertificationResponse", "PortalManagement",
    //            "PortalManagementFNH", "PortalRegistrationManagement", "PortalSettingManagement",
    //            "PortalTrainingManagement", "PrintCertificatePreferences", "ProctorMonitor",
    //            "ProductMakeAndModelMaintenance", "PurgeHRRecords", "QuestionAndAnswerAnalysisReportManagement",
    //            "QuestionAndAnswerImportManagement", "QuestionPerformanceManagement", "QuestionSearch",
    //            "RegistrationManagement", "RejectedTrainingMonitor", "ReportManagement",
    //            "ResourceFNHManagement", "ResourceManagement", "RetestAndWaiverRules", "RoleManagement",
    //            "RulesManagement", "SanityChecking", "SavePointManagement", "ScenarioConfiguration",
    //            "ScheduleManagement", "SchedulingManagement", "SchedulingSetup", "SelectableInServiceRoster",
    //            "StudentManagement", "SurveyManagement", "TaskManagement", "TestManagement",
    //            "TestManagementFNH", "TestopiaFirstForwardDataServicesAcadisClient",
    //            "TestopiaFirstForwardDataServicesCertificationClient",
    //            "TestopiaFirstForwardDataServicesClassClient",
    //            "TestopiaFirstForwardDataServicesCourseClient",
    //            "TestopiaFirstForwardDataServicesOrganizationClient",
    //            "TestopiaFirstForwardDataServicesUserClient", "TestSetRetakeLimitPersistence",
    //            "TestStructureExport", "TrainingCategorizer", "TrainingEventManagement", "TrainingManagement",
    //            "TrainingSearchResultsView", "UpdateIssuedCertification", "UsageStatisticsManagement",
    //            "UserCreation", "UserManagementFNH", "VariableTrainingHoursEntryView", "VehicleEdit",
    //            "VehicleList", "VehicleReadOnly", "ViewCertificationApplication", "ViewPerformanceReport",
    //            "ViewProperty", "ViewVerification", "WisconsinCustomViews", "WrittenTestManagement",
    //            "XadrAcadisBasicTests", "XadrInvokeGetUserTests", "XadrTesterTests"
    //        };

    //        pre_rejected = new List<string>
    //        {
    //            "Acadis", "AcadisServiceHelper", "AccessDeniedException",
    //            "ActionEnhancedMVCModelProperty`1", "Actions", "ActivityAuditRecordType", "AddressData",
    //            "AdHocHelper", "AttributeContainer", "BrowsingSessionExtensions", "ButtonAttribute",
    //            "CertificationQualificationData", "CertificationQualificationDataType", "CheckBoxAttribute",
    //            "CsvExtensions", "DayOrWeek", "DebugAssertFailedException", "DropdownAttribute",
    //            "EmailAddressData", "EmployerAuthorityData", "EmploymentActionHandler", "EmploymentData",
    //            "EnabledDisabledAttribute", "EnumerableAssertionHelpers", "EnumScenarioExtensions",
    //            "ExceptionThrowingAssertListener", "ExportWorkflowHelper", "FeatureToggleAttribute",
    //            "FieldAttribute", "FormBuilderWorkflowHelper", "GeneratedScenarioBase",
    //            "GetPersonCompletedEventArgs", "GetPersonCompletedEventHandler", "HeightData",
    //            "HeightUnitType", "HooverImportHelper", "IImportBaseViewExtensions", "Iz", "LabelAttribute",
    //            "MaintenanceTicketResourceTypeAheadHelper", "MvcAction", "MVCViewModelProperty`1",
    //            "NavigatedAttribute", "OrderedCollectionEquivalentConstraint", "OtherIDDisplayMode",
    //            "Pass", "PersonnelReadinessRecord", "PhoneNumberData", "Portal", "PortalUser",
    //            "PortalUserAnswers", "RadioGroupAttribute", "RadioGroupOptionAttribute",
    //            "ReservationManagement", "RuntimeContext", "ScenarioContextExtensions", "ScenarioException",
    //            "Scope", "SelectListEnhancedModelProperty`1", "ServiceEnhancedMVCModelProperty`2",
    //            "SetUpAction", "Shared", "ShowHideAttribute", "SiteBrowsingSession", "StatisticsDto",
    //            "Table", "TearDownAction", "TestopiaAcadisOutsideFactory", "TestopiaAcadisService",
    //            "TestopiaBindings", "TestopiaChannelWrapper`1", "TestopiaConfigurationManager",
    //            "TestopiaConnectionManager", "TestopiaDataServicesClientFactory",
    //            "TestopiaDataServicesClientProxy", "TestopiaDateTimeProvider",
    //            "TestopiaDocumentStorageProviderFactory", "TestopiaEmailSender", "TestopiaFilterView",
    //            "TestopiaFirstForwardDataServiceClientFactory", "TestopiaHttpContextHelper",
    //            "TestopiaImportMessageHandler", "TestopiaMessageQueue", "TestopiaMessengerFactory",
    //            "TestopiaMonitor", "TestopiaMvcView`1", "TestopiaObjectBuilder", "TestopiaPerson",
    //            "TestopiaPostedFile", "TestopiaReadonlyDBConnectionFactory", "TestopiaSmtpClient",
    //            "TestopiaSmtpClientFactory", "TestopiaStudentListRow", "TestopiaThreadContext",
    //            "TestopiaUrlResolver", "TestopiaUserContext", "TestopiaUserDetailEditor", "TextAttribute",
    //            "TrainingData", "TrainingEventTestHelpers", "TuitionBilling_generated", "WeightData",
    //            "WeightUnitType", "WorkflowProperty"
    //        };
    //        var pre_rejected_unsure = new List<string>
    //        {
    //            "AcademyRecords",
    //        };
    //    }

    //    [Test]
    //    public void the_pre_accepted_and_pre_rejected_lists_are_disjoint()
    //    {
    //        var intersection = pre_accepted.Where(a => pre_rejected.Contains(a));
    //        Assert.That(intersection.Count(), Is.EqualTo(0),
    //            $"Sallah, I said _no_ camels!  That's {intersection.Count()}!  Can't you count?" );
    //    }

    //    [Test]
    //    public void the_current_accepts_should_include_no_pre_rejected_types()
    //    {
    //        var accepted_bad_types = accepted.Where(a => pre_rejected.Contains(a.Name));
    //        Assert.That(accepted_bad_types.Count(), Is.EqualTo(0));
    //    }

    //    [Test]
    //    public void the_current_rejects_should_include_no_pre_accepted_types()
    //    {
    //        var rejected_good_types = rejected.Where(r => pre_accepted.Contains(r.Name));
    //        Assert.That(rejected_good_types.Count(), Is.EqualTo(0));
    //    }

    //    [Test]
    //    public void the_current_accepts_have_all_been_vetted_correct()
    //    {
    //        var accepted_unvetted = accepted.Where(a => !pre_accepted.Contains(a.Name));
    //        var au_names = accepted_unvetted
    //            .Where(au => !au.Name.EndsWith("Workflow"))
    //            .OrderBy(au => au.Name).Select(au => au.Name).ToList();
    //        var names_text = string.Join("\", \"", au_names);
    //        Assert.That(au_names.Count(), Is.EqualTo(0));
    //    }

    //    [Test]
    //    public void the_current_rejects_have_all_been_vetted_correct()
    //    {
    //        var rejected_unvetted = rejected.Where(r => !pre_rejected.Contains(r.Name));
    //        var ru_names = rejected_unvetted
    //            .Where(ru => !ru.Name.EndsWith("Workflow"))
    //            .OrderBy(ru => ru.Name).Select(ru => ru.Name).ToList();
    //        var names_text = string.Join("\", \"", ru_names);
    //        Assert.That(rejected_unvetted.Count(), Is.EqualTo(0));
    //    }
    //}
}
