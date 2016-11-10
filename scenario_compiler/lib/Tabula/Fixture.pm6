use v6;

class Method-Descriptor {
    has $.signature;
    has $.name;
    has $.args;
    has $.name-key;
    has $.args-key;

    submethod BUILD(:$!signature is required) {
        $!signature ~~ / $<name> = [ <-[(]>+ ] \( $<args> = [.+] \) /;
        $!name = $<name>;
        $!args = $<args>;

        $!name-key = $!name.lc.subst('_', '');

        $!args ~~ / sdfsdf /;
    }

    #  Keys are flat names suffixed with an encoding of its arg types,
    # for example, "(sdi)" for (string, date, int), both easy to parse
    # and good for approximate matching.

}


#  Fixture:  A class containing testing methods to be called by the
# class which implements a Tabula scenario.
class Fixture {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.canon-name;

    has %.methods;
    method add-method($full-name) {
        my $method = Method-Descriptor.new($full-name);
        %!methods{$method.key} = $method;
    }
}

#   ===================================
class StepLibrary-old {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.flat-name;

    has %.steps;
    has %.args;

    submethod BUILD(:$!class-name is required, :$!instance-name = "") {
        my $short-name = $!class-name.subst('Workflow', '');
        $!flat-name = $short-name.lc;
        if $!instance-name eq "" {
            $!instance-name = $short-name;
        }
    }
}
