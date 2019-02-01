using System;
using System.Collections.Generic;
using LibraryHoldingTestWorkflows;
using NUnit.Framework;

namespace ScenarioContext.Implementations
{

    public class GlobalSettingManagement : Workflow
    {
        public void Enable_Duty_Locations()
        {
        }

    }
}

namespace ScenarioContext
{
    public class UserCreation : Workflow
    {
        public void Create_Person_named__(string name)
        {
        }
    }

}

namespace ScenarioContext.Implementations.Organizations
{
    public class OrganizationFNHManagement : Workflow
    {
        public void Create_organization_named__of_type__under_base_parent_group(string name, string orgType)
        {

        }
    }
}

namespace ScenarioContext.Implementations.Administration
{
    public class ListManagement : Workflow
    {
        public void Create__list_item__with_description__with_usage__(string itemType, string name, string description,
            string usage)
        {
        }

        //public void Create__list_item__with_description__with_usage__(string itemType, string name, string description,
        //    string usage)
        //{
        //}

        public void A_step_in_ListManagement()
        {
            Assert.That(true);
        }
    }
}

namespace ScenarioContext.ViewImplementations.People
{
    public class EmploymentActionEdit : Workflow
    {
        public void Browse_to_add_person_employment_for__(string name)
        {

        }

        public void Add_primary_employment_at__with_title__starting_on__(string orgName, string title,
            DateTime startDate)
        {

        }

        public void Add_primary_employment_at__with_title__employment_type__and_appointment_type__starting_on__(
            string org, string title, string emplType, string apptType, DateTime startDate)
        {
        }

    }
}

namespace ScenarioContext.People
{
    public enum AssignmentStatus
    {
        Current,
        Past
    }

    public class PersonEmploymentFNHManagement : Workflow
    {
        public void Add_employment_comment__for__at__(string comment, string empName, string orgName)
        {
        }

        public void Using_employment_of__at__(string name, string org)
        {
        }

        public void Add_duty_assignment_at__starting__with_status__(string orgName, DateTime startDate, AssignmentStatus status)
        {
        }

        public void Add_temporary_duty_assignment_at__from__to__with_status__(string orgName, DateTime startDate,
            DateTime endDate, string status)
        {
        }
    }
}

namespace ScenarioContext.PresenterImplementations.People
{
    public class PersonSearchWorkflow : Workflow
    {
        public void Search_with_criteria()
        {
        }

        public void Verify_that_there_are__results(int num)
        {
        }

        public void Verify_that_results_are__(List<string> nameList)
        {
        }
    }

    public class PersonSearchCriteriaWorkflow : Workflow
    {
        public void Browse_to_page()
        {
        }

        public void Click_Switch_to_advanced_mode()
        {
        }

        public void Add_new_employment_criterion_to_expression()
        {
        }

        public void With__duty_assignments(string assignmentType)
        {
        }

        public void With_employment_criteria_effective_on_a_specific_date__(DateTime effectivedate)
        {
        }

        public void With_employment_effective_date(DateTime effectiveDate)
        {
        }

        public void With_employment_types(string employmentType)
        {
        }

        public void With_organizations(string orgName)
        {
        }

        public void Click_done_in_employment_criteria_popover()
        {
        }

        public void Click_Search()
        {
        }

        public void Verify_that_page_navigated_to_search_results()
        {
        }

        public void With_duty_locations__(List<string> orgNames)
        {
        }

        public void With_duty_assignment_statuses(List<string> statusNames)
        {
        }

        private void SetDutyAssignmentDates(DateTime? startDate, DateTime? endDate)
        {
        }

        public void With_duty_assignment_dates_between__and__(DateTime startDate, DateTime endDate)
        {
        }

        public void With_duty_assignment_dates_before__(DateTime endDate)
        {
        }

        public void With_duty_assignment_dates_after__(DateTime startDate)
        {
        }

        public void With_undated_assignments_excluded()
        {
        }

        public void With_undated_assignments_only()
        {
        }

        public void With_undated_assignments(string modifier)
        {
            if (modifier == "Excluded")
                With_undated_assignments_excluded();
            else if (modifier == "Only")
                With_undated_assignments_only();
        }
    }
}
