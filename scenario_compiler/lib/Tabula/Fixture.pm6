use v6;

#  Fixture:  A class containing testing methods to be called by the
# class which implements a Tabula scenario.
class Fixture {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.canon-name;

    #  Keys are flat names suffixed with an encoding of its arg types,
    # perhaps ":ssis" or "(sdis)" or similar, something easy to parse
    # and good for approximate matching.
    has %.methods;

    method add-method($full-name) {
        my $key-name = self.key-from($full-name);
        %!methods{$key-name} = $full-name;
    }

    method key-from($full-name) {
        $full-name.lc.subst('_', '')
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
