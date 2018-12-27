using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tabula.CST;

namespace Tabula
{
    [TestFixture]
    public class GeneratorTests
    {
        StringBuilder builder;
        Scenario scenario;
        Generator generator;

        [SetUp]
        public void SetUp()
        {
            builder = new StringBuilder();
            scenario = new Scenario();
            generator = new Generator {Builder = builder, Scenario = scenario, InputFilePath = "scenario_source.tab"};
        }

        //FUTURE:  Report scenario errors in Visual Studio error list
        //FUTURE:  Levenshtein suggestions for unfound workflows and steps

        [TestCase("squangle_widgets_across_timezones.tab")]
        [TestCase("groplet_quality_within_tolerance.tab")]
        public void Header_warns_it_is_generated_and_tells_where_source_file_is(string fileName)
        {
            generator.InputFilePath = fileName;
            generator.WriteHeader();

            var result = builder.ToString();
            var lines = Regex.Split(result, Environment.NewLine);

            Assert.That(lines.Count(), Is.GreaterThan(2));
            Assert.That(lines[0], Does.Contain("generated"));
            Assert.That(lines[0], Does.Contain("TabulaClassGenerator"));
            Assert.That(lines[1], Does.Contain(fileName));
        }

        [TestCase("my_scenario.tab", "my_scenario_generated")]
        [TestCase("c:\\proj\\scenarios\\my.tab", "my_generated")]
        [TestCase("scenarios\\my_tests.v4.tab", "my_tests_v4_generated")]
        public void scenario_class_name_matches_file_name(string fileName, string expectedClassName)
        {
            generator.InputFilePath = fileName;

            var className = generator.GetGeneratedClassName();

            Assert.That(className, Is.EqualTo(expectedClassName));
        }

        [Test]
        public void Class_declaration_includes_base_and_interface()
        {
            generator.WriteClassOpen();

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($": GeneratedScenarioBase, IGeneratedScenario"));
        }

