#!perl   Updating Testopia scenarios to Tabula
use strict;
use warnings;
use File::Find;

my @target_folders = ("k:\\code\\acadis_trunk\\ScenarioTests\\ScenarioContext\\Scenarios");
sub filter { update_scenario( $_ ) if (m/\.(txt|inc)$/); }
find( \&filter, @target_folders );
exit;

sub update_scenario {
	my $filename = shift;
	$_ = slurp_file( $filename );

	s/\n([^#\n]+): +(".+")/\n$2:\n>use: $1/g;		# Paragraph header
	s/^\[ *([^\]]+?) *\]\nName: +(.+)/Scenario: "$2"\n>tags: ($1)/i;
	s/Name: ([^\n]+)/Scenario: "$1"/i;
	s/^\[ *([^\]]+?) *\]/>tags: ($1)/g;				# Remaining tags
	s/^(\s*)# /$1/mig;								# Steps

	write_file( $_, $filename );
}

sub slurp_file {
	my $filename = shift;
    local $/ = undef;

    open( my $fh, "<", $filename ) or die "Could not open [$filename]:  $!";

    <$fh>;
}

sub write_file {
	my $document = shift;
	my $filename = shift;

	$filename =~ s/(.*)\.(txt|inc)$/$1.tab/;
	open( my $fh, '>', $filename ) or die "Could not open [$filename]:  $!";

	print $fh $document;
	close $fh;
}