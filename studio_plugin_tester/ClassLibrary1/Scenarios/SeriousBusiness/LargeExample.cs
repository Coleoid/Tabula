//  This file was generated by TabulaClassGenerator version 0.4.
//  To change this file, change the Tabula scenario at K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab.
//  Version 0.4 is not yet alpha (known missing 1.0 features).  You have been warned.
using System;
using System.Collections.Generic;
using Acadis.Constants.Accounting;
using Acadis.Constants.Admin;
using Acadis.SystemUtilities;
using Tabula.API;

namespace Tabula
{
    //  "I hope for Advanced person search with duty assignments"
    public class LargeExample_generated
        : GeneratedScenarioBase, IGeneratedScenario
    {
        public void ExecuteScenario()
        {
            paragraph__005_to_006();
            Foreach_Row_in( table__010_to_014, paragraph__009_to_009 );
            Foreach_Row_in( table__018_to_021, paragraph__017_to_017 );
            Foreach_Row_in( table__027_to_031, paragraph__025_to_026 );
            paragraph__034_to_034();
            Foreach_Row_in( table__039_to_042, paragraph__037_to_038 );
            Foreach_Row_in( table__048_to_057, paragraph__045_to_047 );
            Foreach_Row_in( table__064_to_068, paragraph__060_to_063 );
            paragraph__070_to_082();
            Foreach_Row_in( table__086_to_088, paragraph__085_to_085 );
            Foreach_Row_in( table__091_to_093, paragraph__090_to_090 );
            Foreach_Row_in( table__096_to_098, paragraph__095_to_095 );
            Foreach_Row_in( table__107_to_110, paragraph__102_to_106 );
            paragraph__113_to_119();
            Foreach_Row_in( table__148_to_151, paragraph__123_to_145 );
            Foreach_Row_in( table__154_to_158, paragraph__123_to_145 );
            Foreach_Row_in( table__161_to_167, paragraph__123_to_145 );
            Foreach_Row_in( table__170_to_173, paragraph__123_to_145 );
            Foreach_Row_in( table__176_to_178, paragraph__123_to_145 );
        }

        public void paragraph__005_to_006()
        {
            Label(     "Enable duty locations");
            Do(() =>    GlobalSettingManagement.Enable_Duty_Locations(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:6", @"GlobalSettingManagement.Enable_Duty_Locations()");
        }

        public void paragraph__009_to_009()
        {
            Label(     "What we'll call our people in this scenario");
            Do(() =>    Var["#handle"] = "fullnamelf", "K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:9", @"Var[""#handle""] = ""fullnamelf""");
        }

        public Table table__010_to_014()
        {
            return new Table {
                Header = new List<string>     { "handle", "FullNameLF" },
                Data = new List<List<string>> {
                    new List<string>          { "Lina", "Frixell, Rorolina" },
                    new List<string>          { "Gio", "Arland, Giovanni" },
                    new List<string>          { "Sterky", "Cranach, Sterkenberg" },
                    new List<string>          { "Mimi", "Schwarzlang, Mimi" },
                }
            };
        };

        public void paragraph__017_to_017()
        {
            Label(     "What we'll call the organizations they work for");
            Do(() =>    Var["#TLA"] = "organizationname", "K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:17", @"Var[""#TLA""] = ""organizationname""");
        }

        public Table table__018_to_021()
        {
            return new Table {
                Header = new List<string>     { "TLA", "OrganizationName" },
                Data = new List<List<string>> {
                    new List<string>          { "HPD", "Hometown Police Department" },
                    new List<string>          { "OPD", "Otherville Police Department" },
                    new List<string>          { "OA", "Otherville Academy" },
                }
            };
        };

        public void paragraph__025_to_026()
        {
            Label(     "Create our people");
            Do(() =>    UserCreation.Create_Person_named__(Var["name"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:26", @"UserCreation.Create_Person_named__(Var[""name""])");
        }

        public Table table__027_to_031()
        {
            return new Table {
                Header = new List<string>     { "name" },
                Data = new List<List<string>> {
                    new List<string>          { "#lina" },
                    new List<string>          { "#gio" },
                    new List<string>          { "#sterky" },
                    new List<string>          { "#mimi" },
                }
            };
        };

        public void paragraph__034_to_034()
        {
            Label(     "Lina changed her name");
            Do(() =>    Var[""Lina""] = "Tina Lina", "K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:34", @"Var[""""Lina""""] = ""Tina Lina""");
        }

        public void paragraph__037_to_038()
        {
            Label(     "Create our organizations");
            Do(() =>    OrganizationFNHManagement.Create_organization_named__of_type__under_base_parent_group(Var["orgname"], $"organization"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:38", @"OrganizationFNHManagement.Create_organization_named__of_type__under_base_parent_group(Var[""orgname""], $""organization"")");
        }

        public Table table__039_to_042()
        {
            return new Table {
                Header = new List<string>     { "orgName" },
                Data = new List<List<string>> {
                    new List<string>          { "#hpd" },
                    new List<string>          { "#opd" },
                    new List<string>          { "#oa" },
                }
            };
        };

        public void paragraph__045_to_047()
        {
            Label(     "Create list items");
            Unfound(  @"Create ""#itemType"" list item ""Testopia #item"" with description ""Testopia #item description"" with usage ""Available for new records""", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:47");
        }

        public Table table__048_to_057()
        {
            return new Table {
                Header = new List<string>     { "itemType", "item" },
                Data = new List<List<string>> {
                    new List<string>          { "EmploymentAction", "Update" },
                    new List<string>          { "EmploymentType", "Civilian" },
                    new List<string>          { "EmploymentType", "Employee" },
                    new List<string>          { "AppointmentType", "Reserve" },
                    new List<string>          { "AppointmentType", "Part | Time" },
                    new List<string>          { "AppointmentType", "Full Time" },
                    new List<string>          { "TitleRank", "Chief" },
                    new List<string>          { "TitleRank", "Instructor" },
                    new List<string>          { "TitleRank", "Lackey" },
                }
            };
        };

        public void paragraph__060_to_063()
        {
            Label(     "Add employments");
            Do(() =>    EmploymentActionEdit.Browse_to_add_person_employment_for__(Var["empname"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:62", @"EmploymentActionEdit.Browse_to_add_person_employment_for__(Var[""empname""])");
            Do(() =>    EmploymentActionEdit.Add_primary_employment_at__with_title__starting_on__(Var["orgname"], $"Testopia {Var["empTitle"]}" employment type "Testopia {Var["empType"]}" and appointment type "Testopia {Var["apptType"]}", Var["startdate"].To<DateTime>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:63", @"EmploymentActionEdit.Add_primary_employment_at__with_title__starting_on__(Var[""orgname""], $""Testopia {Var[""empTitle""]}"" employment type ""Testopia {Var[""empType""]}"" and appointment type ""Testopia {Var[""apptType""]}"", Var[""startdate""].To<DateTime>())");
        }

        public Table table__064_to_068()
        {
            return new Table {
                Header = new List<string>     { "empName", "orgName", "empTitle", "empType", "apptType", "startDate" },
                Data = new List<List<string>> {
                    new List<string>          { "#lina", "#hpd", "Lackey", "Civilian", "Contract", "8/15/2008" },
                    new List<string>          { "#gio", "#hpd", "Lackey", "Civilian", "Contract", "8/15/2008" },
                    new List<string>          { "#sterky", "#opd", "Lackey", "Civilian", "Contract", "8/15/2008" },
                    new List<string>          { "#mimi", "#oa", "Instructor", "Reserve", "Part Time", "8/15/2012" },
                }
            };
        };

        public void paragraph__070_to_082()
        {
            Label(     "");
            Alias(     "Add employment actions for #employeeName at #orgName", Tabula.CST.Block ); //TODO: Actually implement
        }

        public void paragraph__085_to_085()
        {
            Label(     "Add employment actions");
            Unfound(  @"Add employment actions for #lina at #hpd", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:85");
        }

        public Table table__086_to_088()
        {
            return new Table {
                Header = new List<string>     { "actionName", "empType", "apptType", "empTitle", "newStatus", "effectiveDate" },
                Data = new List<List<string>> {
                    new List<string>          { "Testopia Update", "Employee", "Part Time", "Instructor", "On Leave (Active)", "10/15/2008" },
                    new List<string>          { "Separation", "Reserve", "Full Time", "Chief", "Separated (Inactive)", "12/15/2008" },
                }
            };
        };

        public void paragraph__090_to_090()
        {
            Label(     "");
            Unfound(  @"Add employment actions for #gio at #hpd", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:90");
        }

        public Table table__091_to_093()
        {
            return new Table {
                Header = new List<string>     { "actionName", "empType", "apptType", "empTitle", "newStatus", "effectiveDate" },
                Data = new List<List<string>> {
                    new List<string>          { "Testopia Update", "Reserve", "Full Time", "Chief", "Active (Active)", "10/15/2008" },
                    new List<string>          { "Testopia Update", "Reserve", "Full Time", "Chief", "On Leave (Active)", "12/15/2008" },
                }
            };
        };

        public void paragraph__095_to_095()
        {
            Label(     "");
            Unfound(  @"Add employment actions for #sterky at #opd", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:95");
        }

        public Table table__096_to_098()
        {
            return new Table {
                Header = new List<string>     { "actionName", "empType", "apptType", "empTitle", "newStatus", "effectiveDate", "comment" },
                Data = new List<List<string>> {
                    new List<string>          { "Testopia Update", "Employee", "Part Time", "Instructor", "On Leave (Active)", "10/15/2008", "Testopia Comment" },
                    new List<string>          { "Separation", "Civiilan", "Contract", "Lackey", "Separated (Inactive)", "12/15/2008", "" },
                }
            };
        };

        public void paragraph__102_to_106()
        {
            Label(     "Add a comment and duty assignment");
            Do(() =>    PersonEmploymentFNHManagement.Add_employment_comment__for__at__($"Testopia comment", Var["empname"], Var["orgname"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:104", @"PersonEmploymentFNHManagement.Add_employment_comment__for__at__($""Testopia comment"", Var[""empname""], Var[""orgname""])");
            Do(() =>    PersonEmploymentFNHManagement.Using_employment_of__at__(Var["empname"], Var["orgname"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:105", @"PersonEmploymentFNHManagement.Using_employment_of__at__(Var[""empname""], Var[""orgname""])");
            Do(() =>    PersonEmploymentFNHManagement.Add_duty_assignment_at__starting__with_status__(Var["orgname"], Var["startdate"].To<DateTime>(), $"Current"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:106", @"PersonEmploymentFNHManagement.Add_duty_assignment_at__starting__with_status__(Var[""orgname""], Var[""startdate""].To<DateTime>(), $""Current"")");
        }

        public Table table__107_to_110()
        {
            return new Table {
                Header = new List<string>     { "empName", "orgName", "startDate" },
                Data = new List<List<string>> {
                    new List<string>          { "#lina", "#hpd", "8/15/2008" },
                    new List<string>          { "#sterky", "#opd", "8/15/2008" },
                    new List<string>          { "#mimi", "#oa", "8/15/2012" },
                }
            };
        };

        public void paragraph__113_to_119()
        {
            Label(     "Add temporary duty assignments");
            Do(() =>    PersonEmploymentFNHManagement.Using_employment_of__at__(Var["lina"], Var["hpd"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:114", @"PersonEmploymentFNHManagement.Using_employment_of__at__(Var[""lina""], Var[""hpd""])");
            Do(() =>    PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__from__to__with_status__(Var["opd"], "1/1/2013".To<DateTime>(), "4/1/2013".To<DateTime>(), $"Past"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:115", @"PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__from__to__with_status__(Var[""opd""], ""1/1/2013"".To<DateTime>(), ""4/1/2013"".To<DateTime>(), $""Past"")");
            Do(() =>    PersonEmploymentFNHManagement.Using_employment_of__at__(Var["sterky"], Var["opd"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:117", @"PersonEmploymentFNHManagement.Using_employment_of__at__(Var[""sterky""], Var[""opd""])");
            Do(() =>    PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__from__to__with_status__(Var["hpd"], "1/1/2013".To<DateTime>(), "4/1/2013".To<DateTime>(), $"Past"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:118", @"PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__from__to__with_status__(Var[""hpd""], ""1/1/2013"".To<DateTime>(), ""4/1/2013"".To<DateTime>(), $""Past"")");
            Do(() =>    PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__with_status__(Var["oa"], $"Past"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:119", @"PersonEmploymentFNHManagement.Add_temporary_duty_assignment_at__with_status__(Var[""oa""], $""Past"")");
        }

        public void paragraph__123_to_145()
        {
            Label(     "Search people with many different duty assignment criteria");
            Do(() =>    PersonSearchCriteria.Browse_to_page(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:126", @"PersonSearchCriteria.Browse_to_page()");
            Do(() =>    PersonSearchCriteria.Click_Switch_to_advanced_mode(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:127", @"PersonSearchCriteria.Click_Switch_to_advanced_mode()");
            Unfound(  @"Add new employment criterion to expression", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:128");
            Unfound(  @"With #assignment duty assignments", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:129");
            Do(() =>    PersonSearchCriteria.With_duty_locations__(Var["location"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:130", @"PersonSearchCriteria.With_duty_locations__(Var[""location""])");
            Do(() =>    PersonSearchCriteria.With_duty_assignment_statuses(Var["status"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:131", @"PersonSearchCriteria.With_duty_assignment_statuses(Var[""status""])");
            Do(() =>    PersonSearchCriteria.With_duty_assignment_dates_before__(Var["before"].To<DateTime>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:132", @"PersonSearchCriteria.With_duty_assignment_dates_before__(Var[""before""].To<DateTime>())");
            Do(() =>    PersonSearchCriteria.With_duty_assignment_dates_after__(Var["after"].To<DateTime>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:133", @"PersonSearchCriteria.With_duty_assignment_dates_after__(Var[""after""].To<DateTime>())");
            Do(() =>    PersonSearchCriteria.With_duty_assignment_dates_between__and__(Var["between"].To<DateTime>(), Var["and"].To<DateTime>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:134", @"PersonSearchCriteria.With_duty_assignment_dates_between__and__(Var[""between""].To<DateTime>(), Var[""and""].To<DateTime>())");
            Unfound(  @"With undated assignments #undated", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:135");
            Unfound(  @"With employment criteria effective on a specific date #effectivedate", @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:136");
            Do(() =>    PersonSearchCriteria.With_employment_effective_date(Var["effectivedate"].To<DateTime>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:137", @"PersonSearchCriteria.With_employment_effective_date(Var[""effectivedate""].To<DateTime>())");
            Do(() =>    PersonSearchCriteria.With_employment_types($"Testopia {Var["EmpType"]}"), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:138", @"PersonSearchCriteria.With_employment_types($""Testopia {Var[""EmpType""]}"")");
            Do(() =>    PersonSearchCriteria.With_organizations(Var["organization"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:139", @"PersonSearchCriteria.With_organizations(Var[""organization""])");
            Do(() =>    PersonSearchCriteria.Click_done_in_employment_criteria_popover(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:140", @"PersonSearchCriteria.Click_done_in_employment_criteria_popover()");
            Do(() =>    PersonSearchCriteria.Click_Search(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:141", @"PersonSearchCriteria.Click_Search()");
            Do(() =>    PersonSearchCriteria.Verify_that_page_navigated_to_search_results(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:142", @"PersonSearchCriteria.Verify_that_page_navigated_to_search_results()");
            Do(() =>    PersonSearch.Search_with_criteria(), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:143", @"PersonSearch.Search_with_criteria()");
            Do(() =>    PersonSearch.Verify_that_there_are__results(Var["num"].To<int>()), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:144", @"PersonSearch.Verify_that_there_are__results(Var[""num""].To<int>())");
            Do(() =>    PersonSearch.Verify_that_results_are__(Var["found"]), @"K:\code\Tabula\studio_plugin_tester\ClassLibrary1\Scenarios\SeriousBusiness\LargeExample.tab:145", @"PersonSearch.Verify_that_results_are__(Var[""found""])");
        }

        public Table table__148_to_151()
        {
            return new Table {
                Label = "Search Assignment regular/temporary, location, and org",
                Header = new List<string>     { "Assignment", "Location", "Organization", "Num", "Found" },
                Data = new List<List<string>> {
                    new List<string>          { "regular", "#hpd", "", "1", "#lina" },
                    new List<string>          { "regular temporary", "#hpd", "", "2", "#lina #sterky" },
                    new List<string>          { "regular temporary", "", "#hpd", "1", "#lina" },
                }
            };
        };

        public Table table__154_to_158()
        {
            return new Table {
                Label = "Assignment status, before, and after",
                Header = new List<string>     { "Location", "Status", "Before", "After", "Num", "Found" },
                Data = new List<List<string>> {
                    new List<string>          { "#hpd", "", "", "", "2", "#lina #sterky" },
                    new List<string>          { "#hpd", "Past", "", "", "1", "#sterky" },
                    new List<string>          { "#hpd", "", "9/1/2008", "", "1", "#lina" },
                    new List<string>          { "#hpd", "", "", "3/1/2013", "2", "#lina #sterky" },
                }
            };
        };

        public Table table__161_to_167()
        {
            return new Table {
                Label = "Assignment date range, location and org",
                Header = new List<string>     { "Between", "And", "Location", "Organization", "Num", "Found" },
                Data = new List<List<string>> {
                    new List<string>          { "9/01/2009", "10/01/2010", "#hpd", "", "1", "#lina" },
                    new List<string>          { "8/18/2012", "03/13/2013", "#hpd", "", "2", "#lina #sterky" },
                    new List<string>          { "5/30/2012", "05/30/2013", "#hpd", "", "2", "#lina #sterky" },
                    new List<string>          { "6/01/2013", "07/01/2013", "#hpd", "", "1", "#lina" },
                    new List<string>          { "6/01/2008", "08/14/2008", "#hpd", "", "0", "" },
                    new List<string>          { "8/08/2012", "03/13/2013", "", "#hpd", "1", "#lina" },
                }
            };
        };

        public Table table__170_to_173()
        {
            return new Table {
                Label = "Assignment date range, location, and undated",
                Header = new List<string>     { "Between", "And", "Location", "Undated", "Num", "Found" },
                Data = new List<List<string>> {
                    new List<string>          { "1/01/2013", "03/14/2013", "#oa", "Excluded", "1", "#mimi" },
                    new List<string>          { "1/01/2013", "03/14/2013", "#oa", "Only", "1", "#sterky" },
                    new List<string>          { "8/01/2008", "05/15/2013", "#oa #hpd", "", "3", "#lina #sterky #mimi" },
                }
            };
        };

        public Table table__176_to_178()
        {
            return new Table {
                Label = "Assignment date range, location/org, and employment type",
                Header = new List<string>     { "Between", "And", "Location", "Organization", "EmpType", "Num", "Found" },
                Data = new List<List<string>> {
                    new List<string>          { "1/01/2013", "03/14/2013", "", "", "Reserve", "2", "#lina #mimi" },
                    new List<string>          { "1/01/2013", "03/14/2013", "#hpd #opd", "#hpd #opd", "", "2", "#lina #sterky" },
                }
            };
        };


        public ScenarioContext.UserCreation UserCreation;
        public ScenarioContext.Implementations.GlobalSettingManagement GlobalSettingManagement;
        public ScenarioContext.Implementations.Administration.ListManagement ListManagement;
        public ScenarioContext.Implementations.Organizations.OrganizationFNHManagement OrganizationFNHManagement;
        public ScenarioContext.People.PersonEmploymentFNHManagement PersonEmploymentFNHManagement;
        public ScenarioContext.PresenterImplementations.People.PersonSearchCriteriaWorkflow PersonSearchCriteria;
        public ScenarioContext.PresenterImplementations.People.PersonSearchWorkflow PersonSearch;
        public ScenarioContext.ViewImplementations.People.EmploymentActionEdit EmploymentActionEdit;

        public LargeExample_generated()
            : base()
        {
            UserCreation = new ScenarioContext.UserCreation();
            GlobalSettingManagement = new ScenarioContext.Implementations.GlobalSettingManagement();
            ListManagement = new ScenarioContext.Implementations.Administration.ListManagement();
            OrganizationFNHManagement = new ScenarioContext.Implementations.Organizations.OrganizationFNHManagement();
            PersonEmploymentFNHManagement = new ScenarioContext.People.PersonEmploymentFNHManagement();
            PersonSearchCriteria = new ScenarioContext.PresenterImplementations.People.PersonSearchCriteriaWorkflow();
            PersonSearch = new ScenarioContext.PresenterImplementations.People.PersonSearchWorkflow();
            EmploymentActionEdit = new ScenarioContext.ViewImplementations.People.EmploymentActionEdit();
        }
    }
}