        [Test]
        public void Class_declaration_includes_scenario_label_as_comment()
        {
            var label = "AC-39393: Snock the cubid foratically";
            var scenario = new Scenario {Label = label};
            generator.Scenario = scenario;
            generator.InputFilePath = "TuitionBilling";
            generator.WriteClassOpen();

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"  //  \"{label}\""));
        }

        [TestCase("ScenarioContext.Implementations.Administration.TaskRunnerWorkflow", "TaskRunner")]
        [TestCase("ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow", "AddEnrollment")]
        public void instance_name_known_from_workflow(string workflowName, string expectedInstanceName)
        {
            string instanceName = Formatter.InstanceName_from_TypeName(workflowName);

            Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        }

        [Test]
        public void BuildStep_Unfound_includes_source_text_and_location()
        {
            var step = new Step(12,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );

            generator.CurrentParagraph = new Paragraph();
            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("@\"hello world\""));
            Assert.That(result, Contains.Substring("\"scenario_source.tab:12\""));
        }


        [TestCase("Do call with lambda", "Do(() =>")]
        [TestCase("source location", "\"scenario_source.tab:34\"")]
        [TestCase("workflow instance and method name", "myWorkflow.MyMethod_correctly_spelled()")]
        public void Step_Call_includes_(string description, string part)
        {
            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "MyMethod_correctly_spelled"});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(34,
                (TokenType.Word, "my"),
                (TokenType.Word, "method"),
                (TokenType.Word, "correctly"),
                (TokenType.Word, "spelled")
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring(part), description);
        }

        [Test]
        public void Step_Call_includes_arguments()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friend__turned__on__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "turned"),
                (TokenType.Number, "28"),
                (TokenType.Word, "on"),
                (TokenType.Date, "07/15/2017")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            //  string arguments are quoted, ints aren't, dates go through conversion
            var expectedArguments = @"(""Bob"", 28, ""07/15/2017"".To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));

            //  to aid test developers, the Do() call includes a string copy of the args
            var requotedArguments = expectedArguments.Replace("\"", "\"\"");
            Assert.That(result, Contains.Substring(requotedArguments));
        }

        [Test]
        public void CommandSet_generates_proper_string_for_source_location()
        {
            var term = new Symbol(TokenType.String, "bar", 1);
            var command = new CommandSet("foo", term) {StartLine = 12};

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring(@"@""scenario_source.tab:12"""));
        }

        [Test]
        public void BuildStep_sets_a_variable_on_CommandSet()
        {
            var term = new Symbol(TokenType.String, "bar", 1);
            var command = new CommandSet("foo", term) {StartLine = 12};

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Var[\"foo\"] = \"bar\","));
        }

        [Test]
        public void BuildStep_can_set_a_variable_to_a_variable_on_CommandSet()
        {
            var term = new Symbol(TokenType.Variable, "bar", 1);
            var command = new CommandSet("foo", term) {StartLine = 12};

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Var[\"foo\"] = Var[\"bar\"],"));
        }

        [Test]
        public void BuildStep_can_set_a_referenced_value_as_a_variable_to_a_variable_on_CommandSet()
        {
            var term = new Symbol(TokenType.Variable, "bar", 1);
            var command = new CommandSet("#foo", term) {StartLine = 12};

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Var[Var[\"foo\"]] = Var[\"bar\"],"));
        }

        [Test]
        public void BuildStep_can_set_a_variable_to_a_date_on_CommandSet()
        {
            var term = new Symbol(TokenType.Date, "1/1/2323", 1);
            var command = new CommandSet("foo", term) {StartLine = 12};

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Var[\"foo\"] = \"1/1/2323\","));
        }

        [Test]
        public void BuildStep_Unfound_on_arg_count_mismatch()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "comment", Type = typeof(string)}
            };


            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "user__made_comment__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment")
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Unfound"));
            Assert.That(result, Contains.Substring("@\"user \"\"Bob\"\" made comment\""));
        }


        [Test]
        public void BuildStep_complaint_when_TokenType_unrecognized()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "user__thing", Args = args});
            var paragraph = new Paragraph();
            generator.CurrentParagraph = paragraph;
            paragraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "user"),
                ((TokenType) (-1), "mystery"),
                (TokenType.Word, "thing")
            );

            var ex = Assert.Throws<Exception>(() => generator.BuildStep(step));
            Assert.That(ex.Message, Contains.Substring("token type [-1]"));
        }


        [Test]
        public void Step_Call_handles_variables()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friend__turned__on__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "turned"),
                (TokenType.Number, "21"),
                (TokenType.Word, "on"),
                (TokenType.Date, "1/1/2001")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            var expectedArguments = @"(Var[""friendname""], 21, ""1/1/2001"".To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        [Test]
        public void Step_Call_interpolates_variables_in_strings()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "greeting", Type = typeof(string)},
                new ArgDetail {Name = "sequence", Type = typeof(string)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "To__I_say__and__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "to"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "I"),
                (TokenType.Word, "say"),
                (TokenType.String, "Hi, #friendname!"),
                (TokenType.Word, "and"),
                (TokenType.String, "#one, #two, #three")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.To__I_say__and__"));

            var singleInterpolation = @", $""Hi, {Var[""friendname""]}!"", ";
            var tripleInterpolation = @", $""{Var[""one""]}, {Var[""two""]}, {Var[""three""]}"")";
            Assert.That(result, Contains.Substring(singleInterpolation));
            Assert.That(result, Contains.Substring(tripleInterpolation));
        }

        [Test]
        public void Step_Call_variables_get_standard_types()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friend__turned__on__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "turned"),
                (TokenType.Variable, "newage"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            //  variables going into an argument of type int need to get a .To<int>()
            //  variables going into an argument of type DateTime need to get a .To<DateTime>()
            var expectedArguments =
                @"(Var[""friendname""], Var[""newage""].To<Int32>(), Var[""birthday""].To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        [Test]
        public void Step_Calls_can_pass_multiple_collections()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "names", Type = typeof(List<string>)},
                new ArgDetail {Name = "greets", Type = typeof(List<string>)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friends__say__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friends")
            );
            var friends = new SymbolCollection();
            friends.Values.Add(new Symbol(TokenType.String, "Ann"));
            friends.Values.Add(new Symbol(TokenType.String, "Bob"));

            var ages = new SymbolCollection();
            ages.Values.Add(new Symbol(TokenType.String, "hello"));
            ages.Values.Add(new Symbol(TokenType.String, "howdy"));

            step.Symbols.Add(friends);
            step.Symbols.Add(new Symbol(TokenType.Word, "say"));
            step.Symbols.Add(ages);

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friends__say__"));

            var expectedNamesConstruction = @"var arg_222_0 = new List<String> { ""Ann"", ""Bob"" };";
            Assert.That(result, Contains.Substring(expectedNamesConstruction));

            var expectedAgesConstruction = @"var arg_222_1 = new List<String> { ""hello"", ""howdy"" };";
            Assert.That(result, Contains.Substring(expectedAgesConstruction));

            var expectedArguments = @"(arg_222_0, arg_222_1)";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        [Test]
        public void Step_Call_can_send_list_of_int()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "numbers", Type = typeof(List<int>)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "Lucky_numbers__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "lucky"),
                (TokenType.Word, "numbers")
            );

            var nums = new SymbolCollection();
            nums.Values.Add(new Symbol(TokenType.Number, "33"));
            nums.Values.Add(new Symbol(TokenType.Number, "31"));

            step.Symbols.Add(nums);

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.Lucky_numbers__"));

            var expectedAgesConstruction = @"var arg_222_0 = new List<Int32> { 33, 31 };";
            Assert.That(result, Contains.Substring(expectedAgesConstruction));

            var expectedArguments = @"(arg_222_0)";
            Assert.That(result, Contains.Substring(expectedArguments));
        }


        // generation time:
        //decl for List<int>:
        // var arg_387_0 = new List<int> { 13, Var["offset"].To<int>() };

        //Prepare_input_for_param(sym, paramType, step.StartLine, argIndex);
        [Test]
        public void Convert_Step_Arg_To_Param_Type_can_create_collection_of_int()
        {
            var argSymbol = new SymbolCollection();
            argSymbol.Values.Add(new Symbol(TokenType.Number, "10"));
            argSymbol.Values.Add(new Symbol(TokenType.String, "11"));
            argSymbol.Values.Add(new Symbol(TokenType.Variable, "dozen"));
            var paramType = typeof(List<int>);
            var startLine = 125;
            var argIndex = 3;

            (string text, string declaration) =
                generator.Prepare_input_for_param(argSymbol, paramType, startLine, argIndex);

            Assert.That(declaration,
                Contains.Substring(@"var arg_125_3 = new List<Int32> { 10, 11, Var[""dozen""].To<Int32>() }"));
        }


        [Test]
        public void Step_Call_constant_int_arguments_are_inlined()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friend__turned__on__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "turned"),
                (TokenType.String, "23"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            var expectedArguments = @"(Var[""friendname""], 23, Var[""birthday""].To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }



        [Test]
        public void Step_Call_constant_numeric_arguments_get_m_when_going_to_decimal_parameter()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "percentage", Type = typeof(decimal)}
            };

            var detail = new WorkflowDetail {Name = "SomeWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "I_feel__percent_awake", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "I"),
                (TokenType.Word, "feel"),
                (TokenType.Number, "23.02"),
                (TokenType.Word, "percent"),
                (TokenType.Word, "AWAKE")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.I_feel__percent_awake"));

            var expectedArguments = @"(23.02m)";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        enum Assignment
        {
            HR,
            Ops
        }


        [Test]
        public void Step_Call_variables_get_enum_types()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "assignment", Type = typeof(Assignment)}
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friend__now_works_at__", Args = args});

            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "now"),
                (TokenType.Word, "works"),
                (TokenType.Word, "at"),
                (TokenType.String, "HR")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__now_works_at__"));

            //  variables going into an argument of an enum type get a .To<enumType>()
            var expectedArguments = @"(Var[""friendname""], ""HR"".To<Tabula.Assignment>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        //TODO:  Args which are list or array types

        [TestCase(143)]
        [TestCase(202)]
        public void List_arguments_create_declaration_line(int lineNumber)
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "names", Type = typeof(List<string>)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friends_are__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(lineNumber,
                (TokenType.Word, "my"),
                (TokenType.Word, "friends"),
                (TokenType.Word, "are")
            );

            var list = new SymbolCollection();
            list.Values.Add(new Symbol(TokenType.String, "Left"));
            list.Values.Add(new Symbol(TokenType.Variable, "Right"));
            step.Symbols.Add(list);

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            var expected = $"var arg_{lineNumber}_0 = new List<String> {{ \"Left\", Var[\"Right\"] }};";
            Assert.That(result, Contains.Substring(expected));
            Assert.That(result, Contains.Substring($"myWorkflow.My_friends_are__(arg_{lineNumber}_0)"));
        }

        [Test]
        public void Single_values_going_to_a_list_parameter_get_wrapped_in_lists()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "names", Type = typeof(List<string>)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My_friends_are__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(202,
                (TokenType.Word, "my"),
                (TokenType.Word, "friends"),
                (TokenType.Word, "are"),
                (TokenType.String, "Frank")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            var expected = $"var arg_202_0 = new List<string> {{ \"Frank\" }};";
            Assert.That(result, Contains.Substring(expected));
            Assert.That(result, Contains.Substring($"myWorkflow.My_friends_are__(arg_202_0)"));
        }

        // undecided which of these should pass:
        //[TestCase(TokenType.Number, typeof(int), "12.5", "(12.5).To<Int32>()")]
        //[TestCase(TokenType.Number, typeof(int), "12.5", "12")]
        //[TestCase(TokenType.Number, typeof(int), "12.5", "13")]
        //[TestCase(TokenType.Number, typeof(int), "12.5", "12") ==> exception]

        [TestCase(TokenType.Number, typeof(int), "12", "12")]
        [TestCase(TokenType.Number, typeof(float), "12", "12")]
        [TestCase(TokenType.Number, typeof(double), "12", "12")]
        [TestCase(TokenType.Number, typeof(decimal), "12", "12m")]
        [TestCase(TokenType.Number, typeof(string), "12", "\"12\"")]
        [TestCase(TokenType.String, typeof(int), "12", "12")]
        [TestCase(TokenType.String, typeof(float), "12", "12")]
        [TestCase(TokenType.String, typeof(double), "12", "12")]
        [TestCase(TokenType.String, typeof(decimal), "12", "12m")]
        [TestCase(TokenType.String, typeof(string), "12", "\"12\"")]
        [TestCase(TokenType.Variable, typeof(string), "dept", "Var[\"dept\"]")]
        [TestCase(TokenType.String, typeof(DateTime), "01/04/2013", "\"01/04/2013\".To<DateTime>()")]
        [TestCase(TokenType.Date, typeof(DateTime), "01/04/2013", "\"01/04/2013\".To<DateTime>()")]
        public void TokenToType(TokenType tokenType, Type collectedType, string text, string expected)
        {
            var result = generator.TokenToType(tokenType, collectedType, text);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(TokenType.String, typeof(int), "12#extra", "$\"12{Var[\"extra\"]}\".To<Int32>()")]
        [TestCase(TokenType.String, typeof(float), "-0.#extra", "$\"-0.{Var[\"extra\"]}\".To<Single>()")]
        [TestCase(TokenType.String, typeof(double), "cannot #extra", "$\"cannot {Var[\"extra\"]}\".To<Double>()")]
        [TestCase(TokenType.String, typeof(decimal), "runtime #extra", "$\"runtime {Var[\"extra\"]}\".To<Decimal>()")]
        public void TokenToType_strings_containing_variables_need_runtime_cast(TokenType tokenType, Type collectedType,
            string text, string expected)
        {
            var result = generator.TokenToType(tokenType, collectedType, text);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(TokenType.String, typeof(int), "c2c")]
        [TestCase(TokenType.String, typeof(float), "c2c")]
        [TestCase(TokenType.String, typeof(double), "c2c")]
        [TestCase(TokenType.String, typeof(decimal), "c2c")]
        [TestCase(TokenType.String, typeof(DateTime), "Fourth of July, 1776")]
        [TestCase(TokenType.Date, typeof(DateTime), "91/04/2013")]
        [TestCase(TokenType.Date, typeof(int), "01/04/2013")]
        [TestCase(TokenType.Comma, typeof(string), ",")]
        [TestCase(TokenType.Number, typeof(DateTime), "3")]
        public void TokenToType_fails_to_convert(TokenType tokenType, Type collectedType, string text)
        {
            var ex = Assert.Throws<Exception>(() => generator.TokenToType(tokenType, collectedType, text));
            Assert.That(ex.Message, Does.StartWith("Cannot convert"));
        }

        [Test]
        public void List_arguments_are_indexed_by_arg_position()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "group", Type = typeof(string)},
                new ArgDetail {Name = "names", Type = typeof(List<string>)},
            };

            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "My__are__", Args = args});
            generator.CurrentParagraph = new Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            var step = new Step(321,
                (TokenType.Word, "my"),
                (TokenType.String, "friends"),
                (TokenType.Word, "are")
            );

            var list = new SymbolCollection();
            list.Values.Add(new Symbol(TokenType.String, "Left"));
            step.Symbols.Add(list);

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring($"myWorkflow.My__are__(\"friends\", arg_321_1)"));
        }

        //TODO:  Scoping of workflows in aliases and blocks

        class UnknownAction : CST.Action
        {
        }

        [Test]
        public void DispatchAction_explains_when_an_unknown_action_type_arrives()
        {
            var ex = Assert.Throws<NotImplementedException>(() => generator.PrepareAction(new UnknownAction()));
            Assert.That(ex.Message, Is.EqualTo(
                "Extend Tabula to prepare action type [Tabula.GeneratorTests+UnknownAction]."));
        }

        [Test]
        public void BuildParagraph_gets_all_the_bits_together()
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "comment", Type = typeof(string)}
            };

            var step = new Step(22,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment"),
                (TokenType.String, "where am I?")
            );

            var paragraph = new Paragraph
            {
                Label = "short paragraph",
                MethodName = "paragraph__021_to_022"
            };
            paragraph.Actions.Add(step);
            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow", Namespace = "App"};
            detail.AddMethod(new MethodDetail {Name = "user__made_comment__", Args = args});
            paragraph.WorkflowsInScope.Add(detail);

            generator.PrepareParagraph(paragraph);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Label(     \"short paragraph\");"));
            Assert.That(result, Contains.Substring("public void paragraph__021_to_022()"));
            Assert.That(result, Contains.Substring("myWorkflow = new App.GreetingWorkflow();"));
            Assert.That(result, Contains.Substring("myWorkflow.user__made_comment__(\"Bob\", \"where am I?\")"));
        }

        [Test]
        public void BuildParagraph_will_reinstantiate_all_workflows_in_scope()
        {
            var paragraph = new Paragraph
            {
                Label = "short paragraph",
                MethodName = "paragraph__021_to_022"
            };

            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "comment", Type = typeof(string)}
            };
            var detail = new WorkflowDetail {Name = "GreetingWorkflow", InstanceName = "myWorkflow", Namespace = "App"};
            detail.AddMethod(new MethodDetail {Name = "user__made_comment__", Args = args});
            paragraph.WorkflowsInScope.Add(detail);

            detail = new WorkflowDetail {Name = "ExtraWorkflow", InstanceName = "otherWorkflow", Namespace = "App"};
            paragraph.WorkflowsInScope.Add(detail);
            detail = new WorkflowDetail
                {Name = "ThirdWorkflow", InstanceName = "thirdWorkflow", Namespace = "App.FancyNamespace"};
            paragraph.WorkflowsInScope.Add(detail);

            var step = new Step(22,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment"),
                (TokenType.String, "where am I?")
            );

            paragraph.Actions.Add(step);

            generator.PrepareParagraph(paragraph);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("myWorkflow = new App.GreetingWorkflow();"));
            Assert.That(result, Contains.Substring("otherWorkflow = new App.ExtraWorkflow();"));
            Assert.That(result, Contains.Substring("thirdWorkflow = new App.FancyNamespace.ThirdWorkflow();"));
        }

        [TestCase("method name", "public Table table__030_to_035()")]
        [TestCase("tags", "Tags = new List<string> { \"Crucial\", \"AC-22222\" }")]
        [TestCase("label", "Label = \"This is my label\",")]
        [TestCase("header", "Header = new List<string>     { \"First\", \"Second\" },")]
        [TestCase("row one", "\"Blood\", \"Guessing\"")]
        [TestCase("var in cell", "\"Impressions\", \"#level\"")]
        public void BuildTable_gets_all_the_bits_together(string part, string substring)
        {
            var row1 = new TableRow("Blood", "Guessing");
            var row2 = new TableRow("Step", "Chance");
            var row3 = new TableRow("Impressions", "#level");

            var table = new Table
            {
                MethodName = "table__030_to_035",
                Tags = new List<string> {"Crucial", "AC-22222"},
                Label = "This is my label",
                ColumnNames = new List<string> {"First", "Second"},
                Rows = new List<TableRow> {row1, row2, row3}
            };

            generator.PrepareTable(table);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring(substring), $"generated table is missing {part}.");
        }

        [TestCase("empty tags", "Tags =")]
        [TestCase("empty Label", "Label =")]
        public void BuildTable_skips_label_and_tags_if_empty(string part, string substring)
        {

            var row1 = new TableRow("Blood", "Guessing");
            var row2 = new TableRow("Time", "Chance");
            var row3 = new TableRow("Impressions", "Thoughts");

            var table = new Table
            {
                MethodName = "table__030_to_035",
                ColumnNames = new List<string> {"First", "Second"},
                Rows = new List<TableRow> {row1, row2, row3}
            };

            generator.PrepareTable(table);
            var result = generator.sectionsBody.ToString();

            Assert.That(result.IndexOf(substring) == -1, $"generated table should not contain {part}.");
        }

        [Test]
        public void BuildParagraph_skips_tags_if_empty()
        {

            var row1 = new TableRow("Blood", "Guessing");
            var row2 = new TableRow("Time", "Chance");
            var row3 = new TableRow("Impressions", "Thoughts");

            var paragraph = new Paragraph
            {
                Label = "How to Rebombulate",
                MethodName = "paragraph__030_to_035",
            };

            generator.PrepareParagraph(paragraph);
            var result = generator.sectionsBody.ToString();

            Assert.That(result.IndexOf("Tags("), Is.LessThan(0),
                $"generated paragraph should not contain empty Tags() call.");
        }

        [Test]
        public void Row_ToCodeText_returns_text_for_List_of_string()
        {
            var cells = new List<List<string>>();
            var cell = new List<string> {"Fluffy"};
            cells.Add(cell);
            var row = new TableRow(cells);

            var result = generator.CodeTextFrom(row);

            Assert.That(result, Contains.Substring("new List<string>"));
            Assert.That(result, Contains.Substring("{ \"Fluffy\" }"));
        }

        [Test]
        public void WriteNamespaceOpen()
        {
            generator.WriteNamespaceOpen();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("namespace Tabula"));
        }

        [Test]
        public void WriteConstructor_puts_constructor_method_into_main_StringBuilder()
        {
            generator.GeneratedClassName = "TryAnything_generated";
            generator.WriteConstructor();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("public TryAnything_generated()"));
        }

        [Test]
        public void WriteSectionMethods_puts_built_sections_into_main_StringBuilder()
        {
            generator.sectionsBody.AppendLine("all my sections");

            generator.WriteSectionMethods();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("all my sections"));
        }

        [Test]
        public void WriteExecuteMethod_put_built_required_interface_methods_into_main_StringBuilder()
        {
            generator.executeMethodBody.AppendLine("my required method");

            generator.WriteExecuteMethod();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("my required method"));
        }

        private class UnrecognizedSection : Section
        {
        }

        [Test]
        public void Unrecognized_Section_type_complaint()
        {
            scenario.Sections.Add(new UnrecognizedSection { });
            var ex = Assert.Throws<Exception>(() => generator.PrepareSections());
            Assert.That(ex.Message, Contains.Substring("Extend Tabula"));
            Assert.That(ex.Message, Contains.Substring("UnrecognizedSection"));
        }

        [Test]
        public void Single_paragraph_scenario_gets_one_paragraph_call()
        {
            scenario.Sections.Add(new Paragraph {MethodName = "paragraph_one"});

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("paragraph_one();"));
        }

        [Test]
        public void two_paragraph_scenario_gets_both_calls()
        {
            scenario.Sections.Add(new Paragraph {MethodName = "paragraph_one"});
            scenario.Sections.Add(new Paragraph {MethodName = "paragraph_two"});

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("paragraph_one();"));
            Assert.That(built, Contains.Substring("paragraph_two();"));
        }

        [Test]
        public void Single_paragraph_followed_by_table_gets_run_x_over_y_call()
        {
            scenario.Sections.Add(new Paragraph {MethodName = "paragraph_one"});
            scenario.Sections.Add(new Table {MethodName = "table_one"});

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("Foreach_Row_in( table_one, paragraph_one );"));
        }

        [Test]
        public void Single_paragraph_followed_by_two_tables_gets_two_calls()
        {
            scenario.Sections.Add(new Paragraph {MethodName = "paragraph_one"});
            scenario.Sections.Add(new Table {MethodName = "table_one"});
            scenario.Sections.Add(new Table {MethodName = "table_two"});

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("Foreach_Row_in( table_one, paragraph_one );"));
            Assert.That(built, Contains.Substring("Foreach_Row_in( table_two, paragraph_one );"));
        }
    }
}
