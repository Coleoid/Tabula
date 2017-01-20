using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public class TrainingStub_generated
        : GeneratedScenarioBase, IGeneratedScenario
    {
        public ScenarioContext.UserCreation UserCreation;

        public TrainingStub_generated()
            : base()
        {
            ScenarioLabel = @"TrainingStub.tab:  ""Training Search Link Permissions""";
            UserCreation = new ScenarioContext.UserCreation();
        }

        public void ExecuteScenario()
        {
            paragraph_from_003_to_007();
        }

        public void paragraph_from_003_to_007()
        {
            Label(  "Setup" );
            Do(() =>     UserCreation.Create_Person_named("Jon Doe"),     "TrainingStub.tab:5", @"UserCreation.Create_Person_named(""Jon Doe"")" );
            Do(() =>     UserCreation.Create_Person_named("Tom Doe"),     "TrainingStub.tab:6", @"UserCreation.Create_Person_named(""Tom Doe"")" );
            Do(() =>     UserCreation.Create_Person_named("Don Doe"),     "TrainingStub.tab:7", @"UserCreation.Create_Person_named(""Don Doe"")" );
        }
    }
}
