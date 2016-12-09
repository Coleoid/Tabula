use v6;
use Tabula::CSharp-Writer;
use Tabula::Match-Helper;

#  Composes a Tabula scenario into a C# class
class Code-Scribe
    does Match-Helper
    does CSharp-Writer {
    has $.scenario-title is rw;
    has $.file-name is rw;

    ##################################################################
    #  Sections (tables and paragraphs) are added by declare-section()
    has Str @.section-declarations;
    has Str $.section-declaration-text;
    method declare-section( $section ) {
        @!section-declarations.push( $section );
    }

    has %.seen-fixtures;
    has Str $.fixture-declaration-text;
    has Str $.fixture-instantiation-text;
    method initialize-fixture($fixture) {
        if not %!seen-fixtures{$fixture.key} {
            %!seen-fixtures{$fixture.key} = True;
            $!fixture-declaration-text ~= "        public " ~ $fixture.class-name ~ " "
                ~ $fixture.instance-name ~ ";\n";
            $!fixture-instantiation-text ~= "            " ~
                $fixture.instance-name ~ " = new " ~ $fixture.class-name ~ "();\n";
        }
    }


    ##################################################################
    #  Our ExecuteScenario text, built by add-next-section()
    has Str $.execute-body-text;

    #  Tabula scenario flow:
    #  A paragraph may run directly, or per-row of tables below it,
    # so we stage each paragraph until we know what comes after it.
    has Str $.staged-paragraph;
    has Bool $.paragraph-pending;

    method add-next-section($name) {
        if $name ~~ /^paragraph/ { self.stage-paragraph($name); }
        elsif $name ~~ /^table/  { self.add-table-call($name); }
        elsif $name eq ''        { self.flush-pending-paragraph; }
        else { die "I do not know how to add a section named [$name]." }
    }

    method stage-paragraph($name) {
        self.flush-pending-paragraph();
        $!staged-paragraph = $name;
        $!paragraph-pending = True;
    }

    method flush-pending-paragraph() {
        self.add-paragraph-call($!staged-paragraph) if $!paragraph-pending;
        $!paragraph-pending = False;
    }

    method add-paragraph-call($paragraph-name) {
        $!execute-body-text ~= '            ' ~ $paragraph-name ~ "();\n";
    }

    method add-table-call($table-name) {
        $!execute-body-text ~= '            Run_para_over_table( ' ~ $!staged-paragraph ~ ', ' ~ $table-name ~ " );\n";
        $!paragraph-pending = False;
    }


    ##################################################################
    #  Composing code for paragraphs and tables of the scenario class,
    # and adding them to the ExecuteScenario() plan.

    method compose-paragraph($/) {
        my $name = self.name-section('paragraph', $/);
        my $statements = [~] $<Statement>.map({ .made ?? ("            " ~ .made) !! "" });

        my $para = "        public void " ~ $name ~ "()\n        \{\n"
            ~ $statements
            ~ "        \}\n";

        self.declare-section($para);
        self.add-next-section($name);
        return $para;
    }

    #TODO: Table Labels
    method compose-table($/) {
        my $name = self.name-section('table', $/);

        #TODO: Row hashes
        my $table = '        public Table ' ~ $name ~ '()
        {
            return new Table {
                Header = new List<string>     ' ~ $<Table-Header>.ast.chomp ~ ','
            ~ '
                Data = new List<List<string>> {';

        for $<Table-Row> -> $row {
            $table ~= '
                    new List<string>          ' ~ $row.ast.chomp ~ ',';
        }

        $table ~= '
                }
            };
        }
';

        self.declare-section($table);
        self.add-next-section($name);
        return $table;
    }


    ##################################################################
    #  Composing portions of the scenario class file

    method compose-constructor() {
'        public ' ~ self.class-name ~ '(TabulaStepRunner runner)
            : base(runner)
        {
'       ~ $!fixture-instantiation-text ~ '        }
';
    }

    #  ExecuteScenario() runs the scenario as it was sequenced, at the
    # paragraph granularity.
    method compose-method-ExecuteScenario() {
        self.flush-pending-paragraph();
'        public void ExecuteScenario()
        {
' ~ $!execute-body-text ~ '        }
';
    }

    # Paragraph and Table method declarations
    method compose-section-declarations() {
        $!section-declaration-text = @!section-declarations.join("\n");
    }

    #  Returns the full source file--includes, namespace, class.
    method compose-file() {
        my $full-source-file =
            self.get-class-header() ~
            $!fixture-declaration-text ~ "\n" ~
            self.compose-constructor() ~ "\n" ~
            self.compose-method-ExecuteScenario() ~ "\n" ~
            self.compose-section-declarations() ~
            self.get-class-footer();

        return $full-source-file;
    }


    ##################################################################
    #  Initialization

    method clear-work() {
        $!staged-paragraph = '';
        $!paragraph-pending = False;

        %!seen-fixtures = Empty;
        $!fixture-declaration-text = '';
        $!fixture-instantiation-text = '';

        $!execute-body-text = '';

        @!section-declarations = Empty;
        $!section-declaration-text = '';
    }

    submethod BUILD {
        self.clear-work();
    }
}
