using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public class TuitionBilling_2_generated
        : GeneratedScenarioBase, IGeneratedScenario
    {
        public ScenarioContext.Implementations.Organizations.OrganizationFNHManagement OrganizationFNHManagement;
        public ScenarioContext.Implementations.Curriculum.CurriculumFNHManagement CurriculumFNHManagement;
        public ScenarioContext.ViewImplementations.ClassTemplateAddEditWorkflow ClassTemplateAddEdit;
        public ScenarioContext.ViewImplementations.Curriculum.EditProgramWorkflow EditProgram;
        public ScenarioContext.ViewImplementations.Curriculum.EditClassWorkflow EditClass;
        public ScenarioContext.Implementations.Registration.AddEditClassRegistrationWorkflow AddEditClassRegistration;
        public ScenarioContext.ViewImplementations.Curriculum.ClassDescriptiveFieldsWorkflow ClassDescriptiveFields;
        public ScenarioContext.Implementations.People.PersonFNHManagement PersonFNHManagement;
        public ScenarioContext.ViewImplementations.Administration.BillingPreferencesWorkflow BillingPreferences;
        public ScenarioContext.ViewImplementations.Registration.RegisterStudentWorkflow RegisterStudent;
        public ScenarioContext.ViewImplementations.Administration.BillingItemsExportWorkflow BillingItemsExport;
        public ScenarioContext.Implementations.Administration.TaskRunnerWorkflow TaskRunner;
        public ScenarioContext.ViewImplementations.Registration.TransferStudentWorkflow TransferStudent;
        public ScenarioContext.ViewImplementations.Curriculum.AddEnrollmentWorkflow AddEnrollment;
        public ScenarioContext.Implementations.AcadisDataServices.AccountingApiWorkflow AccountingApi;
        public ScenarioContext.ViewImplementations.Curriculum.StudentListWorkflow StudentList;
        public ScenarioContext.People.PersonManagement PersonManagement;

        public TuitionBilling_2_generated()
            : base()
        {
            ScenarioLabel = @"TuitionBilling_2.tab:  ""﻿AC-20439: Tuition billing""";
            OrganizationFNHManagement = new ScenarioContext.Implementations.Organizations.OrganizationFNHManagement();
            CurriculumFNHManagement = new ScenarioContext.Implementations.Curriculum.CurriculumFNHManagement();
            ClassTemplateAddEdit = new ScenarioContext.ViewImplementations.ClassTemplateAddEditWorkflow();
            EditProgram = new ScenarioContext.ViewImplementations.Curriculum.EditProgramWorkflow();
            EditClass = new ScenarioContext.ViewImplementations.Curriculum.EditClassWorkflow();
            AddEditClassRegistration = new ScenarioContext.Implementations.Registration.AddEditClassRegistrationWorkflow();
            ClassDescriptiveFields = new ScenarioContext.ViewImplementations.Curriculum.ClassDescriptiveFieldsWorkflow();
            PersonFNHManagement = new ScenarioContext.Implementations.People.PersonFNHManagement();
            BillingPreferences = new ScenarioContext.ViewImplementations.Administration.BillingPreferencesWorkflow();
            RegisterStudent = new ScenarioContext.ViewImplementations.Registration.RegisterStudentWorkflow();
            BillingItemsExport = new ScenarioContext.ViewImplementations.Administration.BillingItemsExportWorkflow();
            TaskRunner = new ScenarioContext.Implementations.Administration.TaskRunnerWorkflow();
            TransferStudent = new ScenarioContext.ViewImplementations.Registration.TransferStudentWorkflow();
            AddEnrollment = new ScenarioContext.ViewImplementations.Curriculum.AddEnrollmentWorkflow();
            AccountingApi = new ScenarioContext.Implementations.AcadisDataServices.AccountingApiWorkflow();
            StudentList = new ScenarioContext.ViewImplementations.Curriculum.StudentListWorkflow();
            PersonManagement = new ScenarioContext.People.PersonManagement();
        }

        public void ExecuteScenario()
        {
            paragraph_from_003_to_008();
            paragraph_from_010_to_016();
            paragraph_from_018_to_021();
            Run_para_over_table( paragraph_from_023_to_026, table_from_027_to_031 );
            paragraph_from_033_to_037();
            Run_para_over_table( paragraph_from_039_to_045, table_from_046_to_050 );
            Run_para_over_table( paragraph_from_052_to_054, table_from_055_to_067 );
            paragraph_from_070_to_075();
            paragraph_from_077_to_083();
            paragraph_from_088_to_092();
            paragraph_from_094_to_097();
            paragraph_from_099_to_101();
            paragraph_from_103_to_106();
            paragraph_from_108_to_110();
            paragraph_from_112_to_115();
            paragraph_from_120_to_129();
            paragraph_from_131_to_136();
            paragraph_from_138_to_141();
            paragraph_from_143_to_147();
            paragraph_from_149_to_152();
            paragraph_from_154_to_159();
            paragraph_from_161_to_164();
            paragraph_from_166_to_168();
            paragraph_from_170_to_173();
            paragraph_from_175_to_177();
            paragraph_from_179_to_182();
            paragraph_from_186_to_196();
            paragraph_from_199_to_204();
            paragraph_from_206_to_209();
            paragraph_from_211_to_213();
            paragraph_from_215_to_218();
            paragraph_from_220_to_225();
            paragraph_from_227_to_230();
            Run_para_over_table( paragraph_from_232_to_244, table_from_246_to_251 );
            paragraph_from_253_to_255();
            paragraph_from_257_to_260();
            paragraph_from_262_to_265();
            Run_para_over_table( paragraph_from_267_to_277, table_from_279_to_284 );
            paragraph_from_288_to_294();
            paragraph_from_296_to_298();
            paragraph_from_300_to_303();
            Run_para_over_table( paragraph_from_305_to_308, table_from_310_to_315 );
            paragraph_from_318_to_323();
            paragraph_from_325_to_334();
            paragraph_from_336_to_341();
            paragraph_from_343_to_348();
            paragraph_from_350_to_360();
            paragraph_from_362_to_367();
            paragraph_from_369_to_374();
            paragraph_from_376_to_381();
            paragraph_from_383_to_385();
            paragraph_from_387_to_390();
            paragraph_from_392_to_394();
            paragraph_from_396_to_401();
            paragraph_from_403_to_413();
            paragraph_from_415_to_419();
            paragraph_from_421_to_423();
            paragraph_from_426_to_429();
            Run_para_over_table( paragraph_from_431_to_446, table_from_448_to_459 );
            paragraph_from_462_to_625();
        }

        public void paragraph_from_003_to_008()
        {
            Label(  "Setup - create a training academy and an organization" );
            Do(() =>     ClassTemplateAddEdit.Set_today_to_("1/1/2023"),     "TuitionBilling_2.tab:5", @"ClassTemplateAddEdit.Set_today_to_(""1/1/2023"")" );
            Do(() =>     OrganizationFNHManagement.Create_academy_named_("Testopia Academy"),     "TuitionBilling_2.tab:6", @"OrganizationFNHManagement.Create_academy_named_(""Testopia Academy"")" );
            Do(() =>     OrganizationFNHManagement.Create_organization_named_("Local Police Department"),     "TuitionBilling_2.tab:7", @"OrganizationFNHManagement.Create_organization_named_(""Local Police Department"")" );
            Do(() =>     CurriculumFNHManagement.Create_program_category__("Training"),     "TuitionBilling_2.tab:8", @"CurriculumFNHManagement.Create_program_category__(""Training"")" );
        }

        public void paragraph_from_010_to_016()
        {
            Label(  "Create a new program" );
            Do(() =>     EditProgram.Browse_to_page_from_organization__("Testopia Academy"),     "TuitionBilling_2.tab:12", @"EditProgram.Browse_to_page_from_organization__(""Testopia Academy"")" );
            Do(() =>     EditProgram.Set_program_name_to__("Driving"),     "TuitionBilling_2.tab:13", @"EditProgram.Set_program_name_to__(""Driving"")" );
            Do(() =>     EditProgram.Select__as_category("Training"),     "TuitionBilling_2.tab:14", @"EditProgram.Select__as_category(""Training"")" );
            Do(() =>     EditProgram.Set_program__active("is"),     "TuitionBilling_2.tab:15", @"EditProgram.Set_program__active(""is"")" );
            Do(() =>     EditProgram.Save(),     "TuitionBilling_2.tab:16", @"EditProgram.Save()" );
        }

        public void paragraph_from_018_to_021()
        {
            Label(  "Setup - create a training academy and an organization" );
            Do(() =>     ClassTemplateAddEdit.Create_class_template_named__in_program__for_organization__("Highway Pursuit", "Driving", "Testopia Academy"),     "TuitionBilling_2.tab:20", @"ClassTemplateAddEdit.Create_class_template_named__in_program__for_organization__(""Highway Pursuit"", ""Driving"", ""Testopia Academy"")" );
            Do(() =>     CurriculumFNHManagement.Validate_Class_Template_named__in_Program__("Highway Pursuit", "Driving"),     "TuitionBilling_2.tab:21", @"CurriculumFNHManagement.Validate_Class_Template_named__in_Program__(""Highway Pursuit"", ""Driving"")" );
        }

        public void paragraph_from_023_to_026()
        {
            Label(  "Setup - Add a new class" );
            Do(() =>     EditClass.Browse_to_Page(),     "TuitionBilling_2.tab:25", @"EditClass.Browse_to_Page()" );
            Do(() =>     EditClass.Create_Class_named__from__starting__ending_(var["name"], "Highway Pursuit", "01/1/2023", var["endDate"]),     "TuitionBilling_2.tab:26", @"EditClass.Create_Class_named__from__starting__ending_(var[""name""], ""Highway Pursuit"", ""01/1/2023"", var[""endDate""])" );
        }

        public Table table_from_027_to_031()
        {
            return new Table {
                Header = new List<string>         { "Name", "End Date" },
                Data = new List<List<string>> {
                    new List<string>              { "Highway Pursuit 100", "1/1/2023" },
                    new List<string>              { "Highway Pursuit 101", "1/1/2023" },
                    new List<string>              { "Highway Pursuit 102", "2/1/2023" },
                    new List<string>              { "Highway Pursuit 103", "2/1/2023" },
                }
            };
        }

        public void paragraph_from_033_to_037()
        {
            Label(  "Setup class registrations" );
            Do(() =>     AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max("Highway Pursuit 100", "0", "0", "10"),     "TuitionBilling_2.tab:35", @"AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max(""Highway Pursuit 100"", ""0"", ""0"", ""10"")" );
            Do(() =>     AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max("Highway Pursuit 101", "0", "0", "10"),     "TuitionBilling_2.tab:36", @"AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max(""Highway Pursuit 101"", ""0"", ""0"", ""10"")" );
            Do(() =>     AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max("Highway Pursuit 102", "0", "0", "10"),     "TuitionBilling_2.tab:37", @"AddEditClassRegistration.Create_class_registration_for__to_start__days_before_and_end__days_after_with__students_max(""Highway Pursuit 102"", ""0"", ""0"", ""10"")" );
        }

        public void paragraph_from_039_to_045()
        {
            Label(  "Set and verify Tuitions" );
            Do(() =>     ClassDescriptiveFields.Browse_to_Page_for_class(var["name"]),     "TuitionBilling_2.tab:41", @"ClassDescriptiveFields.Browse_to_Page_for_class(var[""name""])" );
            Do(() =>     ClassDescriptiveFields.Enter_text__for__(var["fees"], "Fees"),     "TuitionBilling_2.tab:42", @"ClassDescriptiveFields.Enter_text__for__(var[""fees""], ""Fees"")" );
            Do(() =>     ClassDescriptiveFields.Click_button__("Done"),     "TuitionBilling_2.tab:43", @"ClassDescriptiveFields.Click_button__(""Done"")" );
            Do(() =>     ClassDescriptiveFields.Browse_to_Page_for_class(var["name"]),     "TuitionBilling_2.tab:44", @"ClassDescriptiveFields.Browse_to_Page_for_class(var[""name""])" );
            Do(() =>     ClassDescriptiveFields.Verify_text_for__is__("Fees", var["fees"]),     "TuitionBilling_2.tab:45", @"ClassDescriptiveFields.Verify_text_for__is__(""Fees"", var[""fees""])" );
        }

        public Table table_from_046_to_050()
        {
            return new Table {
                Header = new List<string>         { "Name", "Fees" },
                Data = new List<List<string>> {
                    new List<string>              { "Highway Pursuit 100", "101.42" },
                    new List<string>              { "Highway Pursuit 101", "131.45" },
                    new List<string>              { "Highway Pursuit 102", "152.55" },
                    new List<string>              { "Highway Pursuit 103", "172.55" },
                }
            };
        }

        public void paragraph_from_052_to_054()
        {
            Label(  "Create people" );
            Do(() =>     PersonFNHManagement.Create_person_with_academy_id__(var["name"], var["id"]),     "TuitionBilling_2.tab:54", @"PersonFNHManagement.Create_person_with_academy_id__(var[""name""], var[""id""])" );
        }

        public Table table_from_055_to_067()
        {
            return new Table {
                Header = new List<string>         { "Name", "ID" },
                Data = new List<List<string>> {
                    new List<string>              { "Agmeier, Armin", "A1" },
                    new List<string>              { "Bradley, Blake", "B1" },
                    new List<string>              { "Brown, Wendell", "B2" },
                    new List<string>              { "Chase, Charles", "C1" },
                    new List<string>              { "Doo, Dewbie", "D1" },
                    new List<string>              { "Easley, Erwin", "E1" },
                    new List<string>              { "Eady, Forrest", "E2" },
                    new List<string>              { "Fairchild, Florence", "F1" },
                    new List<string>              { "Johnson, Danny", "J1" },
                    new List<string>              { "Murphy, Sam", "M1" },
                    new List<string>              { "Nicholas, Jamila", "N1" },
                    new List<string>              { "Worthen, Jorge", "W1" },
                }
            };
        }

        public void paragraph_from_070_to_075()
        {
            Label(  "Enable billing" );
            Do(() =>     BillingPreferences.Browse_to_Page(),     "TuitionBilling_2.tab:72", @"BillingPreferences.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Choose_to_track_billing(),     "TuitionBilling_2.tab:73", @"BillingPreferences.Choose_to_track_billing()" );
            Do(() =>     BillingPreferences.Prepare_to_save(),     "TuitionBilling_2.tab:74", @"BillingPreferences.Prepare_to_save()" );
            Do(() =>     BillingPreferences.Click_save(),     "TuitionBilling_2.tab:75", @"BillingPreferences.Click_save()" );
        }

        public void paragraph_from_077_to_083()
        {
            Label(  "Disable billing for class tuition" );
            Do(() =>     BillingPreferences.Browse_to_Page(),     "TuitionBilling_2.tab:79", @"BillingPreferences.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:80", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_not_to_track_category(),     "TuitionBilling_2.tab:81", @"BillingPreferences.Choose_not_to_track_category()" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:82", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:83", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_088_to_092()
        {
            Label(  "Register person A" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:90", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     RegisterStudent.Select_student__("Agmeier, Armin"),     "TuitionBilling_2.tab:91", @"RegisterStudent.Select_student__(""Agmeier, Armin"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:92", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_094_to_097()
        {
            Label(  "Verify no bill was created at registration" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:96", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("0"),     "TuitionBilling_2.tab:97", @"BillingItemsExport.Verify_there_are__items(""0"")" );
        }

        public void paragraph_from_099_to_101()
        {
            Label(  "Enroll person A" );
            Unfound(     "Enroll \"Agmeier, Armin\" in  \"Highway Pursuit 100\" from registration",     "TuitionBilling_2.tab:101" );
        }

        public void paragraph_from_103_to_106()
        {
            Label(  "Verify no bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:105", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("0"),     "TuitionBilling_2.tab:106", @"BillingItemsExport.Verify_there_are__items(""0"")" );
        }

        public void paragraph_from_108_to_110()
        {
            Label(  "Complete class" );
            Do(() =>     TaskRunner.Run_task_("AutoUpdateClassStatus"),     "TuitionBilling_2.tab:110", @"TaskRunner.Run_task_(""AutoUpdateClassStatus"")" );
        }

        public void paragraph_from_112_to_115()
        {
            Label(  "Verify no bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:114", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("0"),     "TuitionBilling_2.tab:115", @"BillingItemsExport.Verify_there_are__items(""0"")" );
        }

        public void paragraph_from_120_to_129()
        {
            Label(  "Change billing to at registration" );
            Do(() =>     RegisterStudent.Browse_to_Page(),     "TuitionBilling_2.tab:122", @"RegisterStudent.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:123", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_to_track_category(),     "TuitionBilling_2.tab:124", @"BillingPreferences.Choose_to_track_category()" );
            Do(() =>     BillingPreferences.Choose_to_create_billing_information__("At registration"),     "TuitionBilling_2.tab:125", @"BillingPreferences.Choose_to_create_billing_information__(""At registration"")" );
            Do(() =>     BillingPreferences.Choose_to_calculate_billing_as_a_variable_fee(),     "TuitionBilling_2.tab:126", @"BillingPreferences.Choose_to_calculate_billing_as_a_variable_fee()" );
            Do(() =>     BillingPreferences.Choose__to_be_responsible_for_fees("Individual"),     "TuitionBilling_2.tab:127", @"BillingPreferences.Choose__to_be_responsible_for_fees(""Individual"")" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:128", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:129", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_131_to_136()
        {
            Label(  "Register Bradley, Blake" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:133", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     RegisterStudent.Select_student__("Bradley, Blake"),     "TuitionBilling_2.tab:134", @"RegisterStudent.Select_student__(""Bradley, Blake"")" );
            Do(() =>     RegisterStudent.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:135", @"RegisterStudent.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:136", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_138_to_141()
        {
            Label(  "Verify bill was created at registration" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:140", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("1"),     "TuitionBilling_2.tab:141", @"BillingItemsExport.Verify_there_are__items(""1"")" );
        }

        public void paragraph_from_143_to_147()
        {
            Label(  "Transfer Bradley, Blake" );
            Do(() =>     TransferStudent.Browse_to_transfer_registration_for__from_class__("Bradley, Blake", "Highway Pursuit 100"),     "TuitionBilling_2.tab:145", @"TransferStudent.Browse_to_transfer_registration_for__from_class__(""Bradley, Blake"", ""Highway Pursuit 100"")" );
            Do(() =>     TransferStudent.Select__to_transfer_to("Highway Pursuit 101"),     "TuitionBilling_2.tab:146", @"TransferStudent.Select__to_transfer_to(""Highway Pursuit 101"")" );
            Do(() =>     TransferStudent.Click_Transfer(),     "TuitionBilling_2.tab:147", @"TransferStudent.Click_Transfer()" );
        }

        public void paragraph_from_149_to_152()
        {
            Label(  "Verify bill was created at registration transfer" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:151", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("2"),     "TuitionBilling_2.tab:152", @"BillingItemsExport.Verify_there_are__items(""2"")" );
        }

        public void paragraph_from_154_to_159()
        {
            Label(  "Enroll Nicholas, Jamila directly in course without registration" );
            Do(() =>     AddEnrollment.Browse_to_page_for__("Highway Pursuit 103"),     "TuitionBilling_2.tab:156", @"AddEnrollment.Browse_to_page_for__(""Highway Pursuit 103"")" );
            Do(() =>     AddEnrollment.Select__to_enroll("Nicholas, Jamila"),     "TuitionBilling_2.tab:157", @"AddEnrollment.Select__to_enroll(""Nicholas, Jamila"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:158", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     AddEnrollment.Click_Continue(),     "TuitionBilling_2.tab:159", @"AddEnrollment.Click_Continue()" );
        }

        public void paragraph_from_161_to_164()
        {
            Label(  "Verify bill was created at registration transfer" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:163", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("3"),     "TuitionBilling_2.tab:164", @"BillingItemsExport.Verify_there_are__items(""3"")" );
        }

        public void paragraph_from_166_to_168()
        {
            Label(  "Bradley, Blake" );
            Unfound(     "Enroll \"Bradley, Blake\" in \"Highway Pursuit 101\" from registration",     "TuitionBilling_2.tab:168" );
        }

        public void paragraph_from_170_to_173()
        {
            Label(  "Verify no bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:172", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("3"),     "TuitionBilling_2.tab:173", @"BillingItemsExport.Verify_there_are__items(""3"")" );
        }

        public void paragraph_from_175_to_177()
        {
            Label(  "Complete class" );
            Do(() =>     TaskRunner.Run_task_("AutoUpdateClassStatus"),     "TuitionBilling_2.tab:177", @"TaskRunner.Run_task_(""AutoUpdateClassStatus"")" );
        }

        public void paragraph_from_179_to_182()
        {
            Label(  "Verify no bill was created at class completion" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:181", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("3"),     "TuitionBilling_2.tab:182", @"BillingItemsExport.Verify_there_are__items(""3"")" );
        }

        public void paragraph_from_186_to_196()
        {
            Label(  "Change billing to at enrollment with flat fee" );
            Do(() =>     AddEnrollment.Browse_to_Page(),     "TuitionBilling_2.tab:188", @"AddEnrollment.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:189", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_to_track_category(),     "TuitionBilling_2.tab:190", @"BillingPreferences.Choose_to_track_category()" );
            Do(() =>     BillingPreferences.Choose_to_create_billing_information__("At enrollment"),     "TuitionBilling_2.tab:191", @"BillingPreferences.Choose_to_create_billing_information__(""At enrollment"")" );
            Do(() =>     BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee(),     "TuitionBilling_2.tab:192", @"BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee()" );
            Do(() =>     BillingPreferences.Enter__as_a_flat_fee("99.99"),     "TuitionBilling_2.tab:193", @"BillingPreferences.Enter__as_a_flat_fee(""99.99"")" );
            Do(() =>     BillingPreferences.Choose__to_be_responsible_for_fees("Individual"),     "TuitionBilling_2.tab:194", @"BillingPreferences.Choose__to_be_responsible_for_fees(""Individual"")" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:195", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:196", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_199_to_204()
        {
            Label(  "Register C" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:201", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     RegisterStudent.Select_student__("Chase, Charles"),     "TuitionBilling_2.tab:202", @"RegisterStudent.Select_student__(""Chase, Charles"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:203", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:204", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_206_to_209()
        {
            Label(  "Verify no bill was created at registration" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:208", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("3"),     "TuitionBilling_2.tab:209", @"BillingItemsExport.Verify_there_are__items(""3"")" );
        }

        public void paragraph_from_211_to_213()
        {
            Label(  "Enroll person C" );
            Unfound(     "Enroll \"Chase, Charles\" in \"Highway Pursuit 100\" from registration",     "TuitionBilling_2.tab:213" );
        }

        public void paragraph_from_215_to_218()
        {
            Label(  "Verify bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:217", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("4"),     "TuitionBilling_2.tab:218", @"BillingItemsExport.Verify_there_are__items(""4"")" );
        }

        public void paragraph_from_220_to_225()
        {
            Label(  "Enroll person D directly" );
            Do(() =>     AddEnrollment.Browse_to_page_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:222", @"AddEnrollment.Browse_to_page_for__(""Highway Pursuit 100"")" );
            Do(() =>     AddEnrollment.Select__to_enroll("Doo, Dewbie"),     "TuitionBilling_2.tab:223", @"AddEnrollment.Select__to_enroll(""Doo, Dewbie"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:224", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     AddEnrollment.Click_Continue(),     "TuitionBilling_2.tab:225", @"AddEnrollment.Click_Continue()" );
        }

        public void paragraph_from_227_to_230()
        {
            Label(  "Verify bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:229", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("5"),     "TuitionBilling_2.tab:230", @"BillingItemsExport.Verify_there_are__items(""5"")" );
        }

        public void paragraph_from_232_to_244()
        {
            Do(() =>     BillingItemsExport.Verify_row__(var["num"]),     "TuitionBilling_2.tab:232", @"BillingItemsExport.Verify_row__(var[""num""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Category", "Class"),     "TuitionBilling_2.tab:233", @"BillingItemsExport.Verify__is__(""Category"", ""Class"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Bill Created At", var["BilledAt"]),     "TuitionBilling_2.tab:234", @"BillingItemsExport.Verify__is__(""Bill Created At"", var[""BilledAt""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Event / Class*", var["ClassItem"]),     "TuitionBilling_2.tab:235", @"BillingItemsExport.Verify__is__(""Event / Class*"", var[""ClassItem""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Billing Item", var["ClassItem"]),     "TuitionBilling_2.tab:236", @"BillingItemsExport.Verify__is__(""Billing Item"", var[""ClassItem""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Billing Usage", ""),     "TuitionBilling_2.tab:237", @"BillingItemsExport.Verify__is__(""Billing Usage"", """")" );
            Do(() =>     BillingItemsExport.Verify__is__("Usage Period Start", ""),     "TuitionBilling_2.tab:238", @"BillingItemsExport.Verify__is__(""Usage Period Start"", """")" );
            Do(() =>     BillingItemsExport.Verify__is__("Usage Period End", ""),     "TuitionBilling_2.tab:239", @"BillingItemsExport.Verify__is__(""Usage Period End"", """")" );
            Do(() =>     BillingItemsExport.Verify__is__("For", var["Name"]),     "TuitionBilling_2.tab:240", @"BillingItemsExport.Verify__is__(""For"", var[""Name""])" );
            Do(() =>     BillingItemsExport.Verify__is_equal_to_today("Date"),     "TuitionBilling_2.tab:241", @"BillingItemsExport.Verify__is_equal_to_today(""Date"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Bill To", var["Name"]),     "TuitionBilling_2.tab:242", @"BillingItemsExport.Verify__is__(""Bill To"", var[""Name""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Amount", var["Amount"]),     "TuitionBilling_2.tab:243", @"BillingItemsExport.Verify__is__(""Amount"", var[""Amount""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Cancelled", "N"),     "TuitionBilling_2.tab:244", @"BillingItemsExport.Verify__is__(""Cancelled"", ""N"")" );
        }

        public Table table_from_246_to_251()
        {
            return new Table {
                Header = new List<string>     { "Num", "Billed At", "Class Item", "Name", "Amount" },
                Data = new List<List<string>> {
                    new List<string>          { "1", "Registration", "Driving - Highway Pursuit 100", "Bradley, Blake", "101.42" },
                    new List<string>          { "2", "Registration", "Driving - Highway Pursuit 101", "Bradley, Blake", "131.45" },
                    new List<string>          { "3", "Enrollment", "Driving - Highway Pursuit 103", "Nicholas, Jamila", "172.55" },
                    new List<string>          { "4", "Enrollment", "Driving - Highway Pursuit 100", "Chase, Charles", "99.99" },
                    new List<string>          { "5", "Enrollment", "Driving - Highway Pursuit 100", "BDoo, Dewbie", "99.99" },
                }
            };
        }

        public void paragraph_from_253_to_255()
        {
            Label(  "Complete class" );
            Do(() =>     TaskRunner.Run_task_("AutoUpdateClassStatus"),     "TuitionBilling_2.tab:255", @"TaskRunner.Run_task_(""AutoUpdateClassStatus"")" );
        }

        public void paragraph_from_257_to_260()
        {
            Label(  "Verify no bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:259", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("5"),     "TuitionBilling_2.tab:260", @"BillingItemsExport.Verify_there_are__items(""5"")" );
        }

        public void paragraph_from_262_to_265()
        {
            Label(  "Verify billing items are returned correctly in web service" );
            Do(() =>     AccountingApi.Get_Invoiceable_Items(),     "TuitionBilling_2.tab:264", @"AccountingApi.Get_Invoiceable_Items()" );
            Do(() =>     AccountingApi.Verify_there_are__Invoiceable_Items("5"),     "TuitionBilling_2.tab:265", @"AccountingApi.Verify_there_are__Invoiceable_Items(""5"")" );
        }

        public void paragraph_from_267_to_277()
        {
            Do(() =>     AccountingApi.Begin_verifying_invoiceable_item__(var["Num"]),     "TuitionBilling_2.tab:267", @"AccountingApi.Begin_verifying_invoiceable_item__(var[""Num""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_Event_is__(var["EventName"]),     "TuitionBilling_2.tab:268", @"AccountingApi.Verify_invoiceable_item_Event_is__(var[""EventName""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_Item_is__(var["EventName"]),     "TuitionBilling_2.tab:269", @"AccountingApi.Verify_invoiceable_item_Item_is__(var[""EventName""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_Usage_is_blank(),     "TuitionBilling_2.tab:270", @"AccountingApi.Verify_invoiceable_item_Usage_is_blank()" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_PeriodStart_is_null(),     "TuitionBilling_2.tab:271", @"AccountingApi.Verify_invoiceable_item_PeriodStart_is_null()" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_PeriodEnd_is_null(),     "TuitionBilling_2.tab:272", @"AccountingApi.Verify_invoiceable_item_PeriodEnd_is_null()" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_BillFor_is__(var["BillName"]),     "TuitionBilling_2.tab:273", @"AccountingApi.Verify_invoiceable_item_BillFor_is__(var[""BillName""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_BillTo_is__(var["BillName"]),     "TuitionBilling_2.tab:274", @"AccountingApi.Verify_invoiceable_item_BillTo_is__(var[""BillName""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_BillToID_is_greater_than_zero(),     "TuitionBilling_2.tab:275", @"AccountingApi.Verify_invoiceable_item_BillToID_is_greater_than_zero()" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_Amount_is__(var["Amount"]),     "TuitionBilling_2.tab:276", @"AccountingApi.Verify_invoiceable_item_Amount_is__(var[""Amount""])" );
            Do(() =>     AccountingApi.Verify_invoiceable_item_CreationDate_is__("1/1/2023"),     "TuitionBilling_2.tab:277", @"AccountingApi.Verify_invoiceable_item_CreationDate_is__(""1/1/2023"")" );
        }

        public Table table_from_279_to_284()
        {
            return new Table {
                Header = new List<string>     { "Num", "Event name", "BillName", "Amount" },
                Data = new List<List<string>> {
                    new List<string>          { "1", "Driving - Highway Pursuit 100", "Bradley, Blake", "101.42" },
                    new List<string>          { "2", "Driving - Highway Pursuit 101", "Bradley, Blake", "131.45" },
                    new List<string>          { "3", "Driving - Highway Pursuit 103", "Nicholas, Jamila", "172.55" },
                    new List<string>          { "4", "Driving - Highway Pursuit 100", "Chase, Charles", "99.99" },
                    new List<string>          { "5", "Driving - Highway Pursuit 100", "Doo, Dewbie", "99.99" },
                }
            };
        }

        public void paragraph_from_288_to_294()
        {
            Label(  "Delete an enrollment" );
            Do(() =>     StudentList.Browse_to_Student_List_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:290", @"StudentList.Browse_to_Student_List_for__(""Highway Pursuit 100"")" );
            Do(() =>     StudentList.Consider_Student__("3"),     "TuitionBilling_2.tab:291", @"StudentList.Consider_Student__(""3"")" );
            Do(() =>     StudentList.Verify_label_for__is__("Student Name", "Doo, Dewbie"),     "TuitionBilling_2.tab:292", @"StudentList.Verify_label_for__is__(""Student Name"", ""Doo, Dewbie"")" );
            Do(() =>     StudentList.Select_Student_("Doo, Dewbie"),     "TuitionBilling_2.tab:293", @"StudentList.Select_Student_(""Doo, Dewbie"")" );
            Do(() =>     StudentList.Delete_Enrollment(),     "TuitionBilling_2.tab:294", @"StudentList.Delete_Enrollment()" );
        }

        public void paragraph_from_296_to_298()
        {
            Label(  "Cancel a registration" );
            Unfound(     "Cancel registration for \"Bradley, Blake\" in \"Highway Pursuit 101\"",     "TuitionBilling_2.tab:298" );
        }

        public void paragraph_from_300_to_303()
        {
            Label(  "Verify bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:302", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("5"),     "TuitionBilling_2.tab:303", @"BillingItemsExport.Verify_there_are__items(""5"")" );
        }

        public void paragraph_from_305_to_308()
        {
            Do(() =>     BillingItemsExport.Verify_row__(var["Num"]),     "TuitionBilling_2.tab:305", @"BillingItemsExport.Verify_row__(var[""Num""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Event / Class*", var["EventName"]),     "TuitionBilling_2.tab:306", @"BillingItemsExport.Verify__is__(""Event / Class*"", var[""EventName""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Bill To", var["BillName"]),     "TuitionBilling_2.tab:307", @"BillingItemsExport.Verify__is__(""Bill To"", var[""BillName""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Cancelled", var["Cancelled"]),     "TuitionBilling_2.tab:308", @"BillingItemsExport.Verify__is__(""Cancelled"", var[""Cancelled""])" );
        }

        public Table table_from_310_to_315()
        {
            return new Table {
                Header = new List<string>     { "Num", "Event name", "BillName", "Cancelled" },
                Data = new List<List<string>> {
                    new List<string>          { "1", "Driving - Highway Pursuit 100", "Bradley, Blake", "Y" },
                    new List<string>          { "2", "Driving - Highway Pursuit 101", "Bradley, Blake", "Y" },
                    new List<string>          { "3", "Driving - Highway Pursuit 103", "Nicholas, Jamila", "N" },
                    new List<string>          { "4", "Driving - Highway Pursuit 100", "Chase, Charles", "N" },
                    new List<string>          { "5", "Driving - Highway Pursuit 100", "Doo, Dewbie", "N" },
                }
            };
        }

        public void paragraph_from_318_to_323()
        {
            Label(  "Cancel a class" );
            Do(() =>     EditClass.Browse_to_page_to_edit_class__("Highway Pursuit 100"),     "TuitionBilling_2.tab:320", @"EditClass.Browse_to_page_to_edit_class__(""Highway Pursuit 100"")" );
            Do(() =>     EditClass.Select_status_("Cancelled"),     "TuitionBilling_2.tab:321", @"EditClass.Select_status_(""Cancelled"")" );
            Do(() =>     EditClass.Save_class(),     "TuitionBilling_2.tab:322", @"EditClass.Save_class()" );
            Do(() =>     EditClass.Confirm_class_cancellation(),     "TuitionBilling_2.tab:323", @"EditClass.Confirm_class_cancellation()" );
        }

        public void paragraph_from_325_to_334()
        {
            Label(  "Change billing to at registration" );
            Do(() =>     StudentList.Browse_to_Page(),     "TuitionBilling_2.tab:327", @"StudentList.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:328", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_to_track_category(),     "TuitionBilling_2.tab:329", @"BillingPreferences.Choose_to_track_category()" );
            Do(() =>     BillingPreferences.Choose_to_create_billing_information__("At registration"),     "TuitionBilling_2.tab:330", @"BillingPreferences.Choose_to_create_billing_information__(""At registration"")" );
            Do(() =>     BillingPreferences.Choose_to_calculate_billing_as_a_variable_fee(),     "TuitionBilling_2.tab:331", @"BillingPreferences.Choose_to_calculate_billing_as_a_variable_fee()" );
            Do(() =>     BillingPreferences.Choose__to_be_responsible_for_fees("Individual"),     "TuitionBilling_2.tab:332", @"BillingPreferences.Choose__to_be_responsible_for_fees(""Individual"")" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:333", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:334", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_336_to_341()
        {
            Label(  "Register Eady, Forrest" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:338", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     StudentList.Select_Student_("Eady, Forrest"),     "TuitionBilling_2.tab:339", @"StudentList.Select_Student_(""Eady, Forrest"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:340", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:341", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_343_to_348()
        {
            Label(  "Register Johnson, Danny" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:345", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     StudentList.Select_Student_("Johnson, Danny"),     "TuitionBilling_2.tab:346", @"StudentList.Select_Student_(""Johnson, Danny"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:347", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:348", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_350_to_360()
        {
            Label(  "Change billing to at enrollment with flat fee" );
            Do(() =>     StudentList.Browse_to_Page(),     "TuitionBilling_2.tab:352", @"StudentList.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:353", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_to_track_category(),     "TuitionBilling_2.tab:354", @"BillingPreferences.Choose_to_track_category()" );
            Do(() =>     BillingPreferences.Choose_to_create_billing_information__("At enrollment"),     "TuitionBilling_2.tab:355", @"BillingPreferences.Choose_to_create_billing_information__(""At enrollment"")" );
            Do(() =>     BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee(),     "TuitionBilling_2.tab:356", @"BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee()" );
            Do(() =>     BillingPreferences.Enter__as_a_flat_fee("104.99"),     "TuitionBilling_2.tab:357", @"BillingPreferences.Enter__as_a_flat_fee(""104.99"")" );
            Do(() =>     BillingPreferences.Choose__to_be_responsible_for_fees("Individual"),     "TuitionBilling_2.tab:358", @"BillingPreferences.Choose__to_be_responsible_for_fees(""Individual"")" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:359", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:360", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_362_to_367()
        {
            Label(  "Enroll Worthen, Jorge directly" );
            Do(() =>     AddEnrollment.Browse_to_page_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:364", @"AddEnrollment.Browse_to_page_for__(""Highway Pursuit 100"")" );
            Do(() =>     AddEnrollment.Select__to_enroll("Worthen, Jorge"),     "TuitionBilling_2.tab:365", @"AddEnrollment.Select__to_enroll(""Worthen, Jorge"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:366", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     AddEnrollment.Click_Continue(),     "TuitionBilling_2.tab:367", @"AddEnrollment.Click_Continue()" );
        }

        public void paragraph_from_369_to_374()
        {
            Label(  "Enroll Murphy, Sam directly" );
            Do(() =>     AddEnrollment.Browse_to_page_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:371", @"AddEnrollment.Browse_to_page_for__(""Highway Pursuit 100"")" );
            Do(() =>     AddEnrollment.Select__to_enroll("Murphy, Sam"),     "TuitionBilling_2.tab:372", @"AddEnrollment.Select__to_enroll(""Murphy, Sam"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:373", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     AddEnrollment.Click_Continue(),     "TuitionBilling_2.tab:374", @"AddEnrollment.Click_Continue()" );
        }

        public void paragraph_from_376_to_381()
        {
            Label(  "Enroll Brown, Wendell directly" );
            Do(() =>     AddEnrollment.Browse_to_page_for__("Highway Pursuit 101"),     "TuitionBilling_2.tab:378", @"AddEnrollment.Browse_to_page_for__(""Highway Pursuit 101"")" );
            Do(() =>     AddEnrollment.Select__to_enroll("Brown, Wendell"),     "TuitionBilling_2.tab:379", @"AddEnrollment.Select__to_enroll(""Brown, Wendell"")" );
            Do(() =>     AddEnrollment.Select__for_Tuition_Bill_to_Party("Individual"),     "TuitionBilling_2.tab:380", @"AddEnrollment.Select__for_Tuition_Bill_to_Party(""Individual"")" );
            Do(() =>     AddEnrollment.Click_Continue(),     "TuitionBilling_2.tab:381", @"AddEnrollment.Click_Continue()" );
        }

        public void paragraph_from_383_to_385()
        {
            Label(  "Delete Registration for Eady, Forrest" );
            Unfound(     "Delete registration for \"Eady, Forrest\" in \"Highway Pursuit 100\"",     "TuitionBilling_2.tab:385" );
        }

        public void paragraph_from_387_to_390()
        {
            Label(  "Move Brown, Wendell from enrolled to registered" );
            Unfound(     "Move \"Brown, Wendell\" in \"Highway Pursuit 101\" from enrolled to registered",     "TuitionBilling_2.tab:389" );
            Unfound(     "? Verify \"Brown, Wendell\" is registered for class \"Highway Pursuit 101\"",     "TuitionBilling_2.tab:390" );
        }

        public void paragraph_from_392_to_394()
        {
            Label(  "Delete Worthen, Jorge" );
            Do(() =>     PersonManagement.Delete_Person("Worthen, Jorge"),     "TuitionBilling_2.tab:394", @"PersonManagement.Delete_Person(""Worthen, Jorge"")" );
        }

        public void paragraph_from_396_to_401()
        {
            Label(  "Move Murphy, Sam and Johnson, Danny to waitlist" );
            Unfound(     "Move \"Murphy, Sam\" in \"Highway Pursuit 100\" to waitlist",     "TuitionBilling_2.tab:398" );
            Unfound(     "? Verify \"Murphy, Sam\" is waitlisted for class \"Highway Pursuit 100\"",     "TuitionBilling_2.tab:399" );
            Unfound(     "Move \"Johnson, Danny\" in \"Highway Pursuit 100\" to waitlist",     "TuitionBilling_2.tab:400" );
            Unfound(     "? Verify \"Johnson, Danny\" is waitlisted for class \"Highway Pursuit 100\"",     "TuitionBilling_2.tab:401" );
        }

        public void paragraph_from_403_to_413()
        {
            Label(  "Change billing to at enrollment with flat fee" );
            Do(() =>     StudentList.Browse_to_Page(),     "TuitionBilling_2.tab:405", @"StudentList.Browse_to_Page()" );
            Do(() =>     BillingPreferences.Click_to_edit_category__("Class"),     "TuitionBilling_2.tab:406", @"BillingPreferences.Click_to_edit_category__(""Class"")" );
            Do(() =>     BillingPreferences.Choose_to_track_category(),     "TuitionBilling_2.tab:407", @"BillingPreferences.Choose_to_track_category()" );
            Do(() =>     BillingPreferences.Choose_to_create_billing_information__("At registration"),     "TuitionBilling_2.tab:408", @"BillingPreferences.Choose_to_create_billing_information__(""At registration"")" );
            Do(() =>     BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee(),     "TuitionBilling_2.tab:409", @"BillingPreferences.Choose_to_calculate_billing_as_a_flat_fee()" );
            Do(() =>     BillingPreferences.Enter__as_a_flat_fee("200.00"),     "TuitionBilling_2.tab:410", @"BillingPreferences.Enter__as_a_flat_fee(""200.00"")" );
            Do(() =>     BillingPreferences.Choose__to_be_responsible_for_fees("Individual"),     "TuitionBilling_2.tab:411", @"BillingPreferences.Choose__to_be_responsible_for_fees(""Individual"")" );
            Do(() =>     BillingPreferences.Prepare_to_save_preference(),     "TuitionBilling_2.tab:412", @"BillingPreferences.Prepare_to_save_preference()" );
            Do(() =>     BillingPreferences.Click_save_preference(),     "TuitionBilling_2.tab:413", @"BillingPreferences.Click_save_preference()" );
        }

        public void paragraph_from_415_to_419()
        {
            Label(  "Register person A" );
            Do(() =>     RegisterStudent.Browse_to_page_from_available_training_list_registering_for__("Highway Pursuit 100"),     "TuitionBilling_2.tab:417", @"RegisterStudent.Browse_to_page_from_available_training_list_registering_for__(""Highway Pursuit 100"")" );
            Do(() =>     StudentList.Select_Student_("Fairchild, Florence"),     "TuitionBilling_2.tab:418", @"StudentList.Select_Student_(""Fairchild, Florence"")" );
            Do(() =>     RegisterStudent.Click_register(),     "TuitionBilling_2.tab:419", @"RegisterStudent.Click_register()" );
        }

        public void paragraph_from_421_to_423()
        {
            Label(  "Cancel a registration" );
            Unfound(     "Cancel registration for \"Fairchild, Florence\" in \"Highway Pursuit 100\"",     "TuitionBilling_2.tab:423" );
        }

        public void paragraph_from_426_to_429()
        {
            Label(  "Verify  bill was created at enrollment" );
            Do(() =>     BillingItemsExport.Export_all_billing_items(),     "TuitionBilling_2.tab:428", @"BillingItemsExport.Export_all_billing_items()" );
            Do(() =>     BillingItemsExport.Verify_there_are__items("11"),     "TuitionBilling_2.tab:429", @"BillingItemsExport.Verify_there_are__items(""11"")" );
        }

        public void paragraph_from_431_to_446()
        {
            Do(() =>     BillingItemsExport.Verify_row__(var["Num"]),     "TuitionBilling_2.tab:431", @"BillingItemsExport.Verify_row__(var[""Num""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Category", "Class"),     "TuitionBilling_2.tab:432", @"BillingItemsExport.Verify__is__(""Category"", ""Class"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Bill Created At", var["BilledAt"]),     "TuitionBilling_2.tab:433", @"BillingItemsExport.Verify__is__(""Bill Created At"", var[""BilledAt""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Event / Class*", var["ClassItem"]),     "TuitionBilling_2.tab:434", @"BillingItemsExport.Verify__is__(""Event / Class*"", var[""ClassItem""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Billing Item", var["ClassItem"]),     "TuitionBilling_2.tab:435", @"BillingItemsExport.Verify__is__(""Billing Item"", var[""ClassItem""])" );
            Do(() =>     BillingItemsExport.Verify__is_blank("Billing Usage"),     "TuitionBilling_2.tab:436", @"BillingItemsExport.Verify__is_blank(""Billing Usage"")" );
            Do(() =>     BillingItemsExport.Verify__is_blank("Usage Period Start"),     "TuitionBilling_2.tab:437", @"BillingItemsExport.Verify__is_blank(""Usage Period Start"")" );
            Do(() =>     BillingItemsExport.Verify__is_blank("Usage Period End"),     "TuitionBilling_2.tab:438", @"BillingItemsExport.Verify__is_blank(""Usage Period End"")" );
            Do(() =>     BillingItemsExport.Verify__is__("For", var["Name"]),     "TuitionBilling_2.tab:439", @"BillingItemsExport.Verify__is__(""For"", var[""Name""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Date", "01/01/2023"),     "TuitionBilling_2.tab:440", @"BillingItemsExport.Verify__is__(""Date"", ""01/01/2023"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Bill To", var["Name"]),     "TuitionBilling_2.tab:441", @"BillingItemsExport.Verify__is__(""Bill To"", var[""Name""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Amount", var["Amount"]),     "TuitionBilling_2.tab:442", @"BillingItemsExport.Verify__is__(""Amount"", var[""Amount""])" );
            Do(() =>     BillingItemsExport.Verify__is__("Cancelled", "Y"),     "TuitionBilling_2.tab:443", @"BillingItemsExport.Verify__is__(""Cancelled"", ""Y"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Description", "TUITION CHARGE for #Name (#ID): #ClassItem (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:444", @"BillingItemsExport.Verify__is__(""Description"", ""TUITION CHARGE for #Name (#ID): #ClassItem (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Quantity", "1"),     "TuitionBilling_2.tab:445", @"BillingItemsExport.Verify__is__(""Quantity"", ""1"")" );
            Do(() =>     BillingItemsExport.Verify__is__("Unit Price", var["Amount"]),     "TuitionBilling_2.tab:446", @"BillingItemsExport.Verify__is__(""Unit Price"", var[""Amount""])" );
        }

        public Table table_from_448_to_459()
        {
            return new Table {
                Header = new List<string>     { "Num", "Name", "ID", "Billed At", "Class Item", "Amount", "Cancelled" },
                Data = new List<List<string>> {
                    new List<string>          { "1", "Bradley, Blake", "B1", "Registration", "Driving - Highway Pursuit 100", "101.42", "Y" },
                    new List<string>          { "2", "Bradley, Blake", "B1", "Registration", "Driving - Highway Pursuit 101", "131.45", "Y" },
                    new List<string>          { "3", "Nicholas, Jamila", "N1", "Enrollment", "Driving - Highway Pursuit 103", "172.55", "N" },
                    new List<string>          { "4", "Chase, Charles", "C1", "Enrollment", "Driving - Highway Pursuit 100", "99.99", "N" },
                    new List<string>          { "5", "Doo, Dewbie", "D1", "Enrollment", "Driving - Highway Pursuit 100", "99.99", "N" },
                    new List<string>          { "6", "Eady, Forrest", "E2", "Registration", "Driving - Highway Pursuit 100", "101.42", "Y" },
                    new List<string>          { "7", "Johnson, Danny", "J1", "Registration", "Driving - Highway Pursuit 100", "101.42", "Y" },
                    new List<string>          { "8", "Worthen, Jorge", "W1", "Registration", "Driving - Highway Pursuit 100", "104.99", "Y" },
                    new List<string>          { "9", "Murphy, Sam", "M1", "Enrollment", "Driving - Highway Pursuit 100", "104.99", "Y" },
                    new List<string>          { "10", "Brown, Wendell", "B2", "Enrollment", "Driving - Highway Pursuit 100", "104.99", "Y" },
                    new List<string>          { "11", "Fairchild, Florence", "F1", "Registration", "Driving - Highway Pursuit 100", "200", "Y" },
                }
            };
        }

        public void paragraph_from_462_to_625()
        {
            Label(  "Verify that cancellations are transmitted to an external accounting system" );
            Do(() =>     AccountingApi.Get_Cancellations(),     "TuitionBilling_2.tab:464", @"AccountingApi.Get_Cancellations()" );
            Do(() =>     AccountingApi.Verify_there_are__cancellations("10"),     "TuitionBilling_2.tab:465", @"AccountingApi.Verify_there_are__cancellations(""10"")" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("1"),     "TuitionBilling_2.tab:466", @"AccountingApi.Begin_verifying_cancellation_item__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:467", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Transfer to Different Class"),     "TuitionBilling_2.tab:468", @"AccountingApi.Verify_cancellation_reason_is__(""Transfer to Different Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("101.42"),     "TuitionBilling_2.tab:469", @"AccountingApi.Verify_cancellation_amount_is__(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:470", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Bradley, Blake"),     "TuitionBilling_2.tab:471", @"AccountingApi.Verify_cancellation_bill_for_is__(""Bradley, Blake"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Bradley, Blake"),     "TuitionBilling_2.tab:472", @"AccountingApi.Verify_cancellation_bill_to_is__(""Bradley, Blake"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:473", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Bradley, Blake (B1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:474", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Bradley, Blake (B1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:475", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:476", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:477", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:478", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:479", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("101.42"),     "TuitionBilling_2.tab:480", @"AccountingApi.Verify_cancellation_unit_price_is(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:481", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("2"),     "TuitionBilling_2.tab:482", @"AccountingApi.Begin_verifying_cancellation_item__(""2"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:483", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Deleted Enrollment"),     "TuitionBilling_2.tab:484", @"AccountingApi.Verify_cancellation_reason_is__(""Deleted Enrollment"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("99.99"),     "TuitionBilling_2.tab:485", @"AccountingApi.Verify_cancellation_amount_is__(""99.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:486", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Doo, Dewbie"),     "TuitionBilling_2.tab:487", @"AccountingApi.Verify_cancellation_bill_for_is__(""Doo, Dewbie"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Doo, Dewbie"),     "TuitionBilling_2.tab:488", @"AccountingApi.Verify_cancellation_bill_to_is__(""Doo, Dewbie"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:489", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Doo, Dewbie (D1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:490", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Doo, Dewbie (D1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:491", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:492", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:493", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:494", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:495", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("99.99"),     "TuitionBilling_2.tab:496", @"AccountingApi.Verify_cancellation_unit_price_is(""99.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:497", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("3"),     "TuitionBilling_2.tab:498", @"AccountingApi.Begin_verifying_cancellation_item__(""3"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:499", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Cancelled Registration"),     "TuitionBilling_2.tab:500", @"AccountingApi.Verify_cancellation_reason_is__(""Cancelled Registration"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("131.45"),     "TuitionBilling_2.tab:501", @"AccountingApi.Verify_cancellation_amount_is__(""131.45"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:502", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Bradley, Blake"),     "TuitionBilling_2.tab:503", @"AccountingApi.Verify_cancellation_bill_for_is__(""Bradley, Blake"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Bradley, Blake"),     "TuitionBilling_2.tab:504", @"AccountingApi.Verify_cancellation_bill_to_is__(""Bradley, Blake"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:505", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Bradley, Blake (B1): Driving - Highway Pursuit 101 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:506", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Bradley, Blake (B1): Driving - Highway Pursuit 101 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 101"),     "TuitionBilling_2.tab:507", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 101"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 101"),     "TuitionBilling_2.tab:508", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 101"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:509", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:510", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:511", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("131.45"),     "TuitionBilling_2.tab:512", @"AccountingApi.Verify_cancellation_unit_price_is(""131.45"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:513", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("4"),     "TuitionBilling_2.tab:514", @"AccountingApi.Begin_verifying_cancellation_item__(""4"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:515", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Cancelled Class"),     "TuitionBilling_2.tab:516", @"AccountingApi.Verify_cancellation_reason_is__(""Cancelled Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("99.99"),     "TuitionBilling_2.tab:517", @"AccountingApi.Verify_cancellation_amount_is__(""99.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:518", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Chase, Charles"),     "TuitionBilling_2.tab:519", @"AccountingApi.Verify_cancellation_bill_for_is__(""Chase, Charles"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Chase, Charles"),     "TuitionBilling_2.tab:520", @"AccountingApi.Verify_cancellation_bill_to_is__(""Chase, Charles"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:521", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Chase, Charles (C1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:522", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Chase, Charles (C1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:523", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:524", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:525", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:526", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:527", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("99.99"),     "TuitionBilling_2.tab:528", @"AccountingApi.Verify_cancellation_unit_price_is(""99.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:529", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("5"),     "TuitionBilling_2.tab:530", @"AccountingApi.Begin_verifying_cancellation_item__(""5"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:531", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Deleted Registration"),     "TuitionBilling_2.tab:532", @"AccountingApi.Verify_cancellation_reason_is__(""Deleted Registration"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("101.42"),     "TuitionBilling_2.tab:533", @"AccountingApi.Verify_cancellation_amount_is__(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:534", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Eady, Forrest"),     "TuitionBilling_2.tab:535", @"AccountingApi.Verify_cancellation_bill_for_is__(""Eady, Forrest"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Eady, Forrest"),     "TuitionBilling_2.tab:536", @"AccountingApi.Verify_cancellation_bill_to_is__(""Eady, Forrest"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:537", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Eady, Forrest (E2): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:538", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Eady, Forrest (E2): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:539", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:540", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:541", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:542", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:543", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("101.42"),     "TuitionBilling_2.tab:544", @"AccountingApi.Verify_cancellation_unit_price_is(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:545", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("6"),     "TuitionBilling_2.tab:546", @"AccountingApi.Begin_verifying_cancellation_item__(""6"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:547", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Move to Pending"),     "TuitionBilling_2.tab:548", @"AccountingApi.Verify_cancellation_reason_is__(""Move to Pending"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("104.99"),     "TuitionBilling_2.tab:549", @"AccountingApi.Verify_cancellation_amount_is__(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:550", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Brown, Wendell"),     "TuitionBilling_2.tab:551", @"AccountingApi.Verify_cancellation_bill_for_is__(""Brown, Wendell"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Brown, Wendell"),     "TuitionBilling_2.tab:552", @"AccountingApi.Verify_cancellation_bill_to_is__(""Brown, Wendell"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:553", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Brown, Wendell (B2): Driving - Highway Pursuit 101 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:554", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Brown, Wendell (B2): Driving - Highway Pursuit 101 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 101"),     "TuitionBilling_2.tab:555", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 101"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 101"),     "TuitionBilling_2.tab:556", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 101"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:557", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:558", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:559", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("104.99"),     "TuitionBilling_2.tab:560", @"AccountingApi.Verify_cancellation_unit_price_is(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:561", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("7"),     "TuitionBilling_2.tab:562", @"AccountingApi.Begin_verifying_cancellation_item__(""7"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:563", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Deleted Person"),     "TuitionBilling_2.tab:564", @"AccountingApi.Verify_cancellation_reason_is__(""Deleted Person"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("104.99"),     "TuitionBilling_2.tab:565", @"AccountingApi.Verify_cancellation_amount_is__(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:566", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Worthen, Jorge"),     "TuitionBilling_2.tab:567", @"AccountingApi.Verify_cancellation_bill_for_is__(""Worthen, Jorge"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Worthen, Jorge"),     "TuitionBilling_2.tab:568", @"AccountingApi.Verify_cancellation_bill_to_is__(""Worthen, Jorge"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:569", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Worthen, Jorge (W1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:570", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Worthen, Jorge (W1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:571", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:572", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:573", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:574", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:575", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("104.99"),     "TuitionBilling_2.tab:576", @"AccountingApi.Verify_cancellation_unit_price_is(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:577", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("8"),     "TuitionBilling_2.tab:578", @"AccountingApi.Begin_verifying_cancellation_item__(""8"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:579", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Move to Waitlist"),     "TuitionBilling_2.tab:580", @"AccountingApi.Verify_cancellation_reason_is__(""Move to Waitlist"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("104.99"),     "TuitionBilling_2.tab:581", @"AccountingApi.Verify_cancellation_amount_is__(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:582", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Murphy, Sam"),     "TuitionBilling_2.tab:583", @"AccountingApi.Verify_cancellation_bill_for_is__(""Murphy, Sam"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Murphy, Sam"),     "TuitionBilling_2.tab:584", @"AccountingApi.Verify_cancellation_bill_to_is__(""Murphy, Sam"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:585", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Murphy, Sam (M1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:586", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Murphy, Sam (M1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:587", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:588", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:589", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:590", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:591", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("104.99"),     "TuitionBilling_2.tab:592", @"AccountingApi.Verify_cancellation_unit_price_is(""104.99"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:593", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("9"),     "TuitionBilling_2.tab:594", @"AccountingApi.Begin_verifying_cancellation_item__(""9"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:595", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Move to Waitlist"),     "TuitionBilling_2.tab:596", @"AccountingApi.Verify_cancellation_reason_is__(""Move to Waitlist"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("101.42"),     "TuitionBilling_2.tab:597", @"AccountingApi.Verify_cancellation_amount_is__(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:598", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Johnson, Danny"),     "TuitionBilling_2.tab:599", @"AccountingApi.Verify_cancellation_bill_for_is__(""Johnson, Danny"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Johnson, Danny"),     "TuitionBilling_2.tab:600", @"AccountingApi.Verify_cancellation_bill_to_is__(""Johnson, Danny"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:601", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Johnson, Danny (J1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:602", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Johnson, Danny (J1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:603", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:604", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:605", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:606", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:607", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("101.42"),     "TuitionBilling_2.tab:608", @"AccountingApi.Verify_cancellation_unit_price_is(""101.42"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:609", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
            Do(() =>     AccountingApi.Begin_verifying_cancellation_item__("10"),     "TuitionBilling_2.tab:610", @"AccountingApi.Begin_verifying_cancellation_item__(""10"")" );
            Do(() =>     AccountingApi.Verify_cancellation_date_is__("1/1/2023"),     "TuitionBilling_2.tab:611", @"AccountingApi.Verify_cancellation_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_reason_is__("Cancelled Registration"),     "TuitionBilling_2.tab:612", @"AccountingApi.Verify_cancellation_reason_is__(""Cancelled Registration"")" );
            Do(() =>     AccountingApi.Verify_cancellation_amount_is__("200.00"),     "TuitionBilling_2.tab:613", @"AccountingApi.Verify_cancellation_amount_is__(""200.00"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_date_is__("1/1/2023"),     "TuitionBilling_2.tab:614", @"AccountingApi.Verify_cancellation_bill_date_is__(""1/1/2023"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_for_is__("Fairchild, Florence"),     "TuitionBilling_2.tab:615", @"AccountingApi.Verify_cancellation_bill_for_is__(""Fairchild, Florence"")" );
            Do(() =>     AccountingApi.Verify_cancellation_bill_to_is__("Fairchild, Florence"),     "TuitionBilling_2.tab:616", @"AccountingApi.Verify_cancellation_bill_to_is__(""Fairchild, Florence"")" );
            Do(() =>     AccountingApi.Verify_cancellation_category_is__("Class"),     "TuitionBilling_2.tab:617", @"AccountingApi.Verify_cancellation_category_is__(""Class"")" );
            Do(() =>     AccountingApi.Verify_cancellation_description_is__("TUITION CHARGE for Fairchild, Florence (F1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"),     "TuitionBilling_2.tab:618", @"AccountingApi.Verify_cancellation_description_is__(""TUITION CHARGE for Fairchild, Florence (F1): Driving - Highway Pursuit 100 (occurring 01/01/2023 - 01/01/2023)"")" );
            Do(() =>     AccountingApi.Verify_cancellation_event_is__("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:619", @"AccountingApi.Verify_cancellation_event_is__(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_item_is("Driving - Highway Pursuit 100"),     "TuitionBilling_2.tab:620", @"AccountingApi.Verify_cancellation_item_is(""Driving - Highway Pursuit 100"")" );
            Do(() =>     AccountingApi.Verify_cancellation_period_start_is_blank(),     "TuitionBilling_2.tab:621", @"AccountingApi.Verify_cancellation_period_start_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_period_end_is_blank(),     "TuitionBilling_2.tab:622", @"AccountingApi.Verify_cancellation_period_end_is_blank()" );
            Do(() =>     AccountingApi.Verify_cancellation_quantity_is__("1"),     "TuitionBilling_2.tab:623", @"AccountingApi.Verify_cancellation_quantity_is__(""1"")" );
            Do(() =>     AccountingApi.Verify_cancellation_unit_price_is("200.00"),     "TuitionBilling_2.tab:624", @"AccountingApi.Verify_cancellation_unit_price_is(""200.00"")" );
            Do(() =>     AccountingApi.Verify_cancellation_usage_is_blank(),     "TuitionBilling_2.tab:625", @"AccountingApi.Verify_cancellation_usage_is_blank()" );
        }
    }
}
